using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Music.Backend.HttpContextExtensions;
using Music.Backend.Models.DTO.Http;
using Music.CommandHandlers.Accounts;
using Music.Commands.Accounts;
using Music.QueryHandlers.Accounts;

namespace Music.Backend.Endpoints.Accounts;

public record UserLoginInfo(string Username, string Password);

[HttpPost("/accounts/login")]
public class LoginEndpoint : Endpoint<UserLoginInfo, UserInformation>
{
    private readonly LoginHandler _loginHandler;
    private readonly GetAccountByUsernameHandler _getAccountByUsername;

    public LoginEndpoint(LoginHandler loginHandler, GetAccountByUsernameHandler getAccountByUsername)
    {
        _loginHandler = loginHandler;
        _getAccountByUsername = getAccountByUsername;
    }

    public override async Task HandleAsync(UserLoginInfo req, CancellationToken ct)
    {
        var token = _loginHandler.Execute(new LoginCommand(req.Username, req.Password));
        var account = _getAccountByUsername.Execute(req.Username)!;

        HttpContext.Response.SetAuthenticationCookie(token);

        await SendAsync(
            new UserInformation
            {
                DisplayName = account.DisplayName
            }, 200, ct);
    }
}
