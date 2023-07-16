using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using NUnit.Framework;
using TestableApp.ViewModels;
using TestableApp.Views;

namespace TestableApp.Headless.NUnit;

public class CalculatorTests
{
    [AvaloniaTest]
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
        window.KeyPress(Key.Enter, RawInputModifiers.None);

        Assert.That(window.ResultBox.Text, Is.EqualTo("30"));
    }

    [AvaloniaTest]
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
        window.KeyPress(Key.Enter, RawInputModifiers.None);

        Assert.That(window.ResultBox.Text, Is.EqualTo("Cannot divide by zero!"));
    }
}