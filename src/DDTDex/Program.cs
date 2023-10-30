using FFMpegCore;
using FFMpegCore.Pipes;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos.Streams;

var basePath = AppDomain.CurrentDomain.BaseDirectory;
var videosPath = Path.Combine(basePath, "videos");
var imagesPath = Path.Combine(basePath, "frame_images");

Directory.CreateDirectory(videosPath);
Directory.CreateDirectory(imagesPath);

// await FazDownloadDosVideos();
FFMpegArguments
    .FromFileInput(Path.Join(videosPath, "Yl3IY78KFrs.mp4"))
    .OutputToFile(Path.Join(imagesPath, "frame.jpg"), true, options => options
        .Seek(TimeSpan.FromSeconds(255))
        .WithCustomArgument("-vframes 1")) // Extract only one frame
    .ProcessSynchronously();

var mediaInfo = FFProbe.Analyse(Path.Join(videosPath, "Yl3IY78KFrs.mp4"));

var memoryStream = new MemoryStream();
FFMpegArguments
    .FromFileInput(Path.Join(videosPath, "Yl3IY78KFrs.mp4"))
    .OutputToPipe(new StreamPipeSink(memoryStream), options =>
        options.Seek(TimeSpan.FromSeconds(1)))
    .ProcessSynchronously();


async Task FazDownloadDosVideos()
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
        Console.WriteLine("Título: {0} | URL: {1}", video.Title, video.Url);

        var videoStream = await youtubeClient.Videos.Streams.GetManifestAsync(video.Url);

        var streamInfo = videoStream.GetMuxedStreams().Where(v => v.Container == Container.Mp4 &&
                                                                  v.VideoQuality.Label == "720p")
            .GetWithHighestVideoQuality();

        Console.WriteLine("Qualidade do Vídeo: {0} | FPS: {1}", streamInfo.VideoQuality.Label,
            streamInfo.VideoQuality.Framerate);

        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.Combine(basePath, $"videos/{video.Id.Value}.mp4");
        await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, path);

        Console.WriteLine($"Video Downloaded at: {path}");
    }
}