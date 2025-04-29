using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.S3;
using Constructs;
using InstanceType = Amazon.CDK.AWS.EC2.InstanceType;

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
                    SubnetType = SubnetType.PRIVATE_WITH_EGRESS,
                    CidrMask = 24,
                },
                new SubnetConfiguration
                {
                    Name = publicSubnetName,
                    SubnetType = SubnetType.PUBLIC,
                    CidrMask = 24,
                    MapPublicIpOnLaunch = true,
                }
            ]
        });

        return vpc;
    }

    private void CreateProductionEnvironment(Vpc vpc)
    {
        var serviceEnvironment = ServiceEnvironment.Production;

        var repositoryName = serviceEnvironment.CreateName("frontend");
        var repository = new Repository(this, repositoryName, new RepositoryProps
        {
            RepositoryName = repositoryName,
            RemovalPolicy = RemovalPolicy.DESTROY,
            EmptyOnDelete = true,
        });

        var dbName = serviceEnvironment.CreateName("db").Replace("-", "");
        var database = new DatabaseInstance(this, dbName, new DatabaseInstanceProps
        {
            DatabaseName = dbName,
            Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps
            {
                Version = PostgresEngineVersion.VER_17,
            }),
            Credentials = Credentials.FromGeneratedSecret("superuser", new CredentialsBaseOptions
            {
                SecretName = "database",
                ExcludeCharacters = "\"@",
            }),
            RemovalPolicy = RemovalPolicy.RETAIN,
            InstanceType = InstanceType.Of(InstanceClass.T4G, InstanceSize.MICRO),
            EnablePerformanceInsights = true,
            StorageType = StorageType.GP3,
            AllocatedStorage = 20,
            MaxAllocatedStorage = 20,
            Vpc = vpc,
            PubliclyAccessible = false,
        });

        // var frontendName = serviceEnvironment.CreateName("frontend-s3");
        // var frontend = new Bucket(this, frontendName, new BucketProps
        // {
        //
        // });
    }
}
