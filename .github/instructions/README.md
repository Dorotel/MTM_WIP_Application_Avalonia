# MTM WIP Application - GitHub Instructions Directory

This directory contains comprehensive technical instructions and guidelines for the MTM WIP Application development, organized to support GitHub Copilot awesome implementation standards.

## 📁 Directory Structure

### Core Architecture Instructions
- `dotnet-architecture-good-practices.instructions.md` - .NET 8 architecture patterns and best practices
- `avalonia-ui-guidelines.instructions.md` - Avalonia UI 11.3.4 development standards  
- `mvvm-community-toolkit.instructions.md` - MVVM Community Toolkit implementation patterns
- `mysql-database-patterns.instructions.md` - MySQL stored procedure patterns and 45+ procedure catalog

### Development Standards
- `service-layer-documentation.md` - Service architecture for all 12 services
- `model-specifications.md` - Data model documentation for all 10 models
- `component-relationships.md` - View/ViewModel/Service dependency mapping

### Implementation Guidelines
- `code-quality-standards.md` - Coding standards for C#, AXAML, and SQL
- `testing-strategy.md` - Unit testing, integration testing, and quality assurance
- `deployment-procedures.md` - Build, deployment, and DevOps guidelines

## 🎯 Purpose

These instruction files serve as the authoritative reference for:

1. **Architecture Compliance**: Ensuring all code follows established MTM patterns
2. **Copilot Enhancement**: Providing context for intelligent code generation
3. **Developer Onboarding**: Comprehensive guidance for new team members
4. **Quality Assurance**: Standards for code review and validation processes

## 📚 Usage with GitHub Copilot

