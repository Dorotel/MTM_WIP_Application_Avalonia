# MTM WIP Application - Master Refactor Implementation Plan

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Project**: MTM WIP Application Comprehensive Refactoring  
**Timeline**: 3-Phase Implementation (Reorganization → Overlay System → Integration)  
**Target**: Complete project restructuring and Universal Overlay Service implementation  
**Copilot Optimization**: Project-Task-SubTask structure for optimal AI workflow execution

---

## 🎯 Master Plan Overview

### **Three-Phase Implementation Strategy**

1. **Phase 1: Project Reorganization** (Foundation) - 2-3 days
2. **Phase 2: Universal Overlay System** (Core Features) - 3-4 days  
3. **Phase 3: Integration & Polish** (Completion) - 1-2 days

### **Success Metrics**

- ✅ **Code Organization**: 24 Services → 9 consolidated files
- ✅ **View Structure**: Mirror ViewModels to Views organization
- ✅ **Overlay Coverage**: 40% → 85% of views with overlay support
- ✅ **Developer Experience**: 50% reduction in implementation time
- ✅ **Memory Optimization**: 20% reduction through pooling

---

## 📋 PROJECT 1: PROJECT REORGANIZATION FOUNDATION

### **Project Objective**

Reorganize Services, ViewModels, Views, and WeekendRefactor folders using folder-based organization with proper naming conventions, creating clean foundation for overlay implementation.

### **TASK 1.1: Services Folder-Based Organization**

#### **SubTask 1.1.1: Analyze Service Dependencies**

```bash
# Copilot Action: Service dependency analysis
- Scan all 24 Services/*.cs files
- Map interdependencies using semantic search
- Identify circular references and resolve
- Document service call patterns
```

#### **SubTask 1.1.2: Organize Core Services**

```bash
# Target: Services/Core/ with proper naming
- Rename: Configuration.cs → Core.ConfigurationService.cs  
- Rename: Database.cs → Core.DatabaseService.cs
- Create: Core.ApplicationStateService.cs (if separate)
- Create: Core.ErrorHandling.cs (static class)
- Update: All references to use Services.Core namespace
- Validate: No breaking changes in service registration
```

#### **SubTask 1.1.3: Organize Business Services**

```bash
# Target: Services/Business/ with proper naming
- Rename: MasterDataService.cs → Business.MasterDataService.cs
- Rename: RemoveService.cs → Business.RemoveService.cs
- Rename: InventoryEditingService.cs → Business.InventoryEditingService.cs
- Move: QuickButtons.cs → Business.QuickButtonsService.cs
- Create: Business.ProgressService.cs (if separate from QuickButtons)
- Update: All ViewModel references to use Services.Business namespace
- Validate: All stored procedure calls remain functional
```

#### **SubTask 1.1.4: Organize UI Services**

```bash
# Target: Services/UI/ with proper naming
- Rename: NavigationService.cs → UI.NavigationService.cs
- Rename: ThemeService.cs → UI.ThemeService.cs  
- Rename: FocusManagementService.cs → UI.FocusManagementService.cs
- Rename: SuccessOverlay.cs → UI.SuccessOverlayService.cs
- Rename: SuggestionOverlay.cs → UI.SuggestionOverlayService.cs
- Rename: VirtualPanelManager.cs → UI.VirtualPanelManager.cs
- Rename: SettingsPanelStateManager.cs → UI.SettingsPanelStateManager.cs
- Rename: CustomDataGridService.cs → UI.CustomDataGridService.cs
- Rename: ColumnConfigurationService.cs → UI.ColumnConfigurationService.cs
- Update: All View code-behind references
- Validate: Theme switching and navigation work correctly
```

#### **SubTask 1.1.5: Organize Infrastructure Services**

