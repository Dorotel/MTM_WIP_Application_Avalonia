# Data Model: TransferTabView Implementation

## Core Entities

### TransferTabViewModel

**Purpose**: Main ViewModel for TransferTabView following MVVM Community Toolkit patterns
**Base Class**: BaseViewModel (MTM standard)
**Attributes**:

- `[ObservableProperty] string partText` - Part ID search input
- `[ObservableProperty] string operationText` - Operation search input  
- `[ObservableProperty] string toLocation` - Destination location
- `[ObservableProperty] int transferQuantity` - Transfer amount
- `[ObservableProperty] bool isLoading` - Loading state indicator
- `[ObservableProperty] ObservableCollection<InventoryItem> inventoryItems` - Search results
- `[ObservableProperty] InventoryItem? selectedItem` - Selected inventory row
- `[ObservableProperty] bool isEditDialogVisible` - EditInventoryView visibility
- `[ObservableProperty] List<string> availableColumns` - Column customization options
- `[ObservableProperty] List<string> selectedColumns` - User's column selection

**Commands**:

- `[RelayCommand] SearchCommand` - Execute inventory search
- `[RelayCommand] TransferCommand` - Execute transfer operation
- `[RelayCommand] ResetCommand` - Clear form fields
- `[RelayCommand] EditCommand` - Open EditInventoryView
- `[RelayCommand] SaveColumnPreferencesCommand` - Persist column settings

**Relationships**:

- Contains EditInventoryViewModel for overlay functionality
- References MasterDataService for location validation
- Uses TransferService for business operations

### InventoryItem

**Purpose**: Represents inventory row data for DataGrid binding
**Source**: Database query results with strict column mapping
**Attributes**:

- `string PartId` - Maps to "PartID" database column
- `string Operation` - Maps to "Operation" database column
- `string FromLocation` - Maps to "FromLocation" database column
- `int AvailableQuantity` - Maps to "AvailableQuantity" database column
- `int TransferQuantity` - User input for transfer amount
- `string Notes` - Maps to "Notes" database column
- `string BatchNumber` - Preserved during transfer operations
- `DateTime CreatedDate` - Audit information
- `string CreatedBy` - Original creator

**Validation Rules**:

- PartId: Required, non-empty string
- Operation: Must be valid operation number (90/100/110/120)
- AvailableQuantity: Must be positive integer
- TransferQuantity: Must be ≤ AvailableQuantity, auto-capped by UI

### TransferOperation

**Purpose**: Represents single transfer transaction for audit trail
**Attributes**:

- `string TransactionId` - Unique identifier
- `string PartId` - Part being transferred
- `string Operation` - Operation context
- `string FromLocation` - Source location
- `string ToLocation` - Destination location
- `int OriginalQuantity` - Pre-transfer amount
- `int TransferredQuantity` - Amount moved
- `int RemainingQuantity` - Amount left at source
- `string BatchNumber` - Preserved batch identifier
- `string UserId` - Operator performing transfer
- `DateTime TransferDate` - Transaction timestamp
- `string SplitDetails` - JSON details for partial transfers

**State Transitions**:

- Pending → Processing → Completed
- Error states: ValidationFailed, DatabaseError, Cancelled

### ColumnConfiguration

**Purpose**: User preferences for DataGrid column customization
**Storage**: MySQL usr_ui_settings.SettingsJson["TransferTabColumns"]
**Attributes**:

- `string UserId` - User identifier
- `List<string> VisibleColumns` - Selected column names
- `Dictionary<string, int> ColumnOrder` - Display order preferences
- `Dictionary<string, int> ColumnWidths` - User-adjusted widths
- `DateTime LastModified` - Preference update timestamp

**Default Configuration**:

```json
{
  "VisibleColumns": ["PartID", "Operation", "FromLocation", "AvailableQuantity", "TransferQuantity", "Notes"],
  "ColumnOrder": { "PartID": 0, "Operation": 1, "FromLocation": 2, "AvailableQuantity": 3, "TransferQuantity": 4, "Notes": 5 },
  "ColumnWidths": { "PartID": 120, "Operation": 80, "FromLocation": 100, "AvailableQuantity": 120, "TransferQuantity": 120, "Notes": 200 }
}
```

## Service Contracts

### ITransferService

**Purpose**: Business logic for transfer operations
**Methods**:

- `Task<ServiceResult<List<InventoryItem>>> SearchInventoryAsync(string partId, string operation)`
- `Task<ServiceResult> ExecuteTransferAsync(TransferOperation transfer)`
- `Task<ServiceResult> ValidateTransferAsync(TransferOperation transfer)`
- `Task<ServiceResult<List<string>>> GetValidLocationsAsync()`

### IColumnConfigurationService  

**Purpose**: User preference persistence
**Methods**:

- `Task<ServiceResult<ColumnConfiguration>> LoadColumnConfigAsync(string userId)`
- `Task<ServiceResult> SaveColumnConfigAsync(string userId, ColumnConfiguration config)`
- `Task<ServiceResult> ResetToDefaultsAsync(string userId)`

## Database Schema Dependencies

### Existing Tables

- `inv_inventory` - Source data for transfer operations
- `usr_ui_settings` - User preference storage
- `inv_transactions` - Audit trail storage

### New Stored Procedures Required

- `usr_ui_settings_Get_TransferColumns` - Load column preferences
- `usr_ui_settings_Set_TransferColumns` - Save column preferences  
- `inv_transfer_Execute_WithSplit` - Handle partial transfer operations
- `inv_transfer_Validate_Operation` - Pre-transfer validation

## Validation Rules

### Business Rules

- Transfer quantity cannot exceed available quantity (auto-capped in UI)
- Destination location must exist in master data
- Operation must be valid for the part
- Batch numbers must be preserved during transfers
- Single transaction audit trail required

### UI Validation

- Part ID and Operation required for search
- Destination location required for transfer
- Numeric validation on quantity fields
- Real-time validation feedback with error styling

## Error Handling Patterns

### Database Errors

- Connection failures: Show error, require manual retry
- Timeout errors: Display timeout message with retry option
- Validation errors: Highlight problematic fields

### UI Errors  

- AVLN2000 prevention: Use x:Name instead of Name on controls
- Theme resolution: All styling via DynamicResource
- Memory leaks: Proper IDisposable implementation
