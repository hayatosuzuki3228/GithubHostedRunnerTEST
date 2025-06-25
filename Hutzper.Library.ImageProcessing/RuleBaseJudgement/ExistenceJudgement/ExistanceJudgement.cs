using Hutzper.Library.Common;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.ExistenceJudgement;

// カウントするピクセルの色
public enum ColorMode
{
    White,
    Red,
    Green,
    Blue
}

public class ExistenceJudgementResult : SafelyDisposable, IExistenceJudgementResult
{
    // IExistenceJudgementResult
    public bool IsSuccessful { get; set; } = true; // 処理が成功したかどうか
    public bool IsJudgeOK { get; set; } = false; // 判定がOKかどうか
    public Bitmap? ProcessedImage { get; set; } // 処理後の画像
}

public class ExistenceJudgement : IExistenceJudgement
{
    public Stopwatch Stopwatch { get; set; } = new();
    public int ThresholdValue { get; set; } // しきい値
    public ColorMode Mode { get; set; } // 判定対象のカラーモード

    public ExistenceJudgement(
        int thresholdValue = 128,
        ColorMode mode = ColorMode.White,
        double minScale = 30,
        double maxScale = 255)
    {
        this.ThresholdValue = thresholdValue;
        this.Mode = mode;
    }

    public IExistenceJudgementResult Execute(IExistenceJudgementInput input)
    {
        Serilog.Log.Information($"ExistenceJudgement.Execute, begin");
        this.Stopwatch.Restart();

        var result = new ExistenceJudgementResult();
        try
        {
            using Mat inputMat = input.Image.ToMat();
            using Mat binaryMask = new Mat();

            // カラーモードに応じた処理
            switch (this.Mode)
            {
                case ColorMode.White:
                    // グレースケールに変換してしきい値を適用
                    using (Mat grayImage = new Mat())
                    {
                        Cv2.CvtColor(inputMat, grayImage, ColorConversionCodes.BGR2GRAY);
                        Cv2.Threshold(grayImage, binaryMask, this.ThresholdValue, 255, ThresholdTypes.Binary);
                    }
                    break;

                case ColorMode.Red:
                    this.ExtractChannelMask(inputMat, binaryMask, channelIndex: 2, this.ThresholdValue);
                    break;

                case ColorMode.Green:
                    this.ExtractChannelMask(inputMat, binaryMask, channelIndex: 1, this.ThresholdValue);
                    break;

                case ColorMode.Blue:
                    this.ExtractChannelMask(inputMat, binaryMask, channelIndex: 0, this.ThresholdValue);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported color mode: {this.Mode}");
            }

            // ラベリング処理で最大領域のピクセル数で判定（誤判定対策）
            int largestRegionPixels = this.GetLargestRegionPixelCount(binaryMask);

            // 結果を格納
            result.IsJudgeOK = largestRegionPixels > this.ThresholdValue;
            result.ProcessedImage = BitmapConverter.ToBitmap(binaryMask);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            Serilog.Log.Information(ex, ex.Message);
        }

        Serilog.Log.Information($"ExistenceJudgement.Execute, end, time = {this.Stopwatch.ElapsedMilliseconds} ms");
        return result;
    }

    private void ExtractChannelMask(Mat inputMat, Mat binaryMask, int channelIndex, int thresholdValue)
    {
        // アルファチャネルを削除
        if (inputMat.Channels() == 4)
        {
            Cv2.CvtColor(inputMat, inputMat, ColorConversionCodes.BGRA2BGR);
        }

        // チャネルごとに分割
        Mat[] channels = Cv2.Split(inputMat);

        // 指定チャネルに対してしきい値を適用
        using Mat channel = channels[channelIndex];
        Cv2.Threshold(channel, binaryMask, thresholdValue, 255, ThresholdTypes.Binary);

        // 解放
        foreach (var mat in channels) mat.Dispose();
    }

    private int GetLargestRegionPixelCount(Mat binaryMask)
    {
        using Mat labels = new Mat();
        using Mat stats = new Mat();
        using Mat centroids = new Mat();

        // ラベリング処理
        int numberOfLabels = Cv2.ConnectedComponentsWithStats(binaryMask, labels, stats, centroids);

        // 最大の領域を探す
        int largestRegionPixels = 0;
        for (int i = 1; i < numberOfLabels; i++) // ラベル0は背景
        {
            int regionPixels = stats.At<int>(i, (int)ConnectedComponentsTypes.Area);
            if (regionPixels > largestRegionPixels)
            {
                largestRegionPixels = regionPixels;
            }
        }

        return largestRegionPixels;
    }
}