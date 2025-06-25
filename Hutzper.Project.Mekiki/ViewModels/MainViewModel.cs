using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hutzper.Project.Mekiki.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private string _fieldstatusMessage = string.Empty;
    public string StatusMessage
    {
        get => this._fieldstatusMessage;
        set
        {
            if (this._fieldstatusMessage != value)
            {
                this._fieldstatusMessage = value;
                this.OnPropertyChanged();
            }
        }
    }

    public MainViewModel()
    {
        this.StatusMessage = "アプリ準備完了";
    }

    public void OnLoaded()
    {
        this.StatusMessage = "画面が読み込まれました";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}