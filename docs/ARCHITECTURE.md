# RESET API - Architecture Documentation

## Overview

RESET API is built using a layered, domain-driven architecture with clear separation of concerns. The design ensures scalability, testability, and maintainability.

## Architectural Layers

### 1. Presentation Layer (ResetAPI.UI)

**Responsibility**: User interface and user interaction handling

- **Views**: XAML-based WPF windows and user controls
  - MainWindow: Primary application interface
  - Layout: Grid-based responsive design
  - Styles: Fluent Design System compliance

- **ViewModels**: Business logic for UI state management
  - MainViewModel: Handles skin data, filtering, search, chart generation
  - RelayCommand: MVVM command implementation
  - ViewModelBase: INotifyPropertyChanged base class

**Key Design Pattern**: MVVM (Model-View-ViewModel)

```
View (XAML)
  ↓ (Binds)
  ↓
ViewModel (C#)
  ↓ (Calls)
  ↓
Services & Domain Models
```

### 2. Domain Layer (ResetAPI.Domain)

**Responsibility**: Core business entities and value objects

- **Models**: 
  - `Skin`: Represents a market item
  - `PriceHistory`: Historical price data points
  - `Game`: Steam game reference

- **DTOs** (Data Transfer Objects):
  - `SkinDTO`: API response structure
  - `PriceHistoryDTO`: Price point structure
  - Decouples domain models from API responses

- **Enums**:
  - `GameType`: Supported games (CS2, Dota 2)
  - `PriceInterval`: Time range filters (1H, 24H, 7D, 30D)

### 3. Services Layer (ResetAPI.Services)

**Responsibility**: Business logic and data aggregation

#### Cache Service
- In-memory caching with TTL (Time-To-Live)
- Automatic eviction of oldest entries when max capacity reached
- Thread-safe using `ConcurrentDictionary`
- Configurable duration and max entries

#### Steam Market Service
- Direct integration with Steam Community Market API
- Endpoints used:
  - `https://steamcommunity.com/market/priceoverview/`
- Features:
  - Price parsing with currency normalization
  - Request rate limiting (1 request/second)
  - Automatic retry with exponential backoff
  - Cache layer for frequently accessed items

#### Market Data Service
- Aggregates skin data from multiple sources
- Filtering operations:
  - Price range filtering
  - Volume-based sorting
  - Game-specific filtering
- Mock data for demonstration (production would use real API)

### 4. Infrastructure Layer (ResetAPI.Infrastructure)

**Responsibility**: Technical implementation details

#### HTTP Client Wrapper
- Wraps `HttpClient` with advanced policies:
  - **Retry Policy**: Exponential backoff (up to 3 retries)
  - **Circuit Breaker**: Prevents cascading failures
  - **Timeout**: Configurable per request
- Thread-safe singleton pattern
- Automatic error logging

#### Configuration Management
- Loads from `appsettings.json`
- Structured config classes:
  - `SteamConfig`: API endpoints and timeouts
  - `CacheConfig`: Cache behavior
  - `LoggingConfig`: Log paths and levels
  - `GameConfig`: Supported games

#### Logging
- Serilog-based structured logging
- Sinks:
  - Console output (development)
  - File storage (production)
  - Rolling file policy (7-day retention)
- Configurable log levels

#### Dependency Injection
- Built on `Microsoft.Extensions.DependencyInjection`
- Singleton registration for stateless services
- Configured in `App.xaml.cs` during startup

## Data Flow

### 1. User Search Flow

```
UI Input (MainWindow.xaml)
  ↓
SearchQuery Property Changed
  ↓
MainViewModel.PerformSearch()
  ↓
LINQ Filtering on In-Memory Collection
  ↓
FilteredSkins ObservableCollection Updated
  ↓
WPF Binding Updates ListBox
  ↓
UI Rendered
```

### 2. Skin Details Flow

```
User Selects Skin (ListBox)
  ↓
SelectSkinCommand Executes
  ↓
MainViewModel.LoadSkinDetails()
  ↓
SteamMarketService.GetPriceHistoryAsync()
  ↓
Cache Check → Cache Hit?
  ├─ YES: Return cached data
  └─ NO: Fetch from API
      ↓
      Steam Market API Request
      ↓
      HttpClientWrapper (with retry)
      ↓
      JSON Parse & DTO Conversion
      ↓
      Cache Result (5 min TTL)
      ↓
      Return Data
  ↓
MainViewModel.BuildPriceChart()
  ↓
OxyPlot Model Created
  ↓
WPF PlotView Updates
  ↓
Chart Rendered
```

### 3. Filtering Flow

