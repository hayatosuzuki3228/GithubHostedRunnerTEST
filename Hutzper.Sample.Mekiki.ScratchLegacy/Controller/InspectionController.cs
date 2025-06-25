using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.DigitalIO;
using Hutzper.Library.DigitalIO.Device;
using Hutzper.Library.ImageGrabber;
using Hutzper.Library.ImageGrabber.Device;
using Hutzper.Library.ImageProcessing;
using Hutzper.Library.Insight.Controller;
using Hutzper.Library.InsightLinkage;
using Hutzper.Library.InsightLinkage.Data;
using Hutzper.Library.LightController;
using Hutzper.Library.Onnx;
using Hutzper.Library.Onnx.Data;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data;
using Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Controller
{
    /// <summary>
    /// 検査制御
    /// </summary>
    public class InspectionController : ControllerBase, IInspectionController
    {
        #region サブクラス

        /// <summary>
        /// 検査制御情報
        /// </summary>
        protected class InspectionControllInfo
        {
            #region プロパティ

            /// <summary>
            /// 全てのIGrabber
            /// </summary>
            /// <remarks>AreaSensor → LineSensorの順番で格納されます</remarks>
            public List<IGrabber> AllGrabbers { get; init; } = new();

            /// <summary>
            /// 撮影遅延時間別にグループ分けしたIGrabber
            /// </summary>
            public List<List<IGrabber>> AcqTimingGroupsGrabber { get; init; } = new();

            /// <summary>
            /// 撮影遅延時間別にグループ分けしたTimeSpan
            /// </summary>
            /// <remarks>AcqTimingGroupsGrabberと一致</remarks>
            public List<TimeSpan> AcqTimingGroupsTimeSpan { get; init; } = new();

            /// <summary>
            /// タスクカウンタ
            /// </summary>
            /// <remarks>1検査毎にインクリメント</remarks>
            public CircularCounter TaskCounter { get; set; } = new CircularCounter(0, 99999);

            /// <summary>
            /// 推論実行リソース
            /// </summary>
            /// <remarks>推論を並列処理させる場合の同時実行許可数</remarks>
            public SemaphoreSlim InferenceExecutionResource { get; init; }

            /// <summary>
            /// タスク項目カウンタ
            /// </summary>
            /// <remarks>1検査毎にインクリメント</remarks>
            public CircularCounter[] TaskItemCounters { get; set; } = Array.Empty<CircularCounter>();

            /// <summary>
            /// 検査タスク管理同期オブジェクト
            /// </summary>
            public object InspectionTaskSync = new();

            /// <summary>
            /// 検査タスクリスト
            /// </summary>
            public List<IInspectionTask> InspectionTaskList = new();

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="maxDegreeOfInferenceParallelism"></param>
            public InspectionControllInfo(int maxDegreeOfInferenceParallelism)
            {
                this.InferenceExecutionResource = new SemaphoreSlim(maxDegreeOfInferenceParallelism, maxDegreeOfInferenceParallelism);
            }

            #endregion

            #region メソッド

            /// <summary>
            /// 初期化
            /// </summary>
            public void Initialize()
            {
                // タスクカウンタを初期化する
                this.TaskCounter = new CircularCounter(this.TaskCounter.Minimum, this.TaskCounter.Maximum);

                // タスク項目カウンタを初期化する
                this.TaskItemCounters = new CircularCounter[this.AllGrabbers.Count];
                foreach (var i in Enumerable.Range(0, this.TaskItemCounters.Length))
                {
                    this.TaskItemCounters[i] = new CircularCounter(this.TaskCounter.Minimum, this.TaskCounter.Maximum);
                }

                // 撮影遅延時間別にグループ
                this.AcqTimingGroupsGrabber.Clear();
                this.AcqTimingGroupsTimeSpan.Clear();
            }

            /// <summary>
            /// カメラの一次元インデックスを取得する
            /// </summary>
            /// <param name="location"></param>
            /// <returns></returns>
            public int GetSerializedGrabberIndex(Library.Common.Drawing.Point location)
            {
                var serializedIndex = 0;
                foreach (var grabber in this.AllGrabbers)
                {
                    if (true == location.Equals(grabber.Location))
                    {
                        break;
                    }
                    serializedIndex++;
                }

                return (this.AllGrabbers.Count > serializedIndex) ? serializedIndex : -1;
            }

            /// <summary>
            /// 全てのタスクが完了しているかどうか
            /// </summary>
            public bool IsAllTaskEnd
            {
                get
                {
                    var tempList = new List<IInspectionTask>();

                    lock (this.InspectionTaskSync)
                    {
                        tempList.AddRange(this.InspectionTaskList);
                    }

                    var allEnded = true;
                    foreach (var t in tempList)
                    {
                        allEnded &= t.IsTaskEnd;
                    }

                    return allEnded;
                }
            }

            /// <summary>
            /// 新しい検査タスクへの参照を取得する
            /// </summary>
            /// <param name="origin"></param>
            /// <returns></returns>
            public IInspectionTask? GetNewTask(DateTime origin, Stopwatch stopwatchFromOrigin)
            {
                // 新しい検査タスクの初期化
                var newTask = (IInspectionTask?)null;

                try
                {
                    lock (this.InspectionTaskSync)
                    {
                        var canAddTask = true;
                        if (0 < this.InspectionTaskList.Count)
                        {
                            var previousTask = this.InspectionTaskList.Last();
                            canAddTask = previousTask.IsIoFalled | previousTask.IsGrabEnd;
                        }

                        if (true == canAddTask)
                        {
                            // タスクを生成する
                            newTask = new InspectionTask();
                            newTask.Initialize(this.TaskCounter.PostIncrement(), origin, stopwatchFromOrigin, this.AllGrabbers.Count);

                            // タスクを追加する
                            this.InspectionTaskList.Add(newTask);
                        }
                    }
                }
                catch
                {
                }

                return newTask;
            }

            /// <summary>
            /// タスクを完了としてリストから取り除く
            /// </summary>
            /// <param name="selectedTask"></param>
            public void EndAndRemoveTask(IInspectionTask selectedTask)
            {
                try
                {
                    // タスクを完了とする
                    selectedTask.IsTaskEnd = true;

                    // タスクリストから取り除く
                    lock (this.InspectionTaskSync)
                    {
                        if (true == this.InspectionTaskList.Contains(selectedTask))
                        {
                            this.InspectionTaskList.Remove(selectedTask);
                        }
                    }
                }
                catch
                {
                }
            }

            /// <summary>
            /// 全検査タスク無効化
            /// </summary>
            public void InvalidateAllTask()
            {
                try
                {
                    lock (this.InspectionTaskSync)
                    {
                        // タスク無効化
                        this.InspectionTaskList.ForEach(task => task.Invalidate());
                    }
                }
                catch
                {
                }
            }

            /// <summary>
            /// 全検査タスク破棄
            /// </summary>
            public void DisposeAllTask()
            {
                try
                {
                    lock (this.InspectionTaskSync)
                    {
                        // 検査タスク破棄
                        this.InspectionTaskList.ForEach(task => task.Dispose());
                        this.InspectionTaskList.Clear();
                    }
                }
                catch
                {
                }
            }

            /// <summary>
            /// 物有り信号OFFが未完了(ON中)のタスクを取得する
            /// </summary>
            /// <param name="taskNumber"></param>
            /// <returns></returns>
            public IInspectionTask? GetTaskOfIoRising()
            {
                var selectedTask = (IInspectionTask?)null;

                try
                {
                    lock (this.InspectionTaskSync)
                    {
                        foreach (var task in this.InspectionTaskList)
                        {
                            if (false == task.IsIoFalled)
                            {
                                selectedTask = task;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                }

                return selectedTask;
            }

            /// <summary>
            /// 撮影が未完了のタスクを取得する
            /// </summary>
            /// <param name="taskNumber"></param>
            /// <returns></returns>
            public IInspectionTask? GetTaskOfGrabNotEnd(int index)
            {
                var selectedTask = (IInspectionTask?)null;

                try
                {
                    lock (this.InspectionTaskSync)
                    {
                        foreach (var task in this.InspectionTaskList)
                        {
                            if (false == task.IsGrabEnd && false == task.FlagsOfGrabEnd[index])
                            {
                                selectedTask = task;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                }

                return selectedTask;
            }

            #endregion
        }

        #endregion

        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            try
            {
                base.Initialize(serviceCollection);

                // 画像取得制御リスト
                _grabberControllers = new();

                // Insight用ヘルパ
                this.MekikiHelper.Initialize(this.Services);

                #region IControllerの初期化

                // エリアセンサ画像取得制御
                _areaSensorController = this.Services?.ServiceProvider?.GetRequiredService<IAreaSensorController>();
                if (_areaSensorController is not null)
                {
                    if (0 < _areaSensorController.NumberOfGrabber)
                    {
                        _grabberControllers.Add(_areaSensorController);
                    }
                }
                else
                {
                    Serilog.Log.Error($"{this}, IAreaSensorController null");
                }

                // ラインセンサ画像取得制御
                _lineSensorController = this.Services?.ServiceProvider?.GetRequiredService<ILineSensorController>();
                if (_lineSensorController is not null)
                {
                    if (0 < _lineSensorController.NumberOfGrabber)
                    {
                        _grabberControllers.Add(_lineSensorController);
                    }
                }
                else
                {
                    Serilog.Log.Error($"{this}, ILineSensorController null");
                }

                // DIO制御
                _dioController = this.Services?.ServiceProvider?.GetRequiredService<IDigitalIODeviceController>();
                if (_dioController is not null)
                {
                    _controller.Add(_dioController);
                }
                else
                {
                    Serilog.Log.Error($"{this}, IDigitalIODeviceController null");
                }

                // 照明制御
                _lightController = this.Services?.ServiceProvider?.GetRequiredService<ILightControllerSupervisor>();
                if (_lightController is not null)
                {
                    _controller.Add(_lightController);
                }
                else
                {
                    Serilog.Log.Error($"{this}, ILightControllerSupervisor null");
                }

                // Insight
                _insightLinkageController = this.Services?.ServiceProvider?.GetRequiredService<IInsightLinkageController>();
                if (_insightLinkageController is not null)
                {
                    _controller.Add(_insightLinkageController);
                }
                else
                {
                    Serilog.Log.Error($"{this}, IInsightLinkageController null");
                }

                // ONNX
                _onnxModelController = this.Services?.ServiceProvider?.GetRequiredService<IOnnxModelController>();
                if (_onnxModelController is not null)
                {
                    _controller.Add(_onnxModelController);
                }
                else
                {
                    Serilog.Log.Error($"{this}, IOnnxModelController null");
                }

                // コントローラーの初期化
                foreach (var c in _controller)
                {
                    c.Initialize(this.Services);
                }

                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// Config設定
        /// </summary>
        public override void SetConfig(IApplicationConfig? config)
        {
            try
            {
                base.SetConfig(config);

                if (this.Config is ProjectConfiguration prjConfig)
                {
                    // 各制御へConfig設定
                    _controller.ForEach(c => c.SetConfig(config));
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// パラメータ設定
        /// </summary>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is IInspectionControllerParameter ip)
                {
                    this.Parameter = ip;

                    // 検査制御情報
                    _controllInfo = new InspectionControllInfo(System.Math.Max(1, ip.MaxDegreeOfInferenceParallelism));

                    // 画像取得制御が存在する場合
                    if (0 < _grabberControllers.Count)
                    {
                        // 制御リストに追加する
                        _controller.AddRange(_grabberControllers);

                        // 全カメラを1次元リストに登録する
                        _controllInfo.AllGrabbers.Clear();
                        foreach (var g in _grabberControllers)
                        {
                            _controllInfo.AllGrabbers.AddRange(g.Grabbers);
                        }
                    }
                }

                if (this.Config is ProjectConfiguration prjConfig)
                {
                    // 撮影制御
                    foreach (var grabberController in _grabberControllers)
                    {
                        if (GrabberType.AreaSensor == grabberController.GrabberType)
                        {
                            grabberController.SetParameter(prjConfig.AreaSensorControllerParameter);
                        }
                        else if (GrabberType.LineSensor == grabberController.GrabberType)
                        {
                            grabberController.SetParameter(prjConfig.LineSensorControllerParameter);
                        }
                    }

                    // DIO制御
                    _dioController?.SetParameter(prjConfig.DigitalIODeviceControllerParameter);

                    // 照明制御
                    _lightController?.SetParameter(prjConfig.LightControllerParameter);

                    // Insight
                    _insightLinkageController?.SetParameter(prjConfig.InsightLinkageControllerParameter);

                    // ONNX
                    if (_onnxModelController is not null && prjConfig.OnnxModelControllerParameter is not null)
                    {
                        _onnxModelController.SetParameter(prjConfig.OnnxModelControllerParameter);

                        _onnxInputData = new IOnnxDataInput[_controllInfo.AllGrabbers.Count];
                        foreach (var i in Enumerable.Range(0, _onnxInputData.Length))
                        {
                            _onnxInputData[i] = this.Services?.ServiceProvider?.GetRequiredService<IOnnxDataInput>();
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
        /// オープン
        /// </summary>
        /// <remarks>検査を有効にする</remarks>
        public override bool Open()
        {
            var isSuccess = false;

            using var waitEvent = new System.Threading.ManualResetEvent(false);

            Task.Run(async () =>
            {
                try
                {
                    isSuccess = await this.ActivateInspectionAsync();
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    waitEvent.Set();
                }
            });

            waitEvent.WaitOne();

            return isSuccess;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <remarks>検査を無効にする</remarks>
        public override bool Close()
        {
            var isSuccess = false;

            using var waitEvent = new System.Threading.ManualResetEvent(false);

            Task.Run(async () =>
            {
                try
                {
                    await this.DeactivateInspectionAsync();
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    waitEvent.Set();
                }
            });

            waitEvent.WaitOne();

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
                _inspectionTaskQueueThread.Dequeue -= this.InspectionTaskQueueThread_Dequeue;
                _saveImageAndInsightTaskQueueThread.Dequeue -= this.SaveImageAndInsightTaskQueueThread_Dequeue;

                this.DisposeSafely(_inspectionTaskQueueThread);
                this.DisposeSafely(_saveImageAndInsightTaskQueueThread);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region IInspectionController

        /// <summary>
        /// 検査が有効かどうか
        /// </summary>
        public bool Enabled => _inspectionActivationCounter >= 2;

        /// <summary>
        /// 検査ステータス変化
        /// </summary>
        public event InspectionEventHandler? InspectionStatusChanged;

        /// <summary>
        /// 機器状態変化イベント
        /// </summary>
        public event DeviceStatusChangedEventHandler? DeviceStatusChanged;

        /// <summary>
        /// デジタルIO出力デバイス
        /// </summary>
        public IDigitalIOOutputDevice? DigitalIOOutputDevice => _dioController?.OutputDevices.FirstOrDefault();

        /// <summary>
        /// 検査ステータス変化イベント通知
        /// </summary>
        /// <param name="result"></param>
        public virtual void OnInspectionStatusChanged(InspectionEvent @event, IInspectionTask? currentTask, IInspectionTaskItem? currentTaskItem = null)
        {
            try
            {
                this.InspectionStatusChanged?.Invoke(this, @event, currentTask, currentTaskItem);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 機器状態変化イベント通知
        /// </summary>
        /// <param name="device"></param>
        /// <param name="kind"></param>
        /// <param name="status"></param>
        public virtual void OnDeviceStatusChanged(IController device, DeviceKind kind, int index, DeviceStatusKind status)
        {
            try
            {
                this.DeviceStatusChanged?.Invoke(this, device, kind, index, status);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// 制御パラメータ
        /// </summary>
        protected IInspectionControllerParameter? Parameter;

        /// <summary>
        /// 制御リスト
        /// </summary>
        private readonly List<IController> _controller = new();

        /// <summary>
        /// エリアセンサ画像取得制御
        /// </summary>
        private IAreaSensorController? _areaSensorController;

        /// <summary>
        /// ラインセンサ画像取得制御
        /// </summary>
        private ILineSensorController? _lineSensorController;

        /// <summary>
        /// 画像取得制御
        /// </summary>
        private List<IGrabberController> _grabberControllers = new();

        /// <summary>
        /// DIO制御
        /// </summary>
        private IDigitalIODeviceController? _dioController;

        /// <summary>
        /// 照明制御
        /// </summary>
        private ILightControllerSupervisor? _lightController;

        /// <summary>
        /// Insight連携
        /// </summary>
        private IInsightLinkageController? _insightLinkageController;

        /// <summary>
        /// ONNX制御
        /// </summary>
        private IOnnxModelController? _onnxModelController;

        /// <summary>
        /// 検査有効化フラグ
        /// </summary>
        private int _inspectionActivationCounter;

        /// <summary>
        /// 検査制御情報
        /// </summary>
        private InspectionControllInfo _controllInfo;

        /// <summary>
        /// ONNX用入力データ
        /// </summary>
        private IOnnxDataInput?[] _onnxInputData = Array.Empty<IOnnxDataInput>();

        /// <summary>
        /// 検査スレッド
        /// </summary>
        private readonly QueueThread<IInspectionTask> _inspectionTaskQueueThread;

        /// <summary>
        /// 空き容量チェッカー
        /// </summary>
        protected StorageSpaceChecker StorageSpaceChecker = new();

        /// <summary>
        /// 保存画像リスト
        /// </summary>
        /// <remarks>削除用の古いファイルリスト</remarks>
        protected List<FileInfo> SavedImageFileList = new();

        /// <summary>
        /// Insight用ヘルパ
        /// </summary>
        protected MekikiHelper MekikiHelper = new();

        /// <summary>
        /// Jpegコーデック
        /// </summary>
        private readonly ImageCodecInfo? _jpegCodec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

        /// <summary>
        /// 保存とInsightタスクスレッド
        /// </summary>
        private QueueThread<Tuple<IInspectionTask, IInspectionTask>> _saveImageAndInsightTaskQueueThread;
        private bool _localImageSaveSuspend = false;

        /// <summary>
        /// 検査終了用キャンセルトークン
        /// </summary>
        private CancellationTokenSource _deactivationCts = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InspectionController() : base(typeof(InspectionController).Name, -1)
        {
            // 検査制御情報
            _controllInfo = new InspectionControllInfo(1);

            // 検査タスク処理スレッド
            _inspectionTaskQueueThread = new QueueThread<IInspectionTask>();
            _inspectionTaskQueueThread.Dequeue += this.InspectionTaskQueueThread_Dequeue;

            _saveImageAndInsightTaskQueueThread = new QueueThread<Tuple<IInspectionTask, IInspectionTask>>() { Priority = ThreadPriority.Lowest };
            _saveImageAndInsightTaskQueueThread.Dequeue += this.SaveImageAndInsightTaskQueueThread_Dequeue;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 検査の有効化
        /// </summary>
        protected virtual async Task<bool> ActivateInspectionAsync()
        {
            try
            {
                // Config無効
                if (this.Config is not ProjectConfiguration prjConfig)
                {
                    Serilog.Log.Error($"{this}, invalid config");
                    return false;
                }

                // 有効化の開始
                if (0 == Interlocked.CompareExchange(ref _inspectionActivationCounter, 1, 0))
                {
                    var connectionTask = new List<Task<bool>>();

                    Task<bool> taskItem;

                    // DIO接続
                    taskItem = Task.Run(() =>
                    {
                        try
                        {
                            if (true == (_dioController?.Open() ?? false))
                            {
                                Serilog.Log.Information($"{this}, dio connection success");
                                var index = 0;
                                foreach (var d in _dioController.AllDevices)
                                {
                                    this.OnDeviceStatusChanged(d, DeviceKind.DigitalIO, index++, DeviceStatusKind.Enabled);
                                }
                            }
                            else
                            {
                                Serilog.Log.Error($"{this}, dio connection failed");

                                if (_dioController is not null)
                                {
                                    var index = 0;
                                    foreach (var d in _dioController.AllDevices)
                                    {
                                        this.OnDeviceStatusChanged(d, DeviceKind.DigitalIO, index++, DeviceStatusKind.Disabled);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }

                        return _dioController?.Enabled ?? false;
                    });
                    connectionTask.Add(taskItem);

                    // 照明接続
                    taskItem = Task.Run(() =>
                    {
                        try
                        {
                            if (true == (_lightController?.Open() ?? false))
                            {
                                Serilog.Log.Information($"{this}, light connection success");

                                var index = 0;
                                foreach (var d in _lightController.Devices)
                                {
                                    this.OnDeviceStatusChanged(d, DeviceKind.Light, index++, DeviceStatusKind.Enabled);
                                }
                            }
                            else
                            {
                                Serilog.Log.Error($"{this}, light connection failed");

                                if (_lightController is not null)
                                {
                                    var index = 0;
                                    foreach (var d in _lightController.Devices)
                                    {
                                        this.OnDeviceStatusChanged(d, DeviceKind.Light, index++, DeviceStatusKind.Disabled);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }

                        return _lightController?.Enabled ?? false;
                    });
                    connectionTask.Add(taskItem);

                    // カメラ接続
                    taskItem = Task.Run(() =>
                    {
                        var isSuccess = true;

                        try
                        {
                            foreach (var grabCtrl in _grabberControllers)
                            {
                                if (true == grabCtrl.Open())
                                {
                                    Serilog.Log.Information($"{this}, {grabCtrl.GrabberType} connection success");

                                    var index = 0;
                                    foreach (var g in grabCtrl.Grabbers)
                                    {
                                        this.OnDeviceStatusChanged(g, DeviceKind.Camera, index++, DeviceStatusKind.Enabled);
                                    }
                                }
                                else
                                {
                                    isSuccess = false;
                                    Serilog.Log.Error($"{this}, {grabCtrl.GrabberType} connection failed");

                                    var index = 0;
                                    foreach (var g in grabCtrl.Grabbers)
                                    {
                                        this.OnDeviceStatusChanged(g, DeviceKind.Camera, index++, DeviceStatusKind.Disabled);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }

                        return isSuccess;
                    });
                    connectionTask.Add(taskItem);

                    // Insight接続
                    taskItem = Task.Run(() =>
                    {
                        var isSuccess = false;

                        try
                        {
                            if (true == (prjConfig.InsightLinkageControllerParameter?.IsUse ?? false))
                            {
                                isSuccess = _insightLinkageController?.Open() ?? false;

                                if (true == isSuccess)
                                {
                                    Serilog.Log.Information($"{this}, insight connection success");
                                }
                                else
                                {
                                    Serilog.Log.Error($"{this}, insight connection failed");
                                }
                            }
                            else
                            {
                                isSuccess = true;
                                Serilog.Log.Information($"{this}, insight not use");
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }

                        return isSuccess;
                    });
                    connectionTask.Add(taskItem);

                    // Onnx
                    taskItem = Task.Run(() =>
                    {
                        var isSuccess = false;

                        try
                        {
                            if (true == (this.Parameter?.ImageAcquisitionOnly ?? true))
                            {
                                return isSuccess = true;
                            }

                            isSuccess = _onnxModelController?.Open() ?? false;

                            if (true == isSuccess && _onnxModelController is not null)
                            {
                                Serilog.Log.Information($"{this}, onnx initialization success");

                                foreach (var m in _onnxModelController.Models)
                                {
                                    var index = _onnxModelController.Models.IndexOf(m);

                                    _onnxInputData[index]?.Initialize(m);

                                    m.DryFire(1);
                                }
                            }
                            else
                            {
                                Serilog.Log.Error($"{this}, onnx initialization failed");
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }

                        return isSuccess;
                    });
                    connectionTask.Add(taskItem);

                    // 空き容量チェックと最古画像の削除
                    var taskDisk = Task.CompletedTask;
                    if (true == prjConfig.Operation.IsAutoDeleteOldestImage)
                    {
                        taskDisk = Task.Run(() => this.CheckStorageSpaceAndReduction());
                    }

                    // 接続タスク待機
                    var result = await Task.WhenAll(connectionTask);

                    // 画像ファイル削除を待機する(接続と並列で実行させるためにこのタイミング)
                    await taskDisk;

                    // 接続結果
                    var isSuccess = true;
                    foreach (var r in result)
                    {
                        isSuccess &= r;
                    }

                    // 1つでも接続できない場合は検査開始を不可とする
                    if (false == isSuccess)
                    {
                        throw new Exception("device invalid");
                    }

                    // 空き容量チェッカー
                    this.StorageSpaceChecker.Clear();
                    this.StorageSpaceChecker.AddTargetPath(prjConfig.Path.Data.FullName);
                    this.StorageSpaceChecker.SetLowerLimit(StorageSpaceChecker.LowerLimitUnit.BasisPoint, (long)(prjConfig.Operation.LowerLimitPercentageOfAutoDeleteImage * 100));

                    // 検査制御情報を初期化する
                    _controllInfo.Initialize();

                    if (this.Parameter is IInspectionControllerParameter ip)
                    {
                        // 撮影遅延時間の補正
                        var delayRaito = 1d;
                        if (0 < ip.ReferenceConveyingSpeedMillPerSecond && 0 < ip.SpecifiedConveyingSpeedMillPerSecond)
                        {
                            delayRaito *= ip.ReferenceConveyingSpeedMillPerSecond / ip.SpecifiedConveyingSpeedMillPerSecond;
                        }

                        #region カメラグルーピング(撮影遅延時間毎)
                        if (_controllInfo.AllGrabbers.Count == ip.AcquisitionTriggerDelayMs.Length)
                        {
                            // 撮影遅延時間毎にグループを分ける
                            var tempGroups = new Dictionary<int, List<IGrabber>>();
                            foreach (var i in Enumerable.Range(0, ip.AcquisitionTriggerDelayMs.Length))
                            {
                                var delayTimeMs = System.Math.Max(0, ip.AcquisitionTriggerDelayMs[i]);

                                if (false == tempGroups.ContainsKey(delayTimeMs))
                                {
                                    tempGroups.Add(delayTimeMs, new List<IGrabber>() { _controllInfo.AllGrabbers[i] });
                                }
                                else
                                {
                                    var grabbers = tempGroups[delayTimeMs];
                                    grabbers.Add(_controllInfo.AllGrabbers[i]);
                                }
                            }

                            // TimeSpan毎にIGrabberを登録する
                            foreach (var group in tempGroups)
                            {
                                var timeSpan = TimeSpan.FromMilliseconds(group.Key * delayRaito);
                                _controllInfo.AcqTimingGroupsTimeSpan.Add(timeSpan);
                                _controllInfo.AcqTimingGroupsGrabber.Add(group.Value);
                            }
                        }
                        else
                        {
                            // 全IGrabberを同一のTimeSpanで登録する
                            var allAtOnce = (0 < ip.AcquisitionTriggerDelayMs.Length) ? ip.AcquisitionTriggerDelayMs.First() : 0;

                            var timeSpan = TimeSpan.FromMilliseconds(System.Math.Max(0, allAtOnce) * delayRaito);
                            _controllInfo.AcqTimingGroupsTimeSpan.Add(timeSpan);
                            _controllInfo.AcqTimingGroupsGrabber.Add(_controllInfo.AllGrabbers);
                        }
                        #endregion
                    }
                    else
                    {
                        isSuccess = false;
                        throw new NullReferenceException("IInspectionControllerParameter");
                    }

                    // 検査有効化イベント通知
                    this.OnInspectionStatusChanged(InspectionEvent.Activating, null);

                    // 検査有効化
                    Interlocked.Increment(ref _inspectionActivationCounter);

                    #region イベント登録

                    // 画像取得制御リスト
                    foreach (var grabberController in _grabberControllers)
                    {
                        grabberController.DataGrabbed += this.GrabImageController_DataGrabbed;
                        grabberController.GrabberDisabled += this.GrabImageController_GrabberDisabled;
                    }

                    // DIO制御
                    if (_dioController is not null)
                    {
                        _dioController.InputChanged += this.DioController_InputChanged;
                        _dioController.Disabled += this.DioController_Disabled;
                    }

                    // 照明制御
                    if (_lightController is not null)
                    {
                        _lightController.Disabled += this.LightController_Disabled;
                    }

                    // Insight
                    if (_insightLinkageController is not null)
                    {
                        _insightLinkageController.Processed += this.InsightLinkageController_Processed;
                    }

                    #endregion

                    // 入力信号監視を開始                
                    _dioController?.StartMonitoring();

                    // 照明点灯
                    if (_lightController is not null && this.Parameter is IInspectionControllerParameter param)
                    {
                        // 常時点灯の場合 照明ON
                        Parallel.For(0, _lightController.NumberOfDevice, (i) =>
                        {
                            if (LightingOnOffControlType.Continuous == prjConfig.LightControllerParameter?.DeviceParameters[i].OnOffControlType)
                            {
                                _lightController.Devices[i].TurnOn();
                            }
                        });
                    }

                    // 撮影開始
                    _areaSensorController?.Grabbers.ForEach(g => g.GrabContinuously());
                    _lineSensorController?.Grabbers.ForEach(g => g.GrabContinuously());
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.Enabled;
        }

        /// <summary>
        /// 検査の無効化
        /// </summary>
        protected virtual async Task DeactivateInspectionAsync(bool withClose = true)
        {
            try
            {
                // Config無効
                if (this.Config is not ProjectConfiguration prjConfig)
                {
                    Serilog.Log.Error($"{this}, invalid config");
                    return;
                }

                var stepIndex = 0;

                // 検査の無効化開始
                if (2 == Interlocked.CompareExchange(ref _inspectionActivationCounter, 1, 2))
                {
                    // 入力信号監視を停止          
                    _dioController?.StopMonitoring();
                    Serilog.Log.Debug($"{this}, {stepIndex++},{"DeactivateInspectionAsync"},{"StopMonitoring"}");

                    // 撮影停止
                    _grabberControllers.ForEach(g => g.StopGrabbing());
                    Serilog.Log.Debug($"{this}, {stepIndex++},{"DeactivateInspectionAsync"},{"StopGrabbing"}");

                    // 検査完了待機
                    await Task.Run(async () =>
                    {
                        var waitWatch = Stopwatch.StartNew();

                        try
                        {
                            Serilog.Log.Debug($"{this}, inspection completion wait started");

                            var allEnded = _controllInfo.IsAllTaskEnd;
                            var stopwatch = Stopwatch.StartNew();

                            while (false == allEnded && stopwatch.ElapsedMilliseconds < 5000)
                            {
                                await Task.Delay(100);

                                allEnded = _controllInfo.IsAllTaskEnd;
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                        finally
                        {
                            // 推論待ち解除
                            _deactivationCts.Cancel();

                            // タスク無効化
                            _controllInfo.InvalidateAllTask();

                            Serilog.Log.Information($"{this}, inspection completion wait finished {waitWatch.ElapsedMilliseconds}ms");
                        }
                    });

                    // 照明消灯
                    if (_lightController is not null && this.Parameter is IInspectionControllerParameter param)
                    {
                        // 常時点灯の場合 照明OFF
                        Parallel.For(0, _lightController.NumberOfDevice, (i) =>
                        {
                            if (LightingOnOffControlType.Continuous == prjConfig.LightControllerParameter?.DeviceParameters[i].OnOffControlType)
                            {
                                _lightController.Devices[i].TurnOn();
                            }
                        });
                    }
                    Serilog.Log.Debug($"{this}, {stepIndex++},{"DeactivateInspectionAsync"},{"LightingOff"}");

                    if (true == withClose)
                    {
                        var deactivateTask = new List<Task>();
                        Task taskItem;

                        Serilog.Log.Debug($"{this}, {stepIndex++},{"DeactivateInspectionAsync"},{"withClose"}");

                        // DIO切断
                        taskItem = Task.Run(() =>
                        {
                            try
                            {
                                if (true == (_dioController?.Close() ?? false))
                                {
                                    Serilog.Log.Information($"{this}, dio close success");
                                }
                                else
                                {
                                    Serilog.Log.Error($"{this}, dio close failed");
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });
                        deactivateTask.Add(taskItem);

                        // 照明切断
                        taskItem = Task.Run(async () =>
                        {
                            try
                            {
                                _lightController?.TurnOff();

                                await Task.Delay(1000);

                                if (true == (_lightController?.Close() ?? false))
                                {
                                    Serilog.Log.Information($"{this}, light close success");
                                }
                                else
                                {
                                    Serilog.Log.Error($"{this}, light close failed");
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });
                        deactivateTask.Add(taskItem);

                        // カメラ切断
                        taskItem = Task.Run(() =>
                        {
                            try
                            {
                                foreach (var grabCtrl in _grabberControllers)
                                {
                                    if (true == grabCtrl.Close())
                                    {
                                        Serilog.Log.Information($"{this}, {grabCtrl.GrabberType} close success");
                                    }
                                    else
                                    {
                                        Serilog.Log.Error($"{this}, {grabCtrl.GrabberType} close failed");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });
                        deactivateTask.Add(taskItem);

                        // Insight
                        taskItem = Task.Run(() =>
                        {
                            try
                            {
                                if (true == (_insightLinkageController?.Close() ?? false))
                                {
                                    Serilog.Log.Information($"{this}, insight close success");
                                }
                                else
                                {
                                    Serilog.Log.Error($"{this}, insight close failed");
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });
                        deactivateTask.Add(taskItem);

                        Serilog.Log.Debug($"{this}, {stepIndex++},{"DeactivateInspectionAsync"},{"withClose"}");

                        // タスク待機
                        await Task.WhenAll(deactivateTask);

                        Serilog.Log.Debug($"{this}, {stepIndex++},{"DeactivateInspectionAsync"},{"withClose"}");

                        // 検査タスク破棄
                        _controllInfo.DisposeAllTask();

                        Serilog.Log.Debug($"{this}, {stepIndex++},{"DeactivateInspectionAsync"},{"withClose"}");
                    }

                    // DIO制御
                    if (_dioController is not null)
                    {
                        _dioController.InputChanged -= this.DioController_InputChanged;
                        _dioController.Disabled -= this.DioController_Disabled;
                    }

                    // 照明制御
                    if (_lightController is not null)
                    {
                        _lightController.Disabled -= this.LightController_Disabled;
                    }

                    // Insight
                    if (_insightLinkageController is not null)
                    {
                        _insightLinkageController.Processed -= this.InsightLinkageController_Processed;
                    }

                    // 画像取得制御リスト
                    foreach (var grabberController in _grabberControllers)
                    {
                        grabberController.DataGrabbed -= this.GrabImageController_DataGrabbed;
                        grabberController.GrabberDisabled -= this.GrabImageController_GrabberDisabled;

                    }

                    // 検査無効化イベント通知
                    this.OnInspectionStatusChanged(InspectionEvent.Deactivating, null);

                    // 検査無効化の確定
                    Interlocked.Exchange(ref _inspectionActivationCounter, 0);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 撮影トリガー信号処理
        /// </summary>
        /// <param name="holding">信号がONの場合はtrue</param>
        /// <param name="origin">信号ONの起点時刻</param>
        /// <param name="stopwatchFromOrigin">信号ONからの経過時間</param>
        protected virtual void ProcessAcquisitionTriggerSignal(bool holding, DateTime origin, Stopwatch stopwatchFromOrigin)
        {
            try
            {
                // ONした場合
                if (true == holding)
                {
                    // 検査が有効な場合
                    if (true == this.Enabled)
                    {
                        // 新しいタスクへの参照を取得する
                        var newTask = _controllInfo.GetNewTask(origin, stopwatchFromOrigin);

                        // 処理すべきタスクが存在する場合
                        if (newTask is not null)
                        {
                            // キューに入れる
                            _inspectionTaskQueueThread.Enqueue(newTask);
                        }
                        else
                        {
                            Serilog.Log.Warning($"{this}, ignore signal ON, overlapped");
                        }

                        // 信号ONをイベント通知
                        this.OnInspectionStatusChanged(InspectionEvent.InputSignalOn, newTask);
                    }
                }
                // OFFした場合
                else
                {
                    var stopwatch = Stopwatch.StartNew();

                    // 対応するタスクへの参照を取得する
                    var selectedTask = _controllInfo.GetTaskOfIoRising();

                    // 処理中のタスクがある場合
                    if (selectedTask is not null && this.Config is ProjectConfiguration prjConfig)
                    {
                        // 信号ON時間を取得する
                        var onTime = Convert.ToInt32(selectedTask.StopwatchFromOrigin.ElapsedMilliseconds);

                        // 信号ON時間が極端に短い場合
                        if ((prjConfig.InspectionControllerParameter?.AcquisitionTriggerValidHoldingTimeMs ?? 0) > onTime)
                        {
                            // チャタリングとみなす(OFFとして採用しない)
                            Serilog.Log.Warning($"{this}, ignore signal OFF, chattering");
                            return;
                        }
                        else
                        {
                            // 物有り信号OFFを通知する
                            selectedTask.NotifyIoFalled(stopwatch);
                        }
                    }
                    else
                    {
                        Serilog.Log.Debug($"{this}, ignore signal OFF, task not exists");
                    }

                    // 信号OFFをイベント通知
                    this.OnInspectionStatusChanged(InspectionEvent.InputSignalOff, selectedTask);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 空き容量チェックと最古画像の削除
        /// </summary>
        /// <remarks>検査開始時と保存時に使用する</remarks>
        protected void CheckStorageSpaceAndReduction()
        {
            try
            {
                if (this.Config is not ProjectConfiguration prjConfig || this.Parameter is not IInspectionControllerParameter param)
                {
                    Serilog.Log.Error($"{this}, invalid config");
                    return;
                }

                // 空き容量が確保できるまで削除を繰り返す
                while (true == this.StorageSpaceChecker.IsBelowLowerLimit)
                {
                    #region 削除対象の画像をリストアップ
                    // 削除対象の画像をリストアップ
                    if (0 >= this.SavedImageFileList.Count)
                    {
                        // 画像保存フォルダを列挙
                        var candidateDirectoies = prjConfig.Path.Data.GetDirectories("20*");

                        // 画像保存フォルダが存在する場合
                        if (0 < candidateDirectoies.Length)
                        {
                            // フォルダ名でソート
                            Array.Sort(candidateDirectoies, (d1, d2) => d1.Name.CompareTo(d2.Name));

                            // フォルダを列挙
                            foreach (var d in candidateDirectoies)
                            {
                                var candidateFiles = d.GetFiles($"*20*{param.ImageSaveFormat.GetFileExtension()}");

                                // 画像ファイルが存在しなければフォルダ削除
                                if (0 >= candidateFiles.Length)
                                {
                                    try
                                    {
                                        d.Delete(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Serilog.Log.Warning(ex, ex.Message);
                                    }

                                    // 削除に失敗した場合はリネームを試みる(列挙対象から外す意図)
                                    try
                                    {
                                        d.Refresh();
                                        if (true == d.Exists)
                                        {
                                            d.MoveTo(System.IO.Path.Combine(d.Parent?.FullName ?? "", "failed_" + d.Name));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Serilog.Log.Warning(ex, ex.Message);
                                    }

                                    continue;
                                }

                                // 画像ファイルが存在している場合はソートしてリストに追加
                                Array.Sort(candidateFiles, (f1, f2) => f1.Name.CompareTo(f2.Name));
                                this.SavedImageFileList.AddRange(candidateFiles);

                                break;
                            }
                        }
                    }
                    #endregion

                    // 削除対象の画像が無ければ終了
                    if (0 >= this.SavedImageFileList.Count)
                    {
                        break;
                    }
                    // 削除対象の画像ファイルがある場合は削除する
                    else
                    {
                        // 最古ファイルへの参照を取得する
                        var oldestFileInfo = this.SavedImageFileList.FirstOrDefault();

                        // 最古ファイルを削除する
                        try
                        {
                            if (oldestFileInfo is not null)
                            {
                                oldestFileInfo.Delete();
                                Serilog.Log.Information($"{this}, image delete. {oldestFileInfo.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);

                            // 削除に失敗した場合はリネームを試みる
                            try
                            {
                                if (oldestFileInfo is not null)
                                {
                                    oldestFileInfo.Refresh();
                                    if (true == oldestFileInfo.Exists)
                                    {
                                        oldestFileInfo.MoveTo(System.IO.Path.Combine(oldestFileInfo.Directory?.FullName ?? "", "failed_" + oldestFileInfo.Name), true);
                                    }
                                }
                            }
                            catch (Exception ex2)
                            {
                                Serilog.Log.Warning(ex2, ex2.Message);
                            }
                        }
                        finally
                        {
                            // 削除の成否に関わらず削除対象ファイルから除外する
                            this.SavedImageFileList.RemoveAt(0);
                        }

                        // 削除したファイルが含まれるディレクトリが空の場合は削除する
                        try
                        {
                            if (oldestFileInfo?.Directory is DirectoryInfo parentDirectory)
                            {
                                var candidateFiles = parentDirectory.GetFiles($"*20*{param.ImageSaveFormat.GetFileExtension()}");

                                if (0 >= candidateFiles.Length)
                                {
                                    try
                                    {
                                        parentDirectory.Delete(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Serilog.Log.Warning(ex, ex.Message);
                                    }

                                    // 削除に失敗した場合はリネームを試みる(列挙対象から外す意図)
                                    try
                                    {
                                        parentDirectory.Refresh();
                                        if (true == parentDirectory.Exists)
                                        {
                                            parentDirectory.MoveTo(System.IO.Path.Combine(parentDirectory.Parent?.FullName ?? "", "failed_" + parentDirectory.Name));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Serilog.Log.Warning(ex, ex.Message);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
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
        /// 検査タスクを処理する
        /// </summary>
        /// <param name="selectedTask">対象のタスクデータ</param>
        /// <remarks>トリガ信号ON以降の処理</remarks>
        protected async Task ProcessInspectionTaskAsync(IInspectionTask selectedTask)
        {
            try
            {
                if (this.Config is not ProjectConfiguration prjConfig || this.Parameter is not IInspectionControllerParameter param)
                {
                    Serilog.Log.Error($"{this}, invalid config");
                    return;
                }

                // 画像保存有無の初期化
                selectedTask.ImageSavingEnabled = prjConfig.Operation.ImageSavingCondition == ImageSavingCondition.Always;

                var taskResultList = new List<Task<bool>>();

                // 照明を点灯させる
                var taskLightOn = Task.Run<bool>(() =>
                {
                    #region 照明を点灯させる

                    var isSuccess = !selectedTask.Enabled;

                    if (true == selectedTask.Enabled)
                    {
                        try
                        {
                            if (_lightController is not null)
                            {
                                // 照明制御無しの場合を考慮して最低true1つで初期化
                                var results = Enumerable.Repeat(1, System.Math.Max(1, _lightController.NumberOfDevice)).ToArray();

                                // 信号ON時点灯の場合
                                if (
                                    (param.LightTurnOnTiming == LightControlTiming.Fastest)
                                || (param.LightTurnOnTiming == LightControlTiming.FixedDelayFromSignalOn)
                                )
                                {
                                    // 照明点灯処理
                                    Parallel.For(0, _lightController.NumberOfDevice, async (i) =>
                                    {
                                        // ワーク単位点灯制御が有効な場合
                                        if (prjConfig.LightControllerParameter?.DeviceParameters[i].OnOffControlType == LightingOnOffControlType.OneByOne)
                                        {
                                            // 遅延設定が有効な場合
                                            if (param.LightTurnOnTiming == LightControlTiming.FixedDelayFromSignalOn)
                                            {
                                                // 現在までの経過時間
                                                var currentSpan = selectedTask.StopwatchFromOrigin.Elapsed;

                                                // 待機時間を算出する
                                                var delayMs = param.LightTurnOnDelayMs.FirstOrDefault();
                                                if (param.LightTurnOnDelayMs.Length > i)
                                                {
                                                    delayMs = param.LightTurnOnDelayMs[i];
                                                }
                                                var nowNeedSpanMs = System.Math.Max(0, Convert.ToInt32(delayMs - currentSpan.TotalMilliseconds));

                                                // 撮影遅延
                                                await Task.Delay(nowNeedSpanMs);
                                            }

                                            // 照明ON
                                            results[i] = Convert.ToInt32(_lightController.Devices[i].TurnOn());
                                        }
                                    });
                                }
                                // 信号OFF時点灯の場合
                                else if (param.LightTurnOnTiming == LightControlTiming.FixedDelayFromSignalOff)
                                {
                                    // 信号OFFを待機する
                                    selectedTask.WaitIoFalled(param.AcquisitionTriggerHoldingTimeoutMs, out bool isTimeout);

                                    // 照明点灯処理
                                    Parallel.For(0, _lightController.NumberOfDevice, async (i) =>
                                    {
                                        // ワーク単位点灯制御が有効な場合
                                        if (prjConfig.LightControllerParameter?.DeviceParameters[i].OnOffControlType == LightingOnOffControlType.OneByOne)
                                        {
                                            // 現在までの経過時間
                                            var currentSpan = selectedTask.StopwatchFromIoFalled?.Elapsed ?? TimeSpan.FromMilliseconds(param.AcquisitionTriggerHoldingTimeoutMs);

                                            // 待機時間を算出する
                                            var delayMs = param.LightTurnOnDelayMs.FirstOrDefault();
                                            if (param.LightTurnOnDelayMs.Length > i)
                                            {
                                                delayMs = param.LightTurnOnDelayMs[i];
                                            }
                                            var nowNeedSpanMs = System.Math.Max(0, Convert.ToInt32(delayMs - currentSpan.TotalMilliseconds));

                                            // 撮影遅延
                                            await Task.Delay(nowNeedSpanMs);

                                            // 照明ON
                                            results[i] = Convert.ToInt32(_lightController.Devices[i].TurnOn());
                                        }
                                    });
                                }

                                isSuccess = results.Sum() >= results.Length;
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    }

                    return isSuccess;

                    #endregion
                });
                taskResultList.Add(taskLightOn);

                // 撮影を開始する
                var taskGrabBegin = Task.Run<bool>(async () =>
                {
                    #region 撮影を開始する
                    var isSuccess = !selectedTask.Enabled;

                    if (true == selectedTask.Enabled)
                    {
                        try
                        {
                            // 撮影開始処理
                            var grabberTasks = new Task<int>[_controllInfo.AcqTimingGroupsTimeSpan.Count];
                            Parallel.For(0, grabberTasks.Length, index =>
                            {
                                grabberTasks[index] = Task<int>.Run(async () =>
                                {
                                    var grabberResults = Enumerable.Repeat(1, _controllInfo.AcqTimingGroupsGrabber[index].Count).ToArray();

                                    // 現在までの経過時間
                                    var currentSpan = selectedTask.StopwatchFromOrigin.Elapsed;

                                    // 待機が必要な場合
                                    if (_controllInfo.AcqTimingGroupsTimeSpan[index] > currentSpan)
                                    {
                                        // 待機時間を算出する
                                        var nowNeedSpan = _controllInfo.AcqTimingGroupsTimeSpan[index] - currentSpan;

                                        // 撮影遅延
                                        await Task.Delay(nowNeedSpan);
                                    }

                                    // 撮影開始
                                    Parallel.ForEach(_controllInfo.AcqTimingGroupsGrabber[index], grabber =>
                                    {
                                        var grabberIndex = _controllInfo.AcqTimingGroupsGrabber[index].IndexOf(grabber);

                                        grabberResults[grabberIndex] = Convert.ToInt32(grabber.DoSoftTrigger());
                                    });

                                    return Convert.ToInt32(grabberResults.Sum() >= grabberResults.Length);
                                });
                            });

                            // 撮影開始処理の待機
                            var results = await Task.WhenAll(grabberTasks);

                            // 撮影開始処理の結果
                            isSuccess &= (results.Sum() >= results.Length);
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    }

                    return isSuccess;
                    #endregion
                });
                taskResultList.Add(taskGrabBegin);

                // 照明を消灯させる
                var taskLightOff = Task.Run<bool>(() =>
                {
                    #region 照明を消灯させる

                    var isSuccess = !selectedTask.Enabled;

                    try
                    {
                        if (_lightController is not null)
                        {
                            // 照明制御無しの場合を考慮して最低true1つで初期化
                            var results = Enumerable.Repeat(1, System.Math.Max(1, _lightController.NumberOfDevice)).ToArray();

                            // 即時消灯の場合
                            if (param.LightTurnOffTiming == LightControlTiming.Fastest)
                            {
                                // 撮影完了を待機する
                                selectedTask.WaitGrabEnd();

                                // 照明消灯処理
                                Parallel.For(0, _lightController.NumberOfDevice, i =>
                                {
                                    // ワーク単位点灯制御が有効な場合
                                    if (prjConfig.LightControllerParameter?.DeviceParameters[i].OnOffControlType == LightingOnOffControlType.OneByOne)
                                    {
                                        // 照明OFF
                                        results[i] = Convert.ToInt32(_lightController.Devices[i].TurnOff());
                                    }
                                });
                            }
                            // 信号OFF時消灯の場合
                            else if (param.LightTurnOffTiming == LightControlTiming.FixedDelayFromSignalOff)
                            {
                                // 信号OFFを待機する
                                selectedTask.WaitIoFalled(param.AcquisitionTriggerHoldingTimeoutMs, out bool isTimeout);

                                // 照明消灯処理
                                Parallel.For(0, _lightController.NumberOfDevice, async (i) =>
                                {
                                    // ワーク単位点灯制御が有効な場合
                                    if (prjConfig.LightControllerParameter?.DeviceParameters[i].OnOffControlType == LightingOnOffControlType.OneByOne)
                                    {
                                        // 現在までの経過時間
                                        var currentSpan = selectedTask.StopwatchFromIoFalled?.Elapsed ?? TimeSpan.FromMilliseconds(param.AcquisitionTriggerHoldingTimeoutMs);

                                        // 待機時間を算出する
                                        var delayMs = param.LightTurnOffDelayMs.FirstOrDefault();
                                        if (param.LightTurnOffDelayMs.Length > i)
                                        {
                                            delayMs = param.LightTurnOffDelayMs[i];
                                        }
                                        var nowNeedSpanMs = System.Math.Max(0, Convert.ToInt32(delayMs - currentSpan.TotalMilliseconds));

                                        // 撮影遅延
                                        await Task.Delay(nowNeedSpanMs);

                                        // 照明OFF
                                        results[i] = Convert.ToInt32(_lightController.Devices[i].TurnOff());
                                    }
                                });
                            }
                            // 信号ON時消灯の場合
                            else if (param.LightTurnOffTiming == LightControlTiming.FixedDelayFromSignalOn)
                            {
                                // 照明点灯処理
                                Parallel.For(0, _lightController.NumberOfDevice, async (i) =>
                                {
                                    // ワーク単位点灯制御が有効な場合
                                    if (prjConfig.LightControllerParameter?.DeviceParameters[i].OnOffControlType == LightingOnOffControlType.OneByOne)
                                    {
                                        // 現在までの経過時間
                                        var currentSpan = selectedTask.StopwatchFromOrigin.Elapsed;

                                        // 待機時間を算出する
                                        var delayMs = param.LightTurnOffDelayMs.FirstOrDefault();
                                        if (param.LightTurnOffDelayMs.Length > i)
                                        {
                                            delayMs = param.LightTurnOffDelayMs[i];
                                        }
                                        var nowNeedSpanMs = System.Math.Max(0, Convert.ToInt32(delayMs - currentSpan.TotalMilliseconds));

                                        // 撮影遅延
                                        await Task.Delay(nowNeedSpanMs);

                                        // 照明OFF
                                        results[i] = Convert.ToInt32(_lightController.Devices[i].TurnOff());
                                    }
                                });
                            }

                            isSuccess = results.Sum() >= results.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }

                    return isSuccess;

                    #endregion
                });
                taskResultList.Add(taskLightOff);

                // 撮影完了を待機する
                selectedTask.WaitGrabEnd();
                this.OnInspectionStatusChanged(InspectionEvent.AllGrabCompleted, selectedTask);

                // 撮影のみでない場合
                if (false == param.ImageAcquisitionOnly)
                {
                    // 推論完了を待機する
                    selectedTask.WaitInferenceEnd();

                    // 判定処理を行う
                    var taskJudgement = Task.Run<bool>(() =>
                    {
                        var isSuccess = !selectedTask.Enabled;

                        this.OnInspectionStatusChanged(InspectionEvent.JudgmentRequest, selectedTask);

                        return isSuccess;
                    });
                    taskResultList.Add(taskJudgement);

                    // 判定処理を待機する
                    await taskJudgement;

                    // 判定処理までの完了を通知する
                    this.OnInspectionStatusChanged(InspectionEvent.JudgmentCompleted, selectedTask);
                }

                #region 画像保存用にタスク情報を複製する

                // 画像保存用にタスク情報を複製する
                var selectedTaskForImageSaving = selectedTask.ShallowCopy();
                {
                    try
                    {
                        // Bitmapのみディープコピー
                        foreach (var itemIndex in Enumerable.Range(0, selectedTaskForImageSaving.Items.Length))
                        {
                            if (
                                (selectedTaskForImageSaving.Items[itemIndex] is IInspectionTaskItem destinationItem)
                            && (selectedTask.Items[itemIndex] is IInspectionTaskItem sourceItem)
                            )
                            {
                                destinationItem.Bitmap = (Bitmap?)sourceItem.Bitmap?.Clone();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                }

                #endregion

                #region 検査結果データを生成する
                try
                {
                    selectedTask.CreateInspectionResult();
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                #endregion

                // 結果出力タイミングを供給する
                var taskTimingProviding = Task.Run<bool>(async () =>
                {
                    #region 設定に従いタイミングを提供(イベント通知)する
                    var isSuccess = !selectedTask.Enabled;

                    if (true == selectedTask.Enabled)
                    {
                        try
                        {
                            var waitSpan = new TimeSpan();

                            if (ResultOutputTiming.FixedDelayFromSignalOn == param.ResultOutputTiming)
                            {
                                var currentSpan = selectedTask.StopwatchFromOrigin.Elapsed;
                                if (param.ResultOutputDelayMs > currentSpan.TotalMilliseconds)
                                {
                                    waitSpan = new TimeSpan(0, 0, 0, 0, param.ResultOutputDelayMs - (int)currentSpan.TotalMilliseconds);
                                }
                            }
                            else if (ResultOutputTiming.FixedDelayFromAcquisition == param.ResultOutputTiming)
                            {
                                var currentSpan = selectedTask.StopwatchFromGrabEnd?.Elapsed ?? new TimeSpan();
                                if (param.ResultOutputDelayMs > currentSpan.TotalMilliseconds)
                                {
                                    waitSpan = new TimeSpan(0, 0, 0, 0, param.ResultOutputDelayMs - (int)currentSpan.TotalMilliseconds);
                                }
                            }

                            // 待機が必要な場合
                            if (0 < waitSpan.TotalMilliseconds)
                            {
                                await Task.Delay(waitSpan);
                            }

                            // タイミング提供
                            this.OnInspectionStatusChanged(InspectionEvent.OutputTiming, selectedTask);
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    }

                    return isSuccess;
                    #endregion
                });
                taskResultList.Add(taskTimingProviding);

                // 各タスクの完了を待機する
                await Task.WhenAll(taskResultList.ToArray());

                #region 画像保存とInsightへの結果送信を行う(待機せずにバックグラウンド処理)
                {
                    if (false == _localImageSaveSuspend)
                    {
                        // キューが上限を超えたらローカル画像保存を一時停止する
                        if (param.LocalImageSaveTaskQueueUpperLimit < _saveImageAndInsightTaskQueueThread.Count)
                        {
                            Serilog.Log.Warning($"{this}, local image save suspend, {_saveImageAndInsightTaskQueueThread.Count}/ {param.LocalImageSaveTaskQueueUpperLimit}");
                            _localImageSaveSuspend = true;
                        }
                    }
                    // キューが再開可能な状態になったらローカル画像保存を再開する
                    else if (param.LocalImageSaveTaskQueueResumeLimit >= _saveImageAndInsightTaskQueueThread.Count)
                    {
                        _localImageSaveSuspend = false;
                        Serilog.Log.Warning($"{this}, local image save resume, {_saveImageAndInsightTaskQueueThread.Count}/ {param.LocalImageSaveTaskQueueResumeLimit}");
                    }

                    _saveImageAndInsightTaskQueueThread.Enqueue(Tuple.Create(selectedTask, selectedTaskForImageSaving));
                }
                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 検査タスクを処理する
        /// </summary>
        private void InspectionTaskQueueThread_Dequeue(object sender, IInspectionTask selectedTask)
        {
            try
            {
                Task.Run(() => this.ProcessInspectionTaskAsync(selectedTask));
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 画像保存とInsightへの結果送信を行う
        /// </summary>
        private async void SaveImageAndInsightTaskQueueThread_Dequeue(object sender, Tuple<IInspectionTask, IInspectionTask> item)
        {
            var taskSource = item.Item1;
            var taskImageSave = item.Item2;

            try
            {
                if (this.Config is not ProjectConfiguration prjConfig || this.Parameter is not IInspectionControllerParameter param)
                {
                    throw new ArgumentNullException("invalid config");
                }

                var taskResultList = new List<Task<bool>>();

                // 画像を保存する(Insightデータも作る)
                var taskImageSaving = Task.Run<bool>(async () =>
                {
                    #region 画像を保存する

                    var isSuccess = true;

                    if (true == taskImageSave.Enabled)
                    {
                        try
                        {
                            var timeStamp = taskImageSave.DateTimeOrigin;

                            // フォルダ情報
                            var directoryInfo = new DirectoryInfo(System.IO.Path.Combine(prjConfig.Path.Data.FullName, $"{timeStamp:yyyyMMdd}"));
                            try
                            {
                                if (false == directoryInfo.Exists)
                                {
                                    directoryInfo.Create();
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                            finally
                            {
                                directoryInfo.Refresh();
                            }

                            // 付帯情報の有無をチェックする
                            var aditionalInfo = string.Empty;
                            if (false == string.IsNullOrEmpty(taskImageSave.LinkedInfomation.Trim()))
                            {
                                aditionalInfo = "_" + taskImageSave.LinkedInfomation.Trim();
                            }

                            // タイムスタンプをプリフィックスに持つ基準となるファイル名
                            var baseFileNameFixed = $"{timeStamp:yyyyMMddHHmmssffffff}{aditionalInfo}{param.ImageSaveFormat.GetFileExtension()}";

                            // 保存通知(必要に応じて保存有無が書き換えられる)
                            this.OnInspectionStatusChanged(InspectionEvent.ImageSaving, taskImageSave);

                            // カメラ単位の処理結果
                            foreach (var item in taskImageSave.Items)
                            {
                                if (item is not null)
                                {
                                    // 画像ファイル情報
                                    var baseFileName = baseFileNameFixed;
                                    var fileInfo = new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, $"camera{item.GrabberIndex + 1}_{baseFileName}"));

                                    // 保存の有無
                                    var isNeedToSaveImage = taskImageSave.ImageSavingEnabled && (false == _localImageSaveSuspend);

                                    // 保存が必要な場合
                                    if (true == isNeedToSaveImage)
                                    {
                                        // 画像ファイル保存
                                        var imageSaveWatch = Stopwatch.StartNew();
                                        if (true == BitmapProcessor.Write(fileInfo, item.Bitmap))
                                        {
                                            Serilog.Log.Information($"{this}, task no={taskImageSave.TaskIndex:D7}, image save, success, {fileInfo.Name}, {imageSaveWatch.ElapsedMilliseconds}");
                                        }
                                        else
                                        {
                                            isSuccess = false;
                                            Serilog.Log.Error($"{this}, taksk no={taskImageSave.TaskIndex:D7}, image save, failed, {fileInfo.Name}");

                                            fileInfo = null;
                                            baseFileName = string.Empty;

                                            // 画像保存失敗フラグを立てる
                                            item.ImageSaveSucceeded = false;
                                            var itemIndex = Array.IndexOf(taskImageSave.Items, item);
                                            if (taskSource.Items.Length > itemIndex)
                                            {
                                                if (taskSource.Items[itemIndex] is InspectionTaskItem itemSource)
                                                {
                                                    itemSource.ImageSaveSucceeded = false;
                                                }
                                            }
                                        }
                                    }
                                    // 保存が不要な場合
                                    else
                                    {
                                        Serilog.Log.Debug($"{this}, taksk no={taskImageSave.TaskIndex:D7}, image save, canceled, {fileInfo.Name}");

                                        fileInfo = null;
                                        baseFileName = string.Empty;
                                    }

                                    #region Insightへの送信はjpgファイルとする(送信後に削除される)                                    
                                    try
                                    {
                                        if (_jpegCodec is null || item.Bitmap is null)
                                        {
                                            throw new ArgumentNullException("jpeg codec or bitmap");
                                        }

                                        // tempディレクトリ
                                        var tempDir = new DirectoryInfo(System.IO.Path.Combine(prjConfig.Path.Temp.FullName, "insight", "upload"));
                                        if (false == tempDir.Exists)
                                        {
                                            tempDir.Create();
                                        }
                                        tempDir.Refresh();

                                        // 基準ファイル名(jpg)
                                        baseFileName = $"{timeStamp:yyyyMMddHHmmssffffff}{AvailableImageFormat.Jpg.GetFileExtension()}";

                                        // 画像ファイル情報
                                        fileInfo = new FileInfo(System.IO.Path.Combine(tempDir.FullName, $"camera{item.GrabberIndex + 1}_{baseFileName}"));

                                        // MemoryStreamを使ってメモリ上に画像を保存
                                        using var memoryStream = new MemoryStream();

                                        // BitmapをJPEG形式でMemoryStreamに保存
                                        using var encoderParams = new EncoderParameters(1)
                                        {
                                            Param = { [0] = new EncoderParameter(Encoder.Quality, 85L) }
                                        };
                                        item.Bitmap.Save(memoryStream, _jpegCodec, encoderParams);

                                        // MemoryStreamをファイルに保存
                                        memoryStream.Seek(0, SeekOrigin.Begin);
                                        using var fileStream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write);
                                        memoryStream.CopyTo(fileStream);
                                    }
                                    catch (Exception ex)
                                    {
                                        fileInfo = null;
                                        baseFileName = string.Empty;
                                        Serilog.Log.Warning(ex, ex.Message);
                                        Serilog.Log.Debug($"{this}, taksk no={taskImageSave.TaskIndex:D7}, image save, failed, {fileInfo?.Name ?? string.Empty}");
                                    }
                                    #endregion

                                    // Bitmapを破棄
                                    this.DisposeSafely(item.Bitmap);

                                    // 判定が正しく行われている場合にInsight用データを作成する
                                    if (0 <= taskImageSave.JudgementIndex)
                                    {
                                        // 先頭カメラはallとしても保存する
                                        if (0 == Array.IndexOf(taskImageSave.Items, item))
                                        {
                                            // allメキキデータ追加
                                            var mekikiAll = new MekikiUnit(timeStamp, $"all", taskImageSave.ResultClassNames[taskImageSave.JudgementIndex], fileInfo, baseFileName);

                                            mekikiAll.Options.AddRange(taskImageSave.ResultClassNames);
                                            mekikiAll.GeneralValues.AddRange(taskImageSave.GeneralValues);

                                            taskImageSave.MekikiUnitList.Add(mekikiAll);
                                        }

                                        // cameraメキキデータ追加
                                        var mekikiCamera = new MekikiUnit(timeStamp, $"camera{item.GrabberIndex + 1}", taskImageSave.ResultClassNames[item.JudgementIndex], fileInfo, baseFileName);

                                        mekikiCamera.Options.AddRange(taskImageSave.ResultClassNames);
                                        mekikiCamera.GeneralValues.AddRange(item.GeneralValues);

                                        taskImageSave.MekikiUnitList.Add(mekikiCamera);
                                    }
                                }
                            }

                            // 空き容量チェックと最古画像の削除
                            if (true == prjConfig.Operation.IsAutoDeleteOldestImage)
                            {
                                await Task.Run(() => this.CheckStorageSpaceAndReduction());
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    }

                    return isSuccess;

                    #endregion
                });
                taskResultList.Add(taskImageSaving);

                // 画像保存を待機する
                var resultImageSaving = await taskImageSaving;

                // 撮影のみでない場合
                if (false == param.ImageAcquisitionOnly)
                {
                    // Insight連携(画像保存の後)
                    var taskInsightLingage = Task.Run<bool>(() =>
                    {
                        #region Insight連携

                        var isSuccess = true;

                        if (true == taskSource.Enabled && prjConfig.InsightLinkageControllerParameter is not null)
                        {
                            try
                            {
                                // リクエスト作成
                                var requests = this.MekikiHelper.CreateInsightLinkageRequest($"{taskSource.TaskIndex:D7}", prjConfig.InsightLinkageControllerParameter.DeviceUuid, InsightRequestType.Inspection, taskSource.MekikiUnitList.ToArray());

                                // リクエスト送信
                                _insightLinkageController?.Send(requests);
                            }
                            catch (Exception ex)
                            {
                                isSuccess = false;
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        }

                        return isSuccess;

                        #endregion
                    });
                    taskResultList.Add(taskInsightLingage);
                }

                // 各タスクの完了を待機する
                await Task.WhenAll(taskResultList.ToArray());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                // 検査タスクを完了状態としてリストから除外する
                _controllInfo.EndAndRemoveTask(taskSource);

                // タスク完了を通知する
                this.OnInspectionStatusChanged(InspectionEvent.TaskCompleted, taskSource);

                // 検査タスクを破棄
                this.DisposeSafely(taskSource);
            }
        }

        #endregion

        #region IGrabberControllerイベント

        /// <summary>
        /// 画像取得
        /// </summary>
        private void GrabImageController_DataGrabbed(object sender, Library.ImageGrabber.Data.IGrabberData data)
        {
            try
            {
                // カメラインデックスを取得する
                var grabberIndex = _controllInfo.GetSerializedGrabberIndex(data.Location);

                // 撮影カウンタを更新する
                var grabCounter = _controllInfo.TaskItemCounters[grabberIndex].PostIncrement();

                // 対応するタスクへの参照を取得する
                var selectedTask = _controllInfo.GetTaskOfGrabNotEnd(grabberIndex);

                // 対応するタスクが存在する場合
                if (selectedTask is not null)
                {
                    // 整合性チェック
                    if (selectedTask.TaskIndex != grabCounter)
                    {
                        Serilog.Log.Warning($"{this}, sequence anomalies detected. task={selectedTask.TaskIndex}, camera={data.Location.Y + 1}{data.Location.X + 1}, grab={grabCounter}");
                    }

                    // 撮影完了を通知する(カメラ単位のタスク項目への参照を取得する)
                    if (selectedTask.NotifyGrabEnd(grabberIndex, data.ToBitmap()) is IInspectionTaskItem taskItem)
                    {
                        if (false == (this.Parameter?.ImageAcquisitionOnly ?? true))
                        {
                            // 推論処理
                            Task.Run(async () =>
                            {
                                // カメラ単位撮影完了通知
                                this.OnInspectionStatusChanged(InspectionEvent.ItemGrabbed, selectedTask, taskItem);

                                var isEntered = false;

                                try
                                {
                                    // 処理権を取得できるまで待機
                                    await _controllInfo.InferenceExecutionResource.WaitAsync(_deactivationCts.Token);
                                    isEntered = true;

                                    if (_onnxInputData[taskItem.GrabberIndex] is OnnxDataInputBitmap inputData && taskItem.Bitmap is not null)
                                    {
                                        // 入力画像のセット
                                        if (inputData.Images != null)
                                        {
                                            inputData.Images.Clear();
                                            inputData.Images.Add(inputData.InputMetadata.Keys.First(), new[] { taskItem.Bitmap });
                                        }

                                        // 推論実行
                                        taskItem.InferenceResult = _onnxModelController?.Models[grabberIndex].Run(inputData);

                                        // 後処理要求イベント
                                        this.OnInspectionStatusChanged(InspectionEvent.ItemsPostProcessRequest, selectedTask, taskItem);
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    Serilog.Log.Warning($"{this}, inference canceled. task={selectedTask.TaskIndex}, camera={data.Location.Y + 1}{data.Location.X + 1}, grab={grabCounter}");
                                }
                                catch (Exception ex)
                                {
                                    Serilog.Log.Warning(ex, ex.Message);
                                }
                                finally
                                {
                                    if (true == isEntered)
                                    {
                                        // 処理権をリリース
                                        _controllInfo.InferenceExecutionResource.Release();

                                        // 推論完了を通知する
                                        selectedTask.NotifyInferenceEnd(taskItem.GrabberIndex);
                                    }
                                }
                            });
                        }
                    }
                }
                else
                {
                    Serilog.Log.Warning($"{this}, sequence anomalies detected. task not exists, camera={data.Location.Y + 1}{data.Location.X + 1}, grab={grabCounter}");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                data.Dispose();
            }
        }

        /// <summary>
        /// 画像取得無効化
        /// </summary>
        private void GrabImageController_GrabberDisabled(object sender, IGrabber g)
        {
            try
            {
                Serilog.Log.Error($"{this}, grabber{g.Location.Y + 1}{g.Location.X + 1} disabled");

                // カメラインデックスを取得する
                var grabberIndex = _controllInfo.GetSerializedGrabberIndex(g.Location);

                this.OnDeviceStatusChanged(g, DeviceKind.Camera, grabberIndex, DeviceStatusKind.Disabled);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IDigitalIODeviceControllerイベント

        /// <summary>
        /// IDigitalIODeviceControllerイベント:入力信号変化
        /// </summary>
        /// <param name="d">入力デバイスへの参照</param>
        /// <param name="risingEdgeIndex">立上り信号インデックス</param>
        /// <param name="fallingEdgeIndex">立下り信号インデックス</param>
        /// <param name="values">入力信号のON/OFF状態</param>
        private void DioController_InputChanged(object sender, IDigitalIOInputDevice d, int[] risingEdgeIndex, int[] fallingEdgeIndex, bool[] values)
        {
            try
            {
                // 信号変化日時を取得する
                var stopwatchFromOrigin = Stopwatch.StartNew();
                var currentDateTime = DateTime.Now;

                // DIOデバイスインデックスを取得する
                var deviceIndex = _dioController?.AllDevices.IndexOf(d);

                #region 撮影トリガ処理
                if (this.Config is ProjectConfiguration prjConfig)
                {
                    // 立ち上がり検知
                    if (true == risingEdgeIndex.Contains(prjConfig.DigitalIOConfiguration.AcquisitionTriggerInputIndex))
                    {
                        // 撮影トリガ対象デバイスの場合
                        if (true == prjConfig.DigitalIOConfiguration.AcquisitionTriggerDeviceIndex.Equals(deviceIndex))
                        {
                            this.ProcessAcquisitionTriggerSignal(true, currentDateTime, stopwatchFromOrigin);
                        }
                    }
                    // 立下り検知
                    else if (fallingEdgeIndex.Contains(prjConfig.DigitalIOConfiguration.AcquisitionTriggerInputIndex))
                    {
                        // 撮影トリガ対象デバイスの場合
                        if (true == prjConfig.DigitalIOConfiguration.AcquisitionTriggerDeviceIndex.Equals(deviceIndex))
                        {
                            this.ProcessAcquisitionTriggerSignal(false, currentDateTime, stopwatchFromOrigin);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// IDigitalIODeviceControllerイベント:デバイス無効
        /// </summary>
        private void DioController_Disabled(object sender, IDigitalIODevice device)
        {
            Serilog.Log.Error($"{this}, dio disabled");

            this.OnDeviceStatusChanged(device, DeviceKind.DigitalIO, device.Index, DeviceStatusKind.Disabled);
        }

        #endregion

        #region ILightControllerSupervisor

        /// <summary>
        /// ILightControllerSupervisorイベント:デバイス無効
        /// </summary>
        private void LightController_Disabled(object sender, Library.LightController.Device.ILightController device)
        {
            try
            {
                Serilog.Log.Error($"{this}, light{device.Location.Y + 1}{device.Location.X + 1} disabled");

                this.OnDeviceStatusChanged(device, DeviceKind.Light, device.Location.X, DeviceStatusKind.Disabled);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IInsightLinkageController

        /// <summary>
        /// IInsightLinkageControllerイベント:処理済:
        /// </summary>
        private void InsightLinkageController_Processed(object sender, Library.InsightLinkage.Data.IInsightLingageResult result)
        {
            try
            {
                if (true == result.IsSuccess)
                {
                    Serilog.Log.Information($"{this}, task no={result.Request.RequestId}, insight linkage processed ok");
                }
                else
                {
                    Serilog.Log.Error($"{this}, task no={result.Request.RequestId}, insight linkage processed ng");
                }

                if (result.Request.FileUploadRequest?.SourceFileInfo is FileInfo fileInfo)
                {
                    if (true == fileInfo.Exists)
                    {
                        fileInfo.Delete();
                        Serilog.Log.Debug($"{this}, task no={result.Request.RequestId}, upload file, deleted, {fileInfo.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}