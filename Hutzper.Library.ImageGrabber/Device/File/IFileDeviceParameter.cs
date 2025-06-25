namespace Hutzper.Library.ImageGrabber.Device.File
{
    public interface IFileDeviceParameter : IGrabberParameter
    {
        /// <summary>
        /// ディレクトリ情報
        /// </summary>
        public DirectoryInfo? ImageDirectoryInfo { get; set; }

        /// <summary>
        /// 有効な拡張子
        /// </summary>
        public string[] ValidExtensions { get; set; }

        /// <summary>
        /// 前回の位置から再開する
        /// </summary>
        public bool ResumeFromLastFile { get; set; }
    }
}