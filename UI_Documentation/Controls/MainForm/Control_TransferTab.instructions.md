# Control_TransferTab.instructions.md

## UI Element Name
**Control_TransferTab**

## Description
The inventory transfer interface within the MTM WIP Application. This UserControl provides comprehensive functionality for transferring inventory items between locations, including search capabilities, quantity specification, batch transfer operations, and complete transaction history tracking. It features a DataGridView-based interface for viewing and selecting inventory items to transfer, with intelligent quantity management and audit trail capabilities.

## Visual Representation
- **Control Type**: UserControl (TabPage content)
- **Layout**: TableLayoutPanel-based responsive design with DataGridView
- **Primary Components**:
  - Part ID ComboBox for filtering
  - Operation ComboBox for refined filtering
  - To Location ComboBox for destination selection
  - Quantity NumericUpDown for transfer amount specification
  - Search/Reset buttons for data retrieval
  - DataGridView for inventory display and selection
  - Transfer button for executing operations
  - Print functionality for transfer reports

## Component Structure

### **Main Layout Hierarchy**
```
Control_TransferTab
├── Control_TransferTab_GroupBox_Main (GroupBox)
│   └── Control_TransferTab_TableLayout_Main (TableLayoutPanel)
│       ├── Search Controls Row
│       │   ├── Control_TransferTab_ComboBox_Part (ComboBox)
│       │   ├── Control_TransferTab_ComboBox_Operation (ComboBox)
│       │   ├── Control_TransferTab_Button_Search (Button)
│       │   └── Control_TransferTab_Button_Reset (Button)
│       ├── Transfer Configuration Row
│       │   ├── Control_TransferTab_ComboBox_ToLocation (ComboBox)
│       │   └── Control_TransferTab_NumericUpDown_Quantity (NumericUpDown)
│       ├── Data Display Area
│       │   ├── Control_TransferTab_Panel_DataGridView (Panel)
│       │   │   ├── Control_TransferTab_DataGridView_Main (DataGridView)
│       │   │   └── Control_TransferTab_Image_NothingFound (PictureBox)
│       └── Action Controls Row
│           ├── Control_TransferTab_Button_Transfer (Button)
│           ├── Control_TransferTab_Button_Print (Button)
│           └── Control_TransferTab_Button_Toggle_RightPanel (Button)
```

### **Key Controls and Properties**
| Control Name | Type | Purpose | Functionality |
|--------------|------|---------|---------------|
| Control_TransferTab_ComboBox_Part | ComboBox | Part ID filtering | Searches inventory by specific part |
| Control_TransferTab_ComboBox_Operation | ComboBox | Operation filtering | Refines search by operation |
| Control_TransferTab_ComboBox_ToLocation | ComboBox | Destination location | Specifies transfer destination |
| Control_TransferTab_NumericUpDown_Quantity | NumericUpDown | Transfer quantity | Specifies amount to transfer |
| Control_TransferTab_DataGridView_Main | DataGridView | Inventory display | Shows searchable/selectable inventory |
| Control_TransferTab_Button_Search | Button | Execute search | Queries database based on criteria |
| Control_TransferTab_Button_Transfer | Button | Execute transfer | Performs transfer operations |
| Control_TransferTab_Button_Print | Button | Print inventory | Generates printable transfer report |
| Control_TransferTab_Image_NothingFound | PictureBox | No results indicator | Visual feedback when search returns no results |

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
- **SharedToolTip**: Cached ToolTip instance for performance optimization

### **Internal State Management**
- **_progressHelper**: Helper_StoredProcedureProgress for operation feedback

## Interactions/Events

### **Primary Events**
1. **Search Operations**
   - `Control_TransferTab_Button_Search_Click`: Executes inventory search with progress tracking
   - `Control_TransferTab_Button_Reset_Click`: Clears search criteria and refreshes all data

2. **Transfer Operations**
   - `Control_TransferTab_Button_Transfer_Click`: Executes transfer operations based on selection
   - Supports both single-row and multi-row transfer operations

3. **Configuration Events**
   - `Control_TransferTab_ComboBox_ToLocation_SelectedIndexChanged`: Validates destination location
   - `Control_TransferTab_NumericUpDown_Quantity_ValueChanged`: Updates transfer quantity

4. **Navigation and Integration**
   - `Control_TransferTab_Button_Toggle_RightPanel_Click`: Toggles quick actions panel
   - `Control_TransferTab_Button_Print_Click`: Generates printable transfer report

### **Keyboard Shortcuts**
- **F5**: Execute search (same as Search button)
- **Enter**: Execute transfer (same as Transfer button)
- **Ctrl+P**: Print current inventory view
- **Escape**: Cancel current operation

### **Transfer Validation Logic**
```csharp
private bool ValidateTransferOperation()
{
    // Validate destination location is selected
    if (Control_TransferTab_ComboBox_ToLocation.SelectedIndex <= 0)
        return false;
    
    // Validate transfer quantity
    if (Control_TransferTab_NumericUpDown_Quantity.Value <= 0)
        return false;
    
    // Validate selection exists
    if (Control_TransferTab_DataGridView_Main.SelectedRows.Count == 0)
        return false;
        
    return true;
}
```

## Business Logic

### **Transfer Operation Types**

#### **1. Partial Quantity Transfer**
```csharp
// When transfer quantity < original quantity
if (transferQuantity < originalQuantity)
{
    await Dao_Inventory.TransferInventoryQuantityAsync(
        batchNumber, partId, operation, 
        transferQuantity, originalQuantity, 
        newLocation, user
    );
}
```

