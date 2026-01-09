namespace Music.Commands.Accounts;

public class LoginCommand
{
    public LoginCommand(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; }
    public string Password { get; }
}
