using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller.Plc;
using Hutzper.Library.Common.Diagnostics;
using Hutzper.Library.Common.Forms;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Net.Sockets;
using Hutzper.Library.Common.Windows;
using Hutzper.Library.DigitalIO;
using Hutzper.Library.DigitalIO.Device;
using Hutzper.Library.DigitalIO.Device.Hutzper;
using Hutzper.Library.DigitalIO.Device.Moxa;
using Hutzper.Library.DigitalIO.IO.Configuration;
using Hutzper.Library.ImageGrabber;
using Hutzper.Library.ImageGrabber.Device;
using Hutzper.Library.ImageGrabber.Device.File;
using Hutzper.Library.ImageGrabber.Device.GigE.Jai;
using Hutzper.Library.ImageGrabber.Device.GigE.Pylon;
using Hutzper.Library.ImageGrabber.Device.GigE.Sentech;
using Hutzper.Library.ImageGrabber.Device.USB.Pylon;
using Hutzper.Library.ImageGrabber.Device.USB.Sentech;
using Hutzper.Library.ImageGrabber.IO.Configuration;
using Hutzper.Library.Insight.Controller;
using Hutzper.Library.InsightLinkage.Connection;
using Hutzper.Library.InsightLinkage.Connection.Aws.S3;
using Hutzper.Library.InsightLinkage.Connection.Mqtt;
using Hutzper.Library.InsightLinkage.Controller;
using Hutzper.Library.InsightLinkage.Data;
using Hutzper.Library.LightController;
using Hutzper.Library.LightController.Device;
using Hutzper.Library.LightController.Device.Net.Sockets;
using Hutzper.Library.LightController.Device.Net.Sockets.Aitec;
using Hutzper.Library.LightController.Device.Net.Sockets.Ccs;
using Hutzper.Library.LightController.Device.Net.Sockets.Hutzper;
using Hutzper.Library.LightController.Device.Net.Sockets.OptexFA;
using Hutzper.Library.LightController.Device.Net.Sockets.Revox;
using Hutzper.Library.LightController.Device.Net.Sockets.UTech;
using Hutzper.Library.LightController.Device.Net.Sockets.Vst;
using Hutzper.Library.LightController.IO.Configuration;
using Hutzper.Library.Onnx;
using Hutzper.Library.Onnx.Data;
using Hutzper.Library.Onnx.Model;
using Hutzper.Sample.Mekiki.ScratchLegacy.Controller;
using Hutzper.Sample.Mekiki.ScratchLegacy.Forms;
using Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber;
using Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection;
using Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Lighting;
using Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement;
using Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // 起動制御
            var selfBootController = new SelfBootController(Application.ProductName ?? "hutzper");
            if (!Control.ModifierKeys.HasFlag(Keys.Shift))
            {
                if (false == selfBootController.CanBootApplication)
                {
                    return;
                }
            }

            // タイマの精度変更
            WinSystemController.TimeBeginPeriod();

            // 例外の補足
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

            ApplicationConfiguration.Initialize();

            try
            {
                #region サービスを追加(DIの指定)
                {
                    // Form
                    ServiceCollectionSharing.Instance.ServiceCollection.AddSingleton<MenuForm>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<OperationForm>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<MaintenanceForm>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<ConfigurationForm>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<ConfirmationForm>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IGrabberParameterForm, GrabberParameterForm>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<ILightParameterForm, LightParameterForm>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<GigESentechLineSensorFFC_Form>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IResultJudgmentParameterForm, ResultJudgmentParameterForm>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<PlcDiagnosticsForm>();

                    // 共有サービス
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IServiceCollectionSharing, ServiceCollectionSharing>(p => ServiceCollectionSharing.Instance);

                    #region パス
                    {
                        // パス管理
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IPathManager, PathManager>();
                    }
                    #endregion

                    // アプリ設定
                    var configFile = ApplicationConfig.GetStandardConfigFileInfo();

                    #region IAreaSensorController
                    {
                        var config = new CameraConfiguration();
                        config.Read(new Library.Common.IO.IniFileReaderWriter(configFile.FullName));

                        ServiceCollectionSharing.Instance.ServiceCollection.AddSingleton<IAreaSensorController, AreaSensorController>(provider =>
                        {
                            var controller = new AreaSensorController();

                            foreach (var index in Enumerable.Range(0, config.AreaSensors.Length))
                            {
                                var device = (IAreaSensor?)null;

                                var deviceName = config.AreaSensors[index].Trim().ToLower();
                                if (true == string.IsNullOrEmpty(deviceName))
                                {
                                    continue;
                                }

                                var isGigE = deviceName.Contains("GigE".ToLower());

                                if (true == deviceName.Contains("Sentech".ToLower()))
                                {
                                    device = isGigE ? new GigESentechAreaSensorGrabber() : new UsbSentechAreaSensorGrabber();
                                }
                                else if (true == deviceName.Contains("Pylon".ToLower()))
                                {
                                    device = isGigE ? new GigEPylonAreaSensorGrabber() : new UsbPylonAreaSensorGrabber();
                                }
                                else if (true == deviceName.Contains("Jai".ToLower()))
                                {
                                    device = isGigE ? new GigEJaiAreaSensorGrabber() : null;
                                }
                                else if (true == deviceName.Contains("File".ToLower()))
                                {
                                    device = new AreaFileGrabber();
                                }

                                if (device is not null)
                                {
                                    controller.Attach(device);
                                }
                            }

                            return controller;
                        });
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IAreaSensorControllerParameter, AreaSensorrControllerParameter>(provider =>
                        {
                            var cp = new AreaSensorrControllerParameter();

                            foreach (var index in Enumerable.Range(0, config.AreaSensors.Length))
                            {
                                var param = (IAreaSensorParameter?)null;

                                var deviceName = config.AreaSensors[index].Trim().ToLower();
                                if (true == string.IsNullOrEmpty(deviceName))
                                {
                                    continue;
                                }

                                var isGigE = deviceName.Contains("Gige".ToLower());

                                if (true == deviceName.Contains("Sentech".ToLower()))
                                {
                                    param = isGigE ? new GigESentechAreaSensorGrabberParameter(new Library.Common.Drawing.Point(index, 0)) : new UsbSentechAreaSensorGrabberParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("Pylon".ToLower()))
                                {
                                    param = isGigE ? new GigEPylonAreaSensorGrabberParameter(new Library.Common.Drawing.Point(index, 0)) : new UsbPylonAreaSensorGrabberParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("Jai".ToLower()))
                                {
                                    param = isGigE ? new GigEJaiAreaSensorGrabberParameter(new Library.Common.Drawing.Point(index, 0)) : null;
                                }
                                else if (true == deviceName.Contains("File".ToLower()))
                                {
                                    param = new AreaFileGrabberParameter(new Library.Common.Drawing.Point(index, 0));
                                }

                                if (param is not null)
                                {
                                    cp.GrabberParameters.Add(param);
                                }
                            }

                            return cp;
                        });
                    }
                    #endregion

                    #region ILineSensorController
                    {
                        var config = new CameraConfiguration();
                        config.Read(new Library.Common.IO.IniFileReaderWriter(configFile.FullName));

                        ServiceCollectionSharing.Instance.ServiceCollection.AddSingleton<ILineSensorController, LineSensorController>(provider =>
                        {
                            var controller = new LineSensorController();

                            foreach (var index in Enumerable.Range(0, config.LineSensors.Length))
                            {
                                var device = (ILineSensor?)null;

                                var deviceName = config.LineSensors[index].Trim().ToLower();
                                if (true == string.IsNullOrEmpty(deviceName))
                                {
                                    continue;
                                }

                                var isGigE = deviceName.Contains("GigE".ToLower());

                                if (true == deviceName.Contains("Sentech".ToLower()))
                                {
                                    device = isGigE ? new GigESentechLineSensorGrabber() : null;
                                }
                                else if (true == deviceName.Contains("Pylon".ToLower()))
                                {
                                    device = new GigEPylonLineSensorGrabber();
                                }
                                else if (true == deviceName.Contains("File".ToLower()))
                                {
                                    device = new LineFileGrabber();
                                }

                                if (device is not null)
                                {
                                    controller.Attach(device);
                                }
                            }

                            return controller;
                        });
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<ILineSensorControllerParameter, LineSensorrControllerParameter>(provider =>
                        {
                            var cp = new LineSensorrControllerParameter();

                            foreach (var index in Enumerable.Range(0, config.LineSensors.Length))
                            {
                                var param = (ILineSensorParameter?)null;

                                var deviceName = config.LineSensors[index].Trim().ToLower();
                                if (true == string.IsNullOrEmpty(deviceName))
                                {
                                    continue;
                                }

                                var isGigE = deviceName.Contains("GigE".ToLower());

                                if (true == deviceName.Contains("Sentech".ToLower()))
                                {
                                    param = isGigE ? new GigESentechLineSensorGrabberParameter(new Library.Common.Drawing.Point(index, 1)) : null;
                                }
                                else if (true == deviceName.Contains("Pylon".ToLower()))
                                {
                                    param = new GigEPylonLineSensorGrabberParameter();
                                }
                                else if (true == deviceName.Contains("File".ToLower()))
                                {
                                    param = new LineFileGrabberParameter(new Library.Common.Drawing.Point(index, 1));
                                }

                                if (param is not null)
                                {
                                    cp.GrabberParameters.Add(param);
                                }
                            }

                            return cp;
                        });
                    }
                    #endregion

                    #region IDigitalIODeviceController
                    {
                        var config = new DigitalIOConfiguration();
                        config.Read(new Library.Common.IO.IniFileReaderWriter(configFile.FullName));

                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IDigitalIODeviceControllerParameter, DigitalIODeviceControllerParameter>(provider =>
                        {
                            var cp = new DigitalIODeviceControllerParameter();

                            foreach (var index in Enumerable.Range(0, config.Devices.Length))
                            {
                                var param = (IDigitalIODeviceParameter?)null;

                                var deviceName = config.Devices[index].Trim().ToLower();
                                if (true == string.IsNullOrEmpty(deviceName))
                                {
                                    continue;
                                }

                                if (true == deviceName.Contains("MoxaE1200".ToLower()))
                                {
                                    param = new MoxaE1200DigitalIORemoteDeviceParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("Hutzper".ToLower()))
                                {
                                    param = new HutzperDigitalIOTcpClientParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("Manual".ToLower()))
                                {
                                    param = new HutzperTimingInjectedIOParameter(new Library.Common.Drawing.Point(index, 0));
                                }

                                if (param is not null)
                                {
                                    cp.DeviceParameters.Add(param);
                                }
                            }

                            return cp;
                        });
                        ServiceCollectionSharing.Instance.ServiceCollection.AddSingleton<IDigitalIODeviceController, DigitalIODeviceController>(provider =>
                        {
                            var controller = new DigitalIODeviceController();

                            foreach (var index in Enumerable.Range(0, config.Devices.Length))
                            {
                                var device = (IDigitalIODevice?)null;

                                var deviceName = config.Devices[index].Trim().ToLower();
                                if (true == string.IsNullOrEmpty(deviceName))
                                {
                                    continue;
                                }

                                if (true == deviceName.Contains("MoxaE1200".ToLower()))
                                {
                                    device = new MoxaE1200DigitalIORemoteDevice();
                                }
                                else if (true == deviceName.Contains("Hutzper".ToLower()))
                                {
                                    device = new HutzperDigitalIOTcpClient();
                                }
                                else if (true == deviceName.Contains("Manual".ToLower()))
                                {
                                    device = new HutzperTimingInjectedIO();
                                }

                                if (device is not null)
                                {
                                    controller.Attach(device);
                                }
                            }

                            return controller;
                        });
                    }
                    #endregion

                    #region ILightControllerSupervisor
                    {
                        var config = new LightConfiguration();
                        config.Read(new Library.Common.IO.IniFileReaderWriter(configFile.FullName));

                        ServiceCollectionSharing.Instance.ServiceCollection.AddSingleton<ILightControllerSupervisor, LightControllerSupervisor>(provider =>
                        {
                            var controller = new LightControllerSupervisor();

                            foreach (var index in Enumerable.Range(0, config.Devices.Length))
                            {
                                var device = (ILightController?)null;

                                var deviceName = config.Devices[index].Trim().ToLower();
                                if (true == string.IsNullOrEmpty(deviceName))
                                {
                                    continue;
                                }

                                if (true == deviceName.Contains("CcsPSB4".ToLower()))
                                {
                                    device = new TcpCcsPSB4LightController();
                                }
                                else if (true == deviceName.Contains("CcsPD3".ToLower()))
                                {
                                    device = new TcpCcsPD3LightController();
                                }
                                else if (true == deviceName.Contains("CcsPD4".ToLower()))
                                {
                                    device = new TcpCcsPD4LightController();
                                }
                                else if (true == deviceName.Contains("OptexFA_OPPX".ToLower()))
                                {
                                    device = new TcpOptexFA_OPPX_LightController();
                                }
                                else if (true == deviceName.Contains("AitecLPDCK".ToLower()))
                                {
                                    device = new TcpAitecLPDCK_LightController();
                                }
                                else if (true == deviceName.Contains("RevoxCBTB".ToLower()))
                                {
                                    device = new TcpRevoxCBTB_LightController();
                                }
                                else if (true == deviceName.Contains("UTechUpd2450".ToLower()))
                                {
                                    device = new TcpUTechUpd2450_LightController();
                                }
                                else if (true == deviceName.Contains("VstVps24X0".ToLower()))
                                {
                                    device = new TcpVstVps24X0_LightController();
                                }
                                else if (true == deviceName.Contains("HutzperMock".ToLower()))
                                {
                                    device = new TcpHutzperMock_LightController();
                                }

                                if (device is not null)
                                {
                                    controller.Attach(device);
                                }
                            }

                            return controller;
                        });
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<ILightControllerSupervisorParameter, LightControllerSupervisorParameter>(provider =>
                        {
                            var cp = new LightControllerSupervisorParameter();

                            foreach (var index in Enumerable.Range(0, config.Devices.Length))
                            {
                                var param = (ILightControllerParameter?)null;

                                var deviceName = config.Devices[index].Trim().ToLower();
                                if (true == string.IsNullOrEmpty(deviceName))
                                {
                                    continue;
                                }

                                if (true == deviceName.Contains("CcsPSB4".ToLower()))
                                {
                                    param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("CcsPD3".ToLower()))
                                {
                                    param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("CcsPD4".ToLower()))
                                {
                                    param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("OptexFA_OPPX".ToLower()))
                                {
                                    param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("AitecLPDCK".ToLower()))
                                {
                                    param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("RevoxCBTB".ToLower()))
                                {
                                    param = new TcpRevoxCBTB_Parameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("UTechUpd2450".ToLower()))
                                {
                                    param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("VstVps24X0".ToLower()))
                                {
                                    param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                                }
                                else if (true == deviceName.Contains("HutzperMock".ToLower()))
                                {
                                    param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                                }

                                if (param is not null)
                                {
                                    cp.DeviceParameters.Add(param);
                                }
                            }

                            return cp;
                        });
                    }
                    #endregion

                    #region IOnnxModelController
                    {
                        var config = new InspectionControllerParameter();
                        config.Read(new Library.Common.IO.IniFileReaderWriter(configFile.FullName));

                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IOnnxModel, OnnxModelGpuCuda>();

                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IOnnxDataInput, OnnxDataInputBitmap>();

                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IOnnxModelControllerParameter, OnnxModelControllerParameterBase>(provider =>
                        {
                            var cp = new OnnxModelControllerParameterBase();

                            foreach (var index in Enumerable.Range(0, config.InferenceIndecies.Length))
                            {
                                var mp = new OnnxModelParameterBase(new Library.Common.Drawing.Point(index, 0));

                                cp.ModelParameters.Add(mp);
                            }

                            return cp;
                        });
                        ServiceCollectionSharing.Instance.ServiceCollection.AddSingleton<IOnnxModelController, OnnxModelControllerBase>(provider =>
                        {
                            var controller = new OnnxModelControllerBase();

                            foreach (var index in Enumerable.Range(0, config.InferenceIndecies.Length))
                            {
                                if (provider.GetRequiredService<IOnnxModel>() is IOnnxModel model)
                                {
                                    controller.Attach(model);
                                }
                            }

                            return controller;
                        });
                    }
                    #endregion

                    #region IInsightLinkageController
                    {
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IFileUploader, AwsS3FileUploader>();
                        string accessKeyID = Path.Combine(Directory.GetCurrentDirectory(), @".\AccessKeyID");
                        string secretKey = Path.Combine(Directory.GetCurrentDirectory(), @".\SecretKey");
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IFileUploaderParameter>(provider => new AwsS3FileUploaderParameter(accessKeyID, secretKey));

                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<ITextMessenger, MqttTextMessenger>();
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<ITextMessengerParameter, MqttTextMessengerParameter>();

                        ServiceCollectionSharing.Instance.ServiceCollection.AddSingleton<IInsightLinkageController, InsightLinkageController>(provider =>
                        {
                            var controller = new InsightLinkageController();

                            if (provider.GetRequiredService<IFileUploader>() is IFileUploader fileUploader)
                            {
                                controller.Attach(fileUploader);
                            }

                            if (provider.GetRequiredService<ITextMessenger>() is ITextMessenger textMessenger)
                            {
                                controller.Attach(textMessenger);
                            }

                            return controller;
                        });
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IInsightLinkageControllerParameter, InsightLinkageControllerParameter>(provider =>
                        {
                            var p = new InsightLinkageControllerParameter();

                            if (provider.GetRequiredService<IFileUploaderParameter>() is IFileUploaderParameter fp)
                            {
                                p.ConnectionParameters.Add(fp);
                            }

                            if (provider.GetRequiredService<ITextMessengerParameter>() is ITextMessengerParameter tp)
                            {
                                p.ConnectionParameters.Add(tp);
                            }

                            return p;
                        });
                    }
                    #endregion

                    //IInspectionController
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IInspectionController, InspectionController>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IInspectionControllerParameter, InspectionControllerParameter>();

                    //IInsightMekikiData
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IInsightMekikiData, InsightMekikiData>();

                    // IPlcTcpCommunicator
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IPlcTcpCommunicator, PlcTcpCommunicator<PlcDeviceReaderWriterMcp>>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IPlcTcpCommunicatorParameter, PlcTcpCommunicatorParameter>();

                    // IInferenceResultJudgment
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IInferenceResultJudgment, SampleJudgement>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IInferenceResultJudgementClass, SampleJudgementClass>();
                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IInferenceResultJudgmentParameter, SampleJudgementParameter>();

                    // サービスの作成
                    ServiceCollectionSharing.Instance.BuildServiceProvider();
                }
                #endregion

                // アプリ設定
                var appConfigFile = ApplicationConfig.GetStandardConfigFileInfo();
                var appConfig = new ProjectInspectionSetting(ServiceCollectionSharing.Instance, appConfigFile.FullName);
                appConfig.Load();
                appConfig.Save();

                #region パスの作成
                foreach (var path in appConfig.Path)
                {
                    try
                    {
                        if (false == path.Value.Exists)
                        {
                            path.Value.Create();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                #endregion

                // ログの設定
                UseSerilog.StartLogger();

                // ビルド情報
                var processBits = Environment.Is64BitProcess ? "64bit" : "32bit";
                var processMode = "release build";
#if DEBUG
                processMode = "debug build";
#endif
                //バージョンの取得
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                var version = asm.GetName().Version;

                // 起動ログ出力
                Serilog.Log.Information($"application start ver.{version?.ToString() ?? Application.ProductVersion} ({processBits} {processMode})");

                // 起動フォーム生成
                var form = ServiceCollectionSharing.Instance.ServiceProvider?.GetRequiredService<MenuForm>();

                // 実行
                Application.Run(form!);

                // 再起動要求の場合
                if (null != form && DialogResult.Abort == form.DialogResult)
                {
                    // アプリケーション再起動ログ出力
                    Serilog.Log.Information("application reboot");

                    SelfBootController.RebootApplication();
                }
            }
            finally
            {
                Serilog.Log.Information("application end");

                selfBootController?.NotifyExitApplication();

                // タイマの精度変更
                WinSystemController.TimeEndPeriod();
            }
        }

        /// 処理されていない例外をキャッチします。
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine("UnhandledException: " + e.ExceptionObject.ToString() + ". Terminating:" + e.IsTerminating);
            if (e.ExceptionObject is Exception ex)
            {
                HandleException("UnhandledException", ex);
            }
            Environment.Exit(0);
        }

        /// 処理されていないThreadの例外をキャッチします。
        private static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException("ThreadException", e.Exception);
            Application.Exit();
        }

        private static void HandleException(string exceptionType, Exception ex)
        {
            // ログ出力とダンプの処理
            Serilog.Log.Fatal(ex, $"{exceptionType},{ex.Message}");

            SentrySdk.Flush();
            Serilog.Log.CloseAndFlush();
        }
    }
}