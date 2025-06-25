using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageGrabber.Device.File
{
    [Serializable]
    public class AreaFileGrabber : FileGrabber, IAreaSensor
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
        public AreaFileGrabber() : this(typeof(AreaFileGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AreaFileGrabber(Common.Drawing.Point location) : this(typeof(AreaFileGrabber).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public AreaFileGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.GrabberType = GrabberType.AreaSensor;
        }

        #endregion
    }
}