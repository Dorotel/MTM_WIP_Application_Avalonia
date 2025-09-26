<!-- markdownlint-disable-file -->
# Task Checklist: MTM Style System Transformation for MainView.axaml

**Implementation Date**: September 25, 2025
**Target File**: MainView.axaml
**Research Reference**: 20250925-mainview-style-transformation-research.md

## Overview

Transform `MainView.axaml` to use enhanced Theme V2 + StyleSystem implementation through complete file recreation, eliminating all hardcoded styling while preserving business logic and ensuring parent container compatibility.

## Objectives

- **100% StyleSystem Coverage**: Replace all hardcoded styling with StyleSystem classes
- **100% Theme V2 Compliance**: Replace all colors with semantic tokens for perfect light/dark theme support  
- **100% Business Logic Preservation**: Maintain all MVVM bindings, commands, and manufacturing workflows
- **Parent Container Compatibility**: Ensure content fits properly without overflow
- **ScrollViewer Policy Compliance**: Follow approved usage patterns only

## Research Summary

### Project Files
- **MainView.axaml** - Main application navigation hub with tabbed interface (25% StyleSystem compliance)
- **Resources/Styles/StyleSystem.axaml** - Master style system requiring 5 new component files
- **Resources/ThemesV2/Theme.Light.axaml** - Light theme requiring 6 new semantic tokens
- **Resources/ThemesV2/Theme.Dark.axaml** - Dark theme requiring 6 new semantic tokens

### External References
- #file:../research/20250925-mainview-style-transformation-research.md - Comprehensive MainView analysis with 20+ hardcoded styles identified
- Research shows: Navigation hub pattern with header menu, status bar, tabbed content, and overlay management
- Manufacturing context: Primary interface for operations 90/100/110 navigation

### Standards References
- #file:../../.github/instructions/style-system-implementation.instructions.md - StyleSystem 8-category organization patterns
- #file:../../.github/instructions/theme-v2-implementation.instructions.md - Theme V2 semantic token architecture
- #file:../../.github/instructions/avalonia-ui-guidelines.instructions.md - Avalonia AXAML compliance (AVLN2000 prevention)

## Implementation Checklist

### [ ] Phase 1: Pre-Implementation Setup

- [ ] Task 1.1: Create Missing StyleSystem Components
  - **Navigation/TabStyles.axaml** - Tab styling with icon backgrounds and consistent spacing
    - TabItem styling for MainNavigation class
    - TabHeader StackPanel styling with proper spacing
    - TabIcon Border styling with Primary/Secondary/Success variants
    - TabTitle TextBlock styling with Theme V2 typography integration
  
  - **Layout/HeaderFooterPanels.axaml** - Header and footer panel styling
    - HeaderPanel class with proper padding and shadow effects
    - FooterPanel class with border and background styling
    - WithShadow modifier class for drop shadow effects
    - WithBorder modifier class for footer borders
  
  - **Overlays/OverlayPanels.axaml** - Overlay panel styling for modals
    - OverlayPanel base class with background and positioning
    - Elevated modifier for overlay shadows and depth
    - OverlayContent class for proper content container styling
    - Backdrop styling for overlay background dimming
  
  - **Navigation/MenuStyles.axaml** - Main menu styling
    - MainMenu class for header menu styling
    - MenuItem styling with Theme V2 semantic tokens
    - Menu separator and submenu styling
  
  - **Effects/Shadows.axaml** - Standardized shadow effects
    - HeaderShadow effect using ThemeV2.Shadow.Color.Header
    - FooterShadow effect using ThemeV2.Shadow.Color.Footer
    - CardShadow effect using ThemeV2.Shadow.Color.Card
    - OverlayShadow effect for modal panels

- [ ] Task 1.2: Add Missing Theme V2 Tokens
  - **Light Theme (Theme.Light.axaml)**:
    - ThemeV2.Navigation.TabIcon.Background: #0078D4 (Windows 11 Blue)
    - ThemeV2.Overlay.Background: #000000 with 40% opacity
    - ThemeV2.Shadow.Color.Header: #2563EB with 30% opacity
    - ThemeV2.Shadow.Color.Footer: #6B7280 with 20% opacity
    - ThemeV2.Opacity.Overlay: 0.4 for overlay backgrounds
    - ThemeV2.Opacity.Shadow: 0.15 for standard shadows
  
  - **Dark Theme (Theme.Dark.axaml)**:
    - ThemeV2.Navigation.TabIcon.Background: #60A5FA (Lighter blue for dark mode)
    - ThemeV2.Overlay.Background: #000000 with 60% opacity
    - ThemeV2.Shadow.Color.Header: #1E40AF with 25% opacity
    - ThemeV2.Shadow.Color.Footer: #374151 with 15% opacity
    - ThemeV2.Opacity.Overlay: 0.6 for dark theme overlays
    - ThemeV2.Opacity.Shadow: 0.12 for dark theme shadows

