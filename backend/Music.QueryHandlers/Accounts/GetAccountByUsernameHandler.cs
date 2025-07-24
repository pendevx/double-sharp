using Music.EntityFramework;
using Music.Models.Data;

namespace Music.QueryHandlers.Accounts;

public class GetAccountByUsernameHandler : IBaseQueryHandler<string, Account?>
{
    private readonly MusicContext _dbContext;

    public GetAccountByUsernameHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Account? Execute(string username) => _dbContext.Accounts.FirstOrDefault(acc => acc.Username == username);
}
