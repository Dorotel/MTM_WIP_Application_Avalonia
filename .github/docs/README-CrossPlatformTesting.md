# Cross-Platform Testing Guide for MTM File Selection Service

## Overview
This document provides comprehensive guidance for testing the MTM File Selection Service across macOS and Android platforms, ensuring your Avalonia application works consistently across all supported platforms.

## 🎯 Current Implementation Status
Your `FileSelectionService` is built using Avalonia's cross-platform patterns:
- ✅ Uses `IStorageProvider` for platform abstraction
- ✅ Supports both Desktop (`IClassicDesktopStyleApplicationLifetime`) and Mobile (`ISingleViewApplicationLifetime`)
- ✅ Implements proper platform detection via `GetTopLevelFromCurrentView()`
- ✅ Handles file system differences through Avalonia's storage APIs

## 📱 Platform Support Matrix

| Platform | File Dialogs | Storage Provider | MTM Compatibility | Testing Method |
|----------|-------------|------------------|-------------------|----------------|
| Windows  | ✅ Native   | ✅ Full Support  | ✅ Primary Platform | Local Development |
| macOS    | ✅ Native   | ✅ Full Support  | ✅ Supported | VM/CI/Physical Mac |
| Linux    | ✅ GTK/QT   | ✅ Full Support  | ✅ Supported | Docker/VM/CI |
| Android  | ✅ Intent   | ✅ Android SAF   | ✅ Mobile Support | Emulator/Device |
| iOS      | ✅ Native   | ✅ Document Picker | 🔄 Future Support | Physical Device/Simulator |

## 🖥️ macOS Testing Approaches

### Option 1: Physical Mac (Ideal)
```bash
# Install .NET 8 on macOS
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0

# Clone and test your project
git clone https://github.com/Dorotel/MTM_WIP_Application_Avalonia.git
cd MTM_WIP_Application_Avalonia
dotnet build -r osx-x64
dotnet run

# Run cross-platform tests
dotnet test --framework net8.0 --filter "CrossPlatformSupportTests"
```

### Option 2: macOS Virtual Machine
```powershell
# Using VMware Workstation Pro on Windows
# 1. Install VMware Workstation Pro
# 2. Download macOS installer (requires valid license)
# 3. Create macOS VM with:
#    - 8GB+ RAM
#    - 100GB+ storage
#    - Enable virtualization features

# Inside macOS VM:
brew install dotnet
git clone https://github.com/Dorotel/MTM_WIP_Application_Avalonia.git
cd MTM_WIP_Application_Avalonia
dotnet build -r osx-x64
dotnet run
```

### Option 3: Cloud-Based macOS (GitHub Actions)
```yaml
# .github/workflows/macos-test.yml
name: macOS Testing
on: [push, pull_request]
jobs:
  test-macos:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - name: Build for macOS
        run: dotnet build -r osx-x64
      - name: Run Cross-Platform Tests
        run: dotnet test --filter "CrossPlatformSupportTests"
      - name: Test File Selection Service
        run: dotnet run -- --test-file-selection
```

## 🤖 Android Testing Approaches

### Option 1: Android Emulator (Recommended)
```bash
# Install Android Studio and set up AVD
# Create AVD with API 31+ (required for .NET 8 Avalonia apps)

# Build for Android
dotnet workload install android
dotnet add package Avalonia.Android
dotnet build -f net8.0-android

# Deploy to emulator
dotnet run -f net8.0-android
```

### Option 2: Physical Android Device
```bash
# Enable Developer Options and USB Debugging
# Connect device via USB

# Build and deploy
dotnet build -f net8.0-android
adb install ./bin/Debug/net8.0-android/com.mtm.wipapplication.apk
adb shell am start -n com.mtm.wipapplication/.MainActivity
```

### Android Project Template
Create a new Android-specific project to test mobile functionality:

```xml
<!-- MTM.Android.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-android</TargetFramework>
    <SupportedOSPlatformVersion>21</SupportedOSPlatformVersion>
    <OutputType>Exe</OutputType>
    <UseAndroidX>true</UseAndroidX>
    <ApplicationId>com.mtm.wipapplication</ApplicationId>
    <ApplicationVersion>1</ApplicationVersion>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Avalonia.Android" Version="11.3.4" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\MTM_WIP_Application_Avalonia.csproj" />
  </ItemGroup>
</Project>
```

