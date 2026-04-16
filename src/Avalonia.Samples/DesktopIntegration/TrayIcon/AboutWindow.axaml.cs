using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TrayIcon
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        public void SetActivationMode(ActivationMode mode)
        {
            switch (mode)
            {
                case ActivationMode.Click: lblActivationMode.Content = "Activated by clicking"; break;
                case ActivationMode.MenuItem: lblActivationMode.Content = "Activated by selecting menu item"; break;
            }
        }

        void cmdClose_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
