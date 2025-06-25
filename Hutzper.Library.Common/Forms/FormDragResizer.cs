namespace Hutzper.Library.Common.Forms
{
    public class FormDragResizer
    {
        // サイズ変更が有効になる枠
        public ResizeDirection ResizeDirection { get; init; }

        // サイズ変更中を表す状態
        public ResizeDirection ResizeStatus { get; private set; }

        public bool IsResizing => this.ResizeStatus != ResizeDirection.None;

        // サイズ変更の対象となるフォーム
        private readonly Form resizeForm;

        private readonly Control targetCotrol;

        // サイズ変更が有効になる範囲の幅
        private readonly int resizeAreaRange;

        // 標準のカーソル
        private readonly Cursor defaultCursor;

        // マウスをクリックした位置
        private Point lastMouseDownPoint;

        // マウスをクリックした時点のサイズ
        private Size lastMouseDownSize;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="resizeForm">サイズ変更の対象となるフォーム</param>
        /// <param name="resizeDirection">サイズ変更が有効になる枠</param>
        /// <param name="resizeAreaWidth">サイズ変更が有効になる範囲の幅</param>
        public FormDragResizer(Form resizeForm, Control? cursorControl = null, ResizeDirection resizeDirection = ResizeDirection.All, int resizeAreaWidth = 8)
        {
            this.resizeForm = resizeForm;
            this.ResizeDirection = resizeDirection;
            this.resizeAreaRange = resizeAreaWidth;

            // 現時点でのカーソルを保存しておく
            this.defaultCursor = resizeForm.Cursor;

            // イベントハンドラを追加
            this.targetCotrol = (null != cursorControl) ? cursorControl : this.resizeForm;
            this.targetCotrol.MouseDown += this.resize_MouseDown;
            this.targetCotrol.MouseMove += this.resize_MouseMove;
            this.targetCotrol.MouseUp += this.resize_MouseUp;
            this.targetCotrol.MouseLeave += this.resize_MouseLeave;
        }

        /// <summary>
        /// マウスボタン押下イベントハンドラ
        /// </summary>
        void resize_MouseDown(object? sender, MouseEventArgs e)
        {
            // クリックしたポイントを保存する
            this.lastMouseDownPoint = e.Location;

            // クリックした時点でのフォームのサイズを保存する
            this.lastMouseDownSize = this.resizeForm.Size;

            // クリックした位置から、サイズ変更する方向を決める
            this.ResizeStatus = ResizeDirection.None;

            // 上の判定
            if ((this.ResizeDirection & ResizeDirection.Top) == ResizeDirection.Top)
            {
                var topRect = new Rectangle(0, 0, this.targetCotrol.Width, this.resizeAreaRange);
                if (topRect.Contains(e.Location))
                {
                    this.ResizeStatus |= ResizeDirection.Top;
                }
            }

            // 左の判定
            if ((this.ResizeDirection & ResizeDirection.Left) == ResizeDirection.Left)
            {
                var leftRect = new Rectangle(0, 0, this.resizeAreaRange, this.targetCotrol.Height);
                if (leftRect.Contains(e.Location))
                {
                    this.ResizeStatus |= ResizeDirection.Left;
                }
            }

            // 下の判定
            if ((this.ResizeDirection & ResizeDirection.Bottom) == ResizeDirection.Bottom)
            {
                var bottomRect = new Rectangle(0, this.targetCotrol.Height - this.resizeAreaRange, this.targetCotrol.Width, this.resizeAreaRange);
                if (bottomRect.Contains(e.Location))
                {
                    this.ResizeStatus |= ResizeDirection.Bottom;
                }
            }

            // 右の判定
            if ((this.ResizeDirection & ResizeDirection.Right) == ResizeDirection.Right)
            {
                var rightRect = new Rectangle(this.targetCotrol.Width - this.resizeAreaRange, 0, this.resizeAreaRange, this.targetCotrol.Height);
                if (rightRect.Contains(e.Location))
                {
                    this.ResizeStatus |= ResizeDirection.Right;
                }
            }

            // サイズ変更の対象だったら、マウスキャプチャー
            if (this.ResizeStatus != ResizeDirection.None)
            {
                this.targetCotrol.Capture = true;
            }
        }

        /// <summary>
        /// マウス移動イベントハンドラ
        /// </summary>
        void resize_MouseMove(object? sender, MouseEventArgs e)
        {
            // サイズ変更が有効になる枠の上にカーソルが乗ったら
            // マウスカーソルをサイズ変更用のものに変更する

            // どの枠の上にカーソルが乗っているか
            var cursorPos = ResizeDirection.None;

            // 上の判定
            if ((this.ResizeDirection & ResizeDirection.Top) == ResizeDirection.Top)
            {
                var topRect = new Rectangle(0, 0, this.targetCotrol.Width, this.resizeAreaRange);
                if (topRect.Contains(e.Location))
                {
                    cursorPos |= ResizeDirection.Top;
                }
            }

            // 左の判定
            if ((this.ResizeDirection & ResizeDirection.Left) == ResizeDirection.Left)
            {
                var leftRect = new Rectangle(0, 0, this.resizeAreaRange, this.targetCotrol.Height);
                if (leftRect.Contains(e.Location))
                {
                    cursorPos |= ResizeDirection.Left;
                }
            }

            // 下の判定
            if ((this.ResizeDirection & ResizeDirection.Bottom) == ResizeDirection.Bottom)
            {
                var bottomRect = new Rectangle(0, this.targetCotrol.Height - this.resizeAreaRange, this.targetCotrol.Width, this.resizeAreaRange);
                if (bottomRect.Contains(e.Location))
                {
                    cursorPos |= ResizeDirection.Bottom;
                }
            }

            // 右の判定
            if ((this.ResizeDirection & ResizeDirection.Right) == ResizeDirection.Right)
            {
                var rightRect = new Rectangle(this.targetCotrol.Width - this.resizeAreaRange, 0, this.resizeAreaRange, this.targetCotrol.Height);
                if (rightRect.Contains(e.Location))
                {
                    cursorPos |= ResizeDirection.Right;
                }
            }

            // 左上（左上から右下への斜め矢印）
            if (((cursorPos & ResizeDirection.Left) == ResizeDirection.Left)
                && ((cursorPos & ResizeDirection.Top) == ResizeDirection.Top))
            {
                this.resizeForm.Cursor = Cursors.SizeNWSE;
            }
            // 右下（左上から右下への斜め矢印）
            else if (((cursorPos & ResizeDirection.Right) == ResizeDirection.Right)
                && ((cursorPos & ResizeDirection.Bottom) == ResizeDirection.Bottom))
            {
                this.resizeForm.Cursor = Cursors.SizeNWSE;
            }
            // 右上（右上から左下への斜め矢印）
            else if (((cursorPos & ResizeDirection.Right) == ResizeDirection.Right)
                && ((cursorPos & ResizeDirection.Top) == ResizeDirection.Top))
            {
                this.resizeForm.Cursor = Cursors.SizeNESW;
            }
            // 左下（右上から左下への斜め矢印）
            else if (((cursorPos & ResizeDirection.Left) == ResizeDirection.Left)
                && ((cursorPos & ResizeDirection.Bottom) == ResizeDirection.Bottom))
            {
                this.resizeForm.Cursor = Cursors.SizeNESW;
            }
            // 上（上下矢印）
            else if ((cursorPos & ResizeDirection.Top) == ResizeDirection.Top)
            {
                this.resizeForm.Cursor = Cursors.SizeNS;
            }
            // 左（左右矢印）
            else if ((cursorPos & ResizeDirection.Left) == ResizeDirection.Left)
            {
                this.resizeForm.Cursor = Cursors.SizeWE;
            }
            // 下（上下矢印）
            else if ((cursorPos & ResizeDirection.Bottom) == ResizeDirection.Bottom)
            {
                this.resizeForm.Cursor = Cursors.SizeNS;
            }
            // 右（左右矢印）
            else if ((cursorPos & ResizeDirection.Right) == ResizeDirection.Right)
            {
                this.resizeForm.Cursor = Cursors.SizeWE;
            }
            // どこにも属していない（デフォルト）
            else
            {
                this.resizeForm.Cursor = this.defaultCursor;
            }

            // ボタンを押していた場合は、サイズ変更を行う
            if (e.Button == MouseButtons.Left)
            {
                // ドラッグにより移動した距離を計算
                var diffX = e.X - this.lastMouseDownPoint.X;
                var diffY = e.Y - this.lastMouseDownPoint.Y;

                // 上
                if ((this.ResizeStatus & ResizeDirection.Top) == ResizeDirection.Top)
                {
                    var h = this.resizeForm.Height;
                    this.resizeForm.Height -= diffY;
                    this.resizeForm.Top += h - this.resizeForm.Height;
                }
                // 左
                if ((this.ResizeStatus & ResizeDirection.Left) == ResizeDirection.Left)
                {
                    var w = this.resizeForm.Width;
                    this.resizeForm.Width -= diffX;
                    this.resizeForm.Left += w - this.resizeForm.Width;
                }
                // 下
                if ((this.ResizeStatus & ResizeDirection.Bottom) == ResizeDirection.Bottom)
                {
                    this.resizeForm.Height = this.lastMouseDownSize.Height + diffY;
                }
                // 右
                if ((this.ResizeStatus & ResizeDirection.Right) == ResizeDirection.Right)
                {
                    this.resizeForm.Width = this.lastMouseDownSize.Width + diffX;
                }
            }
        }

        /// <summary>
        /// マウスボタン押上イベントハンドラ
        /// </summary>
        void resize_MouseUp(object? sender, MouseEventArgs e)
        {
            // マウスキャプチャーを終了する
            this.targetCotrol.Capture = false;
        }

        void resize_MouseLeave(object? sender, EventArgs e)
        {
            this.resizeForm.Cursor = this.defaultCursor;
        }
    }

    /// <summary>
    /// サイズ変更の対象となる枠の位置
    /// </summary>
    public enum ResizeDirection
    {
        None = 0,
        Top = 1,
        Left = 2,
        Bottom = 4,
        Right = 8,
        All = 15
    }
}