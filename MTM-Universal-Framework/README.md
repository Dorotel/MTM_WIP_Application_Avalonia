# MTM Universal Framework

A comprehensive, production-ready universal framework extracted from the MTM WIP Application Avalonia codebase that serves as a foundation for building enterprise applications across multiple platforms.

## 🎯 Framework Overview

This framework provides:

- **Cross-Platform Avalonia Support**: Windows desktop and Android mobile, with macOS/Linux compatibility
- **Manufacturing-Grade Quality**: 95%+ test coverage standards and comprehensive validation
- **Complete GitHub Copilot Integration**: AI-assisted development with 33+ instruction files
- **5-Minute Project Setup**: From framework installation to running application
- **MVVM Community Toolkit**: Source generator-based MVVM patterns
- **Enterprise Architecture**: Service-oriented design with dependency injection

## 🏗️ Framework Structure

```
MTM-Universal-Framework/
├── 📁 01-Core-Libraries/           # Universal NuGet packages and core infrastructure
├── 📁 02-Project-Templates/        # Visual Studio/Rider project templates
├── 📁 03-Documentation-System/     # Complete GitHub Copilot integration and docs
├── 📁 04-Testing-Framework/        # Comprehensive testing strategies and tools
├── 📁 05-Deployment-Tools/         # CI/CD, build scripts, and deployment automation
├── 📁 06-Configuration-System/     # Universal configuration and settings management
├── 📁 07-Quality-Assurance/        # QA templates, code review, and validation
├── 📁 08-Developer-Experience/     # IDE extensions, snippets, and productivity tools
├── 📁 09-Cross-Platform-Support/   # Platform-specific adaptations and optimizations
└── 📁 10-Help-Documentation/       # Comprehensive help system and implementation guides
```

## 🚀 Quick Start

### 1. Install Framework Templates

```bash
# Install MTM Universal Framework templates
dotnet new install MTM.UniversalFramework.Templates

# Create new cross-platform business application
dotnet new mtm-business-app -n MyBusinessApp -o ./MyBusinessApp

# Navigate and run
cd MyBusinessApp
dotnet run --framework net8.0-windows  # For Windows
# OR
dotnet build -t:Run --framework net8.0-android  # For Android
```

### 2. Framework Components

The framework includes:

- **MTM.Core**: Application startup, configuration, error handling, logging
- **MTM.MVVM**: MVVM Community Toolkit patterns with cross-platform lifecycle
- **MTM.Avalonia**: UI controls optimized for Windows + Android
- **MTM.Data**: Database abstraction with offline-first capabilities

## 🎨 Technology Stack

- **.NET 8**: Target framework with C# 12 features
- **Avalonia UI 11.3.4**: Cross-platform UI framework
- **MVVM Community Toolkit 8.3.2**: Source generator MVVM patterns
- **Microsoft Extensions**: Dependency injection, logging, configuration
- **MySQL/PostgreSQL/SQLite**: Database support with abstraction layer

## 📚 Documentation

- **Implementation Guide**: Complete step-by-step implementation
- **API Reference**: Full API documentation with examples
- **Migration Guide**: From existing frameworks to MTM Universal
- **Best Practices**: Architecture patterns and design principles

## 🧪 Quality Standards

- **95%+ Test Coverage**: Unit, integration, and UI tests
- **Cross-Platform Validation**: All features tested on target platforms
- **Performance Benchmarks**: Manufacturing-grade performance standards
- **Accessibility Compliance**: WCAG 2.1 AA compliance built-in

## 🤖 GitHub Copilot Integration

The framework includes comprehensive GitHub Copilot integration with:

- 33+ specialized instruction files for different development scenarios
- Context-aware code completion for framework patterns
- AI-assisted testing and documentation generation
- Domain-agnostic templates that adapt to any business context

## 📄 License

Licensed under the MIT License - see LICENSE.txt for details.

## 🙋‍♂️ Support

- **Documentation**: Complete implementation guides and API reference
- **Examples**: Sample applications demonstrating framework usage
- **Community**: Active support and contribution guidelines