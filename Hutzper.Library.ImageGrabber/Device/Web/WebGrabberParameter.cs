using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.ImageGrabber.Device.Web
{
    /// <summary>
    /// ファイル画像取得
    /// </summary>
    [Serializable]
    public record WebGrabberParameter : GrabberParameterBase, IWebDeviceParameter
    {
        /// <summary>
        /// ディレクトリ情報
        /// </summary>
        [IniKey(true, "")]
        public DirectoryInfo? ImageDirectoryInfo { get; set; } = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public WebGrabberParameter() : this(Common.Drawing.Point.New())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public WebGrabberParameter(Common.Drawing.Point location) : base(location, typeof(WebGrabberParameter).Name)
        {
        }
    }
}