using DDTDex.Proxy;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;

namespace DDTDex.VideoDownloader;

public class PergunteAoPastorVideosDownloader
{
    readonly IYoutubeClientProxy _youtubeClient;

    readonly string _playlistUrl =
        "https://www.youtube.com/watch?v=6MjmEVd2Zpk&list=PLRPNvughqc8RiYOvgtthDIqG_74kNNBOy";

    public PergunteAoPastorVideosDownloader(IYoutubeClientProxy youtubeClient)
    {
        _youtubeClient = youtubeClient;
    }

    public async Task DownloadVideos(string outputPath)
    {
        var pergunteAoPastorVideos = await _youtubeClient.GetPlaylistVideo(_playlistUrl);

        if (Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        foreach (var video in pergunteAoPastorVideos)
        {
            await _youtubeClient.DownloadVideoIn720P(video.VideoUrl, video.VideoUrl, outputPath);
        }
    }
}