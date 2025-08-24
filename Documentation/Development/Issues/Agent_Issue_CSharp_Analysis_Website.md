# Agent Issue: Generate C# Code Analysis Website

## Issue Type
**Agent Task** - Automated code analysis and documentation generation

## Priority
**Medium** - Documentation and analysis tool for development workflow

## Assigned Agent
**Code Analysis Agent** - Specialized in static code analysis and relationship mapping

## Issue Description

### Objective
Generate a comprehensive website that analyzes and displays information about all C# files in the MTM WIP Application Avalonia project, including:
- File structure and organization
- Class relationships and dependencies
- Method signatures and documentation
- MVVM pattern compliance
- Service dependency mapping
- Interface implementations
- ReactiveUI pattern usage

### Scope
Analyze all 41 C# files in the project and create an interactive website for development team reference.

## Technical Requirements

### 1. Website Structure
```
Documentation/Development/CodeAnalysis/
??? index.html (Main dashboard)
??? css/
?   ??? mtm-theme.css (MTM purple branding)
?   ??? analysis.css (Analysis-specific styles)
??? js/
?   ??? analysis.js (Interactive functionality)
?   ??? search.js (File/class search)
??? data/
?   ??? files.json (C# file metadata)
?   ??? relationships.json (Dependencies/relationships)
?   ??? patterns.json (MVVM/ReactiveUI pattern analysis)
??? pages/
    ??? services/
    ??? viewmodels/
    ??? views/
    ??? models/
    ??? detailed/ (Individual file analysis pages)
```

### 2. Analysis Categories

#### A. File Organization Analysis
- **Services Layer**: Analyze all service classes and their interfaces
  - `ConfigurationService.cs`, `DatabaseService.cs`, `InventoryService.cs`, etc.
  - Interface implementations (`ICoreServices`, `IBusinessServices`, `INavigationService`)
  - Service registration patterns in `ServiceCollectionExtensions.cs`

#### B. MVVM Pattern Compliance
- **ViewModels**: Analyze inheritance from `BaseViewModel`
  - ReactiveUI pattern usage (`WhenAnyValue`, `ReactiveCommand`)
  - Property binding patterns
  - Command implementations
- **Views**: Code-behind analysis and MVVM compliance
- **Models**: Data structure analysis (`CoreModels.cs`, `Result.cs`, `ResultPattern.cs`)

#### C. Dependency Mapping
- Service injection patterns
- Interface dependencies
- Circular dependency detection
- Unused service identification

#### D. Code Quality Metrics
- Method complexity analysis
- Documentation coverage (XML comments)
- Error handling patterns
- Async/await usage

### 3. Interactive Features

#### A. Visual Relationship Graph
- Interactive dependency graph using D3.js or similar
- Color-coded by layer (Services=Blue, ViewModels=Green, Views=Purple, Models=Orange)
- Click to drill down into specific relationships

#### B. Search and Filter
- Real-time search by file name, class name, method name
- Filter by:
  - File type (Service, ViewModel, View, Model)
  - Pattern compliance (ReactiveUI, MVVM)
  - Documentation status
  - Dependency count

#### C. Analysis Dashboard
- Summary cards showing:
  - Total files analyzed
  - MVVM compliance percentage
  - Services with/without interfaces
  - Documentation coverage
  - Pattern usage statistics

### 4. MTM Branding and Styling

#### A. Color Scheme
```css
:root {
  --mtm-primary: #6B46C1; /* MTM Purple */
  --mtm-secondary: #9333EA;
  --mtm-accent: #A855F7;
  --mtm-background: #F8FAFC;
  --mtm-surface: #FFFFFF;
  --mtm-text: #1E293B;
  --mtm-text-secondary: #64748B;
}
```

#### B. Design Elements
- Modern card-based layout
- MTM gradient backgrounds
- Clean typography (Inter or similar)
- Responsive design for all screen sizes
- Dark mode toggle

### 5. Data Generation Process

#### A. C# File Analysis Script
Create automated script to:
1. Parse all 41 C# files in the project
2. Extract class information, methods, properties
3. Identify interface implementations
4. Map service dependencies
5. Analyze ReactiveUI pattern usage
6. Generate JSON data files

#### B. Pattern Recognition
- **ReactiveUI Patterns**: `WhenAnyValue`, `ReactiveCommand`, `ObservableAsPropertyHelper`
- **MVVM Patterns**: Property binding, command binding, view-viewmodel pairing
- **Service Patterns**: Constructor injection, interface usage, singleton/scoped patterns
- **Error Handling**: Usage of `Service_ErrorHandler`, try-catch patterns

### 6. Specific File Analysis Focus

#### A. High-Priority Files for Analysis
1. **Service Layer**:
   - `ServiceCollectionExtensions.cs` - DI registration patterns
   - `DatabaseService.cs` - Database access patterns
   - `Service_ErrorHandler.cs` - Error handling implementation
   - `NavigationService.cs` - Navigation patterns

2. **ViewModels**:
   - `BaseViewModel.cs` - Base class analysis
   - `MainViewViewModel.cs` - Primary ViewModel patterns
   - `AdvancedInventoryViewModel.cs` - Complex business logic
   - `AdvancedRemoveViewModel.cs` - Advanced operations

