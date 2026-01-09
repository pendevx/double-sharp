using System.Text;
using Music.Commands.Accounts;
using Music.EntityFramework;
using Music.Models.Data;
using Music.Services;
using Account = Music.Models.Data.Account;

namespace Music.CommandHandlers.Accounts;

public class RegisterAccountHandler : IBaseCommandHandler<RegisterAccountCommand, bool>
{
    private readonly MusicContext _dbContext;

    public RegisterAccountHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Execute(RegisterAccountCommand command)
    {
        var existingAccount = _dbContext.Accounts.FirstOrDefault(acc => acc.Username == command.Username);

        if (existingAccount is not null)
            return false;

        var newGuid = Guid.NewGuid();
        var saltedPassword = Cryptography.GenerateSaltedHash(Encoding.UTF8.GetBytes(command.Password), newGuid.ToByteArray());

        var userRole = _dbContext.Roles.Single(r => r.Name == RoleName.User);

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
