namespace Hutzper.Library.ImageGrabber.Device.Sentech
{
    [Serializable]
    public enum FFCOffsetMode
    {
        Off = 0,
        On = 1,
        Once = 2,
    }

    [Serializable]
    public enum FFCGainMode
    {
        Off = 0,
        On = 1,
        Once = 2,
        TargetPlusOnce = 3,
    }
}
