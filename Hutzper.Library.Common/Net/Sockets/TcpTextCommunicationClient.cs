using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// TCP 文字列通信クライアント
    /// </summary>
    [Serializable]
    public class TcpTextCommunicationClient : SafelyDisposable, ILoggable
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

        #region プロパティ

        /// <summary>
        /// 文字列エンコーディング
        /// </summary>
        public Encoding Encoding { get; init; }

        /// <summary>
        /// 読み書きが有効かどうか
        /// </summary>
        public bool Enabled => (null != this.tcpClient) && (true == this.tcpClient.Connected);

        #endregion

        #region イベント

        /// <summary>
        /// イベント:接続
        /// </summary>
        public event Action<object>? Connected;

        /// <summary>
        /// イベント:切断
        /// </summary>
        public event Action<object>? Disconnected;

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
        /// クライアント接続
        /// </summary>
        protected TcpClient? tcpClient;

        /// <summary>
        /// 文字列読み書き
        /// </summary>
        protected readonly TcpTextCommunicationLinkage communicationLinkage;

        /// <summary>
        /// 同期用オブジェクト
        /// </summary>
        protected readonly object syncObject = new object();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpTextCommunicationClient(Encoding encoding)
        {
            this.Encoding = encoding;

            this.communicationLinkage = new TcpTextCommunicationLinkage(this.Encoding);
            this.communicationLinkage.StreamInvalidated += CommunicationLinkage_StreamInvalidated;
            this.communicationLinkage.TransferDataRead += (sender, data) =>
            {
                Serilog.Log.Debug($"{this},{MethodBase.GetCurrentMethod()?.Name},RC,{data}");
                this.TransferDataRead?.Invoke(this, data);
            };
        }

        /// <summary>
        /// ストリーム無効
        /// </summary>
        /// <param name="obj"></param>
        private void CommunicationLinkage_StreamInvalidated(object obj) => this.Disconnect();

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.Disconnect();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 接続処理
        /// </summary>
        /// <param name="ipAddress">接続先のIPアドレス</param>
        /// <param name="portNumber">接続先のポート番号</param>
        /// <param name="isUsePing">クライアント接続を試みる前にPingを行うかどうか</param>
        /// <returns>接続したかどうか</returns>
        public bool Connect(string ipAddress, int portNumber, bool isUsePing = true)
        {
            // 既に接続している場合
            if (true == this.Enabled)
            {
                return true;
            }

            var isSuccess = false;

            TcpClient? client = null;

            try
            {
                var canTryConnect = !isUsePing;

                // Pingを使用する場合
                if (true == isUsePing)
                {
                    // Ping実行
                    canTryConnect = NetworkInfo.Ping(ipAddress);

                    // Pingに成功した場合
                    if (true == canTryConnect)
                    {
                        Serilog.Log.Debug($"{this},ping success. {ipAddress}");
                    }
                    // Pingに失敗した場合
                    else
                    {
                        Serilog.Log.Error($"{this},ping fail. {ipAddress}");
                    }
                }

                // Pingを使用しない、またはPingに成功した場合
                if (true == canTryConnect)
                {
                    // クライアント接続を生成する
                    client = new TcpClient();
                    client.Connect(ipAddress, portNumber);

                    // クライアント接続を通信インタフェースに割り当てる
                    this.communicationLinkage.Attach(client.GetStream());

                    isSuccess = client.Connected;

                    Serilog.Log.Debug($"{this},connected. {ipAddress}({portNumber})");
                }
            }
            catch (Exception ex)
            {
                this.DisposeSafely(client);
                client = null;
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.tcpClient = client;
            }

            // 接続に成功した場合
            if (true == isSuccess)
            {
                // 接続イベント通知
                this.Connected?.Invoke(this);
            }

            return isSuccess;
        }

        /// <summary>
        /// 接続済みのクライアントを割り当てる
        /// </summary>
        /// <param name="activeClient"></param>
        /// <returns></returns>
        /// <remarks>TcpSerializationCommunicatorListener経由の使用を想定</remarks>
        public bool Attach(TcpClient activeClient, bool isUsePing = false)
        {
            // 既に接続している場合
            if (true == this.Enabled)
            {
                this.Disconnect();
            }

            try
            {
                // IPアドレスとポート番号を取得する
                var ipAddress = string.Empty;
                var portNumber = 0;
                if (activeClient.Client.RemoteEndPoint != null)
                {
                    if (AddressFamily.InterNetwork == activeClient.Client.RemoteEndPoint.AddressFamily)
                    {
                        ipAddress = activeClient.Client.RemoteEndPoint.AddressFamily.ToString();
                        portNumber = ((IPEndPoint)activeClient.Client.RemoteEndPoint).Port;
                    }
                    else
                    {
                        isUsePing = false;
                    }
                }

                var canTryConnect = !isUsePing;

                // Pingを使用する場合
                if (true == isUsePing)
                {
                    // Ping実行
                    canTryConnect = NetworkInfo.Ping(ipAddress);

                    // Pingに成功した場合
                    if (true == canTryConnect)
                    {
                        Serilog.Log.Debug($"{this},ping success. {ipAddress}");
                    }
                    // Pingに失敗した場合
                    else
                    {
                        Serilog.Log.Error($"{this},ping fail. {ipAddress}");
                    }
                }

                // Pingを使用しない、またはPingに成功した場合
                if (true == canTryConnect)
                {
                    // クライアント接続を通信インタフェースに割り当てる
                    this.communicationLinkage.Attach(activeClient.GetStream());

                    this.tcpClient = activeClient;

                    Serilog.Log.Debug($"{this},connection. {ipAddress}:{portNumber}");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.Enabled;
        }

        /// <summary>
        /// 切断する
        /// </summary>
        public void Disconnect()
        {
            try
            {
                var notifyOnDisconnected = false;

                lock (this.syncObject)
                {
                    try
                    {
                        // 接続している場合
                        if (this.tcpClient is not null)
                        {
                            // 切断
                            this.tcpClient.Close();

                            // 切断イベント通知が必要
                            notifyOnDisconnected = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        this.tcpClient = null;
                    }
                }

                // 切断イベント通知が必要な場合
                if (true == notifyOnDisconnected)
                {
                    this.Disconnected?.Invoke(this);
                }
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
        public void AysncWrite(string data) => this.communicationLinkage.AysncWrite(data);

        #endregion
    }
}