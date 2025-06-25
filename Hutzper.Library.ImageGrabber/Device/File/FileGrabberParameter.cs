using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.File
{
    /// <summary>
    /// ファイル画像取得
    /// </summary>
    [Serializable]
    public record FileGrabberParameter : GrabberParameterBase, IFileDeviceParameter
    {
        /// <summary>
        /// ディレクトリ情報
        /// </summary>
        [IniKey(true, "")]
        public DirectoryInfo? ImageDirectoryInfo { get; set; } = null;

        /// <summary>
        /// 有効な拡張子
        /// </summary>
        [IniKey(true, "png")]
        public string[] ValidExtensions { get; set; } = new[] { "png" };

        /// <summary>
        /// 前回の位置から再開する
        /// </summary>
        [IniKey(true, false)]
        public bool ResumeFromLastFile { get; set; } = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public FileGrabberParameter() : this(Common.Drawing.Point.New())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public FileGrabberParameter(Common.Drawing.Point location) : base(location, typeof(FileGrabberParameter).Name)
        {
        }
    }
}