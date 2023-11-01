using DDTDex.Values;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace DDTDex.Proxy;

public class YoutubeClientProxy : IYoutubeClientProxy
{
    private readonly YoutubeClient _youtubeClient = new();

    public async Task<VideoData> GetVideoData(string videoUrl)
    {
        var video = await _youtubeClient.Videos.GetAsync(videoUrl);
        return new VideoData
        {
            VideoDuration = video.Duration,
            VideoName = video.Title,
            VideoId = video.Id.Value,
            VideoUrl = video.Url
        };
    }

    public async Task<IEnumerable<VideoData>> GetPlaylistVideosAsync(string playlistUrl)
    {
        var playlistVideos = await _youtubeClient.Playlists.GetVideosAsync(playlistUrl);

        var videosData = new List<VideoData>();
        foreach (var video in playlistVideos)
        {
            videosData.Add(new VideoData
            {
                VideoDuration = video.Duration,
                VideoName = video.Title,
                VideoId = video.Id.Value,
                VideoUrl = video.Url
            });
        }

        return videosData;
    }

    public async Task DownloadVideoIn720P(string videoUrl, string fileName, string outputPath)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);
        var streamWith720PQuality = streamManifest.GetMuxedStreams().Where(s => s.VideoQuality.Label == "720p")
            .GetWithHighestVideoQuality();

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        await _youtubeClient.Videos.Streams.DownloadAsync(streamWith720PQuality, $"{outputPath}/{fileName}.mp4");
    }
}