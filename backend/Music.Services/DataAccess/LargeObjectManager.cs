using Microsoft.EntityFrameworkCore;
using Music.Models.Data.DbContexts;
using Npgsql;

namespace Music.Services.DataAccess;

public class LargeObjectManager
{
    private const int ChunkSize = 16 * 1024;

    private readonly MusicContext _dbContext;

    public LargeObjectManager(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(NpgsqlConnection, NpgsqlTransaction)> BeginOperation()
    {
        await _dbContext.Database.OpenConnectionAsync();
        var conn = (NpgsqlConnection) _dbContext.Database.GetDbConnection();
        var transaction = await conn.BeginTransactionAsync();

        return (conn, transaction);
    }

    public async Task WriteLargeObject(NpgsqlConnection conn, NpgsqlTransaction transaction, uint oid, Stream stream)
    {
        var buffer = new byte[ChunkSize];

        for (var i = 0; ; i++)
        {
            var remaining = stream.Length - stream.Position;

            if (remaining is 0)
                break;

            if (remaining < buffer.Length)
            {
                var sizedBuffer = new byte[remaining];
                await stream.ReadExactlyAsync(sizedBuffer);
                await PutLargeObject(conn, transaction, oid, sizedBuffer, i * buffer.Length);
            }
            else
            {
                await stream.ReadExactlyAsync(buffer);
                await PutLargeObject(conn, transaction, oid, buffer, i * buffer.Length);
            }
        }
    }

    public async Task<byte[]> ReadLargeObject(NpgsqlConnection conn, NpgsqlTransaction tran, uint oid,
        CancellationToken ct)
    {
        var cmd = CreateCommand(conn, tran, $"SELECT lo_get({oid})");

        return await cmd.ExecuteScalarAsync(ct) as byte[] ?? null!;
    }

    #region Basic operations

    private static NpgsqlCommand CreateCommand(NpgsqlConnection connection, NpgsqlTransaction transaction, string text)
    {
        var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = text;
        return cmd;
    }

    public uint CreateLargeObject(NpgsqlConnection conn, NpgsqlTransaction transaction)
    {
        var oidResult = CreateCommand(conn, transaction, "SELECT lo_create(0)")
            .ExecuteScalar();

        if (oidResult is not uint oid)
            throw new Exception("Failed to create large object.");

        return oid;
    }

    private static async Task PutLargeObject(NpgsqlConnection conn, NpgsqlTransaction transaction, uint oid, byte[] data, long offset = 0)
    {
        var cmd = CreateCommand(conn, transaction, $"SELECT lo_put({oid}, {offset}, @data)");
        cmd.Parameters.AddWithValue("data", data);

        await cmd.ExecuteNonQueryAsync();
    }

    #endregion
}

internal enum LargeObjectOpenModes
{
    InvWrite = 0x20000,
    InvRead = 0x40000,
    InvReadWrite = 0x60000,
}
