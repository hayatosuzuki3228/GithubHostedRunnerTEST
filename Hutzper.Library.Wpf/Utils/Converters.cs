using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf.Utils
{
    /// <summary>
    /// bool値をTextWrapping列挙型に変換するコンバーター
    /// </summary>
    public class BooleanToTextWrappingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TextWrapping)value == TextWrapping.Wrap;
        }
    }

    /// <summary>
    /// bool値をScrollBarVisibility列挙型に変換するコンバーター
    /// </summary>
    public class BooleanToScrollVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ScrollBarVisibility)value == ScrollBarVisibility.Auto;
        }
    }

    /// <summary>
    /// bool値を最小高さに変換するコンバーター
    /// </summary>
    public class BooleanToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 60.0 : double.NaN; // 複数行モードの場合は最小高さ60pxを設定
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value > 0;
        }
    }

    /// <summary>
    /// bool値をVisibility列挙型に変換するコンバーター
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// falseの場合に返すVisibility値（デフォルトはCollapsed）
        /// </summary>
        public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// 変換結果を反転するかどうか
        /// </summary>
        public bool Inverse { get; set; } = false;

        /// <summary>
        /// boolからVisibilityへの変換
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;

            // nullの場合はfalseとして扱う
            if (value != null)
            {
                boolValue = (bool)value;
            }

            // パラメータ指定がある場合は反転する
            if (parameter != null && parameter.ToString() == "Inverse")
            {
                boolValue = !boolValue;
            }

            // Inverse設定がtrueの場合は反転する
            if (this.Inverse)
            {
                boolValue = !boolValue;
            }

            return boolValue ? Visibility.Visible : this.FalseVisibility;
        }

        /// <summary>
        /// Visibilityからboolへの変換
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
            {
                return false;
            }

            bool result = (Visibility)value == Visibility.Visible;

            // パラメータ指定がある場合は反転する
            if (parameter != null && parameter.ToString() == "Inverse")
            {
                result = !result;
            }

            // Inverse設定がtrueの場合は反転する
            if (this.Inverse)
            {
                result = !result;
            }

            return result;
        }
    }
    /// <summary>
    /// Pointの配列に変換するコンバーター
    /// </summary>
    public class PointsConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // パラメータから解像度情報を取得
            float widthResolution = 1.0f;
            float heightResolution = 1.0f;

            if (parameter is string paramString)
            {
                var resParams = paramString.Split(',');
                if (resParams.Length == 2)
                {
                    float.TryParse(resParams[0], out widthResolution);
                    float.TryParse(resParams[1], out heightResolution);
                }
            }

            if (values.Length == 8 &&
                float.TryParse(values[0]?.ToString(), out float x1) &&
                float.TryParse(values[1]?.ToString(), out float y1) &&
                float.TryParse(values[2]?.ToString(), out float x2) &&
                float.TryParse(values[3]?.ToString(), out float y2) &&
                float.TryParse(values[4]?.ToString(), out float x3) &&
                float.TryParse(values[5]?.ToString(), out float y3) &&
                float.TryParse(values[6]?.ToString(), out float x4) &&
                float.TryParse(values[7]?.ToString(), out float y4))
            {
                return new PointCollection
            {
                new Point(x1 * widthResolution, y1 * heightResolution),
                new Point(x2 * widthResolution, y2 * heightResolution),
                new Point(x3 * widthResolution, y3 * heightResolution),
                new Point(x4 * widthResolution, y4 * heightResolution)
            };
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// FontSizeType列挙型を実際のフォントサイズに変換するコンバーター
    /// </summary>
    public class FontSizeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontSizeType sizeType)
            {
                // 列挙値に基づいてリソースキーを取得
                switch (sizeType)
                {
                    case FontSizeType.ThreeXs:
                        return Application.Current.Resources["3Xs"];
                    case FontSizeType.TwoXs:
                        return Application.Current.Resources["2xs"];
                    case FontSizeType.Xs:
                        return Application.Current.Resources["Xs"];
                    case FontSizeType.Sm:
                        return Application.Current.Resources["Sm"];
                    case FontSizeType.Base:
                        return Application.Current.Resources["Base"];
                    case FontSizeType.Lg:
                        return Application.Current.Resources["Lg"];
                    case FontSizeType.Xl:
                        return Application.Current.Resources["Xl"];
                    case FontSizeType.TwoXl:
                        return Application.Current.Resources["2xl"];
                    case FontSizeType.ThreeXl:
                        return Application.Current.Resources["3xl"];
                    case FontSizeType.FourXl:
                        return Application.Current.Resources["4xl"];
                    default:
                        return Application.Current.Resources["Base"]; // デフォルト値
                }
            }

            // 値がFontSizeType型でない場合はデフォルト値
            return Application.Current.Resources["Base"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 数値からFontSizeTypeへの変換（必要な場合）
            if (value is double fontSize)
            {
                if (fontSize <= 8) return FontSizeType.ThreeXs;
                if (fontSize <= 12) return FontSizeType.TwoXs;
                if (fontSize <= 16) return FontSizeType.Xs;
                if (fontSize <= 20) return FontSizeType.Sm;
                if (fontSize <= 24) return FontSizeType.Base;
                if (fontSize <= 28) return FontSizeType.Lg;
                if (fontSize <= 32) return FontSizeType.Xl;
                if (fontSize <= 36) return FontSizeType.TwoXl;
                if (fontSize <= 40) return FontSizeType.ThreeXl;
                return FontSizeType.FourXl;
            }

            // デフォルト値
            return FontSizeType.Base;
        }
    }

    /// <summary>
    /// ButtonSizeType列挙型を実際のボタンサイズに変換するコンバーター
    /// </summary>
    public class ButtonSizeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SettingEnums.ButtonSizeType sizeType)
            {
                // パラメーターに基づいてWidthまたはHeightを返す
                string? param = parameter as string;
                bool isWidth = param == "Width";

                Size size;
                switch (sizeType)
                {
                    case SettingEnums.ButtonSizeType.Sm:
                        size = (Size)Application.Current.FindResource("sm");
                        break;
                    case SettingEnums.ButtonSizeType.Base:
                        size = (Size)Application.Current.FindResource("base");
                        break;
                    case SettingEnums.ButtonSizeType.Lg:
                        size = (Size)Application.Current.FindResource("lg");
                        break;
                    default:
                        size = (Size)Application.Current.FindResource("base");
                        break;
                }

                return isWidth ? size.Width : size.Height;
            }
            return 96; // デフォルト値
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Widthから指定した値を引いた値を返すコンバーター
    /// </summary>
    public class WidthMinusStarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && parameter is string paramStr && double.TryParse(paramStr, out double subtract))
            {
                return Math.Max(0, width - subtract);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
