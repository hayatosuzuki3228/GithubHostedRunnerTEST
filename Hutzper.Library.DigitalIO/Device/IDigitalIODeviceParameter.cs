using Hutzper.Library.Common.Data;

namespace Hutzper.Library.DigitalIO.Device
{
    /// <summary>
    /// デジタルIOデバイスパラメータインタフェース
    /// </summary>
    public interface IDigitalIODeviceParameter : IControllerParameter
    {
        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; }

        /// <summary>
        /// 入力チャネル
        /// </summary>
        public int[] InputChannels { get; set; }

        /// <summary>
        /// 出力チャネル
        /// </summary>
        public int[] OutputChannels { get; set; }

        /// <summary>
        /// 入力フィルタリングミリ秒
        /// </summary>
        public int[] FilteringTimeMsIntervals { get; set; }
    }
}