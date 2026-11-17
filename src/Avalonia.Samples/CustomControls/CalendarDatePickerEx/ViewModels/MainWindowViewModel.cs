using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CalendarDatePickerEx.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the selected date.
    /// </summary>
    
    [ObservableProperty]
    public partial DateTime? SelectedDate {get; set;}
}