```bash
# Target: Services/Infrastructure/ with proper naming
- Rename: FileLoggingService.cs → Infrastructure.FileLoggingService.cs
- Rename: MTMFileLoggerProvider.cs → Infrastructure.MTMFileLoggerProvider.cs
- Rename: FilePathService.cs → Infrastructure.FilePathService.cs
- Rename: FileSelection.cs → Infrastructure.FileSelectionService.cs
- Rename: PrintService.cs → Infrastructure.PrintService.cs
- Rename: EmergencyKeyboardHook.cs → Infrastructure.EmergencyKeyboardHookService.cs
- Update: All external file operation references
- Validate: File operations and printing functionality preserved
```

#### **SubTask 1.1.6: Update Service Registration**

```bash
# Target: Extensions/ServiceCollectionExtensions.cs
- Create: AddCoreServices() extension method
- Create: AddBusinessServices() extension method  
- Create: AddUIServices() extension method
- Create: AddInfrastructureServices() extension method
- Create: AddFeatureServices() extension method
- Update: All service registrations to use new namespaces
- Maintain: Same interface contracts and lifetimes
- Add: Integration tests for service resolution
```

### **TASK 1.2: ViewModels Reorganization**

#### **SubTask 1.2.1: Create Application Folder Structure**

```bash
# Target: ViewModels/Application/
- Move: MainWindowViewModel.cs (if exists)
- Move: Application-level ViewModels
- Update: Namespace to MTM_WIP_Application_Avalonia.ViewModels.Application
- Validate: Application startup functionality preserved
```

#### **SubTask 1.2.2: Create MainForm Folder Structure**

```bash
# Target: ViewModels/MainForm/
- Move: InventoryTabViewModel.cs
- Move: RemoveTabViewModel.cs  
- Move: TransferTabViewModel.cs
- Move: AdvancedInventoryViewModel.cs
- Move: AdvancedRemoveViewModel.cs
- Update: All namespaces to ViewModels.MainForm
- Validate: MainView integration remains functional
```

#### **SubTask 1.2.3: Create Overlay Folder Structure**

```bash
# Target: ViewModels/Overlay/
- Move: SuggestionOverlayViewModel.cs
- Move: PrintLayoutControlViewModel.cs (if overlay-related)
- Create: BaseOverlayViewModel.cs for future overlays
- Update: Namespaces to ViewModels.Overlay
- Validate: Existing overlay functionality preserved
```

#### **SubTask 1.2.4: Remove Deprecated ViewModels**

```bash
# Action: Safe removal of deprecated code
- Remove: SuggestionOverlayViewModel_duplicate.cs
- Remove: TransactionHistoryViewModel_fixed.cs  
- Remove: Any ViewModels marked as deprecated
- Update: Remove references from service registrations
- Validate: No broken references remain in Views or Services
```

#### **SubTask 1.2.5: Update ViewModel Service Registration**

```bash
# Target: Extensions/ServiceCollectionExtensions.cs
- Update all ViewModel registrations with new namespaces
- Maintain transient lifetime for all ViewModels
- Add ViewModels for new folder structure
- Validate: Dependency injection resolution works correctly
```

### **TASK 1.3: Views Reorganization**

#### **SubTask 1.3.1: Create MainForm Views Structure**

```bash
# Target: Views/MainForm/
- Move: InventoryTabView.axaml + .cs
- Move: RemoveTabView.axaml + .cs
- Move: TransferTabView.axaml + .cs  
- Move: AdvancedInventoryView.axaml + .cs
- Move: AdvancedRemoveView.axaml + .cs
- Update: All x:Class namespaces in AXAML files
- Validate: Views compile without AVLN2000 errors
```

#### **SubTask 1.3.2: Create Settings Views Structure**

```bash
# Target: Views/Settings/
- Move: SettingsForm.axaml + .cs (and related)
- Move: Any settings-related views
- Update: x:Class namespaces for Views.Settings
- Update: Any NavigationService references
- Validate: Settings functionality preserved
```

#### **SubTask 1.3.3: Create Print Views Structure**

```bash
# Target: Views/Print/
- Move: Print-related views
- Update: x:Class namespaces for Views.Print
- Update: PrintService integration references
- Validate: Print preview and printing functionality works
```

#### **SubTask 1.3.4: Create Overlay Views Structure**

