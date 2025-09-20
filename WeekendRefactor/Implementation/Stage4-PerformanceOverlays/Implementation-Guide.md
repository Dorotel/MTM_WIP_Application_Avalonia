# Stage 4: Performance Overlays & View Integration - Implementation Guide

**Priority**: 🟡 High  
**Timeline**: Weekend Day 2-3  
**Estimated Time**: 6-8 hours  

## 🎯 Overview

Stage 4 focuses on implementing performance-critical overlays and integrating overlay systems into existing views. This stage enhances user experience through progress indicators, connection monitoring, and systematic view integration.

## 📋 Task Breakdown

### **Task 4.1: Global Progress Overlay Implementation**

**Estimated Time**: 2.5 hours  
**Risk Level**: Medium  
**Dependencies**: Stage 2 Universal Service complete  

#### **Implementation Specification**

```csharp
// Progress Overlay Request Model
public record ProgressOverlayRequest(
    string Title,
    string InitialStep = "",
    bool CanCancel = true,
    bool ShowPercentage = true,
    bool ShowETA = false
) : BaseOverlayRequest;

// Progress Overlay Response Model  
public record ProgressOverlayResponse(
    OverlayResult Result,
    bool WasCancelled = false,
    string? CancellationReason = null
) : BaseOverlayResponse;

// Progress Overlay ViewModel
[ObservableObject]
public partial class ProgressOverlayViewModel : BaseViewModel
{
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string currentStep = string.Empty;
    [ObservableProperty] private double progressPercentage = 0.0;
    [ObservableProperty] private bool canCancel = true;
    [ObservableProperty] private bool showPercentage = true;
    [ObservableProperty] private bool showEta = false;
    [ObservableProperty] private TimeSpan estimatedTimeRemaining = TimeSpan.Zero;
    [ObservableProperty] private string progressDetails = string.Empty;
    [ObservableProperty] private bool isIndeterminate = false;
    [ObservableProperty] private bool isVisible = false;

    // Cancellation support
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    [RelayCommand]
    private async Task CancelOperationAsync()
    {
        if (CanCancel)
        {
            _cancellationTokenSource.Cancel();
            IsVisible = false;
            await CloseAsync(new ProgressOverlayResponse(
                OverlayResult.Cancelled, 
                WasCancelled: true, 
                CancellationReason: "User cancelled"));
        }
    }

    // Progress update methods
    public async Task UpdateProgressAsync(double percentage, string step, string details = "")
    {
        await Task.Run(() =>
        {
            ProgressPercentage = Math.Clamp(percentage, 0, 100);
            CurrentStep = step;
            ProgressDetails = details;
            IsIndeterminate = false;
            
            // Calculate ETA if enabled and has progress
            if (ShowEta && percentage > 0)
            {
                EstimatedTimeRemaining = CalculateETA(percentage);
            }
        });
    }

    private TimeSpan CalculateETA(double currentProgress)
    {
        // Simple ETA calculation based on elapsed time and current progress
        var elapsed = DateTime.Now - _startTime;
        var remainingProgress = 100 - currentProgress;
        var progressRate = currentProgress / elapsed.TotalSeconds;
        
        if (progressRate > 0)
        {
            var remainingSeconds = remainingProgress / progressRate;
            return TimeSpan.FromSeconds(remainingSeconds);
        }
        
        return TimeSpan.Zero;
    }

    private readonly DateTime _startTime = DateTime.Now;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cancellationTokenSource?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

#### **AXAML Implementation**

```xml
<!-- Views/Overlay/ProgressOverlayView.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Overlay"
             x:Class="MTM_WIP_Application_Avalonia.Views.Overlay.ProgressOverlayView"
             x:DataType="vm:ProgressOverlayViewModel">

  <!-- Overlay background with proper z-index -->
  <Border IsVisible="{Binding IsVisible}"
          Background="#80000000"
          x:Name="OverlayContainer">
    
    <!-- Main progress card -->
    <Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
            BorderThickness="2"
            CornerRadius="12"
            Padding="24"
            MinWidth="400"
            MaxWidth="600"
            HorizontalAlignment="Center"
            VerticalAlignment="Center">
      
      <Grid x:Name="ProgressContent" RowDefinitions="Auto,Auto,*,Auto,Auto">
        
        <!-- Title -->
        <TextBlock Grid.Row="0"
                   Text="{Binding Title}"
                   FontSize="18"
                   FontWeight="Bold"
                   Foreground="{DynamicResource MTM_Shared_Logic.PrimaryTextBrush}"
                   TextAlignment="Center"
                   Margin="0,0,0,16" />
        
        <!-- Current step -->
        <TextBlock Grid.Row="1"
                   Text="{Binding CurrentStep}"
                   FontSize="14"
                   Foreground="{DynamicResource MTM_Shared_Logic.SecondaryTextBrush}"
                   TextAlignment="Center"
                   Margin="0,0,0,12"
                   TextWrapping="Wrap" />
        
        <!-- Progress bar -->
        <StackPanel Grid.Row="2" Spacing="8" Margin="0,0,0,16">
          
          <!-- Percentage progress bar -->
          <ProgressBar IsVisible="{Binding ShowPercentage}"
                       Value="{Binding ProgressPercentage}"
                       Maximum="100"
                       Height="8"
                       Background="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
                       Foreground="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
          
          <!-- Indeterminate progress bar -->
          <ProgressBar IsVisible="{Binding IsIndeterminate}"
                       IsIndeterminate="True"
                       Height="8"
                       Background="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
                       Foreground="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
          
          <!-- Progress percentage and ETA -->
          <Grid x:Name="ProgressStats" ColumnDefinitions="*,*">
            
            <!-- Percentage display -->
            <TextBlock Grid.Column="0"
                       IsVisible="{Binding ShowPercentage}"
                       Text="{Binding ProgressPercentage, StringFormat='{}{0:F0}%'}"
                       FontSize="12"
                       Foreground="{DynamicResource MTM_Shared_Logic.SecondaryTextBrush}"
                       HorizontalAlignment="Left" />
            
            <!-- ETA display -->
            <TextBlock Grid.Column="1"
                       IsVisible="{Binding ShowEta}"
                       Text="{Binding EstimatedTimeRemaining, StringFormat='ETA: {0:mm\\:ss}'}"
                       FontSize="12"
                       Foreground="{DynamicResource MTM_Shared_Logic.SecondaryTextBrush}"
                       HorizontalAlignment="Right" />
          </Grid>
        </StackPanel>
        
        <!-- Progress details -->
        <TextBlock Grid.Row="3"
                   Text="{Binding ProgressDetails}"
                   FontSize="12"
                   Foreground="{DynamicResource MTM_Shared_Logic.SecondaryTextBrush}"
                   TextAlignment="Center"
                   TextWrapping="Wrap"
                   Margin="0,0,0,16"
                   IsVisible="{Binding ProgressDetails, Converter={StaticResource StringNotNullOrEmptyConverter}}" />
        
        <!-- Cancel button -->
        <Button Grid.Row="4"
                IsVisible="{Binding CanCancel}"
                Content="Cancel Operation"
                Command="{Binding CancelOperationCommand}"
                Background="{DynamicResource MTM_Shared_Logic.SecondaryAction}"
                Foreground="White"
                Padding="16,8"
                CornerRadius="4"
                HorizontalAlignment="Center" />
      </Grid>
    </Border>
  </Border>
