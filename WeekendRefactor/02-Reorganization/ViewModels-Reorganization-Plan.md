# ViewModels Reorganization Plan

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Audience**: MTM Application Development Team  

## 🎯 ViewModels Reorganization Overview

This document outlines the reorganization of ViewModels to mirror the Views structure, remove deprecated code, and ensure clear logical grouping while maintaining all existing functionality.

## 📊 Current ViewModels Analysis

### **Current Structure**

```
ViewModels/
├── FilterPanelViewModel.cs.disabled        # DEPRECATED - Remove
├── MainForm/                              # Main application ViewModels (12 files)
│   ├── AddItemViewModel.cs
│   ├── AdvancedInventoryViewModel.cs
│   ├── AdvancedRemoveViewModel.cs
│   ├── InventoryTabViewModel.cs
│   ├── InventoryViewModel.cs
│   ├── MainViewViewModel.cs
│   ├── MainWindowViewModel.cs
│   ├── QuickButtonsViewModel.cs
│   ├── RemoveItemViewModel.cs
│   ├── SearchInventoryViewModel.cs
│   ├── SuggestionOverlayViewModel.cs
│   └── TransferItemViewModel.cs
├── Overlay/                               # Overlay ViewModels (6 files)
│   ├── ConfirmationOverlayViewModel.cs
│   ├── EditInventoryViewModel.cs
│   ├── NewQuickButtonOverlayViewModel.cs
│   ├── NoteEditorViewModel.cs
│   ├── SuccessOverlayViewModel.cs
│   └── SuggestionOverlayViewModel.cs      # DUPLICATE - needs consolidation
├── PrintLayoutControlViewModel.cs          # Print-related ViewModels (2 files)
├── PrintViewModel.cs
├── SettingsForm/                          # Settings ViewModels (25 files)
│   ├── AboutViewModel.cs
│   ├── [Add/Edit/Remove] × [Part/Location/Operation/User/ItemType] (15 files)
│   ├── BackupRecoveryViewModel.cs
│   ├── DatabaseSettingsViewModel.cs
│   ├── SecurityPermissionsViewModel.cs
│   ├── SettingsCategoryViewModel.cs
│   ├── SettingsViewModel.cs
│   ├── ShortcutsViewModel.cs
│   ├── SystemHealthViewModel.cs
│   ├── SystemHealthViewModel_Fixed.cs      # DUPLICATE - needs consolidation
│   └── ThemeBuilderViewModel.cs
├── Shared/                                # Base and utility ViewModels (3 files)
│   ├── BaseViewModel.cs
│   ├── CustomDataGridViewModel.cs
│   └── TransferCustomDataGridViewModel.cs
├── ThemeEditorViewModel.cs                # Standalone ViewModels
└── TransactionsForm/                      # Transaction ViewModels (2 files)
    ├── TransactionHistoryViewModel.cs
    └── TransactionHistoryViewModel_Fixed.cs  # DUPLICATE - needs consolidation

```

### **Corresponding Views Structure**

```
Views/
├── ConfirmationOverlayView.axaml          # Root overlays
├── NoteEditorView.axaml
├── PrintLayoutControl.axaml               # Print views
├── PrintView.axaml
├── MainForm/                              # Main form views
│   ├── Overlays/                          # Form-specific overlays
│   │   ├── SuggestionOverlayView.axaml
│   │   └── ThemeQuickSwitcher.axaml
│   └── Panels/                            # Main content panels
│       ├── AdvancedInventoryView.axaml
│       ├── AdvancedRemoveView.axaml
│       ├── InventoryTabView.axaml
│       ├── MainView.axaml
│       ├── NewQuickButtonView.axaml
│       ├── QuickButtonsView.axaml
│       ├── RemoveTabView.axaml
│       └── TransferTabView.axaml
├── Overlay/                               # Generic overlays
├── SettingsForm/                          # Settings views

└── ThemeEditor/                           # Theme editor views
```

## 🏗️ Issues Identified

### **1. Structural Mismatches**

- ViewModels/MainForm contains overlays that should be in ViewModels/MainForm/Overlays

- Missing ViewModels/MainForm/Panels structure to match Views
- Print ViewModels not grouped properly

- Theme editor ViewModels scattered

### **2. Deprecated Code**

- `FilterPanelViewModel.cs.disabled` - Remove completely

- `SystemHealthViewModel_Fixed.cs` - Consolidate with original
- `TransactionHistoryViewModel_Fixed.cs` - Consolidate with original

### **3. Duplicate ViewModels**

- `SuggestionOverlayViewModel.cs` exists in both MainForm/ and Overlay/
- Fixed versions of ViewModels need consolidation

### **4. Missing Structure**

