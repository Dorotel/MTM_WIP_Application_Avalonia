<!-- markdownlint-disable-file -->
# MTM Style System Single-File Transformation Template

**TEMPLATE USAGE**: Replace `{TARGET_FILE}` with actual AXAML file name when using this template.

## Research Executed

### Target File Analysis: `{TARGET_FILE}.axaml`
- **File Location**: `{FULL_FILE_PATH}`
  - **Purpose**: {BUSINESS_FUNCTION_DESCRIPTION}
  - **Current StyleSystem Compliance**: {PERCENTAGE}% - {COMPLIANCE_DETAILS}
  - **Theme V2 Integration Status**: {INTEGRATION_STATUS}
  - **ScrollViewer Policy Compliance**: {SCROLLVIEWER_STATUS}
  - **Manufacturing Domain Requirements**: {DOMAIN_SPECIFIC_NEEDS}

### Code Search Results for `{TARGET_FILE}`
- **Current Styling Patterns Found**:
  - {PATTERN_1}: {USAGE_DETAILS}
  - {PATTERN_2}: {USAGE_DETAILS}
  - {HARDCODED_STYLES_FOUND}: {SPECIFIC_INSTANCES}

- **MVVM Bindings Analysis**:
  - {BINDING_PATTERN_1}: {PRESERVATION_REQUIREMENTS}
  - {EVENT_HANDLERS}: {PRESERVATION_REQUIREMENTS}
  - {BUSINESS_LOGIC_DEPENDENCIES}: {PRESERVATION_REQUIREMENTS}

### StyleSystem Component Dependencies
- **Required Components (Existing)**:
  - ✅ {EXISTING_COMPONENT_1}: Available in StyleSystem
  - ✅ {EXISTING_COMPONENT_2}: Available in StyleSystem

- **Required Components (Missing)**:
  - ❌ {MISSING_COMPONENT_1}: {CREATION_REQUIREMENTS}
  - ❌ {MISSING_COMPONENT_2}: {CREATION_REQUIREMENTS}

### Theme V2 Token Requirements
- **Required Tokens (Available)**:
  - ✅ {EXISTING_TOKEN_1}: {USAGE_CONTEXT}
  - ✅ {EXISTING_TOKEN_2}: {USAGE_CONTEXT}

- **Required Tokens (Missing)**:
  - ❌ {MISSING_TOKEN_1}: {CREATION_REQUIREMENTS}
  - ❌ {MISSING_TOKEN_2}: {CREATION_REQUIREMENTS}

### External Research for `{TARGET_FILE}` Context
- **#githubRepo:"avaloniaui/avalonia" {SEARCH_TERMS_FOR_FILE_TYPE}**
  - {RELEVANT_PATTERN_FOUND}
  - {IMPLEMENTATION_GUIDANCE}

- **#fetch:{RELEVANT_DOCUMENTATION_URL}**
  - {KEY_GUIDANCE_FOR_FILE_TYPE}
  - {BEST_PRACTICES_APPLICABLE}

### Project Conventions Applied
- **Standards referenced**: {INSTRUCTION_FILES_USED}
- **Instructions followed**: {SPECIFIC_GUIDELINES_APPLIED}
- **Architecture patterns**: {PATTERNS_RELEVANT_TO_TARGET_FILE}

## Key Discoveries for `{TARGET_FILE}.axaml`

### Current State Analysis

**File Structure and Purpose:**
- **Component Type**: {UI_COMPONENT_TYPE} (View/UserControl/CustomControl/Overlay)
- **Business Function**: {SPECIFIC_BUSINESS_PURPOSE}
- **Parent Dependencies**: {PARENT_CONTAINERS_OR_DEPENDENCIES}
- **Child Components**: {CHILD_CONTROLS_OR_DEPENDENCIES}

**StyleSystem Compliance Assessment:**
- **Current Compliance Level**: {PERCENTAGE}%
- **StyleSystem Classes Already Used**: 
  - ✅ {EXISTING_CLASS_1}: {USAGE_CONTEXT}
  - ✅ {EXISTING_CLASS_2}: {USAGE_CONTEXT}
- **Hardcoded Styles Found**:
  - ❌ {HARDCODED_STYLE_1}: {SPECIFIC_LOCATION_AND_VALUE}
  - ❌ {HARDCODED_STYLE_2}: {SPECIFIC_LOCATION_AND_VALUE}

**Theme V2 Integration Status:**
- **Theme V2 Tokens Already Used**:
  - ✅ {EXISTING_TOKEN_1}: {USAGE_CONTEXT}
  - ✅ {EXISTING_TOKEN_2}: {USAGE_CONTEXT}
