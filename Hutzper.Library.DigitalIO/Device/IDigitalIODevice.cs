using Hutzper.Library.Common.Controller;

namespace Hutzper.Library.DigitalIO.Device
{
    /// <summary>
    /// デジタルIOデバイスインタフェース
    /// </summary>
    public interface IDigitalIODevice : IController
    {
        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; }

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; protected set; }

        /// <summary>
        /// 有効か
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object>? Disabled;
    }
}