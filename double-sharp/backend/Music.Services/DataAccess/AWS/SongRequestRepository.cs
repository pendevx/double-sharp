using Amazon.S3;
using Music.Global.Contracts;
using Music.Models.Data;

namespace Music.Services.DataAccess.AWS;

public class SongRequestRepository : S3Repository
{
    private readonly CreateSongRequestPath _createSongRequestPath;

    public SongRequestRepository(IAmazonS3 s3Client, GetBucketName getBucketName,
        CreateSongRequestPath createSongRequestPath)
        : base(s3Client, getBucketName)
    {
        _createSongRequestPath = createSongRequestPath;
    }

    public Task UploadAsync(Guid id, MimeType contentType, Stream contents) =>
        UploadObjectAsync(_createSongRequestPath(id), contents, contentType);

    public async Task<(bool exists, string key)> ExistsAsync(Guid id)
    {
        var key = _createSongRequestPath(id);
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
