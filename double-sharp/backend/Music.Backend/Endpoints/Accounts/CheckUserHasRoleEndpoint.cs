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
    private readonly IAuthContext _authContext;

    public CheckUserHasRoleEndpoint(IAuthContext authContext)
    {
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

        var accountHasRole = account.HasAllRoles(req.Role);

        await SendAsync(
            accountHasRole,
            (int)(accountHasRole ? HttpStatusCode.OK : HttpStatusCode.Forbidden),
            ct);
    }
}
