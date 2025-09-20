# MTM WIP Application Avalonia - Complete Implementation Prompts

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Project**: MTM WIP Application Comprehensive Refactoring & Implementation  
**Created**: September 19, 2025  
**Purpose**: Complete collection of implementation prompts for GitHub Copilot AI assistance  
**Scope**: All phases from project reorganization through overlay system to final polish  

---

## 🎯 How to Use This Prompt Collection

### **For GitHub Copilot AI Assistant:**

1. **Reference this file** by using `#file:WeekendRefactor/MTM-Implementation-Prompts.md` in your chat
2. **Pick specific prompts** based on your current implementation phase
3. **Follow the established MTM patterns** as defined in copilot-instructions.md
4. **Execute prompts sequentially** within each phase for best results
5. **Validate each step** using the provided verification commands

### **For Human Developers:**

1. Use prompts as **task descriptions** and implementation guidance
2. **Copy specific prompt sections** to GitHub Copilot for AI assistance  
3. **Reference related documentation** links provided in each prompt
4. **Track progress** using the embedded progress tracking templates

---

## 📋 PHASE 1: PROJECT REORGANIZATION FOUNDATION

### **Prompt 1.1: Services Consolidation Analysis**

```
Analyze the Services folder in this MTM WIP Application and help me consolidate 24 services into 9 organized files following the .NET architecture best practices.

Current Services (24 files):
- ColumnConfigurationService.cs, Configuration.cs, CustomDataGridService.cs
- Database.cs, EmergencyKeyboardHook.cs, ErrorHandling.cs
- FileLoggingService.cs, FilePathService.cs, FileSelection.cs
- FocusManagementService.cs, InventoryEditingService.cs, MasterDataService.cs
- MTMFileLoggerProvider.cs, Navigation.cs, PrintService.cs
- QuickButtons.cs, RemoveService.cs, SettingsPanelStateManager.cs
- SettingsService.cs, StartupDialog.cs, SuccessOverlay.cs
- SuggestionOverlay.cs, ThemeService.cs, VirtualPanelManager.cs

Target Organization (9 consolidated files):
1. Services/Core/CoreServices.cs (Configuration, Database, ErrorHandling)
2. Services/Business/BusinessServices.cs (MasterData, InventoryEditing, Remove)
3. Services/UI/UIServices.cs (Navigation, Theme, FocusManagement, SuccessOverlay, SuggestionOverlay)
4. Services/Infrastructure/InfrastructureServices.cs (FileLogging, FilePath, FileSelection, Print)
5. Services/Data/DataServices.cs (CustomDataGrid, ColumnConfiguration)
6. Services/Workflow/WorkflowServices.cs (QuickButtons, VirtualPanelManager)
7. Services/System/SystemServices.cs (EmergencyKeyboardHook, StartupDialog)
8. Services/Settings/SettingsServices.cs (Settings, SettingsPanelStateManager)
9. Services/Logging/LoggingServices.cs (MTMFileLoggerProvider, FileLoggingService)

Requirements:
- Maintain all existing interfaces and public APIs
- Update Extensions/ServiceCollectionExtensions.cs with new registrations
- Keep service lifetimes (singleton, scoped, transient) unchanged
- Follow MVVM Community Toolkit dependency patterns
- Use MySQL stored procedure patterns for data operations
- Maintain MTM error handling with Services.ErrorHandling.HandleErrorAsync()

Please analyze dependencies between these services and provide a step-by-step consolidation plan that prevents breaking changes.
```

### **Prompt 1.2: ViewModels Reorganization**

```
Help me reorganize the ViewModels folder structure in this MTM WIP Application to mirror a clean architecture pattern.

Current ViewModels (scattered organization):
- FilterPanelViewModel.cs.disabled
- PrintLayoutControlViewModel.cs, PrintViewModel.cs
- ThemeEditorViewModel.cs
- Multiple other ViewModels in root folder

Target Structure:
ViewModels/
├── Application/
│   ├── MainWindowViewModel.cs
│   └── ApplicationStateViewModel.cs
├── MainForm/
│   ├── InventoryTabViewModel.cs
│   ├── RemoveTabViewModel.cs
│   ├── TransferTabViewModel.cs
│   ├── AdvancedInventoryViewModel.cs
│   └── AdvancedRemoveViewModel.cs
├── Settings/
│   ├── SettingsFormViewModel.cs
│   ├── ThemeEditorViewModel.cs
│   └── ConfigurationViewModel.cs
├── Print/
│   ├── PrintViewModel.cs
│   └── PrintLayoutControlViewModel.cs
├── Overlay/
│   ├── BaseOverlayViewModel.cs
│   ├── ConfirmationOverlayViewModel.cs
│   ├── SuccessOverlayViewModel.cs
│   ├── SuggestionOverlayViewModel.cs
│   └── EditInventoryViewModel.cs
└── Shared/
    ├── BaseViewModel.cs
    └── BasePoolableOverlayViewModel.cs

Requirements:
- Use MVVM Community Toolkit with [ObservableObject], [ObservableProperty], [RelayCommand]
- Update all namespace declarations to match new folder structure
- Update Extensions/ServiceCollectionExtensions.cs with correct registrations
- Remove deprecated/disabled ViewModels safely
- Maintain all existing ViewModel constructor injection patterns
- Ensure all Views can resolve their ViewModels via DI
- Follow MTM patterns for async commands and error handling

Please provide the file move operations and namespace updates needed.
```

