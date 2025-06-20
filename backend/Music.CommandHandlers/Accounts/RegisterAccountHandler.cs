using System.Security.Cryptography;
using System.Text;
using Music.Commands.Accounts;
using Music.Models.Data;
using Music.Models.Data.DbContexts;
using Account = Music.Models.Data.Account;
using AccountRole = Music.Models.Data.AccountRole;

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

        var createdAccount = _dbContext.Accounts.Add(new Account
        {
            Guid = newGuid,
            Username = command.Username,
            SaltedPassword = saltedPassword,
            DisplayName = command.DisplayName
        });

        _dbContext.SaveChanges();

        var userRoleId = _dbContext.Roles.Where(r => r.Name == nameof(RoleName.User))
            .Select(r => r.Id)
            .Single();

        _dbContext.AccountRoles.Add(new AccountRole
        {
            RoleId = userRoleId,
            AccountId = createdAccount.Entity.Id
        });

        _dbContext.SaveChanges();

        return true;
    }
}
