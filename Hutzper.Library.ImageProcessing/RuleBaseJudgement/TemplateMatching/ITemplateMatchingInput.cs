namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.TemplateMatching;

public interface ITemplateMatchingInput : IRuleBaseJudgementInput
{
    public double ThresholdValue { get; set; } // 確信度の閾値
}