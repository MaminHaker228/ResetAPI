using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ResetAPI.Infrastructure.Logging
{
    /// <summary>
    /// Configures structured logging with Serilog
    /// </summary>
    public class LoggerSetup
    {
        public static void Initialize(string logFilePath, string logLevel)
        {
            var level = Enum.Parse<LogEventLevel>(logLevel, ignoreCase: true);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(level)
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: logFilePath,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7
                )
                .CreateLogger();
        }
    }
}
