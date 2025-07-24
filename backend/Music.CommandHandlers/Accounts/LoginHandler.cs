using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Music.Commands.Accounts;
using Music.EntityFramework;
using Music.Models.Data;
using Music.Repositories.Contracts;

namespace Music.CommandHandlers.Accounts;

public class LoginHandler : IBaseCommandHandler<LoginCommand, Guid>
{
    private const string GenericAuthenticationFailure = "The username or password did not match.";

    private readonly MusicContext _dbContext;

    public LoginHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static byte[] GenerateSaltedHash(byte[] raw, byte[] salt)
    {
        var salted = raw.Concat(salt).ToArray();
        return SHA256.HashData(salted);
    }

    private static bool IsEqual(byte[] original, byte[] other)
    {
        if (original.Length != other.Length)
            return false;

        return !original.Where((t, i) => t != other[i]).Any();
    }

    public Guid Execute(LoginCommand command)
    {
        var existingUser = _dbContext.Accounts.FirstOrDefault(acc => acc.Username == command.Username);

        if (existingUser is null)
            throw new AuthenticationException(GenericAuthenticationFailure);

        var saltedPassword = GenerateSaltedHash(Encoding.UTF8.GetBytes(command.Password), existingUser.Guid.ToByteArray());

        if (!IsEqual(saltedPassword, existingUser.SaltedPassword))
            throw new AuthenticationException(GenericAuthenticationFailure);

        var token = Guid.NewGuid();

        var session = new Session
        {
            Account = existingUser,
            ExpiresOn = DateTime.UtcNow.AddDays(7), // Expire the token in one week
            Token = token
        };

        _dbContext.Sessions.Add(session);
        _dbContext.SaveChanges();

        return token;
    }
}
