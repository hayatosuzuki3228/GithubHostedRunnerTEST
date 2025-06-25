using System.Windows;

namespace Hutzper.Library.Wpf
{
    public class ComboBox : System.Windows.Controls.ComboBox
    {
        static ComboBox()
        {
            // デフォルトスタイルの指定
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
        }

        /// <summary>
        /// コンボボックスの選択状態をクリアします
        /// </summary>
        public void ClearSelection()
        {
            // null! を使用して警告を抑制
            this.SelectedItem = null!;
            this.SelectedValue = null!;
            this.Text = string.Empty;
            this.SelectedIndex = -1;
        }

    }
}
