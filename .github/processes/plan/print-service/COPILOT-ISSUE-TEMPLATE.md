# GitHub Issue: Print Service Implementation

**Copy and paste this content into a new GitHub issue in your repository to trigger the Copilot coding agent.**

---

**Issue Title:**
```
[FEATURE] Print Service - Full Window Print Interface with Preview and Layout Control
```

**Issue Body:**

## Feature Overview & Business Need

**What this feature does:**
This feature provides a comprehensive Print Service for the MTM WIP Application that enables full-window printing capabilities with professional print options, real-time preview, and advanced layout customization for DataGrid contents.

**Business need:**
Users currently struggle with printing inventory reports, transaction histories, and other DataGrid data in a professional format. The existing application lacks dedicated printing functionality that matches the quality and customization needs of manufacturing operations.

**Expected benefit:**
This will enable users to:
- Print professional-quality reports directly from any DataGrid
- Customize column visibility and print layout styles
- Preview print output before printing to avoid waste
- Access advanced print options (printer selection, orientation, copies, collation)
- Save and reuse custom layout templates for consistent reporting

## Detailed Requirements

**Core Requirements:**
- Requirement 1: Must provide full-window Print View that integrates with existing NavigationService pattern (like ThemeEditorView)
- Requirement 2: Should integrate with all existing DataGrid controls (Inventory, Remove, Transfer, Transactions)
- Requirement 3: Must follow established MTM MVVM Community Toolkit patterns with dependency injection
- Requirement 4: Must support dual-panel layout: Left panel (print options) + Right panel (preview/layout)
- Requirement 5: Must provide PrintLayoutControl for column visibility and print style management

**UI/UX Requirements:**
- Interface should include Print Options panel (printer, orientation, copies, collation)
- Navigation should follow existing full-window pattern used by ThemeEditor
- Theme integration with MTM design system using DynamicResource bindings
- Print Preview panel with zoom controls and scrollable viewing
- PrintLayoutControl with column visibility toggles and Simple/Stylized style options
- Professional action buttons with MTM Windows 11 Blue styling

**Technical Requirements:**
- Must use MVVM Community Toolkit patterns with [ObservableProperty] and [RelayCommand]
- Integration with existing services (NavigationService, ThemeService, ErrorHandling, ConfigurationService)
- Avalonia AXAML with proper syntax compliance (xmlns="https://github.com/avaloniaui")
- Optional database integration using Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()
- Professional print output using System.Drawing.Printing or equivalent

## User Workflow & Interaction Design

**User Workflow:**
1. User clicks [Print] button from [DataGrid context menu or toolbar] in any view (Inventory, Remove, Transfer, etc.)
2. System displays [Full-window PrintView] replacing current MainWindow content
3. User can [select printer], [choose orientation], [set copies/collation], [adjust print options]
4. User sees [real-time print preview] in right panel with zoom controls
5. User can click [Layout] button to customize column visibility and print style
6. User completes workflow with [Print] button for actual printing or [Cancel] to return to MainView

**Key Interactions:**
- Primary actions: Print button (execute print), Layout button (customize), Preview navigation
- Secondary actions: Cancel (return to MainView), Save Template, Zoom in/out
- Error handling: Graceful printer errors, large dataset warnings, theme fallbacks

## Technical Specifications for Copilot

**Technology Stack:**
- .NET 8, Avalonia 11.3.4, MVVM Community Toolkit 8.3.2
- MySQL 9.4.0 with stored procedures only (optional for user preferences)
- Microsoft Extensions 9.0.8 (DI, Logging, Configuration, Hosting)
- System.Drawing.Printing for Windows print integration
- Existing MTM services (NavigationService, ThemeService, ErrorHandling)

**Architecture Patterns:**
- MVVM with [ObservableProperty] and [RelayCommand] attributes only
- Service-oriented design with dependency injection via ServiceCollectionExtensions
- Navigation via existing NavigationService for full-window transitions
- Error handling via Services.ErrorHandling.HandleErrorAsync() centralized pattern
- Theme integration via ThemeService with DynamicResource bindings

