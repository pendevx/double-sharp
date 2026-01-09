using Music.EntityFramework;

namespace Music.Backend.Endpoints.Audio;

public record struct SongInfo(int Id, string Name);

[HttpGet("/music/list")]
public class ListSongs : Ep.NoReq.Res<IEnumerable<SongInfo>>
{
    private readonly MusicContext _dbContext;

    public ListSongs(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var files = _dbContext.Songs
            .Select(song => new SongInfo(song.Id, song.Name));

        await SendAsync(files, 200, ct);
    }
}
