using System.Diagnostics;
using Amazon.S3;
using Amazon.S3.Model;
using Music.Global.Contracts;
using Music.Models.Data.SongRequests;
using Music.Services.DataAccess.AWS;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace Music.Services.SongRequests;

public class SongRequestService
{
    private readonly IAmazonS3 _s3Client;
    private readonly GetBucketName _getBucketName;
    private readonly GetSongPath _getSongPath;
    private readonly SongsRepository _songsRepository;

    public SongRequestService(IAmazonS3 s3Client, GetBucketName getBucketName, GetSongPath getSongPath,
        SongsRepository songsRepository)
    {
        _s3Client = s3Client;
        _getBucketName = getBucketName;
        _getSongPath = getSongPath;
        _songsRepository = songsRepository;
    }

    public Task ApproveSongRequestAsync(SongRequest request, int newSongId, CancellationToken ct) =>
        request.Url switch
        {
            SongRequestWebUrl => DownloadSongFromUrlAsync(request.RawUrl, newSongId, ct),
            S3Key s3Key => CopySongRequestFileToSongsAsync(s3Key, newSongId),
            _ => throw new UnreachableException(nameof(request.Url))
        };

    private async Task DownloadSongFromUrlAsync(string url, int newSongId, CancellationToken ct)
    {
        var ytdl = new YoutubeDL { OutputFileTemplate = "%(id)s.%(ext)s" };
        var path = await ytdl.RunAudioDownload(url, overrideOptions: new OptionSet
        {
            Cookies = Path.Combine(Environment.CurrentDirectory, "cookies.txt"),
        }, ct: ct);

        if (!path.Success)
            throw new Exception(string.Join("\n", path.ErrorOutput));

        var hlsBase = await ConvertToAac(path.Data, ct);
        var hlsFiles = Directory.GetFiles(hlsBase, "*", SearchOption.AllDirectories)
            .Select(hlsFile => new UploadData(
                hlsFile,
                $"Songs/{newSongId}/{hlsFile.Replace($"{hlsBase}", "").Replace('\\', '/')}".Replace("//", "/")))
            .ToList();

        try
        {
            await _songsRepository.UploadAsync(hlsFiles);
        }
        finally
        {
            foreach (var hlsFile in hlsFiles)
                File.Delete(hlsFile.File);
        }
    }

    private Task<CopyObjectResponse> CopySongRequestFileToSongsAsync(S3Key key, int newSongId)
    {
        var bucketName = _getBucketName();
        var newSongKey = _getSongPath(newSongId);

        var request = new CopyObjectRequest
        {
            SourceBucket = bucketName,
            SourceKey = key,
            DestinationBucket = bucketName,
            DestinationKey = newSongKey,
        };

        return _s3Client.CopyObjectAsync(request);
    }

    private static async Task<string> ConvertToAac(string fileName, CancellationToken ct)
    {
        var baseDir = Path.GetFileNameWithoutExtension(fileName);

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments =
                $"""
                     -i "{fileName}"
                     -filter_complex "[0:a]asplit=3[a1][a2][a3]"
                     -map "[a1]"
                     -c:a:0 aac
                     -b:a:0 48k
                     -map "[a2]"
                     -c:a:1 aac
                     -b:a:1 64k
                     -map "[a3]"
                     -c:a:2 aac
                     -b:a:2 96k
                     -f hls
                     -hls_time 6
                     -hls_list_size 0
                     -hls_playlist_type vod
                     -master_pl_name "master.m3u8"
                     -var_stream_map "a:0,name:48k a:1,name:64k a:2,name:96k"
                     -hls_segment_filename "{baseDir}/%v/data%03d.m4s"
                     -hls_flags independent_segments
                     -hls_segment_type fmp4
                     "{baseDir}/%v/playlist.m3u8"
                     """.Replace(Environment.NewLine, " ")
        });

        await process?.WaitForExitAsync(ct)!;

        return baseDir;
    }
}
