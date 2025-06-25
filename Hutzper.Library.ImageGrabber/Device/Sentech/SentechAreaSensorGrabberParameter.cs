using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.Sentech
{
    /// <summary>
    /// Sentechエリアセンサ画像取得パラメータ
    /// </summary>
    [Serializable]
    public record SentechAreaSensorGrabberParameter : SentechGrabberParameter, IAreaSensorParameter
    {
        /// <summary>
        /// フレームレート
        /// </summary>
        [IniKey(true, -1d)]
        public double FramesPerSecond { get; set; } = -1d;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SentechAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SentechAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}