### **Prompt 1.3: Views Reorganization and AXAML Validation**

```
Help me reorganize the Views folder to match the ViewModels structure and ensure all AXAML files follow proper Avalonia syntax to prevent AVLN2000 compilation errors.

Current Issues:
- Views scattered in root folder without clear organization
- Need to verify all AXAML uses correct Avalonia namespace
- Ensure no WPF syntax is accidentally used
- Grid definitions must use x:Name not Name property
- All Views must follow MTM design system patterns

Target Views Structure:
Views/
├── Application/
│   └── MainWindow.axaml/.cs
├── MainForm/
│   ├── InventoryTabView.axaml/.cs
│   ├── RemoveTabView.axaml/.cs
│   ├── TransferTabView.axaml/.cs
│   ├── AdvancedInventoryView.axaml/.cs
│   └── AdvancedRemoveView.axaml/.cs
├── Settings/
│   ├── SettingsForm.axaml/.cs
│   └── ThemeEditorView.axaml/.cs
├── Print/
│   └── PrintView.axaml/.cs
└── Overlay/
    ├── ConfirmationOverlayView.axaml/.cs
    ├── SuccessOverlayView.axaml/.cs
    ├── SuggestionOverlayView.axaml/.cs
    └── EditInventoryView.axaml/.cs

Critical AXAML Requirements:
- xmlns="https://github.com/avaloniaui" (NOT WPF namespace)
- Use x:Name for Grid definitions (NEVER Name property)
- TextBlock instead of Label
- Use ColumnDefinitions="Auto,*" attribute form
- All MTM theme resources via DynamicResource
- Follow InventoryTabView grid pattern for all tab views

Please scan existing Views, identify AXAML syntax issues, and provide corrected file organization.
```

### **Prompt 1.4: Universal Overlay Service Foundation**

```
Help me create the Universal Overlay Service for this MTM WIP Application that will manage all overlay types in a consistent, performant manner.

Current Overlay System Issues:
- Multiple separate overlay services (ISuggestionOverlayService, ISuccessOverlayService)  
- Inconsistent overlay registration patterns
- No overlay pooling or reuse mechanism
- Mixed window dialogs vs embedded overlays
- No centralized overlay management

Required Universal Service Architecture:

1. IUniversalOverlayService Interface:
```csharp
public interface IUniversalOverlayService
{
    Task<TResponse> ShowOverlayAsync<TRequest, TResponse, TViewModel>(TRequest request) 
        where TRequest : BaseOverlayRequest
        where TResponse : BaseOverlayResponse  
        where TViewModel : BaseOverlayViewModel;
        
    Task HideOverlayAsync(string overlayId);
    Task<bool> IsOverlayVisibleAsync(string overlayId);
    void RegisterOverlay<TViewModel, TView>() 
        where TViewModel : BaseOverlayViewModel
        where TView : UserControl;
}
```

2. Base Classes:

- BaseOverlayRequest (with overlayId, priority, modal settings)
- BaseOverlayResponse (with result, validation, timing info)
- BaseOverlayViewModel (with common properties, lifecycle methods)
- BasePoolableOverlayViewModel (with Reset() for pooling)

3. Implementation Features:

- Overlay pooling system for performance
- Z-index management for layering
- Parent container detection and management
- Theme integration with MTM color system
- Memory management and disposal
- Async/await throughout with proper error handling

Requirements:

- Follow MVVM Community Toolkit patterns
- Integrate with existing ConfirmationOverlay, SuccessOverlay, SuggestionOverlay
- Use MTM error handling patterns
- Support dependency injection for overlay ViewModels
- Follow MTM service registration patterns

Please create the Universal Overlay Service implementation with proper MTM integration.

```

---

## 📋 PHASE 2: OVERLAY SYSTEM IMPLEMENTATION

### **Prompt 2.1: Critical Safety Overlays**

```

Help me implement critical safety overlays for this MTM manufacturing application to prevent destructive operations without proper confirmation.

Current Safety Gaps:

- AdvancedRemoveView allows mass deletion without confirmation
- AdvancedInventoryView batch operations lack safety checks
- No global error overlay for application-level errors
- No progress feedback for long-running operations

Required Safety Overlays:

1. BatchConfirmationOverlay:

- Show affected items count and details
- Require explicit confirmation for >10 items
- Support operation cancellation mid-process
- Display estimated time and progress

2. DestructiveOperationConfirmation:

- Special high-visibility confirmation for deletions
- Require typing "DELETE" to confirm mass operations
- Show undo information if available
- Log all confirmations for audit

3. GlobalErrorOverlay:

- Application-level error display
- Connection failure notifications  
- Critical system error handling
- Recovery action suggestions

4. ProgressOverlay:

- Long-running database operations
- Batch import/export progress
- Cancellation support with proper cleanup

Implementation Requirements:

- Use Universal Overlay Service
- Follow MVVM Community Toolkit patterns
- Integrate with MTM error handling system
- Support MTM theme system (Windows 11 Blue #0078D4)
- Include comprehensive logging
- Use stored procedures for all database operations

Create these safety overlays with full ViewModel, View AXAML, and integration code.

```

