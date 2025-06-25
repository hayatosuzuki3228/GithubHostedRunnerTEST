using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.ImageGrabber.Data;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace Hutzper.Library.ImageGrabber.Device.Web
{
    /// <summary>
    /// ファイル画像取得
    /// </summary>
    [Serializable]
    public class WebGrabber : GrabberBase, IWebDevice
    {
        #region IGrabber

        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public override bool Enabled => this.IsOpen;

        /// <summary>
        /// トリガーモード
        /// </summary>
        public override TriggerMode TriggerMode
        {
            get => this.triggerMode;
            set { if (value != TriggerMode.ExternalTrigger) this.triggerMode = value; }
        }

        /// <summary>
        /// 画像幅
        /// </summary>
        public override int Width { get => this.width; set => this.width = (value / 16) * 16; }
        protected int width;

        /// <summary>
        /// 画像高さ
        /// </summary>
        public override int Height { get => this.height; set => this.height = (value / 4) * 4; }
        protected int height;

        /// <summary>
        /// Xオフセット
        /// </summary>
        public override int OffsetX { get => this.offsetX; set => this.offsetX = (value / 4) * 4; }
        protected int offsetX;

        /// <summary>
        /// Yオフセット
        /// </summary>
        public override int OffsetY { get => this.offsetY; set => this.offsetY = (value / 4) * 4; }
        protected int offsetY;
        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            this.IsOpen = true;
            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            this.StopGrabbing();
            return !(this.IsOpen = false);
        }

        /// <summary>
        /// 画像取得
        /// </summary>
        /// <returns></returns>
        public override bool Grab()
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo) this.ControlInfo.IsContinuable = false;
                    this.ControlInfo = new WebGrabber.AcquisitionControlInfo(1);
                    this.AcquisitionThread.Enqueue(this.ControlInfo);
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
        /// 連続画像取得
        /// </summary>
        /// <param name="isBegin"></param>
        /// <returns></returns>
        public override bool GrabContinuously(int number = -1)
        {
            var isSuccess = this.Enabled;
            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo)
                    {
                        this.ControlInfo.IsContinuable = false;
                        this.ControlInfo = null;
                    }
                    this.ControlInfo = new WebGrabber.AcquisitionControlInfo(number);
                    this.AcquisitionThread.Enqueue(this.ControlInfo);
                    isSuccess = true;
                }
            }
            catch (Exception ex) { Serilog.Log.Warning(ex, ex.Message); }
            return isSuccess;
        }

        /// <summary>
        /// 連続撮影停止
        /// </summary>
        /// <returns></returns>
        public override bool StopGrabbing()
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo)
                    {
                        this.ControlInfo.IsContinuable = false;
                        this.ControlInfo = null;
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
        /// ソフトトリガー
        /// </summary>
        /// <returns></returns>
        public override bool DoSoftTrigger()
        {
            if (this.triggerMode == TriggerMode.SoftTrigger) return this.ControlInfo?.DoSoftTrigger() ?? false;
            return false;
        }

        /// <summary>
        /// AWB
        /// </summary>
        /// <returns></returns>
        public override bool DoAutoWhiteBalancing()
        {
            return false;
        }

        /// <summary>
        /// ユーザーセットデフォルトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSetDefault() => false;

        /// <summary>
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSet(int index = 0) => false;

        /// <summary>
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public override bool SaveUserSet(int index = 0) => false;

        #endregion

        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            try { base.Initialize(serviceCollection); }
            catch (Exception ex) { Serilog.Log.Warning(ex, ex.Message); }
        }

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="config"></param>
        public override void SetConfig(IApplicationConfig? config)
        {
            try { base.SetConfig(config); }
            catch (Exception ex) { Serilog.Log.Warning(ex, ex.Message); }
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
                if (parameter is WebGrabberParameter webparameter)
                {
                    this.Parameter = webparameter;
                    this.TriggerMode = webparameter.TriggerMode;
                    this.Width = webparameter.Width;
                    this.Height = webparameter.Height;
                    this.OffsetX = webparameter.OffsetX;
                    this.OffsetY = webparameter.OffsetY;
                }
            }
            catch (Exception ex) { Serilog.Log.Warning(ex, ex.Message); }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            try { base.Update(); }
            catch (Exception ex) { Serilog.Log.Warning(ex, ex.Message); }
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
                if (null != this.ControlInfo) this.ControlInfo.IsContinuable = false;
                this.DisposeSafely(this.AcquisitionThread);
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        #endregion

        #region IFileDevice

        /// <summary>
        /// 現在のディレクトリ情報
        /// </summary>
        public DirectoryInfo? CurrentDirectory { get; protected set; }

        #endregion

        /// <summary>
        /// 画像取得制御情報
        /// </summary>
        protected class AcquisitionControlInfo : SafelyDisposable
        {
            #region プロパティ

            // 画像枚数
            public int NumberOfImage { get; init; }

            // 画像取得を続行可能かどうか
            public bool IsContinuable
            {
                get => this.isContinuable;
                set
                {
                    this.isContinuable = value;
                    if (false == this.isContinuable) { if (this.WaitHandles[0] is ManualResetEvent e) e.Set(); }
                }
            }

            #endregion

            #region フィールド

            protected WaitHandle[] WaitHandles = new WaitHandle[2];

            protected WaitHandle[] TriggerHandles = new WaitHandle[2];


            protected bool isContinuable = true;

            protected readonly List<IGrabberData> bufferImages = new();

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="numberOfImage"></param>
            public AcquisitionControlInfo(int numberOfImage)
            {
                this.NumberOfImage = numberOfImage;

                this.WaitHandles[0] = new ManualResetEvent(false);
                this.WaitHandles[1] = new AutoResetEvent(false);

                this.TriggerHandles[0] = this.WaitHandles[0];
                this.TriggerHandles[1] = new AutoResetEvent(false);
            }

            #endregion

            #region SafelyDisposable

            // リソース破棄
            protected override void DisposeExplicit()
            {
                try
                {
                    foreach (var e in this.WaitHandles) e.Close();
                    foreach (var e in this.TriggerHandles) e.Close();
                    this.bufferImages.ForEach(b => b.Dispose());
                }
                catch (Exception ex) { Debug.WriteLine(ex); }
            }

            #endregion

            #region メソッド

            public void OnDataStreamNewBuffer(IGrabberData data)
            {
                try
                {
                    if (this.WaitHandles[1] is AutoResetEvent e)
                    {
                        lock (this.WaitHandles) this.bufferImages.Add(data);
                        e.Set();
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex); }
            }

            public List<IGrabberData> GetDataStreamNewBuffer()
            {
                var images = new List<IGrabberData>();
                try
                {
                    WaitHandle.WaitAny(this.WaitHandles);

                    lock (this.WaitHandles)
                    {
                        if (0 < this.bufferImages.Count)
                        {
                            images.AddRange(this.bufferImages);
                            this.bufferImages.Clear();
                        }
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex); }
                return images;
            }

            // ソフトトリガー発行
            public bool DoSoftTrigger()
            {
                try
                {
                    if (this.TriggerHandles[1] is AutoResetEvent e)
                    {
                        e.Set();
                        return true;
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex); }
                return false;
            }

            // ソフトトリガー入力待機
            public void WaitTrigger()
            {
                try { WaitHandle.WaitAny(this.TriggerHandles); }
                catch (Exception ex) { Debug.WriteLine(ex); }
            }
            #endregion
        }

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebGrabber() : this(typeof(WebGrabber).Name, Common.Drawing.Point.New()) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebGrabber(Common.Drawing.Point location) : this(typeof(WebGrabber).Name, location) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public WebGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.GrabberType = GrabberType.AreaSensor;
            this.AcquisitionThread = new() { Priority = ThreadPriority.Normal };
            this.AcquisitionThread.Dequeue += this.AcquisitionThread_Dequeue;
        }

        #endregion

        #region protected フィールド

        protected bool IsOpen;

        /// <summary>
        /// トリガーモード
        /// </summary>
        protected TriggerMode triggerMode = TriggerMode.InternalTrigger;

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        protected QueueThread<AcquisitionControlInfo> AcquisitionThread;

        /// <summary>
        /// 画像取得制御情報
        /// </summary>
        protected AcquisitionControlInfo? ControlInfo;

        #endregion

        #region protected メソッド

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        protected virtual void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
        {
            using VideoCapture? capture = new(0);
            try
            {
                var imageCounter = 0;
                var lastAcquisitionDateTime = DateTime.Now - new TimeSpan(0, 0, 10);

                var fps = 15d;
                var data = new BitmapGrabberData(this.location);

                while (info.IsContinuable)
                {
                    if (false == info.IsContinuable) break;

                    Task GrabberTask = Task.Run(() =>
                    {
                        try
                        {
                            info.WaitTrigger();
                            Mat image = new();
                            capture.Read(image);
                            Bitmap orgimg = image.ToBitmap();
                            Bitmap newBitmap = new(orgimg.Width, orgimg.Height, PixelFormat.Format24bppRgb);
                            using Graphics g = Graphics.FromImage(newBitmap);
                            g.DrawImage(orgimg, new Rectangle(0, 0, newBitmap.Width, newBitmap.Height));
                            data.Image = newBitmap;
                        }
                        catch (Exception ex) { Serilog.Log.Warning(ex, ex.Message); }
                        finally { this.ControlInfo?.OnDataStreamNewBuffer(data); }
                    });

                    if (this.triggerMode == TriggerMode.InternalTrigger)
                    {
                        if (0 < fps)
                        {
                            var elapsedSinceLastAcquisition = DateTime.Now - lastAcquisitionDateTime;
                            var waitTimeMs = Convert.ToInt32(1000 / fps - elapsedSinceLastAcquisition.TotalMilliseconds);
                            if (0 < waitTimeMs) Thread.Sleep(waitTimeMs);
                        }
                        info.DoSoftTrigger();
                    }

                    var bufferImages = info.GetDataStreamNewBuffer();

                    lastAcquisitionDateTime = DateTime.Now;

                    foreach (var buffer in bufferImages.Where(b => b.Size.Height > 0))
                    {
                        this.OnDataGrabbed(buffer);
                        if ((false == info.IsContinuable) || (0 < info.NumberOfImage && ++imageCounter >= info.NumberOfImage))
                        {
                            info.IsContinuable = false;
                            break;
                        }
                    }
                    if (false == info.IsContinuable) break;
                }
                info.IsContinuable = false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                this.OnErrorOccurred(new GrabberErrorBase(this.location));
            }
            finally
            {
                try
                {
                    var usedInfo = this.ControlInfo;
                    this.ControlInfo = null;
                    usedInfo?.Dispose();
                }
                catch (Exception ex) { Serilog.Log.Warning(ex, ex.Message); }
            }
        }
        #endregion
    }
}