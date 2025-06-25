using System.Net.Sockets;

namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// MCプロトコルのPLCデバイス読み書きクラス
    /// </summary>
    public class PlcDeviceReaderWriterMcp : SafelyDisposable, IPlcDeviceReaderWriter
    {
        #region サブクラス

        /// <summary>
        /// コマンド
        /// </summary>
        protected enum Command
        {
            Read,
            Write,
        }

        /// <summary>
        /// 送受信情報
        /// </summary>
        protected class HandshakeUnit : SafelyDisposable
        {
            #region プロパティ

            /// <summary>
            /// 正常な応答かどうか
            /// </summary>
            public bool IsNormalResponse => 0 == this.ReturnCodeValue;

            /// <summary>
            /// 終了コード整数値
            /// </summary>
            public int ReturnCodeValue => (9 <= this.ReceivedHeader.Length) ? BitConverter.ToUInt16(this.ReceivedHeader, 7) : -1;

            /// <summary>
            /// 終了コード16進文字列
            /// </summary>
            public string ReturnCodeText => $"{this.ReturnCodeValue:X4}";

            /// <summary>
            /// 送信電文
            /// </summary>                                
            public byte[] Transmitted { get; protected set; }

            /// <summary>
            /// 受信電文ヘッダ部
            /// </summary>
            public byte[] ReceivedHeader { get; set; } = Array.Empty<byte>();

            /// <summary>
            /// 受信電文データ分
            /// </summary>
            public byte[] ReceivedData { get; set; } = Array.Empty<byte>();

            /// <summary>
            /// 応答受信イベント
            /// </summary>
            public AutoResetEvent ResponseEvent = new AutoResetEvent(false);

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="command"></param>
            public HandshakeUnit(byte[] transmitted) => this.Transmitted = transmitted;

            #endregion

            #region リソースの解放

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.ResponseEvent.Close();
                }
                catch
                {
                }
            }

            #endregion
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ワードデバイス一括処理点数
        /// </summary>
        public int BatchProcessingCountOfWordDevice { get; protected set; }

        /// <summary>
        /// ビットデバイス一括処理点数
        /// </summary>
        public int BatchProcessingCountOfBitDevice { get; protected set; }

        #endregion

        #region フィールド

        protected TcpClient? TcpClient;
        protected BinaryWriter? BinaryWriter;
        protected HandshakeUnit? CurrentHandshakeUnit;

        #endregion

        #region コンストラクタ

        public PlcDeviceReaderWriterMcp()
        {
            this.Nickname = "PlcMc";

            this.TimeoutMs = 1000;

            this.BatchProcessingCountOfWordDevice = 480;
            this.BatchProcessingCountOfBitDevice = 480 * 16;
        }

        #endregion

        #region IPlcDeviceReaderWriter

        #region プロパティ

        /// <summary>
        /// 読み書きが有効かどうか
        /// </summary>
        public bool Enabled => this.TcpClient?.Connected ?? false;

        /// <summary>
        /// タイムアウトミリ秒
        /// </summary>
        public int TimeoutMs { get; set; }

        #endregion

        #region イベント

        /// <summary>
        /// 切断イベント
        /// </summary>
        public event Action<object>? Disconnected;

        #endregion

        #region public メソッド

        /// <summary>
        /// オープン
        /// </summary>
        /// <param name="ipAddress">接続先のIPアドレス</param>
        /// <param name="portNumber">接続先のポート番号</param>
        /// <returns>接続に成功したかどうか</returns>
        /// <remarks>接続を試みます</remarks>
        public virtual bool Open(string ipAddress, int portNumber)
        {
            var isSuccess = false;

            try
            {
                if (true == this.Enabled)
                {
                    throw new Exception("already connected.");
                }

                if (true == NetworkInfo.Ping(ipAddress))
                {
                    var client = new TcpClient();
                    client.Connect(ipAddress, portNumber);

                    this.TcpClient = client;
                    this.BinaryWriter = new BinaryWriter(this.TcpClient.GetStream());

                    #region 読み込みスレッド処理を起動する
                    var readingThread = new Thread(async () =>
                    {
                        var binaryReader = (BinaryReader?)null;

                        try
                        {
                            binaryReader = new BinaryReader(this.TcpClient.GetStream());

                            while (binaryReader.BaseStream.CanRead)
                            {
                                #region 繰り返し読み出し処理
                                try
                                {
                                    // サブヘッダ長分読み込む(読み捨てる)
                                    var subHeader = binaryReader.ReadBytes(2);

                                    // QnAヘッダ長分読み込む(終了コードを含む)
                                    var header = binaryReader.ReadBytes(9);

                                    // 現在の送受信情報への参照を取得する
                                    var unit = this.CurrentHandshakeUnit;

                                    // 応答データ長を取得する(既に読み込んだ終了コード2byte分を引く)
                                    var length = BitConverter.ToUInt16(header, 5) - 2;

                                    // 応答データを初期化する
                                    var data = Array.Empty<byte>();

                                    // 応答データが存在する場合(読み出しデータ or エラー情報)
                                    if (0 < length)
                                    {
                                        // 応答データを読み出す
                                        data = binaryReader.ReadBytes(length);
                                    }

                                    // 送受信情報が有効な場合
                                    if (unit is not null)
                                    {
                                        Serilog.Log.Verbose($"{this}, command reception, [HEADER]:{BitConverter.ToString(header)}, [DATA]:{BitConverter.ToString(data)}");

                                        // 受信情報を格納して受信イベントを通知する
                                        unit.ReceivedHeader = header;
                                        unit.ReceivedData = data;

                                        try
                                        {
                                            unit.ResponseEvent.Set();
                                        }
                                        catch (ObjectDisposedException ex)
                                        {
                                            Serilog.Log.Warning(ex, ex.Message);
                                        }
                                    }
                                    // 送受信情報が無効な場合
                                    else
                                    {
                                        Serilog.Log.Warning($"{this}, unscheduled reception, [HEADER]:{BitConverter.ToString(header)}, [DATA]:{BitConverter.ToString(data)}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (true == client.Connected)
                                    {
                                        Serilog.Log.Warning(ex, "readingThread");
                                    }
                                    break;
                                }
                                #endregion
                            }

                            // クライアント接続を閉じる
                            try
                            {
                                this.DisposeSafely(binaryReader);
                                this.DisposeSafely(this.BinaryWriter);
                                client.Close();
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, "closing");
                            }
                            finally
                            {
                                this.TcpClient = null;
                                this.BinaryWriter = null;
                            }

                            // ストリーム無効通知
                            await Task.Run(() =>
                            {
                                this.OnDisconnected(this);
                            });
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    })
                    { IsBackground = true };

                    readingThread.Start();
                    #endregion

                    isSuccess = true;

                    Serilog.Log.Debug($"{this}, connected. {ipAddress}({portNumber})");
                }
                else
                {
                    Serilog.Log.Error($"{this}, ping failed. {ipAddress}");
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
        /// <remarks>切断します</remarks>
        public virtual void Close()
        {
            try
            {
                if (this.TcpClient?.Connected ?? false)
                {
                    this.TcpClient?.Close();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ワードデバイスを読み込む
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">読み取り値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>読取に成功したかどうか</returns>
        public virtual bool ReadDevice(WordDeviceType deviceType, int address, int number, out List<int> values, out string errorMessage)
        {
            var isSuccess = true;

            values = new List<int>();
            errorMessage = string.Empty;

            try
            {
                // バッチ設定
                var batchList = this.GetBatchList(number, this.BatchProcessingCountOfWordDevice);

                // バッチ処理
                var batchAddress = address;
                foreach (var selectedBatch in batchList)
                {
                    isSuccess &= this.BatchRead(deviceType, batchAddress, selectedBatch, out List<int> batchValues, out string batchError);

                    if (false == isSuccess)
                    {
                        errorMessage = batchError;
                        break;
                    }
                    else
                    {
                        values.AddRange(batchValues);
                    }

                    batchAddress += selectedBatch;
                }

                if (false == isSuccess)
                {
                    values.Clear();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"rw, error";
                Serilog.Log.Warning(ex, $"{this}, {errorMessage}");
            }

            return isSuccess;
        }

        /// <summary>
        /// ビットデバイスを読み込む
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">読み取り値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>読取に成功したかどうか</returns>
        /// <remarks>論理値として結果を取得します</remarks>
        public virtual bool ReadDevice(BitDeviceType deviceType, int address, int number, out List<bool> values, out string errorMessage)
        {
            var result = this.BatchRead(deviceType, address, number, out List<int> tempValues, out errorMessage);

            values = tempValues.ConvertAll(i => Convert.ToBoolean(i));

            return result;
        }

        /// <summary>
        /// ビットデバイスを読み込む
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">読み取り値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>読取に成功したかどうか</returns>
        /// <remarks>整数値として結果を取得します</remarks>
        public virtual bool ReadDevice(BitDeviceType deviceType, int address, int number, out List<int> values, out string errorMessage)
        {
            var isSuccess = true;

            values = new List<int>();
            errorMessage = string.Empty;

            try
            {
                // バッチ設定
                var batchList = this.GetBatchList(number, this.BatchProcessingCountOfBitDevice);

                // バッチ処理
                var batchAddress = address;
                foreach (var selectedBatch in batchList)
                {
                    isSuccess &= this.BatchRead(deviceType, batchAddress, selectedBatch, out List<int> batchValues, out string batchError);

                    if (false == isSuccess)
                    {
                        errorMessage = batchError;
                        break;
                    }
                    else
                    {
                        values.AddRange(batchValues);
                    }

                    batchAddress += selectedBatch;
                }

                if (false == isSuccess)
                {
                    values.Clear();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"rb, error";
                Serilog.Log.Warning(ex, $"{this}, {errorMessage}");
            }

            return isSuccess;
        }

        /// <summary>
        /// ワードデバイスに書き込む
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">書き込み値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>書き込みにに成功したかどうか</returns>
        public virtual bool WriteDevice(WordDeviceType deviceType, int address, int number, List<int> values, out string errorMessage)
        {
            var isSuccess = true;

            errorMessage = string.Empty;

            try
            {
                // バッチ設定
                var batchValues = this.GetBatchValues(number, this.BatchProcessingCountOfWordDevice, values);

                // バッチ処理
                var batchAddress = address;
                foreach (var selectedBatch in batchValues)
                {
                    isSuccess &= this.BatchWrite(deviceType, batchAddress, selectedBatch.Count, selectedBatch, out string batchError);

                    if (false == isSuccess)
                    {
                        errorMessage = batchError;
                        break;
                    }

                    batchAddress += selectedBatch.Count;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"ww, error";
                Serilog.Log.Warning(ex, $"{this}, {errorMessage}");
            }

            return isSuccess;
        }

        /// <summary>
        /// ビットデバイスに書き込む
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">書き込み値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>書き込みにに成功したかどうか</returns>
        public virtual bool WriteDevice(BitDeviceType deviceType, int address, int number, List<bool> values, out string errorMessage)
        {
            var writeValues = values.ConvertAll(i => Convert.ToInt32(i));

            return this.WriteDevice(deviceType, address, number, writeValues, out errorMessage);
        }

        /// <summary>
        /// ビットデバイスに書き込む
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">書き込み値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>書き込みにに成功したかどうか</returns>
        /// <remarks>書き込み値は0か0以外の論理型として扱われます</remarks>
        public virtual bool WriteDevice(BitDeviceType deviceType, int address, int number, List<int> values, out string errorMessage)
        {
            var isSuccess = true;

            errorMessage = string.Empty;

            try
            {
                // バッチ設定
                var batchValues = this.GetBatchValues(number, this.BatchProcessingCountOfBitDevice, values);

                // バッチ処理
                var batchAddress = address;
                foreach (var selectedBatch in batchValues)
                {
                    isSuccess &= this.BatchWrite(deviceType, batchAddress, selectedBatch.Count, selectedBatch, out string batchError);

                    if (false == isSuccess)
                    {
                        errorMessage = batchError;
                        break;
                    }

                    batchAddress += selectedBatch.Count;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"wb, error";
                Serilog.Log.Warning(ex, $"{this}, {errorMessage}");
            }

            return isSuccess;
        }

        #endregion

        #endregion

        #region publicメソッド

        /// <summary>
        /// 現在のオブジェクトを表す文字列を返します
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.UniqueName;

        #endregion

        #region protectedメソッド

        /// <summary>
        /// 切断イベントを通知する
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnDisconnected(object sender) => this.Disconnected?.Invoke(sender);

        /// <summary>
        /// ワードデバイス一括読み込み
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス数</param>
        /// <param name="values">読み込んだデバイス値の配列</param>
        /// <returns>成功/失敗</returns>
        protected bool BatchRead(WordDeviceType deviceType, int address, int number, out List<int> values, out string errorMessage)
        {
            var isSuccess = false;

            values = new List<int>();
            errorMessage = string.Empty;

            try
            {
                // 接続している場合
                if (true == this.Enabled)
                {
                    // 電文を作成する
                    var telegram = this.CreateHeader(Command.Read, deviceType, address, number);

                    // 送受信情報を作成する
                    this.CurrentHandshakeUnit = new HandshakeUnit(telegram.ToArray());

                    // 電文を送信する
                    if (true == this.WriteAndFlush(this.BinaryWriter, this.CurrentHandshakeUnit.Transmitted))
                    {
                        // 電文を受信した場合
                        if (true == this.CurrentHandshakeUnit.ResponseEvent.WaitOne(this.TimeoutMs))
                        {
                            // 正常応答の場合
                            if (true == this.CurrentHandshakeUnit.IsNormalResponse)
                            {
                                // データ長が正しい場合
                                if (number <= this.CurrentHandshakeUnit.ReceivedData.Length / 2)
                                {
                                    // 読み込んだデバイス値を取得する                                    
                                    for (var i = 0; i < number; i++)
                                    {
                                        values.Add(BitConverter.ToUInt16(this.CurrentHandshakeUnit.ReceivedData, i * 2));
                                    }

                                    // 成功とする
                                    isSuccess = true;
                                }
                            }
                            // 異常応答の場合
                            else
                            {
                                errorMessage = $"rw, erroneous response, [CODE]:{this.CurrentHandshakeUnit.ReturnCodeText}";
                                Serilog.Log.Warning($"{this}, {errorMessage}");
                            }
                        }
                        // タイムアウトした場合
                        else
                        {
                            errorMessage = $"rw, reception timeout";
                            Serilog.Log.Warning($"{this}, {errorMessage}");
                        }
                    }
                    // 送信に失敗した場合   
                    else
                    {
                        errorMessage = $"rw, transmission failure";
                        Serilog.Log.Error($"{this}, {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"rw, error";
                Serilog.Log.Warning(ex, $"{this}, {errorMessage}");
            }
            finally
            {
                // 送受信情報を無効化する
                this.DisposeSafely(this.CurrentHandshakeUnit);
                this.CurrentHandshakeUnit = null;
            }

            return isSuccess;
        }

        /// <summary>
        /// ビットデバイス一括読み込み
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス数</param>
        /// <param name="values">読み込んだデバイス値の配列</param>
        /// <returns>成功/失敗</returns>
        protected bool BatchRead(BitDeviceType deviceType, int address, int number, out List<bool> values, out string errorMessage)
        {
            var tempValues = new List<int>();

            var result = this.BatchRead(deviceType, address, number, out tempValues, out errorMessage);

            values = tempValues.ConvertAll(i => Convert.ToBoolean(i));

            return result;
        }

        /// <summary>
        /// ビットデバイス一括読み込み
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス数</param>
        /// <param name="values">読み込んだデバイス値の配列</param>
        /// <returns>成功/失敗</returns>
        protected bool BatchRead(BitDeviceType deviceType, int address, int number, out List<int> values, out string errorMessage)
        {
            var isSuccess = false;

            values = new List<int>();
            errorMessage = string.Empty;

            try
            {
                // 接続している場合
                if (true == this.Enabled)
                {
                    // 電文を作成する
                    var telegram = this.CreateHeader(Command.Read, deviceType, address, number);

                    // 送受信情報を作成する
                    this.CurrentHandshakeUnit = new HandshakeUnit(telegram.ToArray());

                    // 電文を送信する
                    if (true == this.WriteAndFlush(this.BinaryWriter, this.CurrentHandshakeUnit.Transmitted))
                    {
                        // 電文を受信した場合
                        if (true == this.CurrentHandshakeUnit.ResponseEvent.WaitOne(this.TimeoutMs))
                        {
                            // 正常応答の場合
                            if (true == this.CurrentHandshakeUnit.IsNormalResponse)
                            {
                                // データ長が正しい場合
                                if (number <= this.CurrentHandshakeUnit.ReceivedData.Length * 2)
                                {
                                    // 読み込んだデバイス値を取得する   
                                    var tempValues = new List<int>();
                                    foreach (var selectedValue in this.CurrentHandshakeUnit.ReceivedData)
                                    {
                                        int valueL = 0x0F & selectedValue;
                                        int valueH = 0x0F & (selectedValue >> 4);
                                        tempValues.Add(valueH);
                                        tempValues.Add(valueL);
                                    }

                                    if (number < tempValues.Count)
                                    {
                                        tempValues.RemoveAt(tempValues.Count - 1);
                                    }

                                    values.AddRange(tempValues);

                                    // 成功とする
                                    isSuccess = true;
                                }
                            }
                            // 異常応答の場合
                            else
                            {
                                errorMessage = $"rb, erroneous response, [CODE]:{this.CurrentHandshakeUnit.ReturnCodeText}";
                                Serilog.Log.Warning($"{this}, {errorMessage}");
                            }
                        }
                        // タイムアウトした場合
                        else
                        {
                            errorMessage = $"rb, reception timeout";
                            Serilog.Log.Warning($"{this}, {errorMessage}");
                        }
                    }
                    // 送信に失敗した場合
                    else
                    {
                        errorMessage = $"rb, transmission failure";
                        Serilog.Log.Error($"{this}, {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"rb, error";
                Serilog.Log.Warning(ex, $"{this}, {errorMessage}");
            }
            finally
            {
                // 送受信情報を無効化する
                base.DisposeSafely(this.CurrentHandshakeUnit);
                this.CurrentHandshakeUnit = null;
            }

            return isSuccess;
        }

        /// <summary>
        /// ワードデバイス一括書き込み
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス数</param>
        /// <param name="values">書き込むデバイス値の配列</param>
        /// <returns>成功/失敗</returns>
        protected bool BatchWrite(WordDeviceType deviceType, int address, int number, List<int> values, out string errorMessage)
        {
            var isSuccess = false;

            errorMessage = string.Empty;

            try
            {
                // 接続している場合
                if (true == this.Enabled)
                {
                    // 電文を作成する
                    var telegram = this.CreateHeader(Command.Write, deviceType, address, number);

                    // 電文に書き込みデータを追加する
                    foreach (var selectedValue in values)
                    {
                        telegram.AddRange(BitConverter.GetBytes((ushort)selectedValue));
                    }

                    // 送受信情報を作成する
                    this.CurrentHandshakeUnit = new HandshakeUnit(telegram.ToArray());

                    // 電文を送信する
                    if (true == this.WriteAndFlush(this.BinaryWriter, this.CurrentHandshakeUnit.Transmitted))
                    {
                        // 電文を受信した場合
                        if (true == this.CurrentHandshakeUnit.ResponseEvent.WaitOne(this.TimeoutMs))
                        {
                            // 正常応答の場合
                            if (true == this.CurrentHandshakeUnit.IsNormalResponse)
                            {
                                // 成功とする
                                isSuccess = true;
                            }
                            // 異常応答の場合
                            else
                            {
                                errorMessage = $"ww, erroneous response, [CODE]:{this.CurrentHandshakeUnit.ReturnCodeText}";
                                Serilog.Log.Warning($"{this}, {errorMessage}");
                            }
                        }
                        // タイムアウトした場合
                        else
                        {
                            errorMessage = $"ww, reception timeout";
                            Serilog.Log.Warning($"{this}, {errorMessage}");
                        }
                    }
                    // 送信に失敗した場合 
                    else
                    {
                        errorMessage = $"ww, transmission failure";
                        Serilog.Log.Error($"{this}, {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"ww, error";
                Serilog.Log.Warning(ex, $"{this}, {errorMessage}");
            }
            finally
            {
                // 送受信情報を無効化する
                this.DisposeSafely(this.CurrentHandshakeUnit);
                this.CurrentHandshakeUnit = null;
            }

            return isSuccess;
        }

        /// <summary>
        /// ビットデバイス一括書き込み
        /// </summary>
        /// <param name="deviceType">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス数</param>
        /// <param name="values">書き込むデバイス値の配列</param>
        /// <returns>成功/失敗</returns>
        protected bool BatchWrite(BitDeviceType deviceType, int address, int number, List<int> values, out string errorMessage)
        {
            bool isSuccess = false;

            errorMessage = string.Empty;

            try
            {
                // 接続している場合
                if (true == this.Enabled)
                {
                    // 電文を作成する
                    var telegram = this.CreateHeader(Command.Write, deviceType, address, number);

                    // 書き込み個数の偶数化
                    var writeValues = new List<int>(values);
                    if (0 < (writeValues.Count % 2))
                    {
                        // 書き込み値が奇数の場合はダミー追加
                        writeValues.Add(0);
                    }

                    // 電文に書き込みデータを追加する(1点を4ビット単位で上位ビットから格納)
                    var index = 0;
                    for (var i = 0; i < writeValues.Count / 2; i++)
                    {
                        byte combinedValue = Convert.ToByte(writeValues[index++] << 4);
                        combinedValue |= Convert.ToByte(writeValues[index++]);
                        telegram.Add(combinedValue);
                    }

                    // 送受信情報を作成する
                    this.CurrentHandshakeUnit = new HandshakeUnit(telegram.ToArray());

                    // 電文を送信する
                    if (true == this.WriteAndFlush(this.BinaryWriter, this.CurrentHandshakeUnit.Transmitted))
                    {
                        // 電文を受信した場合
                        if (true == this.CurrentHandshakeUnit.ResponseEvent.WaitOne(this.TimeoutMs))
                        {
                            // 正常応答の場合
                            if (true == this.CurrentHandshakeUnit.IsNormalResponse)
                            {
                                // 成功とする
                                isSuccess = true;
                            }
                            // 異常応答の場合
                            else
                            {
                                errorMessage = $"wb, erroneous response, [CODE]:{this.CurrentHandshakeUnit.ReturnCodeText}";
                                Serilog.Log.Warning($"{this}, {errorMessage}");
                            }
                        }
                        // タイムアウトした場合
                        else
                        {
                            errorMessage = $"wb, reception timeout";
                            Serilog.Log.Warning($"{this}, {errorMessage}");
                        }
                    }
                    // 送信に失敗した場合  
                    else
                    {
                        errorMessage = $"wb, transmission failure";
                        Serilog.Log.Error($"{this}, {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"wb, error";
                Serilog.Log.Warning(ex, $"{this}, {errorMessage}");
            }
            finally
            {
                // 送受信情報を無効化する
                this.DisposeSafely(this.CurrentHandshakeUnit);
                this.CurrentHandshakeUnit = null;
            }

            return isSuccess;
        }

        /// <summary>
        /// 書き込み
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual bool WriteAndFlush(BinaryWriter? writer, byte[] data)
        {
            var isSuccess = false;

            if (writer is not null)
            {
                try
                {
                    writer.Write(data);
                    writer.Flush();

                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 電文ヘッダ作成(ワードデバイス)
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="kind">デバイス種</param>
        /// <param name="address">アドレス</param>
        /// <param name="number">デバイス数</param>
        /// <returns></returns>
        protected List<byte> CreateHeader(Command command, WordDeviceType kind, int address, int number)
        {
            var header = this.CreateHeader(command);

            if (Command.Write == command)
            {
                var length = BitConverter.ToUInt16(header.ToArray(), 5);
                length += (ushort)(number * 2);

                var lengthBytes = BitConverter.GetBytes(length);

                header[5] = lengthBytes[0];
                header[6] = lengthBytes[1];
            }

            header.Add(0x00);   // サブコマンド:L
            header.Add(0x00);   // サブコマンド:H

            // 先頭デバイス
            var addresAndDeviceCode = BitConverter.GetBytes(address);

            // デバイスコード
            switch (kind)
            {
                case WordDeviceType.W:
                    {
                        addresAndDeviceCode[addresAndDeviceCode.Length - 1] = 0xB4;
                    }
                    break;

                case WordDeviceType.D:
                    {
                        addresAndDeviceCode[addresAndDeviceCode.Length - 1] = 0xA8;
                    }
                    break;

                default:
                    {
                        throw new Exception("undefined device was specified");
                    }
            }

            header.AddRange(addresAndDeviceCode);

            header.AddRange(BitConverter.GetBytes((ushort)number));  // デバイス点数

            // サブヘッダを先頭に追加
            header.Insert(0, 0x50);
            header.Insert(1, 0x00);

            return header;
        }

        /// <summary>
        /// 電文ヘッダ作成(ビットデバイス)
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="kind">デバイス種</param>
        /// <param name="address">アドレス</param>
        /// <param name="number">デバイス数</param>
        /// <returns></returns>
        protected List<byte> CreateHeader(Command command, BitDeviceType kind, int address, int number)
        {
            var header = this.CreateHeader(command);

            if (Command.Write == command)
            {
                var length = BitConverter.ToUInt16(header.ToArray(), 5);
                length += (ushort)(number / 2);
                length += (ushort)(number % 2);

                var lengthBytes = BitConverter.GetBytes(length);

                header[5] = lengthBytes[0];
                header[6] = lengthBytes[1];
            }

            header.Add(0x01);   // サブコマンド:L
            header.Add(0x00);   // サブコマンド:H

            // 先頭デバイス
            var addresAndDeviceCode = BitConverter.GetBytes(address);

            // デバイスコード
            switch (kind)
            {
                case BitDeviceType.B:
                    {
                        addresAndDeviceCode[addresAndDeviceCode.Length - 1] = 0xA0;
                    }
                    break;

                case BitDeviceType.M:
                    {
                        addresAndDeviceCode[addresAndDeviceCode.Length - 1] = 0x90;
                    }
                    break;

                default:
                    {
                        throw new Exception("undefined device was specified");
                    }
            }

            header.AddRange(addresAndDeviceCode);

            header.AddRange(BitConverter.GetBytes((ushort)number));  // デバイス点数

            // サブヘッダを先頭に追加
            header.Insert(0, 0x50);
            header.Insert(1, 0x00);

            return header;
        }

        /// <summary>
        /// 指定されたコマンドに対応する電文ヘッダを作成する
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <returns>電文ヘッダ</returns>
        protected List<byte> CreateHeader(Command command)
        {
            var header = new List<byte>
            {
                0x00,   // ネットワーク番号
                0xFF,   // PC番号
                0xFF,   // 固有値1;L
                0x03,   // 固有値1:H
                0x00   // 固有値2
            };

            // 要求データ長
            ushort length = 12;
            header.AddRange(BitConverter.GetBytes(length));

            header.Add(0x10);   // CPU監視タイマ:L
            header.Add(0x00);   // CPU監視タイマ:H

            // コマンド
            switch (command)
            {
                case Command.Read:
                    {
                        header.Add(0x01);
                        header.Add(0x04);
                    }
                    break;

                case Command.Write:
                    {
                        header.Add(0x01);
                        header.Add(0x14);
                    }
                    break;

                default:
                    {
                        throw new Exception("undefined command was specified");
                    }
            }

            return header;
        }

        /// <summary>
        /// バッチリストを取得する
        /// </summary>
        /// <param name="totalNumber">合計数</param>
        /// <param name="batchUnit">バッチ単位</param>
        /// <returns>バッチリスト</returns>
        protected List<int> GetBatchList(int totalNumber, int batchUnit)
        {
            var batchList = new List<int>();

            if (batchUnit < totalNumber)
            {
                var batchQuotient = totalNumber / batchUnit;

                batchList = new List<int>(Enumerable.Repeat(batchUnit, batchQuotient));

                var batchRemainder = totalNumber % batchUnit;
                if (0 < batchRemainder)
                {
                    batchList.Add(batchRemainder);
                }
            }
            else
            {
                batchList.Add(totalNumber);
            }

            return batchList;
        }

        /// <summary>
        /// バッチリストを取得する
        /// </summary>
        /// <param name="totalNumber">合計数</param>
        /// <param name="batchUnit">バッチ単位</param>
        /// <param name="values">書き込み値</param>
        /// <returns></returns>
        protected List<List<int>> GetBatchValues(int totalNumber, int batchUnit, List<int> values)
        {
            var batchValues = new List<List<int>>();

            if (batchUnit < totalNumber)
            {
                var batchQuotient = totalNumber / batchUnit;
                var batchOffset = 0;
                for (var i = 0; i < batchQuotient; i++)
                {
                    batchValues.Add(values.GetRange(batchOffset, batchUnit));

                    batchOffset += batchUnit;
                }

                int batchRemainder = totalNumber % batchUnit;
                if (0 < batchRemainder)
                {
                    batchValues.Add(values.GetRange(batchOffset, batchRemainder));
                }
            }
            else
            {
                batchValues.Add(values);
            }

            return batchValues;
        }

        #endregion

        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName => (0 <= this.Index) ? $"{this.Nickname}{this.Index + 1:D2}" : $"{this.Nickname}";

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(this.nickname))
                {
                    value = this.nickname;
                }

                return value;
            }

            set => this.nickname = value;
        }
        protected string nickname = String.Empty;

        /// <summary>
        /// 整数インデックス
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public void Attach(string nickname, int index)
        {
            try
            {
                this.nickname = nickname;
                this.Index = index;
            }
            catch
            {
            }
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
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}