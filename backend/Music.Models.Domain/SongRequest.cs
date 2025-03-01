namespace Music.Models.Domain;

public class SongRequest : Models.Data.SongRequest
{
    public RequestStatus RequestStatusEnum
    {
        get => Enum.Parse<RequestStatus>(RequestStatus);
        set => RequestStatus = value.ToString();
    }
}

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected
}
