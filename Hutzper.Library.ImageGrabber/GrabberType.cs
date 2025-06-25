using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// 画像取得タイプ
    /// </summary>
    [Serializable]
    public enum GrabberType
    {
        /// <summary>
        /// エリアセンサ
        /// </summary>
        [AliasName("AreaSensor")]
        AreaSensor,

        /// <summary>
        /// ラインセンサ
        /// </summary>
        [AliasName("LineSensor")]
        LineSensor,

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        [AliasName("File")]
        File,
    }
}