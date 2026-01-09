using Music.Backend.HttpContextExtensions;
using Music.Backend.Models.DTO.Http;
using Music.EntityFramework;

namespace Music.Backend.Endpoints.Accounts;

[RequiresAuthenticated]
public class GetUserInformationEndpoint : Ep.NoReq.Res<UserInformation>
{
    private readonly MusicContext _dbContext;

    public GetUserInformationEndpoint(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/accounts/user");
        AllowAnonymous();
        Options(x => x.AddEndpointFilter<RequiresAuthenticatedFilter>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var token = HttpContext.Request.GetAuthenticationCookie();
        var account = _dbContext.Accounts.FirstOrDefault(a => a.Sessions.Any(s => s.Token == token))!;

        await SendAsync(new UserInformation
        {
            DisplayName = account.DisplayName
        }, cancellation: ct);
    }
}
