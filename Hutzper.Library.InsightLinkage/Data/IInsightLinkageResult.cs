using Hutzper.Library.InsightLinkage.Controller;

namespace Hutzper.Library.InsightLinkage.Data;

/// <summary>
/// Insightへの送信結果
/// </summary>
public interface IInsightLinkageResult
{
    /// <summary>
    /// 処理が正常に行われたかどうか
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// 送信リクエスト
    /// </summary>
    public IInsightLinkageRequest Request { get; }

    /// <summary>
    /// 個別の処理結果
    /// </summary>
    public Dictionary<InsightLinkageType, bool> Results { get; set; }
}