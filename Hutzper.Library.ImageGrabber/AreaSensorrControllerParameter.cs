namespace Hutzper.Library.ImageGrabber
{
    public record AreaSensorrControllerParameter : GrabberControllerParameterBase, IAreaSensorControllerParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AreaSensorrControllerParameter() : this("AreaSensorControl")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AreaSensorrControllerParameter(string fileNameWithoutExtension) : base("AreaSensor_Control", typeof(AreaSensorrControllerParameter).Name, $"{fileNameWithoutExtension}.ini")
        {
        }
    }
}