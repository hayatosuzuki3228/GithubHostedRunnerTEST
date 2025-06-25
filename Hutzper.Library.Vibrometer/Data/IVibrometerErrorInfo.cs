namespace Hutzper.Library.Vibrometer.Data
{
    /// <summary>
    /// 振動計エラー情報インタフェース
    /// </summary>
    public interface IVibrometerErrorInfo
    {
    }

    /// <summary>
    /// IVibrometerErrorInfo実装
    /// </summary>
    [Serializable]
    public record VibrometerErrorInfo : IVibrometerErrorInfo
    {
    }
}