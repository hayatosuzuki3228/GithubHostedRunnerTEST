using Hutzper.Library.Common.Serialization;
using System.Net;
using System.Net.Sockets;

namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// TCPシリアル化通信リスナー
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <remarks>TcpSerializationCommunicatorClientを生成します</remarks>
    public class TcpSerializationCommunicatorListener<T> : SafelyDisposable, ILoggable where T : ISerializableTransferData, new()
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

        #region イベント

        /// <summary>
        /// イベント:クライアント接続
        /// </summary>
        /// <remarks>提供されたクライアント接続を使用しない場合はnotUseClientをfalse にすることで、呼び出し元で破棄されます</remarks>
        public delegate void AcceptTcpClientEventHandler(object sender, TcpSerializationCommunicatorClient<T> client, out bool notUseClient);
        public event AcceptTcpClientEventHandler? AcceptTcpClient;

        #endregion

        #region フィールド

        /// <summary>
        /// スレッド制御フラグ
        /// </summary>
        protected class ThreadControlFlag
        {
            public bool IsTerminate { get; set; }

            public void Terminate() => this.IsTerminate = true;
        }

        /// <summary>
        /// スレッド終了要求
        /// </summary>
        protected ThreadControlFlag threadControlFlag = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="isSingleConnection"></param>
        public TcpSerializationCommunicatorListener()
        {
        }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            this.EndListening();
        }

        #endregion

        #region メソッド

        /// <summary>
        /// リスニング開始
        /// </summary>
        /// <param name="l_ListeningPort">受信接続の試行を待機するポート</param>
        public void BeginListening(int listeningPort)
        {
            try
            {
                // TCPリスナー生成
                var tcpListener = new TcpListener(IPAddress.Any, listeningPort);

                // 接続受付処理をスレッドで実行する
                this.threadControlFlag?.Terminate();
                this.threadControlFlag = new ThreadControlFlag() { IsTerminate = false };
                var listeningThread = new Thread(() =>
                {
                    listeningProcess(tcpListener, this.threadControlFlag);
                })
                { IsBackground = true };

                void listeningProcess(TcpListener activeListener, ThreadControlFlag controlFlag)
                {
                    try
                    {
                        // 受信接続要求待機を開始する
                        activeListener.Start();

                        while (false == controlFlag.IsTerminate)
                        {
                            // 保留中の接続要求を受け付ける
                            var acceptClient = activeListener.AcceptTcpClient();

                            // スレッド終了要求が有効な場合 
                            if (true == controlFlag.IsTerminate)
                            {
                                // 処理を中断する
                                break;
                            }

                            // 通信クライアント
                            var serializationComClient = new TcpSerializationCommunicatorClient<T>();

                            #region 通信クライアントに接続を割り当てイベント通知する
                            try
                            {
                                // クライアント接続割り当て
                                serializationComClient.Attach(acceptClient);

                                #region 接続イベント通知
                                try
                                {
                                    // 接続されたクライアント情報を取得する
                                    var ipAddress = string.Empty;
                                    var portNumber = 0;
                                    if (acceptClient.Client.RemoteEndPoint != null)
                                    {
                                        if (AddressFamily.InterNetwork == acceptClient.Client.RemoteEndPoint.AddressFamily)
                                        {
                                            ipAddress = acceptClient.Client.RemoteEndPoint.AddressFamily.ToString();
                                            portNumber = ((IPEndPoint)acceptClient.Client.RemoteEndPoint).Port;
                                        }
                                    }

                                    Serilog.Log.Debug($"{this},accept client. {ipAddress}:{portNumber})");

                                    // イベント通知
                                    var notUseClient = false;
                                    this.AcceptTcpClient?.Invoke(this, serializationComClient, out notUseClient);
                                    if (true == notUseClient)
                                    {
                                        // 通信クライアント破棄
                                        this.DisposeSafely(serializationComClient);
                                        serializationComClient = null;

                                        // 切断する
                                        acceptClient.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Serilog.Log.Warning(ex, ex.Message);
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);

                                #region 割り当てに失敗した場合はクライアント接続を閉じる
                                try
                                {
                                    // 切断する
                                    acceptClient.Close();
                                }
                                finally
                                {
                                    acceptClient = null;
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        activeListener.Stop();
                    }
                }

                listeningThread.Start();

                Serilog.Log.Debug($"{this},begin listening. {listeningPort})");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// リスニング終了
        /// </summary>
        public void EndListening()
        {
            try
            {
                // スレッド処理を終了する
                this.threadControlFlag?.Terminate();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}