### **Prompt 2.2: Missing View Integration Overlays**

```

Based on the comprehensive overlay analysis, help me add missing overlay integrations to Views that currently lack proper user feedback and confirmation systems.

Views Needing Overlay Integration:

1. InventoryTabView (40% coverage - missing confirmations):

- Add confirmation for inventory additions >100 units
- Add validation overlay for Part ID/Operation combinations  
- Add success feedback for completed operations
- Integrate with existing SuggestionOverlay

2. TransferTabView (50% coverage - missing confirmations):

- Add confirmation for transfer operations
- Add validation overlay for source/destination locations
- Add progress overlay for transfer processing
- Show transfer history overlay

3. NewQuickButtonView (25% coverage - missing feedback):

- Add success overlay for quick button creation
- Add validation overlay for button configuration
- Add confirmation for quick button deletion

4. AdvancedInventoryView (0% coverage - no overlays):

- Add batch operation confirmation overlay
- Add field validation overlay for bulk data entry
- Add progress overlay for large dataset processing
- Add error overlay for batch operation failures

5. AdvancedRemoveView (0% coverage - critical missing):

- Add critical confirmation for mass deletion operations  
- Add safety warning overlays for destructive operations
- Add progress overlay with cancellation support
- Add undo notification overlay

Integration Requirements:

- Use Universal Overlay Service for all overlays
- Follow established MTM overlay patterns from existing implementations
- Maintain MVVM Community Toolkit patterns in ViewModels
- Use MTM design system colors and styling
- Integrate with existing Services (ErrorHandling, SuccessOverlay, etc.)
- Add proper keyboard navigation support
- Include comprehensive error handling

Please provide the overlay integration code for each View with proper ViewModel updates.

```

### **Prompt 2.3: Advanced Overlay Features**

```

Help me implement advanced overlay features for the MTM WIP Application including overlay stacking, animations, and performance optimizations.

Required Advanced Features:

1. Overlay Stacking System:

- Z-index management for multiple overlays
- Modal vs non-modal overlay handling
- Overlay priority system (Critical > Warning > Info)
- Stack overflow prevention (max 3 overlays)

2. Overlay Animations:

- Fade-in/fade-out with 300ms duration
- Slide-in from appropriate edges
- Scale animations for confirmation overlays
- Smooth transitions between overlay states

3. Performance Optimizations:

- Overlay ViewModel pooling system
- Resource preloading for frequently used overlays
- Memory management with proper disposal
- Background loading for complex overlays

4. Enhanced User Experience:

- Auto-positioning to avoid covering important content
- Smart focus management and restoration
- Keyboard navigation improvement (Tab, Arrow keys)
- Screen reader accessibility support

5. Developer Experience:

- Overlay performance monitoring
- Development-time overlay debugging
- Visual overlay hierarchy inspector
- Performance metrics collection

Implementation Details:

```csharp
// Overlay Stacking Manager
public interface IOverlayStackManager
{
    Task<bool> CanShowOverlayAsync(OverlayPriority priority);
    Task PushOverlayAsync(BaseOverlayViewModel overlay);
    Task<BaseOverlayViewModel?> PopOverlayAsync();
    IEnumerable<BaseOverlayViewModel> GetCurrentStack();
}

// Overlay Animation System  
public interface IOverlayAnimationService
{
    Task ShowWithAnimationAsync(UserControl view, OverlayAnimationType type);
    Task HideWithAnimationAsync(UserControl view, OverlayAnimationType type);  
    Task TransitionAsync(UserControl fromView, UserControl toView);
}

// Overlay Pool Manager
public interface IOverlayPoolManager
{
    Task<TViewModel> RentAsync<TViewModel>() where TViewModel : BasePoolableOverlayViewModel;
    Task ReturnAsync<TViewModel>(TViewModel viewModel) where TViewModel : BasePoolableOverlayViewModel;
    PoolStatistics GetStatistics();
}
```

Requirements:

- Maintain compatibility with existing overlays
- Follow MTM performance standards (20% memory reduction target)
- Use MVVM Community Toolkit patterns
- Integrate with MTM theme system
- Proper error handling and logging
- Unit tests for all new functionality

Create the advanced overlay system with full implementation.

```

---

## 📋 PHASE 3: INTEGRATION & TESTING

### **Prompt 3.1: Comprehensive Testing Framework**

```

Help me create a comprehensive testing framework for the MTM WIP Application's overlay system and reorganized architecture.

Required Test Coverage:

1. Unit Tests (ViewModels):

- All overlay ViewModels with MVVM Community Toolkit patterns
- Service consolidation validation
- Database operation testing with mock stored procedures  
- Error handling and validation logic

2. Integration Tests (Services):

- Universal Overlay Service integration
- Service dependency resolution after consolidation
- Database connection and stored procedure execution
- Theme system integration with overlays

3. UI Automation Tests (Views):

- Overlay display and interaction testing
- Keyboard navigation validation
- Theme switching with overlay preservation
- Cross-platform behavior (Windows primary, Linux/macOS if supported)

4. Performance Tests:

- Overlay pooling efficiency measurement
- Memory usage validation (20% reduction target)
- Application startup time after reorganization
- Database operation performance with new service organization

Test Framework Structure:

```
Tests/
├── Unit/
│   ├── ViewModels/
│   │   ├── Overlay/
│   │   ├── MainForm/
│   │   └── Settings/
│   └── Services/
│       ├── Core/
│       ├── Business/
│       └── UI/
├── Integration/
│   ├── Services/
│   ├── Database/
│   └── Navigation/
├── UI/
│   ├── Overlay/
│   ├── MainForm/
│   └── Navigation/
└── Performance/
    ├── Memory/
    ├── Startup/
    └── Database/
