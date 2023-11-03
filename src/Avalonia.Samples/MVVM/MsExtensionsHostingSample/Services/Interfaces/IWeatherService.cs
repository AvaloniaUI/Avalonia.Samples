using System.Collections.Generic;
using System.Threading.Tasks;
using MsExtensionsHostingSample.Models;

namespace MsExtensionsHostingSample.Services.Interfaces;

public interface IWeatherService
{
    Task<IReadOnlyList<DayReport>> GetFiveDayTemperaturesAsync();
}