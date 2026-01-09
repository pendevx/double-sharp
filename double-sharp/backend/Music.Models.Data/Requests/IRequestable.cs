namespace Music.Models.Data.Requests;

public interface IRequestable<out TRequestResult>
{
    RequestStatus Status { get; set; }

    TRequestResult Approve();

    void Reject();
}
