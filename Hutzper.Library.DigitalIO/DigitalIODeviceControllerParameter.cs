using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;
using Hutzper.Library.DigitalIO.Device;

namespace Hutzper.Library.DigitalIO
{
    [Serializable]
    public record DigitalIODeviceControllerParameter : ControllerParameterBaseRecord, IDigitalIODeviceControllerParameter
    {
        #region IControllerParameter

        /// <summary>
        /// IIniFileCompatibleリストを取得する
        /// </summary>
        /// <returns></returns>
        public override List<IIniFileCompatible> GetItems() => this.DeviceParameters.ConvertAll(p => (IIniFileCompatible)p);

        #endregion

        /// <summary>
        /// デバイスパラメータ
        /// </summary>
        public List<IDigitalIODeviceParameter> DeviceParameters { get; init; } = new List<IDigitalIODeviceParameter>();

        /// <summary>
        /// 監視間隔ミリ秒
        /// </summary>
        [IniKey(true, new int[0])]
        public List<int> MonitoringIntervalMs { get; init; } = new List<int>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public DigitalIODeviceControllerParameter() : this(typeof(DigitalIODeviceControllerParameter).Name)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public DigitalIODeviceControllerParameter(string fileNameWithoutExtension) : base("DigitalIO_Device_Control".ToUpper(), "DigitalIODeviceControl", $"{fileNameWithoutExtension}.ini")
        {
            this.IsHierarchy = false;
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public override bool Read(IniFileReaderWriter iniFile)
        {
            var isSuccess = base.Read(iniFile);

            foreach (var d in this.DeviceParameters)
            {
                if (d is IIniFileCompatible i)
                {
                    isSuccess &= i.Read(iniFile);
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public override bool Write(IniFileReaderWriter iniFile)
        {
            var isSuccess = base.Write(iniFile);

            foreach (var d in this.DeviceParameters)
            {
                if (d is IIniFileCompatible i)
                {
                    isSuccess &= i.Write(iniFile);
                }
            }

            return isSuccess;
        }
    }
}