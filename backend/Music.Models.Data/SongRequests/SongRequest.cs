namespace Music.Models.Data.SongRequests;

public class SongRequest : BaseEntity
{
    protected SongRequest() { }

    private SongRequest(string name, RequestStatus status, Source source, string urlValue) =>
        (Name, Status, Source, _urlValue) = (name, status, source, urlValue);

    private SongRequest(string name, Account uploader, RequestStatus status, Source source, SongRequestUrl url) =>
        (Name, Uploader, Status, Source, Url) = (name, uploader, status, source, url);

    public string Name { get; }
    public virtual Account Uploader { get; }
    public RequestStatus Status { get; private set; }
    public Source Source { get; }

    private string _urlValue;
    public string RawUrl => _urlValue;
    public SongRequestUrl Url
    {
        get => Source switch
        {
            Source.File => new S3Key(_urlValue),
            Source.YouTube => new YouTubeUrl(_urlValue),
            Source.YouTubeMusic => new YouTubeMusicUrl(_urlValue),
            _ => throw new InvalidOperationException("Unknown source")
        };
        set => _urlValue = value.Url;
    }

    public static SongRequest CreateFileRequest(string name, Account uploader, S3Key key) =>
        new(name, uploader, RequestStatus.Pending, Source.File, key);

    public static SongRequest CreateYouTubeRequest(string name, Account uploader, YouTubeUrl url) =>
        new(name, uploader, RequestStatus.Pending, Source.YouTube, url);

    public static SongRequest CreateYouTubeMusicRequest(string name, Account uploader, YouTubeMusicUrl url) =>
        new(name, uploader, RequestStatus.Pending, Source.YouTubeMusic, url);

    public Song Approve()
    {
        Status = RequestStatus.Approved;
        return Song.Create(Name);
    }

    public void Reject() => Status = RequestStatus.Rejected;
}

public enum RequestStatus
{
    Pending, // Awaiting review
    Approved, // Approved and already added to the system
    Rejected, // Rejected
}

public enum Source
{
    YouTube,
    YouTubeMusic,
    File,
}
