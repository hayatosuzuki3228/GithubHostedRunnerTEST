using Hutzper.Library.ImageGrabber.Device.GigE;
using Hutzper.Library.ImageGrabber.Device.Pylon;

namespace Hutzper.Library.ImageGrabber.Device.USB.Pylon
{
    [Serializable]
    public record GigEPylonAreaSensorGrabberParameter : PylonAreaSensorGrabberParameter, IGigEDeviceParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigEPylonAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigEPylonAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}