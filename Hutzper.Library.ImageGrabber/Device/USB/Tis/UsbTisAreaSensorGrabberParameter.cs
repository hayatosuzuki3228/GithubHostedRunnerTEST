using Hutzper.Library.ImageGrabber.Device.Tis;

namespace Hutzper.Library.ImageGrabber.Device.USB.Tis
{
    [Serializable]
    public record UsbTisAreaSensorGrabberParameter : TisAreaSensorGrabberParameter, IUsbDeviceParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UsbTisAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UsbTisAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}