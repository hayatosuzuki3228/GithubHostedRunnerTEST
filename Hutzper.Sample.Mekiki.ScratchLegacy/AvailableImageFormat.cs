using Hutzper.Library.Common.Attribute;

namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    [Serializable]
    public enum AvailableImageFormat
    {
        [AliasName("png")]
        Png,
        [AliasName("bmp")]
        Bmp,
        [AliasName("jpg")]
        Jpg,
    }
}
