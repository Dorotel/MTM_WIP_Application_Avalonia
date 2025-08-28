# 🏭 Implement Complete Inventory Management Database Logic

## 📋 Overview
Implement comprehensive database logic for all inventory management operations using the existing stored procedures. This includes inventory operations (Add/Remove/Transfer), master data management, user management, and system configuration.

## 🎯 Objectives
- Implement all database service methods for inventory operations
- Create comprehensive ViewModels for all inventory functions
- Build UI components for all inventory management features  
- Integrate with QuickButtons system for enhanced user experience
- Ensure proper error handling and logging throughout
- **📋 Create comprehensive stored procedures documentation and update all instruction files**

## 📊 Scope

<details>
<summary><strong>🔧 Core Inventory Operations</strong></summary>

**Stored Procedures to Implement:**

#### Inventory Management
- ✅ `inv_inventory_Add_Item` - Add inventory items (COMPLETED in InventoryTabViewModel)
- ⭐ `inv_inventory_Get_ByPartID` - Search inventory by part
- ⭐ `inv_inventory_Get_ByPartIDandOperation` - Search by part and operation
- ⭐ `inv_inventory_Get_ByUser` - Get user's inventory history
- ⭐ `inv_inventory_Remove_Item` - Remove inventory items
- ⭐ `inv_inventory_Transfer_Part` - Transfer entire part to new location
- ⭐ `inv_inventory_Transfer_Quantity` - Transfer partial quantity

#### Master Data Management
- ✅ `md_part_ids_Get_All` - Get all parts (COMPLETED in InventoryTabViewModel)
- ✅ `md_operation_numbers_Get_All` - Get all operations (COMPLETED in InventoryTabViewModel)
- ✅ `md_locations_Get_All` - Get all locations (COMPLETED in InventoryTabViewModel)
- ⭐ `md_part_ids_Add_Part` - Add new parts
- ⭐ `md_part_ids_Update_Part` - Update part information
- ⭐ `md_part_ids_Delete_ByItemNumber` - Delete parts
- ⭐ `md_part_ids_Get_ByItemNumber` - Get specific part details
- ⭐ `md_operation_numbers_Add_Operation` - Add new operations
- ⭐ `md_operation_numbers_Update_Operation` - Update operations
- ⭐ `md_operation_numbers_Delete_ByOperation` - Delete operations
- ⭐ `md_locations_Add_Location` - Add new locations
- ⭐ `md_locations_Update_Location` - Update locations
- ⭐ `md_locations_Delete_ByLocation` - Delete locations
- ⭐ `md_item_types_Get_All` - Get all item types
- ⭐ `md_item_types_Add_ItemType` - Add new item types
- ⭐ `md_item_types_Update_ItemType` - Update item types
- ⭐ `md_item_types_Delete_ByType` - Delete item types

#### User Management
- ⭐ `usr_users_Get_All` - Get all users
- ⭐ `usr_users_Get_ByUser` - Get specific user
- ⭐ `usr_users_Add_User` - Add new users
- ⭐ `usr_users_Update_User` - Update user information
- ⭐ `usr_users_Delete_User` - Delete users
- ⭐ `usr_users_Exists` - Check if user exists

#### System Configuration
- ⭐ `usr_ui_settings_Get` - Get user UI settings
- ⭐ `usr_ui_settings_SetJsonSetting` - Save UI settings
- ⭐ `usr_ui_settings_SetThemeJson` - Save theme settings
- ⭐ `usr_ui_settings_SetShortcutsJson` - Save keyboard shortcuts
- ⭐ `usr_ui_settings_GetShortcutsJson` - Get keyboard shortcuts
- ⭐ `sys_roles_Get_ById` - Get role information
- ⭐ `sys_user_roles_Add` - Add user roles
- ⭐ `sys_user_roles_Update` - Update user roles
- ⭐ `sys_user_roles_Delete` - Delete user roles
- ⭐ `log_changelog_Get_Current` - Get current changelog

