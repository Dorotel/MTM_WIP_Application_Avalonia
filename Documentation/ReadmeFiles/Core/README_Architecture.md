# 🏗️ MTM WIP Application Architecture

This document provides a comprehensive overview of the MTM WIP Inventory Management System architecture, design patterns, and technical decisions.

## 🎯 **Architectural Overview**

The MTM WIP Application follows a modern, layered architecture with strict separation of concerns, designed for maintainability, testability, and scalability.

### **High-Level Architecture**

```
┌─────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                       │
├─────────────────────────────────────────────────────────────────┤
│  Avalonia UI Views (.axaml)                                    │
│  ├─ MainWindow, InventoryView, TransactionView                 │
│  ├─ Modern UI with MTM branding                                │
│  └─ Responsive design, accessibility support                   │
├─────────────────────────────────────────────────────────────────┤
│                        VIEW MODEL LAYER                         │
├─────────────────────────────────────────────────────────────────┤
│  ReactiveUI ViewModels                                          │
│  ├─ MVVM pattern implementation                                 │
│  ├─ Reactive programming with observables                      │
│  ├─ Command pattern for user actions                           │
│  └─ Two-way data binding                                        │
├─────────────────────────────────────────────────────────────────┤
│                         SERVICE LAYER                           │
├─────────────────────────────────────────────────────────────────┤
│  Business Services (MTM_Shared_Logic.Services namespace)                    │
│  ├─ IInventoryService - Inventory business logic               │
│  ├─ IUserService - User management and authentication          │
│  ├─ ITransactionService - Transaction processing               │
│  ├─ IValidationService - Business rule validation              │
│  └─ ICacheService - Performance optimization                   │
├─────────────────────────────────────────────────────────────────┤
│                     INFRASTRUCTURE LAYER                        │
├─────────────────────────────────────────────────────────────────┤
│  Data Access & Infrastructure Services                         │
│  ├─ IDatabaseService - Database operations                     │
│  ├─ IDbConnectionFactory - Connection management               │
│  ├─ DatabaseTransactionService - Transaction coordination      │
│  ├─ Helper_Database_StoredProcedure - Stored procedure executor│
│  └─ Configuration, Logging, Error Handling                     │
├─────────────────────────────────────────────────────────────────┤
│                         DATA LAYER                              │
├─────────────────────────────────────────────────────────────────┤
│  MySQL Database                                                │
│  ├─ Stored Procedures (ALL database operations)                │
│  ├─ Tables: Inventory, Parts, Transactions, Users              │
│  ├─ Views for complex queries                                  │
│  └─ Triggers for audit trails                                  │
└─────────────────────────────────────────────────────────────────┘
```

## 🎨 **Design Patterns**

### **1. Model-View-ViewModel (MVVM)**
Strict MVVM implementation using ReactiveUI for reactive programming:

```csharp
// View (AXAML) - Pure UI markup
<UserControl x:DataType="vm:InventoryViewModel">
    <Button Content="Add Item" Command="{Binding AddItemCommand}"/>
</UserControl>

// ViewModel - UI logic and state management
public class InventoryViewModel : ReactiveObject
{
    private readonly IInventoryService _inventoryService;
    
    public ReactiveCommand<Unit, Unit> AddItemCommand { get; }
    
    public InventoryViewModel(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
        AddItemCommand = ReactiveCommand.CreateFromTask(ExecuteAddItemAsync);
    }
}

// Service - Business logic (no UI dependencies)
public class InventoryService : IInventoryService
{
    public async Task<Result> AddItemAsync(AddItemRequest request)
    {
        // Pure business logic using stored procedures
    }
}
```

### **2. Dependency Injection (DI)**
Comprehensive DI container setup for loose coupling and testability:

```csharp
// ✅ CRITICAL: Use comprehensive service registration
services.AddMTMServices(configuration); // Registers ALL dependencies

// Service interfaces for easy mocking and testing
public interface IInventoryService
{
    Task<Result<InventoryItem>> AddItemAsync(AddItemRequest request);
}
```

### **3. Repository Pattern (via Stored Procedures)**
All data access through stored procedures for security and performance:

```csharp
// Database operations always use stored procedures
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item_Enhanced",
    parameters
);
```

### **4. Command Pattern**
ReactiveUI commands for user actions with async support:

```csharp
// Commands with built-in CanExecute logic
var canExecute = this.WhenAnyValue(vm => vm.IsValid);
SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canExecute);
```

### **5. Observer Pattern**
Reactive programming for UI updates:

```csharp
// Reactive property updates
this.WhenAnyValue(vm => vm.SearchText)
    .Throttle(TimeSpan.FromMilliseconds(300))
    .ObserveOn(RxApp.MainThreadScheduler)
    .Subscribe(async text => await SearchAsync(text));
```

