using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.Common.Controller.Plc
{
    /// <summary>
    /// PLC通信制御設定
    /// </summary>
    public record PlcTcpCommunicatorParameter : ControllerParameterBaseRecord, IPlcTcpCommunicatorParameter
    {
        #region IControllerParameter

        /// <summary>
        /// IIniFileCompatibleリストを取得する
        /// </summary>
        /// <returns></returns>
        public override List<IIniFileCompatible> GetItems() => Array.ConvertAll(this.AllDeviceUnits, p => (IIniFileCompatible)p).ToList();

        #endregion

        #region IPlcTcpCommunicatorParameter

        /// <summary>
        /// IPアドレス
        /// </summary>
        [IniKey(true, "127.0.0.1")]
        public string IpAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// ポート番号
        /// </summary>
        [IniKey(true, 50002)]
        public int PortNumber { get; set; } = 50002;

        /// <summary>
        /// 監視間隔ミリ秒
        /// </summary>
        [IniKey(true, 5)]
        public int MonitoringIntervalMs { get; set; } = 5;

        /// <summary>
        /// 再接続インターバル秒
        /// </summary>
        [IniKey(true, 5)]
        public int ConnectionAttemptIntervalSec { get; set; } = 5;

        /// <summary>
        /// 受信タイムアウトミリ秒
        /// </summary>
        [IniKey(true, 1000)]
        public int ReceiveTimeoutMs { get; set; } = 1000;

        /// <summary>
        /// 連続デバイスセット
        /// </summary>
        public PlcDeviceUnit[] AllDeviceUnits
        {
            get
            {
                var units = new List<PlcDeviceUnit>();

                if (this.BitDeviceUnits is not null)
                {
                    units.AddRange(this.BitDeviceUnits);
                }

                if (this.WordDeviceUnits is not null)
                {
                    units.AddRange(this.WordDeviceUnits);
                }

                return units.ToArray();
            }
        }

        /// <summary>
        /// 連続ビットデバイスセット
        /// </summary>
        public PlcDeviceUnit[] BitDeviceUnits { get; set; }

        /// <summary>
        /// 連続ワードデバイスセット
        /// </summary>
        public PlcDeviceUnit[] WordDeviceUnits { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public PlcTcpCommunicatorParameter() : this(typeof(PlcTcpCommunicatorParameter).Name)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public PlcTcpCommunicatorParameter(string fileNameWithoutExtension) : base("Plc_Communication".ToUpper(), typeof(PlcTcpCommunicatorParameter).Name, $"{fileNameWithoutExtension}.ini")
        {
            this.IsHierarchy = false;

            // 初期設定をしやすいように1ユニットずつ用意しておく
            this.BitDeviceUnits = new[] { new PlcDeviceUnit(0, false) };
            this.WordDeviceUnits = new[] { new PlcDeviceUnit(0, true) };
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public override bool Read(IniFileReaderWriter iniFile)
        {
            var isSuccess = base.Read(iniFile);

            foreach (var isWord in new[] { false, true })
            {
                var unitList = new List<PlcDeviceUnit>();
                var unitNext = new PlcDeviceUnit(unitList.Count, isWord);
                foreach (var s in iniFile.GetSections())
                {
                    if (true == s.Equals(unitNext.Section))
                    {
                        if (true == unitNext.Read(iniFile))
                        {
                            unitList.Add(unitNext);
                            unitNext = new PlcDeviceUnit(unitList.Count, isWord);
                        }
                    }
                }

                if (true == isWord)
                {
                    this.WordDeviceUnits = unitList.ToArray();
                }
                else
                {
                    this.BitDeviceUnits = unitList.ToArray();
                }
            }

            if (0 >= this.BitDeviceUnits.Length)
            {
                this.BitDeviceUnits = new[] { new PlcDeviceUnit(0, false) };
            }

            if (0 >= this.WordDeviceUnits.Length)
            {
                this.WordDeviceUnits = new[] { new PlcDeviceUnit(0, true) };
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

            foreach (var d in this.AllDeviceUnits)
            {
                if (d is IIniFileCompatible i)
                {
                    isSuccess &= i.Write(iniFile);
                }
            }

            return isSuccess;
        }

        #endregion
    }
}