using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace Music.Services;

public class AwsEnvironment
{
    private readonly IAmazonSecurityTokenService _stsClient;

    public AwsEnvironment(IAmazonSecurityTokenService stsClient)
    {
        _stsClient = stsClient;
    }

    private string? _userId;
    public string UserId
    {
        get
        {
            if (_userId is not null)
                return _userId;

            var userId = _stsClient.GetCallerIdentityAsync(new GetCallerIdentityRequest()).Result.UserId;

            _userId = userId;

            return _userId;
        }
    }
}
