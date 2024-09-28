namespace Music.Backend.Services.Contracts;

public interface IAuthenticationService
{
    bool Register(string username, string password, string displayName);
    string Login(string username, string password);
}