```bash
# Target: Views/Overlay/
- Create: Base overlay view structure for future implementation
- Move: Any existing overlay views
- Update: x:Class namespaces for Views.Overlay
- Prepare: Integration points for Universal Overlay Service
```

#### **SubTask 1.3.5: Update Navigation Service**

```bash
# Target: Services/Navigation.cs (before consolidation)
- Update all view navigation paths to use new folder structure
- Update ViewModel resolution to use new namespaces
- Test all navigation flows work correctly
- Validate: No broken navigation routes exist
```

### **TASK 1.4: Models Folder Organization**

#### **SubTask 1.4.1: Analyze Model Dependencies**

```bash
# Complete: Models/MODEL_DEPENDENCY_ANALYSIS.md already created
- Review: 21 model files across 6 functional categories
- Confirm: {Folder}.{Model}.cs naming pattern implementation
- Validate: Dependency analysis matches Services folder organization
```

#### **SubTask 1.4.2: Create Core Models Folder Structure**

```bash
# Target: Models/Core/ 
- Create: Models/Core/Core.AppVariables.cs (from Model_AppVariables.cs)
- Create: Models/Core/Core.EditInventoryModel.cs (from EditInventoryModel.cs)
- Create: Models/Core/Core.EditInventoryResult.cs (from EditInventoryResult.cs)
- Create: Models/Core/Core.SessionTransaction.cs (from SessionTransaction.cs)
- Update: All namespace references to MTM_WIP_Application_Avalonia.Models.Core
- Validate: No compilation errors after refactoring
```

#### **SubTask 1.4.3: Create Events Models Folder Structure**

```bash
# Target: Models/Events/
- Create: Models/Events/Events.EventArgs.cs (from EventArgs.cs)
- Create: Models/Events/Events.FocusManagementEventArgs.cs (from FocusManagementEventArgs.cs)
- Create: Models/Events/Events.InventoryEventArgs.cs (from InventoryEventArgs.cs)
- Create: Models/Events/Events.InventorySavedEventArgs.cs (from InventorySavedEventArgs.cs)
- Update: All event handler references to use new namespace
- Validate: Event handling functionality preserved
```

#### **SubTask 1.4.4: Create UI Models Folder Structure**

```bash
# Target: Models/UI/
- Move: Models/CustomDataGrid/* to Models/UI/UI.CustomDataGrid/
- Rename: Files to UI.CustomDataGrid.{OriginalName}.cs pattern
- Update: Namespaces to MTM_WIP_Application_Avalonia.Models.UI
- Validate: UI component bindings work correctly
```

#### **SubTask 1.4.5: Create Overlay and Print Models Structure**

```bash
# Target: Models/Overlay/ and Models/Print/
- Move: Models/Overlay/* to Models/Overlay/Overlay.{Model}.cs
- Create: Models/Print/Print.PrintModel.cs (from PrintModel.cs)
- Create: Models/Print/Print.PrintTemplateModel.cs (from PrintTemplateModel.cs)
- Update: All print and overlay references
- Validate: Print functionality and overlay systems work
```

#### **SubTask 1.4.6: Create Shared Models and Clean Root**

```bash
# Target: Models/Shared/ and root cleanup
- Move: Models/Shared/* to Models/Shared/Shared.{Model}.cs pattern
- Create: Models/Shared/Shared.ViewModels.cs (from ViewModels.cs)
- Remove: Original files from Models root after verification
- Update: All using statements and dependency injection registrations
- Validate: Application compiles and runs without errors
```

### **TASK 1.5: WeekendRefactor Organization**

#### **SubTask 1.5.1: Create Numbered Folder Structure**

```bash
# Target: WeekendRefactor/01-Analysis/
- Move: OverlayAnalysis/* to 01-Analysis/Overlay/
- Create: 01-Analysis/README.md navigation document
- Update: All internal cross-references between analysis documents
- Validate: All markdown links work correctly
```

#### **SubTask 1.5.2: Reorganize Implementation Documents**

