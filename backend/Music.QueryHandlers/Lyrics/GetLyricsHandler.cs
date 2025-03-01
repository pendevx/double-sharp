using System.Text.RegularExpressions;

namespace Music.QueryHandlers.Lyrics;

public class GetLyricsHandler : IBaseQueryHandler<int, IEnumerable<LyricsLine>>
{
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

    public IEnumerable<LyricsLine> Execute(int command)
    {
        return [];
    }
}

public record struct LyricsLine(int Time, string Words);