</details>

## 🛠️ Implementation Plan

<details>
<summary><strong>Phase 1: Enhanced Database Service Layer</strong></summary>

**Estimated Time: 2-3 days**

#### 1.1 Expand DatabaseService 
```csharp
// Add to Services/Database.cs (following service organization rule)
public interface IDatabaseService
{
    // Inventory Operations
    Task<StoredProcedureResult> AddInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string notes);
    Task<DataTable> GetInventoryByPartIdAsync(string partId);
    Task<DataTable> GetInventoryByPartAndOperationAsync(string partId, string operation);
    Task<DataTable> GetInventoryByUserAsync(string user);
    Task<StoredProcedureResult> RemoveInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes);
    Task<bool> TransferPartAsync(string batchNumber, string partId, string operation, string newLocation);
    Task<bool> TransferQuantityAsync(string batchNumber, string partId, string operation, int transferQuantity, int originalQuantity, string newLocation, string user);
    
    // Master Data Operations
    Task<StoredProcedureResult> AddPartAsync(string partId, string customer, string description, string issuedBy, string itemType);
    Task<StoredProcedureResult> UpdatePartAsync(int id, string partId, string customer, string description, string issuedBy, string itemType);
    Task<bool> DeletePartAsync(string partId);
    Task<DataTable> GetPartByIdAsync(string partId);
    
    // Operations Management
    Task<StoredProcedureResult> AddOperationAsync(string operation, string issuedBy);
    Task<StoredProcedureResult> UpdateOperationAsync(string operation, string newOperation, string issuedBy);
    Task<bool> DeleteOperationAsync(string operation);
    
    // Locations Management
    Task<StoredProcedureResult> AddLocationAsync(string location, string issuedBy, string building);
    Task<StoredProcedureResult> UpdateLocationAsync(string oldLocation, string location, string issuedBy, string building);
    Task<bool> DeleteLocationAsync(string location);
    
    // Item Types Management
    Task<DataTable> GetAllItemTypesAsync();
    Task<StoredProcedureResult> AddItemTypeAsync(string itemType, string issuedBy);
    Task<StoredProcedureResult> UpdateItemTypeAsync(int id, string itemType, string issuedBy);
    Task<bool> DeleteItemTypeAsync(string itemType);
    
    // User Management
    Task<DataTable> GetAllUsersAsync();
    Task<DataTable> GetUserAsync(string username);
    Task<bool> UserExistsAsync(string username);
    Task<StoredProcedureResult> AddUserAsync(UserInfo userInfo);
    Task<StoredProcedureResult> UpdateUserAsync(UserInfo userInfo);
    Task<bool> DeleteUserAsync(string username);
    
    // System Configuration
    Task<string> GetUserSettingsAsync(string userId);
    Task<bool> SaveUserSettingsAsync(string userId, string settingsJson);
    Task<bool> SaveThemeSettingsAsync(string userId, string themeJson);
    Task<string> GetUserShortcutsAsync(string userId);
    Task<bool> SaveUserShortcutsAsync(string userId, string shortcutsJson);
}
```

#### 1.2 Create Data Models
```csharp
// Add to Models/InventoryModels.cs
public class InventoryItem
{
    public int ID { get; set; }
    public string PartID { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public DateTime ReceiveDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public string User { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class PartInfo
{
    public int ID { get; set; }
    public string PartID { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IssuedBy { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
}

public class OperationInfo
{
    public int ID { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string IssuedBy { get; set; } = string.Empty;
}

public class LocationInfo
{
    public int ID { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public string IssuedBy { get; set; } = string.Empty;
}

public class ItemTypeInfo
{
    public int ID { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string IssuedBy { get; set; } = string.Empty;
}

public class UserInfo
{
    public string User { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Shift { get; set; } = string.Empty;
    public bool VitsUser { get; set; }
    public string Pin { get; set; } = string.Empty;
    public string LastShownVersion { get; set; } = string.Empty;
    public string HideChangeLog { get; set; } = string.Empty;
    public string ThemeName { get; set; } = string.Empty;
    public int ThemeFontSize { get; set; }
    public string VisualUserName { get; set; } = string.Empty;
    public string VisualPassword { get; set; } = string.Empty;
    public string WipServerAddress { get; set; } = string.Empty;
    public string WipServerPort { get; set; } = string.Empty;
    public string WipDatabase { get; set; } = string.Empty;
}
```