```

Testing Requirements:

- Use MSTest framework (existing pattern)
- FluentAssertions for readable assertions
- Moq for service mocking
- In-memory database for data tests
- Avalonia UI testing framework for view tests
- Performance counters for metrics validation

Example Test Patterns:

```csharp
[TestMethod]
public async Task ShowOverlayAsync_WithValidRequest_ShouldDisplayOverlay()
{
    // Arrange
    var mockService = new Mock<IUniversalOverlayService>();
    var request = new InventoryQuickEditRequest(/*...*/);
    
    // Act
    var result = await mockService.Object.ShowOverlayAsync</*...*/>(request);
    
    // Assert
    result.Should().NotBeNull();
    result.Result.Should().Be(OverlayResult.Confirmed);
}
```

Please create the comprehensive testing framework with examples for all test types.

```

### **Prompt 3.2: Performance Optimization and Monitoring**

```

Help me implement performance optimization and monitoring for the refactored MTM WIP Application, focusing on the 20% memory reduction target and improved user experience.

Current Performance Baseline Needed:

- Application startup time measurement
- Memory usage during normal operation  
- Database operation latency
- Overlay display/hide performance
- Theme switching performance

Required Optimizations:

1. Memory Management:

- Overlay ViewModel pooling system
- Service consolidation memory impact
- Resource disposal improvement
- Large DataTable handling optimization

2. Database Performance:

- Stored procedure execution monitoring
- Connection pooling optimization  
- Query result caching for master data
- Batch operation optimization

3. UI Responsiveness:

- Overlay animation performance
- Theme switching smoothness
- Grid virtualization for large datasets
- Background thread utilization

4. Application Startup:

- Service registration optimization
- Lazy loading implementation
- Resource preloading strategy
- Dependency injection performance

Performance Monitoring Implementation:

```csharp
public interface IPerformanceMonitor
{
    void StartOperation(string operationName);
    void EndOperation(string operationName);
    TimeSpan GetOperationDuration(string operationName);
    MemoryUsage GetCurrentMemoryUsage();
    Task<DatabasePerformanceMetrics> GetDatabaseMetricsAsync();
}

public class PerformanceMetrics
{
    public TimeSpan ApplicationStartupTime { get; set; }
    public MemoryUsage MemoryUsage { get; set; }  
    public Dictionary<string, TimeSpan> OperationTimings { get; set; }
    public DatabasePerformanceMetrics DatabaseMetrics { get; set; }
}
```

Optimization Targets:

- Startup time: <3 seconds (current baseline needed)
- Memory reduction: 20% from current usage
- Overlay display: <100ms
- Database operations: <500ms for standard queries
- Theme switching: <200ms with smooth transitions

Implementation Requirements:

- Use .NET performance counters
- Integrate with MTM logging system
- Optional performance overlay for debugging
- Automated performance regression detection
- Performance metrics export for analysis

Create the performance optimization system with monitoring and reporting.

```

### **Prompt 3.3: Production Deployment and Documentation**

```

Help me prepare the refactored MTM WIP Application for production deployment with comprehensive documentation and deployment procedures.

Required Documentation Updates:

1. Architecture Documentation:

- Updated project-blueprint.md with new service organization
- Service consolidation documentation with before/after diagrams
- Universal Overlay Service architecture guide
- Database integration patterns documentation

2. Developer Onboarding:

- Updated development setup instructions
- New service location guide after consolidation
- Overlay development workflow and templates
- Troubleshooting guide for common refactoring issues

3. User Guide Updates:

- New overlay interaction patterns
- Updated keyboard shortcuts and navigation
- Theme system changes and new options
- Feature improvements and enhanced workflows

4. Deployment Guide:

- Production deployment checklist
- Database schema validation procedures
- Service registration verification steps
- Performance baseline establishment

Production Readiness Checklist:

System Validation:

- [ ] All services resolve correctly through dependency injection
- [ ] Database stored procedures execute without errors
- [ ] All overlay integrations function properly
- [ ] Theme system works across all views
- [ ] Memory usage meets optimization targets
- [ ] Application startup time within acceptable range

Quality Assurance:

- [ ] Unit tests pass (target: >90% coverage)
- [ ] Integration tests validate service interactions
- [ ] UI automation tests cover critical paths
- [ ] Performance tests meet established benchmarks
- [ ] Cross-platform testing completed (if applicable)

Deployment Preparation:

- [ ] Configuration files updated for production
- [ ] Database connection strings validated
- [ ] Logging configuration optimized for production
- [ ] Error handling tested under various failure scenarios
- [ ] Backup and rollback procedures documented

Documentation Structure:

```
docs/
├── architecture/
│   ├── project-blueprint.md (UPDATED)
│   ├── service-consolidation-guide.md (NEW)
│   └── overlay-system-architecture.md (NEW)
├── development/
│   ├── getting-started.md (UPDATED)
│   ├── service-location-guide.md (NEW)
│   └── overlay-development-guide.md (NEW)
├── deployment/
│   ├── production-deployment.md (NEW)
│   ├── rollback-procedures.md (NEW)
│   └── monitoring-setup.md (NEW)
└── user-guide/
    ├── overlay-interactions.md (NEW)
    └── keyboard-shortcuts.md (UPDATED)
