using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    /// <summary>
    /// LabeledRadioButtonGroup.xaml の相互作用ロジック
    /// ラベル付きラジオボタングループを提供します
    /// <!-- 横並びラジオボタングループの例 -->
    /// <local:LabeledRadioButtonGroup x:Name="genderGroup" 
    ///                           LabelText="性別" 
    ///                           LabelWidth="100"
    ///                           Orientation="Horizontal"/>
    /// <!-- 縦並びラジオボタングループの例 -->
    /// <local:LabeledRadioButtonGroup x:Name="categoryGroup" 
    ///                           LabelText="カテゴリ" 
    ///                           LabelWidth="100"
    ///                           NoInitialSelection="True"
    ///                           Orientation="Vertical"/>
    /// コードでの設定例
    /// genderGroup.AddItem("男性", "male", true);  // 初期選択
    /// genderGroup.AddItem("女性", "female");
    /// genderGroup.AddItem("その他", "other");
    ///
    /// categoryGroup.AddItem("食品", 1);
    /// categoryGroup.AddItem("衣類", 2);
    /// categoryGroup.AddItem("電化製品", 3);
    /// 選択変更イベントの処理
    /// genderGroup.SelectionChanged += (sender, e) => {
    /// var selected = (sender as LabeledRadioButtonGroup).SelectedValue;
    /// 
    /// </summary>
    public partial class LabeledRadioButtonGroup : UserControl
    {
        #region 初期化

        /// <summary>
        /// LabeledRadioButtonGroupコントロールを初期化します
        /// </summary>
        public LabeledRadioButtonGroup()
        {
            InitializeComponent();
            this.Items = new ObservableCollection<RadioButtonItem>();
        }

        #endregion

        #region 依存関係プロパティ

        // ラベルテキスト
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledRadioButtonGroup),
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
            DependencyProperty.Register("LabelWidth", typeof(GridLength), typeof(LabeledRadioButtonGroup),
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
            DependencyProperty.Register("LabelFontSize", typeof(FontSizeType), typeof(LabeledRadioButtonGroup),
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
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(LabeledRadioButtonGroup),
                new PropertyMetadata(FontWeights.Normal)); // デフォルトは通常の太さ

        /// <summary>
        /// 項目ラベルのフォントの太さ
        /// </summary>
        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)this.GetValue(LabelFontWeightProperty); }
            set { this.SetValue(LabelFontWeightProperty, value); }
        }

        // 配置方向（横/縦）
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(LabeledRadioButtonGroup),
                new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// ラジオボタンの配置方向
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        // 必須項目フラグ
        public static readonly DependencyProperty RequiredProperty =
            DependencyProperty.Register("Required", typeof(bool), typeof(LabeledRadioButtonGroup),
                new PropertyMetadata(false));

        /// <summary>
        /// 必須フラグ
        /// </summary>
        public bool Required
        {
            get { return (bool)this.GetValue(RequiredProperty); }
            set { this.SetValue(RequiredProperty, value); }
        }

        // 初期選択なしフラグ
        public static readonly DependencyProperty NoInitialSelectionProperty =
            DependencyProperty.Register("NoInitialSelection", typeof(bool), typeof(LabeledRadioButtonGroup),
                new PropertyMetadata(false));

        /// <summary>
        /// 初期表示時に選択なしにするかどうか
        /// </summary>
        public bool NoInitialSelection
        {
            get { return (bool)this.GetValue(NoInitialSelectionProperty); }
            set { this.SetValue(NoInitialSelectionProperty, value); }
        }

        // ラジオボタングループの有効/無効状態を制御するプロパティ
        public static readonly DependencyProperty IsRadioButtonGroupEnabledProperty =
            DependencyProperty.Register("IsRadioButtonGroupEnabled", typeof(bool), typeof(LabeledRadioButtonGroup),
                new PropertyMetadata(true)); // デフォルトは有効

        /// <summary>
        /// ラジオボタングループが有効かどうかを指定します
        /// </summary>
        public bool IsRadioButtonGroupEnabled
        {
            get { return (bool)this.GetValue(IsRadioButtonGroupEnabledProperty); }
            set { this.SetValue(IsRadioButtonGroupEnabledProperty, value); }
        }

        // 選択された値のための依存関係プロパティ
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                nameof(SelectedValue),
                typeof(string),
                typeof(LabeledRadioButtonGroup),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChanged));

        public string? SelectedValue
        {
            get => (string)this.GetValue(SelectedValueProperty);
            set => this.SetValue(SelectedValueProperty, value);
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LabeledRadioButtonGroup group)
            {
                group.UpdateSelectedRadioButton();
            }
        }

        // ラジオボタンの項目のための依存関係プロパティ
        public static readonly DependencyProperty RadioItemsSourceProperty =
            DependencyProperty.Register(
                nameof(RadioItemsSource),
                typeof(IEnumerable<RadioButtonItem>),
                typeof(LabeledRadioButtonGroup),
                new PropertyMetadata(null, OnRadioItemsSourceChanged));

        public IEnumerable<RadioButtonItem> RadioItemsSource
        {
            get => (IEnumerable<RadioButtonItem>)this.GetValue(RadioItemsSourceProperty);
            set => this.SetValue(RadioItemsSourceProperty, value);
        }

        private static void OnRadioItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LabeledRadioButtonGroup group)
            {
                group.UpdateItems();
            }
        }

        #endregion

        #region 依存関係プロパティ - エラー関連

        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register(
            nameof(HasError),
            typeof(bool),
            typeof(LabeledRadioButtonGroup),
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
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(LabeledRadioButtonGroup),
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
            DependencyProperty.Register("ErrorMessageFontSize", typeof(FontSizeType), typeof(LabeledRadioButtonGroup),
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

        #region プロパティとイベント

        /// <summary>
        /// ラジオボタンアイテムのコレクション
        /// </summary>
        public ObservableCollection<RadioButtonItem> Items { get; private set; }

        /// <summary>
        /// 選択変更イベント
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// ラジオボタンのアイテム追加メソッド
        /// </summary>
        /// <param name="text">表示テキスト</param>
        /// <param name="value">値</param>
        /// <param name="isSelected">選択されているか</param>
        public void AddItem(string text, object value, bool isSelected = false)
        {
            // 初期選択なしモードで、かつこれが選択されるアイテムなら選択状態をオフに
            if (this.NoInitialSelection && isSelected)
            {
                isSelected = false;
            }

            // 選択されるアイテムの場合、SelectedValueも設定
            if (isSelected)
            {
                this.SelectedValue = value?.ToString();
            }

            this.Items.Add(new RadioButtonItem
            {
                Text = text,
                Value = value?.ToString() ?? string.Empty,
                IsSelected = isSelected
            });
        }

        /// <summary>
        /// すべてのアイテムをクリア
        /// </summary>
        public void ClearItems()
        {
            this.Items.Clear();
            this.SelectedValue = null;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// 選択状態の更新
        /// </summary>
        private void UpdateSelectedRadioButton()
        {
            foreach (var item in this.Items)
            {
                item.IsSelected = item.Value == this.SelectedValue;
            }
        }

        /// <summary>
        /// ソースからアイテムを更新
        /// </summary>
        private void UpdateItems()
        {
            this.Items.Clear();

            if (this.RadioItemsSource == null) return;

            foreach (var item in this.RadioItemsSource)
            {
                this.Items.Add(item);
            }

            this.UpdateSelectedRadioButton();
        }

        /// <summary>
        /// ラジオボタン選択変更時の処理
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null)
            {
                // SelectedValueを更新
                this.SelectedValue = rb.Tag.ToString();

                // イベント発火
                this.RaiseSelectionChangedEvent();
            }
        }

        /// <summary>
        /// 選択変更イベントを発生させる
        /// </summary>
        private void RaiseSelectionChangedEvent()
        {
            var oldItem = this.Items.FirstOrDefault(i => i.IsSelected && i.Value != this.SelectedValue);
            var newItem = this.Items.FirstOrDefault(i => i.Value == this.SelectedValue);

            // SelectionChangedEventArgsにはRoutedEventが必要
            var args = new SelectionChangedEventArgs(
                Selector.SelectionChangedEvent,  // RoutedEventを指定
                oldItem != null ? new[] { oldItem } : Array.Empty<object>(),
                newItem != null ? new[] { newItem } : Array.Empty<object>()
            );

            SelectionChanged?.Invoke(this, args);
        }

        #endregion
    }

    #region ラジオボタン項目クラス

    /// <summary>
    /// ラジオボタンの項目を表すクラス
    /// </summary>
    public class RadioButtonItem : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        private string _value = string.Empty;
        private bool _isSelected;

        /// <summary>
        /// ラジオボタンに表示するテキスト
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                this.OnPropertyChanged(nameof(this.Text));
            }
        }

        /// <summary>
        /// ラジオボタン選択時に返される値
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                this.OnPropertyChanged(nameof(this.Value));
            }
        }

        /// <summary>
        /// ラジオボタンが選択されているかどうか
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                this.OnPropertyChanged(nameof(this.IsSelected));
            }
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
    }

    #endregion
}