- No ViewModels/Print/ folder for print-related ViewModels
- No clear separation between panels and overlays in MainForm

## 🎯 Proposed Reorganization

### **Target Structure**

```
ViewModels/
├── Application/                           # Application-level ViewModels
│   ├── MainWindowViewModel.cs
│   └── ThemeEditorViewModel.cs
├── MainForm/                              # Main form structure
│   ├── Overlays/                          # MainForm overlay ViewModels
│   │   ├── SuggestionOverlayViewModel.cs  # Move from MainForm root
│   │   └── ThemeQuickSwitcherViewModel.cs # Create if needed
│   ├── Panels/                            # MainForm panel ViewModels
│   │   ├── AdvancedInventoryViewModel.cs  # Move from MainForm root
│   │   ├── AdvancedRemoveViewModel.cs     # Move from MainForm root
│   │   ├── InventoryTabViewModel.cs       # Move from MainForm root
│   │   ├── MainViewViewModel.cs           # Move from MainForm root
│   │   ├── QuickButtonsViewModel.cs       # Move from MainForm root
│   │   ├── RemoveTabView Model.cs         # Rename from RemoveItemViewModel.cs
│   │   └── TransferTabViewModel.cs        # Rename from TransferItemViewModel.cs
│   └── MainFormBaseViewModel.cs           # Rename from AddItemViewModel.cs or create base
├── Overlay/                               # Generic overlay ViewModels
│   ├── ConfirmationOverlayViewModel.cs    # Keep as-is
│   ├── EditInventoryViewModel.cs          # Keep as-is
│   ├── NewQuickButtonOverlayViewModel.cs  # Keep as-is
│   ├── NoteEditorViewModel.cs             # Keep as-is
│   └── SuccessOverlayViewModel.cs         # Keep as-is
├── Print/                                 # Print ViewModels group
│   ├── PrintLayoutControlViewModel.cs     # Move from root
│   └── PrintViewModel.cs                  # Move from root
├── Settings/                              # Renamed from SettingsForm
│   ├── MasterData/                        # Group master data operations
│   │   ├── MasterDataViewModels.cs        # Consolidate Add/Edit/Remove operations
│   │   └── MasterDataBaseViewModel.cs     # Base class for master data operations
│   ├── System/                            # System-related settings
│   │   ├── AboutViewModel.cs
│   │   ├── BackupRecoveryViewModel.cs

│   │   ├── DatabaseSettingsViewModel.cs
│   │   ├── SecurityPermissionsViewModel.cs
│   │   ├── ShortcutsViewModel.cs

│   │   └── SystemHealthViewModel.cs       # Consolidate Fixed version
│   ├── UI/                                # UI-related settings
│   │   └── ThemeBuilderViewModel.cs
│   ├── SettingsCategoryViewModel.cs

│   └── SettingsViewModel.cs

├── Shared/                                # Shared and base ViewModels
│   ├── BaseViewModel.cs                   # Keep as-is

│   ├── CustomDataGridViewModel.cs         # Keep as-is
│   └── TransferCustomDataGridViewModel.cs # Keep as-is
└── Transactions/                          # Renamed from TransactionsForm
    └── TransactionHistoryViewModel.cs     # Consolidate Fixed version

```

## 📋 Reorganization Tasks

### **Task 1: Remove Deprecated Code**

**Files to Remove:**

- `ViewModels/FilterPanelViewModel.cs.disabled`

**Files to Consolidate:**

- Merge `SystemHealthViewModel_Fixed.cs` into `SystemHealthViewModel.cs`

- Merge `TransactionHistoryViewModel_Fixed.cs` into `TransactionHistoryViewModel.cs`

### **Task 2: Create New Folder Structure**

**New Folders to Create:**

```bash
mkdir ViewModels/Application



mkdir ViewModels/MainForm/Overlays
mkdir ViewModels/MainForm/Panels

mkdir ViewModels/Print
mkdir ViewModels/Settings/MasterData
mkdir ViewModels/Settings/System
mkdir ViewModels/Settings/UI


mkdir ViewModels/Transactions
```

**Folders to Rename:**

- `ViewModels/SettingsForm` → `ViewModels/Settings`
- `ViewModels/TransactionsForm` → `ViewModels/Transactions`

### **Task 3: Move and Reorganize MainForm ViewModels**

**Move to ViewModels/Application:**

- `ViewModels/MainForm/MainWindowViewModel.cs` → `ViewModels/Application/MainWindowViewModel.cs`
- `ViewModels/ThemeEditorViewModel.cs` → `ViewModels/Application/ThemeEditorViewModel.cs`

**Move to ViewModels/MainForm/Overlays:**

