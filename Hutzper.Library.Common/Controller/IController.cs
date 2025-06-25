using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;

namespace Hutzper.Library.Common.Controller
{
    /// <summary>
    /// 制御系インタフェース
    /// </summary>
    /// <remarks>制御系クラスはこのインタフェースを継承して、統一された手順で扱えるようにします。</remarks>
    public interface IController : ISafelyDisposable, ILoggable
    {
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public void Initialize(IServiceCollectionSharing? serviceCollection);

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="config"></param>
        public void SetConfig(IApplicationConfig? config);

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public void SetParameter(IControllerParameter? parameter);

        /// <summary>
        /// 更新
        /// </summary>
        public void Update();

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public bool Open();

        /// <summary>
        /// クローズ
        /// </summary>
        public bool Close();
    }

    /// <summary>
    /// 制御系基本クラス
    /// </summary>」
    /// <remarks>IControllerの実装です。</remarks>
    public abstract class ControllerBase : SafelyDisposable, IController
    {
        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName
        {
            #region 取得
            get
            {
                if (0 <= this.Index)
                {
                    return string.Format("{0}{1:D2}", this.Nickname, this.Index + 1);
                }
                else
                {
                    return string.Format("{0}", this.Nickname);
                }
            }
            #endregion
        }

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            #region 取得
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(this.nickname))
                {
                    value = this.nickname;
                }

                return value;
            }
            #endregion

            #region 更新
            set
            {
                this.nickname = value;
            }
            #endregion
        }
        protected string nickname = String.Empty;

        /// <summary>
        /// 整数インデックス
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public void Attach(string nickname, int index)
        {
            try
            {
                this.nickname = nickname;
                this.Index = index;
            }
            catch
            {
            }
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
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public virtual void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            try
            {
                this.Services = serviceCollection;
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
        public virtual void SetConfig(IApplicationConfig? config)
        {
            try
            {
                this.Config = config;
                this.PathManager = this.Config?.GetPathManager();
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
        public virtual void SetParameter(IControllerParameter? parameter)
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
        /// 更新
        /// </summary>
        public virtual void Update()
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
        public virtual bool Open()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return false;
        }


        /// <summary>
        /// クローズ
        /// </summary>
        public virtual bool Close()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return true;
        }

        #endregion

        #region フィールド

        // IServiceCollection
        protected IServiceCollectionSharing? Services;

        // IPathManager
        protected IPathManager? PathManager;

        protected string[] Directories = Array.Empty<string>();

        protected IApplicationConfig? Config;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public ControllerBase(string nickname, int index = -1)
        {
            this.Attach(nickname, index);
        }

        #endregion
    }
}