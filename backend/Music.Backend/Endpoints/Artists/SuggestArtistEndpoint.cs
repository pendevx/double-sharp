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
            .Map(account => ArtistRequest.Create(req.Name, req.DateOfBirth, account))
            .MapAsync(artistRequest => SaveArtistRequest(artistRequest, ct))
            .MapAsync(MapArtistRequestToResponse)
            .MatchAsync(
                TResponse (artistRequest) => TypedResults.Ok(artistRequest),
                TResponse (_) => TypedResults.Forbid()
            );

    private async Task<ArtistRequest> SaveArtistRequest(ArtistRequest artist, CancellationToken ct)
    {
        await _musicContext.ArtistRequests.AddAsync(artist, ct);
        await _musicContext.SaveChangesAsync(ct);
        return artist;
    }

    private static SuggestArtistResponse MapArtistRequestToResponse(ArtistRequest artistRequest) =>
        new(artistRequest.Id, artistRequest.Name, artistRequest.DateOfBirth);
}