**Database Integration:**
- Use Helper_Database_StoredProcedure.ExecuteDataTableWithStatus() if database operations needed
- Stored procedures: Optional user preferences storage (print settings, layout templates)
- No direct SQL queries allowed - stored procedures only pattern
- Fallback to in-memory configuration if database unavailable

**UI Framework:**
- Avalonia UserControl inheritance with minimal code-behind
- AXAML with xmlns="https://github.com/avaloniaui" (NOT WPF namespace)
- MTM design system with DynamicResource bindings (MTM_Shared_Logic.* resources)
- Grid layouts with proper ColumnDefinitions/RowDefinitions syntax
- Card-based design with consistent 8px/16px/24px spacing

## Expected Files to Create/Modify

**New Files to Create:**
- `Services/PrintService.cs` - Core print service implementation with IPrintService interface
- `ViewModels/PrintViewModel.cs` - Main print view model with MVVM Community Toolkit patterns
- `ViewModels/PrintLayoutControlViewModel.cs` - Layout customization view model
- `Views/PrintView.axaml` - Main print interface with dual-panel layout
- `Views/PrintView.axaml.cs` - Minimal code-behind following established pattern
- `Views/PrintLayoutControl.axaml` - Column layout customization interface  
- `Views/PrintLayoutControl.axaml.cs` - Minimal code-behind
- `Models/PrintConfiguration.cs` - Print settings and configuration models
- `Models/PrintLayoutTemplate.cs` - Layout template data models

**Files to Modify:**
- `Extensions/ServiceCollectionExtensions.cs` - Add IPrintService and ViewModels registration
- `ViewModels/MainForm/InventoryTabViewModel.cs` - Add PrintDataGridCommand
- `ViewModels/MainForm/RemoveItemViewModel.cs` - Add PrintDataGridCommand
- `ViewModels/MainForm/TransferItemViewModel.cs` - Add PrintDataGridCommand
- `ViewModels/TransactionsForm/TransactionsViewModel.cs` - Add PrintDataGridCommand (if exists)
- `Views/MainForm/Panels/InventoryTabView.axaml` - Add Print button to UI
- `Views/MainForm/Panels/RemoveTabView.axaml` - Add Print button to UI
- `Views/MainForm/Panels/TransferTabView.axaml` - Add Print button to UI

**Optional Database Files:**
- New stored procedures for user print preferences if database integration desired
- Print layout template storage procedures if template persistence needed

## Integration Points & Dependencies

**Service Dependencies:**
- INavigationService - For full-window view navigation (MainView ↔ PrintView transitions)
- IThemeService - For UI theming and MTM design system integration
- IConfigurationService - For print settings storage and user preferences
- IErrorHandling - For centralized error management and user feedback
- ILogger<T> - For comprehensive logging of print operations

**ViewModel Dependencies:**
- MainViewViewModel - For navigation integration and parent context
- BaseViewModel - For inheritance and common functionality
- Program.GetService<T>() - For service resolution in commands

**Database Dependencies:**
- Existing stored procedures: None required (optional preference storage only)
- Master data tables: None required (self-contained feature)
- Helper_Database_StoredProcedure - If database operations implemented

**UI Integration:**
- Parent views: All DataGrid-containing views (Inventory, Remove, Transfer, Transactions)
- Navigation flow: DataGrid → Print Button → Full-window PrintView → Back to original view
- Theme system: Complete integration with all MTM theme variants (Blue, Green, Dark, Red)

## Acceptance Criteria

**Functional Criteria:**
- [ ] Feature accessible from Print button on DataGrid toolbars/context menus
- [ ] Full-window PrintView opens with NavigationService integration
- [ ] Left panel shows printer selection, orientation, copies, collation options
- [ ] Right panel shows real-time print preview with zoom controls
- [ ] Layout button opens PrintLayoutControl for column customization
- [ ] Simple and Stylized print style options available
- [ ] Professional print output matches preview accurately
- [ ] Cancel/Back navigation returns to original MainView context
- [ ] Integration with existing NavigationService pattern works seamlessly
- [ ] Error handling works for printer errors, large datasets, connectivity issues

