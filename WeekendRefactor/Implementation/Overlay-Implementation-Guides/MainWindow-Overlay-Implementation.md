# MainWindow Overlay Implementation Guide

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Audience**: MTM Application Development Team  

## 🎯 MainWindow Overlay Implementation Overview

This document provides detailed task-based implementation guidance for adding overlay functionality to MainWindow, including emergency overlays, connection status, theme switching, and application-level notifications.

## 📊 Current MainWindow Analysis

### **Current Implementation Status**

- **Location**: `MainWindow.axaml` / `MainWindow.axaml.cs`
- **ViewModel**: `ViewModels/Application/MainWindowViewModel.cs` (after reorganization)
- **Current Overlays**: Limited emergency keyboard hook functionality
- **Missing Overlays**: Connection status, theme switcher, emergency dialogs, shutdown confirmation

### **MainWindow Responsibilities**

- Application window management
- Emergency keyboard hook integration
- Global application state display
- Theme management UI access
- Critical system notifications

## 🏗️ Required Overlay Implementations

### **1. Connection Status Overlay**

**Purpose**: Display database connection status and provide reconnection options

**Implementation Tasks:**

#### **Task 1.1: Create Connection Status Overlay ViewModel**

```csharp
// File: ViewModels/Application/Overlays/ConnectionStatusOverlayViewModel.cs
namespace MTM_WIP_Application_Avalonia.ViewModels.Application.Overlays;

[ObservableObject]
public partial class ConnectionStatusOverlayViewModel : BasePoolableOverlayViewModel
{
    [ObservableProperty]
    private ConnectionStatus currentStatus = ConnectionStatus.Unknown;

    [ObservableProperty]
    private string statusMessage = "Checking connection...";

    [ObservableProperty]
    private bool isReconnecting;

    [ObservableProperty]
    private DateTime lastSuccessfulConnection;

    [ObservableProperty]
    private int reconnectAttempts;

    private readonly IConfigurationService _configurationService;
    private readonly IDatabaseService _databaseService;
    private readonly Timer _connectionCheckTimer;

    public ConnectionStatusOverlayViewModel(
        ILogger<ConnectionStatusOverlayViewModel> logger,
        IConfigurationService configurationService,
        IDatabaseService databaseService)
        : base(logger)
    {
        _configurationService = configurationService;
        _databaseService = databaseService;
        
        _connectionCheckTimer = new Timer(CheckConnectionStatus, null, 
            TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        IsReconnecting = true;
        try
        {
            Logger.LogInformation("Testing database connection manually");
            
            var isConnected = await _databaseService.TestConnectionAsync();
            
            if (isConnected)
            {
                CurrentStatus = ConnectionStatus.Connected;
                StatusMessage = "Database connection successful";
                LastSuccessfulConnection = DateTime.Now;
                ReconnectAttempts = 0;
            }
            else
            {
                CurrentStatus = ConnectionStatus.Disconnected;
                StatusMessage = "Database connection failed";
                ReconnectAttempts++;
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Manual connection test failed");
            CurrentStatus = ConnectionStatus.Error;
            StatusMessage = $"Connection error: {ex.Message}";
        }
        finally
        {
            IsReconnecting = false;
        }
    }

    [RelayCommand]
    private async Task RetryConnectionAsync()
    {
        await TestConnectionAsync();
    }

    [RelayCommand]
    private void ShowDatabaseSettings()
    {
        // Navigate to database settings
        Logger.LogInformation("Navigating to database settings");
    }

    private async void CheckConnectionStatus(object? state)
    {
        if (IsReconnecting) return;

        try
        {
            var isConnected = await _databaseService.TestConnectionAsync();
            
            var newStatus = isConnected ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
            
            if (newStatus != CurrentStatus)
            {
                CurrentStatus = newStatus;
                StatusMessage = isConnected 
                    ? "Database connection restored" 
                    : "Database connection lost";
                
                if (isConnected)
                {
                    LastSuccessfulConnection = DateTime.Now;
                    ReconnectAttempts = 0;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Connection status check failed");
            CurrentStatus = ConnectionStatus.Error;
        }
    }

    protected override void OnRequestReceived(OverlayRequestBase request)
    {
        if (request is ConnectionStatusOverlayRequest statusRequest)
        {
            // Update display based on request
            _ = TestConnectionAsync();
        }
    }

    public override void Dispose()
    {
        _connectionCheckTimer?.Dispose();
        base.Dispose();
    }
}

public enum ConnectionStatus
{
    Unknown,
    Connected,
    Disconnected,
    Error,
    Reconnecting
}
```