- `ViewModels/MainForm/SuggestionOverlayViewModel.cs` → `ViewModels/MainForm/Overlays/SuggestionOverlayViewModel.cs`

**Move to ViewModels/MainForm/Panels:**

- `ViewModels/MainForm/AdvancedInventoryViewModel.cs` → `ViewModels/MainForm/Panels/AdvancedInventoryViewModel.cs`
- `ViewModels/MainForm/AdvancedRemoveViewModel.cs` → `ViewModels/MainForm/Panels/AdvancedRemoveViewModel.cs`
- `ViewModels/MainForm/InventoryTabViewModel.cs` → `ViewModels/MainForm/Panels/InventoryTabViewModel.cs`
- `ViewModels/MainForm/MainViewViewModel.cs` → `ViewModels/MainForm/Panels/MainViewViewModel.cs`

- `ViewModels/MainForm/QuickButtonsViewModel.cs` → `ViewModels/MainForm/Panels/QuickButtonsViewModel.cs`

**Rename and Move:**

- `ViewModels/MainForm/RemoveItemViewModel.cs` → `ViewModels/MainForm/Panels/RemoveTabViewModel.cs`
- `ViewModels/MainForm/TransferItemViewModel.cs` → `ViewModels/MainForm/Panels/TransferTabViewModel.cs`

**Handle Remaining Files:**

- `ViewModels/MainForm/AddItemViewModel.cs` - Analyze if this should become a base class or be relocated
- `ViewModels/MainForm/InventoryViewModel.cs` - Determine relationship to InventoryTabViewModel

- `ViewModels/MainForm/SearchInventoryViewModel.cs` - Determine if this should be in Panels or separate

### **Task 4: Reorganize Print ViewModels**

**Move to ViewModels/Print:**

- `ViewModels/PrintLayoutControlViewModel.cs` → `ViewModels/Print/PrintLayoutControlViewModel.cs`

- `ViewModels/PrintViewModel.cs` → `ViewModels/Print/PrintViewModel.cs`

### **Task 5: Consolidate Settings Master Data ViewModels**

**Current Master Data ViewModels (15 files):**

- Add/Edit/Remove × Part/Location/Operation/User/ItemType (15 combinations)

**Consolidation Strategy:**
Create `ViewModels/Settings/MasterData/MasterDataViewModels.cs` containing:

```csharp
namespace MTM_WIP_Application_Avalonia.ViewModels.Settings.MasterData;

// Base class for all master data operations
public abstract class MasterDataBaseViewModel : BaseViewModel

{
    // Common properties and methods

}

// Part management ViewModels
public class AddPartViewModel : MasterDataBaseViewModel { }
public class EditPartViewModel : MasterDataBaseViewModel { }

public class RemovePartViewModel : MasterDataBaseViewModel { }

// Location management ViewModels

public class AddLocationViewModel : MasterDataBaseViewModel { }
public class EditLocationViewModel : MasterDataBaseViewModel { }
public class RemoveLocationViewModel : MasterDataBaseViewModel { }



// Operation management ViewModels
public class AddOperationViewModel : MasterDataBaseViewModel { }
public class EditOperationViewModel : MasterDataBaseViewModel { }
public class RemoveOperationViewModel : MasterDataBaseViewModel { }


// User management ViewModels

public class AddUserViewModel : MasterDataBaseViewModel { }
public class EditUserViewModel : MasterDataBaseViewModel { }
public class RemoveUserViewModel : MasterDataBaseViewModel { }


// ItemType management ViewModels

public class AddItemTypeViewModel : MasterDataBaseViewModel { }

public class EditItemTypeViewModel : MasterDataBaseViewModel { }
public class RemoveItemTypeViewModel : MasterDataBaseViewModel { }

```

**Files to Consolidate:**

- All 15 Add/Edit/Remove master data ViewModels into single file
- Maintain separate interfaces if needed
- Preserve all existing functionality

### **Task 6: Reorganize Settings ViewModels**

**Move to ViewModels/Settings/System:**

- `ViewModels/SettingsForm/AboutViewModel.cs` → `ViewModels/Settings/System/AboutViewModel.cs`
- `ViewModels/SettingsForm/BackupRecoveryViewModel.cs` → `ViewModels/Settings/System/BackupRecoveryViewModel.cs`
- `ViewModels/SettingsForm/DatabaseSettingsViewModel.cs` → `ViewModels/Settings/System/DatabaseSettingsViewModel.cs`

- `ViewModels/SettingsForm/SecurityPermissionsViewModel.cs` → `ViewModels/Settings/System/SecurityPermissionsViewModel.cs`

- `ViewModels/SettingsForm/ShortcutsViewModel.cs` → `ViewModels/Settings/System/ShortcutsViewModel.cs`