</details>

<details>
<summary><strong>Phase 2: Inventory Management ViewModels</strong></summary>

**Estimated Time: 3-4 days**

#### 2.1 RemoveInventoryViewModel
```csharp
// ViewModels/MainForm/RemoveInventoryViewModel.cs
public class RemoveInventoryViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<RemoveInventoryViewModel> _logger;
    
    // Properties for search criteria - Standard .NET patterns
    private string _searchPartId = string.Empty;
    public string SearchPartId
    {
        get => _searchPartId;
        set => SetProperty(ref _searchPartId, value);
    }
    
    private string _searchOperation = string.Empty;
    public string SearchOperation
    {
        get => _searchOperation;
        set => SetProperty(ref _searchOperation, value);
    }
    
    private string _searchLocation = string.Empty;
    public string SearchLocation
    {
        get => _searchLocation;
        set => SetProperty(ref _searchLocation, value);
    }
    
    // Results collection - Standard ObservableCollection
    public ObservableCollection<InventoryItem> SearchResults { get; } = new();
    
    private InventoryItem? _selectedItem;
    public InventoryItem? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }
    
    // Commands - Standard ICommand implementations
    public ICommand SearchCommand { get; private set; }
    public ICommand RemoveSelectedCommand { get; private set; }
    public ICommand RemoveCustomQuantityCommand { get; private set; }
    
    // Integration with QuickButtons - Standard event pattern
    public event EventHandler<InventoryRemovedEventArgs>? ItemRemoved;
    
    public RemoveInventoryViewModel(IDatabaseService databaseService, ILogger<RemoveInventoryViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands - Standard pattern
        SearchCommand = new AsyncCommand(ExecuteSearchAsync);
        RemoveSelectedCommand = new AsyncCommand(ExecuteRemoveSelectedAsync, () => SelectedItem != null);
        RemoveCustomQuantityCommand = new AsyncCommand<int>(ExecuteRemoveCustomQuantityAsync);
    }
    
    private async Task ExecuteSearchAsync()
    {
        // Implementation using Helper_Database_StoredProcedure
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _databaseService.GetConnectionString(),
            "inv_inventory_Get_ByPartID",
            new Dictionary<string, object> { ["p_PartID"] = SearchPartId }
        );
        // Convert DataTable to ObservableCollection<InventoryItem>
    }
}
```

#### 2.2 TransferInventoryViewModel
```csharp
// ViewModels/MainForm/TransferInventoryViewModel.cs
public class TransferInventoryViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<TransferInventoryViewModel> _logger;
    
    // Properties for transfer operations - Standard .NET patterns
    private string _sourceLocation = string.Empty;
    public string SourceLocation
    {
        get => _sourceLocation;
        set => SetProperty(ref _sourceLocation, value);
    }
    
    private string _destinationLocation = string.Empty;
    public string DestinationLocation
    {
        get => _destinationLocation;
        set => SetProperty(ref _destinationLocation, value);
    }
    
    private int _transferQuantity;
    public int TransferQuantity
    {
        get => _transferQuantity;
        set => SetProperty(ref _transferQuantity, value);
    }
    
    // Commands - Standard ICommand implementations
    public ICommand TransferPartCommand { get; private set; }
    public ICommand TransferQuantityCommand { get; private set; }
    public ICommand ValidateTransferCommand { get; private set; }
    
    // Integration with QuickButtons - Standard event pattern
    public event EventHandler<InventoryTransferredEventArgs>? ItemTransferred;
    
    public TransferInventoryViewModel(IDatabaseService databaseService, ILogger<TransferInventoryViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands
        TransferPartCommand = new AsyncCommand(ExecuteTransferPartAsync);
        TransferQuantityCommand = new AsyncCommand(ExecuteTransferQuantityAsync);
        ValidateTransferCommand = new AsyncCommand(ExecuteValidateTransferAsync);
    }
}
```

