# Contributing to RESET API

## Code Standards

### C# Style Guide
- Use **PascalCase** for public members
- Use **camelCase** for private members
- Use **_prefixed_camelCase** for private fields
- 4 spaces for indentation
- No trailing whitespace

### Naming Conventions
```csharp
public class SkinViewModel { }        // Classes
public string PropertyName { get; }   // Properties
private string _fieldName;            // Private fields
private void MethodName() { }         // Methods
public const string ConstantName = ""; // Constants
```

### Comments
- Use `///` for public API documentation
- Use `//` for code explanation
- Comments should explain WHY, not WHAT

```csharp
/// <summary>
/// Gets the current price for a skin from cache or API
/// </summary>
public async Task<decimal> GetPriceAsync(int appId, string name)
{
    // Try cache first to reduce API calls
    if (cache.TryGet(key, out var price))
        return price;
        
    // Cache miss - fetch from Steam
    return await apiClient.GetPriceAsync(appId, name);
}
```

## Architecture Guidelines

### Layering
- **UI Layer**: XAML + ViewModels only (no business logic)
- **Domain Layer**: Models, DTOs, Enums (no external dependencies)
- **Services Layer**: Business logic, interfaces, implementations
- **Infrastructure Layer**: HTTP, Config, Logging

### Dependency Injection
- Register services in `App.xaml.cs`
- Use interfaces for all dependencies
- Constructor injection only (no service locator)

```csharp
public class MyService
{
    private readonly IHttpClient _http;
    private readonly ILogger _logger;
    
    public MyService(IHttpClient http, ILogger logger)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

### Async/Await
- Always use `async/await` for I/O operations
- Never use `.Result` or `.Wait()`
- Propagate `CancellationToken` for cancellation support

```csharp
// ✅ Good
public async Task<Data> FetchDataAsync()
{
    return await _http.GetAsync("endpoint");
}

// ❌ Bad
public Data FetchData()
{
    return _http.GetAsync("endpoint").Result; // Deadlock risk
}
```

### Error Handling
- Use specific exception types
- Always log exceptions with context
- Provide meaningful error messages

```csharp
if (string.IsNullOrWhiteSpace(name))
    throw new ArgumentException("Skin name cannot be empty", nameof(name));

try
{
    var data = await FetchAsync();
}
catch (HttpRequestException ex)
{
    _logger.Error(ex, "Failed to fetch {Endpoint}", endpoint);
    throw;
}
```

## Pull Request Process

1. **Fork and Create Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Implement Feature**
   - Follow code standards
   - Add tests
   - Update documentation

3. **Commit Messages**
   - Use present tense: "Add feature" not "Added feature"
   - Reference issues: "Fix #123"
   - Keep messages concise

   ```bash
   git commit -m "feat: add price prediction model"
   git commit -m "fix: resolve rate limiting issue (fixes #456)"
   git commit -m "docs: update API integration guide"
   ```

4. **Push and Create PR**
   ```bash
   git push origin feature/your-feature-name
   ```
   - Provide clear PR description
   - Link related issues
   - Include screenshots if UI changes

5. **Code Review**
   - Address reviewer feedback
   - Keep commits clean
   - Rebase if needed: `git rebase main`

## Testing Guidelines

### Unit Tests
- Test service logic only
- Mock external dependencies
- Aim for >80% coverage

```csharp
[Fact]
public async Task GetPrice_ValidItem_ReturnsPriceDTO()
{
    // Arrange
    var mockHttp = new Mock<IHttpClient>();
    var service = new SteamService(mockHttp.Object);
    
    // Act
    var result = await service.GetPriceAsync(730, "AK-47");
    
    // Assert
    Assert.NotNull(result);
    Assert.True(result.Price > 0);
}
```

### Integration Tests
- Test service interactions
- Use real (or mocked) API responses
- Verify error handling

## Documentation

### Update These Files
- `README.md`: Feature or build process changes
- `docs/ARCHITECTURE.md`: Design changes
- `docs/API_INTEGRATION.md`: API-related changes
- Inline code comments for complex logic

### Code Documentation
```csharp
/// <summary>
/// Fetches and caches skin prices from Steam Market
/// </summary>
/// <param name="appId">Steam application ID (730 for CS2, 570 for Dota 2)</param>
/// <param name="marketHashName">URL-encoded item name</param>
/// <returns>Price DTO with current and median prices</returns>
/// <exception cref="HttpRequestException">If API call fails</exception>
public async Task<PriceDTO> GetSkinPriceAsync(int appId, string marketHashName)
```

## Common Issues

### Issue: Changes don't apply
**Solution**: Rebuild solution
```bash
dotnet clean
dotnet build -c Release
```

### Issue: Tests fail with "service not registered"
**Solution**: Check DI container in test setup
```csharp
var services = new ServiceCollection();
services.AddSingleton<IMyService, MyService>();
var provider = services.BuildServiceProvider();
```

### Issue: Async deadlock in tests
**Solution**: Use `.Result` only in tests, not production
```csharp
// Test only
var result = service.GetAsync().Result;
```

## Performance Considerations

### Before Optimizing
- Measure first (use Stopwatch)
- Profile with dotTrace or PerfView
- Identify bottleneck

### Common Optimizations
- Cache expensive operations
- Use async I/O
- Batch API requests
- Lazy loading for UI

## Security Review Checklist

- [ ] Input validation on all user inputs
- [ ] No hardcoded credentials or keys
- [ ] HTTPS used for external APIs
- [ ] SQL injection prevention (if adding DB)
- [ ] XSS prevention (if adding web)
- [ ] Secure error messages (no stack traces to users)
- [ ] Logging doesn't expose sensitive data

## Release Process

1. **Version Bump**
   ```bash
   # Update version in .csproj files
   # Version: 1.0.0 → 1.0.1 (patch), 1.1.0 (minor), 2.0.0 (major)
   ```

2. **Update Changelog**
   ```markdown
   ## [1.0.1] - 2025-12-15
   ### Added
   - New feature description
   ### Fixed
   - Bug fix description
   ```

3. **Create Tag and Release**
   ```bash
   git tag v1.0.1
   git push origin v1.0.1
   ```

4. **Build Release Package**
   ```bash
   dotnet publish -c Release -o release/
   ```

## Getting Help

- **Questions**: Open GitHub Discussion
- **Bugs**: Create Issue with:
  - Steps to reproduce
  - Expected behavior
  - Actual behavior
  - Logs from `logs/resetapi.log`
- **Ideas**: Start Discussion in Ideas category

## Code of Conduct

- Be respectful and inclusive
- No harassment or discrimination
- Constructive feedback only
- Focus on code quality, not personalities

---

**Thank you for contributing to RESET API!** 🎉
