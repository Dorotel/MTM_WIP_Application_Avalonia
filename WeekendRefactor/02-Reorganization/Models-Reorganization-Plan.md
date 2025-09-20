# Models Reorganization Plan

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: January 18, 2025  
**Target Audience**: MTM Application Development Team  

## 🎯 Reorganization Overview

This document outlines the consolidation and reorganization of the MTM Models folder to implement a folder-based structure with `{Folder}.{Model}.cs` naming pattern, matching the Services folder organization and improving logical grouping while preserving all existing functionality.

## 📊 Current Models Analysis

### **Current Structure (21 Files)**

```
Models/
├── EditInventoryModel.cs
├── EditInventoryResult.cs
├── EventArgs.cs
├── FocusManagementEventArgs.cs
├── InventoryEventArgs.cs
├── InventorySavedEventArgs.cs
├── Model_AppVariables.cs
├── PrintModel.cs
├── PrintTemplateModel.cs
├── SessionTransaction.cs
├── ViewModels.cs
├── CustomDataGrid/
│   ├── AutoCompleteColumn.cs
│   ├── DataGridColumnConfiguration.cs
│   ├── DataGridPreferences.cs
│   ├── EditResult.cs
│   ├── FrozenColumn.cs
│   └── VisibilityColumn.cs
├── Overlay/
│   ├── DynamicOverlayContent.cs
│   └── OverlayConfiguration.cs
└── Shared/
    ├── BaseViewModel.cs
    └── RowSelectionModel.cs
```

### **Functional Grouping Analysis**

#### **Core Business Models (4 files → 1 consolidated file)**

- `Model_AppVariables.cs` - Application state and global variables
- `EditInventoryModel.cs` - Inventory editing data model
- `EditInventoryResult.cs` - Result wrapper for inventory operations
- `SessionTransaction.cs` - Transaction session management

**Target**: `Models/Core/Core.AppVariables.cs`, `Core.EditInventoryModel.cs`, `Core.EditInventoryResult.cs`, `Core.SessionTransaction.cs`

#### **Event Models (4 files → 1 consolidated file)**

- `EventArgs.cs` - Generic event arguments base
- `FocusManagementEventArgs.cs` - Focus management events
- `InventoryEventArgs.cs` - Inventory-related events  
- `InventorySavedEventArgs.cs` - Inventory save completion events

**Target**: `Models/Events/Events.EventArgs.cs`, `Events.FocusManagementEventArgs.cs`, `Events.InventoryEventArgs.cs`, `Events.InventorySavedEventArgs.cs`

#### **UI Models (6 files → 1 consolidated folder)**

- `CustomDataGrid/AutoCompleteColumn.cs` - AutoComplete column configuration
- `CustomDataGrid/DataGridColumnConfiguration.cs` - Column setup configuration
- `CustomDataGrid/DataGridPreferences.cs` - User preference storage
- `CustomDataGrid/EditResult.cs` - Edit operation results
- `CustomDataGrid/FrozenColumn.cs` - Column freezing configuration
- `CustomDataGrid/VisibilityColumn.cs` - Column visibility management

**Target**: `Models/UI/UI.CustomDataGrid.AutoCompleteColumn.cs`, `UI.CustomDataGrid.DataGridColumnConfiguration.cs`, etc.

#### **Overlay Models (2 files → maintained structure)**

- `Overlay/DynamicOverlayContent.cs` - Dynamic overlay content model
- `Overlay/OverlayConfiguration.cs` - Overlay configuration settings

**Target**: `Models/Overlay/Overlay.DynamicOverlayContent.cs`, `Overlay.OverlayConfiguration.cs`

#### **Print Models (2 files → 1 consolidated file)**

- `PrintModel.cs` - Print operation data model
- `PrintTemplateModel.cs` - Print template configuration

**Target**: `Models/Print/Print.PrintModel.cs`, `Print.PrintTemplateModel.cs`

#### **Shared Models (3 files → 1 consolidated file)**

- `ViewModels.cs` - Shared ViewModel definitions
- `Shared/BaseViewModel.cs` - Base ViewModel class
- `Shared/RowSelectionModel.cs` - Row selection state model

**Target**: `Models/Shared/Shared.ViewModels.cs`, `Shared.BaseViewModel.cs`, `Shared.RowSelectionModel.cs`

## 🏗️ Target Structure (6 Folders, 21 Files)

### **New Folder-Based Organization**

