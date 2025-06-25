using Hutzper.Library.Common;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement
{
    /// <summary>
    /// 推論結果判定
    /// </summary>
    public interface IInferenceResultJudgment : ILoggable
    {
        public bool ExcecuteJudgment(IInspectionTask selectedTask, IInferenceResultJudgmentParameter parameter);
    }

    /// <summary>
    /// IInferenceResultJudgment実装
    /// </summary>
    public abstract class InferenceResultJudgment : IInferenceResultJudgment
    {
        #region IInferenceResultJudgment

        /// <summary>
        /// 実行
        /// </summary>
        public virtual bool ExcecuteJudgment(IInspectionTask selectedTask, IInferenceResultJudgmentParameter parameter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ILoggable<LogCategory>

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName
        {
            #region 取得
            get
            {
                if (0 <= Index)
                {
                    return string.Format("{0}{1:D2}", Nickname, Index + 1);
                }
                else
                {
                    return string.Format("{0}", Nickname);
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

                if (false == string.IsNullOrEmpty(nickname))
                {
                    value = nickname;
                }

                return value;
            }
            #endregion

            #region 更新
            set
            {
                nickname = value;
            }
            #endregion
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
        public void Attach(string nickname, int index)
        {
            try
            {
                this.nickname = nickname;
                Index = index;
            }
            catch
            {
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InferenceResultJudgment() : this(typeof(InferenceResultJudgment).Name, 0)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InferenceResultJudgment(string nickname, int index)
        {
            Attach(nickname, index);
        }

        #endregion
    }
}