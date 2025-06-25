namespace Hutzper.Library.Wpf
{
    public static class SettingEnums
    {
        /// <summary>
        ///  ボタンカラータイプ
        /// </summary>
        public enum ColorType
        {
            Primary,
            Outline,
        }

        /// <summary>
        /// フォントサイズタイプ
        /// </summary>
        public enum FontSizeType
        {
            /// <summary>8pt - 最小フォントサイズ</summary>
            ThreeXs,

            /// <summary>12pt - 極小フォントサイズ</summary>
            TwoXs,

            /// <summary>16pt - 小さいフォントサイズ</summary>
            Xs,

            /// <summary>20pt - 小フォントサイズ</summary>
            Sm,

            /// <summary>24pt - 標準フォントサイズ</summary>
            Base,

            /// <summary>28pt - 大フォントサイズ</summary>
            Lg,

            /// <summary>32pt - 大きいフォントサイズ</summary>
            Xl,

            /// <summary>36pt - 特大フォントサイズ</summary>
            TwoXl,

            /// <summary>40pt - 極大フォントサイズ</summary>
            ThreeXl,

            /// <summary>44pt - 最大フォントサイズ</summary>
            FourXl
        }

        /// <summary>
        /// ボタンサイズタイプ
        /// </summary>
        public enum ButtonSizeType
        {
            Sm,
            Base,
            Lg,
        }

        // テキストタイプの列挙型
        public enum TextType
        {
            String,
            Number
        }
    }
}