```
Models/
├── Core/
│   ├── Core.AppVariables.cs (from Model_AppVariables.cs)
│   ├── Core.EditInventoryModel.cs (from EditInventoryModel.cs)
│   ├── Core.EditInventoryResult.cs (from EditInventoryResult.cs)
│   └── Core.SessionTransaction.cs (from SessionTransaction.cs)
├── Events/
│   ├── Events.EventArgs.cs (from EventArgs.cs)
│   ├── Events.FocusManagementEventArgs.cs (from FocusManagementEventArgs.cs)
│   ├── Events.InventoryEventArgs.cs (from InventoryEventArgs.cs)
│   └── Events.InventorySavedEventArgs.cs (from InventorySavedEventArgs.cs)
├── UI/
│   ├── UI.CustomDataGrid.AutoCompleteColumn.cs (from CustomDataGrid/AutoCompleteColumn.cs)
│   ├── UI.CustomDataGrid.DataGridColumnConfiguration.cs (from CustomDataGrid/DataGridColumnConfiguration.cs)
│   ├── UI.CustomDataGrid.DataGridPreferences.cs (from CustomDataGrid/DataGridPreferences.cs)
│   ├── UI.CustomDataGrid.EditResult.cs (from CustomDataGrid/EditResult.cs)
│   ├── UI.CustomDataGrid.FrozenColumn.cs (from CustomDataGrid/FrozenColumn.cs)
│   └── UI.CustomDataGrid.VisibilityColumn.cs (from CustomDataGrid/VisibilityColumn.cs)
├── Overlay/
│   ├── Overlay.DynamicOverlayContent.cs (from Overlay/DynamicOverlayContent.cs)
│   └── Overlay.OverlayConfiguration.cs (from Overlay/OverlayConfiguration.cs)
├── Print/
│   ├── Print.PrintModel.cs (from PrintModel.cs)
│   └── Print.PrintTemplateModel.cs (from PrintTemplateModel.cs)
├── Shared/
│   ├── Shared.ViewModels.cs (from ViewModels.cs)
│   ├── Shared.BaseViewModel.cs (from Shared/BaseViewModel.cs)
│   └── Shared.RowSelectionModel.cs (from Shared/RowSelectionModel.cs)
└── MODEL_DEPENDENCY_ANALYSIS.md (existing analysis file)
```

## 🔧 Implementation Strategy

### **Phase 1: Analysis Validation**

1. **Review MODEL_DEPENDENCY_ANALYSIS.md**
   - Confirm all 21 model files are categorized correctly
   - Validate dependency relationships between models
   - Ensure no circular dependencies exist

2. **Scan All Model References**
   - Use semantic search across ViewModels, Services, and Views
   - Identify all `using` statements for Models namespace
   - Document model usage patterns for validation

### **Phase 2: Create New Folder Structure**

1. **Create Category Folders**

   ```bash
   mkdir Models/Core
   mkdir Models/Events  
   mkdir Models/UI
   mkdir Models/Overlay    # Already exists - rename files only
   mkdir Models/Print
   mkdir Models/Shared     # Already exists - rename files only
   ```

2. **Apply {Folder}.{Model}.cs Naming Pattern**
   - Follow exact pattern used in Services reorganization
   - Maintain original class names within files
   - Update file names only, preserve internal structure

### **Phase 3: File Migration and Renaming**

#### **Core Models Migration**

```bash
# Move and rename core business models
mv Models/Model_AppVariables.cs Models/Core/Core.AppVariables.cs
mv Models/EditInventoryModel.cs Models/Core/Core.EditInventoryModel.cs  
mv Models/EditInventoryResult.cs Models/Core/Core.EditInventoryResult.cs
mv Models/SessionTransaction.cs Models/Core/Core.SessionTransaction.cs

# Update namespaces in each file
# From: namespace MTM_WIP_Application_Avalonia.Models
# To:   namespace MTM_WIP_Application_Avalonia.Models.Core
```

#### **Events Models Migration**

```bash
# Move and rename event models
mv Models/EventArgs.cs Models/Events/Events.EventArgs.cs
mv Models/FocusManagementEventArgs.cs Models/Events/Events.FocusManagementEventArgs.cs
mv Models/InventoryEventArgs.cs Models/Events/Events.InventoryEventArgs.cs
mv Models/InventorySavedEventArgs.cs Models/Events/Events.InventorySavedEventArgs.cs

# Update namespaces in each file
# From: namespace MTM_WIP_Application_Avalonia.Models
# To:   namespace MTM_WIP_Application_Avalonia.Models.Events
```

#### **UI Models Migration**

