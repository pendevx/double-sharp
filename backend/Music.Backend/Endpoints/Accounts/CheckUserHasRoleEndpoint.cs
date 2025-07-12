using System.Net;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.EntityFramework;

namespace Music.Backend.Endpoints.Accounts;

[HttpGet("/accounts/checkUserHasRole/{role}")]
public class CheckUserHasRoleEndpoint : Ep.NoReq.Res<bool>
{
    private readonly MusicContext _dbContext;
    private readonly IAuthContext _authContext;

    public CheckUserHasRoleEndpoint(MusicContext dbContext, IAuthContext authContext)
    {
        _dbContext = dbContext;
        _authContext = authContext;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var role = (string?)HttpContext.Request.RouteValues["role"];

        if (role is null || !Enum.TryParse<RoleName>(role, out var roleName))
        {
            await SendAsync(false, (int)HttpStatusCode.BadRequest, ct);
            return;
        }

        var account = _authContext.GetAccount();

        if (account is null)
        {
            await SendAsync(false, (int)HttpStatusCode.Forbidden, ct);
            return;
        }

        var accountFound = _dbContext.Accounts.Any(acc => acc.Id == account.Id);

        if (!accountFound)
        {
            await SendAsync(false, (int)HttpStatusCode.NotFound, ct);
            return;
        }

        var accountHasRole = _dbContext.AccountRoles
            .Any(ar => ar.AccountId == account.Id && ar.Role.Name == roleName.ToString());

        await SendAsync(
            accountHasRole,
            statusCode: (int)(accountHasRole ? HttpStatusCode.OK : HttpStatusCode.NotFound),
            cancellation: ct);
    }
}
