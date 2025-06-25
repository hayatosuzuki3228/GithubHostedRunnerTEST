using System.Globalization;
using System.Text.RegularExpressions;

namespace Hutzper.Library.Wpf.Utils
{
    /// <summary>
    /// バリデーションユーティリティクラス
    /// </summary>
    public static class ValidationUtils
    {
        #region 文字列バリデーション

        /// <summary>
        /// 文字列の最小長をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="minLength">最小文字数</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateMinLength(string text, int minLength, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || text.Length < minLength)
            {
                errorMessage = $"{minLength}文字以上で入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 文字列の最大長をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="maxLength">最大文字数</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateMaxLength(string text, int maxLength, out string errorMessage)
        {
            if (text != null && text.Length > maxLength)
            {
                errorMessage = $"{maxLength}文字以内で入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 半角数字のみかチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateDigitOnly(string text, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !Regex.IsMatch(text, @"^[0-9]+$"))
            {
                errorMessage = "半角数字のみで入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 半角英字のみかチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateAlphabeticOnly(string text, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !Regex.IsMatch(text, @"^[a-zA-Z]+$"))
            {
                errorMessage = "半角英字のみで入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 半角英数字のみかチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateAlphanumeric(string text, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !Regex.IsMatch(text, @"^[a-zA-Z0-9]+$"))
            {
                errorMessage = "半角英数字のみで入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 半角英数字記号のみかチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateAsciiOnly(string text, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !Regex.IsMatch(text, @"^[\x20-\x7E]+$"))
            {
                errorMessage = "半角英数字記号のみで入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 全角文字のみかチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateFullWidthOnly(string text, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || text.Any(c => !IsFullWidth(c)))
            {
                errorMessage = "全角文字のみで入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 文字が全角かどうかを判定します
        /// </summary>
        /// <param name="c">判定する文字</param>
        /// <returns>全角の場合はtrue</returns>
        private static bool IsFullWidth(char c)
        {
            // 全角文字の範囲をチェック
            return c >= '\u3000' && c <= '\u9FFF'    // CJK統合漢字、ひらがな、カタカナなど
                || c >= '\uFF00' && c <= '\uFFEF';   // 全角英数字、記号など
        }

        #endregion

        #region 数値バリデーション

        /// <summary>
        /// 数値として評価できるかチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateIsNumeric(string text, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                errorMessage = "有効な数値を入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 桁数をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="digits">桁数</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateDigitCount(string text, int digits, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !decimal.TryParse(text, out decimal value))
            {
                errorMessage = "有効な数値を入力してください。";
                return false;
            }

            // 小数点を除いた桁数をチェック
            string digitsOnly = text.Replace(".", "").Replace("-", "");
            if (digitsOnly.Length != digits)
            {
                errorMessage = $"{digits}桁で入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 小数点以下の桁数をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="decimals">小数点以下の桁数</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateDecimalPlaces(string text, int decimals, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !decimal.TryParse(text, out decimal value))
            {
                errorMessage = "有効な数値を入力してください。";
                return false;
            }

            // 小数点以下の桁数をチェック
            int decimalPlaces = 0;
            int dotIndex = text.IndexOf('.');
            if (dotIndex != -1)
            {
                decimalPlaces = text.Length - dotIndex - 1;
            }

            if (decimalPlaces > decimals)
            {
                errorMessage = $"小数点以下は{decimals}桁までで入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 最大値をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateMaxValue(string text, decimal maxValue, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !decimal.TryParse(text, out decimal value))
            {
                errorMessage = "有効な数値を入力してください。";
                return false;
            }

            if (value > maxValue)
            {
                errorMessage = $"{maxValue}以下の値を入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 最小値をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="minValue">最小値</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateMinValue(string text, decimal minValue, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !decimal.TryParse(text, out decimal value))
            {
                errorMessage = "有効な数値を入力してください。";
                return false;
            }

            if (value < minValue)
            {
                errorMessage = $"{minValue}以上の値を入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 数値範囲をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="minValue">最小値</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateValueRange(string text, decimal minValue, decimal maxValue, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !decimal.TryParse(text, out decimal value))
            {
                errorMessage = "有効な数値を入力してください。";
                return false;
            }

            if (value < minValue || value > maxValue)
            {
                errorMessage = $"{minValue}以上{maxValue}以下の値を入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        #region Decimal型バリデーション

        /// <summary>
        /// Decimal値の整数部分の桁数をチェックします
        /// </summary>
        /// <param name="value">検証するdecimal値</param>
        /// <param name="maxIntegerDigits">整数部分の最大桁数</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateIntegerDigits(decimal value, int maxIntegerDigits, out string errorMessage)
        {
            // 整数部分の桁数を取得
            int integerDigits = (int)Math.Floor(Math.Log10((double)Math.Abs(Math.Floor(value)))) + 1;

            // 値が0の場合は特別処理
            if (value == 0m)
            {
                integerDigits = 1;
            }

            if (integerDigits > maxIntegerDigits)
            {
                errorMessage = $"整数部分は{maxIntegerDigits}桁以内で入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Decimal値の小数部分の桁数をチェックします
        /// </summary>
        /// <param name="value">検証するdecimal値</param>
        /// <param name="maxFractionDigits">小数部分の最大桁数</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateFractionDigits(decimal value, int maxFractionDigits, out string errorMessage)
        {
            // 小数部分の桁数を取得
            int fractionDigits = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];

            if (fractionDigits > maxFractionDigits)
            {
                errorMessage = $"小数点以下は{maxFractionDigits}桁以内で入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Decimal値の全体の桁数と小数部分の桁数をチェックします
        /// </summary>
        /// <param name="value">検証するdecimal値</param>
        /// <param name="maxTotalDigits">全体の最大桁数</param>
        /// <param name="maxFractionDigits">小数部分の最大桁数</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateDecimalDigits(decimal value, int maxTotalDigits, int maxFractionDigits, out string errorMessage)
        {
            // 小数部分の桁数を取得
            int fractionDigits = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];

            // 整数部分の桁数を取得
            int integerDigits = (int)Math.Floor(Math.Log10((double)Math.Abs(Math.Floor(value)))) + 1;

            // 値が0の場合は特別処理
            if (value == 0m)
            {
                integerDigits = 1;
            }

            // 全体の桁数
            int totalDigits = integerDigits + fractionDigits;

            if (totalDigits > maxTotalDigits)
            {
                errorMessage = $"数値は全体で{maxTotalDigits}桁以内で入力してください。";
                return false;
            }

            if (fractionDigits > maxFractionDigits)
            {
                errorMessage = $"小数点以下は{maxFractionDigits}桁以内で入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 文字列をdecimalとして解析し、桁数制限をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="maxTotalDigits">全体の最大桁数</param>
        /// <param name="maxFractionDigits">小数部分の最大桁数</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateDecimalString(string text, int maxTotalDigits, int maxFractionDigits, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !decimal.TryParse(text, out decimal value))
            {
                errorMessage = "有効な数値を入力してください。";
                return false;
            }

            return ValidateDecimalDigits(value, maxTotalDigits, maxFractionDigits, out errorMessage);
        }

        #endregion

        #endregion

        #region 複合バリデーション

        // デリゲート型の定義を修正
        public delegate bool ValidationDelegate(string text, out string errorMessage);

        /// <summary>
        /// 複数のバリデーションを一括で行います
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <param name="validations">バリデーションデリゲートのリスト</param>
        /// <returns>検証結果（true=すべて成功、false=いずれか失敗）</returns>
        public static bool ValidateAll(string text, out string errorMessage, params ValidationDelegate[] validations)
        {
            errorMessage = string.Empty;

            foreach (var validation in validations)
            {
                string tempErrorMessage;
                if (!validation(text, out tempErrorMessage))
                {
                    errorMessage = tempErrorMessage;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// バリデーションデリゲートを結合して新しいバリデーションを作成します
        /// </summary>
        /// <param name="validations">結合するバリデーションデリゲートのリスト</param>
        /// <returns>結合されたバリデーションデリゲート</returns>
        public static ValidationDelegate CombineValidations(params ValidationDelegate[] validations)
        {
            return (string text, out string errorMessage) => ValidateAll(text, out errorMessage, validations);
        }

        #endregion

        #region メール・電話番号バリデーション

        /// <summary>
        /// メールアドレスの形式をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateEmail(string text, out string errorMessage)
        {
            // メールアドレスの正規表現パターン
            string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

            if (string.IsNullOrEmpty(text) || !Regex.IsMatch(text, pattern))
            {
                errorMessage = "有効なメールアドレスを入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 電話番号の形式をチェックします（ハイフンあり/なし両対応）
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidatePhoneNumber(string text, out string errorMessage)
        {
            // 日本の電話番号パターン（ハイフンあり/なし両対応）
            string pattern = @"^(0\d{1,4}-\d{1,4}-\d{4}|0\d{9,10})$";

            if (string.IsNullOrEmpty(text) || !Regex.IsMatch(text, pattern))
            {
                errorMessage = "有効な電話番号を入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        #endregion

        #region 日付・時刻バリデーション

        /// <summary>
        /// 日付形式をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="format">日付形式（例: "yyyy/MM/dd"）</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateDate(string text, string format, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                errorMessage = $"有効な日付を{format}形式で入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 日付の範囲をチェックします
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="format">日付形式（例: "yyyy/MM/dd"）</param>
        /// <param name="minDate">最小日付</param>
        /// <param name="maxDate">最大日付</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidateDateRange(string text, string format, DateTime minDate, DateTime maxDate, out string errorMessage)
        {
            if (string.IsNullOrEmpty(text) || !DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                errorMessage = $"有効な日付を{format}形式で入力してください。";
                return false;
            }

            if (date < minDate || date > maxDate)
            {
                errorMessage = $"{minDate.ToString(format)}から{maxDate.ToString(format)}までの日付を入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        #endregion

        #region 郵便番号バリデーション

        /// <summary>
        /// 日本の郵便番号形式をチェックします（ハイフンあり/なし両対応）
        /// </summary>
        /// <param name="text">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（失敗時）</param>
        /// <returns>検証結果（true=成功、false=失敗）</returns>
        public static bool ValidatePostalCode(string text, out string errorMessage)
        {
            // 日本の郵便番号パターン（ハイフンあり/なし両対応）
            string pattern = @"^(\d{3}-\d{4}|\d{7})$";

            if (string.IsNullOrEmpty(text) || !Regex.IsMatch(text, pattern))
            {
                errorMessage = "有効な郵便番号を入力してください。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        #endregion

        #region 使用例

        #region 使用例

        // 使用例:
        // LabeledTextBox コントロールでバリデーションを設定する方法
        /*
        public void SetupValidation()
        {
            // 英数字チェックを設定
            myTextBox.ValidationEnabled = true;
            myTextBox.ValidateText += (text, out string errorMessage) => 
                ValidationUtils.ValidateAlphanumeric(text, out errorMessage);

            // 複数のバリデーションを設定
            myAnotherTextBox.ValidationEnabled = true;
            myAnotherTextBox.ValidateText += (text, out string errorMessage) =>
                ValidationUtils.ValidateAll(
                    text, 
                    out errorMessage,
                    ValidationUtils.ValidateMinLength, // デリゲートをそのまま渡す
                    (t, out string e) => ValidationUtils.ValidateMaxLength(t, 20, out e),
                    ValidationUtils.ValidateAlphanumeric
                );

            // 数値範囲チェック
            myNumberBox.ValidationEnabled = true;
            myNumberBox.ValidateText += (text, out string errorMessage) =>
                ValidationUtils.ValidateValueRange(text, 1, 100, out errorMessage);
        }
        */

        #endregion


        #endregion
    }
}