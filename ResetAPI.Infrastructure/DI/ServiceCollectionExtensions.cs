using System;
using Microsoft.Extensions.DependencyInjection;
using ResetAPI.Infrastructure.Http;

namespace ResetAPI.Infrastructure.DI
{
    /// <summary>
    /// Dependency injection configuration extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, 
            int httpTimeoutSeconds = 30, 
            int requestDelayMs = 1000)
        {
            services.AddSingleton(_ => new HttpClientWrapper(httpTimeoutSeconds, requestDelayMs));
            return services;
        }
    }
}
