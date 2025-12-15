using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ResetAPI.Domain.DTO;
using ResetAPI.Infrastructure.Config;
using ResetAPI.Infrastructure.Http;
using Serilog;

namespace ResetAPI.Services.Steam
{
    /// <summary>
    /// Integration with Steam Community Market API
    /// </summary>
    public class SteamMarketService : ISteamMarketService
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly SteamConfig _config;
        private readonly Cache.CacheService _cacheService;
        private static readonly ILogger Log = Serilog.Log.ForContext<SteamMarketService>();

        public SteamMarketService(HttpClientWrapper httpClient, SteamConfig config, Cache.CacheService cacheService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        /// <summary>
        /// Gets current price for a specific skin
        /// </summary>
        public async Task<SkinDTO> GetSkinPriceAsync(int appId, string marketHashName)
        {
            if (string.IsNullOrWhiteSpace(marketHashName))
                throw new ArgumentException("Market hash name cannot be null or empty", nameof(marketHashName));

            var cacheKey = $"skin_{appId}_{marketHashName}";

            // Try cache first
            if (_cacheService.TryGet(cacheKey, out SkinDTO cached))
            {
                return cached;
            }

            try
            {
                // URL encode the market hash name
                var encodedName = Uri.EscapeDataString(marketHashName);
                var url = $"{_config.MarketUrl}/priceoverview/?appid={appId}&currency=1&market_hash_name={encodedName}";

                Log.Information("Fetching price for {Game} skin: {SkinName}", appId, marketHashName);
                var response = await _httpClient.GetAsync(url);

                // Rate limiting
                await Task.Delay(_config.RequestDelayMs);

                var json = JObject.Parse(response);

                if (json["success"]?.Value<bool>() != true)
                {
                    Log.Warning("Steam API returned unsuccessful response for {SkinName}", marketHashName);
                    return null;
                }

                var skinDto = new SkinDTO
                {
                    MarketHashName = marketHashName,
                    AppId = appId,
                    GameName = GetGameName(appId),
                    LowestPrice = ParsePrice(json["lowest_price"]?.Value<string>()),
                    MedianPrice = ParsePrice(json["median_price"]?.Value<string>()),
                    Volume = int.TryParse(json["volume"]?.Value<string>(), out var vol) ? vol : 0,
                    SuccessPercent = decimal.TryParse(json["success_percent"]?.Value<string>(), out var pct) ? pct : 0,
                    RequestTimestamp = DateTime.UtcNow
                };

                // Cache result
                _cacheService.Set(cacheKey, skinDto);

                Log.Debug("Successfully fetched price for {SkinName}: {Price}", marketHashName, skinDto.LowestPrice);
                return skinDto;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching skin price for {SkinName}", marketHashName);
                throw;
            }
        }

        /// <summary>
        /// Searches for skins by name (simulated - returns mock data as Steam doesn't provide search API)
        /// </summary>
        public async Task<List<SkinDTO>> SearchSkinsAsync(int appId, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return new List<SkinDTO>();

            // Note: Steam doesn't provide a search API, so this is a stub.
            // In production, you would scrape the market page or use a third-party API.
            return await Task.FromResult(new List<SkinDTO>());
        }

        /// <summary>
        /// Gets price history (simulated - requires scraping or external service)
        /// </summary>
        public async Task<List<PriceHistoryDTO>> GetPriceHistoryAsync(int appId, string marketHashName)
        {
            var cacheKey = $"history_{appId}_{marketHashName}";

            if (_cacheService.TryGet(cacheKey, out List<PriceHistoryDTO> cached))
            {
                return cached;
            }

            // Simulated history data
            var history = GenerateMockHistory();
            _cacheService.Set(cacheKey, history);

            return await Task.FromResult(history);
        }

        private decimal ParsePrice(string priceString)
        {
            if (string.IsNullOrWhiteSpace(priceString))
                return 0m;

            // Remove currency symbols and whitespace
            var cleanPrice = Regex.Replace(priceString, @"[^\d.,]", "").Trim();
            
            // Handle different decimal separators
            cleanPrice = cleanPrice.Replace(",", ".");

            if (decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                return price;
            }

            return 0m;
        }

        private string GetGameName(int appId)
        {
            return appId switch
            {
                730 => "CS2",
                570 => "Dota 2",
                _ => "Unknown"
            };
        }

        private List<PriceHistoryDTO> GenerateMockHistory()
        {
            var random = new Random();
            var history = new List<PriceHistoryDTO>();
            var basePrice = 10m + (decimal)random.NextDouble() * 100;

            for (int i = 30; i >= 0; i--)
            {
                basePrice += (decimal)(random.NextDouble() - 0.5) * 2;
                basePrice = Math.Max(0.5m, basePrice);

                history.Add(new PriceHistoryDTO
                {
                    Date = DateTime.UtcNow.AddDays(-i),
                    Price = Math.Round(basePrice, 2),
                    Volume = random.Next(100, 1000)
                });
            }

            return history;
        }
    }
}
