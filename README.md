# MTM WIP Application Avalonia (.NET 8)

A modern, cross-platform WIP (Work In Progress) Inventory Management System built with Avalonia UI and .NET 8 for Manitowoc Tool and Manufacturing (MTM).

## 🚀 Overview

The MTM WIP Application provides comprehensive inventory management capabilities with real-time tracking, advanced analytics, and modern UI patterns. Built using Avalonia UI with ReactiveUI for MVVM architecture and targeting .NET 8, this application offers:

- **Cross-Platform Compatibility**: Runs on Windows, macOS, and Linux with native performance
- **Modern UI**: Clean, responsive interface with MTM brand colors and Avalonia 11+ controls
- **Real-Time Inventory Tracking**: Live inventory updates and comprehensive transaction logging
- **Advanced Analytics**: Comprehensive reporting and data analysis tools
- **Multi-User Support**: Role-based access control and user management
- **Database Integration**: MySQL backend with comprehensive stored procedures
- **.NET 8 Performance**: Latest framework with improved performance and modern C# features

## 🏗️ Architecture

### Technology Stack
- **.NET 8**: Latest .NET framework with modern C# 12 features and performance improvements
- **Avalonia UI 11+**: Cross-platform UI framework with native look and feel
- **ReactiveUI**: MVVM framework for reactive programming patterns
- **MySQL 8.0+**: Robust database backend with comprehensive stored procedures
- **Dependency Injection**: Built-in .NET 8 DI container with comprehensive service registration
- **Nullable Reference Types**: Full null safety with modern C# patterns

### Project Structure
```
MTM_WIP_Application_Avalonia/
├── Views/                      # Avalonia 11+ AXAML views with modern controls
├── ViewModels/                 # ReactiveUI ViewModels with .NET 8 patterns
├── Models/                     # Data models with record types and nullable references
├── Services/                   # Business logic and data access services
├── Extensions/                 # Service collection and DI extensions
├── Database_Files/             # 🔒 PRODUCTION database files (READ-ONLY)
│   ├── Production_Database_Schema.sql      # Current production schema
│   ├── Existing_Stored_Procedures.sql     # READ-ONLY production procedures
│   └── README_*.md                         # Production documentation
├── Development/                # 🔧 Development files and documentation
│   ├── Database_Files/         # 📝 Development database files (EDITABLE)
│   │   ├── Development_Database_Schema.sql  # Development schema changes
│   │   ├── New_Stored_Procedures.sql       # ✅ 12 NEW PROCEDURES IMPLEMENTED
│   │   ├── Updated_Stored_Procedures.sql   # Ready for future updates
│   │   └── README_*.md                     # Comprehensive documentation
│   ├── Compliance Reports/     # Code compliance and analysis reports
│   ├── UI_Documentation/       # UI component specifications
│   ├── Custom_Prompts/         # Development prompt templates
│   ├── Examples/               # Code examples and patterns
│   └── Docs/                   # Documentation and guides
├── Docs/                       # 📖 HTML documentation and guides
└── .github/                    # GitHub workflows and comprehensive instructions
```

### 🗄️ Database File Organization

#### **Production Files** (`Database_Files/` - READ-ONLY)
- **Purpose**: Current production state - **DO NOT EDIT DIRECTLY**
- **Files**:
  - `Production_Database_Schema.sql` - Production database structure
  - `Existing_Stored_Procedures.sql` - All current production procedures
  - `README_*.md` - Production documentation

#### **Development Files** (`Documentation/Development/Database_Files/` - EDITABLE)
- **Purpose**: Development changes and new procedures
- **Status**: ✅ **CRITICAL FIX #1 COMPLETED - DEVELOPMENT UNBLOCKED**
- **Files**:
  - `Development_Database_Schema.sql` - Schema modifications
  - `New_Stored_Procedures.sql` - ✅ **12 comprehensive procedures implemented**
  - `Updated_Stored_Procedures.sql` - Ready template for future updates
  - `README_NewProcedures.md` - ✅ **Complete documentation with examples**

