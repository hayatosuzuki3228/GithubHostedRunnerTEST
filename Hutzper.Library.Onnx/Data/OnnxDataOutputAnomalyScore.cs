namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// ONNX出力データ異常スコア
/// </summary>
/// <remarks>FastFlow想定</remarks>
[Serializable]
public class OnnxDataOutputAnomalyScore : OnnxDataOutputClassProbability, IOnnxDataOutputAnomalyScore
{
    #region IOnnxDataOutputAnomalyScore

    /// <summary>
    /// 異常スコア
    /// </summary>
    public float AnomalyScore => this.ClassProbability.First();

    #endregion
}