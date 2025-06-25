using Hutzper.Library.Common.Data;
using Hutzper.Library.Onnx.Model;

namespace Hutzper.Library.Onnx;

public interface IOnnxModelControllerParameter : IControllerParameter
{
    /// <summary>
    /// 画像取得パラメータ
    /// </summary>
    public List<IOnnxModelParameter> ModelParameters { get; }
}