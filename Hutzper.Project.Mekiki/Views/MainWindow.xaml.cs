using Hutzper.Project.Mekiki.ViewModels;
using System.Windows;

namespace Hutzper.Project.Mekiki;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = new MainViewModel();
        this.Loaded += this.OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is MainViewModel vm)
        {
            vm.OnLoaded();
        }
    }
}