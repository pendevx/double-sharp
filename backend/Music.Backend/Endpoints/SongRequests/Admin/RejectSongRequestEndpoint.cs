using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;

namespace Music.Backend.Endpoints.SongRequests.Admin;

public record RejectSongRequestRequest(int Id);

public class RejectSongRequestEndpoint : Ep.Req<RejectSongRequestRequest>.NoRes
{
    private readonly MusicContext _dbContext;
    private readonly RequiresPermission _requiresPermission;

    public RejectSongRequestEndpoint(MusicContext dbContext, RequiresPermission requiresPermission)
    {
        _dbContext = dbContext;
        _requiresPermission = requiresPermission;
    }

    public override void Configure()
    {
        Post("/song-requests/reject/{id}");
        Description(x => x.Accepts<RejectSongRequestRequest>());
        Options(o => o.AddEndpointFilter<RequiresAuthenticatedFilter>());
    }

    public override async Task HandleAsync(RejectSongRequestRequest req, CancellationToken ct)
    {
        if (!_requiresPermission(RoleName.Admin))
            throw new UnauthorizedAccessException();

        if (await _dbContext.SongRequests.FirstOrDefaultAsync(sr => sr.Id == req.Id, ct) is not { } songRequest)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        songRequest.Reject();

        await _dbContext.SaveChangesAsync(ct);
        await SendOkAsync(null, ct);
    }
}
