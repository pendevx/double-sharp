using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.S3;
using Constructs;

namespace Music.CDK.Services;

public class ApiGateway
{
    public static void Create(Construct scope, ServiceEnvironment serviceEnvironment, Vpc vpc,
        Bucket otherArtifacts, string bucketName)
    {
        // 2. Security Group for HTTP/HTTPS access
        var sg = new SecurityGroup(scope, "NginxSg", new SecurityGroupProps
        {
            Vpc = vpc,
            AllowAllOutbound = true,
            Description = "Allow HTTP/HTTPS to EC2 for NGINX",
            SecurityGroupName = "nginx-proxy-sg"
        });
        sg.AddIngressRule(Peer.AnyIpv4(), Port.AllTcp());

        var ec2Role = new Role(scope, "NginxProxyRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("ec2.amazonaws.com"),
            ManagedPolicies =
            [
                ManagedPolicy.FromAwsManagedPolicyName("AmazonS3ReadOnlyAccess")
            ]
        });

        otherArtifacts.AddToResourcePolicy(new PolicyStatement(new PolicyStatementProps
        {
            Actions = ["s3:GetObject"],
            Resources = [$"arn:aws:s3:::{otherArtifacts.BucketName}/*"],
            Principals = [new ArnPrincipal(ec2Role.RoleArn)]
        }));

        // 3. Provision the EC2 instance in a public subnet
        var instance = new Instance_(scope, "NginxProxyInstance", new InstanceProps
        {
            Vpc = vpc,
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PUBLIC },
            InstanceType = InstanceType.Of(InstanceClass.T4G, InstanceSize.MICRO), // ARM-based instance type
            MachineImage = MachineImage.LatestAmazonLinux2(new AmazonLinux2ImageSsmParameterProps
            {
                CpuType = AmazonLinuxCpuType.ARM_64,
            }),
            SecurityGroup = sg,
            AssociatePublicIpAddress = false, // We'll attach an EIP next
            Role = ec2Role // Attach the IAM role to the EC2 instance
        });

        const string certFilePath = "ssl_certs/fullchain.pem";
        const string keyFilePath = "ssl_certs/privkey.pem";

        var nginxConfig = Utils.ReadCodeFromEmbeddedResource("ApiGateway.nginx.conf");

        // Optional: install NGINX
        instance.UserData.AddCommands(
            "amazon-linux-extras install nginx1 -y",
            // Fetch the SSL certificate files from S3
            $"aws s3 cp s3://{bucketName}/{certFilePath} /etc/nginx/ssl/fullchain.pem",
            $"aws s3 cp s3://{bucketName}/{keyFilePath} /etc/nginx/ssl/privkey.pem",
            // Write custom nginx config
            "cat > /etc/nginx/conf.d/reverse-proxy.conf << 'EOF'",
            nginxConfig,
            "EOF",
            // Clean up default config (optional)
            "rm -f /etc/nginx/conf.d/default.conf",
            // Start/restart NGINX
            "systemctl restart nginx"
        );

        // 4. Allocate and associate an Elastic IP
        var eip = new CfnEIP(scope, "NginxEip", new CfnEIPProps
        {
            Domain = "vpc"
        });

        Domains.CreateAliasForService(scope, serviceEnvironment, ServicesWithDomains.ApiGateway, "apigateway",
            RecordTarget.FromIpAddresses(eip.AttrPublicIp), createIpv6: false);

        _ = new CfnEIPAssociation(scope, "EipAssoc", new CfnEIPAssociationProps
        {
            AllocationId = eip.AttrAllocationId,
            InstanceId = instance.InstanceId,
        });

        // Optional outputs for visibility
        _ = new CfnOutput(scope, "PublicIp", new CfnOutputProps
        {
            Value = eip.AttrPublicIp,
            Description = "Elastic IP assigned to the NGINX proxy",
        });

    }
}
