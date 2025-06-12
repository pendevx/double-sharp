using Music.Global.Contracts;
using Music.Models.Data.DbContexts;
using Music.Models.Data;
using Music.Tests.Base;

namespace Music.QueryHandlers.Tests.Accounts;

public class UserRoleCheckerTests : BaseTest
{
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
        return _dbContext.Accounts.FirstOrDefault(acc => acc.Username == "admin");
    }
}