Reference these instruction files in Copilot prompts using the pattern:
```
# MTM WIP Application - Comprehensive Testing Infrastructure Documentation

## 🎯 Overview

This directory contains comprehensive documentation for creating complete testing infrastructure for the MTM WIP Application following awesome-copilot patterns. All content has been integrated from the original Testing folder and expanded to provide complete agent-driven test creation guidance.

## 📁 Documentation Structure

### 🎯 Core Testing Standards

#### [`testing-standards.instructions.md`](./testing-standards.instructions.md)
- **Purpose**: 5-tier comprehensive testing strategy for MTM WIP Application
- **Scope**: Unit, Integration, UI, Database, and Cross-Platform testing standards
- **Features**: Cross-platform feature matrix, manufacturing workflow coverage, performance thresholds
- **Key Components**: NUnit 4.1.0, Moq 4.20.70, Avalonia.Headless, NBomber performance testing

#### [`cross-platform-testing-standards.instructions.md`](./cross-platform-testing-standards.instructions.md)
- **Purpose**: Ensure consistent functionality across Windows, macOS, Linux, and Android
- **Scope**: Platform-specific testing requirements, UI adaptations, file system compatibility
- **Features**: Platform detection, responsive layouts, touch interface support, performance validation

### 🔧 Specialized Testing Patterns

#### [`unit-testing-patterns.instructions.md`](./unit-testing-patterns.instructions.md)
- **Purpose**: MVVM Community Toolkit testing patterns for ViewModels and Services
- **Scope**: `[ObservableProperty]` and `[RelayCommand]` testing, service mocking, validation testing
- **Features**: Property change notifications, command execution, dependency injection testing

#### [`integration-testing-patterns.instructions.md`](./integration-testing-patterns.instructions.md)
- **Purpose**: Multi-service workflow testing and cross-system integration
- **Scope**: Database integration, service communication, manufacturing workflow validation
- **Features**: Real database connections, service orchestration, end-to-end scenarios

#### [`ui-automation-standards.instructions.md`](./ui-automation-standards.instructions.md)
- **Purpose**: Avalonia.Headless UI testing framework and automation patterns
- **Scope**: Data binding validation, user interactions, theme testing, responsive behavior
- **Features**: Headless testing, control automation, cross-platform UI validation

#### [`database-testing-patterns.instructions.md`](./database-testing-patterns.instructions.md)
- **Purpose**: MySQL stored procedure testing and data integrity validation
- **Scope**: 45+ stored procedures, transaction consistency, performance under load
- **Features**: Data integrity checks, concurrent access testing, performance thresholds

### 🏗️ Application Architecture Instructions

#### [`mvvm-community-toolkit.instructions.md`](./mvvm-community-toolkit.instructions.md)
- **Purpose**: MVVM Community Toolkit implementation patterns (exclusive - ReactiveUI removed)
- **Scope**: Source generator attributes, property/command patterns, ViewModel architecture
- **Features**: `[ObservableProperty]`, `[RelayCommand]`, dependency injection integration

#### [`mysql-database-patterns.instructions.md`](./mysql-database-patterns.instructions.md)
- **Purpose**: Database access patterns using stored procedures exclusively
- **Scope**: `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` usage, 45+ procedures
- **Features**: No direct SQL, stored procedure only, data integrity patterns

#### [`avalonia-ui-guidelines.instructions.md`](./avalonia-ui-guidelines.instructions.md)
- **Purpose**: Avalonia AXAML syntax and MTM design system implementation
- **Scope**: AVLN2000 error prevention, responsive layouts, theme integration
- **Features**: Windows 11 Blue theme, card-based layouts, cross-platform UI patterns

#### [`service-architecture.instructions.md`](./service-architecture.instructions.md)
- **Purpose**: Service layer organization and dependency injection patterns
- **Scope**: Category-based service consolidation, DI lifecycle management, error handling
- **Features**: Singleton/Scoped/Transient patterns, centralized error handling

#### [`dotnet-architecture-good-practices.instructions.md`](./dotnet-architecture-good-practices.instructions.md)
- **Purpose**: .NET 8 architecture patterns and best practices for enterprise applications
- **Scope**: Code quality, performance optimization, maintainability standards
- **Features**: C# 12 features, nullable reference types, async patterns

#### [`data-models.instructions.md`](./data-models.instructions.md)
- **Purpose**: Data model design patterns and validation for manufacturing domain
- **Scope**: Entity design, validation patterns, data integrity rules
- **Features**: Manufacturing entities, business rule validation, data consistency

## 🤖 Agent-Driven Test Creation Prompts

Located in `../.github/copilot/prompts/`, these provide detailed templates for agent-driven test creation:

### [`create-unit-test.prompt.md`](../copilot/prompts/create-unit-test.prompt.md)
- **Template**: Comprehensive MVVM Community Toolkit unit testing
- **Patterns**: `[ObservableProperty]` testing, `[RelayCommand]` validation, service mocking
- **Examples**: ViewModel testing, property change notifications, command execution scenarios

### [`create-integration-test.prompt.md`](../copilot/prompts/create-integration-test.prompt.md)
- **Template**: Multi-service workflow integration testing
- **Patterns**: Service orchestration, database integration, file system operations
- **Examples**: Manufacturing workflows, transaction consistency, cross-service communication

### [`create-ui-test.prompt.md`](../copilot/prompts/create-ui-test.prompt.md)
- **Template**: Avalonia.Headless UI automation testing
- **Patterns**: Control automation, data binding validation, user interaction simulation
- **Examples**: Form submission, navigation, theme switching, responsive behavior

### [`create-database-test.prompt.md`](../copilot/prompts/create-database-test.prompt.md)
- **Template**: MySQL stored procedure testing and performance validation
- **Patterns**: Stored procedure validation, data integrity checking, concurrent access testing
- **Examples**: Transaction consistency, performance under load, data validation

### [`create-performance-test.prompt.md`](../copilot/prompts/create-performance-test.prompt.md)
- **Template**: NBomber performance testing and load validation
- **Patterns**: Load testing, stress testing, memory leak detection, throughput validation
- **Examples**: High-volume operations, concurrent user scenarios, system limits

### [`create-cross-platform-test.prompt.md`](../copilot/prompts/create-cross-platform-test.prompt.md)
- **Template**: Cross-platform compatibility testing for Windows, macOS, Linux, Android
- **Patterns**: Platform detection, file system compatibility, UI adaptation validation
- **Examples**: Path handling, platform-specific features, responsive layouts, touch interfaces

## 📚 Supporting Documentation

Located in `../.github/docs/`:

### [`Platform-Specific-File-System-Differences.md`](../docs/Platform-Specific-File-System-Differences.md)
- **Content**: Detailed technical guide for file system differences across platforms
- **Covers**: Path handling, permissions, storage providers, platform-specific considerations
- **Usage**: Technical reference for cross-platform file operations

### [`README-CrossPlatformTesting.md`](../docs/README-CrossPlatformTesting.md)
- **Content**: Comprehensive cross-platform testing guide and setup instructions
- **Covers**: Testing approaches, platform support matrix, VM setup, emulator configuration
- **Usage**: Step-by-step guide for setting up cross-platform testing environments

### [`Test-CrossPlatformCompatibility.ps1`](../scripts/Test-CrossPlatformCompatibility.ps1)
- **Content**: PowerShell script for quick cross-platform validation
- **Covers**: Build validation, platform detection, file system testing, basic compatibility
- **Usage**: Quick validation script for local development and CI/CD pipelines

## 🎨 MTM Manufacturing Domain Context

The testing infrastructure is specifically designed for MTM's manufacturing inventory management domain:

### **Core Business Entities**
- **PartInfo**: Manufacturing part identifiers with operations and quantities
- **InventoryItem**: Inventory management with locations and users
- **TransactionInfo**: Manufacturing transactions (IN/OUT/TRANSFER by user intent)
- **QuickButtonInfo**: Rapid manufacturing operation execution

### **Manufacturing Workflow Coverage**
- **Inventory Operations**: Add, remove, transfer inventory across manufacturing stations
- **Transaction Processing**: Manufacturing transaction recording and history
- **QuickButton Execution**: Rapid manufacturing operation shortcuts
- **Master Data Management**: Parts, operations, locations, users
- **Error Logging**: Manufacturing operation error tracking and analysis

### **Cross-Platform Manufacturing Scenarios**
- **Windows**: Primary manufacturing floor workstations
- **macOS**: Office management and reporting systems
- **Linux**: Manufacturing automation and server systems
- **Android**: Mobile manufacturing operations and warehouse management

## 🚀 Getting Started with Agent-Driven Testing

### For Unit Testing
1. Reference `testing-standards.instructions.md` for overall strategy
2. Use `unit-testing-patterns.instructions.md` for specific patterns
3. Apply `create-unit-test.prompt.md` template for agent creation
4. Focus on MVVM Community Toolkit patterns and service mocking

### For Integration Testing
1. Reference `integration-testing-patterns.instructions.md` for multi-service scenarios
2. Use `database-testing-patterns.instructions.md` for database integration
3. Apply `create-integration-test.prompt.md` template for comprehensive workflows
4. Include real database connections and manufacturing scenarios

### For UI Testing
1. Reference `ui-automation-standards.instructions.md` for Avalonia.Headless patterns
2. Use `avalonia-ui-guidelines.instructions.md` for UI standards
3. Apply `create-ui-test.prompt.md` template for automation scenarios
4. Test data binding, user interactions, and responsive behavior

### For Cross-Platform Testing
1. Reference `cross-platform-testing-standards.instructions.md` for platform requirements
2. Use `Platform-Specific-File-System-Differences.md` for technical details
3. Apply `create-cross-platform-test.prompt.md` for comprehensive coverage
4. Test Windows, macOS, Linux, and Android scenarios

## ✅ Validation Checklist

When creating tests using these instructions:

- [ ] **Unit Tests**: Cover all ViewModels, Services, and business logic with MVVM Community Toolkit patterns
- [ ] **Integration Tests**: Validate multi-service workflows and database operations with real connections
- [ ] **UI Tests**: Automate user interactions and validate data binding with Avalonia.Headless
- [ ] **Database Tests**: Test all 45+ stored procedures with performance and integrity validation
- [ ] **Performance Tests**: Use NBomber for load testing and memory leak detection
- [ ] **Cross-Platform Tests**: Ensure functionality across Windows, macOS, Linux, and Android
- [ ] **Manufacturing Domain**: Cover inventory, transactions, QuickButtons, and master data scenarios
- [ ] **Error Handling**: Test centralized error handling and logging across all scenarios
- [ ] **Documentation**: Include clear test descriptions, expected outcomes, and validation criteria

## 🔄 Integration with GitHub Copilot

This documentation structure is designed to work seamlessly with GitHub Copilot for agent-driven test creation:

1. **Instruction Files** provide comprehensive context and patterns
2. **Prompt Templates** offer detailed examples and code structures
3. **Supporting Documentation** gives technical background and platform details
4. **Cross-References** ensure consistent patterns across all test types

Agents can use these files to understand the complete MTM WIP Application context and generate appropriate, comprehensive tests that follow established patterns and cover all critical scenarios.

---

**Created**: December 2024  
**Last Updated**: December 2024  
**Version**: 1.0.0  
**Scope**: Complete testing infrastructure for MTM WIP Application  
**Platforms**: Windows, macOS, Linux, Android  
**Testing Framework**: NUnit, Moq, Avalonia.Headless, NBomber  
**Architecture**: .NET 8, Avalonia UI 11.3.4, MVVM Community Toolkit, MySQL
```

This ensures Copilot has comprehensive context about MTM-specific patterns, business logic, and technical requirements.

## 🔧 Maintenance

- **Review Frequency**: Monthly updates to reflect architecture evolution
- **Version Control**: Track changes to maintain instruction file history
- **Cross-Reference**: Ensure consistency with `/docs/ways-of-work/plan/` documentation
- **Validation**: Test instruction effectiveness with actual Copilot sessions

---

**Last Updated**: September 4, 2025  
**Maintained By**: MTM Development Team
