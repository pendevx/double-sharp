using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;
using Music.Services.DataAccess.AWS;

namespace Music.Backend.Endpoints.Audio;

public record GetSongRequest(int Id);

[HttpGet("/music/download/{id}")]
public class GetSongEndpoint : Endpoint<GetSongRequest, string>
{
    private readonly MusicContext _dbContext;
    private readonly SongsRepository _songsRepository;

    public GetSongEndpoint(MusicContext dbContext, SongsRepository songsRepository)
    {
        _dbContext = dbContext;
        _songsRepository = songsRepository;
    }

    public override async Task HandleAsync(GetSongRequest req, CancellationToken ct)
    {
        if (await _dbContext.Songs.FirstOrDefaultAsync(s => s.Id == req.Id, ct) is not { } song)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var rangeHeader = HttpContext.Request.Headers.Range.ToString();

        if (string.IsNullOrEmpty(rangeHeader))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var url = await _songsRepository.GetMediaSignedUrl(song.Id);
        HttpContext.Response.Headers.Location = url;
        HttpContext.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        await SendAsync(null!, 307, ct);
    }
}
