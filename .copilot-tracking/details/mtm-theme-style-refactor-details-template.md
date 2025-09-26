<!-- markdownlint-disable-file -->
# MTM Style System Transformation Implementation Details: {TARGET_FILE}.axaml

**TEMPLATE USAGE**: Replace `{TARGET_FILE}` with actual AXAML file name when using this template.

## Research Reference

**Source Research**: #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md

## Phase 1: Pre-Implementation Setup

### Task 1.1: Create Missing StyleSystem Components

**Objective**: Create required StyleSystem components identified in research phase before file transformation.

- **Components to Create**:
  - `{MISSING_COMPONENT_1}.axaml` - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}
  - `{MISSING_COMPONENT_2}.axaml` - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}
  - `{MISSING_COMPONENT_3}.axaml` - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}

- **Implementation Details**:
  ```xml
  <!-- Resources/Styles/{COMPONENT_CATEGORY}/{MISSING_COMPONENT_1}.axaml -->
  <Styles xmlns="https://github.com/avaloniaui"
          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      
      <!-- Component styles based on research requirements -->
      {COMPONENT_STYLES_IMPLEMENTATION}
      
  </Styles>
  ```

- **Success Criteria**:
  - All missing components created in correct StyleSystem categories
  - Components compile without errors  
  - Components follow established StyleSystem patterns
  - All required styles for `{TARGET_FILE}` are available

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Missing components analysis
  - #file:../../.github/instructions/style-system-implementation.instructions.md - StyleSystem patterns

- **Dependencies**:
  - Research phase completed with component requirements identified
  - Access to Resources/Styles/ directory structure

### Task 1.2: Add Missing Theme V2 Tokens

**Objective**: Add required Theme V2 semantic tokens to both light and dark theme files.

- **Tokens to Add**:
  - `{MISSING_TOKEN_1}` - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}
  - `{MISSING_TOKEN_2}` - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}

- **Implementation Details**:
  ```xml
  <!-- Resources/ThemesV2/Theme.Light.axaml -->
  <SolidColorBrush x:Key="{MISSING_TOKEN_1}" Color="{StaticResource {LIGHT_COLOR_REFERENCE}}"/>
  
  <!-- Resources/ThemesV2/Theme.Dark.axaml -->
  <SolidColorBrush x:Key="{MISSING_TOKEN_1}" Color="{StaticResource {DARK_COLOR_REFERENCE}}"/>
  ```

- **Success Criteria**:
  - All missing tokens added to both Light and Dark themes
  - Token naming follows Theme V2 semantic conventions
  - Colors provide adequate contrast for manufacturing environment
  - Perfect light/dark theme compatibility maintained

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Missing tokens analysis
  - #file:../../.github/instructions/theme-v2-implementation.instructions.md - Theme V2 patterns

- **Dependencies**:
  - Task 1.1 completion (components ready to use tokens)
  - Theme V2 infrastructure in place

### Task 1.3: Update StyleSystem.axaml Includes

**Objective**: Update master StyleSystem file to include new component files.

- **Updates Required**:
  - Add `<StyleInclude>` references for new component files
  - Maintain proper loading order to prevent circular dependencies
  - Validate all includes compile successfully

- **Implementation Details**:
  ```xml
  <!-- Resources/Styles/StyleSystem.axaml -->
  <!-- Add new includes in appropriate category sections -->
  <StyleInclude Source="avares://MTM_WIP_Application_Avalonia/Resources/Styles/{COMPONENT_CATEGORY}/{MISSING_COMPONENT_1}.axaml"/>
  ```

- **Success Criteria**:
  - All new component files properly included
  - StyleSystem.axaml compiles without errors
  - No circular dependency issues
  - All styles available for `{TARGET_FILE}` transformation

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Component integration requirements

- **Dependencies**:
  - Task 1.1 and 1.2 completion (all components and tokens created)

## Phase 2: File Analysis and Backup

### Task 2.1: Create File Backup

**Objective**: Create safety backup of original file before transformation.

