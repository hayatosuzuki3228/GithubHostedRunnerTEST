namespace Hutzper.Library.ImageGrabber.Device
{
    /// <summary>
    /// 画像取得情報
    /// </summary>
    public interface IDeviceInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// シリアル番号
        /// </summary>
        public string SerialNumber { get; set; }
    }
}