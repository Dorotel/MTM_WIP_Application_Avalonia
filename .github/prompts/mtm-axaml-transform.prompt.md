---
title: "MTM AXAML StyleSystem Transformation"
description: "Transform AXAML files to use MTM Theme V2 + StyleSystem with complete documentation workflow"
version: "1.0"
usage: "#file:mtm-axaml-transform.prompt.md {TARGET_FILE}.axaml {ADDITIONAL_REQUIREMENTS}"
command: "/mtm-transform {TARGET_FILE}.axaml {ADDITIONAL_REQUIREMENTS}"
author: "MTM Development Team"
requires:
  - ".github/templates/mtm-transform-prompt/"
  - ".github/scripts/Start-MTMAXAMLTransform.ps1"
---

## MTM AXAML StyleSystem Transformation

**USAGE**:

- **Prompt Pattern**: `#file:mtm-axaml-transform.prompt.md {TARGET_FILE}.axaml {ADDITIONAL_REQUIREMENTS}`
- **PowerShell Script**: `.github/scripts/Start-MTMAXAMLTransform.ps1 "{TARGET_FILE}" "{ADDITIONAL_REQUIREMENTS}"`
- **Examples**:
  - `#file:mtm-axaml-transform.prompt.md RemoveTabView.axaml`
  - `#file:mtm-axaml-transform.prompt.md TransferTabView.axaml Remove CustomDataGrid and integrate EditInventoryView`
  - `.github/scripts/Start-MTMAXAMLTransform.ps1 "RemoveTabView" "Add column customization support"`

**Note**: When additional requirements are provided after the target file name, the agent will:

1. Display the additional requirements text exactly as provided
2. Show their interpretation of these requirements
3. Request explicit approval before proceeding with the transformation

## Objective

Transform `{TARGET_FILE}.axaml` to use enhanced Theme V2 + StyleSystem implementation through complete file recreation, eliminating all hardcoded styling while maintaining business logic and ensuring proper parent container compatibility.

**CRITICAL**: If any ScrollViewer is needed in `{TARGET_FILE}` beyond approved locations, STOP and request approval first.

**Single-File Focus**: Transform `{TARGET_FILE}` completely before considering any other files.

## Auto-Generated Documentation Files

This prompt automatically creates comprehensive tracking files:

- **Research**: `.copilot-tracking/research/{YYYYMMDD}-{target-file-name}-style-transformation-research.md`
- **Planning**: `.copilot-tracking/plans/{YYYYMMDD}-{target-file-name}-style-transformation-plans.md`
- **Implementation**: `.copilot-tracking/details/{YYYYMMDD}-{target-file-name}-style-transformation-details.md`
- **Changes**: `.copilot-tracking/changes/{YYYYMMDD}-{target-file-name}-style-transformation-changes.md`

## Workflow Phases

### Phase 1: Auto-Generate Documentation Files (MANDATORY FIRST STEP)

**CRITICAL**: If additional requirements are provided after the target file name, STOP and perform requirements confirmation:

1. **Requirements Confirmation** (when additional text follows target file):
   - **Read**: Display the additional requirements text exactly as provided
   - **Interpret**: Show how you understand these requirements in the context of the transformation
   - **Confirm**: Ask for explicit approval: "Do these interpretations align with your expectations? Should I proceed with the transformation using these requirements?"
   - **Wait**: Do not proceed until receiving explicit approval ("yes", "proceed", "correct", etc.)

2. **Documentation File Creation** (execute after requirements confirmation if applicable):
   - **Create Research File**: Generate `.copilot-tracking/research/{YYYYMMDD}-{target-file-name}-style-transformation-research.md` using template from `.github/templates/mtm-transform-prompt/research-template.md`
   - **Create Planning File**: Generate `.copilot-tracking/plans/{YYYYMMDD}-{target-file-name}-style-transformation-plans.md` using template from `.github/templates/mtm-transform-prompt/plans-template.md`
   - **Create Details File**: Generate `.copilot-tracking/details/{YYYYMMDD}-{target-file-name}-style-transformation-details.md` using template from `.github/templates/mtm-transform-prompt/details-template.md`
   - **Create Changes File**: Generate `.copilot-tracking/changes/{YYYYMMDD}-{target-file-name}-style-transformation-changes.md` using template from `.github/templates/mtm-transform-prompt/changes-template.md`
   - **Replace Template Variables**: Substitute all `{TARGET_FILE}` and other variables with actual values

### Phase 2: Research Analysis

1. **Analyze `{TARGET_FILE}`**: Read complete file and understand all business requirements
2. **Identify Dependencies**: Document required StyleSystem classes and Theme V2 tokens
3. **Check ScrollViewer Policy**: Verify compliance for `{TARGET_FILE}`
4. **Update Research File**: Complete all analysis in generated research file
5. **Validate Research**: Ensure all template sections are filled with actual data

### Phase 3: Implementation Planning

