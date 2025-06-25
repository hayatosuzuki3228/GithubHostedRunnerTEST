using Hutzper.Library.Common.Forms;
using System.Collections;

namespace Hutzper.Library.Common.Laungage
{
    /// <summary>
    /// 翻訳ヘルパ
    /// </summary>
    [Serializable]
    public class TranslationHelper : ILoggable
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

        /// <summary>
        /// 翻訳インスタンス
        /// </summary>
        public ITranslator? Translator { get; init; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="translator"></param>
        public TranslationHelper(ITranslator? translator)
        {
            this.Translator = translator;
        }

        /// <summary>
        /// コントロール翻訳
        /// </summary>
        public void TranslateControl(Control control)
        {
            try
            {
                if (this.Translator is not null)
                {
                    if (ControlUtilities.GetAllControls(control) is IList<Control> collection)
                    {
                        foreach (var item in collection)
                        {
                            this.TranslateControlWithList(item);
                            item.Text = this.Translator.Translate(item.Text);
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
        /// コントロールの翻訳
        /// </summary>
        /// <param name="container"></param>
        public void TranslateControlWithList(Control control)
        {
            try
            {
                if (this.Translator is not null)
                {
                    if (control is ComboBox comboBox)
                    {
                        this.TranslateList(comboBox.Items);
                    }
                    else if (control is ListBox listBox)
                    {
                        this.TranslateList(listBox.Items);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// IList翻訳
        /// </summary>
        /// <param name="list"></param>
        public void TranslateList(IList list)
        {
            try
            {
                if (this.Translator is not null)
                {
                    foreach (var i in Enumerable.Range(0, list.Count))
                    {
                        if (list[i] is string target)
                        {
                            if (false == string.IsNullOrEmpty(target))
                            {
                                list[i] = this.Translator.Translate(target);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }
    }
}