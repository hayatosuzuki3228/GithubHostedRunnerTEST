namespace Hutzper.Library.ImageGrabber.Device
{
    /// <summary>
    /// デバイス情報
    /// </summary>
    [Serializable]
    public record DeviceInfoBase : IDeviceInfo
    {
        #region IDeviceInfo

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; } = string.Empty;

        /// <summary>
        /// シリアル番号
        /// </summary>
        public virtual string SerialNumber { get; set; } = string.Empty;

        #endregion
    }
}