namespace Hutzper.Library.ImageGrabber.Device.Pylon
{
    /// <summary>
    /// Pylon画像取得パラメータ
    /// </summary>
    [Serializable]
    public record PylonGrabberParameter : GrabberParameterBase, IPylonGrabberParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public PylonGrabberParameter(Common.Drawing.Point location) : base(location, typeof(PylonGrabberParameter).Name)
        {
        }
    }
}