- **Backup Process**:
  - Copy `{TARGET_FILE}.axaml` to `{TARGET_FILE}.axaml.backup`
  - Preserve all file metadata and timestamps
  - Verify backup integrity

- **Implementation Details**:
  ```powershell
  Copy-Item "{TARGET_FILE}.axaml" "{TARGET_FILE}.axaml.backup" -Force
  ```

- **Success Criteria**:
  - Backup file created successfully
  - Backup contains identical content to original
  - Backup is accessible for restoration if needed

- **Research References**:
  - Standard safety procedure for file transformation
  - No specific research lines (safety procedure)

- **Dependencies**:
  - Original file exists and is accessible
  - Write permissions to target directory

### Task 2.2: Document Current File State

**Objective**: Record baseline metrics and analyze current implementation patterns.

- **Metrics to Document**:
  - File size and line count
  - Number of hardcoded values found
  - Existing StyleSystem classes used
  - Current Theme V2 tokens used
  - MVVM bindings and business logic inventory

- **Analysis Details**:
  ```text
  File: {TARGET_FILE}.axaml
  Size: {ORIGINAL_FILE_SIZE} bytes
  Lines: {ORIGINAL_LINE_COUNT}
  Hardcoded Values: {COUNT_HARDCODED_VALUES}
  StyleSystem Classes: {COUNT_EXISTING_CLASSES}
  Theme V2 Tokens: {COUNT_EXISTING_TOKENS}
  ```

- **Success Criteria**:
  - Complete baseline documented
  - All hardcoded values cataloged for replacement
  - Business logic mapped for preservation
  - Current compliance level calculated

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Current state analysis

- **Dependencies**:
  - Research phase completed with current state analysis
  - Access to target file for analysis

## Phase 3: File Transformation

### Task 3.1: Replace Hardcoded Styles with StyleSystem Classes

**Objective**: Replace all hardcoded styling with appropriate StyleSystem classes.

- **Transformation Patterns**:
  ```xml
  <!-- BEFORE: Hardcoded styles -->
  {CURRENT_HARDCODED_PATTERN_1}
  
  <!-- AFTER: StyleSystem classes -->
  {REPLACEMENT_STYLESYSTEM_PATTERN_1}
  
  <!-- BEFORE: Another hardcoded pattern -->
  {CURRENT_HARDCODED_PATTERN_2}
  
  <!-- AFTER: StyleSystem replacement -->
  {REPLACEMENT_STYLESYSTEM_PATTERN_2}
  ```

- **StyleSystem Classes to Apply**:
  - **Layout Classes**: {SPECIFIC_LAYOUT_CLASSES_NEEDED}
  - **Component Classes**: {SPECIFIC_COMPONENT_CLASSES_NEEDED}
  - **Modifier Classes**: {SPECIFIC_MODIFIER_CLASSES_NEEDED}
  - **Context Classes**: {SPECIFIC_CONTEXT_CLASSES_NEEDED}
  - **Manufacturing Classes**: {SPECIFIC_MANUFACTURING_CLASSES_NEEDED}

- **Success Criteria**:
  - All hardcoded styles replaced with StyleSystem classes
  - Visual appearance maintained or improved
  - Classes applied consistently throughout file
  - No compilation errors introduced

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Transformation patterns
  - #file:../../.github/instructions/style-system-implementation.instructions.md - StyleSystem usage patterns

- **Dependencies**:
  - Phase 1 completion (all required StyleSystem components available)
  - File backup created (Task 2.1)

### Task 3.2: Replace Hardcoded Colors with Theme V2 Tokens

**Objective**: Replace all hardcoded colors and values with Theme V2 semantic tokens.

- **Token Replacement Patterns**:
  ```xml
  <!-- BEFORE: Hardcoded colors -->
  <Border Background="White" BorderBrush="#E0E0E0">
  
  <!-- AFTER: Theme V2 tokens -->
  <Border Background="{DynamicResource ThemeV2.Background.Card}"
          BorderBrush="{DynamicResource ThemeV2.Border.Default}">
  ```

