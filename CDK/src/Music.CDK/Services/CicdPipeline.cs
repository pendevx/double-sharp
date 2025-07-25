using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SecretsManager;
using Constructs;
using Environment = System.Environment;
using Secret = Amazon.CDK.AWS.SecretsManager.Secret;
using StageProps = Amazon.CDK.AWS.CodePipeline.StageProps;

namespace Music.CDK.Services;

public class CicdPipeline
{
    private readonly Construct _scope;
    private readonly ServiceEnvironment _serviceEnvironment;
    private readonly Repository _repository;

    public const string GithubOauthFlagName = "GITHUB_OAUTH_ENABLED";

    public CicdPipeline(Construct scope, ServiceEnvironment serviceEnvironment, Repository repository)
    {
        _scope = scope;
        _serviceEnvironment = serviceEnvironment;
        _repository = repository;
    }

    public bool IsOauthTokenEnabled() =>
        (Environment.GetEnvironmentVariable(GithubOauthFlagName) ?? _scope.Node.TryGetContext(GithubOauthFlagName))
        as string == "true";

    public void Create(Bucket frontendDeployTarget, BaseService backendDeployTarget)
    {
        var artifactsBucketName = _serviceEnvironment.CreateName("cicd-artifacts");
        var artifactsBucket = new Bucket(_scope, artifactsBucketName, new BucketProps
        {
            BucketName = artifactsBucketName,
            RemovalPolicy = RemovalPolicy.DESTROY,
            AutoDeleteObjects = true,
        });

        var (pipelineRole, buildServiceRole) = GetIamRoles();

        var sourceAction = GetSourceCode(out var sourceCode);

        if (sourceAction is null) // The GithubOauthToken secret needs to be populated
            return;

        var buildUIAction = BuildWebUI(sourceCode, out var uiArtifacts);
        var buildBackendAction = BuildWebBackend(sourceCode, buildServiceRole,
            backendDeployTarget.TaskDefinition.DefaultContainer?.ContainerName, out var backendArtifacts);

        var deployUIAction = DeployWebUI(uiArtifacts, frontendDeployTarget);
        var deployBackendAction = DeployWebBackend(backendArtifacts, backendDeployTarget);

        var pipelineName = _serviceEnvironment.CreateName("cicd");
        var pipeline = new Pipeline(_scope, pipelineName, new PipelineProps
        {
            PipelineName = pipelineName,
            Stages = [
                new StageProps
                {
                    StageName = "Source",
                    Actions = [ sourceAction ],
                },
                new StageProps
                {
                    StageName = "Build",
                    Actions = [ buildUIAction, buildBackendAction ],
                },
                new StageProps
                {
                    StageName = "Deploy",
                    Actions = [ deployUIAction, deployBackendAction ],
                }
            ],
            ArtifactBucket = artifactsBucket,
            Role = pipelineRole,
            Triggers = [],
            PipelineType = PipelineType.V2,
        });
    }

