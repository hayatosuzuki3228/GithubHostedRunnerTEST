namespace Hutzper.Library.ImageGrabber.Device
{
    public interface IAreaSensor : IGrabber
    {
        /// <summary>
        /// フレームレート
        /// </summary>
        public double FramesPerSecond { get; set; }
    }
}