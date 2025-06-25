using Hutzper.Library.ImageGrabber.Device.Pylon;

namespace Hutzper.Library.ImageGrabber.Device.GigE.Pylon
{
    [Serializable]
    public record GigEPylonLineSensorGrabberParameter : PylonLineSensorGrabberParameter, IGigEDeviceParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigEPylonLineSensorGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigEPylonLineSensorGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}