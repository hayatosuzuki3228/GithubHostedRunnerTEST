using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hutzper.Library.Wpf
{
    /// <summary>
    /// ImagePreview.xaml の相互作用ロジック
    /// 画像のプレビュー表示と基本的な操作機能を提供するコントロール
    /// </summary>
    public partial class ImagePreview : UserControl
    {
        #region 初期化

        /// <summary>
        /// ImagePreviewコントロールを初期化します
        /// </summary>
        public ImagePreview()
        {
            InitializeComponent();
            this.SampleImage.MouseLeftButtonDown += OnMouseLeftButtonDown;
            this.SampleImage.MouseMove += OnMouseMove;

            // MyCanvasを利用する場合
            if (this.ShowArea)
            {
                this.MyCanvas.MouseMove += OnMouseMove;
                this.MyCanvas.MouseLeftButtonDown += OnMouseLeftButtonDown;
            }
        }

        #endregion

        #region 依存関係プロパティ

        /// <summary>
        /// エリア表示状態を管理する依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty ShowAreaProperty =
            DependencyProperty.Register("ShowArea", typeof(bool), typeof(ImagePreview),
            new PropertyMetadata(false));

        /// <summary>
        /// MyCanvas表示・非表示
        /// </summary>
        public bool ShowArea
        {
            get { return (bool)this.GetValue(ShowAreaProperty); }
            set { this.SetValue(ShowAreaProperty, value); }
        }

        #endregion

        #region プロパティとイベント

        // オリジナル画像
        private BitmapImage? _originImage;

        /// <summary>
        /// プレビュー画像のマウスクリックイベント
        /// </summary>
        public event MouseButtonEventHandler? PreviewImageMouseDown;

        /// <summary>
        /// プレビュー画像のサイズ変更イベント
        /// </summary>
        public event SizeChangedEventHandler? PreviewImageSizeChanged;

        /// <summary>
        /// プレビュー画像の左ボタンクリックイベント
        /// </summary>
        public event MouseButtonEventHandler? PreviewImageMouseLeftButtonDown;

        /// <summary>
        /// プレビュー画像のマウス移動イベント
        /// </summary>
        public event MouseEventHandler? PreviewImageMouseMove;

        /// <summary>
        /// 現在ドラッグ操作中かどうか
        /// </summary>
        public bool IsDragging { get; set; }

        /// <summary>
        /// ドラッグ中のポイントのインデックス
        /// </summary>
        public int DraggedPointIndex { get; set; } = -1;

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// リソースの終了処理を行います
        /// </summary>
        public void Clear()
        {
            // 画像ソースをnullに設定して破棄
            this.SampleImage.Source = null;
            this.MyCanvas.Children.Clear();

            if (this._originImage is not null)
            {
                this._originImage.UriSource = null;
                this._originImage.StreamSource = null;

                // 参照をnullに設定
                this._originImage = null;
            }
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// Polygonの各頂点に対して、動的なバインディングを設定するメソッド
        /// </summary>
        /// <param name="polygon">バインディングを設定するポリゴン</param>
        /// <param name="xPaths">X座標のバインディングパス配列</param>
        /// <param name="yPaths">Y座標のバインディングパス配列</param>
        /// <example>
        /// string[] xPaths = new[] { "CustomX1", "CustomX2", "CustomX3", "CustomX4" };
        /// string[] yPaths = new[] { "CustomY1", "CustomY2", "CustomY3", "CustomY4" };
        /// SetDynamicPointBindings(myPolygon, xPaths, yPaths);
        /// </example>
        private void SetDynamicPointBindings(Polygon polygon, string[] xPaths, string[] yPaths)
        {
            // 新しいMultiBindingを作成
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Converter = this.Resources["PointsConverter"] as IMultiValueConverter;

            // 動的にBindingを追加
            for (int i = 0; i < xPaths.Length; i++)
            {
                multiBinding.Bindings.Add(new Binding(xPaths[i]));
                multiBinding.Bindings.Add(new Binding(yPaths[i]));
            }

            // Polygonに適用
            polygon.SetBinding(Polygon.PointsProperty, multiBinding);
        }


        #endregion

        #region イベントハンドラ

        /// <summary>
        /// 画像をクリックしたときの処理
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                this._originImage = new();
                this._originImage.BeginInit();
                this._originImage.UriSource = new Uri(filePath, UriKind.Absolute);
                this._originImage.CacheOption = BitmapCacheOption.OnLoad;
                this._originImage.EndInit();
                this._originImage.Freeze();
                this.SampleImage.Source = this._originImage;

                try
                {
                    this.Cursor = Cursors.Wait;

                    if (PreviewImageMouseDown is not null)
                    {
                        PreviewImageMouseDown?.Invoke(sender, e);
                    }
                }
                finally
                {
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        /// <summary>
        /// 画像サイズが変更されたときの処理
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (PreviewImageSizeChanged is not null)
            {
                PreviewImageSizeChanged?.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 画像の左ボタンをクリックしたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // イベントを外部に伝播
            PreviewImageMouseLeftButtonDown?.Invoke(sender, e);
        }

        /// <summary>
        /// 画像のマウス移動イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // イベントを外部に伝播
            PreviewImageMouseMove?.Invoke(sender, e);
        }

        #endregion
    }
}
