namespace PergunteAoPastorML.Values;

public record ImageData
{
    public required string ImagePath { get; init; }
    public required string Label { get; init; }
}