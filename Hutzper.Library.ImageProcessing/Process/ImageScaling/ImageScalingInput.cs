using Hutzper.Library.Common;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process.ImageScaling;

public class ImageScalingInput : SafelyDisposable, IImageScalingInput
{
    public Bitmap Image { get; set; } // 処理対象の画像

    // コンストラクタ
    public ImageScalingInput(Bitmap bitmap)
    {
        this.Image = bitmap;
    }

    // リソース解放
    protected override void DisposeExplicit()
    {
        this.DisposeSafely(this.Image);
    }
}