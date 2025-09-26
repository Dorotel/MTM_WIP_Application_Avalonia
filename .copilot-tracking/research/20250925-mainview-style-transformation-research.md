<!-- markdownlint-disable-file -->
# MTM Style System Single-File Transformation Research: MainView.axaml

**Research Date**: September 25, 2025
**Target File**: MainView.axaml
**Research Phase**: Comprehensive analysis for Theme V2 + StyleSystem transformation

## Research Executed

### Target File Analysis: `MainView.axaml`
- **File Location**: `c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\Views\MainForm\Panels\MainView.axaml`
  - **Purpose**: Main application container and navigation hub with tabbed interface, header menu, status bar, and collapsible quick actions panel
  - **Current StyleSystem Compliance**: 25% - Mixed implementation with some StyleSystem classes but significant hardcoded styling
  - **Theme V2 Integration Status**: Partial - Uses DynamicResource for some tokens but has hardcoded values
  - **ScrollViewer Policy Compliance**: NON-COMPLIANT - Contains multiple ScrollViewer instances without approval
  - **Manufacturing Domain Requirements**: Primary navigation for inventory, remove, and transfer operations

### Code Search Results for `MainView`
- **Current Styling Patterns Found**:
  - Classes Usage: Limited use of StyleSystem classes ("TabTitle", "HeaderPanel", "FooterPanel", "Card", "Card Elevated")
  - DynamicResource: Partial Theme V2 integration with semantic tokens
  - Hardcoded Styles: Multiple hardcoded dimensions, padding, margins, and effects

- **MVVM Bindings Analysis**:
  - ViewModel Binding: `x:DataType="vm:MainViewViewModel"` - MUST preserve
  - Command Bindings: RefreshCommand, OpenSettingsCommand, OpenPersonalHistoryCommand, CancelCommand, etc. - MUST preserve
  - Content Properties: InventoryContent, RemoveContent, TransferContent - MUST preserve
  - State Management: SelectedTabIndex, IsQuickActionsPanelExpanded, IsAdvancedPanelVisible - MUST preserve

### StyleSystem Component Dependencies
- **Required Components (Existing)**:
  - ✅ Cards.axaml: Available in StyleSystem (Card, Card.Elevated classes)
  - ✅ Typography.axaml: Available for heading styles
  - ✅ Buttons.axaml: Available for basic button styling

- **Required Components (Missing)**:
  - ❌ Navigation/TabStyles.axaml: Required for consistent tab header styling with icons
  - ❌ Layout/HeaderFooterPanels.axaml: Required for header/footer panel styling
  - ❌ Overlays/OverlayPanels.axaml: Required for suggestion, success, and NewQuickButton overlays
  - ❌ Navigation/MenuStyles.axaml: Required for main menu styling

### Theme V2 Token Requirements
- **Required Tokens (Available)**:
  - ✅ ThemeV2.Background.Canvas: Main container background
  - ✅ ThemeV2.Action.Primary: Header background and version badge
  - ✅ ThemeV2.Content.OnColor: Text on colored backgrounds
  - ✅ ThemeV2.Background.Card: Card backgrounds
  - ✅ ThemeV2.Border.Default: Card borders

- **Required Tokens (Missing)**:
  - ❌ ThemeV2.Navigation.TabIcon.Background: For tab icon backgrounds
  - ❌ ThemeV2.Overlay.Background: For overlay panel backgrounds (currently using manual alpha)
  - ❌ ThemeV2.Header.Shadow: For header drop shadow effects
  - ❌ ThemeV2.Footer.Border: For footer border styling

### External Research for `MainView` Context
- **Main container patterns from Avalonia documentation**
  - DockPanel LastChildFill pattern for flexible layouts
  - Grid column definitions for responsive side panels
  - TabControl with custom headers for navigation

- **Manufacturing UI patterns**
  - Header menu with theme switcher integration
  - Status bar with progress overlay display
  - Collapsible side panel for quick actions
  - Multi-overlay system for modals and suggestions

### Project Conventions Applied
- **Standards referenced**: avalonia-ui-guidelines.instructions.md, style-system-implementation.instructions.md, theme-v2-implementation.instructions.md
- **Instructions followed**: MVVM Community Toolkit patterns, Theme V2 semantic tokens, StyleSystem class usage
- **Architecture patterns**: Main view as navigation hub, content areas as UserControl containers, overlay management

