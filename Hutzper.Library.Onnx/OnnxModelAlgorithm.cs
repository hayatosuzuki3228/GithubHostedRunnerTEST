using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.Onnx;

/// <summary>
/// モデルのアルゴリズム
/// </summary>
[Serializable]
public enum OnnxModelAlgorithm
{
    [AliasName("Undefined")]
    Undefined = 0,

    [AliasName("Classification")]
    Classification = 1,

    [AliasName("ClassificationFGVC")]
    ClassificationFGVC = 2,

    [AliasName("AnomalyDetection")]
    AnomalyDetection = 3,

    [AliasName("Segmentation_Image")]
    Segmentation_Image = 4,

    [AliasName("object_detection")]
    ObjectDetection = 5,

    [AliasName("Segmentation_Instance")]
    Segmentation_Instance = 6,
}