using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Net.Sockets;
using System.Diagnostics;

namespace Hutzper.Library.LightController.Device.Net.Sockets.UTech
{
    /// <summary>
    /// U-TECHNOLOGY 照明電源UPD2450シリーズ
    /// </summary>
    /// <remarks>UPD2450W-4SEI 取扱説明書 4051063001A準拠</remarks>
    public class TcpUTechUpd2450_LightController : LightControllerBase, ITcpLightController
    {
        #region TcpUTechUpd2450_LightController

        /// <summary>
        /// チャンネル数
        /// </summary>
        public readonly int NumberOfChannels = 4;

        #endregion

        #region ILightController

        /// <summary>
        /// 制御が可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public override bool Enabled => this.CommunicationClient.Enabled;

        /// <summary>
        /// オープン
        /// </summary>
        public override bool Open()
        {
            try
            {
                if (Monitor.TryEnter(this.CommunicationClient))
                {
                    try
                    {
                        if (null != this.CommunicationParameter)
                        {
                            if (true == this.CommunicationClient.Connect(this.CommunicationParameter.IpAddress, this.CommunicationParameter.PortNumber))
                            {
                                if (this.CommunicationParameter is ILightControllerParameter)
                                {
                                    #region "調光設定"
                                    // 調光設定反映（レシピの調光設定とは無関係）
                                    this.Modulate(this.CommunicationParameter.Modulation);
                                    #endregion

                                    return true;
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.CommunicationClient);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return false;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        public override bool Close()
        {
            try
            {
                if (Monitor.TryEnter(this.CommunicationClient))
                {
                    try
                    {
                        this.CommunicationClient.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.CommunicationClient);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 点灯
        /// </summary>
        /// <remarks>Channelプロパティに従う</remarks>
        public override bool TurnOn() => this.TurnOnOff(true, this.Channel);

        /// <summary>
        /// 点灯(ch指定)
        /// </summary>
        /// <param name="channel">対象のチャンネル 0～3</param>
        /// <returns></returns>
        public override bool TurnOn(int channel) => this.TurnOnOff(true, channel);

        /// <summary>
        /// 消灯
        /// </summary>
        /// <remarks>Channelプロパティに従う</remarks>
        public override bool TurnOff() => this.TurnOnOff(false, this.Channel);

        /// <summary>
        /// 消灯(ch指定)
        /// </summary>
        /// <param name="channel">対象のチャンネル 0～3</param>
        public override bool TurnOff(int channel) => this.TurnOnOff(false, channel);

        /// <summary>
        /// 調光
        /// </summary>
        /// <remarks>Channelプロパティに従う</remarks>
        public override bool Modulate(double value) => this.Modulate(this.Channel, value);

        /// <summary>
        /// 調光(ch指定)
        /// </summary>
        /// <param name="channel">対象のチャンネル 0～3</param>
        /// <param name="value">調光値 0～255</param>
        public override bool Modulate(int channel, double value)
        {
            var isSuccess = false;

            try
            {
                if (false == this.Enabled)
                {
                    return false;
                }

                // 調光設定0～255
                var valueInt = Convert.ToInt32(System.Math.Clamp(value, 0, 255));

                Task.Run(async () =>
                {
                    var command = $"F";

                    var message = string.Empty;

                    var valueText = $"{valueInt:D3}";
                    if (0 > channel)
                    {
                        var values = Enumerable.Repeat(valueText, this.NumberOfChannels);
                        message = this.BuildMessage($"{command}{string.Join("/", values)}", channel);
                    }
                    else
                    {
                        message = this.BuildMessage($"{command}{valueText}", channel);
                    }

                    var result = await this.Handshake(message);

                    if (false == this.VerifyMessage(command, result, out _))
                    {
                        Serilog.Log.Error($"{this}, {message} -> {result}");
                    }
                });

                isSuccess = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 調光値の取得
        /// </summary>
        /// <remarks>Channelプロパティに従う</remarks>
        public override Task<double> ModulationAsync
        {
            get
            {
                if (false == this.Enabled)
                {
                    return Task.FromResult(0d);
                }

                var result = Task<double>.Run(async () =>
                {
                    var value = 0d;

                    try
                    {
                        // 調光データ読み出し
                        var command = $"M";

                        var message = this.BuildMessage($"{command}", this.Channel);

                        var text = await this.Handshake(message);

                        if (true == this.VerifyMessage(command, text, out List<string[]> valueText))
                        {
                            if (1 < valueText.Count)
                            {
                                var indexCh = 0;
                                if (0 < this.Channel)
                                {
                                    indexCh = this.Channel;
                                }

                                if (valueText.Count > indexCh)
                                {
                                    var stateCh = valueText[indexCh];

                                    value = Convert.ToInt32(stateCh[0]);
                                }
                                else
                                {
                                    throw new IndexOutOfRangeException();
                                }
                            }
                            else
                            {
                                value = Convert.ToInt32(valueText[0][0]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }

                    return value;
                });

                return result;
            }
        }

        /// <summary>
        /// 外部トリガー点灯時間変更
        /// </summary>
        public override bool ChangeExternalTriggerStrobeTimeUs(int strobeTimeUs = 0) => false;
        public override bool ChangeExternalTriggerStrobeTimeUs(int channel, int strobeTimeUs = 0) => false;

        /// <summary>
        /// 外部トリガー点灯時間
        /// </summary>
        public override Task<int> ExternalTriggerStrobeTimeUsAsync => Task.FromResult(0);

        #endregion

        #region IController

        /// <summary>
        /// パラメーター設定
        /// </summary>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is ITcpCommunicationParameter gp)
                {
                    this.CommunicationParameter = gp;

                    if (true == this.Enabled)
                    {
                        this.Modulate(this.CommunicationParameter.Modulation);
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
                this.IsReconnectable = false;
                this.DisposeSafely(this.CommunicationClient);
                this.DisposeSafely(this.CommunicationHandshake);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region ITcpLightController

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
        public virtual bool IsReconnectable
        {
            get => this.CommunicationParameter?.IsReconnectable ?? false;

            set
            {
                if (null != this.CommunicationParameter)
                {
                    this.CommunicationParameter.IsReconnectable = value;
                }
            }
        }

        /// <summary>
        /// 再接続試行間隔
        /// </summary>
        public virtual int ReconnectionAttemptsIntervalSec
        {
            get => this.CommunicationParameter?.ReconnectionAttemptsIntervalSec ?? -1;

            set
            {
                if (null != this.CommunicationParameter)
                {
                    this.CommunicationParameter.ReconnectionAttemptsIntervalSec = value;
                }
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// 通信パラメータ
        /// </summary>
        protected ITcpCommunicationParameter? CommunicationParameter;

        /// <summary>
        /// 通信クライアント
        /// </summary>
        protected TcpAsciiTextCommunicationClient CommunicationClient;

        /// <summary>
        /// ハンドシェイク情報
        /// </summary>
        protected class HandshakeControll : SafelyDisposable
        {
            protected AutoResetEvent? EventBody;

            public Semaphore Semaphore { get; init; } = new(1, 1);

            public string ReceivedText { get; protected set; } = string.Empty;

            public void ReadyWrite()
            {
                this.EventBody?.Close();
                this.ReceivedText = string.Empty;

                this.EventBody = new AutoResetEvent(false);
            }

            public void NotifyReceivedEvent(string data)
            {
                this.ReceivedText += data;

                var currentEvent = this.EventBody;
                currentEvent?.Set();
            }

            public bool WaitReceive(int timeoutMs)
            {
                return this.EventBody?.WaitOne(timeoutMs) ?? false;
            }

            #region SafelyDisposable

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.DisposeSafely(this.EventBody);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion
        }

        protected HandshakeControll CommunicationHandshake = new();

        /// <summary>
        /// コマンド送信間隔
        /// </summary>
        /// <remarks>仕様書に間隔制限の記載はないが調整できるように変数としておく</remarks>
        protected readonly TimeSpan MinTransmissionInterval = TimeSpan.FromMilliseconds(0);

        /// <summary>
        /// 最後の受信からの経過時間
        /// </summary>
        protected readonly Stopwatch TimeSinceLastReception = Stopwatch.StartNew();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpUTechUpd2450_LightController() : this(typeof(TcpUTechUpd2450_LightController).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpUTechUpd2450_LightController(Common.Drawing.Point location) : this(typeof(TcpUTechUpd2450_LightController).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpUTechUpd2450_LightController(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.CommunicationClient = new();
            this.CommunicationClient.Connected += this.CommunicationClient_Connected;
            this.CommunicationClient.Disconnected += this.CommunicationClient_Disconnected;
            this.CommunicationClient.TransferDataRead += this.CommunicationClient_TransferDataRead;
        }

        #endregion

        #region 通信クライアントイベント

        /// <summary>
        /// 通信クライアントイベント:受信
        /// </summary>
        protected virtual void CommunicationClient_TransferDataRead(object sender, string data)
        {
            try
            {
                this.CommunicationHandshake.NotifyReceivedEvent(data);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 通信クライアントイベント:切断
        /// </summary>
        protected virtual void CommunicationClient_Disconnected(object sender)
        {
            try
            {
                this.OnDisabled();

                this.Disconnected?.Invoke(this);

                if (null != this.CommunicationParameter && this.CommunicationParameter.IsReconnectable)
                {
                    if (0 < this.CommunicationParameter.ReconnectionAttemptsIntervalSec)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await Task.Delay(this.CommunicationParameter.ReconnectionAttemptsIntervalSec * 1000);

                                if (this.CommunicationParameter.IsReconnectable)
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
        /// 通信クライアントイベント:接続
        /// </summary>
        protected virtual void CommunicationClient_Connected(object sender)
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

        #endregion

        #region protectedメソッド

        /// <summary>
        /// 消灯
        /// </summary>
        protected bool TurnOnOff(bool isOn, int channel)
        {
            var isSuccess = false;

            try
            {
                if (true == this.Enabled)
                {
                    var result = string.Empty;

                    Task.Run(async () =>
                    {
                        var command = $"L";
                        var state = isOn ? "1" : "0";

                        var message = string.Empty;

                        if (0 > channel)
                        {
                            var states = Enumerable.Repeat(state, this.NumberOfChannels);
                            message = this.BuildMessage($"{command}{string.Join("/", states)}", channel);
                        }
                        else
                        {
                            message = this.BuildMessage($"{command}{state}", channel);
                        }

                        result = await this.Handshake(message);

                        if (false == this.VerifyMessage(command, result, out _))
                        {
                            Serilog.Log.Error($"{this}, {message} -> {result}");
                        }
                    });

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 送受信
        /// </summary>
        protected async virtual Task<string> Handshake(string command, int timeoutMs = 1000)
        {
            var receivedText = string.Empty;

            try
            {
                if (true == this.CommunicationHandshake.Semaphore.WaitOne())
                {
                    try
                    {
                        var elapsed = this.TimeSinceLastReception.Elapsed;
                        if (this.MinTransmissionInterval > elapsed)
                        {
                            await Task.Delay(this.MinTransmissionInterval - elapsed);
                        }

                        this.CommunicationHandshake.ReadyWrite();

                        this.CommunicationClient.AysncWrite(command);

                        await Task.Run(() =>
                        {
                            this.CommunicationHandshake.WaitReceive(timeoutMs);
                        });

                        receivedText = this.CommunicationHandshake.ReceivedText;

                        this.TimeSinceLastReception.Restart();
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        this.CommunicationHandshake.Semaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return receivedText;
        }

        /// <summary>
        /// コマンド書式化
        /// </summary>
        protected virtual string BuildMessage(string message, int channel)
        {
            var ch = "FF";

            if (0 <= channel)
            {
                ch = $"{channel:D2}";
            }

            var body = $"@{ch}{message}";
            var checksum = body.Sum(c => (int)c) & 0xFF;    // チェックサムは合計値の下位byte

            return $"{body}{checksum:X2}\r\n";
        }

        /// <summary>
        /// コマンドチェック
        /// </summary>
        protected virtual bool VerifyMessage(string command, string receivedText, out List<string[]> value)
        {
            value = new List<string[]>();

            if (false == receivedText.StartsWith("@"))
            {
                return false;
            }

            if (false == receivedText.EndsWith("\r\n"))
            {
                return false;
            }

            if ('O' != receivedText[3])
            {
                return false;
            }

            // 問い合わせコマンドの場合はデータ部を取り出す
            if ("M" == command)
            {
                var startIndex = 4; // ヘッダを除外
                var length = receivedText.Length - startIndex - 4; // チェックサムとデリミタを除外

                if (0 >= length)
                {
                    return false;
                }

                var body = receivedText.Substring(startIndex, length);

                foreach (var unit in body.Split('/'))
                {
                    var state = unit.Split('.');

                    if (3 > state.Length)
                    {
                        return false;
                    }

                    var valueF = state[0].Substring(1); // F
                    var valueS = state[1].Substring(1); // S
                    var valueL = state[2].Substring(1); // L

                    if (false == int.TryParse(valueF, out _) || false == int.TryParse(valueS, out _) || false == int.TryParse(valueL, out _))
                    {
                        return false;
                    }

                    value.Add(new[] { valueF, valueS, valueL });
                }
            }

            return true;
        }

        #endregion
    }
}
