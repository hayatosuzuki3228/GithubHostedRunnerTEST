using Hutzper.Library.Common.Data;
using Hutzper.Library.DigitalIO.Device;

namespace Hutzper.Library.DigitalIO
{
    public interface IDigitalIODeviceControllerParameter : IControllerParameter
    {
        /// <summary>
        /// デバイスパラメータ
        /// </summary>
        public List<IDigitalIODeviceParameter> DeviceParameters { get; init; }

        /// <summary>
        /// 監視間隔ミリ秒
        /// </summary>
        public List<int> MonitoringIntervalMs { get; init; }
    }
}