# MTM Sequence Diagrams - Manufacturing Workflow Documentation

## 📋 Overview

This document provides comprehensive sequence diagrams for all major workflows in the MTM WIP Application, illustrating the interactions between Views, ViewModels, Services, and the Database layer.

## 🏭 **Core Manufacturing Workflows**

### **1. Add Inventory Workflow**

```mermaid
sequenceDiagram
    participant U as User
    participant V as InventoryTabView
    participant VM as InventoryTabViewModel
    participant IS as IInventoryService
    parameter DB as Database (Stored Procedures)
    participant EH as ErrorHandling Service
    participant L as Logger

    U->>V: Enter Part ID, Operation, Quantity, Location
    V->>VM: Binding updates properties
    U->>V: Click "Add Inventory" button
    V->>VM: AddInventoryCommand.Execute()
    
    VM->>VM: Validate input data
    alt Input validation fails
        VM->>V: Display validation error
        V->>U: Show error message
    else Input validation passes
        VM->>VM: Set IsLoading = true
        VM->>L: Log operation start
        VM->>IS: AddInventoryAsync(partId, operation, quantity, location)
        
        IS->>DB: Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()
        DB->>DB: Call inv_inventory_Add_Item stored procedure
        
        alt Database operation successful
            DB-->>IS: Return Status=1, ItemID
            IS-->>VM: Return Result.Success with data
            VM->>VM: Clear form fields
            VM->>V: Update UI with success state
            VM->>L: Log successful operation
            V->>U: Display success message
        else Database operation fails
            DB-->>IS: Return Status=0, Error message
            IS-->>VM: Return Result.Failure with error
            VM->>EH: HandleErrorAsync(exception, context)
            EH->>L: Log error details
            EH->>DB: Call log_error_Add_Error stored procedure
            VM->>V: Display error message
            V->>U: Show error to user
        end
        
        VM->>VM: Set IsLoading = false
    end
```

### **2. Remove Inventory Workflow**

```mermaid
sequenceDiagram
    participant U as User
    participant V as RemoveTabView
    participant VM as RemoveTabViewModel
    participant IS as IInventoryService
    participant DS as IDialogService
    participant DB as Database
    participant L as Logger

    U->>V: Enter Part ID and Operation
    V->>VM: Binding updates search criteria
    U->>V: Click "Search" button
    V->>VM: SearchInventoryCommand.Execute()
    
    VM->>IS: GetInventoryByPartIdAndOperationAsync(partId, operation)
    IS->>DB: Call inv_inventory_Get_ByPartIDandOperation
    DB-->>IS: Return inventory data
    IS-->>VM: Return current inventory details
    VM->>V: Update inventory display
    V->>U: Show current inventory (quantity, location)
    
    U->>V: Enter removal quantity
    V->>VM: Binding updates RemovalQuantity
    U->>V: Click "Remove Inventory" button
    V->>VM: RemoveInventoryCommand.Execute()
    
    VM->>VM: Validate removal quantity <= available
    alt Insufficient inventory
        VM->>V: Display insufficient inventory error
        V->>U: Show error message
    else Sufficient inventory
        VM->>DS: ShowConfirmationDialogAsync("Confirm removal?")
        DS-->>VM: User confirmation response
        
        alt User cancels
            VM->>V: Operation cancelled
        else User confirms
            VM->>IS: RemoveInventoryAsync(partId, operation, quantity, location)
            IS->>DB: Call inv_inventory_Remove_Item stored procedure
            
            alt Removal successful
                DB-->>IS: Return Status=1, Updated quantity
                IS-->>VM: Return success result
                VM->>VM: Update local inventory display
                VM->>V: Show success message
                V->>U: Display operation success
            else Removal fails
                DB-->>IS: Return Status=0, Error message
                IS-->>VM: Return failure result
                VM->>V: Display error message
                V->>U: Show error to user
            end
        end
    end
```

### **3. Transfer Inventory Workflow**

