using System;

namespace ResetAPI.Domain.Models
{
    /// <summary>
    /// Historical price data for a skin
    /// </summary>
    public class PriceHistory
    {
        public int Id { get; set; }
        public int SkinId { get; set; }
        public Skin Skin { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