## 🧪 Testing Strategies

### 1. Automated Platform Detection Tests
```csharp
[Test]
public void Should_Detect_Platform_Correctly()
{
    var isWindows = OperatingSystem.IsWindows();
    var isMacOS = OperatingSystem.IsMacOS();
    var isLinux = OperatingSystem.IsLinux();
    var isAndroid = OperatingSystem.IsAndroid();
    
    // At least one should be true
    Assert.IsTrue(isWindows || isMacOS || isLinux || isAndroid);
    
    // Test FileSelectionService platform adaptation
    var service = GetFileSelectionService();
    Assert.IsNotNull(service);
}
```

### 2. File System Path Tests
```csharp
[Test]
public async Task Should_Handle_Platform_Specific_Paths()
{
    var service = GetFileSelectionService();
    var options = new FileSelectionOptions
    {
        InitialDirectory = GetPlatformDocumentsFolder()
    };
    
    // This should not throw on any platform
    var result = await service.ValidateFileAccessAsync("/nonexistent/path");
    Assert.IsFalse(result);
}

private string GetPlatformDocumentsFolder()
{
    if (OperatingSystem.IsWindows())
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    else if (OperatingSystem.IsMacOS())
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Documents");
    else if (OperatingSystem.IsAndroid())
        return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
    else
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
}
```

### 3. Storage Provider Tests
```csharp
[Test]
public async Task Should_Access_Storage_Provider_On_All_Platforms()
{
    var service = GetFileSelectionService();
    
    // Test that storage provider is available
    // This validates the GetTopLevelFromCurrentView() method
    var options = new FileSelectionOptions
    {
        Title = "Cross-Platform Test",
        Extensions = new[] { "*.json" },
        Mode = FileSelectionMode.Import
    };
    
    // Should not throw - validates platform detection
    Assert.DoesNotThrowAsync(async () =>
    {
        await service.ShowFileSelectionViewAsync(new Button(), options, _ => { });
    });
}
```

## 🔧 Development Environment Setup

### Windows Development for Cross-Platform Testing
```powershell
# Install required workloads
dotnet workload install android
dotnet workload install macos # if you have Mac access

# Install platform-specific tools
winget install AndroidStudio
winget install VMware.WorkstationPro # for macOS VM

# Verify installation
dotnet workload list
```

### Cross-Platform Build Verification
```bash
# Test all runtime identifiers
dotnet build -r win-x64
dotnet build -r win-x86
dotnet build -r osx-x64
dotnet build -r osx-arm64
dotnet build -r linux-x64
dotnet build -r linux-arm64
# dotnet build -f net8.0-android  # if Android workload installed
```

## 🚀 Continuous Integration Setup

### GitHub Actions Multi-Platform Testing
```yaml
name: Cross-Platform Tests
on: [push, pull_request]

jobs:
  test:
    strategy:
      matrix:
        os: [windows-latest, ubuntu-latest, macos-latest]
        include:
          - os: windows-latest
            runtime: win-x64
          - os: ubuntu-latest
            runtime: linux-x64
          - os: macos-latest
            runtime: osx-x64
    
    runs-on: ${{ matrix.os }}
    
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
          
      - name: Restore dependencies
        run: dotnet restore
        
      - name: Build for platform
        run: dotnet build -r ${{ matrix.runtime }}
        
      - name: Run cross-platform tests
        run: dotnet test --filter "CrossPlatformSupportTests"
        
      - name: Test file operations
        shell: bash
        run: |
          # Create test file
          echo '{"test": "data"}' > test-file.json
          
          # Run application with test parameters
          if [[ "${{ matrix.os }}" == "windows-latest" ]]; then
            dotnet run -- --test-file-operations "./test-file.json"
          else
            dotnet run -- --test-file-operations "$(pwd)/test-file.json"
          fi

  android-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
          
      - name: Install Android workload
        run: dotnet workload install android
        
      # Note: Full Android testing requires emulator setup
      # This validates build compatibility
      - name: Build for Android
        run: dotnet build -f net8.0-android
```

