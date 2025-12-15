using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Serilog;

namespace ResetAPI.Infrastructure.Http
{
    /// <summary>
    /// Wraps HttpClient with retry policy and circuit breaker
    /// </summary>
    public class HttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        private readonly IAsyncPolicy<HttpResponseMessage> _circuitBreakerPolicy;

        public HttpClientWrapper(int timeoutSeconds = 30, int requestDelayMs = 1000)
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };

            // Retry policy: exponential backoff
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                    onRetry: (outcome, timespan, attempt, context) =>
                    {
                        ILogger.Log.Warning("HTTP request failed. Retry {Attempt} after {Delay}ms",
                            attempt, timespan.TotalMilliseconds);
                    }
                );

            // Circuit breaker: 5 failures, 30-second break
            _circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
                .CircuitBreakerAsync<HttpResponseMessage>(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, duration) =>
                    {
                        ILogger.Log.Error("Circuit breaker opened for {Duration}ms", duration.TotalMilliseconds);
                    },
                    onReset: () =>
                    {
                        ILogger.Log.Information("Circuit breaker reset");
                    }
                );
        }

        /// <summary>
        /// Sends GET request with retry and circuit breaker policies
        /// </summary>
        public async Task<string> GetAsync(string url)
        {
            try
            {
                var policy = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);

                var response = await policy.ExecuteAsync(() => _httpClient.GetAsync(url));
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (BrokenCircuitException ex)
            {
                ILogger.Log.Error(ex, "Circuit breaker is open. Service temporarily unavailable");
                throw new HttpRequestException("Service temporarily unavailable due to repeated failures", ex);
            }
            catch (HttpRequestException ex)
            {
                ILogger.Log.Error(ex, "HTTP request failed after retries");
                throw;
            }
        }

        public void SetBaseAddress(string baseAddress)
        {
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        public void SetDefaultHeader(string name, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
