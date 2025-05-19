using System.Collections.Generic;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.Route53;
using Constructs;

namespace Music.CDK.Services;

public static class Domains
{
    public const string HostedZoneDomain = "pendevx.com";
    public const string HostedZoneId = "Z06678342HDTZIF3GWZNF";
    public const string RootDomain = "music.pendevx.com";

    public static Dictionary<ServicesWithDomains, string> DomainsList { get; } = new()
    {
        { ServicesWithDomains.CloudfrontDistribution, RootDomain },
        { ServicesWithDomains.WebBackend, $"api.{RootDomain}" },
        { ServicesWithDomains.Database, $"db.{RootDomain}" }
    };

    private static IHostedZone _hostedZone;

    private static IHostedZone GetHostedZone(Construct scope, ServiceEnvironment serviceEnvironment)
    {
        if (_hostedZone is not null)
            return _hostedZone;

        _hostedZone = HostedZone.FromHostedZoneAttributes(scope, serviceEnvironment.CreateName("imported-zone"), new HostedZoneAttributes
        {
            ZoneName = HostedZoneDomain,
            HostedZoneId = HostedZoneId,
        });

        return _hostedZone;
    }

    private static ICertificate _certificate;
    public static ICertificate GenerateCertificate(Construct scope, ServiceEnvironment serviceEnvironment)
    {
        if (_certificate is not null)
            return _certificate;

        var certificateName = serviceEnvironment.CreateName("ssl-cert");

        _certificate = new DnsValidatedCertificate(scope, certificateName, new DnsValidatedCertificateProps
        {
            HostedZone = GetHostedZone(scope, serviceEnvironment),
            DomainName = DomainsList[ServicesWithDomains.CloudfrontDistribution],
            Validation = CertificateValidation.FromDns(_hostedZone),
            SubjectAlternativeNames = [ $"*.{RootDomain}" ],
            Region = "us-east-1",
        });

        return _certificate;
    }

    public static CnameRecord CreateCnameForService(Construct scope, ServiceEnvironment serviceEnvironment,
        string recordName, string domainName)
    {
        return new CnameRecord(scope, recordName + "-cname", new CnameRecordProps
        {
            Zone = GetHostedZone(scope, serviceEnvironment),
            RecordName = recordName,
            DomainName = domainName,
        });
    }

    public static (ARecord, AaaaRecord) CreateAliasForService(Construct scope, ServiceEnvironment serviceEnvironment,
        ServicesWithDomains service, string serviceName, IAliasRecordTarget target)
    {
        var ipv4 = new ARecord(scope, serviceName + "-alias", new ARecordProps
        {
            Zone = GetHostedZone(scope, serviceEnvironment),
            Target = RecordTarget.FromAlias(target),
            RecordName = DomainsList[service],
        });

        var ipv6 = new AaaaRecord(scope, serviceName + "-alias-ipv6", new AaaaRecordProps
        {
            Zone = GetHostedZone(scope, serviceEnvironment),
            Target = RecordTarget.FromAlias(target),
            RecordName = DomainsList[service],
        });

        return (ipv4, ipv6);
    }

    public static void CreateAliasForRoot(Construct scope, ServiceEnvironment serviceEnvironment, Distribution distribution)
    {
        CreateCnameForService(scope, serviceEnvironment, RootDomain, distribution.DistributionDomainName);
    }
}

public enum ServicesWithDomains
{
    CloudfrontDistribution,
    WebBackend,
    Database,
}
