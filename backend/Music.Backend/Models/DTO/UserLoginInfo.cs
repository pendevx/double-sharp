namespace Music.Backend.Models.DTO;

public class UserLoginInfo
{
    public UserLoginInfo(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; }
    public string Password { get; }
}
