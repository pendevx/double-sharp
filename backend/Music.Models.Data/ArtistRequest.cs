using Music.Models.Data.Requests;

namespace Music.Models.Data;

public class ArtistRequest : BaseEntity, IRequestable<Artist>
{
    private ArtistRequest() { }

    private ArtistRequest(string name, DateOnly dateOfBirth) => (Name, DateOfBirth) = (name, dateOfBirth);

    private ArtistRequest(string name, DateOnly dateOfBirth, Account requester, RequestStatus requestStatus) =>
        (Name, DateOfBirth, Requester, Status) = (name, dateOfBirth, requester, requestStatus);
    public static ArtistRequest Create(string name, DateOnly dateOfBirth, Account requester) => new(name, dateOfBirth, requester, RequestStatus.Pending);

    public string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public virtual Account Requester { get; set; }
    public RequestStatus Status { get; set; }
    public virtual Artist? Artist { get; set; }
    public int? ArtistId { get; set; }

    public Artist Approve()
    {
        Status = RequestStatus.Approved;
        Artist = Artist.Create(Name);
        return Artist;
    }

    public void Reject() =>
        Status = RequestStatus.Rejected;
}
