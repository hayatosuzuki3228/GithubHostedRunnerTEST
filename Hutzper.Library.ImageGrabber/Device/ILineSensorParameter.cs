namespace Hutzper.Library.ImageGrabber.Device
{
    public interface ILineSensorParameter : IGrabberParameter
    {
        /// <summary>
        /// X反転
        /// </summary>
        public bool ReverseX { get; set; }

        /// <summary>
        /// ラインレート
        /// </summary>
        public double LineRateHz { get; set; }
    }
}