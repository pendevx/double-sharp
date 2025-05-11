using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SecretsManager;
using Constructs;
using Secret = Amazon.CDK.AWS.SecretsManager.Secret;
using StageProps = Amazon.CDK.AWS.CodePipeline.StageProps;

namespace Music.CDK.Services;

public record PipelinePackage(
    Construct Scope,
    ServiceEnvironment ServiceEnvironment,
    Bucket FrontendDeployTarget
);

public class CicdPipeline
{
    private static Construct _scope;
    private static ServiceEnvironment _serviceEnvironment;

    public static void Create(PipelinePackage createPipeline)
    {
        _scope = createPipeline.Scope;
        _serviceEnvironment = createPipeline.ServiceEnvironment;

        var artifactsBucketName = _serviceEnvironment.CreateName("cicd-artifacts");
        var artifactsBucket = new Bucket(_scope, artifactsBucketName, new BucketProps
        {
            BucketName = artifactsBucketName,
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        var sourceAction = GetSourceCode(out var sourceCode);
        var buildUIAction = BuildWebUI(sourceCode, out var uiArtifacts);
        var buildBackendAction = BuildWebBackend(sourceCode, out var backendArtifacts);

        var deployUIAction = DeployWebUI(uiArtifacts, createPipeline.FrontendDeployTarget);

        var pipelineName = _serviceEnvironment.CreateName("cicd");
        var pipeline = new Pipeline(_scope, pipelineName, new PipelineProps
        {
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
                    Actions = [ deployUIAction ],
                }
            ],
            ArtifactBucket = artifactsBucket,
        });
    }

    private static GitHubSourceAction GetSourceCode(out Artifact_ sourceCode)
    {
        sourceCode = new Artifact_("repository");

        var secretName = _serviceEnvironment.CreateName("github-oauth");

        var oauthSecret = new Secret(_scope, secretName, new SecretProps
        {
            SecretName = secretName,
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        var sourceAction = new GitHubSourceAction(new GitHubSourceActionProps
        {
            Owner = "pendevx",
            Repo = "double-sharp",
            Branch = "main",
            OauthToken = oauthSecret.SecretValue,
            Output = sourceCode,
            ActionName = "Copy-Repository",
        });

        return sourceAction;
    }

    private static CodeBuildAction BuildWebUI(Artifact_ sourceCode, out Artifact_ buildArtifacts)
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

    private static CodeBuildAction BuildWebBackend(Artifact_ sourceCode, out Artifact_ buildArtifacts)
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
            }),
        });

        return buildAction;
    }

    private static S3DeployAction DeployWebUI(Artifact_ uiArtifacts, Bucket s3DeployLocation)
    {
        return new S3DeployAction(new S3DeployActionProps
        {
            ActionName = "Deploy-WebUI",
            Bucket = s3DeployLocation,
            Extract = true,
            Input = uiArtifacts,
        });
    }

    // Unused for now, until the ECS deployments get figured out.
    private static EcsDeployAction DeployWebBackend(Artifact_ backendArtifacts, FargateService deployLocation)
    {
        return new EcsDeployAction(new EcsDeployActionProps
        {
            ActionName = "Deploy-WebBackend",
            Input = backendArtifacts,
            Service = deployLocation,
        });
    }
}
