using Hutzper.Library.Onnx.Model;
using System.Drawing;

namespace Hutzper.Library.Onnx.Data;

public interface IOnnxDataBitmap
{
    /// <summary>
    /// Bitmapリスト
    /// </summary>
    public Dictionary<string, Bitmap[]>? Images { get; set; }

    /// <summary>
    /// Bitmap化
    /// </summary>
    /// <returns></returns>
    public Bitmap[] ToBitmap();

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(IOnnxModel model);
}