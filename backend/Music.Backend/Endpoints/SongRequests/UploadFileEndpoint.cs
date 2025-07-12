using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Services.DataAccess.AWS;

namespace Music.Backend.Endpoints.SongRequests;

[HttpPost("/song-requests/request/file")]
[RequiresAuthenticated]
public class UploadFileEndpoint : Ep.NoReq.Res<Guid>
{
    private readonly SongRequestRepository _songRequestRepository;

    public UploadFileEndpoint(IAuthContext authContext, MusicContext dbContext,
        SongRequestRepository songRequestRepository)
    {
        _songRequestRepository = songRequestRepository;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await using var stream = HttpContext.Request.Body;
        var contentType = MimeType.Create(HttpContext.Request.Headers.ContentType.ToString());
        var id = Guid.NewGuid();

        await _songRequestRepository.UploadAsync(id, contentType, stream);

        await SendOkAsync(id, ct);
    }
}
