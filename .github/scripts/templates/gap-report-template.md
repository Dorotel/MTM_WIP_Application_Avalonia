---
name: Gap Report Template
description: 'Implementation gap analysis template for MTM feature development'
applies_to: 'development/*'
development_context: true
template_type: 'gap-analysis'
quality_gate: 'development'
---

# MTM Feature Implementation Gap Report Template

**Branch**: {BRANCH-NAME}  
**Feature**: {FEATURE-NAME}  
**Generated**: {DATE-TIME}  
**Implementation Plan**: {PLAN-PATH}  
**Template Version**: 1.0

## Executive Summary

**Overall Progress**: {X}% complete  
**Critical Gaps**: {X} items requiring immediate attention  
**Ready for Testing**: {Yes/No}  
**Estimated Completion**: {X} hours of development time  
**MTM Pattern Compliance**: {X}% compliant  

### Quick Status

- 🟢 **Services**: {X/Y} implemented (following MTM service patterns)
- 🟡 **ViewModels**: {X/Y} implemented (MVVM Community Toolkit 8.3.2)
- 🔴 **Views**: {X/Y} implemented (Avalonia 11.3.4 syntax)
- 🟢 **Database**: {X/Y} stored procedures integrated
- 🟢 **DI Registration**: {X/Y} services registered in ServiceCollectionExtensions
- 🟡 **Theme Integration**: {X/Y} views using DynamicResource bindings

## Technology Stack Compliance

### .NET 8 + Avalonia 11.3.4 Compliance

- **Target Framework**: {✅/❌} .NET 8.0 (`<TargetFramework>net8.0</TargetFramework>`)
- **Avalonia Version**: {✅/❌} 11.3.4 (in MTM_WIP_Application_Avalonia.csproj)
- **Namespace Usage**: {✅/❌} `xmlns="https://github.com/avaloniaui"` (NOT WPF)
- **Control Naming**: {✅/❌} `x:Name` used (never `Name` attribute)

### MVVM Community Toolkit 8.3.2 Compliance

- **Source Generators**: {✅/❌} `[ObservableObject]` partial classes
- **Properties**: {✅/❌} `[ObservableProperty]` private fields
- **Commands**: {✅/❌} `[RelayCommand]` methods with proper async/await
- **Notifications**: {✅/❌} `[NotifyPropertyChangedFor]` dependencies
- **ReactiveUI Removal**: {✅/❌} No ReactiveUI patterns found

### MySQL 9.4.0 Database Integration

- **Stored Procedures Only**: {✅/❌} No direct SQL queries found
- **Parameter Construction**: {✅/❌} MySqlParameter arrays properly used
- **Service Integration**: {✅/❌} IDatabaseService dependency injection
- **Error Handling**: {✅/❌} ServiceResult pattern for database operations
- **Connection Management**: {✅/❌} IConfigurationService for connection strings

## File Status Analysis

### ✅ Fully Completed Files

```bash
#!/bin/bash
{COMPLETED_FILES_SECTION}
```

### 🔄 Partially Implemented Files

```bash
#!/bin/bash
{PARTIAL_FILES_SECTION}
```

```bash
#!/bin/bash
{PARTIAL_FILES_SECTION}
```

```bash
#!/bin/bash
{PARTIAL_FILES_SECTION}
```

### ❌ Missing Required Files

```bash
#!/bin/bash
{MISSING_FILES_SECTION}
```

## MTM Architecture Compliance Analysis

### Current Service Architecture (24 Services)

- **Configuration Services**: ConfigurationService, ApplicationStateService
- **Data Services**: DatabaseService, MasterDataService, InventoryEditingService, RemoveService  
- **UI Services**: ThemeService, NavigationService, SuggestionOverlayService, VirtualPanelManager
- **State Services**: SettingsService, SettingsPanelStateManager, StartupDialogService
- **Utility Services**: QuickButtonsService, FocusManagementService, FileServices, etc.

**Service Pattern Compliance**:

- **Interface/Implementation**: {✅/❌} All services have proper abstractions
- **Constructor Injection**: {✅/❌} ArgumentNullException.ThrowIfNull validation
- **Lifetime Management**: {✅/❌} Proper Singleton/Scoped/Transient registration
- **Error Handling**: {✅/❌} Centralized via Services.ErrorHandling

### Theme System Integration (19 Themes Available)

**Available Themes**: MTM_Blue, MTM_Blue_Dark, MTM_Green, MTM_Green_Dark, MTM_Red, MTM_Red_Dark, MTM_Amber, MTM_Teal, MTM_Indigo, MTM_Rose, MTM_Emerald, MTM_Light, MTM_Dark, MTM_HighContrast, etc.

