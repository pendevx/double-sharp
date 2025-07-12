using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;

namespace Music.Repositories.Contracts;

public interface IGenericRepository<T> where T : class
{
    MusicContext MusicContext { get; }
    DbSet<T> Entities { get; }

    /**
     * Requires an active connection with the database.
     */
    TQueryResult? ExecuteScalar<TQueryResult>(string command, DbTransaction? transaction = null) where TQueryResult : class;
    /**
     * Requires an active connection with the database.
     */
    DbTransaction BeginTransaction();
    T? GetById(int id);
    void Create(T entity);
    void Create(IEnumerable<T> entities);
    void Delete(T entity, bool commit = false);
    void Delete(IEnumerable<T> entities, bool commit = false);
}
