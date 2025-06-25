using System.Windows;
using System.Windows.Input;

namespace Hutzper.Library.Wpf.Sample
{
    public partial class UserSettingsWindow : Window
    {
        // ViewModelを取得するプロパティを追加
        private UserSettingsViewModel? ViewModel => this.DataContext is UserSettingsViewModel viewModel ? viewModel : null;

        public UserSettingsWindow()
        {
            InitializeComponent();

            // DataContextが未設定の場合は初期化する
            if (this.DataContext == null)
            {
                this.DataContext = new UserSettingsViewModel();
            }

            // イベントハンドラの追加
            imagePreview.PreviewImageMouseDown += ImagePreview_MouseDown;
            imagePreview.PreviewImageSizeChanged += ImagePreview_SizeChanged;
            imagePreview.PreviewImageMouseLeftButtonDown += ImagePreview_LeftButtonDown;
            imagePreview.PreviewImageMouseMove += ImagePreview_MouseMove;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void ImagePreview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 画像クリック時の処理
        }

        private void ImagePreview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // サイズ変更時の処理
        }

        private void ImagePreview_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 左ボタンクリック時の処理
        }

        private void ImagePreview_MouseMove(object sender, MouseEventArgs e)
        {
            // マウス移動時の処理
        }
    }
}