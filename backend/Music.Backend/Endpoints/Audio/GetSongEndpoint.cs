using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Music.Backend.EndpointProcessors.PostProcessors;
using Music.Backend.EndpointProcessors.PreProcessors;
using Music.Models.Data.DbContexts;
using Music.Services.DataAccess;

namespace Music.Backend.Endpoints.Audio;

public record GetSongRequest(int Id);

[HttpGet("/music/download/{id}")]
[PreProcessor<OpenDbConnection<GetSongRequest>>]
[PostProcessor<CloseDbConnection<GetSongRequest, Stream>>]
[AllowAnonymous]
public class GetSongEndpoint : Endpoint<GetSongRequest, Stream>
{
    private readonly MusicContext _dbContext;
    private readonly LargeObjectStreamReader _largeObjectStreamReader;

    public GetSongEndpoint(MusicContext dbContext, LargeObjectStreamReader largeObjectStreamReader)
    {
        _dbContext = dbContext;
        _largeObjectStreamReader = largeObjectStreamReader;
    }

    public override async Task HandleAsync(GetSongRequest req, CancellationToken ct)
    {
        if (await _dbContext.Songs.FirstOrDefaultAsync(s => s.Id == req.Id, ct) is not { } song)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var contentsPipeReader = await _largeObjectStreamReader.GetPipeReader(song.ContentsOid);

        await SendStreamAsync(
            contentsPipeReader.AsStream(),
            contentType: song.MimeType,
            enableRangeProcessing: true,
            cancellation: ct);
    }
}
