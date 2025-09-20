# GitHub Copilot Continuation Prompt - MTM WIP Application

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

## 🚨 CRITICAL: Development Context & Status

**Date**: September 19, 2025  
**Branch**: `copilot/fix-ceed4abc-44a8-47a5-a008-f4549b04c054`  
**Active PR**: [Complete MTM Refactor Phase 1-2 - Project Reorganization Foundation + Universal Overlay System Implementation #87](https://github.com/Dorotel/MTM_WIP_Application_Avalonia/pull/87)

### **Project Status Summary**

**Overall Progress**: 59% complete (10/17 SubTasks)  
**Phase 1**: ✅ COMPLETE - Project Reorganization Foundation (16/16 SubTasks)  
**Phase 2**: 🟡 IN PROGRESS - Universal Overlay System (10/17 SubTasks, 59%)  
**Phase 3**: 🔴 NOT STARTED - Integration & Polish (0/14 SubTasks)

### **Current Development State**

**✅ MAJOR ACCOMPLISHMENTS:**

1. **Services Consolidation Complete**: 21+ services → 9 consolidated groups
   - Core Services: Database, Configuration, ErrorHandling
   - Business Services: MasterData, Remove, InventoryEditing
   - UI Services: Navigation, Theme, Focus, SuccessOverlay
   - Infrastructure Services: File operations, Print, Logging

2. **Universal Overlay System Foundation**: BaseOverlayViewModel and service architecture implemented

3. **Critical Safety Overlays**: Confirmation, Error, Progress, Loading overlays completed

4. **Project Organization**: WeekendRefactor structured with numbered folders (01-04)

**🎯 IMMEDIATE NEXT ACTIONS:**

Continue Phase 2 implementation focusing on:

- **Task 2.3**: MainWindow Integration Overlays (1/4 SubTasks) - IN PROGRESS
  - ✅ SubTask 2.3.1: Connection Status Overlay - COMPLETED  
  - 🎯 **NEXT**: SubTask 2.3.2: Emergency Shutdown Overlay
- **Task 2.4**: View-Specific Overlay Integration (0/5 SubTasks)

---

## 💻 Developer Continuation Instructions

### **Starting Development Session**

When you continue development on this project:

1. **Verify Build Status**: Check that the application compiles successfully
2. **Review Current Phase**: Focus on Phase 2 - Universal Overlay System
3. **Check Progress**: Review completed SubTasks in [Progress-Tracking.md](Progress-Tracking.md)
4. **Start Next Task**: Begin with Task 2.3.1 (Connection Status Overlay)

### **Phase 2 Continuation Checklist**

#### **Task 2.3: MainWindow Integration Overlays (NEXT)**

**Estimated Time**: 6-8 hours  
**Priority**: HIGH (Critical user experience overlays)

- [ ] **SubTask 2.3.1**: ✅ **COMPLETE** - Implement Connection Status Overlay (2 hours)
  - ConnectionStatusOverlayViewModel with database connection testing
  - ConnectionStatusOverlayView with MTM design system
  - Retry logic with configurable max attempts  
  - Proper service registration and integration

- [ ] **SubTask 2.3.2**: Implement Emergency Shutdown Overlay (1.5 hours)  
  - Create EmergencyShutdownOverlayViewModel
  - Provide safe application shutdown
  - Save critical data before shutdown
  - Display shutdown progress

- [ ] **SubTask 2.3.3**: Implement Theme Quick Switcher Overlay (2 hours)
  - Create ThemeQuickSwitcherOverlayViewModel  
  - Allow rapid theme switching
  - Preview theme changes
  - Apply theme selections

- [ ] **SubTask 2.3.4**: Integrate MainWindow Overlays (0.5 hours)
  - Update MainWindow to use new overlays
  - Test overlay integration
  - Validate performance

#### **Task 2.4: View-Specific Overlay Integration (FOLLOWING)**

**Estimated Time**: 8-10 hours  
**Priority**: HIGH (Core functionality enhancement)

- [ ] **SubTask 2.4.1**: InventoryTabView Overlay Integration (2 hours)
- [ ] **SubTask 2.4.2**: RemoveTabView Overlay Integration (2 hours)  
- [ ] **SubTask 2.4.3**: TransferTabView Overlay Integration (2 hours)
- [ ] **SubTask 2.4.4**: AdvancedInventoryView Overlay Integration (1 hour)
- [ ] **SubTask 2.4.5**: AdvancedRemoveView Overlay Integration (1 hour)

---

## 🏗️ Technical Implementation Patterns

### **Overlay ViewModel Pattern (MANDATORY)**

All new overlays MUST follow the established BaseOverlayViewModel pattern:

```csharp
[ObservableObject]
public partial class ConnectionStatusOverlayViewModel : BaseOverlayViewModel
{
    [ObservableProperty]
    private string connectionStatus = "Checking...";
    
    [ObservableProperty]
    private bool isConnected;
    
    [ObservableProperty]
    private string lastConnectionError = string.Empty;
    
    [RelayCommand]
    private async Task RetryConnectionAsync()
    {
        IsProcessing = true;
        try
        {
            // Test database connection
            var result = await _databaseService.TestConnectionAsync();
            IsConnected = result.IsSuccess;
            ConnectionStatus = result.IsSuccess ? "Connected" : "Disconnected";
            
            if (!result.IsSuccess)
            {
                LastConnectionError = result.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Connection retry failed");
            IsConnected = false;
            ConnectionStatus = "Connection Failed";
        }
        finally
        {
            IsProcessing = false;
        }
    }
    
    public ConnectionStatusOverlayViewModel(
        ILogger<ConnectionStatusOverlayViewModel> logger,
        IDatabaseService databaseService) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(databaseService);
        _databaseService = databaseService;
    }
    
    private readonly IDatabaseService _databaseService;
}
```

### **Avalonia AXAML Pattern (MANDATORY)**

All overlay AXAML files MUST follow MTM design patterns:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Overlay"
             x:Class="MTM_WIP_Application_Avalonia.Views.Overlay.ConnectionStatusOverlayView">

    <Border Background="{DynamicResource MTM_Shared_Logic.OverlayBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
            BorderThickness="1"
            CornerRadius="8"
            Padding="16"
            MaxWidth="400"
            MaxHeight="300">
        
        <Grid RowDefinitions="Auto,*,Auto" Spacing="16">
            
            <!-- Header -->
            <Border Grid.Row="0" 
                    Background="{DynamicResource MTM_Shared_Logic.AccentBrush}"
                    CornerRadius="4"
                    Padding="12,8">
                <TextBlock Text="Database Connection Status"
                           Foreground="White"
                           FontWeight="Bold"
                           HorizontalAlignment="Center" />
            </Border>
            
            <!-- Content -->
            <StackPanel Grid.Row="1" Spacing="12">
                <TextBlock Text="{Binding ConnectionStatus}"
                           FontSize="16"
                           HorizontalAlignment="Center" />
                           
                <Border IsVisible="{Binding IsConnected}"
                        Background="{DynamicResource MTM_Shared_Logic.SuccessBrush}"
                        CornerRadius="4"
                        Padding="8">
                    <TextBlock Text="✓ Connected Successfully"
                               Foreground="White"
                               HorizontalAlignment="Center" />
                </Border>
                
                <Border IsVisible="{Binding !IsConnected}"
                        Background="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
                        CornerRadius="4"
                        Padding="8">
                    <StackPanel Spacing="4">
                        <TextBlock Text="✗ Connection Failed"
                                   Foreground="White"
                                   HorizontalAlignment="Center" />
                        <TextBlock Text="{Binding LastConnectionError}"
                                   Foreground="White"
                                   FontSize="12"
                                   TextWrapping="Wrap"
                                   HorizontalAlignment="Center"
                                   IsVisible="{Binding LastConnectionError, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" />
                    </StackPanel>
                </Border>
            </StackPanel>
            
            <!-- Actions -->
            <StackPanel Grid.Row="2" 
                        Orientation="Horizontal"
                        HorizontalAlignment="Right"
                        Spacing="8">
                <Button Content="Retry Connection"
                        Command="{Binding RetryConnectionCommand}"
                        IsEnabled="{Binding !IsProcessing}"
                        Background="{DynamicResource MTM_Shared_Logic.AccentBrush}"
                        Foreground="White"
                        Padding="12,6" />
                        
                <Button Content="Close"
                        Command="{Binding HideCommand}"
                        Background="{DynamicResource MTM_Shared_Logic.SecondaryBrush}"
                        Padding="12,6" />
            </StackPanel>
            
        </Grid>
    </Border>
</UserControl>
```

### **Service Integration Pattern (MANDATORY)**

Register new overlays in the consolidated UI services:

```csharp
// In Services/UI/UIServices.cs
services.TryAddTransient<ConnectionStatusOverlayViewModel>();
services.TryAddTransient<EmergencyShutdownOverlayViewModel>();
services.TryAddTransient<ThemeQuickSwitcherOverlayViewModel>();
```

---

## 🎯 Quality Standards & Validation

### **Code Quality Requirements**

1. **MVVM Community Toolkit**: All ViewModels use [ObservableObject], [ObservableProperty], [RelayCommand]
2. **Avalonia Syntax**: Use x:Name (not Name), proper xmlns namespaces, DynamicResource bindings
3. **Error Handling**: Use Services.Core.ErrorHandling.HandleErrorAsync() for all exceptions
4. **Database**: Only stored procedures via Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()
5. **Dependency Injection**: ArgumentNullException.ThrowIfNull() validation for all constructor parameters

### **Testing Requirements**

After each SubTask completion:

1. **Build Validation**: Application compiles with 0 errors
2. **Functionality Test**: Overlay displays and functions correctly
3. **Integration Test**: Overlay works with target view/window
4. **Theme Compatibility**: Overlay works with all MTM themes (Blue, Green, Red, Dark)
5. **Performance Test**: No significant performance degradation

### **Progress Tracking Requirements**

After each SubTask completion, update Progress-Tracking.md:

1. Mark SubTask as completed: `- [x] **SubTask X.Y.Z**: ✅ **COMPLETE** - [Description]`
2. Update task progress percentage: `Task Progress: XX% (Y/Z)`
3. Update phase progress: `Phase 2: Status: In Progress (XX%)`
4. Add completion notes with file changes and validation results

---

## 📋 Development Environment Setup

### **Required Tools & Extensions**

- **VS Code**: Latest version with Avalonia extension
- **.NET SDK**: 8.0 or later
- **MySQL**: Connection to MTM database available
- **Git**: Configured for Dorotel/MTM_WIP_Application_Avalonia repository

### **Branch Management**

- **Current Branch**: `copilot/fix-ceed4abc-44a8-47a5-a008-f4549b04c054`
- **Keep commits focused**: Each SubTask should be a logical commit
- **Update PR description**: Reflect progress and completed SubTasks

### **Build & Test Commands**

```powershell
# Verify build
dotnet build

# Run application for testing
dotnet run

# Check for warnings
dotnet build --verbosity normal
```

---

## 🚨 Critical Success Factors

### **Do Not Break Existing Functionality**

- Maintain all existing overlay functionality (ConfirmationOverlayViewModel, SuccessOverlayViewModel)
- Preserve all service interfaces and dependency injection patterns
- Keep all database operations using stored procedures only
- Maintain theme system compatibility

### **Follow Established Patterns**

- Use only patterns found in existing codebase
- Follow MTM design system colors and spacing
- Maintain MVVM Community Toolkit standards throughout
- Use consolidated service patterns (Services.UI, Services.Core, etc.)

### **Performance Considerations**

- Implement overlay pooling for memory efficiency
- Avoid memory leaks in overlay lifecycle management
- Minimize overlay creation/destruction overhead
- Maintain responsive UI during overlay operations

---

## 📚 Reference Documentation

### **Key Files for Reference**

- **Progress Tracking**: [Progress-Tracking.md](Progress-Tracking.md) - Complete implementation status
- **Service Architecture**: [../Services/SERVICE_DEPENDENCY_ANALYSIS.md](../Services/SERVICE_DEPENDENCY_ANALYSIS.md)
- **Master Plan**: [../Master-Refactor-Implementation-Plan.md](../Master-Refactor-Implementation-Plan.md)
- **Implementation Guide**: [../03-Implementation/README.md](../03-Implementation/README.md)

### **Existing Overlay Examples**

- **BaseOverlayViewModel.cs**: Foundation class for all overlays
- **ConfirmationOverlayViewModel.cs**: User confirmation overlay example
- **ProgressOverlayViewModel.cs**: Long-running operation overlay example
- **LoadingOverlayViewModel.cs**: Quick operation overlay example

### **Service References**

- **Services/UI/UIServices.cs**: UI service consolidation including overlay service
- **Services/Core/CoreServices.cs**: Core services including error handling and database
- **Extensions/ServiceCollectionExtensions.cs**: Service registration patterns

---

## 🎯 Success Definition

### **Phase 2 Complete When:**

- All 17 SubTasks in Phase 2 marked as completed
- All major views have appropriate overlay integration
- MainWindow has critical system overlays (connection, shutdown, theme)
- Application compiles and runs without errors
- All overlays function correctly with proper themes
- Performance remains acceptable with overlay system

### **Ready for Phase 3 When:**

- Universal overlay system fully functional across the application
- All overlay ViewModels follow BaseOverlayViewModel pattern
- Service integration complete and tested
- Documentation updated to reflect Phase 2 changes

---

**START HERE**: Begin with **SubTask 2.3.1: Implement Connection Status Overlay** following the technical patterns outlined above.

**REMEMBER**: Update Progress-Tracking.md after each SubTask completion with specific details about what was implemented, files changed, and validation results.
