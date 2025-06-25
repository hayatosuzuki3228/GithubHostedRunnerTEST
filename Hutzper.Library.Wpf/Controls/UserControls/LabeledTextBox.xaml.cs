using Hutzper.Library.Wpf.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    /// <summary>
    /// LabeledTextBox.xaml の相互作用ロジック
    /// ラベル付きテキストボックスコントロールを提供します
    /// </summary>
    public partial class LabeledTextBox : UserControl, INotifyPropertyChanged
    {
        #region 初期化と基本イベント

        /// <summary>
        /// 内部のTextBoxコントロールを取得します
        /// </summary>
        public TextBox TextBox
        {
            get { return PART_TextBox; }
        }

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
        /// LabeledTextBoxコントロールを初期化します
        /// </summary>
        public LabeledTextBox()
        {
            InitializeComponent();

            // テキスト変更時にバリデーションを実行
            this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Text" && this.ValidationEnabled)
                {
                    this.Validate();
                }
            };

            // コントロールがロードされたときに内部のTextBoxにプロパティを設定
            this.Loaded += (sender, e) =>
            {
                // 内部のTextBoxにプロパティを反映
                PART_TextBox.TextType = this.TextType;
                PART_TextBox.DecimalPlaces = this.DecimalPlaces;
            };
        }
        #endregion

        #region 依存関係プロパティ - ラベル関連

        // ラベルテキスト
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledTextBox),
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
            DependencyProperty.Register("LabelWidth", typeof(GridLength), typeof(LabeledTextBox),
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
            DependencyProperty.Register("LabelFontSize", typeof(FontSizeType), typeof(LabeledTextBox),
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
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(LabeledTextBox),
                new PropertyMetadata(FontWeights.Normal)); // デフォルトは通常の太さ

        /// <summary>
        /// 項目ラベルのフォントの太さ
        /// </summary>
        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)this.GetValue(LabelFontWeightProperty); }
            set { this.SetValue(LabelFontWeightProperty, value); }
        }

        // 必須項目フラグ
        public static readonly DependencyProperty RequiredProperty =
            DependencyProperty.Register("Required", typeof(bool), typeof(LabeledTextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// 必須フラグ
        /// </summary>
        public bool Required
        {
            get { return (bool)this.GetValue(RequiredProperty); }
            set { this.SetValue(RequiredProperty, value); }
        }

        #endregion

        #region 依存関係プロパティ - テキストボックス関連

        // テキストボックスの値
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LabeledTextBox),
                new PropertyMetadata(string.Empty, OnTextChanged));

        /// <summary>
        /// テキストボックスの値
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// テキストボックス値変更時の処理
        /// </summary>
        /// <param name="d">依存関係プロパティを持つオブジェクト</param>
        /// <param name="e">イベント引数</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LabeledTextBox;
            control?.OnPropertyChanged("Text");
        }

        // 背景色
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(System.Windows.Media.Brush), typeof(LabeledTextBox),
                new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// テキストボックスの背景色
        /// </summary>
        public System.Windows.Media.Brush BackgroundColor
        {
            get { return (System.Windows.Media.Brush)this.GetValue(BackgroundColorProperty); }
            set { this.SetValue(BackgroundColorProperty, value); }
        }

        // テキストボックスを複数行として表示するかどうか
        public static readonly DependencyProperty IsMultilineProperty =
            DependencyProperty.Register("IsMultiline", typeof(bool), typeof(LabeledTextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// テキストボックスを複数行として表示するかどうか
        /// </summary>
        public bool IsMultiline
        {
            get { return (bool)this.GetValue(IsMultilineProperty); }
            set { this.SetValue(IsMultilineProperty, value); }
        }

        // テキストボックスの幅用の依存関係プロパティ
        public static readonly DependencyProperty TextBoxWidthProperty =
            DependencyProperty.Register("TextBoxWidth", typeof(double), typeof(LabeledTextBox),
                new PropertyMetadata(double.NaN)); // デフォルトはAuto幅

        /// <summary>
        /// テキストボックスの幅
        /// </summary>
        public double TextBoxWidth
        {
            get { return (double)this.GetValue(TextBoxWidthProperty); }
            set { this.SetValue(TextBoxWidthProperty, value); }
        }

        // テキストボックスの高さ
        public static readonly DependencyProperty TextBoxHeightProperty =
            DependencyProperty.Register("TextBoxHeight", typeof(double), typeof(LabeledTextBox),
                new PropertyMetadata(double.NaN)); // デフォルトはAutoサイズ

        /// <summary>
        /// テキストボックスの高さ（複数行モード時に使用）
        /// </summary>
        public double TextBoxHeight
        {
            get { return (double)this.GetValue(TextBoxHeightProperty); }
            set { this.SetValue(TextBoxHeightProperty, value); }
        }

        // テキストボックスの有効/無効状態を制御するプロパティ
        public static readonly DependencyProperty IsTextBoxEnabledProperty =
            DependencyProperty.Register("IsTextBoxEnabled", typeof(bool), typeof(LabeledTextBox),
                new PropertyMetadata(true)); // デフォルトは有効

        /// <summary>
        /// テキストボックスが有効かどうかを指定します
        /// </summary>
        public bool IsTextBoxEnabled
        {
            get { return (bool)this.GetValue(IsTextBoxEnabledProperty); }
            set { this.SetValue(IsTextBoxEnabledProperty, value); }
        }

        // FontSizeプロパティ
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(FontSizeType), typeof(LabeledTextBox),
                new PropertyMetadata(FontSizeType.Xs));

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public FontSizeType TextFontSize
        {
            get { return (FontSizeType)this.GetValue(TextFontSizeProperty); }
            set { this.SetValue(TextFontSizeProperty, value); }
        }

        // 最大文字数プロパティ
        public static readonly DependencyProperty MaxTextLengthProperty =
            DependencyProperty.Register("MaxTextLength", typeof(int), typeof(LabeledTextBox),
                new PropertyMetadata(0));

        /// <summary>
        /// 最大文字数
        /// </summary>
        public int MaxTextLength
        {
            get { return (int)this.GetValue(MaxTextLengthProperty); }
            set { this.SetValue(MaxTextLengthProperty, value); }
        }

        // テキストタイプのプロパティ
        public static readonly DependencyProperty TextTypeProperty =
            DependencyProperty.Register("TextType", typeof(TextType), typeof(LabeledTextBox),
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
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(LabeledTextBox),
                new PropertyMetadata(2, OnDecimalPlacesChanged));

        /// <summary>
        /// 数値タイプの場合の小数点以下の桁数
        /// </summary>
        public int DecimalPlaces
        {
            get { return (int)this.GetValue(DecimalPlacesProperty); }
            set { this.SetValue(DecimalPlacesProperty, value); }
        }

        /// <summary>
        /// テキストボックスの文字タイプ変更時処理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnTextTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LabeledTextBox labeledTextBox)
            {
                // 内部のTextBoxにプロパティを反映
                labeledTextBox.PART_TextBox.TextType = labeledTextBox.TextType;
            }
        }

        /// <summary>
        /// 小数点桁数変更時処理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LabeledTextBox labeledTextBox)
            {
                // 内部のTextBoxにプロパティを反映
                labeledTextBox.PART_TextBox.DecimalPlaces = labeledTextBox.DecimalPlaces;
            }
        }

        /// <summary>
        /// テキストボックスの内容を数値として取得
        /// </summary>
        /// <returns>数値、または無効な場合はnull</returns>
        public decimal? GetNumericValue()
        {
            return PART_TextBox.GetNumericValue();
        }
        #endregion

        #region 依存関係プロパティ - エラー関連

        // エラーフラグ
        public static readonly DependencyProperty HasErrorProperty =
           DependencyProperty.Register(
           nameof(HasError),
           typeof(bool),
           typeof(LabeledTextBox),
           new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// エラーフラグ
        /// </summary>
        public bool HasError
        {
            get { return (bool)this.GetValue(HasErrorProperty); }
            set { this.SetValue(HasErrorProperty, value); }
        }

        // エラーメッセージ
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(LabeledTextBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage
        {
            get { return (string)this.GetValue(ErrorMessageProperty); }
            set { this.SetValue(ErrorMessageProperty, value); }
        }

        // FontSizeプロパティ
        public static readonly DependencyProperty ErrorMessageFontSizeProperty =
            DependencyProperty.Register("ErrorMessageFontSize", typeof(FontSizeType), typeof(LabeledTextBox),
                new PropertyMetadata(FontSizeType.Xs));

        /// <summary>
        /// エラーメッセージのフォントサイズ
        /// </summary>
        public FontSizeType ErrorMessageFontSize
        {
            get { return (FontSizeType)this.GetValue(ErrorMessageFontSizeProperty); }
            set { this.SetValue(ErrorMessageFontSizeProperty, value); }
        }


        #endregion

        #region バリデーション

        // バリデーション実行フラグ
        private bool _validationEnabled = false;

        /// <summary>
        /// バリデーション実行フラグ
        /// </summary>
        public bool ValidationEnabled
        {
            get { return _validationEnabled; }
            set { _validationEnabled = value; }
        }

        // バリデーションデリゲートのコレクション
        private ObservableCollection<ValidationUtils.ValidationDelegate> _validationRules =
            new ObservableCollection<ValidationUtils.ValidationDelegate>();

        /// <summary>
        /// バリデーションルールのコレクション
        /// </summary>
        public ObservableCollection<ValidationUtils.ValidationDelegate> ValidationRules
        {
            get { return _validationRules; }
        }

        /// <summary>
        /// バリデーション用のデリゲート
        /// </summary>
        public delegate bool ValidateTextHandler(string text, out string errorMessage);

        /// <summary>
        /// バリデーション用のイベント
        /// </summary>
        public event ValidateTextHandler? ValidateText;

        /// <summary>
        /// バリデーションを実行するメソッド
        /// </summary>
        /// <returns>バリデーション結果（true=成功、false=エラー）</returns>
        public bool Validate()
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            // 必須が無効もしくはバリデーションが無効またはイベントハンドラが未登録の場合は成功とする
            if (!this.Required && (!this.ValidationEnabled || ValidateText == null))
            {
                this.HasError = !isValid;
                this.ErrorMessage = string.Empty;
                return isValid;
            }

            // 必須チェック
            if (this.Required && string.IsNullOrWhiteSpace(this.Text))
            {
                isValid = false;
                this.HasError = !isValid;
                this.ErrorMessage = "必須項目です";
            }

            //文字チェック
            if (isValid && this.PART_TextBox.TextType == TextType.String)
            {
                // 文字列の場合
                isValid = ValidationUtils.ValidateMaxLength(this.PART_TextBox.Text, this.PART_TextBox.MaxLength, out errorMessage);
                this.HasError = !isValid;
                this.ErrorMessage = errorMessage;
            }
            else if (isValid && this.PART_TextBox.TextType == TextType.Number)
            {
                // 数値の場合
                isValid = ValidationUtils.ValidateDecimalString(this.PART_TextBox.Text, this.PART_TextBox.MaxLength, this.DecimalPlaces, out errorMessage);
                this.HasError = !isValid;
                this.ErrorMessage = errorMessage;
            }

            // ValidationUtilSを使用したバリデーション
            if (isValid && this.ValidationRules.Count > 0)
            {
                isValid = ValidationUtils.ValidateAll(this.Text, out errorMessage, this.ValidationRules.ToArray());
                this.HasError = !isValid;
                this.ErrorMessage = errorMessage;
                return isValid;
            }
            // それ以外のカスタムバリデーション
            else if (isValid && ValidateText != null)
            {
                isValid = ValidateText(this.Text, out errorMessage);
                this.HasError = !isValid;
                this.ErrorMessage = errorMessage;
                return isValid;
            }

            return isValid;
        }

        #endregion
    }
}
