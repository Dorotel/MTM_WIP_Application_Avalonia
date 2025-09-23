---
name: Template Validation
description: 'Quality assurance checklist for MTM template file accuracy, completeness, and usability'
applies_to: '**/*.md'
manufacturing_context: true
review_type: 'documentation'
quality_gate: 'important'
---

# Template Validation - Quality Assurance Checklist

## Context

- **Component Type**: Template Files (Issues, PRs, Code Review, Testing, Architecture)
- **Manufacturing Domain**: MTM development process templates and manufacturing workflow templates
- **Quality Gate**: Pre-merge validation for consistent development processes

## Template Content Validation

### Template Structure Validation

- [ ] **YAML Frontmatter**: All templates include proper YAML frontmatter with required fields
  - [ ] **name**: Template name clearly identifies purpose
  - [ ] **description**: Clear, concise description of template usage
  - [ ] **applies_to**: Proper file pattern or scope specification
  - [ ] **manufacturing_context**: Boolean flag set appropriately
  - [ ] **Additional Fields**: Any template-specific required fields present

### Template Completeness

- [ ] **All Sections Present**: Template includes all necessary sections for its purpose
- [ ] **Required Fields**: All required fields clearly marked and explained
- [ ] **Optional Fields**: Optional fields clearly distinguished from required fields
- [ ] **Instructions**: Clear instructions provided for template usage
- [ ] **Examples**: Realistic examples provided where helpful

### Manufacturing Context Integration

- [ ] **Manufacturing Scenarios**: Template addresses manufacturing-specific scenarios appropriately
- [ ] **Inventory Management**: Template includes inventory management context where relevant
- [ ] **Manufacturing Workflows**: Template addresses manufacturing workflow requirements
- [ ] **Quality Standards**: Template enforces manufacturing-grade quality standards
- [ ] **Performance Requirements**: Template includes manufacturing performance considerations

## Template Type-Specific Validation

### Issue Templates

- [ ] **Problem Description**: Clear structure for describing manufacturing issues
- [ ] **Reproduction Steps**: Structured format for manufacturing scenario reproduction
- [ ] **Expected Behavior**: Clear format for expected manufacturing system behavior
- [ ] **Environment Information**: Manufacturing environment context fields
- [ ] **Impact Assessment**: Manufacturing impact assessment structure

### Pull Request Templates

- [ ] **Change Description**: Clear structure for describing manufacturing-related changes
- [ ] **Testing Validation**: Manufacturing testing validation checklist
- [ ] **Breaking Changes**: Manufacturing compatibility impact assessment
- [ ] **Documentation Updates**: Manufacturing documentation update requirements
- [ ] **Review Requirements**: Manufacturing domain review requirements

### Code Review Templates

- [ ] **Component Validation**: Appropriate validation for different component types
- [ ] **Manufacturing Standards**: Manufacturing-grade quality standards enforcement
- [ ] **Performance Validation**: Manufacturing performance requirement validation
- [ ] **Security Review**: Manufacturing data security consideration
- [ ] **Compliance Check**: Manufacturing compliance requirement validation

### Testing Templates

- [ ] **Test Coverage**: Manufacturing test coverage requirements
- [ ] **Test Types**: All relevant manufacturing test types covered
- [ ] **Performance Testing**: Manufacturing performance testing requirements
- [ ] **Cross-Platform Testing**: Manufacturing cross-platform validation
- [ ] **Integration Testing**: Manufacturing integration testing requirements

### Architecture Templates

- [ ] **Decision Context**: Manufacturing business context for architectural decisions
- [ ] **Options Analysis**: Manufacturing-appropriate option evaluation
- [ ] **Impact Assessment**: Manufacturing impact assessment criteria
- [ ] **Implementation Plan**: Manufacturing implementation considerations
- [ ] **Monitoring Plan**: Manufacturing system monitoring requirements

## Template Usability Validation

### User Experience

