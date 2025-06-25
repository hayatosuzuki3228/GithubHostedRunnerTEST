using Hutzper.Library.Common;
using System.Diagnostics;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process.ImageCropper;

public class ImageCropperResult : SafelyDisposable, IImageCropperResult
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

public class ImageCropper : IImageCropper
{
    public Stopwatch Stopwatch { get; set; } = new();

    public IImageCropperResult Execute(IImageCropperInput input)
    {
        Serilog.Log.Information($"ImageCropper.Execute, begin");
        this.Stopwatch.Restart();

        var result = new ImageCropperResult();
        try
        {
            var bitmap = input.Image;
            var rectangleValue = input.CropRectangle;

            // 矩形が画像範囲外の場合のチェック
            if (rectangleValue.X < 0 || rectangleValue.Y < 0 ||
                rectangleValue.X + rectangleValue.Width > bitmap.Width ||
                rectangleValue.Y + rectangleValue.Height > bitmap.Height)
            {
                result.IsSuccessful = false;
                Serilog.Log.Information("ImageCropper.Execute, The rectangle is outside the boundaries of the image.");
                return result; // 範囲外の場合、処理を終了
            }

            // 画像の切り取り
            result.ProcessedImage = bitmap.Clone(rectangleValue, bitmap.PixelFormat);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            Serilog.Log.Information(ex, ex.Message);
        }
        Serilog.Log.Information($"ImageCropper.Execute, end, time = {this.Stopwatch.ElapsedMilliseconds} ms");

        return result;
    }
}
