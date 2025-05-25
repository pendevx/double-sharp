using Amazon.CDK;
using Environment = Amazon.CDK.Environment;

namespace Music.CDK;

class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        new MusicStack(app, nameof(MusicStack), new StackProps
        {
            // If you don't specify 'env', this stack will be environment-agnostic.
            // Account/Region-dependent features and context lookups will not work,
            // but a single synthesized template can be deployed anywhere.

            // Uncomment the next block to specialize this stack for the AWS Account
            // and Region that are implied by the current CLI configuration.
            Env = new Environment
            {
                Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
            }

            // For more information, see https://docs.aws.amazon.com/cdk/latest/guide/environments.html
        });
        app.Synth();
    }
}