#### **Task 1.2: Create Connection Status Overlay View**

```xml
<!-- File: Views/Application/Overlays/ConnectionStatusOverlayView.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Application.Overlays"
             x:Class="MTM_WIP_Application_Avalonia.Views.Application.Overlays.ConnectionStatusOverlayView">
  
  <Border Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
          BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
          BorderThickness="2" CornerRadius="8" Padding="16">
    
    <Grid x:Name="MainContainer" RowDefinitions="Auto,*,Auto" MaxWidth="400">
      
      <!-- Header -->
      <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.HeaderBackgroundBrush}"
              CornerRadius="4" Padding="12,8" Margin="0,0,0,16">
        <TextBlock Text="Database Connection Status" 
                   FontSize="16" FontWeight="Bold"
                   Foreground="{DynamicResource MTM_Shared_Logic.HeaderForegroundBrush}"
                   HorizontalAlignment="Center"/>
      </Border>
      
      <!-- Status Content -->
      <StackPanel Grid.Row="1" Spacing="16">
        
        <!-- Status Indicator -->
        <Grid x:Name="StatusIndicator" ColumnDefinitions="Auto,*">
          
          <!-- Status Icon -->
          <Border Grid.Column="0" Width="24" Height="24" CornerRadius="12"
                  Background="{Binding CurrentStatus, Converter={StaticResource ConnectionStatusToColorConverter}}"
                  Margin="0,0,12,0" VerticalAlignment="Center">
            <TextBlock Text="{Binding CurrentStatus, Converter={StaticResource ConnectionStatusToIconConverter}}"
                       FontSize="14" FontWeight="Bold" Foreground="White"
                       HorizontalAlignment="Center" VerticalAlignment="Center"/>
          </Border>
          
          <!-- Status Text -->
          <StackPanel Grid.Column="1" VerticalAlignment="Center">
            <TextBlock Text="{Binding StatusMessage}" FontWeight="SemiBold"/>
            <TextBlock Text="{Binding LastSuccessfulConnection, StringFormat='Last connected: {0:HH:mm:ss}'}"
                       FontSize="12" Foreground="{DynamicResource MTM_Shared_Logic.SecondaryTextBrush}"
                       IsVisible="{Binding CurrentStatus, Converter={StaticResource EqualityConverter}, ConverterParameter=Connected}"/>
          </StackPanel>
        </Grid>
        
        <!-- Connection Details -->
        <Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
                BorderThickness="1" CornerRadius="4" Padding="12">
          <StackPanel Spacing="8">
            <TextBlock Text="Connection Details" FontWeight="SemiBold"/>
            <TextBlock Text="{Binding ReconnectAttempts, StringFormat='Reconnect attempts: {0}'}" 
                       FontSize="12"/>
            <TextBlock Text="Auto-checking every 30 seconds" 
                       FontSize="12" 
                       Foreground="{DynamicResource MTM_Shared_Logic.SecondaryTextBrush}"/>
          </StackPanel>
        </Border>
        
        <!-- Loading Indicator -->
        <ProgressBar IsIndeterminate="True" 
                     IsVisible="{Binding IsReconnecting}"
                     Height="4" 
                     Foreground="{DynamicResource MTM_Shared_Logic.AccentBrush}"/>
      </StackPanel>
      
      <!-- Action Buttons -->
      <Grid Grid.Row="2" ColumnDefinitions="*,Auto,Auto" Margin="0,16,0,0">
        
        <!-- Settings Button -->
        <Button Grid.Column="0" Content="Database Settings"
                Command="{Binding ShowDatabaseSettingsCommand}"
                Background="Transparent"
                BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
                Padding="12,8" HorizontalAlignment="Left"/>
        
        <!-- Retry Button -->
        <Button Grid.Column="1" Content="Test Connection"
                Command="{Binding TestConnectionCommand}"
                Background="{DynamicResource MTM_Shared_Logic.AccentBrush}"
                Foreground="White"
                Padding="16,8" Margin="8,0"/>
        
        <!-- Close Button -->
        <Button Grid.Column="2" Content="Close"
                Command="{Binding CloseCommand}"
                Background="{DynamicResource MTM_Shared_Logic.PrimaryBrush}"
                Foreground="White"
                Padding="16,8"/>
      </Grid>
    </Grid>
  </Border>
</UserControl>
```

### **Task 1.3: Add Connection Status Integration to MainWindow**

