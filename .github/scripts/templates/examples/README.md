---
name: Examples README Template
description: 'Documentation template for MTM audit system examples directory'
applies_to: 'examples/*'
development_context: true
template_type: 'documentation'
quality_gate: 'development'
---

# MTM Audit System Examples

This directory contains real-world examples of the MTM Audit System analyzing feature implementations. Use these files as templates for analyzing and developing other features.

## Example Files

### Gap Report Examples

- Feature-specific gap analysis reports showing implementation status
- Missing component identification and priority ordering
- MTM pattern compliance checking
- Actionable next steps with time estimates

### Copilot Prompt Examples  

- Ready-to-use GitHub Copilot continuation prompts
- Generated from gap report analysis
- Critical context and implementation priorities
- MTM pattern examples and compliance requirements

## How Examples Are Generated

1. **Gap Analysis Phase**: Audit system analyzes feature implementation against current state
2. **Pattern Compliance Check**: Verifies adherence to MTM MVVM, Avalonia, and service patterns
3. **Priority Assessment**: Identifies critical blockers vs. enhancement items
4. **Context Generation**: Creates developer-friendly prompts with implementation examples

## Using These Examples

### For Active Development

1. Copy content from relevant copilot prompt example
2. Paste into GitHub issue or development environment
3. Use with GitHub Copilot for pattern-compliant implementation

### For New Features

1. Use existing examples as templates for structure and style
2. Replace feature-specific details with your requirements
3. Follow gap analysis → priority ordering → implementation workflow
4. Maintain MTM pattern compliance checking standards

## Key Benefits

- **Consistency**: All implementations follow established MTM patterns
- **Efficiency**: Targeted, actionable development guidance
- **Quality**: Comprehensive compliance checking prevents pattern violations
- **Continuity**: Rich context enables effective multi-session development

## Template Adaptation

When creating new examples, customize:

- Feature name and business requirements
- Component architecture and file organization
- Database integration and service patterns
- UI/UX implementation and navigation flows
- Testing strategies and quality gates

The core audit methodology, pattern compliance framework, and priority assessment approach remain consistent across all MTM features.