## 🐛 Common Cross-Platform Issues and Solutions

### 1. File Path Separators
```csharp
// ❌ Wrong - Windows-specific
var path = "C:\\Users\\johnk\\Documents\\file.json";

// ✅ Correct - Cross-platform
var path = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
    "file.json"
);
```

### 2. File Permissions
```csharp
// Android requires special permissions for external storage
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
```

### 3. Storage Provider Availability
Your current implementation already handles this correctly:
```csharp
private TopLevel? GetTopLevelFromCurrentView()
{
    var applicationLifetime = Avalonia.Application.Current?.ApplicationLifetime;
    
    switch (applicationLifetime)
    {
        case IClassicDesktopStyleApplicationLifetime desktop:
            return desktop.MainWindow;  // Windows, macOS, Linux
        case ISingleViewApplicationLifetime singleView:
            return singleView.MainView as TopLevel;  // Android, iOS
        default:
            return null;  // Unsupported platform
    }
}
```

## 📊 Testing Checklist

### Pre-Testing Setup
- [ ] Verify .NET 8 SDK installed on target platform
- [ ] Confirm Avalonia 11.3.4 compatibility
- [ ] Test file system access permissions
- [ ] Validate storage provider availability

### Functional Testing
- [ ] File selection dialogs open correctly
- [ ] File validation works across platforms
- [ ] Path handling respects platform conventions
- [ ] Error handling provides appropriate feedback
- [ ] Panel placement adapts to platform UI patterns

### Performance Testing
- [ ] File operations complete within targets (&lt;2s export, &lt;3s import)
- [ ] Memory usage stays within bounds (&lt;5MB additional)
- [ ] No memory leaks during repeated operations
- [ ] UI remains responsive during file operations

### Platform-Specific Testing
- [ ] **Windows**: Native file dialogs, UNC path support
- [ ] **macOS**: Finder integration, sandboxing compliance
- [ ] **Linux**: GTK/Qt dialog support, permission model
- [ ] **Android**: SAF integration, intent-based file selection

## 📚 Resources and References

### Official Documentation
- [Avalonia Cross-Platform Guide](https://docs.avaloniaui.net/docs/deployment)
- [.NET 8 Platform Support](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Avalonia Mobile Development](https://docs.avaloniaui.net/docs/deployment/mobile)

### Testing Tools
- **Windows**: Native development environment
- **macOS**: Xcode, VMware Workstation Pro
- **Linux**: Docker, VirtualBox, WSL2
- **Android**: Android Studio, AVD Manager
- **CI/CD**: GitHub Actions, Azure DevOps

### Community Resources
- [Avalonia Community Samples](https://github.com/AvaloniaUI/Avalonia.Samples)
- [Cross-Platform File Operations](https://github.com/AvaloniaUI/Avalonia/discussions)

---

## 🎯 Quick Start Testing Commands

```bash
# Test on current platform
dotnet test --filter "CrossPlatformSupportTests" --verbosity normal

# Build for all platforms (validation only)
dotnet build -r win-x64
dotnet build -r osx-x64  
dotnet build -r linux-x64

# Run application with file selection test
dotnet run -- --enable-file-selection-test

# Check platform detection
dotnet run -- --show-platform-info
```

This comprehensive testing strategy ensures your MTM File Selection Service works consistently across all target platforms while providing multiple approaches for validation based on your available resources and requirements.

---

## 🏭 COMPLETE MTM APPLICATION TESTING

**Note**: The above covers FileSelectionService testing. For **complete application testing**, see the comprehensive strategy below that covers the entire MTM manufacturing system.

### Full Application Testing Scope
- **UI Testing**: All 7+ views (MainView, InventoryTabView, QuickButtonsView, RemoveTabView, etc.)
- **Database Testing**: 45+ stored procedures across platforms
- **Service Testing**: 15+ services including QuickButtons, Configuration, Theme, etc.
- **Integration Testing**: Complete user workflows from startup to shutdown
- **Performance Testing**: High-volume transaction processing
- **Cross-Platform Testing**: Windows, macOS, Linux, Android compatibility

See `Testing/FULL-APPLICATION-TESTING-STRATEGY.md` for complete details.