#### 2.3 SearchInventoryViewModel
```csharp
// ViewModels/MainForm/SearchInventoryViewModel.cs
public class SearchInventoryViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<SearchInventoryViewModel> _logger;
    
    // Advanced search capabilities - Standard .NET patterns
    private string _partIdFilter = string.Empty;
    public string PartIdFilter
    {
        get => _partIdFilter;
        set => SetProperty(ref _partIdFilter, value);
    }
    
    private string _operationFilter = string.Empty;
    public string OperationFilter
    {
        get => _operationFilter;
        set => SetProperty(ref _operationFilter, value);
    }
    
    private string _locationFilter = string.Empty;
    public string LocationFilter
    {
        get => _locationFilter;
        set => SetProperty(ref _locationFilter, value);
    }
    
    private string _userFilter = string.Empty;
    public string UserFilter
    {
        get => _userFilter;
        set => SetProperty(ref _userFilter, value);
    }
    
    private DateTime? _dateFromFilter;
    public DateTime? DateFromFilter
    {
        get => _dateFromFilter;
        set => SetProperty(ref _dateFromFilter, value);
    }
    
    private DateTime? _dateToFilter;
    public DateTime? DateToFilter
    {
        get => _dateToFilter;
        set => SetProperty(ref _dateToFilter, value);
    }
    
    // Results and pagination - Standard collections
    public ObservableCollection<InventoryItem> Results { get; } = new();
    
    private int _currentPage = 1;
    public int CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }
    
    private int _totalPages = 1;
    public int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }
    
    // Commands - Standard ICommand implementations
    public ICommand SearchCommand { get; private set; }
    public ICommand ExportCommand { get; private set; }
    public ICommand ClearFiltersCommand { get; private set; }
    
    public SearchInventoryViewModel(IDatabaseService databaseService, ILogger<SearchInventoryViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands
        SearchCommand = new AsyncCommand(ExecuteSearchAsync);
        ExportCommand = new AsyncCommand(ExecuteExportAsync);
        ClearFiltersCommand = new RelayCommand(ExecuteClearFilters);
    }
}
```

</details>

<details>
<summary><strong>Phase 3: Master Data Management ViewModels</strong></summary>

**Estimated Time: 2-3 days**

#### 3.1 PartsManagementViewModel
```csharp
// ViewModels/Administration/PartsManagementViewModel.cs
public class PartsManagementViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<PartsManagementViewModel> _logger;
    
    // Collections - Standard ObservableCollection
    public ObservableCollection<PartInfo> Parts { get; } = new();
    
    private PartInfo? _selectedPart;
    public PartInfo? SelectedPart
    {
        get => _selectedPart;
        set => SetProperty(ref _selectedPart, value);
    }
    
    private PartInfo _newPart = new();
    public PartInfo NewPart
    {
        get => _newPart;
        set => SetProperty(ref _newPart, value);
    }
    
    // Commands - Standard ICommand implementations
    public ICommand LoadPartsCommand { get; private set; }
    public ICommand AddPartCommand { get; private set; }
    public ICommand UpdatePartCommand { get; private set; }
    public ICommand DeletePartCommand { get; private set; }
    public ICommand SearchPartsCommand { get; private set; }
    
    public PartsManagementViewModel(IDatabaseService databaseService, ILogger<PartsManagementViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands
        LoadPartsCommand = new AsyncCommand(ExecuteLoadPartsAsync);
        AddPartCommand = new AsyncCommand(ExecuteAddPartAsync);
        UpdatePartCommand = new AsyncCommand(ExecuteUpdatePartAsync);
        DeletePartCommand = new AsyncCommand(ExecuteDeletePartAsync);
        SearchPartsCommand = new AsyncCommand(ExecuteSearchPartsAsync);
    }
}
```

