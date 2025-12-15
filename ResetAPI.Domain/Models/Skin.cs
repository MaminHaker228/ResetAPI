using System;

namespace ResetAPI.Domain.Models
{
    /// <summary>
    /// Represents a Steam skin/item in the market
    /// </summary>
    public class Skin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AppId { get; set; }
        public string GameName { get; set; }
        public string MarketHashName { get; set; }
        public decimal CurrentPrice { get; set; }
        public int Volume { get; set; }
        public decimal PriceChange24H { get; set; }
        public string Rarity { get; set; }
        public string Condition { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ImageUrl { get; set; }
        public string CommunityImageUrl { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
