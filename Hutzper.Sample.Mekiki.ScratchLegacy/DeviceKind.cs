namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    /// <summary>
    /// 機器種別
    /// </summary>
    [Serializable]
    public enum DeviceKind
    {
        Unknown,
        Camera,
        Light,
        DigitalIO,
        Plc,
        Other_A,
        Other_B,
        Other_C,
    }
}