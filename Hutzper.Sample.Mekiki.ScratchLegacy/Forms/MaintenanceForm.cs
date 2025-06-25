using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Controller.Plc;
using Hutzper.Library.Common.Diagnostics;
using Hutzper.Library.Common.Forms;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.DigitalIO;
using Hutzper.Library.DigitalIO.Device;
using Hutzper.Library.Forms;
using Hutzper.Library.Forms.ImageView;
using Hutzper.Library.ImageGrabber;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageGrabber.Device;
using Hutzper.Library.ImageGrabber.Device.File;
using Hutzper.Library.ImageGrabber.Device.GigE.Sentech;
using Hutzper.Library.ImageProcessing;
using Hutzper.Library.LightController;
using Hutzper.Library.LightController.Device;
using Hutzper.Sample.Mekiki.ScratchLegacy.Controller;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Diagnostics;
using Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber;
using Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Lighting;
using Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static Hutzper.Library.Forms.ImageView.ImageViewControl;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    /// <summary>
    /// メンテナンス画面
    /// </summary>
    public partial class MaintenanceForm : ServiceCollectionSharingForm
    {
        #region サブクラス

        /// <summary>
        /// テスト運転タスク
        /// </summary>
        private class TestTask
        {
            /// <summary>
            /// シーケンシャルなトリガ番号
            /// </summary>
            public readonly int TriggerNo;

            /// <summary>
            /// トリガONからの時間計測
            /// </summary>
            public readonly Stopwatch OriginWatch;

            /// <summary>
            /// 合計撮影回数/タスク
            /// </summary>
            /// <remarks>カメラ台数で初期化し、画像が得られるたびに減算して0になったら終了と判断します</remarks>
            public int TotalGrabbedCount;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public TestTask(int triggerNo, Stopwatch originWatch, int totalGrabbedCount)
            {
                this.TriggerNo = triggerNo;
                this.OriginWatch = originWatch;
                this.TotalGrabbedCount = totalGrabbedCount;
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// アプリ設定
        /// </summary>
        private readonly ProjectInspectionSetting AppConfig;

        /// <summary>
        /// 制御リスト
        /// </summary>
        private readonly List<IController> Controller = new();

        /// <summary>
        /// 全てのIGrabber
        /// </summary>
        /// <remarks>AreaSensor → LineSensorの順番で格納されます</remarks>
        public List<IGrabber> AllGrabbers { get; init; } = new();

        /// <summary>
        /// 画像取得制御
        /// </summary>
        private List<IGrabberController> GrabberControllers = new();

        /// <summary>
        /// 画像取得制御
        /// </summary>
        private readonly IAreaSensorController? AreaSensorController;

        /// <summary>
        /// ラインセンサ画像取得制御
        /// </summary>
        private ILineSensorController? LineSensorController;

        /// <summary>
        /// DIO制御
        /// </summary>
        private readonly IDigitalIODeviceController? DioController;

        /// <summary>
        /// DIO シミュレーションデバイス
        /// </summary>
        private IDigitalIODeviceInputSimulatable? TimingInjectedIO;

        /// <summary>
        /// 照明制御
        /// </summary>
        private readonly ILightControllerSupervisor? LightController;

        /// <summary>
        /// FPS算出
        /// </summary>
        private FpsCalculator[] FpsCalculator = Array.Empty<FpsCalculator>();

        /// <summary>
        /// Formヘルパ
        /// </summary>
        private readonly FormsHelper FormsHelper;

        /// <summary>
        /// テスト運転中かどうか
        /// </summary>
        private bool IsTestRunning;

        /// <summary>
        /// テスト運転の信号カウント
        /// </summary>
        private int TestRunningTriggerCount;

        /// <summary>
        /// ヒストグラム更新セマフォ
        /// </summary>
        private SemaphoreSlim SemaphoreHistgram = new(1, 1);

        /// <summary>
        /// カメラ制御セマフォ
        /// </summary>
        private Dictionary<IGrabber, SemaphoreSlim> SemaphoreGrabber = new();

        /// <summary>
        /// 照明制御セマフォ
        /// </summary>
        private Dictionary<ILightController, SemaphoreSlim> SemaphoreLighting = new();

        /// <summary>
        /// カメラ設定画面
        /// </summary>
        private IGrabberParameterForm? GrabberParameterForm;

        /// <summary>
        /// 照明設定画面
        /// </summary>
        private ILightParameterForm? LightParameterForm;

        /// <summary>
        /// ラインセンサFFC設定画面
        /// </summary>
        private ILineSensorFFC_Form? LineSensorFFC_Form;

        /// <summary>
        /// テスト運転タスク
        /// </summary>
        private List<TestTask> TestTaskList = new();
        private object TestTaskSync = new();

        /// <summary>
        /// PLC通信
        /// </summary>
        private IPlcTcpCommunicator? PlcTcpCommunicator;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MaintenanceForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MaintenanceForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            InitializeComponent();

            // エラー
            if (null == this.Services)
            {
                throw new Exception("services is null");
            }

            this.FormsHelper = new FormsHelper(this, this.Services);

            // 設定ファイルロード
            var appConfigFile = ApplicationConfig.GetStandardConfigFileInfo();
            this.AppConfig = new ProjectInspectionSetting(this.Services, appConfigFile.FullName);
            this.AppConfig.Load();

            // エリアカメラ画像取得制御
            this.AreaSensorController = this.Services?.ServiceProvider?.GetRequiredService<IAreaSensorController>();
            if (this.AreaSensorController is not null)
            {
                this.AreaSensorController.DataGrabbed += this.GrabberController_DataGrabbed;
                this.AreaSensorController.GrabberDisabled += this.GrabberController_GrabberDisabled;

                if (0 < this.AreaSensorController.NumberOfGrabber)
                {
                    this.GrabberControllers.Add(this.AreaSensorController);
                }
            }

            // ラインセンサ画像取得制御
            this.LineSensorController = this.Services?.ServiceProvider?.GetRequiredService<ILineSensorController>();
            if (this.LineSensorController is not null)
            {
                this.LineSensorController.DataGrabbed += this.GrabberController_DataGrabbed;
                this.LineSensorController.GrabberDisabled += this.GrabberController_GrabberDisabled;

                if (0 < this.LineSensorController.NumberOfGrabber)
                {
                    this.GrabberControllers.Add(this.LineSensorController);
                }
            }

            // 画像取得制御が存在する場合
            if (0 < this.GrabberControllers.Count)
            {
                // 制御リストに追加する
                this.Controller.AddRange(this.GrabberControllers);

                foreach (var ctrl in this.GrabberControllers)
                {
                    this.AllGrabbers.AddRange(ctrl.Grabbers);
                }
            }

            // DIO制御
            this.DioController = this.Services?.ServiceProvider?.GetRequiredService<IDigitalIODeviceController>();
            if (this.DioController is not null)
            {
                this.DioController.InputChanged += this.DioController_InputChanged;
                this.DioController.Disabled += this.DioController_Disabled;

                this.Controller.Add(this.DioController);
            }
            else
            {
                this.onOffUcTestRunning.Enabled = false;
            }

            // 照明制御
            this.LightController = this.Services?.ServiceProvider?.GetRequiredService<ILightControllerSupervisor>();
            if (this.LightController is not null)
            {
                this.LightController.Disabled += this.LightController_Disabled;

                this.Controller.Add(this.LightController);
            }
            else
            {
                this.buttonLightParameter.Enabled = false;
            }

            // PLC通信
            if (false == string.IsNullOrEmpty(this.AppConfig.PlcTcpCommunicatorParameter?.IpAddress) && 0 < this.AppConfig.PlcTcpCommunicatorParameter.PortNumber)
            {
                this.PlcTcpCommunicator = this.Services?.ServiceProvider?.GetRequiredService<IPlcTcpCommunicator>();
            }
            if (this.PlcTcpCommunicator is not null)
            {
                this.PlcTcpCommunicator.Connected += this.PlcTcpCommunicator_Connected;
                this.PlcTcpCommunicator.Disconnected += this.PlcTcpCommunicator_Disconnected;
                this.PlcTcpCommunicator.ProcessingStatusChanged += this.PlcTcpCommunicator_ProcessingStatusChanged;

                this.Controller.Add(this.PlcTcpCommunicator);
            }

            // コントローラーの初期化
            foreach (var c in this.Controller)
            {
                c.Initialize(this.Services);
                c.SetConfig(this.AppConfig);
            }

            // 撮影制御
            this.AreaSensorController?.SetParameter(this.AppConfig.AreaSensorControllerParameter);
            this.LineSensorController?.SetParameter(this.AppConfig.LineSensorControllerParameter);

            // DIO制御
            this.DioController?.SetParameter(this.AppConfig.DigitalIODeviceControllerParameter);

            // 照明制御
            this.LightController?.SetParameter(this.AppConfig.LightControllerParameter);

            // PLC通信
            this.PlcTcpCommunicator?.SetParameter(this.AppConfig.PlcTcpCommunicatorParameter);

            // 画像保存ダイアログ設定
            {
                this.saveImageFileDialog.InitialDirectory = this.AppConfig.Path.Temp.FullName;
                this.saveImageFileDialog.Filter = "Image Files(*.PNG;*.BMP;*.JPG;*.GIF)|*.PNG;*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
                this.saveImageFileDialog.DefaultExt = ".png";
            }

            // テスト運転用セマフォ
            {
                foreach (var grabber in this.AllGrabbers)
                {
                    this.SemaphoreGrabber.Add(grabber, new SemaphoreSlim(1, 1));
                }

                foreach (var light in this.LightController?.Devices ?? new List<ILightController>())
                {
                    this.SemaphoreLighting.Add(light, new SemaphoreSlim(1, 1));
                }
            }

            // 機器状態表示ユーザーコントロール
            this.ucMultiDeviceStatus.Initialize();
            this.ucMultiDeviceStatus.AddDevice(DeviceKind.DigitalIO, new MonitoredDeviceInfoUnit("トリガ", 0, Math.Max(1, this.DioController?.AllDevices.Count ?? 0)));
            this.ucMultiDeviceStatus.AddDevice(DeviceKind.Camera, new MonitoredDeviceInfoUnit("カメラ", 0, Math.Max(1, this.AllGrabbers.Count)));
            this.ucMultiDeviceStatus.AddDevice(DeviceKind.Light, new MonitoredDeviceInfoUnit("照明", 0, Math.Max(1, this.LightController?.NumberOfDevice ?? 0)));
            if (this.PlcTcpCommunicator is not null)
            {
                this.ucMultiDeviceStatus.AddDevice(DeviceKind.Plc, new MonitoredDeviceInfoUnit("PLC", 0, 1));
            }

            // FPS計算
            this.FpsCalculator = new FpsCalculator[this.AllGrabbers.Count];
            foreach (var i in Enumerable.Range(0, this.FpsCalculator.Length))
            {
                this.FpsCalculator[i] = new FpsCalculator();
            }

            // マルチ画像表示ユーザーコントロール
            this.UcMultiImageView.LayoutType = MultiImageLayoutType.Spotlight;
            this.UcMultiImageView.ImageUpdated += this.UcMultiImageView_ImageUpdated;
            this.UcMultiImageView.SelectedIndexChanged += this.UcMultiImageView_SelectedIndexChanged;

            foreach (var i in Enumerable.Range(0, this.AllGrabbers.Count))
            {
                // プロパティを設定
                var viewItem = new ImageViewControl()
                {
                    ImageDescription = $"カメラ{i + 1}",    // 画像の表示名
                    DisplayImageDescription = true,         // 画像の説明を表示するかどうか
                    DisplayPixelInfoEnabled = false,        // ピクセル情報を表示するかどうか
                    DisplayFpsEnabled = false,              // FPSを表示するかどうか
                    UseMeasurement = false,                 // 計測機能を使用するかどうか
                    AcceptImageFileDrop = true,             // 画像ファイルのドロップを受け付けるかどうか
                    PseudoColorEnabled = false,             // 疑似カラー表示を有効にするかどうか
                    ResolutionMmPerPixel = 0.1,               // 1ピクセルあたりの解像度(mm)
                };

                // コントロールを追加
                this.UcMultiImageView.Add(viewItem);
            }

            // UI設定値を反映
            foreach (var control in this.UcMultiImageView)
            {
                control.CenterLines.Enabled = this.onOffUcViewCenter.Value;
                control.HProjection.Enabled = this.onOffUcViewProjection.Value;
                control.PseudoColorEnabled = this.onOffUcViewPseudo.Value;
                control.UseMeasurement = this.onOffUcDistanceMeasurement.Value;

                if (0 == control.Index)
                {
                    control.DisplayPixelInfoEnabled = true;
                    control.DisplayFpsEnabled = true;
                }
            }
            this.histoUc.PseudColorEnabled = this.onOffUcViewPseudo.Value;
        }

        #endregion

        #region IGrabberController

        /// <summary>
        /// 画像取得
        /// </summary>
        /// <param name="data">取得画像データ</param>
        private void GrabberController_DataGrabbed(object sender, IGrabberData data)
        {
            try
            {
                var grabberIndex = this.getGrabberCameraIndex(data.Location);

                if (this.UcMultiImageView.NumberOfImages <= grabberIndex)
                {
                    Serilog.Log.Warning($"{this}, invalid grabber index");
                    return;
                }

                // テストタスクの完了確認
                this.checkTestTaskCompletion(grabberIndex);

                // FPS算出
                if (true == this.FpsCalculator[grabberIndex].AddFrame())
                {
                    this.UcMultiImageView[grabberIndex].CurrentFps = this.FpsCalculator[grabberIndex].Result;
                }

                // Bitmapに変換する
                using var bitmap = this.convertGrabberData(data, out var imgData);

                // 保持している最新画像を更新する
                if (bitmap is not null)
                {
                    this.UcMultiImageView[grabberIndex].SetImage(bitmap, imgData);
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
        private void GrabberController_GrabberDisabled(object sender, IGrabber g)
        {
            Serilog.Log.Warning($"{this}, {this.ToString(g)} disabled");

            var grabberIndex = this.getGrabberCameraIndex(g.Location);
            this.ucMultiDeviceStatus.Deviecs[DeviceKind.Camera].Items[grabberIndex].Status = DeviceStatusKind.Disabled;
        }

        #endregion

        #region IDigitalIODeviceController

        /// <summary>
        /// IDigitalIODeviceControllerイベント:入力信号変化
        /// </summary>
        private void DioController_InputChanged(object sender, IDigitalIOInputDevice d, int[] risingEdgeIndex, int[] fallingEdgeIndex, bool[] values)
        {
            try
            {
                // 時間計測を開始する
                var taskWatch = Stopwatch.StartNew();

                // DIOデバイスインデックスを取得する
                var deviceIndex = this.DioController?.AllDevices.IndexOf(d) ?? 0;

                // 各パラメータへの参照を取得する
                var ip = this.AppConfig.InspectionControllerParameter!;
                var lp = this.AppConfig.LightControllerParameter!;
                var dp = this.AppConfig.DigitalIOConfiguration;

                // 撮影トリガ対象デバイス以外の場合
                if (false == dp.AcquisitionTriggerDeviceIndex.Equals(deviceIndex))
                {
                    return;
                }

                #region 撮影トリガ処理

                // 立ち上がり検知
                if (true == risingEdgeIndex.Contains(dp.AcquisitionTriggerInputIndex))
                {
                    // テスト運転中の場合
                    if (true == this.IsTestRunning)
                    {
                        Serilog.Log.Information($"{this}, No.{++this.TestRunningTriggerCount:D4} trigger on. accepted");

                        var currentNo = this.TestRunningTriggerCount;

                        lock (this.TestTaskSync)
                        {
                            this.TestTaskList.Add(new TestTask(currentNo, taskWatch, this.AllGrabbers.Count));
                        }

                        #region 照明
                        if (this.LightController is ILightControllerSupervisor lightSupervisor)
                        {
                            Parallel.ForEach(lightSupervisor.Devices, device =>
                            {
                                var index = lightSupervisor.Devices.IndexOf(device);
                                var param = lp.DeviceParameters[index];
                                var semaphore = this.SemaphoreLighting[device];

                                if (param.OnOffControlType is not LightingOnOffControlType.OneByOne)
                                {
                                    return;
                                }

                                #region ON制御
                                if (ip.LightTurnOnTiming is LightControlTiming.Fastest)
                                {
                                    if (true == semaphore.Wait(0))
                                    {
                                        Serilog.Log.Information($"{this}, No.{currentNo:D4}, turn on {this.ToString(device)}, delay = fastest");
                                        device.TurnOn();
                                        semaphore.Release();
                                    }
                                    else
                                    {
                                        Serilog.Log.Warning($"{this}, No.{currentNo:D4}, turn on overlapped {this.ToString(device)}, delay = fastest");
                                    }
                                }
                                else if (ip.LightTurnOnTiming is LightControlTiming.FixedDelayFromSignalOn)
                                {
                                    Task.Run(async () =>
                                    {
                                        var waitTimeMs = ip.LightTurnOnDelayMs[index] - (int)taskWatch.ElapsedMilliseconds;

                                        if (0 < waitTimeMs)
                                        {
                                            await Task.Delay(waitTimeMs);
                                        }

                                        if (true == semaphore.Wait(0))
                                        {
                                            Serilog.Log.Information($"{this}, No.{currentNo:D4}, turn on {this.ToString(device)}, delay = {ip.LightTurnOnDelayMs[index]}");
                                            device.TurnOn();
                                            await Task.Delay(100);
                                            semaphore.Release();
                                        }
                                        else
                                        {
                                            Serilog.Log.Warning($"{this}, No.{currentNo:D4}, turn on overlapped {this.ToString(device)}, delay = {ip.LightTurnOnDelayMs[index]}");
                                        }
                                    });
                                }
                                #endregion

                                #region OFF制御
                                if (ip.LightTurnOffTiming is LightControlTiming.FixedDelayFromSignalOn)
                                {
                                    Task.Run(async () =>
                                    {
                                        var waitTimeMs = ip.LightTurnOffDelayMs[index] - (int)taskWatch.ElapsedMilliseconds;

                                        if (0 < waitTimeMs)
                                        {
                                            await Task.Delay(waitTimeMs);
                                        }

                                        if (true == semaphore.Wait(0))
                                        {
                                            Serilog.Log.Information($"{this}, No.{currentNo:D4}, turn off {this.ToString(device)}, delay = {ip.LightTurnOffDelayMs[index]}");
                                            device.TurnOff();
                                            await Task.Delay(100);
                                            semaphore.Release();
                                        }
                                        else
                                        {
                                            Serilog.Log.Warning($"{this}, No.{currentNo:D4}, turn off overlapped {this.ToString(device)}, delay = {ip.LightTurnOffDelayMs[index]}");
                                        }
                                    });
                                }
                                #endregion
                            });
                        }
                        #endregion

                        #region 撮影
                        Parallel.ForEach(this.AllGrabbers, grabber =>
                        {
                            var index = this.AllGrabbers.IndexOf(grabber);
                            var semaphore = this.SemaphoreGrabber[grabber];

                            // 即時撮影の場合
                            if (0 >= ip.AcquisitionTriggerDelayMs[index])
                            {
                                if (true == semaphore.Wait(0))
                                {
                                    Serilog.Log.Information($"{this}, No.{currentNo:D4}, acquisition {this.ToString(grabber)}, delay={ip.AcquisitionTriggerDelayMs[index]}");
                                    grabber.DoSoftTrigger();
                                }
                                else
                                {
                                    Serilog.Log.Warning($"{this}, No.{currentNo:D4}, overlapped {this.ToString(grabber)}, delay={ip.AcquisitionTriggerDelayMs[index]}");
                                }
                            }
                            // 遅延撮影の場合
                            else
                            {
                                Task.Run(async () =>
                                {
                                    var waitTimeMs = ip.AcquisitionTriggerDelayMs[index] - (int)taskWatch.ElapsedMilliseconds;

                                    if (0 < waitTimeMs)
                                    {
                                        await Task.Delay(waitTimeMs);
                                    }

                                    if (true == semaphore.Wait(0))
                                    {
                                        Serilog.Log.Information($"{this}, No.{currentNo:D4}, acquisition {this.ToString(grabber)}, delay={ip.AcquisitionTriggerDelayMs[index]}");
                                        grabber.DoSoftTrigger();
                                    }
                                    else
                                    {
                                        Serilog.Log.Warning($"{this}, No.{currentNo:D4}, overlapped {this.ToString(grabber)}, delay={ip.AcquisitionTriggerDelayMs[index]}");
                                    }
                                });
                            }
                        });
                        #endregion
                    }
                    // テスト運転中以外
                    else
                    {
                        Serilog.Log.Information($"{this}, trigger on. ignored");
                    }

                    this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items[deviceIndex].Status = DeviceStatusKind.On;
                }
                // 立下り検知
                else if (fallingEdgeIndex.Contains(dp.AcquisitionTriggerInputIndex))
                {
                    // テスト運転中の場合
                    if (this.IsTestRunning && 0 < this.TestRunningTriggerCount)
                    {
                        var currentNo = this.TestRunningTriggerCount;
                        Serilog.Log.Information($"{this}, No.{currentNo:D4} trigger off. accepted");

                        #region 照明
                        if (this.LightController is ILightControllerSupervisor lightSupervisor)
                        {
                            Parallel.ForEach(lightSupervisor.Devices, device =>
                            {
                                var index = lightSupervisor.Devices.IndexOf(device);
                                var param = lp.DeviceParameters[index];
                                var semaphore = this.SemaphoreLighting[device];

                                if (param.OnOffControlType is not LightingOnOffControlType.OneByOne)
                                {
                                    return;
                                }

                                #region OFF制御
                                if (ip.LightTurnOffTiming is LightControlTiming.FixedDelayFromSignalOff)
                                {
                                    Task.Run(async () =>
                                    {
                                        var waitTimeMs = ip.LightTurnOffDelayMs[index] - (int)taskWatch.ElapsedMilliseconds;

                                        if (0 < waitTimeMs)
                                        {
                                            await Task.Delay(waitTimeMs);
                                        }

                                        if (true == semaphore.Wait(0))
                                        {
                                            Serilog.Log.Information($"{this}, No.{currentNo:D4}, turn off {this.ToString(device)}, delay = {ip.LightTurnOffDelayMs[index]}");
                                            device.TurnOff();
                                            await Task.Delay(100);
                                            semaphore.Release();
                                        }
                                        else
                                        {
                                            Serilog.Log.Warning($"{this}, No.{currentNo:D4}, turn off overlapped {this.ToString(device)}, delay = {ip.LightTurnOffDelayMs[index]}");
                                        }
                                    });
                                }
                                #endregion

                                #region ON制御
                                if (ip.LightTurnOnTiming is LightControlTiming.FixedDelayFromSignalOff)
                                {
                                    Task.Run(async () =>
                                    {
                                        var waitTimeMs = ip.LightTurnOnDelayMs[index] - (int)taskWatch.ElapsedMilliseconds;

                                        if (0 < waitTimeMs)
                                        {
                                            await Task.Delay(waitTimeMs);
                                        }

                                        if (true == semaphore.Wait(0))
                                        {
                                            Serilog.Log.Information($"{this}, No.{currentNo:D4}, turn on {this.ToString(device)}, delay = {ip.LightTurnOnDelayMs[index]}");
                                            device.TurnOn();
                                            await Task.Delay(100);
                                            semaphore.Release();
                                        }
                                        else
                                        {
                                            Serilog.Log.Warning($"{this}, No.{currentNo:D4}, turn on overlapped {this.ToString(device)}, delay = {ip.LightTurnOnDelayMs[index]}");
                                        }
                                    });
                                }
                                #endregion
                            });
                        }
                        #endregion
                    }
                    // テスト運転中以外
                    else
                    {
                        Serilog.Log.Information($"{this}, trigger off. ignored");
                    }

                    this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items[deviceIndex].Status = DeviceStatusKind.Off;
                }

                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// IDigitalIODeviceControllerイベント:無効化された
        /// </summary>
        private void DioController_Disabled(object sender, IDigitalIODevice device)
        {
            Serilog.Log.Warning($"{this}, dio disabled");

            this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items[device.Index].Status = DeviceStatusKind.Disabled;
        }

        #endregion

        #region ILightControllerSupervisor

        /// <summary>
        /// ILightControllerSupervisorイベント:無効化された
        /// </summary>
        private void LightController_Disabled(object sender, ILightController device)
        {
            Serilog.Log.Warning($"{this}, {this.ToString(device)} disabled");

            this.ucMultiDeviceStatus.Deviecs[DeviceKind.Light].Items[device.Index].Status = DeviceStatusKind.Disabled;
        }

        #endregion

        #region IPlcTcpCommunicator

        /// <summary>
        /// IPlcTcpCommunicatorイベント:接続
        /// </summary>
        private void PlcTcpCommunicator_Disconnected(object sender)
        {
            Serilog.Log.Warning($"{this}, plc disconnected");

            this.ucMultiDeviceStatus.Deviecs[DeviceKind.Plc].Items[0].Status = DeviceStatusKind.Disabled;
        }

        /// <summary>
        /// IPlcTcpCommunicatorイベント:切断
        /// </summary>
        private void PlcTcpCommunicator_Connected(object sender)
        {
            Serilog.Log.Information($"{this}, plc connected");

            this.ucMultiDeviceStatus.Deviecs[DeviceKind.Plc].Items[0].Status = DeviceStatusKind.Enabled;
        }

        /// <summary>
        /// IPlcTcpCommunicatorイベント:処理状態変化
        /// </summary>
        private void PlcTcpCommunicator_ProcessingStatusChanged(object sender, bool status, string errorMessage)
        {
            if (false == status)
            {
                Serilog.Log.Warning($"{this}, plc status changed to abnormal. {errorMessage}");
                this.ucMultiDeviceStatus.Deviecs[DeviceKind.Plc].Items[0].Status = DeviceStatusKind.Warning;
            }
            else if (this.PlcTcpCommunicator?.Enabled ?? false)
            {
                Serilog.Log.Information($"{this}, plc status recovered to normal. {errorMessage}");
                this.ucMultiDeviceStatus.Deviecs[DeviceKind.Plc].Items[0].Status = DeviceStatusKind.Enabled;
            }
            else
            {
                this.ucMultiDeviceStatus.Deviecs[DeviceKind.Plc].Items[0].Status = DeviceStatusKind.Disabled;
            }
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// Formイベント:Shown
        /// </summary>
        private void MaintenanceForm_Shown(object sender, EventArgs e)
        {
            try
            {
                using var marqueue = ProgressBarForm.NewMarqueeType(this, "起動中", $"機器に接続しています。{Environment.NewLine}しばらくお待ちください。");

                var formBlockingAccessor = new FormBlockingAccessor(marqueue);

                #region 機器への接続
                Task.Run(async () =>
                {
                    try
                    {
                        // PLC通信
                        if (this.PlcTcpCommunicator is not null)
                        {
                            this.PlcTcpCommunicator.Open();

                            var plcWatch = Stopwatch.StartNew();
                            do
                            {
                                await Task.Delay(100);
                            }
                            while (false == this.PlcTcpCommunicator.Enabled || 3000 > plcWatch.ElapsedMilliseconds);

                            if (false == this.PlcTcpCommunicator.IsProcessingCorrectly)
                            {
                                this.ucMultiDeviceStatus.Deviecs[DeviceKind.Plc].Items[0].Status = DeviceStatusKind.Disabled;
                            }
                        }

                        #region DIO接続
                        if (this.DioController is not null)
                        {
                            if (true == this.DioController.Open())
                            {
                                if (0 < this.DioController.AllDevices.Count)
                                {
                                    Serilog.Log.Information($"{this}, dio connection success");

                                    foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items)
                                    {
                                        d.Status = DeviceStatusKind.Enabled;
                                    }

                                    foreach (var device in this.DioController.AllDevices)
                                    {
                                        if (device is IDigitalIODeviceInputSimulatable timingInjectedIO)
                                        {
                                            this.TimingInjectedIO = timingInjectedIO;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items)
                                    {
                                        d.Status = DeviceStatusKind.Nonuse;
                                    }
                                }
                            }
                            else
                            {
                                Serilog.Log.Warning($"{this}, dio connection failed");

                                foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items)
                                {
                                    var index = this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items.IndexOf(d);

                                    d.Status = this.DioController.AllDevices[index].Enabled ? DeviceStatusKind.Enabled : DeviceStatusKind.Disabled;
                                }
                            }
                        }
                        #endregion

                        #region 照明接続
                        if (this.LightController is not null)
                        {
                            if (true == this.LightController.Open())
                            {
                                if (0 < this.LightController.Devices.Count)
                                {
                                    Serilog.Log.Information($"{this}, light connection success");
                                    foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.Light].Items)
                                    {
                                        d.Status = DeviceStatusKind.Enabled;
                                    }

                                    this.LightController.TurnOff();
                                }
                                else
                                {
                                    foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.Light].Items)
                                    {
                                        d.Status = DeviceStatusKind.Nonuse;
                                    }
                                }
                            }
                            else
                            {
                                Serilog.Log.Warning($"{this}, light connection failed");

                                foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.Light].Items)
                                {
                                    var index = this.ucMultiDeviceStatus.Deviecs[DeviceKind.Light].Items.IndexOf(d);

                                    d.Status = this.LightController.Devices[index].Enabled ? DeviceStatusKind.Enabled : DeviceStatusKind.Disabled;
                                }
                            }
                        }
                        #endregion

                        #region カメラ接続                        
                        {
                            var grabberControllers = new[] { (IGrabberController?)this.AreaSensorController, (IGrabberController?)this.LineSensorController };
                            var parallelOptions = new ParallelOptions();
#if DEBUG
                            parallelOptions.MaxDegreeOfParallelism = 1;
#endif

                            var grabberResult = Enumerable.Repeat(true, grabberControllers.Length).ToArray();
                            Parallel.ForEach(grabberControllers, parallelOptions, controller =>
                            {
                                grabberResult[Array.IndexOf(grabberControllers, controller)] = controller?.Open() ?? false;
                            });

                            if (true == grabberResult.All(r => r))
                            {
                                Serilog.Log.Information($"{this}, camera connection success");

                                foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.Camera].Items)
                                {
                                    d.Status = DeviceStatusKind.Enabled;
                                }
                            }
                            else if (0 < this.AllGrabbers.Count)
                            {
                                Serilog.Log.Warning($"{this}, camera connection failed");

                                foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.Camera].Items)
                                {
                                    var index = this.ucMultiDeviceStatus.Deviecs[DeviceKind.Camera].Items.IndexOf(d);

                                    d.Status = this.AllGrabbers[index].Enabled ? DeviceStatusKind.Enabled : DeviceStatusKind.Disabled;
                                }
                            }
                            else
                            {
                                foreach (var d in this.ucMultiDeviceStatus.Deviecs[DeviceKind.Camera].Items)
                                {
                                    d.Status = DeviceStatusKind.Nonuse;
                                }
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        formBlockingAccessor.Set(DialogResult.OK);
                    }
                });
                #endregion

                formBlockingAccessor.ShowBlocking();

                #region カメラ設定画面
                {
                    // カメラパラメータ設定画面の生成
                    this.GrabberParameterForm = this.Services?.ServiceProvider?.GetRequiredService<IGrabberParameterForm>();

                    if (this.GrabberParameterForm is not null && this.GrabberParameterForm.GetForm() is Form paramForm)
                    {
                        paramForm.StartPosition = FormStartPosition.Manual;
                        this.GrabberParameterForm.ParameterChanged += this.GrabberParameterForm_ParameterChanged;

                        var parameterSets = new List<GrabberParameterSet>();

                        // エリアカメラ
                        if (this.AreaSensorController is not null && this.AppConfig.AreaSensorControllerParameter is not null)
                        {
                            foreach (var grabber in this.AreaSensorController.Grabbers)
                            {
                                var indexLocal = this.AreaSensorController.Grabbers.IndexOf(grabber);
                                var indexGlobal = this.getGrabberCameraIndex(grabber.Location);

                                //IGrabberParaeter
                                var gp = this.AppConfig.AreaSensorControllerParameter.GrabberParameters[indexLocal];

                                // IInspectionControllerParameter
                                var op = new Grabber.BehaviorOptions();
                                if ((this.AppConfig.InspectionControllerParameter?.AcquisitionTriggerDelayMs.Length ?? 0) > indexGlobal)
                                {
                                    op.AcquisitionTriggerDelayMs = this.AppConfig.InspectionControllerParameter?.AcquisitionTriggerDelayMs[indexGlobal] ?? 1;
                                }

                                // ImagingConfiguration
                                var ip = this.AppConfig.ImagingConfiguration.GetOrAdd(grabber.Location);

                                var item = new GrabberParameterSet(grabber, gp, ip, op, $"エリアカメラ{indexLocal + 1:D2}");
                                parameterSets.Add(item);
                            }
                        }

                        // ラインセンサ
                        if (this.LineSensorController is not null && this.AppConfig.LineSensorControllerParameter is not null)
                        {
                            foreach (var grabber in this.LineSensorController.Grabbers)
                            {
                                var indexLocal = this.LineSensorController.Grabbers.IndexOf(grabber);
                                var indexGlobal = this.getGrabberCameraIndex(grabber.Location);

                                // IGrabberParaeter
                                var gp = this.AppConfig.LineSensorControllerParameter.GrabberParameters[indexLocal];

                                // IInspectionControllerParameter
                                var op = new Grabber.BehaviorOptions();
                                if ((this.AppConfig.InspectionControllerParameter?.AcquisitionTriggerDelayMs.Length ?? 0) > indexGlobal)
                                {
                                    op.AcquisitionTriggerDelayMs = this.AppConfig.InspectionControllerParameter?.AcquisitionTriggerDelayMs[indexGlobal] ?? 1;
                                }

                                // ImagingConfiguration
                                var ip = this.AppConfig.ImagingConfiguration.GetOrAdd(grabber.Location);

                                var item = new GrabberParameterSet(grabber, gp, ip, op, $"ラインセンサ{indexLocal + 1:D2}");
                                parameterSets.Add(item);
                            }
                        }

                        // 初期化
                        this.GrabberParameterForm.Initialize(parameterSets.ToArray());
                    }
                }
                #endregion

                #region カメラ選択
                if (this.GrabberParameterForm is not null)
                {
                    var parameterSets = this.GrabberParameterForm.GetParameterSet();

                    // カメラ選択リストへの追加                    
                    foreach (var parameter in parameterSets)
                    {
                        var grabberIndex = parameterSets.IndexOf(parameter);
                        this.UcCameraSelection.Items.Add(parameter.Nickname);
                        this.UcMultiImageView[grabberIndex].ImageDescription = parameter.Nickname;
                    }

                    // カメラ選択リストの選択変更イベント
                    this.UcCameraSelection.SelectedIndexChanged += (sender, index, name) =>
                    {
                        // 選択されたカメラ設定に切り替える
                        this.GrabberParameterForm.ChangeView(index);

                        // 選択されたカメラ画像表示に切り替える
                        this.UcMultiImageView.SelectedIndex = index;
                    };

                    // カメラ選択リストの初期選択
                    if (0 < this.UcCameraSelection.Items.Count)
                    {
                        this.UcCameraSelection.SelectedIndex = 0;
                    }
                }
                #endregion

                #region 照明設定画面
                {
                    // パラメータ設定画面の生成
                    this.LightParameterForm = this.Services?.ServiceProvider?.GetRequiredService<ILightParameterForm>();

                    if (this.LightParameterForm is not null && this.LightParameterForm.GetForm() is Form paramForm)
                    {
                        paramForm.StartPosition = FormStartPosition.Manual;
                        this.LightParameterForm.ParameterChanged += this.LightParameterForm_ParameterChanged;

                        var parameterSets = new List<LightParameterSet>();

                        if (this.LightController is not null && this.AppConfig.LightControllerParameter is not null)
                        {
                            foreach (var device in this.LightController.Devices)
                            {
                                var index = this.LightController.Devices.IndexOf(device);

                                //ILightParaeter
                                var lp = this.AppConfig.LightControllerParameter.DeviceParameters[index];

                                // IInspectionControllerParameter
                                var op = new Lighting.BehaviorOptions();
                                op.LightTurnOnTiming = this.AppConfig.InspectionControllerParameter?.LightTurnOnTiming ?? LightControlTiming.Fastest;
                                op.LightTurnOffTiming = this.AppConfig.InspectionControllerParameter?.LightTurnOffTiming ?? LightControlTiming.Fastest;

                                if ((this.AppConfig.InspectionControllerParameter?.LightTurnOnDelayMs.Length ?? 0) > index)
                                {
                                    op.LightTurnOnDelayMs = this.AppConfig.InspectionControllerParameter?.LightTurnOnDelayMs[index] ?? 0;
                                }

                                if ((this.AppConfig.InspectionControllerParameter?.LightTurnOffDelayMs.Length ?? 0) > index)
                                {
                                    op.LightTurnOffDelayMs = this.AppConfig.InspectionControllerParameter?.LightTurnOffDelayMs[index] ?? 0;
                                }

                                var item = new LightParameterSet(device, lp, op, $"照明{index + 1:D2}");
                                parameterSets.Add(item);
                            }
                        }

                        // 初期化
                        this.LightParameterForm.Initialize(parameterSets.ToArray());
                    }
                }
                #endregion

                #region ラインセンサFFC設定画面
                if (this.LineSensorController is not null && this.AppConfig.LineSensorControllerParameter is not null)
                {
                    // ラインセンサが存在する場合のみ、FFC設定画面を生成
                    foreach (var grabber in this.LineSensorController.Grabbers)
                    {
                        this.LineSensorFFC_Form = this.Services?.ServiceProvider?.GetRequiredService<GigESentechLineSensorFFC_Form>();
                        break;
                    }
                }
                if (this.LineSensorFFC_Form is not null)
                {
                    this.LineSensorFFC_Form.GrabberStatusRequested += this.LineSensorFFC_Form_GrabberStatusRequested;
                    this.buttonLineSensorCorrection.Visible = true;
                }
                else
                {
                    this.buttonLineSensorCorrection.Visible = false;
                }
                #endregion

                // マニュアルトリガボタンの表示
                this.buttonManualTrigger.Visible = this.TimingInjectedIO is not null;

                // 入力信号監視を開始                
                this.DioController?.StartMonitoring();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 終了ボタンクリック
        /// </summary>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            try
            {
                // 停止時のみ保存を許可する
                if (true == this.onOffUcCameraLive.Value || true == this.onOffUcTestRunning.Value)
                {
                    using var confirmDialog = new ConfirmationForm();
                    confirmDialog.ShowError(this, MessageBoxButtons.OK, "終了確認", "撮影を停止してから終了してください。");

                    return;
                }

                this.updateConfiguration();

                // 設定ファイルロード
                var appConfigFile = ApplicationConfig.GetStandardConfigFileInfo();
                var previousSetting = new ProjectInspectionSetting(this.Services, appConfigFile.FullName);
                previousSetting.Load();

                // 変更がある場合
                if (false == this.AppConfig.Equals(previousSetting))
                {
                    using var confirmDialog = this.FormsHelper.NewConfirmationForm();

                    var dialogResult = confirmDialog.ShowQuestion(this, MessageBoxButtons.YesNoCancel, "終了確認", "変更があります。\n保存してから終了しますか?");

                    if (DialogResult.Cancel == dialogResult)
                    {
                        return;
                    }
                    else
                    {
                        if (DialogResult.Yes == dialogResult)
                        {
                            this.saveConfiguration();
                        }

                        this.closeProcessing();
                        this.Close();
                    }
                }
                else
                {
                    this.closeProcessing();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ライブ撮影ボタン
        /// </summary>
        private void onOffUcLive_ValueChanged(object sender)
        {
            try
            {
                // 排他利用コントロール
                var exclusiveControls = new List<Control>(new Control[] { this.onOffUcTestRunning });

                // ライブ撮影開始
                if (true == this.onOffUcCameraLive.Value)
                {
                    // 設定値の更新
                    this.updateConfiguration();

                    // FPS計算を初期化
                    foreach (var calc in this.FpsCalculator)
                    {
                        calc.Reset();
                    }

                    // 照明点灯ON
                    if (this.LightController is ILightControllerSupervisor lightSupervisor && this.AppConfig.LightControllerParameter is ILightControllerSupervisorParameter lightParam)
                    {
                        Parallel.ForEach(lightSupervisor.Devices, device =>
                        {
                            var index = lightSupervisor.Devices.IndexOf(device);
                            var param = lightParam.DeviceParameters[index];
                            var semaphore = this.SemaphoreLighting[device];

                            if (param.OnOffControlType is LightingOnOffControlType.Continuous or LightingOnOffControlType.OneByOne)
                            {
                                if (true == semaphore.Wait(0))
                                {
                                    Serilog.Log.Information($"{this}, turn on {this.ToString(device)}, Continuous / OneByOne");
                                    device.TurnOn();
                                    semaphore.Release();
                                }
                                else
                                {
                                    Serilog.Log.Warning($"{this}, turn on overlapped {this.ToString(device)}, Continuous / OneByOne");
                                }
                            }
                        });
                    }

                    #region 撮影停止中のみ変更可能な設定を更新する
                    Parallel.ForEach(this.GrabberControllers, controller =>
                    {
                        if (controller is ILineSensorController lineController && this.AppConfig.LineSensorControllerParameter is ILineSensorControllerParameter lineParameter)
                        {
                            foreach (ILineSensor device in lineController.Grabbers)
                            {
                                var deviceIndex = lineController.Grabbers.IndexOf(device);
                                var deviceParam = lineParameter.GrabberParameters[deviceIndex];

                                device.Height = deviceParam.Height; // 検査用画像高さに復帰
                            }
                        }
                    });
                    #endregion

                    // 連続撮影開始
                    Parallel.ForEach(this.GrabberControllers, controller =>
                    {
                        controller.Grabbers.ForEach(g => g.TriggerMode = TriggerMode.InternalTrigger);
                        controller.GrabContinuously();
                    });

                    Serilog.Log.Information($"{this}, camera start live");

                    // 排他利用コントロールを無効化
                    exclusiveControls.ForEach(c => c.Enabled = false);
                }
                // ライブ撮影停止
                else
                {
                    // 撮影停止
                    Parallel.ForEach(this.GrabberControllers, controller =>
                    {
                        controller.StopGrabbing();
                    });

                    Serilog.Log.Information($"{this}, camera stop live");

                    // 照明点灯OFF
                    if (this.LightController is ILightControllerSupervisor lightSupervisor && this.AppConfig.LightControllerParameter is ILightControllerSupervisorParameter lightParam)
                    {
                        Parallel.ForEach(lightSupervisor.Devices, device =>
                        {
                            var index = lightSupervisor.Devices.IndexOf(device);
                            var param = lightParam.DeviceParameters[index];
                            var semaphore = this.SemaphoreLighting[device];

                            if (param.OnOffControlType is LightingOnOffControlType.Continuous or LightingOnOffControlType.OneByOne)
                            {
                                if (true == semaphore.Wait(0))
                                {
                                    Serilog.Log.Information($"{this}, turn off {this.ToString(device)}, Continuous / OneByOne");
                                    device.TurnOff();
                                    semaphore.Release();
                                }
                                else
                                {
                                    Serilog.Log.Warning($"{this}, turn off overlapped {this.ToString(device)}, Continuous / OneByOne");
                                }
                            }
                        });
                    }

                    // 排他利用コントロールを有効化
                    exclusiveControls.ForEach(c => c.Enabled = true);

                    // ヒストグラム表示
                    using var bitmap = this.UcMultiImageView[UcMultiImageView.SelectedIndex].GetImage();
                    this.vieweHistgram(bitmap, false); // 最後の画像は必ず表示したいので非同期にしない
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// テスト運転
        /// </summary>
        /// <remarks>このコードは全てのカメラが検査にSoftTriggerを用いることを前提としています</remarks>
        private void onOffUcTestRunning_ValueChanged(object obj)
        {
            try
            {
                // 検査パラメータが設定されていない場合は例外をスロー
                if (this.AppConfig.InspectionControllerParameter is null)
                {
                    throw new InvalidOperationException("inspection parameter is null");
                }
                var ip = this.AppConfig.InspectionControllerParameter;

                // 設定値の更新
                this.updateConfiguration();

                // 排他利用コントロール
                var exclusiveControls = new List<Control>(new Control[] { this.onOffUcCameraLive });

                // 自動撮影ON
                if (this.onOffUcTestRunning.Value)
                {
                    // 排他利用コントロールを無効化
                    exclusiveControls.ForEach(c => c.Enabled = false);

                    // FPS計算を初期化
                    foreach (var calc in this.FpsCalculator)
                    {
                        calc.Reset();
                    }

                    // セマフォの設定
                    foreach (var s in this.SemaphoreGrabber.Values)
                    {
                        if (s.CurrentCount == 0)
                        {
                            s.Release();
                        }
                    }

                    // トリガカウントをリセット
                    this.TestRunningTriggerCount = 0;

                    // テストタスクリストをクリア
                    lock (this.TestTaskSync)
                    {
                        this.TestTaskList.Clear();
                    }

                    // 検査中常時点灯の照明を点灯
                    if (this.LightController is not null && this.AppConfig.LightControllerParameter is ILightControllerSupervisorParameter lp)
                    {
                        Parallel.ForEach(this.LightController.Devices, device =>
                        {
                            var lightIndex = this.LightController.Devices.IndexOf(device);
                            var lightParam = lp.DeviceParameters[lightIndex];
                            var semaphore = this.SemaphoreLighting[device];

                            if (LightingOnOffControlType.Continuous == lightParam.OnOffControlType)
                            {
                                if (true == semaphore.Wait(0))
                                {
                                    Serilog.Log.Information($"{this}, turn on {this.ToString(device)}, Continuous");
                                    device.TurnOn();
                                    semaphore.Release();
                                }
                                else
                                {
                                    Serilog.Log.Warning($"{this}, turn on overlapped {this.ToString(device)}, Continuous");
                                }
                            }
                        });
                    }

                    #region 撮影停止中のみ変更可能な設定を更新する
                    Parallel.ForEach(this.GrabberControllers, controller =>
                    {
                        if (controller is ILineSensorController lineController && this.AppConfig.LineSensorControllerParameter is ILineSensorControllerParameter lineParameter)
                        {
                            foreach (ILineSensor device in lineController.Grabbers)
                            {
                                var deviceIndex = lineController.Grabbers.IndexOf(device);
                                var deviceParam = lineParameter.GrabberParameters[deviceIndex];

                                device.Height = deviceParam.Height; // 検査用画像高さに復帰
                            }
                        }
                    });
                    #endregion

                    // 撮影開始(ソフトトリガー)
                    Parallel.ForEach(this.GrabberControllers, controller =>
                    {
                        controller.Grabbers.ForEach(g => g.TriggerMode = TriggerMode.SoftTrigger);
                        controller.GrabContinuously();
                    });

                    // テスト運転フラグを立てる
                    Serilog.Log.Information($"{this}, start test running");
                    this.IsTestRunning = true;
                }
                // 自動撮影OFF
                else
                {
                    // テスト運転フラグを落とす
                    this.IsTestRunning = false;

                    // 撮影停止
                    Parallel.ForEach(this.GrabberControllers, controller =>
                    {
                        controller.StopGrabbing();
                    });

                    Serilog.Log.Information($"{this}, stop test running");

                    // 検査中常時点灯の照明を消灯
                    if (this.LightController is not null && this.AppConfig.LightControllerParameter is ILightControllerSupervisorParameter lp)
                    {
                        Parallel.ForEach(this.LightController.Devices, device =>
                        {
                            var lightIndex = this.LightController.Devices.IndexOf(device);
                            var lightParam = lp.DeviceParameters[lightIndex];
                            var semaphore = this.SemaphoreLighting[device];

                            if (LightingOnOffControlType.Continuous == lightParam.OnOffControlType)
                            {
                                if (true == semaphore.Wait(0))
                                {
                                    Serilog.Log.Information($"{this}, turn off {this.ToString(device)}, Continuous");
                                    device.TurnOff();
                                    semaphore.Release();
                                }
                                else
                                {
                                    Serilog.Log.Warning($"{this}, turn off overlapped {this.ToString(device)}, Continuous");
                                }
                            }
                        });
                    }

                    // 排他利用コントロールを有効化
                    exclusiveControls.ForEach(c => c.Enabled = true);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 保存ボタンクリック
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 停止時のみ保存を許可する
                if (true == this.onOffUcCameraLive.Value || true == this.onOffUcTestRunning.Value)
                {
                    using var confirmDialog = new ConfirmationForm();
                    confirmDialog.ShowError(this, MessageBoxButtons.OK, "設定の保存", "撮影を停止してから保存を行ってください。");

                    return;
                }

                this.updateConfiguration();
                this.saveConfiguration();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 表示オプション切り替え
        /// </summary>
        private void onOffUcViewOption_ValueChanged(object obj)
        {
            try
            {
                foreach (var control in this.UcMultiImageView)
                {
                    control.CenterLines.Enabled = this.onOffUcViewCenter.Value;
                    control.HProjection.Enabled = this.onOffUcViewProjection.Value;
                    control.PseudoColorEnabled = this.onOffUcViewPseudo.Value;
                    control.UseMeasurement = this.onOffUcDistanceMeasurement.Value;
                }

                this.histoUc.PseudColorEnabled = this.onOffUcViewPseudo.Value;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.UcMultiImageView.Redraw();
            }
        }

        /// <summary>
        /// 画像保存ボタン
        /// </summary>
        private async void btnSaveImage_Click(object sender, EventArgs e)
        {
            try
            {
                // 1click保存が有効な場合
                if (true == this.onOffUc1ClickSave.Value)
                {
                    if (this.UcMultiImageView[this.UcMultiImageView.SelectedIndex].GetImage() is Bitmap bitmap)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                // 画像ファイル情報
                                var fileInfo = new FileInfo(System.IO.Path.Combine(this.saveImageFileDialog.InitialDirectory, $"{DateTime.Now:yyyyMMddHHmmssffffff}.{this.saveImageFileDialog.DefaultExt}"));

                                BitmapProcessor.Write(fileInfo, bitmap);

                                Serilog.Log.Information($"{this}, image save success. {fileInfo.Name}");
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });

                        bitmap.Dispose();
                    }
                }
                // 名前を付けて画像を保存
                else if (DialogResult.OK == this.saveImageFileDialog.ShowDialog(this))
                {
                    if (this.UcMultiImageView[this.UcMultiImageView.SelectedIndex].GetImage() is Bitmap bitmap)
                    {
                        var fileInfo = new FileInfo(this.saveImageFileDialog.FileName);

                        await Task.Run(() =>
                        {
                            try
                            {
                                BitmapProcessor.Write(fileInfo, bitmap);

                                Serilog.Log.Information($"{this}, image save success. {fileInfo.Name}");
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });

                        bitmap.Dispose();

                        this.saveImageFileDialog.InitialDirectory = Path.GetDirectoryName(this.saveImageFileDialog.FileName);
                        this.saveImageFileDialog.FileName = fileInfo.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ボタンイベント:パラメータ表示
        /// </summary>
        private void buttonShowParameter(object sender, EventArgs e)
        {
            try
            {
                var paramButton = sender as Button;
                if (paramButton is null)
                {
                    return;
                }

                var paramForm = (Form?)null;

                if (true == this.buttonGrabberParameter.Equals(sender))
                {
                    paramForm = this.GrabberParameterForm?.GetForm();  // カメラ設定画面
                }
                else if (true == this.buttonLightParameter.Equals(sender))
                {
                    paramForm = this.LightParameterForm?.GetForm();    // 照明設定画面
                }

                if (paramForm is null)
                {
                    return;
                }

                // 画面の表示位置を設定
                paramForm.Left = this.Left + paramButton.Left - paramForm.Width;
                paramForm.Top = this.Top + paramButton.Top;

                // 画面の表示
                if (true == paramForm.Visible)
                {
                    paramForm.BringToFront();
                }
                else
                {
                    paramForm.Show(this);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ボタンイベント:手動トリガー
        /// </summary>
        private async void buttonManualTrigger_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.TimingInjectedIO is not null && this.AppConfig.InspectionControllerParameter is IInspectionControllerParameter ip)
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            this.TimingInjectedIO.SimulateInput(0, 1);  // ON

                            await Task.Delay((int)(ip.AcquisitionTriggerValidHoldingTimeMs * 1.5)); // ON期間のウェイト

                            this.TimingInjectedIO.SimulateInput(0, 0);  // OFF
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ボタンイベント:カメラ補正
        /// </summary>
        private void buttonLineSensorCorrection_Click(object sender, EventArgs e)
        {
            try
            {
                var paramButton = sender as Button;
                if (paramButton is null)
                {
                    return;
                }

                if (this.LineSensorFFC_Form is null)
                {
                    return;
                }

                // 対象のカメラインデックスを取得
                var grabberIndex = this.UcMultiImageView.SelectedIndex;

                // 対象のIGrabberを取得
                if (this.AllGrabbers[grabberIndex] is GigESentechLineSensorGrabber lineSensor)
                {
                    if (this.LineSensorFFC_Form.GetForm() is Form form)
                    {
                        // 非表示の場合
                        if (false == form.Visible)
                        {
                            // 対象のIGrabberを通知して初期化
                            this.LineSensorFFC_Form.Initialize(lineSensor);

                            // 画面の表示
                            form.Show(this);
                        }

                        // 画面の表示位置を設定
                        form.Left = this.Left + paramButton.Left - form.Width;
                        form.Top = this.Top + paramButton.Bottom - form.Height;
                        form.BringToFront();
                    }
                }
                else
                {
                    using var confirmDialog = new ConfirmationForm();
                    confirmDialog.ShowError(this, MessageBoxButtons.OK, "ラインセンサの補正", "選択中のカメラは補正機能を利用することは出来ません。");

                    return;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 設定更新
        /// </summary>
        private void updateConfiguration()
        {
            try
            {
                if (this.AppConfig.InspectionControllerParameter is IInspectionControllerParameter ip)
                {
                    // カメラ設定
                    if (this.GrabberParameterForm is not null)
                    {
                        // 現在の設定値を取得
                        var grabberParameterSetAll = this.GrabberParameterForm.GetParameterSet();

                        // 撮影遅延時間の設定域の確認と拡張
                        if (ip.AcquisitionTriggerDelayMs.Length < grabberParameterSetAll.Count)
                        {
                            var previousValues = ip.AcquisitionTriggerDelayMs;
                            var expandedValues = new int[this.AllGrabbers.Count];
                            Array.Copy(previousValues, expandedValues, previousValues.Length);

                            ip.AcquisitionTriggerDelayMs = expandedValues;
                        }

                        // 設定値の更新
                        foreach (var parameterSet in grabberParameterSetAll)
                        {
                            var index = grabberParameterSetAll.IndexOf(parameterSet);
                            ip.AcquisitionTriggerDelayMs[index] = parameterSet.BehaviorOptions.AcquisitionTriggerDelayMs;
                        }
                    }

                    // 照明設定
                    if (this.LightParameterForm is not null && this.LightController is not null)
                    {
                        // 現在の設定値を取得
                        var lightParameterSetAll = this.LightParameterForm.GetParameterSet();

                        // 照明点灯遅延時間の設定域の確認と拡張
                        if (ip.LightTurnOnDelayMs.Length < lightParameterSetAll.Count)
                        {
                            var previousValues = ip.LightTurnOnDelayMs;
                            var expandedValues = new int[this.LightController.NumberOfDevice];
                            Array.Copy(previousValues, expandedValues, previousValues.Length);

                            ip.LightTurnOnDelayMs = expandedValues;
                        }

                        // 照明消灯遅延時間の設定域の確認と拡張
                        if (ip.LightTurnOffDelayMs.Length < lightParameterSetAll.Count)
                        {
                            var previousValues = ip.LightTurnOffDelayMs;
                            var expandedValues = new int[this.LightController.NumberOfDevice];
                            Array.Copy(previousValues, expandedValues, previousValues.Length);

                            ip.LightTurnOffDelayMs = expandedValues;
                        }

                        // 設定値の更新
                        foreach (var parameterSet in lightParameterSetAll)
                        {
                            var index = lightParameterSetAll.IndexOf(parameterSet);

                            ip.LightTurnOnTiming = parameterSet.BehaviorOptions.LightTurnOnTiming;
                            ip.LightTurnOffTiming = parameterSet.BehaviorOptions.LightTurnOffTiming;

                            ip.LightTurnOnDelayMs[index] = parameterSet.BehaviorOptions.LightTurnOnDelayMs;
                            ip.LightTurnOffDelayMs[index] = parameterSet.BehaviorOptions.LightTurnOffDelayMs;
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
        /// 設定保存
        /// </summary>
        private void saveConfiguration()
        {
            try
            {
                using var marqueue = ProgressBarForm.NewMarqueeType(this, "保存中", $"設定を保存しています。{Environment.NewLine}しばらくお待ちください。");

                var formBlockingAccessor = new FormBlockingAccessor(marqueue);

                #region 設定値とカメラUserSetの保存
                Task.Run(() =>
                {
                    var saveResuls = Enumerable.Repeat(false, this.AllGrabbers.Count + 1).ToArray();

                    try
                    {
                        // 設定値の保存
                        saveResuls[saveResuls.Length - 1] = this.AppConfig.Save();

                        // カメラUserSetの保存
                        Parallel.ForEach(this.AllGrabbers, grabber =>
                        {
                            try
                            {
                                if (grabber is IFileDevice)
                                {
                                    saveResuls[this.AllGrabbers.IndexOf(grabber)] = true;
                                }
                                else
                                {
                                    saveResuls[this.AllGrabbers.IndexOf(grabber)] = grabber.SaveUserSet();
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        if (saveResuls.All(r => r))
                        {
                            formBlockingAccessor.Set(DialogResult.OK);
                        }
                        else
                        {
                            formBlockingAccessor.Set(DialogResult.Abort);
                        }
                    }
                });
                #endregion

                if (DialogResult.OK == formBlockingAccessor.ShowBlocking())
                {
                    Serilog.Log.Information($"{this}, setting save succeed");
                }
                else
                {
                    Serilog.Log.Warning($"{this}, setting save failed");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, $"setting save failed");
            }
        }

        /// <summary>
        /// クローズ処理
        /// </summary>
        private void closeProcessing()
        {
            try
            {
                using var marqueue = ProgressBarForm.NewMarqueeType(this, "終了中", $"終了しています。{Environment.NewLine}しばらくお待ちください。");

                var formBlockingAccessor = new FormBlockingAccessor(marqueue);

                Task.Run(async () =>
                {
                    try
                    {
                        // 画像取得制御
                        Parallel.ForEach(this.GrabberControllers, controller =>
                        {
                            controller.GrabberDisabled -= this.GrabberController_GrabberDisabled;

                            if (controller is IAreaSensorController areaController)
                            {
                                areaController.DataGrabbed -= this.GrabberController_DataGrabbed;
                            }
                            else if (controller is ILineSensorController lineController)
                            {
                                lineController.DataGrabbed -= this.GrabberController_DataGrabbed;
                            }
                        });

                        // DIO制御
                        if (this.DioController is not null)
                        {
                            this.DioController.InputChanged -= this.DioController_InputChanged;
                            this.DioController.Disabled -= this.DioController_Disabled;
                        }

                        // 照明制御
                        if (this.LightController is not null)
                        {
                            this.LightController.Disabled -= this.LightController_Disabled;
                        }

                        // PLC通信
                        if (this.PlcTcpCommunicator is not null)
                        {
                            this.PlcTcpCommunicator.Disconnected -= this.PlcTcpCommunicator_Disconnected;
                            this.PlcTcpCommunicator.Connected -= this.PlcTcpCommunicator_Connected;
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }

                    try
                    {
                        // 終了時処理
                        await Task.Run(async () =>
                        {
                            // 照明OFF
                            this.LightController?.TurnOff();

                            // DIO監視停止
                            this.DioController?.StopMonitoring();

                            // 撮影停止
                            Parallel.ForEach(this.GrabberControllers, controller =>
                            {
                                controller.StopGrabbing();
                            });

                            await Task.Delay(1000);
                        });

                        // 制御クローズ
                        this.PlcTcpCommunicator?.Close();
                        this.DioController?.Close();
                        this.LightController?.Close();
                        Parallel.ForEach(this.GrabberControllers, controller =>
                        {
                            controller.Close();
                        });

                        #region セマフォの破棄
                        {
                            foreach (var s in this.SemaphoreGrabber.Values)
                            {
                                s.Dispose();
                            }
                            this.SemaphoreGrabber.Clear();

                            foreach (var s in this.SemaphoreLighting.Values)
                            {
                                s.Dispose();
                            }
                            this.SemaphoreLighting.Clear();

                            this.SemaphoreHistgram.Dispose();
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        formBlockingAccessor.Set(DialogResult.OK);
                    }
                });

                formBlockingAccessor.ShowBlocking();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// IGrabberDataをBitmapに変換する    
        /// </summary>
        private Bitmap? convertGrabberData(IGrabberData data, out ImageData? imgData)
        {
            var bitmap = data.ToBitmap();
            imgData = (ImageData?)null;

            // byte配列データ保持
            if (data is IByteArrayGrabberData byteArrayData)
            {
                imgData = new ImageData
                {
                    Bytes = byteArrayData.Image,
                    Format = byteArrayData.PixelFormat,
                    Stride = byteArrayData.Stride,
                    Width = byteArrayData.Size.Width,
                    Height = byteArrayData.Size.Height,
                };
            }
            else if (data is IByteJaggedArrayGrabberData jaggedArrayData)
            {
                imgData = new ImageData
                {
                    Bytes = jaggedArrayData.ToArray(),
                    Format = jaggedArrayData.PixelFormat,
                    Stride = jaggedArrayData.Image[0].Length,
                    Width = jaggedArrayData.Size.Width,
                    Height = jaggedArrayData.Size.Height,
                };
            }
            // bitmapが存在しない場合はByte配列から生成する
            else if (bitmap is not null)
            {
                imgData = new ImageData
                {
                    Format = bitmap.PixelFormat,
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                };

                var bmpData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

                try
                {
                    imgData.Stride = bmpData.Stride;
                    imgData.Bytes = new byte[bmpData.Stride * bmpData.Height];

                    var ptrBmp = bmpData.Scan0;
                    Marshal.Copy(ptrBmp, imgData.Bytes, 0, imgData.Bytes.Length);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    bitmap.UnlockBits(bmpData);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// カメラ画像インデックスを取得する
        /// </summary>
        /// <param name="location">判定するカメラインデックス</param>
        /// <returns>直列化されたインデックス（エリア→ライン）</returns>
        public int getGrabberCameraIndex(Library.Common.Drawing.Point location)
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
        /// ヒストグラムを表示する
        /// </summary>
        /// <param name="bitmap">表示するBitmap</param>
        /// <param name="isAsync">非同期で実行するかどうか</param>
        private void vieweHistgram(Bitmap? bitmap, bool isAsync = false)
        {
            if (true == isAsync)
            {
                if (bitmap?.Clone() is Bitmap clonedBitmap)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            if (true == this.SemaphoreHistgram.Wait(0))
                            {
                                try
                                {
                                    this.histoUc.Set(clonedBitmap);
                                }
                                catch (Exception ex)
                                {
                                    Serilog.Log.Warning(ex, ex.Message);
                                }
                                finally
                                {
                                    this.SemaphoreHistgram.Release();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                        finally
                        {
                            clonedBitmap.Dispose();
                        }
                    });
                }
            }
            else if (bitmap is not null)
            {
                try
                {
                    this.histoUc.Set(bitmap);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        private void checkTestTaskCompletion(int grabberIndex)
        {
            if (false == this.IsTestRunning)
            {
                return;
            }

            #region 対象のカメラ制御セマフォを解放する
            try
            {
                var semaphore = this.SemaphoreGrabber[this.AllGrabbers[grabberIndex]];
                semaphore.Release();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion

            var currentTask = (TestTask?)null;

            #region テストタスクの完了確認
            lock (this.TestTaskSync)
            {
                if (0 < this.TestTaskList.Count)
                {
                    currentTask = this.TestTaskList.First();
                }
            }

            if (currentTask is not null)
            {
                if (0 == Interlocked.Decrement(ref currentTask.TotalGrabbedCount))
                {
                    Serilog.Log.Information($"{this}, No.{currentTask.TriggerNo:D4}, all grabbed.");

                    lock (this.TestTaskSync)
                    {
                        if (this.TestTaskList.Contains(currentTask))
                        {
                            this.TestTaskList.Remove(currentTask);
                        }
                    }
                }
                else
                {
                    currentTask = null;
                }
            }
            else
            {
                Serilog.Log.Warning($"{this}, unexpected grabbed. {this.ToString(this.AllGrabbers[grabberIndex])}");
            }
            #endregion

            #region 照明OFF制御(LightControlTiming.Fastest)
            if (currentTask is not null)
            {
                Task.Run(() =>
                {
                    if (this.LightController is ILightControllerSupervisor lightSupervisor)
                    {
                        var ip = this.AppConfig.InspectionControllerParameter!;
                        var lp = this.AppConfig.LightControllerParameter!;

                        Parallel.ForEach(lightSupervisor.Devices, device =>
                        {
                            var index = lightSupervisor.Devices.IndexOf(device);
                            var param = lp.DeviceParameters[index];

                            if (param.OnOffControlType is not LightingOnOffControlType.OneByOne)
                            {
                                return;
                            }

                            var semaphore = this.SemaphoreLighting[device];
                            if (ip.LightTurnOffTiming is LightControlTiming.Fastest)
                            {
                                if (true == semaphore.Wait(0))
                                {
                                    Serilog.Log.Information($"{this}, No.{currentTask.TriggerNo:D4}, turn off {this.ToString(device)}, delay = fastest");
                                    device.TurnOff();
                                    semaphore.Release();
                                }
                                else
                                {
                                    Serilog.Log.Warning($"{this}, No.{currentTask.TriggerNo:D4}, turn off overlapped {this.ToString(device)}, delay = fastest");
                                }
                            }
                        });
                    }
                });
            }
            #endregion
        }

        private string ToString(IGrabber grabber) => $"grabber Y{grabber.Location.Y + 1:D2}_X{grabber.Location.X + 1:D2}";
        private string ToString(ILightController device) => $"light Y{device.Location.Y + 1:D2}_X{device.Location.X + 1:D2}";

        #endregion

        #region GrabberParameterForm

        /// <summary>
        /// GrabberParameterForm:パラメータ変更イベント
        /// </summary>
        /// <param name="parameterSet">変更されたパラメータ</param>
        private void GrabberParameterForm_ParameterChanged(object sender, GrabberParameterSet parameterSet)
        {
            try
            {
                var imageIndex = this.getGrabberCameraIndex(parameterSet.Device.Location);
                if (0 > imageIndex || this.GrabberParameterForm is null)
                {
                    return;
                }

                if (this.AppConfig.InspectionControllerParameter is IInspectionControllerParameter ip)
                {
                    // 撮影遅延時間
                    if (ip.AcquisitionTriggerDelayMs.Length > imageIndex)
                    {
                        ip.AcquisitionTriggerDelayMs[imageIndex] = parameterSet.BehaviorOptions.AcquisitionTriggerDelayMs;
                    }
                    // 設定項目数が足りない場合は拡張
                    else
                    {
                        var previousValues = ip.AcquisitionTriggerDelayMs;
                        var expandedValues = new int[this.AllGrabbers.Count];
                        Array.Copy(previousValues, expandedValues, previousValues.Length);

                        var prameterSetAll = this.GrabberParameterForm.GetParameterSet();
                        for (int i = imageIndex; i < expandedValues.Length; i++)
                        {
                            expandedValues[i] = prameterSetAll[i].BehaviorOptions.AcquisitionTriggerDelayMs;
                        }

                        ip.AcquisitionTriggerDelayMs = expandedValues;
                    }
                }

                // 分解能を更新する
                this.UcMultiImageView[imageIndex].ResolutionMmPerPixel = parameterSet.ImageProperties.ResolutionMmPerPixel;

                // 画像表示を更新する
                this.UcMultiImageView.Redraw();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region LightParameterForm

        /// <summary>
        /// LightParameterForm:パラメータ変更イベント
        /// </summary>
        /// <param name="parameterSet">変更されたパラメータ</param>
        private void LightParameterForm_ParameterChanged(object sender, LightParameterSet parameterSet)
        {
            try
            {
                if (this.LightController is null || this.LightParameterForm is null)
                {
                    return;
                }

                var index = this.LightController.Devices.IndexOf(parameterSet.Device);
                if (0 > index)
                {
                    return;
                }

                if (this.AppConfig.InspectionControllerParameter is IInspectionControllerParameter ip)
                {
                    // 照明点灯/消灯 タイミング
                    ip.LightTurnOnTiming = parameterSet.BehaviorOptions.LightTurnOnTiming;
                    ip.LightTurnOffTiming = parameterSet.BehaviorOptions.LightTurnOffTiming;

                    // 照明点灯遅延
                    if (ip.LightTurnOnDelayMs.Length > index)
                    {
                        ip.LightTurnOnDelayMs[index] = parameterSet.BehaviorOptions.LightTurnOnDelayMs;
                    }
                    // 設定項目数が足りない場合は拡張
                    else
                    {
                        var previousValues = ip.LightTurnOnDelayMs;
                        var expandedValues = new int[this.LightController.NumberOfDevice];
                        Array.Copy(previousValues, expandedValues, previousValues.Length);

                        var prameterSetAll = this.LightParameterForm.GetParameterSet();
                        for (int i = index; i < expandedValues.Length; i++)
                        {
                            expandedValues[i] = prameterSetAll[i].BehaviorOptions.LightTurnOnDelayMs;
                        }

                        ip.LightTurnOnDelayMs = expandedValues;
                    }

                    // 照明消灯遅延
                    if (ip.LightTurnOffDelayMs.Length > index)
                    {
                        ip.LightTurnOffDelayMs[index] = parameterSet.BehaviorOptions.LightTurnOffDelayMs;
                    }
                    // 設定項目数が足りない場合は拡張
                    else
                    {
                        var previousValues = ip.LightTurnOffDelayMs;
                        var expandedValues = new int[this.LightController.NumberOfDevice];
                        Array.Copy(previousValues, expandedValues, previousValues.Length);

                        var prameterSetAll = this.LightParameterForm.GetParameterSet();
                        for (int i = index; i < expandedValues.Length; i++)
                        {
                            expandedValues[i] = prameterSetAll[i].BehaviorOptions.LightTurnOffDelayMs;
                        }

                        ip.LightTurnOffDelayMs = expandedValues;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region MultiImageViewControl

        /// <summary>
        /// MultiImageViewControlイベント:画像更新
        /// </summary>
        /// <param name="control">対象のImageViewControl</param>
        /// <remarks>選択中の画像の場合にヒストグラムを表示する</remarks>
        private void UcMultiImageView_ImageUpdated(object sender, ImageViewControl control)
        {
            try
            {
                // 注目画像以外の場合は処理しない
                if (this.UcMultiImageView.SelectedIndex != control.Index)
                {
                    return;
                }

                // 表示中の画像を取得する
                using var bitmap = control.GetImage();

                // ヒストグラム表示
                this.vieweHistgram(bitmap, this.AppConfig.GuiOperation.IsHistogramDisplayAsync | this.IsTestRunning);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// MultiImageViewControlイベント:画像選択変更
        /// </summary>
        private void UcMultiImageView_SelectedIndexChanged(object sender, ImageViewControl control)
        {
            try
            {
                this.UcCameraSelection.SelectedIndex = this.UcMultiImageView.SelectedIndex;

                // 配置位置によって表示内容を変更する
                foreach (var c in this.UcMultiImageView)
                {
                    if (this.UcMultiImageView.SelectedIndex == c.Index)
                    {
                        c.DisplayPixelInfoEnabled = true;
                        c.DisplayFpsEnabled = true;
                    }
                    else
                    {
                        c.DisplayPixelInfoEnabled = false;
                        c.DisplayFpsEnabled = false;
                    }
                }

                // 表示中の画像を取得する
                using var bitmap = control.GetImage();

                // ヒストグラム表示
                this.vieweHistgram(bitmap, this.AppConfig.GuiOperation.IsHistogramDisplayAsync | this.IsTestRunning);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region ILineSensorFFC_Form

        /// <summary>
        /// ILineSensorFFC_Form
        /// </summary>
        private void LineSensorFFC_Form_GrabberStatusRequested(object? sender, GrabberStatusEventArgs e)
        {
            e.IsLive = this.onOffUcCameraLive.Value;
        }

        #endregion
    }
}
