namespace Music.CDK;

public class ServiceEnvironment
{
    private ServiceEnvironment(string prefix, string suffix)
    {
        _prefix = prefix;
        _suffix = suffix;
    }

    public static ServiceEnvironment Development { get; } = new("doublesharp", "-dev");
    public static ServiceEnvironment Production { get; } = new("doublesharp", "");

    private readonly string _prefix;
    private readonly string _suffix;

    public string CreateName(string serviceName) =>
        $"{_prefix}-{serviceName}{_suffix}";
}
