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
            VideoUrl = video.Url
        };
    }

    public async Task<IEnumerable<VideoData>> GetPlaylistVideo(string playlistUrl)
    {
        var playlistVideos = await _youtubeClient.Playlists.GetVideosAsync(playlistUrl);

        var videosData = new List<VideoData>();
        foreach (var video in playlistVideos)
        {
            videosData.Add(new VideoData
            {
                VideoDuration = video.Duration,
                VideoName = video.Title,
                VideoUrl = video.Url
            });
        }

        return videosData;
    }


    public async Task DownloadVideoIn720P(string videoUrl, string fileName, string outputPath)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);
        var streamWith720pQuality = streamManifest.GetMuxedStreams().Where(s => s.VideoQuality.Label == "720p")
            .GetWithHighestVideoQuality();
        await _youtubeClient.Videos.Streams.DownloadAsync(streamWith720pQuality, $"{outputPath}/{fileName}.mp4");
    }
}