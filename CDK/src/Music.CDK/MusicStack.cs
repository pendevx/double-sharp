using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Constructs;

namespace Music.CDK;

public class MusicStack : Stack
{
    internal MusicStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var bucket = new Bucket(this, "doublesharp-files-dev", new BucketProps
        {
            BucketName = "doublesharp-files-dev"
        });
    }
}
