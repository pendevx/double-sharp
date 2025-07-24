using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Music.Backend.EndpointConfigurations;
using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Services.SongRequests;

namespace Music.Backend.Endpoints.SongRequests.Admin;

public record ApproveSongRequestRequest(int Id);

[HttpPost("/song-requests/approve/{id}")]
[NoBody]
[RequiresAuthenticated]
public class ApproveSongRequestEndpoint : Ep.Req<ApproveSongRequestRequest>.NoRes
{
    private readonly MusicContext _dbContext;
    private readonly SongRequestService _songRequestService;
    private readonly RequiresPermission _requiresPermission;

    public ApproveSongRequestEndpoint(MusicContext dbContext, SongRequestService songRequestService,
        RequiresPermission requiresPermission)
    {
        _dbContext = dbContext;
        _songRequestService = songRequestService;
        _requiresPermission = requiresPermission;
    }

    public override async Task HandleAsync(ApproveSongRequestRequest req, CancellationToken ct)
    {
        if (!_requiresPermission(RoleName.Admin))
            throw new UnauthorizedAccessException();

        if (await _dbContext.SongRequests.FirstOrDefaultAsync(sr => sr.Id == req.Id, ct) is not { } songRequest)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        var newSong = songRequest.Approve();

        await _dbContext.Songs.AddAsync(newSong, ct);
        await _dbContext.SaveChangesAsync(ct);

        await _songRequestService.ApproveSongRequestAsync(songRequest, newSong.Id);
        await transaction.CommitAsync(ct);

        await SendOkAsync(null, ct);
    }
}
