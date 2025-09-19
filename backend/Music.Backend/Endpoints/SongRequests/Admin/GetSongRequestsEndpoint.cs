using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Models.Data.Requests;

namespace Music.Backend.Endpoints.SongRequests.Admin;

public record GetSongRequests(int Page);
public record GetSongRequestsResponse(int Id, string Name, string Source, string SourceUrl, string UploadedBy);

[HttpGet("/song-requests/{page}")]
public class GetSongRequestsEndpoint : Endpoint<GetSongRequests>
{
    private readonly MusicContext _dbContext;
    private readonly IAuthContext _authContext;

    public GetSongRequestsEndpoint(MusicContext dbContext, IAuthContext authContext)
    {
        _dbContext = dbContext;
        _authContext = authContext;
    }

    public override async Task HandleAsync(GetSongRequests req, CancellationToken ct)
    {
        var account = _authContext.GetAccount();

        if (account is null || !account.HasAllRoles(RoleName.Admin))
            throw new UnauthorizedAccessException();

        const int pageSize = 50;

        var response = _dbContext.SongRequests
            .AsNoTracking()
            .Where(sr => sr.Status == RequestStatus.Pending)
            .Skip((req.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(sr => new GetSongRequestsResponse(
                sr.Id,
                sr.Name,
                sr.Source.ToString(),
                sr.RawUrl,
                sr.Uploader.DisplayName
            ))
            .ToList();

        await SendOkAsync(response, ct);
    }
}
