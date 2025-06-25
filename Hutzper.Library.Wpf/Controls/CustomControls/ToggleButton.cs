using System.Windows;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton
    {
        static ToggleButton()
        {
            // デフォルトスタイルの指定
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton), new FrameworkPropertyMetadata(typeof(ToggleButton)));
        }

        // ONテキスト用の依存関係プロパティ
        public static readonly DependencyProperty OnTextProperty =
            DependencyProperty.Register("OnText", typeof(string), typeof(ToggleButton),
                new PropertyMetadata("ON"));

        // OFFテキスト用の依存関係プロパティ
        public static readonly DependencyProperty OffTextProperty =
            DependencyProperty.Register("OffText", typeof(string), typeof(ToggleButton),
                new PropertyMetadata("OFF"));

        // ONテキストのプロパティ
        public string OnText
        {
            get { return (string)this.GetValue(OnTextProperty); }
            set { this.SetValue(OnTextProperty, value); }
        }

        // OFFテキストのプロパティ
        public string OffText
        {
            get { return (string)this.GetValue(OffTextProperty); }
            set { this.SetValue(OffTextProperty, value); }
        }

        // OFFテキストのフォントサイズ
        public static readonly DependencyProperty OffTextFontSizeProperty =
            DependencyProperty.Register(
                nameof(OffTextFontSize),
                typeof(FontSizeType),
                typeof(Button),
                new PropertyMetadata(FontSizeType.Xs));

        // OFFテキストのフォントサイズ
        public FontSizeType OffTextFontSize
        {
            get => (FontSizeType)this.GetValue(OffTextFontSizeProperty);
            set => this.SetValue(OffTextFontSizeProperty, value);
        }

        // ONテキストのフォントサイズ
        public static readonly DependencyProperty OnTextFontSizeProperty =
            DependencyProperty.Register(
                nameof(OnTextFontSize),
                typeof(FontSizeType),
                typeof(ToggleButton),
                new PropertyMetadata(FontSizeType.Xs));

        // OFFテキストのフォントサイズ
        public FontSizeType OnTextFontSize
        {
            get => (FontSizeType)this.GetValue(OnTextFontSizeProperty);
            set => this.SetValue(OnTextFontSizeProperty, value);
        }
    }
}