- **Token Categories to Apply**:
  - **Background Tokens**: {SPECIFIC_BACKGROUND_TOKENS_NEEDED}
  - **Content Tokens**: {SPECIFIC_CONTENT_TOKENS_NEEDED}
  - **Action Tokens**: {SPECIFIC_ACTION_TOKENS_NEEDED}
  - **Status Tokens**: {SPECIFIC_STATUS_TOKENS_NEEDED}
  - **Manufacturing Tokens**: {SPECIFIC_MANUFACTURING_TOKENS_NEEDED}

- **Success Criteria**:
  - All hardcoded colors replaced with semantic tokens
  - Perfect light/dark theme compatibility achieved
  - Manufacturing accessibility requirements met
  - No visual regressions in either theme

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Theme V2 integration patterns
  - #file:../../.github/instructions/theme-v2-implementation.instructions.md - Theme V2 token usage

- **Dependencies**:
  - Task 3.1 completion (StyleSystem classes applied)
  - Phase 1 completion (all required tokens available)

### Task 3.3: Preserve Business Logic and MVVM Bindings

**Objective**: Ensure all business functionality is preserved during transformation.

- **Elements to Preserve**:
  - **DataContext Bindings**: {LIST_CRITICAL_BINDINGS}
  - **Command Bindings**: {LIST_CRITICAL_COMMANDS}
  - **Property Bindings**: {LIST_CRITICAL_PROPERTIES}
  - **Event Handlers**: {LIST_CRITICAL_EVENTS}
  - **Converters**: {LIST_CRITICAL_CONVERTERS}
  - **Behaviors**: {LIST_CRITICAL_BEHAVIORS}

- **Preservation Strategy**:
  ```xml
  <!-- CRITICAL: Preserve existing MVVM bindings -->
  {MVVM_BINDING_EXAMPLES_TO_PRESERVE}
  
  <!-- CRITICAL: Preserve event handlers -->
  {EVENT_HANDLER_EXAMPLES_TO_PRESERVE}
  ```

- **Success Criteria**:
  - All MVVM bindings functional
  - All commands working correctly
  - All event handlers preserved
  - Business logic unchanged
  - Manufacturing workflows operational

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Business logic preservation requirements

- **Dependencies**:
  - Task 3.2 completion (color/styling transformation complete)
  - Business logic inventory from Task 2.2

### Task 3.4: Validate ScrollViewer Policy Compliance

**Objective**: Ensure ScrollViewer usage complies with established policy.

- **Policy Validation**:
  - **Current Usage**: {SCROLLVIEWER_USAGE_ANALYSIS}
  - **Policy Status**: {COMPLIANT/NON_COMPLIANT/NOT_APPLICABLE}
  - **Required Action**: {ACTION_IF_NON_COMPLIANT}

- **Approved ScrollViewer Locations for `{TARGET_FILE}`**:
  - ✅ **If QuickButtonsView**: Transaction history panel only
  - ✅ **If contains DataGrid**: Large data set navigation
  - ❌ **All other cases**: Require explicit approval

- **Success Criteria**:
  - ScrollViewer usage follows approved policy
  - No unauthorized ScrollViewer elements
  - Alternative layout solutions implemented where needed
  - User approval obtained for any policy exceptions

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - ScrollViewer policy compliance

- **Dependencies**:
  - Policy understanding from research phase
  - User approval process if violations found

## Phase 4: Validation and Testing

### Task 4.1: Compilation Validation

**Objective**: Ensure transformed file compiles without errors.

- **Validation Steps**:
  - Build project with transformed file
  - Verify no XAML compilation errors
  - Check for missing resource references
  - Validate all bindings resolve correctly

- **Success Criteria**:
  - Project builds successfully
  - No compilation errors or warnings
  - All StyleSystem classes resolve
  - All Theme V2 tokens resolve

- **Research References**:
  - Standard compilation validation procedure

- **Dependencies**:
  - Phase 3 completion (file transformation complete)
  - All StyleSystem components and tokens available

### Task 4.2: Theme Compatibility Testing

**Objective**: Verify perfect light/dark theme functionality.

- **Testing Procedure**:
  - Test light theme: All elements visible and readable
  - Test dark theme: All elements visible and readable
  - Verify theme switching works smoothly
  - Check manufacturing accessibility requirements

