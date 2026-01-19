using Avalonia;
using Avalonia.Headless;
using AdvancedToDoList;

[assembly: AvaloniaTestApplication(typeof(AdvancedToDoListTests.TestAppBuilder))]

namespace AdvancedToDoListTests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
