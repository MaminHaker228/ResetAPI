using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResetAPI.Domain.DTO;
using Serilog;

namespace ResetAPI.Services.Market
{
    /// <summary>
    /// Market data aggregation and filtering service
    /// </summary>
    public class MarketDataService : IMarketDataService
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<MarketDataService>();

        private static readonly List<SkinDTO> MockPopularSkinsCS2 = new()
        {
            new SkinDTO { Name = "AK-47 | Phantom Disruptor", MarketHashName = "AK-47 | Phantom Disruptor (Factory New)", LowestPrice = 15.50m, Volume = 1500 },
            new SkinDTO { Name = "M4A4 | Nightmare", MarketHashName = "M4A4 | Nightmare (Factory New)", LowestPrice = 12.30m, Volume = 1200 },
            new SkinDTO { Name = "AWP Dragon Lore", MarketHashName = "AWP Dragon Lore (Factory New)", LowestPrice = 8500.00m, Volume = 5 },
            new SkinDTO { Name = "Knife | Fade", MarketHashName = "★ Knife | Fade (Factory New)", LowestPrice = 350.00m, Volume = 50 },
            new SkinDTO { Name = "Gloves | Fade", MarketHashName = "★ Gloves | Fade (Factory New)", LowestPrice = 200.00m, Volume = 80 }
        };

        private static readonly List<SkinDTO> MockPopularSkinsDota2 = new()
        {
            new SkinDTO { Name = "Arcana | Lina", MarketHashName = "Lina | Arcana (Perfect)", LowestPrice = 45.00m, Volume = 300 },
            new SkinDTO { Name = "Immortal | Anti-Mage", MarketHashName = "Anti-Mage | Immortal (Perfect)", LowestPrice = 25.00m, Volume = 400 },
            new SkinDTO { Name = "Exotic | Sven", MarketHashName = "Sven | Exotic (Perfect)", LowestPrice = 12.00m, Volume = 600 },
            new SkinDTO { Name = "Mythical | Pudge", MarketHashName = "Pudge | Mythical (Perfect)", LowestPrice = 8.50m, Volume = 800 },
            new SkinDTO { Name = "Rare | Drow Ranger", MarketHashName = "Drow Ranger | Rare (Perfect)", LowestPrice = 5.20m, Volume = 1200 }
        };

        public async Task<List<SkinDTO>> GetPopularSkinsAsync(int appId)
        {
            Log.Information("Fetching popular skins for app {AppId}", appId);

            var skins = appId == 730 ? MockPopularSkinsCS2 : MockPopularSkinsDota2;
            return await Task.FromResult(new List<SkinDTO>(skins));
        }

        public async Task<List<SkinDTO>> FilterSkinsAsync(List<SkinDTO> skins, decimal minPrice, decimal maxPrice)
        {
            if (skins == null || skins.Count == 0)
                return new List<SkinDTO>();

            var filtered = skins
                .Where(s => s.LowestPrice >= minPrice && s.LowestPrice <= maxPrice)
                .OrderByDescending(s => s.Volume)
                .ToList();

            Log.Debug("Filtered {Count} skins from {Total} with price range {Min}-{Max}",
                filtered.Count, skins.Count, minPrice, maxPrice);

            return await Task.FromResult(filtered);
        }
    }
}
