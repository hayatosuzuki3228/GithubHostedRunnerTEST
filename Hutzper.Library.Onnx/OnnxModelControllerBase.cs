using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Onnx.Model;
using System.Diagnostics;

namespace Hutzper.Library.Onnx;

/// <summary>
/// ONNXモデル制御インタフェース実装
/// </summary>
[Serializable]
public class OnnxModelControllerBase : ControllerBase, IOnnxModelController
{
    #region IOnnxModelControllercs

    /// <summary>
    /// モデルインスタンス数
    /// </summary>
    public virtual int NumberOfModel => this.Models.Count;

    /// <summary>
    /// ONNXモデルインスタンスリスト
    /// </summary>
    public List<IOnnxModel> Models { get; } = new();

    /// <summary>
    /// デバイス割り付け
    /// </summary>
    /// <param name="devices"></param>
    /// <returns></returns>
    public virtual bool Attach(params IOnnxModel[] models)
    {
        var isSuccess = true;

        try
        {
            foreach (var m in models)
            {
                if (false == this.Models.Contains(m))
                {
                    if (m is not null)
                    {
                        this.Models.Add(m);
                    }
                    else
                    {
                        throw new Exception("model is null");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            isSuccess = false;
            Serilog.Log.Warning(ex, ex.Message);
        }

        return isSuccess;
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

            this.Models.ForEach(g => g.Initialize(serviceCollection));
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
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

            this.Models.ForEach(g => g.SetConfig(config));
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

            if (parameter is IOnnxModelControllerParameter cp)
            {
                foreach (var model in this.Models)
                {
                    var index = this.Models.IndexOf(model);

                    if (cp.ModelParameters.Count > index)
                    {
                        model.SetParameter(cp.ModelParameters[index]);
                    }
                }
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
            base.Update();

            this.Models.ForEach(g => g.Update());
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
        var isSuccess = true;

        try
        {
            this.Models.ForEach(g => isSuccess &= g.Open());
        }
        catch (Exception ex)
        {
            isSuccess = false;
            Serilog.Log.Warning(ex, ex.Message);
        }

        return isSuccess;
    }

    /// <summary>
    /// クローズ
    /// </summary>
    public override bool Close()
    {
        var isSuccess = true;

        try
        {
            this.Models.ForEach(g => isSuccess &= g.Close());
        }
        catch (Exception ex)
        {
            isSuccess = false;
            Serilog.Log.Warning(ex, ex.Message);
        }

        return isSuccess;
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
            foreach (var item in this.Models)
            {
                this.DisposeSafely(item);
            }
            this.Models.Clear();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="index"></param>
    public OnnxModelControllerBase() : this(typeof(OnnxModelControllerBase).Name)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="index"></param>
    public OnnxModelControllerBase(string nickname, int index = -1) : base(nickname, index)
    {
    }

    #endregion
}