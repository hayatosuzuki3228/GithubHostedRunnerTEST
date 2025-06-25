namespace Hutzper.Library.ImageProcessing.Process.ImageRotator;

public interface IImageRotatorInput : IImageProcessorInput
{
    public double AngleDegree { get; set; } // 回転角度
}
