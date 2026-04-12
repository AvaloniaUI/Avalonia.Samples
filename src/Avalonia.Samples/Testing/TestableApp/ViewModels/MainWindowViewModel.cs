using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestableApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    public partial double? FirstOperand { get; set; }
    
    [ObservableProperty]
    public partial double? SecondOperand { get; set; }
    
    [ObservableProperty]
    public partial string? Result { get; set; }

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
