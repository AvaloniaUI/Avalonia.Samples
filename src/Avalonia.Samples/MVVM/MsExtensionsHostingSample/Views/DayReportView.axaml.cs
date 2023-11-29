using Avalonia.ReactiveUI;
using MsExtensionsHostingSample.ViewModels;

namespace MsExtensionsHostingSample.Views;

public partial class DayReportView : ReactiveUserControl<DayReportViewModel>
{
    public DayReportView()
    {
        InitializeComponent();
    }
}