**Technical Criteria:**
- [ ] Follows MVVM Community Toolkit patterns with [ObservableProperty] and [RelayCommand]
- [ ] Proper Avalonia AXAML syntax with no AVLN2000 compilation errors
- [ ] Service registration in ServiceCollectionExtensions with TryAddSingleton/TryAddTransient
- [ ] Comprehensive error handling via Services.ErrorHandling.HandleErrorAsync()
- [ ] MTM theme integration with DynamicResource bindings working correctly
- [ ] Full compatibility with all MTM theme variants (Blue, Green, Dark, Red)
- [ ] NavigationService integration follows ThemeEditor pattern exactly

**Quality Criteria:**
- [ ] No compilation errors or warnings in Release configuration
- [ ] Follows established MTM naming conventions throughout
- [ ] Proper dependency injection implementation with constructor injection
- [ ] Code follows existing architectural patterns found in MainViewViewModel
- [ ] Performance: Print preview generation under 2 seconds for 1000+ row DataGrids
- [ ] Memory efficiency: Under 100MB memory usage for typical print operations

## Implementation Complexity

**Complex - Advanced feature with multiple components (6-10 files)**

This feature requires:
- Multiple interconnected ViewModels with complex state management
- Advanced UI with dual-panel layout and dynamic content switching
- Integration with system printing APIs
- Real-time preview generation and rendering
- Professional print document formatting
- Template management and user preferences
- Comprehensive error handling and recovery

## Design Mockups & UI Specifications

**Layout Description:**
- Main container: Grid with ColumnDefinitions="300,*" for dual-panel layout
- Left panel: StackPanel with print options cards using 16px spacing
- Right panel: Grid with RowDefinitions="*,Auto" for preview area and action buttons
- Action buttons: Horizontal StackPanel with 8px spacing, right-aligned

**Visual Specifications:**
- Primary colors: MTM Windows 11 Blue (#0078D4) for Print buttons
- Card-based layout with Border controls, CornerRadius="8", consistent Padding="16"
- DynamicResource bindings: MTM_Shared_Logic.CardBackgroundBrush, BorderLightBrush, etc.
- Professional typography: FontWeight="Bold" for headers, FontSize="16-18" for titles

**Interactive Elements:**
- Buttons: Primary style (MTM Blue background, White foreground) for Print actions
- Buttons: Secondary style (default theme) for Cancel, Layout, Back actions
- Input controls: ComboBox for printers, NumericUpDown for copies, CheckBox for collation
- Data displays: ScrollViewer with Canvas for print preview, ItemsControl for column management

## Related Documentation & References

**Implementation Plan:**
- Detailed plan location: `docs/ways-of-work/plan/print-service/implementation-plan/implementation-plan.md`
- Complete technical specifications, architecture diagrams, and phase-by-phase implementation guide

**Related Features:**
- Similar implementation: ThemeEditorView/ThemeEditorViewModel for full-window navigation pattern
- Integration examples: MainViewViewModel service injection and NavigationService usage

**Technical References:**
- MTM patterns: All established ViewModels follow MVVM Community Toolkit patterns
- Avalonia docs: UserControl inheritance, AXAML syntax, DynamicResource bindings
- MVVM examples: MainViewViewModel, ThemeEditorViewModel for established patterns

## Special Instructions for Copilot

**Implementation Priority:**
- Start with PrintService and IPrintService interface first
- Focus on NavigationService integration as highest priority for seamless user experience
- Implement basic print preview before advanced layout customization

**Code Style Preferences:**
- Follow existing patterns in MainViewViewModel for service injection and command implementation
- Use established error handling patterns from Services.ErrorHandling.HandleErrorAsync
- Maintain consistency with ThemeEditorViewModel for full-window view implementation

**Testing Considerations:**
- Ensure feature works with existing DataGrid data structures across all views
- Test integration with NavigationService for proper full-window transitions
- Verify theme compatibility across all MTM variants (Blue, Green, Dark, Red)
- Test print preview accuracy with various DataGrid column configurations

**Special Requirements:**
- Must integrate with existing NavigationService exactly like ThemeEditor does
- Print preview must accurately reflect final print output
- Layout customization must persist during session (optional database persistence)
- Error handling must be comprehensive for all printer-related failures

---

## 🤖 Copilot Agent Activation

#github-pull-request_copilot-coding-agent
