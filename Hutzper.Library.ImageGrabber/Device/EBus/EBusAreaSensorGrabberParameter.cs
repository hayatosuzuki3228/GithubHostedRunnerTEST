using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.EBus
{
    /// <summary>
    /// eBusエリアセンサ画像取得パラメータ
    /// </summary>
    [Serializable]
    public record EBusAreaSensorGrabberParameter : EBusGrabberParameter, IAreaSensorParameter
    {
        /// <summary>
        /// フレームレート
        /// </summary>
        [IniKey(true, -1d)]
        public double FramesPerSecond { get; set; } = -1d;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EBusAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EBusAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}