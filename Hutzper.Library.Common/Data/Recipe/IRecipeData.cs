using System.Diagnostics;

namespace Hutzper.Library.Common.Data.Recipe
{
    /// <summary>
    /// レシピデータインタフェース
    /// </summary>
    public interface IRecipeData : IDirectoryCompatible, IRecordable, ISafelyDisposable
    {
        /// <summary>
        /// ファイル名を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetFileName();
    }

    /// <summary>
    /// IRecipeData実装;class
    /// </summary>
    [Serializable]
    public abstract class RecipeDataBase : SafelyDisposable, IRecipeData
    {
        #region IRecordable

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; } = string.Empty;

        /// <summary>
        /// 日時
        /// </summary>
        public virtual DateTime DateTime { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public virtual string Description { get; set; } = string.Empty;

        #endregion

        #region IRecipeData

        /// <summary>
        /// ファイル名を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual string GetFileName() => "recipe.dat";

        #endregion

        #region IDirectoryCompatible

        /// <summary>
        /// ディレクトリを指定して読み込み
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public virtual bool Load(DirectoryInfo directory, params string[] fileNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ディレクトリを指定して書き込み
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public virtual bool Save(DirectoryInfo directory, params string[] fileNames)
        {
            throw new NotImplementedException();
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
                Debug.WriteLine(ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// IRecipeData実装;record
    /// </summary>
    [Serializable]
    public abstract record RecipeDataBaseRecord : SafelyDisposableRecord, IRecipeData
    {
        #region IRecordable

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; } = string.Empty;

        /// <summary>
        /// 日時
        /// </summary>
        public virtual DateTime DateTime { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public virtual string Description { get; set; } = string.Empty;

        #endregion

        #region IRecipeData

        /// <summary>
        /// ファイル名を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual string GetFileName() => "recipe.dat";

        #endregion

        #region IDirectoryCompatible

        /// <summary>
        /// ディレクトリを指定して読み込み
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public virtual bool Load(DirectoryInfo directory, params string[] fileNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ディレクトリを指定して書き込み
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public virtual bool Save(DirectoryInfo directory, params string[] fileNames)
        {
            throw new NotImplementedException();
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
                Debug.WriteLine(ex);
            }
        }

        #endregion
    }
}