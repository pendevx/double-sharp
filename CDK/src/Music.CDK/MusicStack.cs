using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
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

        return vpc;
    }

    private void CreateProductionEnvironment(Vpc vpc)
    {
        var serviceEnvironment = ServiceEnvironment.Production;

        var frontendName = serviceEnvironment.CreateName("frontend-site");
        var frontend = new Bucket(this, frontendName, new BucketProps
        {
            BucketName = "music.pendevx.com",
            WebsiteIndexDocument = "index.html",
            BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions
            {
                BlockPublicAcls = false,
                BlockPublicPolicy = false,
                IgnorePublicAcls = false,
                RestrictPublicBuckets = false,
            }),
        });

        frontend.AddToResourcePolicy(
            new PolicyStatement(new PolicyStatementProps
            {
                Sid = "AllowPublicAccess",
                Effect = Effect.ALLOW,
                Resources = [ $"{frontend.BucketArn}/*" ],
                Actions = [ "s3:GetObject" ],
                Principals = [ new AnyPrincipal() ]
            }));

        Containers.Create(this, serviceEnvironment.CreateName("backend"), vpc);
        Database.Create(this, serviceEnvironment, vpc);
        Cloudfront.Create(this, serviceEnvironment, frontend);
        CicdPipeline.Create(new PipelinePackage(this, serviceEnvironment, frontend));
    }
}
