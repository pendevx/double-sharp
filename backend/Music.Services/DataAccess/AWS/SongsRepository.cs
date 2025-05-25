using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using Music.Models.Data;

namespace Music.Services.DataAccess.AWS;

public sealed class SongsRepository
{
    private const string AudioMp3FileName = "audio.mp3";

    private readonly IAmazonS3 _s3Client;
    private readonly AwsEnvironment _awsEnvironment;
    private readonly string _bucketName;
    private readonly ILogger<SongsRepository> _logger;

    public SongsRepository(IAmazonS3 s3Client, AwsEnvironment awsEnvironment, string bucketName, ILogger<SongsRepository> logger)
    {
        _s3Client = s3Client;
        _awsEnvironment = awsEnvironment;
        _bucketName = bucketName;
        _logger = logger;
    }

    public void Upload(Song song, Stream contents)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = _bucketName,
            Key = $"{_awsEnvironment.UserId}{song.Id}/{AudioMp3FileName}",
            InputStream = contents,
            ContentType = song.MimeType,
        };
    }

    public async Task<Stream> Download(int id)
    {
        var key = $"{_awsEnvironment.UserId}/{id}/{AudioMp3FileName}";

        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
        };

        _logger.LogInformation($"Requested for object '{key}'");
        var response = await _s3Client.GetObjectAsync(request);

        return AmazonS3Util.MakeStreamSeekable(response.ResponseStream);
    }
}
