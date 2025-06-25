using Hutzper.Library.Common.Drawing;

namespace Hutzper.Library.Onnx.Data;

public interface IOnnxDataOutputClassIndexed : IOnnxDataOutput
{
    /// <summary>
    /// フレームサイズ
    /// </summary>
    public Size ClassFrameSize { get; set; }

    /// <summary>
    /// クラス別検出インデックス
    /// </summary>
    public List<byte[]> ClassIndexed { get; }
}