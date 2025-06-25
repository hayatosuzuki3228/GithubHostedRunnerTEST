using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.Tis
{
    /// <summary>
    /// TISエリアセンサ画像取得パラメータ
    /// </summary>
    [Serializable]
    public record TisAreaSensorGrabberParameter : TisGrabberParameter, IAreaSensorParameter
    {

        public double GainR { get; init; }
        public double GainG { get; init; }
        public double GainB { get; init; }

        /// <summary>
        /// フレームレート
        /// </summary>
        [IniKey(true, -1d)]
        public double FramesPerSecond { get; set; } = -1d;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TisAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TisAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}