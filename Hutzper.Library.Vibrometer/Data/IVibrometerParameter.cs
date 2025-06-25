using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.Vibrometer.Data
{
    /// <summary>
    /// 振動計パラメータインタフェース
    /// </summary>
    public interface IVibrometerParameter : IIniFileCompatible, IControllerParameter
    {
        /// <summary>
        /// 監視開始遅延ミリ秒
        /// </summary>
        [IniKey(true, 1000)]
        public int MonitoringStartDelayMs { get; set; }

        /// <summary>
        /// 主軸回転信号を使用するかどうか
        /// </summary>
        [IniKey(true, true)]
        public bool UseSignalOfSpindleRotation { get; set; }

        /// <summary>
        /// ATC信号を使用するかどうか
        /// </summary>
        [IniKey(true, true)]
        public bool UseSignalOfAutomaticToolChanger { get; set; }
    }
}