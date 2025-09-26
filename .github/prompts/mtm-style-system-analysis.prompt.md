# MTM Avalonia Style System Analysis & Enhancement Specification

You WILL conduct a comprehensive analysis of all AXAML files in the MTM WIP Application to systematically identify missing styles, reusable UI patterns, and optimization opportunities. You MUST transform the current mixed styling approach into a maintainable StyleSystem-based design system while preserving functionality.

## PHASE 1: ACCURATE FILE INVENTORY (MANDATORY FIRST STEP)

You MUST begin by establishing an accurate baseline using these exact commands and MUST resolve any discrepancies before proceeding:

### Complete File Count Verification

```bash
# Get complete AXAML file inventory across entire project
file_search --query "**/*.axaml" --maxResults 200

# Categorize by major folders
file_search --query "Views/**/*.axaml" --maxResults 100
file_search --query "Controls/**/*.axaml" --maxResults 50  
file_search --query "Resources/**/*.axaml" --maxResults 50
file_search --query "MainWindow.axaml" --maxResults 10
file_search --query "App.axaml" --maxResults 10
```

### File Count Validation Requirement

You MUST document the exact count and MUST address any discrepancy between stated counts and actual search results:

```markdown
## File Inventory Verification
**Total AXAML Files Found:** [EXACT count from search results]
**Breakdown:**
- Views/: [X] files
- Controls/: [X] files  
- Resources/: [X] files
- Root level: [X] files

**Discrepancy Resolution:** [If search results differ from user's stated count, explain the difference and proceed with actual count]
```

## PHASE 2: STYLESYSTEM BASELINE ASSESSMENT

You WILL establish current StyleSystem infrastructure:

```bash
# Read StyleSystem master file
read_file --filePath "Resources/Styles/StyleSystem.axaml"

# Catalog existing style components  
list_dir --path "Resources/Styles"

# Check Theme V2 integration
semantic_search --query "ThemeV2 semantic tokens DynamicResource StyleSystem"
```

### Required Baseline Documentation

```markdown
## Current StyleSystem Infrastructure Status
**Available Style Categories:**
- [✓] Buttons/: [List specific files found]
- [✓] Inputs/: [List specific files found]  
- [✓] Layout/: [List specific files found]
- [✓] Manufacturing/: [List specific files found]
- [✓] Navigation/: [List specific files found]
- [✓] Typography/: [List specific files found]

**StyleSystem Classes Available:** [Extract from StyleSystem.axaml analysis]
**Theme V2 Integration Status:** [Document semantic token usage patterns]
```

## PHASE 3: SYSTEMATIC PATTERN DETECTION

You WILL execute these searches in order and document ALL results:

### Critical Styling Issues Detection

```bash
# Find hardcoded styling (highest priority fixes)
grep_search --query 'Background="#|BorderBrush="#' --isRegexp true --includePattern "**/*.axaml" --maxResults 50

# Find hardcoded spacing (medium priority)  
grep_search --query 'Margin="|Padding="' --isRegexp true --includePattern "**/*.axaml" --maxResults 30
```

### StyleSystem vs Legacy Usage Analysis  

```bash
# Find legacy MTM classes (migration required)
grep_search --query 'Classes="mtm-' --isRegexp false --includePattern "**/*.axaml" --maxResults 25

# Find proper StyleSystem class usage (working examples)
grep_search --query 'Classes="Primary|Secondary|Card|Form\.|Icon"' --isRegexp true --includePattern "**/*.axaml" --maxResults 25

# Find missing class applications (elements that should have Classes)
grep_search --query '<Button(?!.*Classes=)' --isRegexp true --includePattern "**/*.axaml" --maxResults 20
```

### Repeated UI Structure Detection

```bash
# Manufacturing form patterns
grep_search --query 'PartID|Part ID.*TextBox|Operation.*TextBox' --isRegexp true --includePattern "Views/**/*.axaml" --maxResults 15

# Button toolbar patterns  
grep_search --query 'Search.*Delete.*Reset|Button.*Button.*Button.*Button' --isRegexp true --includePattern "Views/**/*.axaml" --maxResults 15
```

## PHASE 4: IMPACT ASSESSMENT & PRIORITIZATION

For each pattern identified, you MUST provide this specific analysis format:

### Missing Styles Report Format

```markdown
## [PRIORITY LEVEL]: [Style Name]
**Pattern Found:** `[Exact code pattern from search results]`
**Frequency:** [X] occurrences across [Y] files  
**Files Affected:** 
- `[exact file path]` line [number]: `[actual code snippet]`
- `[exact file path]` line [number]: `[actual code snippet]`

**Theme V2 Compliance:** [✓ Compliant | ⚠ Partial | ❌ Non-compliant]
**Recommended StyleSystem Location:** `Resources/Styles/[Category]/[FileName].axaml`
**Proposed Class Name:** `Classes="[ClassName]"`
**Estimated Impact:** [X] lines → [Y] lines ([Z]% reduction)
**Maintenance Benefit:** [Specific consistency/theme support gains]
```

### UserControl Consolidation Analysis Format

```markdown  
## [IMPACT LEVEL]: [UserControl Name]
**Repeated Structure Pattern:** [Description of UI pattern]
**Code Volume Analysis:**
- Lines per usage: ~[X] lines  
- Total files using pattern: [Y]
- Current total lines: [X×Y] lines
- Proposed reduction: [X×Y] → [Z] lines ([percentage]% reduction)

**Files Using Pattern:**
- `[exact file path]` lines [X-Y]: [brief description]
- `[exact file path]` lines [X-Y]: [brief description]

**Proposed UserControl:** `Controls/[ComponentName].axaml`
**MVVM Integration Requirements:**
- Bindable properties: [list required properties]
- Commands: [list required commands]
- Events: [list required events]

**Manufacturing Domain Validation:** [How this supports MTM workflows]
```

