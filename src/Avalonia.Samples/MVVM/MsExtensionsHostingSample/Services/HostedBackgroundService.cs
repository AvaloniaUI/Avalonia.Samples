using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MsExtensionsHostingSample.Services;

public class HostedBackgroundService : IHostedService
{
    private readonly ILogger<HostedBackgroundService> _logger;

    public HostedBackgroundService(ILogger<HostedBackgroundService> logger)
    {
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BackgroundService started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BackgroundService ended.");
    }
}