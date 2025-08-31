using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ServiceDiscovery;
using Constructs;

namespace Music.CDK.Services;

public static class ServiceDiscovery
{
    public static PrivateDnsNamespace Create(Construct scope, ServiceEnvironment serviceEnvironment, Vpc vpc)
    {
        var namespaceName = serviceEnvironment.CreateName("servicediscovery-namespace");
        var @namespace = new PrivateDnsNamespace(scope, namespaceName, new PrivateDnsNamespaceProps
        {
            Name = "doublesharp-internal",
            Vpc = vpc,
        });

        return @namespace;
    }
}