#### 3.2 OperationsManagementViewModel
```csharp
// ViewModels/Administration/OperationsManagementViewModel.cs
public class OperationsManagementViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<OperationsManagementViewModel> _logger;
    
    // Collections - Standard ObservableCollection
    public ObservableCollection<OperationInfo> Operations { get; } = new();
    
    private OperationInfo? _selectedOperation;
    public OperationInfo? SelectedOperation
    {
        get => _selectedOperation;
        set => SetProperty(ref _selectedOperation, value);
    }
    
    // Commands for CRUD operations - Standard ICommand implementations
    public ICommand AddOperationCommand { get; private set; }
    public ICommand UpdateOperationCommand { get; private set; }
    public ICommand DeleteOperationCommand { get; private set; }
    
    public OperationsManagementViewModel(IDatabaseService databaseService, ILogger<OperationsManagementViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands
        AddOperationCommand = new AsyncCommand(ExecuteAddOperationAsync);
        UpdateOperationCommand = new AsyncCommand(ExecuteUpdateOperationAsync);
        DeleteOperationCommand = new AsyncCommand(ExecuteDeleteOperationAsync);
    }
}
```

#### 3.3 LocationsManagementViewModel
```csharp
// ViewModels/Administration/LocationsManagementViewModel.cs
public class LocationsManagementViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<LocationsManagementViewModel> _logger;
    
    // Collections - Standard ObservableCollection
    public ObservableCollection<LocationInfo> Locations { get; } = new();
    
    private LocationInfo? _selectedLocation;
    public LocationInfo? SelectedLocation
    {
        get => _selectedLocation;
        set => SetProperty(ref _selectedLocation, value);
    }
    
    // Commands for CRUD operations - Standard ICommand implementations
    public ICommand AddLocationCommand { get; private set; }
    public ICommand UpdateLocationCommand { get; private set; }
    public ICommand DeleteLocationCommand { get; private set; }
    
    public LocationsManagementViewModel(IDatabaseService databaseService, ILogger<LocationsManagementViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands
        AddLocationCommand = new AsyncCommand(ExecuteAddLocationAsync);
        UpdateLocationCommand = new AsyncCommand(ExecuteUpdateLocationAsync);
        DeleteLocationCommand = new AsyncCommand(ExecuteDeleteLocationAsync);
    }
}
```

#### 3.4 ItemTypesManagementViewModel
```csharp
// ViewModels/Administration/ItemTypesManagementViewModel.cs
public class ItemTypesManagementViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<ItemTypesManagementViewModel> _logger;
    
    // Collections - Standard ObservableCollection
    public ObservableCollection<ItemTypeInfo> ItemTypes { get; } = new();
    
    private ItemTypeInfo? _selectedItemType;
    public ItemTypeInfo? SelectedItemType
    {
        get => _selectedItemType;
        set => SetProperty(ref _selectedItemType, value);
    }
    
    // Commands for CRUD operations - Standard ICommand implementations
    public ICommand AddItemTypeCommand { get; private set; }
    public ICommand UpdateItemTypeCommand { get; private set; }
    public ICommand DeleteItemTypeCommand { get; private set; }
    
    public ItemTypesManagementViewModel(IDatabaseService databaseService, ILogger<ItemTypesManagementViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands
        AddItemTypeCommand = new AsyncCommand(ExecuteAddItemTypeAsync);
        UpdateItemTypeCommand = new AsyncCommand(ExecuteUpdateItemTypeAsync);
        DeleteItemTypeCommand = new AsyncCommand(ExecuteDeleteItemTypeAsync);
    }
}
```

