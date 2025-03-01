using Music.Models.Data.DbContexts;
using Music.Models.Domain;

namespace Music.CommandHandlers.Audio;

public class SilentlyUploadSongHandler : IBaseCommandHandler
{
    private readonly MusicContext _dbContext;

    public SilentlyUploadSongHandler(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Execute()
    {
        // // Silently scan ON THE SERVER
        // var files = Directory.GetFiles(@"C:\Downloads");
        // foreach (var file in files)
        // {
        //     var content = File.ReadAllBytes(file);
        //     _dbContext.Songs.Add(new Song
        //     {
        //         Contents = content,
        //         Name = file[(file.LastIndexOf('\\') + 1)..],
        //         MimeType = "audio/mpeg"
        //     });
        // }
        //
        // _dbContext.SaveChanges();
        //
        // // Remove once added
        // foreach (var file in files)
        // {
        //     File.Delete(file);
        // }
    }
}
