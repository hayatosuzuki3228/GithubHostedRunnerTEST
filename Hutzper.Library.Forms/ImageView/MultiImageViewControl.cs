using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Drawing;
using System.Collections;
using System.ComponentModel;

namespace Hutzper.Library.Forms.ImageView
{
    /// <summary>
    /// 複数画像表示用ユーザーコントロール
    /// </summary>
    public partial class MultiImageViewControl : HutzperUserControl, IEnumerable<ImageViewControl>
    {
        #region プロパティ

        /// <summary>
        /// 扱う画像表示の個数
        /// </summary>
        public int NumberOfImages => this.Items.Count;

        /// <summary>
        /// 選択画像のインデックス
        /// </summary>
        /// <remarks>配置タイプがSpotlight/SingleSpotlightの場合に有効</remarks>
        public int SelectedIndex
        {
            get => this.selectedIndex;
            set
            {
                if (value >= 0 && value < this.Items.Count)
                {
                    if (this.selectedIndex != value)
                    {
                        this.selectedIndex = value;
                        this.OnMultiImageLayoutTypeChanged();

                        this.OnSelectedIndexChanged();
                    }
                }
                else
                {
                    Serilog.Log.Error($"{this}, selected index is out of range.");
                }
            }
        }
        /// <summary>
        /// 配置タイプ
        /// </summary>
        public MultiImageLayoutType LayoutType
        {
            get => this.selectedLayoutType;
            set
            {
                if (this.selectedLayoutType != value)
                {
                    this.selectedLayoutType = value;

                    if (false == this.DesignMode)
                    {
                        this.OnMultiImageLayoutTypeChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Spotlight配置時の配置比率
        /// </summary>
        public double SpotlightRatio
        {
            get => this.selectedSpotlightRatio;
            set
            {
                if (value > 0 && value < 1)
                {
                    this.selectedSpotlightRatio = value;

                    if (false == this.DesignMode)
                    {
                        this.OnMultiImageLayoutTypeChanged();
                    }
                }
                else
                {
                    Serilog.Log.Error($"{this}, spotlight ratio must be between 0 and 1.");
                }
            }
        }
        #endregion

        #region インデクサ

        /// <summary>
        /// インデックスを指定してImageViewControlへの参照を取得します
        /// </summary>
        /// <param name="index">対象のImageViewControlを示すインデックス</param>
        /// <returns>ImageViewControlへの参照</returns>
        public ImageViewControl this[int index]
        {
            get
            {
                if (index >= 0 && index < this.Items.Count)
                {
                    return this.Items[index];
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "index is out of range.");
                }
            }
        }

        #endregion

        #region イベント

        /// <summary>
        /// イベント:描画
        /// </summary>
        /// <remarks>このイベントを補足することでSurfaceを通じて画像上に任意の描画を行うことが出来ます</remarks>
        public event Action<object, ImageViewControl, Renderer, Surface>? Drawing;

        /// <summary>
        /// イベント:選択画像の変更
        /// </summary>
        public event Action<object, ImageViewControl>? SelectedIndexChanged;

        /// <summary>
        /// イベント:画像更新
        /// </summary>
        /// <remarks>実際に表示画像が更新されたタイミングで発生します。表示中の画像が必要な場合はImageViewControl.GetImageメソッドで取得してください</remarks>
        public Action<object, ImageViewControl>? ImageUpdated;

        #endregion

        #region IEnumerable

        /// <summary>
        /// IEnumerable<ImageViewControl> の実装
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ImageViewControl> GetEnumerator() => this.Items.GetEnumerator();

        /// <summary>
        /// 非ジェネリック版の GetEnumerator の実装（IEnumerable インターフェイスの要求）
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        #region リソースの破棄

        /// <summary>
        /// リソース破棄
        /// </summary>
        protected override void DisposeImplicit()
        {
            try
            {
                this.Items.ForEach(control => control.Dispose());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Items.Clear();
            }
        }

        #endregion

        #region フィールド

        protected List<ImageViewControl> Items = new();

        protected readonly int LayoutMargin = 8;
        protected System.Drawing.Size LatestClientSize;

        protected MultiImageLayoutType selectedLayoutType;
        protected int selectedIndex = -1;
        protected double selectedSpotlightRatio = 0.7;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MultiImageViewControl()
        {
            this.InitializeComponent();

            this.selectedLayoutType = MultiImageLayoutType.Spotlight;
            this.LatestClientSize = this.ClientSize;
        }

        #endregion

        #region publicメソッド

        /// <summary>
        /// ImageViewControlを追加する
        /// </summary>
        /// <param name="control">追加するImageViewControl</param>
        public void Add(ImageViewControl control)
        {
            try
            {
                if (this.Items.Contains(control))
                {
                    return;
                }

                control.Index = this.Items.Count;
                control.UseInjectedTiming = true;
                control.ImageUpdated += this.Item_ImageUpdated;
                control.Renderer.Drawing += this.Renderer_Drawing;
                control.Renderer.MouseDoubleClick += this.Renderer_MouseDoubleClick;

                this.Items.Add(control);
                this.Controls.Add(control);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.OnMultiImageLayoutTypeChanged();

                if (0 < this.Items.Count)
                {
                    this.ImageDrawingTimer.Interval = Math.Max(1000 / 20 / this.Items.Count, 10);
                    this.ImageDrawingTimer.Start();
                }
            }
        }

        /// <summary>
        /// ImageViewControlを削除する
        /// </summary>
        /// <param name="control">削除対象のImageViewControl</param>
        /// <param name="shouldDispose">取り除いたImageViewControlをDisposeするかどうか</param>
        public void Remove(ImageViewControl control, bool shouldDispose = true)
        {
            try
            {
                if (true == this.Items.Contains(control))
                {
                    control.ImageUpdated -= this.Item_ImageUpdated;
                    control.Renderer.Drawing -= this.Renderer_Drawing;
                    control.Renderer.MouseDoubleClick -= this.Renderer_MouseDoubleClick;

                    this.Items.Remove(control);
                    this.Controls.Remove(control);

                    if (true == shouldDispose)
                    {
                        control.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.OnMultiImageLayoutTypeChanged();

                if (0 < this.Items.Count)
                {
                    this.ImageDrawingTimer.Interval = Math.Max(1000 / 20 / this.Items.Count, 10);
                    this.ImageDrawingTimer.Start();
                }
                else
                {
                    this.ImageDrawingTimer.Stop();
                }
            }
        }

        /// <summary>
        /// 再描画を行う
        /// </summary>
        /// <remarks>プロパティ変更時に呼び出して設定の変更を描画に反映させます</remarks>
        public void Redraw() => this.Items.ForEach(control => control.Redraw());

        /// <summary>
        /// Rendererに対応するImageViewControlのインデックスを取得する
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public int IndexOf(Renderer r) => this.Items.FindIndex(control => control.Renderer == r);

        #endregion

        #region protectedメソッド

        /// <summary>
        /// 配置変更処理
        /// </summary>
        protected void OnMultiImageLayoutTypeChanged()
        {
            try
            {
                if (this.Items.Count == 0)
                {
                    return;
                }
                else if (0 > this.selectedIndex)
                {
                    this.selectedIndex = 0;
                }

                this.SuspendLayout();

                this.Items.ForEach(control => control.Visible = false);

                switch (this.LayoutType)
                {
                    case MultiImageLayoutType.HorizontalTile:
                        this.ArrangeHorizontally();
                        break;
                    case MultiImageLayoutType.VerticalTile:
                        this.ArrangeVertically();
                        break;
                    case MultiImageLayoutType.Spotlight:
                        this.ArrangeSpotlight();
                        break;
                    case MultiImageLayoutType.SingleSpotlight:
                        this.ArrangeSingleSpotlight();
                        break;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.ResumeLayout();
                this.Invalidate();
            }
        }

        /// <summary>
        /// 水平方向にタイリングする配置
        /// </summary>
        protected void ArrangeHorizontally()
        {
            var totalMargin = (this.Items.Count - 1) * this.LayoutMargin;
            var availableWidth = this.Width - totalMargin;
            var widthPerControl = availableWidth / this.Items.Count;

            var xOffset = 0;
            foreach (var control in this.Items)
            {
                control.SetBounds(xOffset, 0, widthPerControl, this.Height - 1);
                control.Visible = true;
                xOffset += widthPerControl + this.LayoutMargin;
            }
        }

        /// <summary>
        /// 垂直方向にタイリングする配置
        /// </summary>
        protected void ArrangeVertically()
        {
            var totalMargin = (this.Items.Count - 1) * this.LayoutMargin;
            var availableHeight = this.Height - totalMargin;
            var heightPerControl = availableHeight / this.Items.Count;

            var yOffset = 0;
            foreach (var control in this.Items)
            {
                control.SetBounds(0, yOffset, this.Width - 1, heightPerControl);
                control.Visible = true;
                yOffset += heightPerControl + this.LayoutMargin;
            }
        }

        /// <summary>
        /// スポットライト配置
        /// </summary>
        protected void ArrangeSpotlight()
        {
            if (2 <= this.Items.Count)
            {
                // 最初の画像を大きく表示（デフォルトは幅の 70%）
                var spotlightWidth = (int)(this.Width * this.selectedSpotlightRatio) - this.LayoutMargin; // 右側にスペースを確保
                this.Items[this.selectedIndex].SetBounds(0, 0, spotlightWidth, this.Height);
                this.Items[this.selectedIndex].Visible = true;

                // 残りの画像を右側に縦に並べる（デフォルトは幅の 30%）
                var smallWidth = this.Width - spotlightWidth;
                var totalMargin = (this.Items.Count - 2) * this.LayoutMargin;
                var availableHeight = this.Height - totalMargin;
                var smallHeight = availableHeight / (this.Items.Count - 1);

                var yOffset = 0;
                foreach (var control in this.Items)
                {
                    if (this.Items[this.selectedIndex] == control)
                    {
                        continue;
                    }

                    control.SetBounds(spotlightWidth + this.LayoutMargin, yOffset, smallWidth, smallHeight);
                    control.Visible = true;
                    yOffset += smallHeight + this.LayoutMargin;
                }
            }
            else
            {
                this.ArrangeSingleSpotlight();
            }
        }

        /// <summary>
        /// 単一スポットライト配置
        /// </summary>
        protected void ArrangeSingleSpotlight()
        {
            var control = this.Items.FirstOrDefault();

            if (this.Items.Count > this.selectedIndex)
            {
                control = this.Items[this.selectedIndex];
            }

            if (control is not null)
            {
                control.SetBounds(0, 0, this.Width - 1, this.Height - 1);
                control.Visible = true;
            }
        }

        /// <summary>
        /// 描画イベント通知
        /// </summary>
        /// <param name="r"></param>
        /// <param name="s"></param>
        protected void OnRenderer_Drawing(Renderer r, Surface s)
        {
            try
            {
                var control = this.Items[this.IndexOf(r)];
                this.Drawing?.Invoke(this, control, r, s);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 選択画像変更イベント通知
        /// </summary>
        protected void OnSelectedIndexChanged()
        {
            try
            {
                this.SelectedIndexChanged?.Invoke(this, this.Items[this.selectedIndex]);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 画像更新イベント通知
        /// </summary>
        protected void OnImageUpdated(object sender)
        {
            try
            {
                if (sender is ImageViewControl control)
                {
                    this.ImageUpdated?.Invoke(this, control);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region ImageViewControl

        private void Renderer_Drawing(object sender, Surface s) => this.OnRenderer_Drawing((Renderer)sender, s);

        private void Renderer_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            try
            {
                if (this.LayoutType != MultiImageLayoutType.Spotlight && this.LayoutType != MultiImageLayoutType.SingleSpotlight)
                {
                    return;
                }

                if (sender is Renderer renderer)
                {
                    this.SelectedIndex = this.IndexOf(renderer);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        private void Item_ImageUpdated(object sender) => this.OnImageUpdated((ImageViewControl)sender);

        #endregion

        #region GUIイベント

        /// <summary>
        /// 描画更新タイマイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageDrawingTimer_Tick(object sender, EventArgs e)
        {
            if (false == this.LatestClientSize.Equals(this.ClientSize))
            {
                this.LatestClientSize = this.ClientSize;
                this.OnMultiImageLayoutTypeChanged();

                if (this.Parent is not null)
                {
                    this.Parent.Invalidate();
                }
            }

            this.Items.ForEach(control => control.InjectUpdateTiming());
        }

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);

        //    // デザイン時にはサンプルの画像を追加
        //    if (this.DesignMode)
        //    {
        //        foreach (var index in Enumerable.Range(0, 4))
        //        {
        //            var control = new ImageViewControl()
        //            {
        //                ImageDescription = $"カメラ{this.Items.Count + 1}",
        //                DisplayImageDescription = true,
        //                UseInjectedTiming = true,
        //            };

        //            control.Attach(this.Logger);

        //            this.Items.Add(control);
        //            this.Controls.Add(control);
        //        }

        //        this.OnMultiImageLayoutTypeChanged();

        //        this.ImageDrawingTimer.Interval = 500;
        //        this.ImageDrawingTimer.Start();
        //    }
        //}

        #endregion
    }

    /// <summary>
    /// 画像配置タイプ
    /// </summary>
    [Serializable]
    public enum MultiImageLayoutType
    {
        [AliasName("HorizontalTile")]
        [Description("水平タイル")]
        HorizontalTile,  // 水平方向にタイリングする配置

        [AliasName("VerticalTile")]
        [Description("垂直タイル")]
        VerticalTile,    // 垂直方向にタイリングする配置

        [AliasName("Spotlight")]
        [Description("スポットライト")]
        Spotlight,       // 注目画像を大きく表示し、その他を右側垂直に小さく表示する

        [AliasName("SingleSpotlight")]
        [Description("シングルスポットライト")]
        SingleSpotlight, // 注目画像のみを大きく表示し、その他を非表示にする
    }
}
