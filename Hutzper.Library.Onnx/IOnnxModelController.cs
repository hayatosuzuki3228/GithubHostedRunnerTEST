using Hutzper.Library.Common.Controller;
using Hutzper.Library.Onnx.Model;

namespace Hutzper.Library.Onnx;

/// <summary>
/// ONNXモデル制御インタフェース
/// </summary>
public interface IOnnxModelController : IController
{
    /// <summary>
    /// モデルインスタンス数
    /// </summary>
    public int NumberOfModel { get; }

    /// <summary>
    /// ONNXモデルインスタンスリスト
    /// </summary>
    public List<IOnnxModel> Models { get; }

    /// <summary>
    /// モデル割り付け
    /// </summary>
    /// <param name="devices"></param>
    /// <returns></returns>
    public bool Attach(params IOnnxModel[] models);
}