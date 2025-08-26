# MTM WIP Application Avalonia

A modern, cross-platform WIP (Work In Progress) Inventory Management System built with Avalonia UI for Manitowoc Tool and Manufacturing (MTM).

## 🚀 Overview

The MTM WIP Application provides comprehensive inventory management capabilities with real-time tracking, advanced analytics, and modern UI patterns. Built using Avalonia UI with ReactiveUI for MVVM architecture, this application offers:

- **Cross-Platform Compatibility**: Runs on Windows, macOS, and Linux
- **Modern UI**: Clean, responsive interface with MTM brand colors
- **Real-Time Inventory Tracking**: Live inventory updates and transaction logging
- **Advanced Analytics**: Comprehensive reporting and data analysis tools
- **Multi-User Support**: Role-based access control and user management
- **Database Integration**: MySQL backend with stored procedures for all operations

## 🏗️ Architecture

### Technology Stack
- **.NET 8**: Latest .NET framework for performance and modern features
- **Avalonia UI**: Cross-platform UI framework with native look and feel
- **ReactiveUI**: MVVM framework for reactive programming patterns
- **MySQL**: Robust database backend with stored procedures
- **Dependency Injection**: Built-in .NET DI container for loose coupling

### Project Structure
```
MTM_WIP_Application_Avalonia/
├── Views/                      # Avalonia AXAML views
├── ViewModels/                 # ViewModels using ReactiveUI
├── Models/                     # Data models and business entities
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
└── .github/                    # GitHub workflows and instructions
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

### MTM Brand Colors
The application uses a modern purple-based color scheme:

- **Primary Purple**: `#4B45ED` (rgba(75, 69, 237, 1))
- **Magenta Accent**: `#BA45ED` (rgba(186, 69, 237, 1))
- **Secondary Purple**: `#8345ED` (rgba(131, 69, 237, 1))
- **Blue Accent**: `#4574ED` (rgba(69, 116, 237, 1))
- **Pink Accent**: `#ED45E7` (rgba(237, 69, 231, 1))
- **Light Purple**: `#B594ED` (rgba(181, 148, 237, 1))

### Design Patterns
- **Modern Card-Based Layout**: Clean, organized information presentation
- **Sidebar Navigation**: Intuitive navigation with collapsible sections
- **Hero Gradients**: Eye-catching banners and call-to-action areas
- **Responsive Design**: Adapts to different screen sizes and resolutions

## 📊 Key Features

### Inventory Management
- **Real-Time Tracking**: Live inventory updates with immediate feedback
- **Advanced Search**: Multi-criteria search with filtering and sorting
- **Batch Operations**: Efficient bulk inventory operations
- **Location Management**: Multi-location inventory tracking
- **Part Management**: Comprehensive part database with relationships

### Transaction System
- **Transaction Types**: IN, OUT, and TRANSFER operations
- **Operation Tracking**: Workflow step identification (90, 100, 110, etc.)
- **Audit Trail**: Complete transaction history with user attribution
- **Quick Actions**: Customizable quick buttons for frequent operations

### User Management
- **Role-Based Access**: Granular permissions and access control
- **User Preferences**: Customizable themes, settings, and shortcuts
- **Activity Tracking**: User action logging and analytics
- **Multi-User Environment**: Concurrent user support with conflict resolution

### Reporting & Analytics
- **Real-Time Reports**: Live data with automatic refresh
- **Custom Reports**: User-defined report parameters and filters
- **Export Capabilities**: PDF, Excel, and CSV export options
- **Data Visualization**: Charts and graphs for trend analysis

## 💾 Database Architecture

### 🚨 Core Principles

#### **CRITICAL: NO HARD-CODED SQL RULE**
**ALL database operations MUST use stored procedures only. Zero hard-coded SQL commands allowed in application code.**

```csharp
// ✅ CORRECT - Using stored procedures
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Add_Item_Enhanced",
    new Dictionary<string, object> 
    { 
        ["p_PartID"] = partId,
        ["p_OperationID"] = operationId,
        ["p_LocationID"] = locationId,
        ["p_Quantity"] = quantity,
        ["p_UserID"] = userId
    }
);

// ❌ PROHIBITED - Direct SQL
var query = "SELECT * FROM inventory WHERE part_id = @partId"; // NEVER DO THIS
```

