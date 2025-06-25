using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.ImageGrabber.IO.Configuration
{
    /// <summary>
    /// カメラ構成
    /// </summary>
    /// <remarks>エリアとライン別に使用するカメラを指定します</remarks>
    [Serializable]
    public record CameraConfiguration : IniFileCompatible<CameraConfiguration>
    {
        /// <summary>
        /// エリアセンサリスト
        /// </summary>
        [IniKey(true, new string[] { "sentech" })]
        public string[] AreaSensors { get; set; }

        /// <summary>
        /// ラインセンサリスト
        /// </summary>
        [IniKey(true, new string[0])]
        public string[] LineSensors { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 全カメラ数
        /// </summary>
        public int NumberOfAllGrabbers => this.AreaSensors.Length + this.LineSensors.Length;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraConfiguration() : base("Camera_Configuration".ToUpper())
        {
            this.AreaSensors = new string[] { "sentech" };
        }
    }
}