using System.Security.Authentication;
using Music.Commands.Audio;
using Music.Global.Contracts;
using Music.Models.Data.DbContexts;
using Music.Models.Data;

namespace Music.CommandHandlers.Audio;

public class UploadSongByUrlHandler : IBaseCommandHandler<UploadSongByUrlCommand>
{
    private readonly MusicContext _dbContext;
    private readonly IAuthContext _authContext;

    public UploadSongByUrlHandler(MusicContext dbContext, IAuthContext authContext)
    {
        _dbContext = dbContext;
        _authContext = authContext;
    }

    public void Execute(UploadSongByUrlCommand command)
    {
        var account = _authContext.GetAccount();

        if (account is null)
            throw new AuthenticationException("no.");

        _dbContext.SongRequests.Add(new SongRequest
        {
            Name = command.Title,
            UploaderAccount = account,
            RequestStatus = RequestStatus.Pending,
            Source = command.Source.ToString(),
            SourceUrl = GetSourceUrl(command.Url, command.Source),
        });

        _dbContext.SaveChanges();
    }

    private static string GetSourceUrl(string urlSuffix, UploadSongSource source)
    {
        return source switch
        {
            UploadSongSource.YouTube => "https://www.youtube.com/watch?v=",
            UploadSongSource.YouTubeMusic => "https://music.youtube.com/watch?v=",
            UploadSongSource.Soundcloud => "https://soundcloud.com/",
            _ => throw new Exception("Terrible, terrible value.")
        } + urlSuffix;
    }
}
