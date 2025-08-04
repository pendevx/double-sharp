using System.Text.RegularExpressions;

namespace Music.Backend.Endpoints.Lyrics;

public record GetLyricsRequest(int Id);
public record struct LyricsLine(int Time, string Words);

[HttpGet("/download/{id}")]
public class GetLyricsForAudioEndpoint : Endpoint<GetLyricsRequest, IEnumerable<LyricsLine>>
{
    public override async Task HandleAsync(GetLyricsRequest req, CancellationToken ct)
    {
        await SendAsync([], cancellation: ct);
    }

    private static LyricsLine ProcessLyricsLine(string line)
    {
        int ToSeconds(string mins, string secs, string ms)
            => int.Parse(mins) * 60 + int.Parse(secs) + int.Parse(ms) / 1000;

        var regex = new Regex(@"\[(\d{2}):(\d{2})\.(\d{2,3})\](.*)", RegexOptions.IgnoreCase);
        var groups = regex.Matches(line)[0].Groups;

        return new LyricsLine()
        {
            Time = ToSeconds(groups[1].Value, groups[2].Value, groups[3].Value),
            Words = groups[4].Value
        };
    }
}
