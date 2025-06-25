using System.ComponentModel;
using System.Diagnostics;

namespace Hutzper.Library.Forms
{
    /// <summary>
    /// 入力ボックスフォーム
    /// </summary>
    public partial class FloatingInputBoxForm : Form
    {
        #region サブクラス

        /// <summary>
        /// 入力タイプ
        /// </summary>
        public enum FloatingInputType
        {
            TextBox
        , NumericUpDown
        , Container
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 入力タイプ
        /// </summary>
        [Category("Custom Appearance")]
        public FloatingInputType InputType
        {
            get => this.inputType;
            protected set
            {
                this.inputType = value;

                this.textBox1.Visible = false;
                this.numericUpDown1.Visible = false;

                switch (this.inputType)
                {
                    case FloatingInputType.NumericUpDown:
                        this.numericUpDown1.Visible = true;
                        break;

                    case FloatingInputType.TextBox:
                        this.textBox1.Visible = true;
                        break;

                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// TextBox:テキスト配置
        /// </summary>
        [Category("TextBox Appearance")]
        public HorizontalAlignment TextBoxTextAlign
        {
            get => this.textBox1.TextAlign;
            set => this.textBox1.TextAlign = value;
        }

        /// <summary>
        /// TextBox:透かし文字
        /// </summary>
        [Category("TextBox Appearance")]
        public string TextBoxWatermarkText
        {
            get => this.textBox1.WatermarkText;
            set => this.textBox1.WatermarkText = value;
        }

        /// <summary>
        /// TextBox:入力テキスト値
        /// </summary>
        [Category("TextBox Appearance")]
        public string TextBoxText
        {
            get => this.textBox1.Text;
            set => this.textBox1.Text = value;
        }

        /// <summary>
        /// TextBox:テキスト長
        /// </summary>
        [Category("TextBox Appearance")]
        public int TextBoxMaxLength
        {
            get => this.textBox1.MaxLength;
            set => this.textBox1.MaxLength = value;
        }

        /// <summary>
        /// TextBox:入力モード
        /// </summary>
        [Category("TextBox Appearance")]
        public ImeMode TextBoxImeMode
        {
            get => this.textBox1.ImeMode;
            set => this.textBox1.ImeMode = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public int NumericUpDownDecimalPlaces
        {
            get => this.numericUpDown1.DecimalPlaces;
            set => this.numericUpDown1.DecimalPlaces = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal NumericUpDownIncrement
        {
            get => this.numericUpDown1.Increment;
            set => this.numericUpDown1.Increment = value;
        }

        /// <summary>
        /// NumericUpDown:入力値
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal NumericUpDownValue
        {
            get => this.numericUpDown1.Value;
            set => this.numericUpDown1.Value = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal NumericUpDownMaximum
        {
            get => this.numericUpDown1.Maximum;
            set => this.numericUpDown1.Maximum = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public decimal NumericUpDownMinimum
        {
            get => this.numericUpDown1.Minimum;
            set => this.numericUpDown1.Minimum = value;
        }

        /// <summary>
        /// NumericUpDown:小数点以下の桁数
        /// </summary>
        [Category("NumericUpDown Appearance")]
        public bool NumericUpDownThousandsSeparator
        {
            get => this.numericUpDown1.ThousandsSeparator;
            set => this.numericUpDown1.ThousandsSeparator = value;
        }

        #endregion

        #region イベント

        /// <summary>
        /// イベント:入力値変更
        /// </summary>
        public event Action<object>? InputValueChanged;

        public event Action<object, string>? InputTextChanged;

        #endregion

        #region フィールド

        protected FloatingInputType inputType;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FloatingInputBoxForm()
        {
            this.InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.None;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            this.TextBoxWatermarkText = "ここに入力";

            this.textBox1.TextChanged += this.textBox1_TextChanged;
            this.numericUpDown1.ValueChanged += this.textBox1_TextChanged;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// TextBoxの表示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="defaultText"></param>
        public void Fit(IWin32Window owner, Control target, string defaultText)
        {
            this.Fit(owner, target, defaultText, new Point(), target.Size, target.Font.Size, this.BackColor);
        }

        /// <summary>
        /// TextBoxの表示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="defaultText"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="fontSize"></param>
        public void Fit(IWin32Window owner, Control target, string defaultText, Point offset, Size size, float fontSize)
        {
            this.Fit(owner, target, defaultText, offset, size, fontSize, this.BackColor);
        }

        /// <summary>
        /// TextBoxの表示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="defaultText"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="fontSize"></param>
        /// <param name="backColor"></param>
        public void Fit(IWin32Window owner, Control target, string defaultText, Point offset, Size size, float fontSize, Color backColor)
        {
            try
            {
                this.InputType = FloatingInputType.TextBox;

                this.textBox1.TextChanged -= this.textBox1_TextChanged;
                {
                    this.textBox1.Text = string.IsNullOrEmpty(defaultText) ? string.Empty : defaultText;
                    this.textBox1.Font = new Font(target.Font.FontFamily, fontSize);
                }
                this.textBox1.TextChanged += this.textBox1_TextChanged;

                this.BackColor = backColor;
                this.Size = size;

                this.textBox1.Width = this.Width - 16;
                this.textBox1.Left = (this.ClientSize.Width - this.textBox1.Width) / 2;
                this.textBox1.Top = (this.ClientSize.Height - this.textBox1.Height) / 2;

                this.Location = target.PointToScreen(offset);

                this.textBox1.SelectAll();

                this.Visible = false;
                this.Show(owner);
                this.BringToFront();
                this.Activate();
                this.textBox1.Focus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// NumericUpDownの表示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public void Fit(IWin32Window owner, Control target, decimal value, decimal minimum, decimal maximum)
        {
            this.Fit(owner, target, value, minimum, maximum, new Point(), target.Size, target.Font.Size, this.BackColor);
        }

        /// <summary>
        /// NumericUpDownの表示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="fontSize"></param>
        public void Fit(IWin32Window owner, Control target, decimal value, decimal minimum, decimal maximum, Point offset, Size size, float fontSize)
        {
            this.Fit(owner, target, value, minimum, maximum, offset, size, fontSize, this.BackColor);
        }

        /// <summary>
        /// NumericUpDownの表示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="fontSize"></param>
        /// <param name="backColor"></param>
        public void Fit(IWin32Window owner, Control target, decimal value, decimal minimum, decimal maximum, Point offset, Size size, float fontSize, Color backColor)
        {
            try
            {
                this.InputType = FloatingInputType.NumericUpDown;

                this.numericUpDown1.ValueChanged -= this.textBox1_TextChanged;
                {
                    this.numericUpDown1.Minimum = System.Math.Min(minimum, maximum);
                    this.numericUpDown1.Maximum = System.Math.Max(minimum, maximum);
                    this.numericUpDown1.Value = System.Math.Min(this.numericUpDown1.Maximum, System.Math.Max(this.numericUpDown1.Minimum, value));
                    this.numericUpDown1.Font = new Font(target.Font.FontFamily, fontSize);
                }
                this.numericUpDown1.ValueChanged += this.textBox1_TextChanged;


                this.BackColor = backColor;
                this.Size = size;

                this.numericUpDown1.Width = this.Width - 16;
                this.numericUpDown1.Left = (this.ClientSize.Width - this.numericUpDown1.Width) / 2;
                this.numericUpDown1.Top = (this.ClientSize.Height - this.numericUpDown1.Height) / 2;

                this.Location = target.PointToScreen(offset);

                this.numericUpDown1.Text = this.numericUpDown1.Value.ToString();
                this.numericUpDown1.Select(0, this.numericUpDown1.Text.Length);

                this.Visible = false;
                this.Show(owner);
                this.BringToFront();
                this.Activate();
                this.numericUpDown1.Focus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Fit(IWin32Window owner, Control target, Control input)
        {
            this.Fit(owner, target, input, new Point(), target.Size, target.Font.Size, this.BackColor);
        }

        public void Fit(IWin32Window owner, Control target, Control input, Point offset, Size size, float fontSize)
        {
            this.Fit(owner, target, input, offset, size, fontSize, this.BackColor);
        }

        public void Fit(IWin32Window owner, Control target, Control input, Point offset, Size size, float fontSize, Color backColor)
        {
            try
            {
                this.InputType = FloatingInputType.Container;

                input.Parent = this;
                input.Visible = true;
                input.Font = new Font(target.Font.FontFamily, fontSize);

                this.BackColor = backColor;
                this.Size = size;

                input.Width = this.Width - 16;
                input.Left = (this.ClientSize.Width - this.numericUpDown1.Width) / 2;
                input.Top = (this.ClientSize.Height - this.numericUpDown1.Height) / 2;

                this.Location = target.PointToScreen(offset);

                this.Visible = false;
                this.Show(owner);
                this.BringToFront();
                this.Activate();
                input.Focus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    this.InputValueChanged?.Invoke(this);
                }
                else if (e.KeyChar == (char)Keys.Escape)
                {
                    e.Handled = true;
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// NumericUpDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    e.SuppressKeyPress = true;
                    this.Hide();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    this.InputValueChanged?.Invoke(this);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void textBox1_TextChanged(object? sender, EventArgs e)
        {
            try
            {
                if (null != sender)
                {
                    this.InputTextChanged?.Invoke(this, ((Control)sender).Text);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}