- [ ] **Clear Instructions**: Template instructions are clear and unambiguous
- [ ] **Logical Flow**: Template fields follow logical order
- [ ] **Reasonable Length**: Template is comprehensive but not overwhelming
- [ ] **Field Descriptions**: All fields have clear descriptions and examples
- [ ] **Completion Guidance**: Clear guidance on how to complete each section

### Manufacturing Operator Experience

- [ ] **Operator-Friendly Language**: Templates use language appropriate for manufacturing operators
- [ ] **Workflow Integration**: Templates integrate with manufacturing workflows
- [ ] **Time Considerations**: Templates respect manufacturing operator time constraints
- [ ] **Priority Guidance**: Templates help prioritize manufacturing issues appropriately
- [ ] **Escalation Paths**: Templates provide clear escalation paths for manufacturing issues

### Developer Experience

- [ ] **Technical Accuracy**: Templates use correct technical terminology
- [ ] **Development Integration**: Templates integrate with development workflows
- [ ] **Tool Integration**: Templates work well with development tools
- [ ] **Automation Friendly**: Templates structured for automation where appropriate
- [ ] **Version Control**: Templates work well with version control workflows

## Manufacturing Domain Validation

### Business Process Alignment

- [ ] **Inventory Workflows**: Templates align with MTM inventory management workflows
- [ ] **Manufacturing Operations**: Templates support manufacturing operation requirements
- [ ] **Quality Processes**: Templates enforce manufacturing quality processes
- [ ] **Audit Requirements**: Templates support manufacturing audit requirements
- [ ] **Compliance Standards**: Templates ensure manufacturing compliance standards

### Manufacturing Data Handling

- [ ] **Data Accuracy**: Templates ensure manufacturing data accuracy
- [ ] **Data Security**: Templates address manufacturing data security
- [ ] **Data Integrity**: Templates validate manufacturing data integrity
- [ ] **Audit Trails**: Templates support manufacturing audit trail requirements
- [ ] **Performance Data**: Templates address manufacturing performance data requirements

### Manufacturing System Integration

- [ ] **System Compatibility**: Templates ensure manufacturing system compatibility
- [ ] **Integration Requirements**: Templates address manufacturing system integration
- [ ] **Performance Requirements**: Templates validate manufacturing performance requirements
- [ ] **Scalability Requirements**: Templates address manufacturing scalability needs
- [ ] **Reliability Requirements**: Templates ensure manufacturing system reliability

## Content Quality Standards

### Writing Quality

- [ ] **Grammar and Spelling**: No grammar or spelling errors
- [ ] **Professional Tone**: Consistent, professional tone throughout
- [ ] **Clear Language**: Language is clear and accessible
- [ ] **Consistent Terminology**: Technical terms used consistently
- [ ] **Appropriate Detail Level**: Appropriate level of detail for intended audience

### Technical Accuracy

- [ ] **Current Information**: All technical information is current and accurate
- [ ] **Version Compatibility**: All references compatible with MTM technology stack
- [ ] **Best Practices**: Templates promote current best practices
- [ ] **Security Considerations**: Security best practices appropriately addressed
- [ ] **Performance Considerations**: Performance implications properly addressed

### Manufacturing Accuracy

- [ ] **Process Accuracy**: Manufacturing processes accurately represented
- [ ] **Terminology Accuracy**: Manufacturing terminology used correctly
- [ ] **Workflow Accuracy**: Manufacturing workflows accurately reflected
- [ ] **Standard Compliance**: Manufacturing standards properly referenced
- [ ] **Quality Requirements**: Manufacturing quality requirements accurately stated

## Template Maintenance Validation

### Version Control

- [ ] **Change Tracking**: Template changes properly tracked and documented
- [ ] **Version History**: Clear version history maintained
- [ ] **Change Rationale**: Reasons for template changes documented
- [ ] **Impact Assessment**: Change impact on existing usage assessed
- [ ] **Migration Guidance**: Guidance provided for template updates

