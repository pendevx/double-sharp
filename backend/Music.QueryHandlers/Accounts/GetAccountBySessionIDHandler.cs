using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;
using Music.Models.Data;

namespace Music.QueryHandlers.Accounts;

public class GetAccountBySessionIDHandler : IBaseQueryHandler<Guid, Account?>
{
    private readonly MusicContext _dbContext;

    public GetAccountBySessionIDHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Account? Execute(Guid token) =>
        _dbContext.Sessions.Where(s => s.Token == token && s.ExpiresOn > DateTime.UtcNow)
            .Include(s => s.Account)
            .FirstOrDefault()?
            .Account;
}
