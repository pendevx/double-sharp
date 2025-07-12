using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;
using Music.Models.Data.SongRequests;

namespace Music.Backend.Endpoints.SongRequests.Admin;

public record GetSongRequests(int Page);
public record GetSongRequestsResponse(int Id, string Name, string Source, string SourceUrl, string UploadedBy);

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
            .Where(sr => sr.Status == RequestStatus.Pending)
            .Skip((req.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(sr => new GetSongRequestsResponse(
                sr.Id,
                sr.Name,
                sr.Source.ToString(),
                sr.Url,
                sr.Uploader.DisplayName
            ))
            .ToList();

        await SendOkAsync(response, ct);
    }
}
