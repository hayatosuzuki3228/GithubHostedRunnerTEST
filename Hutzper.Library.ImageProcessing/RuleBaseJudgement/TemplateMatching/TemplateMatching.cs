using Hutzper.Library.Common;
using Hutzper.Library.Common.Drawing;
using Hutzper.Library.ImageProcessing.RuleBaseJudgement.TemplateMatching.Data;
using OpenCvSharp;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.TemplateMatching;

public class TemplateMatchingResult : SafelyDisposable, ITemplateMatchingResult
{
    // IImageProcessorResult
    public bool IsSuccessful { get; set; } = true; // 処理が成功したかどうか
    public bool IsJudgeOK { get; set; } = false; // 判定がOKかどうか
    // ITemplateMatchingResult
    public TemplateDataItem? Template { get; set; } = new();
    public PointD Point { get; set; } = new();
    public double Score { get; set; } = new();

    public TemplateMatchingResult() => this.Clear();

    public void Clear()
    {
        this.Template = null;
        this.Point = new();
        this.Score = 0;
    }

    // リソース解放
    protected override void DisposeExplicit()
    {
        this.DisposeSafely(this.Template);
    }
}

public class TemplateMatching : ITemplateMatching
{
    public Stopwatch Stopwatch { get; set; } = new();
    private ParallelOptions MaxDegreeOfParallelism = new();
    private int SearchSkipNumber = 4;
    private double ReductionRatio = 8;
    private TemplateData TemplateData;
    private double ThresholdValue = 0.50d;

    public TemplateMatching(string templatePath, int searchSkipNumber, int reductionDenominator, int maxDegreeOfParallelism)
    {
        SearchSkipNumber = searchSkipNumber;
        ReductionRatio = 1d / reductionDenominator;
        MaxDegreeOfParallelism = new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism };

