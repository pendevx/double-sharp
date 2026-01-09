namespace Music.Models.Data.SongRequests;

public abstract record SongRequestUrl(string Url)
{
    public static implicit operator string(SongRequestUrl url) => url.Url;

    public static SongRequestUrl FromSource(Source source, string url) => source switch
    {
        Source.File => new S3Key(url),
        Source.YouTube => new YouTubeUrl(url),
        Source.YouTubeMusic => new YouTubeMusicUrl(url),
        _ => throw new NotSupportedException($"Unknown source: {source}"),
    };
}

public sealed record S3Key(string Key) : SongRequestUrl(Key);

public abstract record SongRequestWebUrl(string UrlPrefix, string Id) : SongRequestUrl(UrlPrefix + Id);
public sealed record YouTubeUrl(string Id) : SongRequestWebUrl("https://www.youtube.com/watch?v=", Id);
public sealed record YouTubeMusicUrl(string Id) : SongRequestWebUrl("https://music.youtube.com/watch?v=", Id);
