using System;

using Avalonia;

namespace TrayIcon
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Console.WriteLine("Check the system notification area for a new icon");

			var app = BuildAvaloniaApp();

			app.StartWithClassicDesktopLifetime(args);
		}

		public static AppBuilder BuildAvaloniaApp()
		{
			return AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.WithInterFont()
				.LogToTrace();
		}
	}
}
