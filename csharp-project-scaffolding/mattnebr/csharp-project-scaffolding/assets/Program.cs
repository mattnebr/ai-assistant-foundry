namespace {{RootNamespace}};

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// The main entry point class for the application.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Get Configuration
                IConfiguration config = context.Configuration;

                // Install databases and services
                services
                    .InstallServices()
                    .AddHostedService<Worker>();
            })
            .Build();

        // Runs the hosted services until the app stops
        await host.RunAsync();
    }

    /// <summary>
    /// Installs the services required by the application.
    /// </summary>
    /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
    private static IServiceCollection InstallServices(this IServiceCollection services)
    {
        return services;
    }
}