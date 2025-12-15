# Steam API Integration Guide

## Overview

RESET API integrates with Steam Community Market to fetch real-time skin prices and historical data.

## Steam Market Endpoints

### 1. Price Overview Endpoint

**URL**: `https://steamcommunity.com/market/priceoverview/`

**Method**: GET

**Parameters**:
- `appid` (int): Steam application ID
  - CS2: 730
  - Dota 2: 570
- `currency` (int): Currency code (1 = USD, 2 = GBP, etc.)
- `market_hash_name` (string): URL-encoded item name from Steam market

**Example Request**:
```
https://steamcommunity.com/market/priceoverview/?appid=730&currency=1&market_hash_name=AK-47%20%7C%20Phantom%20Disruptor
```

**Response Format**:
```json
{
  "success": true,
  "lowest_price": "$15.50",
  "median_price": "$15.75",
  "volume": "1523",
  "success_percent": 95.5
}
```

**Response Fields**:
- `success`: Boolean indicating successful request
- `lowest_price`: Current ask price (string with currency)
- `median_price`: Calculated median price (string with currency)
- `volume`: 24-hour trading volume (string)
- `success_percent`: Success rate of recent transactions (%)

## Rate Limiting

### Steam Market Limits
- **Default**: 1 request per second per IP
- **Burst**: Up to 5 requests in quick succession
- **Daily**: No documented daily limit
- **Behavior**: Returns 429 on rate limit (no backoff required)

### Implementation in RESET API
```csharp
// Configured in SteamMarketService
RequestDelayMs = 1000  // 1 second between requests
```

### Retry Policy
```
Attempt 1: Immediate
Attempt 2: 2 seconds later (exponential backoff)
Attempt 3: 4 seconds later
Attempt 4: Circuit breaker opens (30 second cooldown)
```

## Market Hash Names

Market hash names are Steam's unique identifier for items. They include:
- Item name
- Condition/wear (Factory New, Minimal Wear, etc.)
- Special attributes (★ for knives/gloves)

### Examples

**CS2**:
- `AK-47 | Phantom Disruptor (Factory New)`
- `M4A4 | Nightmare (Minimal Wear)`
- `★ Knife | Fade (Factory New)`
- `★ Gloves | Fade (Factory New)`

**Dota 2**:
- `Lina | Arcana (Perfect)`
- `Anti-Mage | Immortal (Perfect)`
- `Pudge | Mythical (Perfect)`

### Obtaining Hash Names
1. Visit item on Steam Market
2. Extract from URL: `...?market_hash_name=VALUE`
3. Or use Steam API search endpoints (third-party APIs)

## Price Parsing

Steam returns prices as formatted strings:
- Input: `"$15.50"`
- Parsing: Remove currency symbols and whitespace
- Conversion: Parse as decimal with InvariantCulture

```csharp
// Implementation
private decimal ParsePrice(string priceString)
{
    // "$15.50" → "15.50" → 15.50m
    var cleanPrice = Regex.Replace(priceString, @"[^\d.,]", "");
    cleanPrice = cleanPrice.Replace(",", ".");
    decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
    return price;
}
```

## Caching Strategy

### TTL (Time-To-Live)
- **Price Data**: 5 minutes
- **History Data**: 5 minutes
- **Search Results**: 1 minute

### Cache Keys
```
"skin_730_AK-47 | Phantom Disruptor (Factory New)"
"history_730_AK-47 | Phantom Disruptor (Factory New)"
```

### Eviction
- LRU (Least Recently Used) when max entries reached
- Automatic expiration after TTL
- Manual clear on game/filter change

## Error Handling

### Common Error Scenarios

#### 1. Invalid Market Hash Name
```json
{
  "success": false
}
```
Action: Log warning, return null, allow user retry

#### 2. Network Timeout
```
TaskCanceledException
```
Action: Retry with exponential backoff, max 3 attempts

#### 3. Circuit Breaker Open
```
BrokenCircuitException
```
Action: Return cached data if available, user-friendly message

#### 4. Rate Limited
```
HTTP 429 Too Many Requests
```
Action: Delay increased request delay, retry after cooldown

### HTTP Status Codes Handled
- `200`: Success ✓
- `400`: Bad request (invalid params) → Log & return null
- `429`: Rate limited → Retry with backoff
- `500`: Server error → Retry
- `503`: Service unavailable → Circuit breaker

## Price History Data

**Current Implementation**: Mock data generator
- Generates 30-day price history
- Realistic price movements (±50 cents daily)
- Random volume data (100-1000 units)

**Production Enhancement**:
- Steam doesn't provide historical endpoint
- Options:
  1. Scrape historical data from third-party APIs
  2. Build own database (collect daily snapshots)
  3. Use services like SteamAnalyst, CSGOFloat

## Testing Integration

### Unit Test Example
```csharp
[Fact]
public async Task GetSkinPrice_ValidHashName_ReturnsDTO()
{
    // Arrange
    var service = new SteamMarketService(_httpClient, _config, _cache);
    var hashName = "AK-47 | Phantom Disruptor (Factory New)";
    
    // Act
    var result = await service.GetSkinPriceAsync(730, hashName);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(hashName, result.MarketHashName);
    Assert.True(result.LowestPrice > 0);
}
```

### Mock HTTP Response
```csharp
var mockResponse = new HttpResponseMessage
{
    StatusCode = HttpStatusCode.OK,
    Content = new StringContent(
        @"{\"success\": true, \"lowest_price\": \"$15.50\", ...}"
    )
};
```

## Production Considerations

### Security
- Use HTTPS only (✓ configured)
- Validate all user input before passing to API
- Don't expose API keys in client (none needed)

### Performance
- Cache aggressively (5 min TTL)
- Batch requests where possible
- Use connection pooling (HttpClient)
- Implement circuit breaker (✓ Polly)

### Monitoring
- Log all API calls with timestamps
- Track cache hit/miss rates
- Monitor response times
- Alert on circuit breaker activation

### Limits
- Max 60 requests/minute (1 per second)
- Max 1000 cached items
- Max 30-day history per item
- Timeout: 30 seconds

## Future Enhancements

1. **Multi-Currency Support**
   - Store prices in USD
   - Convert on display based on user locale

2. **Historical Snapshots**
   - Daily background job to save prices
   - Database storage (SQLite)
   - Trend analysis (moving averages)

3. **Third-Party Integration**
   - SteamAnalyst API for historical data
   - Price prediction models
   - Arbitrage detection

4. **Advanced Filtering**
   - Float values (wear)
   - Sticker combinations (CS2)
   - Market supply/demand

5. **Real-Time Updates**
   - WebSocket integration
   - Push notifications for price changes
   - Portfolio tracking

---

*Last Updated: December 2025*
