using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.Vibrometer.Data.Vibrometer.Hutzper
{
    /// <summary>
    /// TCP通信 振動計クラス パラメータクラス
    /// </summary>
    [Serializable]
    public record HutzperTcpClientVibrometerParameter : IniFileCompatible<HutzperTcpClientVibrometerParameter>, IVibrometerParameter
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

        [IniKey(true, "localhost")]
        public string IpAddress { get; set; } = string.Empty;

        [IniKey(true, 50000)]
        public int PortNumber { get; set; }

        public HutzperTcpClientVibrometerParameter() : base("Vibrometer_Parameter".ToUpper())
        {
        }
    }
}