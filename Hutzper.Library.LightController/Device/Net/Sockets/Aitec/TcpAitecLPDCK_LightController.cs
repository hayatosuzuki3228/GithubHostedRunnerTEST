using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Net.Sockets;
using System.Diagnostics;

namespace Hutzper.Library.LightController.Device.Net.Sockets.Aitec
{
    /// <summary>
    /// Aitec LPDCKシリーズ用TCP通信
    /// </summary>
    public class TcpAitecLPDCK_LightController : LightControllerBase, ITcpLightController
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
        public override bool TurnOn()
        {
            var isSuccess = false;

            try
            {
                if (true == this.Enabled)
                {
                    var result = string.Empty;

                    Task.Run(async () =>
                    {
                        var header = $"LCON";

                        var command = this.FormatCommand($"{this.GetChannelText(this.Channel)}{header}");

                        result = await this.Handshake(command);

                        this.VerifyCommand(header, result, out string value);

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
        /// 消灯
        /// </summary>
        public override bool TurnOff()
        {
            var isSuccess = false;

            try
            {
                if (true == this.Enabled)
                {
                    var result = string.Empty;

                    Task.Run(async () =>
                    {
                        var header = $"LCOF";

                        var command = this.FormatCommand($"{this.GetChannelText(this.Channel)}{header}");

                        result = await this.Handshake(command);

                        this.VerifyCommand(header, result, out string value);

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
        /// 調光
        /// </summary>
        public override bool Modulate(double value)
        {
            var isSuccess = false;

            try
            {
                var valueInt = Convert.ToInt32(System.Math.Clamp(value, 0, 999));

                Task.Run(async () =>
                {
                    var header = $"LC";

                    var command = this.FormatCommand($"{this.GetChannelText(this.Channel)}{header}{valueInt}");

                    var result = await this.Handshake(command);

                    this.VerifyCommand(header, result, out string value);
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
                var result = Task<double>.Run(async () =>
                {
                    var value = 0d;

                    try
                    {
                        var header = $"LC?";

                        var command = this.FormatCommand($"{this.GetChannelText(this.Channel)}{header}");

                        var text = await this.Handshake(command);

                        if (true == this.VerifyCommand(header, text, out string valueText))
                        {
                            value = double.Parse(valueText);
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
        public TcpAitecLPDCK_LightController() : this(typeof(TcpAitecLPDCK_LightController).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpAitecLPDCK_LightController(Common.Drawing.Point location) : this(typeof(TcpAitecLPDCK_LightController).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public TcpAitecLPDCK_LightController(string nickname, Common.Drawing.Point location) : base(nickname, location)
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

        #region メソッド

        /// <summary>
        /// 送受信
        /// </summary>
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
        /// <param name="selectedChannel">0から始まるインデックス(全ch指定時は-1:0未満を指定)</param>
        protected virtual string GetChannelText(int selectedChannel)
        {
            if (0 > selectedChannel)
            {
                return "A";
            }
            else
            {
                return $"{selectedChannel + 1}";
            }
        }

        /// <summary>
        /// コマンド書式化
        /// </summary>
        protected virtual string FormatCommand(string command) => $"@{command}\r\n";

        /// <summary>
        /// コマンドチェック
        /// </summary>
        protected virtual bool VerifyCommand(string sendHead, string receivedText, out string value)
        {
            var result = false;

            value = string.Empty;

            // ヘッダ有 & 可変長応答 コマンド:min2 + デリミタ:2
            if (false == string.IsNullOrEmpty(receivedText) && receivedText.Length >= 4)
            {
                var body = receivedText[..^2];   // デリミタ除去

                // 問い合わせ
                if (true == sendHead.EndsWith("?"))
                {
                    var desiredHead = sendHead[..^1]; // ?除去

                    if (true == body.StartsWith(desiredHead) && desiredHead.Length < body.Length)
                    {
                        value = body.Substring(desiredHead.Length);
                        result = true;
                    }
                }
                // 設定
                else
                {
                    result = body.StartsWith("ACK");
                }
            }

            return result;
        }

        #endregion
    }
}