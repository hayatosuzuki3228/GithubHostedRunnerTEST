namespace Hutzper.Library.Onnx.Data;

[Serializable]
public class OnnxDataOutputClassArea : OnnxDataOutputClassProbability, IOnnxDataOutputClassArea
{
    #region IOnnxDataOutputClassArea

    /// <summary>
    /// クラス面積
    /// </summary>
    /// <remarks>ピクセル数</remarks>
    public float[] ClassArea => this.ClassProbability;

    #endregion
}