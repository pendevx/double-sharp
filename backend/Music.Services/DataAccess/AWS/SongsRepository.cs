using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using Music.Global.Contracts;
using Music.Models.Data;

namespace Music.Services.DataAccess.AWS;

public sealed class SongsRepository : S3Repository
{
    private readonly GetSongPath _generateSongPath;
    private readonly ILogger<SongsRepository> _logger;

    public SongsRepository(IAmazonS3 s3Client, GetSongPath generateSongPath, GetBucketName getBucketName,
        ILogger<SongsRepository> logger)
        : base(s3Client, getBucketName)
    {
        _generateSongPath = generateSongPath;
        _logger = logger;
    }

    public Task UploadAsync(int id, Stream contents, MimeType contentType) =>
        UploadObjectAsync(_generateSongPath(id), contents, contentType);

    public async Task<(bool exists, string key)> ExistsAsync(int id)
    {
        var key = _generateSongPath(id);
        try
        {
            await CheckObjectExistsAsync(key);
            return (true, key);
        }
        catch (AmazonS3Exception)
        {
            return (false, "");
        }
    }

    public async Task<(Stream, MimeType)> DownloadAsync(int id)
    {
        var key = _generateSongPath(id);

        var request = new GetObjectRequest
        {
            BucketName = BucketName,
            Key = key,
        };

        _logger.LogInformation($"Requested for object '{key}'");
        var audioContents = await S3Client.GetObjectAsync(request);
        var mimeType = await S3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
        {
            BucketName = BucketName,
            Key = key,
        });

        return (AmazonS3Util.MakeStreamSeekable(audioContents.ResponseStream), MimeType.Create(mimeType.Headers.ContentType));
    }
}
