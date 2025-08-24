# Control_RemoveTab.instructions.md

## UI Element Name
**Control_RemoveTab**

## Description
The inventory removal interface within the MTM WIP Application. This UserControl provides comprehensive functionality for removing inventory items from the system, including search capabilities, batch deletion operations, undo functionality, and transaction history tracking. It features a DataGridView-based interface for viewing and selecting inventory items to remove, with built-in validation and audit trail capabilities.

## Visual Representation
- **Control Type**: UserControl (TabPage content)
- **Layout**: TableLayoutPanel-based responsive design with DataGridView
- **Primary Components**:
  - Part ID ComboBox for filtering
  - Operation ComboBox for refined filtering 
  - Search/Reset buttons for data retrieval
  - DataGridView for inventory display and selection
  - Action buttons (Delete, Undo, Advanced Removal, Toggle Panel)
  - "Nothing Found" indicator image
  - Print functionality for inventory reports

## Component Structure

### **Main Layout Hierarchy**
```
Control_RemoveTab
├── Control_RemoveTab_GroupBox_MainControl (GroupBox)
│   └── Control_RemoveTab_Panel_Main (TableLayoutPanel)
│       ├── Search Controls Row
│       │   ├── Control_RemoveTab_ComboBox_Part (ComboBox)
│       │   ├── Control_RemoveTab_ComboBox_Operation (ComboBox)
│       │   ├── Control_RemoveTab_Button_Search (Button)
│       │   └── Control_RemoveTab_Button_Reset (Button)
│       ├── Data Display Area
│       │   ├── Control_RemoveTab_Panel_DataGridView (Panel)
│       │   │   ├── Control_RemoveTab_DataGridView_Main (DataGridView)
│       │   │   └── Control_RemoveTab_Image_NothingFound (PictureBox)
│       └── Action Controls Row
│           ├── Control_RemoveTab_Button_Delete (Button)
│           ├── Control_RemoveTab_Button_Undo (Button)
│           ├── Control_RemoveTab_Button_AdvancedItemRemoval (Button)
│           ├── Control_RemoveTab_Button_Print (Button)
│           └── Control_RemoveTab_Button_Toggle_RightPanel (Button)
```

### **Key Controls and Properties**
| Control Name | Type | Purpose | Functionality |
|--------------|------|---------|---------------|
| Control_RemoveTab_ComboBox_Part | ComboBox | Part ID filtering | Searches inventory by specific part |
| Control_RemoveTab_ComboBox_Operation | ComboBox | Operation filtering | Refines search by operation |
| Control_RemoveTab_DataGridView_Main | DataGridView | Inventory display | Shows searchable/selectable inventory |
| Control_RemoveTab_Button_Search | Button | Execute search | Queries database based on criteria |
| Control_RemoveTab_Button_Delete | Button | Remove selected items | Batch deletion with transaction logging |
| Control_RemoveTab_Button_Undo | Button | Restore last removal | Reverses last delete operation |
| Control_RemoveTab_Button_Print | Button | Print inventory | Generates printable inventory report |
| Control_RemoveTab_Image_NothingFound | PictureBox | No results indicator | Visual feedback when search returns no results |

## Props/Inputs

### **Constructor Parameters**
- None (parameterless constructor)

### **Progress Control Integration**
```csharp
public void SetProgressControls(ToolStripProgressBar progressBar, ToolStripStatusLabel statusLabel)
```
- **progressBar**: Main form progress bar for operation feedback
- **statusLabel**: Status label for descriptive progress messages

### **Static Dependencies**
- **MainFormInstance**: Static reference to parent MainForm for inter-control communication

### **Internal State Management**
- **_lastRemovedItems**: List<Model_HistoryRemove> for undo functionality
- **_progressHelper**: Helper_StoredProcedureProgress for operation feedback

## Interactions/Events

### **Primary Events**
1. **Search Operations**
   - `Control_RemoveTab_Button_Search_Click`: Executes inventory search with progress tracking
   - `Control_RemoveTab_Button_Reset_Click`: Clears search criteria and refreshes all data

2. **Removal Operations**
   - `Control_RemoveTab_Button_Delete_Click`: Batch deletes selected items with transaction logging
   - `Control_RemoveTab_Button_Undo_Click`: Restores last deleted items

3. **Navigation and Integration**
   - `Control_RemoveTab_Button_AdvancedItemRemoval_Click`: Opens advanced removal features
   - `Control_RemoveTab_Button_Toggle_RightPanel_Click`: Toggles quick actions panel
   - `Control_RemoveTab_Button_Print_Click`: Generates printable inventory report

4. **Data Selection Events**
   - DataGridView row selection for batch operations
   - Multi-row selection support for batch deletions

### **Keyboard Shortcuts**
- **F5**: Execute search (same as Search button)
- **Delete**: Remove selected items (same as Delete button)
- **Ctrl+Z**: Undo last removal operation
- **Ctrl+P**: Print current inventory view
- **Escape**: Cancel current operation

### **Search Logic**
```csharp
// Dynamic search based on selection criteria
if (!string.IsNullOrWhiteSpace(operation) && operationSelected)
{
    // Search by both part and operation
    var result = await Dao_Inventory.GetInventoryByPartIdAndOperationAsync(partId, operation, true);
}
else
{
    // Search by part only
    var result = await Dao_Inventory.GetInventoryByPartIdAsync(partId, true);
}
```

## Business Logic

