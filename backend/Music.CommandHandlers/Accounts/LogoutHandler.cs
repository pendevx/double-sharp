using Music.EntityFramework;

namespace Music.CommandHandlers.Accounts;

public class LogoutHandler : IBaseCommandHandler<Guid>
{
    private readonly MusicContext _dbContext;

    public LogoutHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Execute(Guid token)
    {
        var session = _dbContext.Sessions.FirstOrDefault(s => s.Token == token);

        if (session is not null)
        {
            _dbContext.Sessions.Remove(session);
            _dbContext.SaveChanges();
        }
    }
}
