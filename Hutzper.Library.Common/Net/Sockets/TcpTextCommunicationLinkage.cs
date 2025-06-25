using Hutzper.Library.Common.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// TCP文字列通信
    /// </summary>
    [Serializable]
    public class TcpTextCommunicationLinkage : SafelyDisposable, ILoggable
    {
        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
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

        #region SafelyDisposable

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

        /// <summary>
        /// 文字エンコーディング
        /// </summary>
        public Encoding Encoding { get; init; }

        #region イベント

        /// <summary>
        /// ストリーム無効
        /// </summary>
        public event Action<object>? StreamInvalidated;

        /// <summary>
        /// データ読み込み
        /// </summary>
        public event Action<object, string>? TransferDataRead;

        /// <summary>
        /// データ書き込み完了
        /// </summary>
        public event Action<object, string>? TransferDataWriteCompleted;

        #endregion

        #region フィールド

        /// <summary>
        /// 書き込みスレッド
        /// </summary>
        protected QueueThread<Tuple<Stream, string>> writingThread;

        /// <summary>
        /// 通信ストリーム
        /// </summary>
        protected Stream? communicationStream;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="encoding"></param>
        public TcpTextCommunicationLinkage(Encoding encoding)
        {
            this.Encoding = encoding;

            this.writingThread = new QueueThread<Tuple<Stream, string>>();
            this.writingThread.Dequeue += this.WritingThread_Dequeue;
        }

        #endregion

        #region メソッド

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
                var readingThread = new Thread(async () =>
                {
                    var buffer = new byte[1024];

                    #region 読み込み処理
                    while (stream.CanRead)
                    {
                        try
                        {
                            var stopwatch = Stopwatch.StartNew();

                            // データの受信
                            var retLength = await stream.ReadAsync(buffer);

                            if (0 < retLength)
                            {
                                // イベント通知
                                var transferData = this.Encoding.GetString(buffer, 0, retLength);
                                this.TransferDataRead?.Invoke(this, transferData);
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

                    this.DisposeSafely(stream);

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
        /// 非同期書き込み
        /// </summary>
        /// <param name="data"></param>
        public void AysncWrite(string data)
        {
            try
            {
                if (null != this.communicationStream && false == string.IsNullOrEmpty(data))
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

        #endregion

        /// <summary>
        /// 書き込みスレッド処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void WritingThread_Dequeue(object sender, Tuple<Stream, string> data)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                {
                    var bytes = this.Encoding.GetBytes(data.Item2);

                    data.Item1.Write(bytes);
                    data.Item1.Flush();
                }
                Serilog.Log.Verbose($"{this},{MethodBase.GetCurrentMethod()?.Name},WC,{data},{stopwatch.ElapsedMilliseconds}ms");
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
    }
}