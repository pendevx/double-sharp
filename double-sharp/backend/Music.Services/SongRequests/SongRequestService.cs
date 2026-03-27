using System.Diagnostics;
using Amazon.S3;
using Amazon.S3.Model;
using Music.Global.Contracts;
using Music.Models.Data;
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

    private static int[] BitRates => [ 48, 64, 96 ];

    public SongRequestService(IAmazonS3 s3Client, GetBucketName getBucketName, GetSongPath getSongPath,
        SongsRepository songsRepository)
    {
        _s3Client = s3Client;
        _getBucketName = getBucketName;
        _getSongPath = getSongPath;
        _songsRepository = songsRepository;
    }

    public Task ApproveSongRequestAsync(SongRequest request, int newSongId) =>
        request.Url switch
        {
            SongRequestWebUrl => DownloadSongFromUrlAsync(request.RawUrl, newSongId),
            S3Key s3Key => CopySongRequestFileToSongsAsync(s3Key, newSongId),
            _ => throw new UnreachableException(nameof(request.Url))
        };

    private async Task DownloadSongFromUrlAsync(string url, int newSongId)
    {
        var ytdl = new YoutubeDL { OutputFileTemplate = "%(id)s.%(ext)s" };
        var path = await ytdl.RunAudioDownload(url, overrideOptions: new OptionSet
        {
            Cookies = Path.Combine(Environment.CurrentDirectory, "cookies.txt"),
        });

        if (!path.Success)
            throw new Exception(string.Join("\n", path.ErrorOutput));

        try
        {
            var hlsFiles = await Task.WhenAll(BitRates.Select(ConvertToAac(path.Data)));

            foreach (var result in hlsFiles)
            {
                var mimeType = MimeType.InferFromFileName(result);
                await using var contents = File.OpenRead(result);
                await _songsRepository.UploadAsync(newSongId, contents, mimeType);
            }
        }
        finally
        {
            File.Delete(path.Data);
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

    private static Func<int, Task<string>> ConvertToAac(string fileName) => bitrateMbps => Task.Run(() =>
    {
        var outputFileName = $"output-{bitrateMbps}.m4a";

        Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i {fileName} -c:a aac -b:a {bitrateMbps}k -f hls -hls_time 6 -hls_list_size 0 -hls_segment_filename \"data%03d.ts\" playlist.m3u8",
            // ffmpeg -i audio.opus -filter_complex "[0:a]asplit=3[a1][a2][a3]" -map "[a1]" -c:a:0 aac -b:a:0 48k -map "[a2]" -c:a:1 aac -b:a:1 64k -map "[a3]" -c:a:2 aac -b:a:2 96k -f hls -hls_time 6 -hls_list_size 0 -hls_playlist_type vod -master_pl_name master.m3u8 -var_stream_map "a:0,name:48k a:1,name:64k a:2,name:96k" -hls_segment_filename "%v/data%03d.m4s" -hls_flags independent_segments -hls_segment_type fmp4 %v/playlist.m3u8
        });

        return outputFileName;
    });
}
