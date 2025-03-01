using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Music.Models.Data;
using Music.Models.Data.DbContexts;
using Music.Repositories.Contracts;

namespace Music.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    public MusicContext MusicContext { get; }
    public DbSet<T> Entities { get; }

    protected GenericRepository(MusicContext ctx)
    {
        MusicContext = ctx;
        Entities = ctx.Set<T>();
    }

    public TQueryResult? ExecuteScalar<TQueryResult>(string commandText, DbTransaction? transaction = null)
        where TQueryResult : class
    {
        var connection = MusicContext.Database.GetDbConnection();

        transaction ??= connection.BeginTransaction();

        var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.Transaction = transaction;

        var results = command.ExecuteScalar();

        return results as TQueryResult;
    }

    public DbTransaction BeginTransaction()
    {
        return MusicContext.Database.GetDbConnection().BeginTransaction();
    }

    public T? GetById(int id)
    {
        return Entities.FirstOrDefault(e => e.Id == id);
    }

    public void Create(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Entities.Add(entity);
        }

        MusicContext.SaveChanges();
    }

    public void Delete(T entity, bool commit)
    {
        Delete(new[] { entity }, commit);
    }

    public void Delete(IEnumerable<T> entities, bool commit)
    {
        Entities.RemoveRange(entities);

        if (commit)
            MusicContext.SaveChanges();
    }

    public void Create(T entity)
    {
        Create(new[] { entity });
    }
}