</UserControl>
```

#### **Service Integration**

```csharp
// Add to IUniversalOverlayService
public interface IUniversalOverlayService
{
    // ... existing methods
    
    Task<IProgressTracker> ShowProgressAsync(ProgressOverlayRequest request);
    Task HideProgressAsync();
}

// Progress tracker for ongoing operations
public interface IProgressTracker : IDisposable
{
    Task UpdateProgressAsync(double percentage, string step, string details = "");
    Task CompleteAsync(string completionMessage = "Operation completed successfully");
    Task CancelAsync(string reason = "Operation cancelled");
    CancellationToken CancellationToken { get; }
}
```

#### **Implementation Steps**

1. **Create Progress Overlay Models** (30 minutes)
   - Create `ProgressOverlayRequest` and `ProgressOverlayResponse` records
   - Add to Universal Overlay Service interface

2. **Implement ProgressOverlayViewModel** (60 minutes)
   - Follow MVVM Community Toolkit patterns
   - Add cancellation support
   - Implement ETA calculation

3. **Create Progress Overlay AXAML** (45 minutes)
   - Follow MTM design system
   - Support all theme variations
   - Implement responsive layout

4. **Add Service Integration** (30 minutes)
   - Update `IUniversalOverlayService` interface
   - Create `IProgressTracker` implementation
   - Add service registration

5. **Testing and Validation** (15 minutes)
   - Unit tests for ViewModel
   - Theme compatibility testing
   - Cancellation behavior testing

---

### **Task 4.2: Connection Status Overlay**

**Estimated Time**: 1.5 hours  
**Risk Level**: Low  
**Dependencies**: Database helper patterns  

#### **Implementation Specification**

```csharp
// Connection Status Overlay ViewModel
[ObservableObject]  
public partial class ConnectionStatusOverlayViewModel : BaseViewModel
{
    [ObservableProperty] private ConnectionState connectionState = ConnectionState.Unknown;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string connectionDetails = string.Empty;
    [ObservableProperty] private DateTime lastConnectionCheck = DateTime.Now;
    [ObservableProperty] private bool isRetrying = false;
    [ObservableProperty] private int retryAttempts = 0;
    [ObservableProperty] private bool showRetryButton = false;
    [ObservableProperty] private bool isVisible = false;

