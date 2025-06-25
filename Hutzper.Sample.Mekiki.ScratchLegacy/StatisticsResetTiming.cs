using Hutzper.Library.Common.Attribute;
using System.ComponentModel;

namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    /// <summary>
    /// 統計情報リセットタイミング
    /// </summary>
    [Serializable]
    public enum StatisticsResetTiming
    {
        [AliasName("Daily")]
        [Description("日次リセット")]
        Daily,

        [AliasName("OnInspectionStart")]
        [Description("検査開始時にリセット")]
        OnInspectionStart,

        [AliasName("Manual")]
        [Description("ユーザーによる手動リセット")]
        Manual,
    }
}
