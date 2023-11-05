namespace PergunteAoPastorML.Values;

public record InputData
{
    public byte[] ImageBytes { get; set; } = Array.Empty<byte>(); //byte representation of image
    public uint LabelKey { get; set; } //numerical representation of label
    public string ImagePath { get; set; } = string.Empty; //path of the image
    public string Label { get; set; } = string.Empty;
}