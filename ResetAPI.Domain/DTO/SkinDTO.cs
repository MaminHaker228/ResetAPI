using System;

namespace ResetAPI.Domain.DTO
{
    public class SkinDTO
    {
        public string Name { get; set; }
        public int AppId { get; set; }
        public string GameName { get; set; }
        public string MarketHashName { get; set; }
        public decimal LowestPrice { get; set; }
        public int Volume { get; set; }
        public decimal SuccessPercent { get; set; }
        public decimal MedianPrice { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public string ImageUrl { get; set; }
        public string RarityColor { get; set; }
        public decimal PriceChange24H { get; set; }
    }
}
