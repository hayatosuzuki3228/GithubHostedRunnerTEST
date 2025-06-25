using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Net.Sockets;
using System.Diagnostics;

namespace Hutzper.Library.LightController.Device.Net.Sockets.Ccs
{
    /// <summary>
    /// CCS TCP通信照明制御（PSB4）
    /// </summary>
    public class TcpCcsPSB4LightController : LightControllerBase, ITcpLightController
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
        /// <returns></returns>
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
                                if (this.CommunicationParameter is ILightControllerParameter lp)
                                {

                                    #region "調光設定"
                                    // 調光設定反映（レシピの調光設定とは無関係）
                                    this.Modulate(this.CommunicationParameter.Modulation);
                                    #endregion

                                    #region IPアドレス変更
                                    {
                                        //Task.Run(async () =>
                                        //{
                                        //    var command = this.FormatCommand($"FFE01{"192.168.127.100"}");

                                        //    var result = await this.Handshake(command);

                                        //    this.VerifyCommand(result, out string value);

                                        //    command = this.FormatCommand($"FFE05{"192.168.127.101"}");

                                        //    result = await this.Handshake(command);

                                        //    this.VerifyCommand(result, out value);

                                        //}).ConfigureAwait(false);
                                    }
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
        /// <returns></returns>
        public override bool Close()
        {
            try
            {
                if (Monitor.TryEnter(this.CommunicationClient))
                {
                    try
                    {
                        this.Modulate(0); // 強制パワーOFF（PSB4のみ）
                        Thread.Sleep(100); // コマンド送信完了待機
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
        /// <returns></returns>
        public override bool TurnOn()
        {
            return this.TurnOn(this.Channel);
        }
        public override bool TurnOn(int channel)
        {
            var isSuccess = false;

            try
            {
                if (true == this.Enabled)
                {
                    var result = string.Empty;

                    Task.Run(async () =>
                    {
                        var command = this.FormatCommand($"{this.GetChannelText(channel)}L1");

                        result = await this.Handshake(command);

                        isSuccess = this.VerifyCommand(result, out string value);

                    }).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }
        /// <summary>
        /// 消灯
        /// </summary>
        /// <returns></returns>
        public override bool TurnOff()
        {
            return this.TurnOff(this.Channel);
        }
        public override bool TurnOff(int channel)
        {
            var isSuccess = false;

            try
            {
                if (true == this.Enabled)
                {
                    var result = string.Empty;

                    Task.Run(async () =>
                    {
                        var command = this.FormatCommand($"{this.GetChannelText(channel)}L0");

                        result = await this.Handshake(command);

                        isSuccess = this.VerifyCommand(result, out string value);

                    }).ConfigureAwait(false).GetAwaiter().GetResult();
                }
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
        public override bool Modulate(double value)
        {
            return this.Modulate(this.Channel, value);
        }
        public override bool Modulate(int channel, double value)
        {
            var isSuccess = false;

            try
            {
                var valueInt = Convert.ToInt32(System.Math.Clamp(value, 0, 1023));

                Task.Run(async () =>
                {
                    var command = this.FormatCommand($"{this.GetChannelText(channel)}F{valueInt:D4}");

                    var result = await this.Handshake(command);

                    isSuccess = this.VerifyCommand(result, out string value);
                }).ConfigureAwait(false).GetAwaiter().GetResult();
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
                var result = Task<double>.Run(async () =>
                {
                    var value = 0d;

                    try
                    {
                        var command = this.FormatCommand($"{this.GetChannelText(this.Channel)}M");

                        var text = await this.Handshake(command);

                        if (true == this.VerifyCommand(text, out string valueText))
                        {
                            if (valueText[0] == 'M')
                            {
                                value = double.Parse(valueText.Substring(5, 4));
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
        public override bool ChangeExternalTriggerStrobeTimeUs(int strobeTimeUs = 0)
        {
            return this.ChangeExternalTriggerStrobeTimeUs(this.Channel, strobeTimeUs);
        }

        /// <summary>
        /// 外部トリガー点灯時間変更(チャンネル指定)
        /// </summary>
        public override bool ChangeExternalTriggerStrobeTimeUs(int channel, int strobeTimeUs = 0) => false;

        /// <summary>
        /// 外部トリガー点灯時間
        /// </summary>
        public override Task<int> ExternalTriggerStrobeTimeUsAsync
        {
            get
            {
                var result = Task<int>.Run(async () =>
                {
                    var value = 0;

                    try
                    {
                        await Task.Delay(1);
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

        #endregion

        #region IController

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
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
                        this.ChangeExternalTriggerStrobeTimeUs(this.CommunicationParameter.ExternalTriggerStrobeTimeUs);
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

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpCcsPSB4LightController() : this(typeof(TcpCcsPSB4LightController).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpCcsPSB4LightController(Common.Drawing.Point location) : this(typeof(TcpCcsPSB4LightController).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public TcpCcsPSB4LightController(string nickname, Common.Drawing.Point location) : base(nickname, location)
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
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <exception cref="NotImplementedException"></exception>
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
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
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
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
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

        #region メソッド

        /// <summary>
        /// 送受信
        /// </summary>
        /// <param name="command"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        protected async virtual Task<string> Handshake(string command, int timeoutMs = 500)
        {
            var receivedText = string.Empty;

            try
            {
                if (true == this.CommunicationHandshake.Semaphore.WaitOne())
                {
                    try
                    {
                        this.CommunicationHandshake.ReadyWrite();

                        this.CommunicationClient.AysncWrite(command);

                        await Task.Run(() =>
                        {
                            this.CommunicationHandshake.WaitReceive(timeoutMs);
                        });

                        receivedText = this.CommunicationHandshake.ReceivedText;
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
        /// 指定チャンネルに対応する文字列を得る
        /// </summary>
        /// <param name="selectedChannel"></param>
        /// <returns></returns>
        protected virtual string GetChannelText(int selectedChannel)
        {
            if (0 > selectedChannel)
            {
                return "FF";
            }
            else
            {
                return $"{selectedChannel:D2}";
            }
        }

        /// <summary>
        /// コマンド書式化
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual string FormatCommand(string command) => $"@{command}\r\n";

        /// <summary>
        /// コマンドチェック
        /// </summary>
        /// <param name="command"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool VerifyCommand(string command, out string value)
        {
            var result = false;

            value = string.Empty;

            if (false == string.IsNullOrEmpty(command) && command.Length >= 6)
            {
                var body = command[1..^2];

                result = body.Substring(2, 1) == "O";
                value = body[3..];
            }

            return result;
        }

        #endregion
    }
}