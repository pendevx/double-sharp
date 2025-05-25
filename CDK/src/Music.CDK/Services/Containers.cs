using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancing;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.Route53.Targets;
using Constructs;
using ApplicationLoadBalancerProps = Amazon.CDK.AWS.ElasticLoadBalancingV2.ApplicationLoadBalancerProps;
using Cluster = Amazon.CDK.AWS.ECS.Cluster;
using ClusterProps = Amazon.CDK.AWS.ECS.ClusterProps;
using HealthCheck = Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck;
using Secret = Amazon.CDK.AWS.SecretsManager.Secret;

namespace Music.CDK.Services;

public class Containers
{
    public static (Repository, ApplicationLoadBalancedFargateService) Create(Construct scope, ServiceEnvironment serviceEnvironment, Vpc vpc, Secret dbConnectionString)
    {
        var baseName = serviceEnvironment.CreateName("backend");
        var clusterName = baseName + nameof(Cluster);
        var repositoryName = baseName + nameof(Repository).ToLower();
        var containerDefinitionName = baseName + nameof(ContainerDefinition);
        var taskDefinitionName = baseName + nameof(TaskDefinition);
        var serviceName = baseName + nameof(FargateService);
        var backendSgName = baseName + nameof(SecurityGroup);
        var loadBalancerName = baseName + nameof(LoadBalancer);

        var cluster = new Cluster(scope, clusterName, new ClusterProps
        {
            Vpc = vpc,
            ClusterName = clusterName,
        });

        var repo = new Repository(scope, repositoryName, new RepositoryProps
        {
            RepositoryName = repositoryName,
            RemovalPolicy = RemovalPolicy.DESTROY,
            EmptyOnDelete = true,
        });

        var taskDefinition = new FargateTaskDefinition(scope, taskDefinitionName, new FargateTaskDefinitionProps
        {
            Cpu = 256,
            Family = taskDefinitionName,
            RuntimePlatform = new RuntimePlatform { OperatingSystemFamily = OperatingSystemFamily.LINUX, },
            EphemeralStorageGiB = 21,
            MemoryLimitMiB = 512,
        });

        var containerDefinition = new ContainerDefinition(scope, containerDefinitionName, new ContainerDefinitionProps
        {
            ContainerName = containerDefinitionName,
            Cpu = 256,
            MemoryLimitMiB = 512,
            Image = ContainerImage.FromEcrRepository(repo),
            PortMappings = [
                new PortMapping
                {
                    ContainerPort = 8080,
                    HostPort = 8080,
                }
            ],
            TaskDefinition = taskDefinition,
            Logging = LogDriver.AwsLogs(new AwsLogDriverProps { StreamPrefix = "doublesharp-backend", }),
        });

        containerDefinition.AddSecret("DOUBLESHARP_DB_CONNECTION_STRING", Amazon.CDK.AWS.ECS.Secret.FromSecretsManager(dbConnectionString));

        var backendSg = new SecurityGroup(scope, backendSgName, new SecurityGroupProps
        {
            Vpc = vpc,
            AllowAllOutbound = true,
            SecurityGroupName = backendSgName,
        });
        backendSg.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(8080)); // Allow port 8080

        var service = new ApplicationLoadBalancedFargateService(scope, serviceName, new ApplicationLoadBalancedFargateServiceProps
        {
            ServiceName = serviceName,
            TaskDefinition = taskDefinition,
            Cluster = cluster,
            DesiredCount = 1,
            TaskSubnets = new SubnetSelection { SubnetType = SubnetType.PUBLIC },
            AssignPublicIp = true,
            SecurityGroups = [ backendSg ],
            LoadBalancer = new ApplicationLoadBalancer(scope, loadBalancerName, new ApplicationLoadBalancerProps
            {
                Vpc = vpc,
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PUBLIC, OnePerAz = true },
                LoadBalancerName = loadBalancerName,
                InternetFacing = true,
            }),
        });

        service.TargetGroup.ConfigureHealthCheck(new HealthCheck { Path = "/healthcheck.html" });

        Domains.CreateAliasForService(scope, serviceEnvironment, ServicesWithDomains.WebBackend,
            serviceEnvironment.CreateName("backend"), new LoadBalancerTarget(service?.LoadBalancer));

        return (repo, service);
    }
}
