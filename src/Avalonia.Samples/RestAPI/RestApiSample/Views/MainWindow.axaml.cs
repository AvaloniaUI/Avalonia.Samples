using Avalonia.Controls;
using RestApiSample.ViewModels;

namespace RestApiSample.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void HandleWindowLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            await viewModel.LoadPokemonsAsync();
        }
    }
}
