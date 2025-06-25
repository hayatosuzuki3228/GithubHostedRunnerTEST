using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.Web
{
    [Serializable]
    public record AreaWebGrabberParameter : WebGrabberParameter, IAreaSensorParameter
    {
        #region IAreaSensorParameter

        /// <summary>
        /// フレームレート
        /// </summary>
        [IniKey(true, 15d)]
        public double FramesPerSecond { get; set; } = 15d;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AreaWebGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AreaWebGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}