#### **2. Complete Item Transfer**
```csharp
// When transfer quantity >= original quantity
else
{
    await Dao_Inventory.TransferPartSimpleAsync(
        batchNumber, partId, operation, 
        quantityStr, newLocation
    );
}
```

### **Transaction History Logging**
```csharp
await Dao_History.AddTransactionHistoryAsync(new Model_TransactionHistory
{
    TransactionType = "TRANSFER",
    PartId = partId,
    FromLocation = fromLocation,
    ToLocation = newLocation,
    Operation = operation,
    Quantity = transferQuantity,
    Notes = notes,
    User = user,
    ItemType = itemType,
    BatchNumber = batchNumber,
    DateTime = DateTime.Now
});
```

### **Search and Data Retrieval**
- **Dynamic Search Logic**: Searches by part only or part+operation based on criteria
- **Progressive Loading**: Shows detailed progress during data retrieval
- **Column Management**: Displays relevant columns with proper ordering
- **Theme Integration**: Applies user-selected themes to DataGridView

### **Quantity Management**
- **Smart Quantity Control**: NumericUpDown automatically limits to available quantity
- **Batch Transfer Support**: Handles multiple item transfers efficiently
- **Validation**: Prevents invalid transfer quantities
- **Inventory Preservation**: Maintains source inventory when partial transfers occur

### **User Interface Features**
- **Status Updates**: Real-time transfer status in status strip
- **Visual Feedback**: Clear progress indication during all operations
- **Error Prevention**: Validation prevents invalid transfer operations
- **Empty State Handling**: Clear visual indication when no data found

## Related Files

### **Direct Dependencies**
- `Controls/MainForm/Control_TransferTab.Designer.cs` - UI layout and control definitions
- `Forms/MainForm/Classes/MainFormControlHelper.cs` - Control state management utilities

### **Business Logic Dependencies**
- `Data/Dao_Inventory.cs` - Core inventory database operations
  - `GetInventoryByPartIdAsync()` - Part-based inventory queries
  - `GetInventoryByPartIdAndOperationAsync()` - Combined part/operation queries
  - `TransferInventoryQuantityAsync()` - Partial quantity transfers
  - `TransferPartSimpleAsync()` - Complete item transfers
- `Data/Dao_History.cs` - Transaction history logging
- `Data/Dao_User.cs` - User information retrieval
- `Data/Dao_ErrorLog.cs` - Error logging and handling

### **Integration Points**
- `Controls/MainForm/Control_QuickButtons.cs` - Quick actions panel
- `Core/Core_DgvPrinter.cs` - DataGridView printing functionality
- `Core/Core_Themes.cs` - Theme application and DataGridView styling

### **Helper Dependencies**
- `Helpers/Helper_StoredProcedureProgress.cs` - Progress tracking
- `Helpers/Helper_UI_ComboBoxes.cs` - ComboBox data management

### **Model Dependencies**
- `Models/Model_TransactionHistory.cs` - Transaction audit data structure
- `Models/Model_AppVariables.cs` - Application state management

## Notes

### **Performance Optimizations**
- **Async Database Operations**: All database calls use async patterns to prevent UI blocking
- **Cached ToolTip**: Shared ToolTip instance to avoid repeated instantiation
- **Progressive Loading**: Shows detailed progress for time-consuming operations
- **Smart Data Retrieval**: Only queries necessary data based on search criteria

### **Transfer Operation Features**
- **Flexible Quantity Management**: Supports both partial and complete transfers
- **Batch Transfer Support**: Efficient multi-item transfer operations
- **Location Validation**: Prevents transfers to invalid or same locations
- **Inventory Integrity**: Maintains accurate inventory counts during transfers

### **Security and Auditing**
- **Complete Transaction Log**: Every transfer operation fully audited
- **User Attribution**: All operations traced to specific users
- **Location Tracking**: Full from/to location audit trail
- **Quantity Verification**: Validates transfer quantities against available inventory

### **Data Integrity Features**
- **Transaction Safety**: Database operations wrapped in transactions
- **Quantity Validation**: Prevents over-transfer scenarios
- **Location Verification**: Validates destination locations exist
- **Batch Number Preservation**: Maintains batch tracking through transfers

### **User Experience Enhancements**
- **Intelligent Defaults**: Smart default values for common operations
- **Visual Progress**: Clear feedback during all transfer operations
- **Error Prevention**: UI validation prevents invalid operations
- **Status Communication**: Real-time status updates in main form

### **Integration Architecture**
- **Modular Design**: Clean separation between UI and business logic
- **Event-Driven Communication**: Communicates with parent form via events
- **Shared Progress System**: Integrates with application-wide progress tracking
- **Theme Consistency**: Automatically applies user-selected themes

### **Advanced Features**
- **Multi-Row Processing**: Handles complex batch transfer scenarios
- **Smart Quantity Calculation**: Automatically limits transfers to available quantities
- **Professional Printing**: High-quality formatted transfer reports
- **Panel Management**: Intelligent quick actions panel integration

### **Error Handling and Recovery**
- **Comprehensive Error Logging**: All errors logged with full context
- **User-Friendly Messages**: Technical errors translated to actionable feedback
- **Graceful Degradation**: Continues operation even with non-critical failures
- **Transaction Rollback**: Automatic rollback on transfer failures