### Update Procedures

- [ ] **Review Process**: Clear process for template updates
- [ ] **Approval Process**: Appropriate approval process for template changes
- [ ] **Testing Process**: Template changes tested before deployment
- [ ] **Communication Process**: Template changes properly communicated
- [ ] **Training Process**: Training provided for significant template changes

### Usage Analytics

- [ ] **Usage Tracking**: Template usage tracked and analyzed
- [ ] **Effectiveness Measurement**: Template effectiveness measured
- [ ] **Feedback Collection**: User feedback collected and analyzed
- [ ] **Improvement Identification**: Areas for improvement identified
- [ ] **Success Metrics**: Template success metrics defined and tracked

## Automated Validation Checks

### Format Validation

- [ ] **Markdown Syntax**: Valid markdown syntax throughout template
- [ ] **YAML Validation**: YAML frontmatter is valid and properly formatted
- [ ] **Link Validation**: All internal links are valid
- [ ] **Reference Validation**: All file references are accurate
- [ ] **Formatting Consistency**: Consistent formatting throughout template

### Content Validation

- [ ] **Required Field Check**: All required fields are present
- [ ] **Example Validation**: All examples are accurate and current
- [ ] **Instruction Clarity**: Instructions are clear and complete
- [ ] **Manufacturing Context**: Manufacturing context is appropriate and accurate
- [ ] **Technology Accuracy**: All technology references are current and correct

## Integration Testing

### Workflow Integration

- [ ] **GitHub Integration**: Templates integrate properly with GitHub workflows
- [ ] **Tool Integration**: Templates work with development and manufacturing tools
- [ ] **Process Integration**: Templates fit into existing development and manufacturing processes
- [ ] **Automation Integration**: Templates work with automated processes
- [ ] **Notification Integration**: Templates trigger appropriate notifications

### User Acceptance Testing

- [ ] **Developer Testing**: Developers can successfully use templates
- [ ] **Manufacturing Team Testing**: Manufacturing team can successfully use templates
- [ ] **Manager Testing**: Management can extract needed information from template usage
- [ ] **Quality Team Testing**: Quality team can use templates for validation
- [ ] **Support Team Testing**: Support team can use templates for issue resolution

## Manual Review Items

### Template Design Review

- [ ] **Purpose Clarity**: Template purpose is clear and well-defined
- [ ] **Scope Appropriateness**: Template scope is appropriate for its intended use
- [ ] **Structure Logic**: Template structure is logical and intuitive
- [ ] **Completeness Assessment**: Template covers all necessary aspects
- [ ] **Usability Assessment**: Template is usable by intended audience

### Manufacturing Domain Review

- [ ] **Business Alignment**: Template aligns with manufacturing business needs
- [ ] **Process Integration**: Template integrates with manufacturing processes
- [ ] **Quality Standards**: Template enforces appropriate manufacturing quality standards
- [ ] **Performance Requirements**: Template addresses manufacturing performance needs
- [ ] **Compliance Requirements**: Template ensures manufacturing compliance

### Technical Review

- [ ] **Technical Accuracy**: All technical content is accurate
- [ ] **Implementation Feasibility**: Template requirements are technically feasible
- [ ] **Tool Compatibility**: Template works with required tools
- [ ] **Integration Capability**: Template integrates with existing systems
- [ ] **Maintenance Sustainability**: Template is maintainable long-term

## Sign-off

- [ ] **Template Author Review**: [Name] - [Date]
- [ ] **Manufacturing Domain Expert Review**: [Name] - [Date]
- [ ] **Technical Review**: [Name] - [Date]
- [ ] **Quality Gate Approval**: [Name] - [Date]

## Notes

[Space for reviewer notes, template improvement suggestions, and manufacturing context enhancements]

---

**Template Version**: 1.0  
**Last Updated**: 2025-09-14  
**Manufacturing Grade**: Important Quality Gate