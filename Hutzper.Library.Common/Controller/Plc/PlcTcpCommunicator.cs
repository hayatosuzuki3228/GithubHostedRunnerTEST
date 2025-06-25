using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Diagnostics;
using Hutzper.Library.Common.Net.Sockets;
using System.Diagnostics;

namespace Hutzper.Library.Common.Controller.Plc
{
    /// <summary>
    /// PLC制御クラス
    /// </summary>
    /// <typeparam name="T">使用するIPlcController</typeparam>
    public class PlcTcpCommunicator<T> : ControllerBase, IPlcTcpCommunicator where T : IPlcDeviceReaderWriter, new()
    {
        #region サブクラス

        /// <summary>
        /// スレッド制御
        /// </summary>
        protected class ThreadControlSet : SafelyDisposable
        {
            #region プロパティ

            /// <summary>
            /// 待機イベント
            /// </summary>
            public readonly AutoResetEvent WaitEvent;

            /// <summary>
            /// 終了フラグ
            /// </summary>
            public bool IsTerminated { get; protected set; }

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public ThreadControlSet()
            {
                this.WaitEvent = new AutoResetEvent(false);
                this.IsTerminated = false;
            }

            #endregion

            #region パブリックメソッド

            /// <summary>
            /// 停止
            /// </summary>    		    		    		
            public void Terminate()
            {
                this.IsTerminated = true;
                this.WaitEvent.Set();
            }

            #endregion

            #region ISafelyDisposable

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.WaitEvent.Close();
                }
                catch
                {
                }
            }

