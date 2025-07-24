using System.Diagnostics;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<SongRequestService> _logger;

    public SongRequestService(IAmazonS3 s3Client, GetBucketName getBucketName, GetSongPath getSongPath,
        SongsRepository songsRepository, ILogger<SongRequestService> logger)
    {
        _s3Client = s3Client;
        _getBucketName = getBucketName;
        _getSongPath = getSongPath;
        _songsRepository = songsRepository;
        _logger = logger;
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
            var mimeType = MimeType.InferFromFileName(path.Data);
            await using var contents = File.OpenRead(path.Data);
            await _songsRepository.UploadAsync(newSongId, contents, mimeType);
        }
        finally
        {
            File.Delete(path.Data);
        }
    }

//     private async Task DownloadSongFromUrlAsync(string url, int newSongId)
//     {
//         // might change the ignore config
//         var arguments = $"""
//                         --external-downloader "m3u8:native" --external-downloader-args "ffmpeg:-nostats -loglevel 0" -o "%(id)s.%(ext)s" --force-overwrites --no-part --cookies "cookies.txt" -i --ignore-config -x --ffmpeg-location "ffmpeg" --print "after_move:outfile: %(filepath)s" -f "bestaudio/best" --no-playlist -- "{url}"
//                         """;
//
//         var ytDlp = new ProcessStartInfo
//         {
//             FileName = "yt-dlp",
//             Arguments = arguments,
//             RedirectStandardOutput = true,
//             RedirectStandardError = true,
//             UseShellExecute = false,
//             CreateNoWindow = true,
//         };
//
//         using var process = new Process { StartInfo = ytDlp };
//         process.Start();
//
//         var output = await process.StandardOutput.ReadToEndAsync();
//         var error = await process.StandardError.ReadToEndAsync();
//
//         await process.WaitForExitAsync();
//
//         if (!string.IsNullOrWhiteSpace(error))
//             _logger.LogError("yt-dlp error: {Error}", error);
//
//         _logger.LogInformation("yt-dlp output: {Output}", output);
//
//         var fileName = new Regex("^outfile: (.*)").Match(output).Groups[1].Value;
//         var mimeType = MimeType.InferFromFileName(fileName);
//
//         await using (var contents = File.OpenRead(fileName))
//             await _songsRepository.UploadAsync(newSongId, contents, mimeType);
//
//         File.Delete(fileName);
//     }

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
}
