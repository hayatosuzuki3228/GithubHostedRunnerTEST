using Hutzper.Library.Common;
using Hutzper.Library.Common.IO;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.DigitalIO;
using Hutzper.Library.DigitalIO.IO.Configuration;
using Hutzper.Library.ImageGrabber;
using Hutzper.Library.ImageGrabber.IO.Configuration;
using Hutzper.Library.InsightLinkage.Controller;
using Hutzper.Library.LightController;
using Hutzper.Library.LightController.IO.Configuration;
using Hutzper.Library.Onnx;
using Hutzper.Sample.Mekiki.ScratchLegacy.Controller;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration
{
    /// <summary>
    /// システム設定
    /// </summary>
    [Serializable]
    public class ProjectConfiguration : ApplicationConfig
    {
        /// <summary>
        /// 運用設定
        /// </summary>
        public OperationSetting Operation { get; init; }

        /// <summary>
        /// カメラ構成
        /// </summary>
        public CameraConfiguration CameraConfiguration { get; init; }

        /// <summary>
        /// 照明構成
        /// </summary>
        public LightConfiguration LightConfiguration { get; init; }

        /// <summary>
        /// デジタルIO構成
        /// </summary>
        public DigitalIOConfiguration DigitalIOConfiguration { get; init; }

        /// <summary>
        /// 検査制御パラメータ
        /// </summary>
        public IInspectionControllerParameter? InspectionControllerParameter { get; init; }

        /// <summary>
        /// エリアセンサパラメータ
        /// </summary>
        public IAreaSensorControllerParameter? AreaSensorControllerParameter { get; init; }

        /// <summary>
        /// ラインセセンサパラメータ
        /// </summary>
        public ILineSensorControllerParameter? LineSensorControllerParameter { get; init; }

        /// <summary>
        /// DIOパラメータ
        /// </summary>
        public IDigitalIODeviceControllerParameter? DigitalIODeviceControllerParameter { get; init; }

        /// <summary>
        /// 照明パラメータ
        /// </summary>
        public ILightControllerSupervisorParameter? LightControllerParameter { get; init; }

        /// <summary>
        /// ONNXパラメータ
        /// </summary>
        public IOnnxModelControllerParameter? OnnxModelControllerParameter { get; init; }

        /// <summary>
        /// Insight連携パラメータ
        /// </summary>
        public IInsightLinkageControllerParameter? InsightLinkageControllerParameter { get; init; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProjectConfiguration(IServiceCollectionSharing? serviceCollectionSharing, string fullFileName) : base(serviceCollectionSharing, fullFileName)
        {
            this.Items.Add(this.Operation = new());
            this.Items.Add(this.CameraConfiguration = new());
            this.Items.Add(this.LightConfiguration = new());
            this.Items.Add(this.DigitalIOConfiguration = new());

            // 検査制御パラメータ
            this.InspectionControllerParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<IInspectionControllerParameter>();
            if (this.InspectionControllerParameter is IIniFileCompatible iniInspectionControllerParameter)
            {
                this.Items.Add(iniInspectionControllerParameter);
            }

            // エリアセンサパラメータ
            this.AreaSensorControllerParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<IAreaSensorControllerParameter>();
            if (this.AreaSensorControllerParameter is IIniFileCompatible iniAreaSensorControllerParameter)
            {
                this.Items.Add(iniAreaSensorControllerParameter);
            }

            // ラインセンサパラメータ
            this.LineSensorControllerParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<ILineSensorControllerParameter>();
            if (this.LineSensorControllerParameter is IIniFileCompatible initLineSensorControllerParameter)
            {
                this.Items.Add(initLineSensorControllerParameter);
            }

            // DIOパラメータ
            this.DigitalIODeviceControllerParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<IDigitalIODeviceControllerParameter>();
            if (this.DigitalIODeviceControllerParameter is IIniFileCompatible iniDigitalIODeviceControllerParameter)
            {
                this.Items.Add(iniDigitalIODeviceControllerParameter);
            }

            // 照明パラメータ
            this.LightControllerParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<ILightControllerSupervisorParameter>();
            if (this.LightControllerParameter is IIniFileCompatible iniLightControllerParameter)
            {
                this.Items.Add(iniLightControllerParameter);
            }

            // ONNXパラメータ
            this.OnnxModelControllerParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<IOnnxModelControllerParameter>();
            if (this.OnnxModelControllerParameter is IIniFileCompatible iniOnnxControllerParameter)
            {
                this.Items.Add(iniOnnxControllerParameter);
            }

            // Insight連携パラメータ
            this.InsightLinkageControllerParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<IInsightLinkageControllerParameter>();
            if (this.InsightLinkageControllerParameter is IIniFileCompatible iniInsightLinkageControllerParameter)
            {
                this.Items.Add(iniInsightLinkageControllerParameter);
            }
        }
    }
}