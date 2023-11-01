using DDTDex.Extractor;
using DDTDex.Proxy;
using DDTDex.VideoDownloader;

var basePath = AppDomain.CurrentDomain.BaseDirectory;

IYoutubeClientProxy youtubeClientProxy = new YoutubeClientProxy();

var videosDatasExtractor = new PergunteAoPastorVideosDataExtractor(youtubeClientProxy);
var videos = await videosDatasExtractor.ExtractVideosDataAsync();
var downloader = new VideosDownloader(youtubeClientProxy);
var outputVideosPath = Path.Join(basePath, "output_videos");
await downloader.DownloadVideos(outputVideosPath, videos);
