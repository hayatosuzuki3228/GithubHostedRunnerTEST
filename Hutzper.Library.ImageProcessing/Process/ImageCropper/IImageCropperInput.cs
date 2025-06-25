using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process.ImageCropper;

public interface IImageCropperInput : IImageProcessorInput
{
    public Rectangle CropRectangle { get; set; } // 切り出し矩形
}
