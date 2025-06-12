using Music.Models.Data;

namespace Music.Global.Contracts;

public interface IAuthContext
{
    Account? GetAccount();
}
