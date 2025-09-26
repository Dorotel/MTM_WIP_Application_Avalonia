---
applyTo: '.copilot-tracking/changes/YYYYMMDD-{target-file-name}-style-transformation-changes.md'
---
<!-- markdownlint-disable-file -->
# Task Checklist: MTM Style System Transformation for {TARGET_FILE}.axaml

**TEMPLATE USAGE**: Replace `{TARGET_FILE}` and `{TEMPLATE_VARIABLES}` with actual file and analysis details.

## Overview

Transform `{TARGET_FILE}.axaml` to use enhanced Theme V2 + StyleSystem implementation through complete file recreation, eliminating all hardcoded styling while preserving business logic and ensuring parent container compatibility.

## Objectives

- **100% StyleSystem Coverage**: Replace all hardcoded styling with StyleSystem classes
- **100% Theme V2 Compliance**: Replace all colors with semantic tokens for perfect light/dark theme support
- **100% Business Logic Preservation**: Maintain all MVVM bindings, commands, and manufacturing workflows
- **Parent Container Compatibility**: Ensure content fits properly without overflow
- **ScrollViewer Policy Compliance**: Follow approved usage patterns only

## Research Summary

### Project Files
- **{TARGET_FILE}.axaml** - Target file for style system transformation
- **Resources/Styles/StyleSystem.axaml** - Master style system with 8-category organization
- **Resources/ThemesV2/Theme.Light.axaml** - Light theme semantic tokens
- **Resources/ThemesV2/Theme.Dark.axaml** - Dark theme semantic tokens

### External References
- #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md - Comprehensive file analysis and requirements
- #githubRepo:"microsoft/fluentui-blazor theme implementation" - Theme system patterns and token usage
- #fetch:https://docs.avaloniaui.net/docs/styling/styles - Avalonia styling documentation

### Standards References
- #file:../../.github/instructions/style-system-implementation.instructions.md - StyleSystem usage patterns and conventions
- #file:../../.github/instructions/theme-v2-implementation.instructions.md - Theme V2 semantic token system
- #file:../../.github/instructions/avalonia-ui-guidelines.instructions.md - Avalonia AXAML standards and practices

## Implementation Checklist

### [ ] Phase 1: Pre-Implementation Setup

- [ ] Task 1.1: Create Missing StyleSystem Components
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 15-55)
  - Create required StyleSystem component files identified in research phase
  - Add missing styles for {MISSING_COMPONENT_CATEGORIES}
  - Validate component compilation and availability

- [ ] Task 1.2: Add Missing Theme V2 Tokens
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 57-85)
  - Add required semantic tokens to both Light and Dark themes
  - Ensure proper contrast ratios for manufacturing environment
  - Validate token accessibility and theme compatibility

- [ ] Task 1.3: Update StyleSystem.axaml Includes
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 87-110)
  - Add StyleInclude references for new component files
  - Maintain proper loading order and dependency management
  - Validate master StyleSystem compilation

### [ ] Phase 2: File Analysis and Backup

- [ ] Task 2.1: Create File Backup
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 115-135)
  - Create safety backup: `{TARGET_FILE}.axaml.backup`
  - Preserve original file integrity and metadata
  - Verify backup accessibility and completeness

- [ ] Task 2.2: Document Current File State
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 137-165)
  - Record baseline metrics (size, line count, hardcoded values)
  - Catalog existing StyleSystem and Theme V2 usage  
  - Map business logic and MVVM bindings to preserve

### [ ] Phase 3: File Transformation

- [ ] Task 3.1: Replace Hardcoded Styles with StyleSystem Classes
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 170-220)
  - Apply Layout, Component, Modifier, Context, and Manufacturing classes
  - Transform hardcoded styling patterns to StyleSystem equivalents
  - Maintain visual appearance while improving maintainability

- [ ] Task 3.2: Replace Hardcoded Colors with Theme V2 Tokens
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 222-255)
  - Replace all color values with semantic tokens
  - Apply Background, Content, Action, Status, and Manufacturing tokens
  - Ensure perfect light/dark theme compatibility

- [ ] Task 3.3: Preserve Business Logic and MVVM Bindings
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 257-290)
  - Maintain all DataContext, Command, and Property bindings
  - Preserve event handlers, converters, and behaviors
  - Ensure manufacturing workflows remain operational

- [ ] Task 3.4: Validate ScrollViewer Policy Compliance
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 292-315)
  - Verify ScrollViewer usage follows approved patterns only
  - Request approval for any policy exceptions
  - Implement alternative layout solutions where needed

### [ ] Phase 4: Validation and Testing

- [ ] Task 4.1: Compilation Validation
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 320-340)
  - Verify project builds without errors or warnings
  - Check all StyleSystem classes and Theme V2 tokens resolve
  - Validate XAML compilation and resource binding

- [ ] Task 4.2: Theme Compatibility Testing
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 342-370)
  - Test light theme: All elements visible and readable
  - Test dark theme: All elements visible and readable
  - Verify smooth theme transitions and accessibility compliance

- [ ] Task 4.3: Business Logic Verification
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 372-400)
  - Test all MVVM bindings, commands, and event handlers
  - Verify manufacturing workflows (operations 90/100/110 if applicable)
  - Confirm user interactions and data handling work correctly

- [ ] Task 4.4: Visual Regression Testing
  - Details: .copilot-tracking/details/mtm-theme-style-refactor-details-template.md (Lines 402-430)
  - Compare before/after visual appearance
  - Verify layout consistency and component alignment
  - Validate enhanced manufacturing UI elements

## Dependencies

### StyleSystem Infrastructure
- **Complete 8-category StyleSystem** - Layout, Components, Modifiers, Context, Content, Status, Action, Manufacturing
- **StyleSystem.axaml Master File** - Centralized style aggregation and organization
- **Theme V2 Semantic Tokens** - Light/dark theme compatibility with manufacturing-optimized tokens

### Development Tools
- **.NET 8.0 SDK** - Project compilation and build requirements
- **Avalonia UI 11.3.4** - XAML rendering and UI framework support
- **MVVM Community Toolkit 8.3.2** - ViewModel and command binding support

### Manufacturing Domain Knowledge
- **MTM Business Rules** - Inventory operations, workflow states, manufacturing processes
- **User Experience Patterns** - Operator workflows, accessibility requirements, industrial UI standards

## Success Criteria

### Technical Compliance
- ✅ Zero hardcoded values in `{TARGET_FILE}.axaml`
- ✅ 100% StyleSystem class coverage for all styling
- ✅ 100% Theme V2 token usage for all colors and values
- ✅ Perfect compilation without errors or warnings
- ✅ ScrollViewer policy compliance verified

### Functional Preservation
- ✅ All business logic preserved and operational
- ✅ All MVVM bindings functional and responsive
- ✅ Manufacturing workflows working correctly
- ✅ Parent container compatibility maintained
- ✅ User interactions preserved and enhanced

### Quality Enhancement
- ✅ Perfect light/dark theme compatibility
- ✅ Professional MTM manufacturing interface appearance
- ✅ WCAG 2.1 AA accessibility compliance
- ✅ Maintainable codebase with StyleSystem patterns
- ✅ Enhanced visual consistency with established design system
