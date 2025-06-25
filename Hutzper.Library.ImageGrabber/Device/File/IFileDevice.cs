namespace Hutzper.Library.ImageGrabber.Device.File
{
    /// <summary>
    /// ファイルインタフェース
    /// </summary>
    public interface IFileDevice : IGrabber
    {
        /// <summary>
        /// 現在のディレクトリ情報
        /// </summary>
        public DirectoryInfo? CurrentDirectory { get; }
    }
}