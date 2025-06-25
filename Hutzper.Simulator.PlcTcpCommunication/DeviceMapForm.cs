using Hutzper.Library.Common;
using Hutzper.Library.Common.Drawing;
using Hutzper.Library.Forms;

namespace Hutzper.Simulator.PlcTcpCommunication
{
    public partial class DeviceMapForm : ServiceCollectionSharingForm
    {
        #region サブクラス

        /// <summary>
        /// 行の種類
        /// </summary>
        [Serializable]
        [Flags]
        public enum RowKind : long
        {
            Disable = 0,
            Title = 1L << 0,
            Data = 1L << 1,
            Invalid = 1L << 2,
            Selected = 1L << 3,
        }

        /// <summary>
        /// 列の種類
        /// </summary>
        [Serializable]
        [Flags]
        public enum ColumnKind : long
        {
            Disable = 0,
            Address = 1L << 0,
            Device = 1L << 1,

            Control = 1L << 10,
            ControlNumericUpdown = ColumnKind.Control | (1L << 11),
        }

        /// <summary>
        /// セル種類
        /// </summary>
        public class CellKind
        {
            public RowKind Row { get; set; }

            public ColumnKind Column { get; set; }

            public CellKind(RowKind row, ColumnKind col)
            {
                this.Row = row;
                this.Column = col;
            }
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 一度に表示できるデータ行数
        /// </summary>
        /// <remarks>タイトル行は含まない</remarks>
        public readonly int MaxDisplayedDataRows = 20;

        /// <summary>
        /// ワードデバイスマップかどうか
        /// </summary>
        public bool IsWordDevice { get; private set; }

        /// <summary>
        /// ビットデバイスマップかどうか
        /// </summary>
        public bool IsBitDevice => !this.IsWordDevice;

        /// <summary>
        /// 表示更新間隔(ms)
        /// </summary>
        public int UpdateViewIntervalMs
        {
            get => this.timer1.Interval;
            set => this.timer1.Interval = value;
        }

        #endregion

        #region フィールド

        private FloatingInputBoxForm FloatingInput = new();

        private int ViewFirstLineIndex = 0;
        private int SelectedDataIndex = -1;

        private int[] DeviceValues = Array.Empty<int>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DeviceMapForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DeviceMapForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            this.InitializeComponent();

            this.IsWordDevice = true;
            this.DeviceValues = Enumerable.Range(0, short.MaxValue).ToArray();

            #region 表のサイズ設定
            {
                var colums = new List<int>
                {
                    160,
                    160,
                };
                this.tableViewRendererUserControl1.RowNumber = this.MaxDisplayedDataRows + 1;   // タイトル行を含む
                this.tableViewRendererUserControl1.RowHeight = 40;
                this.tableViewRendererUserControl1.ColumnWidthStrings = TableViewRendererUserControl.ConcatenateByLineBreakString(colums);
                this.tableViewRendererUserControl1.BorderColor = SystemColors.ControlDarkDark;
                this.tableViewRendererUserControl1.Size = this.tableViewRendererUserControl1.ViewSize;
            }
            #endregion

            #region タイトル行の設定
            {
                var titleCells = this.tableViewRendererUserControl1.Cells[0];

                foreach (var c in titleCells)
                {
                    c.IsMouseOverEmphasize = false;
                    c.BackColor = Color.Silver;
                    c.ForeColor = Color.Black;
                    c.FontSize = this.Font.Size;
                    c.IsTextShrinkToFit = true;

                    switch (c.Index.X)
                    {
                        case 0:
                            {
                                c.Text = "アドレス.";
                                c.TextAlign = ContentAlignment.MiddleCenter;
                                c.Tag = new CellKind(RowKind.Title, ColumnKind.Address);
                            }
                            break;

                        case 1:
                            {
                                c.Text = "デバイス値";
                                c.TextAlign = ContentAlignment.MiddleCenter;
                                c.Tag = new CellKind(RowKind.Title, ColumnKind.Device);
                            }
                            break;
                    }
                }
            }
            #endregion

            #region データ行の設定

