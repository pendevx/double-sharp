using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Models.Data.Errors;
using Music.Models.Data.Utils;

// Results<Ok<SuggestArtistResponse>, BadRequest>
using TResponse = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Ok<
        Music.Backend.Endpoints.Artists.SuggestArtistResponse>,
    Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>;

namespace Music.Backend.Endpoints.Artists;

public record SuggestArtistRequest(string Name, DateOnly DateOfBirth);
public record SuggestArtistResponse(int Id, string Name, DateOnly DateOfBirth);

[RequiresAuthenticated]
[HttpPost("/artists/suggest")]
public class SuggestArtistEndpoint : Ep.Req<SuggestArtistRequest>.Res<TResponse>
{
    private readonly MusicContext _musicContext;
    private readonly IAuthContext _authContext;

    public SuggestArtistEndpoint(MusicContext musicContext, IAuthContext authContext)
    {
        _musicContext = musicContext;
        _authContext = authContext;
    }

    public override Task<TResponse> HandleAsync(SuggestArtistRequest req, CancellationToken ct) =>
        _authContext.GetAccount_Option()
            .ToResult(new UnauthenticatedError())
            .Map(account => Artist.SuggestNew(req.Name, req.DateOfBirth, account))
            .MapAsync(artistRequest => SaveArtistSuggestion(artistRequest, ct))
            .MapAsync(MapSuggestedArtistToResponse)
            .MatchAsync(
                TResponse (artistRequest) => TypedResults.Ok(artistRequest),
                TResponse (_) => TypedResults.Forbid()
            );

    private async Task<Artist> SaveArtistSuggestion(Artist artist, CancellationToken ct)
    {
        await _musicContext.Artists.AddAsync(artist, ct);
        await _musicContext.SaveChangesAsync(ct);
        return artist;
    }

    private static SuggestArtistResponse MapSuggestedArtistToResponse(Artist artist) =>
        new(artist.Id, artist.Name, artist.DateOfBirth);
}