## PHASE 5: IMPLEMENTATION ROADMAP

You MUST provide measurable success criteria:

### Quantified Success Metrics

```markdown
## Implementation Impact Analysis

### Immediate Wins (Phase 1 - Week 1)
- **Code Reduction:** [X] lines eliminated across [Y] files
- **Consistency Gain:** [Z] components standardized to StyleSystem
- **Theme Compliance:** [A]% of files using Theme V2 tokens

### Developer Productivity (Phase 2 - Week 2-3)  
- **UserControl Consolidation:** [B] repeated patterns → [C] reusable components
- **Development Speed:** Estimated [D]% faster component creation
- **Maintenance Reduction:** [E] files requiring manual theme updates → [F] files

### Long-term Benefits (Phase 3 - Month 2)
- **Accessibility Compliance:** [G]% WCAG 2.1 AA compliant components
- **Cross-platform Consistency:** Standardized appearance across all platforms
- **Manufacturing Workflow Efficiency:** [H] standardized transaction UI patterns
```

### Validation Checklist

Before completing analysis, you MUST verify:

- [ ] **File count accuracy:** Search results documented and discrepancies explained
- [ ] **Pattern frequency:** All identified patterns include actual line numbers
- [ ] **Impact quantification:** Specific line count reductions calculated  
- [ ] **Priority justification:** Ranking based on user experience and maintenance impact
- [ ] **Manufacturing validation:** Domain-specific patterns identified and addressed
- [ ] **Theme V2 compliance:** Migration path documented for all non-compliant patterns

## SUCCESS CRITERIA VALIDATION

### Analysis Completeness (Required for Task Completion)

- **100% File Coverage:** All AXAML files searched and categorized
- **Specific Evidence:** Every recommendation backed by exact file paths and line numbers
- **Quantified Impact:** Measurable benefits for each proposed change
- **Implementation Feasibility:** All recommendations technically achievable within MTM architecture

### Quality Standards (Required for Acceptance)  

- **Accuracy:** No file references without verification
- **Prioritization:** Clear ranking by business impact and effort required  
- **Manufacturing Context:** MTM workflow patterns specifically addressed
- **Technical Compliance:** All recommendations follow MTM StyleSystem and Theme V2 patterns

## Examples

### Example Missing Style Analysis

```markdown
### CRITICAL: Warning Action Button Style
**Pattern Found:** `Background="#FF6B35" Foreground="White" CornerRadius="4" Padding="8,4" FontWeight="SemiBold"`
**Frequency:** 3 occurrences across 3 files
**Files Affected:** 
- `Views/MainForm/Panels/RemoveTabView.axaml` line 67: `<Button Background="#FF6B35" Foreground="White" CornerRadius="4">Delete</Button>`
- `Views/SettingsForm/AdvancedRemovalView.axaml` line 23: `<Button Background="#FF6B35" Foreground="White" CornerRadius="4">Remove</Button>`

**Theme V2 Compliance:** ❌ Non-compliant (hardcoded colors)
**Recommended StyleSystem Location:** `Resources/Styles/Buttons/Actions.axaml`
**Proposed Class Name:** `Classes="Danger"`
**Estimated Impact:** 45 lines → 9 lines (80% reduction)
**Maintenance Benefit:** Consistent warning styling, automatic theme support, accessibility compliance
```

### Example UserControl Candidate

```markdown
### HIGH IMPACT: Manufacturing Search Panel UserControl
**Repeated Structure Pattern:** Part ID TextBox + Operation TextBox + Search Button in horizontal layout
**Code Volume Analysis:**
- Lines per usage: ~45 lines  
- Total files using pattern: 3
- Current total lines: 135 lines
- Proposed reduction: 135 → 15 lines (89% reduction)

**Files Using Pattern:**
- `Views/MainForm/Panels/InventoryTabView.axaml` lines 115-160: Part ID + Operation fields with search functionality
- `Views/MainForm/Panels/RemoveTabView.axaml` lines 85-130: Similar search pattern for remove operations
- `Views/Overlay/EditInventoryView.axaml` lines 45-90: Edit form with same field structure

**Proposed UserControl:** `Controls/ManufacturingSearchPanel.axaml`
**MVVM Integration Requirements:**
- Bindable properties: PartId, Operation, PartWatermark, OperationWatermark
- Commands: SearchCommand, ClearCommand
- Events: PartChanged, OperationChanged

**Manufacturing Domain Validation:** Standardizes the core inventory search pattern used across all manufacturing workflows (Add, Remove, Edit operations)
```

## Notes

### Technical Requirements

- **Theme V2 Compliance**: All new styles MUST use `DynamicResource ThemeV2.*` semantic tokens
- **Legacy Migration**: Identify and flag any remaining `MTM_Shared_Logic` resource references
- **Code Quality**: Follow existing MTM patterns with minimal code-behind for UserControls
- **File Organization**: Place new styles in appropriate subfolder structure under Resources/Styles/

### Manufacturing Context Awareness

- Prioritize patterns specific to inventory management, transaction processing, and manufacturing workflows
- Consider the domain-specific nature of Part IDs, Operation numbers, and Location codes
- Maintain consistency with existing MTM business logic and user interaction patterns

### Validation Criteria

- Provide specific file paths and line numbers for all identified patterns
- Include frequency counts for each missing style or UserControl opportunity
- Estimate maintenance effort reduction from each proposed change
- Ensure all recommendations preserve existing functionality while improving consistency

**CRITICAL:** You MUST complete all phases in order and provide the specified documentation format. Do not proceed to the next phase until the current phase requirements are fully met and documented.
