using Hutzper.Library.CodeReader.Data;
using Hutzper.Library.Common;
using Hutzper.Library.Common.Net.Sockets;
using System.Diagnostics;

namespace Hutzper.Library.CodeReader.Device.Keyence
{
    /// <summary>
    /// KEYENCE SR-X
    /// </summary>
    [Serializable]
    public class CodeReaderKeyence_SRX : CodeReaderBase, ICodeReaderKeyence, ITcpCodeReader
    {
        #region ITcpCodeReader

        #region イベント

        /// <summary>
        /// 接続された
        /// </summary>
        public event Action<object>? Connected;

        #endregion

        #endregion

        #region ICodeReader

        /// <summary>
        /// 自動調整が利用可能かどうか
        /// </summary>
        public override bool AutotuningAvailable { get; } = true;

        /// <summary>
        /// 読取可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public override bool Enabled => this.TcpClient?.Enabled ?? false;

        /// <summary>
        /// 自動チューニング実行
        /// </summary>
        /// <returns></returns>
        public override async Task<ICodeReaderTuningResult> RunAutotuningAsync()
        {
            var result = new CodeReaderTuningResult();

            try
            {
                if (this.TcpClient is not null && this.Parameter is CodeReaderParameterKeyence_SRX cp && true == this.SessionSemaphore.WaitOne(0))
                {
                    result.IsSuccess = true;

                    try
                    {
                        #region フォーカス調整
                        {
                            var command = "FTUNE";

                            var receivedText = await this.Handshake(command, cp.CommandResponseTimeoutMs, cp.FocusTuningTimeoutMs);

                            if (2 <= receivedText.Count)
                            {
                                var tempResult = this.CheckResponse(command, receivedText.Last(), out string responseBody);

                                result.ReadDateTime = DateTime.Now;

                                if (true == tempResult)
                                {
                                    result.TuningInfomation.Add("focus", "succeeded");
                                }
                                else
                                {
                                    result.TuningInfomation.Add("focus", "failed");
                                }

                                result.IsSuccess &= tempResult;
                            }
                            else
                            {
                                result.TuningInfomation.Add("focus", "timeout");
                                result.IsSuccess = false;
                            }
                        }
                        #endregion

                        #region オートチューニング
                        {
                            var command = "TUNE01";

                            var receivedText = await this.Handshake(command, cp.CommandResponseTimeoutMs, cp.AutoTuningTimeoutMs);

                            if (2 <= receivedText.Count)
                            {
                                var tempResult = this.CheckResponse(command, receivedText.Last(), out string responseBody);
                                result.ReadDateTime = DateTime.Now;

                                result.TuningInfomation.Add("tuning advice", responseBody[^3..^2]);
                                if (false == tempResult)
                                {
                                    result.TuningInfomation.Add("tuning failure factor", responseBody[^1..]);
                                }

                                result.IsSuccess &= tempResult;
                            }
                            else
                            {
                                this.SendSettingCommand("TQUIT", true);
                                result.DataStrings.Add("timeout");
                                result.IsSuccess = false;
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        this.SessionSemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 読取
        /// </summary>
        public async override Task<ICodeReaderResult> Read(int timeoutMs = -1)
        {
            var result = new CodeReaderResult();

            try
            {
                if (this.TcpClient is not null && this.Parameter is CodeReaderParameterKeyence_SRX cp && true == this.SessionSemaphore.WaitOne(0))
                {
                    try
                    {
                        var waitTimeMs = System.Math.Max(0, timeoutMs);
                        if (0 >= waitTimeMs)
                        {
                            waitTimeMs = cp.ReadTimeoutMs;
                        }

                        // 連続読み取りモードOFF
                        await this.Handshake("WP,200,0", cp.CommandResponseTimeoutMs);

                        // 2度読み防止OFF
                        await this.Handshake("WP,239,2", cp.CommandResponseTimeoutMs);

                        // 読み取り開始
                        var command = "LON";
                        var receivedText = await this.Handshake(command, waitTimeMs);

                        // 結果受信あり
                        if (0 < receivedText.Count)
                        {
                            result.IsSuccess = this.CheckResponse(command, receivedText.Last(), out string responseBody);
                            result.ReadDateTime = DateTime.Now;
                            if (false == string.IsNullOrEmpty(responseBody))
                            {
                                result.DataStrings.Add(responseBody);
                            }
                        }
                        else
                        {
                            await this.Handshake("LOFF", cp.CommandResponseTimeoutMs);
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        this.SessionSemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 連続読取開始
        /// </summary>
        /// <returns></returns>
        public override bool ReadContinuously(int number = -1)
        {
            var isSuccess = false;

            try
            {
                if (this.TcpClient is not null && this.Parameter is CodeReaderParameterKeyence_SRX cp)
                {
                    if (true == this.SessionSemaphore.WaitOne(0))
                    {
                        try
                        {
                            var commandResult = true;

                            // 2度読み防止時間
                            commandResult &= this.SendSettingCommand($"WP,202,{System.Math.Clamp(cp.ContinuousReadingIntervalMs / 100, 0, 255)}", true);

                            // 2度読み防止ON
                            commandResult &= this.SendSettingCommand("WP,239,0", true);

                            // 連続読み取りモード
                            commandResult &= this.SendSettingCommand("WP,200,1", true);

                            // 設定に成功した場合
                            if (true == commandResult)
                            {

                                // 連続読み取り用受信イベント
                                var dataRead = ((object sender, string data) =>
                                {
                                    try
                                    {
                                        // 連続撮影中の場合
                                        if (true == this.IsReading)
                                        {
                                            // 読取結果データを作成する
                                            var result = new CodeReaderResult
                                            {
                                                IsSuccess = this.CheckResponse(string.Empty, data, out string responseBody),
                                                ReadDateTime = DateTime.Now
                                            };

                                            // 読み取り文字がある場合
                                            if (false == string.IsNullOrEmpty(responseBody))
                                            {
                                                result.DataStrings.Add(responseBody);
                                            }

                                            // イベント通知
                                            this.OnDataRead(result);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Serilog.Log.Warning(ex, ex.Message);
                                    }
                                });

                                // 受信イベント切替
                                this.TcpClient.TransferDataRead -= this.TcpClient_TransferDataRead;
                                this.TcpClient.TransferDataRead += dataRead;

                                // 終了スレッド処理
                                var terminationThread = new Thread(() =>
                                {
                                    try
                                    {
                                        // 終了指示待機
                                        this.ReadingTerminatedEvent.WaitOne();

                                        this.TcpClient.TransferDataRead -= dataRead;

                                        // 読み取り停止
                                        this.SendSettingCommand("LOFF", false);

                                        // 連続読み取りモードOFF
                                        this.SendSettingCommand("WP,200,0", false);
                                    }
                                    catch (Exception ex)
                                    {
                                        Serilog.Log.Warning(ex, ex.Message);
                                    }
                                    finally
                                    {
                                        this.TcpClient.TransferDataRead += this.TcpClient_TransferDataRead;

                                        // 権利解放
                                        this.SessionSemaphore.Release();
                                    }
                                })
                                { IsBackground = true };

                                // 終了スレッド起動
                                this.ReadingTerminatedEvent.Reset();
                                terminationThread.Start();

                                // 読み取り開始
                                isSuccess = this.SendSettingCommand("LON", false);

                                this.IsReading = true;
                            }
                            // 設定に失敗した場合
                            else
                            {
                                // 権利解放
                                this.SessionSemaphore.Release();
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        protected ManualResetEvent ReadingTerminatedEvent = new(true);

        /// <summary>
        /// 連続読取停止
        /// </summary>
        /// <returns></returns>
        public override void StopReading()
        {
            try
            {
                this.IsReading = false;
                this.ReadingTerminatedEvent.Set();

                //if (this.TcpClient is not null && this.Parameter is CodeReaderParameterKeyence_SRX cp)
                //{
                //    if (false == this.SessionSemaphore.WaitOne(0))
                //    {
                //        if(true == this.IsReading)
                //        { 
                //            try
                //            {
                //                // 読み取り停止
                //                this.SendSettingCommand("LOFF", false);

                //                this.TcpClient.TransferDataRead += this.TcpClient_TransferDataRead;

                //                // 連続読み取りモードOFF
                //                this.SendSettingCommand("WP,200,0", true);
                //            }
                //            catch (Exception ex)
                //            {
                //                Serilog.Log.Warning(ex, ex.Message);
                //            }
                //            finally
                //            {
                //                this.IsReading = false;
                //                this.SessionSemaphore.Release();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        this.SessionSemaphore.Release();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
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

                if (this.TcpClient is not null)
                {
                    this.TcpClient.Connected -= this.TcpClient_Connected;
                    this.TcpClient.Disconnected -= this.TcpClient_Disconnected;
                    this.TcpClient.TransferDataRead -= this.TcpClient_TransferDataRead;
                }
                this.Close();
                this.DisposeSafely(this.TcpClient);

                this.TcpClient = new TcpAsciiTextCommunicationClient();
                this.TcpClient.Connected += this.TcpClient_Connected;
                this.TcpClient.Disconnected += this.TcpClient_Disconnected;
                this.TcpClient.TransferDataRead += this.TcpClient_TransferDataRead;
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
            var isSuccess = false;

            try
            {
                if (this.Parameter is ITcpCodeReaderParameter tp)
                {
                    if (this.TcpClient is not null)
                    {
                        isSuccess = this.TcpClient.Connect(tp.IpAddress, tp.PortNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        public override bool Close()
        {
            var isSuccess = true;

            try
            {
                this.StopReading();

                // 切断
                this.TcpClient?.Disconnect();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
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
                if (this.TcpClient is not null)
                {
                    this.TcpClient.Connected -= this.TcpClient_Connected;
                    this.TcpClient.Disconnected -= this.TcpClient_Disconnected;
                    this.TcpClient.TransferDataRead -= this.TcpClient_TransferDataRead;
                }

                this.Close();
                this.DisposeSafely(this.TcpClient);
                this.DisposeSafely(this.CommunicationHandshake);
                this.ReadingTerminatedEvent?.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.TcpClient = null;
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// TCP通信用クライアント
        /// </summary>
        protected TcpAsciiTextCommunicationClient? TcpClient;

        /// <summary>
        /// 制御用セマフォ
        /// </summary>
        protected Semaphore SessionSemaphore = new(1, 1);

        protected bool IsReading;

        /// <summary>
        /// ハンドシェイク情報
        /// </summary>
        protected class HandshakeControll : SafelyDisposable
        {
            protected List<EventWaitHandle> ReceivedEvents = new();

            protected int ReceivedCounter;

            public Semaphore Semaphore { get; init; } = new(1, 1);

            public List<string> ReceivedText { get; protected set; } = new();


            public void ReadyWrite(int number = 1)
            {
                this.ReceivedEvents.ForEach(e => e?.Close());
                this.ReceivedText.Clear();

                this.ReceivedEvents.Clear();
                this.ReceivedEvents = new(new EventWaitHandle[number]);
                foreach (var i in Enumerable.Range(0, number))
                {
                    this.ReceivedEvents[i] = new AutoResetEvent(false);
                }
                this.ReceivedCounter = 0;
            }

            public void NotifyReceivedEvent(string data)
            {
                if (this.ReceivedCounter < this.ReceivedEvents.Count)
                {
                    var currentEvent = this.ReceivedEvents[this.ReceivedCounter++];

                    this.ReceivedText.Add(data);

                    currentEvent?.Set();
                }
            }

            public bool WaitReceive(int timeoutMs)
            {
                if (0 < this.ReceivedEvents.Count)
                {
                    return WaitHandle.WaitAll(ReceivedEvents.ToArray(), timeoutMs);
                }
                else
                {
                    return true;
                }
            }

            #region SafelyDisposable

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.ReceivedEvents.ForEach(e => e?.Close());
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
        public CodeReaderKeyence_SRX() : this(typeof(CodeReaderKeyence_SRX).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CodeReaderKeyence_SRX(Common.Drawing.Point location) : this(typeof(CodeReaderKeyence_SRX).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public CodeReaderKeyence_SRX(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
        }

        #endregion

        /// <summary>
        /// 送受信
        /// </summary>
        /// <param name="command"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        protected async virtual Task<List<string>> Handshake(string command, params int[] timeoutMs)
        {
            var receivedText = new List<string>();

            try
            {
                if (this.TcpClient is not null)
                {
                    if (true == this.CommunicationHandshake.Semaphore.WaitOne())
                    {
                        try
                        {
                            this.CommunicationHandshake.ReadyWrite(timeoutMs.Length);

                            this.TcpClient.AysncWrite("\u001b" + command + "\r");

                            await Task.Run(() =>
                            {
                                this.CommunicationHandshake.WaitReceive(timeoutMs.Sum());
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
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return receivedText;
        }

        /// <summary>
        /// 応答電文文字列をチェックする
        /// </summary>
        /// <param name="sendText"></param>
        /// <param name="telegramText"></param>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        protected virtual bool CheckResponse(string sendText, string telegramText, out string responseBody)
        {
            var isSuccess = false;

            responseBody = string.Empty;

            try
            {
                if (false == string.IsNullOrEmpty(telegramText) && 2 < telegramText.Length)
                {
                    var terminator = telegramText[^1..];
                    var message = telegramText[..^1];

                    if (terminator == "\r" && false == message.Contains("ER,") && false == message.Contains("FAILED"))
                    {
                        isSuccess = true;
                    }

                    responseBody = message;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 設定コマンド送信
        /// </summary>
        /// <param name="command"></param>
        protected bool SendSettingCommand(string command, bool isWaitResponse)
        {
            var isSuccess = false;

            try
            {
                if (this.TcpClient is not null && this.Parameter is CodeReaderParameterKeyence_SRX cp)
                {
                    var commandEvent = new AutoResetEvent(false);

                    Task.Run(async () =>
                    {
                        try
                        {
                            if (true == isWaitResponse)
                            {
                                var receivedText = await this.Handshake(command, cp.CommandResponseTimeoutMs);

                                if (0 < receivedText.Count && this.CheckResponse(command, receivedText.Last(), out string responseBody))
                                {
                                    isSuccess = true;
                                }
                            }
                            else
                            {
                                await this.Handshake(command);
                                isSuccess = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                        finally
                        {
                            commandEvent.Set();
                        }
                    });

                    commandEvent.WaitOne();
                    commandEvent.Close();
                    commandEvent = null;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #region TcpClient

        /// <summary>
        /// 受信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void TcpClient_TransferDataRead(object sender, string data)
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
        /// 接続
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void TcpClient_Connected(object obj)
        {
            try
            {
                // 接続イベント通知
                this.Connected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 切断
        /// </summary>
        /// <param name="obj"></param>
        private void TcpClient_Disconnected(object obj)
        {
            try
            {
                // 無効イベント通知
                this.OnDisabled();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}