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

    public SongsRepository(IAmazonS3 s3Client, GetSongPath generateSongPath, GetBucketName getBucketName)
        : base(s3Client, getBucketName)
    {
        _generateSongPath = generateSongPath;
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

    public Task<string> GetMediaSignedUrl(int id) =>
        S3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest
        {
            BucketName = BucketName,
            Key = _generateSongPath(id),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Protocol = Protocol.HTTP
        });
}
