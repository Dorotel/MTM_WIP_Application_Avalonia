# Platform-Specific File System Differences for MTM File Selection Service

## Overview
This document details the file system differences, permissions models, and storage provider behaviors across platforms supported by the MTM File Selection Service. Understanding these differences is crucial for proper testing and troubleshooting.

## 📱 Platform Support Matrix

| Platform | File Dialogs | Storage Provider | Path Format | Permissions | Testing Status |
|----------|-------------|------------------|-------------|-------------|----------------|
| Windows  | Native Win32 | Full Support | `C:\Users\...` | ACL-based | ✅ Primary Platform |
| macOS    | Native Cocoa | Full Support | `/Users/...` | POSIX + Sandboxing | ✅ CI/VM Testing |
| Linux    | GTK/Qt | Full Support | `/home/...` | POSIX | ✅ CI Testing |
| Android  | SAF Intent | Android SAF | `/storage/...` | App Sandboxing | ✅ Build Validation |
| iOS      | Document Picker | iOS APIs | App Sandbox | iOS Sandboxing | 🔄 Future Support |

## 🖥️ Desktop Platforms

### Windows
```csharp
// File paths
var documentsPath = @"C:\Users\username\Documents";
var tempPath = @"C:\Windows\Temp";

// Path separators
var separator = Path.DirectorySeparatorChar; // '\'

// Special folders
Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
// Returns: C:\Users\[username]\Documents

// File permissions - ACL based
var file = new FileInfo(filePath);
var canRead = file.Exists && !file.IsReadOnly;

// Storage provider
// Uses Windows File Dialog (IFileOpenDialog/IFileSaveDialog)
topLevel.StorageProvider.OpenFilePickerAsync(options);
```

**Windows-Specific Considerations:**
- UNC path support: `\\server\share\file.txt`
- Drive letters and case-insensitive paths
- Long path support (260+ characters) with proper configuration
- Windows Defender and antivirus interference
- Administrative privileges for system folders

### macOS
```csharp
// File paths
var documentsPath = "/Users/username/Documents";
var tempPath = "/tmp";

// Path separators
var separator = Path.DirectorySeparatorChar; // '/'

// Special folders
Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
// Returns: /Users/[username]/Documents

// File permissions - POSIX + extended attributes
var info = new FileInfo(filePath);
var canRead = info.Exists && (info.Attributes & FileAttributes.ReadOnly) == 0;

// Storage provider
// Uses NSOpenPanel/NSSavePanel through Avalonia
topLevel.StorageProvider.OpenFilePickerAsync(options);
```

**macOS-Specific Considerations:**
- Case-sensitive file system (APFS) vs case-insensitive (HFS+)
- App sandboxing restrictions
- Security scoped bookmarks for file access outside sandbox
- Gatekeeper and file quarantine attributes
- macOS Catalina+ read-only system volume

### Linux
```csharp
// File paths  
var documentsPath = "/home/username/Documents";
var tempPath = "/tmp";

// Path separators
var separator = Path.DirectorySeparatorChar; // '/'

// Special folders
Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
// Returns: /home/[username]/Documents

// File permissions - POSIX
var info = new FileInfo(filePath);
var canRead = info.Exists;
// Additional: Check owner/group/other permissions

// Storage provider
// Uses GTK or Qt file dialogs through Avalonia
topLevel.StorageProvider.OpenFilePickerAsync(options);
```

**Linux-Specific Considerations:**
- Multiple desktop environments (GNOME, KDE, XFCE)
- Different file dialog implementations (GTK3/4, Qt5/6)
- Flatpak/Snap sandboxing
- XDG Base Directory Specification
- File system case sensitivity

## 📱 Mobile Platforms

### Android
```csharp
// File paths - Scoped Storage (API 29+)
var documentsPath = "/storage/emulated/0/Documents"; // External storage
var appDataPath = "/data/data/com.mtm.wipapp/files"; // App-specific

// Storage Access Framework (SAF)
// Uses Android Intents for file selection
var intent = new Intent(Intent.ActionOpenDocument);
intent.AddCategory(Intent.CategoryOpenable);
intent.SetType("application/json");

// MTM Implementation through Avalonia
topLevel.StorageProvider.OpenFilePickerAsync(options);
// Internally uses Android Storage Provider
```

**Android-Specific Considerations:**
- **Scoped Storage**: Apps can only access their own files and user-selected files
- **Storage Access Framework (SAF)**: Required for file access outside app directory
- **Permissions**: `READ_EXTERNAL_STORAGE`, `WRITE_EXTERNAL_STORAGE` (legacy)
- **API Level Dependencies**: Different storage models for API 29+ vs older versions
- **External Storage**: SD cards, USB drives, cloud storage integration