```bash
# Target: WeekendRefactor/02-Reorganization/
- Move: Reorganization/* to 02-Reorganization/
- Create: 02-Reorganization/README.md with implementation status
- Update: Cross-references to analysis documents
- Validate: All reorganization plans reference correct paths
```

#### **SubTask 1.5.3: Structure Implementation Guides**

```bash
# Target: WeekendRefactor/03-Implementation/
- Move: Implementation/* to 03-Implementation/
- Organize: Stage folders into numbered subdirectories
- Create: 03-Implementation/README.md master guide
- Update: All stage references and cross-links
- Validate: Implementation guides reference correct reorganized structure
```

#### **SubTask 1.5.4: Create Status Tracking**

```bash
# Target: WeekendRefactor/04-Status/
- Create: 04-Status/Progress-Tracking.md
- Create: 04-Status/Implementation-Status.md (updated version)
- Create: 04-Status/README.md status overview
- Integrate: Progress tracking with master plan structure
```

---

## 📋 PROJECT 2: UNIVERSAL OVERLAY SYSTEM

### **Project Objective**

Implement Universal Overlay Service, create missing overlay implementations, and integrate overlays into all major views with consistent patterns.

### **TASK 2.1: Universal Overlay Service Foundation**

#### **SubTask 2.1.1: Design IUniversalOverlayService Interface**

```csharp
# Target: Services/Interfaces/IUniversalOverlayService.cs
- Define: ShowOverlayAsync<T>(T request) generic method
- Define: HideOverlayAsync(string overlayId) method
- Define: RegisterOverlay<TViewModel, TView>() method
- Define: OverlayResult<T> return types for responses
- Include: Overlay lifecycle events (Showing, Shown, Hiding, Hidden)
```

#### **SubTask 2.1.2: Implement Universal Overlay Service**

```csharp
# Target: Services/UI/UniversalOverlayService.cs (after UI consolidation)
- Implement: IUniversalOverlayService interface
- Create: Overlay ViewModel pooling system for memory efficiency
- Create: Z-index management for overlay layering
- Create: Theme integration with MTM color system
- Implement: Async/await patterns for all operations
```

#### **SubTask 2.1.3: Create Base Overlay Components**

```csharp
# Target: ViewModels/Overlay/BaseOverlayViewModel.cs
- Create: [ObservableObject] base class with common properties
- Include: IsVisible, Title, CanClose, IsModal properties
- Include: [RelayCommand] for Close, Confirm, Cancel operations
- Include: Overlay lifecycle virtual methods for inheritance
```

```xml
# Target: Views/Overlay/BaseOverlayView.axaml
- Create: Base overlay AXAML with MTM styling
- Include: Border with proper CornerRadius and shadows
- Include: Title bar with close button
- Include: ContentPresenter for overlay-specific content
- Include: Button panel for confirm/cancel actions
```

#### **SubTask 2.1.4: Integrate Service Registration**

```csharp
# Target: Extensions/ServiceCollectionExtensions.cs
- Register: IUniversalOverlayService as singleton
- Register: All overlay ViewModels as transient
- Register: Overlay view mappings for dependency injection
- Validate: Service resolution works in application startup
```

#### **SubTask 2.1.5: Create Overlay Development Templates**

```csharp
# Target: WeekendRefactor/03-Implementation/Templates/
- Create: OverlayViewModel template with MVVM Community Toolkit
- Create: OverlayView AXAML template with MTM styling
- Create: Overlay integration code template for Views
- Create: Unit test template for overlay ViewModels
```

### **TASK 2.2: Critical Safety Overlays**

#### **SubTask 2.2.1: Implement Confirmation Overlay**

```csharp
# Target: ViewModels/Overlay/ConfirmationOverlayViewModel.cs
- Create: [ObservableProperty] for Message, Title, ConfirmText, CancelText
- Create: [RelayCommand] for ConfirmAsync, CancelAsync
- Include: Severity levels (Info, Warning, Error, Critical)
- Include: Icon binding based on severity
```

