---
mode: agent
model: Claude Sonnet 4
---
<!-- markdownlint-disable-file -->
# MTM Style System Single-File Transformation

**USAGE**: `#file:mtm-theme-style-refactor-prompt-template.md {TARGET_FILE}.axaml`

## Objective

Transform `{TARGET_FILE}.axaml` to use enhanced Theme V2 + StyleSystem implementation through complete file recreation, eliminating all hardcoded styling while maintaining business logic and ensuring proper parent container compatibility. Use comprehensive research from `#file:mtm-theme-style-refactor-research-template.md` customized for this specific file.

**CRITICAL**: If any ScrollViewer is needed in `{TARGET_FILE}` beyond approved locations, STOP and request approval first.

**Single-File Focus**: Transform `{TARGET_FILE}` completely before considering any other files.

**Template Files Used:**
- Research: `#file:mtm-theme-style-refactor-research-template.md`
- Planning: `#file:mtm-theme-style-refactor-plans-template.md`
- Implementation: `#file:mtm-theme-style-refactor-details-template.md`
- Changes Tracking: `#file:mtm-theme-style-refactor-changes-template.md`

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

## Three-Phase Execution Workflow

### Phase 1: Research (`{task-researcher}`)

**MANDATORY FIRST STEP**: Create comprehensive research file for `{TARGET_FILE}.axaml`

1. **Create Research File**: Generate `YYYYMMDD-{target-file-name}-style-transformation-research.md` using `#file:mtm-theme-style-refactor-research-template.md`
2. **Analyze Target File**: Replace all `{TEMPLATE_VARIABLES}` with actual analysis of `{TARGET_FILE}.axaml`
3. **Identify Dependencies**: Document required StyleSystem components and Theme V2 tokens
4. **Document Business Logic**: Catalog all MVVM bindings, commands, and business functionality to preserve

### Phase 2: Planning (`{task-planner}`)

**Create Implementation Plan**: Generate detailed transformation plan based on research findings

1. **Plan Missing Components**: Define exact StyleSystem components to create
2. **Plan Token Requirements**: Define exact Theme V2 tokens to add
3. **Plan Transformation Steps**: Step-by-step file transformation approach
4. **Plan Validation Process**: Testing and verification procedures

### Phase 3: Implementation (`{expert-dotnet-software-engineer}`)

**Execute Transformation**: Follow detailed plan with safety measures

## Transformation Rules for `{TARGET_FILE}.axaml`

### CRITICAL Rules

1. **Parent Container Compatibility**: All content MUST fit within parent calling container without overflow
2. **ScrollViewer Policy**:
   - ✅ **APPROVED**: QuickButtonsView.axaml transaction history panel only
   - ✅ **APPROVED**: DataGrid components only (for large data sets)
   - ❌ **STOP & ASK**: All other ScrollViewer usage requires explicit approval first
3. **Universal Theme Compatibility**: ALL UI elements MUST be visible and functional in both light and dark themes
4. **Business Logic Preservation**: Maintain all bindings, commands, behaviors, and business functionality
5. **StyleSystem Exclusive**: Use ONLY StyleSystem.axaml classes and Theme V2 semantic tokens - zero hardcoded values

### Transformation Process for `{TARGET_FILE}`

**CRITICAL PRE-IMPLEMENTATION STEP**: Before any AXAML transformation work begins, the agent MUST first implement any missing StyleSystem styles and Theme V2 tokens identified in the research phase.

1. **Read Research File**: Use findings from `YYYYMMDD-{target-file-name}-style-transformation-research.md`
2. **Implement Missing Styles**: Create any missing StyleSystem styles and Theme V2 tokens BEFORE file transformation
3. **Validate Style System**: Ensure all required styles compile without errors
4. **Backup Original**: Create `{TARGET_FILE}.axaml.backup` file for safety
5. **Delete Original**: Remove original AXAML file completely  
6. **Recreate with Full Implementation**: Build new file using ONLY Theme V2 + StyleSystem patterns from research

