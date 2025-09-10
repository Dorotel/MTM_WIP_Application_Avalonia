---
description: 'Generate comprehensive MTM feature requests with complete technical specifications, implementation plans, and testing strategies'
tools: ['codebase', 'search', 'usages', 'editFiles']
---

# MTM Feature Request Generator

Create detailed feature requests for the MTM WIP Application that include complete technical specifications, implementation guidance, and testing strategies.

## Feature Analysis Framework

### 1. Business Context Assessment
- **Manufacturing Impact**: How does this feature improve inventory management, part tracking, or operation workflows?
- **User Workflow**: Which MTM user personas benefit (operators, supervisors, managers)?
- **Business Value**: Quantifiable improvements (time savings, error reduction, efficiency gains)
- **Integration Points**: How does this connect to existing MTM functionality?

### 2. Technical Feasibility Analysis
- **.NET 8 Avalonia Compatibility**: Ensure alignment with current technology stack
- **MVVM Pattern Integration**: How does this fit with existing ViewModels and Services?
- **Database Requirements**: Required stored procedures, schema changes, or data migration
- **Performance Implications**: Expected load, response time requirements, scalability needs

### 3. Implementation Strategy
- **Architecture Approach**: Service layer, ViewModel, and View implementation strategy
- **Data Flow Design**: How data moves through the application layers
- **Error Handling**: Integration with Services.ErrorHandling patterns
- **Theme Integration**: MTM design system compliance and dynamic theming support

## Feature Request Template Structure

### Feature Overview
```markdown
## Feature Name: [Descriptive Feature Title]

### Business Justification
- **Problem Statement**: What manufacturing problem does this solve?
- **Current Pain Points**: Specific issues with existing workflow
- **Success Metrics**: How will we measure success?
- **User Stories**: Who benefits and how?

### Technical Summary
- **Component Type**: [UI Enhancement | Service Addition | Database Extension | Integration]
- **Complexity Level**: [Low | Medium | High | Complex]
- **Development Effort**: [1-3 days | 1-2 weeks | 2-4 weeks | 1+ months]
- **Dependencies**: Required prerequisites or blocking issues
```

### Implementation Details
```markdown
## Technical Implementation Plan

### Database Changes
- **New Stored Procedures**: List required procedures with purpose
- **Schema Modifications**: Table changes, new tables, indexing requirements
- **Data Migration**: Required data updates or migrations
- **Performance Considerations**: Query optimization, indexing strategy

### Service Layer Implementation
- **New Services**: Required service classes and interfaces
- **Service Integration**: How services interact with existing architecture
- **Dependency Injection**: Registration patterns and lifetimes
- **Error Handling**: Integration with centralized error management

### ViewModel Implementation
- **New ViewModels**: MVVM Community Toolkit implementation
- **Property Patterns**: [ObservableProperty] attributes and validation
- **Command Patterns**: [RelayCommand] implementation and CanExecute logic
- **Data Binding**: Two-way binding requirements and validation

### UI Implementation
- **View Structure**: AXAML layout following InventoryTabView patterns
- **Design System**: MTM theme integration and responsive design
- **Accessibility**: TabIndex, ToolTip.Tip, and screen reader support
- **User Experience**: Loading states, error handling, empty states
```

### Quality Assurance
```markdown
## Testing Strategy

### Unit Testing
- **ViewModel Tests**: Property changes, command execution, validation logic
- **Service Tests**: Business logic, error handling, database integration
- **Mock Strategy**: Database and external dependency mocking approach

### Integration Testing
- **Database Tests**: Stored procedure validation and data integrity
- **Service Integration**: End-to-end workflow testing
- **UI Integration**: View-ViewModel interaction and data binding

### User Acceptance Testing
- **Test Scenarios**: Real-world usage scenarios and edge cases
- **Performance Testing**: Load testing and response time validation
- **Accessibility Testing**: WCAG compliance and usability validation
```

## Feature Categories

### UI Enhancements
- New tabs, forms, or components
- Improved data grids or visualization
- Enhanced user interactions or workflows

### Service Extensions
- New business logic or data processing
- Integration with external systems
- Enhanced reporting or analytics

### Database Features
- New data models or relationships
- Enhanced querying or reporting capabilities
- Data import/export functionality

### Infrastructure Improvements
- Performance optimizations
- Security enhancements
- Monitoring and logging improvements

## Implementation Guidelines

### Code Quality Standards
- Follow MTM architectural patterns consistently
- Implement comprehensive error handling
- Ensure MVVM Community Toolkit compliance
- Maintain database stored procedure patterns

### Documentation Requirements
- Update relevant instruction files
- Create or update user guides
- Document API changes or new service interfaces
- Update architecture documentation

### Performance Requirements
- Maintain 60fps UI responsiveness
- Database operations under 500ms
- Memory usage within established limits
- Scalable to current user load requirements

Use this template when creating feature requests that require comprehensive technical planning and implementation guidance.