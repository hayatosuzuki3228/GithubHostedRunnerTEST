using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    /// <summary>
    /// LabeledContent.xaml の相互作用ロジック
    /// ラベル付きのテキストコンテンツを表示するためのユーザーコントロール
    /// </summary>
    public partial class LabeledContent : UserControl, INotifyPropertyChanged
    {
        #region 初期化と基本イベント

        /// <summary>
        /// プロパティ変更通知イベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティ変更を通知します
        /// </summary>
        /// <param name="propertyName">変更されたプロパティ名</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// LabeledContentコントロールを初期化します
        /// </summary>
        public LabeledContent()
        {
            InitializeComponent();
        }

        #endregion

        #region 依存関係プロパティ - ラベル関連

        // ラベルテキスト
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledContent),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 項目ラベルのテキスト
        /// </summary>
        public string LabelText
        {
            get { return (string)this.GetValue(LabelTextProperty); }
            set { this.SetValue(LabelTextProperty, value); }
        }

        // ラベル幅
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(GridLength), typeof(LabeledContent),
                new PropertyMetadata(GridLength.Auto));

        /// <summary>
        /// 項目ラベルの幅
        /// </summary>
        public GridLength LabelWidth
        {
            get { return (GridLength)this.GetValue(LabelWidthProperty); }
            set { this.SetValue(LabelWidthProperty, value); }
        }

        // フォントサイズ用の依存関係プロパティ
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(FontSizeType), typeof(LabeledContent),
                new PropertyMetadata(FontSizeType.Xs));

        /// <summary>
        /// 項目ラベルのフォントサイズ
        /// </summary>
        public FontSizeType LabelFontSize
        {
            get { return (FontSizeType)this.GetValue(LabelFontSizeProperty); }
            set { this.SetValue(LabelFontSizeProperty, value); }
        }

        // 項目フォントの太さ用の依存関係プロパティ
        public static readonly DependencyProperty LabelFontWeightProperty =
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(LabeledContent),
                new PropertyMetadata(FontWeights.Normal)); // デフォルトは通常の太さ

        /// <summary>
        /// 項目ラベルのフォントの太さ
        /// </summary>
        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)this.GetValue(LabelFontWeightProperty); }
            set { this.SetValue(LabelFontWeightProperty, value); }
        }

        #endregion

        #region 依存関係プロパティ - コンテンツ関連

        // 内容テキスト
        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register("ContentText", typeof(string), typeof(LabeledContent),
                new PropertyMetadata(string.Empty, OnContentTextChanged));

        /// <summary>
        /// 内容ラベルのテキスト
        /// </summary>
        public string ContentText
        {
            get { return (string)this.GetValue(ContentTextProperty); }
            set { this.SetValue(ContentTextProperty, value); }
        }

        /// <summary>
        /// コンテンツテキスト変更時のコールバック
        /// </summary>
        /// <param name="d">依存関係プロパティを持つオブジェクト</param>
        /// <param name="e">イベント引数</param>
        private static void OnContentTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LabeledContent;
            control?.OnPropertyChanged("ContentText");
        }

        // 内容の背景色
        public static readonly DependencyProperty ContentBackgroundProperty =
            DependencyProperty.Register("ContentBackground", typeof(System.Windows.Media.Brush), typeof(LabeledContent),
                new PropertyMetadata(System.Windows.Media.Brushes.Transparent));

        /// <summary>
        /// 内容ラベルの背景色
        /// </summary>
        public System.Windows.Media.Brush ContentBackground
        {
            get { return (System.Windows.Media.Brush)this.GetValue(ContentBackgroundProperty); }
            set { this.SetValue(ContentBackgroundProperty, value); }
        }

        // 内容のフォント色
        public static readonly DependencyProperty ContentForegroundProperty =
            DependencyProperty.Register("ContentForeground", typeof(System.Windows.Media.Brush), typeof(LabeledContent),
                new PropertyMetadata(System.Windows.Media.Brushes.Black));

        /// <summary>
        /// 内容ラベルのフォント色
        /// </summary>
        public System.Windows.Media.Brush ContentForeground
        {
            get { return (System.Windows.Media.Brush)this.GetValue(ContentForegroundProperty); }
            set { this.SetValue(ContentForegroundProperty, value); }
        }

        // フォントサイズ用の依存関係プロパティ
        public static readonly DependencyProperty ContentFontSizeProperty =
            DependencyProperty.Register("ContentFontSize", typeof(FontSizeType), typeof(LabeledContent),
                new PropertyMetadata(FontSizeType.Xs));

        /// <summary>
        /// Contentのフォントサイズ
        /// </summary>
        public FontSizeType ContentFontSize
        {
            get { return (FontSizeType)this.GetValue(ContentFontSizeProperty); }
            set { this.SetValue(ContentFontSizeProperty, value); }
        }

        #endregion
    }
}
