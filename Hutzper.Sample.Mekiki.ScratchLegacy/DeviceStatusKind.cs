namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    /// <summary>
    /// 機器状態
    /// </summary>
    [Serializable]
    public enum DeviceStatusKind
    {
        Unknown,
        Nonuse,
        Error,
        Warning,
        Disabled,
        Enabled,
        On,
        Off,
    }
}