#### **Database Development Workflow**
1. **New Procedures**: Add to `Documentation/Development/Database_Files/New_Stored_Procedures.sql`
2. **Update Existing**: Copy from `Database_Files/Existing_Stored_Procedures.sql` to `Documentation/Development/Database_Files/Updated_Stored_Procedures.sql`, then modify
3. **Schema Changes**: Update `Documentation/Development/Database_Files/Development_Database_Schema.sql`
4. **Deploy to Production**: Move validated files from Development to Database_Files

## 🎨 UI Design System

### MTM Brand Colors (Avalonia 11+ Implementation)
The application uses a modern purple-based color scheme optimized for Avalonia 11+ theming:

- **Primary Purple**: `#4B45ED` (rgba(75, 69, 237, 1))
- **Magenta Accent**: `#BA45ED` (rgba(186, 69, 237, 1))
- **Secondary Purple**: `#8345ED` (rgba(131, 69, 237, 1))
- **Blue Accent**: `#4574ED` (rgba(69, 116, 237, 1))
- **Pink Accent**: `#ED45E7` (rgba(237, 69, 231, 1))
- **Light Purple**: `#B594ED` (rgba(181, 148, 237, 1))

### Design Patterns (Avalonia 11+)
- **Modern Card-Based Layout**: Clean, organized information presentation with subtle shadows
- **Responsive Sidebar Navigation**: Collapsible navigation with proper accessibility
- **Hero Gradients**: Eye-catching banners using MTM brand gradient
- **TabView Integration**: Modern tab interface with Avalonia 11+ TabView control
- **Responsive Design**: Adapts to different screen sizes with mobile-first approach

## 📊 Key Features

### Inventory Management (.NET 8 Enhanced)
- **Real-Time Tracking**: Live inventory updates with SignalR integration
- **Advanced Search**: Multi-criteria search with LINQ and modern C# patterns
- **Batch Operations**: Efficient bulk operations using async enumerable
- **Location Management**: Multi-location tracking with hierarchical organization
- **Part Management**: Comprehensive part database with record types

### Transaction System
- **Transaction Types**: IN, OUT, and TRANSFER operations with clear business logic
- **Operation Tracking**: Workflow step identification (90, 100, 110, etc.)
- **Audit Trail**: Complete transaction history with comprehensive logging
- **Quick Actions**: Customizable quick buttons for frequent operations
- **Business Rule Validation**: TransactionType determined by user intent, NOT operation numbers

### User Management (.NET 8 Patterns)
- **Role-Based Access**: Granular permissions with .NET 8 authorization patterns
- **User Preferences**: Customizable themes and settings with strong typing
- **Activity Tracking**: User action logging with structured logging
- **Multi-User Environment**: Concurrent user support with conflict resolution

### Reporting & Analytics
- **Real-Time Reports**: Live data with automatic refresh using reactive patterns
- **Custom Reports**: User-defined parameters with modern C# expressions
- **Export Capabilities**: PDF, Excel, and CSV export with async processing
- **Data Visualization**: Charts and graphs using modern charting libraries

## 💾 Database Architecture (.NET 8 Enhanced)

### 🚨 Core Principles

#### **CRITICAL: NO HARD-CODED SQL RULE**
**ALL database operations MUST use stored procedures only. Zero hard-coded SQL commands allowed in application code.**

```csharp
// ✅ CORRECT - Using stored procedures with .NET 8 patterns
public async Task<Result<InventoryItem>> AddInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
{
    var parameters = new Dictionary<string, object> 
    { 
        ["p_PartID"] = item.PartId,
        ["p_OperationID"] = item.Operation,
        ["p_LocationID"] = item.Location,
        ["p_Quantity"] = item.Quantity,
        ["p_UserID"] = item.User
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Model_AppVariables.ConnectionString,
        "inv_inventory_Add_Item_Enhanced",
        parameters
    );

    return result.Status switch
    {
        0 => Result<InventoryItem>.Success(item),
        1 => Result<InventoryItem>.Warning(result.Message),
        _ => Result<InventoryItem>.Failure(result.Message)
    };
}

// ❌ PROHIBITED - Direct SQL
// var query = "SELECT * FROM inventory WHERE part_id = @partId"; // NEVER DO THIS
```

