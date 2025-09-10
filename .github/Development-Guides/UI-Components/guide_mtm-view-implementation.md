# MTM View Implementation Guide
**Complete Documentation for Implementing Views in the MTM WIP Application**

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [Technology Stack & Architecture](#technology-stack--architecture)
3. [Pre-Implementation Requirements](#pre-implementation-requirements)
4. [Step-by-Step Implementation Process](#step-by-step-implementation-process)
5. [File Structure & Naming Conventions](#file-structure--naming-conventions)
6. [AXAML Syntax & Avalonia Guidelines](#axaml-syntax--avalonia-guidelines)
7. [ViewModel Implementation](#viewmodel-implementation)
8. [Service Integration](#service-integration)
9. [Database Integration Patterns](#database-integration-patterns)
10. [UI Design System & Theming](#ui-design-system--theming)
11. [Error Handling & Validation](#error-handling--validation)
12. [Testing Strategies](#testing-strategies)
13. [Common Issues & Troubleshooting](#common-issues--troubleshooting)
14. [Resources & Reference Files](#resources--reference-files)

---

## Overview

This guide provides comprehensive instructions for implementing new views in the MTM WIP (Work-In-Progress) Application, a manufacturing inventory management system. The application follows strict architectural patterns using .NET 8, Avalonia UI, MVVM Community Toolkit, and MySQL database with stored procedures.

### Key Principles
- **Consistency**: Follow established patterns found in the existing codebase
- **Manufacturing Focus**: Understand the manufacturing business domain (parts, operations, inventory)
- **Theme Compliance**: Use MTM theme system with Windows 11 Blue (#0078D4) primary colors
- **Stored Procedures Only**: All database operations must use stored procedures
- **MVVM Community Toolkit**: Use source generator patterns exclusively

---

## Technology Stack & Architecture

### Core Technologies
- **.NET 8.0** with C# 12 and nullable reference types enabled
- **Avalonia UI 11.3.4** (NOT WPF) for cross-platform desktop UI
- **MVVM Community Toolkit 8.3.2** with source generators for properties and commands
- **MySQL 9.4.0** database with MySql.Data package
- **Microsoft Extensions 9.0.8** for dependency injection, logging, and configuration

### Architecture Patterns
- **MVVM with Service-Oriented Design**: Clean separation of concerns
- **Comprehensive Dependency Injection**: Service registration via `ServiceCollectionExtensions`
- **Category-Based Service Consolidation**: Multiple related services in single files
- **Centralized Error Handling**: Via `Services.ErrorHandling.HandleErrorAsync()`
- **Manufacturing Business Domain**: Operation numbers as workflow steps, user intent for transaction types

### Reference Files to Study First
```
📁 Essential Study Files (READ THESE FIRST):
├── .github/copilot-instructions.md (Main instruction file)
├── .github/prompts/mtm-feature-request.prompt.md
├── .github/instructions/avalonia-ui-guidelines.instructions.md
├── .github/instructions/mvvm-community-toolkit.instructions.md
├── .github/instructions/mysql-database-patterns.instructions.md
└── Views/MainForm/InventoryTabView.axaml (Reference implementation)
```

---

## Pre-Implementation Requirements

### 1. Understand the Business Domain
**Manufacturing Context**: The MTM application manages manufacturing inventory with these key concepts:
- **Part IDs**: Unique identifiers for manufactured parts (e.g., "PART001", "ABC-123")
- **Operation Numbers**: Workflow step identifiers ("90", "100", "110", "120") - NOT transaction types
- **Locations**: Physical locations where inventory is stored
- **Transaction Types**: Determined by user intent (IN/OUT/TRANSFER), not operation numbers
- **Quantities**: Integer counts only, no fractional quantities

### 2. Study Existing View Examples
Before implementing, examine these reference views:
```
📁 Reference View Examples:
├── Views/MainForm/InventoryTabView.axaml (PRIMARY REFERENCE)
├── Views/MainForm/RemoveTabView.axaml
├── Views/MainForm/TransferTabView.axaml
├── Views/SettingsForm/ThemeSettingsView.axaml
└── Views/TransactionsForm/TransactionHistoryView.axaml
```

### 3. Review Available Services
Study the service layer structure:
```csharp
📁 Services Directory:
├── ErrorHandling.cs (Centralized error handling - MANDATORY)
├── Configuration.cs (App configuration and state)
├── Database.cs (Database helper utilities)
├── MasterDataService.cs (Part IDs, locations, operations)
├── Navigation.cs (View navigation logic)
├── QuickButtons.cs (Quick action management)
├── ThemeService.cs (UI theme management)
└── More specialized services...
```

### 4. Understand Database Patterns
**CRITICAL**: All database access uses stored procedures only. Study these files:
```
📁 Database Documentation:
├── .github/instructions/mysql-database-patterns.instructions.md
├── .github/instructions/service-architecture.instructions.md
└── .github/instructions/data-models.instructions.md
```

---

## Step-by-Step Implementation Process

### Phase 1: Planning and Design

#### Step 1: Define the View Purpose
**Create a planning document** answering:
- What business function will this view serve?
- What data needs to be displayed/entered?
- What actions can users perform?
- How does it integrate with existing workflows?

#### Step 2: Identify Required Services
**Determine service dependencies**:
```csharp
// Common service dependencies for views:
- ILogger<ViewModelName> (Required for all ViewModels)
- MasterDataService (For dropdowns: parts, operations, locations)
- DatabaseService or specialized service (For data operations)
- ErrorHandling (For error management - used statically)
- ConfigurationService (For app settings)
- Navigation (For view transitions)
```

#### Step 3: Design the Data Flow
**Map the data flow**:
1. **Input**: What user inputs are needed?
2. **Processing**: What business logic will process the data?
3. **Output**: What results are displayed to the user?
4. **Storage**: What data is persisted to the database?

### Phase 2: File Creation

#### Step 4: Create the File Structure
**Create files in this order**:
```bash
# 1. ViewModel first (defines data structure)
ViewModels/[Area]/[Name]ViewModel.cs

# 2. View AXAML file
Views/[Area]/[Name]View.axaml

# 3. View code-behind
Views/[Area]/[Name]View.axaml.cs

# 4. Service if needed (optional)
Services/[Name]Service.cs

# 5. Models if needed (optional)
Models/[Name]Model.cs
```

#### Step 5: Implement the ViewModel
**Use MVVM Community Toolkit patterns**:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.ViewModels.[Area];

[ObservableObject]
public partial class [Name]ViewModel : BaseViewModel
{
    #region Private Fields
    private readonly IRequiredService _service;
    #endregion

    #region Observable Properties
    [ObservableProperty]
    private string inputValue = string.Empty;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private ObservableCollection<ItemModel> items = new();
    #endregion

    #region Commands
    [RelayCommand]
    private async Task ExecuteAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            // Implementation
        });
    }
    #endregion

    #region Constructor
    public [Name]ViewModel(
        ILogger<[Name]ViewModel> logger,
        IRequiredService service)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(service);
        
        _service = service;
    }
    #endregion
}
```

---

## Implementation Checklist

### Pre-Implementation ✅
- [ ] Read all core instruction files
- [ ] Understand the manufacturing business domain
- [ ] Study reference implementations
- [ ] Identify required services and dependencies
- [ ] Plan the data flow and user interactions

### File Creation ✅
- [ ] Create ViewModel with MVVM Community Toolkit patterns
- [ ] Create AXAML view with correct Avalonia syntax
- [ ] Create minimal code-behind
- [ ] Register services in DI container
- [ ] Add navigation integration if needed

### Implementation ✅
- [ ] Use MTM theme resources for all colors
- [ ] Implement mandatory tab view layout pattern
- [ ] Use stored procedures only for database operations
- [ ] Implement centralized error handling
- [ ] Add input validation where appropriate
- [ ] Test data binding and command execution

### Testing ✅
- [ ] No AVLN2000 compilation errors
- [ ] UI elements display correctly
- [ ] Data binding works properly
- [ ] Commands execute without errors
- [ ] Error handling displays user-friendly messages
- [ ] Theme consistency maintained
- [ ] No UI overflow issues

---

**For complete implementation details, see the full guide in the original documentation or reference the instruction files in `.github/instructions/`.**

---

**Document Version**: 1.0  
**Last Updated**: September 10, 2025  
**Created by**: MTM Development Team Documentation System  
**Status**: Migration from docs/development/view-management-md-files/  