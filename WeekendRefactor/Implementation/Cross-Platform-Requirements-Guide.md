# Cross-Platform Overlay Requirements Guide

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Audience**: MTM Application Developers and Platform Engineers  

## 🌐 Cross-Platform Overview

This guide covers platform-specific requirements, adaptations, and best practices for MTM overlay system across Windows, macOS, Linux, and mobile platforms. Ensuring consistent user experience while respecting platform conventions.

## 🖥️ Platform-Specific Behaviors

### **Windows Platform Requirements**

#### **Desktop Integration**

```csharp
// File: Services/Platform/WindowsPlatformService.cs

public class WindowsPlatformService : IPlatformService
{
    public PlatformType CurrentPlatform => PlatformType.Windows;

    public async Task<OverlayConfiguration> GetPlatformOverlayConfigurationAsync()
    {
        return new OverlayConfiguration
        {
            // Windows 11 design system integration
            CornerRadius = 8,
            ShadowEnabled = true,
            ShadowOffset = new Point(0, 4),
            ShadowBlur = 16,
            ShadowColor = Color.FromArgb(40, 0, 0, 0),
            
            // Windows animations
            AnimationDuration = TimeSpan.FromMilliseconds(250),
            AnimationEasing = EasingType.QuadraticOut,
            
            // Windows input handling
            SupportsTouch = await DetectTouchSupportAsync(),
            SupportsStylus = await DetectStylusSupportAsync(),
            KeyboardShortcutsEnabled = true,
            
            // Windows theming
            FollowSystemTheme = true,
            AccentColorSupport = true
        };
    }

    public Task<bool> ValidateOverlayPositioningAsync(OverlayPositionRequest request)
    {
        // Windows-specific validation for multi-monitor setups
        var workingArea = GetWorkingAreaForWindow(request.ParentWindow);
        var overlayBounds = CalculateOverlayBounds(request);
        
        return Task.FromResult(workingArea.Contains(overlayBounds));
    }

    private async Task<bool> DetectTouchSupportAsync()
    {
        // Windows touch detection logic
        return await Task.FromResult(
            Environment.OSVersion.Platform == PlatformID.Win32NT &&
            GetSystemMetrics(SM_DIGITIZER) != 0);
    }
}
```

#### **Windows-Specific Overlay Features**

```csharp
// File: Views/Platform/WindowsOverlayEnhancements.cs

public class WindowsOverlayEnhancements
{
    /// <summary>
    /// Apply Windows 11 Mica material effect to overlay background
    /// </summary>
    public static void ApplyMicaEffect(Border overlayBackground)
    {
        if (IsWindows11OrLater())
        {
            overlayBackground.Background = new SolidColorBrush(Colors.Transparent);
            // Apply Mica backdrop - requires platform-specific implementation
            ApplyBackdropMaterial(overlayBackground, BackdropType.Mica);
        }
    }

    /// <summary>
    /// Configure Windows-specific keyboard accelerators
    /// </summary>
    public static void ConfigureWindowsAccelerators(UserControl overlay)
    {
        var accelerators = new List<KeyBinding>
        {
            new KeyBinding { Key = Key.F4, Modifiers = KeyModifiers.Alt, Command = overlay.CloseCommand },
            new KeyBinding { Key = Key.Space, Modifiers = KeyModifiers.Alt, Command = overlay.SystemMenuCommand },
            new KeyBinding { Key = Key.Tab, Modifiers = KeyModifiers.Alt, Command = overlay.SwitchOverlayCommand }
        };

        overlay.KeyBindings.AddRange(accelerators);
    }
}
```

### **macOS Platform Requirements**

#### **macOS Design System Integration**