### Android Permissions Model
```xml
<!-- AndroidManifest.xml -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

<!-- For API 30+ -->
<uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" 
                 tools:ignore="ScopedStorage" />
```

```csharp
// Runtime permission checks (Avalonia handles this)
var hasPermission = ContextCompat.CheckSelfPermission(
    context, 
    Manifest.Permission.ReadExternalStorage
) == Permission.Granted;
```

### iOS (Future Support)
```csharp
// iOS Document Picker
var documentPicker = new UIDocumentPickerViewController(
    new[] { "public.json" },
    UIDocumentPickerMode.Import
);

// MTM Implementation (Future)
// Will use iOS Storage Provider through Avalonia
topLevel.StorageProvider.OpenFilePickerAsync(options);
```

**iOS-Specific Considerations:**
- App sandbox restrictions
- Document provider extensions
- iCloud Drive integration
- Files app integration
- Security scoped resources

## 🔒 Security and Permissions

### Windows Security Model
```csharp
// Access Control Lists (ACLs)
var security = new FileSecurity();
var rules = security.GetAccessRules(true, true, typeof(NTAccount));

// Check file permissions
var hasAccess = File.Exists(path);
try
{
    using var stream = File.OpenRead(path);
    // Can read
}
catch (UnauthorizedAccessException)
{
    // No read access
}
```

### POSIX Security Model (macOS/Linux)
```bash
# File permissions
-rw-r--r-- 1 user group 1024 Jan 1 12:00 file.json
# Owner: read/write, Group: read, Other: read

# Check permissions
ls -la file.json
stat -c "%a %n" file.json  # Linux
stat -f "%Sp %N" file.json  # macOS
```

```csharp
// Check POSIX permissions in .NET
var info = new FileInfo(path);
var isReadable = (info.Attributes & FileAttributes.ReadOnly) == 0;

// Linux/macOS specific permission checking would require P/Invoke
```

### Android Security Model
```csharp
// App-specific directory (always accessible)
var appDir = Android.App.Application.Context.FilesDir.AbsolutePath;
// /data/data/com.mtm.wipapp/files

// External storage (requires permission)
var externalDir = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
// /storage/emulated/0

// Scoped storage (API 29+)
var documentsDir = Android.OS.Environment.GetExternalStoragePublicDirectory(
    Android.OS.Environment.DirectoryDocuments
).AbsolutePath;
```

## 🛠️ Testing Strategies by Platform

### Windows Testing
```powershell
# Local testing
dotnet build -r win-x64
dotnet run -- --test-file-access "C:\Windows\System32\notepad.exe"

# Test different file systems
# NTFS: Full feature support
# FAT32: Limited features, 4GB file size limit
# ReFS: Advanced features (Server editions)
```

### macOS Testing
```bash
# Local testing (on Mac or VM)
dotnet build -r osx-x64
dotnet run -- --test-file-access "/Users/$(whoami)/Documents/test.json"

# Test file system types
diskutil info /  # Check if APFS or HFS+

# Test sandboxing
# Create app bundle and test restrictions
```

### Linux Testing
```bash
# Local testing
dotnet build -r linux-x64
dotnet run -- --test-file-access "/home/$(whoami)/Documents/test.json"

# Test different desktop environments
echo $XDG_CURRENT_DESKTOP  # GNOME, KDE, XFCE, etc.

# Test different file systems
df -T  # ext4, btrfs, xfs, etc.
```

### Android Testing
```bash
# Emulator testing
# 1. Create AVD with API 31+ (required for .NET 8)
avd create -n MTM_Test -k "system-images;android-31;google_apis;x86_64"

# 2. Start emulator
emulator @MTM_Test

# 3. Build and deploy
dotnet build -f net8.0-android
adb install ./bin/Debug/net8.0-android/com.mtm.wipapp.apk

# 4. Test file operations
adb shell am start -n com.mtm.wipapp/.MainActivity
adb logcat | grep MTM  # Monitor logs
```

## 🚨 Common Cross-Platform Issues

### Path Separator Issues
```csharp
// ❌ WRONG - Hard-coded separators
var path = "Documents\\Config\\settings.json";  // Windows-only
var path = "Documents/Config/settings.json";    // Unix-only

// ✅ CORRECT - Cross-platform
var path = Path.Combine("Documents", "Config", "settings.json");
var path = Path.Join("Documents", "Config", "settings.json");  // .NET Core+
```