## Single-File Execution Approach

**This prompt transforms ONE AXAML file at a time**: `{TARGET_FILE}.axaml`

### File Types and Known Dependencies

**High-Priority Files (Ready for Transformation)**:
- **MainView.axaml** - Master tab container and navigation
- **InventoryTabView.axaml** - Primary inventory management interface  
- **QuickButtonsView.axaml** - Manufacturing side panel (ScrollViewer approved for history only)
- **ProgressOverlayView.axaml** - Progress indication overlay
- **ThemeQuickSwitcher.axaml** - Theme system overlay component

**Discovery Required Files**:
- **RemoveTabView.axaml**, **TransferTabView.axaml** - Manufacturing operation interfaces
- **AdvancedInventoryView.axaml**, **AdvancedRemoveView.axaml** - Advanced panels
- **SuggestionOverlayView.axaml**, **SuccessOverlayView.axaml** - Overlay dependencies
- **CollapsiblePanel.axaml** - Custom control component

### Pre-Transformation Analysis for `{TARGET_FILE}`

**Research Requirements**:
1. **File Discovery**: Locate `{TARGET_FILE}.axaml` in the Views hierarchy
2. **Business Function**: Understand manufacturing workflow purpose
3. **MVVM Analysis**: Catalog ViewModel dependencies and bindings
4. **Style Dependencies**: Identify required StyleSystem components and Theme V2 tokens
5. **ScrollViewer Compliance**: Verify usage aligns with approved policy

## Desktop Manufacturing UI Enhancement for `{TARGET_FILE}`

### Manufacturing Context Integration

**File-Specific Manufacturing Requirements** (Based on research findings):

- **Desktop-First Design**: Mouse/keyboard optimized interactions for `{TARGET_FILE}`
- **Manufacturing Status Visualization**: Apply operation-specific tokens (90/100/110)
- **Information Density**: Optimize typography hierarchy for data display
- **Industrial Accessibility**: WCAG AAA (7:1 contrast) compliance
- **Operational Efficiency**: Enhanced visual feedback and component variants

### Required StyleSystem Components for `{TARGET_FILE}`

**Manufacturing Status Indicators** (Create if missing):
```xml
<!-- ✅ REQUIRED: Manufacturing operation status for {TARGET_FILE} -->
<Border Classes="StatusCard Operation90" IsVisible="{Binding IsAtOperation90}">
    <TextBlock Classes="StatusText" Text="Move Operation (90)"/>
</Border>

<Border Classes="StatusCard Operation100" IsVisible="{Binding IsAtOperation100}">
    <TextBlock Classes="StatusText" Text="Receive Operation (100)"/>
</Border>
```

**Data Display Typography** (Create if missing):
```xml
<!-- ✅ REQUIRED: Manufacturing data display for {TARGET_FILE} -->
<TextBlock Classes="DataLabel" Text="Part ID:"/>
<TextBlock Classes="DataValue" Text="{Binding PartId}"/>
<TextBlock Classes="DataQuantity" Text="{Binding Quantity}"/>
```

**Interactive Components** (Use existing or create):
```xml
<!-- ✅ REQUIRED: Enhanced interactive feedback for {TARGET_FILE} -->
<Button Classes="ActionPrimary ManufacturingSave" Content="Process Operation" 
        Command="{Binding ProcessCommand}"/>
<Button Classes="ActionSecondary" Content="Cancel" 
        Command="{Binding CancelCommand}"/>
```

### CRITICAL: Style Implementation Requirements for `{TARGET_FILE}`

**Before transforming `{TARGET_FILE}.axaml`, the agent MUST implement missing StyleSystem components identified in the research phase.**

#### Research-Driven Component Creation

**Based on `YYYYMMDD-{target-file-name}-style-transformation-research.md` findings:**

1. **Missing StyleSystem Components** (Create only if needed for `{TARGET_FILE}`):
   - StatusCards.axaml for manufacturing status indicators
   - DataDisplayStyles.axaml for enhanced typography
   - Additional component files as identified in research

