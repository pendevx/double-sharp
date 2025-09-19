namespace Music.Models.Data.Requests;

public enum RequestStatus
{
    Pending, // Awaiting review
    Approved, // Approved and already added to the system
    Rejected, // Rejected
}