```csharp
// Update MainWindowViewModel.cs
[RelayCommand]
private async Task ShowConnectionStatusAsync()
{
    try
    {
        var request = new ConnectionStatusOverlayRequest();
        
        var response = await _overlayService.ShowOverlayAsync(
            typeof(ConnectionStatusOverlayRequest),
            typeof(ConnectionStatusOverlayResponse),
            typeof(ConnectionStatusOverlayViewModel),
            request
        );
        
        if (response.Result == OverlayResult.Confirmed)
        {
            Logger.LogInformation("Connection status overlay completed");
        }
    }
    catch (Exception ex)
    {
        await ErrorHandling.HandleErrorAsync(ex, "Failed to show connection status");
    }
}
```

### **2. Emergency Shutdown Confirmation Overlay**

**Purpose**: Provide safe application shutdown with pending work warnings

#### **Task 2.1: Create Emergency Shutdown Overlay ViewModel**

```csharp
// File: ViewModels/Application/Overlays/EmergencyShutdownOverlayViewModel.cs
[ObservableObject]
public partial class EmergencyShutdownOverlayViewModel : BasePoolableOverlayViewModel
{
    [ObservableProperty]
    private bool hasPendingWork;

    [ObservableProperty]
    private List<string> pendingWorkItems = new();

    [ObservableProperty]
    private bool forceShutdown;

    [ObservableProperty]
    private int shutdownCountdown = 10;

    private readonly IApplicationStateService _applicationStateService;
    private Timer? _countdownTimer;

    public EmergencyShutdownOverlayViewModel(
        ILogger<EmergencyShutdownOverlayViewModel> logger,
        IApplicationStateService applicationStateService)
        : base(logger)
    {
        _applicationStateService = applicationStateService;
    }

    [RelayCommand]
    private async Task CheckPendingWorkAsync()
    {
        try
        {
            // Check for pending work across the application
            var pendingItems = new List<string>();

            // Check for unsaved changes
            // TODO: Implement checks based on application state
            
            PendingWorkItems = pendingItems;
            HasPendingWork = pendingItems.Count > 0;

            if (!HasPendingWork && !ForceShutdown)
            {
                // Safe to shutdown immediately
                await ConfirmShutdownAsync();
            }
            else if (ForceShutdown)
            {
                // Start countdown for force shutdown
                StartShutdownCountdown();
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Failed to check pending work");
        }
    }

    [RelayCommand]
    private async Task ConfirmShutdownAsync()
    {
        Logger.LogInformation("User confirmed application shutdown");
        CompleteOverlay(new EmergencyShutdownOverlayResponse 
        { 
            Result = OverlayResult.Confirmed,
            ProceedWithShutdown = true
        });
    }

    [RelayCommand]
    private void CancelShutdown()
    {
        Logger.LogInformation("User cancelled application shutdown");
        _countdownTimer?.Dispose();
        CompleteOverlay(new EmergencyShutdownOverlayResponse 
        { 
            Result = OverlayResult.Cancelled,
            ProceedWithShutdown = false
        });
    }

    [RelayCommand]
    private async Task SaveAndShutdownAsync()
    {
        try
        {
            // Attempt to save pending work
            Logger.LogInformation("Attempting to save pending work before shutdown");
            
            // TODO: Implement save logic based on pending work
            
            await ConfirmShutdownAsync();
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Failed to save pending work");
        }
    }

    private void StartShutdownCountdown()
    {
        _countdownTimer = new Timer(CountdownTick, null, 
            TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private void CountdownTick(object? state)
    {
        ShutdownCountdown--;
        
        if (ShutdownCountdown <= 0)
        {
            _countdownTimer?.Dispose();
            
            // Force shutdown
            Application.Current?.Dispatcher.Invoke(async () =>
            {
                await ConfirmShutdownAsync();
            });
        }
    }

    protected override void OnRequestReceived(OverlayRequestBase request)
    {
        if (request is EmergencyShutdownOverlayRequest shutdownRequest)
        {
            ForceShutdown = shutdownRequest.IsEmergencyShutdown;
            _ = CheckPendingWorkAsync();
        }
    }

    public override void Dispose()
    {
        _countdownTimer?.Dispose();
        base.Dispose();
    }
}
```

### **3. Theme Quick Switcher Overlay**

**Purpose**: Quick theme switching without opening full settings

#### **Task 3.1: Create Theme Quick Switcher ViewModel**