```mermaid
sequenceDiagram
    participant U as User
    participant V as TransferTabView
    participant VM as TransferTabViewModel
    participant IS as IInventoryService
    participant MDS as IMasterDataService
    participant DB as Database

    U->>V: Enter Part ID and Operation
    V->>VM: Binding updates search criteria
    U->>V: Click "Load Current Inventory"
    V->>VM: LoadInventoryCommand.Execute()
    
    VM->>IS: GetInventoryByPartIdAndOperationAsync(partId, operation)
    IS->>DB: Call inv_inventory_Get_ByPartIDandOperation
    DB-->>IS: Return current inventory locations
    IS-->>VM: Return inventory by location
    VM->>V: Populate source locations dropdown
    V->>U: Show available source locations
    
    U->>V: Select source location
    V->>VM: SelectedSourceLocation property updated
    VM->>MDS: GetAllLocationsAsync()
    MDS->>DB: Call md_locations_Get_All
    DB-->>MDS: Return all valid locations
    MDS-->>VM: Return location list
    VM->>VM: Filter out source location from destinations
    VM->>V: Populate destination locations dropdown
    V->>U: Show available destination locations
    
    U->>V: Select destination location and quantity
    V->>VM: Binding updates transfer details
    U->>V: Click "Transfer Inventory"
    V->>VM: TransferInventoryCommand.Execute()
    
    VM->>VM: Validate transfer parameters
    alt Validation fails
        VM->>V: Display validation errors
        V->>U: Show validation messages
    else Validation passes
        VM->>IS: TransferInventoryAsync(partId, operation, fromLocation, toLocation, quantity)
        IS->>DB: Begin transaction
        IS->>DB: Call inv_inventory_Remove_Item (from source)
        IS->>DB: Call inv_inventory_Add_Item (to destination)
        IS->>DB: Call inv_transaction_Add (TRANSFER type)
        IS->>DB: Commit transaction
        
        alt Transfer successful
            DB-->>IS: Return Status=1, Transfer ID
            IS-->>VM: Return success result
            VM->>VM: Clear form fields
            VM->>V: Show transfer success
            V->>U: Display success message with transfer ID
        else Transfer fails
            DB-->>IS: Rollback transaction, Return error
            IS-->>VM: Return failure result
            VM->>V: Display error message
            V->>U: Show transfer failure
        end
    end
```

### **4. Settings Configuration Workflow**

```mermaid
sequenceDiagram
    participant U as User
    participant V as SettingsView
    participant VM as SettingsViewModel
    participant CS as IConfigurationService
    participant TS as IThemeService
    participant DB as Database Connection Test

    U->>V: Navigate to Settings
    V->>VM: Initialize ViewModel
    VM->>CS: LoadCurrentSettingsAsync()
    CS-->>VM: Return current configuration
    VM->>V: Populate settings form
    V->>U: Display current settings
    
    U->>V: Modify database connection string
    V->>VM: ConnectionString property updated
    U->>V: Click "Test Connection"
    V->>VM: TestConnectionCommand.Execute()
    
    VM->>DB: Attempt test connection
    alt Connection successful
        DB-->>VM: Connection test passed
        VM->>V: Show green checkmark
        V->>U: Display "Connection successful"
    else Connection fails
        DB-->>VM: Connection test failed with error
        VM->>V: Show red X with error details
        V->>U: Display connection error
    end
    
    U->>V: Change theme selection
    V->>VM: SelectedTheme property updated
    VM->>TS: PreviewThemeAsync(selectedTheme)
    TS->>V: Apply theme preview
    V->>U: Show theme preview
    
    U->>V: Click "Save Settings"
    V->>VM: SaveSettingsCommand.Execute()
    
    VM->>CS: SaveConfigurationAsync(settings)
    CS->>CS: Validate configuration
    
    alt Configuration valid
        CS->>CS: Write to appsettings.json
        CS-->>VM: Return save success
        VM->>TS: ApplyThemeAsync(selectedTheme)
        TS->>TS: Update application theme
        VM->>V: Show save success
        V->>U: Display "Settings saved successfully"
    else Configuration invalid
        CS-->>VM: Return validation errors
        VM->>V: Display validation errors
        V->>U: Show configuration errors
    end
```

## 📊 **Data Flow Workflows**

### **5. Master Data Loading Workflow**

```mermaid
sequenceDiagram
    participant App as Application Startup
    participant MDS as IMasterDataService
    participant Cache as Memory Cache
    participant DB as Database
    participant L as Logger

    App->>MDS: InitializeMasterDataAsync()
    MDS->>L: Log initialization start
    
    par Load Part IDs
        MDS->>DB: Call md_part_ids_Get_All
        DB-->>MDS: Return all active part IDs
    and Load Locations
        MDS->>DB: Call md_locations_Get_All
        DB-->>MDS: Return all active locations
    and Load Operations
        MDS->>DB: Call md_operation_numbers_Get_All
        DB-->>MDS: Return all active operations
    end
    
    MDS->>Cache: Store part IDs in cache (1 hour TTL)
    MDS->>Cache: Store locations in cache (1 hour TTL)
    MDS->>Cache: Store operations in cache (1 hour TTL)
    
    MDS->>L: Log successful initialization
    MDS-->>App: Return initialization complete
```

### **6. Error Handling Workflow**

```mermaid
sequenceDiagram
    participant VM as ViewModel
    participant EH as ErrorHandling Service
    participant L as Logger
    participant DB as Database
    participant U as User

    VM->>VM: Exception occurs during operation
    VM->>EH: HandleErrorAsync(exception, context)
    
    EH->>L: LogError(exception, context)
    L->>L: Write to console and file logs
    
    EH->>EH: Determine error severity
    EH->>EH: Generate user-friendly message
    
    alt Critical error (data corruption, DB connection)
        EH->>DB: Call log_error_Add_Error (Severity: CRITICAL)
        EH->>EH: Generate incident ID
        EH-->>VM: Return critical error response
        VM->>U: Show critical error dialog with incident ID
    else Standard error (validation, business logic)
        EH->>DB: Call log_error_Add_Error (Severity: ERROR)
        EH-->>VM: Return standard error response
        VM->>U: Show standard error message
    else Warning (performance, connectivity)
        EH->>DB: Call log_error_Add_Error (Severity: WARNING)
        EH-->>VM: Return warning response
        VM->>U: Show warning notification
    end
```

