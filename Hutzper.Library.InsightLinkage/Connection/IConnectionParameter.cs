using Hutzper.Library.Common.Data;

namespace Hutzper.Library.InsightLinkage.Connection;

/// <summary>
/// Insight連携通信パラメータ
/// </summary>
public interface IConnectionParameter : IControllerParameter
{
    /// <summary>
    /// 使用するかどうか
    /// </summary>
    public bool IsUse { get; set; }
}