## 🔧 **Technology Stack**

### **Frontend Technologies**
- **Avalonia UI 11.0+**: Cross-platform UI framework
- **ReactiveUI 19.0+**: MVVM framework with reactive extensions
- **.NET 8**: Latest .NET framework for performance
- **AXAML**: Avalonia's XAML-based markup language

### **Backend Technologies**
- **MySQL 8.0+**: Primary database with stored procedures
- **Entity Framework Core**: ORM for complex queries (limited use)
- **Serilog**: Structured logging framework
- **Polly**: Resilience patterns (retry, circuit breaker)

### **Development Tools**
- **Visual Studio 2022**: Primary IDE
- **Git**: Version control with GitHub workflows
- **xUnit**: Unit testing framework
- **Moq**: Mocking framework for testing

## 📊 **Data Architecture**

### **Database Design Principles**

#### **1. Stored Procedure First**
```sql
-- All operations use stored procedures
CALL inv_inventory_Add_Item_Enhanced(
    p_PartID := 'PART001',
    p_OperationID := '90',
    p_LocationID := 'RECEIVING',
    p_Quantity := 100,
    @p_Status,
    @p_ErrorMsg
);
```

#### **2. Standardized Error Handling**
```sql
-- All procedures return status and error message
OUT p_Status INT,           -- 0=Success, 1=Warning, 2=Error
OUT p_ErrorMsg VARCHAR(255) -- Descriptive error message
```

#### **3. Transaction Management**
```sql
-- Proper transaction boundaries
START TRANSACTION;
-- Business operations
IF error_condition THEN
    ROLLBACK;
    SET p_Status = 2;
ELSE
    COMMIT;
    SET p_Status = 0;
END IF;
```

### **Core Database Tables**

#### **Inventory Management**
```sql
inventory_transactions
├── TransactionID (PK)
├── PartID
├── OperationID
├── LocationID
├── TransactionType (IN/OUT/TRANSFER)
├── Quantity
├── UserID
└── Timestamp

inventory_current
├── PartID (PK)
├── LocationID (PK)
├── OperationID
├── CurrentQuantity
├── ReservedQuantity
└── LastUpdated
```

#### **Business Entities**
```sql
parts
├── PartID (PK)
├── PartName
├── Description
├── UnitOfMeasure
└── IsActive

operations
├── OperationID (PK)
├── OperationName
├── Description
└── SequenceNumber

locations
├── LocationID (PK)
├── LocationName
├── LocationType
└── IsActive
```

## 🔄 **Business Logic Rules**

### **Critical Rule: Transaction Type Determination**
**TransactionType is determined by USER INTENT, NOT Operation number:**

```csharp
// ✅ CORRECT: Based on user action
public async Task<Result> AddStockAsync(AddStockRequest request)
{
    var transaction = new Transaction
    {
        TransactionType = TransactionType.IN, // User adding stock
        Operation = request.Operation // Just workflow step identifier
    };
}

// ❌ WRONG: Based on operation number
private TransactionType DetermineTransactionType(string operation)
{
    return operation switch
    {
        "90" => TransactionType.IN,  // Incorrect logic!
        "100" => TransactionType.OUT // Operation != TransactionType
    };
}
```

### **Operation Number Usage**
- **Purpose**: Workflow step identification only
- **Examples**: "90" (Receiving), "100" (In Process), "110" (Finished)
- **Rule**: Same operation can have IN, OUT, or TRANSFER transactions

### **Inventory Validation Rules**
```csharp
// Stock validation before removal
public async Task<bool> ValidateStockAvailabilityAsync(
    string partId, string location, int requestedQuantity)
{
    var available = await GetAvailableStockAsync(partId, location);
    return available >= requestedQuantity;
}
```

## 🏗️ **Service Layer Architecture**

### **Service Dependencies**
```
IInventoryService
├── Depends on: IValidationService ⭐ CRITICAL
├── Depends on: ICacheService ⭐ CRITICAL  
├── Depends on: IDatabaseService
└── Depends on: ILogger<InventoryService>

IUserService
├── Depends on: ICacheService ⭐ CRITICAL
├── Depends on: IDatabaseService
└── Depends on: ILogger<UserService>

ITransactionService
├── Depends on: IValidationService ⭐ CRITICAL
├── Depends on: IDatabaseService
└── Depends on: ILogger<TransactionService>
```

### **Service Registration Pattern**
```csharp
// ✅ REQUIRED: Comprehensive registration
services.AddMTMServices(configuration);

// This registers:
// - All business services (IInventoryService, IUserService, etc.)
// - All supporting services (IValidationService, ICacheService, etc.)
// - All infrastructure services (IDatabaseService, etc.)
// - Proper lifetime management (Singleton, Scoped, Transient)
```

