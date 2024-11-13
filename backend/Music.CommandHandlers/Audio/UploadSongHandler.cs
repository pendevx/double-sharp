using Music.Repositories.Contracts;
using Music.Repository.EF.Models.Generated;

namespace Music.CommandHandlers.Audio;

public class UploadSongHandler : IBaseCommandHandler<object>
{
    private readonly ISongRepository _songRepository;

    public UploadSongHandler(ISongRepository songRepository)
    {
        _songRepository = songRepository;
    }

    public void Execute(object o)
    {
        // Silently scan ON THE SERVER
        var files = Directory.GetFiles(@"C:\Downloads");
        foreach (var file in files)
        {
            var content = File.ReadAllBytes(file);
            _songRepository.Create(new Song
            {
                Contents = content,
                Name = file[(file.LastIndexOf('\\') + 1)..],
                MimeType = "audio/mpeg"
            });
        }

        // Remove once added
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }
}