        // テンプレートデータの読み込み
        TemplateData = new TemplateData();
        DirectoryInfo directoryInfo = new DirectoryInfo(templatePath);
        if (TemplateData.Load(directoryInfo) == true)
        {
            TemplateData.CreateMatrix();
            Serilog.Log.Information($"Successfully loaded matching template, TemplatePath = {templatePath}");
        }
        else
        {
            Serilog.Log.Information($"Failed to load matching template, TemplatePath = {templatePath}");
        }
        TemplateData.Tag = templatePath;
    }

    // マッチング処理
    public ITemplateMatchingResult Execute(ITemplateMatchingInput input)
    {
        Serilog.Log.Information($"TemplateMatching.Execute, begin");
        this.Stopwatch.Restart();

        var bestResult = new TemplateMatchingResult();
        var bitmap = input.Image;
        try
        {
            TemplateData.ResetResults(); // マッチング結果を初期化する

            // 画像データ形式を変換する
            using var matImage = OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap);
            using var matImageGray = matImage.Channels() > 1
                ? matImage.CvtColor(ColorConversionCodes.BGR2GRAY)
                : matImage.Clone();

            // 指定された縮小率で画像をリサイズする
            using var matImageResized = matImageGray.Resize(new OpenCvSharp.Size(), ReductionRatio, ReductionRatio, InterpolationFlags.Cubic);
            matImageGray.Dispose();

            // スキップ設定に従い処理順序リストを作成する
            var skipSum = SearchSkipNumber / 2;
            var candidateList = new List<int>();
            foreach (var i in Enumerable.Range(0, TemplateData.Items.Count))
            {
                if (skipSum++ >= SearchSkipNumber)
                {
                    skipSum = 0;
                    candidateList.Add(i);
                }
            }

            // マッチング処理定義
            Action<Mat, Mat, double, TemplateMatchingResult> matching = (targetImage, selectedTemplate, selectedAngle, selectedResult) =>
            {
                try
                {
                    using var result = new Mat();
                    Cv2.MatchTemplate(targetImage, selectedTemplate, result, TemplateMatchModes.CCoeffNormed);

                    var minVal = 0d;
                    var maxVal = 0d;
                    var minPos = new OpenCvSharp.Point();
                    var maxPos = new OpenCvSharp.Point();
                    result.MinMaxLoc(out minVal, out maxVal, out minPos, out maxPos);

                    selectedResult.Score = maxVal;
                    selectedResult.Point = new PointD(maxPos.X, maxPos.Y);
                }
                catch (Exception ex)
                {
                    bestResult.IsSuccessful = false;
                    Serilog.Log.Information(ex, ex.ToString());
                    return;
                }
            };

            // マッチング処理実行
            Parallel.ForEach(candidateList, MaxDegreeOfParallelism, i =>
            {
                // テンプレート選択
                var selectedTemplate = TemplateData.Items[i];
                if (selectedTemplate.Matrix is null) return;

                // 実行
                var selectedIndex = TemplateData.Items.IndexOf(selectedTemplate);
                var selectedResult = TemplateData.Results[selectedIndex];
                selectedResult.Template = selectedTemplate; // 使用テンプレートへの参照を結果に紐付ける
                using var resizeTemplate = selectedTemplate.Matrix.Resize(new OpenCvSharp.Size(), ReductionRatio, ReductionRatio, InterpolationFlags.Cubic);
                matching(matImageResized, resizeTemplate, selectedTemplate.AngleDegree, selectedResult);
            });

            var results = TemplateData.Results; // マッチング結果への参照を取得
            var tempResults = new List<TemplateMatchingResult>(results);
            tempResults.Sort((a, b) => b.Score.CompareTo(a.Score));
            bestResult = tempResults.First();

            // 座標変換
            this.TransformPoints(bestResult, TemplateData, bitmap, ReductionRatio);

            // スキップが有効な場合は近傍で再検索する
            if (0 < SearchSkipNumber)
            {
                // 初回検索でスコアが最大だった位置
                var maxLocation = 0;
                foreach (var item in TemplateData.Items)
                {
                    if (item == bestResult.Template)
                    {
                        maxLocation = TemplateData.Items.IndexOf(item);
                        break;
                    }
                }

                // 初回検索で類似度が最大だった座標を中心に検索リストを作成する
                candidateList.Clear();
                var counter = new CircularCounter(0, TemplateData.Items.Count - 1, maxLocation);
                for (int i = 0; i < SearchSkipNumber; i++) candidateList.Add(counter.PreDecrement());

                counter.Counter = maxLocation;
                for (int i = 0; i < SearchSkipNumber; i++) candidateList.Add(counter.PreStep());

                // マッチング処理実行
                Parallel.ForEach(candidateList, MaxDegreeOfParallelism, i =>
                {
                    // テンプレート選択
                    var selectedTemplate = TemplateData.Items[i];
                    if (selectedTemplate.Matrix is null) return;

                    // 実行
                    var selectedIndex = TemplateData.Items.IndexOf(selectedTemplate);
                    var selectedResult = TemplateData.Results[selectedIndex];
                    selectedResult.Template = selectedTemplate; // 使用テンプレートへの参照を結果に紐付ける
                    using var resizeTemplate = selectedTemplate.Matrix.Resize(new OpenCvSharp.Size(), ReductionRatio, ReductionRatio, InterpolationFlags.Cubic);
                    matching(matImageResized, resizeTemplate, selectedTemplate.AngleDegree, selectedResult);
                });

                // スコア最大値が更新された場合は結果を差し替える
                results.Sort((a, b) => b.Score.CompareTo(a.Score));
                if (bestResult.Score < results.First().Score)
                {
                    bestResult = results.First();
                    this.TransformPoints(bestResult, TemplateData, bitmap, ReductionRatio); // 座標変換
                }
            }

            // しきい値判定
            if (bestResult.Score >= input.ThresholdValue) bestResult.IsJudgeOK = true;
            else Serilog.Log.Information($"TemplateMatching.Execute, thresholdValue = {ThresholdValue}, under threshold");
        }
        catch (Exception ex)
        {
            bestResult.IsSuccessful = false;
            Serilog.Log.Information(ex, ex.Message);
        }
        Serilog.Log.Information($"TemplateMatching.Execute, end, time = {this.Stopwatch.ElapsedMilliseconds} ms");

        return bestResult;
    }

    // 座標変換
    private void TransformPoints(ITemplateMatchingResult selectedResult, TemplateData templateUnit, Bitmap bitmap, double reductionRatio)
    {
        if (selectedResult.Template is TemplateDataItem bestTemplate2)
        {
            selectedResult.Point.X /= reductionRatio;
            selectedResult.Point.Y /= reductionRatio;
            selectedResult.Point.X += TemplateData.TemplateRectangle.Size.Width / 2;
            selectedResult.Point.Y += TemplateData.TemplateRectangle.Size.Height / 2;

            using Matrix matrix = new();
            var centerF = new PointF(bitmap.Width * 0.5f, bitmap.Height * 0.5f);
            matrix.RotateAt((float)bestTemplate2.AngleDegree, centerF); // 時計回り

            var transformedPoints = new PointF[] { selectedResult.Point.ToPointF() };
            matrix.TransformPoints(transformedPoints);

            selectedResult.Point.X = transformedPoints[0].X;
            selectedResult.Point.Y = transformedPoints[0].Y;
        }
    }
}