## 🎨 **UI Architecture**

### **Avalonia UI Structure**
```
MainWindow
├── Navigation Sidebar
│   ├── Inventory Section
│   ├── Transactions Section
│   ├── Users Section
│   └── Reports Section
├── Content Area
│   ├── Dynamic View Loading
│   ├── Breadcrumb Navigation
│   └── Status Messages
└── Status Bar
    ├── User Information
    ├── Connection Status
    └── Background Tasks
```

### **ViewModel Hierarchy**
```
BaseViewModel (Abstract)
├── Implements INotifyPropertyChanged
├── Provides common logging
├── Handles error states
└── Navigation helpers

MainWindowViewModel
├── CurrentView property
├── Navigation commands
└── Global state management

Feature ViewModels
├── InventoryViewModel
├── TransactionViewModel
├── UserManagementViewModel
└── ReportsViewModel
```

### **MTM Design System**
```css
/* MTM Brand Colors */
--mtm-primary: #4B45ED;     /* Primary Purple */
--mtm-secondary: #8345ED;   /* Secondary Purple */
--mtm-magenta: #BA45ED;     /* Magenta Accent */
--mtm-blue: #4574ED;        /* Blue Accent */
--mtm-pink: #ED45E7;        /* Pink Accent */
--mtm-light: #B594ED;       /* Light Purple */
```

## 🔒 **Security Architecture**

### **Database Security**
- **Stored Procedures Only**: No dynamic SQL to prevent injection
- **Parameter Validation**: All inputs validated at procedure level
- **Connection Security**: Encrypted connections with minimal permissions

### **Application Security**
- **Input Validation**: Client and server-side validation
- **User Authentication**: Role-based access control
- **Audit Trails**: All transactions logged with user attribution

### **Data Protection**
- **Connection Strings**: Secured in configuration with encryption
- **User Sessions**: Secure session management
- **Error Handling**: No sensitive information in error messages

## 📈 **Performance Architecture**

### **Caching Strategy**
```csharp
// Strategic caching for reference data
public class CacheService : ICacheService
{
    // Cache parts, operations, locations (relatively static)
    // Cache user preferences and settings
    // Cache frequently accessed inventory summaries
}
```

### **Database Performance**
- **Indexed Tables**: Proper indexing on frequently queried columns
- **Stored Procedures**: Compiled execution plans for better performance
- **Connection Pooling**: Efficient database connection management

### **UI Performance**
- **Reactive Programming**: Efficient UI updates only when needed
- **Virtualization**: Large data sets use virtualized controls
- **Lazy Loading**: Data loaded only when required

## 🧪 **Testing Architecture**

### **Testing Strategy**
```
Unit Tests
├── ViewModel Logic Testing (ReactiveUI)
├── Service Layer Testing (Business Logic)
├── Validation Rule Testing
└── Utility Method Testing

Integration Tests
├── Database Operation Testing
├── Service Integration Testing
└── End-to-End Workflow Testing

UI Tests
├── View Binding Testing
├── User Interaction Testing
└── Accessibility Testing
```

### **Mocking Strategy**
```csharp
// Mock external dependencies for unit testing
Mock<IInventoryService> mockInventoryService;
Mock<IDatabaseService> mockDatabaseService;
Mock<IValidationService> mockValidationService;
```

## 🔄 **Development Workflow**

### **Architecture Compliance**
1. **MVVM Adherence**: Views never directly access business logic
2. **Dependency Injection**: All dependencies injected, never new'd up
3. **Stored Procedures**: No direct SQL in application code
4. **Error Handling**: Comprehensive error handling at all layers
5. **Testing**: All business logic covered by unit tests

### **Code Quality Gates**
- **Build Verification**: Code must compile without warnings
- **Unit Test Coverage**: Minimum 80% coverage for business logic
- **Code Review**: All changes reviewed by team members
- **Documentation**: Architecture decisions documented

---

## 📚 **Related Documentation**

- **[Getting Started Guide](README_Getting_Started.md)** - Setup and installation
- **[Database Architecture](../../Documentation/Development/Database_Files/README_Production_Schema.md)** - Database design details
- **[Service Layer Guide](../../Documentation/Development/Components/README_Services.md)** - Service implementation patterns
- **[UI Component Guide](../../Documentation/Development/UI_Documentation/README_UI_Documentation.md)** - UI architecture patterns

---

*This architecture is designed to support the MTM WIP Application's goals of reliability, maintainability, and scalability while following modern software development best practices.*