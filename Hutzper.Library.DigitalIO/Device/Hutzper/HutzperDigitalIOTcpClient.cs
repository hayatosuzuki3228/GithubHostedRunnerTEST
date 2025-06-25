using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Net.Sockets;
using System.Diagnostics;
using System.Text;

namespace Hutzper.Library.DigitalIO.Device.Hutzper
{
    /// <summary>
    /// デバッグ用DIO
    /// </summary>
    /// <remarks>ソケット通信でDIOを疑似します</remarks>
    [Serializable]
    public class HutzperDigitalIOTcpClient : ControllerBase, IDigitalIORemoteDevice, IDigitalIOInputDevice, IDigitalIOOutputDevice
    {
        protected enum ClientType
        {
            Read,
            Write
        }

        protected class ClientUnit
        {
            public bool Enabled => this.Client?.Enabled ?? false;

            public TcpTextCommunicationClient? Client;

            public readonly object CommandSync = new();

            public List<string> ReceivedCommand = new();

            public WaitHandle[] ReceivedWaitEvents = Array.Empty<WaitHandle>();

            public ClientUnit(WaitHandle commonHandle)
            {
                this.ReceivedWaitEvents = new WaitHandle[2];
                this.ReceivedWaitEvents[0] = commonHandle;
            }
        }

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