- **Hardcoded Colors/Values Found**:
  - ❌ {HARDCODED_VALUE_1}: {SPECIFIC_LOCATION_AND_VALUE}
  - ❌ {HARDCODED_VALUE_2}: {SPECIFIC_LOCATION_AND_VALUE}

### Missing Components Analysis

**StyleSystem Components Needed:**
```text
{REQUIRED_COMPONENT_CATEGORY}/
├── {REQUIRED_COMPONENT_1}.axaml ❌ (Must create)
│   └── Required for: {USAGE_REQUIREMENT}
├── {REQUIRED_COMPONENT_2}.axaml ❌ (Must create)  
│   └── Required for: {USAGE_REQUIREMENT}
└── {EXISTING_COMPONENT}.axaml ✅ (Available)
```

**Theme V2 Tokens Needed:**
- **Missing Color Tokens**: {MISSING_COLOR_TOKENS_LIST}
- **Missing Semantic Tokens**: {MISSING_SEMANTIC_TOKENS_LIST}
- **Missing Typography Tokens**: {MISSING_TYPOGRAPHY_TOKENS_LIST}

### Implementation Patterns for `{TARGET_FILE}.axaml`

**Current Patterns Found in File:**
```xml
<!-- ✅ EXISTING: Pattern currently used in {TARGET_FILE} -->
{EXISTING_PATTERN_EXAMPLE_FROM_FILE}

<!-- ✅ EXISTING: Another pattern from {TARGET_FILE} -->
{ANOTHER_EXISTING_PATTERN_EXAMPLE}
```

**Required Transformation Patterns:**
```xml
<!-- 🔄 TRANSFORM: Replace hardcoded styles with StyleSystem classes -->
<!-- BEFORE (current): -->
{CURRENT_HARDCODED_PATTERN}

<!-- AFTER (required): -->
{REPLACEMENT_STYLESYSTEM_PATTERN}
```

**New Patterns Needed:**
```xml
<!-- ❌ NEW: Missing patterns required for {TARGET_FILE} -->
{NEW_PATTERN_1_REQUIRED}

{NEW_PATTERN_2_REQUIRED}
```

### Complete Examples for `{TARGET_FILE}` Transformation

**File-Specific StyleSystem Implementation:**
```xml
<!-- Specific to {TARGET_FILE} requirements -->
{COMPLETE_EXAMPLE_RELEVANT_TO_TARGET_FILE}
```

**Business Logic Preservation:**
```xml
<!-- CRITICAL: Preserve existing MVVM bindings -->
{MVVM_BINDING_EXAMPLES_TO_PRESERVE}

<!-- CRITICAL: Preserve event handlers -->
{EVENT_HANDLER_EXAMPLES_TO_PRESERVE}
```

### Target File API and Integration Points

**MVVM Dependencies:**
- **ViewModel**: `{VIEWMODEL_CLASS_NAME}`
- **Key Properties**: {KEY_PROPERTIES_TO_PRESERVE}
- **Commands**: {COMMANDS_TO_PRESERVE}
- **Event Handlers**: {EVENT_HANDLERS_TO_PRESERVE}

**Parent/Child Integration:**
- **Parent Container**: {PARENT_CONTAINER_DETAILS}
- **Child Controls**: {CHILD_CONTROL_DEPENDENCIES}
- **Service Dependencies**: {SERVICE_DEPENDENCIES}

### ScrollViewer Policy Compliance for `{TARGET_FILE}`

**Current ScrollViewer Usage:**
- **Status**: {COMPLIANT/NON_COMPLIANT/NOT_APPLICABLE}
- **Usage Details**: {SCROLLVIEWER_USAGE_ANALYSIS}
- **Required Action**: {ACTION_IF_NON_COMPLIANT}

### Manufacturing Domain Requirements for `{TARGET_FILE}`

**Specific Manufacturing Context:**
- **Operation Types Supported**: {OPERATION_TYPES_90_100_110}
- **Data Types Handled**: {PART_ID_QUANTITY_LOCATION_ETC}
- **User Workflow Integration**: {USER_WORKFLOW_CONTEXT}
- **Industrial UI Requirements**: {ACCESSIBILITY_CONTRAST_REQUIREMENTS}

## Recommended Approach for `{TARGET_FILE}.axaml`

### Pre-Implementation Requirements

**Missing StyleSystem Components to Create:**
1. **{MISSING_COMPONENT_1}** - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}
2. **{MISSING_COMPONENT_2}** - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}
3. **{MISSING_COMPONENT_3}** - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}

