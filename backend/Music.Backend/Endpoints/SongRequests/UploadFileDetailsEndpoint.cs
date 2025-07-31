using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Models.Data.SongRequests;
using Music.Services.DataAccess.AWS;

namespace Music.Backend.Endpoints.SongRequests;

public record UploadFileDetailsRequest(Guid Id, string Title);

[HttpPost("/song-requests/request/file-details")]
[RequiresAuthenticated]
public class UploadFileDetailsEndpoint : Ep.Req<UploadFileDetailsRequest>.NoRes
{
    private readonly IAuthContext _authContext;
    private readonly MusicContext _dbContext;
    private readonly SongRequestRepository _songRequestRepository;

    public UploadFileDetailsEndpoint(IAuthContext authContext, MusicContext dbContext,
        SongRequestRepository songRequestRepository)
    {
        _authContext = authContext;
        _dbContext = dbContext;
        _songRequestRepository = songRequestRepository;
    }

    public override async Task HandleAsync(UploadFileDetailsRequest req, CancellationToken ct)
    {
        if (await _songRequestRepository.ExistsAsync(req.Id) is not { exists: true } check)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var account = _authContext.GetAccount()!;
        var songRequest = SongRequest.CreateFileRequest(req.Title, account, new(check.key));

        await _dbContext.SongRequests.AddAsync(songRequest, ct);
        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(songRequest.Id, ct);
    }
}
