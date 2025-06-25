using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageGrabber.Device.EBus
{
    /// <summary>
    /// Pylonエリアカメラ画像取得
    /// </summary>
    public class EBusAreaSensorGrabber : EBusGrabber, IAreaSensor
    {
        #region IAreaSensor

        /// <summary>
        /// フレームレート
        /// </summary>
        public virtual double FramesPerSecond
        {
            get
            {
                var value = 0d;

                try
                {
                    value = this.Handles.Device?.Parameters.GetFloatValue("AcquisitionFrameRate") ?? value;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    if (0 >= value)
                    {
                        var min = 0d;
                        var max = 0d;
                        this.Handles.Device?.Parameters.GetFloatRange("AcquisitionFrameRate", ref min, ref max);

                        if (0 < max)
                        {
                            var currentTriggerMode = this.TriggerMode;
                            if (currentTriggerMode != TriggerMode.InternalTrigger)
                            {
                                this.TriggerMode = TriggerMode.InternalTrigger;
                            }

                            this.Handles.Device?.Parameters.SetFloatValue("AcquisitionFrameRate", (int)max);

                            if (currentTriggerMode != TriggerMode.InternalTrigger)
                            {
                                this.TriggerMode = currentTriggerMode;
                            }
                        }
                    }
                    else if (this.Handles.Device is not null)
                    {
                        var currentTriggerMode = this.TriggerMode;
                        if (currentTriggerMode != TriggerMode.InternalTrigger)
                        {
                            this.TriggerMode = TriggerMode.InternalTrigger;
                        }

                        this.Handles.Device.Parameters.SetFloatValue("AcquisitionFrameRate", value);

                        if (currentTriggerMode != TriggerMode.InternalTrigger)
                        {
                            this.TriggerMode = currentTriggerMode;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
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
            try
            {
                if (base.Open())
                {
                    if (null != this.Handles.Device)
                    {
                        if (this.Parameter is IAreaSensorParameter gp)
                        {
                            this.FramesPerSecond = gp.FramesPerSecond;
                        }
                    }
                }
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
            try
            {
                base.Initialize(serviceCollection);

                if (this.Parameter is IAreaSensorParameter gp)
                {
                    this.FramesPerSecond = gp.FramesPerSecond;
                }
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

                if (parameter is IAreaSensorParameter gp)
                {
                    this.FramesPerSecond = gp.FramesPerSecond;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EBusAreaSensorGrabber() => this.GrabberType = GrabberType.AreaSensor;
    }
}