```csharp
// File: Services/Platform/MacOSPlatformService.cs

public class MacOSPlatformService : IPlatformService
{
    public PlatformType CurrentPlatform => PlatformType.macOS;

    public async Task<OverlayConfiguration> GetPlatformOverlayConfigurationAsync()
    {
        return new OverlayConfiguration
        {
            // macOS design principles
            CornerRadius = 12, // Larger corner radius for macOS
            ShadowEnabled = true,
            ShadowOffset = new Point(0, 8), // More pronounced shadow
            ShadowBlur = 24,
            ShadowColor = Color.FromArgb(30, 0, 0, 0),
            
            // macOS animations (longer, more fluid)
            AnimationDuration = TimeSpan.FromMilliseconds(400),
            AnimationEasing = EasingType.CubicBezierOut,
            
            // macOS input handling
            SupportsTouch = await DetectTrackpadAsync(),
            SupportsStylus = await DetectApplePencilAsync(),
            KeyboardShortcutsEnabled = true,
            
            // macOS theming
            FollowSystemTheme = true,
            AccentColorSupport = true,
            VibrancySupport = true // macOS-specific
        };
    }

    public async Task ConfigureMacOSGesturesAsync(UserControl overlay)
    {
        // Configure macOS-specific gestures
        var gestureRecognizers = new List<GestureRecognizer>
        {
            new SwipeGestureRecognizer { Direction = SwipeDirection.Down, Command = overlay.DismissCommand },
            new PinchGestureRecognizer { Command = overlay.ZoomCommand },
            new TwoFingerTapGestureRecognizer { Command = overlay.ContextMenuCommand }
        };

        overlay.GestureRecognizers.AddRange(gestureRecognizers);
    }
}
```

#### **macOS Native Integration**

```csharp
// File: Views/Platform/MacOSOverlayEnhancements.cs

public class MacOSOverlayEnhancements
{
    /// <summary>
    /// Apply macOS vibrancy effect to overlay
    /// </summary>
    public static void ApplyVibrancyEffect(Border overlayBackground, VibrancyType vibrancy)
    {
        if (IsMacOS())
        {
            // Apply NSVisualEffectView equivalent
            var vibrancyBrush = CreateVibrancyBrush(vibrancy);
            overlayBackground.Background = vibrancyBrush;
        }
    }

    /// <summary>
    /// Configure macOS menu bar integration
    /// </summary>
    public static void ConfigureMenuBarIntegration(IOverlayViewModel viewModel)
    {
        var menuItems = new List<MenuItemModel>
        {
            new MenuItemModel("Edit", KeyModifiers.Cmd, Key.E, viewModel.EditCommand),
            new MenuItemModel("Save", KeyModifiers.Cmd, Key.S, viewModel.SaveCommand),
            new MenuItemModel("Cancel", KeyModifiers.Cmd, Key.Period, viewModel.CancelCommand)
        };

        MenuBarService.RegisterOverlayMenuItems(viewModel.OverlayId, menuItems);
    }

    /// <summary>
    /// Handle macOS dark mode transitions
    /// </summary>
    public static void ConfigureDarkModeHandling(UserControl overlay)
    {
        SystemThemeWatcher.ThemeChanged += (sender, theme) =>
        {
            Application.Current.RequestedThemeVariant = theme switch
            {
                SystemTheme.Dark => ThemeVariant.Dark,
                SystemTheme.Light => ThemeVariant.Light,
                _ => ThemeVariant.Default
            };
        };
    }
}
```

### **Linux Platform Requirements**

#### **Linux Distribution Support**

```csharp
// File: Services/Platform/LinuxPlatformService.cs

public class LinuxPlatformService : IPlatformService
{
    public PlatformType CurrentPlatform => PlatformType.Linux;
    public LinuxDistribution Distribution { get; private set; }

    public async Task<OverlayConfiguration> GetPlatformOverlayConfigurationAsync()
    {
        Distribution = await DetectLinuxDistributionAsync();
        
        return new OverlayConfiguration
        {
            // Linux/GTK design principles
            CornerRadius = Distribution == LinuxDistribution.Ubuntu ? 6 : 8,
            ShadowEnabled = IsCompositorSupported(),
            ShadowOffset = new Point(0, 2),
            ShadowBlur = 8,
            ShadowColor = Color.FromArgb(50, 0, 0, 0),
            
            // Conservative animations for Linux
            AnimationDuration = TimeSpan.FromMilliseconds(200),
            AnimationEasing = EasingType.Linear,
            
            // Linux input handling
            SupportsTouch = await DetectTouchscreenAsync(),
            SupportsStylus = await DetectWacomSupportAsync(),
            KeyboardShortcutsEnabled = true,
            
            // Linux theming
            FollowSystemTheme = true,
            AccentColorSupport = Distribution == LinuxDistribution.Ubuntu,
            HighContrastSupport = true
        };
    }

    private async Task<LinuxDistribution> DetectLinuxDistributionAsync()
    {
        try
        {
            var osRelease = await File.ReadAllTextAsync("/etc/os-release");
            
            return osRelease.Contains("Ubuntu") ? LinuxDistribution.Ubuntu :
                   osRelease.Contains("Fedora") ? LinuxDistribution.Fedora :
                   osRelease.Contains("openSUSE") ? LinuxDistribution.OpenSUSE :
                   osRelease.Contains("Arch") ? LinuxDistribution.Arch :
                   LinuxDistribution.Generic;
        }
        catch
        {
            return LinuxDistribution.Generic;
        }
    }

    private bool IsCompositorSupported()
    {
        // Check for X11 compositor or Wayland
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")) ||
               !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY"));
    }
}
```

