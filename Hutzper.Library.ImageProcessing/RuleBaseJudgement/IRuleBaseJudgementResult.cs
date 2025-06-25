using Hutzper.Library.Common;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement;

public interface IRuleBaseJudgementResult : ISafelyDisposable
{
    public bool IsSuccessful { get; set; } // 処理が成功したかどうか
    public bool IsJudgeOK { get; set; } // 判定結果
}