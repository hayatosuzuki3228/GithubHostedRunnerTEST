using Hutzper.Library.Common;
using System.Drawing;
namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.TemplateMatching;

public class TemplateMatchingInput : SafelyDisposable, ITemplateMatchingInput
{
    public Bitmap Image { get; set; } // 処理対象の画像
    public double ThresholdValue { get; set; } // 確信度の閾値

    // コンストラクタ
    public TemplateMatchingInput(Bitmap image, double thresholdValue)
    {
        this.Image = image;
        this.ThresholdValue = thresholdValue;
    }

    // リソース解放
    protected override void DisposeExplicit()
    {
        this.DisposeSafely(this.Image);
    }
}