3. **Core Infrastructure**:
   - `Program.cs` - Application setup
   - `App.axaml.cs` - Application lifecycle
   - Interface definitions in `Services/Interfaces/`

#### B. Relationship Mapping Priority
1. Service dependencies and injection patterns
2. ViewModel-to-Service relationships
3. View-to-ViewModel bindings
4. Interface implementations
5. Model usage patterns

## Deliverables

### 1. Interactive Website
- Fully functional HTML/CSS/JS website
- Mobile-responsive design
- MTM branding compliance
- Fast loading and smooth interactions

### 2. Analysis Data
- Complete JSON datasets for all C# files
- Relationship mapping data
- Pattern compliance metrics
- Documentation coverage reports

### 3. Documentation
- Usage guide for the analysis website
- Instructions for updating analysis data
- Integration with development workflow

### 4. Automation Scripts
- PowerShell/C# script for automated analysis
- Integration with build process (optional)
- GitHub Actions workflow for automated updates

## Success Criteria

### Functional Requirements
- [x] Analyze all 41 C# files in the project
- [x] Generate interactive relationship visualizations
- [x] Provide searchable file/class directory
- [x] Display MVVM and ReactiveUI pattern compliance
- [x] Show service dependency mapping
- [x] Mobile-responsive design with MTM branding

### Quality Requirements
- [x] Loading time under 3 seconds
- [x] Works in Chrome, Firefox, Safari, Edge
- [x] Accessible design (WCAG 2.1 AA compliance)
- [x] Clean, maintainable code structure
- [x] Comprehensive error handling

### Business Requirements
- [x] Improves development team productivity
- [x] Aids in code review process
- [x] Supports onboarding new developers
- [x] Identifies technical debt opportunities
- [x] Maintains MTM visual identity

## Implementation Notes

### Dependencies
- No external API dependencies
- Client-side only (HTML/CSS/JS)
- Uses project's existing C# codebase as data source
- Compatible with existing MTM documentation structure

### Security Considerations
- Static website with no sensitive data exposure
- Read-only analysis of source code
- No authentication required
- Can be hosted internally or on GitHub Pages

### Maintenance
- Automated analysis script for updates
- Quarterly review of analysis accuracy
- Integration with development workflow
- Documentation updates as codebase evolves

## Related Issues
- Link to any existing documentation improvement issues
- Reference to MVVM pattern compliance tracking
- Connection to code quality initiatives

## Files to Analyze (41 Total)

### Services (11 files)
- `Services\ConfigurationService.cs`
- `Services\DatabaseService.cs` 
- `Services\LoggingUtility.cs`
- `Services\InventoryService.cs`
- `Services\NavigationService.cs`
- `Services\Service_ErrorHandler.cs`
- `Services\ApplicationStateService.cs`
- `Services\UserAndTransactionServices.cs`
- `Services\Interfaces\ICoreServices.cs`
- `Services\Interfaces\IBusinessServices.cs`
- `Services\Interfaces\INavigationService.cs`

### Extensions (1 file)
- `Extensions\ServiceCollectionExtensions.cs`

### Core Application (3 files)
- `App.axaml.cs`
- `MainWindow.axaml.cs`
- `Program.cs`

### ViewModels (15 files)
- `ViewModels\MainForm\MainViewModel.cs`
- `ViewModels\MainForm\MainViewViewModel.cs`
- `ViewModels\MainForm\MainWindowViewModel.cs`
- `ViewModels\MainForm\InventoryTabViewModel.cs`
- `ViewModels\MainForm\InventoryViewModel.cs`
- `ViewModels\MainForm\QuickButtonsViewModel.cs`
- `ViewModels\MainForm\AddItemViewModel.cs`
- `ViewModels\MainForm\RemoveItemViewModel.cs`
- `ViewModels\MainForm\TransferItemViewModel.cs`
- `ViewModels\MainForm\AdvancedInventoryViewModel.cs`
- `ViewModels\MainForm\AdvancedRemoveViewModel.cs`
- `ViewModels\SettingsForm\UserManagementViewModel.cs`
- `ViewModels\TransactionsForm\TransactionHistoryViewModel.cs`
- `ViewModels\Shared\BaseViewModel.cs`

### Views (7 files)
- `Views\MainForm\MainView.axaml.cs`
- `Views\MainForm\InventoryTabView.axaml.cs`
- `Views\MainForm\QuickButtonsView.axaml.cs`
- `Views\MainForm\RemoveTabView.axaml.cs`
- `Views\MainForm\TransferTabView.axaml.cs`
- `Views\MainForm\AdvancedInventoryView.axaml.cs`
- `Views\MainForm\AdvancedRemoveView.axaml.cs`

### Models (3 files)
- `Models\Shared\CoreModels.cs`
- `Models\Shared\Result.cs`
- `Models\Shared\ResultPattern.cs`

### Examples (1 file)
- `Documentation\Development\Examples\EnhancedErrorHandlingExample.cs`

---

**Issue Created**: {Current Date}
**Estimated Effort**: 16-24 hours
**Target Completion**: 1 week from assignment
**Review Required**: Development team lead approval before implementation