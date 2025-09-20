# Services Reorganization Plan

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Audience**: MTM Application Development Team  

## 🎯 Reorganization Overview

This document outlines the consolidation and reorganization of the MTM Services folder to improve logical grouping, reduce file count, and enhance maintainability while preserving all existing functionality.

## 📊 Current Services Analysis

### **Current Structure (24 Files)**

```
Services/
├── ColumnConfigurationService.cs
├── Configuration.cs
├── CustomDataGridService.cs
├── Database.cs
├── EmergencyKeyboardHook.cs
├── ErrorHandling.cs
├── FileLoggingService.cs
├── FilePathService.cs
├── FileSelection.cs
├── FocusManagementService.cs
├── InventoryEditingService.cs
├── MasterDataService.cs
├── MTMFileLoggerProvider.cs
├── Navigation.cs
├── PrintService.cs
├── QuickButtons.cs
├── RemoveService.cs
├── SettingsPanelStateManager.cs
├── SettingsService.cs
├── StartupDialog.cs
├── SuccessOverlay.cs
├── SuggestionOverlay.cs
├── ThemeService.cs
└── VirtualPanelManager.cs
```

### **Functional Grouping Analysis**

#### **Core Infrastructure Services (3 files → 2 files)**

- `Configuration.cs` - Contains `IConfigurationService` + `IApplicationStateService`
- `ErrorHandling.cs` - Static error handling + `ErrorEntry` + `ErrorConfiguration`
- `Database.cs` - Core database operations

**Consolidation**: Keep separate (already well-organized)

#### **Data Management Services (3 files → 1 file)**

- `MasterDataService.cs` - Master data operations (parts, locations, operations)
- `InventoryEditingService.cs` - Inventory-specific operations
- `RemoveService.cs` - Removal operations

**Consolidation**: Merge into `DataServices.cs`

#### **UI Infrastructure Services (6 files → 2 files)**

- `Navigation.cs` - View navigation
- `ThemeService.cs` - Theme management
- `FocusManagementService.cs` - Focus management
- `VirtualPanelManager.cs` - Panel management
- `SettingsPanelStateManager.cs` - Settings panel state
- `SettingsService.cs` - Settings operations

**Consolidation**:

- Merge into `UIServices.cs` (Navigation, Theme, Focus, Panel management)
- Merge into `SettingsServices.cs` (Settings operations)

#### **Overlay & Dialog Services (3 files → 1 file)**

- `StartupDialog.cs` - Startup dialog management
- `SuccessOverlay.cs` - Success overlay functionality
- `SuggestionOverlay.cs` - Suggestion overlay functionality

**Consolidation**: Merge into `OverlayServices.cs`

#### **File & Path Services (3 files → 1 file)**

- `FilePathService.cs` - File path management
- `FileSelection.cs` - File selection operations
- `FileLoggingService.cs` - File logging
- `MTMFileLoggerProvider.cs` - Logging provider

**Consolidation**: Merge into `FileServices.cs`

#### **Custom Control Services (2 files → 1 file)**

- `ColumnConfigurationService.cs` - DataGrid column configuration
- `CustomDataGridService.cs` - DataGrid operations

**Consolidation**: Merge into `DataGridServices.cs`

#### **Specialized Services (Keep Separate)**

- `PrintService.cs` - Print operations (complex, keep separate)
- `QuickButtons.cs` - Quick button management (keep separate)
- `EmergencyKeyboardHook.cs` - System-level keyboard hooks (keep separate)

## 🏗️ Proposed New Structure (9 Files)

### **Target Organization**

```
Services/
├── Core/
│   ├── Configuration.cs                 # Keep as-is (IConfigurationService + IApplicationStateService)
│   ├── Database.cs                      # Keep as-is (Core database operations)
│   └── ErrorHandling.cs                 # Keep as-is (Error management)
├── Business/
│   ├── DataServices.cs                  # MasterData + InventoryEditing + Remove services
│   └── PrintService.cs                  # Keep separate (complex print operations)
├── UI/
│   ├── UIServices.cs                    # Navigation + Theme + Focus + VirtualPanel management
│   ├── SettingsServices.cs              # Settings + SettingsPanelState management
│   ├── OverlayServices.cs               # Startup + Success + Suggestion overlays
│   └── DataGridServices.cs              # Column + CustomDataGrid services
├── Infrastructure/
│   ├── FileServices.cs                  # FilePath + FileSelection + FileLogging + MTMFileLoggerProvider
│   ├── QuickButtons.cs                  # Keep separate (specialized functionality)
│   └── EmergencyKeyboardHook.cs         # Keep separate (system-level operations)
```

## 📋 Consolidation Tasks

### **Task 1: Create DataServices.cs**

**Files to Merge:**

- `MasterDataService.cs`
- `InventoryEditingService.cs`
- `RemoveService.cs`

**Approach:**

