namespace Hutzper.Library.Onnx.Data;

public interface IOnnxDataOutputClassArea : IOnnxDataOutput
{
    /// <summary>
    /// クラス面積
    /// </summary>
    /// <remarks>ピクセル数</remarks>
    public float[] ClassArea { get; }
}