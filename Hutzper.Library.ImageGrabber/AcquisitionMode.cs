using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// 画像取得モード
    /// </summary>
    [Serializable]
    public enum AcquisitionMode
    {
        /// <summary>
        /// 連続
        /// </summary>
        [AliasName("Continuous")]
        Continuous,

        /// <summary>
        /// シングルフレーム
        /// </summary>
        [AliasName("SingleFrame")]
        SingleFrame,

        /// <summary>
        /// マルチフレーム
        /// </summary>
        [AliasName("MultiFrame")]
        MultiFrame,
    }
}
