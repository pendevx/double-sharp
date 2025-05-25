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
    public static Distribution Create(Construct scope, ServiceEnvironment serviceEnvironment, Bucket bucket)
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
        var certificate = Domains.GenerateSharedCertificate(scope, serviceEnvironment);
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
                ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
                AllowedMethods = AllowedMethods.ALLOW_ALL,
                CachePolicy = CachePolicy.CACHING_DISABLED,
            },
            DomainNames = [ Domains.RootDomain ],
            Certificate = certificate,
            DefaultRootObject = "index.html",
            ErrorResponses =
            [
                new ErrorResponse
                {
                    ResponsePagePath = "/index.html",
                    ResponseHttpStatus = 200,
                    HttpStatus = 404,
                }
            ]
        });

        return distribution;
    }

    private static string ReadCodeFromEmbeddedResource(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyName = assembly.GetName().Name;

        using var stream = assembly.GetManifestResourceStream($"{assemblyName}.Resources.{fileName}");

        if (stream is null)
            throw new FileNotFoundException($"{nameof(fileName)}: Embedded resource '{fileName}' not found.");

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }
}
