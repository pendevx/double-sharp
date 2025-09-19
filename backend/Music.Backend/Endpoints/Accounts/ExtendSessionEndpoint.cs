using Music.Backend.HttpContextExtensions;
using Music.CommandHandlers.Accounts;
using Music.Commands.Accounts;

namespace Music.Backend.Endpoints.Accounts;

public class ExtendSessionEndpoint : Ep.NoReq.NoRes
{
    private readonly ExtendSessionHandler _extendSessionHandler;

    public ExtendSessionEndpoint(ExtendSessionHandler extendSessionHandler)
    {
        _extendSessionHandler = extendSessionHandler;
    }

    public override void Configure()
    {
        Post("/accounts/extendsession");
        AllowAnonymous();
        Options(x => x.AddEndpointFilter<RequiresAuthenticatedFilter>());
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        var token = HttpContext.Request.GetAuthenticationCookie();

        _extendSessionHandler.Execute(
            new ExtendSessionCommand(token, (int)TimeSpan.FromDays(7).TotalSeconds));

        HttpContext.Response.SetAuthenticationCookie(token);
        return Task.CompletedTask;
    }
}
