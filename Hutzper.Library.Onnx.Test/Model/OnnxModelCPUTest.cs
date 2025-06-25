using Hutzper.Library.Common.TestHelper;
using Hutzper.Library.Onnx.Data;
using Hutzper.Library.Onnx.Model;
using System.Drawing;
using Xunit.Abstractions;

namespace Hutzper.Library.Onnx.Test;

public class OnnxModelCpuTest(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
{
    [Fact(DisplayName = "Onnxモデルの読み込みの正常系")]
    public void LoadOnnxModel()
    {
        IOnnxModelParameter modelParameter = new OnnxModelParameterBase(new Library.Common.Drawing.Point(0, 0))
        {
            DeviceID = 0,
            OnnxModelFullFileName = "TestData/classification_model_256_256.onnx",
            Algorithm = OnnxModelAlgorithm.Classification,
            ExecutionProvider = OnnxModelExecutionProvider.Cpu,
        };

        var onnxModel = new OnnxModelCpu();
        onnxModel.Initialize(null);
        onnxModel.SetParameter(modelParameter);

        //Onnxファイルが正常に読み込まれることを確認
        Assert.True(onnxModel.Open());
    }

    [Fact(DisplayName = "Onnxモデルの読み込みの異常系")]
    public void LoadOnnxModelError()
    {
        IOnnxModelParameter modelParameter = new OnnxModelParameterBase(new Library.Common.Drawing.Point(0, 0))
        {
            DeviceID = 0,
            OnnxModelFullFileName = "invalid_path.onnx",
            Algorithm = OnnxModelAlgorithm.Classification,
            ExecutionProvider = OnnxModelExecutionProvider.Cpu,
        };

        var onnxModel = new OnnxModelCpu();
        onnxModel.Initialize(null);
        onnxModel.SetParameter(modelParameter);

        //Openに失敗することを確認
        Assert.False(onnxModel.Open());
    }


    [Fact]
    public void InferrenceTestClassification()
    {
        IOnnxModelParameter modelParameter = new OnnxModelParameterBase(new Library.Common.Drawing.Point(0, 0))
        {
            DeviceID = 0,
            OnnxModelFullFileName = "TestData/classification_model_256_256.onnx",
            Algorithm = OnnxModelAlgorithm.Classification,
            ExecutionProvider = OnnxModelExecutionProvider.Cpu,
        };

        var onnxModel = new OnnxModelCpu();
        onnxModel.Initialize(null);
        onnxModel.SetParameter(modelParameter);

        //Onnxファイルが正常に読み込まれることを確認
        Assert.True(onnxModel.Open());

        var imagePath = "TestData/muffin_658_492_ok.jpg";
        OnnxDataInputBitmap onnxDataInputBitmap = new OnnxDataInputBitmap();
        onnxDataInputBitmap.Initialize(onnxModel);
        if (onnxDataInputBitmap.Images != null)
        {
            onnxDataInputBitmap.Images.Clear();
            onnxDataInputBitmap.Images.Add(onnxDataInputBitmap.InputMetadata.Keys.First(), new[] { new Bitmap(imagePath) });
        }
        var inferenceResult = onnxModel.Run(onnxDataInputBitmap) as OnnxDataOutputClassProbability;

        //inferenceResultがnullでないことを確認
        Assert.NotNull(inferenceResult);

        //inferenceResult.ClassProbabilityが小数の配列であることを確認
        Assert.True(inferenceResult.ClassProbability is float[]);

        //それぞれの確率値が0以上1以下であることを確認
        foreach (var probability in inferenceResult.ClassProbability)
        {
            Assert.True(probability >= 0 && probability <= 1);
        }


    }

}