using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Services.DataAccess.AWS;

namespace Music.Backend.Endpoints.SongRequests;

[HttpPost("/song-requests/request/file")]
[RequiresAuthenticated]
public class UploadFileEndpoint : Ep.NoReq.Res<Guid>
{
    private readonly RequiresPermission _requiresPermission;
    private readonly SongRequestRepository _songRequestRepository;

    public UploadFileEndpoint(IAuthContext authContext, MusicContext dbContext, RequiresPermission requiresPermission,
        SongRequestRepository songRequestRepository)
    {
        _requiresPermission = requiresPermission;
        _songRequestRepository = songRequestRepository;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!_requiresPermission(RoleName.Admin))
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await using var stream = HttpContext.Request.Body;
        var contentType = HttpContext.Request.Headers.ContentType.ToString();
        var id = Guid.NewGuid();

        await _songRequestRepository.UploadAsync(id, contentType, stream);

        await SendOkAsync(id, ct);
    }
}