## 🔄 **Background Process Workflows**

### **7. Periodic Data Refresh Workflow**

```mermaid
sequenceDiagram
    participant Timer as Background Timer
    participant RefreshService as IDataRefreshService
    participant Cache as Memory Cache
    participant MDS as IMasterDataService
    participant ViewModels as Active ViewModels

    Timer->>RefreshService: Trigger periodic refresh (every 5 minutes)
    RefreshService->>Cache: Check cache expiration
    
    alt Cache expired
        RefreshService->>MDS: RefreshMasterDataAsync()
        MDS->>DB: Reload master data from database
        DB-->>MDS: Return updated data
        MDS->>Cache: Update cached data
        MDS-->>RefreshService: Return refresh complete
        
        RefreshService->>ViewModels: Notify data refresh available
        ViewModels->>ViewModels: Update UI with fresh data
        ViewModels->>User: Reflect updated data in interface
    else Cache still valid
        RefreshService->>RefreshService: Skip refresh cycle
    end
```

### **8. Application Startup Sequence**

```mermaid
sequenceDiagram
    participant Main as Program.Main()
    participant Startup as ApplicationStartup
    participant DI as Dependency Injection
    participant Config as Configuration
    participant Health as Health Check
    participant MainWindow as MainWindow

    Main->>Startup: Initialize application
    Startup->>Config: Load appsettings.json
    Config-->>Startup: Return configuration
    
    Startup->>DI: Configure services
    DI->>DI: Register ViewModels (Transient)
    DI->>DI: Register Services (Scoped/Singleton)
    DI->>DI: Register Configuration services
    DI-->>Startup: Service container configured
    
    Startup->>Health: PerformStartupValidationAsync()
    Health->>Health: Test database connectivity
    Health->>Health: Validate configuration
    Health->>Health: Check required files
    
    alt Startup validation passes
        Health-->>Startup: Validation successful
        Startup->>MainWindow: Initialize main window
        MainWindow->>DI: Resolve MainViewModel
        DI-->>MainWindow: Return configured ViewModel
        MainWindow->>User: Display application interface
    else Startup validation fails
        Health-->>Startup: Validation failed with errors
        Startup->>User: Display startup error dialog
        Startup->>Main: Exit application with error code
    end
```

## 🎯 **Integration Workflows**

### **9. Theme Change Workflow**

```mermaid
sequenceDiagram
    participant U as User
    participant VM as ThemeSelectionViewModel
    participant TS as IThemeService
    participant Config as IConfigurationService
    participant App as Application Resources

    U->>VM: Select new theme (MTM_Blue, MTM_Green, etc.)
    VM->>TS: PreviewThemeAsync(themeName)
    TS->>App: Load theme resource dictionary
    App->>App: Merge theme resources
    App-->>TS: Theme preview applied
    TS-->>VM: Preview complete
    VM->>U: Show theme preview in UI
    
    U->>VM: Click "Apply Theme"
    VM->>TS: ApplyThemeAsync(themeName)
    TS->>Config: SaveThemePreferenceAsync(themeName)
    Config->>Config: Update appsettings.json
    Config-->>TS: Theme preference saved
    
    TS->>App: Apply theme permanently
    App->>App: Update all windows and controls
    App-->>TS: Theme applied successfully
    TS-->>VM: Return success result
    VM->>U: Show "Theme applied successfully"
```

These sequence diagrams provide comprehensive documentation of all major workflows in the MTM WIP Application, ensuring developers understand the complete interaction patterns between components in the manufacturing inventory management system.


## 🚀 Joyride Development Enhancement

**Priority Directive**: Leverage Joyride automation for enhanced MTM development workflows.

### Core Joyride Tools
- **`joyride_evaluate_code`**: Execute ClojureScript in VS Code Extension Host
- **`joyride_request_human_input`**: Interactive decision-making workflows
- **`joyride_basics_for_agents`**: Access automation patterns and capabilities
- **`joyride_assisting_users_guide`**: User assistance automation

### MTM-Optimized Automation
- **Pattern Enforcement**: Automated MVVM Community Toolkit validation
- **Domain Validation**: Manufacturing operation codes and business rules
- **UI Consistency**: Avalonia theme system and component standards
- **Database Integration**: MySQL connection testing and stored procedure validation
- **Cross-Platform**: Automated testing across Windows/macOS/Linux

**Implementation**: Use Joyride first when safe, fall back to traditional tools as needed.