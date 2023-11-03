using Avalonia.Controls;
using Avalonia.ReactiveUI;
using MsExtensionsHostingSample.ViewModels;

namespace MsExtensionsHostingSample.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
}