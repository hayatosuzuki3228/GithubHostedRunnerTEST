namespace Hutzper.Library.ImageGrabber.Device.File
{
    /// ファイル画像取得
    [Serializable]
    public record ObserveFolderGrabberParameter : GrabberParameterBase, IAreaSensorParameter
    {
        public DirectoryInfo? ImageDirectoryInfo { get; set; } = null;
        public int ConsumeDurationMs { get; set; } = 700;
        public double FramesPerSecond { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ObserveFolderGrabberParameter() : this(Common.Drawing.Point.New())
        {
        }

        public ObserveFolderGrabberParameter(Common.Drawing.Point location) : base(location, typeof(ObserveFolderGrabberParameter).Name)
        {
        }
    }
}