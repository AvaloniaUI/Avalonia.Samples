using System.Globalization;
using System.Runtime.InteropServices;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Mac;
using OpenQA.Selenium.Appium.Windows;
using Xunit;

namespace TestableApp.Appium;

[CollectionDefinition("Default")]
public class DefaultCollection : ICollectionFixture<DefaultAppFixture>
{
}


public class DefaultAppFixture : IDisposable
{
    private const string TestAppPath =
        @"..\..\..\..\TestableApp\bin\Debug\net6.0\TestableApp.exe";

    private const string TestAppBundleId = "net.avaloniaui.testableApp";

    public DefaultAppFixture()
    {
        var options = new AppiumOptions();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ConfigureWin32Options(options);
            Session = new WindowsDriver(
                new Uri("http://127.0.0.1:4723"),
                options);

            // https://github.com/microsoft/WinAppDriver/issues/1025
            SetForegroundWindow(new IntPtr(int.Parse(
                Session.WindowHandles[0].Substring(2),
                NumberStyles.AllowHexSpecifier)));
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            ConfigureMacOptions(options);
            Session = new MacDriver(
                new Uri("http://127.0.0.1:4723/wd/hub"),
                options);
        }
        else
        {
            throw new NotSupportedException("Unsupported platform.");
        }
    }

    protected virtual void ConfigureWin32Options(AppiumOptions options)
    {
        var path = Path.GetFullPath(TestAppPath);
        options.App = path;
        options.PlatformName = MobilePlatform.Windows;
        options.DeviceName= "WindowsPC";
        // options.AddAdditionalCapability("appArguments", "--customArg");
    }

    protected virtual void ConfigureMacOptions(AppiumOptions options)
    {
        options.AddAdditionalOption("appium:bundleId", TestAppBundleId);
        options.PlatformName = MobilePlatform.MacOS;
        options.AutomationName = "mac2";
        options.AddAdditionalOption("appium:showServerLogs", true);
        // options.AddAdditionalCapability("appium:arguments", new[] { "--customArg" });
    }

    public AppiumDriver Session { get; }

    public void Dispose()
    {
        try
        {
            Session.Close();
        }
        catch
        {
            // Closing the session currently seems to crash the mac2 driver.
        }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
}
