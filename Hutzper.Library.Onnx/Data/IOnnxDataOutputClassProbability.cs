namespace Hutzper.Library.Onnx.Data;

public interface IOnnxDataOutputClassProbability : IOnnxDataOutput
{
    /// <summary>
    /// クラス確率値
    /// </summary>
    public float[] ClassProbability { get; }
}