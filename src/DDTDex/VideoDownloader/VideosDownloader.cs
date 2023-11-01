using DDTDex.Proxy;
using DDTDex.Values;

namespace DDTDex.VideoDownloader;

public class VideosDownloader
{
    private readonly IYoutubeClientProxy _youtubeClient;

    public VideosDownloader(IYoutubeClientProxy youtubeClient)
    {
        _youtubeClient = youtubeClient;
    }

    public async Task DownloadVideos(string outputPath, IEnumerable<VideoData> videosDatas)
    {
        foreach (var video in videosDatas)
        {
            if (VideoNotExists(video.VideoId, outputPath))
                await _youtubeClient.DownloadVideoIn720P(video.VideoUrl, $"{video.VideoId}", outputPath);
        }
    }

    private bool VideoNotExists(string videoId, string path) =>
        !File.Exists(Path.Join(path, $"{videoId}.mp4"));
}