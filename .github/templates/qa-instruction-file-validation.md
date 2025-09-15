---
name: Instruction File Validation
description: 'Quality assurance checklist for MTM instruction file accuracy, completeness, and GitHub Copilot optimization'
applies_to: '**/*.instructions.md'
manufacturing_context: true
review_type: 'documentation'
quality_gate: 'important'
---

# Instruction File Validation - Quality Assurance Checklist

## Context
- **Component Type**: Instruction Documentation (GitHub Copilot Instructions, Implementation Guides)
- **Manufacturing Domain**: MTM inventory management instruction accuracy and AI optimization
- **Quality Gate**: Pre-merge validation for accurate AI-assisted development guidance

## Instruction File Content Validation

### Technology Accuracy Validation
- [ ] **Framework Versions**: All technology versions match MTM_WIP_Application_Avalonia.csproj exactly
  - [ ] **.NET 8** (TargetFramework: net8.0)
  - [ ] **Avalonia UI 11.3.4** (NOT WPF)
  - [ ] **MVVM Community Toolkit 8.3.2** (Source generator patterns)
  - [ ] **MySQL 9.4.0** (MySql.Data package)
  - [ ] **Microsoft Extensions 9.0.8** (DI, Logging, Configuration, Hosting)

### MVVM Pattern Validation
- [ ] **Community Toolkit Only**: All MVVM patterns use MVVM Community Toolkit exclusively
- [ ] **No ReactiveUI**: Zero ReactiveUI patterns or references present
- [ ] **[ObservableProperty] Usage**: All property patterns use [ObservableProperty] attribute
- [ ] **[RelayCommand] Usage**: All command patterns use [RelayCommand] attribute
- [ ] **Source Generator Patterns**: All examples use source generator features correctly

### Database Pattern Validation
- [ ] **Stored Procedures Only**: All database examples use stored procedures exclusively
- [ ] **Helper Pattern**: All database calls use Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()
- [ ] **No Direct SQL**: Zero direct SQL query examples present
- [ ] **45+ Procedures**: Reference to complete catalog of 45+ MTM stored procedures
- [ ] **Transaction Type Logic**: Correct manufacturing transaction type determination (user intent, not operation numbers)

### Avalonia UI Pattern Validation
- [ ] **Correct Namespace**: All AXAML examples use https://github.com/avaloniaui namespace
- [ ] **x:Name Usage**: All Grid naming uses x:Name (not Name) to prevent AVLN2000 errors
- [ ] **DynamicResource Usage**: All theme references use DynamicResource for theme consistency
- [ ] **Control Equivalents**: Use TextBlock (not Label), Flyout (not Popup), etc.
- [ ] **MTM Design System**: All examples reference MTM design system colors and patterns

## Manufacturing Domain Accuracy

### Business Logic Validation
- [ ] **Inventory Management**: Instruction examples accurately reflect MTM inventory workflows
- [ ] **Manufacturing Operations**: Operation sequences (90→100→110→120) correctly explained as workflow steps
- [ ] **Transaction Types**: Transaction types (IN/OUT/TRANSFER) correctly determined by user intent
- [ ] **Part ID Patterns**: Part ID validation patterns match MTM manufacturing standards
- [ ] **Location Management**: Location concepts accurately reflect manufacturing physical locations

### Manufacturing Workflow Instructions
- [ ] **Complete Workflows**: Instructions cover complete manufacturing operator workflows
- [ ] **QuickButtons Usage**: QuickButtons functionality accurately documented
- [ ] **Error Scenarios**: Manufacturing error scenarios properly documented
- [ ] **Performance Requirements**: Manufacturing performance requirements included
- [ ] **Multi-User Scenarios**: Multi-operator/multi-shift scenarios addressed

### Manufacturing Data Integrity
- [ ] **Data Validation Rules**: Manufacturing data validation rules accurately documented
- [ ] **Business Rules**: Manufacturing business rules correctly explained
- [ ] **Audit Requirements**: Manufacturing audit trail requirements covered
- [ ] **Quality Standards**: Manufacturing quality standards reflected in instructions
- [ ] **Compliance Requirements**: Manufacturing compliance requirements addressed

## GitHub Copilot Optimization

### AI Context Optimization
- [ ] **Clear Examples**: Code examples are clear and complete for AI understanding
- [ ] **Pattern Consistency**: All patterns consistently demonstrated across instruction file
- [ ] **Anti-Pattern Documentation**: Common mistakes documented with ❌ WRONG examples
- [ ] **Best Practice Emphasis**: ✅ CORRECT patterns clearly highlighted
- [ ] **Context Completeness**: Sufficient context provided for AI to generate accurate code

### Instruction Structure Validation
- [ ] **Frontmatter Present**: YAML frontmatter with description and applies_to fields
- [ ] **Clear Headings**: Logical heading structure for AI navigation
- [ ] **Code Block Language**: All code blocks properly tagged with language identifiers
- [ ] **Cross-References**: Proper cross-references to related instruction files
- [ ] **Table of Contents**: Complex instruction files include navigation aids

### Manufacturing Context for AI
- [ ] **Manufacturing Examples**: All examples use realistic MTM manufacturing scenarios
- [ ] **Business Context**: Manufacturing business context explained for AI understanding
- [ ] **Domain Terminology**: Manufacturing terminology properly defined and used consistently
- [ ] **Workflow Context**: Manufacturing workflow context provided for AI code generation
- [ ] **Performance Context**: Manufacturing performance requirements context provided

## Implementation Pattern Validation

### Code Example Quality
- [ ] **Compilation Ready**: All code examples would compile without modification
- [ ] **Complete Examples**: Code examples are complete, not partial snippets
- [ ] **Error Handling**: Code examples include appropriate error handling
- [ ] **Resource Cleanup**: Code examples include proper resource disposal
- [ ] **Manufacturing Realistic**: Code examples use realistic manufacturing data

