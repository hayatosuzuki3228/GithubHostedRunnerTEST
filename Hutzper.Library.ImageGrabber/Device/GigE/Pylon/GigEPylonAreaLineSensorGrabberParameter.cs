using Hutzper.Library.ImageGrabber.Device.GigE;
using Hutzper.Library.ImageGrabber.Device.Pylon;

namespace Hutzper.Library.ImageGrabber.Device.USB.Pylon
{
    [Serializable]
    public record GigEPylonAreaLineSensorGrabberParameter : PylonAreaSensorGrabberParameter, IGigEDeviceParameter
    {
        public int LineHeight { get; init; } = 2;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigEPylonAreaLineSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigEPylonAreaLineSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}