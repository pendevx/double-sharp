namespace Music.Models.Data;

public class SongRequest : BaseEntity
{
    public string SourceUrl { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? MimeType { get; set; }

    public virtual Account UploaderAccount { get; set; } = null!;

    public RequestStatus RequestStatus { get; set; }
}

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected
}
