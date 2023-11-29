using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MsExtensionsHostingSample.Models;
using MsExtensionsHostingSample.Services.Interfaces;

namespace MsExtensionsHostingSample.Services;

public class WeatherService : IWeatherService
{
    private readonly IOptions<WeatherSettings> _weatherSettings;

    public WeatherService(IOptions<WeatherSettings> weatherSettings)
    {
        _weatherSettings = weatherSettings;
    }

    public async Task<IReadOnlyList<DayReport>> GetFiveDayTemperaturesAsync()
    {
        await Task.Delay(100); // simulate async operation

        var weatherDataList = new List<DayReport>();

        for (int i = 0; i < 10; i++)
        {
            var weatherData = new DayReport(
                DateOnly.FromDateTime(DateTime.Today.AddDays(i)),
                GetRandomWeatherCondition(),
                ConvertTemperature(Random.Shared.Next(65, 80)),
                _weatherSettings.Value.Unit,
                Random.Shared.Next(40, 70),
                Random.Shared.Next(5, 20));

            weatherDataList.Add(weatherData);
        }

        return weatherDataList;
    }

    private string GetRandomWeatherCondition()
    {
        var weatherConditions = new[] { "Sunny", "Partly Cloudy", "Cloudy", "Rainy", "Windy" };
        return weatherConditions[Random.Shared.Next(weatherConditions.Length)];
    }
    
    private double ConvertTemperature(double baseInF)
    {
        if (_weatherSettings.Value.Unit.Equals("C", StringComparison.OrdinalIgnoreCase))
        {
            return (int)Math.Round((baseInF - 32) / 1.8);
        }

        return baseInF;
    }
}