    private readonly Timer _connectionMonitorTimer;
    private readonly IConfigurationService _configurationService;

    public ConnectionStatusOverlayViewModel(
        ILogger<ConnectionStatusOverlayViewModel> logger,
        IConfigurationService configurationService) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(configurationService);
        
        _configurationService = configurationService;
        
        // Monitor connection every 30 seconds
        _connectionMonitorTimer = new Timer(CheckConnectionStatus, null, 
            TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }

    [RelayCommand]
    private async Task RetryConnectionAsync()
    {
        IsRetrying = true;
        ShowRetryButton = false;
        
        try
        {
            await CheckConnectionStatus();
            RetryAttempts++;
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Connection retry failed");
        }
        finally
        {
            IsRetrying = false;
        }
    }

    private async void CheckConnectionStatus(object? state)
    {
        try
        {
            var connectionString = await _configurationService.GetConnectionStringAsync();
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "sys_connection_Test",  // Test stored procedure
                Array.Empty<MySqlParameter>()
            );

            await UpdateConnectionState(result.Status == 1 ? 
                ConnectionState.Connected : ConnectionState.Disconnected);
        }
        catch (Exception ex)
        {
            await UpdateConnectionState(ConnectionState.Error);
            Logger.LogWarning(ex, "Connection status check failed");
        }
    }

    private async Task UpdateConnectionState(ConnectionState newState)
    {
        await Task.Run(() =>
        {
            ConnectionState = newState;
            LastConnectionCheck = DateTime.Now;
            
            (StatusMessage, ConnectionDetails, ShowRetryButton) = newState switch
            {
                ConnectionState.Connected => (
                    "Database Connected", 
                    $"Last verified: {LastConnectionCheck:HH:mm:ss}", 
                    false),
                ConnectionState.Disconnected => (
                    "Database Disconnected", 
                    "Unable to connect to MTM database", 
                    true),
                ConnectionState.Error => (
                    "Connection Error", 
                    "Database connection error occurred", 
                    true),
                ConnectionState.Retrying => (
                    "Reconnecting...", 
                    $"Attempt {RetryAttempts + 1}", 
                    false),
                _ => ("Unknown Status", "Connection status unknown", false)
            };

            // Show overlay for problematic states
            IsVisible = newState != ConnectionState.Connected;
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connectionMonitorTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
}

public enum ConnectionState
{
    Unknown,
    Connected,
    Disconnected,
    Error,
    Retrying
}
```

#### **AXAML Implementation**

```xml
<!-- Views/Overlay/ConnectionStatusOverlayView.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Overlay"
             x:Class="MTM_WIP_Application_Avalonia.Views.Overlay.ConnectionStatusOverlayView"
             x:DataType="vm:ConnectionStatusOverlayViewModel">

  <!-- Connection status banner -->
  <Border IsVisible="{Binding IsVisible}"
          Background="{Binding ConnectionState, Converter={StaticResource ConnectionStateToColorConverter}}"
          BorderThickness="0,0,0,2"
          BorderBrush="{Binding ConnectionState, Converter={StaticResource ConnectionStateToBorderConverter}}"
          HorizontalAlignment="Stretch"
          VerticalAlignment="Top">
    
    <Grid x:Name="StatusContent" 
          ColumnDefinitions="Auto,*,Auto" 
          Margin="16,8">
      
      <!-- Status icon -->
      <Border Grid.Column="0"
              Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
              CornerRadius="12"
              Width="24"
              Height="24"
              Margin="0,0,12,0">
        
        <TextBlock Text="{Binding ConnectionState, Converter={StaticResource ConnectionStateToIconConverter}}"
                   FontFamily="{StaticResource MaterialIcons}"
                   FontSize="16"
                   Foreground="White"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
      </Border>
      
      <!-- Status message -->
      <StackPanel Grid.Column="1" VerticalAlignment="Center">
        <TextBlock Text="{Binding StatusMessage}"
                   FontSize="14"
                   FontWeight="Bold"
                   Foreground="White" />
        
        <TextBlock Text="{Binding ConnectionDetails}"
                   FontSize="12"
                   Foreground="White"
                   Opacity="0.8"
                   IsVisible="{Binding ConnectionDetails, Converter={StaticResource StringNotNullOrEmptyConverter}}" />
      </StackPanel>
      
      <!-- Action buttons -->
      <StackPanel Grid.Column="2" 
                  Orientation="Horizontal" 
                  Spacing="8"
                  VerticalAlignment="Center">
        
        <!-- Retry button -->
        <Button IsVisible="{Binding ShowRetryButton}"
                Content="Retry"
                Command="{Binding RetryConnectionCommand}"
                Background="Transparent"
                BorderBrush="White"
                BorderThickness="1"
                Foreground="White"
                Padding="12,4"
                CornerRadius="4"
                FontSize="12" />
        
        <!-- Retrying indicator -->
        <Border IsVisible="{Binding IsRetrying}"
                Background="Transparent"
                BorderBrush="White"
                BorderThickness="1"
                CornerRadius="4"
                Padding="12,4">
          
          <StackPanel Orientation="Horizontal" Spacing="8">
            <Border Width="12" Height="12"
                    Background="White"
                    CornerRadius="6">
              <Border.Styles>
                <Style Selector="Border">
                  <Style.Animations>
                    <Animation Duration="0:0:1" IterationCount="Infinite">
                      <KeyFrame Cue="0%">
                        <Setter Property="Opacity" Value="0.3"/>
                      </KeyFrame>
                      <KeyFrame Cue="50%">
                        <Setter Property="Opacity" Value="1"/>
                      </KeyFrame>
                      <KeyFrame Cue="100%">
                        <Setter Property="Opacity" Value="0.3"/>
                      </KeyFrame>
                    </Animation>
                  </Style.Animations>
                </Style>
              </Border.Styles>
            </Border>
            
            <TextBlock Text="Retrying..."
                       FontSize="12"
                       Foreground="White"
                       VerticalAlignment="Center" />
          </StackPanel>
        </Border>
      </StackPanel>
    </Grid>
  </Border>
</UserControl>
```

#### **Value Converters**

```csharp
// Connection state to background color converter
public class ConnectionStateToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ConnectionState state)
        {
            return state switch
            {
                ConnectionState.Connected => "#4CAF50",      // Green
                ConnectionState.Disconnected => "#F44336",   // Red  
                ConnectionState.Error => "#FF9800",          // Orange
                ConnectionState.Retrying => "#2196F3",       // Blue
                _ => "#9E9E9E"                               // Gray
            };
        }
        
        return "#9E9E9E";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Connection state to icon converter
public class ConnectionStateToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ConnectionState state)
        {
            return state switch
            {
                ConnectionState.Connected => "\ue876",       // check_circle
                ConnectionState.Disconnected => "\ue002",    // error  
                ConnectionState.Error => "\ue000",          // warning
                ConnectionState.Retrying => "\ue863",       // sync
                _ => "\ue88f"                               // help
            };
        }
        
        return "\ue88f";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

