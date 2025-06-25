using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.File
{
    [Serializable]
    public record AreaFileGrabberParameter : FileGrabberParameter, IAreaSensorParameter
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
        public AreaFileGrabberParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AreaFileGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}