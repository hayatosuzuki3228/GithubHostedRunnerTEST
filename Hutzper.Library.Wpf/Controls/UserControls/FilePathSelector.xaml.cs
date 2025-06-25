using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using static Hutzper.Library.Wpf.SettingEnums;

namespace Hutzper.Library.Wpf
{
    /// <summary>
    /// FilePathSelector.xaml の相互作用ロジック
    /// ファイル選択ダイアログを表示し、選択したファイルパスを管理するコントロール
    /// </summary>
    public partial class FileInputControl : UserControl
    {
        #region 初期化

        /// <summary>
        /// FileInputControlコントロールを初期化します
        /// </summary>
        public FileInputControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 依存関係プロパティ

        // ファイルパス用の依存関係プロパティ
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(FileInputControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 選択されたファイルパス
        /// </summary>
        public string FilePath
        {
            get { return (string)this.GetValue(FilePathProperty); }
            set { this.SetValue(FilePathProperty, value); }
        }

        // テキストボックスの幅用の依存関係プロパティ
        public static readonly DependencyProperty TextBoxWidthProperty =
            DependencyProperty.Register("TextBoxWidth", typeof(double), typeof(FileInputControl),
                new PropertyMetadata(double.NaN)); // デフォルトはAuto幅

        /// <summary>
        /// テキストボックスの幅
        /// </summary>
        public double TextBoxWidth
        {
            get { return (double)this.GetValue(TextBoxWidthProperty); }
            set { this.SetValue(TextBoxWidthProperty, value); }
        }

        // 初期ディレクトリ用の依存関係プロパティ
        public static readonly DependencyProperty InitialDirectoryProperty =
            DependencyProperty.Register("InitialDirectory", typeof(string), typeof(FileInputControl),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// OpenFileDialogで表示する初期ディレクトリのパス
        /// </summary>
        public string InitialDirectory
        {
            get { return (string)this.GetValue(InitialDirectoryProperty); }
            set { this.SetValue(InitialDirectoryProperty, value); }
        }

        // カスタムフィルター用の依存関係プロパティ
        public static readonly DependencyProperty CustomFilterProperty =
            DependencyProperty.Register("CustomFilter", typeof(string), typeof(FileInputControl),
                new PropertyMetadata("All files (*.*)|*.*"));

        /// <summary>
        /// OpenFileDialogで使用するカスタムフィルター (FileType.CUSTOMの場合に使用)
        /// 例: "テキストファイル (*.txt)|*.txt|CSVファイル (*.csv)|*.csv"
        /// </summary>
        public string CustomFilter
        {
            get { return (string)this.GetValue(CustomFilterProperty); }
            set { this.SetValue(CustomFilterProperty, value); }
        }

        // 複数ファイル選択用の依存関係プロパティ
        public static readonly DependencyProperty IsMultiselectProperty =
            DependencyProperty.Register("IsMultiselect", typeof(bool), typeof(FileInputControl),
                new PropertyMetadata(false));

        /// <summary>
        /// 複数ファイルの選択を許可するかどうか
        /// </summary>
        public bool IsMultiselect
        {
            get { return (bool)this.GetValue(IsMultiselectProperty); }
            set { this.SetValue(IsMultiselectProperty, value); }
        }

        // ラベルテキスト用の依存関係プロパティ
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(FileInputControl),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 項目ラベルのテキスト
        /// </summary>
        public string LabelText
        {
            get { return (string)this.GetValue(LabelTextProperty); }
            set { this.SetValue(LabelTextProperty, value); }
        }

        // ラベル幅用の依存関係プロパティ
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(GridLength), typeof(FileInputControl),
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
            DependencyProperty.Register("LabelFontSize", typeof(FontSizeType), typeof(FileInputControl),
                new PropertyMetadata(FontSizeType.Xs)); // デフォルトサイズを12に設定

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
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(FileInputControl),
                new PropertyMetadata(FontWeights.Normal)); // デフォルトは通常の太さ

        /// <summary>
        /// 項目ラベルのフォントの太さ
        /// </summary>
        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)this.GetValue(LabelFontWeightProperty); }
            set { this.SetValue(LabelFontWeightProperty, value); }
        }

        // 参照ボタンの有効/無効状態を制御するプロパティ
        public static readonly DependencyProperty IsBrowseButtonEnabledProperty =
            DependencyProperty.Register("IsBrowseButtonEnabled", typeof(bool), typeof(FileInputControl),
                new PropertyMetadata(true)); // デフォルトは有効

        /// <summary>
        /// 参照ボタンが有効かどうかを示す値を取得または設定します
        /// </summary>
        public bool IsBrowseButtonEnabled
        {
            get { return (bool)this.GetValue(IsBrowseButtonEnabledProperty); }
            set { this.SetValue(IsBrowseButtonEnabledProperty, value); }
        }

        // フォントサイズ用の依存関係プロパティ
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(FontSizeType), typeof(FileInputControl),
                new PropertyMetadata(FontSizeType.Xs)); // デフォルトサイズを12に設定

        /// <summary>
        /// 項目ラベルのフォントサイズ
        /// </summary>
        public FontSizeType TextFontSize
        {
            get { return (FontSizeType)this.GetValue(TextFontSizeProperty); }
            set { this.SetValue(TextFontSizeProperty, value); }
        }
        #endregion

        #region プロパティとイベント

        /// <summary>
        /// 選択ファイルタイプ
        /// </summary>
        public FileType SelectFileType { get; set; } = FileType.ALL;

        /// <summary>
        /// 複数選択時に選択されたすべてのファイルパスを保持するプロパティ
        /// </summary>
        private string[] _selectedFiles = new string[0];

        /// <summary>
        /// 複数選択時のすべての選択ファイルパス
        /// </summary>
        public string[] SelectedFiles
        {
            get { return _selectedFiles; }
        }

        /// <summary>
        /// 複数ファイル選択時のイベント
        /// </summary>
        public delegate void FilesSelectedEventHandler(object sender, string[] files);

        /// <summary>
        /// ファイル選択完了時に発生するイベント
        /// </summary>
        public event FilesSelectedEventHandler? FilesSelectedEvent;

        #endregion

        #region イベントハンドラ

        /// <summary>
        /// 参照ボタンクリック時の処理
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // フィルターの種類を選択するためのプロパティやパラメータを用意
            string filter;

            // 条件に応じてフィルターを設定
            switch (this.SelectFileType)
            {
                case FileType.TOML:
                    filter = "TOML files (*.toml)|*.toml";
                    break;
                case FileType.Image:
                    filter = "Image files (*.jpeg;*.jpg;*.png)|*.jpeg;*.jpg;*.png";
                    break;
                case FileType.ONNX:
                    filter = "ONNX files (*.onnx)|*.onnx";
                    break;
                case FileType.CUSTOM:
                    filter = this.CustomFilter;
                    break;
                case FileType.ALL:
                default:
                    filter = "All files (*.*)|*.*";
                    break;
            }

            // フィルターが空の場合はデフォルトのフィルターを設定
            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = "All files (*.*)|*.*";
            }

            var dialog = new OpenFileDialog
            {
                Filter = filter,
                FilterIndex = 1,
                Multiselect = this.IsMultiselect
            };

            // 初期ディレクトリを設定
            if (!string.IsNullOrEmpty(this.InitialDirectory) && System.IO.Directory.Exists(this.InitialDirectory))
            {
                dialog.InitialDirectory = this.InitialDirectory;
            }

            if (dialog.ShowDialog() == true)
            {
                if (this.IsMultiselect && dialog.FileNames.Length > 0)
                {
                    // 複数ファイルが選択された場合
                    _selectedFiles = dialog.FileNames; // すべてのファイルパスを保存

                    // テキストボックスには選択されたファイル数を表示
                    if (_selectedFiles.Length == 1)
                    {
                        this.FilePath = _selectedFiles[0];
                        FilePathTextBox.Text = this.FilePath;
                    }
                    else
                    {
                        this.FilePath = _selectedFiles[0]; // 最初のファイルをFilePathプロパティに設定
                        FilePathTextBox.Text = $"{_selectedFiles.Length}個のファイルが選択されています";
                    }

                    // 選択変更イベントを発火
                    FilesSelectedEvent?.Invoke(this, _selectedFiles);
                }
                else
                {
                    // 単一ファイル選択時
                    this.FilePath = dialog.FileName;
                    FilePathTextBox.Text = this.FilePath;
                    _selectedFiles = new string[] { this.FilePath };
                }
            }
        }

        #endregion

        #region 列挙型

        /// <summary>
        /// 参照ファイル種類
        /// </summary>
        public enum FileType
        {
            /// <summary>TOMLファイルのみ</summary>
            TOML,
            /// <summary>画像ファイルのみ</summary>
            Image,
            /// <summary>ONNXファイルのみ</summary>
            ONNX,
            /// <summary>すべてのファイル</summary>
            ALL,
            /// <summary>カスタムフィルター設定</summary>
            CUSTOM
        }

        #endregion
    }
}
