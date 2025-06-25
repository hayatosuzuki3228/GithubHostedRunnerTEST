using Hutzper.Library.Common;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement;

public interface IRuleBaseJudgementInput : ISafelyDisposable
{
    public Bitmap Image { get; set; } // 判定対象の画像
}