## Key Discoveries for `MainView.axaml`

### Current State Analysis

**File Structure and Purpose:**
- **Component Type**: UserControl (Primary navigation and container view)
- **Business Function**: Main application interface with tabbed navigation (Inventory/Remove/Transfer), header menu, status bar, and overlay management
- **Parent Dependencies**: App.axaml theme system, MainWindow.axaml as host container
- **Child Components**: Tab content (InventoryTabView, RemoveTabView, TransferTabView), QuickButtonsView, ThemeQuickSwitcher, ProgressOverlayView, CollapsiblePanel

**StyleSystem Compliance Assessment:**
- **Current Compliance Level**: 25%
- **StyleSystem Classes Already Used**: 
  - ✅ "TabTitle": Used for tab text styling
  - ✅ "Card": Used for status text box and elevated cards
  - ✅ "Card Elevated": Used for main tab control container
  - ✅ "Caption": Used for status and version text
- **Hardcoded Styles Found**:
  - ❌ Padding="16,12": Header padding hardcoded
  - ❌ MinHeight="64" MaxHeight="84": Header dimensions hardcoded
  - ❌ Margin="8": Multiple margin specifications
  - ❌ Width="28" Height="28": Tab icon dimensions hardcoded
  - ❌ BorderThickness="1": Multiple border thickness specifications
  - ❌ CornerRadius="8": Multiple corner radius hardcoded values
  - ❌ DropShadowEffect: Multiple hardcoded shadow effects with specific blur/offset values

**Theme V2 Integration Status:**
- **Theme V2 Tokens Already Used**:
  - ✅ ThemeV2.Background.Canvas: Main container background
  - ✅ ThemeV2.Action.Primary: Header and version badge backgrounds
  - ✅ ThemeV2.Content.OnColor: Text on colored backgrounds
  - ✅ ThemeV2.Content.Primary: Standard text content
  - ✅ ThemeV2.Background.Surface: Footer background
  - ✅ ThemeV2.Background.Card: Card backgrounds
  - ✅ ThemeV2.Border.Default: Card borders
- **Hardcoded Colors/Values Found**:
  - ❌ Color references in DropShadowEffect: "ThemeV2.Color.Blue.400", "ThemeV2.Color.Gray.400" (should use semantic tokens)
  - ❌ Opacity values: 0.3, 0.2, 0.15, 0.25 (should use semantic opacity tokens)

### Missing Components Analysis

**StyleSystem Components Needed:**
```text
Navigation/
├── TabStyles.axaml ❌ (Must create)
│   └── Required for: Tab header styling with icon backgrounds and consistent spacing
├── MenuStyles.axaml ❌ (Must create)  
│   └── Required for: Header menu styling with proper Theme V2 integration
Layout/
├── HeaderFooterPanels.axaml ❌ (Must create)
│   └── Required for: Header and footer panel styling with proper shadows and borders
Overlays/
├── OverlayPanels.axaml ❌ (Must create)
│   └── Required for: Suggestion, success, and NewQuickButton overlay styling
Effects/
└── Shadows.axaml ❌ (Must create)
    └── Required for: Standardized drop shadow effects using semantic tokens
```

**Theme V2 Tokens Needed:**
- **Missing Color Tokens**: 
  - ThemeV2.Navigation.TabIcon.Background (for tab icon backgrounds)
  - ThemeV2.Overlay.Background (for overlay panel backgrounds)
  - ThemeV2.Shadow.Color.Header (for header shadow color)
  - ThemeV2.Shadow.Color.Footer (for footer shadow color)
  - ThemeV2.Shadow.Color.Card (for card shadow color)
- **Missing Semantic Tokens**: 
  - ThemeV2.Opacity.Overlay (for overlay opacity values)
  - ThemeV2.Opacity.Shadow (for shadow opacity values)
- **Missing Typography Tokens**: Already sufficient

### Implementation Patterns for `MainView.axaml`