- [ ] Task 1.3: Update StyleSystem.axaml Includes
  - Add StyleInclude for Navigation/TabStyles.axaml
  - Add StyleInclude for Layout/HeaderFooterPanels.axaml
  - Add StyleInclude for Overlays/OverlayPanels.axaml
  - Add StyleInclude for Navigation/MenuStyles.axaml
  - Add StyleInclude for Effects/Shadows.axaml
  - Validate proper loading order (base styles before modifiers)
  - Test compilation with all new includes

### [ ] Phase 2: File Analysis and Backup

- [ ] Task 2.1: Create File Backup
  - Create: `MainView.axaml.backup` with original 450+ line file
  - Preserve file metadata and timestamps
  - Verify backup integrity and accessibility
  - Document backup location in transformation log

- [ ] Task 2.2: Document Current File State
  - **Baseline Metrics**: 450+ lines, 25% StyleSystem compliance
  - **Hardcoded Values**: 20+ padding/margin values, 8+ shadow effects, 15+ dimensions
  - **MVVM Dependencies**: 15+ critical bindings (SelectedTabIndex, StatusText, content properties)
  - **Business Logic**: Tab navigation, overlay management, theme switching, status display

### [ ] Phase 3: File Transformation

- [ ] Task 3.1: Replace Hardcoded Styles with StyleSystem Classes
  - **Container Structure**:
    ```xml
    <Grid Classes="MainContainer">
        <DockPanel Classes="MainLayout">
            <Border Classes="HeaderPanel WithShadow" DockPanel.Dock="Top">
            <Border Classes="FooterPanel WithBorder" DockPanel.Dock="Bottom">
            <Grid Classes="MainContent">
    ```
  
  - **Tab Control Styling**:
    ```xml
    <TabControl Classes="MainNavigation">
        <TabItem Classes="PrimaryTab">
            <TabItem.Header>
                <StackPanel Classes="TabHeader">
                    <Border Classes="TabIcon Primary">
                        <materialIcons:MaterialIcon Classes="TabIcon"/>
                    </Border>
                    <TextBlock Classes="TabTitle"/>
                </StackPanel>
            </TabItem.Header>
        </TabItem>
    </TabControl>
    ```
  
  - **Overlay Panel Styling**:
    ```xml
    <Border Classes="OverlayPanel Elevated">
        <ContentControl Classes="OverlayContent"/>
    </Border>
    ```

- [ ] Task 3.2: Replace Hardcoded Colors with Theme V2 Tokens
  - **Background Tokens**:
    - Grid: `Background="{DynamicResource ThemeV2.Background.Canvas}"`
    - Cards: `Background="{DynamicResource ThemeV2.Background.Card}"`
    - Header: `Background="{DynamicResource ThemeV2.Action.Primary}"`
    - Footer: `Background="{DynamicResource ThemeV2.Background.Surface}"`
  
  - **Content Tokens**:
    - Primary text: `Foreground="{DynamicResource ThemeV2.Content.Primary}"`
    - On-color text: `Foreground="{DynamicResource ThemeV2.Content.OnColor}"`
    - Secondary text: `Foreground="{DynamicResource ThemeV2.Content.Secondary}"`
  
  - **Border and Effect Tokens**:
    - Card borders: `BorderBrush="{DynamicResource ThemeV2.Border.Default}"`
    - Footer border: `BorderBrush="{DynamicResource ThemeV2.Border.Subtle}"`
    - Shadow effects: Use new ThemeV2.Shadow.Color.* tokens

- [ ] Task 3.3: Preserve Business Logic and MVVM Bindings
  - **Critical Bindings to Preserve**:
    - `x:DataType="vm:MainViewViewModel"` and `x:CompileBindings="True"`
    - `SelectedIndex="{Binding SelectedTabIndex, Mode=TwoWay}"`
    - `Content="{Binding InventoryContent}"`, `Content="{Binding RemoveContent}"`, `Content="{Binding TransferContent}"`
    - `IsVisible="{Binding IsAdvancedPanelVisible}"`, `IsExpanded="{Binding IsQuickActionsPanelExpanded}"`
    - `Text="{Binding StatusText}"` for status display
  
  - **Command Bindings to Preserve**:
    ```xml
    <UserControl.KeyBindings>
        <KeyBinding Gesture="F5" Command="{Binding RefreshCommand}"/>
        <KeyBinding Gesture="Ctrl+S" Command="{Binding OpenSettingsCommand}"/>
        <KeyBinding Gesture="Ctrl+H" Command="{Binding OpenPersonalHistoryCommand}"/>
        <KeyBinding Gesture="Escape" Command="{Binding CancelCommand}"/>
    </UserControl.KeyBindings>
    ```
  
  - **Event Handlers to Preserve**:
    - `SelectionChanged="OnTabSelectionChanged"`
    - `PointerPressed="OnTabControlPointerPressed"`