</details>

<details>
<summary><strong>Phase 4: User Management System</strong></summary>

**Estimated Time: 2-3 days**

#### 4.1 UserManagementViewModel
```csharp
// ViewModels/Administration/UserManagementViewModel.cs
public class UserManagementViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<UserManagementViewModel> _logger;
    
    // Collections - Standard ObservableCollection
    public ObservableCollection<UserInfo> Users { get; } = new();
    
    private UserInfo? _selectedUser;
    public UserInfo? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }
    
    private UserInfo _newUser = new();
    public UserInfo NewUser
    {
        get => _newUser;
        set => SetProperty(ref _newUser, value);
    }
    
    // Commands - Standard ICommand implementations
    public ICommand LoadUsersCommand { get; private set; }
    public ICommand AddUserCommand { get; private set; }
    public ICommand UpdateUserCommand { get; private set; }
    public ICommand DeleteUserCommand { get; private set; }
    public ICommand ValidateUserCommand { get; private set; }
    
    public UserManagementViewModel(IDatabaseService databaseService, ILogger<UserManagementViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands
        LoadUsersCommand = new AsyncCommand(ExecuteLoadUsersAsync);
        AddUserCommand = new AsyncCommand(ExecuteAddUserAsync);
        UpdateUserCommand = new AsyncCommand(ExecuteUpdateUserAsync);
        DeleteUserCommand = new AsyncCommand(ExecuteDeleteUserAsync);
        ValidateUserCommand = new AsyncCommand(ExecuteValidateUserAsync);
    }
}
```

#### 4.2 UserSettingsViewModel
```csharp
// ViewModels/Settings/UserSettingsViewModel.cs
public class UserSettingsViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<UserSettingsViewModel> _logger;
    
    // Settings properties - Standard .NET patterns
    private string _themeSettings = string.Empty;
    public string ThemeSettings
    {
        get => _themeSettings;
        set => SetProperty(ref _themeSettings, value);
    }
    
    private string _shortcutsSettings = string.Empty;
    public string ShortcutsSettings
    {
        get => _shortcutsSettings;
        set => SetProperty(ref _shortcutsSettings, value);
    }
    
    private string _uiSettings = string.Empty;
    public string UISettings
    {
        get => _uiSettings;
        set => SetProperty(ref _uiSettings, value);
    }
    
    // Commands - Standard ICommand implementations
    public ICommand SaveSettingsCommand { get; private set; }
    public ICommand LoadSettingsCommand { get; private set; }
    public ICommand ResetToDefaultsCommand { get; private set; }
    
    public UserSettingsViewModel(IDatabaseService databaseService, ILogger<UserSettingsViewModel> logger) : base(logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Initialize commands
        SaveSettingsCommand = new AsyncCommand(ExecuteSaveSettingsAsync);
        LoadSettingsCommand = new AsyncCommand(ExecuteLoadSettingsAsync);
        ResetToDefaultsCommand = new AsyncCommand(ExecuteResetToDefaultsAsync);
    }
}
```

</details>

<details>
<summary><strong>Phase 5: UI Implementation</strong></summary>

**Estimated Time: 4-5 days**

#### 5.1 Inventory Management Views
- **RemoveTabView.axaml** - Inventory removal interface using standard Avalonia patterns
- **TransferTabView.axaml** - Inventory transfer interface with standard data binding
- **SearchTabView.axaml** - Advanced inventory search with standard controls
- **InventoryHistoryView.axaml** - User inventory history display

#### 5.2 Master Data Management Views
- **PartsManagementView.axaml** - Parts CRUD interface with DataGrid and standard controls
- **OperationsManagementView.axaml** - Operations CRUD interface
- **LocationsManagementView.axaml** - Locations CRUD interface
- **ItemTypesManagementView.axaml** - Item types CRUD interface

#### 5.3 Administration Views
- **UserManagementView.axaml** - User administration interface
- **SystemSettingsView.axaml** - System configuration interface
- **RoleManagementView.axaml** - User roles management

