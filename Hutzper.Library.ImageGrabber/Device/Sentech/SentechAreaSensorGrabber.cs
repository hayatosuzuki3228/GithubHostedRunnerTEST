using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageGrabber.Device.Sentech
{
    /// <summary>
    /// Sentechエリアカメラ画像取得
    /// </summary>
    public class SentechAreaSensorGrabber : SentechGrabber, IAreaSensor
    {
        #region IAreaSensor

        /// <summary>
        /// フレームレート
        /// </summary>
        public virtual double FramesPerSecond
        {
            get
            {
                double value = 0d;
                base.GetValue("AcquisitionFrameRate", out value);
                return value;
            }

            set
            {
                double min = 0, max = 0;
                base.GetValue("AcquisitionFrameRate", out double _, out min, out max);
                if (0 >= value && 0 < max) base.SetValue("AcquisitionFrameRate", (double)(int)max);
                else base.SetValue("AcquisitionFrameRate", System.Math.Clamp(value, (int)min, (int)max));
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
            try
            {
                if (base.Open() && null != this.Handles.Device && this.Parameter is IAreaSensorParameter gp) this.FramesPerSecond = gp.FramesPerSecond;
            }
            catch (Exception ex)
            {
                this.Close();
                Serilog.Log.Warning(ex, ex.Message);
            }
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
            if (true == this.Enabled && this.Parameter is IAreaSensorParameter gp) this.FramesPerSecond = gp.FramesPerSecond;
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
        public SentechAreaSensorGrabber() => this.GrabberType = GrabberType.AreaSensor;
    }
}