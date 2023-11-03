using System;
using MsExtensionsHostingSample.Models;

namespace MsExtensionsHostingSample.ViewModels;

public class DayReportViewModel : ViewModelBase
{
    public DayReportViewModel(DayReport dayReport)
    {
        Date = dayReport.Date;
        Temperature = dayReport.Temperature;
        TemperatureUnit = dayReport.TemperatureUnit;
        RelativeHumidity = dayReport.RelativeHumidity;
        WindSpeed = dayReport.WindSpeed;
        WeatherDescription = dayReport.WeatherCondition;
    }
    
    public DateOnly Date { get; }
    public double Temperature { get; }
    public string TemperatureUnit { get; }
    public double RelativeHumidity { get; }
    public double WindSpeed { get; }
    public object WeatherDescription { get; }
}