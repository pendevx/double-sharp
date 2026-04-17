using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Music.Global.Contracts;
using Music.Models.Data;

namespace Music.Services.DataAccess.AWS;

public sealed class SongsRepository : S3Repository
{
    private const string AudioMp3FileName = "audio.mp3";

    public SongsRepository(IAmazonS3 s3Client, GetBucketName getBucketName,
        ITransferUtility transferUtility)
        : base(s3Client, getBucketName, transferUtility)
    {
    }

    public Task UploadAsync(IEnumerable<UploadData> uploadData) =>
        Task.WhenAll(uploadData.Select(toUpload =>
            TransferUtility.UploadAsync(new TransferUtilityUploadRequest
            {
                BucketName = BucketName,
                FilePath = toUpload.File,
                Key = toUpload.Key,
                ContentType = MimeType.InferFromFileName(toUpload.File)
            })
        ));

    public Task<string> GetMediaSignedUrl(int id) =>
        S3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest
        {
            BucketName = BucketName,
            // Key = _generateSongPath(id),
            Key = $"Songs/{id}/{AudioMp3FileName}",
            Expires = DateTime.UtcNow.AddMinutes(5),
            Protocol = Protocol.HTTP
        });
}

public record struct UploadData(string File, string Key);
