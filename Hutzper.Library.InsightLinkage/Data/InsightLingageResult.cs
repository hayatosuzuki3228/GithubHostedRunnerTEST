using Hutzper.Library.InsightLinkage.Controller;

namespace Hutzper.Library.InsightLinkage.Data;

/// <summary>
/// IInsightLinkageResult実装
/// </summary>
[Serializable]
public record InsightLinkageResult : IInsightLinkageResult
{
    /// <summary>
    /// 処理が正常に行われたかどうか
    /// </summary>
    public bool IsSuccess
    {
        get
        {
            var isSuccess = true;
            foreach (var r in this.Results.Values)
            {
                isSuccess &= r;
            }

            return isSuccess;
        }
    }

    /// <summary>
    /// 送信リクエスト
    /// </summary>
    public IInsightLinkageRequest Request { get; init; }

    /// <summary>
    /// 個別の処理結果
    /// </summary>
    public Dictionary<InsightLinkageType, bool> Results { get; set; } = new();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="request"></param>
    public InsightLinkageResult(IInsightLinkageRequest request)
    {
        this.Request = request;
    }
}