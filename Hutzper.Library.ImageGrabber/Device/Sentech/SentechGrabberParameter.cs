namespace Hutzper.Library.ImageGrabber.Device.Sentech
{
    /// <summary>
    /// Sentech画像取得パラメータ
    /// </summary>
    [Serializable]
    public record SentechGrabberParameter : GrabberParameterBase, ISentechGrabberParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public SentechGrabberParameter(Common.Drawing.Point location) : base(location, typeof(SentechGrabberParameter).Name)
        {
        }
    }
}