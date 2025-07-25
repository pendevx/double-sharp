using System.Diagnostics;
using Amazon.S3;
using Amazon.S3.Model;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Models.Data.SongRequests;
using Music.Services.DataAccess.AWS;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace Music.Services.SongRequests;

public class SongRequestService
{
    private readonly IAmazonS3 _s3Client;
    private readonly GetBucketName _getBucketName;
    private readonly GetSongPath _getSongPath;
    private readonly SongsRepository _songsRepository;

    public SongRequestService(IAmazonS3 s3Client, GetBucketName getBucketName, GetSongPath getSongPath,
        SongsRepository songsRepository)
    {
        _s3Client = s3Client;
        _getBucketName = getBucketName;
        _getSongPath = getSongPath;
        _songsRepository = songsRepository;
    }

    public Task ApproveSongRequestAsync(SongRequest request, int newSongId) =>
        request.Url switch
        {
            SongRequestWebUrl => DownloadSongFromUrlAsync(request.RawUrl, newSongId),
            S3Key s3Key => CopySongRequestFileToSongsAsync(s3Key, newSongId),
            _ => throw new UnreachableException(nameof(request.Url))
        };

    private async Task DownloadSongFromUrlAsync(string url, int newSongId)
    {
        var ytdl = new YoutubeDL { OutputFileTemplate = "%(id)s.%(ext)s" };
        var path = await ytdl.RunAudioDownload(url, overrideOptions: new OptionSet
        {
            Cookies = Path.Combine(Environment.CurrentDirectory, "cookies.txt"),
        });

        if (!path.Success)
            throw new Exception(string.Join("\n", path.ErrorOutput));

        try
        {
            var mimeType = MimeType.InferFromFileName(path.Data);
            await using var contents = File.OpenRead(path.Data);
            await _songsRepository.UploadAsync(newSongId, contents, mimeType);
        }
        finally
        {
            File.Delete(path.Data);
        }
    }

    private Task<CopyObjectResponse> CopySongRequestFileToSongsAsync(S3Key key, int newSongId)
    {
        var bucketName = _getBucketName();
        var newSongKey = _getSongPath(newSongId);

        var request = new CopyObjectRequest
        {
            SourceBucket = bucketName,
            SourceKey = key,
            DestinationBucket = bucketName,
            DestinationKey = newSongKey,
        };

        return _s3Client.CopyObjectAsync(request);
    }
}
