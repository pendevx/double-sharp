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

    private static string GetPrefix(AwsEnvironment awsEnvironment, string environmentName, string prefix) =>
        environmentName switch
        {
            "Production" => $"{prefix}/",
            "Development" => $"{awsEnvironment.UserId}/{prefix}/",
            _ => throw new InvalidOperationException()
        };

    public static GetSongPath GetSongPath(AwsEnvironment env, GetEnvironment getEnv) => songId =>
        $"{GetPrefix(env, getEnv(), "Songs")}{songId}/{AudioMp3FileName}";

    public static CreateSongRequestPath CreateSongRequestPath(AwsEnvironment env, GetEnvironment getEnv) => songRequestId =>
        $"{GetPrefix(env, getEnv(), "SongRequests")}{songRequestId}/{AudioMp3FileName}";
}
