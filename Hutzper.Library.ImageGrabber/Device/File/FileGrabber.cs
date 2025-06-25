using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.ImageGrabber.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace Hutzper.Library.ImageGrabber.Device.File
{
    /// <summary>
    /// ファイル画像取得
    /// </summary>
    [Serializable]
    public class FileGrabber : GrabberBase, IFileDevice
    {
        #region IGrabber

        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public override bool Enabled => this.IsOpen && (this.CurrentDirectory?.Exists ?? false);

        /// <summary>
        /// トリガーモード
        /// </summary>
        public override TriggerMode TriggerMode
        {
            get => this.triggerMode;
            set
            {
                if (value != TriggerMode.ExternalTrigger)
                {
                    this.triggerMode = value;
                }
            }
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
        public override bool Close() => !(this.IsOpen = false);

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
                    this.ControlInfo?.Invalidate();
                    this.ControlInfo = new FileGrabber.AcquisitionControlInfo(1);
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
                    this.ControlInfo?.Invalidate();
                    this.ControlInfo = new FileGrabber.AcquisitionControlInfo(number);
                    this.AcquisitionThread.Enqueue(this.ControlInfo);

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
                    this.ControlInfo?.Invalidate();
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
            if (this.triggerMode == TriggerMode.SoftTrigger)
            {
                return this.ControlInfo?.DoSoftTrigger() ?? false;
            }

            return false;
        }

        /// <summary>
        /// AWB
        /// </summary>
        /// <returns></returns>
        public override bool DoAutoWhiteBalancing() => false;

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
            try
            {
                base.Initialize(serviceCollection);

                this.Files.Clear();
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

                if (parameter is FileGrabberParameter fp)
                {
                    this.Parameter = fp;

                    this.TriggerMode = fp.TriggerMode;
                    this.Width = fp.Width;
                    this.Height = fp.Height;
                    this.OffsetX = fp.OffsetX;
                    this.OffsetY = fp.OffsetY;

                    if (fp.ImageDirectoryInfo is not null)
                    {
                        this.CurrentDirectory = new DirectoryInfo(fp.ImageDirectoryInfo.FullName);
                    }

                    this.LastFileInfo = null;
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

                this.CurrentDirectory?.Refresh();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
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
                this.ControlInfo?.Invalidate();
                this.ControlInfo = null;
                this.DisposeSafely(this.AcquisitionThread);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
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
            public bool IsContinuable => this.isContinuable;

            // 画像取得を無効化
            public void Invalidate()
            {
                if (1 == Interlocked.Increment(ref this.invalidationCount))
                {
                    this.isContinuable = false;
                    if (this.WaitHandles[0] is ManualResetEvent e)
                    {
                        e.Set();
                    }
                }
            }

            #endregion

            #region フィールド

            protected WaitHandle[] WaitHandles = new WaitHandle[2];

            protected WaitHandle[] TriggerHandles = new WaitHandle[2];


            protected bool isContinuable = true;

            protected readonly List<IGrabberData> bufferImages = new();

            protected int invalidationCount;

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
                    foreach (var e in this.WaitHandles)
                    {
                        e.Close();
                    }

                    foreach (var e in this.TriggerHandles)
                    {
                        e.Close();
                    }

                    this.bufferImages.ForEach(b => b.Dispose());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion

            #region メソッド

            // 画像取得通知
            public void OnDataStreamNewBuffer(IGrabberData data)
            {
                try
                {
                    if (this.WaitHandles[1] is AutoResetEvent e)
                    {
                        lock (this.WaitHandles)
                        {
                            this.bufferImages.Add(data);
                        }

                        e.Set();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            // 画像取得待機
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
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

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
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                return false;
            }

            // ソフトトリガー入力待機
            public void WaitTrigger()
            {
                try
                {
                    WaitHandle.WaitAny(this.TriggerHandles);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion
        }

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileGrabber() : this(typeof(FileGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileGrabber(Common.Drawing.Point location) : this(typeof(FileGrabber).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public FileGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.GrabberType = GrabberType.AreaSensor;

            this.AcquisitionThread = new()
            {
                Priority = ThreadPriority.Normal
            };
            this.AcquisitionThread.Dequeue += this.AcquisitionThread_Dequeue;
        }

        #endregion

        #region protected フィールド

        protected bool IsOpen;

        /// <summary>
        /// /ファイルリスト
        /// </summary>
        protected readonly List<FileInfo> Files = new();

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

        protected FileInfo? LastFileInfo;   // 最後に取得したファイル情報

        #endregion

        #region protected メソッド

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        protected virtual void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
        {
            try
            {
                if (this.Parameter is not FileGrabberParameter fp)
                {
                    throw new InvalidOperationException($"{nameof(this.Parameter)} is not FileGrabberParameter.");
                }

                // カレントディレクトリ情報取得
                var currentDirectory = this.CurrentDirectory;

                // 拡張子リストを取得
                var validExtensions = new List<string>();
                foreach (var ext in fp.ValidExtensions)
                {
                    if (false == string.IsNullOrEmpty(ext.Trim()))
                    {
                        var formatedExt = (ext[0] == '.') ? ext.Trim() : $".{ext.Trim()}";
                        if (false == validExtensions.Contains(formatedExt))
                        {
                            validExtensions.Add(formatedExt);
                        }
                    }
                }

                // カレントディレクトリが存在する場合
                if (currentDirectory?.Exists ?? false)
                {
                    // 画像枚数を初期化
                    var imageCounter = 0;
                    var lastAcquisitionDateTime = DateTime.Now - new TimeSpan(0, 0, 10);

                    var fps = 15d;

                    var skipUntilLastReadFile = this.LastFileInfo is not null && fp.ResumeFromLastFile;

                    // 続行可能な場合は繰り返し処理
                    while (info.IsContinuable)
                    {
                        // ファイル情報を列挙
                        foreach (var fileInfo in currentDirectory.EnumerateFiles())
                        {
                            // 拡張子チェック
                            if (0 < validExtensions.Count && false == validExtensions.Contains(System.IO.Path.GetExtension(fileInfo.FullName)))
                            {
                                continue;
                            }

                            // 最後に読み込んだファイルの次から再開する
                            if (true == skipUntilLastReadFile)
                            {
                                if (this.LastFileInfo?.FullName == fileInfo.FullName)
                                {
                                    // 前回のファイルに一致するまでスキップ
                                    skipUntilLastReadFile = false;
                                }
                                continue;
                            }

                            // 画像取得終了
                            if (false == info.IsContinuable)
                            {
                                break;
                            }

                            // 画像ファイル読み込み
                            var ret = Task.Run(() =>
                            {
                                // 画像データを生成
                                var data = new BitmapGrabberData(this.location);

                                // 画像ファイル読み込み
                                try
                                {
                                    data.Read(fileInfo);
                                    //data.ImageのFormatがFormat32bppArgbの場合Format24bppRgbに転換
                                    if (data.Image is not null && data.Image.PixelFormat == PixelFormat.Format32bppArgb)
                                    {
                                        data.Image = data.Image.Clone(new Rectangle(0, 0, data.Image.Width, data.Image.Height), PixelFormat.Format24bppRgb);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Serilog.Log.Warning(ex, ex.Message);
                                }
                                finally
                                {
                                    // トリガ待機
                                    info.WaitTrigger();

                                    // 完了通知
                                    info.OnDataStreamNewBuffer((IGrabberData)data);
                                }
                            });

                            // ソフトトリガーの場合
                            if (this.triggerMode == TriggerMode.InternalTrigger)
                            {
                                // 設定FPSから待機時間を算出
                                if (0 < fps)
                                {
                                    var elapsedSinceLastAcquisition = DateTime.Now - lastAcquisitionDateTime;

                                    var waitTimeMs = Convert.ToInt32(1000 / fps - elapsedSinceLastAcquisition.TotalMilliseconds);

                                    // 待機が必要な場合
                                    if (0 < waitTimeMs)
                                    {
                                        // FPSに合わせて待機                                 
                                        Thread.Sleep(waitTimeMs);
                                    }
                                }

                                // トリガ発行
                                info.DoSoftTrigger();
                            }

                            // 画像取得完了待ち
                            var bufferImages = info.GetDataStreamNewBuffer();

                            // 画像取得終了
                            if (false == info.IsContinuable)
                            {
                                break;
                            }

                            // 最後に取得したファイル情報を保持
                            this.LastFileInfo = fileInfo;

                            // 画像取得間隔計測開始
                            lastAcquisitionDateTime = DateTime.Now;

                            // 画像バッファから有効データを列挙
                            foreach (var buffer in bufferImages.Where(b => b.Size.Height > 0))
                            {
                                // カウンタ値を設定
                                buffer.Counter = (ulong)++imageCounter;

                                // 画像取得通知
                                this.OnDataGrabbed(buffer);

                                //　必要画像枚数に達している、もしくは続行不可能な場合
                                if (
                                    (false == info.IsContinuable)
                                || (0 < info.NumberOfImage && imageCounter >= info.NumberOfImage)
                                )
                                {
                                    // 画像取得終了
                                    info.Invalidate();
                                    break;
                                }
                            }

                            // 画像取得終了
                            if (false == info.IsContinuable)
                            {
                                break;
                            }
                        }
                    }
                }

                info.Invalidate();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);

                this.OnErrorOccurred(new GrabberErrorBase(this.location));
            }
            finally
            {
                this.DisposeSafely(info);
            }
        }

        #endregion
    }
}