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
/// <remarks>セマンティックセグメンテーションの処理結果を扱うことを想定しています</remarks>
public class BlobDetection : ControllerBase, IController
{
    #region IController

    /// <summary>
    /// パラメーター設定
    /// </summary>
    public override void SetParameter(IControllerParameter? parameter)
    {
        try
        {
            base.SetParameter(parameter);

            if (parameter is BlobDetectionParameter bdp)
            {
                this.Parameter = bdp;
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }
    }

    /// <summary>
    /// オープン
    /// </summary>
    public override bool Open()
    {
        try
        {
            if (this.Parameter is BlobDetectionParameter bdp)
            {
                this.RleController = new IRleController[bdp.MaxNumberOfClasses];
                Parallel.For(0, this.RleController.Length, i =>
                {
                    this.RleController[i] = new RleController();
                    this.RleController[i].Initialize(this.Services);
                    this.RleController[i].SetParameter(bdp.RleControllerParameter);
                    this.RleController[i].Open();
                });
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }

        return 0 < this.RleController.Length;
    }

    #endregion

    /// <summary>
    /// パラメータ
    /// </summary>
    protected BlobDetectionParameter? Parameter;

    /// <summary>
    /// Rle
    /// </summary>
    protected IRleController[] RleController = Array.Empty<IRleController>();

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="index">インスタンスを識別するためのインデックス</param>
    public BlobDetection(int index = -1) : base(typeof(BlobDetection).Name, index)
    {
        this.Parameter = new BlobDetectionParameter();
    }

    #endregion

    #region ISafelyDisposable

    /// <summary>
    /// リソースの破棄
    /// </summary>
    protected override void DisposeExplicit()
    {
        try
        {
            base.DisposeExplicit();

            // 配列の要素を個別に解放
            foreach (var controller in this.RleController)
            {
                this.DisposeSafely(controller);
            }

            // 配列自体を解放
            this.RleController = Array.Empty<IRleController>();
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, ex.Message);
        }
    }

    #endregion

    #region メソッド

    /// <summary>
    /// ラベリング実行
    /// </summary>
    /// <param name="outputData">対象のonnx出力データ</param>
    /// <param name="threshold">閾値(マスク画像内で検出された画素に格納される値)</param>
    /// <returns>ラベルリスト</returns>
    public List<IRleLabel>[] LabelingWithRle(IOnnxDataOutputClassIndexed outputData, int threshold = 1)
    {
        var labels = Array.Empty<List<IRleLabel>>();

        try
        {
            if (this.Parameter is BlobDetectionParameter bdp)
            {
                if (outputData.ClassIndexed.Count > bdp.MaxNumberOfClasses)
                {
                    throw new Exception("The number of classes that can be processed has been exceeded.");
                }

                // 処理結果を初期化する
                labels = new List<IRleLabel>[outputData.ClassIndexed.Count];

                var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = bdp.MaxDegreeOfParallelism };
                Parallel.For(0, labels.Length, parallelOptions, classIndex =>
                {
                    try
                    {
                        // 使用するRleContorollerへの参照を取得する
                        var selectedRle = this.RleController[classIndex];

                        // 画像データからRleデータを作成する
                        selectedRle.CreateNewRleFrom(outputData.ClassIndexed[classIndex], outputData.ClassFrameSize.Width, threshold);

                        // ラベリングを実行する
                        selectedRle.SynchronousLabeling();

                        // 処理結果を格納する
                        labels[classIndex] = selectedRle.CollectValidLabels(bdp.IgnoreObjectsTouchingBoundary, label => 0 != label.RleDataElem);
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                });
            }
            else
            {
                throw new Exception("Parameter is not set.");
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }

        return labels ?? Array.Empty<List<IRleLabel>>();
    }

    /// <summary>
    /// ラベルの輪郭を取得する
    /// </summary>
    /// <param name="selectedLabel">対象のラベル</param>
    /// <param name="classInex">クラスインデックス</param>
    /// <param name="contourPoints">輪郭座標</param>
    public void GetLabelContour(IRleLabel selectedLabel, int classInex, out PointF[] contourPoints)
    {
        contourPoints = Array.Empty<PointF>();

        try
        {
            this.RleController[classInex].GetLabelContour(selectedLabel, out List<ContourElementData> rawContours, out _);

            contourPoints = rawContours.ConvertAll(e => new PointF(e.Point.X + 0.5f, e.Point.Y + 0.5f)).ToArray();
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }
    }

    /// <summary>
    /// ラベルの輪郭を取得する
    /// </summary>
    /// <param name="selectedLabel">対象のラベル</param>
    /// <param name="classInex">クラスインデックス</param>
    /// <param name="contours">輪郭情報</param>
    /// <param name="contourCenter"></param>
    public void GetLabelContour(IRleLabel selectedLabel, int classInex, out List<ContourElementData> contours, out PointD contourCenter)
    {
        contours = new();
        contourCenter = new PointD();

        try
        {
            this.RleController[classInex].GetLabelContour(selectedLabel, out contours, out contourCenter);
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }
    }

    /// <summary>
    /// ラベルのランレングス情報を取得する
    /// </summary>
    /// <param name="selectedLabel"></param>
    /// <param name="classInex">クラスインデックス</param>
    /// <param name="listRunLength">ランレングス情報</param>
    public void GetLabelRunLength(IRleLabel selectedLabel, int classInex, out List<RunLength> listRunLength)
    {
        listRunLength = new List<RunLength>();

        try
        {
            var selectedRle = this.RleController[classInex];

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
            Serilog.Log.Warning(ex, ex.Message);
        }
    }

    #endregion
}