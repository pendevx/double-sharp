using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Services.SongRequests;

namespace Music.Backend.Endpoints.SongRequests.Admin;

public record ApproveSongRequestRequest(int Id);

public class ApproveSongRequestEndpoint : Ep.Req<ApproveSongRequestRequest>.NoRes
{
    private readonly MusicContext _dbContext;
    private readonly SongRequestService _songRequestService;
    private readonly RequiresPermission _requiresPermission;

    public ApproveSongRequestEndpoint(MusicContext dbContext, SongRequestService songRequestService, RequiresPermission requiresPermission)
    {
        _dbContext = dbContext;
        _songRequestService = songRequestService;
        _requiresPermission = requiresPermission;
    }

    public override void Configure()
    {
        Post("/song-requests/approve/{id}");
        Description(x => x.Accepts<ApproveSongRequestRequest>());
        Options(o => o.AddEndpointFilter<RequiresAuthenticatedFilter>());
    }

    public override async Task HandleAsync(ApproveSongRequestRequest req, CancellationToken ct)
    {
        if (!_requiresPermission(RoleName.Admin))
            throw new UnauthorizedAccessException();

        if (await _dbContext.SongRequests.FirstOrDefaultAsync(sr => sr.Id == req.Id, ct) is not { } songRequest)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var newSong = songRequest.Approve();

        await _dbContext.Songs.AddAsync(newSong, ct);
        await _dbContext.SaveChangesAsync(ct);

        await _songRequestService.ApproveSongRequestAsync(songRequest, newSong.Id);

        await SendOkAsync(null, ct);
    }
}
