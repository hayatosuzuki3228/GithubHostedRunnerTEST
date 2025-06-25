using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller.Plc;
using Hutzper.Library.Common.Drawing;

namespace Hutzper.Library.Forms.Diagnostics
{
    /// <summary>
    /// IPlcDeviceMapControl実装ユーザーコントロール
    /// </summary>
    public partial class PlcDeviceMapControl : HutzperUserControl, IPlcDeviceMapControl
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

        #region IPlcDeviceMapControl

        /// <summary>
        /// 一度に表示できるデータ行数
        /// </summary>
        /// <remarks>タイトル行は含まない</remarks>
        public int MaxDisplayedDataRows { get; private set; } = 10;

        /// <summary>
        /// ワードデバイスかどうか(ビットデバイスの場合はfalse)
        /// </summary>
        public bool IsWordDevice { get; private set; }

        /// <summary>
        /// 16進数表示かどうか
        /// </summary>
        public bool IsHexadecimal
        {
            get => this._IsHexadecimal;
            set
            {
                this._IsHexadecimal = value;
                this.FloatingInput.Hide();
                this.UpdateView();
            }
        }

        /// <summary>
        /// デバイス書き込み要求イベント
        /// </summary>
        public event Action<object, PlcDeviceUnit, int, int>? DeviceWriteRequested;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="deviceUnit">設定</param>
        /// <param name="referenceDeviceValues">表示のために参照されるデバイス値が格納されるユニット単位の配列</param>
        public void Initialize(PlcDeviceUnit[] deviceUnit, params int[][] referenceDeviceValues)
        {
            this.Visible = false;

            if (deviceUnit.Length != referenceDeviceValues.Length)
            {
                throw new ArgumentException("length of deviceUnit and referenceDeviceValues must be the same.");
            }

            if (2 <= deviceUnit.Length)
            {
                var firstUnit = deviceUnit[0];
                foreach (var unit in deviceUnit.Skip(1))
                {
                    if (firstUnit.IsWord != unit.IsWord)
                    {
                        throw new ArgumentException("deviceUnit must be the same type.");
                    }
                }
            }

            // デバイス種を設定
            this.IsWordDevice = deviceUnit.FirstOrDefault()?.IsWord ?? true;

            // 有効なデバイス情報を先頭アドレス順で保持
            this.DeviceInfo = deviceUnit
                .Where(unit => unit.Enabled) // Enabled が true のものだけを選択
                .OrderBy(unit => unit.StartingAddress) // StartingAddress でソート
                .Select((unit, index) => (unit, index < referenceDeviceValues.Length ? referenceDeviceValues[index] : Array.Empty<int>()))
                .ToList();

            #region 表のサイズ設定
            {
                var colums = new List<int>
                {
                    160,
                    160,
                };
                this.UcDeviceMap.RowNumber = this.MaxDisplayedDataRows + 1;   // タイトル行を含む
                this.UcDeviceMap.RowHeight = 40;
                this.UcDeviceMap.ColumnWidthStrings = TableViewRendererUserControl.ConcatenateByLineBreakString(colums);
                this.UcDeviceMap.BorderColor = SystemColors.ControlDarkDark;
                this.UcDeviceMap.Size = this.UcDeviceMap.ViewSize;
            }
            #endregion

            #region タイトル行の設定
            {
                var titleCells = this.UcDeviceMap.Cells[0];

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
                                c.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                                c.Tag = new CellKind(RowKind.Title, ColumnKind.Address);
                            }
                            break;

                        case 1:
                            {
                                c.Text = "デバイス値";
                                c.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                                c.Tag = new CellKind(RowKind.Title, ColumnKind.Device);
                            }
                            break;
                    }
                }
            }
            #endregion

            #region データ行の設定

            foreach (var r in this.UcDeviceMap.Cells.Skip(1))
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
                                c.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                                c.Tag = new CellKind(RowKind.Data, ColumnKind.Address);
                            }
                            break;

                        case 1:
                            {
                                c.Text = "";
                                c.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                                c.Tag = new CellKind(RowKind.Data, ColumnKind.Device | ColumnKind.ControlNumericUpdown);
                            }
                            break;
                    }
                }
            }
            #endregion

            // デバイスマップ
            this.UcDeviceMap.Font = new Font("MS Gothic", this.Font.Size, FontStyle.Regular);
            this.UcDeviceMap.CellClick += this.TableViewRendererUserControl1_CellClick;

            // 入力イベント
            this.FloatingInput.InputValueChanged += this.FloatingInput_InputValueChanged;

            // スクロールバー
            this.vScrollBar1.Left = this.UcDeviceMap.Left + this.UcDeviceMap.Width;
            this.vScrollBar1.Top = this.UcDeviceMap.Top;
            this.vScrollBar1.Height = this.UcDeviceMap.Height;
            this.vScrollBar1.MouseWheel += this.VScrollBar1_MouseWheel;
            this.vScrollBar1.ValueChanged += this.vScrollBar1_ValueChanged;

            this.Visible = true;
            this.UpdateView(0);
        }

        /// <summary>
        /// デバイスマップ表示の更新要求
        /// </summary>
        /// <remarks>SetDeviceMapReferenceメソッドで渡したデバイス値を表示します</remarks>
        public void RequestUpdateMap() => this.UpdateView();

        /// <summary>
        /// 入力コントロールを非表示にします
        /// </summary>
        public void HideInputControl() => this.FloatingInput.Hide();

        #endregion

        #region フィールド

        private int ViewFirstLineIndex = 0;
        private int SelectedDataIndex = -1;
        private FloatingInputBoxForm FloatingInput = new();
        private List<(PlcDeviceUnit Unit, int[] Values)> DeviceInfo = new();

        private Form? PreviousParentForm;
        private bool _IsHexadecimal;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlcDeviceMapControl()
        {
            this.InitializeComponent();

            this.nickname = "PlcDeviceMap";

            this.Visible = false;

            this.HandleCreated += (sender, e) =>
            {
                // 以前の親フォームが存在する場合、イベントハンドラを解除
                if (this.PreviousParentForm is not null)
                {
                    this.PreviousParentForm.LocationChanged -= this.ParentForm_LocationChanged;
                }

                // 新しい親フォームが設定された場合、イベントハンドラを追加
                if (this.FindForm() is Form newParentForm)
                {
                    newParentForm.LocationChanged += this.ParentForm_LocationChanged;
                    this.PreviousParentForm = newParentForm; // 現在の親フォームを保存
                }
            };
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// PlcDeviceMapControl:Loadイベント
        /// </summary>
        private void PlcDeviceMapControl_Load(object sender, EventArgs e)
        {
            if (this.Parent is not null)
            {
                this.BackColor = this.Parent.BackColor;
            }
        }

        /// <summary>
        /// スクロールバーのマウスホイールイベント
        /// </summary>
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

        /// <summary>
        /// スクロールバーの値変更イベント
        /// </summary>
        private void vScrollBar1_ValueChanged(object? sender, EventArgs e)
        {
            // 表示先頭行インデックスの変更
            this.ViewFirstLineIndex = this.vScrollBar1.Value;

            // 表示更新
            this.FloatingInput.Hide();
            this.UpdateView();
        }

        /// <summary>
        /// FloatingInputの値変更イベント
        /// </summary>
        private void FloatingInput_InputValueChanged(object obj)
        {
            if (this.FloatingInput.Tag is TableViewRendererUserControl.Cell selectedCell)
            {
                // 表示位置と行数
                var tableRowOffset = 1;  // タイトル行を除外

                // 対象データ
                var selectedItemIndex = this.ViewFirstLineIndex + selectedCell.Index.Y - tableRowOffset;
                var isValueChanged = false;
                var selectedItemValue = (int)this.FloatingInput.NumericUpDownValue;

                if (this.FloatingInput.InputType == FloatingInputBoxForm.FloatingInputType.NumericUpDown)
                {
                    isValueChanged = selectedCell.Text != this.FloatingInput.NumericUpDownValue.ToString();

                    if (false == this.IsHexadecimal)
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
                        if (this.GetDeviceInfo(selectedItemIndex) is (PlcDeviceUnit Unit, int[] Values, int ValueIndex) info)
                        {
                            this.OnDeviceWriteRequested(info.Unit, info.ValueIndex, (int)this.FloatingInput.NumericUpDownValue);
                        }
                    }
                }

                // 表示更新
                this.UpdateView();
            }
        }

        /// <summary>
        /// TableViewRendererUserControlのセルクリックイベント
        /// </summary>
        /// <param name="e">マウスイベント</param>
        /// <param name="c">対象のセル</param>
        /// <param name="r">対象のセル矩形</param>
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

                // 書込み可否確認
                var selectedUnit = this.GetDeviceInfo(this.SelectedDataIndex)?.Unit;
                var isWritable = selectedUnit is not null && selectedUnit.Access == PlcDeviceAccess.Write;

                // 数値入力欄
                if (cellKind.Column.Contains(ColumnKind.ControlNumericUpdown) && isWritable)
                {
                    isFloatingInputEnabled = true;
                    this.FloatingInput.Tag = c;

                    var decimalValue = 0M;
                    if (false == this.IsHexadecimal)
                    {
                        decimalValue = Convert.ToDecimal(c.Text);
                    }
                    else
                    {
                        decimalValue = Convert.ToInt32(c.Text.Substring(2), 16);
                    }

                    if (true == this.IsWordDevice)
                    {
                        this.FloatingInput.Fit(this, this.UcDeviceMap, decimalValue, 0, ushort.MaxValue, offset, size, c.FontSize);
                    }
                    else
                    {
                        this.FloatingInput.Fit(this, this.UcDeviceMap, decimalValue, 0, 1, offset, size, c.FontSize);
                    }
                }

                this.UpdateView();
            }

            this.FloatingInput.Visible = isFloatingInputEnabled;
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
                var tableRowCount = this.UcDeviceMap.RowNumber - tableRowOffset;

                // 表示したいデータ数
                var dataRealCount = this.DeviceInfo.Sum(x => x.Unit.RangeLength);

                // 先頭行インデックスの調整
                this.ViewFirstLineIndex = System.Math.Min((dataRealCount > 0) ? dataRealCount - 1 : 0, recordOffset);    // データ数制限
                if (tableRowCount > dataRealCount)
                {
                    this.ViewFirstLineIndex = 0;    // 1画面に収まる
                }
                else
                {
                    this.ViewFirstLineIndex = System.Math.Min(dataRealCount - tableRowCount + 1, this.ViewFirstLineIndex);
                }

                // スクロールバーの設定
                if (tableRowCount - 1 < dataRealCount)
                {
                    this.vScrollBar1.ValueChanged -= this.vScrollBar1_ValueChanged;
                    this.vScrollBar1.LargeChange = tableRowCount;
                    this.vScrollBar1.Minimum = 0;
                    this.vScrollBar1.Maximum = (dataRealCount + 1 - tableRowCount) + this.vScrollBar1.LargeChange - 1;   // +1 )データ追加行
                    this.vScrollBar1.Value = this.ViewFirstLineIndex;
                    this.vScrollBar1.ValueChanged += this.vScrollBar1_ValueChanged;
                }
                else
                {
                    this.vScrollBar1.Minimum = 0;
                    this.vScrollBar1.Maximum = 0;
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
                var tableRowCount = this.UcDeviceMap.RowNumber - tableRowOffset; // タイトル行を除外

                // 表示するデータ数
                var dataRealCount = this.DeviceInfo.Sum(x => x.Unit.RangeLength);
                var dataViewCount = System.Math.Max(0, dataRealCount - this.ViewFirstLineIndex);
                dataViewCount = System.Math.Min(tableRowCount, dataViewCount);

                // 有効行へのデータ表示
                var dataViewIndex = this.ViewFirstLineIndex;
                foreach (var r in this.UcDeviceMap.Cells.Skip(tableRowOffset).Take(dataViewCount))
                {
                    if (this.GetDeviceInfo(dataViewIndex) is (PlcDeviceUnit Unit, int[] Values, int ValueIndex) info)
                    {
                        var unit = info.Unit;
                        var value = info.Values[info.ValueIndex];
                        var address = unit.StartingAddress + info.ValueIndex;

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

                                if (false == this.IsHexadecimal)
                                {
                                    if (cellKind.Column.Contains(ColumnKind.Address))
                                    {
                                        c.Text = $"{unit.Type.StringValueOf()}{address.ToString("D5")}";
                                    }
                                    else if (cellKind.Column.Contains(ColumnKind.Device))
                                    {
                                        c.Text = value.ToString();
                                    }
                                }
                                else
                                {
                                    if (cellKind.Column.Contains(ColumnKind.Address))
                                    {
                                        c.Text = $"{unit.Type.StringValueOf()}0x{address:X4}";
                                    }
                                    else if (cellKind.Column.Contains(ColumnKind.Device))
                                    {
                                        c.Text = $"0x{value:X4}";
                                    }
                                }
                            }
                            else
                            {
                                var stackFrame = new System.Diagnostics.StackFrame(0, false);
                                Serilog.Log.Error($"{this}, [{stackFrame.GetType().Name}:{stackFrame.GetMethod()?.Name ?? "unknown"}");
                            }
                            #endregion
                        }
                    }

                    dataViewIndex++;
                }

                // 無効行の表示
                foreach (var r in this.UcDeviceMap.Cells.Skip(tableRowOffset + dataViewCount))
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
                            Serilog.Log.Error($"{this}, [{stackFrame.GetType().Name}:{stackFrame.GetMethod()?.Name ?? "unknown"}");
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
                this.UcDeviceMap.UpdateView();
            }
        }

        /// <summary>
        /// 指定されたインデックスのデバイスを含むPlcDeviceUnitへの参照を取得する
        /// </summary>
        /// <param name="globalRowIndex">全体中のインデックス</param>
        /// <returns>対象のユニット</returns>
        private (PlcDeviceUnit Unit, int[] Values, int ValueIndex)? GetDeviceInfo(int globalRowIndex)
        {
            var cumulativeIndex = 0;

            foreach (var info in this.DeviceInfo)
            {
                var unit = info.Unit;
                var values = info.Values;

                var nextCumulativeIndex = cumulativeIndex + unit.RangeLength;

                if (globalRowIndex >= cumulativeIndex && globalRowIndex < nextCumulativeIndex)
                {
                    return (unit, values, globalRowIndex - cumulativeIndex);
                }

                cumulativeIndex = nextCumulativeIndex;
            }

            return null;
        }

        /// <summary>
        /// デバイス書き込み要求イベント通知
        /// </summary>
        /// <param name="deviceUnit">対象のユニット</param>
        /// <param name="relativeIndexInUnit">ユニット内の相対アドレス</param>
        /// <param name="value">書込み値</param>
        private void OnDeviceWriteRequested(PlcDeviceUnit deviceUnit, int relativeIndexInUnit, int deviceValue)
        {
            try
            {
                this.DeviceWriteRequested?.Invoke(this, deviceUnit, relativeIndexInUnit, deviceValue);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }


        private void ParentForm_LocationChanged(object? sender, EventArgs e) => this.FloatingInput.Hide();
    }


    #endregion

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
        public static bool Contains(this PlcDeviceMapControl.RowKind value, PlcDeviceMapControl.RowKind flag) => (value & flag) == flag;

        /// <summary>
        /// 指定された値を含むかどうか
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool Contains(this PlcDeviceMapControl.ColumnKind value, PlcDeviceMapControl.ColumnKind flag) => (value & flag) == flag;
    }
}
