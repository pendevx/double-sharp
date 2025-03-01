using System.IO.Pipelines;
using Microsoft.EntityFrameworkCore;
using Music.Models.Data.DbContexts;
using Npgsql;

namespace Music.Services.DataAccess;

public class LargeObjectStreamReader
{
    private const int BufferSize = 60 * 1024;
    private const int ChunkSize = 16 * 1024;

    private readonly MusicContext _dbContext;

    public LargeObjectStreamReader(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PipeReader> GetPipeReader(int oid)
    {
        await _dbContext.Database.OpenConnectionAsync();
        var conn = (NpgsqlConnection) _dbContext.Database.GetDbConnection();
        await using var transaction = await conn.BeginTransactionAsync();

        var cts = new CancellationTokenSource();

        var reader = BeginWriteToPipe(conn, transaction, (uint)oid, cts.Token);

        return reader;
    }

    private PipeReader BeginWriteToPipe(NpgsqlConnection conn, NpgsqlTransaction transaction, uint oid, CancellationToken cancellationToken)
    {
        var loFd = OpenLargeObject(conn, transaction, oid, LargeObjectOpenModes.InvRead);

        var pipeOptions = new PipeOptions(
            pauseWriterThreshold: BufferSize,
            resumeWriterThreshold: BufferSize - ChunkSize);

        var pipe = new Pipe(pipeOptions);

        _ = FillPipeAsync(conn, transaction, (uint)loFd, pipe.Writer, cancellationToken);

        return pipe.Reader;
    }

    private int OpenLargeObject(NpgsqlConnection conn, NpgsqlTransaction transaction, uint oid, LargeObjectOpenModes mode)
    {
        var loFdResult = CreateCommand(conn, transaction, $"SELECT lo_open({oid}, {(int)mode})")
            .ExecuteScalar();

        if (loFdResult is not int loFd)
            throw new Exception("Failed to open large object.");

        return loFd;
    }

    private async Task FillPipeAsync(NpgsqlConnection conn, NpgsqlTransaction transaction, uint loFd, PipeWriter writer, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var cmd = CreateCommand(conn, transaction, $"SELECT loread({loFd}, {ChunkSize})");

                if (await cmd.ExecuteScalarAsync(cancellationToken) is not byte[] chunk || chunk.Length == 0)
                    break;

                var memory = writer.GetMemory(chunk.Length);
                chunk.CopyTo(memory.Span);
                writer.Advance(chunk.Length);

                var result = await writer.FlushAsync(cancellationToken);
                if (result.IsCompleted)
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from large object: {ex.Message}");
        }
        finally
        {
            await writer.CompleteAsync();
        }
    }

    private static NpgsqlCommand CreateCommand(NpgsqlConnection connection, NpgsqlTransaction transaction, string text)
    {
        var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = text;
        return cmd;
    }
}

internal enum LargeObjectOpenModes
{
    InvWrite = 0x20000,
    InvRead = 0x40000,
    InvReadWrite = 0x60000,
}
