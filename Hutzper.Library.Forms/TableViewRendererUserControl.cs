using Hutzper.Library.Common.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using Point = Hutzper.Library.Common.Drawing.Point;

namespace Hutzper.Library.Forms
{
    /// <summary>
    /// 行列形式の表
    /// </summary>
    public partial class TableViewRendererUserControl : HutzperUserControl
    {
        #region サブクラス

        /// <summary>
        /// セル情報
        /// </summary>
        /// <remarks>セルの表示情報をプロパティに持ちます</remarks>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Serializable]
        public class Cell
        {
            #region プロパティ

            /// <summary>
            /// セルの位置を示すインデックス
            /// </summary>
            public readonly Point Index;

            /// <summary>
            /// セルの背景色を設定します
            /// </summary>
            public Color BackColor { get; set; }

            /// <summary>
            /// セルの文字色を設定します
            /// </summary>
            public Color ForeColor { get; set; }

            /// <summary>
            /// セルの文字配置を設定します
            /// </summary>
            public ContentAlignment TextAlign { get; set; }

            /// <summary>
            /// セルのフォントサイズを設定します
            /// </summary>
            public float FontSize { get; set; }

            /// <summary>
            /// セルに表示する文字列を設定します
            /// </summary>
            /// <remarks>改行で句切られた文字を設定できます</remarks>
            public string Text { get; set; }

            /// <summary>
            /// 汎用タグ
            /// </summary>
            public object? Tag { get; set; }

            /// <summary>
            /// 文字列を90度回転するかどうか
            /// </summary>
            public bool IsTextRotate90 { get; set; }

            /// <summary>
            /// 文字列を縮小して全体を表示する
            /// </summary>
            public bool IsTextShrinkToFit { get; set; }

            /// <summary>
            /// 太字にするかどうか
            /// </summary>
            public bool IsTextBold { get; set; }

            /// <summary>
            /// セルを引き伸ばすか
            /// </summary>
            /// <remarks>trueにすると右側行端まで引き伸ばします</remarks>
            public bool IsStretched { get; set; }

            /// <summary>
            /// マウスカーソルがセル上にある時に強調するかどうか
            /// </summary>
            public bool IsMouseOverEmphasize { get; set; }

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="rowIndex"></param>
            /// <param name="colIndex"></param>
            /// <param name="fontSize"></param>
            public Cell(int rowIndex, int colIndex, float fontSize)
            {
                this.Index = new Point(colIndex, rowIndex);

                this.BackColor = SystemColors.Window;
                this.ForeColor = SystemColors.ControlText;
                this.TextAlign = ContentAlignment.MiddleCenter;
                this.FontSize = fontSize;
                this.Text = string.Empty;

                this.IsTextRotate90 = false;
                this.IsTextShrinkToFit = false;
                this.IsTextBold = false;
                this.IsStretched = false;
                this.IsMouseOverEmphasize = false;
            }

            #endregion

            #region パブリックメソッド

            /// <summary>
            /// コピー
            /// </summary>
            /// <param name="source">コピー元</param>
            /// <remarks>Location以外のプロパティをコピーします</remarks>
            public void CopyFrom(Cell source)
            {
                if (null != source)
                {
                    this.BackColor = source.BackColor;
                    this.ForeColor = source.ForeColor;
                    this.TextAlign = source.TextAlign;
                    this.FontSize = source.FontSize;
                    this.Text = source.Text;
                    this.Tag = source.Tag;

                    this.IsTextRotate90 = source.IsTextRotate90;
                    this.IsTextShrinkToFit = source.IsTextShrinkToFit;
                    this.IsTextBold = false;
                    this.IsStretched = source.IsStretched;
                }
            }

            #endregion
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 行数を設定します
        /// </summary>        
        [Category("一覧表のデザイン")]
        [DefaultValue(5)]
        [Description("一覧表の行数を設定します。")]
        public int RowNumber
        {
            get => this.rowNumber;

            set
            {
                if (0 < value)
                {
                    this.rowNumber = value;
                    this.RebuildLayout();
                }
            }
        }

