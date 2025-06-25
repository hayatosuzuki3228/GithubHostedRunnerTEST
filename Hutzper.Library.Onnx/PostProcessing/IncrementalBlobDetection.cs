using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Drawing;
using Hutzper.Library.ImageProcessing.Controller;
using Hutzper.Library.ImageProcessing.Data;
using Hutzper.Library.Onnx.Data;
using System.Drawing;

namespace Hutzper.Library.Onnx.PostProcessing;

/// <summary>
/// ブロブ検出
/// </summary>
public class IncrementalBlobDetection : ControllerBase, IController
{
    #region サブクラス

    /// <summary>
    /// 処理単位
    /// </summary>
    public record ProcessingUnit(int ClassIndex, int ObjectValue);

    /// <summary>
    /// 処理結果
    /// </summary>
    public record ProcessingResult : ProcessingUnit
    {
        public readonly int RleIndex;

        public List<IRleLabel> Labels { get; set; } = new();

        public ProcessingResult(ProcessingUnit unit, int rleIndex) : base(unit.ClassIndex, unit.ObjectValue)
        {
            this.RleIndex = rleIndex;
        }
    }

    #endregion

    #region IController

    /// <summary>
    /// パラメーター設定
    /// </summary>
    /// <param name="parameter"></param>
    public override void SetParameter(IControllerParameter? parameter)
    {
        try
        {
            base.SetParameter(parameter);

            if (parameter is IncrementalBlobDetectionParameter bdp)
            {
                this.Parameter = bdp;
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"{ex.Message}");
        }
    }

    /// <summary>
    /// オープン
    /// </summary>
    /// <returns></returns>
    public override bool Open()
    {
        try
        {
            if (this.Parameter is IncrementalBlobDetectionParameter bdp)
            {
                this.RleController = new IRleController[bdp.MaxDegreeOfParallelism];
                foreach (var i in Enumerable.Range(0, this.RleController.Length))
                {
                    this.RleController[i] = new RleController();
                    this.RleController[i].Initialize(this.Services);
                    this.RleController[i].SetParameter(bdp.RleControllerParameter);
                    this.RleController[i].Open();
                }
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"{ex.Message}");
        }

        return 0 < this.RleController.Length;
    }

    #endregion

    #region フィールド

    /// <summary>
    /// パラメータ
    /// </summary>
    protected IncrementalBlobDetectionParameter? Parameter;

    /// <summary>
    /// Rle
    /// </summary>
    protected IRleController[] RleController = Array.Empty<IRleController>();

    /// <summary>
    /// 対象のONNX出力データ
    /// </summary>
    protected IOnnxDataOutputClassIndexedInstance? OutputData;

    /// <summary>
    /// 並列処理オプション
    /// </summary>
    protected ParallelOptions ParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="index"></param>
    public IncrementalBlobDetection(int index = -1) : base(typeof(IncrementalBlobDetection).Name, index)
    {
        this.Parameter = new IncrementalBlobDetectionParameter();
    }

    #endregion

    #region メソッド

    /// <summary>
    /// ラベリングを実行するための準備を行う
    /// </summary>
    /// <param name="outputData">対象のonnx出力データ</param>
    /// <param name="originalImageSize">元画像サイズ</param>
    /// <param name="fitToOriginalImage">元画像に合わせるための画像サイズ比とパディング</param>
    /// <returns>ラベリングの引数をリストで返します。</returns>
    public List<ProcessingUnit[]> PrepareLabelingWithRle(IOnnxDataOutputClassIndexedInstance outputData, System.Drawing.Size originalImageSize, out (double Scale, SizeD Padding) fitToOriginalImage)
    {
        return this.PrepareLabelingWithRle(outputData, new SizeD(originalImageSize.Width, originalImageSize.Height), out fitToOriginalImage);
    }

