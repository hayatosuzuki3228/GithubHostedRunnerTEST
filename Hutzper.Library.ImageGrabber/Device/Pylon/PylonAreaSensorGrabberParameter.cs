using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.Pylon
{
    /// <summary>
    /// Pylonエリアセンサ画像取得パラメータ
    /// </summary>
    [Serializable]
    public record PylonAreaSensorGrabberParameter : PylonGrabberParameter, IAreaSensorParameter
    {
        /// <summary>
        /// フレームレート
        /// </summary>
        [IniKey(true, -1d)]
        public double FramesPerSecond { get; set; } = -1d;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PylonAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PylonAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}