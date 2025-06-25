using Hutzper.Library.Common.Attribute;

namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    /// <summary>
    /// 結果出力タイミング
    /// </summary>
    [Serializable]
    public enum ResultOutputTiming
    {
        [AliasName("Fastest")]
        Fastest,

        [AliasName("FixedDelayFromSignalOn")]
        FixedDelayFromSignalOn,

        [AliasName("FixedDelayFromAcquisition")]
        FixedDelayFromAcquisition,
    }
}