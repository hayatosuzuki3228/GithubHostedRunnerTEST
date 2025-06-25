using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.File
{
    [Serializable]
    public record LineFileGrabberParameter : FileGrabberParameter, ILineSensorParameter
    {
        #region ILineSensorParameter

        /// <summary>
        /// X反転
        /// </summary>
        [IniKey(true, false)]
        public bool ReverseX { get; set; } = false;

        /// <summary>
        /// ラインレート
        /// </summary>
        [IniKey(true, 1000d)]
        public double LineRateHz { get; set; } = 1000d;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineFileGrabberParameter() : this(Common.Drawing.Point.New())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineFileGrabberParameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}