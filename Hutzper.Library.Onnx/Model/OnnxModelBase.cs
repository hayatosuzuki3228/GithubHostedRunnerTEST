using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Onnx.Data;
using Microsoft.ML.OnnxRuntime;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Hutzper.Library.Onnx.Model;

/// <summary>
/// ONNXモデル基底クラス
/// </summary>
[Serializable]
public abstract class OnnxModelBase : ControllerBase, IOnnxModel
{
    #region IOnnxModel

    /// <summary>
    /// 識別
    /// </summary>
    public virtual Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }
    public virtual string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// 画像が取得可能な状態かどうか
    /// </summary>
    /// <remarks>Open済みか</remarks>
    public virtual bool Enabled => this.InferenceSession is not null;

    public event Action<object, Exception>? Disabled;

    /// <summary>
    /// モデルのアルゴリズム
    /// </summary>
    public virtual OnnxModelAlgorithm Algorithm { get; protected set; }

    /// <summary>
    /// ExecutionProvider
    /// </summary>
    public virtual OnnxModelExecutionProvider ExecutionProvider { get; protected set; }

    /// <summary>
    /// 入力メタデータ取得
    /// </summary>
    /// <returns></returns>
    public virtual IReadOnlyDictionary<string, NodeMetadata> GetInputMetadata() => this.InferenceSession?.InputMetadata ?? new ReadOnlyDictionary<string, NodeMetadata>(new Dictionary<string, NodeMetadata>());

    /// <summary>
    /// 出力メタデータ取得
    /// </summary>
    /// <returns></returns>
    public virtual IReadOnlyDictionary<string, NodeMetadata> GetOutputMetadata() => this.InferenceSession?.OutputMetadata ?? new ReadOnlyDictionary<string, NodeMetadata>(new Dictionary<string, NodeMetadata>());

    /// <summary>
    /// 実行
    /// </summary>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public virtual IOnnxDataOutput Run(IOnnxDataInput inputData)
    {
        var outputData = this.GetInitializedOutputData();

        try
        {
            var watch = Stopwatch.StartNew();
            var inputs = inputData.ToReadOnlyCollection();

            watch.Stop();
            Serilog.Log.Debug($"input time is {watch.Elapsed.TotalMilliseconds}");

            watch.Restart();

            var result = this.InferenceSession?.Run(inputs);

            watch.Stop();
            Serilog.Log.Debug($"run time is {watch.Elapsed.TotalMilliseconds}");

            watch.Restart();

            outputData.CopyFrom(result);

            watch.Stop();
            Serilog.Log.Debug($"output time is {watch.Elapsed.TotalMilliseconds}");
        }
        catch (Microsoft.ML.OnnxRuntime.OnnxRuntimeException ex)
        {
            Serilog.Log.Error("同じタイミングに推論が実行しすぎると起こるです。");
            this.Disabled?.Invoke(this, ex);
            Serilog.Log.Warning(ex, ex.Message);
        }
        catch (Exception ex)
        {
            this.Disabled?.Invoke(this, ex);
            Serilog.Log.Warning(ex, ex.Message);
        }
        return outputData;
    }

    /// <summary>
    /// 空撃ち
    /// </summary>
    /// <returns></returns>
    public virtual List<TimeSpan> DryFire(int tryCount)
    {
        var result = new List<TimeSpan>();
        try
        {
            var inputData = new OnnxDataInput();
            inputData.Initialize(this);

            var inputs = inputData.ToReadOnlyCollectionForDryFire();

            foreach (var i in Enumerable.Range(0, System.Math.Max(0, tryCount)))
            {
                var watch = new Stopwatch();
                watch.Restart();

                using var outputs = this.InferenceSession?.Run(inputs);

                watch.Stop();
                result.Add(watch.Elapsed);
            }
        }
        catch (Exception ex)
        {
            this.Disabled?.Invoke(this, ex);
            Serilog.Log.Warning(ex, ex.Message);
        }

        return result;
    }

    #endregion

    #region IController

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="serviceCollection"></param>
    public override void Initialize(IServiceCollectionSharing? serviceCollection)
    {
        try
        {
            base.Initialize(serviceCollection);

            this.DisposeSafely(this.InferenceSession);
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }
        finally
        {
            this.InferenceSession = null;
        }
    }

    /// <summary>
    /// 設定
    /// </summary>
    /// <param name="config"></param>
    public override void SetConfig(IApplicationConfig? config)
    {
        try
        {
            base.SetConfig(config);
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }
    }

    /// <summary>
    /// パラメーター設定
    /// </summary>
    /// <param name="parameter"></param>
    public override void SetParameter(IControllerParameter? parameter)
    {
        try
        {
            base.SetParameter(parameter);

            if (parameter is IOnnxModelParameter p)
            {
                this.Parameter = p;
                this.Location = p.Location;

                this.Algorithm = p.Algorithm;
                this.ExecutionProvider = p.ExecutionProvider;
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    public override void Update()
    {
        try
        {
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
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
            if (this.Parameter is not null && this.InferenceSession is null)
            {
                var fileInfo = new FileInfo(this.Parameter.OnnxModelFullFileName);

                if (true == fileInfo.Exists)
                {
                    this.InferenceSession = new InferenceSession(fileInfo.FullName);
                }
            }
        }
        catch (Exception ex)
        {
            this.InferenceSession = null;
            Serilog.Log.Warning(ex, ex.Message);
        }

        return this.Enabled;
    }


    /// <summary>
    /// クローズ
    /// </summary>
    public override bool Close()
    {
        try
        {
            this.DisposeSafely(this.InferenceSession);
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }
        finally
        {
            this.InferenceSession = null;
        }

        return true;
    }

    #endregion

    #region SafelyDisposable

    /// <summary>
    /// リソースの解放
    /// </summary>
    protected override void DisposeExplicit()
    {
        try
        {
            this.Close();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    #endregion

    #region フィールド

    /// <summary>
    /// 識別
    /// </summary>
    protected Common.Drawing.Point location;

    /// <summary>
    /// パラメータ
    /// </summary>
    protected IOnnxModelParameter? Parameter;

    /// <summary>
    /// セッション
    /// </summary>
    protected InferenceSession? InferenceSession;

    /// <summary>
    /// 処理時間計測用ストップウォッチ
    /// </summary>
    protected Stopwatch SessionRunStopwatch = new();

    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="index"></param>
    public OnnxModelBase(string nickname, int locationX, int locationY) : this(nickname, new Common.Drawing.Point(locationX, locationY))
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="index"></param>
    public OnnxModelBase(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
    {
        this.location = location.Clone();
    }
    public OnnxModelBase(string Identifier) : base(Identifier)
    {
        this.location = new Common.Drawing.Point(-1, -1);
        this.Identifier = Identifier;
    }
    #endregion

    /// <summary>
    /// 出力データの取得
    /// </summary>
    /// <returns></returns>
    protected virtual IOnnxDataOutput GetInitializedOutputData()
    {
        IOnnxDataOutput outputData = new OnnxDataOutput();

        try
        {
            switch (this.Algorithm)
            {
                case OnnxModelAlgorithm.Classification:
                    {
                        outputData = new OnnxDataOutputClassProbability();
                    }
                    break;
                case OnnxModelAlgorithm.ClassificationFGVC:
                    {
                        outputData = new OnnxDataOutputClassProbability();
                    }
                    break;

                case OnnxModelAlgorithm.AnomalyDetection:
                    {
                        outputData = new OnnxDataOutputAnomalyScore();
                    }
                    break;

                case OnnxModelAlgorithm.Segmentation_Image:
                    {
                        outputData = new OnnxDataOutputSegmentation();
                    }
                    break;
                case OnnxModelAlgorithm.ObjectDetection:
                    {
                        outputData = new OnnxDataOutputObjectDetection();
                    }
                    break;
                case OnnxModelAlgorithm.Segmentation_Instance:
                    {
                        outputData = new OnnxDataOutputSegmentationInstance();
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }

        try
        {
            outputData.Initialize(this);
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
        }

        return outputData;
    }
}