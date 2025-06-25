using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.Onnx;

/// <summary>
/// ExecutionProvider
/// </summary>
[Serializable]
public enum OnnxModelExecutionProvider
{
    [AliasName("CPU")]
    Cpu,

    [AliasName("QUDA")]
    Cuda,

    [AliasName("DirectML")]
    DirectML,

    [AliasName("TensorRT")]
    TensorRT,
}