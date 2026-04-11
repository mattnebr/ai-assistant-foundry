namespace {{RootNamespace}};

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background worker service that serves as the main execution entry point.
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IHostApplicationLifetime _lifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="Worker"/> class.
    /// </summary>
    /// <param name="loggerFactory">Logger factory for creating loggers.</param>
    /// <param name="lifetime">Host lifetime used to manage console window.</param>
    public Worker(
        ILoggerFactory loggerFactory,
        IHostApplicationLifetime lifetime)
    {
        this._loggerFactory = loggerFactory;
        this._lifetime = lifetime;
    }

    /// <summary>
    /// Entry point for a <see cref="BackgroundService"/>.
    /// </summary>
    /// <param name="stoppingToken">CancellationToken managed by the Host.</param>
    /// <returns>A unit of work representing when operation has been completed.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10, stoppingToken);
    }
}