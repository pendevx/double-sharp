using System.IO;
using System.Reflection;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.S3;
using Constructs;
using Function = Amazon.CDK.AWS.CloudFront.Function;
using FunctionProps = Amazon.CDK.AWS.CloudFront.FunctionProps;

namespace Music.CDK.Services;

public class Cloudfront
{
    public static void Create(Construct scope, ServiceEnvironment serviceEnvironment, Bucket bucket)
    {
        var code = ReadCodeFromEmbeddedResource("Cloudfront.redirect-function.js");

        var functionName = serviceEnvironment.CreateName("redirect-function");
        var redirectFunction = new Function(scope, functionName, new FunctionProps
        {
            FunctionName = functionName,
            Runtime = FunctionRuntime.JS_2_0,
            Code = FunctionCode.FromInline(code),
        });

        var distributionName = serviceEnvironment.CreateName("cf-distribution");
        var distribution = new Distribution(scope, distributionName, new DistributionProps
        {
            DefaultBehavior = new BehaviorOptions
            {
                FunctionAssociations =
                [
                    new FunctionAssociation
                    {
                        Function = redirectFunction,
                        EventType = FunctionEventType.VIEWER_REQUEST,
                    }
                ],
                Origin = new S3StaticWebsiteOrigin(bucket),
            },
        });
    }

    private static string ReadCodeFromEmbeddedResource(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream("Music.CDK.Resources." + fileName)!;
        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }
}
