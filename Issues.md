# MTM WIP Application - Issues Reference

## 🚨 Critical Issues (Blocking)

### 1. Quick Button Save Error - `qb_quickbuttons_Save`
- **Error**: `Parameter 'p_UserID' not found in the collection`
- **Location**: `QuickButtonsService`
- **Root Cause**: Parameter mismatch between C# code (`p_User`) and stored procedure (`p_UserID`)
- **Impact**: Quick buttons cannot be saved to database
- **Fix Required**: Update parameter mapping in QuickButtonsService

### 2. Inventory Save Error - `inv_inventory_Add_Item`
- **Error**: Status: -1, "Unknown error" when saving inventory items
- **Location**: `InventoryTabView` save button
- **Root Cause**: Stored procedure validation failure
- **Impact**: Cannot save new inventory entries
- **Fix Required**: Debug stored procedure parameters and validation logic

---

## 🔴 High Priority (User-Facing)

### 3. Theme Loading Inconsistencies
- **Components**: MainView, InventoryTabView, QuickButtonsView, child panels
- **Issue**: Colors change when clicking "Apply" on same theme (incomplete theme loading on startup)
- **Impact**: UI appears broken until manual theme reapplication
- **Fix Required**: Ensure complete theme application during application startup

### 4. Comprehensive UI Consistency Issues
- **Scope**: All Views within MainView.axaml + all custom controls
- **Components Affected**:
  - `RemoveTabView` (needs InventoryTabView grid layout pattern, proper input field containment)
  - `TransferTabView` (needs InventoryTabView grid layout pattern, proper input field containment)
  - `AdvancedInventoryView` (needs InventoryTabView grid layout pattern, proper input field containment)
  - `AdvancedRemoveView` (needs InventoryTabView grid layout pattern, proper input field containment)
  - `QuickButtonsView` (header layout, contrast issues, needs theme consistency)
  - `SuggestionOverlayView` (focus management, sizing, needs theme consistency)
  - `ThemeQuickSwitcher` (menu bar integration, needs theme consistency)
  - `CollapsiblePanel` (custom control, needs theme consistency)
  - `TransactionExpandableButton` (custom control, needs theme consistency)
- **Requirements**: 
  1. Replace ALL hardcoded colors with DynamicResource theme bindings
  2. Apply InventoryTabView's structured grid layout pattern to all tab views
  3. Ensure all user input fields are properly contained within their grid borders
  4. Implement consistent ScrollViewer with overflow handling
  5. Apply card-based layout system with proper spacing (8px, 16px, 24px)
- **Impact**: Professional appearance, theme consistency, proper input field containment across entire application

### 5. Quick Actions Panel Issues
- **Header Layout**: Lightning bolt, "Quick Actions" text, action count, toggle buttons cramped
- **Text Readability**: "actions" text too dull/unreadable
- **Contrast Problems**: Toggle buttons (Dark on Dark in light themes, Light on Light in dark themes)
- **Missing Features**: Location pass-through checkbox in footer
- **Data Integration**: Verify Part ID, Quantity, Operation, Location pass-through to MainView tabs

### 6. SuggestionOverlayView Focus & Layout
- **Focus Issue**: Focus not automatically set to first list item on view entry
- **Sizing Issue**: ListBox doesn't span full control height (should extend from "No exact match..." to buttons)
- **Impact**: Poor user experience, keyboard navigation problems

---

## 🟡 Medium Priority (Layout & Enhancement)

### 7. InventoryTabView Button Issues
- **Text Alignment**: Button text not centered with left-side icons
- **Border Visibility**: "Advanced" and "Reset" button borders too light on light themes (acceptable on dark themes)
- **Button Width**: "Advanced" button too narrow (text touches border)
- **Version Display**: Remove version from right side of lower panel (already shown in MainView)

### 8. MTM Dark Theme Enhancement
- **Issue**: Dark colors too black, lacks Windows 11 dark theme depth
- **Requirement**: Implement Windows 11-inspired dark color palette with proper contrast ranges
- **Impact**: Professional dark theme experience

### 9. Background Color Consistency
- **Location**: InventoryTabView/MainView base border
- **Issue**: Light tan background doesn't match surrounding white area (MTM Light theme)
- **Fix**: Update background to match theme's base color
- **Enhancement**: Add consistent border color to "MTM Inventory Entry" container

