using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    /// <summary>
    /// LabeledComboBox.xaml の相互作用ロジック
    /// ラベル付きコンボボックスを提供し、バリデーションや必須項目チェックをサポートします
    /// </summary>
    public partial class LabeledComboBox : UserControl, INotifyPropertyChanged
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
        /// LabeledComboBoxコントロールを初期化します
        /// </summary>
        public LabeledComboBox()
        {
            InitializeComponent();

            // 選択変更時にバリデーションを実行
            this.PropertyChanged += (sender, e) =>
            {
                if ((e.PropertyName == "SelectedItem" || e.PropertyName == "SelectedValue") && this.ValidationEnabled)
                {
                    this.Validate();
                }
            };
        }

        #endregion

        #region 依存関係プロパティ - ラベル関連

        // ラベルテキスト
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledComboBox),
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
            DependencyProperty.Register("LabelWidth", typeof(GridLength), typeof(LabeledComboBox),
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
            DependencyProperty.Register("LabelFontSize", typeof(FontSizeType), typeof(LabeledComboBox),
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
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(LabeledComboBox),
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
            DependencyProperty.Register("Required", typeof(bool), typeof(LabeledComboBox),
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

        #region 依存関係プロパティ - コンボボックス関連

        // コンボボックスの幅用の依存関係プロパティ
        public static readonly DependencyProperty ComboBoxWidthProperty =
            DependencyProperty.Register("ComboBoxWidth", typeof(double), typeof(LabeledComboBox),
                new PropertyMetadata(double.NaN)); // デフォルトはAuto幅

        /// <summary>
        /// コンボボックスの幅
        /// </summary>
        public double ComboBoxWidth
        {
            get { return (double)this.GetValue(ComboBoxWidthProperty); }
            set { this.SetValue(ComboBoxWidthProperty, value); }
        }

        // 背景色
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(System.Windows.Media.Brush), typeof(LabeledComboBox),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// コンボボックスの背景色
        /// </summary>
        public System.Windows.Media.Brush BackgroundColor
        {
            get { return (System.Windows.Media.Brush)this.GetValue(BackgroundColorProperty); }
            set { this.SetValue(BackgroundColorProperty, value); }
        }

        // ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(LabeledComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// コンボボックスのアイテムソース
        /// </summary>
        public object ItemsSource
        {
            get { return this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        // SelectedItem
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(LabeledComboBox),
                new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// 選択されたアイテム
        /// </summary>
        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 選択アイテム変更時の処理
        /// </summary>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LabeledComboBox;
            control?.OnPropertyChanged("SelectedItem");
        }

        // SelectedValue
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(LabeledComboBox),
                new PropertyMetadata(null, OnSelectedValueChanged));

        /// <summary>
        /// 選択された値
        /// </summary>
        public object SelectedValue
        {
            get { return this.GetValue(SelectedValueProperty); }
            set { this.SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// 選択値変更時の処理
        /// </summary>
        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LabeledComboBox;
            control?.OnPropertyChanged("SelectedValue");
        }

        // SelectedValuePath
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(LabeledComboBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 選択値のパス
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)this.GetValue(SelectedValuePathProperty); }
            set { this.SetValue(SelectedValuePathProperty, value); }
        }

        // SelectedIndex
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(LabeledComboBox),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// 選択されたインデックス
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)this.GetValue(SelectedIndexProperty); }
            set { this.SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// 選択インデックス変更時の処理
        /// </summary>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LabeledComboBox;
            control?.OnPropertyChanged("SelectedIndex");
        }

        // DisplayMemberPath
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(LabeledComboBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 表示メンバーのパス
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)this.GetValue(DisplayMemberPathProperty); }
            set { this.SetValue(DisplayMemberPathProperty, value); }
        }

        // IsEditable
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(LabeledComboBox),
                new PropertyMetadata(false));

        /// <summary>
        /// コンボボックスが編集可能かどうか
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)this.GetValue(IsEditableProperty); }
            set { this.SetValue(IsEditableProperty, value); }
        }

        // Enabledプロパティの依存関係プロパティを定義
        public static readonly DependencyProperty IsComboBoxEnabledProperty =
            DependencyProperty.Register("IsComboBoxEnabled", typeof(bool), typeof(LabeledComboBox),
                new PropertyMetadata(true)); // デフォルトは有効

        /// <summary>
        /// コンボボックスが有効かどうかを指定します
        /// </summary>
        public bool IsComboBoxEnabled
        {
            get { return (bool)this.GetValue(IsComboBoxEnabledProperty); }
            set { this.SetValue(IsComboBoxEnabledProperty, value); }
        }

        #endregion

        #region 依存関係プロパティ - エラー関連

        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register(
            nameof(HasError),
            typeof(bool),
            typeof(LabeledComboBox),
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
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(LabeledComboBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage
        {
            get { return (string)this.GetValue(ErrorMessageProperty); }
            set { this.SetValue(ErrorMessageProperty, value); }
        }

        #endregion

        #region パブリックプロパティとメソッド

        /// <summary>
        /// コンボボックスのアイテムコレクション
        /// </summary>
        public ItemCollection? Items
        {
            get
            {
                // XAMLで定義した内部ComboBoxコントロールの参照を取得
                var comboBox = this.GetTemplateChild("InternalComboBox") as ComboBox;
                if (comboBox != null)
                {
                    return comboBox.Items;
                }

                // 内部ComboBoxが見つからない場合は、空のItemCollectionを返す代わりにnullを返す
                return null;
            }
        }

        /// <summary>
        /// 選択をクリアします
        /// </summary>
        public void ClearSelection()
        {
            // 内部ComboBoxのメソッドを使用（comboBoxは内部ComboBoxの参照と仮定）
            var comboBox = this.Template.FindName("comboBox", this) as ComboBox;
            comboBox?.ClearSelection();
        }

        /// <summary>
        /// テンプレート適用時に内部コントロールへの参照を取得
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // テンプレート適用後の初期化処理を行う場合はここに記述
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

        /// <summary>
        /// バリデーション用のデリゲート
        /// </summary>
        public delegate bool ValidateSelectionHandler(object selectedItem, object selectedValue, out string errorMessage);

        /// <summary>
        /// バリデーション用のイベント
        /// </summary>
        public event ValidateSelectionHandler? ValidateSelection;

        /// <summary>
        /// バリデーションを実行するメソッド
        /// </summary>
        /// <returns>バリデーション結果（true=成功、false=エラー）</returns>
        public bool Validate()
        {
            // バリデーションが無効またはイベントハンドラが未登録の場合は成功とする
            if (!this.ValidationEnabled || ValidateSelection == null)
            {
                this.HasError = false;
                this.ErrorMessage = string.Empty;
                return true;
            }

            // 必須チェック
            if (this.Required && this.SelectedItem == null)
            {
                this.HasError = true;
                this.ErrorMessage = "選択必須項目です";
                return false;
            }

            // カスタムバリデーション
            string errorMessage;
            bool isValid = ValidateSelection(this.SelectedItem, this.SelectedValue, out errorMessage);
            this.HasError = !isValid;
            this.ErrorMessage = errorMessage;
            return isValid;
        }

        #endregion
    }
}