### Architecture Pattern Validation
- [ ] **Service-Oriented Design**: Examples reflect MTM service-oriented architecture
- [ ] **Dependency Injection**: All examples use proper DI patterns
- [ ] **Configuration Patterns**: Configuration examples match MTM configuration architecture
- [ ] **Testing Patterns**: Testing examples match MTM testing architecture
- [ ] **Performance Patterns**: Examples include performance considerations

### Anti-Pattern Prevention
- [ ] **Common Mistakes**: Common implementation mistakes documented and prevented
- [ ] **Memory Leaks**: Memory leak patterns identified and prevented
- [ ] **Performance Issues**: Performance anti-patterns documented and prevented
- [ ] **Security Issues**: Security anti-patterns documented and prevented
- [ ] **Manufacturing Issues**: Manufacturing-specific anti-patterns prevented

## Documentation Quality Standards

### Writing Quality
- [ ] **Clear Language**: Instructions written in clear, unambiguous language
- [ ] **Logical Flow**: Information presented in logical order
- [ ] **Consistent Terminology**: Technical terminology used consistently
- [ ] **Grammar and Spelling**: No grammar or spelling errors present
- [ ] **Professional Tone**: Professional, instructional tone maintained

### Technical Accuracy
- [ ] **Code Syntax**: All code examples use correct syntax
- [ ] **API Usage**: All API usage examples are correct and current
- [ ] **Configuration Examples**: All configuration examples are valid
- [ ] **Command Examples**: All command line examples are accurate
- [ ] **File Path Examples**: All file path examples are correct

### Completeness Validation
- [ ] **Scenario Coverage**: All relevant manufacturing scenarios covered
- [ ] **Edge Cases**: Important edge cases documented
- [ ] **Troubleshooting**: Common troubleshooting scenarios included
- [ ] **Performance Considerations**: Performance implications documented
- [ ] **Cross-Platform Notes**: Cross-platform considerations documented where relevant

## Cross-Reference Validation

### Internal Cross-References
- [ ] **Link Accuracy**: All internal links point to correct files and sections
- [ ] **File Path Accuracy**: All referenced file paths are correct for current repository structure
- [ ] **Section References**: All section references are accurate and up-to-date
- [ ] **Template References**: All template references are accurate
- [ ] **Context References**: All context file references are accurate

### External References
- [ ] **Documentation Links**: All external documentation links are valid and current
- [ ] **Version Compatibility**: All external references compatible with MTM technology versions
- [ ] **Official Sources**: All technology references point to official documentation
- [ ] **Security Considerations**: All external references are from trusted sources
- [ ] **Maintenance Planning**: Process for maintaining external reference accuracy

### Manufacturing Reference Validation
- [ ] **Business Process References**: All manufacturing process references are accurate
- [ ] **Data Model References**: All manufacturing data model references are correct
- [ ] **Workflow References**: All manufacturing workflow references are accurate
- [ ] **Standard References**: All manufacturing standard references are current
- [ ] **Compliance References**: All manufacturing compliance references are accurate

## Automated Validation Checks

### Markdown Validation
- [ ] **Syntax Validation**: Markdown syntax is valid and renders correctly
- [ ] **Link Validation**: All markdown links are valid and accessible
- [ ] **Image Validation**: All referenced images exist and are accessible
- [ ] **Table Formatting**: All markdown tables are properly formatted
- [ ] **Code Block Validation**: All code blocks are properly formatted and tagged

### Content Validation
- [ ] **Spelling Check**: All text passes spell check validation
- [ ] **Grammar Check**: All text passes grammar validation
- [ ] **Consistency Check**: Terminology and patterns used consistently
- [ ] **Completeness Check**: All required sections are present
- [ ] **Manufacturing Context Check**: Manufacturing context is accurate and complete

## Manual Review Items

### Manufacturing Domain Expert Review
- [ ] **Business Accuracy**: Manufacturing processes accurately represented
- [ ] **Workflow Completeness**: All critical manufacturing workflows covered
- [ ] **Data Accuracy**: Manufacturing data patterns accurately documented
- [ ] **Performance Realism**: Performance requirements realistic for manufacturing
- [ ] **Usability Considerations**: Manufacturing operator usability addressed

### Technical Expert Review
- [ ] **Architecture Accuracy**: Technical architecture accurately represented
- [ ] **Implementation Accuracy**: Implementation patterns are correct and optimal
- [ ] **Performance Implications**: Performance implications accurately documented
- [ ] **Security Considerations**: Security implications properly addressed
- [ ] **Maintainability**: Instructions promote maintainable code patterns

### AI Optimization Review
- [ ] **GitHub Copilot Effectiveness**: Instructions optimize GitHub Copilot code generation
- [ ] **Context Clarity**: AI context is clear and comprehensive
- [ ] **Pattern Recognition**: Patterns are clearly recognizable by AI
- [ ] **Anti-Pattern Prevention**: AI can identify and avoid documented anti-patterns
- [ ] **Manufacturing Context**: AI can understand manufacturing context from instructions

## Sign-off

- [ ] **Technical Writer Review**: [Name] - [Date]
- [ ] **Manufacturing Domain Expert Review**: [Name] - [Date]
- [ ] **Technical Architecture Review**: [Name] - [Date]
- [ ] **Quality Gate Approval**: [Name] - [Date]

## Notes
[Space for reviewer notes, instruction improvements, and manufacturing context enhancements]

---

**Template Version**: 1.0  
**Last Updated**: 2025-09-14  
**Manufacturing Grade**: Important Quality Gate