        /// <summary>
        /// 行の高さを設定します
        /// </summary>
        [Category("一覧表のデザイン")]
        [DefaultValue(32)]
        [Description("一覧表の一行の高さを設定します。")]
        public int RowHeight
        {
            get => this.rowHeight;

            set
            {
                if (8 < value)
                {
                    this.rowHeight = value;
                    this.RebuildLayout();
                }
            }
        }

        /// <summary>
        /// 列数を取得します
        /// </summary>
        [Category("一覧表のデザイン")]
        [Description("一覧表の列数を取得します。")]
        public int ColumnNumber { get; protected set; }

        /// <summary>
        /// 列幅を取得します
        /// </summary>
        [Category("一覧表のデザイン")]
        [Description("一覧表の各列幅を配列として取得します。")]
        public int[] ColumnWidth
        {
            get
            {
                var width = new int[this.columnWidth.Length];

                Array.Copy(this.columnWidth, width, width.Length);

                return width;
            }
        }

        /// <summary>
        /// 列幅を改行で句切られた文字列で指定します
        /// </summary>
        /// <remarks>改行で句切られた文字の数が列数になります</remarks>
        [Category("一覧表のデザイン")]
        [Description("一覧表の列幅を改行で句切られた文字列で設定します。\n改行で句切られた文字の数が列数になります。")]
        [EditorAttribute(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [DefaultValue("80\n80\n80\n80\n80")]
        public string ColumnWidthStrings
        {
            get
            {
                var convertedValues = new string[this.columnWidth.Length];

                for (int i = 0; i < this.columnWidth.Length; i++)
                {
                    convertedValues[i] = this.columnWidth[i].ToString();
                }

                return string.Join("\n", convertedValues);
            }

            set
            {
                if (true == string.IsNullOrEmpty(value))
                {
                    return;
                }

                var listVerifiedWidth = new List<int>();
                foreach (var c in value.Split(new char[] { '\n' }))
                {
                    if (true == int.TryParse(c, out int convertedValue))
                    {
                        listVerifiedWidth.Add(convertedValue);
                    }
                }

                if (0 < listVerifiedWidth.Count)
                {
                    this.columnWidth = listVerifiedWidth.ToArray();
                    this.columnWidthCumulative = new int[this.columnWidth.Length];

                    var cumulativeSum = 0;
                    for (int i = 0; i < this.columnWidthCumulative.Length; i++)
                    {
                        this.columnWidthCumulative[i] = this.columnWidth[i] + cumulativeSum;

                        cumulativeSum += this.columnWidth[i];
                    }

                    this.ColumnNumber = this.columnWidth.Length;

                    this.RebuildLayout();
                }
            }
        }

        /// <summary>
        /// 境界線色
        /// </summary>
        [Category("一覧表のデザイン")]
        [Description("一覧表の境界線色を設定します")]
        [DefaultValue(typeof(Color), "SystemColors.ControlDarkDark")]
        public Color BorderLineColor
        {
            get => this.cellBorderLineColor;

            set
            {
                this.cellBorderLineColor = value;

                this.RebuildLayout();
            }
        }

        /// <summary>
        /// マウスカーソルがセル上にある時の強調境界線色
        /// </summary>
        [Category("一覧表のデザイン")]
        [Description("一覧表の境界線色を設定します")]
        public Color MouseOverEmphasizeBorderColor
        {
            get => this.cellMouseOverEmphasizeBorderColor;
            set => this.cellMouseOverEmphasizeBorderColor = value;
        }

        /// <summary>
        /// 一覧表のサイズ
        /// </summary>
        [Category("一覧表のデザイン")]
        [Description("一覧表のサイズを取得します")]
        public System.Drawing.Size ViewSize
        {
            get
            {
                var height = this.rowHeight * this.rowNumber;
                var width = this.columnWidthCumulative.Last();

                var offset = new Point((int)Math.Ceiling(this.borderSize + 0.5), (int)Math.Ceiling(this.borderSize + 0.5));

                return new System.Drawing.Size(offset.X + width + 1, offset.Y + height + 1);
            }
        }

        /// <summary>
        /// セル情報
        /// </summary>
        [Browsable(false)]
        public List<List<Cell>> Cells
        {
            get
            {
                var currentCells = new List<List<Cell>>();
                foreach (var r in this.cells)
                {
                    currentCells.Add(new List<Cell>(r));
                }

                return currentCells;
            }
        }

        #endregion

        #region イベント

        /// <summary>
        /// イベント:セル描画
        /// </summary>
        [Category("一覧表のイベント")]
        [Description("一覧表のセルが描画されるときに発生します。")]
        [Browsable(true)]
        public event Action<object, PaintEventArgs, Cell, RectangleD>? CellDrawing;

        /// <summary>
        /// イベント:セルクリック
        /// </summary>
        [Category("一覧表のイベント")]
        [Description("一覧表のセルがクリックされたときに発生します。")]
        [Browsable(true)]
        public event Action<object, MouseEventArgs, Cell, RectangleD>? CellClick;

        /// <summary>
        /// イベント:セルダブルクリック
        /// </summary>
        [Category("一覧表のイベント")]
        [Description("一覧表のセルがダブルクリックされたときに発生します。")]
        [Browsable(true)]
        public event Action<object, MouseEventArgs, Cell, RectangleD>? CellDoubleClick;

        #endregion

        #region フィールド

        protected int rowNumber;
        protected int rowHeight;
        protected int[] columnWidth;
        protected int[] columnWidthCumulative;
        protected Color cellBorderLineColor;
        private Color cellMouseOverEmphasizeBorderColor = Color.FromArgb(0, 120, 212);
        protected List<List<Cell>> cells = new();
        protected Cell? mouseOverCell;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TableViewRendererUserControl()
        {
            this.InitializeComponent();

            // フォーム
            base.AutoScaleMode = AutoScaleMode.None;
            base.borderSize = 0f;

            // コーナー丸め
            base.roundedCorner = RoundedCorner.All;
            base.borderRadius = 11;

            // 表サイズ
            this.rowNumber = 5;
            this.rowHeight = 32;
            this.columnWidth = Enumerable.Repeat(80, 5).ToArray();
            this.cellBorderLineColor = SystemColors.ControlDarkDark;

            this.columnWidthCumulative = new int[this.columnWidth.Length];

            var cumulativeSum = 0;
            for (int i = 0; i < this.columnWidthCumulative.Length; i++)
            {
                this.columnWidthCumulative[i] = this.columnWidth[i] + cumulativeSum;

                cumulativeSum += this.columnWidth[i];
            }

            this.ColumnNumber = this.columnWidth.Length;

            this.RebuildLayout();
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 表示更新
        /// </summary>
        public void UpdateView()
        {
            this.Invalidate();
        }

        #endregion

        #region スタティックメソッド

        /// <summary>
        /// 整数値リストをCSV文字列に変換する
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ConcatenateByLineBreakString(List<int> values)
        {
            return TableViewRendererUserControl.ConcatenateByLineBreakString(values.ToArray());
        }

        /// <summary>
        /// 整数値を改行で連結した文字列に変換する
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ConcatenateByLineBreakString(params int[] values)
        {
            var strings = Array.ConvertAll(values, v => v.ToString());

            return string.Join("\n", strings.ToArray());
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// イベント:描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordTableViewUserControl_Paint(object sender, PaintEventArgs e)
        {
            try
            {

                // 描画設定
                using var backBrush = new SolidBrush(this.BackColor);
                using var lineNormalPen = new Pen(this.cellBorderLineColor);
                using var lineEmphasizePen = new Pen(this.cellMouseOverEmphasizeBorderColor);
                using var backPath = GraphicsUtilities.CreateRound(this.ClientRectangle, this.RoundedCorner, (float)this.BorderRadius);

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillPath(backBrush, backPath);

                lineNormalPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                lineEmphasizePen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                // 描画原点
                var offset = new Point((int)Math.Ceiling(this.borderSize + 0.5), (int)Math.Ceiling(this.borderSize + 0.5));

                // 行
                foreach (var r in Enumerable.Range(0, this.rowNumber))
                {
                    // セル座標
                    var cellX = offset.X;
                    var cellY = offset.Y + this.rowHeight * r;
                    foreach (var c in Enumerable.Range(0, this.ColumnNumber))
                    {
                        var selectedCell = this.cells[r][c];

                        using (var cellBackBrush = new SolidBrush(selectedCell.BackColor))
                        using (var cellTextBrush = new SolidBrush(selectedCell.ForeColor))
                        {
                            var cellWidth = this.columnWidth[c];
                            if (true == selectedCell.IsStretched)
                            {
                                cellWidth += this.columnWidth.ToList().Skip(c + 1).Sum();
                            }

                            var cellRect = new RectangleF(cellX, cellY, cellWidth, this.rowHeight);

                            var isEmphasize = selectedCell.IsMouseOverEmphasize & cellRect.Contains(this.PointToClient(Cursor.Position));

                            var cellCorner = RoundedCorner.Disable;
                            #region コーナーセル丸め
                            if (r == 0 && c == 0)
                            {
                                cellCorner = RoundedCorner.LeftTop;
                            }
                            else if (r == 0 && c == this.ColumnNumber - 1)
                            {
                                cellCorner = RoundedCorner.RightTop;
                            }
                            else if (r == this.rowNumber - 1 && c == 0)
                            {
                                cellCorner = RoundedCorner.LeftBottom;
                            }
                            else if (r == this.rowNumber - 1 && c == this.ColumnNumber - 1)
                            {
                                cellCorner = RoundedCorner.RightBottom;
                            }
                            #endregion

                            using var cellPath = GraphicsUtilities.CreateRound(cellRect, cellCorner, (float)this.BorderRadius);
                            e.Graphics.FillPath(cellBackBrush, cellPath);
                            e.Graphics.DrawPath(lineNormalPen, cellPath);

                            // 強調表示
                            if (isEmphasize)
                            {
                                var cellRectInner = cellRect;
                                cellRectInner.Inflate(-0.5f, -0.5f);
                                using var cellInnerPath = GraphicsUtilities.CreateRound(cellRectInner, cellCorner, (float)this.BorderRadius);
                                e.Graphics.DrawPath(lineEmphasizePen, cellInnerPath);
                            }

                            var cellTexts = selectedCell.Text.Split(new char[] { '\n' });
                            if (false == selectedCell.IsTextRotate90)
                            {
                                var splitedHeight = cellRect.Height / cellTexts.Length;
                                var splitedOffset = 0f;
                                for (int t = 0; t < cellTexts.Length; t++)
                                {
                                    var selctedCellText = cellTexts[t];
                                    var selectedFontSize = selectedCell.FontSize;

                                    #region 文字列を縮小して全体を表示する 場合
                                    if (true == selectedCell.IsTextShrinkToFit)
                                    {
                                        var availableSize = new SizeF(cellRect.Width - 2, splitedHeight - 2);
                                        var isSizeFixed = false;

                                        do
                                        {
                                            using var tryFont = new Font(this.Font.FontFamily, selectedFontSize, selectedCell.IsTextBold ? FontStyle.Bold : FontStyle.Regular);
                                            var tryTextSize = TextRenderer.MeasureText(e.Graphics, selctedCellText, tryFont);

                                            if (
                                                (availableSize.Width > tryTextSize.Width)
                                            && (availableSize.Height > tryTextSize.Height)
                                            )
                                            {
                                                isSizeFixed = true;
                                            }
                                            else
                                            {
                                                selectedFontSize--;
                                            }
                                        }
                                        while (false == isSizeFixed);
                                    }
                                    #endregion                                    

                                    using var cellTextFont = new Font(this.Font.FontFamily, selectedFontSize, selectedCell.IsTextBold ? FontStyle.Bold : FontStyle.Regular);
                                    var textSize = e.Graphics.MeasureString(selctedCellText, cellTextFont, new SizeF(cellRect.Width, splitedHeight));
                                    var textLocation = new PointF((cellRect.Width - textSize.Width) / 2, (splitedHeight - textSize.Height) / 2);

                                    if (0 != t % cellTexts.Length)
                                    {
                                        e.Graphics.DrawLine(lineNormalPen, new PointF(cellRect.Left, cellRect.Top + splitedOffset), new PointF(cellRect.Right, cellRect.Top + splitedOffset));
                                    }

                                    #region テキスト配置
                                    if (
                                      (ContentAlignment.BottomLeft == selectedCell.TextAlign)
                                    || (ContentAlignment.TopLeft == selectedCell.TextAlign)
                                    || (ContentAlignment.MiddleLeft == selectedCell.TextAlign)
                                    )
                                    {
                                        textLocation.X = 0;
                                    }
                                    else if (
                                      (ContentAlignment.BottomRight == selectedCell.TextAlign)
                                    || (ContentAlignment.TopRight == selectedCell.TextAlign)
                                    || (ContentAlignment.MiddleRight == selectedCell.TextAlign)
                                    )
                                    {
                                        textLocation.X = cellRect.Width - textSize.Width;
                                    }

                                    if (
                                      (ContentAlignment.TopCenter == selectedCell.TextAlign)
                                    || (ContentAlignment.TopLeft == selectedCell.TextAlign)
                                    || (ContentAlignment.TopRight == selectedCell.TextAlign)
                                    )
                                    {
                                        textLocation.Y = 0;
                                    }
                                    else if (
                                      (ContentAlignment.BottomCenter == selectedCell.TextAlign)
                                    || (ContentAlignment.BottomLeft == selectedCell.TextAlign)
                                    || (ContentAlignment.BottomRight == selectedCell.TextAlign)
                                    )
                                    {
                                        textLocation.Y = splitedHeight - textSize.Height;
                                    }
                                    #endregion

                                    textLocation.X += cellRect.Left + 1;
                                    textLocation.Y += cellRect.Top + 1;
                                    textLocation.Y += splitedOffset;

                                    e.Graphics.DrawString(selctedCellText, cellTextFont, cellTextBrush, textLocation);

                                    splitedOffset += splitedHeight;
                                }
                            }
                            else
                            {
                                var splitedWidth = cellRect.Width / cellTexts.Length;
                                var splitedOffset = 0f;
                                for (int t = 0; t < cellTexts.Length; t++)
                                {
                                    var selctedCellText = cellTexts[t];
                                    var selectedFontSize = selectedCell.FontSize;

                                    #region 文字列を縮小して全体を表示する 場合
                                    if (true == selectedCell.IsTextShrinkToFit)
                                    {
                                        var availableSize = new SizeF(cellRect.Height - 2, splitedWidth - 2);
                                        var isSizeFixed = false;

                                        do
                                        {
                                            using var tryFont = new Font(this.Font.FontFamily, selectedFontSize, selectedCell.IsTextBold ? FontStyle.Bold : FontStyle.Regular);
                                            var tryTextSize = TextRenderer.MeasureText(e.Graphics, selctedCellText, tryFont);

                                            if (
                                                (availableSize.Width > tryTextSize.Width)
                                            && (availableSize.Height > tryTextSize.Height)
                                            )
                                            {
                                                isSizeFixed = true;
                                            }
                                            else
                                            {
                                                selectedFontSize--;
                                            }
                                        }
                                        while (false == isSizeFixed);
                                    }
                                    #endregion                                    

                                    using var cellTextFont = new Font(this.Font.FontFamily, selectedFontSize, selectedCell.IsTextBold ? FontStyle.Bold : FontStyle.Regular);

                                    var textSize = e.Graphics.MeasureString(selctedCellText, cellTextFont, new SizeF(cellRect.Height, splitedWidth));
                                    var textLocation = new PointF((splitedWidth - textSize.Height) / 2, (cellRect.Height - textSize.Width) / 2);
                                    textLocation.X += textSize.Height;

                                    if (0 != t % cellTexts.Length)
                                    {
                                        e.Graphics.DrawLine(lineNormalPen, new PointF(cellRect.Left + splitedOffset, cellRect.Top), new PointF(cellRect.Left + splitedOffset, cellRect.Bottom));
                                    }

                                    #region テキスト配置
                                    if (
                                        (ContentAlignment.BottomLeft == selectedCell.TextAlign)
                                    || (ContentAlignment.TopLeft == selectedCell.TextAlign)
                                    || (ContentAlignment.MiddleLeft == selectedCell.TextAlign)
                                    )
                                    {
                                        textLocation.X = textSize.Height;
                                    }
                                    else if (
                                        (ContentAlignment.BottomRight == selectedCell.TextAlign)
                                    || (ContentAlignment.TopRight == selectedCell.TextAlign)
                                    || (ContentAlignment.MiddleRight == selectedCell.TextAlign)
                                    )
                                    {
                                        textLocation.X = splitedWidth;
                                    }

                                    if (
                                        (ContentAlignment.TopCenter == selectedCell.TextAlign)
                                    || (ContentAlignment.TopLeft == selectedCell.TextAlign)
                                    || (ContentAlignment.TopRight == selectedCell.TextAlign)
                                    )
                                    {
                                        textLocation.Y = 0;
                                    }
                                    else if (
                                        (ContentAlignment.BottomCenter == selectedCell.TextAlign)
                                    || (ContentAlignment.BottomLeft == selectedCell.TextAlign)
                                    || (ContentAlignment.BottomRight == selectedCell.TextAlign)
                                    )
                                    {
                                        textLocation.X = cellRect.Height - textSize.Width;
                                    }
                                    #endregion

                                    textLocation.X += cellRect.Left + 1;
                                    textLocation.Y += cellRect.Top + 1;
                                    textLocation.X += splitedOffset;

                                    e.Graphics.TranslateTransform(textLocation.X, textLocation.Y);
                                    e.Graphics.RotateTransform(90f);

                                    e.Graphics.DrawString(selctedCellText, cellTextFont, cellTextBrush, 0, 0);

                                    e.Graphics.ResetTransform();

                                    splitedOffset += splitedWidth;
                                }
                            }
                            try
                            {
                                using var ce = new PaintEventArgs(e.Graphics, new System.Drawing.Rectangle(cellX, cellY, this.columnWidth[c], this.rowHeight));
                                this.CellDrawing?.Invoke(this, ce, selectedCell, new RectangleD(cellRect));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }

                        // セルを引き伸ばす場合
                        if (true == selectedCell.IsStretched)
                        {
                            // 右側のセルは描画しない
                            break;
                        }

                        cellX += this.columnWidth[c];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// イベント:マウスクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableViewRendererUserControl_MouseClick(object sender, MouseEventArgs e)
        {
            var clickedCell = this.FindClickedCell(e);
            if (null != clickedCell)
            {
                this.OnCellClick(e, clickedCell);
            }
        }

        /// <summary>
        /// イベント:マウスダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableViewRendererUserControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var clickedCell = this.FindClickedCell(e);
            if (null != clickedCell)
            {
                this.OnCellDoubleClick(e, clickedCell);
            }
        }

        private void TableViewRendererUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            var currentMouseOverCell = this.FindClickedCell(e);
            if (null != currentMouseOverCell)
            {
                if (this.mouseOverCell != currentMouseOverCell)
                {
                    this.mouseOverCell = currentMouseOverCell;
                    this.Invalidate();
                }
            }
        }

        #endregion

        #region プロテクテッドメソッド

        /// <summary>
        /// レイアウトを再構築する
        /// </summary>
        protected void RebuildLayout()
        {
            if (
              (this.cells == null)
            || (this.cells.Count != this.rowNumber)
            || (this.cells[0].Count != this.columnWidth.Length)
            )
            {
                var previousCells = this.cells;

                var fontSize = this.Font.Size;

                this.cells = new List<List<Cell>>();
                foreach (var r in Enumerable.Range(0, this.rowNumber))
                {
                    var columns = new List<Cell>();
                    foreach (var c in Enumerable.Range(0, this.columnWidth.Length))
                    {
                        columns.Add(new Cell(r, c, fontSize));
                    }
                    this.cells.Add(columns);
                }

                if (null != previousCells)
                {
                    if (this.cells.Count < previousCells.Count)
                    {
                        previousCells.RemoveRange(this.cells.Count, previousCells.Count - this.cells.Count);
                    }

                    foreach (var previousRow in previousCells)
                    {
                        if (this.columnWidth.Length < previousRow.Count)
                        {
                            previousRow.RemoveRange(this.columnWidth.Length, previousRow.Count - this.columnWidth.Length);
                        }

                        foreach (var c in previousRow)
                        {
                            this.cells[c.Index.Y][c.Index.X].CopyFrom(c);
                        }
                    }
                }
            }

            this.Invalidate();
        }

        /// <summary>
        /// クリックされたセル情報への参照を取得する
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected Cell? FindClickedCell(MouseEventArgs e)
        {
            if (
              (0 > e.X)
            || (0 > e.Y)
            || (this.columnWidthCumulative.Last() < e.X)
            )
            {
                return null;
            }

            var clickedRow = e.Y / this.rowHeight;
            var clickedColumn = Array.FindIndex(this.columnWidthCumulative, (w => w > e.X));
            var clickedCell = (Cell?)null;

            if (
               (this.rowNumber > clickedRow)
            && (this.ColumnNumber > clickedColumn && 0 <= clickedColumn)
            )
            {
                clickedCell = this.cells[clickedRow][clickedColumn];

                foreach (var c in this.cells[clickedRow].GetRange(0, clickedColumn))
                {
                    if (c.IsStretched)
                    {
                        clickedCell = c;
                        break;
                    }
                }
            }

            return clickedCell;
        }

        /// <summary>
        /// 指定したセルの矩形情報を取得する
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>IsStretchedプロパティがtrueの場合は右側のセルが合成された矩形となります</remarks>
        public RectangleD GetCellRectangle(Cell c)
        {
            var location = new PointD((int)Math.Ceiling(this.borderSize + 0.5), (int)Math.Ceiling(this.borderSize + 0.5));

            var colOffset = 0;
            if (0 < c.Index.X)
            {
                colOffset = this.columnWidthCumulative[c.Index.X - 1];
            }
            location.Offset(colOffset, this.rowHeight * c.Index.Y);

            var width = this.columnWidth[c.Index.X];

            if (c.IsStretched)
            {
                width += this.columnWidth.Skip(c.Index.X + 1).Sum();
            }

            return new RectangleD(location, new SizeD(width, this.rowHeight));
        }

        /// <summary>
        /// イベント通知:セルクリック
        /// </summary>
        /// <param name="clickedCell"></param>
        protected void OnCellClick(MouseEventArgs e, Cell clickedCell)
        {
            try
            {
                this.CellClick?.Invoke(this, e, clickedCell, this.GetCellRectangle(clickedCell));
            }
            catch
            {
            }
        }

        /// <summary>
        /// イベント通知:セルクリック
        /// </summary>
        /// <param name="clickedCell"></param>
        protected void OnCellDoubleClick(MouseEventArgs e, Cell clickedCell)
        {
            try
            {
                this.CellDoubleClick?.Invoke(this, e, clickedCell, this.GetCellRectangle(clickedCell));
            }
            catch
            {
            }
        }

        #endregion
    }
}