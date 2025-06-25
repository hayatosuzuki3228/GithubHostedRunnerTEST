
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Common.Diagnostics
{
    /// <summary>
    /// 監視対象デバイスステータス表示インタフェース
    /// </summary>
    public interface IDeviceStatusControl
    {
        /// <summary>
        /// 監視対象デバイス情報
        /// </summary>
        public MonitoredDeviceInfoUnit DeviceInfoUnit { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(MonitoredDeviceInfoUnit monitoredDeviceInfoUnit);
    }
}