---

### **Task 4.3: View Integration Updates**

**Estimated Time**: 3 hours  
**Risk Level**: Medium  
**Dependencies**: Stages 1-3 complete  

#### **InventoryTabView Integration**

```csharp
// Update InventoryTabViewModel
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    // Add overlay integration properties
    [ObservableProperty] private bool showValidationOverlay = false;
    [ObservableProperty] private string validationMessage = string.Empty;
    [ObservableProperty] private bool showAddConfirmation = false;

    private readonly IUniversalOverlayService _overlayService;

    [RelayCommand]
    private async Task AddInventoryWithConfirmationAsync()
    {
        // Validate input first
        var validationErrors = ValidateInventoryInput();
        if (validationErrors.Any())
        {
            var validationRequest = new ValidationOverlayRequest(
                "Input Validation Required",
                validationErrors,
                ValidationSeverity.Error
            );
            
            await _overlayService.ShowValidationOverlayAsync(validationRequest);
            return;
        }

        // Show confirmation overlay
        var confirmRequest = new ConfirmationOverlayRequest(
            "Confirm Inventory Addition",
            $"Add {Quantity} units of {PartId} to operation {Operation}?",
            ConfirmationType.AddItem,
            "Add Inventory",
            "Cancel"
        );

        var confirmResult = await _overlayService.ShowConfirmationOverlayAsync(confirmRequest);
        
        if (confirmResult.Result == OverlayResult.Confirmed)
        {
            // Show progress for the operation
            using var progress = await _overlayService.ShowProgressAsync(
                new ProgressOverlayRequest("Adding Inventory", "Preparing data...", canCancel: false)
            );

            try
            {
                await progress.UpdateProgressAsync(25, "Validating part ID...");
                
                // Perform database operation
                await progress.UpdateProgressAsync(50, "Adding to database...");
                var addResult = await AddInventoryToDatabaseAsync();
                
                await progress.UpdateProgressAsync(75, "Updating display...");
                await RefreshInventoryDataAsync();
                
                await progress.CompleteAsync("Inventory added successfully");
                
                // Show success overlay
                var successRequest = new SuccessOverlayRequest(
                    "Inventory Added",
                    $"Successfully added {Quantity} units of {PartId}",
                    SuccessType.ItemAdded
                );
                
                await _overlayService.ShowSuccessOverlayAsync(successRequest);
            }
            catch (Exception ex)
            {
                await progress.CancelAsync("Operation failed");
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to add inventory");
            }
        }
    }

    private List<ValidationError> ValidateInventoryInput()
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(PartId))
        {
            errors.Add(new ValidationError("PartId", "Part ID is required", ValidationSeverity.Error));
        }

        if (string.IsNullOrWhiteSpace(Operation))
        {
            errors.Add(new ValidationError("Operation", "Operation is required", ValidationSeverity.Error));
        }

        if (Quantity <= 0)
        {
            errors.Add(new ValidationError("Quantity", "Quantity must be greater than zero", ValidationSeverity.Error));
        }

        if (string.IsNullOrWhiteSpace(Location))
        {
            errors.Add(new ValidationError("Location", "Location is required", ValidationSeverity.Error));
        }

        return errors;
    }
}
```

