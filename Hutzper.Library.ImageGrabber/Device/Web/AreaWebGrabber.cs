using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageGrabber.Device.Web
{
    [Serializable]
    public class AreaWebGrabber : WebGrabber, IAreaSensor
    {
        #region IAreaSensor

        /// <summary>
        /// フレームレート
        /// </summary>
        public double FramesPerSecond { get; set; } = 15d;

        #endregion

        #region IController

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is IAreaSensorParameter p)
                {
                    this.FramesPerSecond = p.FramesPerSecond;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AreaWebGrabber() : this(typeof(AreaWebGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AreaWebGrabber(Common.Drawing.Point location) : this(typeof(AreaWebGrabber).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public AreaWebGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.GrabberType = GrabberType.AreaSensor;
        }

        #endregion
    }
}