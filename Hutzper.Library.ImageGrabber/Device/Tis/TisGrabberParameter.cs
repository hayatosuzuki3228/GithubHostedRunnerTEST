namespace Hutzper.Library.ImageGrabber.Device.Tis
{
    /// <summary>
    /// TIS画像取得パラメータ
    /// </summary>
    [Serializable]
    public record TisGrabberParameter : GrabberParameterBase, ITisGrabberParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public TisGrabberParameter(Common.Drawing.Point location) : base(location, typeof(TisGrabberParameter).Name)
        {
        }
    }
}