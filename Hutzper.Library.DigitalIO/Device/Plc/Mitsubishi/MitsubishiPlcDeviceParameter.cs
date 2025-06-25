using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using static Hutzper.Library.DigitalIO.Device.Plc.Mitsubishi.MitsubishiPlcDevice;

namespace Hutzper.Library.DigitalIO.Device.Plc.Mitsubishi
{
    public record MitsubishiPlcDeviceParameter : ControllerParameterBaseRecord, IDigitalIODeviceParameter
    {
        [IniKey(true, "FX5UC")]
        public virtual string DeviceID { get; set; } = string.Empty;

        [IniKey(true, "192.168.3.250")]
        public string IpAddress { get; set; } = "192.168.3.250";

        [IniKey(true, 8000)]
        public int PortNumber { get; set; } = 8000;

        [IniKey(true, "")]
        public int[] InputChannels { get; set; } = Array.Empty<int>();

        [IniKey(true, "")]
        public int[] OutputChannels { get; set; } = Array.Empty<int>();

        [IniKey(true, 5, 5, 5, 5)]
        public int[] FilteringTimeMsIntervals { get; set; } = Enumerable.Repeat(50, 4).ToArray();

        [IniKey(true, false)]
        public bool UseUseDetailedSetting { get; set; } = false;
        public List<DeviceNumberPair>? ReadDeviceNumberPairs { get; set; }
        public List<DeviceNumberPair>? WriteDeviceNumberPairs { get; set; }
        public bool IsBinary { get; set; } = false;
        protected string fileNameWithoutExtension;

        protected Common.Drawing.Point location = new();

        public virtual Common.Drawing.Point Location
        {
            get => this.location.Clone();
        }

        public MitsubishiPlcDeviceParameter() : this(new Common.Drawing.Point())
        {
        }

        public MitsubishiPlcDeviceParameter(Common.Drawing.Point location) : this(location, "FX5UC")
        {
        }

        public MitsubishiPlcDeviceParameter(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"DigitalIO_Device_Y{location.Y + 1:D2}_X{location.X + 1:D2}", " DigitalIODevice", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
        {
            this.IsHierarchy = false;
            this.fileNameWithoutExtension = fileNameWithoutExtension;
            this.location = location.Clone();
        }
    }
}