namespace Hutzper.Library.ImageGrabber.Device
{
    public interface IAreaSensorParameter : IGrabberParameter
    {
        /// <summary>
        /// フレームレート
        /// </summary>
        public double FramesPerSecond { get; set; }
    }
}