    private (Role pipelineRole, Role buildServiceRole) GetIamRoles()
    {
        var buildServiceRole = _serviceEnvironment.CreateName("backend-service-role");
        var buildRole = new Role(_scope, buildServiceRole, new RoleProps
        {
            RoleName = buildServiceRole,
            ManagedPolicies = [ ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2ContainerRegistryPowerUser") ],
            AssumedBy = new AnyPrincipal(),
        });

        var pipelineRoleName = _serviceEnvironment.CreateName("pipeline-role");
        var pipelineRole = new Role(_scope, pipelineRoleName, new RoleProps
        {
            RoleName = pipelineRoleName,
            AssumedBy = new ServicePrincipal("codepipeline.amazonaws.com"),
            InlinePolicies = new Dictionary<string, PolicyDocument>
            {
                {
                    "PassCodeBuildRolePolicy",
                    new PolicyDocument(new PolicyDocumentProps
                    {
                        Statements =
                        [
                            new PolicyStatement(new PolicyStatementProps
                            {
                                Actions = [ "iam:PassRole" ],
                                Resources = [ buildRole.RoleArn ],
                                Effect = Effect.ALLOW,
                            }),
                        ]
                    })
                },
            },
        });

        return (pipelineRole, buildRole);
    }

    private GitHubSourceAction GetSourceCode(out Artifact_ sourceCode)
    {
        sourceCode = new Artifact_("repository");

        var secretName = _serviceEnvironment.CreateName("github-oauth");

        var oauthSecret = new Secret(_scope, secretName, new SecretProps
        {
            SecretName = secretName,
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        if (!IsOauthTokenEnabled())
        {
            _ = new CfnOutput(_scope, _serviceEnvironment.CreateName("populate-github-oauth-warning"), new CfnOutputProps
            {
                Description = "Instructions to set the GitHub OAuth token",
                Value = $"Run this to set the GitHub token:\n" +
                        $"aws secretsmanager put-secret-value " +
                        $"--secret-id {secretName} " +
                        $"--secret-string \"<github-oauth-token>\"",
            });

            return null;
        }

        var sourceAction = new GitHubSourceAction(new GitHubSourceActionProps
        {
            Owner = "pendevx",
            Repo = "double-sharp",
            Branch = "main",
            OauthToken = oauthSecret.SecretValue,
            Output = sourceCode,
            ActionName = "Copy-Repository",
            Trigger = GitHubTrigger.NONE,
        });

        return sourceAction;
    }

    private CodeBuildAction BuildWebUI(Artifact_ sourceCode, out Artifact_ buildArtifacts)
    {
        buildArtifacts = new Artifact_("buildUIArtifacts");

        var buildAction = new CodeBuildAction(new CodeBuildActionProps
        {
            ActionName = "Build-WebUI",
            Outputs = [ buildArtifacts ],
            Input = sourceCode,
            Project = new PipelineProject(_scope, _serviceEnvironment.CreateName("Build-WebUI"), new PipelineProjectProps
            {
                ProjectName = "Build-WebUI",
                BuildSpec = BuildSpec.FromSourceFilename("webapp/buildspec.yml"),
            }),
        });

        return buildAction;
    }

    private CodeBuildAction BuildWebBackend(Artifact_ sourceCode, Role codebuildServiceRole, string containerName, out Artifact_ buildArtifacts)
    {
        buildArtifacts = new Artifact_("buildWebBackendArtifacts");

        var buildAction = new CodeBuildAction(new CodeBuildActionProps
        {
            ActionName = "Build-WebBackend",
            Outputs = [ buildArtifacts ],
            Input = sourceCode,
            Project = new PipelineProject(_scope, _serviceEnvironment.CreateName("Build-WebBackend"), new PipelineProjectProps
            {
                ProjectName = "Build-WebBackend",
                BuildSpec = BuildSpec.FromSourceFilename("backend/buildspec.yml"),
                Environment = new BuildEnvironment
                {
                    EnvironmentVariables = new Dictionary<string, IBuildEnvironmentVariable>
                    {
                        {
                            "REPOSITORY_URI",
                            new BuildEnvironmentVariable
                            {
                                Type = BuildEnvironmentVariableType.PLAINTEXT,
                                Value = _repository.RepositoryUri,
                            }
                        },
                        {
                            "CONTAINER_NAME",
                            new BuildEnvironmentVariable
                            {
                                Type = BuildEnvironmentVariableType.PLAINTEXT,
                                Value = containerName,
                            }
                        },
                    },
                    Privileged = true,
                    ComputeType = ComputeType.SMALL,
                    BuildImage = LinuxBuildImage.STANDARD_7_0,
                },
                Role = codebuildServiceRole,
            }),
            Role = codebuildServiceRole,
        });

        return buildAction;
    }

    private S3DeployAction DeployWebUI(Artifact_ uiArtifacts, Bucket s3DeployLocation)
    {
        return new S3DeployAction(new S3DeployActionProps
        {
            ActionName = "Deploy-WebUI",
            Bucket = s3DeployLocation,
            Extract = true,
            Input = uiArtifacts,
        });
    }

    private EcsDeployAction DeployWebBackend(Artifact_ backendArtifacts, BaseService deployLocation)
    {
        return new EcsDeployAction(new EcsDeployActionProps
        {
            ActionName = "Deploy-WebBackend",
            ImageFile = new ArtifactPath_(backendArtifacts, "backend/imagedefinitions.json"),
            Service = deployLocation,
        });
    }
}
