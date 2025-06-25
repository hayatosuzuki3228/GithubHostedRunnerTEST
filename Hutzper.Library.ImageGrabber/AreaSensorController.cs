namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// エリアカメラ制御
    /// </summary>
    [Serializable]
    public class AreaSensorController : GrabberControllerBase, IAreaSensorController
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public AreaSensorController(int index = -1) : base(typeof(AreaSensorController).Name, index)
        {
            this.GrabberType = GrabberType.AreaSensor;
        }
    }
}