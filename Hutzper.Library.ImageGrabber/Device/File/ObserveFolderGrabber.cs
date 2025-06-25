using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.ImageGrabber.Data;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Hutzper.Library.ImageGrabber.Device.File
{

    /// ファイル画像取得
    [Serializable]
    public class ObserveFolderGrabber : GrabberBase, IAreaSensor
    {
        #region IGrabber
        public DirectoryInfo? TargetInfo;
        public int ConsumeDurationMs;
        public override bool Enabled => this.IsOpen && (this.TargetInfo?.Exists ?? false);
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
        public double FramesPerSecond { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private FileSystemWatcher? Watcher;
        private ulong Counter = 0;
        private ConcurrentQueue<string> FilePathQueue = new();
        private Thread? ConsumerThread;
        private bool isExitThread = false;
        private AutoResetEvent AutoResetEvent = new(false);

        public override bool Open()
        {
            Serilog.Log.Information($"ObserveFolderGrabber.Open {this.TargetInfo?.FullName}");
            this.IsOpen = true;
            this.triggerMode = TriggerMode.InternalTrigger;

            this.Watcher = new FileSystemWatcher();
            this.Watcher.Path = TargetInfo?.FullName ?? "";
            this.Watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            this.Watcher.Filter = "*.*";
            this.Watcher.Created += this.OnCreated;
            this.Watcher.EnableRaisingEvents = true;

            this.ConsumerThread = new Thread(this.ConsumeLoop);
            this.ConsumerThread.Name = "ObserveFolderGrabber.ConsumerThread";
            this.ConsumerThread.IsBackground = true;
            this.ConsumerThread.Start();
            return this.Enabled;
        }
        private void ConsumeLoop()
        {
            Thread.Sleep(1000);
            while (isExitThread == false)
            {
                bool result = this.FilePathQueue.TryDequeue(out string? path);
                if (result && path != null)
                {
                    Thread.Sleep(this.ConsumeDurationMs);  // ファイルが生成された瞬間に読み込みをすると問題がでるので sleep
                    BitmapGrabberData buffer = new BitmapGrabberData();
                    FileInfo targetFile = new(path);
                    while (true)
                    {
                        try
                        {
                            using FileStream _ = System.IO.File.Open(path, FileMode.Open);
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(100);
                        }
                    }
                    if (buffer.Read(targetFile))
                    {
                        buffer.Counter = this.Counter++;
                        this.OnDataGrabbed(buffer);
                    }
                    continue;
                }
                this.AutoResetEvent.WaitOne();
            }
        }
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Serilog.Log.Information($"ObserveFolderGrabber.OnCreated {this.TargetInfo?.FullName ?? "Null"}");

            Console.WriteLine($"[create] {e.ChangeType}: {e.FullPath}");
            Regex AllowedExtensionsRegex = new Regex(@"\.(jpg|png|bmp)$", RegexOptions.IgnoreCase);
            if (!AllowedExtensionsRegex.IsMatch(e.FullPath))
            {
                Serilog.Log.Error($"監視対象以外のファイルが生成されています。 {e.FullPath}");
                return;
            }
            this.FilePathQueue.Enqueue(e.FullPath);
            this.AutoResetEvent.Set();
        }
        public override bool Close()
        {
            this.Watcher?.Dispose();
            this.Watcher = null;
            this.isExitThread = true;
            this.AutoResetEvent.Set();
            return true;
        }

        public override bool Grab()
        {
            return true;
        }

        public override bool GrabContinuously(int number = -1)
        {
            return true;
        }

        public override bool StopGrabbing()
        {
            return true;
        }

        public override bool DoSoftTrigger()
        {
            throw new NotImplementedException();
        }

        public override bool DoAutoWhiteBalancing()
        {
            throw new NotImplementedException();
        }

        public override bool LoadUserSetDefault() => false;
        public override bool LoadUserSet(int index = 0) => false;
        public override bool SaveUserSet(int index = 0) => false;

        #endregion

        #region IController

        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
        }

        public override void SetConfig(IApplicationConfig? config)
        {
        }

        public override void SetParameter(IControllerParameter? parameter)
        {
            base.SetParameter(parameter);

            if (parameter is ObserveFolderGrabberParameter p)
            {
                this.TargetInfo = p.ImageDirectoryInfo;
                this.ConsumeDurationMs = p.ConsumeDurationMs;
            }
        }

        /// 更新
        public override void Update()
        {
            try
            {
                base.Update();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        /// リソースの解放
        protected override void DisposeExplicit()
        {
        }

        #region コンストラクタ

        /// コンストラクタ

        public ObserveFolderGrabber() : this(typeof(ObserveFolderGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// コンストラクタ
        public ObserveFolderGrabber(Common.Drawing.Point location) : this(typeof(ObserveFolderGrabber).Name, location)
        {

        }

        /// コンストラクタ
        public ObserveFolderGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.GrabberType = GrabberType.AreaSensor;
        }

        #endregion

        protected bool IsOpen;

        protected readonly string TargetFolder = String.Empty;
        protected TriggerMode triggerMode = TriggerMode.InternalTrigger;

    }
}