using Music.Global.Contracts;
using Music.Services;

namespace Music.Backend.Global.Impl;

public static class AwsFunctions
{
    private const string BucketKeyFromConfig = "FilesBucketName";

    public static GetBucketName GetBucketName(IConfiguration config) => () =>
        config.GetValue<string>(BucketKeyFromConfig) ??
        throw new InvalidOperationException();
}
