using Microsoft.EntityFrameworkCore;
using Music.Backend.EndpointConfigurations;
using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Models.Data.Errors;
using Music.Models.Data.Utils;

// Results<Ok, ForbidHttpResult, NotFound, BadRequest>
using TResponse = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Ok,
    Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult,
    Microsoft.AspNetCore.Http.HttpResults.NotFound,
    Microsoft.AspNetCore.Http.HttpResults.BadRequest
>;

namespace Music.Backend.Endpoints.ArtistRequests;

public record ApproveArtistRequest(int Id);

[HttpPost("/suggest-artists/approve/{id}")]
[NoBody]
[RequiresAuthenticated]
public class ApproveArtistEndpoint : Ep.Req<ApproveArtistRequest>.Res<TResponse>
{
    private readonly MusicContext _dbContext;
    private readonly IAuthContext _authContext;

    public ApproveArtistEndpoint(MusicContext dbContext, IAuthContext authContext)
    {
        _dbContext = dbContext;
        _authContext = authContext;
    }

    public override Task<TResponse> HandleAsync(ApproveArtistRequest req, CancellationToken ct) =>
        ValidateUserHasRoles(_authContext)
            .BindAsync(_ => TryFindArtistAsync(req.Id, ct))
            .TapAsync(artist => artist.RequestInformation.IfSome(requestInformation => requestInformation.Approve()))
            .TapAsync(artist => AddArtistToDbAsync(artist, _dbContext, ct))
            .MatchAsync(
                TResponse (_) => TypedResults.Ok(),
                TResponse (error) => error switch
                {
                    UnauthorizedError => TypedResults.Forbid(),
                    NotFoundError => TypedResults.NotFound(),
                    _ => TypedResults.BadRequest()
                });

    private static ResultType<Account, ResultError> ValidateUserHasRoles(IAuthContext authContext) =>
        authContext.GetAccount_Option()
            .Bind<Account, Account>(account =>
                account.HasAllRoles(RoleName.Admin) ? Option.Some(account) : Option.None<Account>())
            .ToResult<Account, ResultError>(new UnauthorizedError());

    private async Task<ResultType<Artist, ResultError>> TryFindArtistAsync(int id, CancellationToken ct) =>
        (await _dbContext.Artists.FirstOrDefaultAsync(a => a.Id == id, ct))
            .ToOption()
            .ToResult<Artist, ResultError>(new NotFoundError());

    private static async Task<Artist> AddArtistToDbAsync(Artist artist, MusicContext dbContext, CancellationToken ct)
    {
        dbContext.Artists.Update(artist);
        await dbContext.SaveChangesAsync(ct);
        return artist;
    }
}
