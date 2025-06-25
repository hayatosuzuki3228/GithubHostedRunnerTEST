using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.Insight.Controller;
using Hutzper.Library.InsightLinkage.Connection;
using Hutzper.Library.InsightLinkage.Data;


namespace Hutzper.Library.InsightLinkage.Controller
{
    /// <summary>
    /// IInsightLinkageControllerf実装
    /// </summary>
    [Serializable]
    public class InsightLinkageController : ControllerBase, IInsightLinkageController
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
                this.Connections.ForEach(g => g.Initialize(serviceCollection));
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="config"></param>
        public override void SetConfig(IApplicationConfig? config)
        {
            try
            {
                base.SetConfig(config);
                this.Connections.ForEach(d => d.SetConfig(config));
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
                if (parameter is IInsightLinkageControllerParameter p)
                {
                    this.Parameter = p;

                    foreach (var c in this.Connections)
                    {
                        var index = this.Connections.IndexOf(c);

                        if (p.ConnectionParameters.Count > index)
                        {
                            p.ConnectionParameters[index].Uuid = p.Uuid;
                            c.SetParameter(p.ConnectionParameters[index]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            try
            {
                base.Update();
                this.Connections.ForEach(d => d.Update());
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
            var isSuccess = true;

            try
            {
                this.Connections.ForEach(g => isSuccess &= g.Open());
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
        /// <returns></returns>
        public override bool Close()
        {
            var isSuccess = true;

            try
            {
                this.Connections.ForEach(g => isSuccess &= g.Close());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region IInsightLinkageController

        /// <summary>
        /// 管理するコネクションリスト
        /// </summary>
        public virtual List<IConnection> Connections { get; set; } = new();

        /// <summary>
        /// コネクション割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public virtual bool Attach(params IConnection[] connections)
        {
            var isSuccess = true;

            try
            {
                foreach (var c in connections)
                {
                    if (false == this.Connections.Contains(c))
                    {
                        if (c is not null)
                        {
                            this.Connections.Add(c);
                        }
                        else
                        {
                            throw new Exception("connection is null");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// Insightへの送信リクエスト
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual bool Send(params IInsightLingageRequest[] request)
        {
            var isSuccess = false;

            try
            {
                if (this.Parameter is IInsightLinkageControllerParameter p)
                {
                    if (true == p.IsUse && request is not null)
                    {
                        foreach (var r in request)
                        {
                            this.RequestProcessingThread.Enqueue(r);
                        }
                    }
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
        /// 処理結果イベント
        /// </summary>
        public event Action<object, IInsightLingageResult>? Processed;

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                foreach (var c in this.Connections)
                {
                    this.DisposeSafely(c);
                }
                this.Connections.Clear();

                this.DisposeSafely(this.RequestProcessingThread);
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
        /// 要求処理スレッド
        /// </summary>
        protected QueueThread<IInsightLingageRequest> RequestProcessingThread;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public InsightLinkageController() : this(typeof(InsightLinkageController).Name, -1)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public InsightLinkageController(string nickname, int index = -1) : base(nickname, index)
        {
            this.RequestProcessingThread = new QueueThread<IInsightLingageRequest>();
            this.RequestProcessingThread.Dequeue += this.RequestProcessingThread_Dequeue;
        }

        #endregion

        /// <summary>
        /// 要求スレッド処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        protected virtual void RequestProcessingThread_Dequeue(object sender, IInsightLingageRequest request)
        {
            var result = new InsightLingageResult(request);

            try
            {
                result.Request.RequestCounter++;

                if (this.Parameter is IInsightLinkageControllerParameter p)
                {
                    foreach (var c in this.Connections)
                    {
                        if (c is IFileUploader uploader && request.FileUploadRequest is not null)
                        {
                            if (request.InsightRequestType.HasValue)
                            {
                                IFileUploadRequest[] uploadRequests = new[] { request.FileUploadRequest };

                                var insightRequestType = request.InsightRequestType.Value; // null 許容型から通常の型に変換
                                result.Results.Add(c.LinkageType, uploader.UploadFile(uploadRequests, insightRequestType));
                            }
                            else
                            {
                            }
                        }

                        if (c is ITextMessenger textMessenger)
                        {
                            result.Results.Add(c.LinkageType, textMessenger.SendText(request.MessageText));
                        }
                    }
                }
                else
                {
                    throw new Exception("parameter is null");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            // 処理結果通知
            this.OnProcessed(result);
        }

        /// <summary>
        /// 処理結果イベント通知
        /// </summary>
        protected virtual void OnProcessed(IInsightLingageResult result)
        {
            try
            {
                this.Processed?.Invoke(this, result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }
    }
}