- [ ] Task 3.4: Validate ScrollViewer Policy Compliance
  - **Current Status**: NON-COMPLIANT (potential ScrollViewer usage in child components)
  - **Verification Required**: Check child content areas for ScrollViewer usage
  - **Action**: No direct ScrollViewer usage in MainView.axaml - compliant
  - **Child Component Monitoring**: Ensure InventoryTabView, RemoveTabView, TransferTabView comply

### [ ] Phase 4: Validation and Testing

- [ ] Task 4.1: Compilation Validation
  - Build project with `dotnet build --no-restore`
  - Verify zero AXAML compilation errors (AVLN2000 prevention)
  - Check all StyleSystem classes resolve correctly
  - Validate Theme V2 token binding and DynamicResource usage

- [ ] Task 4.2: Theme Compatibility Testing
  - **Light Theme Testing**:
    - Header: Proper blue background with white text visibility
    - Content areas: White/light backgrounds with dark text contrast
    - Status elements: Clear visibility and proper contrast ratios
    - Tab icons: Proper visibility on colored backgrounds
  
  - **Dark Theme Testing**:
    - Header: Proper blue background with light text visibility
    - Content areas: Dark backgrounds with light text contrast
    - Status elements: Clear visibility with enhanced contrast
    - Tab icons: Proper visibility with adjusted colors
  
  - **Theme Switching**:
    - Smooth transitions between light and dark modes
    - No flash or rendering issues during switch
    - All elements adapt properly to new theme

- [ ] Task 4.3: Business Logic Verification
  - **Navigation Testing**:
    - Tab switching between Inventory/Remove/Transfer works correctly
    - Tab selection state properly bound to ViewModel
    - Content areas display correct child components
  
  - **Command Testing**:
    - F5 refresh command functional
    - Ctrl+S settings command functional
    - Ctrl+H history command functional
    - Escape cancel command functional
    - Menu commands (File, View, Help) functional
  
  - **State Management**:
    - Status text updates correctly
    - Quick actions panel expand/collapse functional
    - Advanced panel visibility toggle functional
    - Overlay panel management operational

- [ ] Task 4.4: Visual Regression Testing
  - **Layout Verification**:
    - Header panel: Proper height, padding, and shadow effects
    - Footer panel: Correct border, background, and content alignment
    - Tab control: Consistent icon/text alignment and spacing
    - Content areas: Proper containment without overflow
  
  - **Manufacturing UI Enhancement**:
    - Operation icons (Archive, Delete, SwapHorizontal) display correctly
    - Theme switcher integration maintains functionality
    - Progress overlay positioning and visibility
    - Version badge styling and placement
  
  - **Responsive Behavior**:
    - Collapsible panel functionality preserved
    - Window resizing behavior maintained
    - Content area stretching and alignment correct

## Dependencies

### StyleSystem Infrastructure
- **Complete 8-category StyleSystem** - Requires 5 new component files for MainView transformation
- **StyleSystem.axaml Master File** - Must include all new component references in proper order
- **Theme V2 Semantic Tokens** - Must add 6 new tokens to both light and dark themes

### Development Tools
- **.NET 8.0 SDK** - Build and compilation support
- **Avalonia UI 11.3.4** - XAML rendering and UI framework
- **MVVM Community Toolkit 8.3.2** - ViewModel binding support
- **Material.Icons.Avalonia** - Icon rendering for tab headers

### Manufacturing Domain Knowledge
- **MTM Navigation Patterns** - Primary interface for manufacturing operations
- **User Experience Requirements** - Operator workflows and accessibility
- **Industrial UI Standards** - WCAG 2.1 AA compliance for manufacturing environment

## Success Criteria

### Technical Compliance
- ✅ Zero hardcoded values in MainView.axaml (20+ current hardcoded values eliminated)
- ✅ 100% StyleSystem class coverage for all styling (from 25% to 100%)
- ✅ 100% Theme V2 token usage for all colors and values
- ✅ Perfect compilation without errors or warnings
- ✅ ScrollViewer policy compliance verified (no direct usage)

### Functional Preservation
- ✅ All business logic preserved and operational (15+ MVVM bindings maintained)
- ✅ All manufacturing navigation workflows functional
- ✅ Tab switching and content loading operational
- ✅ Overlay management system functional
- ✅ Theme switching integration maintained
- ✅ Status display and progress overlay functional

### Quality Enhancement
- ✅ Perfect light/dark theme compatibility with manufacturing-optimized tokens
- ✅ Professional MTM manufacturing interface appearance
- ✅ WCAG 2.1 AA accessibility compliance maintained
- ✅ Maintainable codebase with consistent StyleSystem patterns
- ✅ Enhanced visual consistency with established design system
- ✅ Industrial UI standards met for manufacturing environment
