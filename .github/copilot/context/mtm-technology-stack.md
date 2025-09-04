# MTM Technology Stack Context

## 🛠️ Technology Stack Specifications

### **Core Platform**
```
.NET Version: 8.0 (LTS)
C# Version: 12 with nullable reference types
Target Framework: net8.0
Platform Support: Windows, Linux, macOS (Avalonia cross-platform)
```

### **UI Framework - Avalonia UI**
```
Avalonia Version: 11.3.4
UI Pattern: MVVM with minimal code-behind
Namespace: xmlns="https://github.com/avaloniaui"
Cross-Platform: Native performance on all platforms
Theme System: Dynamic theming with MTM color schemes
```

**Critical Avalonia Differences from WPF:**
- Use `TextBlock` instead of `Label`
- Use `Flyout` instead of `Popup`  
- Use `x:Name` only (never `Name` on Grid definitions)
- Different event handling patterns
- No `DependencyProperty` - use standard properties with INotifyPropertyChanged

### **MVVM Framework - MVVM Community Toolkit**
```
Version: 8.3.2
Pattern: Source generator-based property/command generation
Observable Properties: [ObservableProperty] attribute
Commands: [RelayCommand] attribute
Base Classes: ObservableObject, BaseViewModel
Validation: Built-in validation support
```

**Migration from ReactiveUI Completed:**
- ❌ ReactiveObject → ✅ ObservableObject
- ❌ ReactiveCommand → ✅ RelayCommand
- ❌ RaiseAndSetIfChanged → ✅ Source generators
- ❌ Reactive subscriptions → ✅ Standard event handling

### **Database Technology - MySQL**
```
MySQL Version: 9.4.0
Connection Package: MySql.Data
Pattern: Stored procedures ONLY
Helper: Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()
Connection Management: Helper_Database_Variables.GetConnectionString()
```

**Database Architecture:**
- **Development Database**: `mtm_wip_application_test`
- **Production Database**: `mtm_wip_application`
- **45+ Stored Procedures**: Complete business logic in database
- **No Direct SQL**: All operations via stored procedures

### **Dependency Injection - Microsoft Extensions**
```
Version: 9.0.8
Container: Microsoft.Extensions.DependencyInjection
Configuration: Microsoft.Extensions.Configuration
Logging: Microsoft.Extensions.Logging
Hosting: Microsoft.Extensions.Hosting
```

**Service Lifetimes:**
- **Singleton**: Configuration services, theme services
- **Scoped**: Database services, business services
- **Transient**: ViewModels, short-lived services

### **Logging and Configuration**
```
Logging: Microsoft.Extensions.Logging with structured logging
Configuration: appsettings.json with environment-specific overrides
Settings: appsettings.Development.json for development
User Settings: Database-stored per-user configuration
```

### **Project Structure**
```
MTM_WIP_Application_Avalonia/
├── Core/                 # Application startup, themes
├── ViewModels/           # MVVM Community Toolkit ViewModels
├── Views/               # Avalonia UserControls and Windows
├── Services/            # Business services with DI
├── Models/              # Data models and DTOs
├── Controls/            # Custom UserControls
├── Helpers/             # Database and utility helpers
├── Extensions/          # Extension methods and utilities
├── Resources/           # Themes, images, resources
└── Documentation/       # Comprehensive documentation
```

### **Build and Development Tools**
```
IDE Support: Visual Studio 2022, VS Code, JetBrains Rider
Build System: MSBuild with .NET SDK
Package Manager: NuGet
Version Control: Git with GitHub
CI/CD: GitHub Actions (when configured)
```

### **Key NuGet Packages**
```xml
<PackageReference Include="Avalonia" Version="11.3.4" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.8" />
<PackageReference Include="MySql.Data" Version="9.4.0" />
```

### **Development Environment**
```
Development Database: localhost:3306/mtm_wip_application_test
Production Database: Configured per deployment environment
Connection Strings: Stored in appsettings.json with environment overrides
User Settings: Database-stored with fallback to defaults
```

### **Performance Characteristics**
```
Startup Time: <3 seconds on modern hardware
Memory Usage: ~50MB base, scales with data
UI Response: <100ms for most operations
Database: Sub-second response for typical queries
Cross-Platform: Native performance on all supported platforms
```

### **Security Features**
```
Authentication: User-based with role permissions
Database Security: Parameterized stored procedures prevent SQL injection
Error Handling: Centralized error management with logging
Audit Trail: Complete transaction logging
Configuration: Sensitive settings in secure configuration
```

### **Testing Strategy**
```
Unit Testing: xUnit framework (when implemented)
Integration Testing: Database operation testing
UI Testing: Avalonia UI testing framework
Manual Testing: Comprehensive test scenarios
Performance Testing: Load testing for database operations
```

### **Deployment Model**
```
Deployment: Self-contained .NET executable
Platform Packages: Windows MSI, Linux packages, macOS bundle
Dependencies: .NET 8 runtime (can be self-contained)
Database Setup: SQL scripts for schema and stored procedures
Configuration: Environment-specific settings files
```

### **Version Management**
```
Application Versioning: Semantic versioning (Major.Minor.Patch)
Database Versioning: Migration scripts with version tracking
Dependencies: Locked to specific versions for stability
Release Process: Automated builds with GitHub Actions
```

### **Monitoring and Diagnostics**
```
Logging: Structured logging with configurable levels
Error Tracking: Centralized error handling and storage
Performance: Built-in performance counters
Health Checks: Application health monitoring endpoints
Diagnostics: Debug information in development builds
```

### **Integration Capabilities**
```
REST APIs: HTTP client support for external integrations  
File Import/Export: Excel, CSV data exchange
Email: SMTP support for notifications
Reporting: Built-in reporting with export capabilities
Extensibility: Plugin architecture for custom modules
```