### File System Case Sensitivity
```csharp
// ❌ WRONG - Assumes case insensitive
if (fileName == "CONFIG.JSON") { }

// ✅ CORRECT - Cross-platform comparison
if (string.Equals(fileName, "config.json", StringComparison.OrdinalIgnoreCase)) { }
```

### Directory Access Issues
```csharp
// ❌ WRONG - Assumes specific permissions
Directory.CreateDirectory("/usr/local/myapp");  // Requires root on Unix

// ✅ CORRECT - Use appropriate directories
var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
Directory.CreateDirectory(Path.Combine(appData, "MTM"));
```

### File Locking Differences
```csharp
// Windows: Exclusive locks by default
// Unix: Advisory locking, multiple readers allowed

// ✅ CORRECT - Explicit file sharing
using var stream = new FileStream(
    path, 
    FileMode.Open, 
    FileAccess.Read, 
    FileShare.Read  // Allow other readers
);
```

## 📊 Performance Characteristics

| Platform | Dialog Speed | File I/O Speed | Memory Usage | Notes |
|----------|-------------|----------------|---------------|-------|
| Windows  | Fast | Fast | Moderate | Native performance |
| macOS    | Fast | Fast | Moderate | Cocoa overhead |
| Linux    | Variable | Fast | Low | Depends on desktop environment |
| Android  | Slow | Variable | High | Intent overhead, storage type dependent |

## 🔄 Migration Strategies

### From Windows-Only to Cross-Platform
```csharp
// Phase 1: Replace Windows-specific APIs
// Old: Microsoft.Win32.OpenFileDialog
// New: Avalonia.Platform.Storage.IStorageProvider

// Phase 2: Path handling
// Old: Hard-coded paths with backslashes
// New: Path.Combine() and Environment.GetFolderPath()

// Phase 3: Permission handling
// Old: Windows ACL checks
// New: Generic file access validation

// Phase 4: Testing
// Validate on each target platform
```

### Android-Specific Migration
```csharp
// Phase 1: Storage model
// Old: Direct file system access
// New: Storage Access Framework (SAF)

// Phase 2: Permissions
// Old: Assume file access
// New: Request permissions at runtime

// Phase 3: UI adaptation
// Old: Desktop file dialogs
// New: Android document picker intents
```

## 🧪 Validation Checklist

### Pre-Deployment Testing
- [ ] Build succeeds on all target platforms
- [ ] File dialogs open and respond correctly
- [ ] Path handling works with platform conventions
- [ ] Permissions are properly validated
- [ ] Error handling provides appropriate feedback
- [ ] Performance meets requirements on each platform
- [ ] Memory usage stays within bounds
- [ ] No platform-specific crashes or exceptions

### Runtime Validation
```csharp
public static async Task<bool> ValidatePlatformCompatibility()
{
    var checks = new List<(string name, Func<Task<bool>> check)>
    {
        ("Platform Detection", TestPlatformDetection),
        ("File System Access", TestFileSystemAccess),
        ("Storage Provider", TestStorageProvider),
        ("Path Handling", TestPathHandling),
        ("Permissions", TestPermissions),
        ("Performance", TestPerformance)
    };

    var results = new List<bool>();
    foreach (var (name, check) in checks)
    {
        try
        {
            var result = await check();
            Console.WriteLine($"{name}: {(result ? "PASSED" : "FAILED")}");
            results.Add(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{name}: FAILED ({ex.Message})");
            results.Add(false);
        }
    }

    return results.All(r => r);
}
```

## 📚 Additional Resources

### Platform-Specific Documentation
- **Windows**: [File System Functionality](https://docs.microsoft.com/en-us/windows/win32/fileio/file-systems)
- **macOS**: [File System Programming Guide](https://developer.apple.com/library/archive/documentation/FileManagement/Conceptual/FileSystemProgrammingGuide/)
- **Linux**: [Linux File System Hierarchy](https://www.pathname.com/fhs/)
- **Android**: [Data and File Storage Overview](https://developer.android.com/guide/topics/data/data-storage)
- **Avalonia**: [Platform Storage](https://docs.avaloniaui.net/docs/concepts/services/storage-provider)

### Testing Tools
- **Windows**: Native development environment, WinAppDriver
- **macOS**: Xcode, VMware/Parallels for VM testing  
- **Linux**: Docker containers, virtual machines
- **Android**: Android Studio emulator, physical devices
- **Cross-Platform**: GitHub Actions, Azure DevOps pipelines

---

**Last Updated**: September 13, 2025  
**MTM File Selection Service Version**: 1.0.0  
**Supported Platforms**: Windows, macOS, Linux, Android (build validation)  
**Next Review**: Upon major platform updates or service enhancements