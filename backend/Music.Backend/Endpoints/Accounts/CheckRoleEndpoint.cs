using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Music.QueryHandlers.Accounts;

namespace Music.Backend.Endpoints.Accounts;

[HttpGet("/accounts/checkRole")]
[AllowAnonymous]
public class CheckRoleEndpoint : Ep.NoReq.Res<bool>
{
    private readonly CheckCurrentAccountHasRoleHandler _checkCurrentAccountHasRole;
    
    public CheckRoleEndpoint(CheckCurrentAccountHasRoleHandler checkCurrentAccountHasRole)
    {
        _checkCurrentAccountHasRole = checkCurrentAccountHasRole;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var role = Query<string>("role");

        if (role is null)
        {
            await SendAsync(false, (int)HttpStatusCode.BadRequest, cancellation: ct);
            return;
        }

        var userHasRole = _checkCurrentAccountHasRole.Execute(role);

        await SendAsync(userHasRole, cancellation: ct);
    }
}