            foreach (var r in this.tableViewRendererUserControl1.Cells.Skip(1))
            {
                foreach (var c in r)
                {
                    c.IsMouseOverEmphasize = true;
                    c.FontSize = this.Font.Size;

                    switch (c.Index.X)
                    {
                        case 0:
                            {
                                c.Text = "";
                                c.TextAlign = ContentAlignment.MiddleRight;
                                c.Tag = new CellKind(RowKind.Data, ColumnKind.Address);
                            }
                            break;

                        case 1:
                            {
                                c.Text = "";
                                c.TextAlign = ContentAlignment.MiddleRight;
                                c.Tag = new CellKind(RowKind.Data, ColumnKind.Device | ColumnKind.ControlNumericUpdown);
                            }
                            break;
                    }
                }
            }
            #endregion

            // デバイスマップ
            this.tableViewRendererUserControl1.Font = new Font("MS Gothic", this.Font.Size, FontStyle.Regular);
            this.tableViewRendererUserControl1.CellClick += this.TableViewRendererUserControl1_CellClick;

            // 入力イベント
            this.FloatingInput.InputValueChanged += this.FloatingInput_InputValueChanged;
            this.FloatingInput.TextBoxWatermarkText = this.Translate(this.FloatingInput.TextBoxWatermarkText);

            // スクロールバー
            this.vScrollBar1.Left = this.tableViewRendererUserControl1.Left + this.tableViewRendererUserControl1.Width;
            this.vScrollBar1.Top = this.tableViewRendererUserControl1.Top;
            this.vScrollBar1.Height = this.tableViewRendererUserControl1.Height;
            this.vScrollBar1.MouseWheel += this.VScrollBar1_MouseWheel;
            this.vScrollBar1.ValueChanged += this.vScrollBar1_ValueChanged;

            // フォームサイズ
            this.ClientSize = new System.Drawing.Size(this.vScrollBar1.Left + this.vScrollBar1.Width + 16, this.vScrollBar1.Top + this.vScrollBar1.Height + 16);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            // 表示形式
            this.UcBaseNumber.SetItemStringArray(new[] { "10進数", "16進数" });
            this.UcBaseNumber.SelectedIndex = 0;
        }

        #endregion

        #region publicメソッド

        public void SetDeviceValues(bool isWordDevice, int[] deviceValues)
        {
            this.IsWordDevice = isWordDevice;
            this.Text = this.IsWordDevice ? "ワードデバイスマップ" : "ビットデバイスマップ";
            this.DeviceValues = deviceValues;
            this.FloatingInput.Hide();
            this.UpdateView(0);
            this.timer1.Start();
        }

        #endregion

        #region privateメソッド

