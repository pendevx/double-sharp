using Music.Models.Data.DbContexts;
using Music.Models.Domain;

namespace Music.Services.Validators;

public class AccountHasRoleValidator
{
    private readonly MusicContext _dbContext;

    public AccountHasRoleValidator(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Verify(int accountId, RoleName roleName)
    {
        var accountFound = _dbContext.Accounts.Any(acc => acc.Id == accountId);

        if (!accountFound)
            return false;

        var accountHasRole = _dbContext.AccountRoles
            .Any(ar => ar.AccountId == accountId && ar.Role.Name == roleName.ToString());

        return accountHasRole;
    }
}
