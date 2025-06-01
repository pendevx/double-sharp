using Music.Global.Contracts;
using Music.Services;

namespace Music.Backend.Global.Impl;

public static class AwsFunctions
{
    private const string BucketKeyFromConfig = "FilesBucketName";
    private const string AudioMp3FileName = "audio.mp3";

    public static GetBucketName GetBucketName(IConfiguration config) => () =>
        config.GetValue<string>(BucketKeyFromConfig) ??
        throw new InvalidOperationException();

    private static string GetPrefix(AwsEnvironment awsEnvironment, string environmentName) =>
        environmentName switch
        {
            "Production" => "",
            "Development" => $"{awsEnvironment.UserId}/",
            _ => throw new InvalidOperationException()
        };

    public static GetObjectKey GetObjectKey(AwsEnvironment awsEnvironment, GetEnvironment getEnvironment) => songId =>
        $"{GetPrefix(awsEnvironment, getEnvironment())}{songId}/{AudioMp3FileName}";
}