#### **TransferTabView Integration**

```csharp
[RelayCommand]
private async Task TransferWithConfirmationAsync()
{
    var validationErrors = ValidateTransferInput();
    if (validationErrors.Any())
    {
        await _overlayService.ShowValidationOverlayAsync(
            new ValidationOverlayRequest("Transfer Validation", validationErrors));
        return;
    }

    // Show transfer confirmation with details
    var confirmRequest = new ConfirmationOverlayRequest(
        "Confirm Inventory Transfer",
        $"Transfer {TransferQuantity} units of {PartId}\\nFrom: {FromLocation}\\nTo: {ToLocation}\\n\\nThis will affect inventory levels at both locations.",
        ConfirmationType.Transfer,
        "Transfer Items",
        "Cancel"
    );

    var result = await _overlayService.ShowConfirmationOverlayAsync(confirmRequest);
    
    if (result.Result == OverlayResult.Confirmed)
    {
        using var progress = await _overlayService.ShowProgressAsync(
            new ProgressOverlayRequest("Transferring Inventory", canCancel: false)
        );

        await progress.UpdateProgressAsync(20, "Validating source inventory...");
        await progress.UpdateProgressAsync(40, "Updating source location...");
        await progress.UpdateProgressAsync(60, "Updating destination location...");
        await progress.UpdateProgressAsync(80, "Recording transfer transaction...");
        
        await ProcessTransferAsync();
        
        await progress.CompleteAsync("Transfer completed successfully");
    }
}
```

