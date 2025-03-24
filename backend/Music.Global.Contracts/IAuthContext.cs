using Music.Models.Domain;

namespace Music.Global.Contracts;

public interface IAuthContext
{
    Account? GetAccount();
}
