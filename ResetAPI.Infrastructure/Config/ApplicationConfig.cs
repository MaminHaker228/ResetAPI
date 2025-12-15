using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ResetAPI.Infrastructure.Config
{
    /// <summary>
    /// Application configuration loader
    /// </summary>
    public class ApplicationConfig
    {
        public SteamConfig Steam { get; set; }
        public CacheConfig Cache { get; set; }
        public LoggingConfig Logging { get; set; }
        public List<GameConfig> Games { get; set; }

        public static ApplicationConfig Load(string configPath = "appsettings.json")
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();

            var appConfig = new ApplicationConfig();
            config.GetSection("Steam").Bind(appConfig.Steam = new SteamConfig());
            config.GetSection("Cache").Bind(appConfig.Cache = new CacheConfig());
            config.GetSection("Logging").Bind(appConfig.Logging = new LoggingConfig());
            config.GetSection("Games").Bind(appConfig.Games = new List<GameConfig>());

            return appConfig;
        }
    }

    public class SteamConfig
    {
        public string BaseUrl { get; set; }
        public string MarketUrl { get; set; }
        public int RequestDelayMs { get; set; }
        public int TimeoutSeconds { get; set; }
    }

    public class CacheConfig
    {
        public int DurationMinutes { get; set; }
        public int MaxEntries { get; set; }
    }

    public class LoggingConfig
    {
        public string LogLevel { get; set; }
        public string LogFilePath { get; set; }
    }

    public class GameConfig
    {
        public string Name { get; set; }
        public int AppId { get; set; }
    }
}
