using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using TestableApp.ViewModels;
using TestableApp.Views;
using Xunit;

namespace TestableApp.Headless.XUnit;

public class CalculatorTests
{
    [AvaloniaFact]
    public void Should_Add_Numbers()
    {
        var window = new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };

        window.Show();

        // Set values to the input boxes by simulating text input:
        window.FirstOperandInput.Focus();
        window.KeyTextInput("10");

        // Or directly to the control:
        window.SecondOperandInput.Text = "20";

        // Raise click event on the button:
        window.AddButton.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        Assert.Equal("30", window.ResultBox.Text);
    }
    
    [AvaloniaFact]
    public void Cannot_Divide_By_Zero()
    {
        var window = new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };

        window.Show();

        // Set values to the input boxes by simulating text input:
        window.SecondOperandInput.Text = "10";
        window.SecondOperandInput.Text = "0";

        // Raise click event on the button:
        window.DivideButton.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        Assert.Equal("Cannot divide by zero!", window.ResultBox.Text);
    }
}