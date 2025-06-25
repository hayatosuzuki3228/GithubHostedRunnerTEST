using Hutzper.Library.Common;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process.ImageCropper;

public class ImageCropperInput : SafelyDisposable, IImageCropperInput
{
    public Bitmap Image { get; set; } // 処理対象の画像
    public Rectangle CropRectangle { get; set; } // 切り出し矩形

    // コンストラクタ
    public ImageCropperInput(Bitmap image, Rectangle cropRectangle)
    {
        this.Image = image;
        this.CropRectangle = cropRectangle;
    }

    // リソース解放
    protected override void DisposeExplicit()
    {
        this.DisposeSafely(this.Image);
    }
}