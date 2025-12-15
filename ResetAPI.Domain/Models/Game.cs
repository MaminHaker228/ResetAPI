namespace ResetAPI.Domain.Models
{
    /// <summary>
    /// Represents a Steam game in the market
    /// </summary>
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AppId { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
