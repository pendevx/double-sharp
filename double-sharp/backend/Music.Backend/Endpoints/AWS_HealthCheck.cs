using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Music.Backend.Endpoints;

[HttpGet("/healthcheck.html")]
[AllowAnonymous]
public class AwsHealthCheck : Ep.NoReq.NoRes
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendOkAsync(ct);
    }
}