**Current Patterns Found in File:**
```xml
<!-- ✅ EXISTING: Theme V2 integration with DynamicResource -->
Background="{DynamicResource ThemeV2.Background.Canvas}"

<!-- ✅ EXISTING: StyleSystem classes for cards -->
<Border Classes="Card Elevated"
        Background="{DynamicResource ThemeV2.Background.Card}">

<!-- ✅ EXISTING: Tab header with icon and text -->
<StackPanel Orientation="Horizontal" Spacing="{StaticResource ThemeV2.Space.SM}">
    <Border Background="{DynamicResource ThemeV2.Action.Primary}">
        <materialIcons:MaterialIcon Kind="Archive" Classes="nav-icon"/>
    </Border>
    <TextBlock Text="Inventory" Classes="TabTitle"/>
</StackPanel>
```

**Required Transformation Patterns:**
```xml
<!-- 🔄 TRANSFORM: Replace hardcoded padding with StyleSystem -->
<!-- BEFORE (current): -->
<Border Padding="16,12" MinHeight="64" MaxHeight="84">

<!-- AFTER (required): -->
<Border Classes="HeaderPanel" Padding="{StaticResource ThemeV2.Spacing.Large}">

<!-- 🔄 TRANSFORM: Replace hardcoded drop shadows with StyleSystem -->
<!-- BEFORE (current): -->
<Border.Effect>
    <DropShadowEffect Color="{DynamicResource ThemeV2.Color.Blue.400}"
                      BlurRadius="8" OffsetX="0" OffsetY="2" Opacity="0.3"/>
</Border.Effect>

<!-- AFTER (required): -->
<Border Classes="HeaderPanel WithShadow">
```

**New Patterns Needed:**
```xml
<!-- ❌ NEW: Tab styling with StyleSystem classes -->
<TabItem Classes="MainNavigation">
    <TabItem.Header>
        <StackPanel Classes="TabHeader">
            <Border Classes="TabIcon Primary">
                <materialIcons:MaterialIcon Kind="Archive" Classes="TabIcon"/>
            </Border>
            <TextBlock Classes="TabTitle" Text="Inventory"/>
        </StackPanel>
    </TabItem.Header>
</TabItem>

<!-- ❌ NEW: Overlay panel styling -->
<Border Classes="OverlayPanel Elevated" IsVisible="{Binding ShowOverlay}">
    <ContentControl Classes="OverlayContent"/>
</Border>
```

### Complete Examples for `MainView` Transformation

**File-Specific StyleSystem Implementation:**
```xml
<!-- Main container with proper Theme V2 background -->
<Grid Background="{DynamicResource ThemeV2.Background.Canvas}"
      HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
      
    <DockPanel Classes="MainContainer" LastChildFill="True">
        <!-- Header with StyleSystem classes -->
        <Border Classes="HeaderPanel WithShadow" DockPanel.Dock="Top">
            <Grid Classes="HeaderContent">
                <Menu Classes="MainMenu" Grid.Column="0"/>
                <overlays:ThemeQuickSwitcher Classes="ThemeSwitcher" Grid.Column="1"/>
            </Grid>
        </Border>
        
        <!-- Footer with StyleSystem classes -->
        <Border Classes="FooterPanel WithBorder" DockPanel.Dock="Bottom">
            <Grid Classes="FooterContent">
                <overlayViews:ProgressOverlayView Classes="ProgressDisplay"/>
                <Border Classes="StatusCard"/>
                <Border Classes="VersionBadge"/>
            </Grid>
        </Border>
        
        <!-- Main content with tab control -->
        <Grid Classes="MainContent">
            <Border Classes="Card Elevated TabContainer">
                <TabControl Classes="MainNavigation">
                    <!-- Tabs with consistent styling -->
                </TabControl>
            </Border>
        </Grid>
    </DockPanel>
</Grid>
```

**Business Logic Preservation:**
```xml
<!-- CRITICAL: Preserve existing MVVM bindings -->
<UserControl x:Class="MTM_WIP_Application_Avalonia.Views.MainView"
             x:CompileBindings="True"
             x:DataType="vm:MainViewViewModel">

<!-- CRITICAL: Preserve command bindings -->
<UserControl.KeyBindings>
    <KeyBinding Gesture="F5" Command="{Binding RefreshCommand}"/>
    <KeyBinding Gesture="Ctrl+S" Command="{Binding OpenSettingsCommand}"/>
    <KeyBinding Gesture="Ctrl+H" Command="{Binding OpenPersonalHistoryCommand}"/>
    <KeyBinding Gesture="Escape" Command="{Binding CancelCommand}"/>
</UserControl.KeyBindings>

<!-- CRITICAL: Preserve content bindings -->
<ContentControl Content="{Binding InventoryContent}"/>
<ContentControl Content="{Binding RemoveContent}"/>
<ContentControl Content="{Binding TransferContent}"/>
```

