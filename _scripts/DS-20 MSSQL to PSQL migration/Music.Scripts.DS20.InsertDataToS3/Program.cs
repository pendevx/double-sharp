using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.EntityFrameworkCore;
using MssqlContext = Music.Scripts.DS20.Common.MSSQL.MusicContext;

var path = Path.Combine(Environment.CurrentDirectory, "connectionStrings.txt");

if (!File.Exists(path))
{
    File.Create(path);
    Console.WriteLine($"Populate the file {path} before continuing.");
    Console.WriteLine("The first line should contain the MSSQL connection string.");
    Console.WriteLine("The second line should contain the S3 bucket name.");
    Console.WriteLine("The third line should contain 'true' or 'false' based on whether the script's running for production or not. By default, the value is assumed to be 'false'.");
    return;
}

var config = File.ReadAllLines(path);

if (config.Length < 2)
    throw new FileLoadException(path);

var mssqlOptionsBuilder = new DbContextOptionsBuilder<MssqlContext>();
mssqlOptionsBuilder.UseSqlServer(config[0]);
var bucketName = config[1];
var prefix = config.Length >= 3 ?
    bool.Parse(config[2]) ?
        "" :
        $"{AwsEnvironment.UserId}/" :
    throw new FormatException("The third line should contain 'true' or 'false' based on whether the script's running for production or not. By default, the value is assumed to be 'false'.");

var mssql = new MssqlContext(mssqlOptionsBuilder.Options);

var client = new AmazonS3Client(RegionEndpoint.APSoutheast2);
var fileTransferUtility = new TransferUtility(client);

var songCount = mssql.Songs.Count();
var tasks = new Task[songCount];
var i = 0;

foreach (var song in mssql.Songs)
{
    var uploadRequest = new TransferUtilityUploadRequest
    {
        BucketName = bucketName,
        Key = $"{prefix}{song.Guid}/audio.mp3",
        InputStream = new MemoryStream(song.Contents),
        ContentType = song.MimeType,
    };

    tasks[i++] = fileTransferUtility.UploadAsync(uploadRequest);
}

await Task.WhenAll(tasks);

internal static class AwsEnvironment
{
    private static string? _userId;
    public static string UserId
    {
        get
        {
            if (_userId is not null)
                return _userId;

            var stsClient = new AmazonSecurityTokenServiceClient();
            var userId = stsClient.GetCallerIdentityAsync(new GetCallerIdentityRequest()).Result.UserId;

            _userId = userId;

            return _userId;
        }
    }
}
