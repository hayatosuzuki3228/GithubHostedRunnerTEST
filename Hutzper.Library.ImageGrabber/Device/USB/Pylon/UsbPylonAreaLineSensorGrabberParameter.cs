using Hutzper.Library.ImageGrabber.Device.Pylon;

namespace Hutzper.Library.ImageGrabber.Device.USB.Pylon
{
    [Serializable]
    public record UsbPylonAreaLineSensorGrabberParameter : PylonAreaSensorGrabberParameter, IUsbDeviceParameter
    {
        public int LineHeight { get; init; } = 2;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UsbPylonAreaLineSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UsbPylonAreaLineSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}