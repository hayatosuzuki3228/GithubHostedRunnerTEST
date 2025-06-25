using Hutzper.Library.Common;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Laungage;
using Hutzper.Library.Common.Windows;
using Hutzper.Library.DigitalIO;
using Hutzper.Library.DigitalIO.Device;
using Hutzper.Library.DigitalIO.Device.Hutzper;
using Hutzper.Library.DigitalIO.Device.Moxa;
using Hutzper.Library.DigitalIO.Device.Plc.Mitsubishi;
using Hutzper.Library.DigitalIO.Device.Y2;
using Hutzper.Library.DigitalIO.IO.Configuration;
using Hutzper.Library.ImageGrabber;
using Hutzper.Library.ImageGrabber.Device;
using Hutzper.Library.ImageGrabber.Device.File;
using Hutzper.Library.ImageGrabber.Device.GigE.Jai;
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
using Hutzper.Library.LightController.Device.Net.Sockets.Ccs;
using Hutzper.Library.LightController.Device.Net.Sockets.OptexFA;
using Hutzper.Library.LightController.Device.Net.Sockets.UTech;
using Hutzper.Library.LightController.Device.Net.Sockets.Vst;
using Hutzper.Library.LightController.IO.Configuration;
using Hutzper.Project.Mekiki.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows.Forms;

namespace Hutzper.Project.Mekiki.Services.Application;

static public class LaunchPrepare
{
    static SelfBootController? _selfBootController;
    static public void ServiceCollectionForm(IServiceCollection services)
    {
        // Viewを登録
        services.AddSingleton<MainWindow>();

        // ViewModelを登録
        services.AddSingleton<MainViewModel>();
    }

    static public void ServiceCollection(IServiceCollection s)
    {
        #region サービスを追加(DIの指定)
        {
            // Form
            //// Recipe
            //s.AddSingleton<IConfigurationCollection, ConfigurationCollection>();

            // 翻訳
            s.AddSingleton<ITranslator, Translator>();

            // 共有サービス
            s.AddTransient<IServiceCollectionSharing, ServiceCollectionSharing>(p => ServiceCollectionSharing.Instance);

            #region パス
            {
                // パス管理
                s.AddTransient<IPathManager, PathManager>();
            }
            #endregion

            // アプリ設定
            var configFile = ApplicationConfig.GetStandardConfigFileInfo();

            #region IAreaSensorController
            {
                var config = new CameraConfiguration();
                config.Read(new Library.Common.IO.IniFileReaderWriter(configFile.FullName));

                s.AddSingleton<IAreaSensorController, AreaSensorController>(provider =>
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
                s.AddTransient<IAreaSensorControllerParameter, AreaSensorrControllerParameter>(provider =>
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

                s.AddSingleton<ILineSensorController, LineSensorController>(provider =>
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
                s.AddTransient<ILineSensorControllerParameter, LineSensorrControllerParameter>(provider =>
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

                s.AddTransient<IDigitalIODeviceControllerParameter, DigitalIODeviceControllerParameter>(provider =>
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
                        else if (true == deviceName.Contains("Y2".ToLower()))
                        {
                            param = new Y2DigitalIODeviceParameter(new Library.Common.Drawing.Point(index, 0));
                        }
                        else if (true == deviceName.Contains("Plc".ToLower()))
                        {
                            param = new MitsubishiPlcDeviceParameter(new Library.Common.Drawing.Point(index, 0));
                        }
                        else
                        {
                            param = new HutzperDigitalIOTcpClientParameter(new Library.Common.Drawing.Point(index, 0));
                        }

                        if (param is not null)
                        {
                            cp.DeviceParameters.Add(param);
                        }
                    }

                    return cp;
                });
                s.AddSingleton<IDigitalIODeviceController, DigitalIODeviceController>(provider =>
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
                        else if (true == deviceName.Contains("Y2".ToLower()))
                        {
                            device = new Y2DigitalIODevice();
                        }
                        else if (true == deviceName.Contains("Plc".ToLower()))
                        {
                            device = new MitsubishiPlcDevice();
                        }
                        else
                        {
                            device = new HutzperDigitalIOTcpClient();
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

                s.AddSingleton<ILightControllerSupervisor, LightControllerSupervisor>(provider =>
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
                        else if (true == deviceName.Contains("UTechnologyUPD2450".ToLower()))
                        {
                            device = new TcpUTechUpd2450_LightController();
                        }
                        else if (true == deviceName.Contains("Vst_Vps24X0".ToLower()))
                        {
                            device = new TcpVstVps24X0_LightController();
                        }

                        if (device is not null)
                        {
                            controller.Attach(device);
                        }
                    }

                    return controller;
                });
                s.AddTransient<ILightControllerSupervisorParameter, LightControllerSupervisorParameter>(provider =>
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
                        else if (true == deviceName.Contains("UTech_UPD2450".ToLower()))
                        {
                            param = new TcpCommunicationParameter(new Library.Common.Drawing.Point(index, 0));
                        }
                        else if (true == deviceName.Contains("Vst_Vps24X0".ToLower()))
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

            #region IInsightLinkageController
            {
                s.AddTransient<IFileUploader, AwsS3FileUploader>();

                string accessKeyID = Path.Combine(Directory.GetCurrentDirectory(), @".\AccessKeyID");
                string secretKey = Path.Combine(Directory.GetCurrentDirectory(), @".\SecretKey");
                s.AddTransient<IFileUploaderParameter>(provider => new AwsS3FileUploaderParameter(accessKeyID, secretKey));

                s.AddTransient<ITextMessenger, MqttTextMessenger>();
                s.AddTransient<ITextMessengerParameter, MqttTextMessengerParameter>();

                s.AddSingleton<IInsightLinkageController, InsightLinkageController>(provider =>
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
                s.AddTransient<IInsightLinkageControllerParameter, InsightLinkageControllerParameter>(provider =>
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

            //IInsightMekikiData
            s.AddTransient<IInsightMekikiData, InsightMekikiData>();

            // サービスの作成
            ServiceCollectionSharing.Instance.BuildServiceProvider();
        }
        #endregion
    }

    static public void StartupApplication(string launchNo)
    {
        // 起動制御
        _selfBootController = new SelfBootController(launchNo);
        if (!Control.ModifierKeys.HasFlag(Keys.Shift))
        {
            if (false == _selfBootController?.CanBootApplication)
            {
                Environment.Exit(0);
                return;
            }
        }

        // タイマの精度変更
        WinSystemController.TimeBeginPeriod();
        ApplicationConfiguration.Initialize();
    }
    static public void CleanupApplication()
    {
        _selfBootController?.NotifyExitApplication();
        // タイマの精度変更
        WinSystemController.TimeEndPeriod();
        Task.Delay(1000).ConfigureAwait(false).GetAwaiter().GetResult();
    }

}
