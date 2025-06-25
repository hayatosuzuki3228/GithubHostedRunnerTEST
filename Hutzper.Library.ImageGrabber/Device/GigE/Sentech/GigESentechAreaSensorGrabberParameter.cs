using Hutzper.Library.ImageGrabber.Device.Sentech;

namespace Hutzper.Library.ImageGrabber.Device.GigE.Sentech
{
    [Serializable]
    public record GigESentechAreaSensorGrabberParameter : SentechAreaSensorGrabberParameter, IGigEDeviceParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigESentechAreaSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigESentechAreaSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}