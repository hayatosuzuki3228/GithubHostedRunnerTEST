using Hutzper.Library.Common.Attribute;

namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    /// <summary>
    /// 照明制御タイミング
    /// </summary>
    [Serializable]
    public enum LightControlTiming
    {
        [AliasName("NoControl")]
        NoControl,

        [AliasName("Fastest")]
        Fastest,

        [AliasName("FixedDelayFromSignalOn")]
        FixedDelayFromSignalOn,

        [AliasName("FixedDelayFromSignalOff")]
        FixedDelayFromSignalOff,
    }
}