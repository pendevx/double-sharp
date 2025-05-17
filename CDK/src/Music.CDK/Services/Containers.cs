using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Constructs;
using ApplicationListenerProps = Amazon.CDK.AWS.ElasticLoadBalancingV2.ApplicationListenerProps;
using ApplicationLoadBalancerProps = Amazon.CDK.AWS.ElasticLoadBalancingV2.ApplicationLoadBalancerProps;
using Cluster = Amazon.CDK.AWS.ECS.Cluster;
using ClusterProps = Amazon.CDK.AWS.ECS.ClusterProps;
using Protocol = Amazon.CDK.AWS.ECS.Protocol;

namespace Music.CDK.Services;

public class Containers
{
    public static (Repository, BaseService) Create(Construct scope, string baseName, Vpc vpc)
    {
        var clusterName = baseName + nameof(Cluster);
        var repositoryName = baseName + nameof(Repository).ToLower();
        var containerDefinitionName = baseName + nameof(ContainerDefinition);
        var taskDefinitionName = baseName + nameof(TaskDefinition);
        var serviceName = baseName = nameof(FargateService);

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
                    ContainerPort = 5000,
                    HostPort = 5000,
                }
            ],
            TaskDefinition = taskDefinition,
        });

        var service = new FargateService(scope, serviceName, new FargateServiceProps
        {
            ServiceName = serviceName,
            TaskDefinition = taskDefinition,
            Cluster = cluster,
            DesiredCount = 0,
            VpcSubnets = new SubnetSelection
            {
                SubnetType = SubnetType.PUBLIC,
            },
            AssignPublicIp = true,
        });

        return (repo, service);
    }
}
