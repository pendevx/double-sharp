using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Music.QueryHandlers.Accounts;

namespace Music.Backend.Endpoints.Accounts;

[HttpGet("/accounts/checkUserIsAdmin")]
[AllowAnonymous]
public class CheckUserIsAdminEndpoint : Ep.NoReq.Res<bool>
{
    private readonly CheckCurrentUserIsAdminHandler _checkCurrentCurrentAccountHasRole;
    
    public CheckUserIsAdminEndpoint(CheckCurrentUserIsAdminHandler checkCurrentCurrentAccountHasRole)
    {
        _checkCurrentCurrentAccountHasRole = checkCurrentCurrentAccountHasRole;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userHasRole = _checkCurrentCurrentAccountHasRole.Execute();

        await SendAsync(userHasRole, cancellation: ct);
    }
}