- **DynamicResource Usage**: {✅/❌} All colors use DynamicResource bindings
- **Theme Service**: {✅/❌} ThemeService properly registered and functional
- **Resource Files**: {✅/❌} Theme AXAML files in Resources/Themes/
- **Cross-platform Support**: {✅/❌} Themes work on Windows/macOS/Linux

### Database Integration Patterns

**Required Stored Procedures**:

- **Inventory**: inv_inventory_Add_Item, inv_inventory_Remove_Item, inv_inventory_Get_ByPartID
- **Transactions**: inv_transaction_Add, inv_transaction_Get_History  
- **Master Data**: md_part_ids_Get_All, md_locations_Get_All, md_operation_numbers_Get_All
- **QuickButtons**: qb_quickbuttons_Get_ByUser, qb_quickbuttons_Save
- **Error Logging**: log_error_Add_Error, log_error_Get_All
- **System**: sys_health_check

**Database Compliance**:

- **Stored Procedure Only**: {✅/❌} No direct SQL queries found
- **Helper Usage**: {✅/❌} Helper_Database_StoredProcedure.ExecuteDataTableWithStatus used
- **Parameter Construction**: {✅/❌} MySqlParameter arrays properly constructed
- **Result Processing**: {✅/❌} Status checking and DataTable processing

- **Status**: {STATUS}
- **Details**: {REGISTRATION_DETAILS}

### Navigation Integration

- **Status**: {STATUS}
- **Implementation**: {NAVIGATION_STATUS}
- **Action Required**: {NAVIGATION_ACTIONS}

### Theme System Integration

- **Status**: {STATUS}
- **Complete**: {THEME_COMPLETE}
- **Missing**: {THEME_MISSING}

### Error Handling

- **Status**: {STATUS}
- **Coverage**: {ERROR_COVERAGE}
- **Action Required**: {ERROR_ACTIONS}

## Priority Gap Analysis

### 🚨 Critical Priority (Blocking Issues)

{CRITICAL_GAPS}

### ⚠️ High Priority (Feature Incomplete)

{HIGH_PRIORITY_GAPS}

### 📋 Medium Priority (Enhancement)

{MEDIUM_PRIORITY_GAPS}

## Database Integration Status

### Stored Procedures Pattern Compliance

- **Status**: {STATUS}
- **Pattern**: Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()
- **Issues**: {DATABASE_ISSUES}
- **Action Required**: {DATABASE_ACTIONS}

## Integration Points Status

### NavigationService Integration

- **Current Status**: {NAV_STATUS}
- **Missing Components**: {NAV_MISSING}
- **Dependencies**: {NAV_DEPENDENCIES}

### ThemeService Integration  

- **Current Status**: {THEME_STATUS}
- **Missing Components**: {THEME_MISSING_COMPONENTS}
- **Dependencies**: {THEME_DEPENDENCIES}

### ErrorHandling Integration

- **Current Status**: {ERROR_STATUS}
- **Missing Components**: {ERROR_MISSING}
- **Dependencies**: {ERROR_DEPENDENCIES}

## Next Development Session Action Plan

### Immediate Tasks (Next 2 Hours)

{IMMEDIATE_TASKS}

### Secondary Tasks (Next 4 Hours)

{SECONDARY_TASKS}

## Quality Assurance Checklist

### Pre-Testing Requirements

- [ ] All critical priority gaps resolved
- [ ] No compilation errors or warnings
- [ ] All services properly registered in ServiceCollectionExtensions
- [ ] Navigation integration functional
- [ ] MTM theme compatibility verified

### Testing Scenarios

- [ ] Feature accessible from main navigation
- [ ] All user workflows complete successfully  
- [ ] Error handling graceful for edge cases
- [ ] Theme switching works across all variants
- [ ] Performance meets MTM standards (< 2s for typical operations)

## Development Session Notes

### Context for Next Session

- **Last Focus Area**: {LAST_FOCUS}
- **Current Blocker**: {CURRENT_BLOCKER}
- **Next Logical Step**: {NEXT_STEP}
- **Architecture Decisions**: {ARCHITECTURE_NOTES}

### Code Review Notes

- **Pattern Compliance**: {PATTERN_NOTES}
- **Performance Concerns**: {PERFORMANCE_NOTES}
- **Security Considerations**: {SECURITY_NOTES}
- **Maintainability Issues**: {MAINTAINABILITY_NOTES}

---

**Template Generated**: {DATE-TIME}  
**Usage**: Fill in placeholders with actual implementation data  
**MTM Gap Report Template**: v1.0