```xml
# Target: Views/Overlay/ConfirmationOverlayView.axaml
- Create: AXAML with message display and action buttons
- Include: Icon display based on severity
- Include: MTM button styling with severity-based colors
- Include: Keyboard shortcuts (Enter=Confirm, Escape=Cancel)
```

#### **SubTask 2.2.2: Implement Batch Confirmation Overlay**

```csharp
# Target: ViewModels/Overlay/BatchConfirmationOverlayViewModel.cs
- Create: Properties for batch operation details (count, type, target)
- Create: [ObservableCollection<string>] for affected items list
- Include: Progress indicator for batch operations
- Include: Option to cancel mid-operation
```

#### **SubTask 2.2.3: Implement Field Validation Overlay**

```csharp
# Target: ViewModels/Overlay/ValidationOverlayViewModel.cs
- Create: [ObservableCollection<ValidationError>] for validation results
- Include: Field highlighting and error message display
- Include: Auto-hide after successful validation
- Include: Focus management to invalid fields
```

#### **SubTask 2.2.4: Implement Progress Overlay**

```csharp
# Target: ViewModels/Overlay/ProgressOverlayViewModel.cs
- Create: Properties for progress percentage, current task, cancel support
- Include: Indeterminate progress mode for unknown duration
- Include: Task cancellation token integration
- Include: Time estimation and completion callbacks
```

### **TASK 2.3: MainWindow Integration Overlays**

#### **SubTask 2.3.1: Implement Connection Status Overlay**

```csharp
# Target: ViewModels/Overlay/ConnectionStatusOverlayViewModel.cs
- Create: Properties for database connection status, last check time
- Include: Auto-retry logic with exponential backoff
- Include: Manual reconnection command
- Include: Connection health indicators (latency, query performance)
```

#### **SubTask 2.3.2: Implement Emergency Shutdown Overlay**

```csharp
# Target: ViewModels/Overlay/EmergencyShutdownOverlayViewModel.cs
- Create: Emergency shutdown confirmation with pending work checks
- Include: Unsaved data warning and save options
- Include: Force shutdown option with confirmation
- Include: Session state preservation for recovery
```

#### **SubTask 2.3.3: Implement Theme Quick Switcher Overlay**

```csharp
# Target: ViewModels/Overlay/ThemeQuickSwitcherOverlayViewModel.cs
- Create: Available themes list with preview
- Include: Theme preview functionality
- Include: Theme persistence through SettingsService
- Include: Live theme switching with smooth transitions
```

#### **SubTask 2.3.4: Integrate MainWindow Overlays**

```csharp
# Target: Views/MainWindow.axaml updates
- Add: Overlay container Grid for Universal Overlay Service
- Add: Keyboard shortcuts for overlay activation
- Update: MainWindow.axaml.cs to register overlay handlers
- Validate: Overlays display correctly over main content
```

### **TASK 2.4: View-Specific Overlay Integration**

#### **SubTask 2.4.1: InventoryTabView Overlay Integration**

```csharp
# Integration Points:
- Add confirmation overlays for inventory additions/removals
- Add validation overlay for Part ID and quantity fields
- Add success overlay for completed operations
- Update InventoryTabViewModel to use Universal Overlay Service
```

#### **SubTask 2.4.2: RemoveTabView Overlay Integration**

```csharp
# Integration Points:
- Add confirmation overlay for inventory removal operations
- Add batch confirmation for multiple item removal
- Add validation overlay for remove operation requirements
- Update RemoveTabViewModel to use Universal Overlay Service
```

#### **SubTask 2.4.3: TransferTabView Overlay Integration**

```csharp
# Integration Points:
- Add confirmation overlay for transfer operations
- Add validation overlay for source/destination locations
- Add progress overlay for transfer processing
- Update TransferTabViewModel to use Universal Overlay Service
```

#### **SubTask 2.4.4: AdvancedInventoryView Overlay Integration**

```csharp
# Integration Points:
- Add batch confirmation for mass inventory operations
- Add field validation overlay for bulk data entry
- Add progress overlay for large data processing
- Update AdvancedInventoryViewModel to use Universal Overlay Service
```