- `ViewModels/SettingsForm/SystemHealthViewModel.cs` → `ViewModels/Settings/System/SystemHealthViewModel.cs`

**Move to ViewModels/Settings/UI:**

- `ViewModels/SettingsForm/ThemeBuilderViewModel.cs` → `ViewModels/Settings/UI/ThemeBuilderViewModel.cs`

**Keep in ViewModels/Settings root:**

- `ViewModels/SettingsForm/SettingsCategoryViewModel.cs` → `ViewModels/Settings/SettingsCategoryViewModel.cs`

- `ViewModels/SettingsForm/SettingsViewModel.cs` → `ViewModels/Settings/SettingsViewModel.cs`

### **Task 7: Handle Duplicate SuggestionOverlayViewModel**

**Analysis Required:**

- Compare `ViewModels/MainForm/SuggestionOverlayViewModel.cs` with `ViewModels/Overlay/SuggestionOverlayViewModel.cs`

- Determine which implementation is current and active
- Consolidate into single implementation in appropriate location
- Update all references

## 🔄 Reference Updates Required

### **Namespace Changes**

All moved ViewModels will require namespace updates:

```csharp
// Old namespaces
MTM_WIP_Application_Avalonia.ViewModels.MainForm
MTM_WIP_Application_Avalonia.ViewModels.SettingsForm

MTM_WIP_Application_Avalonia.ViewModels.TransactionsForm


// New namespaces

MTM_WIP_Application_Avalonia.ViewModels.Application
MTM_WIP_Application_Avalonia.ViewModels.MainForm.Panels


MTM_WIP_Application_Avalonia.ViewModels.MainForm.Overlays
MTM_WIP_Application_Avalonia.ViewModels.Print
MTM_WIP_Application_Avalonia.ViewModels.Settings.MasterData
MTM_WIP_Application_Avalonia.ViewModels.Settings.System

MTM_WIP_Application_Avalonia.ViewModels.Settings.UI

MTM_WIP_Application_Avalonia.ViewModels.Transactions
```

### **Files Requiring Reference Updates**

#### **Service Registration**

- `Extensions/ServiceCollectionExtensions.cs`

#### **View Code-behind Files**

- `MainWindow.axaml.cs`

- `Views/MainForm/Panels/*.axaml.cs`
- `Views/MainForm/Overlays/*.axaml.cs`
- `Views/SettingsForm/*.axaml.cs`
- `Views/PrintView.axaml.cs`

- `Views/PrintLayoutControl.axaml.cs`

#### **AXAML Files (DataContext binding)**

- `Views/MainForm/Panels/*.axaml`
- `Views/MainForm/Overlays/*.axaml`

- `Views/SettingsForm/*.axaml`
- `Views/*.axaml`

#### **Other ViewModels (dependencies)**

- Any ViewModel that references moved ViewModels

- Parent-child ViewModel relationships

## 📋 Migration Steps

### **Phase 1: Backup and Preparation (30 minutes)**

1. Create backup of ViewModels folder
2. Create git branch for reorganization
3. Document all current references
4. Create new folder structure

### **Phase 2: Remove Deprecated Code (30 minutes)**

1. Delete `FilterPanelViewModel.cs.disabled`

2. Merge Fixed versions with original files
3. Test compilation

### **Phase 3: Move and Rename Files (1 hour)**

1. Move files to new folder structure

2. Update namespace declarations
3. Rename files as needed
4. Update internal references

### **Phase 4: Consolidate Master Data ViewModels (1 hour)**

1. Create new consolidated file
2. Move implementations
3. Update interfaces if needed
4. Test functionality

### **Phase 5: Update References (1 hour)**

1. Update service registration
2. Update View code-behind files
3. Update AXAML DataContext bindings
4. Update inter-ViewModel references

### **Phase 6: Testing and Validation (30 minutes)**

1. Build solution
2. Test application functionality
3. Verify all Views load correctly
4. Test navigation between views

## ✅ Success Criteria

### **Structural Improvements**

- [ ] ViewModels structure mirrors Views structure
- [ ] Clear separation between Application, MainForm, Overlay, Print, Settings, and Transactions
- [ ] MainForm properly divided into Panels and Overlays
- [ ] Settings organized by functional area (MasterData, System, UI)

### **Code Quality**

- [ ] All deprecated code removed
- [ ] Duplicate ViewModels consolidated
- [ ] Consistent naming conventions
- [ ] Proper namespace organization

### **Functionality**

- [ ] All existing functionality preserved
- [ ] No broken references
- [ ] Application builds and runs correctly
- [ ] All Views load and display correctly

This reorganization will create a much cleaner and more maintainable ViewModel structure that directly mirrors the Views organization.