#### **Linux Accessibility Integration**

```csharp
// File: Services/Platform/LinuxAccessibilityService.cs

public class LinuxAccessibilityService : IAccessibilityService
{
    public async Task ConfigureOverlayAccessibilityAsync(UserControl overlay, IOverlayViewModel viewModel)
    {
        // Configure AT-SPI (Assistive Technology Service Provider Interface)
        await ConfigureATSPIAsync(overlay);
        
        // High contrast mode support
        if (IsHighContrastModeEnabled())
        {
            ApplyHighContrastTheme(overlay);
        }
        
        // Screen reader support
        ConfigureScreenReaderSupport(overlay, viewModel);
        
        // Keyboard navigation
        ConfigureKeyboardNavigation(overlay);
    }

    private async Task ConfigureATSPIAsync(UserControl overlay)
    {
        // Configure accessibility tree for Linux screen readers
        overlay.AutomationProperties.SetName("MTM Inventory Overlay");
        overlay.AutomationProperties.SetAccessibilityRole(AccessibilityRoles.Dialog);
        overlay.AutomationProperties.SetIsKeyboardFocusable(true);
        
        // Configure child elements
        foreach (var control in overlay.GetLogicalDescendants().OfType<Control>())
        {
            ConfigureControlAccessibility(control);
        }
    }

    private bool IsHighContrastModeEnabled()
    {
        // Check GTK/GNOME high contrast setting
        try
        {
            var result = Process.Start(new ProcessStartInfo
            {
                FileName = "gsettings",
                Arguments = "get org.gnome.desktop.interface gtk-theme",
                RedirectStandardOutput = true,
                UseShellExecute = false
            })?.StandardOutput.ReadToEnd();

            return result?.Contains("HighContrast") == true;
        }
        catch
        {
            return false;
        }
    }
}
```

## 📱 Mobile Platform Support

### **Android Requirements**

#### **Android-Specific Overlay Adaptations**

```csharp
// File: Services/Platform/AndroidPlatformService.cs

public class AndroidPlatformService : IPlatformService
{
    public PlatformType CurrentPlatform => PlatformType.Android;

    public async Task<OverlayConfiguration> GetPlatformOverlayConfigurationAsync()
    {
        var screenMetrics = await GetScreenMetricsAsync();
        
        return new OverlayConfiguration
        {
            // Material Design 3 compliance
            CornerRadius = 16,
            ShadowEnabled = true,
            ShadowOffset = new Point(0, 4),
            ShadowBlur = 12,
            ShadowColor = Color.FromArgb(40, 0, 0, 0),
            
            // Android animations
            AnimationDuration = TimeSpan.FromMilliseconds(300),
            AnimationEasing = EasingType.QuadraticOut,
            
            // Touch-optimized input
            MinTouchTargetSize = 48, // dp
            SupportsTouch = true,
            SupportsStylus = await DetectStylusSupportAsync(),
            KeyboardShortcutsEnabled = await HasPhysicalKeyboardAsync(),
            
            // Android theming
            FollowSystemTheme = true,
            DynamicColorSupport = true, // Android 12+
            EdgeToEdgeSupport = true
        };
    }

    public Task AdaptOverlayForScreenSizeAsync(UserControl overlay, ScreenSize screenSize)
    {
        return screenSize switch
        {
            ScreenSize.Compact => AdaptForCompactScreenAsync(overlay),
            ScreenSize.Medium => AdaptForMediumScreenAsync(overlay),
            ScreenSize.Expanded => AdaptForExpandedScreenAsync(overlay),
            _ => Task.CompletedTask
        };
    }

    private async Task AdaptForCompactScreenAsync(UserControl overlay)
    {
        // Full-screen overlay for phones
        overlay.Width = double.NaN; // Fill available width
        overlay.Height = double.NaN; // Fill available height
        overlay.Margin = new Thickness(16);
        
        // Adjust layout for single column
        if (overlay.FindControl<Grid>("MainGrid") is Grid grid)
        {
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            
            // Stack elements vertically
            await ReorganizeForVerticalLayoutAsync(grid);
        }
    }
}
```

