using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.LightController.Device
{
    public record LightControllerParameterBase : ControllerParameterBaseRecord, ILightControllerParameter
    {
        #region ILightControllerParameter

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
        /// 照明制御タイプ
        /// </summary>
        [IniKey(true, Library.LightController.LightingOnOffControlType.ExposureSync)]
        public Library.LightController.LightingOnOffControlType OnOffControlType { get; set; } = LightingOnOffControlType.ExposureSync;

        /// <summary>
        /// チャンネル
        /// </summary>
        [IniKey(true, 0)]
        public int Channel { get; set; } = 0;

        /// <summary>
        /// 調光値
        /// </summary>
        [IniKey(true, 0d)]
        public double Modulation { get; set; }

        /// <summary>
        /// 外部トリガー点灯時間μ秒
        /// </summary>
        [IniKey(true, 1000)]
        public int ExternalTriggerStrobeTimeUs { get; set; } = 1000;

        #endregion

        protected Common.Drawing.Point location = new();
        protected string fileNameWithoutExtension;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public LightControllerParameterBase(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"LightControl_Y{location.Y + 1:D2}_X{location.X + 1:D2}", "LightControllerParameter", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
        {
            this.IsHierarchy = false;
            this.fileNameWithoutExtension = fileNameWithoutExtension;
            this.location = location.Clone();
        }
    }
}