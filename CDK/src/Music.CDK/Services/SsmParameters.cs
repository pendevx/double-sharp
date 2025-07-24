using Amazon.CDK.AWS.SSM;
using Constructs;

namespace Music.CDK.Services;

public class SsmParameters
{
    public static StringParameter CookiesTxt(Construct scope, ServiceEnvironment serviceEnvironment)
    {
        var cookiesParameter = serviceEnvironment.CreateName("cookies");
        return new StringParameter(scope, cookiesParameter, new StringParameterProps
        {
            ParameterName = cookiesParameter,
            StringValue = "<OVERWRITE-THIS>",
        });
    }
}
