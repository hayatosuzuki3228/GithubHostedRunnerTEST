using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// トリガータイプ
    /// </summary>
    [Serializable]
    public enum TriggerSelector
    {
        [AliasName("Unsupported")]
        Unsupported,

        [AliasName("FrameStart")]
        FrameStart,

        [AliasName("AcquisitionStart")]
        AcquisitionStart,

        [AliasName("LineStart")]
        LineStart,
    }
}
