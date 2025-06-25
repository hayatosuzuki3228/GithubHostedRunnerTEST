namespace Hutzper.Library.ImageGrabber.Device.Web
{
    public interface IWebDeviceParameter : IGrabberParameter
    {
        /// <summary>
        /// ディレクトリ情報
        /// </summary>
        public DirectoryInfo? ImageDirectoryInfo { get; set; }
    }
}