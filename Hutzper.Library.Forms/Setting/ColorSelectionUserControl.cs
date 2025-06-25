using Hutzper.Library.Common.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Hutzper.Library.Forms.Setting
{
    /// <summary>
    /// 色選択ユーザーコントロール
    /// </summary>
    public partial class ColorSelectionUserControl : HutzperUserControl
    {
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
        /// 色見本
        /// </summary>
        [Category("ColorUsageGuide")]
        public Color SelectedColor
        {
            get => this.buttonColorUsageGuide.BackColor;

            set => this.buttonColorUsageGuide.BackColor = value;
        }

        /// <summary>
        /// 色見本ラベル幅
        /// </summary>
        public virtual int ColorUsageGuideWidth
        {
            #region 取得
            get
            {
                return this.buttonColorUsageGuide.Width;
            }
            #endregion

            #region 更新
            set
            {
                this.buttonColorUsageGuide.Width = value;
            }
            #endregion
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

        #region イベント

        /// <summary>
        /// イベント:変更
        /// </summary>
        public event Action<object, Color>? SelectedColorChanged;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ColorSelectionUserControl()
        {
            this.InitializeComponent();

            this.label1.Visible = true;
            this.label1.Enabled = true;
            this.isTextShrinkToFit = true;
            this.textSpecifiedFontSize = this.label1.Font.Size;
        }

        private void buttonColorUsageGuide_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.OK == this.colorDialog1.ShowDialog(this))
                {
                    if (this.buttonColorUsageGuide.BackColor != this.colorDialog1.Color)
                    {
                        this.buttonColorUsageGuide.BackColor = this.colorDialog1.Color;

                        this.SelectedColorChanged?.Invoke(this, this.buttonColorUsageGuide.BackColor);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ColorSelectionUserControl_Resize(object sender, EventArgs e)
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
            this.UpdateLabel();

            this.Invalidate();
        }
    }
}