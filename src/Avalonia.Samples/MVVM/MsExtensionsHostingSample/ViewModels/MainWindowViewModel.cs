using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MsExtensionsHostingSample.Services.Interfaces;
using ReactiveUI;

namespace MsExtensionsHostingSample.ViewModels;

public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
    public MainWindowViewModel(IWeatherService weatherService)
    {
        Reports = new ObservableCollection<DayReportViewModel>();
        RefreshReport = ReactiveCommand.CreateFromTask(async _ =>
        {
            var days = await weatherService.GetFiveDayTemperaturesAsync();
            Reports.Clear();
            foreach (var dayReport in days)
            {
                Reports.Add(new DayReportViewModel(dayReport));
            }
        });
        
        this.WhenActivated(disposable =>
        {
            RefreshReport.Execute().Subscribe().DisposeWith(disposable);
        });
    }
    
    public ReactiveCommand<Unit, Unit> RefreshReport { get; }
    
    public ObservableCollection<DayReportViewModel> Reports { get; }

    public ViewModelActivator Activator { get; } = new();
}