```

Please create the production deployment package with all required documentation.

```

---

## 📋 SPECIFIC IMPLEMENTATION PROMPTS

### **Prompt: Create Inventory Quick Edit Overlay**

```

Using the Complete Overlay Development Tutorial as a guide, help me implement the Inventory Quick Edit Overlay for this MTM WIP Application.

Requirements:

- Allow editing inventory quantity and notes for a specific Part ID + Operation + Location
- Validate quantity (must be non-negative, reasonable limits)
- Support notes editing with character limit (250 characters)
- Show real-time validation feedback
- Use MTM design system (Windows 11 Blue #0078D4)
- Follow MVVM Community Toolkit patterns
- Integrate with Universal Overlay Service
- Use stored procedures for database operations
- Support overlay pooling for performance

Implementation includes:

1. InventoryQuickEditRequest/Response models
2. InventoryQuickEditOverlayViewModel with full validation
3. InventoryQuickEditOverlayView.axaml with MTM styling  
4. Universal service integration
5. Unit tests with FluentAssertions
6. Service registration
7. Usage example in InventoryTabViewModel

Follow the established patterns from existing overlays (ConfirmationOverlay, SuccessOverlay) and ensure integration with MTM error handling and logging systems.

Create the complete implementation with all required files and integration code.

```

### **Prompt: Implement Global Error Overlay**

```

Help me create a Global Error Overlay for the MTM WIP Application that provides consistent, user-friendly error handling across the entire application.

Current Error Handling Issues:

- Errors displayed via Services.ErrorHandling.HandleErrorAsync() in inconsistent ways
- No visual error overlay for application-level errors
- Database connection errors not clearly communicated to users
- No error recovery guidance provided to users

Required Global Error Overlay Features:

1. Error Classification:

- Database connection errors (with retry options)
- Validation errors (with field highlighting)
- System errors (with restart options)  
- Business logic errors (with corrective actions)
- Critical errors (with emergency procedures)

2. Error Display:

- Clear error message with user-friendly language
- Technical details (collapsible for advanced users)
- Suggested recovery actions
- Option to report error to support
- Automatic error logging integration

3. Recovery Actions:

- Retry button for transient errors
- Navigate to relevant settings for configuration errors
- Restart application option for critical errors
- Contact support with pre-filled error details

Implementation Requirements:

- Use Universal Overlay Service
- Integrate with existing Services.ErrorHandling
- Follow MVVM Community Toolkit patterns  
- Use MTM design system with error-specific colors
- Support different severity levels (Info, Warning, Error, Critical)
- Include keyboard shortcuts (Escape to close, Enter to retry)
- Log all error interactions for analytics

Create the complete Global Error Overlay with ViewModel, View, and integration code.

```

### **Prompt: Service Consolidation Implementation**

```

Help me consolidate the Services folder in this MTM WIP Application from 24 individual service files into 9 well-organized, consolidated service files.

Current Services to Consolidate:

1. Core Services Group (Services/Core/CoreServices.cs):

- Configuration.cs → CoreServices.ConfigurationService
- Database.cs → CoreServices.DatabaseService  
- ErrorHandling.cs → CoreServices.ErrorHandlingService

2. Business Services Group (Services/Business/BusinessServices.cs):

- MasterDataService.cs → BusinessServices.MasterDataService
- InventoryEditingService.cs → BusinessServices.InventoryEditingService
- RemoveService.cs → BusinessServices.RemoveService

3. UI Services Group (Services/UI/UIServices.cs):

- Navigation.cs → UIServices.NavigationService
- ThemeService.cs → UIServices.ThemeService
- FocusManagementService.cs → UIServices.FocusManagementService
- SuccessOverlay.cs → UIServices.SuccessOverlayService
- SuggestionOverlay.cs → UIServices.SuggestionOverlayService

Consolidation Requirements:

- Maintain all existing interfaces and public APIs
- Update Extensions/ServiceCollectionExtensions.cs registrations
- Preserve service lifetimes (singleton, scoped, transient)
- Keep all dependency injection patterns functional
- Update all using statements in consuming classes
- Ensure no breaking changes for existing functionality

Implementation Steps:

1. Analyze service dependencies to prevent circular references
2. Create consolidated service files with proper namespacing
3. Update service registrations in dependency injection
4. Update all consuming classes with new using statements
5. Remove old individual service files
6. Validate all functionality still works correctly

Please provide the complete consolidation implementation with all file updates needed.

```

### **Prompt: Universal Overlay Service Implementation**

```

Help me implement the Universal Overlay Service for this MTM WIP Application that provides a unified, performant way to manage all overlay types.

Current Overlay System Problems:

- Multiple separate services (ISuggestionOverlayService, ISuccessOverlayService, etc.)
- Inconsistent overlay patterns and registration
- No overlay pooling for performance
- Mixed window dialogs vs embedded overlays
- No centralized overlay lifecycle management

Required Universal Service Architecture:

Interface Design:

```csharp
public interface IUniversalOverlayService
{
    Task<TResponse> ShowOverlayAsync<TRequest, TResponse, TViewModel>(TRequest request) 
        where TRequest : BaseOverlayRequest
        where TResponse : BaseOverlayResponse
        where TViewModel : BaseOverlayViewModel;
        