#### **Implementation Steps**

1. **Update InventoryTabView Integration** (60 minutes)
   - Add validation overlays for all input fields
   - Add confirmation overlay for inventory additions
   - Integrate progress overlay for database operations

2. **Update TransferTabView Integration** (60 minutes)  
   - Add transfer confirmation overlays with details
   - Add location validation overlays
   - Add quantity validation feedback

3. **Update NewQuickButtonView Integration** (30 minutes)
   - Add button creation success overlays
   - Add form validation overlays
   - Add preview overlay for button appearance

4. **Update QuickButtonsView Integration** (30 minutes)
   - Add button deletion confirmations
   - Add edit success feedback
   - Add management operation overlays

5. **Testing All View Integrations** (20 minutes)
   - Test overlay display in all integrated views
   - Verify proper cleanup and disposal
   - Test theme compatibility across all overlays

---

### **Task 4.4: Performance Monitoring Integration**

**Estimated Time**: 1 hour  
**Risk Level**: Low  
**Dependencies**: Development tools setup  

#### **Performance Overlay for Developer Tools**

```csharp
[ObservableObject]
public partial class PerformanceOverlayViewModel : BaseViewModel
{
    [ObservableProperty] private double memoryUsage = 0.0;
    [ObservableProperty] private int activeOverlays = 0;
    [ObservableProperty] private double cpuUsage = 0.0;
    [ObservableProperty] private TimeSpan uptime = TimeSpan.Zero;
    [ObservableProperty] private List<PerformanceMetric> recentMetrics = new();
    [ObservableProperty] private bool isVisible = false;

    private readonly Timer _performanceTimer;
    private readonly PerformanceCounter? _cpuCounter;
    private readonly Process _currentProcess;

    public PerformanceOverlayViewModel(ILogger<PerformanceOverlayViewModel> logger) : base(logger)
    {
        _currentProcess = Process.GetCurrentProcess();
        
        try
        {
            _cpuCounter = new PerformanceCounter("Process", "% Processor Time", _currentProcess.ProcessName);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not initialize CPU performance counter");
        }

        _performanceTimer = new Timer(UpdatePerformanceMetrics, null, 
            TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private async void UpdatePerformanceMetrics(object? state)
    {
        try
        {
            await Task.Run(() =>
            {
                MemoryUsage = _currentProcess.WorkingSet64 / (1024.0 * 1024.0); // MB
                Uptime = DateTime.Now - _currentProcess.StartTime;
                
                if (_cpuCounter != null)
                {
                    CpuUsage = _cpuCounter.NextValue();
                }

                // Update recent metrics
                var metric = new PerformanceMetric
                {
                    Timestamp = DateTime.Now,
                    MemoryMB = MemoryUsage,
                    CpuPercent = CpuUsage,
                    ActiveOverlayCount = ActiveOverlays
                };

                RecentMetrics = RecentMetrics.Take(60).Prepend(metric).ToList(); // Last 60 seconds
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Performance metrics update failed");
        }
    }

    [RelayCommand]
    private void ToggleVisibility()
    {
        IsVisible = !IsVisible;
    }
}

public class PerformanceMetric
{
    public DateTime Timestamp { get; set; }
    public double MemoryMB { get; set; }
    public double CpuPercent { get; set; }
    public int ActiveOverlayCount { get; set; }
}
```

