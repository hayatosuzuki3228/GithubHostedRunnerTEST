using Hutzper.Library.Common.Data;

namespace Hutzper.Library.Vibrometer.Data
{
    /// <summary>
    /// 振動計結果データインタフェース
    /// </summary>
    public interface IVibrometerResult : IResultData
    {
        public int Value { get; set; }

        public uint SequenceNumber { get; set; }

        public bool[] SensorInputChanged { get; set; }

        public bool[] SensorInputValues { get; set; }
    }

    /// <summary>
    /// IVibrometerResult実装
    /// </summary>
    [Serializable]
    public record VibrometerResult : ResultDataBaseRecord, IVibrometerResult
    {
        public int Value { get; set; }

        public uint SequenceNumber { get; set; }

        public bool[] SensorInputChanged { get; set; } = Array.Empty<bool>();

        public bool[] SensorInputValues { get; set; } = Array.Empty<bool>();
    }
}