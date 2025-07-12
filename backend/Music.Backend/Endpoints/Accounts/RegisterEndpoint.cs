using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Music.Backend.HttpContextExtensions;
using Music.Backend.Models.DTO.Http;
using Music.CommandHandlers.Accounts;
using Music.Commands.Accounts;
using Music.QueryHandlers.Accounts;

namespace Music.Backend.Endpoints.Accounts;

public record UserRegistrationInfo(string DisplayName, string Username, string Password);

[HttpPost("/accounts/register")]
public class RegisterEndpoint : Endpoint<UserRegistrationInfo, UserInformation>
{
    private readonly RegisterAccountHandler _registerAccountHandler;
    private readonly LoginHandler _loginHandler;
    private readonly GetAccountByUsernameHandler _getAccountByUsername;

    public RegisterEndpoint(
        RegisterAccountHandler registerAccountHandler,
        LoginHandler loginHandler,
        GetAccountByUsernameHandler getAccountByUsername)
    {
        _registerAccountHandler = registerAccountHandler;
        _loginHandler = loginHandler;
        _getAccountByUsername = getAccountByUsername;
    }

    public override async Task HandleAsync(UserRegistrationInfo req, CancellationToken ct)
    {
        var successfullyRegistered = _registerAccountHandler.Execute(
            new RegisterAccountCommand(req.Username,req.Password,req.DisplayName));

        if (!successfullyRegistered)
        {
            await SendAsync(null!, 409, ct);
            return;
        }

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
