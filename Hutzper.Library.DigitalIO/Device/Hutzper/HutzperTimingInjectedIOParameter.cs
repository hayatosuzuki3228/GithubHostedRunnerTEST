using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.DigitalIO.Device.Hutzper
{
    [Serializable]
    public record HutzperTimingInjectedIOParameter : ControllerParameterBaseRecord, IDigitalIODeviceParameter
    {
        #region IDigitalIODeviceParameter

        /// <summary>
        /// デバイスID
        /// </summary>
        [IniKey(true, "")]
        public virtual string DeviceID { get; set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public virtual Common.Drawing.Point Location
        {
            get => this.location.Clone();
        }

        /// <summary>
        /// 入力チャネル
        /// </summary>
        [IniKey(true, 0, 1, 2, 3)]
        public int[] InputChannels { get; set; } = new int[] { 0, 1, 2, 3 };

        /// <summary>
        /// 出力チャネル
        /// </summary>
        [IniKey(true, 0, 1, 2, 3)]
        public int[] OutputChannels { get; set; } = new int[] { 0, 1, 2, 3 };

        /// <summary>
        /// 入力フィルタリングミリ秒
        /// </summary>
        [IniKey(true, "")]
        public int[] FilteringTimeMsIntervals { get; set; } = Array.Empty<int>();

        #endregion

        protected Common.Drawing.Point location = new();
        protected string fileNameWithoutExtension;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HutzperTimingInjectedIOParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public HutzperTimingInjectedIOParameter(Common.Drawing.Point location) : this(location, "HutzperTimingInjectedIO")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public HutzperTimingInjectedIOParameter(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"DigitalIO_Device_Y{location.Y + 1:D2}_X{location.X + 1:D2}", "DigitalIODevice", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
        {
            this.IsHierarchy = false;
            this.fileNameWithoutExtension = fileNameWithoutExtension;
            this.location = location.Clone();
        }
    }
}