using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Hutzper.Library.Wpf.Samples
{
    /// <summary>
    /// ControlsSampleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ControlsSampleWindow : Window
    {
        public ControlsSampleWindow()
        {
            InitializeComponent();
            this.SetupValidations();
        }

        private void SetupValidations()
        {
            // 年齢入力欄のバリデーション設定
            ValidationTextBox.ValidationEnabled = true;
            ValidationTextBox.ValidateText += ValidateAge;
        }

        private bool ValidateAge(string text, out string errorMessage)
        {
            // 数値チェック
            if (!int.TryParse(text, out int age))
            {
                errorMessage = "数値を入力してください";
                return false;
            }

            // 範囲チェック
            if (age < 0 || age > 120)
            {
                errorMessage = "0～120の範囲で入力してください";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private void ValidateAllButton_Click(object sender, RoutedEventArgs e)
        {
            // すべてのテキストボックスのバリデーションを実行
            bool isValid = true;

            if (RequiredTextBox.Text.Trim() == "")
            {
                RequiredTextBox.HasError = true;
                RequiredTextBox.ErrorMessage = "必須項目です";
                isValid = false;
            }
            else
            {
                RequiredTextBox.HasError = false;
                RequiredTextBox.ErrorMessage = string.Empty;
            }

            // バリデーション機能を持つコントロールのバリデーション実行
            if (!ValidationTextBox.Validate())
            {
                isValid = false;
            }

            if (isValid)
            {
                MessageBox.Show("すべての入力が正常です", "検証結果", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("入力エラーがあります。エラーメッセージを確認してください", "検証結果", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // すべてのテキストボックスをリセット
            BasicTextBox.Text = string.Empty;
            RequiredTextBox.Text = string.Empty;
            CustomBgTextBox.Text = string.Empty;
            ErrorTextBox.Text = string.Empty;
            WideLabelTextBox.Text = string.Empty;
            ValidationTextBox.Text = string.Empty;

            // エラー状態もリセット
            RequiredTextBox.HasError = false;
            RequiredTextBox.ErrorMessage = string.Empty;
            ErrorTextBox.HasError = false;
            ErrorTextBox.ErrorMessage = string.Empty;
            ValidationTextBox.HasError = false;
            ValidationTextBox.ErrorMessage = string.Empty;
        }

        private void ShowErrorButton_Click(object sender, RoutedEventArgs e)
        {
            // エラー表示のサンプル
            ErrorTextBox.HasError = true;
            ErrorTextBox.ErrorMessage = "エラー表示のサンプルです";

            ValidationTextBox.HasError = true;
            ValidationTextBox.ErrorMessage = "入力値が無効です";
        }

        private void ValidateTextAreasButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            // 必須チェック
            if (string.IsNullOrWhiteSpace(RequiredTextArea.Text))
            {
                RequiredTextArea.HasError = true;
                RequiredTextArea.ErrorMessage = "コメントは必須項目です";
                isValid = false;
            }
            else
            {
                RequiredTextArea.HasError = false;
                RequiredTextArea.ErrorMessage = string.Empty;
            }

            // 文字数チェック
            if (ErrorTextArea.Text.Length < 50)
            {
                ErrorTextArea.HasError = true;
                ErrorTextArea.ErrorMessage = $"現在{ErrorTextArea.Text.Length}文字です。50文字以上入力してください";
                isValid = false;
            }
            else
            {
                ErrorTextArea.HasError = false;
                ErrorTextArea.ErrorMessage = string.Empty;
            }

            if (isValid)
            {
                MessageBox.Show("すべてのテキストエリアが有効です", "検証結果", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearTextAreasButton_Click(object sender, RoutedEventArgs e)
        {
            BasicTextArea.Text = string.Empty;
            RequiredTextArea.Text = string.Empty;
            CustomBgTextArea.Text = string.Empty;
            ErrorTextArea.Text = string.Empty;

            // エラー状態のリセット
            RequiredTextArea.HasError = false;
            RequiredTextArea.ErrorMessage = string.Empty;
            ErrorTextArea.HasError = false;
            ErrorTextArea.ErrorMessage = string.Empty;
        }

        // 変更状態をリセットするボタンクリックイベント
        private void ResetModifiedButton_Click(object sender, RoutedEventArgs e)
        {
            // LabeledTextBoxの内部TextBoxに対してResetModifiedを呼び出す
            if (ContentChangeTextBox.TextBox is TextBox textBox1)
            {
                textBox1.ResetModified();
            }

            if (ContentChangeMultilineTextBox.TextBox is TextBox textBox2)
            {
                textBox2.ResetModified();
            }
        }

        // 初期値に戻すボタンクリックイベント
        private void SetDefaultTextButton_Click(object sender, RoutedEventArgs e)
        {
            ContentChangeTextBox.Text = "初期値";
            ContentChangeMultilineTextBox.Text = "複数行の初期テキスト\r\n2行目のテキスト";
        }

        // テキスト変更ボタンクリックイベント
        private void SetChangedTextButton_Click(object sender, RoutedEventArgs e)
        {
            ContentChangeTextBox.Text = "変更後のテキスト";
            ContentChangeMultilineTextBox.Text = "変更後の複数行テキスト\r\n変更後の2行目";
        }

        private void FormatTextButton_Click(object sender, RoutedEventArgs e)
        {
            // 各テキストボックスのフォーカスを強制的に外して
            // フォーマットを適用させる
            Keyboard.ClearFocus();

            // あるいは各テキストボックスのFormatTextメソッドを
            // 直接呼び出すことも可能（メソッドを公開している場合）
            MessageBox.Show("テキストがフォーマットされました。");
        }

        private void GetNumericValuesButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"2桁タイプの値: {NumberType2DecimalTextBox.GetNumericValue()?.ToString() ?? "無効な値"}");
            sb.AppendLine($"3桁タイプの値: {NumberType3DecimalTextBox.GetNumericValue()?.ToString() ?? "無効な値"}");
            sb.AppendLine($"整数タイプの値: {IntegerNumberTextBox.GetNumericValue()?.ToString() ?? "無効な値"}");
            MessageBox.Show(sb.ToString(), "数値取得結果");
        }

        private void ClearFormattedButton_Click(object sender, RoutedEventArgs e)
        {
            // テキストボックスをクリア
            NumberType2DecimalTextBox.Text = string.Empty;
            NumberType3DecimalTextBox.Text = string.Empty;
            EmptyNumberTextBox.Text = string.Empty;
            IntegerNumberTextBox.Text = string.Empty;
        }

    }
}