```csharp
namespace MTM_WIP_Application_Avalonia.Services;

// Master Data Service Interface
public interface IMasterDataService
{
    // Keep all existing methods from MasterDataService.cs
}

// Inventory Editing Service Interface
public interface IInventoryEditingService
{
    // Keep all existing methods from InventoryEditingService.cs
}

// Remove Service Interface
public interface IRemoveService
{
    // Keep all existing methods from RemoveService.cs
}

// Combined implementation
public class MasterDataService : IMasterDataService { /* Implementation */ }
public class InventoryEditingService : IInventoryEditingService { /* Implementation */ }
public class RemoveService : IRemoveService { /* Implementation */ }
```

**References to Update:**

- `ViewModels/MainForm/*.cs` (all inventory-related ViewModels)
- `Extensions/ServiceCollectionExtensions.cs`
- Any test files

### **Task 2: Create UIServices.cs**

**Files to Merge:**

- `Navigation.cs`
- `ThemeService.cs`
- `FocusManagementService.cs`
- `VirtualPanelManager.cs`

**Approach:**

```csharp
namespace MTM_WIP_Application_Avalonia.Services;

// Navigation Service Interface & Implementation
public interface INavigationService { /* Keep existing interface */ }
public class NavigationService : INavigationService { /* Keep implementation */ }

// Theme Service Interface & Implementation
public interface IThemeService { /* Keep existing interface */ }
public class ThemeService : IThemeService { /* Keep implementation */ }

// Focus Management Service Interface & Implementation
public interface IFocusManagementService { /* Keep existing interface */ }
public class FocusManagementService : IFocusManagementService { /* Keep implementation */ }

// Virtual Panel Manager Interface & Implementation
public interface IVirtualPanelManager { /* Keep existing interface */ }
public class VirtualPanelManager : IVirtualPanelManager { /* Keep implementation */ }
```

**References to Update:**

- `MainWindow.axaml.cs`
- `Views/MainForm/*.cs`
- `ViewModels/MainForm/*.cs`
- `Extensions/ServiceCollectionExtensions.cs`

### **Task 3: Create SettingsServices.cs**

**Files to Merge:**

- `SettingsService.cs`
- `SettingsPanelStateManager.cs`

**Approach:**

```csharp
namespace MTM_WIP_Application_Avalonia.Services;

// Settings Service Interface & Implementation
public interface ISettingsService { /* Keep existing interface */ }
public class SettingsService : ISettingsService { /* Keep implementation */ }

// Settings Panel State Manager Interface & Implementation
public interface ISettingsPanelStateManager { /* Keep existing interface */ }
public class SettingsPanelStateManager : ISettingsPanelStateManager { /* Keep implementation */ }
```

**References to Update:**

- `ViewModels/SettingsForm/*.cs`
- `Views/SettingsForm/*.cs`
- `Extensions/ServiceCollectionExtensions.cs`

### **Task 4: Create OverlayServices.cs**

**Files to Merge:**

- `StartupDialog.cs`
- `SuccessOverlay.cs`
- `SuggestionOverlay.cs`

**Approach:**

```csharp
namespace MTM_WIP_Application_Avalonia.Services;

// Startup Dialog Service
public interface IStartupDialogService { /* Keep existing interface */ }
public class StartupDialogService : IStartupDialogService { /* Keep implementation */ }

// Success Overlay Service
public interface ISuccessOverlayService { /* Keep existing interface */ }
public class SuccessOverlayService : ISuccessOverlayService { /* Keep implementation */ }

// Suggestion Overlay Service
public interface ISuggestionOverlayService { /* Keep existing interface */ }
public class SuggestionOverlayService : ISuggestionOverlayService { /* Keep implementation */ }
```

**References to Update:**

- `MainWindow.axaml.cs`
- `ViewModels/MainForm/*.cs`
- `Extensions/ServiceCollectionExtensions.cs`

### **Task 5: Create FileServices.cs**

**Files to Merge:**

- `FilePathService.cs`
- `FileSelection.cs`
- `FileLoggingService.cs`
- `MTMFileLoggerProvider.cs`

**Approach:**

```csharp
namespace MTM_WIP_Application_Avalonia.Services;

// File Path Service Interface & Implementation
public interface IFilePathService { /* Keep existing interface */ }
public class FilePathService : IFilePathService { /* Keep implementation */ }

// File Selection Service Interface & Implementation
public interface IFileSelectionService { /* Keep existing interface */ }
public class FileSelectionService : IFileSelectionService { /* Keep implementation */ }

// File Logging Service Interface & Implementation
public interface IFileLoggingService { /* Keep existing interface */ }
public class FileLoggingService : IFileLoggingService { /* Keep implementation */ }

// MTM File Logger Provider
public class MTMFileLoggerProvider : ILoggerProvider { /* Keep implementation */ }
```

**References to Update:**

- `ViewModels/PrintViewModel.cs`
- `Extensions/ServiceCollectionExtensions.cs`
- Any file-related operations

### **Task 6: Create DataGridServices.cs**

**Files to Merge:**

- `ColumnConfigurationService.cs`
- `CustomDataGridService.cs`

**Approach:**

