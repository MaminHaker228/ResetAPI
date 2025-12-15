namespace ResetAPI.Infrastructure.Http
{
    public static class ILogger
    {
        public static Serilog.ILogger Log { get; set; } = Serilog.Log.Logger;
    }
}