```bash
# Move and rename CustomDataGrid models
mv Models/CustomDataGrid/AutoCompleteColumn.cs Models/UI/UI.CustomDataGrid.AutoCompleteColumn.cs
mv Models/CustomDataGrid/DataGridColumnConfiguration.cs Models/UI/UI.CustomDataGrid.DataGridColumnConfiguration.cs
mv Models/CustomDataGrid/DataGridPreferences.cs Models/UI/UI.CustomDataGrid.DataGridPreferences.cs
mv Models/CustomDataGrid/EditResult.cs Models/UI/UI.CustomDataGrid.EditResult.cs
mv Models/CustomDataGrid/FrozenColumn.cs Models/UI/UI.CustomDataGrid.FrozenColumn.cs
mv Models/CustomDataGrid/VisibilityColumn.cs Models/UI/UI.CustomDataGrid.VisibilityColumn.cs

# Remove empty CustomDataGrid folder
rmdir Models/CustomDataGrid

# Update namespaces in each file
# From: namespace MTM_WIP_Application_Avalonia.Models.CustomDataGrid
# To:   namespace MTM_WIP_Application_Avalonia.Models.UI
```

#### **Overlay Models Migration**

```bash
# Rename existing overlay models (folder already exists)
mv Models/Overlay/DynamicOverlayContent.cs Models/Overlay/Overlay.DynamicOverlayContent.cs
mv Models/Overlay/OverlayConfiguration.cs Models/Overlay/Overlay.OverlayConfiguration.cs

# Update namespaces in each file  
# From: namespace MTM_WIP_Application_Avalonia.Models.Overlay
# To:   namespace MTM_WIP_Application_Avalonia.Models.Overlay (no change)
```

#### **Print Models Migration**

```bash
# Move and rename print models
mv Models/PrintModel.cs Models/Print/Print.PrintModel.cs
mv Models/PrintTemplateModel.cs Models/Print/Print.PrintTemplateModel.cs

# Update namespaces in each file
# From: namespace MTM_WIP_Application_Avalonia.Models
# To:   namespace MTM_WIP_Application_Avalonia.Models.Print
```

#### **Shared Models Migration**

```bash
# Move and rename shared models
mv Models/ViewModels.cs Models/Shared/Shared.ViewModels.cs
mv Models/Shared/BaseViewModel.cs Models/Shared/Shared.BaseViewModel.cs
mv Models/Shared/RowSelectionModel.cs Models/Shared/Shared.RowSelectionModel.cs

# Update namespaces in each file
# From: namespace MTM_WIP_Application_Avalonia.Models.Shared
# To:   namespace MTM_WIP_Application_Avalonia.Models.Shared (no change)
```

### **Phase 4: Update All References**

#### **ViewModels References Update**

```bash
# Scan all ViewModels for model usage
find ViewModels/ -name "*.cs" -exec grep -l "using.*Models" {} \;

# Update using statements
# From: using MTM_WIP_Application_Avalonia.Models;
# To:   using MTM_WIP_Application_Avalonia.Models.Core;
#       using MTM_WIP_Application_Avalonia.Models.Events;
#       using MTM_WIP_Application_Avalonia.Models.UI;
```

#### **Services References Update**

```bash
# Scan all Services for model usage
find Services/ -name "*.cs" -exec grep -l "using.*Models" {} \;

# Update using statements for reorganized namespaces
# Focus on: ErrorHandling.cs, InventoryEditingService.cs, PrintService.cs
```

#### **Views References Update**

```bash
# Scan all Views for model usage (particularly in AXAML bindings)
find Views/ -name "*.cs" -exec grep -l "Models\." {} \;
find Views/ -name "*.axaml" -exec grep -l "Models:" {} \;

# Update AXAML namespace declarations if needed
```

### **Phase 5: Dependency Injection Updates**

```bash
# Update ServiceCollectionExtensions.cs if needed
# Check for any model registrations that need namespace updates
# Typically models aren't registered in DI but check for any exceptions
```

### **Phase 6: Validation and Testing**

1. **Compilation Validation**

   ```bash
   dotnet build --no-restore
   # Ensure no compilation errors
   # Fix any missing using statements
   ```

2. **Functionality Testing**

   ```bash
   # Test key functionality:
   # - Inventory operations (Core models)
   # - Event handling (Events models)  
   # - CustomDataGrid operations (UI models)
   # - Overlay functionality (Overlay models)
   # - Print operations (Print models)
   ```

3. **Reference Integrity Check**

   ```bash
   # Use search tools to verify no broken references
   grep -r "Models\." . --include="*.cs" --include="*.axaml"
   # Ensure all found references use new namespaces
   ```

## 🎯 Success Criteria