### 🚀 **NEW: Enhanced Stored Procedures Available**

#### **✅ CRITICAL FIX #1 COMPLETED - 12 NEW PROCEDURES IMPLEMENTED**

The development database now includes **12 comprehensive stored procedures** with:
- **Standardized Error Handling**: All procedures return `p_Status` and `p_ErrorMsg`
- **Input Validation**: Complete parameter validation for all procedures
- **Transaction Management**: Proper START/COMMIT/ROLLBACK patterns
- **Business Rule Enforcement**: MTM-specific validation and constraints
- **Audit Trail**: Complete logging for troubleshooting and compliance

#### **Available Procedures**:

**Enhanced Inventory Management:**
1. `inv_inventory_Add_Item_Enhanced` - Enhanced add with full error handling
2. `inv_inventory_Remove_Item_Enhanced` - Enhanced remove with stock validation
3. `inv_inventory_Transfer_Item_New` - Transfer between locations

**Inventory Queries:**
4. `inv_inventory_Get_ByLocation_New` - Get all items by location
5. `inv_inventory_Get_ByOperation_New` - Get all items by operation
6. `inv_part_Get_Info_New` - Get detailed part information
7. `inv_inventory_Get_Summary_New` - Get inventory summary

**Validation & Utilities:**
8. `inv_inventory_Validate_Stock_New` - Validate sufficient stock
9. `inv_transaction_Log_New` - Log all inventory transactions
10. `inv_location_Validate_New` - Validate location exists
11. `inv_operation_Validate_New` - Validate operation exists
12. `sys_user_Validate_New` - Validate user exists

#### **Service Layer Integration Ready**

All procedures designed for seamless integration with existing service layer:

```csharp
// Example: Add inventory using new enhanced procedure
var parameters = new Dictionary<string, object>
{
    ["p_PartID"] = "PART001",
    ["p_OperationID"] = "90",
    ["p_LocationID"] = "RECEIVING",
    ["p_Quantity"] = 100,
    ["p_UnitCost"] = 5.25m,
    ["p_ReferenceNumber"] = "PO12345",
    ["p_Notes"] = "Initial stock receipt",
    ["p_UserID"] = "admin"
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Add_Item_Enhanced",
    parameters
);

// Standard status handling
if (result.Status == 0)
{
    // Success
}
else if (result.Status == 1)
{
    // Warning (validation issue)
    ShowWarning(result.Message);
}
else
{
    // Error
    ShowError(result.Message);
}
```

### Business Logic Rules

#### **CRITICAL: Transaction Type Determination**
**TransactionType is determined by USER INTENT, NOT the Operation number.**

- **IN**: User is adding stock to inventory (regardless of operation number)
- **OUT**: User is removing stock from inventory (regardless of operation number)  
- **TRANSFER**: User is moving stock between locations (regardless of operation number)

Operation numbers ("90", "100", "110", etc.) are **workflow step identifiers** only.

#### **Database File Management**
```
🔒 Database_Files/ (Production - READ ONLY)
├── Production_Database_Schema.sql          # Current production schema
├── Existing_Stored_Procedures.sql         # READ ONLY - All production procedures
└── README_*.md                            # Production documentation

🔧 Documentation/Development/Database_Files/ (Development - EDITABLE) ✅ UPDATED
├── Development_Database_Schema.sql        # Schema changes for development
├── New_Stored_Procedures.sql             # ✅ 12 NEW PROCEDURES IMPLEMENTED
├── Updated_Stored_Procedures.sql         # ✅ Ready template for future updates
├── README_NewProcedures.md               # ✅ COMPREHENSIVE DOCUMENTATION
└── README_*.md                           # Development documentation
```

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK or later
- MySQL Server 8.0 or later
- Visual Studio 2022 or VS Code with C# extension

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

### Database Setup

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

