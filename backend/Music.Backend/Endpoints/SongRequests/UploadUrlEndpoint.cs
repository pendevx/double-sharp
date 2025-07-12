using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data.SongRequests;

namespace Music.Backend.Endpoints.SongRequests;

public record UploadUrlRequest(string Name, Source Source, string Url);

[HttpPost("/song-requests/request/url")]
[RequiresAuthenticated]
public class UploadUrlEndpoint : Ep.Req<UploadUrlRequest>.NoRes
{
    private readonly MusicContext _dbContext;
    private readonly IAuthContext _authContext;

    public UploadUrlEndpoint(MusicContext dbContext, IAuthContext authContext)
    {
        _dbContext = dbContext;
        _authContext = authContext;
    }

    public override async Task HandleAsync(UploadUrlRequest req, CancellationToken ct)
    {
        var account = _authContext.GetAccount()!;

        var songRequest = req.Source switch
        {
            Source.YouTube => SongRequest.CreateYouTubeRequest(req.Name, account, new(req.Url)),
            Source.YouTubeMusic => SongRequest.CreateYouTubeMusicRequest(req.Name, account, new(req.Url)),
            _ => throw new Exception(nameof(req.Source))
        };

        await _dbContext.SongRequests.AddAsync(songRequest, ct);
        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(songRequest.Id, ct);
    }
}
