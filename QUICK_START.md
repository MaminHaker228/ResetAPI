# RESET API - Quick Start Guide

## 30 Second Setup

### Step 1: Download
```bash
git clone https://github.com/MaminHaker228/ResetAPI.git
cd ResetAPI
```

### Step 2: Build
```bash
dotnet build -c Release
```

### Step 3: Run
```bash
cd ResetAPI.UI/bin/Release
ResetAPI.UI.exe
```

**Done!** Application is now running.

---

## First Steps

### Loading Skins
1. Click **"Load Popular"** button
2. Application fetches popular CS2/Dota 2 skins
3. List populates within 2-3 seconds

### Searching
1. Type in search box: "Phantom"
2. List filters in real-time
3. Shows matching items with prices

### Viewing Prices
1. Click any item in the list
2. Right panel shows selected skin info
3. Price chart updates with 30-day history
4. Hover over chart for detailed prices

### Filtering
1. Adjust price range slider (0 - $10,000)
2. List updates automatically
3. Switch games: CS2 ↔ Dota 2
4. Items re-sort by volume

### Switching Games
1. Click **CS2** or **Dota 2** radio button
2. Skins reload for selected game
3. Charts reset
4. Cache clears for fresh data

---

## System Requirements

- **Windows 7 SP1+** (7, 8, 10, 11)
- **.NET Framework 4.7.2** ([Download](https://dotnet.microsoft.com/download/dotnet-framework/net472))
- **Internet connection** (for Steam API)
- **512 MB RAM** (minimum)

---

## Troubleshooting

### Application Won't Start
```
Error: Unable to load required .NET Framework
→ Install .NET Framework 4.7.2
```

### Slow Search
```
Search/filter taking >1 second
→ Reduce cache entries in appsettings.json
```

### API Errors
```
Many failed requests in logs
→ Increase RequestDelayMs to 2000
```

### Missing Dependencies
```
Error: OxyPlot.Wpf.dll not found
→ Run: dotnet restore
```

---

## Configuration

Edit `appsettings.json` to customize:

```json
{
  "Steam": {
    "RequestDelayMs": 1000  // Increase if rate limited
  },
  "Cache": {
    "DurationMinutes": 5    // How long to cache prices
  },
  "Logging": {
    "LogLevel": "Information" // Debug/Info/Warning/Error
  }
}
```

---

## Logs

View application logs:
```bash
type logs/resetapi.log    # Windows
cat logs/resetapi.log     # Linux/Mac via WSL
tail -f logs/resetapi.log # Real-time monitoring
```

---

## Next Steps

- Read [ARCHITECTURE.md](docs/ARCHITECTURE.md) for technical details
- Check [API_INTEGRATION.md](docs/API_INTEGRATION.md) for Steam API info
- See [DEPLOYMENT.md](docs/DEPLOYMENT.md) for production setup
- Review [FEATURES.md](docs/FEATURES.md) for complete feature list

---

## Performance Tips

1. **Faster Searches**: Reduce cache TTL to 1-2 minutes
2. **Fewer API Calls**: Increase cache duration to 10-15 minutes
3. **Better Charts**: Load fewer days of history (7D instead of 30D)
4. **Offline Use**: Increase cache TTL to 60 minutes

---

## Getting Help

- **Bugs**: Report on [GitHub Issues](https://github.com/MaminHaker228/ResetAPI/issues)
- **Questions**: Check documentation in `/docs`
- **Logs**: Include `logs/resetapi.log` in bug reports

---

**Built with ❤️ for the Steam community**

*Version: 1.0.0 | Last Updated: December 2025*
