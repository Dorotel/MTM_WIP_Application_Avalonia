# Control_QuickButtons.instructions.md

## UI Element Name
**Control_QuickButtons**

## Description
The quick action buttons interface within the MTM WIP Application. This UserControl provides a customizable panel of frequently used inventory operations, displaying the user's last 10 transactions as clickable buttons for rapid inventory entry. It features dynamic button management, user-specific customization, and seamless integration with the main inventory system.

## Visual Representation
- **Control Type**: UserControl (Right panel content)
- **Layout**: Dynamic button layout with scrolling support
- **Primary Components**:
  - 10 dynamically generated quick action buttons
  - Button management controls (edit, remove, clear all)
  - ToolTip system for detailed button information
  - Auto-layout system for responsive display

## Component Structure

### **Dynamic Button System**
```
Control_QuickButtons
├── quickButtons List<Button> (10 buttons maximum)
│   ├── Button[0] - Position 1: (Operation) - [PartID x Quantity]
│   ├── Button[1] - Position 2: (Operation) - [PartID x Quantity]
│   ├── ...
│   └── Button[9] - Position 10: (Operation) - [PartID x Quantity]
├── ToolTip System
│   └── Control_QuickButtons_Tooltip (ToolTip)
└── Context Menu (Right-click)
    ├── Edit Button
    ├── Remove Button
    └── Clear All Buttons
```

## Props/Inputs

### **Progress Control Integration**
```csharp
public void SetProgressControls(ToolStripProgressBar progressBar, ToolStripStatusLabel statusLabel)
```

### **Main Functionality**
```csharp
public async Task LoadLast10Transactions(string currentUser)
```
- Loads user's last 10 transactions from database
- Populates buttons with Part ID, Operation, and Quantity
- Creates dynamic layout with tooltips

## Interactions/Events

### **Primary Events**
1. **Quick Action Execution**
   - Button click executes inventory operation with stored parameters
   - Automatic inventory entry using stored Part ID, Operation, and Quantity

2. **Button Management**
   - Right-click context menu for editing/removing buttons
   - "Clear All" functionality for resetting user's quick buttons

3. **Dynamic Updates**
   - Automatic button updates when new inventory items are saved
   - Position management and shifting when buttons are removed

## Business Logic

### **Button Data Loading**
```csharp
var dataResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "sys_last_10_transactions_Get_ByUser",
    new Dictionary<string, object> { ["User"] = currentUser }
);
```

### **Button Management Operations**
- **Update**: `Dao_QuickButtons.UpdateQuickButtonAsync()` - Modify existing button
- **Remove**: `Dao_QuickButtons.RemoveQuickButtonAndShiftAsync()` - Remove with position shifting
- **Clear All**: `Dao_QuickButtons.DeleteAllQuickButtonsForUserAsync()` - Reset all user buttons

### **Integration with Inventory**
- Automatic button creation when inventory items are saved
- Position-based management (1-10)
- User-specific button storage and retrieval

## Related Files

### **Business Logic Dependencies**
- `Data/Dao_QuickButtons.cs` - Quick button database operations
- `Helpers/Helper_Database_StoredProcedure.cs` - Database query execution

### **Integration Points**
- `Controls/MainForm/Control_InventoryTab.cs` - Automatic button updates
- `Models/Model_AppVariables.cs` - User and connection state

## Notes

### **User Experience Features**
- **Rapid Access**: One-click inventory entry for frequent operations
- **Visual Feedback**: Clear button text with detailed tooltips
- **Customization**: Right-click editing and management
- **Auto-Update**: Dynamic updates based on user activity

### **Performance Optimizations**
- **Async Loading**: Non-blocking database queries
- **Smart Layout**: Dynamic button sizing and positioning
- **Cached Data**: Efficient button state management