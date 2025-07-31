using System.Security.Cryptography;
using System.Text;
using Music.Commands.Accounts;
using Music.Models.Data;
using Music.EntityFramework;
using Account = Music.Models.Data.Account;

namespace Music.CommandHandlers.Accounts;

public class RegisterAccountHandler : IBaseCommandHandler<RegisterAccountCommand, bool>
{
    private readonly MusicContext _dbContext;

    public RegisterAccountHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static byte[] GenerateSaltedHash(byte[] raw, byte[] salt)
    {
        var salted = raw.Concat(salt).ToArray();
        return SHA256.HashData(salted);
    }

    public bool Execute(RegisterAccountCommand command)
    {
        var existingAccount = _dbContext.Accounts.FirstOrDefault(acc => acc.Username == command.Username);

        if (existingAccount is not null)
            return false;

        var newGuid = Guid.NewGuid();
        var saltedPassword = GenerateSaltedHash(Encoding.UTF8.GetBytes(command.Password), newGuid.ToByteArray());

        var userRole = _dbContext.Roles.Single(r => r.Name == nameof(RoleName.User));

        _dbContext.Accounts.Add(new Account
        {
            Guid = newGuid,
            Username = command.Username,
            SaltedPassword = saltedPassword,
            DisplayName = command.DisplayName,
            Roles = [userRole]
        });

        _dbContext.SaveChanges();

        return true;
    }
}
