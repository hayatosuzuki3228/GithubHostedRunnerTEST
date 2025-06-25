using Hutzper.Library.ImageProcessing.Process;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.ExistenceJudgement;

public interface IExistenceJudgementInput : IRuleBaseJudgementInput
{
    public int PixelThreshold { get; set; } // ワークがあると判定するためのピクセル数
}
