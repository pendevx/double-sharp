using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Constructs;
using Cluster = Amazon.CDK.AWS.ECS.Cluster;
using ClusterProps = Amazon.CDK.AWS.ECS.ClusterProps;

namespace Music.CDK.Services;

public class Containers
{
    public static void Create(Construct scope, string baseName, Vpc vpc)
    {
        var clusterName = baseName + nameof(Cluster);
        var repositoryName = baseName + nameof(Repository).ToLower();
        var containerDefinitionName = baseName + nameof(ContainerDefinition);
        var taskDefinitionName = baseName + nameof(TaskDefinition);

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
                    ContainerPort = 80,
                    HostPort = 80,
                }
            ],
            TaskDefinition = taskDefinition,
        });
    }
}