**Missing Theme V2 Tokens to Add:**
1. **{MISSING_TOKEN_1}** - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}
2. **{MISSING_TOKEN_2}** - Required for {SPECIFIC_USAGE_IN_TARGET_FILE}

### Transformation Strategy for `{TARGET_FILE}`

**Phase 1: Component Preparation**
- Create missing StyleSystem components identified above
- Add missing Theme V2 tokens to both Light and Dark themes
- Update StyleSystem.axaml to include new components
- Validate compilation with new components

**Phase 2: File Backup and Analysis**
- Create backup: `{TARGET_FILE}.axaml.backup`
- Document current file size and complexity metrics
- Identify all hardcoded values for replacement
- Map MVVM bindings and event handlers to preserve

**Phase 3: Complete File Transformation**
- Replace ALL hardcoded styles with StyleSystem classes
- Replace ALL hardcoded colors/values with Theme V2 tokens
- Preserve ALL MVVM bindings and business logic
- Maintain ScrollViewer policy compliance
- Ensure manufacturing domain requirements are met

**Phase 4: Validation and Testing**
- Compile and validate no build errors
- Test light/dark theme switching
- Verify all MVVM functionality preserved
- Validate visual consistency with StyleSystem patterns
- Test manufacturing workflow integration (if applicable)

## Implementation Guidance for `{TARGET_FILE}.axaml`

### Transformation Objectives

**Primary Goal**: Transform `{TARGET_FILE}.axaml` to use 100% Theme V2 + StyleSystem patterns while preserving all business logic and MVVM functionality.

### File-Specific Implementation Plan

**Step 1: Pre-Transformation Setup**
- ✅ Create missing StyleSystem components: {LIST_MISSING_COMPONENTS}
- ✅ Add missing Theme V2 tokens: {LIST_MISSING_TOKENS}
- ✅ Update StyleSystem.axaml includes
- ✅ Backup original file: `{TARGET_FILE}.axaml.backup`

**Step 2: Hardcoded Value Replacement**
- 🔄 Replace hardcoded styles: {COUNT} instances found
- 🔄 Replace hardcoded colors: {COUNT} instances found
- 🔄 Replace hardcoded dimensions: {COUNT} instances found
- 🔄 Replace hardcoded typography: {COUNT} instances found

**Step 3: StyleSystem Class Application**
- 🔄 Apply layout classes: {SPECIFIC_CLASSES_NEEDED}
- 🔄 Apply component classes: {SPECIFIC_CLASSES_NEEDED}
- 🔄 Apply modifier classes: {SPECIFIC_CLASSES_NEEDED}
- 🔄 Apply context classes: {SPECIFIC_CLASSES_NEEDED}

**Step 4: MVVM Preservation Verification**
- ✅ Preserve bindings: {LIST_CRITICAL_BINDINGS}
- ✅ Preserve commands: {LIST_CRITICAL_COMMANDS}
- ✅ Preserve event handlers: {LIST_CRITICAL_EVENTS}
- ✅ Preserve business logic: {LIST_CRITICAL_LOGIC}

### File-Specific Success Criteria

**Functional Requirements:**
- ✅ All existing functionality preserved
- ✅ All MVVM bindings operational
- ✅ All user interactions working
- ✅ {FILE_SPECIFIC_REQUIREMENT_1}
- ✅ {FILE_SPECIFIC_REQUIREMENT_2}

**Styling Requirements:**
- ✅ Zero hardcoded styles remaining
- ✅ Zero hardcoded colors/values remaining
- ✅ Perfect light/dark theme compatibility
- ✅ Consistent with existing StyleSystem patterns
- ✅ {MANUFACTURING_SPECIFIC_REQUIREMENT}

**Quality Gates:**
- ✅ Compilation successful (no build errors)
- ✅ Runtime testing passed (no exceptions)
- ✅ Theme switching works perfectly
- ✅ Visual regression testing passed
- ✅ Manufacturing workflow integration verified (if applicable)

### Rollback Plan

**If transformation fails:**
1. Restore from backup: `{TARGET_FILE}.axaml.backup`
2. Document failure reasons in research notes
3. Identify missing prerequisites
4. Plan corrective actions for retry

### Next Steps After Successful Transformation

**Documentation:**
- Update transformation log with success metrics
- Document lessons learned for future files
- Update StyleSystem usage patterns if new patterns discovered

**Validation:**
- Add to validated files list
- Update overall project StyleSystem compliance percentage
- Prepare for next file transformation
