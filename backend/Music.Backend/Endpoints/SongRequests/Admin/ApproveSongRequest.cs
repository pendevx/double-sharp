using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;
using Music.Services.SongRequests;

namespace Music.Backend.Endpoints.SongRequests.Admin;

public record ApproveSongRequestRequest(int Id);

public class ApproveSongRequest : Ep.Req<ApproveSongRequestRequest>.NoRes
{
    private readonly MusicContext _dbContext;
    private readonly SongRequestService _songRequestService;

    public ApproveSongRequest(MusicContext dbContext, SongRequestService songRequestService)
    {
        _dbContext = dbContext;
        _songRequestService = songRequestService;
    }

    public override void Configure()
    {
        Post("/song-requests/approve/{id}");
        Description(x => x.Accepts<ApproveSongRequestRequest>());
    }

    public override async Task HandleAsync(ApproveSongRequestRequest req, CancellationToken ct)
    {
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
