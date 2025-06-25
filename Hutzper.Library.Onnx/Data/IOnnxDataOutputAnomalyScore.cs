namespace Hutzper.Library.Onnx.Data;

public interface IOnnxDataOutputAnomalyScore : IOnnxDataOutput
{
    /// <summary>
    /// 異常スコア
    /// </summary>
    public float AnomalyScore { get; }
}