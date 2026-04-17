using Amazon.S3;
using Amazon.S3.Transfer;
using Music.Global.Contracts;
using Music.Models.Data;

namespace Music.Services.DataAccess.AWS;

public class SongRequestRepository : S3Repository
{
    private const string AudioMp3FileName = "audio.mp3";

    public SongRequestRepository(IAmazonS3 s3Client, GetBucketName getBucketName, ITransferUtility transferUtility)
        : base(s3Client, getBucketName, transferUtility)
    {
    }

    public Task UploadAsync(Guid id, MimeType contentType, Stream contents) =>
        UploadObjectAsync($"SongRequests/{id}/{AudioMp3FileName}", contents, contentType);

    public async Task<(bool exists, string key)> ExistsAsync(Guid id)
    {
        var key = $"SongRequests/{id}/{AudioMp3FileName}";
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
}
