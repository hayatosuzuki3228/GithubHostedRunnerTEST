using Hutzper.Library.Common.Drawing;
using Hutzper.Library.ImageProcessing.RuleBaseJudgement.TemplateMatching.Data;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.TemplateMatching;

public interface ITemplateMatchingResult : IRuleBaseJudgementResult
{
    public TemplateDataItem? Template { get; set; } // 使用したテンプレート
    public PointD Point { get; set; } // マッチング座標
    public double Score { get; set; } // マッチングスコア
}