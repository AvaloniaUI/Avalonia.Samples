using Avalonia;
using Avalonia.Headless;
using SharedControlsTests;

[assembly: AvaloniaTestApplication(typeof(TestAppliction))]
namespace SharedControlsTests;

public class TestAppliction : Application
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestAppliction>()
         .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}