    Task HideOverlayAsync(string overlayId);
    Task HideAllOverlaysAsync();
    Task<bool> IsOverlayVisibleAsync(string overlayId);
    void RegisterOverlay<TViewModel, TView>() where TViewModel : BaseOverlayViewModel where TView : UserControl;
    event EventHandler<OverlayEventArgs> OverlayShown;
    event EventHandler<OverlayEventArgs> OverlayHidden;
}
```

Base Classes Required:

1. BaseOverlayRequest (overlayId, priority, modal settings, positioning)
2. BaseOverlayResponse (result, validation, timing, user action)
3. BaseOverlayViewModel (common properties, lifecycle methods, commands)
4. BasePoolableOverlayViewModel (Reset() method for pooling support)

Implementation Features:

- Overlay ViewModel pooling system for performance
- Z-index management for overlay stacking  
- Parent container detection and proper containment
- MTM theme system integration
- Memory management with proper disposal
- Async/await patterns throughout
- Comprehensive error handling and logging

Integration Requirements:

- Work with existing ConfirmationOverlay, SuccessOverlay, SuggestionOverlay
- Follow MVVM Community Toolkit patterns
- Use MTM service registration patterns  
- Integrate with Services.ErrorHandling
- Support dependency injection for overlay ViewModels

Create the complete Universal Overlay Service implementation with all base classes and integration code.

```

### **Prompt: Advanced Remove View Safety Implementation**

```

The AdvancedRemoveView in this MTM WIP Application currently allows mass deletion of inventory without proper safety confirmations. Help me implement comprehensive safety overlays and confirmations.

Critical Safety Requirements:

1. Mass Deletion Confirmation:

- Show detailed list of items to be deleted
- Require explicit confirmation by typing "DELETE" for >10 items  
- Display total quantity and value being removed
- Show affected locations and operations
- Require manager approval for >100 items

2. Destructive Operation Warning:

- High-visibility warning overlay with red styling
- Clear explanation of consequences
- Option to export data before deletion
- Undo information (if available through database)
- Audit trail requirement notification

3. Progress and Safety Monitoring:

- Real-time progress overlay during deletion process
- Ability to cancel mid-operation with proper cleanup
- Error handling for partial failures
- Success confirmation with summary of completed actions

4. Batch Operation Validation:

- Pre-deletion validation of business rules
- Check for dependencies (pending transfers, etc.)
- Verify user permissions for selected items
- Validate database constraints before proceeding

Implementation Requirements:

- Use Universal Overlay Service for all confirmations
- Follow MVVM Community Toolkit patterns in AdvancedRemoveViewModel
- Integrate with existing Services.ErrorHandling
- Use stored procedures for all database operations
- Apply MTM design system with appropriate warning colors
- Include comprehensive logging for audit compliance
- Support keyboard navigation and accessibility

Create the safety overlay system for AdvancedRemoveView with complete ViewModel updates and overlay implementations.

```

### **Prompt: MTM Theme System Integration with Overlays**

```

Help me ensure all overlays in the MTM WIP Application properly integrate with the MTM theme system and follow the established design patterns.

Current Theme System:

- Primary Color: Windows 11 Blue (#0078D4)  
- Multiple theme support: MTM_Blue, MTM_Green, MTM_Red, MTM_Dark
- ThemeService for theme switching
- Dynamic resource bindings throughout application

Overlay Theme Integration Requirements:

1. Color System:

- Primary actions: #0078D4 (Windows 11 Blue)
- Secondary blue: #106EBE  
- Success: Green variants from MTM theme
- Warning: Amber/Orange from MTM theme  
- Error: Red variants from MTM theme
- Card background: Dynamic white/dark based on theme

2. Resource Bindings:
All overlays must use DynamicResource bindings:

- MTM_Shared_Logic.CardBackgroundBrush
- MTM_Shared_Logic.BorderBrush  
- MTM_Shared_Logic.PrimaryAction
- MTM_Shared_Logic.SuccessBackgroundBrush
- MTM_Shared_Logic.ErrorBrush
- MTM_Shared_Logic.PrimaryTextBrush

3. Design System Consistency:

- Card-based layout with Border controls
- Rounded corners (CornerRadius="8")
- Consistent spacing (8px, 16px, 24px)
- Drop shadows using CardDropShadowEffect
- Proper button styling with MTM colors

4. Theme Switching Support:

- Overlays must update immediately when theme changes
- No hardcoded colors in AXAML or code-behind
- Resource inheritance from parent containers
- Smooth transitions during theme switches

Current Overlays to Update:

- ConfirmationOverlayView.axaml
- SuccessOverlayView.axaml  
- SuggestionOverlayView.axaml
- EditInventoryView.axaml
- Any new overlays created

Implementation Tasks:

1. Audit all existing overlay AXAML for hardcoded colors
2. Replace hardcoded colors with DynamicResource bindings
3. Ensure consistent card-based layout patterns
4. Test theme switching with overlays visible
5. Update overlay templates for future development

Please provide the complete theme integration updates for all overlay AXAML files.

```

