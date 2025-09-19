using Music.Models.Data;
using Music.Models.Data.Utils;

namespace Music.Global.Contracts;

public interface IAuthContext
{
    Account? GetAccount();
    OptionType<Account> GetAccount_Option();
}
