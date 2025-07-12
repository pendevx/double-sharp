using Music.Models.Data;
using Music.EntityFramework;
using Music.Repositories.Contracts;

namespace Music.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(MusicContext ctx) : base(ctx)
    {
    }

    public Account? GetByUsername(string username)
    {
        var account = Entities.FirstOrDefault(a => a.Username == username);
        return account;
    }
}
