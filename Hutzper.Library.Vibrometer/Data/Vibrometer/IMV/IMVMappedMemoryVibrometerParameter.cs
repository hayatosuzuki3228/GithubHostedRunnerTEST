using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.Vibrometer.Data.Vibrometer.IMV
{
    /// <summary>
    /// IMV製 マップドメモリ通信 振動計クラス パラメータクラス
    /// </summary>
    [Serializable]
    public record IMVMappedMemoryVibrometerParameter : IniFileCompatible<IMVMappedMemoryVibrometerParameter>, IVibrometerParameter
    {
        #region IControllerParameter

        /// <summary>
        /// データの保存に階層構造(サブディレクトリを用いるか)
        /// </summary>
        public bool IsHierarchy { get; init; } = false;

        /// <summary>
        /// ディレクトリ名
        /// </summary>
        public string DirectoryKey { get; init; } = string.Empty;

        /// <summary>
        /// 管理ファイル名
        /// </summary>
        public string[] FileNames { get; init; } = Array.Empty<string>();

        /// <summary>
        /// IIniFileCompatibleリストを取得する
        /// </summary>
        /// <returns></returns>
        public virtual List<IIniFileCompatible> GetItems() => new();

        #endregion

        #region IVibrometerParameter

        /// <summary>
        /// 監視開始遅延ミリ秒
        /// </summary>
        [IniKey(true, 1000)]
        public int MonitoringStartDelayMs { get; set; } = 1000;

        /// <summary>
        /// 主軸回転信号を使用するかどうか
        /// </summary>
        [IniKey(true, true)]
        public bool UseSignalOfSpindleRotation { get; set; }

        /// <summary>
        /// ATC信号を使用するかどうか
        /// </summary>
        [IniKey(true, true)]
        public bool UseSignalOfAutomaticToolChanger { get; set; }

        #endregion

        #region IEquatable

        public virtual bool Equals(IControllerParameter? compalison)
        {
            var items1 = this.GetItems();
            var items2 = compalison?.GetItems() ?? new(); ;

            if (items1.Count != items2.Count)
            {
                return false;
            }

            foreach (var i in Enumerable.Range(0, items1.Count))
            {
                if (false == items1[i].Equals(items2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(base.GetHashCode());

            this.GetItems().ForEach(item => hashCode.Add(item));

            return hashCode.ToHashCode();
        }

        #endregion

        /// <summary>
        /// イベント名:ドライバのデータ書き込み
        /// </summary>
        [IniKey(true, "VIBROMETER_MEMORY_AD_DATA")]
        public string NameOfMappedMemory { get; init; } = "VIBROMETER_MEMORY_AD_DATA";
        /// <summary>
        /// イベント名:ドライバのデータ書き込み
        /// </summary>
        [IniKey(true, "_COMMUNICATION_CONTROL_NEW_DATA_")]
        public string EventNameOfMappedMemoryUpdated { get; init; } = "_COMMUNICATION_CONTROL_NEW_DATA_";

        /// <summary>
        /// イベント名:通信ドライバソフトの終了
        /// </summary>
        [IniKey(true, "_COMMUNICATION_CONTROL_APP_END_")]
        public string EventNameOfDriverEnd { get; init; } = "_COMMUNICATION_CONTROL_APP_END_";

        /// <summary>
        /// イベント名:装置停止信号ON
        /// </summary>
        [IniKey(true, "_COMMUNICATION_CONTROL_RY00_ON_")]
        public string EventNameOfStopSignalOn { get; init; } = "_COMMUNICATION_CONTROL_RY00_ON_";

        /// <summary>
        /// イベント名:装置停止信号OFF
        /// </summary>
        [IniKey(true, "_COMMUNICATION_CONTROL_RY00_OFF_")]
        public string EventNameOfStopSignalOff { get; init; } = "_COMMUNICATION_CONTROL_RY00_OFF_";

        /// <summary>
        /// 共有メモリバイトサイズ
        /// </summary>
        [IniKey(true, 20)]
        public long MappedMemoryBytes { get; set; } = 20;

        /// <summary>
        /// 共有メモリオフセット:ADデータ
        /// </summary>
        [IniKey(true, 2)]
        public int AllocationOffsetOfData { get; set; } = 2;

        /// <summary>
        /// 共有メモリオフセット:センサー①入力
        /// </summary>
        [IniKey(true, 10)]
        public int AllocationOffsetOfSensorInput1 { get; set; } = 10;

        /// <summary>
        /// 共有メモリオフセット:センサー②入力
        /// </summary>
        [IniKey(true, 12)]
        public int AllocationOffsetOfSensorInput2 { get; set; } = 12;

        /// <summary>
        /// センサー入力立下り検知遅延
        /// </summary>
        /// <remarks>秒</remarks>
        [IniKey(true, 30)]
        public int SensorInputFallDetectionDelaySecond { get; set; } = 30;

        /// <summary>
        /// ドライバEXE
        /// </summary>
        [IniKey(true, "..\\Driver\\CommControl.exe")]
        public string DriverExecutablePath { get; set; } = "..\\Driver\\CommControl.exe";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IMVMappedMemoryVibrometerParameter() : base("Vibrometer_Parameter".ToUpper()) { }
    }
}