```
User Adjusts Price Slider
  ↓
MinPrice/MaxPrice Property Changed
  ↓
MainViewModel.ApplyFiltersAsync()
  ↓
MarketDataService.FilterSkinsAsync()
  ↓
LINQ Filter: price >= minPrice AND price <= maxPrice
  ↓
Order by Volume (descending)
  ↓
FilteredSkins Updated
  ↓
UI Re-rendered
```

## Thread Safety

### UI Thread
- All WPF operations on UI thread (enforced by dispatcher)
- Async/await preserves context

### Background Operations
- API calls on ThreadPool threads
- CacheService uses `ConcurrentDictionary` (thread-safe)
- HttpClientWrapper is singleton (thread-safe)

### Synchronization
- ObservableCollection bound to UI (dispatcher-aware)
- Property notifications on UI thread
- No explicit locks needed due to async patterns

## Error Handling Strategy

### Network Errors
```
HttpRequestException
  ↓
HttpClientWrapper.Retry Policy
  ↓
Exponential Backoff (2^n seconds)
  ↓
After 3 failures → Circuit Breaker
  ↓
Throw wrapped exception
  ↓
ViewModel catches & updates StatusMessage
  ↓
UI displays user-friendly error
```

### API Errors
- Invalid response: Parse DTO validation
- Timeout: Fallback to cached data
- Rate limiting: Configured request delays
- Invalid game ID: Enum validation

### Validation Points
- DTO property mapping
- Price parsing (currency format normalization)
- Cache key validation (null checks)
- Async cancellation token support (future enhancement)

## Performance Characteristics

### Memory Usage
- Cache: 1000 entries × avg 500 bytes = ~500 KB
- UI: ~50 MB (WPF framework overhead)
- Total: ~150-200 MB under normal operation

### Latency
- Cache hit: <1 ms
- Cache miss + API call: 1-3 seconds (with retry)
- Filtering 1000 items: <100 ms
- UI rendering: <50 ms

### Concurrency
- Max concurrent HTTP requests: 5 (Polly default)
- Cache operations: Lock-free (ConcurrentDictionary)
- UI updates: Sequential (dispatcher queue)

## Scalability Considerations

### Horizontal Scaling
- Stateless services (except cache)
- CacheService could be replaced with Redis
- HttpClientWrapper could use load balancer

### Vertical Scaling
- Increase cache max entries
- Increase HTTP timeout for slow connections
- Parallel LINQ for bulk filtering

### Future Enhancements
- Database persistence (SQLite/SQL Server)
- Background sync service
- Price prediction ML model
- Multi-user support
- Cloud API integration

## Security Considerations

### Current Implementation
- HTTPS for Steam API calls (configured URL)
- No authentication needed (public market data)
- Input validation at DTO level

### Recommended Additions
- Rate limiting per IP address
- User authentication (OAuth2 with Steam)
- Encryption of cached data at rest
- OWASP top 10 compliance review

## Testing Strategy

### Unit Tests
- Service layer logic (no UI dependencies)
- Cache behavior and TTL
- Price parsing edge cases

### Integration Tests
- Steam API responses (mocked)
- End-to-end data flow
- Error handling paths

### UI Tests
- ViewModel state transitions
- Command execution
- Binding updates

## Deployment

### Build
```bash
dotnet build -c Release
```

### Output
- `ResetAPI.UI.exe`: Standalone executable
- `appsettings.json`: Configuration file
- Supporting DLLs: OxyPlot, Serilog, Polly, etc.

### Runtime Requirements
- Windows 7 SP1 or later
- .NET Framework 4.7.2
- Internet connection (for Steam API)

## Monitoring & Diagnostics

### Logging Levels
- **Debug**: Cache hits/misses, filtering operations
- **Information**: Startup, skin loading, API calls
- **Warning**: Retries, deprecated API calls
- **Error**: Failed requests, parsing errors
- **Critical**: Unrecoverable failures

### Log Files
- `logs/resetapi.log`: All events
- Rolling: Daily rotation, 7-day retention
- Structured: JSON-serializable data fields

## Code Quality

### Design Patterns Used
- MVVM (Model-View-ViewModel)
- Repository (Services layer)
- Singleton (HttpClientWrapper, CacheService)
- Observer (INotifyPropertyChanged)
- Factory (ServiceCollection DI)
- Decorator (Polly retry/circuit breaker)

### SOLID Principles
- **S**ingle Responsibility: Each class has one reason to change
- **O**pen/Closed: Services implement interfaces (extendable)
- **L**iskov Substitution: Interface contracts respected
- **I**nterface Segregation: Focused interfaces (ISteamMarketService)
- **D**ependency Inversion: DI container for loose coupling

---

*Last Updated: December 2025*
