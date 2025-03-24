using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Music.Models.Data.DbContexts;
using Music.Services.DataAccess;

namespace Music.Backend.Endpoints.Audio;

public record GetSongRequest(int Id);

[HttpGet("/music/download/{id}")]
[AllowAnonymous]
public class GetSongEndpoint : Endpoint<GetSongRequest, Stream>
{
    private readonly MusicContext _dbContext;
    private readonly LargeObjectManager _largeObjectManager;

    public GetSongEndpoint(MusicContext dbContext, LargeObjectManager largeObjectManager)
    {
        _dbContext = dbContext;
        _largeObjectManager = largeObjectManager;
    }

    public override async Task HandleAsync(GetSongRequest req, CancellationToken ct)
    {
        var (conn, transaction) = await _largeObjectManager.BeginOperation();

        if (await _dbContext.Songs.FirstOrDefaultAsync(s => s.Id == req.Id, ct) is not { } song)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var bytes = await _largeObjectManager.ReadLargeObject(conn, transaction, song.ContentsOid, ct);

        await SendBytesAsync(
            bytes,
            contentType: song.MimeType,
            enableRangeProcessing: true,
            cancellation: ct);
    }
}
