using System.Diagnostics;

namespace Hutzper.Library.Common.Data
{
    /// <summary>
    /// 結果データインタフェース
    /// </summary>
    /// <remarks>結果データはこのインタフェースを継承します</remarks>
    public interface IResultData : IDirectoryCompatible, IRecordable, ISafelyDisposable
    {
    }

    /// <summary>
    /// IResultData 実装:class
    /// </summary>
    [Serializable]
    public abstract class ResultDataBase : SafelyDisposable, IResultData
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
    }

    /// <summary>
    /// IResultData 実装:record
    /// </summary>
    [Serializable]
    public abstract record ResultDataBaseRecord : SafelyDisposableRecord, IResultData
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
    }
}