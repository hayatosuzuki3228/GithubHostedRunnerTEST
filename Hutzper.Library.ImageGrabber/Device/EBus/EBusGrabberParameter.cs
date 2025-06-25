namespace Hutzper.Library.ImageGrabber.Device.EBus
{
    /// <summary>
    /// eBus画像取得パラメータ
    /// </summary>
    [Serializable]
    public record EBusGrabberParameter : GrabberParameterBase, IEBusGrabberParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public EBusGrabberParameter(Common.Drawing.Point location) : base(location, typeof(EBusGrabberParameter).Name)
        {
            this.AnalogGain = 1;
            this.DigitalGain = 0;
        }
    }
}