                #region 既にインスタンスが存在する場合は破棄
                try
                {
                    foreach (var c in this.Clients.Values)
                    {
                        if (c.Client is not null)
                        {
                            c.Client.Connected -= this.Client_Connected;
                            c.Client.Disconnected -= this.Client_Disconnected;
                            c.Client.TransferDataRead -= this.Client_TransferDataRead;

                            c.Client.Disconnect();
                            c.Client.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    foreach (var c in this.Clients.Values)
                    {
                        c.Client = null;
                    }
                }
                #endregion

                // 通信クライアント生成
                foreach (var c in this.Clients.Values)
                {
                    c.Client = new TcpTextCommunicationClient(Encoding.UTF8);
                    c.Client.Connected += this.Client_Connected;
                    c.Client.Disconnected += this.Client_Disconnected;
                    c.Client.TransferDataRead += this.Client_TransferDataRead;
                }
            }
            catch (Exception ex)
            {
                foreach (var c in this.Clients.Values)
                {
                    c.Client?.Dispose();
                    c.Client = null;
                }
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="config"></param>
        public override void SetConfig(IApplicationConfig? config)
        {
            try
            {
                base.SetConfig(config);
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

                if (parameter is IDigitalIORemoteDeviceParameter p)
                {
                    this.Parameter = p;
                    this.DeviceID = p.DeviceID;
                    this.Location = p.Location;

                    if (this.Parameter is HutzperDigitalIOTcpClientParameter hp)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
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
                #region 既にインスタンスが存在する場合は破棄
                try
                {
                    foreach (var c in this.Clients.Values)
                    {
                        if (c.Client is not null)
                        {
                            c.Client.Connected -= this.Client_Connected;
                            c.Client.Disconnected -= this.Client_Disconnected;
                            c.Client.TransferDataRead -= this.Client_TransferDataRead;

                            c.Client.Disconnect();
                            c.Client.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    foreach (var c in this.Clients.Values)
                    {
                        c.Client = null;
                    }
                }
                #endregion

                this.terminateEvent?.Set();

                foreach (var c in this.Clients.Values)
                {
                    c.ReceivedWaitEvents.LastOrDefault()?.Close();
                    c.ReceivedWaitEvents = Array.Empty<WaitHandle>();
                }

                this.terminateEvent?.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.terminateEvent?.Close();
            }
        }

        #endregion

        #region IDigitalIODevice

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; protected set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }

        /// <summary>
        /// 有効か
        /// </summary>
        public bool Enabled => this.Clients.Values.All(c => c.Enabled);

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object>? Disabled;

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (this.Parameter is HutzperDigitalIOTcpClientParameter hp)
                {
                    foreach (var c in this.Clients.Values)
                    {
                        c.Client?.Connect(hp.IpAddress, hp.PortNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                if (this.Enabled)
                    this.Connected?.Invoke(this);
            }

            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            var isSuccess = true;

            try
            {
                foreach (var c in this.Clients.Values)
                {
                    c.Client?.Disconnect();
                }

                this.terminateEvent?.Set();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                if (isSuccess)
                {
                    this.Disconnected?.Invoke(this);
                }
            }

            return isSuccess;
        }

        #endregion

        #region IDigitalIOInputDevice

        /// <summary>
        /// 入力点数
        /// </summary>
        public int NumberOfInputs => this.Parameter?.InputChannels.Length ?? 0;

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadInput(out bool[] values)
        {
            var isSuccess = this.ReadInput(out int[] intValues);

            values = Array.ConvertAll(intValues, v => v != 0);

            return isSuccess;
        }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadInput(out int[] values)
        {
            var isSuccess = this.Enabled;
            values = new int[this.NumberOfInputs];

            try
            {
                var c = this.Clients[ClientType.Read];

                if (c.Client is not null && true == c.Client.Enabled)
                {
                    // 読み取るチャンネルを列挙
                    var channelIndex = 0;
                    foreach (var channels in GetContinuousRanges(this.Parameter?.InputChannels ?? Array.Empty<int>()))
                    {
                        // 受信イベント
                        var currentEvent = new AutoResetEvent(false);

                        // コマンド送信
                        var sendCommand = $"RI,{channels.First()},{channels.Count}";
                        lock (c.CommandSync)
                        {
                            c.ReceivedWaitEvents.LastOrDefault()?.Close();
                            c.ReceivedWaitEvents[^1] = currentEvent;

                            c.ReceivedCommand.Clear();

                            c.Client.AysncWrite(sendCommand);
                        }

                        // 受信待ち
                        WaitHandle.WaitAny(c.ReceivedWaitEvents, 1000);

                        // 受信データ取得
                        var targetText = string.Empty;
                        lock (c.CommandSync)
                        {
                            if (c.ReceivedCommand.Count > 0)
                            {
                                targetText = c.ReceivedCommand.First();
                            }
                        }

                        // 受信データ解析
                        var commandSuccess = false;
                        var getValues = new int[channels.Count];
                        if (sendCommand.Length <= targetText.Length)
                        {
                            // 受信項目の分割
                            var splitedText = targetText.Split(",");

                            var cmd = splitedText[0];                   // コマンド
                            var pid = Convert.ToInt32(splitedText[1]);  // チャンネルオフセット
                            var num = splitedText.Length - 3;           // チャンネル数

                            // 予定している受信値の場合
                            if (cmd == "RI" && pid == channels.First() && num == channels.Count)
                            {
                                commandSuccess = true;

                                var bitIndex = 0;
                                foreach (var bitValue in new ArraySegment<string>(splitedText, 3, num))
                                {
                                    getValues[bitIndex++] = Convert.ToInt32(bitValue);
                                }
                            }
                        }

                        // 受信結果
                        isSuccess = commandSuccess;
                        if (false == isSuccess)
                        {
                            break;
                        }

                        // 受信データの反映
                        Array.Copy(getValues, 0, values, channelIndex, channels.Count);

                        channelIndex += channels.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadInput(int index, out bool value)
        {
            var isSuccess = this.ReadInput(index, out int intValue);

            value = intValue != 0;

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadInput(int index, out int value)
        {
            var isSuccess = this.Enabled;
            value = 0;

            try
            {
                if (true == this.Enabled && this.ReadInput(out int[] values))
                {
                    if (values.Length > index)
                    {
                        value = values[index];
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region IDigitalIOOutputDevice

        /// <summary>
        /// 出力点数
        /// </summary>
        public int NumberOfOutputs => this.Parameter?.OutputChannels.Length ?? 0;

        /// <summary>
        /// 全書き込み
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(bool[] values)
        {
            var intValues = Array.ConvertAll(values, v => v ? 1 : 0);

            var isSuccess = this.WriteOutput(intValues);

            return isSuccess;
        }

        /// <summary>
        /// 全書き込み
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int[] values)
        {
            var isSuccess = this.Enabled;

            try
            {
                var c = this.Clients[ClientType.Write];

                if (c.Client is not null && true == c.Client.Enabled)
                {
                    var channelIndex = 0;
                    foreach (var channels in GetContinuousRanges(this.Parameter?.OutputChannels ?? Array.Empty<int>()))
                    {
                        // 受信イベント
                        var currentEvent = new AutoResetEvent(false);

                        // 書き込み値
                        var channelValues = Array.ConvertAll(new ArraySegment<int>(values, channelIndex, channels.Count).ToArray(), v => (v != 0) ? "1" : "0");

                        // コマンド送信
                        var sendCommand = $"WO,{channels.First()},{channels.Count},{string.Join(",", channelValues)}";
                        lock (c.CommandSync)
                        {
                            c.ReceivedWaitEvents.LastOrDefault()?.Close();
                            c.ReceivedWaitEvents[^1] = currentEvent;

                            c.ReceivedCommand.Clear();

                            c.Client.AysncWrite(sendCommand);
                        }

                        // 受信待ち
                        WaitHandle.WaitAny(c.ReceivedWaitEvents, 1000);

                        // 受信データ取得
                        var targetText = string.Empty;
                        lock (c.CommandSync)
                        {
                            if (c.ReceivedCommand.Count > 0)
                            {
                                targetText = c.ReceivedCommand.First();
                            }
                        }

                        // 受信データ解析
                        var commandSuccess = false;
                        if (sendCommand.Length <= targetText.Length)
                        {
                            // 受信項目の分割
                            var splitedText = targetText.Split(",");

                            var cmd = splitedText[0];                   // コマンド
                            var pid = Convert.ToInt32(splitedText[1]);  // チャンネルオフセット
                            var num = splitedText.Length - 3;           // チャンネル数

                            // 予定している受信値の場合
                            if (cmd == "WO" && pid == channels.First() && num == channels.Count)
                            {
                                commandSuccess = true;
                            }
                        }

                        // 受信結果
                        isSuccess = commandSuccess;
                        if (false == isSuccess)
                        {
                            break;
                        }

                        channelIndex += channels.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定書き込み
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int index, bool value)
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    isSuccess = this.WriteOutput(index, value ? 1 : 0);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定書き込み
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int index, int value)
        {
            var isSuccess = this.Enabled;

            try
            {
                var c = this.Clients[ClientType.Write];

                if (c.Client is not null && true == c.Client.Enabled)
                {
                    if (this.Parameter?.OutputChannels.Skip(index).First() is int ch)
                    {
                        // 受信イベント
                        var currentEvent = new AutoResetEvent(false);

                        // 書き込み値
                        var channelValues = (value != 0) ? "1" : "0";

                        // コマンド送信
                        var sendCommand = $"WO,{ch},{1},{string.Join(",", channelValues)}";
                        lock (c.CommandSync)
                        {
                            c.ReceivedWaitEvents.LastOrDefault()?.Close();
                            c.ReceivedWaitEvents[^1] = currentEvent;

                            c.ReceivedCommand.Clear();

                            c.Client.AysncWrite(sendCommand);
                        }

                        // 受信待ち
                        WaitHandle.WaitAny(c.ReceivedWaitEvents, 1000);

                        // 受信データ取得
                        var targetText = string.Empty;
                        lock (c.CommandSync)
                        {
                            if (c.ReceivedCommand.Count > 0)
                            {
                                targetText = c.ReceivedCommand.First();
                            }
                        }

                        // 受信データ解析
                        var commandSuccess = false;
                        if (sendCommand.Length <= targetText.Length)
                        {
                            // 受信項目の分割
                            var splitedText = targetText.Split(",");

                            var cmd = splitedText[0];                   // コマンド
                            var pid = Convert.ToInt32(splitedText[1]);  // チャンネルオフセット
                            var num = splitedText.Length - 3;           // チャンネル数

                            // 予定している受信値の場合
                            if (cmd == "WO" && pid == ch && num == 1)
                            {
                                commandSuccess = true;
                            }
                        }

                        // 受信結果
                        isSuccess = commandSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(out bool[] values)
        {
            var isSuccess = this.ReadOutput(out int[] intValues);

            values = Array.ConvertAll(intValues, v => v != 0);

            return isSuccess;
        }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(out int[] values)
        {
            var isSuccess = this.Enabled;
            values = new int[this.NumberOfOutputs];

            try
            {
                var c = this.Clients[ClientType.Read];

                if (c.Client is not null && true == c.Client.Enabled)
                {
                    // 読み取るチャンネルを列挙
                    var channelIndex = 0;
                    foreach (var channels in GetContinuousRanges(this.Parameter?.OutputChannels ?? Array.Empty<int>()))
                    {
                        // 受信イベント
                        var currentEvent = new AutoResetEvent(false);

                        // コマンド送信
                        var sendCommand = $"RO,{channels.First()},{channels.Count}";
                        lock (c.CommandSync)
                        {
                            c.ReceivedWaitEvents.Last()?.Close();
                            c.ReceivedWaitEvents[^1] = currentEvent;

                            c.ReceivedCommand.Clear();

                            c.Client.AysncWrite(sendCommand);
                        }

                        // 受信待ち
                        WaitHandle.WaitAny(c.ReceivedWaitEvents, 1000);

                        // 受信データ取得
                        var targetText = string.Empty;
                        lock (c.CommandSync)
                        {
                            if (c.ReceivedCommand.Count > 0)
                            {
                                targetText = c.ReceivedCommand.First();
                            }
                        }

                        // 受信データ解析
                        var commandSuccess = false;
                        var getValues = new int[channels.Count];
                        if (sendCommand.Length <= targetText.Length)
                        {
                            // 受信項目の分割
                            var splitedText = targetText.Split(",");

                            var cmd = splitedText[0];                   // コマンド
                            var pid = Convert.ToInt32(splitedText[1]);  // チャンネルオフセット
                            var num = splitedText.Length - 3;           // チャンネル数

                            // 予定している受信値の場合
                            if (cmd == "RO" && pid == channels.First() && num == channels.Count)
                            {
                                commandSuccess = true;

                                var bitIndex = 0;
                                foreach (var bitValue in new ArraySegment<string>(splitedText, 2, num))
                                {
                                    getValues[bitIndex++] = Convert.ToInt32(bitValue);
                                }
                            }
                        }

                        // 受信結果
                        isSuccess = commandSuccess;
                        if (false == isSuccess)
                        {
                            break;
                        }

                        // 受信データの反映
                        Array.Copy(getValues, 0, values, channelIndex, channels.Count);

                        channelIndex += channels.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(int index, out bool value)
        {
            var isSuccess = this.ReadOutput(index, out int intValue);

            value = intValue != 0;

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(int index, out int value)
        {
            var isSuccess = this.Enabled;
            value = 0;

            try
            {
                if (true == this.Enabled && this.ReadOutput(out int[] values))
                {
                    if (values.Length > index)
                    {
                        value = values[index];
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region IDigitalIORemoteDevice

        /// <summary>
        /// 接続イベント
        /// </summary>
        public event Action<object>? Connected;

        /// <summary>
        /// 切断イベント
        /// </summary>
        public event Action<object>? Disconnected;

        /// <summary>
        /// 再接続
        /// </summary>
        public bool IsReconnectable { get; set; }

        /// <summary>
        /// 再接続試行間隔
        /// </summary>
        public int ReconnectionAttemptsIntervalSec { get; set; }

        #endregion

        /// <summary>
        /// 識別
        /// </summary>
        protected Common.Drawing.Point location;

        protected IDigitalIORemoteDeviceParameter? Parameter;

        protected ManualResetEvent terminateEvent = new(false);

        /// <summary>
        /// テキスト通信クライアント
        /// </summary>
        protected Dictionary<ClientType, ClientUnit> Clients;

        protected int NumberOfConnection;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public HutzperDigitalIOTcpClient() : this(typeof(HutzperDigitalIOTcpClient).Name, Common.Drawing.Point.New())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public HutzperDigitalIOTcpClient(Common.Drawing.Point location) : this(typeof(HutzperDigitalIOTcpClient).Name, location)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public HutzperDigitalIOTcpClient(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
        {
            this.location = location.Clone();

            this.Clients = new Dictionary<ClientType, ClientUnit>
            {
                { ClientType.Read, new ClientUnit(this.terminateEvent) },
                { ClientType.Write, new ClientUnit(this.terminateEvent) }
            };
        }

        protected static List<List<int>> GetContinuousRanges(int[] values)
        {
            var ranges = new List<List<int>>();

            var temp = new List<int>();
            if (values != null && values.Length > 0)
            {
                temp.Add(values.First());
                foreach (var v in values.Skip(1))
                {
                    if (temp.Last() + 1 == v)
                    {
                        temp.Add(v);
                    }
                    else
                    {
                        ranges.Add(temp);
                        temp = new List<int>
                        {
                            v
                        };
                    }
                }
            }

            if (ranges.Count <= 0 && temp.Count > 0)
            {
                ranges.Add(temp);
            }

            return ranges;
        }

        #region TcpTextCommunicationClient:イベント

        /// <summary>
        /// 接続イベント
        /// </summary>
        /// <param name="obj"></param>
        private void Client_Connected(object sender)
        {
            try
            {
                if (this.Clients.FirstOrDefault(kvp => kvp.Value.Client?.Equals(sender) ?? false).Value is ClientUnit unit)
                {
                    lock (unit.CommandSync)
                    {
                        unit.ReceivedCommand.Clear();
                    }
                }

                if (2 == Interlocked.Increment(ref this.NumberOfConnection))
                {
                    if (true == this.Enabled)
                    {
                        this.terminateEvent?.Reset();
                        this.Connected?.Invoke(this);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 切断イベント
        /// </summary>
        /// <param name="obj"></param>
        private void Client_Disconnected(object obj)
        {
            try
            {
                if (1 == Interlocked.Decrement(ref this.NumberOfConnection))
                {
                    if (true == this.Enabled)
                    {
                        this.Disabled?.Invoke(this);
                    }

                    this.Disconnected?.Invoke(this);
                }

                if (this.Parameter is HutzperDigitalIOTcpClientParameter hp)
                {
                    if (0 < hp.ReconnectionAttemptsIntervalSec)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await Task.Delay(hp.ReconnectionAttemptsIntervalSec * 1000);

                                if (hp.IsReconnectable)
                                {
                                    this.Open();
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// データ受信
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void Client_TransferDataRead(object sender, string data)
        {
            try
            {
                if (this.Clients.FirstOrDefault(kvp => kvp.Value.Client?.Equals(sender) ?? false).Value is ClientUnit unit)
                {
                    lock (unit.CommandSync)
                    {
                        unit.ReceivedCommand.Add(data);
                    }

                    if (unit.ReceivedWaitEvents[1] is AutoResetEvent ar)
                    {
                        ar.Set();
                    }
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