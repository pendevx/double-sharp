using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using Music.Global.Contracts;
using Music.Models.Data;

namespace Music.Services.DataAccess.AWS;

public sealed class SongsRepository
{
    private readonly IAmazonS3 _s3Client;
    private readonly GetBucketName _getBucketName;
    private readonly GetObjectKey _generateObjectKey;
    private readonly ILogger<SongsRepository> _logger;

    public SongsRepository(IAmazonS3 s3Client, GetObjectKey generateObjectKey, GetBucketName getBucketName, ILogger<SongsRepository> logger)
    {
        _s3Client = s3Client;
        _generateObjectKey = generateObjectKey;
        _getBucketName = getBucketName;
        _logger = logger;
    }

    public Task UploadAsync(Song song, Stream contents)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = _getBucketName(),
            Key = _generateObjectKey(song.Id),
            InputStream = contents,
            ContentType = song.MimeType,
        };

        return new TransferUtility(_s3Client).UploadAsync(uploadRequest);
    }

    public async Task<Stream> DownloadAsync(int id)
    {
        var key = _generateObjectKey(id);

        var request = new GetObjectRequest
        {
            BucketName = _getBucketName(),
            Key = key,
        };

        _logger.LogInformation($"Requested for object '{key}'");
        var response = await _s3Client.GetObjectAsync(request);

        return AmazonS3Util.MakeStreamSeekable(response.ResponseStream);
    }
}
