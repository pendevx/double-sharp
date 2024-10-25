using Music.Database.Entities;

namespace Music.Repositories.Contracts;

public interface ISessionRepository : IGenericRepository<Session>
{
    Session? GetSessionByToken(Guid token);
    IQueryable<Session> GetExpiredSessions(int accountId);
    Account? GetAccountFromSession(Guid token);
}
