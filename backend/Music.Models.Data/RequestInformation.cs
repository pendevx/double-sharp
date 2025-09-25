using Music.Models.Data.Errors;
using Music.Models.Data.Utils;

namespace Music.Models.Data;

public class RequestInformation
{
    private RequestInformation() { }
    private RequestInformation(RequestStatus status, Account requester) => (Status, Requester) = (status, requester);

    public RequestStatus Status { get; private set; }
    public virtual Account Requester { get; private set; }

    public static RequestInformation Create(Account requester) => new(RequestStatus.Pending, requester);

    public ResultType<RequestInformation, ResultError> Approve() =>
        Status != RequestStatus.Pending
            ? Result.Fail<RequestInformation, ResultError>(new FailedOperationError("Only Pending requests can be approved."))
            : Result.Ok<RequestInformation, ResultError>(this);

    public ResultType<RequestInformation, ResultError> Reject() =>
        Status != RequestStatus.Pending
            ? Result.Fail<RequestInformation, ResultError>(new FailedOperationError("Only Pending requests can be rejected."))
            : Result.Ok<RequestInformation, ResultError>(this);
}

public enum RequestStatus
{
    Pending, // Awaiting review
    Approved, // Approved and already added to the system
    Rejected, // Rejected
}
