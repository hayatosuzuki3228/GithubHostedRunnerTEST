using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Net.Sockets;
using System.Diagnostics;

namespace Hutzper.Library.LightController.Device.Net.Sockets.OptexFA
{
    /// <summary>
    /// OptexFA  TCP通信照明制御（OPPX)
    /// </summary>
    /// <remarks>2chを想定</remarks>
    [Serializable]
    public class TcpOptexFA_OPPX_LightController : LightControllerBase, ITcpLightController
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
                                if (this.CommunicationParameter is ILightControllerParameter)
                                {

                                    #region "調光設定"
                                    // TODO テストできる該当ライトが存在するときにテストを実施して調光設定を導入する
                                    //// 調光設定反映（レシピの調光設定とは無関係）
                                    //this.Modulate(this.CommunicationParameter.Modulation);
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
                        var pid = 110;
                        var command = Array.Empty<byte>();

                        if (0 <= channel)
                        {
                            var ch = System.Math.Clamp(channel, 0, 1);
                            ch *= 1000;

                            command = this.FormatCommandForWrite(0, Tuple.Create(pid + ch, $"{1:X4}"));
                        }
                        else
                        {
                            var ch1 = 0;
                            var ch2 = 1000;

                            command = this.FormatCommandForWrite(0, Tuple.Create(pid + ch1, $"{1:X4}"), Tuple.Create(pid + ch2, $"{1:X4}"));
                        }

                        var receivedBytes = await this.Handshake(command);

                        if (true == this.VerifyCommand(receivedBytes, out byte[] bodyBytes))
                        {

                        }
                        else
                        {
                            Serilog.Log.Error($"{new StackFrame().GetMethod()?.ReflectedType?.Name ?? "unknown"}");
                        }

                    }).ConfigureAwait(false);

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
                        var pid = 110;
                        var command = Array.Empty<byte>();

                        if (0 <= channel)
                        {
                            var ch = System.Math.Clamp(channel, 0, 1);
                            ch *= 1000;

                            command = this.FormatCommandForWrite(0, Tuple.Create(pid + ch, $"{0:X4}"));
                        }
                        else
                        {
                            var ch1 = 0;
                            var ch2 = 1000;

                            command = this.FormatCommandForWrite(0, Tuple.Create(pid + ch1, $"{0:X4}"), Tuple.Create(pid + ch2, $"{0:X4}"));
                        }

                        var receivedBytes = await this.Handshake(command);

                        if (true == this.VerifyCommand(receivedBytes, out byte[] bodyBytes))
                        {

                        }
                        else
                        {
                            Serilog.Log.Error($"{new StackFrame().GetMethod()?.ReflectedType?.Name ?? "unknown"}");
                        }

                    }).ConfigureAwait(false);

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
            return this.Modulate(this.Channel, value);
        }
        public override bool Modulate(int channel, double value)
        {
            var isSuccess = false;

            try
            {
                var valueInt = Convert.ToInt32(System.Math.Clamp(value, 0, 999));

                Task.Run(async () =>
                {
                    var pid = 103;
                    var command = Array.Empty<byte>();

                    if (0 <= channel)
                    {
                        var ch = System.Math.Clamp(channel, 0, 1);
                        ch *= 1000;

                        command = this.FormatCommandForWrite(0, Tuple.Create(pid + ch, $"{valueInt:X4}"));
                    }
                    else
                    {
                        var ch1 = 0;
                        var ch2 = 1000;

                        command = this.FormatCommandForWrite(0, Tuple.Create(pid + ch1, $"{valueInt:X4}"), Tuple.Create(pid + ch2, $"{valueInt:X4}"));
                    }

                    var receivedBytes = await this.Handshake(command);

                    if (true == this.VerifyCommand(receivedBytes, out byte[] bodyBytes))
                    {

                    }
                    else
                    {
                        Serilog.Log.Error($"{new StackFrame().GetMethod()?.ReflectedType?.Name ?? "unknown"}");
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
                        var pid = 103;
                        var command = Array.Empty<byte>();

                        if (0 <= this.Channel)
                        {
                            var ch = System.Math.Clamp(this.Channel, 0, 1);
                            ch *= 1000;

                            command = this.FormatCommandForRead(0, pid + ch);
                        }
                        else
                        {
                            var ch1 = 0;
                            var ch2 = 1000;

                            command = this.FormatCommandForRead(0, pid + ch1, pid + ch2);
                        }

                        var receivedBytes = await this.Handshake(command);

                        if (true == this.VerifyCommand(receivedBytes, out byte[] bodyBytes))
                        {
                            var bytesPid = new ArraySegment<byte>(bodyBytes, 0, 2).ToArray();
                            var bytesValue1 = new ArraySegment<byte>(bodyBytes, 2, 2).Reverse().ToArray();

                            var value1 = this.ConvertHexBytesToInt32(bytesValue1, 0, 2);

                            value = value1;
                        }
                        else
                        {
                            Serilog.Log.Error($"{new StackFrame().GetMethod()?.ReflectedType?.Name ?? "unknown"}");
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
        public override bool ChangeExternalTriggerStrobeTimeUs(int channel, int strobeTimeUs = 0)
        {
            var isSuccess = false;

            try
            {
                var valueInt = 0;
                var strobeTimeUnit = 0;
                if (0 < strobeTimeUs)
                {
                    if (999 < strobeTimeUs)
                    {
                        strobeTimeUnit = 1;
                        valueInt = System.Math.Clamp(Convert.ToInt32(strobeTimeUs / 1000d), 0, 999);
                    }
                    else
                    {
                        valueInt = System.Math.Clamp(strobeTimeUs / 10, 0, 999);
                    }
                }

                Task.Run(async () =>
                {
                    var pid1 = 105;
                    var pid2 = 104;
                    var command = Array.Empty<byte>();

                    if (0 <= this.Channel)
                    {
                        var ch = System.Math.Clamp(this.Channel, 0, 1);
                        ch *= 1000;

                        command = this.FormatCommandForWrite(0, Tuple.Create(pid1 + ch, $"{strobeTimeUnit:X4}"), Tuple.Create(pid2 + ch, $"{valueInt:X4}"));
                    }
                    else
                    {
                        var ch1 = 0;
                        var ch2 = 1000;

                        command = this.FormatCommandForWrite(0, Tuple.Create(pid1 + ch1, $"{strobeTimeUnit:X4}"), Tuple.Create(pid2 + ch1, $"{valueInt:X4}"), Tuple.Create(pid1 + ch2, $"{strobeTimeUnit:X4}"), Tuple.Create(pid2 + ch2, $"{valueInt:X4}"));
                    }

                    var receivedBytes = await this.Handshake(command);

                    if (true == this.VerifyCommand(receivedBytes, out byte[] bodyBytes))
                    {
                    }
                    else
                    {
                        Serilog.Log.Error($"{new StackFrame().GetMethod()?.ReflectedType?.Name ?? "unknown"}");
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
                        var pid1 = 105;
                        var pid2 = 104;
                        var command = Array.Empty<byte>();

                        if (0 <= this.Channel)
                        {
                            var ch = System.Math.Clamp(this.Channel, 0, 1);
                            ch *= 1000;

                            command = this.FormatCommandForRead(0, pid1 + ch, pid2 + ch);
                        }
                        else
                        {
                            var ch1 = 0;
                            var ch2 = 1000;

                            command = this.FormatCommandForRead(0, pid1 + ch1, pid2 + ch1, pid1 + ch2, pid2 + ch2);
                        }

                        var receivedBytes = await this.Handshake(command);

                        if (true == this.VerifyCommand(receivedBytes, out byte[] bodyBytes))
                        {
                            var bytesPid11 = new ArraySegment<byte>(bodyBytes, 0, 2).ToArray();
                            var bytesValue11 = new ArraySegment<byte>(bodyBytes, 2, 2).Reverse().ToArray();
                            var bytesPid21 = new ArraySegment<byte>(bodyBytes, 4, 2).ToArray();
                            var bytesValue21 = new ArraySegment<byte>(bodyBytes, 6, 2).Reverse().ToArray();

                            var strobeTimeUnit = this.ConvertHexBytesToInt32(bytesValue11, 0, bytesValue11.Length);
                            var strobeTimeUs = this.ConvertHexBytesToInt32(bytesValue21, 0, bytesValue21.Length);

                            if (0 < strobeTimeUnit)
                            {
                                value = strobeTimeUs * 1000;
                            }
                            else
                            {
                                value = strobeTimeUs * 10;
                            }
                        }
                        else
                        {
                            Serilog.Log.Error($"{new StackFrame().GetMethod()?.ReflectedType?.Name ?? "unknown"}");
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
        protected TcpBinaryCommunicationClient<TcpBinaryCommunicationLinkageBase> CommunicationClient;

        /// <summary>
        /// ハンドシェイク情報
        /// </summary>
        protected class HandshakeControll : SafelyDisposable
        {
            protected AutoResetEvent? EventBody;

            public Semaphore Semaphore { get; init; } = new(1, 1);

            public ArraySegment<byte> ReceivedBytes { get; protected set; } = Array.Empty<byte>();

            public void ReadyWrite()
            {
                this.EventBody?.Close();
                this.ReceivedBytes = Array.Empty<byte>();

                this.EventBody = new AutoResetEvent(false);
            }

            public void NotifyReceivedEvent(ArraySegment<byte> data)
            {
                var currentEvent = this.EventBody;

                this.ReceivedBytes = data;

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
        public TcpOptexFA_OPPX_LightController() : this(typeof(TcpOptexFA_OPPX_LightController).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpOptexFA_OPPX_LightController(Common.Drawing.Point location) : this(typeof(TcpOptexFA_OPPX_LightController).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public TcpOptexFA_OPPX_LightController(string nickname, Common.Drawing.Point location) : base(nickname, location)
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
        protected virtual void CommunicationClient_TransferDataRead(object sender, ArraySegment<byte> data)
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
        protected async virtual Task<byte[]> Handshake(byte[] command, int timeoutMs = 500)
        {
            var receivedBytes = Array.Empty<byte>();

            try
            {
                if (true == this.CommunicationHandshake.Semaphore.WaitOne())
                {
                    try
                    {
                        this.CommunicationHandshake.ReadyWrite();

                        this.CommunicationClient.AysncWrite(command);

                        await Task<bool>.Run(() =>
                        {
                            this.CommunicationHandshake.WaitReceive(timeoutMs);
                        });

                        receivedBytes = this.CommunicationHandshake.ReceivedBytes.ToArray();
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

            return receivedBytes;
        }

        /// <summary>
        /// int値をbyte配列に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fixedLength"></param>
        /// <returns></returns>
        protected virtual byte[] ConvertToHexBytes(int value, int fixedLength)
        {
            var bytes = BitConverter.GetBytes(value);

            var result = new byte[fixedLength];
            Array.Copy(bytes, result, System.Math.Min(bytes.Length, result.Length));

            return result.Reverse().ToArray();
        }

        /// <summary>
        /// 16進数文字列をbyte配列に変換する
        /// </summary>
        /// <param name="hexText"></param>
        /// <returns></returns>
        protected virtual byte[] ConvertToHexBytes(string hexText, int fixedLength)
        {
            if (hexText.Length % 2 != 0)
            {
                hexText = hexText.Insert(0, "0");
            }

            var bytes = new byte[System.Math.Max(fixedLength, hexText.Length / 2)];

            var hexUnit = string.Empty;
            var hexIndex = 0;
            foreach (var s in hexText)
            {
                hexUnit += s;

                if (2 <= hexUnit.Length)
                {
                    bytes[hexIndex++] = Convert.ToByte(hexUnit, 16);
                    hexUnit = string.Empty;
                }
            }

            return new ArraySegment<byte>(bytes, 0, fixedLength).ToArray();
        }

        protected virtual int ConvertHexBytesToInt32(byte[] bytes, int offset, int length)
        {
            var fixedSizeBytes = BitConverter.GetBytes(0);

            Array.Copy(bytes, offset, fixedSizeBytes, 0, System.Math.Min(fixedSizeBytes.Length, length));

            return BitConverter.ToInt32(fixedSizeBytes);
        }

        /// <summary>
        /// 読み出しコマンド書式化
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual byte[] FormatCommandForRead(int recipeIndex, params int[] paramIndex)
        {
            var cmdWithHead = new byte[2 + 2 + 2 + 2 * paramIndex.Length];

            var i = 0;
            cmdWithHead[i++] = 0x41;
            cmdWithHead[i++] = 0x00;

            cmdWithHead[i++] = 0x52;
            if (recipeIndex < 0)
            {
                cmdWithHead[i++] = 0x00;
            }
            else
            {
                var temp = this.ConvertToHexBytes(recipeIndex, 1);

                cmdWithHead[i++] = (byte)(0x10 | temp[0]);
            }

            var lenBytes = this.ConvertToHexBytes(paramIndex.Length * 2, 2);

            Array.Copy(lenBytes, 0, cmdWithHead, i, lenBytes.Length);
            i += 2;

            foreach (var pid in paramIndex)
            {
                var pidBytes = this.ConvertToHexBytes(pid, 2);
                Array.Copy(pidBytes, 0, cmdWithHead, i, pidBytes.Length);
                i += pidBytes.Length;
            }

            return cmdWithHead;
        }

        /// <summary>
        /// 書き込みコマンド書式化
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual byte[] FormatCommandForWrite(int recipeIndex, params Tuple<int, string>[] paramIndexAndHexValue)
        {
            var cmdWithHead = new List<byte>();

            cmdWithHead.Add(0x41);
            cmdWithHead.Add(0x00);

            cmdWithHead.Add(0x57);
            if (recipeIndex < 0)
            {
                cmdWithHead.Add(0x00);
            }
            else
            {
                var temp = this.ConvertToHexBytes(recipeIndex, 1);

                cmdWithHead.Add((byte)(0x10 | temp[0]));
            }

            var dataLength = 0;
            foreach (var pidAndHex in paramIndexAndHexValue)
            {
                var pidAndHexBytes = new List<byte>();

                // パラメータID
                var pidBytes = this.ConvertToHexBytes(pidAndHex.Item1, 2);
                pidAndHexBytes.AddRange(pidBytes);

                // 書き込み値
                var valBytes = this.ConvertToHexBytes(pidAndHex.Item2, pidAndHex.Item2.Length / 2);
                pidAndHexBytes.AddRange(valBytes);

                cmdWithHead.AddRange(pidAndHexBytes);

                dataLength += pidAndHexBytes.Count;
            }

            var lenBytes = this.ConvertToHexBytes(dataLength, 2);

            cmdWithHead.InsertRange(4, lenBytes);

            return cmdWithHead.ToArray();
        }

        /// <summary>
        /// コマンドチェック
        /// </summary>
        /// <param name="command"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool VerifyCommand(byte[] response, out byte[] value)
        {
            var result = false;

            value = Array.Empty<byte>();

            if (8 <= response.Length)
            {
                var bytesSubHead = new ArraySegment<byte>(response, 0, 2).ToArray();
                var bytesCommand = new ArraySegment<byte>(response, 2, 2).ToArray();
                var byteslength = new ArraySegment<byte>(response, 4, 2).Reverse().ToArray();
                var bytesStatus = new ArraySegment<byte>(response, 7, 1).ToArray();

                var paramIndexAndHexValue = new ArraySegment<byte>(response, 8, response.Length - 8).ToArray();

                var bodyLength = this.ConvertHexBytesToInt32(byteslength, 0, byteslength.Length) - 2;

                if (0 == bytesStatus.Max() && bodyLength == paramIndexAndHexValue.Length)
                {
                    value = paramIndexAndHexValue;
                    result = true;
                }
            }

            return result;
        }

        #endregion
    }
}