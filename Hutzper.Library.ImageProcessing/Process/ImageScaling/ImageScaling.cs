using Hutzper.Library.Common;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process.ImageScaling;

public enum ProcessingMode
{
    Grayscale, // グレースケール
    Color, // カラー全体
    Red, // 赤チャネルのみ
    Green, // 緑チャネルのみ
    Blue // 青チャネルのみ
}

public class ImageScalingResult : SafelyDisposable, IImageScalingResult
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

public class ImageScaling : IImageScaling
{
    public Stopwatch Stopwatch { get; set; } = new();
    public ProcessingMode Mode { get; set; } // 処理モード
    public double MinScale { get; set; } // スケール範囲の最小値
    public double MaxScale { get; set; } // スケール範囲の最大値

    public ImageScaling(double minScale, double maxScale, ProcessingMode mode = ProcessingMode.Color)
    {
        this.MinScale = minScale;
        this.MaxScale = maxScale;
        this.Mode = mode;
    }

    public IImageScalingResult Execute(IImageScalingInput input)
    {
        Serilog.Log.Information($"ImageScaling.Execute, begin");
        this.Stopwatch.Restart();

        var result = new ImageScalingResult();
        try
        {
            using Mat inputMat = BitmapConverter.ToMat(input.Image);
            using Mat processedMat = this.ProcessImage(inputMat);

            // Mat を Bitmap に変換して結果に設定
            result.ProcessedImage = BitmapConverter.ToBitmap(processedMat);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            Serilog.Log.Information(ex, ex.Message);
        }
        Serilog.Log.Information($"ImageScaling.Execute, end, time = {this.Stopwatch.ElapsedMilliseconds} ms");

        return result;
    }

    private Mat ProcessImage(Mat inputMat)
    {
        var scalingLut = this.CreateScalingLUT();

        // 処理モードに応じて分岐
        Mat resultMat = this.Mode switch
        {
            ProcessingMode.Grayscale => this.ProcessGrayscale(inputMat, scalingLut),
            ProcessingMode.Red or ProcessingMode.Green or ProcessingMode.Blue => this.ProcessSingleChannel(inputMat, scalingLut),
            ProcessingMode.Color => this.ProcessColor(inputMat, scalingLut),
            _ => throw new NotSupportedException($"Unsupported processing mode: {this.Mode}")
        };

        // 結果が4チャネルの場合、3チャネルに変換
        if (resultMat.Channels() == 4)
        {
            using Mat convertedMat = new Mat();
            Cv2.CvtColor(resultMat, convertedMat, ColorConversionCodes.BGRA2BGR);
            return convertedMat.Clone();
        }

        return resultMat.Clone();
    }

    private Mat ProcessGrayscale(Mat inputMat, byte[] scalingLut)
    {
        using Mat grayImage = new Mat();
        Cv2.CvtColor(inputMat, grayImage, ColorConversionCodes.BGR2GRAY);
        Cv2.LUT(grayImage, scalingLut, grayImage);
        return grayImage.Clone();
    }

    private Mat ProcessColor(Mat inputMat, byte[] scalingLut)
    {
        using Mat scaledImage = new Mat();
        Cv2.LUT(inputMat, scalingLut, scaledImage);
        return scaledImage.Clone();
    }

    private Mat ProcessSingleChannel(Mat inputMat, byte[] scalingLut)
    {
        // Mat[] 配列を使用
        Mat[] channels = Cv2.Split(inputMat);

        // 処理対象チャネルを選択
        int channelIndex = this.Mode == ProcessingMode.Red ? 2 :
                           this.Mode == ProcessingMode.Green ? 1 : 0;

        // 指定チャネルに対して LUT を適用
        Cv2.LUT(channels[channelIndex], scalingLut, channels[channelIndex]);

        // 他のチャネルをそのまま維持して結合
        Cv2.Merge(channels, inputMat);
        return inputMat.Clone();
    }

    // スケーリング用のLUTを作成
    private byte[] CreateScalingLUT()
    {
        var scalingLut = new byte[byte.MaxValue + 1];
        double coefficient = (this.MaxScale - this.MinScale) / byte.MaxValue;

        for (int i = 0; i < scalingLut.Length; i++)
        {
            scalingLut[i] = (byte)(this.MinScale + i * coefficient);
        }

        return scalingLut;
    }
}
