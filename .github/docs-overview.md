# MTM WIP Application - Documentation Overview

This is the central documentation hub for the MTM WIP Application, a manufacturing inventory management system built with .NET 8 and Avalonia UI. All documentation is organized within the `.github/` folder structure for optimal GitHub Copilot integration.

## 📋 Documentation Structure

### 🎯 GitHub Copilot Integration
- **Master Instructions**: [copilot-instructions.md](copilot-instructions.md) - Primary AI assistant context
- **Core Instructions**: [instructions/](instructions/) - Comprehensive development guidelines  
- **Templates**: [copilot/templates/](copilot/templates/) - Code generation templates
- **Patterns**: [copilot/patterns/](copilot/patterns/) - Architecture pattern definitions
- **Context**: [copilot/context/](copilot/context/) - Domain knowledge and technology stack

### 🏗️ Architecture and Planning
- **Architecture**: [architecture/](architecture/) - System architecture and project blueprints
- **Project Management**: [project-management/](project-management/) - Implementation plans and specifications
- **Database Documentation**: [database-documentation/](database-documentation/) - Database schemas and procedures

### 💻 Development Resources
- **Development Guides**: [development-guides/](development-guides/) - Implementation guides and best practices
- **QA Framework**: [qa-framework/](qa-framework/) - Testing strategies and quality assurance
- **UI/UX**: [ui-ux/](ui-ux/) - Design system and theme documentation
- **Scripts**: [scripts/](scripts/) - Automation and utility scripts

### 📝 GitHub Workflow Integration
- **Issue Templates**: [ISSUE_TEMPLATE/](ISSUE_TEMPLATE/) - Bug reports, feature requests, enhancements
- **Pull Request Templates**: [PULL_REQUEST_TEMPLATE/](PULL_REQUEST_TEMPLATE/) - Feature implementation, documentation changes
- **Workflows**: [workflows/](workflows/) - CI/CD pipeline automation

### 🚀 Prompts and Automation
- **Prompts**: [prompts/](prompts/) - AI-powered development prompts
- **Memory Bank**: [memory-bank/](memory-bank/) - Project knowledge base
- **Issues**: [issues/](issues/) - Issue templates and management

## 🏗️ Technology Stack

- **.NET 8** with C# 12 language features and nullable reference types
- **Avalonia UI 11.3.4** for cross-platform desktop UI (Windows, macOS, Linux)
- **MVVM Community Toolkit 8.3.2** for MVVM patterns with source generators
- **MySQL 9.4.0** database with stored procedures only architecture
- **Microsoft Extensions 9.0.8** for dependency injection, logging, configuration, hosting

## 🎯 Architecture Patterns

### Core Patterns
- **Service-Oriented MVVM**: Clean separation with comprehensive dependency injection
- **Stored Procedures Only**: All database access via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`
- **Manufacturing Domain**: Inventory management with operation-based workflows (90, 100, 110, 120)
- **Category-Based Services**: Service consolidation in single files for related functionality

### GitHub Copilot Enhanced Development
- **Context-Aware AI**: Comprehensive instruction system for consistent code generation
- **Template-Driven**: Standardized templates for ViewModels, Services, and UI components
- **Pattern Enforcement**: AI-guided adherence to established architectural patterns
- **Manufacturing Domain Intelligence**: AI understanding of inventory management workflows

## 🔍 Quick Navigation

### For Developers
- **Start Here**: [copilot-instructions.md](copilot-instructions.md) - Complete development context
- **New Features**: [development-guides/MTM-View-Implementation-Guide.md](development-guides/MTM-View-Implementation-Guide.md)
- **Database Work**: [instructions/mysql-database-patterns.instructions.md](instructions/mysql-database-patterns.instructions.md)
- **UI Development**: [instructions/avalonia-ui-guidelines.instructions.md](instructions/avalonia-ui-guidelines.instructions.md)

### For Project Management
- **Implementation Plans**: [project-management/](project-management/) - Feature development roadmaps
- **Architecture Decisions**: [architecture/](architecture/) - System design documentation
- **Quality Assurance**: [qa-framework/](qa-framework/) - Testing strategies

### For Contributors
- **Contributing Guidelines**: [development-guides/MTM-Documentation-Standards.md](development-guides/MTM-Documentation-Standards.md)
- **Code Review Process**: [project-management/code-review-guidelines.md](project-management/code-review-guidelines.md)
- **Issue Templates**: [ISSUE_TEMPLATE/](ISSUE_TEMPLATE/) - How to report bugs and request features

## 📊 Documentation Metrics

- **Total Active Files**: ~150 comprehensive documentation files
- **Technology Coverage**: 100% current stack documented
- **GitHub Copilot Ready**: All content optimized for AI-assisted development
- **Cross-Platform Support**: Windows, macOS, Linux development guidance
- **Manufacturing Domain**: Complete inventory management workflow documentation

---

**Migrated**: 2025-09-14 (from docs/README.md)  
**Structure**: Unified .github/ documentation system  
**AI-Enhanced**: Optimized for GitHub Copilot integration