2. **Missing Theme V2 Tokens** (Add only if needed for `{TARGET_FILE}`):
   - Manufacturing operation tokens (Operation90/100/110)
   - File-specific semantic tokens
   - Enhanced color tokens as identified in research

3. **StyleSystem.axaml Updates** (Update includes):
   - Add new component file references
   - Ensure proper loading order
   - Validate no circular dependencies

#### Example Component Templates (Use if needed)

**Manufacturing Status Styles** (Create if `{TARGET_FILE}` requires):
```xml
<!-- Resources/Styles/Layout/StatusCards.axaml -->
<Style Selector="Border.StatusCard">
  <Setter Property="Background" Value="{DynamicResource ThemeV2.Background.Card}"/>
  <Setter Property="BorderBrush" Value="{DynamicResource ThemeV2.Border.Default}"/>
  <Setter Property="BorderThickness" Value="1"/>
  <Setter Property="CornerRadius" Value="8"/>
  <Setter Property="Padding" Value="12"/>
</Style>
```

**Theme V2 Tokens** (Add if `{TARGET_FILE}` requires):
```xml
<!-- Resources/ThemesV2/Theme.Light.axaml and Theme.Dark.axaml -->
<SolidColorBrush x:Key="ThemeV2.Status.Operation90" Color="{StaticResource ThemeV2.Color.Blue.500}"/>
<SolidColorBrush x:Key="ThemeV2.Status.Operation100" Color="{StaticResource ThemeV2.Color.Green.500}"/>
```

## Implementation Standards for `{TARGET_FILE}.axaml`

### Required Transformation Patterns

**Theme V2 + StyleSystem Implementation** (Apply to `{TARGET_FILE}`):

```xml
<!-- ✅ CORRECT: Enhanced StyleSystem implementation for {TARGET_FILE} -->
<Button Classes="ActionPrimary" Content="Save Changes"/>
<Border Classes="Card Elevated ManufacturingPanel">
    <TextBlock Classes="Heading2 HighContrast" Text="Operation Status"/>
    <TextBlock Classes="DataLabel" Text="Current Operation:"/>
    <TextBlock Classes="DataValue OperationStatus" Text="{Binding CurrentOperation}"/>
</Border>

<!-- ✅ CORRECT: Manufacturing data optimization for {TARGET_FILE} -->
<DataGrid Classes="ManufacturingData" ItemsSource="{Binding InventoryItems}">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Part ID" Binding="{Binding PartId}" 
                           HeaderStyle="{StaticResource DataGridColumnHeader}"/>
    </DataGrid.Columns>
</DataGrid>

<!-- ❌ WRONG: Any hardcoded values in {TARGET_FILE} -->
<Button Background="#2196F3" Content="Save"/>
<Border Background="White" BorderBrush="#E0E0E0">
```

### File-Specific UI Standards for `{TARGET_FILE}`

**Apply based on research findings:**
- **Status Indicators**: Use semantic tokens for operation states (Operation90/100/110) if applicable
- **Typography Scale**: DataLabel (12px Medium), DataValue (14px SemiBold), Heading hierarchy
- **Interactive Elements**: 200ms hover transitions, clear focus indicators, 32px+ click targets  
- **Component Variants**: ActionPrimary/Secondary, ManufacturingPanel, StatusCard classes as needed
- **Data Display**: Enhanced DataGrid styling if `{TARGET_FILE}` contains data tables

### ScrollViewer Usage Rules for `{TARGET_FILE}`

**Verify compliance during research phase:**

```xml
<!-- ✅ APPROVED: QuickButtonsView history only (if {TARGET_FILE} is QuickButtonsView) -->
<ScrollViewer IsVisible="{Binding IsShowingHistory}"
              VerticalScrollBarVisibility="Auto"
              HorizontalScrollBarVisibility="Disabled">
    <!-- Transaction history content -->
</ScrollViewer>

<!-- ✅ APPROVED: DataGrid scrolling (if {TARGET_FILE} contains DataGrid) -->
<DataGrid ItemsSource="{Binding Items}"
          ScrollViewer.VerticalScrollBarVisibility="Auto">
</DataGrid>

<!-- ❌ STOP & ASK: Any other ScrollViewer usage in {TARGET_FILE} -->
<ScrollViewer> <!-- Must request approval first -->
```