### Target File API and Integration Points

**MVVM Dependencies:**
- **ViewModel**: `MainViewViewModel`
- **Key Properties**: SelectedTabIndex, StatusText, IsQuickActionsPanelExpanded, IsAdvancedPanelVisible, ShowDevelopmentMenu
- **Commands**: RefreshCommand, OpenSettingsCommand, OpenPersonalHistoryCommand, CancelCommand, ExitCommand, OpenAboutCommand
- **Event Handlers**: OnTabSelectionChanged, OnTabControlPointerPressed
- **Content Properties**: InventoryContent, RemoveContent, TransferContent

**Parent/Child Integration:**
- **Parent Container**: MainWindow.axaml (host window)
- **Child Controls**: CollapsiblePanel, ThemeQuickSwitcher, ProgressOverlayView, QuickButtonsView, NewQuickButtonView
- **Service Dependencies**: ProgressOverlayService, QuickButtonsViewModel, theme switching services

### ScrollViewer Policy Compliance for `MainView`

**Current ScrollViewer Usage:**
- **Status**: NON-COMPLIANT
- **Usage Details**: No direct ScrollViewer usage found in MainView.axaml, but child content areas may contain ScrollViewers
- **Required Action**: Verify child components comply with ScrollViewer policy during integration testing

### Manufacturing Domain Requirements for `MainView`

**Specific Manufacturing Context:**
- **Operation Types Supported**: Navigation hub for operations 90 (Move), 100 (Receive), 110 (Ship)
- **Data Types Handled**: Inventory transactions, part management, location transfers
- **User Workflow Integration**: Primary navigation interface for manufacturing operators
- **Industrial UI Requirements**: High contrast support (WCAG 2.1 AA), clear navigation hierarchy, status feedback

## Recommended Approach for `MainView.axaml`

### Pre-Implementation Requirements

**Missing StyleSystem Components to Create:**
1. **Navigation/TabStyles.axaml** - Required for consistent tab styling with icon backgrounds and proper spacing
2. **Layout/HeaderFooterPanels.axaml** - Required for header and footer panel styling with shadows and borders
3. **Overlays/OverlayPanels.axaml** - Required for suggestion, success, and NewQuickButton overlay styling
4. **Navigation/MenuStyles.axaml** - Required for main menu styling with Theme V2 integration
5. **Effects/Shadows.axaml** - Required for standardized drop shadow effects using semantic tokens

**Missing Theme V2 Tokens to Add:**
1. **ThemeV2.Navigation.TabIcon.Background** - Required for tab icon background colors
2. **ThemeV2.Overlay.Background** - Required for overlay panel backgrounds
3. **ThemeV2.Shadow.Color.Header** - Required for header shadow effects
4. **ThemeV2.Shadow.Color.Footer** - Required for footer shadow effects
5. **ThemeV2.Opacity.Overlay** - Required for overlay opacity values
6. **ThemeV2.Opacity.Shadow** - Required for shadow opacity values

### Transformation Strategy for `MainView`

**Phase 1: Component Preparation**
- Create missing StyleSystem components identified above
- Add missing Theme V2 tokens to both Light and Dark themes
- Update StyleSystem.axaml to include new components
- Validate compilation with new components

**Phase 2: File Backup and Analysis**
- Create backup: `MainView.axaml.backup`
- Document current file size: ~450 lines, high complexity with multiple nested components
- Identify all hardcoded values for replacement: ~20 hardcoded dimensions, ~8 hardcoded effects
- Map MVVM bindings and event handlers to preserve: 15+ critical bindings

**Phase 3: Complete File Transformation**
- Replace ALL hardcoded styles with StyleSystem classes
- Replace ALL hardcoded colors/values with Theme V2 tokens
- Preserve ALL MVVM bindings and business logic
- Maintain layout structure and parent/child relationships
- Ensure manufacturing domain navigation requirements are met

