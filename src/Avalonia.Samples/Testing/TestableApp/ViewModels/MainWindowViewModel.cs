using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestableApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private double? _firstOperand, _secondOperand;

    [ObservableProperty]
    private string? _result;

    [RelayCommand]
    private void Add()
    {
        Result = (FirstOperand + SecondOperand)?.ToString();
    }
    
    [RelayCommand]
    private void Subtract()
    {
        Result = (FirstOperand - SecondOperand)?.ToString();
    }
    
    [RelayCommand]
    private void Multiply()
    {
        Result = (FirstOperand * SecondOperand)?.ToString();
    }
    
    [RelayCommand]
    private void Divide()
    {
        Result = SecondOperand == 0 ? "Cannot divide by zero!" : (FirstOperand / SecondOperand)?.ToString();
    }
}
