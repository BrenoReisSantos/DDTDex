﻿using DDTDex.Values;

namespace DDTDex.Proxy;

public interface IYoutubeClientProxy
{
    Task<VideoData> GetVideoData(string videoUrl);
    Task<IEnumerable<VideoData>> GetPlaylistVideosAsync(string playlistUrl);
    Task DownloadVideoIn720P(string videoUrl, string fileName, string outputPath);
}