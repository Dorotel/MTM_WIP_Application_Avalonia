# Data Model: RemoveTabView Verification Testing

**Feature**: RemoveTabView implementation verification  
**Date**: 2025-09-27 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Core Entities

### InventoryItem

**Purpose**: Represents a single inventory item in the MTM WIP system  
**Source**: `Models/ViewModels.cs`, `Models/Shared/CoreModels.cs`  
**Database**: Maps to `inv_inventory` table via stored procedures

**Fields**:

- `Id` (int) - Primary key identifier
- `PartId` (string) - Part number (required, non-empty)
- `Location` (string) - Physical location code (FLOOR, RECEIVING, SHIPPING, etc.)
- `Operation` (string) - Manufacturing operation number ("90", "100", "110", "120")
- `Quantity` (int) - Item quantity (must be positive integer)
- `ItemType` (string) - Item classification ("WIP" default)
- `BatchNumber` (string, optional) - Production batch identifier
- `ReceiveDate` (DateTime) - When item was received
- `LastUpdated` (DateTime) - Last modification timestamp
- `User` (string) - User who created/modified the item
- `LastUpdatedBy` (string) - Alias for User field
- `Notes` (string, optional) - Additional comments
- `IsSelected` (bool) - UI selection state for multi-selection operations

**Relationships**:

- Associated with User via `User` field
- Associated with Location via `Location` field
- Associated with Operation via `Operation` field

**Validation Rules**:

- PartId must not be empty
- Operation must be valid manufacturing operation ("90", "100", "110", "120")
- Location must be valid location code
- Quantity must be positive integer
- User field required for audit trail

**State Transitions**:

- Created → Active (via inventory addition)
- Active → Removed (via removal operation)
- Removed → Restored (via undo operation)

### ServiceResult<T>

**Purpose**: Standardized result wrapper for all service operations  
**Source**: `Models/ServiceResult.cs`  
**Usage**: Wraps RemoveService operation results with success/failure status

**Fields**:

- `IsSuccess` (bool) - Operation success indicator
- `Value` (T, optional) - Result data if successful
- `ErrorMessage` (string, optional) - Error description if failed
- `Exception` (Exception, optional) - Exception details if failed

**Validation Rules**:

- Must have either Value (success) or ErrorMessage (failure)
- Exception can be provided with ErrorMessage for detailed error context

### RemovalResult

**Purpose**: Comprehensive result for batch removal operations  
**Source**: `Services/RemoveService.cs`  
**Usage**: Tracks success/failure details for multi-item removal operations

**Fields**:

- `SuccessfulRemovals` (List<InventoryItem>) - Items successfully removed
- `FailedRemovals` (List<RemovalFailure>) - Items that failed to remove
- `SuccessCount` (int) - Count of successful removals
- `FailureCount` (int) - Count of failed removals
- `HasSuccesses` (bool) - True if any items succeeded
- `HasFailures` (bool) - True if any items failed

**Relationships**:

- Contains collections of InventoryItem entities
- Contains collections of RemovalFailure entities

### RemovalFailure

**Purpose**: Details about individual removal failures  
**Source**: `Services/RemoveService.cs`  
**Usage**: Tracks why specific items failed to be removed

**Fields**:

- `Item` (InventoryItem) - The item that failed to remove
- `Error` (string) - Specific error message
- `Exception` (Exception, optional) - Technical exception details

**Relationships**:

- References one InventoryItem entity

### Search Criteria

**Purpose**: Parameters for inventory search operations  
**Source**: RemoveItemViewModel properties  
**Usage**: Defines filtering parameters for inventory queries

**Fields**:

- `PartId` (string, optional) - Filter by part identifier
- `Operation` (string, optional) - Filter by operation number
- `Location` (string, optional) - Filter by location code
- `User` (string, optional) - Filter by user

**Validation Rules**:

- At least one search criteria should be provided for meaningful results
- PartId must match available part identifiers if specified
- Operation must be valid operation number if specified
- Location must be valid location code if specified

### Test Data Models

For verification testing, the following test entities are required:

#### TestInventoryItem

**Purpose**: Test fixture data for inventory operations  
**Fields**: Same as InventoryItem with test-specific values
**Validation Rules**: Must use test-specific identifiers to avoid production data conflicts

