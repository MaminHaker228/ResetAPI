using System.Collections.Generic;
using System.Threading.Tasks;
using ResetAPI.Domain.DTO;

namespace ResetAPI.Services.Market
{
    public interface IMarketDataService
    {
        Task<List<SkinDTO>> GetPopularSkinsAsync(int appId);
        Task<List<SkinDTO>> FilterSkinsAsync(List<SkinDTO> skins, decimal minPrice, decimal maxPrice);
    }
}
