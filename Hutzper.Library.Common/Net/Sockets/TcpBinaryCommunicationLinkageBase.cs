using Hutzper.Library.Common.Threading;
using System.Diagnostics;
using System.Reflection;

namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// TCP バイナリ読み書き基本クラス
    /// </summary>
    public class TcpBinaryCommunicationLinkageBase : SafelyDisposable, ITcpBinaryCommunicationLinkage, ILoggable
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
        public event Action<object, ArraySegment<byte>>? TransferDataRead;

        /// <summary>
        /// データ書き込み完了
        /// </summary>
        public event Action<object, ArraySegment<byte>>? TransferDataWriteCompleted;

        #endregion

        #region フィールド

        /// <summary>
        /// 書き込みスレッド
        /// </summary>
        protected QueueThread<Tuple<Stream, ArraySegment<byte>>> writingThread;

        /// <summary>
        /// 通信ストリーム
        /// </summary>
        protected Stream? communicationStream;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpBinaryCommunicationLinkageBase()
        {
            this.writingThread = new QueueThread<Tuple<Stream, ArraySegment<byte>>>();
            this.writingThread.Dequeue += this.WritingThread_Dequeue;
        }

        #endregion

        /// <summary>
        /// 非同期書き込み
        /// </summary>
        /// <param name="data"></param>
        public virtual void AysncWrite(ArraySegment<byte> data)
        {
            try
            {
                if (null != this.communicationStream && 0 < data.Count)
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
        public override string ToString() => this.UniqueName;

        /// <summary>
        /// 書き込みスレッド処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void WritingThread_Dequeue(object sender, Tuple<Stream, ArraySegment<byte>> data)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                {
                    data.Item1.Write(data.Item2);
                    data.Item1.Flush();
                }
                Serilog.Log.Verbose($"{this},{MethodBase.GetCurrentMethod()?.Name},length = {data.Item2.Count},{stopwatch.ElapsedMilliseconds}ms");
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
        public virtual void Attach(Stream stream, TcpBinaryCommunicationReadCallback? readProcess = null)
        {
            try
            {
                base.DisposeSafely(this.communicationStream);

                this.communicationStream = stream;

                var selectedReadProcess = readProcess ?? this.DefaultReadProcess;

                // 読み込みスレッド
                var readingThread = new Thread(async () =>
                {
                    using var memoryStream = new MemoryStream();

                    var buffer = new byte[2048];

                    #region 読み込み処理
                    while (stream.CanRead)
                    {
                        try
                        {
                            // 受信
                            var received = await stream.ReadAsync(buffer);

                            // 受信長が0以上
                            if (0 < received)
                            {
                                // バッファに追記
                                memoryStream.Seek(0, SeekOrigin.End);
                                memoryStream.Write(buffer, 0, received);
                                memoryStream.Seek(0, SeekOrigin.Begin);

                                // 読み込み処理
                                if (true == selectedReadProcess(memoryStream, out ArraySegment<byte> transferData))
                                {
                                    // イベント通知
                                    this.TransferDataRead?.Invoke(this, transferData);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                            break;
                        }
                    }
                    #endregion

                    // ストリーム無効通知
                    await Task.Run(() =>
                    {
                        this.StreamInvalidated?.Invoke(this);
                    });
                })
                { IsBackground = true };

                readingThread.Start();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// デフォルト読み込み処理
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual bool DefaultReadProcess(MemoryStream memoryStream, out ArraySegment<byte> data)
        {
            data = Array.Empty<byte>();

            try
            {
                data = memoryStream.ToArray();
                memoryStream.SetLength(0);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return true;
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
                this.communicationStream = null;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}