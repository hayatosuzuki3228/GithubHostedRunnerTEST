using Hutzper.Library.ImageGrabber.Device.Sentech;

namespace Hutzper.Library.ImageGrabber.Device.USB.Sentech
{
    [Serializable]
    public record UsbSentechAreaSensorGrabberParameter : SentechAreaSensorGrabberParameter, IUsbDeviceParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UsbSentechAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UsbSentechAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}