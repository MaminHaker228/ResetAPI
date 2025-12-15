# RESET API - Build Instructions

## System Requirements

### Minimum
- **OS**: Windows 7 SP1+
- **.NET**: .NET Framework 4.7.2 or .NET 6+ (if using SDK)
- **RAM**: 2 GB
- **Disk**: 500 MB free
- **Visual Studio**: 2019 or later (Community edition OK)

### Recommended
- **OS**: Windows 10/11
- **.NET**: .NET Framework 4.8+
- **RAM**: 8 GB+
- **Disk**: 2 GB SSD
- **Visual Studio**: 2022 Community/Professional

## Installation

### Step 1: Install .NET Framework 4.7.2

**Windows 10/11 (Already Installed)**
- Usually pre-installed
- Verify: Settings → Apps → Apps & features → Search ".NET"

**Windows 7/8**
1. Download from [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472)
2. Run installer
3. Restart computer
4. Verify: `dotnet --version` in PowerShell

### Step 2: Install Visual Studio

**Option A: Visual Studio 2022 Community (Recommended)**
1. Download from [visualstudio.com](https://visualstudio.microsoft.com/downloads/)
2. Run installer
3. Select "Desktop development with C++"
4. Ensure ".NET Framework 4.7.2" is checked
5. Complete installation (~5 GB)

**Option B: Visual Studio Code**
1. Install [VS Code](https://code.visualstudio.com/)
2. Install C# extension
3. Install .NET CLI tools

### Step 3: Clone Repository

**Using Git**
```bash
git clone https://github.com/MaminHaker228/ResetAPI.git
cd ResetAPI
```

**Using GitHub Desktop**
1. Open [GitHub Desktop](https://desktop.github.com/)
2. File → Clone Repository
3. Enter: `MaminHaker228/ResetAPI`
4. Choose local path
5. Click Clone

## Building

### Method 1: Visual Studio GUI (Easiest)

1. **Open Solution**
   - File → Open → Select `ResetAPI.sln`

2. **Restore NuGet Packages**
   - Tools → NuGet Package Manager → Manage Packages for Solution
   - Click "Restore" if needed
   - Or: `Ctrl+Alt+L`

3. **Build Solution**
   - Build → Build Solution
   - Or: `Ctrl+Shift+B`
   - Wait for completion (~1-2 minutes)

4. **Check Output**
   - View → Output
   - Should show: "Build succeeded"
   - 0 errors, 0 warnings (ideal)

5. **Run Application**
   - Debug → Start Debugging
   - Or: `F5`
   - Main window should appear

### Method 2: Command Line

**PowerShell/CMD**
```bash
# Navigate to project
cd C:\path\to\ResetAPI

# Restore NuGet packages
dotnet restore

# Build (Debug)
dotnet build -c Debug

# Build (Release)
dotnet build -c Release

# Run application
cd ResetAPI.UI\bin\Debug\net472
ResetAPI.UI.exe
```

**Windows Terminal (Recommended)**
```pwsh
# Same commands as above, but with better output formatting
$PSVersionTable  # Should show PowerShell 5.1+

dotnet build -c Release

# Output location
ls .\ResetAPI.UI\bin\Release\  # List files
```

### Method 3: VS Code

1. **Install Extensions**
   - C# (powered by Omnisharp)
   - .NET Runtime Installer

2. **Open Workspace**
   - File → Open Folder → ResetAPI

3. **Build Task**
   - Terminal → Run Task
   - Select: "build"
   - Or: `Ctrl+Shift+B`

4. **Debug**
   - Press `F5`
   - Select ".NET: Debug ResetAPI.UI"

## Compilation Output

### Debug Build
```
ResetAPI.UI/bin/Debug/
├── ResetAPI.UI.exe           (5 MB)
├── ResetAPI.UI.pdb           (symbols)
├── ResetAPI.Domain.dll
├── ResetAPI.Services.dll
├── ResetAPI.Infrastructure.dll
├── OxyPlot.Wpf.dll
├── Newtonsoft.Json.dll
├── Serilog.dll
├── Polly.dll
└── [other dependencies]
```

### Release Build
```
ResetAPI.UI/bin/Release/
├── ResetAPI.UI.exe           (4.5 MB, optimized)
├── [dependencies - same as Debug]
└── [no .pdb files]
```

## Configuration

### Copy appsettings.json

**Debug**
```bash
copy appsettings.json ResetAPI.UI\bin\Debug\appsettings.json
```

**Release**
```bash
copy appsettings.json ResetAPI.UI\bin\Release\appsettings.json
```

### Create Logs Directory

**Debug**
```bash
mkdir ResetAPI.UI\bin\Debug\logs
```

**Release**
```bash
mkdir ResetAPI.UI\bin\Release\logs
```

## Verification

### Check Build Success

**Visual Studio**
- Output window shows "Build succeeded"
- Error list is empty
- Executable appears in bin folder

**Command Line**
```bash
echo %ERRORLEVEL%  # Should be 0
ls ResetAPI.UI/bin/Release/ResetAPI.UI.exe  # Should exist
```

### Test Application

1. **Run Executable**
   ```bash
   .\ResetAPI.UI\bin\Release\ResetAPI.UI.exe
   ```

2. **Expected Behavior**
   - Main window opens
   - "RESET API" title visible
   - CS2 / Dota 2 radio buttons visible
   - "Load Popular" button works
   - Status bar shows "Ready"

3. **Test Features**
   - Click "Load Popular" → List populates
   - Type in search → List filters
   - Click skin → Chart updates
   - No errors in logs/resetapi.log

## Troubleshooting

### Error: "Cannot find .NET Framework 4.7.2"

**Solution**: Install .NET Framework 4.7.2
```bash
# Download and run installer from Microsoft
# https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472
```

### Error: "Project dependencies not found"

**Solution**: Restore NuGet packages
```bash
dotnet restore
```

### Error: "Cannot open solution file"

**Solution**: Update Visual Studio
- Help → Check for Updates
- Or reinstall latest version

### Error: "OxyPlot.Wpf.dll not found"

**Solution**: Manual NuGet restore
```bash
dotnet add ResetAPI.UI package OxyPlot.Wpf
dotnet restore
```

### Build Hangs or Takes Too Long

**Solution**: Clean and rebuild
```bash
dotnet clean
rm -r ResetAPI.UI/bin
rm -r ResetAPI.UI/obj
dotnet build -c Release
```

### Application Crashes on Startup

**Check Logs**
```bash
type logs\resetapi.log  # View error messages
```

**Common Causes**
- Missing appsettings.json
- Invalid JSON syntax
- Missing .NET Framework
- Corrupted cache

## Advanced Build Options

### Static Analysis

**Code Analysis (built-in)**
```bash
dotnet build -c Release /p:EnforceCodeStyleInBuild=true
```

**Using StyleCop**
```bash
# Add to .csproj
<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.431" />
</ItemGroup>
```

### Code Coverage

**Using coverlet**
```bash
dotnet add package coverlet.collector
dotnet test /p:CollectCoverage=true
```

### Profile Performance

**Using PerfView**
1. Download [PerfView](https://github.com/Microsoft/perfview)
2. Start collection
3. Run application
4. Stop collection
5. Analyze results

## Deployment Build

### Create Standalone Package

```bash
# Publish Release build
dotnet publish -c Release -o dist/

# Create zip archive
compressArchive dist/ ResetAPI-1.0.0.zip
```

### Installer (Advanced)

**Using WiX Toolset**
1. Install WiX from [wixtoolset.org](https://wixtoolset.org/)
2. Create .wxs installer definition
3. Build with: `candle.exe` and `light.exe`

## CI/CD Build

### GitHub Actions

Automatically triggers on:
- Push to main
- Pull requests

**View Status**
- GitHub → Actions tab
- Check build logs

### Local CI Simulation

```bash
# Simulate CI environment
dotnet clean
dotnet restore
dotnet build -c Release
# All tests would run here
echo "Build complete"
```

## Build Time Optimization

### Parallel Build

```bash
dotnet build -c Release -m
# Or with specific parallel level
msbuild ResetAPI.sln /m:4
```

### Incremental Build

- Only modified files rebuilt
- Cached build results used
- Typical incremental: <10 seconds

### Precompile XAML

- Already enabled in ResetAPI.UI.csproj
- Improves runtime performance

## Post-Build Tasks

### Copy Configuration

```bash
# Auto-copy appsettings.json to output
# Add to .csproj:
<ItemGroup>
  <None Update="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### Generate Documentation

```bash
# XML documentation comments generate .xml files
# Helpful for IntelliSense and documentation
```

---

**Build System**: MSBuild (.NET SDK)  
**Configuration Files**: ResetAPI.sln, *.csproj, .editorconfig  
**Output**: Standalone .exe with all dependencies  
**Status**: Production Ready ✅