#### **Android Touch and Gesture Support**

```csharp
// File: Services/Platform/AndroidGestureService.cs

public class AndroidGestureService
{
    public void ConfigureAndroidGestures(UserControl overlay, IOverlayViewModel viewModel)
    {
        // Swipe to dismiss
        var swipeGesture = new PanGestureRecognizer();
        swipeGesture.PanUpdated += (sender, e) =>
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    HandleSwipeGesture(overlay, e);
                    break;
                case GestureStatus.Completed:
                    HandleSwipeCompleted(overlay, viewModel, e);
                    break;
            }
        };
        
        overlay.GestureRecognizers.Add(swipeGesture);
        
        // Long press for context menu
        var longPressGesture = new TapGestureRecognizer
        {
            NumberOfTapsRequired = 1,
            CommandParameter = "LongPress"
        };
        longPressGesture.Tapped += (sender, e) => ShowContextMenu(overlay, viewModel);
        
        overlay.GestureRecognizers.Add(longPressGesture);
    }

    private void HandleSwipeGesture(UserControl overlay, PanUpdatedEventArgs e)
    {
        // Implement swipe-to-dismiss visual feedback
        var transform = overlay.RenderTransform as CompositeTransform ?? new CompositeTransform();
        transform.TranslateY = Math.Max(0, e.TotalY);
        overlay.RenderTransform = transform;
        
        // Adjust opacity based on swipe distance
        var dismissThreshold = 200;
        overlay.Opacity = Math.Max(0.5, 1 - (e.TotalY / dismissThreshold));
    }

    private async void HandleSwipeCompleted(UserControl overlay, IOverlayViewModel viewModel, PanUpdatedEventArgs e)
    {
        const int dismissThreshold = 200;
        
        if (e.TotalY > dismissThreshold)
        {
            // Animate dismiss
            await AnimateDismissAsync(overlay);
            await viewModel.CancelCommand.ExecuteAsync(null);
        }
        else
        {
            // Snap back to original position
            await AnimateSnapBackAsync(overlay);
        }
    }
}
```

## 🎨 Cross-Platform Theme Adaptation

### **Universal Theme Service**

