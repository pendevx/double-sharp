using Music.Global.Contracts;
using Music.Models.Data.DbContexts;
using Music.Models.Domain;
using Music.QueryHandlers.Accounts;
using Music.Services.Validators;
using Music.Tests.Base;

namespace Music.QueryHandlers.Tests.Accounts;

public class UserRoleCheckerTests : BaseTest
{
    [Fact]
    public void ReturnTrueWhen_CurrentUserIsAdmin()
    {
        #region setup

        const string username = "admin";
        var account = DbContext.Accounts.Add(new Account
        {
            Username = username,
            DisplayName = "Admin",
            SaltedPassword = [],
        });

        DbContext.SaveChanges();

        var role = DbContext.Roles.Single(r => r.Name == nameof(RoleName.Admin));

        DbContext.AccountRoles.Add(new AccountRole
        {
            AccountId = account.Entity.Id,
            RoleId = role.Id,
        });

        DbContext.SaveChanges();

        #endregion

        var authContext = new UserRoleCheckerAuthContext(DbContext);
        var accountHasRoleValidator = new AccountHasRoleValidator(DbContext);

        var handler = new CheckCurrentUserIsAdminHandler(authContext, accountHasRoleValidator);

        var hasAdminRole = handler.Execute();

        Assert.True(hasAdminRole);
    }
}

class UserRoleCheckerAuthContext : IAuthContext
{
    private readonly MusicContext _dbContext;

    public UserRoleCheckerAuthContext(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Account? GetAccount()
    {
        return _dbContext.Accounts.FirstOrDefault(acc => acc.Username == "admin") as Account;
    }
}
