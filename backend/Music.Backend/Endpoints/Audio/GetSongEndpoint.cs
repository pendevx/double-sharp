using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Music.Models.Data.DbContexts;
using Music.Services.DataAccess.AWS;

namespace Music.Backend.Endpoints.Audio;

public record GetSongRequest(int Id);

[HttpGet("/music/download/{id}")]
[AllowAnonymous]
public class GetSongEndpoint : Endpoint<GetSongRequest, Stream>
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

        var stream = await _songsRepository.Download(song.Id);

        await SendStreamAsync(stream, song.Name, contentType: song.MimeType, enableRangeProcessing: true,
            cancellation: ct);
    }
}
