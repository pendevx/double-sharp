using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Constructs;
using InstanceType = Amazon.CDK.AWS.EC2.InstanceType;

namespace Music.CDK.Services;

public class Database
{
    public static void Create(Construct scope, ServiceEnvironment serviceEnvironment, Vpc vpc)
    {
        var dbSgName = serviceEnvironment.CreateName("db-sg");
        var dbSg = new SecurityGroup(scope, dbSgName, new SecurityGroupProps
        {
            Vpc = vpc,
            AllowAllOutbound = true,
            SecurityGroupName = dbSgName,
        });

        dbSg.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(5432)); // Allow port 5432

        var dbName = serviceEnvironment.CreateName("db").Replace("-", "");
        var database = new DatabaseInstance(scope, dbName, new DatabaseInstanceProps
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
