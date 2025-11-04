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
        CreateCommonEnvironment(ServiceEnvironment.Production);

        CreateProductionEnvironment();
    }

    private void CreateCommonEnvironment(ServiceEnvironment serviceEnvironment)
    {
        var bucketName = serviceEnvironment.CreateName("files");

        var filesBucket = new Bucket(this, bucketName, new BucketProps
        {
            BucketName = bucketName,
        });

        var cookiesParameter = SsmParameters.CookiesTxt(this, serviceEnvironment);

        // var vpcName = serviceEnvironment.CreateName("vpc");
        // var privateSubnetName = serviceEnvironment.CreateName("subnet-private");
        // var publicSubnetName = serviceEnvironment.CreateName("subnet-public");
        //
        // var vpc = new Vpc(this, vpcName, new VpcProps
        // {
        //     VpcName = vpcName,
        //     MaxAzs = 2,
        //     IpProtocol = IpProtocol.IPV4_ONLY,
        //     IpAddresses = IpAddresses.Cidr("192.168.0.0/16"),
        //     CreateInternetGateway = true,
        //     RestrictDefaultSecurityGroup = false,
        //     NatGateways = 0,
        //     SubnetConfiguration = [
        //         new SubnetConfiguration
        //         {
        //             Name = privateSubnetName,
        //             SubnetType = SubnetType.PUBLIC, // should be private_with_egress
        //             CidrMask = 24,
        //         },
        //         new SubnetConfiguration
        //         {
        //             Name = publicSubnetName,
        //             SubnetType = SubnetType.PUBLIC,
        //             CidrMask = 24,
        //         }
        //     ],
        // });
        //
        // return vpc;
    }

    private void CreateProductionEnvironment()
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

        var otherArtifactsName = serviceEnvironment.CreateName("other-artifacts");
        var otherArtifacts = new Bucket(this, otherArtifactsName, new BucketProps
        {
            BucketName = otherArtifactsName,
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        // var serviceDiscoveryNamespace = ServiceDiscovery.Create(this, serviceEnvironment, vpc);
        // ApiGateway.Create(this, serviceEnvironment, vpc, otherArtifacts, otherArtifactsName); // adds an access policy to the S3 bucket

        var distribution = Cloudfront.Create(this, serviceEnvironment, frontend);
        Domains.CreateAliasForRoot(this, serviceEnvironment, distribution);

        // var connectionStringSecret = Database.Create(this, serviceEnvironment, vpc);
        var repository = Containers.Create(this, serviceEnvironment);

        // new CicdPipeline(this, serviceEnvironment, repository).Create(frontend, service);
    }
}
