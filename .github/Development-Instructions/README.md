# Development Instructions

This folder contains development workflow, tools, and infrastructure guidelines for the MTM WIP Application.

## Files in this Category

### errorhandler.instruction.md
- **Purpose**: Error handling patterns and logging integration
- **Key Topics**: Service_ErrorHandler usage, ReactiveUI error handling
- **Usage**: Reference for implementing error handling in ViewModels and services

### githubworkflow.instruction.md
- **Purpose**: CI/CD and Git workflow practices
- **Key Topics**: Branching strategy, deployment procedures, code review standards
- **Usage**: Reference for development workflow and team collaboration

### Related Development Files
- **Development/Database_Files/**: Database patterns and stored procedure guidelines
- **Development/Examples/**: Code usage examples and integration patterns
- **Services/**: Error handling service implementations

## Key Development Patterns

### Database Access
- **Stored Procedures Only**: No direct SQL in application code
- **Helper_Database_StoredProcedure**: Standard database access pattern
- **Result<T> Pattern**: Consistent error handling and return types

### Error Handling
- **Service_ErrorHandler**: Centralized error management
- **ReactiveUI Integration**: Command exception handling
- **Logging Integration**: Structured logging throughout application

## Integration Points

- **Core Instructions**: Extends coding conventions with workflow guidance
- **UI Instructions**: Error presentation and user feedback patterns
- **Quality Instructions**: Development quality standards and compliance