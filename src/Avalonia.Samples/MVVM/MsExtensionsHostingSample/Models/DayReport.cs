using System;

namespace MsExtensionsHostingSample.Models;

public record DayReport(
    DateOnly Date,
    string WeatherCondition,
    double Temperature,
    string TemperatureUnit,
    double RelativeHumidity,
    double WindSpeed);