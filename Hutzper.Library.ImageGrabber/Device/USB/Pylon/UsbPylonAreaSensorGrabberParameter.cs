using Hutzper.Library.ImageGrabber.Device.Pylon;

namespace Hutzper.Library.ImageGrabber.Device.USB.Pylon
{
    [Serializable]
    public record UsbPylonAreaSensorGrabberParameter : PylonAreaSensorGrabberParameter, IUsbDeviceParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UsbPylonAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UsbPylonAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}