```csharp
// File: Services/CrossPlatformThemeService.cs

public class CrossPlatformThemeService : IThemeService
{
    private readonly IPlatformService _platformService;
    private readonly Dictionary<PlatformType, IThemeAdapter> _themeAdapters;

    public CrossPlatformThemeService(
        IPlatformService platformService,
        IEnumerable<IThemeAdapter> themeAdapters)
    {
        _platformService = platformService;
        _themeAdapters = themeAdapters.ToDictionary(a => a.SupportedPlatform, a => a);
    }

    public async Task<ResourceDictionary> GetPlatformOptimizedThemeAsync(ThemeType theme)
    {
        var platform = _platformService.CurrentPlatform;
        var adapter = _themeAdapters.GetValueOrDefault(platform);
        
        if (adapter == null)
        {
            return await GetDefaultThemeAsync(theme);
        }

        var baseTheme = await GetBaseThemeAsync(theme);
        return await adapter.AdaptThemeAsync(baseTheme);
    }
}

public interface IThemeAdapter
{
    PlatformType SupportedPlatform { get; }
    Task<ResourceDictionary> AdaptThemeAsync(ResourceDictionary baseTheme);
}

// Windows theme adapter
public class WindowsThemeAdapter : IThemeAdapter
{
    public PlatformType SupportedPlatform => PlatformType.Windows;

    public async Task<ResourceDictionary> AdaptThemeAsync(ResourceDictionary baseTheme)
    {
        var adaptedTheme = new ResourceDictionary();
        
        // Copy base theme
        foreach (var kvp in baseTheme)
        {
            adaptedTheme.Add(kvp.Key, kvp.Value);
        }

        // Windows-specific adaptations
        adaptedTheme["OverlayCornerRadius"] = new CornerRadius(8);
        adaptedTheme["OverlayElevation"] = 8;
        adaptedTheme["ButtonCornerRadius"] = new CornerRadius(4);
        
        // Windows 11 accent color integration
        if (IsWindows11OrLater())
        {
            var accentColor = await GetWindowsAccentColorAsync();
            adaptedTheme["AccentBrush"] = new SolidColorBrush(accentColor);
        }

        return adaptedTheme;
    }
}

// macOS theme adapter
public class MacOSThemeAdapter : IThemeAdapter
{
    public PlatformType SupportedPlatform => PlatformType.macOS;

    public async Task<ResourceDictionary> AdaptThemeAsync(ResourceDictionary baseTheme)
    {
        var adaptedTheme = new ResourceDictionary();
        
        foreach (var kvp in baseTheme)
        {
            adaptedTheme.Add(kvp.Key, kvp.Value);
        }

        // macOS-specific adaptations
        adaptedTheme["OverlayCornerRadius"] = new CornerRadius(12);
        adaptedTheme["OverlayElevation"] = 16;
        adaptedTheme["ButtonCornerRadius"] = new CornerRadius(8);
        
        // Vibrancy support
        if (await SupportsVibrancyAsync())
        {
            adaptedTheme["OverlayBackgroundBrush"] = CreateVibrancyBrush();
        }

        return adaptedTheme;
    }
}
```

## ♿ Accessibility Requirements

### **Cross-Platform Accessibility Service**

```csharp
// File: Services/CrossPlatformAccessibilityService.cs

public class CrossPlatformAccessibilityService : IAccessibilityService
{
    private readonly IPlatformService _platformService;
    private readonly Dictionary<PlatformType, IAccessibilityAdapter> _accessibilityAdapters;

    public async Task ConfigureOverlayAccessibilityAsync(UserControl overlay, IOverlayViewModel viewModel)
    {
        // Universal accessibility configuration
        await ConfigureUniversalAccessibilityAsync(overlay, viewModel);
        
        // Platform-specific accessibility
        var platform = _platformService.CurrentPlatform;
        var adapter = _accessibilityAdapters.GetValueOrDefault(platform);
        
        if (adapter != null)
        {
            await adapter.ConfigurePlatformAccessibilityAsync(overlay, viewModel);
        }
    }

    private async Task ConfigureUniversalAccessibilityAsync(UserControl overlay, IOverlayViewModel viewModel)
    {
        // Set overlay properties
        overlay.AutomationProperties.SetName($"MTM {viewModel.OverlayTitle}");
        overlay.AutomationProperties.SetAccessibilityRole(AccessibilityRoles.Dialog);
        overlay.AutomationProperties.SetIsModal(true);
        
        // Configure focus management
        ConfigureFocusManagement(overlay);
        
        // Configure keyboard navigation
        await ConfigureKeyboardNavigationAsync(overlay);
        
        // Configure screen reader support
        ConfigureScreenReaderSupport(overlay, viewModel);
    }

    private void ConfigureFocusManagement(UserControl overlay)
    {
        // Find first focusable element
        var firstFocusable = overlay.GetLogicalDescendants()
            .OfType<Control>()
            .FirstOrDefault(c => c.Focusable && c.IsVisible && c.IsEnabled);
            
        if (firstFocusable != null)
        {
            overlay.Loaded += (sender, e) => firstFocusable.Focus();
        }

        // Configure focus trap
        overlay.KeyDown += (sender, e) =>
        {
            if (e.Key == Key.Tab)
            {
                HandleTabNavigation(overlay, e);
            }
        };
    }

    private void HandleTabNavigation(UserControl overlay, KeyEventArgs e)
    {
        var focusableElements = overlay.GetLogicalDescendants()
            .OfType<Control>()
            .Where(c => c.Focusable && c.IsVisible && c.IsEnabled)
            .ToList();

        if (focusableElements.Count == 0) return;

        var currentIndex = focusableElements.IndexOf(
            focusableElements.FirstOrDefault(c => c.IsFocused));

        int nextIndex;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            // Shift+Tab - previous element
            nextIndex = currentIndex <= 0 ? focusableElements.Count - 1 : currentIndex - 1;
        }
        else
        {
            // Tab - next element
            nextIndex = (currentIndex + 1) % focusableElements.Count;
        }

        focusableElements[nextIndex].Focus();
        e.Handled = true;
    }
}
```

