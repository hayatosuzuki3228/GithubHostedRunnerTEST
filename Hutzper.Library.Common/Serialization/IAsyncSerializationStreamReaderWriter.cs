using Hutzper.Library.Common.Threading;
using System.Diagnostics;
using System.Reflection;

namespace Hutzper.Library.Common.Serialization
{
    /// <summary>
    /// 非同期ストリーム読み書き
    /// </summary>
    public interface IAsyncSerializationStreamReaderWriter<T> : ISafelyDisposable, ILoggable where T : ISerializableTransferData, new()
    {
        /// <summary>
        /// ストリーム
        /// </summary>
        /// <param name="stream"></param>
        public void Attach(Stream stream);

        /// <summary>
        /// 非同期書き込み
        /// </summary>
        /// <param name="data"></param>
        public void AysncWrite(T data);

        /// <summary>
        /// ストリーム無効
        /// </summary>
        public event Action<object>? StreamInvalidated;

        /// <summary>
        /// データ読み込み
        /// </summary>
        public event Action<object, T>? TransferDataRead;

        /// <summary>
        /// データ書き込み完了
        /// </summary>
        public event Action<object, T>? TransferDataWriteCompleted;
    }

    /// <summary>
    /// IAsyncSerializationStreamReaderWriter実装
    /// </summary>
    public class AsyncSerializationStreamReaderWriter<T> : SafelyDisposable, IAsyncSerializationStreamReaderWriter<T> where T : ISerializableTransferData, new()
    {
        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>PopularNameとIndexプロパティから成る文字列</remarks>
        public string UniqueName
        {
            #region 取得
            get
            {
                if (0 <= this.Index)
                {
                    return string.Format("{0}{1:D2}", this.Nickname, this.Index + 1);
                }
                else
                {
                    return string.Format("{0}", this.Nickname);
                }
            }
            #endregion
        }

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            #region 取得
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(this.nickname))
                {
                    value = this.nickname;
                }

                return value;
            }
            #endregion

            #region 更新
            set
            {
                this.nickname = value;
            }
            #endregion
        }
        protected string? nickname;

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

        #region イベント

        /// <summary>
        /// ストリーム無効
        /// </summary>
        public event Action<object>? StreamInvalidated;

        /// <summary>
        /// データ読み込み
        /// </summary>
        public event Action<object, T>? TransferDataRead;

        /// <summary>
        /// データ書き込み完了
        /// </summary>
        public event Action<object, T>? TransferDataWriteCompleted;

        #endregion

        #region フィールド

        /// <summary>
        /// 読み込みスレッド
        /// </summary>
        protected Thread? readingThread;

        /// <summary>
        /// 書き込みスレッド
        /// </summary>
        protected QueueThread<Tuple<Stream, T>> writingThread;

        /// <summary>
        /// 通信ストリーム
        /// </summary>
        protected Stream? communicationStream;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AsyncSerializationStreamReaderWriter()
        {
            this.writingThread = new QueueThread<Tuple<Stream, T>>();
            this.writingThread.Dequeue += this.WritingThread_Dequeue;
        }

        #endregion

        /// <summary>
        /// 非同期書き込み
        /// </summary>
        /// <param name="data"></param>
        public void AysncWrite(T data)
        {
            try
            {
                if (null != this.communicationStream)
                {
                    this.writingThread.Enqueue(Tuple.Create(this.communicationStream, data));
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 自身を示す文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.UniqueName;
        }

        /// <summary>
        /// 書き込みスレッド処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void WritingThread_Dequeue(object sender, Tuple<Stream, T> data)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                {
                    data.Item1.WriteByte(0x02);
                    data.Item1.Write(data.Item2.Header.ToArray());
                    data.Item1.WriteByte(0x03);
                    data.Item1.Write(data.Item2.SerializedBytes);
                    data.Item1.Flush();
                }
                Serilog.Log.Verbose($"{this},{MethodBase.GetCurrentMethod()?.Name},WC,{data.Item2.Header.Command},{stopwatch.ElapsedMilliseconds}ms");
            }
            catch (EndOfStreamException eose)
            {
                Serilog.Log.Warning(eose, eose.Message);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                // イベント通知
                this.TransferDataWriteCompleted?.Invoke(this, data.Item2);
            }
        }

        /// <summary>
        /// ストリーム
        /// </summary>
        /// <param name="stream"></param>
        public void Attach(Stream stream)
        {
            try
            {
                base.DisposeSafely(this.communicationStream);

                this.communicationStream = stream;

                // 読み込みスレッド
                this.readingThread = new Thread(async () =>
                {
                    var headerArray = new SerializableTransferHeader(0, 0).ToArray();
                    var headerStream = new MemoryStream();

                    #region 読み込み処理
                    while (this.communicationStream.CanRead)
                    {
                        try
                        {
                            var stopwatch = Stopwatch.StartNew();
                            var retLength = 0;

                            // 受信処理
                            while (headerStream.Length < headerArray.Length)
                            {
                                // STX
                                retLength = await this.communicationStream.ReadAsync(headerArray, 0, 1);

                                var stx = headerArray[0];
                                if (0 > retLength || 0x02 != stx)
                                {
                                    continue;
                                }

                                stopwatch.Restart();
                                headerStream.SetLength(0);

                                // ヘッダの受信
                                do
                                {
                                    var ret = await this.communicationStream.ReadAsync(headerArray, 0, headerArray.Length);

                                    if (0 >= ret)
                                    {
                                        break;
                                    }

                                    headerStream.Write(headerArray, 0, ret);
                                }
                                while (headerStream.Length < headerArray.Length);

                                // ETX
                                retLength = await this.communicationStream.ReadAsync(headerArray, 0, 1);
                                var etx = headerArray[0];
                                if (0 > retLength || 0x03 != etx)
                                {
                                    continue;
                                }
                            }

                            // ヘッダの復元
                            var header = new SerializableTransferHeader(headerStream.ToArray());
                            headerStream.SetLength(0);

                            // データの受信
                            var dataArray = new byte[header.DataBytes];
                            retLength = await this.communicationStream.ReadAsync(dataArray, 0, dataArray.Length);
                            if (dataArray.Length > retLength)
                            {
                                continue;
                            }

                            // データの復元
                            var data = new SerializableTransferData(header, dataArray);
                            Serilog.Log.Verbose($"{this},{MethodBase.GetCurrentMethod()?.Name},RC,{data.Header.Command},{stopwatch.ElapsedMilliseconds}ms");

                            // イベント通知
                            var transferData = (T)(object)data;
                            this.TransferDataRead?.Invoke(this, transferData);
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);

                            this.DisposeSafely(this.communicationStream);
                            this.communicationStream = null;

                            // ストリーム無効通知
                            await Task.Run(() =>
                            {
                                this.StreamInvalidated?.Invoke(this);
                            });

                            break;
                        }
                    }
                    #endregion

                })
                { IsBackground = true };

                this.readingThread.Start();
            }
            catch (Exception ex)
            {
                this.communicationStream = null;
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #region リソースの解放

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.DisposeSafely(this.writingThread);
                this.DisposeSafely(this.communicationStream);
                this.communicationStream = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        #endregion
    }
}