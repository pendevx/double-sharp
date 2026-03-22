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

    public static GetSongPath GetSongPath(AwsEnvironment env, GetEnvironment getEnv) => songId =>
        $"Songs/{songId}/{AudioMp3FileName}";

    public static CreateSongRequestPath CreateSongRequestPath(AwsEnvironment env, GetEnvironment getEnv) => songRequestId =>
        $"SongRequests/{songRequestId}/{AudioMp3FileName}";
}