### **Inventory Removal Process**
```csharp
private async void Control_RemoveTab_Button_Delete_Click()
{
    // 1. Validate selection
    if (selectedCount == 0) return;
    
    // 2. Execute removal with progress tracking
    var removeResult = await Dao_Inventory.RemoveInventoryItemsFromDataGridViewAsync(dgv, true);
    
    // 3. Log transaction history for audit trail
    foreach (removed item)
    {
        Model_TransactionHistory transaction = new()
        {
            TransactionType = "OUT",
            PartId = item.PartId,
            Operation = item.Operation,
            Location = item.Location,
            Quantity = item.Quantity,
            User = Model_AppVariables.User,
            Timestamp = DateTime.Now
        };
        await Dao_History.AddTransactionHistoryAsync(transaction);
    }
    
    // 4. Store for undo capability
    _lastRemovedItems.AddRange(removedItems);
}
```

### **Undo Functionality**
```csharp
private async void Control_RemoveTab_Button_Undo_Click()
{
    foreach (var item in _lastRemovedItems)
    {
        // Restore item to inventory
        await Dao_Inventory.AddInventoryItemAsync(
            user: Model_AppVariables.User,
            partId: item.PartId,
            operation: item.Operation,
            location: item.Location,
            quantity: item.Quantity,
            notes: $"Restored via Undo: {item.Notes}"
        );
    }
    _lastRemovedItems.Clear();
}
```

### **Data Display and Filtering**
- **Column Management**: Shows only relevant columns (Location, PartID, Operation, Quantity, Notes)
- **Theme Integration**: Applies user-selected themes to DataGridView
- **Progressive Loading**: Shows progress during data retrieval operations
- **Empty State Handling**: Displays "Nothing Found" image when no results

### **Transaction Auditing**
- **Complete Audit Trail**: Every removal logged in inv_transaction table
- **User Attribution**: All operations attributed to current user
- **Timestamp Tracking**: Precise timestamps for all operations
- **Batch Operation Support**: Handles multiple item removals in single transaction

### **Print Integration**
- **Selective Printing**: Prints only visible columns
- **Formatted Output**: Uses Core_DgvPrinter for professional formatting
- **Search Context**: Includes search criteria in print header

## Related Files

### **Direct Dependencies**
- `Controls/MainForm/Control_RemoveTab.Designer.cs` - UI layout and control definitions
- `Forms/MainForm/Classes/MainFormControlHelper.cs` - Control state management utilities

### **Business Logic Dependencies**
- `Data/Dao_Inventory.cs` - Core inventory database operations
  - `RemoveInventoryItemsFromDataGridViewAsync()` - Batch removal operations
  - `GetInventoryByPartIdAsync()` - Part-based inventory queries
  - `GetInventoryByPartIdAndOperationAsync()` - Combined part/operation queries
  - `AddInventoryItemAsync()` - Undo operation support
- `Data/Dao_History.cs` - Transaction history logging
- `Data/Dao_User.cs` - User information retrieval
- `Data/Dao_ErrorLog.cs` - Error logging and handling

### **Integration Points**
- `Controls/MainForm/Control_AdvancedRemove.cs` - Advanced removal features
- `Controls/MainForm/Control_QuickButtons.cs` - Quick actions panel
- `Core/Core_DgvPrinter.cs` - DataGridView printing functionality
- `Core/Core_Themes.cs` - Theme application and DataGridView styling

### **Helper Dependencies**
- `Helpers/Helper_StoredProcedureProgress.cs` - Progress tracking
- `Helpers/Helper_UI_ComboBoxes.cs` - ComboBox data management
- `Helpers/Helper_Database_StoredProcedure.cs` - Database operation utilities

### **Model Dependencies**
- `Models/Model_HistoryRemove.cs` - Removed item data structure
- `Models/Model_TransactionHistory.cs` - Transaction audit data structure
- `Models/Model_AppVariables.cs` - Application state management

## Notes

### **Performance Optimizations**
- **Async Database Operations**: All database calls use async patterns to prevent UI blocking
- **Batch Processing**: Supports efficient batch removal operations
- **Progressive Loading**: Shows detailed progress for time-consuming operations
- **Smart Data Retrieval**: Only queries necessary data based on search criteria

### **User Experience Features**
- **Visual Feedback**: Clear progress indication during all operations
- **Undo Capability**: Allows reversal of accidental deletions
- **Batch Operations**: Efficient multi-item selection and removal
- **Search Refinement**: Flexible search by part and/or operation
- **Empty State Handling**: Clear visual indication when no data found

### **Security and Auditing**
- **Complete Transaction Log**: Every removal operation fully audited
- **User Attribution**: All operations traced to specific users
- **Privilege Enforcement**: Respects user permission levels
- **Error Recovery**: Graceful handling of database errors with rollback capability

### **Data Integrity Features**
- **Transaction Safety**: Database operations wrapped in transactions
- **Validation**: Validates data before removal operations
- **Relationship Preservation**: Maintains referential integrity during removals
- **History Preservation**: Maintains complete removal history for reporting

### **Integration Architecture**
- **Modular Design**: Clean separation between UI and business logic
- **Event-Driven Communication**: Communicates with parent form via events
- **Shared Progress System**: Integrates with application-wide progress tracking
- **Theme Consistency**: Automatically applies user-selected themes

### **Advanced Features**
- **Multi-Column Filtering**: Supports complex search criteria
- **Professional Printing**: High-quality formatted output
- **Advanced Removal Integration**: Seamless transition to advanced features
- **Panel Management**: Intelligent quick actions panel integration

### **Error Handling and Recovery**
- **Comprehensive Error Logging**: All errors logged with full context
- **User-Friendly Messages**: Technical errors translated to actionable feedback
- **Graceful Degradation**: Continues operation even with non-critical failures
- **Recovery Mechanisms**: Automatic retry and recovery for transient failures