using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Hutzper.Library.Wpf.Sample
{
    public class UserSettingsViewModel : INotifyPropertyChanged
    {

        // 個別のエラー状態
        private bool _hasGenderError;
        public bool HasGenderError
        {
            get => _hasGenderError;
            set
            {
                if (_hasGenderError != value)
                {
                    _hasGenderError = value;
                    this.OnPropertyChanged();
                    this.UpdateHasAnyErrors(); // 全体のエラー状態更新
                }
            }
        }

        private bool _hasNameError;
        public bool HasNameError
        {
            get => _hasNameError;
            set
            {
                if (_hasNameError != value)
                {
                    _hasNameError = value;
                    this.OnPropertyChanged();
                    this.UpdateHasAnyErrors();
                }
            }
        }

        private bool _hasEmailError;
        public bool HasEmailError
        {
            get => _hasEmailError;
            set
            {
                if (_hasEmailError != value)
                {
                    _hasEmailError = value;
                    this.OnPropertyChanged();
                    this.UpdateHasAnyErrors();
                }
            }
        }

        private bool _hasIntroError;
        public bool HasIntroError
        {
            get => _hasIntroError;
            set
            {
                if (_hasIntroError != value)
                {
                    _hasIntroError = value;
                    this.OnPropertyChanged();
                    this.UpdateHasAnyErrors();
                }
            }
        }

        private bool _hasDeptError;
        public bool HasDeptError
        {
            get => _hasDeptError;
            set
            {
                if (_hasDeptError != value)
                {
                    _hasDeptError = value;
                    this.OnPropertyChanged();
                    this.UpdateHasAnyErrors();
                }
            }
        }

        // 全体のエラー状態
        private bool _hasAnyErrors;
        public bool HasAnyErrors
        {
            get => _hasAnyErrors;
            private set
            {
                if (_hasAnyErrors != value)
                {
                    _hasAnyErrors = value;
                    this.OnPropertyChanged();
                    // SaveCommandのCanExecuteを更新
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        // 全体のエラー状態を更新するメソッド
        private void UpdateHasAnyErrors()
        {
            this.HasAnyErrors = this.HasGenderError || this.HasNameError /* || 他のエラー */ ;
        }

        // INotifyPropertyChangedの実装
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // プロパティの実装例
        private string? _name;
        public string? Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string? _selfIntroduction;
        public string? SelfIntroduction
        {
            get => _selfIntroduction;
            set
            {
                if (_selfIntroduction != value)
                {
                    _selfIntroduction = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string? _selectedDepartmentKey;
        public string? SelectedDepartmentKey
        {
            get => _selectedDepartmentKey;
            set
            {
                if (_selectedDepartmentKey != value)
                {
                    _selectedDepartmentKey = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string? _selectedGender;
        public string? SelectedGender
        {
            get => _selectedGender;
            set
            {
                if (_selectedGender != value)
                {
                    _selectedGender = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _isDarkModeEnabled;
        public bool IsDarkModeEnabled
        {
            get => _isDarkModeEnabled;
            set
            {
                if (_isDarkModeEnabled != value)
                {
                    _isDarkModeEnabled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _areNotificationsEnabled;
        public bool AreNotificationsEnabled
        {
            get => _areNotificationsEnabled;
            set
            {
                if (_areNotificationsEnabled != value)
                {
                    _areNotificationsEnabled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _isAutoUpdateEnabled;
        public bool IsAutoUpdateEnabled
        {
            get => _isAutoUpdateEnabled;
            set
            {
                if (_isAutoUpdateEnabled != value)
                {
                    _isAutoUpdateEnabled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        // アカウント情報（表示用）
        public string UserId { get; set; }
        public string AccountCreationDate { get; set; }
        public string LastLoginDate { get; set; }
        public string AccountStatus { get; set; }

        // 部署情報
        public ObservableCollection<KeyValuePair<string, string>> Departments { get; set; }

        // GenderItemsプロパティを追加
        private ObservableCollection<RadioButtonItem> _genderItems;
        public ObservableCollection<RadioButtonItem> GenderItems
        {
            get => _genderItems;
            set
            {
                _genderItems = value;
                this.OnPropertyChanged();
            }
        }

        // コマンド
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public UserSettingsViewModel()
        {
            // 初期値設定
            this.Name = "山田 太郎";
            this.Email = "yamada@example.com";
            this.SelfIntroduction = "新しい技術を学ぶことが好きです。趣味はプログラミングと読書です。";
            this.SelectedDepartmentKey = "dev";
            this.SelectedGender = "male";
            this.IsDarkModeEnabled = true;
            this.AreNotificationsEnabled = true;
            this.IsAutoUpdateEnabled = false;

            // アカウント情報
            this.UserId = "user_12345";
            this.AccountCreationDate = "2023年4月1日";
            this.LastLoginDate = "2023年10月15日 14:30";
            this.AccountStatus = "アクティブ";

            // 部署情報
            this.Departments = new ObservableCollection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("sales", "営業部"),
                new KeyValuePair<string, string>("dev", "開発部")
            };

            // 性別選択肢の初期化
            _genderItems = new ObservableCollection<RadioButtonItem>
            {
                new RadioButtonItem { Text = "男性", Value = "male", IsSelected = true },
                new RadioButtonItem { Text = "女性", Value = "female" },
                new RadioButtonItem { Text = "その他", Value = "other" }
            };

            // 選択された性別を設定
            this.SelectedGender = "male";

            // コマンドの実装
            this.CancelCommand = new RelayCommand(this.Cancel);
            // CanExecuteにHasAnyErrorsの否定を使用
            this.SaveCommand = new RelayCommand(this.Save, () => !this.HasAnyErrors);
        }

        private void Save()
        {
            // 保存処理の実装
            System.Windows.MessageBox.Show("設定を保存しました。");
        }

        private void Cancel()
        {
            // キャンセル処理の実装
            System.Windows.MessageBox.Show("操作をキャンセルしました。");
        }
    }

    // 簡易的なRelayCommand実装
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        public void Execute(object? parameter) => _execute();
    }
}