### 10. MainView Status Bar Reorganization
- **Remove**: Signal bar from left of Connection Status
- **Move**: Signal bar inside Connection Status oval, centered under connection info
- **Implement**: Signal bar functionality on startup
- **Layout**: Progress bar to right of Connection Status oval (no resize)

### 11. Status Box Enhancement
- **Expand**: Status box from Progress bar to Version display
- **Text Handling**: Truncate text when approaching width bounds
- **Tooltip**: Show full message on mouseover
- **Professional**: Maintain clean status display

### 12. Version Display Update
- **Format**: Change to "Ver. {Version}"
- **Source**: Read version from appsettings.json (add version: "5.0")
- **Integration**: Use ConfigurationService for dynamic version display

---

## 🟢 Low Priority (Polish & Enhancement)

### 13. Tab Control Styling
- **Enhancement**: Add contrasting border color to selected tab
- **Impact**: Clear visual indication of active tab

### 14. Menu Bar Height Optimization
- **Issue**: Top panel (File, View, Help, Theme Picker) too large
- **Fix**: Reduce to standard menu bar height
- **Update**: Adjust ThemeQuickSwitcher accordingly for proper integration

### 15. Custom Window Chrome
- **Remove**: Default window title bar
- **Implement**: Custom title bar with theme integration
- **Controls**: Add Exit, Minimize, Maximize buttons with theme consistency
- **Title**: Display application title with proper theming

---

## ⚠️ Code Quality (Must Fix)

### 16. Compilation Warnings
- **Current Count**: 23 warnings
- **Categories**: 
  - Null reference warnings (CS8602, CS8604)
  - Unused async methods (CS1998)
  - Nullable reference types
- **Requirement**: Fix ALL warnings before completion
- **Impact**: Code maintainability and reliability

---

## 📋 Implementation Roadmap

### Phase 1: Critical Database Fixes
1. Fix Quick Button save error (`p_UserID` parameter)
2. Fix Inventory save error (stored procedure validation)
3. Resolve theme loading inconsistencies

### Phase 2: UI Consistency & Theme Integration  
1. **CRITICAL**: Apply InventoryTabView grid layout pattern to RemoveTabView, TransferTabView, AdvancedInventoryView, and AdvancedRemoveView
2. **CRITICAL**: Ensure all input fields are contained within proper grid boundaries
3. **CRITICAL**: Implement ScrollViewer with overflow handling for all tab views
4. Replace ALL hardcoded colors with DynamicResource bindings across all views
5. Fix Quick Actions panel layout and contrast issues
6. Update SuggestionOverlayView focus and layout
7. Apply consistent card-based layout system (8px, 16px, 24px spacing)

### Phase 3: Layout Enhancements
1. InventoryTabView button improvements
2. MainView status bar reorganization
3. MTM Dark theme enhancement
4. Background color consistency

### Phase 4: Polish & Code Quality
1. Tab control styling and menu bar optimization
2. Custom window chrome implementation
3. Fix all 23 compilation warnings
4. Final testing and validation

---

## 🔍 Technical Context

### Key Error Signatures
```
Parameter 'p_UserID' not found in the collection
Stored procedure executed: inv_inventory_Add_Item, Status: -1, Rows: 0
Application.Current is null, cannot apply theme
```

### Child Components Requiring UI Consistency
- **Tab Content**: 
  - InventoryTabView (✅ **REFERENCE IMPLEMENTATION** - proper grid layout with contained input fields)
  - RemoveTabView (❌ needs InventoryTabView grid layout pattern)
  - TransferTabView (❌ needs InventoryTabView grid layout pattern)
  - AdvancedInventoryView (❌ needs InventoryTabView grid layout pattern)
  - AdvancedRemoveView (❌ needs InventoryTabView grid layout pattern)
- **Overlay Systems**: SuggestionOverlayView, ThemeQuickSwitcher
- **Side Panel**: QuickButtonsView with history panel  
- **Custom Controls**: CollapsiblePanel, TransactionExpandableButton

### Required Layout Pattern (InventoryTabView Standard)
All tab views must implement this structure:
1. **ScrollViewer** as root container with overflow handling
2. **Main Grid** with proper RowDefinitions (content + actions)
3. **Card-based Borders** with MTM theme resources
4. **Structured Grid** for form fields with proper containment
5. **Action Button Panel** with consistent spacing
6. **Error Display Section** with theme-appropriate styling

