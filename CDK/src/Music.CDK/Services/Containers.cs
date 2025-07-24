using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancing;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.SecretsManager;
using Constructs;
using ApplicationLoadBalancerProps = Amazon.CDK.AWS.ElasticLoadBalancingV2.ApplicationLoadBalancerProps;
using Cluster = Amazon.CDK.AWS.ECS.Cluster;
using ClusterProps = Amazon.CDK.AWS.ECS.ClusterProps;
using HealthCheck = Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck;
using Secret = Amazon.CDK.AWS.SecretsManager.Secret;

namespace Music.CDK.Services;

public class Containers
{
    public static (Repository, ApplicationLoadBalancedFargateService) Create(Construct scope, ServiceEnvironment serviceEnvironment,
        Vpc vpc, Secret dbConnectionString)
    {
        var baseName = serviceEnvironment.CreateName("backend");
        var clusterName = baseName + nameof(Cluster);
        var repositoryName = baseName + nameof(Repository).ToLower();
        var containerName = $"{baseName}-container";
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

        var taskRoleName = $"{taskDefinitionName}-taskrole";
        var taskRole = new Role(scope, taskRoleName, new RoleProps
        {
            RoleName = taskRoleName,
            Description = "Allow the ECS task to perform read/write operations on the S3 bucket.",
            ManagedPolicies = [ ManagedPolicy.FromAwsManagedPolicyName("AmazonS3FullAccess") ],
            AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com"),
        });

        taskRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
        {
            Actions = [ "ssm:GetParameter" ],
            Resources = [ $"arn:aws:ssm:{Aws.REGION}:{Aws.ACCOUNT_ID}:parameter/*" ],
            Effect = Effect.ALLOW,
        }));

        var taskDefinition = new FargateTaskDefinition(scope, taskDefinitionName, new FargateTaskDefinitionProps
        {
            Cpu = 256,
            Family = taskDefinitionName,
            RuntimePlatform = new RuntimePlatform { OperatingSystemFamily = OperatingSystemFamily.LINUX, },
            EphemeralStorageGiB = 21,
            MemoryLimitMiB = 512,
            TaskRole = taskRole,
        });

        var backendSg = new SecurityGroup(scope, backendSgName, new SecurityGroupProps
        {
            Vpc = vpc,
            AllowAllOutbound = true,
            SecurityGroupName = backendSgName,
        });
        backendSg.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(8080)); // Allow port 8080

        var backendCertificate = Domains.GenerateUniqueCertificate(scope, serviceEnvironment, serviceName, ServicesWithDomains.WebBackend);

        var secrets = GetLoginSecrets(scope, serviceEnvironment);

        var containerDefinition = new ContainerDefinition(scope, containerName, new ContainerDefinitionProps
        {
            ContainerName = containerName,
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

        if (secrets is not null)
        {
            var (emailSecret, passwordSecret) = secrets.Value;
            containerDefinition.AddSecret("DOUBLESHARP_EMAIL", Amazon.CDK.AWS.ECS.Secret.FromSecretsManager(emailSecret));
            containerDefinition.AddSecret("DOUBLESHARP_PASSWORD", Amazon.CDK.AWS.ECS.Secret.FromSecretsManager(passwordSecret));
        }

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
            Certificate = backendCertificate,
            EnableExecuteCommand = true,
        });

        // Override the desired count to 0
        var cfnService = service.Service.Node.DefaultChild as CfnService;
        if (cfnService is not null && secrets is null)
        {
            cfnService.DesiredCount = 0;
        }

        service.TargetGroup.ConfigureHealthCheck(new HealthCheck { Path = "/healthcheck.html" });

        Domains.CreateAliasForService(scope, serviceEnvironment, ServicesWithDomains.WebBackend,
            serviceEnvironment.CreateName("backend"), new LoadBalancerTarget(service?.LoadBalancer));

        return (repo, service);
    }

    private const string LoginsSetupFlagName = "LOGINS_SETUP";
    public static bool IsLoginsSetup(Construct scope) =>
        (System.Environment.GetEnvironmentVariable(LoginsSetupFlagName) ?? scope.Node.TryGetContext(LoginsSetupFlagName))
        as string == "true";

    private static (ISecret, ISecret)? GetLoginSecrets(Construct scope, ServiceEnvironment serviceEnvironment)
    {
        var emailSecretName = serviceEnvironment.CreateName("login-email");
        var passwordSecretName = serviceEnvironment.CreateName("login-password");

        var emailSecret = new Secret(scope, emailSecretName, new SecretProps
        {
            SecretName = emailSecretName,
        });

        var passwordSecret = new Secret(scope, passwordSecretName, new SecretProps
        {
            SecretName = passwordSecretName,
        });

        if (IsLoginsSetup(scope)) return (emailSecret, passwordSecret);

        _ = new CfnOutput(scope, serviceEnvironment.CreateName("logins-setup-warning-email"), new CfnOutputProps
        {
            Description = "Instructions to set up the login secrets",
            Value = $"""
                     Run this to set the login secrets: aws secretsmanager put-secret-value --secret-id {emailSecretName} --secret-string "<email>"
                     """
        });

        _ = new CfnOutput(scope, serviceEnvironment.CreateName("logins-setup-warning-password"), new CfnOutputProps
        {
            Description = "Instructions to set up the login secrets",
            Value = $"""
                     Run this to set the login secrets: aws secretsmanager put-secret-value --secret-id {passwordSecretName} --secret-string "<password>"
                     """
        });

        return null;

    }
}
