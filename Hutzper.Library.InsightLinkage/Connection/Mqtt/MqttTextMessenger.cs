using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.InsightLinkage.Controller;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace Hutzper.Library.InsightLinkage.Connection.Mqtt
{
    /// <summary>
    /// Mqtt テキストメッセンジャーパラメータ
    /// </summary>
    [Serializable]
    public class MqttTextMessenger : ControllerBase, ITextMessenger
    {
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
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);
                if (parameter is ITextMessengerParameter p)
                {
                    this.Parameter = p;
                }
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
            try
            {
                if (this.Parameter is MqttTextMessengerParameter p)
                {
                    if (false == (this.MqttClient?.IsConnected ?? false))
                    {
                        var path = Path.Combine(this.PathManager?.Root?.FullName ?? AppDomain.CurrentDomain.BaseDirectory, "mqtt");

                        var clientCert = new X509Certificate2(Path.Combine(path, p.FileNameOfClientCA), p.PasswordOfClientCA);
                        var rootCa = X509Certificate.CreateFromCertFile(Path.Combine(path, p.FileNameOfRootCA));

                        this.MqttClient = new MqttClient(p.EndpointHost, p.EndpointPort, true, rootCa, clientCert, MqttSslProtocols.TLSv1_2);

                        this.MqttClient.Connect(p.Uuid);
                    }
                }
            }
            catch (Exception ex)
            {
                this.MqttClient = null;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            var isSuccess = true;

            try
            {
                this.MqttClient?.Disconnect();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.MqttClient = null;
            }

            return isSuccess;
        }

        #endregion

        #region IConnection

        /// <summary>
        /// 有効かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled => this.MqttClient?.IsConnected ?? false;

        /// <summary>
        /// Insight連携タイプ
        /// </summary>
        public InsightLinkageType LinkageType => InsightLinkageType.TextMessaging;

        #endregion

        #region ITextMessenger

        /// <summary>
        /// テキストを送信する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool SendText(string text)
        {
            var isSuccess = this.Enabled;

            try
            {
                #region バッファリング処理
                lock (((ICollection)this.MessageSendingQueue).SyncRoot)
                {
                    try
                    {
                        // キューの末尾に要求を追加する
                        this.MessageSendingQueue.Enqueue(text);

                        // 最大バッファリング数に制限する
                        if (this.Parameter is MqttTextMessengerParameter param)
                        {
                            // 最大バッファリング数を超過している場合
                            while (param.MaximumNumberOfBuffers < this.MessageSendingQueue.Count)
                            {
                                // 最古の要求を除外する
                                var removedText = this.MessageSendingQueue.Dequeue();

                                Serilog.Log.Warning($"mqtt buffer overflow, removed = {removedText}");
                            }
                        }

                        // 要素追加をイベント通知
                        this.MessageSendingQueueEvent.Set();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                }
                #endregion
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
                // スレッドの解放
                this.MessageSendingThreadTerminateFlag = true;
                this.MessageSendingQueueEvent.Set();

                this.Close();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// パラメータ
        /// </summary>
        protected IControllerParameter? Parameter;

        /// <summary>
        /// MQTTクライアント
        /// </summary>
        protected MqttClient? MqttClient;

        /// <summary>
        /// メッセージ送信スレッド
        /// </summary>
        protected Thread MessageSendingThread;
        protected bool MessageSendingThreadTerminateFlag;

        /// <summary>
        /// キューイベント
        /// </summary>
        protected readonly AutoResetEvent MessageSendingQueueEvent;

        /// <summary>
        /// キュー
        /// </summary>
        protected readonly Queue<string> MessageSendingQueue;

        private string? _InsightPath = null;
        private bool IsValidOptionValues = true;

        public void SetInsightPath(string path)
        {
            this._InsightPath = path;
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public MqttTextMessenger() : this(typeof(MqttTextMessenger).Name, -1)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public MqttTextMessenger(string nickname, int index = -1) : base(nickname, index)
        {
            // キューの生成
            this.MessageSendingQueue = new Queue<string>();

            // キュー操作イベントの生成
            this.MessageSendingQueueEvent = new AutoResetEvent(false);

            // スレッドの生成
            this.MessageSendingThreadTerminateFlag = false;
            this.MessageSendingThread = new Thread(this.ThreadProcess)
            {
                IsBackground = true
            };
            this.MessageSendingThread.Start();
        }

        #endregion

        /// <summary>
        /// スレッド処理
        /// </summary>
        protected virtual void ThreadProcess()
        {
            try
            {
                // 終了指示があるまで繰り返し処理
                while (false == this.MessageSendingThreadTerminateFlag)
                {
                    // イベント待ち
                    this.MessageSendingQueueEvent.WaitOne();

                    // 終了指示がある場合
                    if (true == this.MessageSendingThreadTerminateFlag)
                    {
                        // 中断
                        break;
                    }

                    // 終了指示があるまで繰り返し処理
                    while (false == this.MessageSendingThreadTerminateFlag)
                    {
                        var text = (string?)null;

                        // キューから送信データを取り出す
                        lock (((ICollection)this.MessageSendingQueue).SyncRoot)
                        {
                            if (0 < this.MessageSendingQueue.Count)
                            {
                                text = this.MessageSendingQueue.Dequeue();
                            }
                        }

                        // 送信データが存在する場合
                        if (false == string.IsNullOrEmpty(text))
                        {
                            // 送信データのバリデーション
                            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(text);
                            var options = jsonObject["option"] as Newtonsoft.Json.Linq.JObject;
                            if (options != null)
                            {
                                foreach (var option in options.Properties())
                                {
                                    // クラス名のバリデーション
                                    string className = option.Name;
                                    if (!ValidateClassName(className))
                                    {
                                        Serilog.Log.Warning($"Invalid character in class name: {className}, Stop sending mqtt text message.");
                                        this.IsValidOptionValues = false;
                                        break;
                                    }
                                    // 確率値の整数部分をチェックし、整数の場合に.0を付与
                                    if (decimal.TryParse(option.Value.ToString(), out decimal result) && result == Math.Truncate(result))
                                    {
                                        option.Value = $"{result}.0";
                                    }
                                }
                            }

                            #region mqttメッセージを送信する
                            try
                            {
                                if (this.Parameter is MqttTextMessengerParameter p && p.IsUse && this.IsValidOptionValues)
                                {
                                    var topicText = p.Uuid.Trim();
                                    if (false == string.IsNullOrEmpty(p.Topic.Trim()))
                                    {
                                        topicText = $"{p.Topic.Trim()}/{topicText}";
                                    }
                                    if (this._InsightPath == null)
                                    {
                                        if (true == this.Open())
                                        {
                                            this.MqttClient?.Publish($"{topicText.ToLower()}", Encoding.UTF8.GetBytes($"{text.ToLower()}"), 1, false);
                                            Serilog.Log.Debug($"mqtt send '{text.ToLower()}'");
                                        }
                                        else
                                        {
                                            Serilog.Log.Warning($"mqtt connection failed, {text.ToLower()}");
                                        }
                                    }
                                    else
                                    {
                                        if (true == this.Open())
                                        {
                                            this.MqttClient?.Publish($"{topicText.ToLower()}", Encoding.UTF8.GetBytes($"{text.ToLower()}"), 1, false);
                                            Serilog.Log.Debug($"mqtt send '{text.ToLower()}'");
                                        }
                                        else
                                        {
                                            Serilog.Log.Warning($"mqtt connection failed, {text.ToLower()}");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                            #endregion
                        }
                        // 送信データが存在しない場合
                        else
                        {
                            // 中断して次のイベントシグナル待ち
                            break;
                        }
                    }
                }
            }
            catch (ThreadAbortException tae)
            {
                Serilog.Log.Warning(tae, tae.Message);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.MessageSendingQueueEvent.Close();
            }
        }

        /// </summary>
        /// クラス名のバリデーション
        /// </summary>
        private static bool ValidateClassName(string className)
        {
            // クラス名にハイフン以外の特殊文字、大文字、スペースを許可しない
            return !className.Any(char.IsUpper) &&
                   !className.Any(c => char.IsWhiteSpace(c) || (char.IsPunctuation(c) && c != '-'));
        }
    }
}