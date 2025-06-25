using Hutzper.Library.Common;
using OpenCvSharp;
using System.Diagnostics;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process.ImageRotator;

public class ImageRotatorResult : SafelyDisposable, IImageRotatorResult
{
    // IImageProcessorResult
    public bool IsSuccessful { get; set; } = true; // 処理が成功したかどうか
    public Bitmap? ProcessedImage { get; set; } = null; // 処理後の画像

    // リソース解放
    protected override void DisposeExplicit()
    {
        this.DisposeSafely(this.ProcessedImage);
    }
}

public class ImageRotator : IImageRotator
{
    public Stopwatch Stopwatch { get; set; } = new();

    public IImageRotatorResult Execute(IImageRotatorInput input)
    {
        Serilog.Log.Information($"ImageRotator.Execute, begin");
        this.Stopwatch.Restart();

        var result = new ImageRotatorResult();

        try
        {
            // 処理結果画像の生成
            var rotateAngle = input.AngleDegree * -1;    // 半時計回り
            var bitmap = input.Image;

            // 回転行列
            using var trans = Cv2.GetRotationMatrix2D(new Point2f(bitmap.Width / 2f, bitmap.Height / 2f), rotateAngle, 1);    // 半時計回り

            // アフィン変換
            using var mat1 = OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap);
            using var mat2 = new Mat();
            Cv2.WarpAffine(mat1, mat2, trans, mat1.Size(), InterpolationFlags.Linear, BorderTypes.Constant, Scalar.All(255)); // 白背景

            // Bitmap化
            result.ProcessedImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat2);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            Serilog.Log.Information($"failed to rotate the image, {ex.ToString()}");
        }
        Serilog.Log.Information($"ImageRotator.Execute, end, time = {this.Stopwatch.ElapsedMilliseconds} ms");

        return result;
    }
}