```csharp
// File: ViewModels/Application/Overlays/ThemeQuickSwitcherOverlayViewModel.cs
[ObservableObject]
public partial class ThemeQuickSwitcherOverlayViewModel : BasePoolableOverlayViewModel
{
    [ObservableProperty]
    private List<ThemeInfo> availableThemes = new();

    [ObservableProperty]
    private ThemeInfo? selectedTheme;

    [ObservableProperty]
    private bool isApplyingTheme;

    private readonly IThemeService _themeService;

    public ThemeQuickSwitcherOverlayViewModel(
        ILogger<ThemeQuickSwitcherOverlayViewModel> logger,
        IThemeService themeService)
        : base(logger)
    {
        _themeService = themeService;
    }

    [RelayCommand]
    private async Task LoadAvailableThemesAsync()
    {
        try
        {
            // Load available themes from theme service
            var themes = await _themeService.GetAvailableThemesAsync();
            AvailableThemes = themes.ToList();

            // Set current theme as selected
            var currentTheme = await _themeService.GetCurrentThemeAsync();
            SelectedTheme = AvailableThemes.FirstOrDefault(t => t.Name == currentTheme);
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Failed to load available themes");
        }
    }

    [RelayCommand]
    private async Task ApplyThemeAsync(ThemeInfo theme)
    {
        if (theme == null) return;

        IsApplyingTheme = true;
        try
        {
            Logger.LogInformation("Applying theme: {ThemeName}", theme.Name);
            
            await _themeService.SetThemeAsync(theme.Name);
            SelectedTheme = theme;
            
            // Brief delay to show application of theme
            await Task.Delay(500);
            
            CompleteOverlay(new ThemeQuickSwitcherOverlayResponse
            {
                Result = OverlayResult.Confirmed,
                SelectedTheme = theme
            });
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, $"Failed to apply theme: {theme.Name}");
        }
        finally
        {
            IsApplyingTheme = false;
        }
    }

    [RelayCommand]
    private void CloseThemeSwitcher()
    {
        CompleteOverlay(new ThemeQuickSwitcherOverlayResponse
        {
            Result = OverlayResult.Cancelled
        });
    }

    protected override void OnRequestReceived(OverlayRequestBase request)
    {
        if (request is ThemeQuickSwitcherOverlayRequest)
        {
            _ = LoadAvailableThemesAsync();
        }
    }
}

public class ThemeInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PreviewColor { get; set; } = "#0078D4";
    public bool IsDarkTheme { get; set; }
}
```

## 🔄 MainWindow Integration Tasks

### **Task 4: Update MainWindow AXAML Structure**

**Add overlay container to MainWindow.axaml:**

```xml
<Grid x:Name="MainWindowGrid">
  <!-- Existing MainWindow content -->
  <ContentPresenter x:Name="MainContent" Content="{Binding CurrentView}"/>
  
  <!-- Overlay Container -->
  <Panel x:Name="MainWindowOverlayContainer" 
         IsVisible="False" 
         Background="#80000000"
         ZIndex="1000">
    
    <!-- Overlay Content -->
    <Border Background="Transparent" 
            HorizontalAlignment="Stretch" 
            VerticalAlignment="Stretch">
      
      <ContentPresenter x:Name="OverlayContentPresenter"
                        HorizontalAlignment="Center"
                        VerticalAlignment="Center"/>
    </Border>
  </Panel>
  
  <!-- Connection Status Indicator -->
  <Border x:Name="ConnectionStatusIndicator"
          Background="{Binding ConnectionStatus, Converter={StaticResource ConnectionStatusToColorConverter}}"
          Width="12" Height="12" CornerRadius="6"
          HorizontalAlignment="Right" VerticalAlignment="Top"
          Margin="0,8,8,0" ZIndex="999"
          ToolTip.Tip="{Binding ConnectionStatusMessage}">
    
    <Border.PointerPressed>
      <EventTrigger>
        <InvokeCommandAction Command="{Binding ShowConnectionStatusCommand}"/>
      </EventTrigger>
    </Border.PointerPressed>
  </Border>
</Grid>
```

### **Task 5: Update MainWindowViewModel Integration**

**Add overlay service integration:**