- **Test Cases**:
  ```xml
  <!-- Light Theme Validation -->
  - Background colors appropriate for light mode
  - Text contrast meets WCAG AAA standards
  - Interactive elements clearly visible
  
  <!-- Dark Theme Validation -->
  - Background colors appropriate for dark mode
  - Text contrast meets WCAG AAA standards
  - No visual artifacts or invisible elements
  ```

- **Success Criteria**:
  - Perfect light theme functionality
  - Perfect dark theme functionality
  - Smooth theme transitions
  - Manufacturing environment compatibility

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Theme compatibility requirements

- **Dependencies**:
  - Task 4.1 completion (compilation successful)
  - Access to theme switching functionality

### Task 4.3: Business Logic Verification

**Objective**: Confirm all business functionality works correctly after transformation.

- **Verification Steps**:
  - Test all MVVM bindings
  - Verify all commands execute
  - Check all event handlers respond
  - Validate manufacturing workflows (if applicable)

- **Test Scenarios**:
  - **Data Binding**: {SPECIFIC_BINDING_TESTS_FOR_TARGET_FILE}
  - **User Interactions**: {SPECIFIC_INTERACTION_TESTS}
  - **Manufacturing Operations**: {SPECIFIC_MANUFACTURING_TESTS}
  - **Error Handling**: {SPECIFIC_ERROR_TESTS}

- **Success Criteria**:
  - All functionality preserved
  - No regression in business logic
  - Manufacturing operations work correctly
  - User experience maintained or improved

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Business logic requirements

- **Dependencies**:
  - Task 4.2 completion (theme testing successful)
  - Manufacturing domain knowledge for workflow testing

### Task 4.4: Visual Regression Testing

**Objective**: Ensure visual appearance is maintained or improved.

- **Visual Validation**:
  - Compare before/after screenshots
  - Verify layout consistency
  - Check component alignment
  - Validate manufacturing UI enhancements

- **Quality Checks**:
  - **Layout**: No overflow or clipping issues
  - **Spacing**: Consistent with StyleSystem patterns
  - **Typography**: Enhanced manufacturing data display
  - **Colors**: Improved semantic meaning

- **Success Criteria**:
  - Visual quality maintained or improved
  - No layout regressions
  - Enhanced manufacturing UI elements
  - Professional MTM interface appearance

- **Research References**:
  - #file:../research/YYYYMMDD-{target-file-name}-style-transformation-research.md (Lines {RESEARCH_LINE_START}-{RESEARCH_LINE_END}) - Visual enhancement requirements

- **Dependencies**:
  - Task 4.3 completion (business logic verified)
  - Visual comparison capabilities

## Dependencies Summary

### External Dependencies
- **StyleSystem Infrastructure**: Complete 8-category system available
- **Theme V2 System**: Light/dark themes with semantic tokens
- **MTM Manufacturing Context**: Business domain requirements
- **Avalonia UI Framework**: Version 11.3.4 patterns and capabilities

### Internal Dependencies
- **Research Phase**: Complete analysis of `{TARGET_FILE}` requirements
- **Planning Phase**: Detailed implementation strategy
- **User Approval**: For any ScrollViewer policy exceptions
- **Backup Strategy**: Safety measures for rollback if needed

## Success Criteria Summary

### Technical Success
- ✅ 100% StyleSystem class coverage
- ✅ 100% Theme V2 token usage
- ✅ Zero hardcoded values remaining
- ✅ Perfect compilation success
- ✅ Complete light/dark theme compatibility

### Functional Success  
- ✅ All business logic preserved
- ✅ All MVVM functionality operational
- ✅ Manufacturing workflows working
- ✅ ScrollViewer policy compliance
- ✅ Parent container compatibility

### Quality Success
- ✅ Professional MTM interface appearance
- ✅ Enhanced manufacturing UI elements
- ✅ WCAG 2.1 AA accessibility compliance
- ✅ Maintainable code with no hardcoded values
- ✅ Consistent visual language with StyleSystem patterns
