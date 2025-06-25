using System.Windows;
using System.Windows.Controls;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    /// <summary>
    /// LabeledToggleButton.xaml の相互作用ロジック
    /// ラベル付きトグルスイッチを提供するカスタムコントロール
    /// </summary>
    public partial class LabeledToggleButton : UserControl
    {
        #region 初期化

        /// <summary>
        /// LabeledToggleButtonコントロールを初期化します
        /// </summary>
        public LabeledToggleButton()
        {
            InitializeComponent();
        }

        #endregion

        #region 依存関係プロパティ - ラベル関連

        // ラベルテキスト用の依存関係プロパティ
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledToggleButton),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 項目ラベルのテキスト
        /// </summary>
        public string LabelText
        {
            get { return (string)this.GetValue(LabelTextProperty); }
            set { this.SetValue(LabelTextProperty, value); }
        }

        // ラベルの幅用の依存関係プロパティ
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(GridLength), typeof(LabeledToggleButton),
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
            DependencyProperty.Register("LabelFontSize", typeof(FontSizeType), typeof(LabeledToggleButton),
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
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(LabeledToggleButton),
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

        #region 依存関係プロパティ - トグルボタン関連

        // トグル状態用の依存関係プロパティ
        public static readonly DependencyProperty IsToggledProperty =
            DependencyProperty.Register("IsToggled", typeof(bool), typeof(LabeledToggleButton),
                new PropertyMetadata(false));

        /// <summary>
        /// トグルスイッチの状態（ON/OFF）
        /// </summary>
        public bool IsToggled
        {
            get { return (bool)this.GetValue(IsToggledProperty); }
            set { this.SetValue(IsToggledProperty, value); }
        }

        // ONテキスト用の依存関係プロパティ
        public static readonly DependencyProperty OnTextProperty =
            DependencyProperty.Register("OnText", typeof(string), typeof(LabeledToggleButton),
                new PropertyMetadata("ON"));

        /// <summary>
        /// トグルスイッチがON状態の時に表示するテキスト
        /// </summary>
        public string OnText
        {
            get { return (string)this.GetValue(OnTextProperty); }
            set { this.SetValue(OnTextProperty, value); }
        }

        // OFFテキスト用の依存関係プロパティ
        public static readonly DependencyProperty OffTextProperty =
            DependencyProperty.Register("OffText", typeof(string), typeof(LabeledToggleButton),
                new PropertyMetadata("OFF"));

        /// <summary>
        /// トグルスイッチがOFF状態の時に表示するテキスト
        /// </summary>
        public string OffText
        {
            get { return (string)this.GetValue(OffTextProperty); }
            set { this.SetValue(OffTextProperty, value); }
        }

        // トグルボタンの有効/無効状態を制御するプロパティ
        public static readonly DependencyProperty IsToggleButtonEnabledProperty =
            DependencyProperty.Register("IsToggleButtonEnabled", typeof(bool), typeof(LabeledToggleButton),
                new PropertyMetadata(true)); // デフォルトは有効

        /// <summary>
        /// トグルボタンが有効かどうかを指定します
        /// </summary>
        public bool IsToggleButtonEnabled
        {
            get { return (bool)this.GetValue(IsToggleButtonEnabledProperty); }
            set { this.SetValue(IsToggleButtonEnabledProperty, value); }
        }

        #endregion
    }
}