#### **SubTask 2.4.5: AdvancedRemoveView Overlay Integration**

```csharp
# Integration Points:
- Add critical confirmation for mass deletion operations
- Add safety warning overlays for destructive operations
- Add progress overlay with cancellation support
- Update AdvancedRemoveViewModel to use Universal Overlay Service
```

---

## 📋 PROJECT 3: INTEGRATION & POLISH

### **Project Objective**

Complete integration testing, performance optimization, comprehensive documentation, and final validation of the entire refactored system.

### **TASK 3.1: Performance Optimization**

#### **SubTask 3.1.1: Implement Overlay Pooling**

```csharp
# Target: Services/UI/OverlayPoolingService.cs
- Create: Overlay ViewModel pooling system to reduce allocations
- Implement: Pool sizing based on usage patterns
- Include: Pool warming on application startup
- Include: Pool monitoring and statistics for debugging
```

#### **SubTask 3.1.2: Optimize Memory Usage**

```csharp
# Analysis and optimization:
- Profile: Memory usage before and after refactoring
- Optimize: Service consolidation memory footprint
- Optimize: Overlay lifecycle management
- Validate: 20% memory reduction target achieved
```

#### **SubTask 3.1.3: Theme Integration Performance**

```csharp
# Target: Services/UI/ThemeService.cs updates
- Optimize: Theme switching performance with overlay system
- Implement: Theme resource caching for overlays
- Include: Smooth transitions between themes
- Validate: No UI freezing during theme changes
```

### **TASK 3.2: Comprehensive Testing**

#### **SubTask 3.2.1: Unit Testing Framework**

```csharp
# Target: Tests/Unit/ViewModels/Overlay/
- Create: Unit tests for all overlay ViewModels
- Create: Mock implementations for Universal Overlay Service
- Test: MVVM Community Toolkit property and command patterns
- Test: Error handling and validation logic
```

#### **SubTask 3.2.2: Integration Testing**

```csharp
# Target: Tests/Integration/Services/
- Create: Integration tests for consolidated services
- Test: Service dependency resolution after reorganization
- Test: Database operations through reorganized services
- Test: Overlay service integration with views
```

#### **SubTask 3.2.3: UI Automation Testing**

```csharp
# Target: Tests/UI/Overlay/
- Create: UI tests for overlay display and interaction
- Test: Keyboard navigation and accessibility
- Test: Theme integration with overlay system
- Test: Cross-platform overlay behavior (if applicable)
```

#### **SubTask 3.2.4: Manual Testing Checklist**

```markdown
# Target: WeekendRefactor/04-Status/Testing-Checklist.md
- Create: Comprehensive manual testing checklist
- Include: All overlay scenarios and edge cases
- Include: Navigation and service functionality validation
- Include: Performance and memory usage verification
```

### **TASK 3.3: Documentation Completion**

#### **SubTask 3.3.1: Update Architecture Documentation**

```markdown
# Target: docs/architecture/ updates
- Update: project-blueprint.md with new service organization
- Update: folder-structure.md with reorganized structure
- Add: Universal Overlay Service architecture documentation
- Include: Updated dependency diagrams and service flow
```

#### **SubTask 3.3.2: Update GitHub Instructions**

```markdown
# Target: .github/instructions/ updates
- Update: service-architecture.instructions.md with consolidated patterns
- Add: overlay-development.instructions.md for future overlay creation
- Update: avalonia-ui-guidelines.instructions.md with overlay patterns
- Update: copilot-instructions.md main file with overlay guidance
```

#### **SubTask 3.3.3: Create Developer Onboarding Guide**

```markdown
# Target: docs/development/MTM-Refactored-System-Guide.md
- Create: Complete guide for developers working with refactored system
- Include: Service location guide after consolidation
- Include: Overlay development workflow and templates
- Include: Troubleshooting guide for common issues
```

#### **SubTask 3.3.4: Update WeekendRefactor Documentation**

