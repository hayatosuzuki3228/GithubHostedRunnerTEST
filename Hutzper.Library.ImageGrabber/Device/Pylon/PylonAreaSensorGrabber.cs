using Basler.Pylon;
using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageGrabber.Device.Pylon
{
    /// <summary>
    /// Pylonエリアカメラ画像取得
    /// </summary>
    public class PylonAreaSensorGrabber : PylonGrabber, IAreaSensor
    {
        #region IAreaSensor

        /// <summary>
        /// フレームレート
        /// </summary>
        public virtual double FramesPerSecond
        {
            get => (base.GetValue<bool?>(PLCamera.AcquisitionFrameRateEnable, GetValueInfo.IsContain) ?? false) ?
                base.GetValue<long?>(PLCamera.AcquisitionFrameRateAbs) ?? 0d :
                base.GetValue<long?>(PLCamera.AcquisitionFrameRateAbs, GetValueInfo.Maximum) ?? 0d;

            set
            {
                if (this.Handles.Device is null) return;
                if (0 >= value) base.SetValue(PLCamera.AcquisitionFrameRateEnable, false);
                else
                {
                    base.SetValue(PLCamera.AcquisitionFrameRateEnable, true);
                    if (base.GetValue<bool>(PLCamera.AcquisitionFrameRateAbs, GetValueInfo.IsContain))
                    {
                        base.SetValue(PLCamera.AcquisitionFrameRateAbs, System.Math.Clamp(value, this.GetValue<double>(PLCamera.AcquisitionFrameRateAbs, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.AcquisitionFrameRateAbs, GetValueInfo.Maximum)));
                    }
                    else if (base.GetValue<bool>(PLCamera.AcquisitionFrameRate, GetValueInfo.IsContain))
                    {
                        base.SetValue(PLCamera.AcquisitionFrameRate, System.Math.Clamp(value, this.GetValue<double>(PLCamera.AcquisitionFrameRate, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.AcquisitionFrameRate, GetValueInfo.Maximum)));
                    }
                }
            }
        }

        #endregion

        #region IGrabber

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            if (!base.Open()) return false;
            if (this.Handles.Device is null) return false;
            this.SetValue(PLCamera.TriggerSelector, PLCamera.TriggerSelector.FrameStart, true);
            if (this.Parameter is IAreaSensorParameter gp) this.FramesPerSecond = gp.FramesPerSecond;
            return this.Enabled;
        }

        #endregion

        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            base.Initialize(serviceCollection);
            if (true != this.Enabled) return;
            if (this.Parameter is IAreaSensorParameter gp) this.FramesPerSecond = gp.FramesPerSecond;
        }

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            base.SetParameter(parameter);
            if (parameter is IAreaSensorParameter gp) this.FramesPerSecond = gp.FramesPerSecond;
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PylonAreaSensorGrabber() => this.GrabberType = GrabberType.AreaSensor;
    }
}