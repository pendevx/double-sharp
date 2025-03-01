using Music.Models.Data;

namespace Music.Repositories.Contracts;

public interface IAccountRepository : IGenericRepository<Account>
{
    Account? GetByUsername(string username);
}