        /// <summary>
        /// 表示更新
        /// </summary>
        /// <param name="recordOffset">表示したいデータインデックスを指定します</param>
        private void UpdateView(int recordOffset)
        {
            try
            {
                // 表示位置と行数
                var tableRowOffset = 1;  // タイトル行を除外
                var tableRowCount = this.tableViewRendererUserControl1.RowNumber - tableRowOffset;

                // 表示したいデータ数
                var dataViewCount = this.DeviceValues.Length;

                // 先頭行インデックスの調整
                this.ViewFirstLineIndex = System.Math.Min((dataViewCount > 0) ? dataViewCount - 1 : 0, recordOffset);    // データ数制限
                if (tableRowCount > dataViewCount)
                {
                    this.ViewFirstLineIndex = 0;    // 1画面に収まる
                }
                else
                {
                    this.ViewFirstLineIndex = System.Math.Min(dataViewCount - tableRowCount + 1, this.ViewFirstLineIndex);
                }

                // スクロールバーの設定
                this.vScrollBar1.Visible = tableRowCount - 1 < dataViewCount;
                if (true == this.vScrollBar1.Visible)
                {
                    this.vScrollBar1.ValueChanged -= this.vScrollBar1_ValueChanged;
                    this.vScrollBar1.LargeChange = tableRowCount;
                    this.vScrollBar1.Minimum = 0;
                    this.vScrollBar1.Maximum = (this.DeviceValues.Length + 1 - tableRowCount) + this.vScrollBar1.LargeChange - 1;   // +1 )データ追加行
                    this.vScrollBar1.Value = this.ViewFirstLineIndex;
                    this.vScrollBar1.ValueChanged += this.vScrollBar1_ValueChanged;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                // 表示更新
                this.UpdateView();
            }
        }

        /// <summary>
        /// 表示更新
        /// </summary>
        /// <remarks>現在の先頭行表示インデックスからデータを表示します</remarks>
        private void UpdateView()
        {
            try
            {
                // 表示位置と行数
                var tableRowOffset = 1;  // タイトル行を除外
                var tableRowCount = this.tableViewRendererUserControl1.RowNumber - tableRowOffset; // タイトル行を除外

                // 表示するデータ数
                var dataViewCount = System.Math.Max(0, this.DeviceValues.Length - this.ViewFirstLineIndex);
                dataViewCount = System.Math.Min(tableRowCount, dataViewCount);

                // 有効行へのデータ表示
                var dataViewIndex = this.ViewFirstLineIndex;
                foreach (var r in this.tableViewRendererUserControl1.Cells.Skip(tableRowOffset).Take(dataViewCount))
                {
                    var selectedItem = this.DeviceValues[dataViewIndex];

                    foreach (var c in r)
                    {
                        #region データの表示
                        if (c.Tag is CellKind cellKind)
                        {
                            cellKind.Row &= ~RowKind.Invalid;
                            cellKind.Row |= RowKind.Data;

                            c.BackColor = this.SelectedDataIndex.Equals(dataViewIndex) ? SystemColors.Highlight : SystemColors.Window;
                            c.ForeColor = this.SelectedDataIndex.Equals(dataViewIndex) ? SystemColors.HighlightText : SystemColors.ControlText;
                            c.IsMouseOverEmphasize = true;
                            c.IsStretched = false;

                            if (0 == this.UcBaseNumber.SelectedIndex)
                            {
                                if (cellKind.Column.Contains(ColumnKind.Address))
                                {
                                    c.Text = dataViewIndex.ToString("D5");
                                }
                                else if (cellKind.Column.Contains(ColumnKind.Device))
                                {
                                    c.Text = selectedItem.ToString();
                                }
                            }
                            else
                            {
                                if (cellKind.Column.Contains(ColumnKind.Address))
                                {
                                    c.Text = $"0x{dataViewIndex:X4}";
                                }
                                else if (cellKind.Column.Contains(ColumnKind.Device))
                                {
                                    c.Text = $"0x{selectedItem:X4}";
                                }
                            }
                        }
                        else
                        {
                            var stackFrame = new System.Diagnostics.StackFrame(0, false);
                            Serilog.Log.Error($"[{stackFrame.GetType().Name}:{stackFrame.GetMethod()?.Name ?? "unknown"}]");
                        }
                        #endregion
                    }

                    dataViewIndex++;
                }

                // 無効行の表示
                foreach (var r in this.tableViewRendererUserControl1.Cells.Skip(tableRowOffset + dataViewCount))
                {
                    foreach (var c in r)
                    {
                        if (c.Tag is CellKind cellKind)
                        {
                            cellKind.Row = RowKind.Disable;
                            cellKind.Row = RowKind.Invalid;

                            c.BackColor = Color.Gray;
                            c.IsStretched = false;
                            c.IsMouseOverEmphasize = false;
                            c.Text = string.Empty;
                        }
                        else
                        {
                            var stackFrame = new System.Diagnostics.StackFrame(0, false);
                            Serilog.Log.Error($"[{stackFrame.GetType().Name}:{stackFrame.GetMethod()?.Name ?? "unknown"}]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                // 表示更新
                this.tableViewRendererUserControl1.UpdateView();
            }
        }

        #endregion

        #region GUIイベント

        private void DeviceMapForm_Shown(object sender, EventArgs e)
        {
            this.UpdateView(0);
        }

        private void comboBoxUserControl1_SelectedIndexChanged(object arg1, int arg2, string arg3)
        {
            this.FloatingInput.Hide();
            this.UpdateView();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.UpdateView();
        }

        private void VScrollBar1_MouseWheel(object? sender, MouseEventArgs e)
        {
            // スクロールバーが表示されている場合は,スクロール値を変更する
            if (false == this.FloatingInput.Visible && this.vScrollBar1.Visible)
            {
                if (e.Delta > 0 && this.vScrollBar1.Minimum < this.vScrollBar1.Value)
                {
                    this.vScrollBar1.Value = Math.Max(this.vScrollBar1.Minimum, this.vScrollBar1.Value - this.vScrollBar1.SmallChange);
                }
                else if (e.Delta < 0 && this.vScrollBar1.Maximum > this.vScrollBar1.Value)
                {
                    this.vScrollBar1.Value = Math.Min(this.vScrollBar1.Maximum, this.vScrollBar1.Value + this.vScrollBar1.SmallChange);
                }
            }
        }

        private void vScrollBar1_ValueChanged(object? sender, EventArgs e)
        {
            // 表示先頭行インデックスの変更
            this.ViewFirstLineIndex = this.vScrollBar1.Value;

            // 表示更新
            this.FloatingInput.Hide();
            this.UpdateView();
        }

        private void FloatingInput_InputValueChanged(object obj)
        {
            if (this.FloatingInput.Tag is TableViewRendererUserControl.Cell selectedCell)
            {
                // 表示位置と行数
                var tableRowOffset = 1;  // タイトル行を除外

                // 対象データ
                var selectedItemIndex = this.ViewFirstLineIndex + selectedCell.Index.Y - tableRowOffset;
                var isValueChanged = false;

                if (this.FloatingInput.InputType == FloatingInputBoxForm.FloatingInputType.NumericUpDown)
                {
                    isValueChanged = selectedCell.Text != this.FloatingInput.NumericUpDownValue.ToString();

                    if (0 == this.UcBaseNumber.SelectedIndex)
                    {
                        selectedCell.Text = this.FloatingInput.NumericUpDownValue.ToString();
                    }
                    else
                    {
                        selectedCell.Text = $"0x{(int)this.FloatingInput.NumericUpDownValue:X4}";
                    }

                    this.FloatingInput.Hide();
                }

                // データへの反映
                if (isValueChanged && (selectedCell.Tag is CellKind cellKind))
                {
                    // ソート番号
                    if (cellKind.Column.Contains(ColumnKind.Device))
                    {
                        if (0 == this.UcBaseNumber.SelectedIndex)
                        {
                            this.DeviceValues[selectedItemIndex] = Convert.ToInt32(selectedCell.Text);
                        }
                        else
                        {
                            this.DeviceValues[selectedItemIndex] = Convert.ToInt32(selectedCell.Text.Substring(2), 16);
                        }
                    }
                }

                // 表示更新
                this.UpdateView();
            }
        }

        private void TableViewRendererUserControl1_CellClick(object sender, MouseEventArgs e, TableViewRendererUserControl.Cell c, RectangleD r)
        {
            var offset = r.Location.ToPoint();
            var size = r.Size.ToSize();
            size.Width += 1;
            size.Height += 1;

            var isFloatingInputEnabled = false;
            this.FloatingInput.Tag = null;

            // 無効行
            if (c.Tag is not CellKind cellKind || cellKind.Row.Contains(RowKind.Invalid))
            {
                // クリックを受け付けない
            }
            // 有効データ行
            else if (cellKind.Row.Contains(RowKind.Data))
            {
                // 表示位置と行数
                var tableRowOffset = 1;  // タイトル行を除外

                // 対象データ
                this.SelectedDataIndex = this.ViewFirstLineIndex + c.Index.Y - tableRowOffset;

                // 数値入力欄
                if (cellKind.Column.Contains(ColumnKind.ControlNumericUpdown))
                {
                    isFloatingInputEnabled = true;
                    this.FloatingInput.Tag = c;

                    var decimalValue = 0M;
                    if (0 == this.UcBaseNumber.SelectedIndex)
                    {
                        decimalValue = Convert.ToDecimal(c.Text);
                    }
                    else
                    {
                        decimalValue = Convert.ToInt32(c.Text.Substring(2), 16);
                    }

                    if (true == this.IsWordDevice)
                    {
                        this.FloatingInput.Fit(this, this.tableViewRendererUserControl1, decimalValue, 0, ushort.MaxValue, offset, size, c.FontSize);
                    }
                    else
                    {
                        this.FloatingInput.Fit(this, this.tableViewRendererUserControl1, decimalValue, 0, 1, offset, size, c.FontSize);
                    }
                }

                this.UpdateView();
            }

            this.FloatingInput.Visible = isFloatingInputEnabled;
        }

        #endregion

        private void DeviceMapForm_MouseDown(object sender, MouseEventArgs e) => this.FloatingInput.Hide();

        private void DeviceMapForm_LocationChanged(object sender, EventArgs e) => this.FloatingInput.Hide();
    }

    /// <summary>
    /// Enum拡張
    /// </summary>
    [Serializable]
    public static class EnumExtensions
    {
        /// <summary>
        /// 指定された値を含むかどうか
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool Contains(this DeviceMapForm.RowKind value, DeviceMapForm.RowKind flag) => (value & flag) == flag;

        /// <summary>
        /// 指定された値を含むかどうか
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool Contains(this DeviceMapForm.ColumnKind value, DeviceMapForm.ColumnKind flag) => (value & flag) == flag;
    }
}