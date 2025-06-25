using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// トリガーモード
    /// </summary>
    [Serializable]
    public enum TriggerMode
    {
        /// <summary>
        /// カメラ内部トリガー
        /// </summary>
        [AliasName("InternalTrigger")]
        InternalTrigger,

        /// <summary>
        /// ソフト(アプリ)マターのトリガ
        /// </summary>
        [AliasName("SoftTrigger")]
        SoftTrigger,

        /// <summary>
        /// 外部トリガ
        /// </summary>
        [AliasName("ExternalTrigger")]
        ExternalTrigger,
    }
}