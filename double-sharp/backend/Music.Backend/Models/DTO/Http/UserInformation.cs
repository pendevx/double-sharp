namespace Music.Backend.Models.DTO.Http;

public class UserInformation
{
    public string DisplayName { get; init; } = null!;
    public Guid SessionToken { get; init; }
}