### **Platform-Specific Accessibility Adapters**

```csharp
// File: Services/Platform/WindowsAccessibilityAdapter.cs

public class WindowsAccessibilityAdapter : IAccessibilityAdapter
{
    public PlatformType SupportedPlatform => PlatformType.Windows;

    public async Task ConfigurePlatformAccessibilityAsync(UserControl overlay, IOverlayViewModel viewModel)
    {
        // Windows UI Automation configuration
        overlay.AutomationProperties.SetAutomationId($"MTM_Overlay_{viewModel.OverlayId}");
        overlay.AutomationProperties.SetHelpText(viewModel.AccessibilityDescription);
        
        // High contrast mode support
        if (IsHighContrastMode())
        {
            await ApplyHighContrastThemeAsync(overlay);
        }
        
        // Narrator support
        ConfigureNarratorSupport(overlay);
        
        // Windows-specific keyboard shortcuts
        ConfigureWindowsAccessibilityShortcuts(overlay, viewModel);
    }

    private void ConfigureNarratorSupport(UserControl overlay)
    {
        // Configure live regions for dynamic content
        foreach (var element in overlay.GetLogicalDescendants().OfType<TextBlock>())
        {
            if (element.Name?.Contains("Status") == true || 
                element.Name?.Contains("Validation") == true)
            {
                element.AutomationProperties.SetLiveSetting(AutomationLiveSetting.Polite);
            }
        }
    }
}
```

## 🔧 Platform Detection and Configuration

### **Platform Detection Service**

```csharp
// File: Services/PlatformDetectionService.cs

public class PlatformDetectionService : IPlatformDetectionService
{
    private PlatformInfo? _cachedPlatformInfo;

    public async Task<PlatformInfo> DetectPlatformAsync()
    {
        if (_cachedPlatformInfo != null)
            return _cachedPlatformInfo;

        var platformInfo = new PlatformInfo
        {
            Type = DetectPlatformType(),
            Version = await DetectPlatformVersionAsync(),
            Architecture = DetectArchitecture(),
            Features = await DetectFeaturesAsync(),
            Capabilities = await DetectCapabilitiesAsync()
        };

        _cachedPlatformInfo = platformInfo;
        return platformInfo;
    }

    private PlatformType DetectPlatformType()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return PlatformType.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return PlatformType.macOS;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return PlatformType.Linux;
        
        // Mobile detection would require additional platform-specific code
        return PlatformType.Unknown;
    }

    private async Task<PlatformCapabilities> DetectCapabilitiesAsync()
    {
        return new PlatformCapabilities
        {
            TouchSupport = await DetectTouchSupportAsync(),
            StylusSupport = await DetectStylusSupportAsync(),
            MultiMonitorSupport = await DetectMultiMonitorSupportAsync(),
            HighDPISupport = await DetectHighDPISupportAsync(),
            HardwareAcceleration = await DetectHardwareAccelerationAsync(),
            WindowComposition = await DetectWindowCompositionAsync()
        };
    }
}

public record PlatformInfo
{
    public PlatformType Type { get; init; }
    public Version Version { get; init; } = new();
    public Architecture Architecture { get; init; }
    public PlatformFeatures Features { get; init; } = new();
    public PlatformCapabilities Capabilities { get; init; } = new();
}

public record PlatformCapabilities
{
    public bool TouchSupport { get; init; }
    public bool StylusSupport { get; init; }
    public bool MultiMonitorSupport { get; init; }
    public bool HighDPISupport { get; init; }
    public bool HardwareAcceleration { get; init; }
    public bool WindowComposition { get; init; }
}
```