**All UI components will use:**
- Standard Avalonia controls (TextBox, Button, DataGrid, etc.)
- Standard data binding (`{Binding PropertyName}`)
- MTM design system (Purple theme #6a0dad)
- Standard UserControl base class (no ReactiveUserControl)

</details>

<details>
<summary><strong>Phase 6: Enhanced QuickButtons Integration</strong></summary>

**Estimated Time: 1-2 days**

#### 6.1 Expand QuickButtons for All Operations
- Add QuickButtons for Remove operations
- Add QuickButtons for Transfer operations
- Add QuickButtons for Search operations
- Integrate with all new ViewModels using standard event patterns

#### 6.2 QuickButtons Context Awareness
```csharp
// Add to QuickButtonsViewModel - Standard .NET patterns
public void AddQuickButtonFromRemoval(string partId, string operation, string location, int quantity)
{
    // Standard implementation using Helper_Database_StoredProcedure
}

public void AddQuickButtonFromTransfer(string partId, string fromLocation, string toLocation, int quantity)
{
    // Standard implementation using Helper_Database_StoredProcedure
}

public void AddQuickButtonFromSearch(string partId, string operation, string location)
{
    // Standard implementation using Helper_Database_StoredProcedure
}
```

</details>

<details>
<summary><strong>Phase 7: Documentation and Instruction Files Update</strong></summary>

**Estimated Time: 1-2 days**

#### 7.1 Create Comprehensive Stored Procedures Instruction File
**File: `.github/Development-Instructions/stored-procedures.instruction.md`**

Create a complete reference guide containing:

```markdown
# Database Stored Procedures Reference Guide

<details>
<summary><strong>🎯 MTM Database Stored Procedures</strong></summary>

Comprehensive reference for all MTM WIP Application stored procedures including parameters, usage patterns, and implementation examples.

### **Core Principles**
- **When to Use**: Specific business scenarios for each stored procedure
- **Parameter Requirements**: All required and optional parameters
- **Return Values**: Expected data structures and status codes
- **Usage Examples**: Complete C# implementation examples
- **Error Handling**: Common error scenarios and handling patterns

</details>

<details>
<summary><strong>📊 Inventory Management Procedures</strong></summary>

#### inv_inventory_Add_Item
**Purpose**: Add new inventory items to the system
**When to Use**: User adding stock to inventory, receiving new parts

**Parameters**:
```sql
p_PartID VARCHAR(50) -- Required: Part identifier
p_OperationID VARCHAR(10) -- Required: Operation number (workflow step)
p_LocationID VARCHAR(50) -- Required: Storage location
p_Quantity INT -- Required: Quantity to add (must be > 0)
p_ItemType VARCHAR(50) -- Required: Type of item
p_UserID VARCHAR(50) -- Required: User performing operation
p_TransactionType VARCHAR(10) -- Required: Always "IN" for adding
p_Notes TEXT -- Optional: Additional notes
```

**Usage Example**:
```csharp
var parameters = new Dictionary<string, object>
{
    ["p_PartID"] = partId,
    ["p_OperationID"] = operation,
    ["p_LocationID"] = location,
    ["p_Quantity"] = quantity,
    ["p_ItemType"] = itemType,
    ["p_UserID"] = currentUser,
    ["p_TransactionType"] = "IN",
    ["p_Notes"] = notes ?? ""
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item",
    parameters
);
```

**Return Values**:
- Status: 0 (success), -1 (error)
- Message: Success confirmation or error description
- Data: Empty DataTable for this procedure

#### inv_inventory_Remove_Item
**Purpose**: Remove inventory items from the system
**When to Use**: User consuming stock, transferring out, scrapping parts

// Complete documentation for all procedures...

</details>

<details>
<summary><strong>🔧 Master Data Procedures</strong></summary>

#### md_part_ids_Get_All
**Purpose**: Retrieve all available part IDs for autocomplete and validation
**When to Use**: Populating dropdown lists, validating part existence

// Complete documentation...

</details>

<details>
<summary><strong>👥 User Management Procedures</strong></summary>

#### usr_users_Get_All
**Purpose**: Retrieve all system users
**When to Use**: User administration, user selection lists

// Complete documentation...

</details>

<details>
<summary><strong>⚙️ System Configuration Procedures</strong></summary>

#### usr_ui_settings_Get
**Purpose**: Retrieve user-specific UI settings
**When to Use**: Application startup, settings page initialization

// Complete documentation...

</details>

<details>
<summary><strong>🎯 Implementation Patterns</strong></summary>

### Standard Implementation Pattern
```csharp
public async Task<StoredProcedureResult> ExecuteStoredProcedureAsync(
    string procedureName, 
    Dictionary<string, object> parameters)
{
    try
    {
        var connectionString = _configurationService.GetConnectionString();
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            procedureName,
            parameters
        );

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully executed {ProcedureName}", procedureName);
            return result;
        }
        else
        {
            _logger.LogWarning("Stored procedure {ProcedureName} returned error: {Error}", 
                procedureName, result.Message);
            return result;
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to execute stored procedure: {ProcedureName}", procedureName);
        await ErrorHandling.HandleErrorAsync(ex, "ExecuteStoredProcedure", 
            Environment.UserName, new { ProcedureName = procedureName, Parameters = parameters });
        
        return new StoredProcedureResult
        {
            Status = -1,
            Message = $"Error executing {procedureName}: {ex.Message}",
            Data = new DataTable()
        };
    }
}
```

### ViewModel Integration Pattern
```csharp
public class InventoryViewModel : BaseViewModel
{
    private async Task ExecuteAddInventoryAsync()
    {
        if (!ValidateInput()) return;

        var parameters = new Dictionary<string, object>
        {
            ["p_PartID"] = SelectedPart,
            ["p_OperationID"] = SelectedOperation,
            ["p_LocationID"] = SelectedLocation,
            ["p_Quantity"] = Quantity,
            ["p_ItemType"] = SelectedItemType,
            ["p_UserID"] = _applicationStateService.CurrentUser,
            ["p_TransactionType"] = DetermineTransactionType(),
            ["p_Notes"] = Notes
        };

        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "inv_inventory_Add_Item", 
            parameters
        );