### Theme Resource Requirements
All components must use MTM theme resources:
- `MTM_Shared_Logic.PrimaryAction`
- `MTM_Shared_Logic.CardBackgroundBrush` 
- `MTM_Shared_Logic.BorderAccentBrush`
- `MTM_Shared_Logic.OverlayTextBrush`
- And all other theme-defined resources

### Build Environment
- **.NET 8.0** with C# 12
- **Avalonia UI 11.3.4**
- **MySQL Database** with stored procedures only
- **23 compilation warnings** requiring resolution

---

## 🏗️ MANDATORY Layout Standards (Critical Implementation)

### InventoryTabView Grid Pattern (REQUIRED FOR ALL TAB VIEWS)

**This pattern MUST be implemented in RemoveTabView and TransferTabView to prevent UI inconsistencies:**

```xml
<!-- ROOT: ScrollViewer for overflow handling -->
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
  
  <!-- MAIN: Container Grid with proper row definitions -->
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto" MinWidth="600" MinHeight="400" Margin="8">
    
    <!-- CONTENT: Entry Panel with card styling -->
    <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" 
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" 
            BorderThickness="1" CornerRadius="8" Padding="16" Margin="0,0,0,8">
      
      <!-- FORM: Structured grid for input fields -->
      <Grid x:Name="FormFieldsGrid" RowDefinitions="Auto,Auto,Auto,Auto,*" RowSpacing="12">
        <!-- Individual field grids with ColumnDefinitions="90,*" pattern -->
      </Grid>
    </Border>
    
    <!-- ACTIONS: Button panel with theme consistency -->
    <Border Grid.Row="1" Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}">
      <!-- Action buttons -->
    </Border>
  </Grid>
</ScrollViewer>
```

**Non-negotiable Requirements:**
1. ScrollViewer as root container
2. Grid with RowDefinitions="*,Auto" for content/actions separation  
3. Form fields contained within proper grid boundaries
4. All colors using DynamicResource theme bindings
5. Consistent spacing: 8px, 16px, 24px margins and padding

---

## 📚 Documentation

This reference document follows the [Diátaxis Documentation Framework](https://diataxis.fr/) principles as a structured information-oriented reference for development teams.

### Document Purpose
- **Type**: Reference Document (Information-Oriented)
- **Target Audience**: Development team, GitHub Copilot, Epic issue creation
- **Update Frequency**: Live document updated as issues are resolved
- **Optimization**: Balanced detail for Copilot assistance without performance impact

### Quick Start
1. **Critical Issues**: Address database parameter mismatches first
2. **High Priority**: Focus on UI consistency and theme integration  
3. **Medium Priority**: Layout improvements and enhancements
4. **Low Priority**: Polish and visual refinements
5. **Code Quality**: Resolve all 23 compilation warnings

### Related Documentation
- [MTM Copilot Instructions](../.github/copilot-instructions.md)
- [Architecture Patterns](../.github/copilot/context/mtm-architecture-patterns.md)
- [UI Design System](../.github/UI-Instructions/ui-generation.instruction.md)
- [Database Patterns](../.github/Development-Instructions/database-patterns.instruction.md)

### Critical Implementation Notes
⚠️ **REGRESSION PREVENTION**: The InventoryTabView grid layout pattern documented in section "🏗️ MANDATORY Layout Standards" is the REQUIRED standard for ALL tab views. Any future modifications to RemoveTabView or TransferTabView MUST maintain this structure to prevent UI inconsistencies and input field overflow issues.

⚠️ **THEME CONSISTENCY**: ALL views connected to MainView.axaml must use DynamicResource bindings for colors. Hardcoded color values will cause theme inconsistencies and must be converted to theme resource references.

⚠️ **DOCUMENTATION UPDATED**: The following documentation files have been updated to prevent regression:
- ✅ `.github/instructions/avalonia-ui-guidelines.instructions.md` (Added mandatory InventoryTabView pattern)
- ✅ `.github/copilot-instructions.md` (Added layout requirements to MTM Design System)  
- ✅ `.github/ui-ux/MTM-Design-System-Documentation.md` (Added mandatory tab view layout pattern)
- ✅ `Issues.md` (Added detailed grid layout requirements and implementation roadmap)

---

*Last Updated: September 5, 2025 | Document Version: 3.0 | Total Issues: 16*
*CRITICAL UPDATE: Added mandatory InventoryTabView grid layout pattern for all tab views*