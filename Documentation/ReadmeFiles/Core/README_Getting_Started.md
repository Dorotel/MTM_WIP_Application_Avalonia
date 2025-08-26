# 🚀 Getting Started with MTM WIP Application

This guide will help you set up and run the MTM WIP Inventory Management System on your development machine.

## 📋 **Prerequisites**

### **Required Software**
- **.NET 8 SDK or later** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL Server 8.0 or later** - [Download here](https://dev.mysql.com/downloads/mysql/)
- **Git** - [Download here](https://git-scm.com/downloads)

### **Recommended Development Tools**
- **Visual Studio 2022** (Windows) - [Download here](https://visualstudio.microsoft.com/vs/)
- **VS Code with C# extension** (Cross-platform) - [Download here](https://code.visualstudio.com/)
- **MySQL Workbench** - Database management tool

### **System Requirements**
- **OS**: Windows 10/11, macOS 10.15+, or Linux (Ubuntu 18.04+)
- **RAM**: 4GB minimum, 8GB recommended
- **Storage**: 2GB free space for development environment
- **Display**: 1024x768 minimum resolution

## 📥 **Installation**

### **Step 1: Clone the Repository**
```bash
# Clone the repository
git clone https://github.com/Dorotel/MTM_WIP_Application_Avalonia.git

# Navigate to the project directory
cd MTM_WIP_Application_Avalonia
```

### **Step 2: Verify .NET Installation**
```bash
# Check .NET version (should be 8.0 or later)
dotnet --version

# List installed SDKs
dotnet --list-sdks
```

### **Step 3: Restore Dependencies**
```bash
# Restore NuGet packages
dotnet restore

# This will download all required packages:
# - Avalonia UI framework
# - ReactiveUI for MVVM
# - MySQL connector
# - Logging and configuration packages
```

### **Step 4: Database Setup**

#### **Option A: Production Database Setup**
```bash
# 1. Create the database
mysql -u root -p -e "CREATE DATABASE mtm_wip_inventory;"

# 2. Install production schema
mysql -u root -p mtm_wip_inventory < Database_Files/Production_Database_Schema.sql

# 3. Install existing stored procedures
mysql -u root -p mtm_wip_inventory < Database_Files/Existing_Stored_Procedures.sql
```

#### **Option B: Development Database Setup** ✅ **Recommended**
```bash
# 1. Create the database
mysql -u root -p -e "CREATE DATABASE mtm_wip_inventory_dev;"

# 2. Install production schema as base
mysql -u root -p mtm_wip_inventory_dev < Database_Files/Production_Database_Schema.sql
mysql -u root -p mtm_wip_inventory_dev < Database_Files/Existing_Stored_Procedures.sql

# 3. Apply development enhancements
mysql -u root -p mtm_wip_inventory_dev < Documentation/Development/Database_Files/Development_Database_Schema.sql

# 4. Install new comprehensive procedures ✅ 12 PROCEDURES AVAILABLE
mysql -u root -p mtm_wip_inventory_dev < Documentation/Development/Database_Files/New_Stored_Procedures.sql

# 5. Apply updates (when available)
mysql -u root -p mtm_wip_inventory_dev < Documentation/Development/Database_Files/Updated_Stored_Procedures.sql
```

### **Step 5: Configure Connection String**

#### **Option A: Using appsettings.json**
```bash
# Copy the development settings file
cp appsettings.Development.json appsettings.json
```

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mtm_wip_inventory_dev;Uid=your_username;Pwd=your_password;SslMode=Required;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### **Option B: Using Environment Variables**
```bash
# Linux/macOS
export ConnectionStrings__DefaultConnection="Server=localhost;Database=mtm_wip_inventory_dev;Uid=your_username;Pwd=your_password;SslMode=Required;"

# Windows Command Prompt
set ConnectionStrings__DefaultConnection=Server=localhost;Database=mtm_wip_inventory_dev;Uid=your_username;Pwd=your_password;SslMode=Required;

# Windows PowerShell
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=mtm_wip_inventory_dev;Uid=your_username;Pwd=your_password;SslMode=Required;"
```

### **Step 6: Build and Run**
```bash
# Build the application
dotnet build

# Run the application
dotnet run

# Or for development with hot reload
dotnet watch run
```

## 🔧 **Development Environment Setup**

### **Visual Studio 2022 Setup**
1. **Open Solution**: Open `MTM_WIP_Application_Avalonia.sln`
2. **Set Startup Project**: Right-click project → "Set as Startup Project"
3. **Configure Debugging**: Set breakpoints and debug configuration
4. **NuGet Package Manager**: Verify all packages are restored

### **VS Code Setup**
1. **Install Extensions**:
   - C# for Visual Studio Code
   - .NET Extension Pack
   - MySQL
   - GitLens (recommended)

2. **Configure Launch Settings**:
```json
// .vscode/launch.json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/net8.0/MTM_WIP_Application_Avalonia.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "console": "internalConsole",
            "stopAtEntry": false
        }
    ]
}
```

### **Database Development Setup**

#### **MySQL Workbench Configuration**
1. **Create Connection**: New connection to your development database
2. **Import Schema**: Use MySQL Workbench to import schema files
3. **Test Procedures**: Verify stored procedures are working

#### **Database File Organization** ✅ **ENHANCED**
```
Database Files Structure:
├── Database_Files/ (🔒 Production - READ ONLY)
│   ├── Production_Database_Schema.sql
│   ├── Existing_Stored_Procedures.sql
│   └── README_*.md
└── Documentation/Development/Database_Files/ (🔧 Development - EDITABLE)
    ├── Development_Database_Schema.sql
    ├── New_Stored_Procedures.sql ✅ 12 PROCEDURES IMPLEMENTED
    ├── Updated_Stored_Procedures.sql ✅ TEMPLATE READY
    └── README_*.md ✅ COMPLETE DOCUMENTATION
```

## ✅ **Verification Steps**

### **1. Build Verification**
```bash
# Should build without errors
dotnet build

# Expected output:
# Build succeeded.
# 0 Error(s)
# X Warning(s) (warnings are acceptable)
```

### **2. Database Connection Test**
```bash
# Test database connection
mysql -u your_username -p mtm_wip_inventory_dev -e "SELECT 'Connection successful' AS status;"
```

### **3. Stored Procedure Verification**
```sql
-- Test new enhanced procedures
CALL inv_inventory_Add_Item_Enhanced(
    'TEST001',    -- p_PartID
    '90',         -- p_OperationID  
    'RECEIVING',  -- p_LocationID
    10,           -- p_Quantity
    5.50,         -- p_UnitCost
    'TEST',       -- p_ReferenceNumber
    'Test item',  -- p_Notes
    'admin',      -- p_UserID
    @status,      -- p_Status (OUT)
    @message      -- p_ErrorMsg (OUT)
);

-- Check results
SELECT @status AS Status, @message AS Message;
```

### **4. Application Startup Test**
```bash
# Run application
dotnet run

# Verify:
# - Application window opens
# - No error messages in console
# - UI loads with MTM branding
# - Navigation sidebar is functional
```

## 🚨 **Common Issues and Solutions**

### **Build Issues**

#### **Issue: Package Restore Fails**
```bash
# Solution: Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore --force
```

#### **Issue: .NET Version Mismatch**
```bash
# Check target framework in .csproj file
# Should be: <TargetFramework>net8.0</TargetFramework>

# Install correct .NET version if needed
dotnet --list-sdks
```

### **Database Issues**

#### **Issue: Connection String Errors**
- **Check**: Username and password are correct
- **Check**: Database name matches what you created
- **Check**: MySQL server is running
- **Check**: Firewall allows connections

#### **Issue: Stored Procedure Missing**
```sql
-- Verify procedures exist
SHOW PROCEDURE STATUS WHERE Db = 'mtm_wip_inventory_dev';

-- Reinstall if missing
mysql -u root -p mtm_wip_inventory_dev < Documentation/Development/Database_Files/New_Stored_Procedures.sql
```

#### **Issue: Permission Denied**
```sql
-- Grant necessary permissions
GRANT ALL PRIVILEGES ON mtm_wip_inventory_dev.* TO 'your_username'@'localhost';
FLUSH PRIVILEGES;
```

### **Application Issues**

#### **Issue: Dependency Injection Errors**
```
Error: Unable to resolve service for type 'MTM_Shared_Logic.Core.Services.IValidationService'
```

**Solution**: Ensure proper service registration in Program.cs:
```csharp
// ✅ REQUIRED: Use comprehensive service registration
services.AddMTMServices(configuration);
```

#### **Issue: UI Not Loading**
- **Check**: All AXAML files are marked as AvaloniaResource
- **Check**: View and ViewModel naming conventions match
- **Check**: DataContext is properly set

## 🧪 **Testing Your Setup**

### **Quick Functionality Test**
1. **Start Application**: `dotnet run`
2. **Navigate UI**: Test sidebar navigation
3. **Database Operations**: Try adding a test inventory item
4. **Transaction History**: Verify transactions are logged
5. **Error Handling**: Test with invalid data to verify error messages

### **Performance Test**
```bash
# Build in Release mode for performance testing
dotnet build -c Release
dotnet run -c Release
```

## 📚 **Next Steps**

### **For Developers**
1. **Read Architecture Guide**: [README_Architecture.md](README_Architecture.md)
2. **Review Database Documentation**: [../Documentation/Development/Database_Files/README_Production_Schema.md](../Documentation/Development/Database_Files/README_Production_Schema.md)
3. **Study Service Layer**: [../Documentation/Development/README_Services.md](../Documentation/Development/README_Services.md)
4. **Explore UI Components**: [../Documentation/Development/README_UI_Documentation.md](../Documentation/Development/README_UI_Documentation.md)

### **For Database Developers**
1. **Database Development Guide**: [../Documentation/Development/README_Database_Files.md](../Documentation/Development/README_Database_Files.md)
2. **New Procedures Documentation**: [../Documentation/Development/Database_Files/README_NewProcedures.md](../Documentation/Development/Database_Files/README_NewProcedures.md)
3. **Stored Procedure Patterns**: [../Documentation/Development/Database_Files/README_Updated_Stored_Procedures.md](../Documentation/Development/Database_Files/README_Updated_Stored_Procedures.md)

### **For QA Teams**
1. **Testing Procedures**: [../Documentation/Development/README_Compliance_Reports.md](../Documentation/Development/README_Compliance_Reports.md)
2. **Quality Assurance**: Review compliance reports in `Development/Compliance Reports/`

## 🆘 **Getting Help**

### **Documentation Resources**
- **Project Overview**: [README_Project_Overview.md](README_Project_Overview.md)
- **Architecture Details**: [README_Architecture.md](README_Architecture.md)
- **Development Guides**: [../Documentation/Development/README_Development_Overview.md](../Documentation/Development/README_Development_Overview.md)

### **Support Channels**
- **GitHub Issues**: Report bugs and request features
- **Development Team**: Contact for technical questions
- **Database Issues**: Review database documentation first

### **Quick Reference**
```bash
# Common commands for development
dotnet restore          # Restore packages
dotnet build           # Build application
dotnet run             # Run application
dotnet watch run       # Run with hot reload
dotnet test            # Run unit tests
```

---

## 🎉 **Success!**

If you've completed all steps successfully, you should have:
- ✅ A fully functional development environment
- ✅ Database with all stored procedures installed
- ✅ Application running with MTM branding
- ✅ Access to comprehensive development documentation

**Welcome to the MTM WIP Application development team!**

---

*Last updated: January 2025*  
*Getting Started Guide v1.0*