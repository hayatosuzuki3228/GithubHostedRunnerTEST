using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Net.Sockets;
using System.Diagnostics;
using System.Text;

namespace Hutzper.Library.LightController.Device.Net.Sockets.Revox
{
    /// <summary>
    /// Revox 照明電源CB-TB
    /// </summary>
    public class TcpRevoxCBTB_LightController : LightControllerBase, ITcpLightController
    {
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

                                    if (this.CommunicationParameter is TcpRevoxCBTB_Parameter rp)
                                    {
                                        Task.Run(() => this.SetPwmMode(rp.PwmMode));
                                    }

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
                else
                {
                    return false;
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
        public override bool TurnOn() => this.TurnOnOff(true);

        /// <summary>
        /// 点灯
        /// </summary>
        /// <param name="channel">チャンネル指定</param>
        /// <remarks>互換性のために実装してありますが、チャンネルは指定できません</remarks>
        public override bool TurnOn(int channel)
        {
            if (0 != channel)
            {
                Serilog.Log.Warning($"{this}, Channel {channel} is not supported. Replacing with channel 0.");
            }

            return this.TurnOnOff(true);
        }

        /// <summary>
        /// 消灯
        /// </summary>
        public override bool TurnOff() => this.TurnOnOff(false);

        /// <summary>
        /// 消灯
        /// </summary>
        /// <param name="channel">チャンネル指定</param>
        /// <remarks>互換性のために実装してありますが、チャンネルは指定できません</remarks>
        public override bool TurnOff(int channel)
        {
            if (0 != channel)
            {
                Serilog.Log.Warning($"{this}, Channel {channel} is not supported. Replacing with channel 0.");
            }

            return this.TurnOnOff(false);
        }

        /// <summary>
        /// 調光
        /// </summary>
        /// <param name="channel">チャンネル指定</param>
        /// <param name="value">調光値0～1023</param>
        /// <remarks>互換性のために実装してありますが、チャンネルは指定できません</remarks>
        public override bool Modulate(int channel, double value)
        {
            if (0 != channel)
            {
                Serilog.Log.Warning($"{this}, Channel {channel} is not supported. Replacing with channel 0.");
            }

            return this.Modulate(value);
        }

        /// <summary>
        /// 調光
        /// </summary>
        /// <param name="value">調光値0～1023</param>
        public override bool Modulate(double value)
        {
            var isSuccess = false;

            try
            {
                if (false == this.Enabled)
                {
                    return false;
                }

                // 調光設定10bit
                var valueInt = Convert.ToInt32(System.Math.Clamp(value, 0, 1023));

                Task.Run(async () =>
                {
                    var command = $"WDB";

                    var message = this.BuildMessage($"{command}{valueInt:X3}");

                    var result = await this.Handshake(message);

                    this.VerifyMessage(command, result, out string value);
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
        /// 調光
        /// </summary>
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
                        // 調光データ読み出し10bit
                        var command = $"RSB";

                        var message = this.BuildMessage($"{command}");

                        var text = await this.Handshake(message);

                        if (true == this.VerifyMessage(command, text, out string valueText))
                        {
                            value = Convert.ToInt32(valueText, 16);
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

                        if (this.CommunicationParameter is TcpRevoxCBTB_Parameter rp)
                        {
                            Task.Run(() => this.SetPwmMode(rp.PwmMode));
                        }
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
                var currentEvent = this.EventBody;

                this.ReceivedText = data;

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
        /// コマンド送信間隔として、応答コマンドの受信から次のコマンドの送信まで40ms 以上確保
        /// </summary>
        protected readonly TimeSpan MinTransmissionInterval = TimeSpan.FromMilliseconds(40);

        /// <summary>
        /// 最後の受信からの経過時間
        /// </summary>
        protected readonly Stopwatch TimeSinceLastReception = Stopwatch.StartNew();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpRevoxCBTB_LightController() : this(typeof(TcpRevoxCBTB_LightController).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpRevoxCBTB_LightController(Common.Drawing.Point location) : this(typeof(TcpRevoxCBTB_LightController).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public TcpRevoxCBTB_LightController(string nickname, Common.Drawing.Point location) : base(nickname, location)
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

        #region TcpRevoxCBTB_LightController


        /// <summary>
        /// PWMモードを設定します
        /// </summary>
        /// <param name="mode">0:無効 1:外部パルス 2:高速調光</param>
        /// <remarks>SPX-TB-80 のみで有効</remarks>
        public async Task<bool> SetPwmMode(int mode)
        {
            var isSuccess = false;

            try
            {
                if (false == this.Enabled)
                {
                    return await Task.FromResult(false);
                }

                if (0 > mode || 2 < mode)
                {
                    throw new ArgumentOutOfRangeException(nameof(mode), "mode must be between 0 and 2.");
                }

                // PWM モード設定
                var command = $"WSP";

                var message = this.BuildMessage($"{command}{mode}");

                var result = await this.Handshake(message);

                isSuccess = this.VerifyMessage(command, result, out string value);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// PWM モード読み出し
        /// </summary>
        /// <remarks>SPX-TB-80 のみで有効</remarks>
        /// <returns>-1:失敗 0:無効 1:外部パルス 2:高速調光</returns>
        public async Task<int> GetPwmMode()
        {
            var mode = -1;

            try
            {
                if (false == this.Enabled)
                {
                    return await Task.FromResult(-1);
                }

                // PWM モード設定
                var command = $"RSP";

                var message = this.BuildMessage($"{command}");

                var result = await this.Handshake(message);

                if (true == this.VerifyMessage(command, result, out string value))
                {
                    mode = Convert.ToInt32(value);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return mode;
        }

        /// <summary>
        /// 高速調光値設定
        /// </summary>
        /// <param name="values">最大12 点同時に設定します</param>
        /// <remarks>SPX-TB-80 のみで有効</remarks>
        public async Task<bool> SetHighSpeedModulationValues(int[] values)
        {
            var isSuccess = false;

            try
            {
                if (false == this.Enabled)
                {
                    return await Task.FromResult(false);
                }

                if (0 >= values.Length || 12 < values.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(values), "modulation value must be between 1 and 12 (inclusive).");
                }

                // 高速調光値設定
                var command = $"WHD";

                var elements = values.Select(v => v.ToString("X4")).ToList();
                elements.Insert(0, $"{values.Length:X2}");

                var message = this.BuildMessage($"{command}{string.Join(",", elements)}");

                var result = await this.Handshake(message);

                isSuccess = this.VerifyMessage(command, result, out string value);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 高速調光値読出し
        /// </summary>
        /// <remarks>SPX-TB-80 のみで有効</remarks>
        public Task<int[]> GetHighSpeedModulationValuesAsync()
        {
            if (false == this.Enabled)
            {
                return Task.FromResult(Array.Empty<int>());
            }

            var result = Task<int[]>.Run(async () =>
            {
                var values = Array.Empty<int>();

                try
                {
                    // 高速調光値読出し
                    var command = $"RHD";

                    var message = this.BuildMessage($"{command}");

                    var text = await this.Handshake(message);

                    if (true == this.VerifyMessage(command, text, out string valueText))
                    {
                        var elements = valueText.Split(',').ToList();

                        var number = Convert.ToInt32(elements.FirstOrDefault(), 16);
                        elements.RemoveAt(0);

                        if (number == elements.Count)
                        {
                            values = elements.Select(e => Convert.ToInt32(e, 16)).ToArray();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return values;
            });

            return result;
        }

        #endregion

        #region protectedメソッド

        /// <summary>
        /// 消灯
        /// </summary>
        protected bool TurnOnOff(bool isOn)
        {
            var isSuccess = false;

            try
            {
                if (true == this.Enabled)
                {
                    var result = string.Empty;

                    Task.Run(async () =>
                    {
                        var command = $"WDD";
                        var state = isOn ? 1 : 0;

                        var message = this.BuildMessage($"{command}{state}");

                        result = await this.Handshake(message);

                        this.VerifyMessage(command, result, out string value);

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
        protected virtual string BuildMessage(string message) => $"\u0002{message}\u0003";

        /// <summary>
        /// コマンドチェック
        /// </summary>
        protected virtual bool VerifyMessage(string command, string receivedText, out string value)
        {
            var ascii = Encoding.ASCII.GetBytes(receivedText);
            var result = (2 < ascii.Length) && ascii[0] == 0x06 && ascii[^1] == 0x03;

            value = string.Empty;

            if (false == result)
            {
                return result;
            }

            // コマンド:3 + STX:1 + ETX:1 (通常コマンドは3文字、バージョン読み出しのみ「V」1文字。Vの使用を想定しない)
            if (true == result && receivedText.Length >= 5)
            {
                var stx_cmd_body = receivedText[..^1];   // ETX除去

                // 問い合わせ
                if (true == command.StartsWith("R"))
                {
                    var cmd_body = stx_cmd_body[1..];   // STX除去

                    value = cmd_body.Substring(command.Length);  // コマンド除去

                    result &= command.StartsWith(command);
                }
            }

            return result;
        }

        #endregion
    }
}