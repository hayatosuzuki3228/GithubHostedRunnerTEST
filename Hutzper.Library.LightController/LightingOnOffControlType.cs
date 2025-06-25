using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.LightController
{
    /// <summary>
    /// 点灯制御タイプ
    /// </summary>
    [Serializable]
    public enum LightingOnOffControlType
    {
        /// <summary>
        /// 制御無し
        /// </summary>
        [AliasName("NoControl")]
        NoControl,

        /// <summary>
        /// 運用時 ON 
        /// </summary>
        [AliasName("Continuous")]
        Continuous,

        /// <summary>
        /// ワーク単位 ON/OFF
        /// </summary>
        [AliasName("OneByOne")]
        OneByOne,

        /// <summary>
        /// 露光同期
        /// </summary>
        [AliasName("ExposureSync")]
        ExposureSync,
    }
}