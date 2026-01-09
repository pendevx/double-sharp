namespace Music.Models.Data;

public static class SlugFactory
{
    public static string Create(string value) =>
        value
            .Trim()
            .ToLowerInvariant()
            .Replace("--", "-")
            .Replace(' ', '-')
            .Replace("\'", "")
            .Replace("\\", "");
}
