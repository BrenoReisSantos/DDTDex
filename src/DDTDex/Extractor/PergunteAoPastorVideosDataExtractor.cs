using DDTDex.Proxy;
using DDTDex.Values;

namespace DDTDex.Extractor;

public class PergunteAoPastorVideosDataExtractor
{
    private readonly IYoutubeClientProxy _youtubeClientProxy;

    private readonly string _playlistUrl =
        "https://www.youtube.com/watch?v=6MjmEVd2Zpk&list=PLRPNvughqc8RiYOvgtthDIqG_74kNNBOy";

    public PergunteAoPastorVideosDataExtractor(IYoutubeClientProxy youtubeClientProxy)
    {
        _youtubeClientProxy = youtubeClientProxy;
    }

    public async Task<IEnumerable<VideoData>> ExtractVideosDataAsync() =>
        await _youtubeClientProxy.GetPlaylistVideosAsync(_playlistUrl);
}