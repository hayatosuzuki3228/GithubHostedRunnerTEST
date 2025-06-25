using Hutzper.Library.Common.Attribute;

namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    /// <summary>
    /// 画像保存条件
    /// </summary>
    [Serializable]
    public enum ImageSavingCondition
    {
        [AliasName("None")]
        None,

        [AliasName("Always")]
        Always,

        [AliasName("SpecificResults")]
        SpecificResults,

        [AliasName("SpecificInterval")]
        SpecificInterval,
    }
}