### 🚀 **Enhanced Stored Procedures Available**

#### **✅ CRITICAL FIX #1 COMPLETED - 12 NEW PROCEDURES IMPLEMENTED**

The development database now includes **12 comprehensive stored procedures** with:
- **Standardized Error Handling**: All procedures return `p_Status` and `p_ErrorMsg`
- **Input Validation**: Complete parameter validation with business rule enforcement
- **Transaction Management**: Proper START/COMMIT/ROLLBACK patterns
- **Performance Optimization**: Indexed queries and efficient execution plans
- **Audit Trail**: Complete logging for troubleshooting and compliance

#### **Service Layer Integration Ready (.NET 8)**

All procedures designed for seamless integration with .NET 8 service layer:

```csharp
// Example: Enhanced inventory service with .NET 8 patterns
public sealed class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly IDbConnectionFactory _connectionFactory;

    public InventoryService(ILogger<InventoryService> logger, IDbConnectionFactory connectionFactory)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<InventoryItem>> AddInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity("AddInventoryItem");
        activity?.SetTag("partId", item.PartId);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = item.PartId,
                ["p_OperationID"] = item.Operation,
                ["p_LocationID"] = item.Location,
                ["p_Quantity"] = item.Quantity,
                ["p_UnitCost"] = item.UnitCost,
                ["p_Notes"] = item.Notes ?? string.Empty,
                ["p_UserID"] = item.User
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                "inv_inventory_Add_Item_Enhanced",
                parameters
            );

            return result.Status switch
            {
                0 => Result<InventoryItem>.Success(item),
                1 => Result<InventoryItem>.Warning(result.Message),
                _ => Result<InventoryItem>.Failure(result.Message)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add inventory item {PartId}", item.PartId);
            return Result<InventoryItem>.Failure($"Database error: {ex.Message}");
        }
    }
}
```

### Business Logic Rules (.NET 8)

#### **CRITICAL: Transaction Type Determination**
**TransactionType is determined by USER INTENT, NOT the Operation number.**

```csharp
// Modern C# pattern matching for transaction types
public enum UserAction { AddStock, RemoveStock, TransferStock }

public static TransactionType GetTransactionTypeForAction(UserAction action) => action switch
{
    UserAction.AddStock => TransactionType.IN,      // User adding stock
    UserAction.RemoveStock => TransactionType.OUT,  // User removing stock
    UserAction.TransferStock => TransactionType.TRANSFER, // User moving stock
    _ => throw new ArgumentException($"Unknown user action: {action}")
};

// Operation numbers are workflow step identifiers only
public record InventoryOperation(string OperationCode, string Description, int Sequence)
{
    public static readonly InventoryOperation[] StandardOperations = 
    [
        new("90", "Receiving", 1),
        new("100", "Initial Processing", 2),
        new("110", "Quality Control", 3),
        new("120", "Final Assembly", 4)
    ];
}
```

## 🚀 Getting Started (.NET 8)