#### TestRemovalScenario

**Purpose**: Defines comprehensive test scenarios for removal operations

**Fields**:

- `ScenarioName` (string) - Test scenario identifier
- `TestItems` (List<TestInventoryItem>) - Items to test with
- `ExpectedResults` (RemovalResult) - Expected outcome
- `ValidationCriteria` (List<string>) - Success criteria to validate

#### MockServiceResponse

**Purpose**: Standardized mock responses for service testing

**Fields**:

- `ResponseType` (enum) - Success, Failure, Timeout, Exception
- `Data` (object, optional) - Response payload
- `ErrorMessage` (string, optional) - Error details
- `DelayMs` (int, optional) - Response delay simulation

## Data Flow

### Search Operation Flow

```
1. User Input → SearchCriteria
2. SearchCriteria → RemoveService.SearchInventoryAsync()
3. Service → Database (stored procedures)
4. Database → ServiceResult<List<InventoryItem>>
5. ServiceResult → ViewModel.InventoryItems (ObservableCollection)
6. InventoryItems → UI DataGrid display
```

### Removal Operation Flow

```
1. User Selection → SelectedItems (ObservableCollection<InventoryItem>)
2. SelectedItems → RemoveService.RemoveInventoryItemsAsync()
3. Service → Database (removal stored procedures)
4. Database → ServiceResult<RemovalResult>
5. RemovalResult → UI updates (collection updates, success overlay)
6. Transaction logging → Undo capability
```

### Undo Operation Flow

```
1. User Action → RemoveService.UndoLastRemovalAsync()
2. Service → UndoItems collection (cached removed items)
3. UndoItems → Database (restoration stored procedures)
4. Database → ServiceResult<RemovalResult>
5. RemovalResult → UI updates (restored items display)
```

## Validation Requirements

### Input Validation

- **PartId**: Must exist in available parts list, non-empty string
- **Operation**: Must be valid operation number ("90", "100", "110", "120")
- **Location**: Must be valid location code from master data
- **Quantity**: Must be positive integer, cannot be zero or negative

### Business Rule Validation

- **Transaction Integrity**: All operations must maintain audit trail
- **Concurrent Access**: Handle concurrent modifications gracefully
- **Data Consistency**: Ensure database and UI collections stay synchronized
- **Error Recovery**: Failed operations must not leave partial state

### UI Validation

- **Selection State**: Multi-selection must sync between UI and ViewModel
- **Loading States**: UI must reflect service operation progress
- **Error Display**: Service errors must be presented to user appropriately
- **Success Feedback**: Successful operations must provide user confirmation

## Testing Validation Matrix

| Entity | Validation Type | Test Scenario | Expected Result |
|--------|----------------|---------------|-----------------|
| InventoryItem | Field Validation | Empty PartId | Validation failure |
| InventoryItem | Field Validation | Invalid Operation | Validation failure |
| InventoryItem | Field Validation | Negative Quantity | Validation failure |
| ServiceResult | Success Case | Valid removal operation | IsSuccess = true, Value populated |
| ServiceResult | Failure Case | Database connection failure | IsSuccess = false, ErrorMessage populated |
| RemovalResult | Batch Success | All items removed successfully | HasSuccesses = true, SuccessCount > 0 |
| RemovalResult | Partial Failure | Some items fail removal | HasSuccesses = true, HasFailures = true |
| RemovalResult | Complete Failure | All items fail removal | HasFailures = true, SuccessCount = 0 |

## Constitutional Compliance

### Code Quality Excellence

- **Nullable Reference Types**: All models use nullable annotations where appropriate
- **MVVM Patterns**: INotifyPropertyChanged implemented for UI binding
- **Validation Attributes**: DataAnnotations used for model validation
- **Naming Conventions**: PascalCase for properties, consistent throughout

### Performance Requirements

- **Memory Efficiency**: ObservableCollections used for UI binding efficiency
- **Database Performance**: ServiceResult pattern minimizes database round trips
- **UI Responsiveness**: Async patterns prevent UI blocking during operations

### User Experience Consistency

- **Data Binding**: Models support two-way binding for consistent UI interaction
- **Error Reporting**: Standardized error messages across all operations
- **State Management**: Clear data flow and state transitions for predictable behavior