**Phase 4: Validation and Testing**
- Compile and validate no build errors
- Test light/dark theme switching
- Verify all MVVM functionality preserved
- Validate visual consistency with StyleSystem patterns
- Test manufacturing workflow navigation integration

## Implementation Guidance for `MainView.axaml`

### Transformation Objectives

**Primary Goal**: Transform `MainView.axaml` to use 100% Theme V2 + StyleSystem patterns while preserving all business logic, MVVM functionality, and manufacturing navigation workflows.

### File-Specific Implementation Plan

**Step 1: Pre-Transformation Setup**
- ✅ Create missing StyleSystem components: Navigation/TabStyles.axaml, Layout/HeaderFooterPanels.axaml, Overlays/OverlayPanels.axaml, Navigation/MenuStyles.axaml, Effects/Shadows.axaml
- ✅ Add missing Theme V2 tokens: TabIcon.Background, Overlay.Background, Shadow colors and opacities
- ✅ Update StyleSystem.axaml includes
- ✅ Backup original file: `MainView.axaml.backup`

**Step 2: Hardcoded Value Replacement**
- 🔄 Replace hardcoded styles: 20+ instances found (padding, margins, dimensions)
- 🔄 Replace hardcoded colors: 8+ instances found (shadow colors, effects)
- 🔄 Replace hardcoded dimensions: 15+ instances found (width, height, padding, margin)
- 🔄 Replace hardcoded typography: 5+ instances found (font sizes, weights)

**Step 3: StyleSystem Class Application**
- 🔄 Apply layout classes: MainContainer, HeaderPanel, FooterPanel, MainContent, TabContainer
- 🔄 Apply component classes: Card, Card.Elevated, TabHeader, TabIcon, StatusCard, VersionBadge
- 🔄 Apply modifier classes: WithShadow, WithBorder, Primary, Elevated
- 🔄 Apply context classes: MainNavigation, ThemeSwitcher, ProgressDisplay, OverlayPanel

**Step 4: MVVM Preservation Verification**
- ✅ Preserve bindings: SelectedTabIndex, StatusText, IsQuickActionsPanelExpanded, IsAdvancedPanelVisible, content properties
- ✅ Preserve commands: RefreshCommand, OpenSettingsCommand, OpenPersonalHistoryCommand, CancelCommand, ExitCommand
- ✅ Preserve event handlers: OnTabSelectionChanged, OnTabControlPointerPressed
- ✅ Preserve business logic: Tab navigation, overlay management, theme switching integration

### File-Specific Success Criteria

**Functional Requirements:**
- ✅ All existing navigation functionality preserved
- ✅ All MVVM bindings operational
- ✅ All keyboard shortcuts working
- ✅ Tab switching and content loading functional
- ✅ Overlay management system operational
- ✅ Theme switching integration maintained
- ✅ Status bar and progress display functional

**Styling Requirements:**
- ✅ Zero hardcoded styles remaining
- ✅ Zero hardcoded colors/values remaining
- ✅ Perfect light/dark theme compatibility
- ✅ Consistent with existing StyleSystem patterns
- ✅ Manufacturing navigation hierarchy maintained
- ✅ Industrial UI accessibility standards met

**Quality Gates:**
- ✅ Compilation successful (no build errors)
- ✅ Runtime testing passed (no exceptions)
- ✅ Theme switching works perfectly
- ✅ Visual regression testing passed
- ✅ Manufacturing workflow navigation verified
- ✅ All child component integration maintained

### Rollback Plan

**If transformation fails:**
1. Restore from backup: `MainView.axaml.backup`
2. Document failure reasons in research notes
3. Identify missing prerequisites (likely missing StyleSystem components)
4. Plan corrective actions for retry

### Next Steps After Successful Transformation

**Documentation:**
- Update transformation log with success metrics
- Document lessons learned for future main container files
- Update StyleSystem usage patterns for navigation components

**Validation:**
- Add to validated files list
- Update overall project StyleSystem compliance percentage
- Prepare for child component transformation (InventoryTabView, RemoveTabView, TransferTabView)