1. **Create Implementation Plan**: Based on research findings in generated research file
2. **Plan Missing Components**: Define exact StyleSystem components to create
3. **Plan Token Requirements**: Define exact Theme V2 tokens to add
4. **Update Plans File**: Complete detailed planning in generated plans file
5. **Update Details File**: Fill implementation details based on planning

### Phase 4: StyleSystem Implementation

1. **IMPLEMENT MISSING STYLES FIRST** (Based on research):
   - Create missing StyleSystem component classes identified in research
   - Add missing Theme V2 semantic tokens identified in research
   - Update StyleSystem.axaml includes if new style files created
   - Validate all styles compile without errors
   - Update Changes File with component creation progress

2. **Transform `{TARGET_FILE}`**:
   - Backup: Create `{TARGET_FILE}.axaml.backup` preserving original
   - Transform: Build new file using ONLY Theme V2 + StyleSystem implementation
   - Validate: Test both themes and verify business logic preservation
   - Update Changes File: Document all transformation steps and results
   - Complete Changes File: Add final success metrics and lessons learned

### Phase 5: Validation & Completion

1. **Build Validation**: Ensure transformation compiles successfully
2. **Theme Testing**: Verify both light and dark themes work correctly
3. **Business Logic**: Confirm all original functionality preserved
4. **Documentation**: Complete all tracking files with final results
5. **Success Metrics**: Document transformation success and lessons learned

## StyleSystem Requirements

### Mandatory StyleSystem Classes

Use ONLY these StyleSystem component classes:

- **Layout**: `ManufacturingTabView`, `Card`, `Card.Header`, `Card.Content`, `Card.Actions`
- **Forms**: `Form.Input`, `Form.Label`, `Form.Container`, `Form.Section`
- **Buttons**: `Button.Primary`, `Button.Secondary`, `Button.Action`, `Button.Label`
- **Icons**: `Icon.Button`, `Icon.Form`, `Icon.Status`
- **Loading**: `LoadingOverlay`, `LoadingSpinner`

### Theme V2 Semantic Tokens

Use ONLY Theme V2 semantic tokens:

- **Colors**: `{DynamicResource MTM_Shared_Logic.PrimaryBrush}`, `{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}`
- **Typography**: `{DynamicResource MTM_Shared_Logic.HeaderTextStyle}`, `{DynamicResource MTM_Shared_Logic.BodyTextStyle}`
- **Spacing**: `{DynamicResource MTM_Shared_Logic.StandardMargin}`, `{DynamicResource MTM_Shared_Logic.CardPadding}`

## Critical Requirements

### AXAML Syntax Rules (AVLN2000 Prevention)

- **NEVER use `Name` property on Grid definitions** - Use `x:Name` only
- **Use Avalonia namespace**: `xmlns="https://github.com/avaloniaui"`
- **Grid syntax**: Use `ColumnDefinitions="Auto,*"` attribute form when possible
- **Control equivalents**: Use `TextBlock` instead of `Label`, `Flyout` instead of `Popup`

### ScrollViewer Policy

**CRITICAL**: ScrollViewer usage is strictly controlled:

- **Approved locations**: Root container for tab views connecting to MainView.axaml
- **Prohibited**: Nested ScrollViewers, ScrollViewers around individual form sections
- **Alternative**: Use proper grid sizing and container management instead

### Business Logic Preservation

- **Data Binding**: Preserve all existing ViewModel bindings
- **Event Handlers**: Maintain all Click/Command handlers
- **Validation**: Keep input validation and error handling
- **State Management**: Preserve loading states and user interactions

## Success Criteria

✅ **Complete Documentation**: All 4 tracking files created and filled out
✅ **StyleSystem Implementation**: All hardcoded styles replaced with StyleSystem classes
✅ **Theme V2 Integration**: All colors/typography using semantic tokens
✅ **Business Logic Preserved**: All original functionality working
✅ **Build Success**: Clean compilation with no errors
✅ **Theme Compatibility**: Both light and dark themes working
✅ **AVLN2000 Compliance**: No Avalonia syntax errors

## Enhanced Automation

For streamlined execution, use the PowerShell guided assistant:

```powershell
# Basic transformation
.github/scripts/Start-MTMAXAMLTransform.ps1 "{TARGET_FILE}"

# With additional requirements
.github/scripts/Start-MTMAXAMLTransform.ps1 "{TARGET_FILE}" "{ADDITIONAL_REQUIREMENTS}"

# Examples
.github/scripts/Start-MTMAXAMLTransform.ps1 "RemoveTabView"
.github/scripts/Start-MTMAXAMLTransform.ps1 "TransferTabView" "Remove CustomDataGrid dependency"
```

This script provides interactive guidance through all phases with @Agent collaboration.

## Referenced Files

**Following MTM copilot-instructions.md** - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Template Files Used:**

- Research: `.github/templates/mtm-transform-prompt/research-template.md`
- Planning: `.github/templates/mtm-transform-prompt/plans-template.md`
- Implementation: `.github/templates/mtm-transform-prompt/details-template.md`
- Changes: `.github/templates/mtm-transform-prompt/changes-template.md`