### **Prompt: Cross-Platform Testing Setup**

```

Help me set up comprehensive cross-platform testing for the MTM WIP Application, focusing on Windows (primary), with potential Linux and macOS support.

Testing Requirements:

1. Platform Coverage:

- Windows 11 (primary development target)
- Windows 10 (compatibility testing)
- Linux (Ubuntu/Debian if supported)
- macOS (if Avalonia support is needed)

2. Test Categories:

- Unit tests (platform-agnostic ViewModels and Services)
- Integration tests (database, service resolution)
- UI tests (Avalonia-specific rendering and interaction)
- Performance tests (memory usage, startup time)

3. Overlay System Testing:

- Overlay display consistency across platforms
- Theme system integration on different OSes
- Keyboard navigation and shortcuts
- Font rendering and text measurement
- Mouse/touch interaction patterns

4. Database Integration Testing:

- MySQL connection across platforms
- Stored procedure execution consistency  
- Connection pooling behavior
- Error handling and timeouts

Test Framework Setup:

```
Tests/
├── CrossPlatform/
│   ├── Windows/
│   │   ├── WindowsSpecificTests.cs
│   │   └── WindowsUITests.cs
│   ├── Linux/
│   │   ├── LinuxCompatibilityTests.cs  
│   │   └── LinuxUITests.cs
│   └── Shared/
│       ├── CrossPlatformUITests.cs
│       └── DatabaseTests.cs
├── Performance/
│   ├── MemoryTests.cs
│   ├── StartupTests.cs
│   └── DatabasePerformanceTests.cs
└── Integration/
    ├── ServiceResolutionTests.cs
    └── OverlayIntegrationTests.cs
```

Implementation Requirements:

- Use MSTest with platform-specific test categories
- Avalonia UI testing framework for cross-platform UI tests
- Docker containers for Linux testing environment
- Automated CI/CD pipeline integration
- Performance baseline establishment per platform
- Screenshot comparison testing for UI consistency

Testing Scenarios:

1. Application startup and service resolution
2. Overlay display and interaction
3. Theme switching across different window managers
4. Database operations and stored procedure execution
5. Memory usage and performance metrics
6. Keyboard shortcuts and accessibility

Create the comprehensive cross-platform testing framework with all required infrastructure.

```

---

## 📋 DEVELOPMENT WORKFLOW PROMPTS

### **Prompt: Create New Overlay (Template)**

```

Help me create a new overlay for the MTM WIP Application following the established patterns and Universal Overlay Service integration.

New Overlay Specification:

- Overlay Name: [OverlayName]
- Purpose: [Brief description of functionality]
- Parent View: [Which view will use this overlay]
- User Actions: [List of actions user can take]
- Validation Rules: [Input validation requirements]
- Database Operations: [Any stored procedures needed]

Template Implementation:

1. Create request/response models in Models/Overlay/
2. Create ViewModel in ViewModels/Overlay/ with MVVM Community Toolkit
3. Create View AXAML in Views/Overlay/ with MTM design system
4. Create minimal code-behind with keyboard shortcuts
5. Add service registration in Extensions/ServiceCollectionExtensions.cs
6. Add extension methods for Universal Overlay Service
7. Create unit tests in Tests/Unit/ViewModels/Overlay/
8. Add usage example in parent ViewModel

Required Patterns:

- [ObservableObject] with [ObservableProperty] and [RelayCommand]
- BasePoolableOverlayViewModel inheritance for pooling support
- MTM theme system integration with DynamicResource
- Universal Overlay Service request/response pattern
- Stored procedures only for database operations
- Services.ErrorHandling.HandleErrorAsync() for error handling
- Comprehensive validation with real-time feedback

Please create the complete overlay implementation following the MTM patterns.

```

### **Prompt: Debug Overlay Integration Issue**

```

I'm having an issue with overlay integration in the MTM WIP Application. Help me debug and resolve the problem.

Current Issue:
[Describe the specific problem - overlay not showing, service not resolving, theme not applying, etc.]

Error Messages:
[Include any error messages, stack traces, or compilation errors]

Affected Components:

- ViewModel: [Name of ViewModel with issue]
- View: [Name of View with issue]  
- Service: [Related services]
- Database: [Any database operations involved]

Expected Behavior:
[Describe what should happen]

Actual Behavior:
[Describe what is actually happening]

Debugging Checklist:

1. Service Registration
   - [ ] ViewModel registered in ServiceCollectionExtensions
   - [ ] Overlay View registered with Universal Service
   - [ ] Interface implementations correct

2. MVVM Community Toolkit
   - [ ] [ObservableObject] attribute present
   - [ ] [ObservableProperty] fields private with correct naming
   - [ ] [RelayCommand] methods follow async patterns

3. Universal Service Integration  
   - [ ] Request/Response models inherit base classes
   - [ ] ViewModel inherits BaseOverlayViewModel
   - [ ] Service registration includes overlay mapping

4. AXAML Syntax
   - [ ] Correct Avalonia namespace (not WPF)
   - [ ] x:Name used (not Name) for Grid definitions
   - [ ] DynamicResource used for all colors
   - [ ] x:DataType matches ViewModel

5. Database Operations
   - [ ] Stored procedures exist and are correct
   - [ ] Parameters match stored procedure expectations
   - [ ] Helper_Database_StoredProcedure.ExecuteDataTableWithStatus used

Please help me diagnose and fix this overlay integration issue.

```

