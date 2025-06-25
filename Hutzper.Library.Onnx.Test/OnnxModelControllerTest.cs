using Hutzper.Library.Common.TestHelper;
using Hutzper.Library.Onnx.Data;
using Hutzper.Library.Onnx.Model;
using System.Drawing;
using Xunit.Abstractions;

namespace Hutzper.Library.Onnx.Test;

public class OnnxModelControllerTest(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
{
    [Fact(DisplayName = "OnnxModelControllerの正常系のテスト")]
    public void OnnxModelControllerNormalTest()
    {
        var onnxModel = new OnnxModelCpu();
        onnxModel.Initialize(null);
        onnxModel.SetParameter(new OnnxModelParameterBase(new Library.Common.Drawing.Point(0, 0))
        {
            DeviceID = 0,
            OnnxModelFullFileName = "TestData/classification_model_256_256.onnx",
            Algorithm = OnnxModelAlgorithm.Classification,
            ExecutionProvider = OnnxModelExecutionProvider.Cpu,
        });

        IOnnxModel[] onnxModels = [onnxModel];

        var onnxModelController = new OnnxModelControllerBase();
        onnxModelController.Attach(onnxModels);
        Assert.True(onnxModelController.Open());


        //モデル数が1であることを確認
        Assert.Equal(1, onnxModelController.NumberOfModel);

        var imagePath = "TestData/muffin_658_492_ok.jpg";
        OnnxDataInputBitmap onnxDataInputBitmap = new OnnxDataInputBitmap();
        onnxDataInputBitmap.Initialize(onnxModelController.Models[0]);
        if (onnxDataInputBitmap.Images != null)
        {
            onnxDataInputBitmap.Images.Clear();
            onnxDataInputBitmap.Images.Add(onnxDataInputBitmap.InputMetadata.Keys.First(), new[] { new Bitmap(imagePath) });
        }
        var inferenceResult = onnxModelController.Models[0].Run(onnxDataInputBitmap) as OnnxDataOutputClassProbability;

        //推論が正常に行われることを確認
        Assert.NotNull(inferenceResult);
    }

}