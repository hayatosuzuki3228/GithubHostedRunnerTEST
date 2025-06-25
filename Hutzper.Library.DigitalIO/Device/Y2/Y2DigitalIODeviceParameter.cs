using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.DigitalIO.Device.Y2
{
    [Serializable]
    public record Y2DigitalIODeviceParameter : ControllerParameterBaseRecord, IDigitalIODeviceParameter
    {
        #region IDigitalIODeviceParameter

        [IniKey(true, "DIO-8/8B-UBT")]
        public virtual string DeviceID { get; set; } = string.Empty;

        [IniKey(true, new int[0])]
        public int[] InputChannels { get; set; } = Array.Empty<int>();

        [IniKey(true, new int[0])]
        public int[] OutputChannels { get; set; } = Array.Empty<int>();

        #endregion

        [IniKey(true, "DIO-8/8B-UBT")]
        public string ModelName { get; set; } = String.Empty;

        [IniKey(true, 0)]
        public ushort BoardNo { get; set; } = 0;

        protected Common.Drawing.Point location = new();

        protected string fileNameWithoutExtension;

        [IniKey(true, 5, 5, 5, 5)]
        public int[] FilteringTimeMsIntervals { get; set; } = Enumerable.Repeat(50, 4).ToArray();

        /// <summary>
        /// 識別
        /// </summary>
        public virtual Common.Drawing.Point Location
        {
            get => this.location.Clone();
        }
        public Y2DigitalIODeviceParameter() : this(new Common.Drawing.Point())
        {
        }

        public Y2DigitalIODeviceParameter(Common.Drawing.Point location) : this(location, "DIO-8/8B-UBT")
        {
        }

        public Y2DigitalIODeviceParameter(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"DigitalIO_Device_Y{location.Y + 1:D2}_X{location.X + 1:D2}", "DigitalIODevice", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
        {
            this.IsHierarchy = false;
            this.fileNameWithoutExtension = fileNameWithoutExtension;
            this.location = location.Clone();
        }
    }
}