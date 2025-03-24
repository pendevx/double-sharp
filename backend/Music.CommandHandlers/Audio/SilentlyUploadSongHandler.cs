using Music.Models.Data.DbContexts;
using Music.Models.Data;
using Music.Services.DataAccess;

namespace Music.CommandHandlers.Audio;

public class SilentlyUploadSongHandler : IBaseCommandHandler
{
    private readonly MusicContext _dbContext;
    private readonly LargeObjectManager _largeObjectManager;

    public SilentlyUploadSongHandler(MusicContext dbContext, LargeObjectManager largeObjectManager)
    {
        _dbContext = dbContext;
        _largeObjectManager = largeObjectManager;
    }

    public void Execute()
    {
        var (conn, transaction) = _largeObjectManager.BeginOperation().GetAwaiter().GetResult();

        // Silently scan ON THE SERVER
        var files = Directory.GetFiles(@"D:\documents\upload_source");

        var songs = new Song[files.Length];
        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var song = new Song
            {
                ContentsOid = _largeObjectManager.CreateLargeObject(conn, transaction),
                Name = file[(file.LastIndexOf('\\') + 1)..],
                MimeType = "audio/mpeg",
            };
            songs[i] = _dbContext.Songs.Add(song).Entity;
        }

        _dbContext.SaveChanges();

        for (var i = 0; i < songs.Length; i++)
        {
            var file = files[i];
            using (var content = File.OpenRead(file))
            {
                _largeObjectManager.WriteLargeObject(conn, transaction, songs[i].ContentsOid, content).GetAwaiter().GetResult();
                songs[i].Length = (uint)content.Length;
            }

            _dbContext.Update(songs[i]);

            File.Delete(file);
        }

        _dbContext.SaveChanges();
        transaction.Commit();
    }
}
