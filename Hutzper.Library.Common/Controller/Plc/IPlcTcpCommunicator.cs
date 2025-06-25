using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.Common.Controller.Plc
{
    /// <summary>
    /// PLC通信制御インタフェース
    /// </summary>
    public interface IPlcTcpCommunicator : IController
    {
        #region プロパティ

        /// <summary>
        /// 有効な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// 処理状態
        /// </summary>
        /// <remarks>最後に実行された読み書き処理の成否を示す</remarks>
        public bool IsProcessingCorrectly { get; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        /// <remarks>IsRefreshCorrectlyプロパティがfalseの場合に参照してください</remarks>
        public string LastErrorMessage { get; }

        /// <summary>
        /// 最終読み込み日時
        /// </summary>
        public DateTime LastReadDateTime { get; }

        #endregion

        #region イベント

        /// <summary>
        /// イベント:接続
        /// </summary>
        public event Action<object>? Connected;

        /// <summary>
        /// イベント:切断
        /// </summary>
        public event Action<object>? Disconnected;

        /// <summary>
        /// イベント:処理状態変化
        /// </summary>
        public event Action<object, bool, string>? ProcessingStatusChanged;

        /// <summary>
        /// イベント:ビットデバイス読み込み値更新
        /// </summary>
        public event Action<object, bool, int>? BitDeviceValueUpdated;

        /// <summary>
        /// イベント:ワードデバイス読み込み値更新
        /// </summary>
        public event Action<object, bool, int>? WordDeviceValueUpdated;

        #endregion

        #region メソッド

        /// <summary>
        /// ビットデバイス書き込み
        /// </summary>
        /// <param name="request">書き込み要求</param>
        /// <returns>要求の成否</returns>
        public void WriteBitDevice(params PlcWriteRequest[] request);

        /// <summary>
        /// ワードデバイス書き込み
        /// </summary>
        /// <param name="request">書き込み要求</param>
        /// <returns>要求の成否</returns>
        public void WriteWordDevice(params PlcWriteRequest[] request);

        /// <summary>
        /// 強制更新指示
        /// </summary>
        /// <remarks>監視間隔のwaitをbreakします</remarks>
        public void Flush();

        /// <summary>
        /// ビットデバイスの瞬時値を取得する
        /// </summary>
        /// <returns>ビットデバイスの瞬時値</returns>
        public List<List<int>> GetInstantBitValues();

        /// <summary>
        /// ビットデバイスの瞬時値を取得する
        /// </summary>
        /// <param name="unitIndex">対象のユニットインデックス</param>
        /// <returns>ビットデバイスの瞬時値</returns>
        public List<int> GetInstantBitValues(int unitIndex);

        /// <summary>
        /// ワードデバイスの瞬時値を取得する
        /// </summary>
        /// <returns>ワードデバイスの瞬時値</returns>
        public List<List<int>> GetInstantWordValues();

        /// <summary>
        /// ワードデバイスの瞬時値を取得する
        /// </summary>
        /// <param name="unitIndex">対象のユニットインデックス</param>
        /// <returns>ワードデバイスの瞬時値</returns>
        public List<int> GetInstantWordValues(int unitIndex);

        #endregion
    }

    /// <summary>
    /// 書き込み要求
    /// </summary>
    [Serializable]
    public record PlcWriteRequest
    {
        #region プロパティ

        /// <summary>
        /// ユニット番号
        /// </summary>
        public int UnitIndex { get; set; }

        /// <summary>
        /// 相対アドレス
        /// </summary>
        public int RelativeAddress { get; set; }

        /// <summary>
        /// 書き込み値
        /// </summary>
        public int Value { get; set; }

        #endregion

        #region コンストラクタ

        public PlcWriteRequest(int unitIndex, int relativeAddress, int value)
        {
            this.UnitIndex = unitIndex;
            this.RelativeAddress = relativeAddress;
            this.Value = value;
        }

        public PlcWriteRequest(PlcWriteRequest source)
        {
            this.UnitIndex = source.UnitIndex;
            this.RelativeAddress = source.RelativeAddress;
            this.Value = source.Value;
        }

        #endregion
    }

    /// <summary>
    /// PLC通信制御設定インタフェース
    /// </summary>
    public interface IPlcTcpCommunicatorParameter : IControllerParameter
    {
        #region プロパティ

        /// <summary>
        /// IPアドレス
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// ポート番号
        /// </summary>
        public int PortNumber { get; set; }

        /// <summary>
        /// 監視間隔ミリ秒
        /// </summary>
        public int MonitoringIntervalMs { get; set; }

        /// <summary>
        /// 再接続インターバル秒
        /// </summary>
        public int ConnectionAttemptIntervalSec { get; set; }

        /// <summary>
        /// 受信タイムアウトミリ秒
        /// </summary>
        public int ReceiveTimeoutMs { get; set; }

        /// <summary>
        /// 連続デバイスセット
        /// </summary>
        public PlcDeviceUnit[] AllDeviceUnits { get; }

        /// <summary>
        /// 連続ビットデバイスセット
        /// </summary>
        public PlcDeviceUnit[] BitDeviceUnits { get; set; }

        /// <summary>
        /// 連続ワードデバイスセット
        /// </summary>
        public PlcDeviceUnit[] WordDeviceUnits { get; set; }

        #endregion
    }

    [Serializable]
    public record PlcDeviceUnit : IniFileCompatible<PlcDeviceUnit>
    {
        #region プロパティ

        /// <summary>
        /// 有効なユニットかどうか
        /// </summary>
        public bool Enabled => 0 <= this.StartingAddress && 0 < this.RangeLength;

        /// <summary>
        /// デバイス種
        /// </summary>
        [IniKey(true, PlcDeviceType.W)]
        public PlcDeviceType Type
        {
            get => this.type;
            set
            {
                if (true == this.IsWord && false == value.IsWord())
                {
                    throw new InvalidOperationException("It is not possible to change from a word device to a bit device.");
                }

                if (true == this.IsBit && false == value.IsBit())
                {
                    throw new InvalidOperationException("It is not possible to change from a bit device to a word device.");
                }

                this.type = value;
            }
        }

        /// <summary>
        /// デバイスアクセス種別
        /// </summary>
        [IniKey(true, PlcDeviceAccess.Read)]
        public PlcDeviceAccess Access { get; set; } = PlcDeviceAccess.Read;

        /// <summary>
        /// 先頭アドレス
        /// </summary>
        [IniKey(true, -1)]
        public int StartingAddress { get; set; } = -1;

        /// <summary>
        /// 範囲長
        /// </summary>
        [IniKey(true, 0)]
        public int RangeLength { get; set; } = 0;

        /// <summary>
        /// ワードデバイスかどうか
        /// </summary>
        public bool IsWord => this.Type.IsWord();

        /// <summary>
        /// ビットデバイスかどうか
        /// </summary>
        public bool IsBit => !this.Type.IsWord();

        #endregion

        private PlcDeviceType type = PlcDeviceType.W;   // デバイス種

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public PlcDeviceUnit(int index, bool isWord) : base($"{"plc_device_unit".ToUpper()}_{(isWord ? "WORD" : "BIT")}_{index + 1:D2}")
        {
            base.Index = index;
            this.type = isWord ? PlcDeviceType.W : PlcDeviceType.M;
        }
    }

    /// <summary>
    /// PLCデバイス種
    /// </summary>
    [Serializable]
    public enum PlcDeviceType
    {
        [AliasName("W")]
        W = 0,

        [AliasName("D")]
        D = 1,

        [AliasName("B")]
        B = 2,

        [AliasName("M")]
        M = 3,
    }

    /// <summary>
    /// アクセス種別
    /// </summary>
    [Serializable]
    public enum PlcDeviceAccess
    {
        [AliasName("Read")]
        Read = 0,

        [AliasName("Write")]
        Write = 1,
    }

    /// <summary>
    /// enum拡張
    /// </summary>
    public static class PlcEnumExtension
    {
        public static string ToString(this PlcDeviceType deviceType) => deviceType.StringValueOf();

        public static bool IsBit(this PlcDeviceType deviceType) => !deviceType.IsWord();

        public static bool IsWord(this PlcDeviceType deviceType)
        {
            var convertArray = new[] { true, true, false, false };

            var index = Array.IndexOf(Enum.GetValues(typeof(PlcDeviceType)), deviceType);

            return convertArray[index];
        }

        public static bool IsWritable(this PlcDeviceAccess deviceAccess)
        {
            var convertArray = new[] { false, true };

            var index = Array.IndexOf(Enum.GetValues(typeof(PlcDeviceAccess)), deviceAccess);

            return convertArray[index];
        }
    }
}