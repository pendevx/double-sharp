using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.S3;
using Constructs;
using Music.CDK.Services;
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

        Containers.CreateRepository(serviceEnvironment.CreateName("backend"), this, vpc);

        var dbSgName = serviceEnvironment.CreateName("db-sg");
        var dbSg = new SecurityGroup(this, dbSgName, new SecurityGroupProps
        {
            Vpc = vpc,
            AllowAllOutbound = true,
            SecurityGroupName = dbSgName,
        });

        dbSg.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(5432)); // Allow port 5432

        var dbName = serviceEnvironment.CreateName("db").Replace("-", "");
        var database = new DatabaseInstance(this, dbName, new DatabaseInstanceProps
        {
            DatabaseName = dbName,
            Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps { Version = PostgresEngineVersion.VER_17, }),
            Credentials = Credentials.FromGeneratedSecret("superuser", new CredentialsBaseOptions
            {
                SecretName = "doublesharp-database",
                ExcludeCharacters = "\"@",
            }),
            RemovalPolicy = RemovalPolicy.RETAIN,
            InstanceType = InstanceType.Of(InstanceClass.T4G, InstanceSize.MICRO),
            EnablePerformanceInsights = true,
            StorageType = StorageType.GP3,
            AllocatedStorage = 20,
            MaxAllocatedStorage = 20,
            Vpc = vpc,
            PubliclyAccessible = true,
            Port = 5432,
            SecurityGroups = [ dbSg ],
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PUBLIC, },
        });
    }
}
