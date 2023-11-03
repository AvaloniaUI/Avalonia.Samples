using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using MsExtensionsHostingSample.ViewModels;

namespace MsExtensionsHostingSample;

// DesignData used for Previewer as a source of generated view models.
public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel { get; } =
        ((App)Application.Current!).GlobalHost!.Services.GetRequiredService<MainWindowViewModel>();
}