### Prerequisites
- **.NET 8 SDK** or later (required for modern C# features)
- **MySQL Server 8.0** or later
- **Visual Studio 2022** (17.8+) or **VS Code** with C# extension
- **Git** for version control

### Installation
1. **Clone the repository**
   ```bash
   git clone https://github.com/Dorotel/MTM_WIP_Application_Avalonia.git
   cd MTM_WIP_Application_Avalonia
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database connection**
   - Update connection string in `appsettings.json`
   - Run database schema scripts from `Database_Files/`

4. **Build and run**
   ```bash
   dotnet build
   dotnet run
   ```

### Database Setup (.NET 8 Enhanced)

#### **Production Database Setup**
```bash
# 1. Create production database with current schema
mysql -u username -p database_name < Database_Files/Production_Database_Schema.sql

# 2. Install all existing stored procedures
mysql -u username -p database_name < Database_Files/Existing_Stored_Procedures.sql
```

#### **Development Database Setup** ✅ **ENHANCED WITH NEW PROCEDURES**
```bash
# 1. Start with production schema
mysql -u username -p database_name < Database_Files/Production_Database_Schema.sql
mysql -u username -p database_name < Database_Files/Existing_Stored_Procedures.sql

# 2. Apply development changes
mysql -u username -p database_name < Documentation/Development/Database_Files/Development_Database_Schema.sql

# 3. Add NEW comprehensive procedures ✅ 12 PROCEDURES READY
mysql -u username -p database_name < Documentation/Development/Database_Files/New_Stored_Procedures.sql

# 4. Apply updated procedures (when available)
mysql -u username -p database_name < Documentation/Development/Database_Files/Updated_Stored_Procedures.sql
```

### Development Setup (.NET 8)
1. **IDE Configuration**
   - Open solution in Visual Studio 2022 (17.8+ for .NET 8 support)
   - Set startup project to `MTM_WIP_Application_Avalonia`
   - Configure debugging settings and enable nullable reference types

2. **Development Dependencies**
   ```bash
   # Install required global tools
   dotnet tool install --global dotnet-ef
   dotnet tool install --global Microsoft.Web.LibraryManager.Cli
   ```

## 🔧 Development Guidelines (.NET 8)

### 🚨 **CRITICAL: Dependency Injection Setup (.NET 8)**

**ALWAYS use `services.AddMTMServices(configuration)` - NEVER register MTM services individually!**

```csharp
// Program.cs with .NET 8 top-level statements
using MTM.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ✅ CRITICAL: Use comprehensive service registration
builder.Services.AddMTMServices(builder.Configuration);

// ✅ Override Avalonia-specific services AFTER AddMTMServices
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
builder.Services.AddSingleton<IApplicationStateService, ApplicationStateService>();

// ✅ Infrastructure Services
builder.Services.AddSingleton<INavigationService, NavigationService>();

// ✅ ViewModels (Transient - new instance each time)
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<InventoryViewModel>();
// ... register all ViewModels

var app = builder.Build();
```

### Code Standards (.NET 8)
- **MVVM Pattern**: Strict separation using ReactiveUI with .NET 8 patterns
- **Async/Await**: All I/O operations with CancellationToken support
- **Nullable Reference Types**: Enable and properly handle null safety
- **File-scoped Namespaces**: Use modern namespace syntax
- **Record Types**: Use for immutable data transfer objects
- **Pattern Matching**: Utilize switch expressions and enhanced patterns
- **Dependency Injection**: Use comprehensive AddMTMServices extension

### Database Development Rules (.NET 8 Enhanced)
- **New Procedures**: Add to `Documentation/Development/Database_Files/New_Stored_Procedures.sql` ✅ **12 IMPLEMENTED**
- **Update Existing**: Copy to `Documentation/Development/Database_Files/Updated_Stored_Procedures.sql` and modify
- **Parameter Validation**: ✅ **ALL NEW PROCEDURES INCLUDE COMPREHENSIVE VALIDATION**
- **Transaction Management**: ✅ **ALL NEW PROCEDURES USE PROPER TRANSACTION PATTERNS**
- **Error Handling**: ✅ **STANDARDIZED ERROR HANDLING WITH STATUS CODES**
- **Performance**: Use async patterns and efficient query execution

### UI Development (.NET 8 + Avalonia 11+)
- **Component Documentation**: Every UI component has detailed `.instructions.md`
- **Modern Controls**: Use Avalonia 11+ controls (TabView, NumberBox, InfoBadge)
- **Responsive Design**: Mobile-first approach with adaptive layouts
- **Accessibility**: Implement proper keyboard navigation and screen reader support
- **Performance**: Use virtualization and compiled bindings

## 📚 Documentation (.NET 8 Enhanced)

### Comprehensive Documentation ✅ **UPDATED FOR .NET 8**
- **Framework Compliance**: All documentation updated for .NET 8 and Avalonia 11+
- **GitHub Instructions**: Complete development guidelines in `.github/copilot-instructions.md`
- **Database Documentation**: Enhanced with new procedures and .NET 8 patterns
- **Code Examples**: Updated with modern C# 12 features and patterns
- **UI Documentation**: Avalonia 11+ patterns and responsive design guides

### Key Documentation Files ✅ **MODERNIZED**
- **Root README**: This file - Complete .NET 8 setup and architecture
- **ViewModels Documentation**: `ViewModels/README.md` - ReactiveUI with .NET 8 patterns
- **Views Documentation**: `Views/README.md` - Avalonia 11+ modern UI patterns
- **Models Documentation**: `Models/README.md` - Record types and nullable references
- **GitHub Instructions**: `.github/copilot-instructions.md` - Comprehensive development guide

## 🤝 Contributing (.NET 8)

### Development Workflow
1. **Create Feature Branch**: `git checkout -b feature/your-feature-name`
2. **Follow .NET 8 Standards**: Use modern C# features and nullable reference types
3. **Database Changes**: Work only in `Documentation/Development/Database_Files/`
4. **Add Documentation**: Update relevant `.md` files
5. **Test Thoroughly**: Ensure compatibility with .NET 8 and Avalonia 11+
6. **Submit Pull Request**: Include detailed description of changes

### Code Quality Standards (.NET 8)
- **Nullable Reference Types**: Enable and handle properly
- **Modern C# Features**: Use file-scoped namespaces, record types, pattern matching
- **Performance**: Utilize .NET 8 performance improvements
- **Async Patterns**: Proper async/await with CancellationToken
- **Error Handling**: Comprehensive error handling with Result patterns

## 📄 License

This project is proprietary software owned by Manitowoc Tool and Manufacturing (MTM). All rights reserved.

## 🆘 Support

### Getting Help
- **GitHub Issues**: Report bugs and request features
- **Documentation**: Check comprehensive guides in `Docs/` and `.github/`
- **Database Issues**: Review enhanced procedure documentation
- **Code Examples**: Modern .NET 8 patterns in `Documentation/Development/Examples/`

---

## 🏷️ Project Status

**Current Version**: v2.0.0 (.NET 8)  
**Development Status**: ✅ **Active Development - Fully Modernized**  
**Platform Support**: Windows, macOS, Linux  
**Database Support**: MySQL 8.0+  
**Framework**: .NET 8 / Avalonia UI 11+

### Recent Updates ✅ **MAJOR MODERNIZATION COMPLETED**
- ✅ **.NET 8 Migration**: Complete framework upgrade with modern C# 12 features
- ✅ **Avalonia 11+ Upgrade**: Modern UI controls and enhanced performance
- ✅ **Nullable Reference Types**: Full null safety implementation
- ✅ **Enhanced Documentation**: All documentation updated for framework compliance
- ✅ **Modern Patterns**: Record types, pattern matching, file-scoped namespaces
- ✅ **Performance Improvements**: .NET 8 optimizations and async enumerables
- ✅ **Database Enhancement**: 12 comprehensive stored procedures with .NET 8 integration

### Development Readiness (.NET 8)
- ✅ **Framework Compliance**: Full .NET 8 and Avalonia 11+ compliance
- ✅ **Database Layer**: Complete with modern async patterns
- ✅ **Service Layer**: Enhanced with .NET 8 dependency injection
- ✅ **UI Layer**: Modern Avalonia 11+ controls and responsive design
- ✅ **Documentation**: Comprehensive and accurate for current implementation

---

*Built with ❤️ for MTM using .NET 8, Avalonia UI 11+, and modern cross-platform technologies.*