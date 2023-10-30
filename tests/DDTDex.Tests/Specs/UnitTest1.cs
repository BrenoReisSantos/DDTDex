using System.Net.Mime;
using FluentAssertions;
using Xunit.Abstractions;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos.Streams;

namespace DDTDex.Tests.Specs;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;

    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Test1()
    {
        var youtubeClient = new YoutubeClient();

        var playlistId =
            PlaylistId.TryParse("https://www.youtube.com/watch?v=6MjmEVd2Zpk&list=PLRPNvughqc8RiYOvgtthDIqG_74kNNBOy");

        if (playlistId is null)
            throw new ArgumentException("Não foi possível resgatar o Id da Playlist");

        var playlistVideos =
            await youtubeClient.Playlists.GetVideosAsync(playlistId.Value);

        var playlistVideosCapped = playlistVideos.Take(5);

        foreach (var video in playlistVideosCapped)
        {
            _output.WriteLine("Título: {0} | URL: {1}", video.Title, video.Url);

            var videoStream = await youtubeClient.Videos.Streams.GetManifestAsync(video.Url);

            var streamInfo = videoStream.GetMuxedStreams().Where(v => v.Container == Container.Mp4 &&
                                                                      v.VideoQuality.Label == "720p")
                .GetWithHighestVideoQuality();

            _output.WriteLine("Qualidade do Vídeo: {0} | FPS: {1}", streamInfo.VideoQuality.Label,
                streamInfo.VideoQuality.Framerate);

            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, $"videos/{video.Id.Value}.mp4");
            await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, path);

            _output.WriteLine($"Video Downloaded at: {path}");
        }
    }
}