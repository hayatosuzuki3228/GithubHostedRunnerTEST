using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.DigitalIO.IO.Configuration
{
    /// <summary>
    /// DIO構成
    /// </summary>
    /// <remarks>使用するデバイスを指定します</remarks>
    [Serializable]
    public record DigitalIOConfiguration : IniFileCompatible<DigitalIOConfiguration>
    {
        /// <summary>
        /// デバイスリスト
        /// </summary>
        [IniKey(true, new string[] { "MoxaE1200" })]
        public string[] Devices { get; set; }

        /// <summary>
        /// 画像取得トリガーデバイスインデックス
        /// </summary>
        [IniKey(true, 0)]
        public int AcquisitionTriggerDeviceIndex { get; set; } = 0;

        /// <summary>
        /// 画像取得トリガー入力インデックス
        /// </summary>
        [IniKey(true, 0)]
        public int AcquisitionTriggerInputIndex { get; set; } = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DigitalIOConfiguration() : base("DigitalIO_Configuration".ToUpper())
        {
            this.Devices = new string[] { "MoxaE1200" };
        }
    }
}