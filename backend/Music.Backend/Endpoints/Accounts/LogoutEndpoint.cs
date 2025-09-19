using Music.Backend.HttpContextExtensions;
using Music.CommandHandlers.Accounts;

namespace Music.Backend.Endpoints.Accounts;

public class LogoutEndpoint : Ep.NoReq.NoRes
{
    private readonly LogoutHandler _logoutHandler;

    public LogoutEndpoint(LogoutHandler logoutHandler)
    {
        _logoutHandler = logoutHandler;
    }

    public override void Configure()
    {
        Post("/accounts/logout");
        AllowAnonymous();
        Options(x => x.AddEndpointFilter<RequiresAuthenticatedFilter>());
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        var token = HttpContext.Request.GetAuthenticationCookie();
        _logoutHandler.Execute(token);

        HttpContext.Response.ResetAuthenticationCookie();
        return Task.CompletedTask;
    }
}
