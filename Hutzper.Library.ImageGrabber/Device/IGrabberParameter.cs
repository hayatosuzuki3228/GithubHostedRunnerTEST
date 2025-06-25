using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageGrabber.Device
{
    /// <summary>
    /// 画像取得パラメータ
    /// </summary>
    public interface IGrabberParameter : IControllerParameter
    {
        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 使用するUserSetのIndex
        /// </summary>
        public int UseUserSetIndex { get; set; }

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; }

        /// <summary>
        /// トリガーモード
        /// </summary>
        public TriggerMode TriggerMode { get; set; }

        /// <summary>
        /// 露光時間μ秒
        /// </summary>
        public double ExposureTimeMicroseconds { get; set; }

        /// <summary>
        /// アナログゲイン
        /// </summary>
        public double AnalogGain { get; set; }

        /// <summary>
        /// デジタルゲイン
        /// </summary>
        public double DigitalGain { get; set; }

        /// <summary>
        /// 画像幅
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 画像高さ
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Xオフセット
        /// </summary>
        public int OffsetX { get; set; }

        /// <summary>
        /// Yオフセット
        /// </summary>
        public int OffsetY { get; set; }

        /// <summary>
        /// 設定内容を反映するかとうか
        /// </summary>
        public bool UseSetParameter { get; set; }
    }
}