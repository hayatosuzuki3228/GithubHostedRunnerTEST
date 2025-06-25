using Hutzper.Library.ImageGrabber.Device.EBus;

namespace Hutzper.Library.ImageGrabber.Device.GigE.Jai
{
    [Serializable]
    public record GigEJaiAreaSensorGrabberParameter : EBusAreaSensorGrabberParameter, IGigEDeviceParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigEJaiAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigEJaiAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}