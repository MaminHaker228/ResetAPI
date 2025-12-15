using System.Collections.Generic;
using System.Threading.Tasks;
using ResetAPI.Domain.DTO;

namespace ResetAPI.Services.Steam
{
    public interface ISteamMarketService
    {
        Task<SkinDTO> GetSkinPriceAsync(int appId, string marketHashName);
        Task<List<SkinDTO>> SearchSkinsAsync(int appId, string searchQuery);
        Task<List<PriceHistoryDTO>> GetPriceHistoryAsync(int appId, string marketHashName);
    }
}
