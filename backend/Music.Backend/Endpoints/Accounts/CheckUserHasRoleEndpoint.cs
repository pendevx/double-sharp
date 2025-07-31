using System.Net;
using Microsoft.EntityFrameworkCore;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.EntityFramework;

namespace Music.Backend.Endpoints.Accounts;

public record CheckUserHasRoleRequest(RoleName Role);

[HttpGet("/accounts/checkUserHasRole/{role}")]
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
        var account = _authContext.GetAccount();

        if (account is null)
        {
            await SendAsync(false, (int)HttpStatusCode.Forbidden, ct);
            return;
        }

        var accountHasRole = _dbContext.AccountRoles
            .Include(a => a.Role)
            .Any(ar => ar.AccountId == account.Id && ar.Role.Name == req.Role.ToString());

        await SendAsync(
            accountHasRole,
            (int)(accountHasRole ? HttpStatusCode.OK : HttpStatusCode.Forbidden),
            ct);
    }
}
