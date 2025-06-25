using System.Diagnostics;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement;

public interface IRuleBaseJudgement<TInput, TResult>
{
    public Stopwatch Stopwatch { get; }
    public TResult Execute(TInput input);
}