## 📏 Responsive Design Patterns

### **Responsive Overlay Layout Service**

```csharp
// File: Services/ResponsiveLayoutService.cs

public class ResponsiveLayoutService : IResponsiveLayoutService
{
    public async Task AdaptOverlayForScreenAsync(UserControl overlay, ScreenInfo screenInfo)
    {
        var breakpoint = DetermineBreakpoint(screenInfo);
        
        await breakpoint switch
        {
            ScreenBreakpoint.Compact => AdaptForCompactScreenAsync(overlay, screenInfo),
            ScreenBreakpoint.Medium => AdaptForMediumScreenAsync(overlay, screenInfo),
            ScreenBreakpoint.Expanded => AdaptForExpandedScreenAsync(overlay, screenInfo),
            ScreenBreakpoint.Large => AdaptForLargeScreenAsync(overlay, screenInfo),
            _ => Task.CompletedTask
        };
    }

    private ScreenBreakpoint DetermineBreakpoint(ScreenInfo screenInfo)
    {
        var width = screenInfo.Width;
        
        return width switch
        {
            < 600 => ScreenBreakpoint.Compact,
            < 840 => ScreenBreakpoint.Medium,
            < 1200 => ScreenBreakpoint.Expanded,
            _ => ScreenBreakpoint.Large
        };
    }

    private async Task AdaptForCompactScreenAsync(UserControl overlay, ScreenInfo screenInfo)
    {
        // Mobile-first approach
        overlay.MaxWidth = screenInfo.Width - 32; // 16dp margin on each side
        overlay.MaxHeight = screenInfo.Height - 64; // Account for status bars
        
        // Single column layout
        await ApplyCompactLayoutAsync(overlay);
        
        // Larger touch targets
        await IncreaseTouchTargetSizesAsync(overlay);
        
        // Simplified UI
        await SimplifyUIForMobileAsync(overlay);
    }

    private async Task AdaptForMediumScreenAsync(UserControl overlay, ScreenInfo screenInfo)
    {
        // Tablet optimization
        overlay.MaxWidth = Math.Min(600, screenInfo.Width - 64);
        overlay.MaxHeight = screenInfo.Height - 128;
        
        // Two-column layout where appropriate
        await ApplyMediumLayoutAsync(overlay);
    }

    private async Task AdaptForExpandedScreenAsync(UserControl overlay, ScreenInfo screenInfo)
    {
        // Desktop optimization
        overlay.MaxWidth = Math.Min(800, screenInfo.Width * 0.8);
        overlay.MaxHeight = Math.Min(600, screenInfo.Height * 0.8);
        
        // Multi-column layout
        await ApplyExpandedLayoutAsync(overlay);
    }
}
```

## 🎯 Platform-Specific Testing

### **Cross-Platform Test Framework**

