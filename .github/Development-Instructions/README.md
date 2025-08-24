# Development Instructions

This folder contains workflow setup, error handling implementation, and database integration guidelines for the MTM WIP Application Avalonia.

## Files in this Category

### errorhandler.instruction.md
- **Purpose**: Error handling patterns and implementation guidelines
- **Key Topics**: Error logging, user-friendly messages, exception management
- **Usage**: Reference when implementing error handling in any component

### githubworkflow.instruction.md
- **Purpose**: GitHub Actions and CI/CD pipeline configuration
- **Key Topics**: Build automation, testing workflows, deployment patterns
- **Usage**: Setting up and maintaining GitHub workflows

### database-patterns.instruction.md ? **NEW**
- **Purpose**: Database access patterns and MTM business logic rules
- **Key Topics**: Stored procedure usage, TransactionType logic, MTM operation numbers
- **Usage**: Critical reference for all database operations and business logic

### templates-documentation.instruction.md ? **NEW**
- **Purpose**: HTML template usage and documentation migration guidelines
- **Key Topics**: FileDefinitions templates, PlainEnglish vs Technical documentation
- **Usage**: When migrating documentation or creating new HTML documentation

## MTM Business Logic Integration

These development instructions include critical MTM-specific business rules:
- **TransactionType Logic**: Determined by user intent, NOT operation numbers
- **Database Access**: Stored procedures only, no direct SQL
- **Operation Numbers**: Workflow step identifiers, not transaction type indicators

## Integration Points

- **Core Instructions**: Database patterns extend coding conventions
- **UI Instructions**: Template systems integrate with UI generation patterns
- **Quality Instructions**: Error handling patterns support quality standards
- **Automation Instructions**: Documentation templates support automation workflows