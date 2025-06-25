using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Controller.Plc;
using Hutzper.Library.Common.Drawing;
using Hutzper.Library.Common.Forms;
using Hutzper.Library.Common.Imaging;
using Hutzper.Library.Common.IO;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.DigitalIO;
using Hutzper.Library.DigitalIO.Device;
using Hutzper.Library.Forms;
using Hutzper.Library.Forms.ImageView;
using Hutzper.Sample.Mekiki.ScratchLegacy.Controller;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Diagnostics;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Inspection;
using Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement;
using Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.PostProcessing;
using Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection
{
    /// <summary>
    /// メイン画面
    /// </summary>
    public partial class OperationForm : ServiceCollectionSharingForm
    {
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
        /// 検査制御
        /// </summary>
        private readonly IInspectionController? InspectionController;

        /// <summary>
        /// DIO制御
        /// </summary>
        private IDigitalIODeviceInputSimulatable? TimingInjectedIO;

        /// <summary>
        /// 最後に画像保存した日時
        /// </summary>
        private DateTime LastImageSavedDateTime = DateTime.MinValue;

        /// <summary>
        /// Formヘルパ
        /// </summary>
        private readonly FormsHelper FormsHelper;

        /// <summary>
        /// 最新の検査結果
        /// </summary>
        private IInspectionResult? LatestInspectionResult;

        /// <summary>
        /// 判定ロジック
        /// </summary>
        private IInferenceResultJudgment? InferenceResultJudgment;

        /// <summary>
        /// 確認ユーザーコントロール
        /// </summary>
        private ConfirmationUserControl UcConfirmation = new();

        /// <summary>
        /// PLC通信
        /// </summary>
        private IPlcTcpCommunicator? PlcTcpCommunicator;

        /// <summary>
        /// 後処理
        /// </summary>
        private IInferenceResultPostProcessing?[] PostProcessings = Array.Empty<IInferenceResultPostProcessing?>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OperationForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OperationForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
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

            // バージョン表示
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var version = asm.GetName().Version;
            this.Text = $"{this.Text} ver.{version?.ToString() ?? Application.ProductVersion}";

            // 検査制御
            this.InspectionController = this.Services?.ServiceProvider?.GetRequiredService<IInspectionController>();
            if (this.InspectionController is not null)
            {
                this.Controller.Add(this.InspectionController);
            }

            // PLC通信
            if (false == string.IsNullOrEmpty(this.AppConfig.PlcTcpCommunicatorParameter?.IpAddress) && 0 < this.AppConfig.PlcTcpCommunicatorParameter.PortNumber)
            {
                this.PlcTcpCommunicator = this.Services?.ServiceProvider?.GetRequiredService<IPlcTcpCommunicator>();

                if (this.PlcTcpCommunicator is not null)
                {
                    this.PlcTcpCommunicator.Connected += this.PlcTcpCommunicator_Connected;
                    this.PlcTcpCommunicator.Disconnected += this.PlcTcpCommunicator_Disconnected;
                    this.PlcTcpCommunicator.ProcessingStatusChanged += this.PlcTcpCommunicator_ProcessingStatusChanged;

                    this.Controller.Add(this.PlcTcpCommunicator);
                }
            }

            // コントローラーの初期化
            foreach (var c in this.Controller)
            {
                c.Initialize(this.Services);
                c.SetConfig(this.AppConfig);
            }

            // 検査制御
            if (this.InspectionController is not null)
            {
                this.InspectionController.InspectionStatusChanged += this.InspectionController_InspectionStatusChanged;
                this.InspectionController.DeviceStatusChanged += this.InspectionController_DeviceStatusChanged;

                this.InspectionController.SetParameter(this.AppConfig.InspectionControllerParameter);
            }

            // PLC通信
            this.PlcTcpCommunicator?.SetParameter(this.AppConfig.PlcTcpCommunicatorParameter);

            #region 判定ロジック(サンプル)の設定と結果表示の初期化
            if (this.AppConfig.JudgmentParameter is IInferenceResultJudgmentParameter judgmentParameter)
            {
                this.InferenceResultJudgment = this.Services?.ServiceProvider?.GetRequiredService<IInferenceResultJudgment>();

                if (this.AppConfig.InspectionControllerParameter is not null)
                {
                    var grabberLocations = new List<Library.Common.Drawing.Point>();

                    #region カメラをリストアップ(エリア→ライン)
                    if (this.AppConfig.AreaSensorControllerParameter is not null)
                    {
                        foreach (var gp in this.AppConfig.AreaSensorControllerParameter.GrabberParameters)
                        {
                            grabberLocations.Add(gp.Location);
                        }
                    }
                    if (this.AppConfig.LineSensorControllerParameter is not null)
                    {
                        foreach (var gp in this.AppConfig.LineSensorControllerParameter.GrabberParameters)
                        {
                            grabberLocations.Add(gp.Location);
                        }
                    }
                    #endregion

                    // 判定クラスの定義
                    var judgmentClass = this.Services?.ServiceProvider?.GetRequiredService<IInferenceResultJudgementClass>();
                    if (judgmentClass is null)
                    {
                        throw new Exception("judgment class is null");
                    }

                    // カメラ毎のクラス名を設定
                    judgmentClass.ClassNamesPerGrabber = new List<string>[grabberLocations.Count];
                    foreach (var i in Enumerable.Range(0, judgmentClass.ClassNamesPerGrabber.Length))
                    {
                        // 使用するクラスを定義する(この例では全カメラで共通クラス)
                        var classNames = new List<string>();
                        classNames.Add("ok");
                        classNames.Add("ng_a");
                        classNames.Add("ng_b");
                        classNames.Add("ng_c");
                        classNames.Add("ng_d");

                        judgmentClass.ClassNamesPerGrabber[i] = classNames;
                    }

                    // 判定パラメータの初期化(カメラ毎のクラス名が統合される)
                    judgmentParameter.Initialize(judgmentClass);

                    // 画像情報(分解能)を設定
                    foreach (var location in grabberLocations)
                    {
                        if (this.AppConfig.ImagingConfiguration.FindByLocation(location) is IImageProperties ip)
                        {
                            var index = grabberLocations.IndexOf(location);
                            judgmentParameter.ImageProperties[index] = ip;
                        }
                    }

                    // 結果表示の初期化
                    this.UcResultAndStatisticsDisplay.Initialize(judgmentParameter);
                    this.UcResultAndStatisticsDisplay.Visible = !this.AppConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false;
                }
            }
            #endregion

            #region 後処理(セグメンテーション使用時のサンプル)の設定
            if (this.AppConfig.InspectionControllerParameter is not null && this.AppConfig.OnnxModelControllerParameter is not null)
            {
                // カメラ毎後処理の格納域を確保
                this.PostProcessings = new IInferenceResultPostProcessing[this.AppConfig.InspectionControllerParameter.InferenceIndecies.Length];
                foreach (var ppId in Enumerable.Range(0, this.PostProcessings.Length))
                {
                    var onnxId = this.AppConfig.InspectionControllerParameter.InferenceIndecies[ppId];      // 使用するonnx
                    var onnxParam = this.AppConfig.OnnxModelControllerParameter.ModelParameters[onnxId];    // 使用するonnxパラメータ

                    // セグメンテーションの場合
                    if (Library.Onnx.OnnxModelAlgorithm.Segmentation_Image == onnxParam.Algorithm)
                    {
                        this.PostProcessings[ppId] = new SegmentationPostProcessing();           // 後処理のインスタンスを生成
                        this.PostProcessings[ppId]?.Initialize(new SegmentationPostProcessingParameter());  // 後処理のパラメータを設定(この例では初期値をそのまま使用している)
                    }
                    else
                    {
                        // 後処理が不要な場合はnull
                    }
                }
            }
            #endregion

            #region マルチ画像表示ユーザーコントロールの設定
            {
                // レイアウトタイプの設定
                this.UcMultiImageView.LayoutType = this.AppConfig.GuiOperation.MultiImageLayoutType;
                this.UcMultiImageView.Drawing += this.UcMultiImageView_Drawing;

                #region エリアカメラ
                foreach (var i in Enumerable.Range(0, this.AppConfig.AreaSensorControllerParameter?.GrabberParameters.Count ?? 0))
                {
                    // プロパティを設定
                    var viewItem = new ImageViewControl()
                    {
                        ImageDescription = $"エリアカメラ{i + 1}",    // 画像の表示名
                        DisplayImageDescription = true,         // 画像の説明を表示するかどうか
                        DisplayPixelInfoEnabled = false,        // ピクセル情報を表示するかどうか
                        DisplayFpsEnabled = false,              // FPSを表示するかどうか
                        UseMeasurement = false,                 // 計測機能を使用するかどうか
                        AcceptImageFileDrop = false,             // 画像ファイルのドロップを受け付けるかどうか
                        PseudoColorEnabled = false,             // 疑似カラー表示を有効にするかどうか
                        ResolutionMmPerPixel = 0.1,               // 1ピクセルあたりの解像度(mm)
                    };

                    // コントロールを追加
                    this.UcMultiImageView.Add(viewItem);
                }
                #endregion

                #region ラインセンサ
                foreach (var i in Enumerable.Range(0, this.AppConfig.LineSensorControllerParameter?.GrabberParameters.Count ?? 0))
                {
                    // プロパティを設定
                    var viewItem = new ImageViewControl()
                    {
                        ImageDescription = $"ラインセンサ{i + 1}",    // 画像の表示名
                        DisplayImageDescription = true,         // 画像の説明を表示するかどうか
                        DisplayPixelInfoEnabled = false,        // ピクセル情報を表示するかどうか
                        DisplayFpsEnabled = false,              // FPSを表示するかどうか
                        UseMeasurement = false,                 // 計測機能を使用するかどうか
                        AcceptImageFileDrop = false,             // 画像ファイルのドロップを受け付けるかどうか
                        PseudoColorEnabled = false,             // 疑似カラー表示を有効にするかどうか
                        ResolutionMmPerPixel = 0.1,               // 1ピクセルあたりの解像度(mm)
                    };

                    // コントロールを追加
                    this.UcMultiImageView.Add(viewItem);
                }
                #endregion
            }
            #endregion

            #region カメラ選択コンボボックスの設定
            // *Spotlightの場合はカメラ選択リストを表示する
            if (this.UcMultiImageView.LayoutType is MultiImageLayoutType.Spotlight || this.UcMultiImageView.LayoutType is MultiImageLayoutType.SingleSpotlight)
            {
                this.UcCameraSelection.Visible = true;

                // カメラ選択リストの登録
                foreach (var view in this.UcMultiImageView)
                {
                    this.UcCameraSelection.Items.Add(view.ImageDescription);
                }

                // カメラ選択リストの選択変更イベント
                this.UcCameraSelection.SelectedIndexChanged += (sender, index, name) =>
                {
                    // 選択されたカメラ画像表示に切り替える
                    this.UcMultiImageView.SelectedIndex = index;
                };

                // カメラ選択リストの初期選択
                if (0 < this.UcCameraSelection.Items.Count)
                {
                    this.UcCameraSelection.SelectedIndex = 0;
                }
            }
            // 非表示の場合は画像表示の位置を調整する
            else
            {
                this.UcCameraSelection.Visible = false;
                var addHeight = Math.Abs(this.UcCameraSelection.Top - this.UcMultiImageView.Top);
                this.UcMultiImageView.Top = this.UcCameraSelection.Top;
                this.UcMultiImageView.Height += addHeight;
            }
            #endregion

            #region 画像収集用コントロール(デザイン時の色を実行時用に変更し、位置を調整する)
            {
                var replaceControl = this.UcResultAndStatisticsDisplay;

                this.panelContainerOfImageCollection.BackColor = this.BackColor;
                this.panelContainerOfImageCollection.Left = replaceControl.Left;
                this.panelContainerOfImageCollection.Top = replaceControl.Top;
                this.panelContainerOfImageCollection.Width = UcResultAndStatisticsDisplay.Width;
                this.panelContainerOfImageCollection.Height = UcResultAndStatisticsDisplay.Height;
                this.panelContainerOfImageCollection.Visible = this.AppConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false;

                this.ucImageFileSuffix.Value = string.Empty;
            }
            #endregion

            #region 機器状態表示ユーザーコントロールの設定
            this.ucMultiDeviceStatus.Initialize();
            this.ucMultiDeviceStatus.AddDevice(DeviceKind.DigitalIO, new MonitoredDeviceInfoUnit("トリガ", 0, Math.Max(1, this.AppConfig.DigitalIOConfiguration.Devices.Length)));
            this.ucMultiDeviceStatus.AddDevice(DeviceKind.Camera, new MonitoredDeviceInfoUnit("カメラ", 0, Math.Max(1, this.AppConfig.CameraConfiguration.NumberOfAllGrabbers)));
            this.ucMultiDeviceStatus.AddDevice(DeviceKind.Light, new MonitoredDeviceInfoUnit("照明", 0, Math.Max(1, this.AppConfig.LightConfiguration.Devices.Length)));
            if (this.PlcTcpCommunicator is not null)
            {
                this.ucMultiDeviceStatus.AddDevice(DeviceKind.Plc, new MonitoredDeviceInfoUnit("PLC", 0, 1));
            }
            #endregion

            // 検査設定ボタン
            this.buttonJudgementSetting.Visible = !(this.AppConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false);

            // 確認ユーザーコントロール
            this.Controls.Add(this.UcConfirmation);
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// Formイベント:画面表示
        /// </summary>
        private void OperationForm_Shown(object sender, EventArgs e)
        {
            try
            {
                using var marqueue = ProgressBarForm.NewMarqueeType(this, "起動中", "\r\nしばらくお待ちください。");

                var formBlockingAccessor = new FormBlockingAccessor(marqueue);

                Task.Run(async () =>
                {
                    var isSuccess = true;

                    try
                    {
                        var connectionTask = new List<Task<bool>>();
                        Task<bool> taskItem;

                        #region PLC通信
                        taskItem = Task.Run(async () =>
                        {
                            var isItemSuccess = true;   // 未使用時はtrueとして扱う

                            try
                            {
                                if (this.PlcTcpCommunicator is not null)
                                {
                                    // PLC通信の開始
                                    this.PlcTcpCommunicator.Open();

                                    // 通信が正常状態になるか指定時間に達するまで待機する
                                    var plcWatch = Stopwatch.StartNew();
                                    do
                                    {
                                        await Task.Delay(100);
                                    }
                                    while (false == this.PlcTcpCommunicator.IsProcessingCorrectly || 3000 > plcWatch.ElapsedMilliseconds);

                                    // 通信が正常状態でない場合はエラーとする
                                    if (false == this.PlcTcpCommunicator.IsProcessingCorrectly)
                                    {
                                        isItemSuccess = false;
                                        this.ucMultiDeviceStatus.Deviecs[DeviceKind.Plc].Items[0].Status = DeviceStatusKind.Disabled;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                isItemSuccess = false;
                                Serilog.Log.Warning(ex, ex.Message);
                            }

                            return isItemSuccess;
                        });
                        connectionTask.Add(taskItem);
                        #endregion

                        // PLCは検査制御の開始前に接続完了させておく
                        await taskItem;

                        #region 検査制御の開始
                        taskItem = Task.Run(() =>
                        {
                            var isItemSuccess = true;

                            try
                            {
                                // 検査制御
                                if (true == (this.InspectionController?.Open() ?? false))
                                {
                                    Serilog.Log.Information($"{this}, inspection controller open success");

                                    // DIO制御(IOシミュレートのインタフェースが存在すれば取得して保持する)
                                    if (this.Services?.ServiceProvider?.GetRequiredService<IDigitalIODeviceController>() is IDigitalIODeviceController dioController)
                                    {
                                        foreach (var device in dioController.AllDevices)
                                        {
                                            if (device is IDigitalIODeviceInputSimulatable timingInjectedIO)
                                            {
                                                this.TimingInjectedIO = timingInjectedIO;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    isItemSuccess = false;
                                    Serilog.Log.Warning($"{this}, inspection controller open failed");
                                }
                            }
                            catch (Exception ex)
                            {
                                isItemSuccess = false;
                                Serilog.Log.Warning(ex, ex.Message);
                            }

                            return isItemSuccess;
                        });
                        connectionTask.Add(taskItem);
                        #endregion

                        // 接続タスク待機
                        var result = await Task.WhenAll(connectionTask);

                        // 接続結果
                        foreach (var r in result)
                        {
                            isSuccess &= r;
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        formBlockingAccessor.Set(isSuccess ? DialogResult.OK : DialogResult.Cancel);
                    }
                });

                // バックグラウンド処理を待機する
                if (DialogResult.OK == formBlockingAccessor.ShowBlocking())
                {
                    Serilog.Log.Information($"{this}, operation begin");

                    // 画面設定
                    this.buttonGrab.Visible = this.TimingInjectedIO is not null;
                    this.ButtonChangePointLog.Visible = this.AppConfig.GuiOperation.UseButtonChangePointLogOutput;

                    #region 統計データの復帰
                    if (StatisticsResetTiming.OnInspectionStart != this.AppConfig.GuiOperation.StatisticsResetTiming)
                    {
                        var statisticsData = new StatisticsData();
                        var statisticsFile = new FileInfo(Path.Combine(this.AppConfig.Path.Data.FullName, $"statistics.json"));
                        if (true == statisticsData.LoadFromFile(statisticsFile))
                        {
                            this.UcResultAndStatisticsDisplay.SetStatisticsData(statisticsData);
                        }
                    }
                    #endregion
                }
                else
                {
                    Serilog.Log.Warning($"{this}, operation begin failed");

                    using var confirmDialog = this.FormsHelper.NewConfirmationForm();
                    confirmDialog.ShowError(this, MessageBoxButtons.OK, "検査開始エラー", "検査を開始できませんでした。.\n機器の接続を確認してください。");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 終了ボタンクリックイベント
        /// </summary>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            using var confirmDialog = this.FormsHelper.NewConfirmationForm();
            if (DialogResult.OK == confirmDialog.ShowQuestion(this, MessageBoxButtons.OKCancel, "終了確認", "検査を終了しますか?"))
            {
                // 検査制御の終了
                this.closeProcessing();

                #region 統計データの保存
                if (false == (this.AppConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false))
                {
                    var statisticsData = this.UcResultAndStatisticsDisplay.GetStatisticsData();

                    if (statisticsData.LatestDateTime is not null)
                    {
                        var statisticsFile = new FileInfo(Path.Combine(this.AppConfig.Path.Data.FullName, $"statistics.json"));
                        if (false == statisticsData.SaveToFile(statisticsFile))
                        {
                            Serilog.Log.Warning($"{this}, failed to save statistical data.");
                        }
                    }
                }
                #endregion

                #region しきい値の保存
                if (this.buttonJudgementSetting.Tag is not null)
                {
                    var configFile = this.AppConfig.FileInfo;
                    var iniFile = new Library.Common.IO.IniFileReaderWriter(configFile.FullName);

                    if (this.AppConfig.JudgmentParameter is IIniFileCompatible iniParam)
                    {
                        iniParam.Write(iniFile);
                    }

                    iniFile.Save();
                }
                #endregion

                this.Close();
            }
        }

        /// <summary>
        /// 検査設定ボタンクリックイベント
        /// </summary>
        private void buttonJudgementSetting_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.AppConfig.JudgmentParameter is null)
                {
                    throw new InvalidOperationException($"{nameof(this.AppConfig.JudgmentParameter)} is null");
                }

                var settingUi = this.Services?.ServiceProvider?.GetRequiredService<IResultJudgmentParameterForm>();

                if (settingUi is null)
                {
                    throw new InvalidOperationException($"{nameof(settingUi)} is null");
                }

                settingUi.ShowParameter(this.AppConfig.JudgmentParameter);
                settingUi.ParameterChanged += this.ResultJudgmentParameterForm_ParameterChanged;

                using var dialog = settingUi.GetForm();
                dialog?.ShowDialog(this);

                settingUi.ParameterChanged -= this.ResultJudgmentParameterForm_ParameterChanged;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 検査設定変更イベント
        /// </summary>
        private void ResultJudgmentParameterForm_ParameterChanged(object sender)
        {
            try
            {
                if (this.AppConfig.JudgmentParameter is null)
                {
                    return;
                }

                if (sender is IResultJudgmentParameterForm settingUi)
                {
                    settingUi.UpdateParameter(this.AppConfig.JudgmentParameter);

                    Serilog.Log.Information($"{this}, judgment parameter changed.");
                    this.buttonJudgementSetting.Tag = new object(); // 変更が行われたことの記録
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 手動ログ出力ボタンクリックイベント
        /// </summary>
        private void noFocusedRoundedButton1_Click(object sender, EventArgs e) => Serilog.Log.Information($"{this}, 変化点指示");

        /// <summary>
        /// 手動撮影ボタンクリックイベント
        /// </summary>
        private async void buttonGrab_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.TimingInjectedIO is not null && this.AppConfig.InspectionControllerParameter is not null)
                {
                    Serilog.Log.Information($"{this}, 撮影指示");

                    await Task.Run(async () =>
                    {
                        try
                        {
                            this.TimingInjectedIO.SimulateInput(0, 1);

                            await Task.Delay((int)(this.AppConfig.InspectionControllerParameter.AcquisitionTriggerValidHoldingTimeMs * 1.5));

                            this.TimingInjectedIO.SimulateInput(0, 0);
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

        #endregion

        #region IInspectionControllerイベント

        /// <summary>
        /// 検査ステータス変更
        /// </summary>
        private void InspectionController_InspectionStatusChanged(object sender, InspectionEvent @event, IInspectionTask? selectedTask, IInspectionTaskItem? selectedTaskItem)
        {
            try
            {
                switch (@event)
                {
                    // 撮影トリガ信号ON
                    case InspectionEvent.InputSignalOn:
                        {
                            if (selectedTask is not null)
                            {
                                Serilog.Log.Information($"{this}, task no={selectedTask.TaskIndex:D7}, input signal ON");

                                // 撮影のみの場合
                                if (true == (this.AppConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false))
                                {
                                    #region 入力欄の文字列を用いる(画像ファイル名に付与される)
                                    this.InvokeSafely(() =>
                                    {
                                        try
                                        {
                                            if (false == string.IsNullOrEmpty(this.ucImageFileSuffix.Value.Trim()))
                                            {
                                                selectedTask.LinkedInfomation = this.ucImageFileSuffix.Value.Trim();
                                            }
                                            else
                                            {
                                                selectedTask.LinkedInfomation = "NoMark";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Serilog.Log.Warning(ex, ex.Message);
                                        }
                                    });
                                    #endregion
                                }
                            }
                            else
                            {
                                Serilog.Log.Information($"{this}, input signal ON");
                            }

                            this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items[0].Status = DeviceStatusKind.On;
                        }
                        break;

                    // 撮影トリガ信号OFF
                    case InspectionEvent.InputSignalOff:
                        {
                            if (selectedTask is not null)
                            {
                                Serilog.Log.Information($"{this}, task no={selectedTask.TaskIndex:D7}, input signal OFF");
                            }
                            else
                            {
                                Serilog.Log.Information($"{this}, input signal OFF");
                            }

                            this.ucMultiDeviceStatus.Deviecs[DeviceKind.DigitalIO].Items[0].Status = DeviceStatusKind.Off;
                        }
                        break;

                    // カメラ単位撮影完了
                    case InspectionEvent.ItemGrabbed:
                        {
                            try
                            {
                                if (selectedTask is not null && selectedTaskItem is not null && selectedTaskItem.Bitmap is not null)
                                {
                                    Serilog.Log.Debug($"{this}, task no={selectedTask.TaskIndex:D7}, camera={selectedTaskItem.GrabberIndex + 1}, grabbed");
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        }
                        break;

                    // 後処理要求
                    case InspectionEvent.ItemsPostProcessRequest:
                        {
                            try
                            {
                                if (selectedTask is not null && selectedTaskItem is not null)
                                {
                                    var itemId = Array.IndexOf(selectedTask.Items, selectedTaskItem);

                                    // 後処理が存在する場合
                                    if (this.PostProcessings[itemId] is IInferenceResultPostProcessing postProcessing)
                                    {
                                        // 後処理を実行する
                                        postProcessing.ExcecutePostProcessing(selectedTaskItem);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        }
                        break;

                    // 全カメラ撮影完了
                    case InspectionEvent.AllGrabCompleted:
                        {
                            if (selectedTask is not null)
                            {
                                Serilog.Log.Information($"{this}, task no={selectedTask.TaskIndex:D7}, grabbed");
                            }
                        }
                        break;

                    // 画像保存時
                    case InspectionEvent.ImageSaving:
                        {
                            #region 画像保存の有無をここで設定する
                            if (selectedTask is not null)
                            {
                                // 画像保存の有無を設定する
                                switch (this.AppConfig.Operation.ImageSavingCondition)
                                {
                                    // 常に保存
                                    case ImageSavingCondition.Always:
                                        {
                                            selectedTask.ImageSavingEnabled = true;
                                        }
                                        break;

                                    // 一定間隔以上で保存
                                    case ImageSavingCondition.SpecificInterval:
                                        {
                                            var passedTimeSec = (DateTime.Now - this.LastImageSavedDateTime).TotalSeconds;
                                            if (this.AppConfig.Operation.ImageSavingIntervalSeconds <= passedTimeSec)
                                            {
                                                selectedTask.ImageSavingEnabled = true;
                                                this.LastImageSavedDateTime = DateTime.Now;
                                            }
                                            else
                                            {
                                                selectedTask.ImageSavingEnabled = false;
                                            }
                                        }
                                        break;

                                    // 特定の結果の場合に保存
                                    case ImageSavingCondition.SpecificResults:
                                        {
                                            Serilog.Log.Warning($"{this}, not supported image saving condition");
                                            selectedTask.ImageSavingEnabled = false;
                                        }
                                        break;
                                    default:
                                        {
                                            selectedTask.ImageSavingEnabled = false;
                                        }
                                        break;
                                }
                            }
                            #endregion
                        }
                        break;

                    // 判定要求時
                    case InspectionEvent.JudgmentRequest:
                        {
                            try
                            {
                                if (selectedTask is not null && this.AppConfig.JudgmentParameter is not null)
                                {
                                    // 判定処理を実行する
                                    this.InferenceResultJudgment?.ExcecuteJudgment(selectedTask, this.AppConfig.JudgmentParameter);
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        }
                        break;

                    // 判定完了時
                    case InspectionEvent.JudgmentCompleted:
                        {
                        }
                        break;

                    // 結果出力タイミング
                    case InspectionEvent.OutputTiming:
                        {
                            #region 判定結果の外部出力と画面表示の更新
                            try
                            {
                                if (selectedTask is not null && selectedTask.GetInspectionResult() is IInspectionResult result)
                                {
                                    // 1つ前の検査結果データを退避する
                                    var currentResut = this.LatestInspectionResult;

                                    // 最新の検査結果データを保持する
                                    this.LatestInspectionResult = result;

                                    // 検査情報のログ出力
                                    var valuesText = string.Join(",", result.GeneralValues);
                                    Serilog.Log.Information($"{this}, task no={result.TaskIndex:D7}, judgement, class={result.JudgementText}, values=[{valuesText}], timeStamp={result.DateTime:yyyy/MM/dd/ HH:mm:ss}, code={result.LinkedInfomation}");

                                    // 判定結果表示
                                    this.InvokeSafely(() =>
                                    {
                                        try
                                        {
                                            if (true == this.UcResultAndStatisticsDisplay.Visible)
                                            {
                                                // 日次リセットの場合
                                                if (StatisticsResetTiming.Daily == this.AppConfig.GuiOperation.StatisticsResetTiming)
                                                {
                                                    // 日付が変わった場合は統計データをリセットする
                                                    if (this.UcResultAndStatisticsDisplay.LatestDateTime is DateTime validDateTime && validDateTime.Date != result.DateTime.Date)
                                                    {
                                                        #region 念のため、このタイミングでも統計データを保存する
                                                        var statisticsData = this.UcResultAndStatisticsDisplay.GetStatisticsData();

                                                        Task.Run(() =>
                                                        {
                                                            if (statisticsData.LatestDateTime is not null)
                                                            {
                                                                var statisticsFile = new FileInfo(Path.Combine(this.AppConfig.Path.Data.FullName, $"statistics.json"));
                                                                if (false == statisticsData.SaveToFile(statisticsFile))
                                                                {
                                                                    Serilog.Log.Warning($"{this}, failed to save statistical data.");
                                                                }
                                                            }
                                                        });
                                                        #endregion

                                                        // 統計データをリセットする
                                                        this.UcResultAndStatisticsDisplay.ClearResults();
                                                    }
                                                }

                                                // 結果表示
                                                this.UcResultAndStatisticsDisplay.AddResult(result);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Serilog.Log.Warning(ex, ex.Message);
                                        }
                                    });

                                    foreach (var bitmap in this.LatestInspectionResult.Images)
                                    {
                                        var index = Array.IndexOf(this.LatestInspectionResult.Images, bitmap);
                                        this.UcMultiImageView[index].SetImage(bitmap);
                                    }

                                    // 1つ前の検査結果データを破棄する
                                    currentResut?.Dispose();
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                            #endregion
                        }

                        break;

                    // タスク完了時
                    case InspectionEvent.TaskCompleted:
                        {
                            if (selectedTask is not null)
                            {
                                Serilog.Log.Information($"{this}, task no={selectedTask.TaskIndex:D7}, completed");

                                // 画像保存に失敗している場合
                                if (false == selectedTask.ImageSaveSucceeded)
                                {
                                    // 撮影のみの場合(画像収集)
                                    if (true == (this.AppConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false))
                                    {
                                        #region エラーメッセージを表示する
                                        this.InvokeSafely(() =>
                                        {
                                            if (this.UcConfirmation.Visible == false)
                                            {
                                                var errorMessages = new List<string>();
                                                errorMessages.Add($"画像保存に失敗しました。{selectedTask.DateTimeOrigin.ToString("MM月dd日HH時mm分ss秒")}");

                                                // カメラ複数台の場合
                                                if (1 < selectedTask.Items.Length)
                                                {
                                                    var subErrorMessage = $"カメラ ";
                                                    foreach (var item in selectedTask.Items)
                                                    {
                                                        if (false == (item?.ImageSaveSucceeded ?? false))
                                                        {
                                                            subErrorMessage += $"{Array.IndexOf(selectedTask.Items, item) + 1} ";
                                                        }
                                                    }
                                                    errorMessages.Add(subErrorMessage);
                                                }

                                                this.UcConfirmation.MessageFont = new Font(this.Font.FontFamily, 12);
                                                this.UcConfirmation.Left = (this.ClientSize.Width - this.UcConfirmation.Width) / 2;
                                                this.UcConfirmation.Top = (this.ClientSize.Height - this.UcConfirmation.Height) / 2;
                                                this.UcConfirmation.ShowError(this, MessageBoxButtons.OK, "画像保存失敗", $"{string.Join(Environment.NewLine, errorMessages)}");
                                            }
                                        });
                                        #endregion
                                    }
                                }
                            }
                        }
                        break;

                    // 検査無効化
                    default:
                        {
                            Serilog.Log.Information($"{this}, {@event}");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 機器状態変化イベント
        /// </summary>
        private void InspectionController_DeviceStatusChanged(object sender, IController? device, DeviceKind kind, int index, DeviceStatusKind status)
        {
            try
            {
                if (true == this.ucMultiDeviceStatus.Deviecs.ContainsKey(kind))
                {
                    this.ucMultiDeviceStatus.Deviecs[kind].Items[index].Status = status;
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
        /// MultiImageViewControlイベント:描画
        /// </summary>
        private void UcMultiImageView_Drawing(object sender, ImageViewControl view, Library.Common.Drawing.Renderer r, Library.Common.Drawing.Surface s)
        {
            try
            {
                // 検査結果が存在しない場合は何もしない
                if (this.LatestInspectionResult is null)
                {
                    return;
                }

                // 画像表示のインデックスを取得
                var index = this.UcMultiImageView.IndexOf(r);

                // セグメンテーション以外の場合は何もしない
                if (this.LatestInspectionResult.AdditionalData[index] is not SegmentationAdditionalData segmentation)
                {
                    return;
                }

                var drawingRect = new Library.Common.Drawing.Rectangle(s.Location, s.Size);
                var drawingY_Begin = drawingRect.Location.Y - 1;
                var drawingY_End = drawingRect.Location.Y + drawingRect.Size.Height;

                // 検出領域の矩形表示
                foreach (var classLabels in segmentation.LabelsPerClass)
                {
                    foreach (var label in classLabels.Take(100))    // 最大100個まで表示
                    {

                        #region 座標軸に平行な外接矩形
                        {
                            //// 元画像の座標系に変換する
                            //var x = label.Rect.Location.X * segmentation.FitToOriginalScale + segmentation.FitToOriginalPadding.Width;
                            //var y = label.Rect.Location.Y * segmentation.FitToOriginalScale + segmentation.FitToOriginalPadding.Height;
                            //var w = label.Rect.Size.Width * segmentation.FitToOriginalScale;
                            //var h = label.Rect.Size.Height * segmentation.FitToOriginalScale;

                            //s.Pen = new Pen(label.IsNg ? Color.Red : Color.Cyan, 4);
                            //s.Mode = Surface.DrawingMode.Outline;

                            //// 検出領域の外接矩形を描画する
                            //s.DrawRectangle1(x, y, w, h);

                            //#region 検出サイズを表示する:対角線長さ(mm)
                            //{
                            //    var textDiagonalLengthMm = $"{label.DiagonalLengthMm:f2}mm";
                            //    var textPosition = new PointD(x + w, y + h);
                            //    var textCoreColor = label.IsNg ? Color.White : Color.Black;
                            //    var textBackColor = label.IsNg ? Color.Red : Color.Cyan;
                            //    s.WriteOutlineString(textDiagonalLengthMm, textPosition, textCoreColor, textBackColor, 2);
                            //}
                            //#endregion
                        }
                        #endregion

                        #region 角度付き最小外接矩形
                        {
                            #region 塗りつぶしコード                            
                            {
                                //s.Mode = Surface.DrawingMode.Fill;
                                //s.Brush = new SolidBrush(Color.Yellow);
                                //foreach (var run in label.RunLength)
                                //{
                                //    var scaledX = run.X * segmentation.FitToOriginalScale + segmentation.FitToOriginalPadding.Width;
                                //    var scaledY = run.Y * segmentation.FitToOriginalScale + segmentation.FitToOriginalPadding.Height;
                                //    var scaledW = run.Length * segmentation.FitToOriginalScale;

                                //    if (drawingY_Begin > scaledY || drawingY_End < scaledY)
                                //    {
                                //        continue;
                                //    }

                                //    s.DrawRectangle1(scaledX, scaledY, scaledW, segmentation.FitToOriginalScale);
                                //}
                            }
                            #endregion

                            // コーナー座標を取得して元画像の座標系に変換する
                            var points = label.RotatedRect.Corners;
                            for (var i = 0; i < points.Length; i++)
                            {
                                points[i].X = (float)(points[i].X * segmentation.FitToOriginalScale + segmentation.FitToOriginalPadding.Width);
                                points[i].Y = (float)(points[i].Y * segmentation.FitToOriginalScale + segmentation.FitToOriginalPadding.Height);
                            }

                            // 検出領域の外接矩形を描画する
                            s.Pen = new Pen(label.IsNg ? Color.Red : Color.Cyan, 4);
                            s.Mode = Surface.DrawingMode.Outline;
                            s.DrawPolygon(points);

                            #region 検出サイズを表示する:対角線長さ(mm)
                            {
                                var cx = label.RotatedRect.Center.X * segmentation.FitToOriginalScale + segmentation.FitToOriginalPadding.Width;
                                var cy = label.RotatedRect.Center.Y * segmentation.FitToOriginalScale + segmentation.FitToOriginalPadding.Height;

                                var textDiagonalLengthMm = $"{label.DiagonalLengthMm:f2}mm";
                                var textPosition = new PointD(cx, cy);
                                var textCoreColor = label.IsNg ? Color.White : Color.Black;
                                var textBackColor = label.IsNg ? Color.Red : Color.Cyan;
                                s.WriteOutlineString(textDiagonalLengthMm, textPosition, textCoreColor, textBackColor, 2);
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
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

        #region privateメソッド

        /// <summary>
        /// 終了処理
        /// </summary>
        private void closeProcessing()
        {
            try
            {
                using var marqueue = ProgressBarForm.NewMarqueeType(this, "終了中", $"{Environment.NewLine}しばらくお待ちください。");

                var formBlockingAccessor = new FormBlockingAccessor(marqueue);

                Task.Run(async () =>
                {
                    try
                    {
                        // PLC通信
                        if (this.PlcTcpCommunicator is not null)
                        {
                            this.PlcTcpCommunicator.Disconnected -= this.PlcTcpCommunicator_Disconnected;
                            this.PlcTcpCommunicator.Connected -= this.PlcTcpCommunicator_Connected;
                            this.PlcTcpCommunicator.ProcessingStatusChanged -= this.PlcTcpCommunicator_ProcessingStatusChanged;
                        }

                        var deactivateTask = new List<Task>();
                        Task taskItem;

                        // 検査制御
                        taskItem = Task.Run(() =>
                        {
                            try
                            {
                                // 検査制御
                                if (this.InspectionController is not null)
                                {
                                    this.InspectionController.InspectionStatusChanged -= this.InspectionController_InspectionStatusChanged;
                                    this.InspectionController.DeviceStatusChanged -= this.InspectionController_DeviceStatusChanged;

                                    this.InspectionController.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        });
                        deactivateTask.Add(taskItem);

                        // タスク待機
                        await Task.WhenAll(deactivateTask);

                        // PLC通信
                        this.PlcTcpCommunicator?.Close();
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

                this.InspectionController?.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// カメラ画像インデックスを取得する
        /// </summary>
        /// <param name="location">直列化されたインデックス（エリア→ライン）</param>
        /// <returns>2次元カメラインデックス</returns>
        private Library.Common.Drawing.Point getGrabberIndex2D(int serializedIndex)
        {
            var deserializedIndex = Library.Common.Drawing.Point.New();

            var numOfArea = this.AppConfig.AreaSensorControllerParameter?.GrabberParameters.Count ?? 0;

            if (serializedIndex < numOfArea)
            {
                deserializedIndex = new Library.Common.Drawing.Point(0, serializedIndex);
            }
            else
            {
                deserializedIndex = new Library.Common.Drawing.Point(1, serializedIndex - numOfArea);
            }

            return deserializedIndex;
        }

        /// <summary>
        /// カメラ名称を取得する
        /// </summary>
        /// <param name="serializedIndex">直列化されたインデックス（エリア→ライン）</param>
        /// <returns>対応するカメラ名称</returns>
        private string getGrabberNickname(int serializedIndex)
        {
            var index2D = this.getGrabberIndex2D(serializedIndex);

            if (index2D.Y == 0)
            {
                return $"エリアカメラ{index2D.X + 1}";
            }
            else if (index2D.Y == 1)
            {
                return $"ラインセンサ{index2D.X + 1}";
            }
            else
            {
                return $"カメラ{index2D.Y + 1}-{index2D.X + 1}";
            }
        }

        #endregion
    }
}