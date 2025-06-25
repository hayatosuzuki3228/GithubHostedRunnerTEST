using Hutzper.Library.Common.Data;

namespace Hutzper.Library.LightController.Device
{
    /// <summary>
    /// 照明制御パラメーター
    /// </summary>
    public interface ILightControllerParameter : IControllerParameter
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
        /// 照明制御タイプ
        /// </summary>
        public Library.LightController.LightingOnOffControlType OnOffControlType { get; set; }

        /// <summary>
        /// チャンネル
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// 調光
        /// </summary>
        public double Modulation { get; set; }

        /// <summary>
        /// 外部トリガー点灯時間μ秒
        /// </summary>
        public int ExternalTriggerStrobeTimeUs { get; set; }
    }
}