using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

var path = Path.Combine(Environment.CurrentDirectory, "config.txt");

if (!File.Exists(path))
{
    File.Create(path);
    Console.WriteLine($"Populate the file {path} before continuing.");
    Console.WriteLine("The first line should contain S3 bucket name.");
    Console.WriteLine("The second line should contain the AWS User ID.");
    return;
}

if (File.ReadAllLines(path) is not { Length: <= 2 } config)
    throw new FileLoadException(path);

var bucketName = config[0];
var userId = config[1];

var client = new AmazonS3Client(RegionEndpoint.APSoutheast2);
var objects = await client.ListObjectsAsync(bucketName);

var tasks = new Task[objects.S3Objects.Count];

for (var i = 0; i < objects.S3Objects.Count; i++)
{
    var sourceKey = objects.S3Objects[i].Key;
    var destinationKey = $"{(!string.IsNullOrWhiteSpace(userId) ? $"{userId}/" : "")}Songs/{sourceKey.Replace($"{userId}/", "")}";

    tasks[i] = Task.Run(async () =>
    {
        // Copy the object to the new key
        await client.CopyObjectAsync(new CopyObjectRequest
        {
            SourceBucket = bucketName,
            SourceKey = sourceKey,
            DestinationBucket = bucketName,
            DestinationKey = destinationKey
        });

        // Delete the original object
        await client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = sourceKey
        });
    });
}

await Task.WhenAll(tasks);
