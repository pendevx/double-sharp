using Music.Models.Data.Errors;
using Music.Models.Data.Requests;
using Music.Models.Data.Utils;

namespace Music.Models.Data;

public class ArtistRequest : BaseEntity
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

    public ResultType<ArtistRequest, ResultError> Approve()
    {
        if (Status == RequestStatus.Rejected)
            return Result.Fail<ArtistRequest, ResultError>(new FailedOperationError("Can't approve a rejected Artist Request."));

        Status = RequestStatus.Approved;
        return Result.Ok<ArtistRequest, ResultError>(this);
    }

    public OptionType<Artist> CreateArtist() =>
        Status == RequestStatus.Approved ? Option.Some(Artist.Create(Name)) : Option.None<Artist>();

    public void Reject() =>
        Status = RequestStatus.Rejected;
}
