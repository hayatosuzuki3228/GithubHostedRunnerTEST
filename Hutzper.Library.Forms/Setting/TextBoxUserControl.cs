using Hutzper.Library.Common.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Hutzper.Library.Forms.Setting
{
    /// <summary>
    /// テキストボックスユーザーコントロール
    /// </summary>
    public partial class TextBoxUserControl : HutzperUserControl
    {
        /// <summary>
        /// テキスト
        /// </summary>
        public string Value
        {
            get => this.watermarkTextbox1.Text;

            set
            {
                this.watermarkTextbox1.TextChanged -= this.watermarkTextbox1_TextChanged;
                this.watermarkTextbox1.Text = value;
                this.watermarkTextbox1.TextChanged += this.watermarkTextbox1_TextChanged;
            }
        }

        /// <summary>
        /// ラベル文字列
        /// </summary>
        [Category("Label Appearance")]
        public string LabelText
        {
            get => this.label1.Text;
            set
            {
                this.label1.Text = string.IsNullOrEmpty(value) ? " " : value;
            }
        }

        /// <summary>
        /// フォント
        /// </summary>
        [Category("Label Appearance")]
        public Font LabelFont
        {
            get => this.label1.Font;
            set
            {
                this.label1.Font = value;
                this.textSpecifiedFontSize = this.label1.Font.Size;

                // ラベル更新
                this.UpdateLabel();
            }
        }

        /// <summary>
        /// 文字色
        /// </summary>
        [Category("Label Appearance")]
        public Color LabelForeColor { get => this.label1.ForeColor; set { this.label1.ForeColor = value; } }

        /// <summary>
        /// コンボボックス幅
        /// </summary>
        [Category("TextBox Appearance")]
        public virtual int TextBoxWidth
        {
            #region 取得
            get
            {
                return this.watermarkTextbox1.Width;
            }
            #endregion

            #region 更新
            set
            {
                this.watermarkTextbox1.Width = value;
            }
            #endregion
        }

        /// <summary>
        /// TextBox:テキスト配置
        /// </summary>
        [Category("TextBox Appearance")]
        public HorizontalAlignment TextBoxTextAlign
        {
            get => this.watermarkTextbox1.TextAlign;
            set => this.watermarkTextbox1.TextAlign = value;
        }

        /// <summary>
        /// TextBox:透かし文字
        /// </summary>
        [Category("TextBox Appearance")]
        public string TextBoxWatermarkText
        {
            get => this.watermarkTextbox1.WatermarkText;
            set => this.watermarkTextbox1.WatermarkText = value;
        }

        /// <summary>
        /// TextBox:入力テキスト値
        /// </summary>
        [Category("TextBox Appearance")]
        public string TextBoxText
        {
            get => this.watermarkTextbox1.Text;
            set => this.watermarkTextbox1.Text = value;
        }

        /// <summary>
        /// TextBox:テキスト長
        /// </summary>
        [Category("TextBox Appearance")]
        public int TextBoxMaxLength
        {
            get => this.watermarkTextbox1.MaxLength;
            set => this.watermarkTextbox1.MaxLength = value;
        }

        /// <summary>
        /// TextBox:入力モード
        /// </summary>
        [Category("TextBox Appearance")]
        public ImeMode TextBoxImeMode
        {
            get => this.watermarkTextbox1.ImeMode;
            set => this.watermarkTextbox1.ImeMode = value;
        }

        /// <summary>
        /// 文字列全体を表示するフォントサイズに調整するか
        /// </summary>
        public override bool IsTextShrinkToFit
        {
            get => this.isTextShrinkToFit;

            set
            {
                this.isTextShrinkToFit = value;

                // ラベル更新
                this.UpdateLabel();

                this.Invalidate();
            }
        }

        /// <summary>
        /// イベント:OnOff値変更
        /// </summary>
        public event Action<object>? ValueChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextBoxUserControl()
        {
            this.InitializeComponent();

            this.Size = new Size(320, 46);

            this.label1.Visible = true;
            this.label1.Enabled = true;
            this.isTextShrinkToFit = true;
            this.textSpecifiedFontSize = this.label1.Font.Size;
        }

        private void watermarkTextbox1_TextChanged(object? sender, EventArgs e)
        {
            try
            {
                if (sender is not null)
                {
                    this.ValueChanged?.Invoke(this);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// ラベル更新
        /// </summary>
        protected void UpdateLabel()
        {
            try
            {
                if (true == this.isTextShrinkToFit)
                {
                    var usedWidth = 0;
                    foreach (Control c in this.flowLayoutPanel1.Controls)
                    {
                        if (this.label1.Equals(c))
                        {
                            usedWidth += c.Margin.Left + c.Margin.Right;
                            break;
                        }
                        usedWidth += c.Width + c.Margin.Left + c.Margin.Right;
                    }
                    int targetHeight = this.label1.Parent != null
                        ? Math.Min(this.label1.ClientSize.Height, this.label1.Parent.ClientSize.Height)
                        : this.label1.ClientSize.Height;

                    var targetSize = new SizeF(this.ClientSize.Width - usedWidth, targetHeight);
                    ControlUtilities.SetFontSizeForTextShrinkToFit(this.label1, this.textSpecifiedFontSize, this.label1.Text, targetSize);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        private void label1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // ラベル更新
                this.UpdateLabel();

                this.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void TextBoxUserControl_Resize(object sender, EventArgs e)
        {
            try
            {
                // ラベル更新
                this.UpdateLabel();

                this.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}