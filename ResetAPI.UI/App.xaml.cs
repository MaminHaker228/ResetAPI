using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ResetAPI.Infrastructure.Config;
using ResetAPI.Infrastructure.DI;
using ResetAPI.Infrastructure.Http;
using ResetAPI.Infrastructure.Logging;
using ResetAPI.Services.Cache;
using ResetAPI.Services.Market;
using ResetAPI.Services.Steam;
using ResetAPI.UI.ViewModels;
using ResetAPI.UI.Views;
using Serilog;

namespace ResetAPI.UI
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Load configuration
                var config = ApplicationConfig.Load("appsettings.json");

                // Setup logging
                LoggerSetup.Initialize(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.Logging?.LogFilePath ?? "logs/resetapi.log"),
                    config.Logging?.LogLevel ?? "Information"
                );

                Log.Information("RESET API Application Starting...");

                // Setup DI container
                var services = new ServiceCollection();

                // Register configuration
                services.AddSingleton(config.Steam);
                services.AddSingleton(config.Cache);

                // Register infrastructure
                services.AddInfrastructureServices(config.Steam.TimeoutSeconds, config.Steam.RequestDelayMs);

                // Register services
                services.AddSingleton(new CacheService(config.Cache.MaxEntries, config.Cache.DurationMinutes));
                services.AddSingleton<ISteamMarketService, SteamMarketService>();
                services.AddSingleton<IMarketDataService, MarketDataService>();

                // Register ViewModels
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();

                _serviceProvider = services.BuildServiceProvider();

                // Show main window
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

                Log.Information("RESET API Application Started Successfully");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal error during application startup");
                MessageBox.Show($"Application failed to start: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("RESET API Application Closing");
            _serviceProvider?.Dispose();
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
