namespace Hutzper.Library.ImageGrabber.Device.Web
{
    /// <summary>
    /// ファイルインタフェース
    /// </summary>
    public interface IWebDevice : IGrabber
    {
        /// <summary>
        /// 現在のディレクトリ情報
        /// </summary>
        public DirectoryInfo? CurrentDirectory { get; }
    }
}