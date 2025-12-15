# RESET API — Steam Skin Price Analyzer

## Overview

**RESET API** is a professional-grade desktop application for real-time analysis of Steam skin prices. Built with C# WPF, it provides advanced price tracking, interactive charts, and intelligent filtering for Counter-Strike 2 and Dota 2 skins.

### Key Features

- 🔍 **Real-Time Price Tracking**: Live data from Steam Market via official Steam Web API
- 📊 **Interactive Charts**: Professional price history visualization with zoom, scroll, and hover tooltips
- 🎮 **Multi-Game Support**: CS2 and Dota 2 skin analytics
- ⚡ **High Performance**: Async/await architecture with intelligent caching
- 🎨 **Modern UI**: Fluent Design System with dark mode support
- 🔧 **Production-Ready**: MVVM architecture, comprehensive error handling, structured logging

## Architecture

```
ResetAPI.sln
├── ResetAPI.UI/               # WPF Presentation Layer
│   ├── Views/                 # XAML Views (no code-behind logic)
│   ├── ViewModels/            # ViewModel state management
│   ├── Styles/                # Shared styles and themes
│   ├── Resources/             # UI resources
│   └── App.xaml               # Application root
├── ResetAPI.Domain/           # Domain Models & DTOs
│   ├── Models/                # Business domain models
│   ├── DTO/                   # Data transfer objects
│   └── Enums/                 # Enumerations
├── ResetAPI.Services/         # Business Logic Services
│   ├── Steam/                 # Steam API integration
│   ├── Market/                # Market data services
│   ├── Cache/                 # Caching strategy
│   └── Charts/                # Chart data processing
└── ResetAPI.Infrastructure/   # Infrastructure Layer
    ├── Http/                  # HTTP client with retry policy
    ├── Config/                # Configuration management
    ├── Logging/               # Structured logging
    └── DI/                    # Dependency injection setup
```

## Technology Stack

| Layer | Technology |
|-------|------------|
| **Framework** | .NET Framework 4.7.2 |
| **UI** | WPF (Windows Presentation Foundation) |
| **Architecture** | MVVM (Model-View-ViewModel) |
| **Charts** | OxyPlot for WPF |
| **HTTP** | HttpClient with Polly for retry policies |
| **Logging** | Serilog (structured logging) |
| **Configuration** | appsettings.json |
| **DI** | Microsoft.Extensions.DependencyInjection |

## System Requirements

- **OS**: Windows 7 SP1 or later
- **.NET Framework**: 4.7.2 or higher
- **RAM**: 512 MB minimum
- **Internet**: Required for Steam Market data

## Building & Running

### Prerequisites

1. Install [Visual Studio 2019+](https://visualstudio.microsoft.com/) with .NET Framework 4.7.2 development tools
2. Clone the repository:
   ```bash
   git clone https://github.com/MaminHaker228/ResetAPI.git
   cd ResetAPI
   ```

### Build Instructions

1. Open `ResetAPI.sln` in Visual Studio
2. Restore NuGet packages: `Ctrl+Alt+L` → Restore
3. Build solution: `Ctrl+Shift+B`
4. Run application: `F5` or Debug → Start Debugging

### Build via Command Line

```bash
# Restore packages
dotnet restore

# Build
dotnet build -c Release

# Run
cd ResetAPI.UI/bin/Release
ResetAPI.UI.exe
```

## Configuration

### appsettings.json

Create `appsettings.json` in the application root:

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
    {
      "Name": "CS2",
      "AppId": 730
    },
    {
      "Name": "Dota 2",
      "AppId": 570
    }
  ]
}
```

## API Integration

### Steam Market Pricing

The application integrates with Steam Market's public endpoints:

- **Endpoint**: `https://steamcommunity.com/market/priceoverview/`
- **Parameters**: `appid`, `currency`, `market_hash_name`
- **Rate Limiting**: 1 request per second (configured in appsettings.json)
- **Caching**: 5-minute TTL on price data

### Data Flow

1. User searches for a skin
2. Service queries cache → if hit, return cached data
3. If miss → queue Steam API request
4. Receive JSON response with current price, volume, trend
5. Cache result with timestamp
6. Update ViewModel → WPF renders changes

## Usage

### Main Screen

1. **Game Selection**: Choose between CS2 or Dota 2 in the game selector
2. **Search**: Type skin name in the search box (real-time filtering)
3. **Filters**: 
   - Price range slider
   - Trend indicator (rising/stable/falling)
   - Sort by price/name/trend
4. **Price Chart**: Click any skin to view price history with interactive chart
5. **Chart Controls**:
   - Scroll wheel to zoom in/out
   - Drag to pan
   - Hover for detailed tooltip
   - Time interval selector (1H, 24H, 7D, 30D)

## Performance Characteristics

- **Startup Time**: < 2 seconds
- **UI Response**: < 100ms for filtering/search operations
- **API Calls**: Batched and cached, minimal network overhead
- **Memory Usage**: ~150 MB under normal operation
- **Max Concurrent Requests**: 5 (configurable)

## Error Handling

The application implements comprehensive error handling:

- **Network Failures**: Automatic retry with exponential backoff
- **API Timeouts**: Graceful fallback to cached data
- **Invalid Data**: Validation and normalization at DTO layer
- **User Feedback**: Non-intrusive notifications in UI

## Logging

Structured logging via Serilog:

```
logs/
├── resetapi.log          # Application events
├── resetapi-error.log    # Errors only
└── resetapi-api.log      # API communication
```

Logging levels: `Debug`, `Information`, `Warning`, `Error`, `Critical`

## Contributing

This is a professional portfolio project. For modifications:

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Commit with clear messages: `git commit -m "feat: add feature description"`
3. Push and create pull request

## License

MIT License — see [LICENSE](LICENSE) file for details.

## Project Structure

```
├── ResetAPI.UI.exe                      # Compiled application
├── .sln                                 # Visual Studio solution
├── ResetAPI.sln
├── src/
│   ├── ResetAPI.UI/
│   ├── ResetAPI.Domain/
│   ├── ResetAPI.Services/
│   └── ResetAPI.Infrastructure/
├── tests/
│   └── ResetAPI.Tests/
├── docs/
│   ├── ARCHITECTURE.md
│   └── API_INTEGRATION.md
└── README.md
```

## Support & Issues

For bug reports or feature requests, use GitHub Issues.

---

**Built with precision. Ready for production.**

*Last updated: December 2025*