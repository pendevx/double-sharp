using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Constructs;

namespace Music.CDK;

public class MusicStack : Stack
{
    internal MusicStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        CreateEnvironment(ServiceEnvironment.Development);
        CreateEnvironment(ServiceEnvironment.Production);
    }

    private void CreateEnvironment(ServiceEnvironment serviceEnvironment)
    {
        var bucketName = CreateServiceName("doublesharp-files", serviceEnvironment);
        var filesBucket = new Bucket(this, bucketName, new BucketProps
        {
            BucketName = bucketName,
        });
    }

    private string CreateServiceName(string name, ServiceEnvironment serviceEnvironment) =>
        $"{name}{serviceEnvironment.Suffix}";
}

public class ServiceEnvironment
{
    private ServiceEnvironment(string suffix)
    {
        Suffix = suffix;
    }

    public static ServiceEnvironment Development { get; } = new("-dev");
    public static ServiceEnvironment Production { get; } = new("");

    public string Suffix { get; }
}

// public enum ServiceEnvironment
// {
//     Development,
//     Production
// }
