namespace Music.Commands.Accounts;

public class RegisterAccountCommand
{
    public RegisterAccountCommand(string username, string password, string displayName)
    {
        Username = username;
        Password = password;
        DisplayName = displayName;
    }

    public string Username { get; }
    public string Password { get; }
    public string DisplayName { get; }
}