```csharp
// File: Tests/CrossPlatform/PlatformTestBase.cs

[TestClass]
public abstract class PlatformTestBase
{
    protected PlatformType CurrentPlatform { get; private set; }
    protected IPlatformService PlatformService { get; private set; }

    [TestInitialize]
    public virtual async Task Setup()
    {
        CurrentPlatform = DetectCurrentTestPlatform();
        PlatformService = CreatePlatformService(CurrentPlatform);
        
        await SetupPlatformSpecificTestEnvironmentAsync();
    }

    [TestMethod]
    public virtual async Task Overlay_ShouldRespectPlatformDesignGuidelines()
    {
        // Arrange
        var overlayConfig = await PlatformService.GetPlatformOverlayConfigurationAsync();
        var overlay = CreateTestOverlay(overlayConfig);

        // Act & Assert - Platform-specific validations
        await ValidatePlatformDesignGuidelinesAsync(overlay, overlayConfig);
    }

    [TestMethod]
    public virtual async Task Overlay_ShouldSupportPlatformInputMethods()
    {
        // Test touch, keyboard, stylus, etc. based on platform capabilities
        var capabilities = await PlatformService.GetPlatformCapabilitiesAsync();
        var overlay = CreateTestOverlay();

        if (capabilities.TouchSupport)
        {
            await TestTouchInteractionAsync(overlay);
        }

        if (capabilities.StylusSupport)
        {
            await TestStylusInteractionAsync(overlay);
        }

        await TestKeyboardInteractionAsync(overlay);
    }

    [TestMethod]
    [DataRow(ScreenBreakpoint.Compact)]
    [DataRow(ScreenBreakpoint.Medium)]
    [DataRow(ScreenBreakpoint.Expanded)]
    [DataRow(ScreenBreakpoint.Large)]
    public virtual async Task Overlay_ShouldAdaptToScreenSize(ScreenBreakpoint breakpoint)
    {
        // Arrange
        var screenInfo = CreateMockScreenInfo(breakpoint);
        var overlay = CreateTestOverlay();

        // Act
        await ResponsiveLayoutService.AdaptOverlayForScreenAsync(overlay, screenInfo);

        // Assert
        await ValidateResponsiveLayoutAsync(overlay, breakpoint);
    }

    protected abstract Task SetupPlatformSpecificTestEnvironmentAsync();
    protected abstract Task ValidatePlatformDesignGuidelinesAsync(UserControl overlay, OverlayConfiguration config);
    protected abstract Task TestTouchInteractionAsync(UserControl overlay);
    protected abstract Task TestStylusInteractionAsync(UserControl overlay);
}

// Windows-specific tests
[TestClass]
public class WindowsOverlayTests : PlatformTestBase
{
    protected override async Task ValidatePlatformDesignGuidelinesAsync(UserControl overlay, OverlayConfiguration config)
    {
        // Windows-specific design validation
        config.CornerRadius.Should().Be(8, "Windows overlays should use 8px corner radius");
        config.AnimationDuration.Should().Be(TimeSpan.FromMilliseconds(250), "Windows animations should be 250ms");
        
        // Validate Windows 11 design elements
        if (IsWindows11())
        {
            await ValidateWindows11DesignAsync(overlay);
        }
    }

    protected override async Task TestTouchInteractionAsync(UserControl overlay)
    {
        // Test Windows touch gestures
        await SimulateTouchTapAsync(overlay, new Point(100, 100));
        await SimulateTouchSwipeAsync(overlay, SwipeDirection.Right);
    }
}

// macOS-specific tests
[TestClass]
public class MacOSOverlayTests : PlatformTestBase
{
    protected override async Task ValidatePlatformDesignGuidelinesAsync(UserControl overlay, OverlayConfiguration config)
    {
        // macOS-specific design validation
        config.CornerRadius.Should().Be(12, "macOS overlays should use 12px corner radius");
        config.AnimationDuration.Should().Be(TimeSpan.FromMilliseconds(400), "macOS animations should be 400ms");
        
        // Validate vibrancy effects
        await ValidateVibrancyEffectsAsync(overlay);
    }

    protected override async Task TestTouchInteractionAsync(UserControl overlay)
    {
        // Test macOS trackpad gestures
        await SimulateTrackpadGestureAsync(overlay, TrackpadGesture.TwoFingerTap);
        await SimulateTrackpadGestureAsync(overlay, TrackpadGesture.Swipe);
    }
}
```

## 🎯 Best Practices Summary

### **Cross-Platform Development Guidelines**

1. **Design System Respect**
   - Follow each platform's design guidelines (Windows 11, macOS Human Interface, Material Design)
   - Adapt colors, spacing, typography, and animations per platform
   - Use platform-appropriate corner radius and elevation values

2. **Input Method Support**
   - Support touch, mouse, keyboard, and stylus where available
   - Implement platform-specific gestures (swipe-to-dismiss, long-press menus)
   - Ensure minimum touch target sizes (44dp iOS, 48dp Android, 32px Windows)

3. **Accessibility Compliance**
   - Support screen readers on all platforms (Narrator, VoiceOver, Orca)
   - Implement proper keyboard navigation and focus management
   - Respect high contrast and reduce motion settings

4. **Performance Optimization**
   - Use hardware acceleration where available
   - Optimize for different screen densities and sizes
   - Implement efficient resource management per platform

5. **Testing Strategy**
   - Create platform-specific test suites
   - Test on actual devices, not just emulators
   - Validate accessibility features on each platform
   - Performance test across different hardware capabilities

This comprehensive cross-platform guide ensures MTM overlay system provides consistent functionality while respecting platform conventions and user expectations across all supported platforms.