    /// <summary>
    /// ラベリングを実行するための準備を行う
    /// </summary>
    /// <param name="outputData">対象のonnx出力データ</param>
    /// <param name="originalImageSize">元画像サイズ</param>
    /// <param name="fitToOriginalImage">元画像に合わせるための画像サイズ比とパディング</param>
    /// <returns>ラベリングの引数をリストで返します。</returns>
    public List<ProcessingUnit[]> PrepareLabelingWithRle(IOnnxDataOutputClassIndexedInstance outputData, SizeD originalImageSize, out (double Scale, SizeD Padding) fitToOriginalImage)
    {
        var unitArrayList = new List<ProcessingUnit[]>();
        fitToOriginalImage = (1.0, new SizeD());

        try
        {
            if (this.Parameter is null)
            {
                throw new Exception("Parameter is not set.");
            }

            // 処理対象データを保持する
            this.OutputData = outputData;
            this.ParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = this.Parameter.MaxDegreeOfParallelism };

            // 処理単位を作成する
            var cumulativeObjectValue = 0;
            var tempUnitList = new List<ProcessingUnit>();
            foreach (var classInex in Enumerable.Range(0, outputData.ClassCounts.Count))
            {
                var classCount = outputData.ClassCounts[classInex];

                // クラスに含まれるオブジェクト数が0以下の場合はスキップ
                if (0 >= classCount)
                {
                    continue;
                }

                // クラスに含まれるオブジェクト数分の処理単位を作成
                foreach (var relativeIndex in Enumerable.Range(0, classCount))
                {
                    tempUnitList.Add(new ProcessingUnit(classInex, ++cumulativeObjectValue));   // オブジェクト値は1から始まる
                }
            }

            // 処理単位を並列処理数に合わせて分割する
            unitArrayList = tempUnitList
            .Select((unit, index) => new { unit, index })
            .GroupBy(x => x.index / this.Parameter.MaxDegreeOfParallelism)
            .Select(g => g.Select(x => x.unit).ToArray())
            .ToList();

            // 画像サイズ比
            fitToOriginalImage.Scale = originalImageSize.Width / (double)outputData.ClassFrameSize.Width;
            if (originalImageSize.Height > originalImageSize.Width)
            {
                fitToOriginalImage.Scale = originalImageSize.Height / (double)outputData.ClassFrameSize.Height;
            }

            // 画像サイズ比に合わせたパディング
            fitToOriginalImage.Padding.Width = (originalImageSize.Width - outputData.ClassFrameSize.Width * fitToOriginalImage.Scale) / 2;
            fitToOriginalImage.Padding.Height = (originalImageSize.Height - outputData.ClassFrameSize.Height * fitToOriginalImage.Scale) / 2;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"{ex.Message}");
        }

        return unitArrayList;
    }

    /// <summary>
    /// ラベリング実行
    /// </summary>
    /// <param name="unitArray">PrepareLabelingWithRleによって事前に生成された処理単位</param>
    /// <returns>処理結果</returns>
    public List<ProcessingResult> LabelingWithRle(ProcessingUnit[] unitArray)
    {
        var resultList = new List<ProcessingResult>();

        try
        {
            if (this.Parameter is null)
            {
                throw new Exception("Parameter is not set.");
            }

            if (this.OutputData is not IOnnxDataOutputClassIndexedInstance outputData)
            {
                throw new Exception("Output data is not set.");
            }

            // 処理結果の一次格納用配列
            var tempResults = new ProcessingResult[unitArray.Length];

            Parallel.For(0, unitArray.Length, this.ParallelOptions, unitIndex =>
            {
                try
                {
                    // 各種データへの参照を取得する
                    var selectedUnit = unitArray[unitIndex];
                    var selectedRle = this.RleController[unitIndex];
                    var selectedResult = tempResults[unitIndex] = new ProcessingResult(selectedUnit, unitIndex);
                    var classIndex = selectedUnit.ClassIndex;
                    var objectValue = selectedUnit.ObjectValue;

                    // 画像データからRleデータを作成する
                    selectedRle.CreateNewRleFrom(outputData.ClassIndexed[classIndex], outputData.ClassFrameSize.Width, objectValue, objectValue);

                    // ラベリングを実行する
                    selectedRle.SynchronousLabeling();

                    // 処理結果を格納する
                    tempResults[unitIndex].Labels = selectedRle.CollectValidLabels(this.Parameter.IgnoreObjectsTouchingBoundary, label => 0 != label.RleDataElem);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, $"{ex.Message}");
                }
            });

            // リスト化
            resultList = tempResults.Where(r => r is not null).ToList();
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"{ex.Message}");
        }

        return resultList;
    }

    /// <summary>
    /// ラベルの輪郭を取得する
    /// </summary>
    /// <param name="result">LabelingWithRleで得られた結果の1つ</param>
    /// <param name="contourPoints">輪郭座標</param>
    public void GetLabelContour(ProcessingResult result, IRleLabel selectedLabel, out PointF[] contourPoints)
    {
        contourPoints = Array.Empty<PointF>();

        try
        {
            this.RleController[result.RleIndex].GetLabelContour(selectedLabel, out List<ContourElementData> rawContours, out _);

            contourPoints = rawContours.ConvertAll(e => new PointF(e.Point.X + 0.5f, e.Point.Y + 0.5f)).ToArray();
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"{ex.Message}");
        }
    }

    /// <summary>
    /// ラベルの輪郭を取得する
    /// </summary>
    /// <param name="result">LabelingWithRleで得られた結果の1つ</param>
    /// <param name="contours">輪郭情報</param>
    /// <param name="contourCenter"></param>
    public void GetLabelContour(ProcessingResult result, IRleLabel selectedLabel, out List<ContourElementData> contours, out PointD contourCenter)
    {
        contours = new();
        contourCenter = new PointD();

        try
        {
            this.RleController[result.RleIndex].GetLabelContour(selectedLabel, out contours, out contourCenter);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"{ex.Message}");
        }
    }

    /// <summary>
    /// ラベルのランレングス情報を取得する
    /// </summary>
    /// <param name="result">LabelingWithRleで得られた結果の1つ</param>
    /// <param name="listRunLength">ランレングス情報</param>
    public void GetLabelRunLength(ProcessingResult result, IRleLabel selectedLabel, out List<RunLength> listRunLength)
    {
        listRunLength = new List<RunLength>();

        try
        {
            var selectedRle = this.RleController[result.RleIndex];

            for (int i = selectedLabel.RleDataBegin; i <= selectedLabel.RleDataEnd; i++)
            {
                var selectedRun = selectedRle.RleData[i];
                if (selectedRun.LabelIndex == selectedLabel.Index)
                {
                    var run = new RunLength()
                    {
                        X = selectedRun.CoordX_Begin,
                        Y = selectedRun.CoordY,
                        Length = selectedRun.CoordX_End - selectedRun.CoordX_Begin + 1,
                    };

                    listRunLength.Add(run);
                }
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"{ex.Message}");
        }
    }

    #endregion
}