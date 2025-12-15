using System;

namespace ResetAPI.Domain.DTO
{
    public class PriceHistoryDTO
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
    }
}
