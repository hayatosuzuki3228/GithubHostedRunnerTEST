using Hutzper.Library.Common.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Hutzper.Library.Forms.Setting
{
    /// <summary>
    /// OnOff設定
    /// </summary>
    public partial class OnOffUserControl : HutzperUserControl
    {
        /// <summary>
        /// OnOff値
        /// </summary>
        public bool Value
        {
            get => this.toggleButtonValue;

            set
            {
                this.toggleButton1.CheckedChanged -= this.ToggleButton1_CheckedChanged;
                this.toggleButton1.Checked = this.toggleButtonValue = value;
                this.toggleButton1.CheckedChanged += this.ToggleButton1_CheckedChanged;
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
        /// トグルON時の背景色
        /// </summary>
        [Category("Toggle Appearance")]
        public Color OnBackColor { get => this.toggleButton1.OnBackColor; set => this.toggleButton1.OnBackColor = value; }

        /// <summary>
        /// トグルON字の前景色
        /// </summary>
        [Category("Toggle Appearance")]
        public Color OnToggleColor { get => this.toggleButton1.OnToggleColor; set => this.toggleButton1.OnToggleColor = value; }

        /// <summary>
        /// トグルOFF時の背景色
        /// </summary>
        [Category("Toggle Appearance")]
        public Color OffBackColor { get => this.toggleButton1.OffBackColor; set => this.toggleButton1.OffBackColor = value; }

        /// <summary>
        /// トグルOFF字の前景色
        /// </summary>
        [Category("Toggle Appearance")]
        public Color OffToggleColor { get => this.toggleButton1.OffToggleColor; set => this.toggleButton1.OffToggleColor = value; }

        /// <summary>
        /// 塗りつぶしスタイルを適用するかどうか
        /// </summary>
        [Category("Toggle Appearance")]
        [DefaultValue(true)]
        public bool SolidStyle { get => this.toggleButton1.SolidStyle; set => this.toggleButton1.SolidStyle = value; }

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

        protected bool toggleButtonValue;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OnOffUserControl()
        {
            this.InitializeComponent();

            this.Size = new Size(240, 40);

            this.toggleButtonValue = this.toggleButton1.Checked;
            this.toggleButton1.OnBackColor = Color.DodgerBlue;
            this.toggleButton1.OnToggleColor = Color.WhiteSmoke;
            this.toggleButton1.CheckedChanged += this.ToggleButton1_CheckedChanged;

            this.toggleButton1.Visible = true;
            this.toggleButton1.Enabled = true;

            this.label1.Visible = true;
            this.label1.Enabled = true;

            this.label1.Click += (sender, e) =>
            {
                this.toggleButton1.Checked = !this.toggleButton1.Checked;
            };

            this.isTextShrinkToFit = true;
            this.textSpecifiedFontSize = this.label1.Font.Size;
        }

        /// <summary>
        /// トグルボタンチェック状態変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton1_CheckedChanged(object? sender, EventArgs e)
        {
            try
            {
                this.toggleButtonValue = this.toggleButton1.Checked;
                this.ValueChanged?.Invoke(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void OnOffUserControl_Resize(object sender, EventArgs e)
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