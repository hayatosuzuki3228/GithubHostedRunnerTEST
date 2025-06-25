using Hutzper.Library.Common.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Hutzper.Library.Forms.Setting
{
    /// <summary>
    /// コンボボックス
    /// </summary>
    public partial class NumericUpDownUserControl : HutzperUserControl
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
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public int NumericUpDownWidth
        {
            get => this.NumericUpDown.Width;
            set => this.NumericUpDown.Width = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public int NumericUpDownDecimalPlaces
        {
            get => this.NumericUpDown.DecimalPlaces;
            set => this.NumericUpDown.DecimalPlaces = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal NumericUpDownIncrement
        {
            get => this.NumericUpDown.Increment;
            set => this.NumericUpDown.Increment = value;
        }

        /// <summary>
        /// NumericUpDown:入力値
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal NumericUpDownValue
        {
            get => this.NumericUpDown.Value;
            set => this.NumericUpDown.Value = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal NumericUpDownMaximum
        {
            get => this.NumericUpDown.Maximum;
            set => this.NumericUpDown.Maximum = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal NumericUpDownMinimum
        {
            get => this.NumericUpDown.Minimum;
            set => this.NumericUpDown.Minimum = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public bool NumericUpDownThousandsSeparator
        {
            get => this.NumericUpDown.ThousandsSeparator;
            set => this.NumericUpDown.ThousandsSeparator = value;
        }

        /// <summary>
        /// NumericUpDown:入力値
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal ValueDecimal
        {
            get => this.NumericUpDown.Value;
            set
            {
                this.notifyEventEnabled = false;
                this.NumericUpDown.Value = value;
                this.notifyEventEnabled = true;
            }
        }

        /// <summary>
        /// NumericUpDown:入力値
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public double ValueDouble
        {
            get => (double)this.NumericUpDown.Value;
            set
            {
                this.notifyEventEnabled = false;
                this.NumericUpDown.Value = (decimal)value;
                this.notifyEventEnabled = true;
            }
        }

        /// <summary>
        /// NumericUpDown:入力値
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public int ValueInt
        {
            get => (int)this.NumericUpDown.Value;
            set
            {
                this.notifyEventEnabled = false;
                this.NumericUpDown.Value = value;
                this.notifyEventEnabled = true;
            }
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
        /// イベント:選択インデックス変更
        /// </summary>
        public event Action<object, decimal>? ValueChanged;

        #endregion

        #region フィールド

        /// <summary>
        /// イベント通知するかどうか
        /// </summary>
        protected bool notifyEventEnabled = true;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NumericUpDownUserControl()
        {
            this.InitializeComponent();

            this.Size = new Size(240, 40);

            this.label1.Visible = true;
            this.label1.Enabled = true;
            this.isTextShrinkToFit = true;
            this.textSpecifiedFontSize = this.label1.Font.Size;
        }

        #region GUIイベント

        /// <summary>
        /// サイズ変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void NumericUpDownUserControl_Resize(object sender, EventArgs e)
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
        #endregion

        #region プロテクテッドメソッド

        #endregion

        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (true == this.notifyEventEnabled)
            {
                this.ValueChanged?.Invoke(this, this.NumericUpDown.Value);
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