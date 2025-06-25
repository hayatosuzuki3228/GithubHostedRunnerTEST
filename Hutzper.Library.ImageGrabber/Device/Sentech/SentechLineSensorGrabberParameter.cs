using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.Sentech
{
    /// <summary>
    /// Sentechラインセンサ画像取得パラメータ
    /// </summary>
    [Serializable]
    public record SentechLineSensorGrabberParameter : SentechGrabberParameter, ILineSensorParameter
    {
        /// <summary>
        /// X反転
        /// </summary>
        [IniKey(true, false)]
        public bool ReverseX { get; set; } = false;

        /// <summary>
        /// ラインレート
        /// </summary>
        [IniKey(true, 1000d)]
        public double LineRateHz { get; set; } = 1000d;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SentechLineSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SentechLineSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}