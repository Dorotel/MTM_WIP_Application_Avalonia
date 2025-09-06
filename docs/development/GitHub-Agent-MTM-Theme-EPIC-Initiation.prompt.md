# GitHub Agent Initiation Prompt: MTM Theme Standardization EPIC

**Agent Configuration Requirements:**
- **Model**: Claude Sonnet 4 (REQUIRED - do not use other models)
- **Session Type**: Extended development session with file persistence
- **Workspace**: MTM_WIP_Application_Avalonia repository access required

---

## Primary Objective

Implement the **MTM Theme Standardization EPIC** - a comprehensive theme system refactoring that standardizes 22 theme files and updates 64+ view files to use the centralized `MTM_Shared_Logic` brush system with WCAG 2.1 AA compliance.

## Context Loading (CRITICAL - Execute First)

**Load these instruction and context files immediately upon session start:**

1. **Master EPIC Specification**: Load the file `EPIC_Theme_Standardization.md` from repository root
   - Contains complete project requirements, 7-phase implementation strategy
   - Includes session continuation protocol and progress tracking templates
   - 86+ files affected with specific completion criteria

2. **Implementation Manual**: Load `MTM-Theme-Standardization-Implementation-Guide.md` from `docs/development/` folder
   - Comprehensive How-to Guide following Diátaxis framework
   - Phase-by-phase procedures with specific refactoring examples
   - MTM_Shared_Logic integration patterns and WCAG compliance requirements

3. **GitHub Copilot Instructions**: Load `copilot-instructions.md` from `.github/` folder
   - Complete development patterns for .NET 8 Avalonia MVVM application
   - AXAML syntax rules, database patterns, UI generation guidelines
   - Auto-includes all specialized instruction files

## Session Management Protocol

### Immediate Startup Actions
```
1. Load all three context files above
2. Analyze current theme standardization progress
3. Identify next incomplete phase from EPIC tracking
4. Create session continuation entry in EPIC document
5. Begin systematic implementation following Implementation Guide procedures
```

### Progress Tracking Requirements
- Update `EPIC_Theme_Standardization.md` session log after each major milestone
- Track completed files in the standardization checklist
- Maintain WCAG compliance validation throughout
- Document any implementation variations for consistency

## Implementation Strategy Overview

### Phase 1: Foundation (Priority)
- **Target**: `MTM_Shared_Logic.axaml` - Core brush definitions (80+ brushes)
- **Scope**: Establish centralized theme architecture
- **Validation**: All brush definitions follow MTM naming conventions

### Phase 2-3: High-Priority Themes
- **Target**: Primary navigation themes (`DarkNavigation`, `BlueNavigation`, etc.)
- **Scope**: Convert hardcoded colors to dynamic resource references
- **Validation**: WCAG 2.1 AA contrast compliance

### Phase 4-7: Comprehensive Rollout
- **Target**: All remaining theme files and view files
- **Scope**: Complete standardization across entire application
- **Validation**: Consistent visual appearance, theme switching functionality

## Technical Requirements

### MTM_Shared_Logic Brush System
```xml
<!-- Standard Pattern -->
<Brush x:Key="MTM_Shared_Logic.CardBackgroundBrush">#FFFFFF</Brush>
<Brush x:Key="MTM_Shared_Logic.PrimaryAccentBrush">#0078D4</Brush>
<Brush x:Key="MTM_Shared_Logic.TextPrimaryBrush">#323130</Brush>
```

### View File Refactoring Pattern
```xml
<!-- Before: Hardcoded -->
<Border Background="White" BorderBrush="#E0E0E0">

<!-- After: Dynamic Resource -->
<Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}">
```

### WCAG Compliance Validation
- Minimum contrast ratio 4.5:1 for normal text
- Minimum contrast ratio 3:1 for large text
- Use provided contrast validation tools for verification

## Key Files and Patterns

### Critical Theme Files (Priority Order)
1. `Resources/Themes/MTM_Shared_Logic.axaml` - Foundation brush system
2. `Resources/Themes/MTM_Dark.axaml` - Dark theme implementation  
3. `Resources/Themes/MTM_Blue.axaml` - Blue theme implementation
4. `Resources/Themes/MTM_Green.axaml` - Green theme implementation
5. `Resources/Themes/MTM_Red.axaml` - Red theme implementation

### High-Impact View Files (Early Targets)
- `Views/MainForm/MainView.axaml` - Primary application interface
- `Views/MainForm/InventoryTabView.axaml` - Core business functionality
- `Views/SettingsForm/ThemeBuilderView.axaml` - Theme testing interface
- Navigation and panel components

### Code Patterns to Follow
- **Avalonia AXAML**: Use `xmlns="https://github.com/avaloniaui"`, avoid `Name` on Grid definitions
- **Database Access**: Stored procedures only via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`
- **MVVM**: MVVM Community Toolkit with `[ObservableObject]`, `[ObservableProperty]`, `[RelayCommand]`
- **Error Handling**: Centralized via `Services.ErrorHandling.HandleErrorAsync()`

## Success Criteria

### Phase Completion Indicators
- [ ] All theme files use MTM_Shared_Logic brush references
- [ ] All view files converted to dynamic resource bindings
- [ ] WCAG 2.1 AA compliance validated across all themes
- [ ] Theme switching functionality preserved and tested
- [ ] No visual regression in application appearance
- [ ] Performance impact minimized (dynamic resources optimized)

### Session Continuation Protocol
1. Document session completion status in EPIC file
2. Provide clear next session startup instructions
3. Maintain consistency across agent handoffs
4. Preserve all implementation decisions and rationale

## Expected Timeline

**Estimated Implementation**: 15-20 working sessions
- **Phase 1**: 2-3 sessions (Foundation)
- **Phase 2-3**: 4-6 sessions (High-priority themes)
- **Phase 4-5**: 6-8 sessions (View file conversion)
- **Phase 6-7**: 3-4 sessions (Testing and validation)

## Agent Interaction Guidelines

### Communication Style
- Provide progress updates after each major milestone
- Ask for validation before making significant architectural changes
- Document any implementation decisions that deviate from the guide
- Maintain focus on systematic, phase-by-phase execution

### Problem Resolution
- Reference Implementation Guide for specific procedures
- Use EPIC document for project scope and requirements clarification  
- Follow established MTM application patterns from copilot-instructions.md
- Escalate complex decisions with clear options and recommendations

---

**Begin implementation by loading the three context files and analyzing current standardization progress. Follow the systematic phase-by-phase approach outlined in the Implementation Guide.**
