using System.Windows;
using System.Windows.Controls;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Hutzper.Library.Wpf.Controls.CustomControls"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Hutzper.Library.Wpf.Controls.CustomControls;assembly=Hutzper.Library.Wpf.Controls.CustomControls"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:TextBox/>
    ///
    /// </summary>
    public class TextBox : System.Windows.Controls.TextBox
    {

        static TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        public TextBox() : base()
        {
            // TextChangedイベントにハンドラを追加
            this.Loaded += this.TextBox_Loaded;
            this.TextChanged += this.TextBox_TextChanged;

            // テキスト入力完了時のイベントハンドラを追加
            this.LostFocus += this.TextBox_LostFocus;

            // 初期テキスト配置を設定
            this.UpdateTextAlignment();
        }

        #region 依存関係プロパティ
        // 必須項目フラグ
        public static readonly DependencyProperty RequiredProperty =
            DependencyProperty.Register("Required", typeof(bool), typeof(TextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// 必須フラグ
        /// </summary>
        public bool Required
        {
            get { return (bool)this.GetValue(RequiredProperty); }
            set { this.SetValue(RequiredProperty, value); }
        }

        // FontSizeプロパティ
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(FontSizeType), typeof(TextBox),
                new PropertyMetadata(FontSizeType.Xs));

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public FontSizeType TextFontSize
        {
            get { return (FontSizeType)this.GetValue(TextFontSizeProperty); }
            set { this.SetValue(TextFontSizeProperty, value); }
        }

        // テキストボックスを複数行として表示するかどうか
        public static readonly DependencyProperty IsMultilineProperty =
            DependencyProperty.Register("IsMultiline", typeof(bool), typeof(TextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// テキストボックスを複数行として表示するかどうか
        /// </summary>
        public bool IsMultiline
        {
            get { return (bool)this.GetValue(IsMultilineProperty); }
            set { this.SetValue(IsMultilineProperty, value); }
        }

        #endregion

        #region 依存関係プロパティ - エラー関連

        // エラーフラグ
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register("HasError", typeof(bool), typeof(TextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// エラーフラグ
        /// </summary>
        public bool HasError
        {
            get { return (bool)this.GetValue(HasErrorProperty); }
            set { this.SetValue(HasErrorProperty, value); }
        }
        #endregion

        #region 依存関係プロパティ - テキスト変更関連
        // 変更されたかどうかを示すプロパティ
        public static readonly DependencyProperty IsModifiedProperty =
            DependencyProperty.Register("IsModified", typeof(bool), typeof(TextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// テキストが変更されたかどうか
        /// </summary>
        public bool IsModified
        {
            get { return (bool)this.GetValue(IsModifiedProperty); }
            set { this.SetValue(IsModifiedProperty, value); }
        }

        // 初期値を保持するプロパティ
        private string? _initialValue;

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            // 初期値を保存
            _initialValue = this.Text;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 現在の値と初期値を比較して、変更があれば IsModified を true に設定
            this.IsModified = this.Text != _initialValue;
        }

        // 変更をリセットするメソッド
        public void ResetModified()
        {
            _initialValue = this.Text;
            this.IsModified = false;
        }
        #endregion

        #region 依存関係プロパティ - テキストフォーマット関連
        // テキストタイプのプロパティ
        public static readonly DependencyProperty TextTypeProperty =
            DependencyProperty.Register("TextType", typeof(TextType), typeof(TextBox),
                new PropertyMetadata(TextType.String, OnTextTypeChanged));

        /// <summary>
        /// テキストタイプ（文字列または数値）
        /// </summary>
        public TextType TextType
        {
            get { return (TextType)this.GetValue(TextTypeProperty); }
            set { this.SetValue(TextTypeProperty, value); }
        }

        // 小数点以下の桁数
        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(TextBox),
                new PropertyMetadata(2, OnDecimalPlacesChanged));

        /// <summary>
        /// 数値タイプの場合の小数点以下の桁数
        /// </summary>
        public int DecimalPlaces
        {
            get { return (int)this.GetValue(DecimalPlacesProperty); }
            set { this.SetValue(DecimalPlacesProperty, value); }
        }

        private static void OnTextTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                // テキストタイプが変更されたときの処理
                textBox.UpdateTextAlignment();
                textBox.FormatText();
            }
        }

        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && textBox.TextType == TextType.Number)
            {
                // 小数点以下の桁数が変更されたときの処理（数値タイプの場合のみ）
                textBox.FormatText();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // フォーカスが外れたときにテキストをフォーマット
            this.FormatText();
        }

        private void UpdateTextAlignment()
        {
            // テキストの配置を設定（文字列：左寄せ、数値：右寄せ）
            this.TextAlignment = this.TextType == TextType.Number ? TextAlignment.Right : TextAlignment.Left;
        }

        /// <summary>
        /// 数値の場合テキストをフォーマットするメソッド
        /// </summary>
        private void FormatText()
        {
            if (this.TextType == TextType.Number && !string.IsNullOrEmpty(this.Text))
            {
                // 数値タイプの場合のフォーマット処理
                if (decimal.TryParse(this.Text, out decimal numValue))
                {
                    // 指定された小数点以下の桁数でフォーマット
                    string format = $"F{this.DecimalPlaces}";
                    this.Text = numValue.ToString(format);
                }
            }
        }

        // テキストの内容を数値として取得するメソッド
        public decimal? GetNumericValue()
        {
            if (this.TextType == TextType.Number && !string.IsNullOrEmpty(this.Text))
            {
                if (decimal.TryParse(this.Text, out decimal numValue))
                {
                    return numValue;
                }
            }
            return null;
        }
        #endregion
    }
}
