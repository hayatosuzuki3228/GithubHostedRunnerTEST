using Hutzper.Library.Common;
using Hutzper.Library.Common.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Hutzper.Library.Forms.Setting
{
    /// <summary>
    /// コンボボックス
    /// </summary>
    public partial class ComboBoxUserControl : HutzperUserControl
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
        /// コンボボックス幅
        /// </summary>
        [Category("ComboBox Appearance")]
        public virtual int ComboBoxWidth
        {
            #region 取得
            get
            {
                return this.ComboBox.Width;
            }
            #endregion

            #region 更新
            set
            {
                this.ComboBox.Width = value;

                this.RelocateControls();
            }
            #endregion
        }

        /// <summary>
        /// コンボボックスフォント
        /// </summary>
        [Category("ComboBox Appearance")]
        public virtual Font ComboBoxFont
        {
            #region 取得
            get
            {
                return this.ComboBox.Font;
            }
            #endregion

            #region 更新
            set
            {
                this.ComboBox.Font = value;
            }
            #endregion
        }

        /// <summary>
        /// コンボボックスに含まれている項目のコレクションを表すオブジェクトを取得します。
        /// </summary>
        public virtual ComboBox.ObjectCollection Items
        {
            #region 取得
            get
            {
                return this.ComboBox.Items;
            }
            #endregion
        }

        /// <summary>
        /// 現在選択している項目を示すインデックスを取得または設定します。
        /// </summary>
        public virtual int SelectedIndex
        {
            #region 取得
            get
            {
                return this.ComboBox.SelectedIndex;
            }
            #endregion

            #region 更新
            set
            {
                this.ComboBox.SelectedIndex = value;
            }
            #endregion
        }

        /// <summary>
        /// 現在選択している項目を取得または設定します。
        /// </summary>
        public virtual object? SelectedItem
        {
            #region 取得
            get
            {
                return this.ComboBox.SelectedItem;
            }
            #endregion

            #region 更新
            set
            {
                this.ComboBox.SelectedItem = value;
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
        /// イベント:選択インデックス変更
        /// </summary>
        public event Action<object, int, string>? SelectedIndexChanged;

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
        public ComboBoxUserControl()
        {
            this.InitializeComponent();

            this.Size = new Size(240, 47);

            this.label1.Visible = true;
            this.label1.Enabled = true;
            this.isTextShrinkToFit = true;
            this.textSpecifiedFontSize = this.label1.Font.Size;

            //オーナードローを指定
            this.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;

            //項目の高さを設定
            this.ComboBox.Top = 0;
            this.ComboBox.ItemHeight = this.flowLayoutPanel1.Height - 6;

            //DrawItemイベントハンドラの追加
            this.ComboBox.DrawItem += this.ComboBox_DrawItem;
        }

        #region GUIイベント

        /// <summary>
        /// コンボボックスイベント:選択インデックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (true == this.notifyEventEnabled)
            {
                this.OnSelectedIndexChanged(this.ComboBox);
            }
        }

        /// <summary>
        /// サイズ変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void ComboBoxUserControl_Resize(object sender, EventArgs e)
        {
            this.RelocateControls();
        }

        //DrawItemイベントハンドラ
        //項目を描画する
        private void ComboBox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            //背景を描画する
            //項目が選択されている時は強調表示される
            e.DrawBackground();

            if (sender is not ComboBox cmb)
                return;

            //項目に表示する文字列
            string txt;
            if (e.Index > -1)
            {
                txt = cmb.Items[e.Index]?.ToString() ?? string.Empty;
            }
            else
            {
                txt = cmb.Text;
            }

            //使用するブラシ
            using var b = new SolidBrush(e.ForeColor);

            //文字列を描画する
            var ym = (e.Bounds.Height - e.Graphics.MeasureString(txt, cmb.Font).Height) / 2;

            e.Graphics.DrawString(
                                    txt
                                    , cmb.Font
                                    , b
                                    , e.Bounds.X
                                    , e.Bounds.Y + ym
                                    );

            //フォーカスを示す四角形を描画
            e.DrawFocusRectangle();
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// Enumを割り当てる
        /// </summary>
        /// <param name="value"></param>
        public void AssignEnumType(Type enumType)
        {
            var currentSetting = this.notifyEventEnabled;

            try
            {
                this.notifyEventEnabled = false;

                if (true == enumType.IsEnum)
                {
                    this.ComboBox.Items.Clear();
                    foreach (Enum item in Enum.GetValues(enumType))
                    {
                        var index = this.ComboBox.Items.Add(EnumExtensions.StringValueOf(item));
                    }
                    if (0 < this.ComboBox.Items.Count)
                    {
                        this.ComboBox.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            this.notifyEventEnabled = currentSetting;
        }

        /// <summary>
        /// Enum値を設定する
        /// </summary>
        /// <param name="enumValue"></param>
        public void SelectFrom(Enum enumValue)
        {
            var currentSetting = this.notifyEventEnabled;

            try
            {
                this.notifyEventEnabled = false;

                if (0 < this.ComboBox.Items.Count)
                {
                    var enumText = EnumExtensions.StringValueOf(enumValue);

                    foreach (var item in this.ComboBox.Items)
                    {
                        if (true == enumText.Equals(item))
                        {
                            this.ComboBox.SelectedIndex = this.ComboBox.Items.IndexOf(item);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            this.notifyEventEnabled = currentSetting;
        }

        /// <summary>
        /// Enum値を取得する
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public Enum? EnumValueOf(Type enumType)
        {
            var values = Enum.GetValues(enumType);

            var result = (Enum?)values.GetValue(0);

            try
            {
                if (0 < this.ComboBox.Items.Count && this.ComboBox.SelectedItem != null)
                {
                    var itemText = (string)this.ComboBox.SelectedItem;

                    result = (Enum)EnumExtensions.EnumValueOf(itemText, enumType);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 文字列で指定する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int SelectFrom(string value)
        {
            var currentSetting = this.notifyEventEnabled;

            this.notifyEventEnabled = false;

            foreach (var item in this.Items)
            {
                if (true == value.Equals(item))
                {
                    this.ComboBox.SelectedIndex = this.ComboBox.Items.IndexOf(item);
                    break;
                }
            }

            this.notifyEventEnabled = currentSetting;

            return this.ComboBox.SelectedIndex;
        }

        /// <summary>
        /// 文字列の配列を取得する
        /// </summary>
        /// <returns></returns>
        public string[] GetItemStringArray()
        {
            var result = new string[this.ComboBox.Items.Count];

            try
            {
                foreach (var item in this.ComboBox.Items)
                {
                    result[this.ComboBox.Items.IndexOf(item)] = item?.ToString() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 文字列の配列を設定する
        /// </summary>
        /// <returns></returns>
        public void SetItemStringArray(string[] stringArray)
        {
            var currentSetting = this.notifyEventEnabled;

            try
            {
                this.notifyEventEnabled = false;

                this.ComboBox.Items.Clear();
                foreach (var item in stringArray)
                {
                    this.ComboBox.Items.Add(item);
                }
                if (0 < this.ComboBox.Items.Count)
                {
                    this.ComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            this.notifyEventEnabled = currentSetting;
        }

        #endregion

        #region プロテクテッドメソッド

        /// <summary>
        /// コントロールの再配置
        /// </summary>
        protected void RelocateControls()
        {
            this.ComboBox.Top = 0;
            this.ComboBox.ItemHeight = this.flowLayoutPanel1.Height - 6;

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
        /// イベント通知:コンボボックス選択インデックス変更
        /// </summary>
        protected void OnSelectedIndexChanged(ComboBox comboBox)
        {
            try
            {
                var selectedString = string.Empty;
                if (null != comboBox.SelectedItem)
                {
                    selectedString = comboBox.SelectedItem.ToString() ?? string.Empty;
                }

                this.SelectedIndexChanged?.Invoke(this, comboBox.SelectedIndex, selectedString);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
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

        #endregion

        private void label1_TextChanged(object sender, EventArgs e)
        {
            this.UpdateLabel();

            this.Invalidate();
        }
    }
}