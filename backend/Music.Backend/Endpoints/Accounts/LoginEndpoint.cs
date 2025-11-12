using Music.Backend.HttpContextExtensions;
using Music.Backend.Models.DTO.Http;
using Music.CommandHandlers.Accounts;
using Music.Commands.Accounts;
using Music.EntityFramework;

namespace Music.Backend.Endpoints.Accounts;

public record UserLoginInfo(string Username, string Password);

[HttpPost("/accounts/login")]
public class LoginEndpoint : Endpoint<UserLoginInfo, UserInformation>
{
    private readonly LoginHandler _loginHandler;
    private readonly MusicContext _dbContext;

    public LoginEndpoint(LoginHandler loginHandler, MusicContext dbContext)
    {
        _loginHandler = loginHandler;
        _dbContext = dbContext;
    }

    public override async Task HandleAsync(UserLoginInfo req, CancellationToken ct)
    {
        var token = _loginHandler.Execute(new LoginCommand(req.Username, req.Password));
        var account = _dbContext.Accounts.First(a => a.Username == req.Username);

        // HttpContext.Response.SetAuthenticationCookie(token);

        await SendAsync(
            new UserInformation
            {
                DisplayName = account.DisplayName,
                SessionToken = token,
            }, 200, ct);
    }
}
