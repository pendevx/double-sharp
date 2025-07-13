using System.Net;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.EntityFramework;

namespace Music.Backend.Endpoints.Accounts;

public record CheckUserHasRoleRequest(RoleName Role);

[HttpGet("/accounts/checkUserHasRole/{role}")]
[RequiresAuthenticated]
public class CheckUserHasRoleEndpoint : Ep.Req<CheckUserHasRoleRequest>.Res<bool>
{
    private readonly MusicContext _dbContext;
    private readonly IAuthContext _authContext;

    public CheckUserHasRoleEndpoint(MusicContext dbContext, IAuthContext authContext)
    {
        _dbContext = dbContext;
        _authContext = authContext;
    }

    public override async Task HandleAsync(CheckUserHasRoleRequest req, CancellationToken ct)
    {
        var account = _authContext.GetAccount()!;

        var accountHasRole = _dbContext.AccountRoles
            .Any(ar => ar.AccountId == account.Id && ar.Role.Name == req.Role.ToString());

        await SendAsync(
            accountHasRole,
            statusCode: (int)(accountHasRole ? HttpStatusCode.OK : HttpStatusCode.Forbidden),
            cancellation: ct);
    }
}
