using Hutzper.Library.Common;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.PostProcessing
{
    /// <summary>
    /// 推論結果後処理
    /// </summary>
    public interface IInferenceResultPostProcessing : ILoggable
    {
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(IInferenceResultPostProcessingParameter parameter);

        /// <summary>
        /// 後処理の実行
        /// </summary>
        /// <param name="selectedTaskItem">対象のタスク項目</param>
        /// <returns>処理の成功/失敗</returns>
        public bool ExcecutePostProcessing(IInspectionTaskItem selectedTaskItem);
    }

    /// <summary>
    /// IInferenceResultPostProcessing実装
    /// </summary>
    public abstract class InferenceResultPostProcessing : IInferenceResultPostProcessing
    {
        #region IInferenceResultPostProcessing

        /// <summary>
        /// 初期化
        /// </summary>
        public virtual void Initialize(IInferenceResultPostProcessingParameter parameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 実行
        /// </summary>
        public virtual bool ExcecutePostProcessing(IInspectionTaskItem selectedTaskItem)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName => 0 <= Index ? string.Format("{0}{1:D2}", Nickname, Index + 1) : string.Format("{0}", Nickname);

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(nickname))
                {
                    value = nickname;
                }

                return value;
            }

            set => nickname = value;
        }
        protected string nickname = string.Empty;

        /// <summary>
        /// 整数インデックス
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public virtual void Attach(string nickname, int index)
        {
            this.nickname = nickname;
            Index = index;
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InferenceResultPostProcessing() : this(typeof(InferenceResultPostProcessing).Name, 0)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InferenceResultPostProcessing(string nickname, int index)
        {
            Attach(nickname, index);
        }

        #endregion
    }
}