### Development Setup
1. **IDE Configuration**
   - Open solution in Visual Studio 2022
   - Set startup project to `MTM_WIP_Application_Avalonia`
   - Configure debugging settings

2. **Database Development** ✅ **NOW UNBLOCKED**
   - Always work in `Documentation/Development/Database_Files/` for changes
   - Never edit files in `Database_Files/` (production)
   - ✅ **NEW PROCEDURES AVAILABLE**: All standard inventory operations implemented
   - Test all changes thoroughly before deployment

## 🔧 Development Guidelines

### 🚨 **CRITICAL: Dependency Injection Setup**

**ALWAYS use `services.AddMTMServices(configuration)` - NEVER register MTM services individually!**

The application has a complex dependency structure that MUST be properly configured to avoid runtime errors.

#### ❌ **Fatal Error Pattern (Will Fail)**
```csharp
// This WILL FAIL at runtime - missing dependencies!
services.AddScoped<MTM_Shared_Logic.Services.IInventoryService, MTM_Shared_Logic.Services.InventoryService>();
// Error: Unable to resolve service for type 'MTM_Shared_Logic.Core.Services.IValidationService'
```

#### ✅ **Required Pattern**
```csharp
// This registers ALL required dependencies correctly
services.AddMTMServices(configuration);
```

#### **🔧 Complete DI Setup Guide**
See [`Documentation/Development/DependencyInjection/README_DependencyInjection.md`](Documentation/Development/DependencyInjection/README_DependencyInjection.md) and [`Documentation/Development/DependencyInjection/DI_Troubleshooting_Guide.md`](Documentation/Development/DependencyInjection/DI_Troubleshooting_Guide.md) for comprehensive setup instructions and error prevention.

#### **Quick Reference - Required Setup:**
1. **Using Statement**: `using MTM_Shared_Logic.Extensions;` in Program.cs
2. **Service Registration**: `services.AddMTMServices(configuration);` first
3. **ViewModel Registration**: ALL ViewModels must be registered individually
4. **Avalonia Overrides**: Register AFTER AddMTMServices

### Code Standards
- **MVVM Pattern**: Strict separation of concerns with ViewModels
- **Async/Await**: All I/O operations must be asynchronous
- **Dependency Injection**: Use DI for all service dependencies (see critical setup above)
- **ReactiveUI**: Use reactive programming patterns for UI interactions
- **Error Handling**: Comprehensive error handling with user-friendly messages

### Database Development Rules ✅ **ENHANCED WITH NEW PROCEDURES**
- **New Procedures**: Add to `Documentation/Development/Database_Files/New_Stored_Procedures.sql` ✅ **12 IMPLEMENTED**
- **Update Existing**: Copy to `Documentation/Development/Database_Files/Updated_Stored_Procedures.sql` and modify ✅ **TEMPLATE READY**
- **Never Edit**: `Database_Files/Existing_Stored_Procedures.sql` is read-only
- **Parameter Validation**: ✅ **ALL NEW PROCEDURES INCLUDE COMPREHENSIVE VALIDATION**
- **Transaction Management**: ✅ **ALL NEW PROCEDURES USE PROPER TRANSACTION PATTERNS**
- **Error Handling**: ✅ **STANDARDIZED ERROR HANDLING IMPLEMENTED**
- **Security**: All procedures must prevent SQL injection and validate permissions

### UI Development
- **Component Documentation**: Every UI component has detailed `.instructions.md`
- **Avalonia Patterns**: Use modern Avalonia UI patterns and controls
- **Responsive Design**: Support multiple screen sizes and DPI settings
- **Accessibility**: Implement proper keyboard navigation and screen reader support

## 📚 Documentation

### Comprehensive Documentation ✅ **ENHANCED**
- **Database Files**: Complete documentation in both `Database_Files/README_*.md` and `Documentation/Development/Database_Files/README_*.md`
- **NEW PROCEDURES**: ✅ **Complete documentation in `Documentation/Development/Database_Files/README_NewProcedures.md`**
- **UI Components**: Detailed specifications in `Documentation/Development/UI_Documentation/`
- **Custom Prompts**: Development templates in `Development/Custom_Prompts/`
- **Code Examples**: Usage patterns in `Documentation/Development/Examples/`
- **HTML Guides**: User-friendly documentation in `Docs/`