### Container Compatibility Pattern for `{TARGET_FILE}`

**Ensure proper parent container fitting:**

```xml
<!-- ✅ CORRECT: Proper parent fitting for {TARGET_FILE} -->
<Grid RowDefinitions="*,Auto" 
      Background="{DynamicResource ThemeV2.Background.Canvas}"
      HorizontalAlignment="Stretch" 
      VerticalAlignment="Stretch">
      
<!-- ❌ WRONG: Fixed sizes that might overflow parent -->
<Grid Width="800" Height="600">
```

### Light/Dark Theme Validation for `{TARGET_FILE}`

**Every element in `{TARGET_FILE}` must pass both theme tests:**

```xml
<!-- ✅ CORRECT: Adaptive theming in {TARGET_FILE} -->
<TextBlock Foreground="{DynamicResource ThemeV2.Content.Primary}"/>
<Border Background="{DynamicResource ThemeV2.Background.Card}"/>

<!-- ❌ WRONG: Fixed colors that break in dark mode -->
<TextBlock Foreground="Black"/>
<Border Background="White"/>
```

## Business Logic Preservation for `{TARGET_FILE}.axaml`

### Required Business Logic Elements to Preserve

**Critical elements that MUST be preserved in `{TARGET_FILE}`:**
- All DataContext bindings (`x:DataType`, `x:CompileBindings`)
- Command bindings (`Command="{Binding SomeCommand}"`)
- Property bindings (`Text="{Binding PropertyName}"`)
- Event handlers and behaviors
- Converter usage and resources
- Input validation and error handling
- Navigation and overlay management

### Manufacturing Domain Context for `{TARGET_FILE}`

**File-specific manufacturing requirements** (Document in research phase):
- Inventory transaction types (IN/OUT/TRANSFER) if applicable
- Operation workflow steps (90/100/110) if applicable
- Quick button functionality if `{TARGET_FILE}` is QuickButtonsView
- Master data integration (parts, locations, operations) if applicable
- Session management and timeouts if applicable
- Manufacturing operator workflows specific to `{TARGET_FILE}`

## ScrollViewer Approval Process for `{TARGET_FILE}`

**If ANY ScrollViewer is found in `{TARGET_FILE}` beyond approved locations:**

1. **STOP EXECUTION** immediately
2. Document exact location in `{TARGET_FILE}` and parent container
3. Explain why scrolling is necessary for UX in this specific file
4. Confirm no alternative layout solution exists for `{TARGET_FILE}`
5. Request explicit approval from user
6. Wait for confirmation before proceeding

**Pre-Approved ScrollViewer Locations for `{TARGET_FILE}`:**
- ✅ **If `{TARGET_FILE}` is QuickButtonsView.axaml**: Transaction history panel only  
- ✅ **If `{TARGET_FILE}` contains DataGrid**: For large data set navigation
- ❌ **All other cases**: Require explicit approval

## Validation Checklist for `{TARGET_FILE}.axaml`

### Pre-Transformation Checklist

- [ ] Research file created: `YYYYMMDD-{target-file-name}-style-transformation-research.md`
- [ ] Business logic documented and understood for `{TARGET_FILE}`
- [ ] All bindings and commands cataloged for `{TARGET_FILE}`
- [ ] Parent container constraints identified for `{TARGET_FILE}`
- [ ] ScrollViewer usage reviewed against policy for `{TARGET_FILE}`
- [ ] Theme compatibility requirements noted for `{TARGET_FILE}`
- [ ] Missing StyleSystem components identified for `{TARGET_FILE}`
- [ ] Missing Theme V2 tokens identified for `{TARGET_FILE}`

