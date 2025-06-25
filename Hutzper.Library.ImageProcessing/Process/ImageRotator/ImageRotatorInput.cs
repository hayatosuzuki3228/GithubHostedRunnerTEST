using Hutzper.Library.Common;
using System.Diagnostics;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process.ImageRotator;

public class ImageRotatorInput : SafelyDisposable, IImageRotatorInput
{
    public Bitmap Image { get; set; } // 処理対象の画像
    public double AngleDegree { get; set; } // 回転角度

    // コンストラクタ
    public ImageRotatorInput(Bitmap image, double angleDegree)
    {
        this.Image = image;
        this.AngleDegree = angleDegree;
    }

    // リソース解放
    protected override void DisposeExplicit()
    {
        this.DisposeSafely(this.Image);
    }
}
