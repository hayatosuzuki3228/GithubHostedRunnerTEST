using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageGrabber.Device.Tis
{
    /// <summary>
    /// TISエリアカメラ画像取得
    /// </summary>
    public class TisAreaSensorGrabber : TisGrabber, IAreaSensor
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
                    value = this.IcImagingControl.DeviceFrameRate;
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
                    if (this.Enabled)
                    {
                        if (0 < this.AvailableFrameFrameRates.Length)
                        {
                            var fpsList = this.AvailableFrameFrameRates.ToList().ConvertAll(f => Math.Abs(f - (float)value));

                            if (0 < fpsList.Count)
                            {
                                var minValu = fpsList.Min();
                                var selectedIndex = fpsList.IndexOf(minValu);

                                this.IcImagingControl.DeviceFrameRate = this.AvailableFrameFrameRates[selectedIndex];
                            }
                            else
                            {
                                this.IcImagingControl.DeviceFrameRate = (float)value;
                            }
                        }
                        else
                        {
                            this.IcImagingControl.DeviceFrameRate = (float)value;
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
                    if (this.Parameter is IAreaSensorParameter gp)
                    {
                        this.FramesPerSecond = gp.FramesPerSecond;
                        this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
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
                if (true == this.Enabled)
                {
                    if (this.Parameter is IAreaSensorParameter gp)
                    {
                        this.FramesPerSecond = gp.FramesPerSecond;
                        this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                    }
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
                    this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
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
        public TisAreaSensorGrabber() => this.GrabberType = GrabberType.AreaSensor;
    }
}