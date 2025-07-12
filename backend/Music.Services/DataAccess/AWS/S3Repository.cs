using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Music.Global.Contracts;

namespace Music.Services.DataAccess.AWS;

public abstract class S3Repository
{
    protected readonly IAmazonS3 S3Client;
    protected readonly string BucketName;

    protected S3Repository(IAmazonS3 s3Client, GetBucketName getBucketName)
    {
        S3Client = s3Client;
        BucketName = getBucketName();
    }

    protected async Task UploadObjectAsync(string key, Stream contents, string contentType)
    {
        var songRequestExists = await CheckObjectExistsAsync(key);

        if (songRequestExists.exists)
            throw new Exception(nameof(key));

        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = BucketName,
            Key = key,
            InputStream = contents,
            ContentType = contentType,
        };

        await new TransferUtility(S3Client).UploadAsync(uploadRequest);
    }

    protected async Task<(bool exists, string key)> CheckObjectExistsAsync(string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = BucketName,
                Key = key,
            };

            await S3Client.GetObjectMetadataAsync(request);
            return (true, key);
        }
        catch (AmazonS3Exception)
        {
            return (false, "");
        }
    }
}
