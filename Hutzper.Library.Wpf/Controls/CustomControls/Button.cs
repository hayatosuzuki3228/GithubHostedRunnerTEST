using System.Windows;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    public class Button : System.Windows.Controls.Button
    {

        static Button()
        {
            // デフォルトスタイルの指定
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
        }

        // Color DependencyProperty
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(ColorType),
                typeof(Button),
                new PropertyMetadata(ColorType.Primary));

        public ColorType Color
        {
            get => (ColorType)this.GetValue(ColorProperty);
            set => this.SetValue(ColorProperty, value);
        }

        // フォントサイズ
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register(
                nameof(TextFontSize),
                typeof(FontSizeType),
                typeof(Button),
                new PropertyMetadata(FontSizeType.Xs));

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public FontSizeType TextFontSize
        {
            get => (FontSizeType)this.GetValue(TextFontSizeProperty);
            set => this.SetValue(TextFontSizeProperty, value);
        }

        // ボタンサイズ
        public static readonly DependencyProperty ButtonSizeProperty =
            DependencyProperty.Register(
                nameof(ButtonSize),
                typeof(ButtonSizeType),
                typeof(Button),
                new PropertyMetadata(ButtonSizeType.Base, OnButtonSizeChanged));

        /// <summary>
        /// ボタンサイズ
        /// </summary>
        public ButtonSizeType ButtonSize
        {
            get => (ButtonSizeType)this.GetValue(ButtonSizeProperty);
            set => this.SetValue(ButtonSizeProperty, value);
        }

        // ボタンサイズが変更されたときのコールバック
        private static void OnButtonSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                ButtonSizeType newSize = (ButtonSizeType)e.NewValue;

                // ボタンサイズに応じてフォントサイズを設定
                switch (newSize)
                {
                    case ButtonSizeType.Sm:
                        button.TextFontSize = FontSizeType.Xs;
                        break;
                    case ButtonSizeType.Base:
                        button.TextFontSize = FontSizeType.Xs;
                        break;
                    case ButtonSizeType.Lg:
                        button.TextFontSize = FontSizeType.Lg;
                        break;
                    default:
                        button.TextFontSize = FontSizeType.Xs;
                        break;
                }
            }
        }

        // 幅を自動調整するかどうかのプロパティを追加
        public static readonly DependencyProperty IsWidthAutoProperty =
            DependencyProperty.Register(
                nameof(IsWidthAuto),
                typeof(bool),
                typeof(Button),
                new PropertyMetadata(false));

        /// <summary>
        /// 幅を自動調整するかどうか
        /// </summary>
        public bool IsWidthAuto
        {
            get => (bool)this.GetValue(IsWidthAutoProperty);
            set => this.SetValue(IsWidthAutoProperty, value);
        }

        // 高さを自動調整するかどうかのプロパティを追加
        public static readonly DependencyProperty IsHeightAutoProperty =
            DependencyProperty.Register(
                nameof(IsHeightAuto),
                typeof(bool),
                typeof(Button),
                new PropertyMetadata(false));

        /// <summary>
        /// 高さを自動調整するかどうか
        /// </summary>
        public bool IsHeightAuto
        {
            get => (bool)this.GetValue(IsHeightAutoProperty);
            set => this.SetValue(IsHeightAutoProperty, value);
        }
    }
}

