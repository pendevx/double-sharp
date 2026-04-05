namespace Music.Models.Data;

public abstract record MimeType(string Value)
{
    public static implicit operator string(MimeType mimeType) => mimeType.Value;

    public static MimeType Create(string value) => value switch
    {
        "audio/ogg" => new Opus(),
        "audio/aac" => new Aac(),
        "audio/webm" => new WebM(),
        "audio/mpeg" => new Mp3(),
        "audio/mp4" => new M4A(),
        _ => throw new ArgumentException($"Unsupported MIME type: {value}")
    };

    public static MimeType InferFromFileName(string fileName) => Path.GetExtension(fileName).ToLowerInvariant() switch
    {
        ".ogg" or ".opus" => new Opus(),
        ".aac" => new Aac(),
        ".webm" => new WebM(),
        ".mp3" => new Mp3(),
        ".m4a" => new M4A(),
        ".m3u8" => new M3U8(),
        ".ts" => new TS(),
        ".mp4" => new MP4(),
        ".m4s" => new M4S(),
        _ => throw new ArgumentException($"Unsupported file extension: {fileName}")
    };
}

public sealed record Opus() : MimeType("audio/ogg");
public sealed record Aac() : MimeType("audio/aac");
public sealed record WebM() : MimeType("audio/webm");
public sealed record Mp3() : MimeType("audio/mpeg");
public sealed record M4A() : MimeType("audio/mp4");
public sealed record M3U8() : MimeType("application/vnd.apple.mpegurl");
public sealed record TS() : MimeType("video/mp2t");
public sealed record MP4() : MimeType("video/mp4");
public sealed record M4S() : MimeType("video/mp4");