```csharp
namespace MTM_WIP_Application_Avalonia.Services;

// Column Configuration Service Interface & Implementation
public interface IColumnConfigurationService { /* Keep existing interface */ }
public class ColumnConfigurationService : IColumnConfigurationService { /* Keep implementation */ }

// Custom DataGrid Service Interface & Implementation
public interface ICustomDataGridService { /* Keep existing interface */ }
public class CustomDataGridService : ICustomDataGridService { /* Keep implementation */ }
```

**References to Update:**

- `Controls/CustomDataGrid/*.cs`
- `ViewModels/MainForm/*.cs`
- `Extensions/ServiceCollectionExtensions.cs`

### **Task 7: Create Services Folder Structure**

**Steps:**

1. Create `Services/Core/` folder
2. Create `Services/Business/` folder
3. Create `Services/UI/` folder
4. Create `Services/Infrastructure/` folder
5. Move appropriate files to new locations
6. Update namespace declarations
7. Update all import statements

## 🔄 Migration Steps

### **Phase 1: Preparation (1 hour)**

1. Create backup of current Services folder
2. Analyze all current references to service files
3. Create new folder structure
4. Document all namespace changes needed

### **Phase 2: Core Infrastructure (2 hours)**

1. Move `Configuration.cs` to `Services/Core/`
2. Move `Database.cs` to `Services/Core/`
3. Move `ErrorHandling.cs` to `Services/Core/`
4. Update namespace declarations
5. Test core functionality

### **Phase 3: Service Consolidation (4 hours)**

1. Create `DataServices.cs` and merge related files
2. Create `UIServices.cs` and merge related files
3. Create `SettingsServices.cs` and merge related files
4. Create `OverlayServices.cs` and merge related files
5. Create `FileServices.cs` and merge related files
6. Create `DataGridServices.cs` and merge related files

### **Phase 4: Reference Updates (2 hours)**

1. Update `Extensions/ServiceCollectionExtensions.cs`
2. Update all ViewModel imports
3. Update all View code-behind imports
4. Update Program.cs if needed
5. Update any test references

### **Phase 5: Validation (1 hour)**

1. Build solution and fix any compilation errors
2. Test application startup
3. Test core functionality
4. Verify service dependency injection
5. Document any issues found

## 📋 Reference Update Checklist

### **Files Requiring Updates**

#### **Service Registration**

- [ ] `Extensions/ServiceCollectionExtensions.cs`
- [ ] `Program.cs`
- [ ] `App.axaml.cs`

#### **ViewModels**

- [ ] `ViewModels/MainForm/MainViewViewModel.cs`
- [ ] `ViewModels/MainForm/InventoryTabViewModel.cs`
- [ ] `ViewModels/MainForm/RemoveItemViewModel.cs`
- [ ] `ViewModels/MainForm/TransferItemViewModel.cs`
- [ ] `ViewModels/MainForm/AdvancedInventoryViewModel.cs`
- [ ] `ViewModels/MainForm/AdvancedRemoveViewModel.cs`
- [ ] `ViewModels/SettingsForm/*.cs`
- [ ] `ViewModels/PrintViewModel.cs`

#### **Views (Code-behind)**

- [ ] `MainWindow.axaml.cs`
- [ ] `Views/MainForm/Panels/*.cs`
- [ ] `Views/SettingsForm/*.cs`
- [ ] `Views/PrintView.axaml.cs`

#### **Controls**

- [ ] `Controls/CustomDataGrid/*.cs`
- [ ] Any custom control implementations

### **Testing Requirements**

#### **Unit Tests (if any)**

- [ ] Update service registration tests
- [ ] Update service interface tests
- [ ] Update mocking configurations

#### **Integration Tests**

- [ ] Test service dependency injection
- [ ] Test cross-service communication
- [ ] Test database operations
- [ ] Test UI service interactions

## 🚨 Risk Mitigation

### **Backup Strategy**

1. Create complete backup of Services folder before starting
2. Use git branches for each consolidation phase
3. Test each phase independently before proceeding
4. Maintain rollback procedures

### **Testing Strategy**

1. Automated build verification after each phase
2. Manual testing of core workflows
3. Database connectivity testing
4. UI functionality testing
5. Service injection validation

### **Rollback Plan**

1. If issues arise, revert to previous git commit
2. Restore service folder from backup
3. Rebuild solution
4. Re-test functionality

## ✅ Success Criteria

### **Technical Criteria**

- [ ] Solution builds without errors
- [ ] All services registered correctly in DI container
- [ ] No broken references or missing imports
- [ ] All existing functionality preserved
- [ ] Performance maintained or improved

### **Organizational Criteria**

- [ ] Services folder reduced from 24 to 9 files
- [ ] Logical grouping implemented (Core, Business, UI, Infrastructure)
- [ ] Related services consolidated appropriately
- [ ] Clear separation of concerns maintained

### **Documentation Criteria**

- [ ] Updated architecture documentation
- [ ] Service dependency documentation updated
- [ ] Developer onboarding materials updated
- [ ] Migration guide completed

This reorganization will significantly improve the Services folder organization while maintaining all existing functionality and following MTM architectural patterns.
