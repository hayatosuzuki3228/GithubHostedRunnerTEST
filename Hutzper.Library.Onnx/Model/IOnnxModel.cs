using Hutzper.Library.Common.Controller;
using Hutzper.Library.Onnx.Data;
using Microsoft.ML.OnnxRuntime;

namespace Hutzper.Library.Onnx.Model;

/// <summary>
/// ONNXモデルインタフェース
/// </summary>
public interface IOnnxModel : IController
{
    /// <summary>
    /// 識別
    /// </summary>
    public Common.Drawing.Point Location { get; protected set; }
    public string Identifier { get; protected set; }

    /// <summary>
    /// 画像が取得可能な状態かどうか
    /// </summary>
    /// <remarks>Open済みか</remarks>
    public bool Enabled { get; }

    /// <summary>
    /// 失敗した
    /// </summary>
    public event Action<object, Exception>? Disabled;

    /// <summary>
    /// モデルのアルゴリズム
    /// </summary>
    public OnnxModelAlgorithm Algorithm { get; }

    /// <summary>
    /// ExecutionProvider
    /// </summary>
    public OnnxModelExecutionProvider ExecutionProvider { get; }

    /// <summary>
    /// 入力メタデータ取得
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<string, NodeMetadata> GetInputMetadata();

    /// <summary>
    /// 出力メタデータ取得
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<string, NodeMetadata> GetOutputMetadata();

    /// <summary>
    /// 実行
    /// </summary>
    /// <param name="ipputData"></param>
    /// <returns></returns>
    public IOnnxDataOutput Run(IOnnxDataInput ipputData);

    /// <summary>
    /// 空撃ち
    /// </summary>
    /// <returns></returns>
    public List<TimeSpan> DryFire(int tryCount);
}