- [x] **MODEL_DEPENDENCY_ANALYSIS.md created** - Complete analysis document exists
- [ ] **All 21 model files migrated** - Files moved to appropriate folders with correct naming
- [ ] **Namespaces updated** - All internal namespaces match new folder structure
- [ ] **References updated** - All using statements across codebase updated
- [ ] **Application compiles** - No compilation errors after migration
- [ ] **All functionality preserved** - Core features work as before reorganization
- [ ] **Consistent naming pattern** - {Folder}.{Model}.cs pattern applied throughout

## 📋 Implementation Checklist

### **Pre-Migration Setup**

- [ ] Backup current Models folder
- [ ] Review MODEL_DEPENDENCY_ANALYSIS.md
- [ ] Create new folder structure (Core, Events, UI, Print)
- [ ] Identify all model references across codebase

### **Core Models (4 files)**

- [ ] Move Model_AppVariables.cs → Core.AppVariables.cs
- [ ] Move EditInventoryModel.cs → Core.EditInventoryModel.cs  
- [ ] Move EditInventoryResult.cs → Core.EditInventoryResult.cs
- [ ] Move SessionTransaction.cs → Core.SessionTransaction.cs
- [ ] Update all Core model namespaces

### **Events Models (4 files)**  

- [ ] Move EventArgs.cs → Events.EventArgs.cs
- [ ] Move FocusManagementEventArgs.cs → Events.FocusManagementEventArgs.cs
- [ ] Move InventoryEventArgs.cs → Events.InventoryEventArgs.cs
- [ ] Move InventorySavedEventArgs.cs → Events.InventorySavedEventArgs.cs
- [ ] Update all Events model namespaces

### **UI Models (6 files)**

- [ ] Move AutoCompleteColumn.cs → UI.CustomDataGrid.AutoCompleteColumn.cs
- [ ] Move DataGridColumnConfiguration.cs → UI.CustomDataGrid.DataGridColumnConfiguration.cs
- [ ] Move DataGridPreferences.cs → UI.CustomDataGrid.DataGridPreferences.cs  
- [ ] Move EditResult.cs → UI.CustomDataGrid.EditResult.cs
- [ ] Move FrozenColumn.cs → UI.CustomDataGrid.FrozenColumn.cs
- [ ] Move VisibilityColumn.cs → UI.CustomDataGrid.VisibilityColumn.cs
- [ ] Update all UI model namespaces
- [ ] Remove empty CustomDataGrid folder

### **Overlay Models (2 files)**

- [ ] Rename DynamicOverlayContent.cs → Overlay.DynamicOverlayContent.cs  
- [ ] Rename OverlayConfiguration.cs → Overlay.OverlayConfiguration.cs
- [ ] Verify Overlay model namespaces

### **Print Models (2 files)**

- [ ] Move PrintModel.cs → Print.PrintModel.cs
- [ ] Move PrintTemplateModel.cs → Print.PrintTemplateModel.cs
- [ ] Update all Print model namespaces

### **Shared Models (3 files)**

- [ ] Move ViewModels.cs → Shared.ViewModels.cs
- [ ] Rename BaseViewModel.cs → Shared.BaseViewModel.cs
- [ ] Rename RowSelectionModel.cs → Shared.RowSelectionModel.cs  
- [ ] Update all Shared model namespaces

### **Reference Updates**

- [ ] Update all ViewModels using statements
- [ ] Update all Services using statements
- [ ] Update all Views using statements  
- [ ] Update any AXAML namespace declarations
- [ ] Check ServiceCollectionExtensions.cs for registrations

### **Validation**

- [ ] Build application successfully
- [ ] Test inventory operations (Core models)
- [ ] Test event handling (Events models)
- [ ] Test CustomDataGrid functionality (UI models)  
- [ ] Test overlay systems (Overlay models)
- [ ] Test print operations (Print models)
- [ ] Verify shared components work (Shared models)

## 🔄 Rollback Plan

If issues arise during migration:

1. **Restore from backup** - Revert Models folder to original state
2. **Reset using statements** - Revert all modified using statements  
3. **Rebuild application** - Ensure original functionality restored
4. **Analyze failure points** - Document what caused issues
5. **Plan remediation** - Address issues before retry

## 📚 Related Documentation

- `Models/MODEL_DEPENDENCY_ANALYSIS.md` - Comprehensive dependency analysis
- `WeekendRefactor/Master-Refactor-Implementation-Plan.md` - Overall project plan  
- `Services/SERVICE_DEPENDENCY_ANALYSIS.md` - Services reorganization reference
- `.github/instructions/data-models.instructions.md` - Model development guidelines

---

**End of Models Reorganization Plan**
