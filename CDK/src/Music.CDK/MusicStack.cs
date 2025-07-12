using System.IO;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SQS;
using Constructs;
using Music.CDK.Services;

namespace Music.CDK;

public class MusicStack : Stack
{
    internal MusicStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        CreateCommonEnvironment(ServiceEnvironment.Development);
        var productionVpc = CreateCommonEnvironment(ServiceEnvironment.Production);

        CreateProductionEnvironment(productionVpc);
    }

    private Vpc CreateCommonEnvironment(ServiceEnvironment serviceEnvironment)
    {
        var bucketName = serviceEnvironment.CreateName("files");

        var filesBucket = new Bucket(this, bucketName, new BucketProps
        {
            BucketName = bucketName,
        });

        var vpcName = serviceEnvironment.CreateName("vpc");
        var privateSubnetName = serviceEnvironment.CreateName("subnet-private");
        var publicSubnetName = serviceEnvironment.CreateName("subnet-public");

        var vpc = new Vpc(this, vpcName, new VpcProps
        {
            VpcName = vpcName,
            MaxAzs = 2,
            IpProtocol = IpProtocol.IPV4_ONLY,
            IpAddresses = IpAddresses.Cidr("192.168.0.0/16"),
            CreateInternetGateway = true,
            RestrictDefaultSecurityGroup = false,
            NatGateways = 0,
            SubnetConfiguration = [
                new SubnetConfiguration
                {
                    Name = privateSubnetName,
                    SubnetType = SubnetType.PUBLIC, // should be private_with_egress
                    CidrMask = 24,
                },
                new SubnetConfiguration
                {
                    Name = publicSubnetName,
                    SubnetType = SubnetType.PUBLIC,
                    CidrMask = 24,
                }
            ],
        });

        var queueName = serviceEnvironment.CreateName("approved-song-requests");
        var queue = new Queue(this, queueName, new QueueProps { QueueName = queueName });

        var code = Utils.ReadCodeFromEmbeddedResource("SQS.DownloadSongRequests.Dockerfile");

        var tempDir = Path.Combine(Path.GetTempPath(), "double-sharp-dockerfile-temp");
        Directory.CreateDirectory(tempDir);

        var dockerfilePath = Path.Combine(tempDir, "Dockerfile");
        File.WriteAllText(dockerfilePath, code);

        var songRequestsProcessorName = serviceEnvironment.CreateName("song-request-processor");
        var songRequestsProcessor = new Function(this, songRequestsProcessorName, new FunctionProps
        {
            Runtime = Runtime.FROM_IMAGE,
            Code = Code.FromDockerBuild(tempDir, new DockerBuildAssetOptions { File = "Dockerfile" }),
            Handler = Handler.FROM_IMAGE,
        });

        songRequestsProcessor.AddEventSource(new SqsEventSource(queue, new SqsEventSourceProps
        {
            BatchSize = 1,
            Enabled = true,
        }));

        return vpc;
    }

    private void CreateProductionEnvironment(Vpc vpc)
    {
        var serviceEnvironment = ServiceEnvironment.Production;
        var domainName = "music.pendevx.com";

        var frontendName = serviceEnvironment.CreateName("frontend-site");
        var frontend = new Bucket(this, frontendName, new BucketProps
        {
            BucketName = domainName,
            WebsiteIndexDocument = "index.html",
            BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions
            {
                BlockPublicAcls = false,
                BlockPublicPolicy = false,
                IgnorePublicAcls = false,
                RestrictPublicBuckets = false,
            }),
            AutoDeleteObjects = true,
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        frontend.AddToResourcePolicy(
            new PolicyStatement(new PolicyStatementProps
            {
                Sid = "AllowPublicAccess",
                Effect = Effect.ALLOW,
                Resources = [ $"{frontend.BucketArn}/*" ],
                Actions = [ "s3:GetObject" ],
                Principals = [ new AnyPrincipal() ],
            }));

        var distribution = Cloudfront.Create(this, serviceEnvironment, frontend);
        Domains.CreateAliasForRoot(this, serviceEnvironment, distribution);

        var connectionStringSecret = Database.Create(this, serviceEnvironment, vpc);
        var (repository, service) = Containers.Create(this, serviceEnvironment, vpc, connectionStringSecret);

        new CicdPipeline(this, serviceEnvironment, repository).Create(frontend, service.Service);
    }
}
