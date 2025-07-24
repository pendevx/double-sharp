using Music.Commands.Accounts;
using Music.EntityFramework;

namespace Music.CommandHandlers.Accounts;

public class ExtendSessionHandler : IBaseCommandHandler<ExtendSessionCommand, bool>
{
    private readonly MusicContext _dbContext;

    public ExtendSessionHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Execute(ExtendSessionCommand command)
    {
        var session = _dbContext.Sessions.FirstOrDefault(s => s.Token == command.Token);

        if (session is null)
            return false;

        session.ExpiresOn = DateTime.UtcNow.AddSeconds(command.ExtensionSeconds);
        _dbContext.SaveChanges();

        return true;
    }
}