### Post-Transformation Checklist for `{TARGET_FILE}`

- [ ] All business logic preserved and functional in `{TARGET_FILE}`
- [ ] Zero hardcoded colors, fonts, or spacing values in `{TARGET_FILE}`
- [ ] Content fits properly in parent without overflow
- [ ] ScrollViewer usage follows approved policy only for `{TARGET_FILE}`
- [ ] Light theme: All elements visible and readable in `{TARGET_FILE}`
- [ ] Dark theme: All elements visible and readable in `{TARGET_FILE}`
- [ ] StyleSystem classes applied consistently throughout `{TARGET_FILE}`
- [ ] Theme V2 semantic tokens used exclusively in `{TARGET_FILE}`
- [ ] Manufacturing workflows operational for `{TARGET_FILE}` (if applicable)
- [ ] Backup file created: `{TARGET_FILE}.axaml.backup`

## Success Metrics for `{TARGET_FILE}.axaml`

### Quantitative Goals for `{TARGET_FILE}`

- **100% StyleSystem Coverage**: Zero local styling in `{TARGET_FILE}`, all StyleSystem classes
- **100% Theme V2 Compliance**: All colors via semantic tokens in `{TARGET_FILE}`
- **100% Business Logic Preservation**: All functionality maintained in `{TARGET_FILE}`
- **100% Theme Compatibility**: Perfect light/dark mode operation for `{TARGET_FILE}`
- **100% ScrollViewer Policy Compliance**: Only approved locations in `{TARGET_FILE}`

### Qualitative Goals for `{TARGET_FILE}`

- Professional MTM manufacturing interface appearance for `{TARGET_FILE}`
- Consistent visual language with existing StyleSystem patterns
- Responsive layout behavior without overflow issues in parent container
- Accessible WCAG 2.1 AA compliant design for `{TARGET_FILE}`
- Maintainable codebase with no hardcoded values in `{TARGET_FILE}`

## Execution Approach for `{TARGET_FILE}.axaml`

**Three-Phase Workflow Execution:**

### Step 1: Research Phase ({task-researcher})

1. **Create Research File**: Generate `YYYYMMDD-{target-file-name}-style-transformation-research.md`
2. **Analyze `{TARGET_FILE}`**: Read complete file and understand all business requirements
3. **Identify Dependencies**: Document required StyleSystem classes and Theme V2 tokens
4. **Check ScrollViewer Policy**: Verify compliance for `{TARGET_FILE}`
5. **Document Findings**: Complete all template variables in research file

### Step 2: Planning Phase ({task-planner})

1. **Create Implementation Plan**: Based on research findings for `{TARGET_FILE}`
2. **Plan Missing Components**: Define exact StyleSystem components to create
3. **Plan Token Requirements**: Define exact Theme V2 tokens to add
4. **Plan Transformation Steps**: Step-by-step approach for `{TARGET_FILE}`
5. **Plan Validation Process**: Testing procedures for `{TARGET_FILE}`

### Step 3: Implementation Phase ({expert-dotnet-software-engineer})

1. **IMPLEMENT MISSING STYLES FIRST** (Based on research):
   - Create missing StyleSystem component classes identified in research
   - Add missing Theme V2 semantic tokens identified in research
   - Update StyleSystem.axaml includes if new style files created
   - Validate all styles compile without errors

2. **Transform `{TARGET_FILE}`**:
   - Backup: Create `{TARGET_FILE}.axaml.backup` preserving original
   - Transform: Build new file using ONLY Theme V2 + StyleSystem implementation
   - Validate: Test both themes and verify business logic preservation
   - Document: Record changes and validate success metrics

**CRITICAL**: If any ScrollViewer is needed in `{TARGET_FILE}` beyond approved locations, STOP and request approval first.

**Single-File Focus**: Transform `{TARGET_FILE}` completely before considering any other files.

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Research Foundation:** Based on file-specific analysis using `#file:mtm-theme-style-refactor-research-template.md` customized for `{TARGET_FILE}.axaml`.
