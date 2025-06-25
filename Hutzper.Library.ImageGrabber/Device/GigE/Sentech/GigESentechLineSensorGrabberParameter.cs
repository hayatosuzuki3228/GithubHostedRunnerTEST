using Hutzper.Library.ImageGrabber.Device.Sentech;

namespace Hutzper.Library.ImageGrabber.Device.GigE.Sentech
{
    [Serializable]
    public record GigESentechLineSensorGrabberParameter : SentechLineSensorGrabberParameter, IGigEDeviceParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigESentechLineSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigESentechLineSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}