```csharp
public partial class MainWindowViewModel : BaseViewModel
{
    [ObservableProperty]
    private ConnectionStatus connectionStatus = ConnectionStatus.Unknown;

    [ObservableProperty]
    private string connectionStatusMessage = "Checking...";

    private readonly IUniversalOverlayService _overlayService;
    private readonly IThemeService _themeService;

    // Keyboard shortcuts for overlays
    [RelayCommand]
    private async Task HandleKeyboardShortcutAsync(string shortcut)
    {
        switch (shortcut.ToUpperInvariant())
        {
            case "CTRL+ALT+C":
                await ShowConnectionStatusAsync();
                break;
            case "CTRL+ALT+T":
                await ShowThemeQuickSwitcherAsync();
                break;
            case "CTRL+ALT+Q":
                await ShowEmergencyShutdownAsync();
                break;
        }
    }

    [RelayCommand]
    private async Task ShowThemeQuickSwitcherAsync()
    {
        try
        {
            var request = new ThemeQuickSwitcherOverlayRequest();
            
            var response = await _overlayService.ShowOverlayAsync(
                typeof(ThemeQuickSwitcherOverlayRequest),
                typeof(ThemeQuickSwitcherOverlayResponse),
                typeof(ThemeQuickSwitcherOverlayViewModel),
                request
            );
            
            if (response.Result == OverlayResult.Confirmed && 
                response is ThemeQuickSwitcherOverlayResponse themeResponse)
            {
                Logger.LogInformation("Theme applied: {ThemeName}", 
                    themeResponse.SelectedTheme?.DisplayName);
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Failed to show theme switcher");
        }
    }

    [RelayCommand]
    private async Task ShowEmergencyShutdownAsync()
    {
        try
        {
            var request = new EmergencyShutdownOverlayRequest
            {
                IsEmergencyShutdown = false
            };
            
            var response = await _overlayService.ShowOverlayAsync(
                typeof(EmergencyShutdownOverlayRequest),
                typeof(EmergencyShutdownOverlayResponse),
                typeof(EmergencyShutdownOverlayViewModel),
                request
            );
            
            if (response.Result == OverlayResult.Confirmed && 
                response is EmergencyShutdownOverlayResponse shutdownResponse &&
                shutdownResponse.ProceedWithShutdown)
            {
                Logger.LogInformation("Proceeding with application shutdown");
                Application.Current?.Shutdown();
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Failed to show shutdown confirmation");
        }
    }
}
```

## 📋 Implementation Checklist

### **Phase 1: Connection Status Overlay**

- [ ] Create ConnectionStatusOverlayViewModel with status checking
- [ ] Create ConnectionStatusOverlayView with status display
- [ ] Add connection status indicator to MainWindow
- [ ] Test connection status overlay functionality
- [ ] Add keyboard shortcut (Ctrl+Alt+C)

### **Phase 2: Emergency Shutdown Overlay**

- [ ] Create EmergencyShutdownOverlayViewModel with work checking
- [ ] Create EmergencyShutdownOverlayView with confirmation UI
- [ ] Add emergency shutdown integration to MainWindow
- [ ] Test shutdown confirmation functionality
- [ ] Add keyboard shortcut (Ctrl+Alt+Q)

### **Phase 3: Theme Quick Switcher**

- [ ] Create ThemeQuickSwitcherOverlayViewModel
- [ ] Create ThemeQuickSwitcherOverlayView
- [ ] Add theme switching integration to MainWindow
- [ ] Test theme switching functionality
- [ ] Add keyboard shortcut (Ctrl+Alt+T)

### **Phase 4: MainWindow Integration**

- [ ] Update MainWindow AXAML with overlay container
- [ ] Update MainWindowViewModel with overlay service
- [ ] Add keyboard shortcut handling
- [ ] Test all overlay integrations
- [ ] Verify overlay positioning and styling

### **Phase 5: Testing and Validation**

- [ ] Unit tests for all overlay ViewModels
- [ ] Integration tests for MainWindow overlays
- [ ] UI automation tests for overlay interactions
- [ ] Performance testing for overlay loading
- [ ] Accessibility testing for overlay navigation

## ✅ Success Criteria

### **Functionality**

- [ ] Connection status overlay displays current database status
- [ ] Emergency shutdown overlay safely handles application closure
- [ ] Theme quick switcher applies themes immediately
- [ ] All overlays properly integrate with MainWindow
- [ ] Keyboard shortcuts work correctly

### **User Experience**

- [ ] Overlays appear centered and properly styled
- [ ] Clear visual feedback for all operations
- [ ] Consistent MTM styling throughout
- [ ] Smooth animations and transitions
- [ ] Accessible via keyboard navigation

### **Technical Quality**

- [ ] All overlays use Universal Overlay Service
- [ ] Proper error handling in all scenarios
- [ ] Memory management and disposal
- [ ] Logging for debugging and monitoring
- [ ] Following MTM architectural patterns

This implementation will provide MainWindow with essential overlay functionality while maintaining consistency with the overall MTM overlay system architecture.
