# MTM WIP Application - Avalonia

## 🏛️ Constitutional Compliance Status

[![Constitutional Compliance](https://img.shields.io/badge/Constitutional%20Compliance-ACTIVE-green?style=flat-square&logo=github)](./constitution.md)
[![Code Quality Excellence](https://img.shields.io/badge/Article%20I-Code%20Quality%20Excellence-blue?style=flat-square)](./constitution.md#article-i-core-principle-i---code-quality-excellence)
[![Testing Standards](https://img.shields.io/badge/Article%20II-Comprehensive%20Testing-blue?style=flat-square)](./constitution.md#article-ii-core-principle-ii---comprehensive-testing-standards)
[![UX Consistency](https://img.shields.io/badge/Article%20III-UX%20Consistency-blue?style=flat-square)](./constitution.md#article-iii-core-principle-iii---user-experience-consistency)
[![Performance Requirements](https://img.shields.io/badge/Article%20IV-Performance%20Standards-blue?style=flat-square)](./constitution.md#article-iv-core-principle-iv---performance-requirements)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Avalonia UI](https://img.shields.io/badge/Avalonia%20UI-11.3.4-orange?style=flat-square)](https://avaloniaui.net/)
[![MySQL](https://img.shields.io/badge/MySQL-9.4.0-blue?style=flat-square&logo=mysql)](https://www.mysql.com/)
[![Cross-Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20macOS%20%7C%20Linux%20%7C%20Android-lightgrey?style=flat-square)](https://avaloniaui.net/)

---

## 📖 Overview

The **MTM WIP Application** is a constitutional governance-driven, cross-platform manufacturing Work-in-Process management system built with .NET 8 and Avalonia UI. This enterprise-grade application manages inventory operations, manufacturing workflows, and operator transactions for Material Handler Manufacturing (MTM) systems.

### 🏭 Manufacturing Domain

- **Operations**: 90 (Move), 100 (Receive), 110 (Ship), 120 (Transfer)
- **Locations**: FLOOR, RECEIVING, SHIPPING, and custom location codes
- **Transactions**: IN, OUT, TRANSFER with complete audit trails
- **Sessions**: 8+ hour manufacturing session support with operator authentication
- **Inventory**: Real-time part tracking, quantity management, location updates

### 🎯 Technology Stack

- **.NET 8.0** - Cross-platform runtime with single target framework
- **Avalonia UI 11.3.4** - Cross-platform XAML UI framework (Windows, macOS, Linux, Android)
- **MVVM Community Toolkit 8.3.2** - Modern MVVM implementation with source generators
- **MySQL 9.4.0** - Production database with 45+ stored procedures
- **Microsoft.Extensions 9.0.8** - Dependency injection, logging, configuration
- **Material Icons Avalonia** - Material Design iconography

### 🏗️ Architecture

- **Constitutional Governance** - Four core principles enforced through CI/CD
- **MVVM Pattern** - ViewModels, Views, Models separation with Community Toolkit
- **Service Layer** - Centralized business logic with dependency injection
- **Cross-Platform Support** - Native performance on all target platforms
- **Modular Design** - Custom controls, behaviors, converters

## 🏛️ Constitutional Framework

This application operates under a **Constitutional Governance System** with four core principles:

### Article I: Code Quality Excellence

- Nullable reference types enabled
- MVVM Community Toolkit patterns exclusively  
- Centralized error handling
- Comprehensive dependency injection

### Article II: Comprehensive Testing Standards

- Minimum 80% code coverage
- TDD approach where applicable
- Cross-platform feature testing
- Manufacturing domain validation

### Article III: User Experience Consistency  

- Avalonia UI 11.3.4 with proper AXAML syntax
- Material Design iconography
- Theme system integration
- 8+ hour session responsiveness

### Article IV: Performance Requirements

- 30-second database query timeout
- MySQL connection pooling (5-100 connections)
- Sub-100ms UI responsiveness
- Cross-platform performance parity

**📖 Full Constitutional Document**: [constitution.md](./constitution.md)

## 🚀 Quick Start

### Prerequisites

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL Server 9.4.0** - [Download](https://dev.mysql.com/downloads/mysql/)
- **Visual Studio 2022** or **VS Code** with C# Dev Kit

### Installation

```powershell
# Clone repository
git clone https://github.com/Dorotel/MTM_WIP_Application_Avalonia.git
cd MTM_WIP_Application_Avalonia

# Restore dependencies
dotnet restore

# Build application
dotnet build

# Run application
dotnet run
```

### Database Setup

1. Install MySQL Server 9.4.0
2. Update connection string in `Config/appsettings.json`
3. Initialize database with provided stored procedures

### Configuration

- **Development**: `Config/appsettings.Development.json`
- **Production**: `Config/appsettings.json`  
- **Database**: Update `ConnectionStrings:DefaultConnection`

## 🧪 Testing

The application follows comprehensive cross-platform testing standards enforced by constitutional compliance:

```powershell
# Run all tests
dotnet test

# Unit tests only
dotnet test --filter Category=Unit

# Integration tests
dotnet test --filter Category=Integration

# Cross-platform tests  
dotnet test --filter Category=CrossPlatform

# Manufacturing domain tests
dotnet test --filter Category=Manufacturing
```

### Test Coverage Requirements

- **Unit Tests**: 80% minimum coverage (constitutional requirement)
- **Integration Tests**: All service interactions
- **Cross-Platform Tests**: Windows, macOS, Linux validation
- **Manufacturing Tests**: Operations 90/100/110/120 validation

## 📱 Cross-Platform Support

### Supported Platforms

| Platform | Support Level | Notes |
|----------|--------------|-------|
| **Windows** | Full Support | Primary development platform |
| **macOS** | Full Support | Intel and Apple Silicon |
| **Linux** | Full Support | Ubuntu, CentOS, RHEL |
| **Android** | Planned | Mobile manufacturing terminals |

### Platform-Specific Building

```powershell
# Windows (x64)
dotnet publish -r win-x64 --self-contained

# macOS (Intel)
dotnet publish -r osx-x64 --self-contained

# macOS (Apple Silicon)  
dotnet publish -r osx-arm64 --self-contained

# Linux (x64)
dotnet publish -r linux-x64 --self-contained
```

## 🔧 Development

### Code Style

- **C# 12** with nullable reference types enabled
- **MVVM Community Toolkit** patterns exclusively (no ReactiveUI)
- **Dependency injection** throughout application architecture
- **Centralized error handling** via `Services.ErrorHandling.HandleErrorAsync()`

### Development Workflow

1. **Constitutional Compliance**: All changes must comply with constitutional principles
2. **TDD Approach**: Write tests before implementation where applicable
3. **Cross-Platform Validation**: Test on multiple platforms
4. **CI/CD Enforcement**: Constitutional compliance checks must pass

### Key Directories

```bash
/Views                 - Avalonia AXAML views (32+ files)
/ViewModels           - MVVM ViewModels (42+ files)
/Services             - Business logic services (20+ files)
/Models               - Data models and DTOs (12+ files)
/Controls             - Custom Avalonia controls
/Resources/Themes     - AXAML theme files (17+ files)
/.github/instructions - Development instruction library (34+ files)
```

## 📊 Manufacturing Operations

### Core Workflows

- **Inventory Management**: Real-time part tracking and location management
- **Quick Buttons**: Operator-defined transaction shortcuts (10 max per user)
- **Remove Operations**: Bulk inventory removal with validation
- **Master Data**: Parts, locations, operations, and user management
- **Session Management**: 8+ hour operator sessions with activity tracking

### Business Rules

- **Operation Validation**: All transactions require valid operation codes
- **Location Validation**: Location codes must exist in master data
- **Transaction Logging**: Complete audit trail for all inventory changes
- **User Authentication**: Operator validation for all manufacturing operations
- **Session Timeout**: 60-minute inactivity timeout with session recovery

## 🛡️ Security & Performance

### Security Features

- **Connection String Encryption**: Database credentials secured
- **SQL Injection Prevention**: Parameterized queries via stored procedures
- **Input Validation**: Service layer validation for all user inputs
- **Audit Logging**: Complete transaction and error logging

### Performance Standards (Constitutional)

- **Database Timeout**: 30-second maximum query time
- **Connection Pooling**: 5-100 MySQL connections managed
- **Memory Usage**: <512MB working set during normal operations  
- **UI Responsiveness**: <100ms interface lag for all interactions
- **Session Support**: Maintain performance during 8+ hour sessions

## 🤝 Contributing

### Pull Request Requirements

1. **Constitutional Compliance**: All four principles must be upheld
2. **Testing**: Minimum 80% code coverage required
3. **Cross-Platform**: Changes validated on Windows, macOS, Linux
4. **Manufacturing Domain**: Business rules validated for operations
5. **Documentation**: Updates to relevant instruction files

### Constitutional Amendments

Changes to `constitution.md` require:

- **Repository Owner** approval
- **@Agent** approval  
- **5 business day** review period
- **30-day** implementation timeline

## 📚 Documentation

### Core Documentation

- **[Constitution](./constitution.md)** - Governance framework and core principles
- **[AGENTS.md](./AGENTS.md)** - AI development automation tools and patterns
- **[Instruction Library](./.github/instructions/)** - 34+ comprehensive development guides

### Specialized Guides

- **[Testing Standards](./.github/instructions/testing-standards.instructions.md)**
- **[Avalonia UI Guidelines](./.github/instructions/avalonia-ui-guidelines.instructions.md)**
- **[Database Patterns](./.github/instructions/mysql-database-patterns.instructions.md)**
- **[MVVM Community Toolkit](./.github/instructions/mvvm-community-toolkit.instructions.md)**

## 🏆 Constitutional Compliance Recognition

This application is governed by constitutional principles that ensure:

- **🎯 Code Quality Excellence**: Consistent, maintainable, and reliable code
- **🧪 Comprehensive Testing**: Thorough validation across all platforms  
- **🎨 User Experience Consistency**: Intuitive, responsive manufacturing interfaces
- **⚡ Performance Requirements**: Enterprise-grade manufacturing performance

**Constitutional enforcement is automated through CI/CD pipelines and cross-platform validation.**

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

---

## 🏛️ Constitutional Footer

**This application operates under constitutional governance as defined in [constitution.md](./constitution.md). All development activities must comply with the four core constitutional principles: Code Quality Excellence, Comprehensive Testing Standards, User Experience Consistency, and Performance Requirements. Constitutional compliance is enforced through automated CI/CD checks and dual approval processes for amendments.**

**Constitutional Version**: 1.0  
**Last Amendment**: N/A  
**Next Review**: Continuous via CI/CD  
**Governance**: Repository Owner + @Agent Dual Authority
