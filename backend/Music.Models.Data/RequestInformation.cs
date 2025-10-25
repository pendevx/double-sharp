using Music.Models.Data.Errors;
using Music.Models.Data.Utils;

namespace Music.Models.Data;

public class RequestInformation
{
    protected RequestInformation() { }
    private RequestInformation(RequestStatus status, Account requester) => (Status, Requester) = (status, requester);

    public RequestStatus Status { get; private set; }
    private int _requesterId;
    public virtual Account Requester { get; private set; }

    public static RequestInformation Create(Account requester) => new(RequestStatus.Pending, requester);

    public ResultType<RequestInformation, ResultError> Approve()
    {
        if (Status != RequestStatus.Pending)
            return Result.Fail<RequestInformation, ResultError>(
                new FailedOperationError("Only Pending requests can be approved."));

        Status = RequestStatus.Approved;

        return Result.Ok<RequestInformation, ResultError>(this);
    }

    public ResultType<RequestInformation, ResultError> Reject()
    {
        if (Status != RequestStatus.Pending)
            return Result.Fail<RequestInformation, ResultError>(
                new FailedOperationError("Only Pending requests can be approved."));

        Status = RequestStatus.Rejected;

        return Result.Ok<RequestInformation, ResultError>(this);
    }
}

public enum RequestStatus
{
    Pending, // Awaiting review
    Approved, // Approved and already added to the system
    Rejected, // Rejected
}
