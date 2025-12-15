# RESET API - Deployment Guide

## Prerequisites

- Windows 7 SP1 or later
- .NET Framework 4.7.2 Runtime installed
- Internet connection (for Steam Market API calls)

## Installation Steps

### Option 1: Standalone Executable

1. **Download Release Package**
   - Download from GitHub Releases page
   - Extract to desired directory: `C:\Program Files\ResetAPI\`

2. **First Run Setup**
   ```bash
   ResetAPI.UI.exe
   ```
   - Application creates `logs/` directory
   - Generates `appsettings.json` if missing
   - Initializes cache

3. **Verify Installation**
   - Main window loads within 2 seconds
   - No error messages in console
   - Status bar shows "Ready"

### Option 2: Build from Source

1. **Clone Repository**
   ```bash
   git clone https://github.com/MaminHaker228/ResetAPI.git
   cd ResetAPI
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore ResetAPI.sln
   ```

3. **Build Release**
   ```bash
   dotnet build -c Release
   ```

4. **Output Location**
   ```
   ResetAPI.UI/bin/Release/
   ```

5. **Run Application**
   ```bash
   ResetAPI.UI/bin/Release/ResetAPI.UI.exe
   ```

## Configuration

### appsettings.json

Place in the same directory as `ResetAPI.UI.exe`:

```json
{
  "Steam": {
    "BaseUrl": "https://api.steampowered.com",
    "MarketUrl": "https://steamcommunity.com/market",
    "RequestDelayMs": 1000,
    "TimeoutSeconds": 30
  },
  "Cache": {
    "DurationMinutes": 5,
    "MaxEntries": 1000
  },
  "Logging": {
    "LogLevel": "Information",
    "LogFilePath": "logs/resetapi.log"
  },
  "Games": [
    { "Name": "CS2", "AppId": 730 },
    { "Name": "Dota 2", "AppId": 570 }
  ]
}
```

### Customization Options

**Request Delay** (milliseconds)
- Default: 1000 (1 request/second)
- Increase to 2000 if rate limited
- Decrease to 500 if API supports it

**Cache Duration** (minutes)
- Default: 5 minutes
- Increase for offline scenarios
- Decrease for real-time accuracy

**Log Level**
- `Debug`: Verbose logging (development)
- `Information`: Normal operation (production)
- `Warning`: Only warnings and errors
- `Error`: Only errors
- `Critical`: Only critical failures

## Directory Structure

```
ResetAPI/
├── ResetAPI.UI.exe           # Main executable
├── ResetAPI.UI.dll           # UI assembly
├── ResetAPI.Domain.dll       # Domain models
├── ResetAPI.Services.dll     # Business logic
├── ResetAPI.Infrastructure.dll
├── appsettings.json          # Configuration
├── OxyPlot.Wpf.dll          # Chart library
├── Newtonsoft.Json.dll      # JSON parsing
├── Serilog.dll              # Logging
├── logs/
│   └── resetapi.log         # Application log file
└── [other supporting DLLs]
```

## Troubleshooting

### Application Won't Start

**Error**: "Unable to load required .NET Framework"
- **Solution**: Install .NET Framework 4.7.2 from Microsoft Download Center

**Error**: "Cannot load application configuration"
- **Solution**: Ensure `appsettings.json` is in application directory with valid JSON

### Slow Performance

**Issue**: Slow response when searching/filtering
- **Cause**: Large cache size or network latency
- **Solution**: Reduce `MaxEntries` in cache config

**Issue**: Slow chart rendering
- **Cause**: 30+ days of history data
- **Solution**: Limit history to 7 days or aggregate data points

### Rate Limiting

**Error**: Many failed API requests
- **Cause**: Too frequent requests to Steam API
- **Solution**: Increase `RequestDelayMs` to 2000 or higher

### Missing Dependencies

**Error**: "OxyPlot.Wpf.dll not found"
- **Solution**: Run `dotnet restore` again
- Or download from NuGet manually and copy to executable directory

## System Requirements

| Requirement | Minimum | Recommended |
|-------------|---------|-------------|
| OS | Windows 7 SP1 | Windows 10/11 |
| RAM | 512 MB | 2 GB |
| Disk Space | 50 MB | 100 MB |
| CPU | Intel Core i3 | Intel Core i5+ |
| Network | 512 Kbps | 1 Mbps+ |
| .NET Framework | 4.7.2 | 4.8+ |

## Monitoring

### Check Application Status

```bash
# View recent logs
type logs/resetapi.log | tail -20

# Monitor in real-time
tail -f logs/resetapi.log
```

### Common Log Messages

```
[Information] RESET API Application Starting...
[Information] Loaded 100 skins for game 730
[Debug] Cache hit: skin_730_AK-47
[Warning] HTTP request failed. Retry 1 after 1000ms
[Error] Error fetching skin price
[Information] RESET API Application Closing
```

## Upgrading

1. **Backup Configuration**
   ```bash
   copy appsettings.json appsettings.json.backup
   ```

2. **Download New Version**
   - Get latest release from GitHub

3. **Replace Executable**
   ```bash
   copy ResetAPI.UI.exe ResetAPI.UI.exe.old
   [Extract new ResetAPI.UI.exe]
   ```

4. **Verify**
   - Run application
   - Check version in title bar
   - Test core functionality

## Performance Tuning

### For High Volume

```json
{
  "Cache": {
    "MaxEntries": 5000,
    "DurationMinutes": 10
  }
}
```

### For Low Latency

```json
{
  "Steam": {
    "RequestDelayMs": 500,
    "TimeoutSeconds": 10
  },
  "Cache": {
    "DurationMinutes": 1
  }
}
```

### For Offline Usage

```json
{
  "Cache": {
    "DurationMinutes": 60,
    "MaxEntries": 10000
  }
}
```

## Uninstallation

1. Close application if running
2. Delete installation directory
3. (Optional) Remove .NET Framework 4.7.2 from Programs & Features

No registry entries or additional cleanup needed.

## Support

- **Issues**: Report on GitHub Issues page
- **Logs**: Include `logs/resetapi.log` with bug reports
- **Configuration**: Share sanitized `appsettings.json`

---

*Last Updated: December 2025*
