using Music.EntityFramework;

namespace Music.QueryHandlers.Accounts;

public class ValidateTokenIsActiveHandler : IBaseQueryHandler<Guid, bool>
{
    private readonly MusicContext _dbContext;

    public ValidateTokenIsActiveHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Execute(Guid token)
    {
        return _dbContext.Sessions.FirstOrDefault(s => s.Token == token)?.ExpiresOn > DateTime.UtcNow;
    }
}
