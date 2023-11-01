namespace DDTDex.Values;

public record VideoData
{
    public required string VideoName { get; init; }
    public required string VideoUrl { get; init; }
    public required string VideoId { get; init; }
    public TimeSpan? VideoDuration { get; init; }
}