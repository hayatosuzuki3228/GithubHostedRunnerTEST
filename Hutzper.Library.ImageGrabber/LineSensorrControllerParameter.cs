namespace Hutzper.Library.ImageGrabber
{
    public record LineSensorrControllerParameter : GrabberControllerParameterBase, ILineSensorControllerParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineSensorrControllerParameter() : this("LineSensorControl")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineSensorrControllerParameter(string fileNameWithoutExtension) : base("LineSensor_Control", typeof(LineSensorrControllerParameter).Name, $"{fileNameWithoutExtension}.ini")
        {
        }
    }
}