### Key Documentation Files ✅ **UPDATED**
- **Production Database**: `Database_Files/README.md` - Production database overview
- **Development Database**: `Documentation/Development/Database_Files/README.md` - Development workflow
- **NEW PROCEDURES**: ✅ **`Documentation/Development/Database_Files/README_NewProcedures.md` - Complete procedure documentation**
- **GitHub Instructions**: `.github/copilot-instructions.md` - Complete development guidelines
- **Code Compliance**: `Compliance Reports/` - Automated code analysis

## 🤝 Contributing

### Development Workflow
1. **Create Feature Branch**: `git checkout -b feature/your-feature-name`
2. **Follow Code Standards**: Use established patterns and conventions
3. **Database Changes**: Work only in `Documentation/Development/Database_Files/`
4. **Add Documentation**: Update relevant `.md` files
5. **Test Thoroughly**: Ensure all functionality works as expected
6. **Submit Pull Request**: Include detailed description of changes

### Database Contribution Rules ✅ **ENHANCED**
- **🚫 NEVER** edit files in `Database_Files/` directly
- **✅ ALWAYS** make changes in `Documentation/Development/Database_Files/`
- **✅ ALWAYS** test procedures thoroughly before requesting deployment
- **✅ ALWAYS** update documentation when adding/modifying procedures
- **✅ NEW**: Use standardized error handling pattern from `New_Stored_Procedures.sql`
- **✅ NEW**: Follow MTM business logic patterns for TransactionType determination

### Commit Guidelines
- **Conventional Commits**: Use structured commit messages
- **Clear Descriptions**: Explain what and why, not just how
- **Reference Issues**: Link to related GitHub issues when applicable

## 📄 License

This project is proprietary software owned by Manitowoc Tool and Manufacturing (MTM). All rights reserved.

## 🆘 Support

### Getting Help
- **GitHub Issues**: Report bugs and request features
- **Documentation**: Check `Docs/` and `Development/` for detailed guides
- **Database Issues**: Review `Database_Files/README.md` and `Documentation/Development/Database_Files/README_NewProcedures.md`
- **Code Examples**: Review `Documentation/Development/Examples/` for usage patterns
- **NEW PROCEDURES**: See `Documentation/Development/Database_Files/README_NewProcedures.md` for complete documentation

### Contact Information
- **Project Maintainer**: [Contact Information]
- **MTM Support**: [Support Contact]
- **Developer Team**: [Team Contact]

---

## 🏷️ Project Status

**Current Version**: v1.0.0  
**Development Status**: ✅ **Active Development - Database Unblocked**  
**Platform Support**: Windows, macOS, Linux  
**Database Support**: MySQL 8.0+  
**Framework**: .NET 8 / Avalonia UI

### Recent Updates ✅ **MAJOR DATABASE ENHANCEMENT**
- ✅ **CRITICAL FIX #1 COMPLETED**: Empty development stored procedures resolved
- ✅ **12 Comprehensive Procedures**: All standard inventory operations implemented
- ✅ **Standardized Error Handling**: All procedures use consistent error handling pattern
- ✅ **Complete Documentation**: Comprehensive procedure documentation with examples
- ✅ **Service Layer Ready**: All procedures designed for seamless integration
- ✅ **Development Unblocked**: All new database functionality development can now proceed
- ✅ **Database File Organization**: Separated production and development database files
- ✅ **Stored Procedure Enforcement**: Implemented "No Hard-Coded SQL" rule

### Development Readiness
- ✅ **Database Layer**: Complete with 12 comprehensive stored procedures
- ⏭️ **Service Layer**: Ready for implementation using new procedures  
- ⏭️ **UI Layer**: Ready for binding to service layer methods
- ⏭️ **Error Handling**: Ready for UI error handling using standardized status codes

---

*Built with ❤️ for MTM by the development team using modern cross-platform technologies.*