### **Prompt: Performance Optimization Analysis**

```

Help me analyze and optimize the performance of the MTM WIP Application after the refactoring, focusing on the 20% memory reduction target and improved responsiveness.

Current Performance Concerns:
[Describe any performance issues observed]

Analysis Requirements:

1. Memory Usage Analysis:

- Application startup memory consumption
- Memory usage during normal operation
- Overlay pooling effectiveness
- Service consolidation impact
- Memory leaks in overlay lifecycle

2. Application Startup Performance:

- Service registration time
- Dependency injection resolution
- Database connection establishment
- UI initialization time
- Resource loading time

3. Database Operation Performance:

- Stored procedure execution times
- Connection pooling efficiency
- Large result set handling
- Batch operation performance

4. UI Responsiveness:

- Overlay display/hide times
- Theme switching performance
- Grid virtualization effectiveness
- Background thread utilization

Performance Monitoring Setup:

```csharp
public class PerformanceProfiler
{
    public static void ProfileOperation(string operationName, Action operation);
    public static async Task ProfileOperationAsync(string operationName, Func<Task> operation);
    public static MemorySnapshot TakeMemorySnapshot();
    public static void LogPerformanceMetrics();
}
```

Optimization Areas:

1. Overlay ViewModel pooling implementation
2. Service consolidation memory footprint
3. Database result caching
4. UI virtualization improvements  
5. Background task optimization

Target Metrics:

- Startup time: <3 seconds
- Memory reduction: 20% from baseline
- Overlay display: <100ms
- Database queries: <500ms
- Theme switching: <200ms

Please provide performance analysis tools and optimization recommendations.

```

---

## 📊 PROGRESS TRACKING TEMPLATES

### **Implementation Progress Tracker**

Use this template to track implementation progress:

```

## MTM WIP Application Implementation Status

**Date**: [Current Date]
**Phase**: [1/2/3 - Reorganization/Overlay/Integration]
**Overall Progress**: [X]% ([X]/[Total] tasks completed)

### Current Phase Tasks

- [ ] Task 1: [Description] - Status: [Not Started/In Progress/Completed]
- [ ] Task 2: [Description] - Status: [Not Started/In Progress/Completed]  
- [ ] Task 3: [Description] - Status: [Not Started/In Progress/Completed]

### Next Phase Preparation

- [ ] Prerequisites for next phase completed
- [ ] All tests passing  
- [ ] Documentation updated
- [ ] Performance benchmarks established

### Blockers and Issues

- Issue 1: [Description] - Priority: [High/Medium/Low]
- Issue 2: [Description] - Priority: [High/Medium/Low]

### Notes

[Implementation notes, decisions made, lessons learned]

```

### **Quality Checklist**

Use this template to ensure quality standards:

```

## MTM Quality Standards Checklist

### Code Quality

- [ ] MVVM Community Toolkit patterns used correctly
- [ ] Avalonia AXAML syntax follows guidelines (no AVLN2000 errors)
- [ ] MTM design system colors used via DynamicResource
- [ ] Stored procedures used for all database operations
- [ ] Services.ErrorHandling.HandleErrorAsync() used for errors
- [ ] Proper disposal and memory management

### Testing

- [ ] Unit tests written and passing
- [ ] Integration tests validate service interactions
- [ ] UI tests cover critical overlay scenarios
- [ ] Performance tests meet benchmarks

### Documentation  

- [ ] Code comments follow XML documentation standards
- [ ] README files updated for new structure
- [ ] Architecture documentation reflects changes
- [ ] Developer onboarding guide updated

### Performance

- [ ] Memory usage optimized (pooling, disposal)
- [ ] Database operations performant (<500ms)
- [ ] UI responsiveness maintained (<100ms overlays)
- [ ] Application startup time acceptable (<3 seconds)

```

---

## 🎯 Quick Reference

### **Most Important Prompts for Getting Started**

1. **Start Here**: Use "Prompt 1.1: Services Consolidation Analysis" to begin reorganization
2. **Foundation**: Use "Prompt 1.4: Universal Overlay Service Foundation" for overlay system
3. **Safety**: Use "Prompt 2.1: Critical Safety Overlays" for manufacturing safety requirements
4. **Template**: Use "Prompt: Create New Overlay (Template)" for adding new overlays
5. **Testing**: Use "Prompt 3.1: Comprehensive Testing Framework" for validation

### **Key MTM Patterns to Remember**

- **ViewModels**: `[ObservableObject]`, `[ObservableProperty]`, `[RelayCommand]`
- **Database**: `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` only
- **AXAML**: `x:Name` not `Name`, `xmlns="https://github.com/avaloniaui"`, DynamicResource for colors
- **Colors**: Windows 11 Blue `#0078D4`, MTM theme system via DynamicResource
- **Error Handling**: `await Services.ErrorHandling.HandleErrorAsync(ex, context)`

---

**This comprehensive prompt collection provides everything needed to implement the complete MTM WIP Application refactoring project using GitHub Copilot AI assistance. Each prompt is designed to provide specific, actionable guidance while maintaining consistency with established MTM patterns and architecture.**
