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

        void cmdClose_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