        if (result.IsSuccess)
        {
            // Handle success
            await LoadDataAsync();
            ClearInputs();
        }
        else
        {
            // Handle error
            HasError = true;
            ErrorMessage = result.Message;
        }
    }
}
```

</details>

<details>
<summary><strong>🚨 Common Pitfalls and Solutions</strong></summary>

### Parameter Validation
```csharp
// Always validate parameters before calling stored procedures
private bool ValidateParameters(Dictionary<string, object> parameters)
{
    if (string.IsNullOrWhiteSpace(parameters["p_PartID"]?.ToString()))
    {
        ErrorMessage = "Part ID is required";
        return false;
    }
    
    if (!int.TryParse(parameters["p_Quantity"]?.ToString(), out int quantity) || quantity <= 0)
    {
        ErrorMessage = "Quantity must be a positive integer";
        return false;
    }
    
    return true;
}
```

### Transaction Type Logic
```csharp
// CORRECT: Base on user intent, not operation numbers
private string DetermineTransactionType()
{
    return UserIntent switch
    {
        UserIntent.AddingStock => "IN",
        UserIntent.RemovingStock => "OUT", 
        UserIntent.MovingStock => "TRANSFER",
        _ => "IN"
    };
}

// WRONG: Don't use operation numbers to determine transaction type
// if (operation == "90") return "IN"; // This is incorrect
```

</details>
````````

This is the result of the code change application. The Markdown file has been updated to include the new phase for documentation and instruction files update, along with the required details for creating stored procedures documentation and updating related instruction files.
