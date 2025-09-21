using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;

namespace Music.Backend.Endpoints.Artists;

public record GetArtistsResponse(int Id, string Name, string ImageUrl, DateOnly DateOfBirth);

[HttpGet("/artists")]
public class GetArtistsEndpoint : Ep.NoReq.Res<IList<GetArtistsResponse>>
{
    private readonly MusicContext _musicContext;

    public GetArtistsEndpoint(MusicContext musicContext)
    {
        _musicContext = musicContext;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var artists = await _musicContext.Artists.AsNoTracking()
            .Select(artist => new GetArtistsResponse(artist.Id, artist.Name, "", artist.DateOfBirth))
            .ToListAsync(ct);

        await SendOkAsync(artists, ct);
    }
}
