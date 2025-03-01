using FastEndpoints;
using Music.QueryHandlers.Lyrics;

namespace Music.Backend.Endpoints.Lyrics;

public record GetLyricsRequest(int Id);

[HttpGet("/download/{id}")]
public class GetLyricsForAudioEndpoint : Endpoint<GetLyricsRequest, IEnumerable<LyricsLine>>
{
    private readonly GetLyricsHandler _getLyrics;

    public GetLyricsForAudioEndpoint(GetLyricsHandler getLyrics)
    {
        _getLyrics = getLyrics;
    }

    public override async Task HandleAsync(GetLyricsRequest req, CancellationToken ct)
    {
        var lyrics = _getLyrics.Execute(req.Id);
        await SendAsync(lyrics, cancellation: ct);
    }
}