```markdown
# Target: WeekendRefactor/04-Status/ completion
- Update: Implementation-Status.md with final status
- Create: Lessons-Learned.md documentation
- Create: Future-Enhancements.md roadmap
- Update: Progress-Tracking.md to reflect completion
```

### **TASK 3.4: Final Validation & Deployment**

#### **SubTask 3.4.1: Complete System Validation**

```bash
# Validation checklist:
- Compile: Entire solution without errors or warnings
- Test: All automated tests passing
- Verify: All overlay functionality working correctly
- Verify: All reorganized services functioning properly
- Verify: No broken references or missing dependencies
```

#### **SubTask 3.4.2: Performance Benchmark Validation**

```bash
# Performance verification:
- Measure: Application startup time after refactoring
- Measure: Memory usage during normal operation
- Measure: Overlay display performance
- Compare: Before/after metrics to validate improvements
```

#### **SubTask 3.4.3: Cross-Platform Testing**

```bash
# If applicable:
- Test: Windows (primary platform)
- Test: Linux (if supported)
- Test: macOS (if supported)
- Verify: Avalonia UI behavior consistency across platforms
```

#### **SubTask 3.4.4: Production Readiness Check**

```bash
# Final deployment preparation:
- Review: All configuration files updated
- Review: Database connection strings and stored procedures
- Review: Logging configuration and error handling
- Create: Deployment checklist and rollback procedures
```

---

## 📊 Implementation Tracking Template

### **Project Progress Overview**

```text
Phase 1: Project Reorganization - 0% (0/20 SubTasks)
├── Task 1.1: Services Organization (Folder-based) - 0% (0/6 SubTasks)
├── Task 1.2: ViewModels Reorganization - 0% (0/5 SubTasks)
├── Task 1.3: Views Reorganization - 0% (0/5 SubTasks)
├── Task 1.4: Models Folder Organization - 0% (0/6 SubTasks)
└── Task 1.5: WeekendRefactor Organization - 0% (0/4 SubTasks)

Phase 2: Universal Overlay System - 0% (0/17 SubTasks)  
├── Task 2.1: Universal Service Foundation - 0% (0/5 SubTasks)
├── Task 2.2: Critical Safety Overlays - 0% (0/4 SubTasks)
├── Task 2.3: MainWindow Integration - 0% (0/4 SubTasks)
└── Task 2.4: View-Specific Integration - 0% (0/5 SubTasks)

Phase 3: Integration & Polish - 0% (0/14 SubTasks)
├── Task 3.1: Performance Optimization - 0% (0/3 SubTasks)
├── Task 3.2: Comprehensive Testing - 0% (0/4 SubTasks)
├── Task 3.3: Documentation Completion - 0% (0/4 SubTasks)
└── Task 3.4: Final Validation - 0% (0/4 SubTasks)

Total Progress: 0% (0/51 SubTasks)
```

### **Copilot Execution Notes**

- Each SubTask is designed for single Copilot execution session
- All file paths are absolute for easy navigation
- Code snippets provided as implementation starting points
- Cross-references maintain consistency during refactoring
- Validation steps ensure no functionality is lost

---

## 🎯 Quick Reference for Copilot

### **SubTask Execution Pattern**

1. **Read SubTask description and code template**
2. **Use semantic_search for context gathering**
3. **Execute file operations (move, create, update)**
4. **Update references using replace_string_in_file**
5. **Validate changes using get_errors or compile checks**
6. **Move to next SubTask in sequence**

### **Critical Dependencies**

- **MVVM Community Toolkit patterns** must be maintained
- **MTM Design System** colors and styling must be preserved  
- **Stored procedures database pattern** must remain unchanged
- **Service registration lifetimes** must be maintained
- **Avalonia AXAML syntax** must follow established patterns

### **Validation Commands**

```bash
# After each SubTask, validate:
dotnet build --no-restore  # Compile check
dotnet test --no-restore   # Run tests (if available)
```

---

**This master plan provides comprehensive, Copilot-optimized guidance for completing the entire MTM WIP Application refactoring project with clear task boundaries and validation steps.**