## 📊 Stage 4 Success Criteria

### **Functionality Verification**

- [ ] Progress overlay displays for all long-running operations
- [ ] Connection status overlay monitors database connectivity
- [ ] All target views have proper overlay integration
- [ ] Performance monitoring works in development builds

### **Performance Requirements**

- [ ] Progress overlays add <50ms overhead to operations
- [ ] Connection monitoring uses <1% CPU when idle
- [ ] View integration doesn't impact existing functionality
- [ ] Memory usage remains stable during heavy overlay usage

### **User Experience Validation**

- [ ] Progress indicators provide clear feedback
- [ ] Connection issues are immediately visible
- [ ] All overlays support proper keyboard navigation
- [ ] Theme switching works correctly with all new overlays

### **Technical Compliance**

- [ ] All code follows MTM MVVM Community Toolkit patterns
- [ ] AXAML uses proper Avalonia syntax (no AVLN2000 errors)
- [ ] Database operations use stored procedures only
- [ ] Service registration uses TryAdd patterns
- [ ] Error handling uses centralized ErrorHandling service

## 🔄 Testing Strategy

### **Unit Testing**

```csharp
[Test]
public async Task ProgressOverlay_UpdateProgress_ShouldUpdatePercentage()
{
    // Arrange
    var viewModel = new ProgressOverlayViewModel(_mockLogger.Object);
    
    // Act
    await viewModel.UpdateProgressAsync(50.0, "Halfway complete");
    
    // Assert
    viewModel.ProgressPercentage.Should().Be(50.0);
    viewModel.CurrentStep.Should().Be("Halfway complete");
}

[Test]
public void ConnectionStatusOverlay_Connected_ShouldHideOverlay()
{
    // Arrange
    var viewModel = new ConnectionStatusOverlayViewModel(_mockLogger.Object, _mockConfigService.Object);
    
    // Act
    viewModel.UpdateConnectionState(ConnectionState.Connected);
    
    // Assert
    viewModel.IsVisible.Should().BeFalse();
    viewModel.StatusMessage.Should().Be("Database Connected");
}
```

### **Integration Testing**

```csharp
[Test]
public async Task InventoryTabView_AddInventory_ShouldShowProgressOverlay()
{
    // Arrange
    var viewModel = CreateInventoryViewModel();
    viewModel.PartId = "TEST001";
    viewModel.Quantity = 5;
    viewModel.Operation = "100";
    
    // Act
    await viewModel.AddInventoryWithConfirmationCommand.ExecuteAsync(null);
    
    // Assert
    _mockOverlayService.Verify(x => x.ShowProgressAsync(It.IsAny<ProgressOverlayRequest>()), Times.Once);
}
```

## 📝 Completion Checklist

- [ ] All Task 4.1-4.4 implementations complete
- [ ] Unit tests written and passing
- [ ] Integration tests written and passing
- [ ] Manual testing completed in all themes
- [ ] Performance impact assessed and documented
- [ ] Documentation updated
- [ ] Code review completed
- [ ] Ready for Stage 5 dependencies

---

**Stage 4 provides the performance foundation and systematic view integration needed for a professional overlay system that enhances user experience while maintaining application performance.**