            #endregion
        }

        #endregion

        #region IPlcTcpCommunicator

        #region プロパティ

        /// <summary>
        /// 有効な状態かどうか
        /// </summary>
        public bool Enabled => false == (this.ThreadControl?.IsTerminated ?? true);

        /// <summary>
        /// 処理状態
        /// </summary>
        /// <remarks>最後に実行された読み書き処理の成否を示す</remarks>
        public bool IsProcessingCorrectly { get; private set; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        /// <remarks>IsRefreshCorrectlyプロパティがfalseの場合に参照してください</remarks>
        public string LastErrorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// 最終読み込み日時
        /// </summary>
        public DateTime LastReadDateTime { get; private set; }

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
        public void WriteBitDevice(params PlcWriteRequest[] request) => this.ValidateAndProcessWriteRequests(new List<PlcWriteRequest>(request), false);

        /// <summary>
        /// ワードデバイス書き込み
        /// </summary>
        /// <param name="request">書き込み要求</param>
        /// <returns>要求の成否</returns>
        public void WriteWordDevice(params PlcWriteRequest[] request) => this.ValidateAndProcessWriteRequests(new List<PlcWriteRequest>(request), true);

        /// <summary>
        /// 強制更新指示
        /// </summary>
        /// <remarks>監視間隔のwaitをbreakします</remarks>
        public void Flush() => this.ThreadControl?.WaitEvent.Set();

        /// <summary>
        /// ビットデバイスの瞬時値を取得する
        /// </summary>
        /// <returns>ビットデバイスの瞬時値</returns>
        public List<List<int>> GetInstantBitValues()
        {
            var listUnit = new List<List<int>>();

            lock (this.SyncRootReading)
            {
                foreach (var selectedUnit in this.LatestDeviceValues[false])
                {
                    listUnit.Add(new List<int>(selectedUnit));
                }
            }

            return listUnit;
        }

        /// <summary>
        /// ビットデバイスの指定ユニットの瞬時値を取得する
        /// </summary>
        /// <param name="unitIndex">対象のユニットインデックス</param>
        /// <returns>ビットデバイスの瞬時値</returns>
        public List<int> GetInstantBitValues(int unitIndex)
        {
            var listValues = new List<int>();

            lock (this.SyncRootReading)
            {
                listValues = new List<int>(this.LatestDeviceValues[false][unitIndex]);
            }

            return listValues;
        }

        /// <summary>
        /// ワードデバイスの瞬時値を取得する
        /// </summary>
        /// <returns>ワードデバイスの瞬時値</returns>
        public List<List<int>> GetInstantWordValues()
        {
            var listUnit = new List<List<int>>();

            lock (this.SyncRootReading)
            {
                foreach (var selectedUnit in this.LatestDeviceValues[true])
                {
                    listUnit.Add(new List<int>(selectedUnit));
                }
            }

            return listUnit;
        }

        /// <summary>
        /// ワードデバイスの指定ユニットの瞬時値を取得する
        /// </summary>
        /// <param name="unitIndex">対象のユニットインデックス</param>
        /// <returns>ワードデバイスの瞬時値</returns>
        public List<int> GetInstantWordValues(int unitIndex)
        {
            var listValues = new List<int>();

            lock (this.SyncRootReading)
            {
                listValues = new List<int>(this.LatestDeviceValues[true][unitIndex]);
            }

            return listValues;
        }

        #endregion

        #endregion

        #region フィールド

        private T? DeviceReaderWriter;   //  デバイス読み書き

        private IPlcTcpCommunicatorParameter? Parameter; // パラメーター

        private ThreadControlSet? ThreadControl; // スレッド制御

        protected object SyncRootWriting = new object();    // 書き込み同期用オブジェクト
        protected object SyncRootReading = new object();    // 読み込み同期用オブジェクト

        protected Dictionary<bool, List<List<int>>> LatestDeviceValues;   // デバイス読み込み値リスト
        protected Dictionary<bool, List<List<PlcWriteRequest>>> WriteRequestList; // デバイス書き込み要求リスト

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public PlcTcpCommunicator(int index = -1) : base(typeof(PlcTcpCommunicator<T>).Name, index)
        {
            this.ThreadControl = new ThreadControlSet();
            this.ThreadControl.Terminate();

            // デバイス値リストを初期化する
            this.LatestDeviceValues = new Dictionary<bool, List<List<int>>>
            {
                { true, new List<List<int>>() },
                { false, new List<List<int>>() }
            };

            // デバイス書き込み要求を初期化する
            this.WriteRequestList = new Dictionary<bool, List<List<PlcWriteRequest>>>
            {
                { true, new List<List<PlcWriteRequest>>() },
                { false, new List<List<PlcWriteRequest>>() }
            };
        }

        #endregion

        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            try
            {
                base.Initialize(serviceCollection);

                if (this.DeviceReaderWriter is null)
                {
                    this.DeviceReaderWriter = new T();
                    this.DeviceReaderWriter.Disconnected += this.DeviceReaderWriter_Disconnected;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is IPlcTcpCommunicatorParameter plcParameter)
                {
                    this.Parameter = plcParameter;

                    // デバイス値リストを初期化する
                    this.LatestDeviceValues = new Dictionary<bool, List<List<int>>>
                    {
                        { true, new List<List<int>>() },
                        { false, new List<List<int>>() }
                    };
                    foreach (var selectedUnit in this.Parameter.AllDeviceUnits)
                    {
                        var listUnitValues = this.LatestDeviceValues[selectedUnit.Type.IsWord()];

                        listUnitValues.Add(Enumerable.Repeat(0, selectedUnit.RangeLength).ToList());
                    }

                    // デバイス書き込み要求を初期化する
                    this.WriteRequestList = new Dictionary<bool, List<List<PlcWriteRequest>>>
                    {
                        { true, new List<List<PlcWriteRequest>>() },
                        { false, new List<List<PlcWriteRequest>>() }
                    };
                    foreach (var selectedUnit in this.Parameter.AllDeviceUnits)
                    {
                        var listUnitWriteRequest = this.WriteRequestList[selectedUnit.Type.IsWord()];

                        listUnitWriteRequest.Add(new List<PlcWriteRequest>());
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            if (this.Parameter is null || this.DeviceReaderWriter is null)
            {
                return false;
            }

            if (true == this.Enabled)
            {
                return this.Enabled;
            }

            // スレッド制御
            var control = this.ThreadControl = new ThreadControlSet();

            // 監視スレッド
            var monitoringThread = new Thread(() =>
            {
                try
                {
                    var connectionAttemptStopWatch = (Stopwatch?)null;      // 接続試行時間計測
                    var monitoringControlStopWatch = Stopwatch.StartNew();  // 監視間隔時間計測

                    var performanceCalculator = new FpsCalculator() { RequiredFrameNumber = 1000 }; // パフォーマンス計算

                    // LUT:ワードデバイス
                    var lutDevWord = new Dictionary<PlcDeviceType, WordDeviceType>
                    {
                        { PlcDeviceType.W, WordDeviceType.W },
                        { PlcDeviceType.D, WordDeviceType.D }
                    };

                    // LUT:ビットデバイス
                    var lutDevBit = new Dictionary<PlcDeviceType, BitDeviceType>
                    {
                        { PlcDeviceType.B, BitDeviceType.B },
                        { PlcDeviceType.M, BitDeviceType.M }
                    };

                    while (false == control.IsTerminated)
                    {
                        // 監視の待機時間を算出する             
                        var waitMs = this.Parameter.MonitoringIntervalMs - Convert.ToInt32(monitoringControlStopWatch.ElapsedMilliseconds);

                        // 待機が必要な場合                    
                        if (0 < waitMs)
                        {
                            // 監視間隔を経過するまでもしくは強制解除が発生するまで待機する
                            control.WaitEvent.WaitOne(waitMs);
                        }

                        // 停止指示がある場合
                        if (true == control.IsTerminated)
                        {
                            break;
                        }

                        // 未接続の場合
                        if (false == this.DeviceReaderWriter.Enabled)
                        {
                            #region 接続処理

                            // 初回の接続もしくは前回の接続処理からの経過時間が再接続間隔を超えている場合                        
                            if ((null == connectionAttemptStopWatch) || (this.Parameter.ConnectionAttemptIntervalSec * 1000 < connectionAttemptStopWatch.ElapsedMilliseconds))
                            {
                                // 接続に成功した場合
                                if (true == this.DeviceReaderWriter.Open(this.Parameter.IpAddress, this.Parameter.PortNumber))
                                {
                                    // 接続イベントを通知する
                                    this.OnConnected();

                                    // 受信タイムアウトを設定する
                                    this.DeviceReaderWriter.TimeoutMs = this.Parameter.ReceiveTimeoutMs;
                                }

                                // 初回の接続処理の場合
                                if (null == connectionAttemptStopWatch)
                                {
                                    // 時間計測を開始する                                
                                    connectionAttemptStopWatch = Stopwatch.StartNew();
                                }
                                // 再接続処理の場合
                                else
                                {
                                    // 時間計測をリセットする
                                    connectionAttemptStopWatch.Reset();
                                    connectionAttemptStopWatch.Start();
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region 読み書き処理

                            var isProcessingCorrectly = true;   // 変化時のみ通知するためのフラグ
                            var errorMessage = string.Empty;
                            var readerWriter = this.DeviceReaderWriter;

                            try
                            {
                                // デバイス読み込み値リストを初期化する
                                var tempDeviceValues = new Dictionary<bool, List<List<int>>>
                                {
                                    { true, new List<List<int>>() },  // ワードデバイス
                                    { false, new List<List<int>>() } // ビットデバイス
                                };

                                #region ユニット単位のデバイス読み込み
                                foreach (var selectedUnit in this.Parameter.AllDeviceUnits)
                                {
                                    var devWord = selectedUnit.Type.IsWord();
                                    var devText = $"{selectedUnit.Type.ToString()}{selectedUnit.StartingAddress:D5}";
                                    var selectedUnitValuesList = tempDeviceValues[devWord];

                                    var result = false;
                                    var selectedUnitValues = new List<int>();

                                    // 無効なユニット
                                    if (false == selectedUnit.Enabled)
                                    {
                                        Serilog.Log.Verbose($"device reading invalid device {devText}");
                                        selectedUnitValuesList.Add(selectedUnitValues);
                                        continue;
                                    }

                                    // ユニットがワードデバイスの場合
                                    if (true == devWord)
                                    {
                                        // ワードデバイス読み込み
                                        result = readerWriter.ReadDevice(lutDevWord[selectedUnit.Type], selectedUnit.StartingAddress, selectedUnit.RangeLength, out selectedUnitValues, out errorMessage);
                                    }
                                    // ユニットがビットデバイスの場合
                                    else
                                    {
                                        result = readerWriter.ReadDevice(lutDevBit[selectedUnit.Type], selectedUnit.StartingAddress, selectedUnit.RangeLength, out selectedUnitValues, out errorMessage);
                                    }

                                    // デバイス読み込みに成功した場合
                                    if (true == result)
                                    {
                                        // 最新のデバイス値を更新する
                                        lock (this.SyncRootReading)
                                        {
                                            this.LatestDeviceValues[devWord][selectedUnit.Index] = new List<int>(selectedUnitValues);
                                        }
                                    }
                                    // デバイス読み込みに失敗した場合
                                    else
                                    {
                                        selectedUnitValues.Clear();
                                        isProcessingCorrectly = false;
                                        if (true == this.IsProcessingCorrectly)
                                        {
                                            Serilog.Log.Warning($"device reading failed {devText}");
                                        }
                                    }

                                    this.OnDeviceValueUpdated(devWord, result, selectedUnit.Index);

                                    selectedUnitValuesList.Add(selectedUnitValues);
                                }
                                #endregion

                                #region デバイス書き込み要求を取得する
                                var tempDeviceWriteRequests = new Dictionary<bool, List<List<PlcWriteRequest>>>
                                {
                                    { true, new List<List<PlcWriteRequest>>() },    // ワードデバイス
                                    { false, new List<List<PlcWriteRequest>>() }    // ビットデバイス
                                };
                                lock (this.SyncRootWriting)
                                {
                                    foreach (var selectedUnit in this.Parameter.AllDeviceUnits)
                                    {
                                        var devWord = selectedUnit.Type.IsWord();
                                        var selectedUnitWriteRequests = this.WriteRequestList[devWord][selectedUnit.Index];

                                        var selectedTempWriteRequests = tempDeviceWriteRequests[devWord];
                                        selectedTempWriteRequests.Add(new List<PlcWriteRequest>(selectedUnitWriteRequests));

                                        selectedUnitWriteRequests.Clear();
                                    }
                                }
                                #endregion

                                #region ユニット単位のデバイス書き込み
                                foreach (var selectedUnit in this.Parameter.AllDeviceUnits)
                                {
                                    var devWord = selectedUnit.Type.IsWord();
                                    var devText = $"{selectedUnit.Type}{selectedUnit.StartingAddress:D5}";

                                    // 最新の読み取り値を初期値とし、要求に従って書き込み値で上書きする
                                    var selectedUnitRequests = tempDeviceWriteRequests[devWord][selectedUnit.Index];
                                    var selectedUnitValues = tempDeviceValues[devWord][selectedUnit.Index];

                                    if ((0 < selectedUnitRequests.Count) && (0 < selectedUnitValues.Count))
                                    {
                                        // 要求に従って書き込み値を作成
                                        foreach (var selectedRequest in selectedUnitRequests)
                                        {
                                            selectedUnitValues[selectedRequest.RelativeAddress] = selectedRequest.Value;
                                        }

                                        var result = false;

                                        // ユニットがワードデバイスの場合
                                        if (true == devWord)
                                        {
                                            result = readerWriter.WriteDevice(lutDevWord[selectedUnit.Type], selectedUnit.StartingAddress, selectedUnit.RangeLength, selectedUnitValues, out errorMessage);
                                        }
                                        // ユニットがビットデバイスの場合
                                        else
                                        {
                                            result = readerWriter.WriteDevice(lutDevBit[selectedUnit.Type], selectedUnit.StartingAddress, selectedUnit.RangeLength, selectedUnitValues, out errorMessage);
                                        }

                                        // デバイス書き込みに失敗した場合
                                        if (false == result)
                                        {
                                            isProcessingCorrectly = false;
                                            if (true == this.IsProcessingCorrectly)
                                            {
                                                Serilog.Log.Warning($"device writing failed {devText}");
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                isProcessingCorrectly = false;
                                Serilog.Log.Warning(ex, $"device processing failed");
                            }
                            finally
                            {
                                if (true == performanceCalculator.AddFrame())
                                {
                                    Serilog.Log.Debug($"plc processing {performanceCalculator.Result:F1}Hz");
                                }

                                // 状態フラグを更新する
                                var isStatusChanged = !this.IsProcessingCorrectly.Equals(isProcessingCorrectly);
                                this.LastReadDateTime = DateTime.Now;
                                this.IsProcessingCorrectly = isProcessingCorrectly;

                                // 状態が変化した場合
                                if (true == isStatusChanged)
                                {
                                    // 状態変化イベントを通知する
                                    if (false == this.IsProcessingCorrectly)
                                    {
                                        this.LastErrorMessage = errorMessage;
                                    }
                                    this.OnProcessingStatusChanged(this.IsProcessingCorrectly, errorMessage);
                                }
                            }

                            #endregion
                        }

                        // 時間計測をリセットする
                        monitoringControlStopWatch.Restart();
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    control?.Dispose();
                    this.DeviceReaderWriter.Close();
                }
            })
            { IsBackground = true };

            monitoringThread.Start();

            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            this.ThreadControl?.Terminate();
            this.ThreadControl = null;
            return true;
        }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.Close();

                if (this.DeviceReaderWriter is not null)
                {
                    this.DeviceReaderWriter.Disconnected -= this.DeviceReaderWriter_Disconnected;
                    this.DeviceReaderWriter.Dispose();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IPlcDeviceReaderWriter

        private void DeviceReaderWriter_Disconnected(object sender) => this.OnDisconnected();

        #endregion

        #region protectedメソッド   

        /// <summary>
        /// 書き込み要求を検証して処理する
        /// </summary>
        /// <param name="writeRequests">書き込み要求リスト</param>
        /// <remarks>書き込みが許可されていないアドレスへの要求は無視されます</remarks>
        protected virtual void ValidateAndProcessWriteRequests(List<PlcWriteRequest> writeRequests, bool isWordDevice)
        {
            if (this.Parameter is null)
            {
                return;
            }

            if (writeRequests is null || 0 >= writeRequests.Count)
            {
                return;
            }

            try
            {
                var deviceUnits = isWordDevice ? this.Parameter.WordDeviceUnits : this.Parameter.BitDeviceUnits;

                if (0 < deviceUnits.Length)
                {
                    // 有効要求リストを初期化する
                    var validWriteRequest = new List<List<PlcWriteRequest>>();
                    for (int i = 0; i < deviceUnits.Length; i++)
                    {
                        validWriteRequest.Add(new List<PlcWriteRequest>());
                    }

                    // 範囲チェックを行う
                    foreach (var selectedRequest in writeRequests)
                    {
                        var selectedUnit = deviceUnits[selectedRequest.UnitIndex];

                        // 書き込み不可の場合
                        if (false == selectedUnit.Enabled || false == selectedUnit.Access.IsWritable())
                        {
                            Serilog.Log.Warning($"write request to a non-writable device. {(isWordDevice ? "WORD" : "BIT")}_{selectedUnit.Index + 1:D2}_{selectedUnit.Type}_{selectedUnit.StartingAddress:D5}");
                        }
                        // 書き込み範囲外の場合
                        else if (selectedUnit.RangeLength <= selectedRequest.RelativeAddress)
                        {
                            Serilog.Log.Warning($"write request out of range. {(isWordDevice ? "WORD" : "BIT")}_{selectedUnit.Index + 1:D2}_{selectedUnit.Type}_{selectedUnit.StartingAddress:D5}");
                        }
                        // 有効な書き込み要求の場合
                        else
                        {
                            Serilog.Log.Verbose($"write request. {(isWordDevice ? "WORD" : "BIT")}_{selectedUnit.Index + 1:D2}_{selectedUnit.Type}_{selectedUnit.StartingAddress:D5}");
                            validWriteRequest[selectedUnit.Index].Add(new PlcWriteRequest(selectedRequest));
                        }
                    }

                    // 書き込み要求を設定する
                    lock (this.SyncRootWriting)
                    {
                        var selectedRequests = this.WriteRequestList[isWordDevice];
                        foreach (var unit in deviceUnits)
                        {
                            selectedRequests[unit.Index].AddRange(validWriteRequest[unit.Index]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, $"error in device write request");
            }
        }

        protected virtual void OnConnected()
        {
            try
            {
                this.Connected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        protected virtual void OnDisconnected()
        {
            try
            {
                this.Disconnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }


        protected virtual void OnProcessingStatusChanged(bool status, string message)
        {
            try
            {
                this.ProcessingStatusChanged?.Invoke(this, status, message);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        protected virtual void OnDeviceValueUpdated(bool isWord, bool status, int unitIndex)
        {
            try
            {
                if (true == isWord)
                {
                    this.WordDeviceValueUpdated?.Invoke(this, status, unitIndex);
                }
                else
                {
                    this.BitDeviceValueUpdated?.Invoke(this, status, unitIndex);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}