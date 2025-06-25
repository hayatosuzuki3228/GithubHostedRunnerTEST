using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageGrabber.Device;
using System.Diagnostics;

namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// 画像取得制御基底クラス
    /// </summary>
    [Serializable]
    public abstract class GrabberControllerBase : ControllerBase, IGrabberController
    {
        #region IGrabberController

        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public virtual bool Enabled { get; protected set; }

        /// <summary>
        /// 画像取得タイプ
        /// </summary>
        public GrabberType GrabberType { get; init; }

        /// <summary>
        /// 画像取得インスタンス数
        /// </summary>
        public virtual int NumberOfGrabber => this.Grabbers.Count;

        /// <summary>
        /// 画像取得インスタンスリスト
        /// </summary>
        public virtual List<IGrabber> Grabbers { get; init; } = new List<IGrabber>();

        /// <summary>
        /// エラーイベント
        /// </summary>
        public event Action<object, IGrabberError>? ErrorOccurred;

        /// <summary>
        /// データ取得イベント
        /// </summary>
        public event Action<object, IGrabberData>? DataGrabbed;

        /// <summary>
        /// グラバー無効
        /// </summary>
        public event Action<object, IGrabber>? GrabberDisabled;

        /// <summary>
        /// デバイス割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public virtual bool Attach(params IGrabber[] grabbers)
        {
            var isSuccess = true;

            try
            {
                foreach (var d in grabbers)
                {
                    if (false == this.Grabbers.Contains(d))
                    {
                        if (null != d)
                        {
                            d.ErrorOccurred += this.Grabber_ErrorOccurred;
                            d.DataGrabbed += this.Grabber_DataGrabbed;
                            d.Disabled += this.Grabber_Disabled;
                            this.Grabbers.Add(d);
                        }
                        else
                        {
                            throw new Exception("grabber is null");
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
        /// 画像取得
        /// </summary>
        /// <returns></returns>
        public virtual bool Grab()
        {
            var isSuccess = new int[this.Grabbers.Count];

            try
            {
                Parallel.For(0, this.Grabbers.Count, i =>
                {
                    isSuccess[i] = Convert.ToInt32(this.Grabbers[i].Grab());
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess.Sum() >= this.Grabbers.Count;
        }

        /// <summary>
        /// 連続画像取得
        /// </summary>
        /// <returns></returns>
        public virtual bool GrabContinuously(int number = -1)
        {
            var isSuccess = new int[this.Grabbers.Count];

            try
            {
                Parallel.For(0, this.Grabbers.Count, i =>
                {
                    isSuccess[i] = Convert.ToInt32(this.Grabbers[i].GrabContinuously(number));
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess.Sum() >= this.Grabbers.Count;
        }

        /// <summary>
        /// 連続撮影停止
        /// </summary>
        /// <returns></returns>
        public virtual bool StopGrabbing()
        {
            var isSuccess = new int[this.Grabbers.Count];

            try
            {
                Parallel.For(0, this.Grabbers.Count, i =>
                {
                    isSuccess[i] = Convert.ToInt32(this.Grabbers[i].StopGrabbing());
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess.Sum() >= this.Grabbers.Count;
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
                this.Grabbers.ForEach(g => g.Initialize(serviceCollection));
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

                this.Grabbers.ForEach(g => g.SetConfig(config));
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

                if (parameter is IGrabberControllerParameter gp)
                {
                    foreach (var grabber in this.Grabbers)
                    {
                        var index = this.Grabbers.IndexOf(grabber);

                        if (gp.GrabberParameters.Count > index)
                        {
                            grabber.SetParameter(gp.GrabberParameters[index]);
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
                this.Grabbers.ForEach(g => g.Update());
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
                this.Grabbers.ForEach(g => isSuccess &= g.Open());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Enabled = isSuccess;
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
                this.Grabbers.ForEach(g => isSuccess &= g.Close());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Enabled = false;
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
                foreach (var grabber in this.Grabbers)
                {
                    grabber.ErrorOccurred -= this.Grabber_ErrorOccurred;
                    grabber.DataGrabbed -= this.Grabber_DataGrabbed;
                    grabber.Disabled -= this.Grabber_Disabled;

                    this.DisposeSafely(grabber);
                }
                this.Grabbers.Clear();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public GrabberControllerBase(string nickname, int index = -1) : base(nickname, index)
        {
        }

        #endregion

        #region イベント通知

        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="grabberError"></param>
        protected virtual void OnErrorOccurred(IGrabberError grabberError)
        {
            try
            {
                this.ErrorOccurred?.Invoke(this, grabberError);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="grabberData"></param>
        protected virtual void OnDataGrabbed(IGrabberData grabberData)
        {
            try
            {
                this.DataGrabbed?.Invoke(this, grabberData);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        protected virtual void OnGrabberDisabled(IGrabber g)
        {
            try
            {
                this.GrabberDisabled?.Invoke(this, g);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IGrabberイベント

        protected virtual void Grabber_DataGrabbed(object sender, IGrabberData data)
        {
            try
            {
                this.OnDataGrabbed(data);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        protected virtual void Grabber_ErrorOccurred(object sender, IGrabberError error)
        {
            try
            {
                this.OnErrorOccurred(error);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        protected virtual void Grabber_Disabled(object sender)
        {
            try
            {
                this.OnGrabberDisabled((IGrabber)sender);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}