using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Music.Models.Data;
using Music.Models.Data.DbContexts;

namespace Music.Backend.Endpoints.SongRequests;

public record GetSongRequests(int Page);
public record GetSongRequestsResponse(int Id, string Name, string Source, string SourceUrl, string UploadedBy);

[AllowAnonymous]
[HttpGet("/song-requests/{page}")]
public class GetSongRequestsEndpoint : Endpoint<GetSongRequests>
{

    private readonly MusicContext _dbContext;

    public GetSongRequestsEndpoint(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task HandleAsync(GetSongRequests req, CancellationToken ct)
    {
        const int pageSize = 50;

        var response = _dbContext.SongRequests
            .AsNoTracking()
            .Where(sr => sr.RequestStatus == RequestStatus.Pending)
            .Skip((req.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(sr => new GetSongRequestsResponse(
                sr.Id,
                sr.Name,
                sr.Source,
                sr.SourceUrl,
                sr.UploaderAccount.DisplayName
            ))
            .ToList();

        await SendOkAsync(response, ct);
    }
}
