using Hutzper.Library.Common;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.ExistenceJudgement;

public class ExistenceJudgementInput : SafelyDisposable, IExistenceJudgementInput
{
    public Bitmap Image { get; set; } // 処理対象の画像
    public int PixelThreshold { get; set; } // ワークがあると判定するためのピクセル数

    // コンストラクタ
    public ExistenceJudgementInput(Bitmap image)
    {
        this.Image = image;
    }

    // リソース解放
    protected override void DisposeExplicit()
    {
        this.DisposeSafely(this.Image);
    }
}