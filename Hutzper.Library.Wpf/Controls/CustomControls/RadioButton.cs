using System.Windows;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{

    public class RadioButton : System.Windows.Controls.RadioButton
    {
        static RadioButton()
        {
            // デフォルトスタイルの指定
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioButton), new FrameworkPropertyMetadata(typeof(RadioButton)));
        }

        // フォントサイズ
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register(
                nameof(TextFontSize),
                typeof(FontSizeType),
                typeof(RadioButton),
                new PropertyMetadata(FontSizeType.Xs));

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public FontSizeType TextFontSize
        {
            get => (FontSizeType)this.GetValue(TextFontSizeProperty);
            set => this.SetValue(TextFontSizeProperty, value);
        }

    }
}
