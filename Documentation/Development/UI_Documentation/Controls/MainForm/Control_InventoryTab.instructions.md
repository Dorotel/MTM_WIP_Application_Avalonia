# Control_InventoryTab.instructions.md

## UI Element Name
**Control_InventoryTab**

## Description
The primary inventory management interface within the MTM WIP Application. This UserControl provides the main functionality for adding new inventory items to the system, including part selection, operation assignment, location specification, quantity entry, and notes. It serves as the most frequently used component for day-to-day inventory operations and includes intelligent validation, progress tracking, and advanced entry capabilities.

## Visual Representation
- **Control Type**: UserControl (TabPage content)
- **Layout**: TableLayoutPanel-based responsive design
- **Primary Components**:
  - Part ID ComboBox with auto-complete
  - Operation ComboBox with validation
  - Location ComboBox with error highlighting
  - Quantity TextBox with numeric validation
  - Notes RichTextBox for additional information
  - Action buttons (Save, Reset, Advanced Entry, Toggle Panel)
  - Version display label

## Component Structure

### **Main Layout Hierarchy**
```
Control_InventoryTab
├── Control_InventoryTab_GroupBox_Main (GroupBox)
│   └── Control_InventoryTab_TableLayout_Main (TableLayoutPanel)
│       ├── Control_InventoryTab_TableLayout_MiddleGroup (TableLayoutPanel)
│       │   ├── Control_InventoryTab_ComboBox_Part (ComboBox)
│       │   ├── Control_InventoryTab_ComboBox_Operation (ComboBox)
│       │   ├── Control_InventoryTab_ComboBox_Location (ComboBox)
│       │   ├── Control_InventoryTab_TextBox_Quantity (TextBox)
│       │   └── Control_InventoryTab_RichTextBox_Notes (RichTextBox)
│       ├── Action Buttons Row
│       │   ├── Control_InventoryTab_Button_Save (Button)
│       │   ├── Control_InventoryTab_Button_Reset (Button)
│       │   ├── Control_InventoryTab_Button_AdvancedEntry (Button)
│       │   └── Control_InventoryTab_Button_Toggle_RightPanel (Button)
│       └── Control_InventoryTab_Label_Version (Label)
```

### **Key Controls and Properties**
| Control Name | Type | Purpose | Validation |
|--------------|------|---------|------------|
| Control_InventoryTab_ComboBox_Part | ComboBox | Part ID selection | Required, error color on invalid |
| Control_InventoryTab_ComboBox_Operation | ComboBox | Operation selection | Required, error color on invalid |
| Control_InventoryTab_ComboBox_Location | ComboBox | Location selection | Required, error color on invalid |
| Control_InventoryTab_TextBox_Quantity | TextBox | Quantity entry | Numeric validation, error color |
| Control_InventoryTab_RichTextBox_Notes | RichTextBox | Additional notes | Optional field |
| Control_InventoryTab_Button_Save | Button | Save inventory item | Enabled only when valid |
| Control_InventoryTab_Button_Reset | Button | Reset form | Shift+Click for hard reset |
| Control_InventoryTab_Button_AdvancedEntry | Button | Open advanced features | Launches Control_AdvancedInventory |

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

### **Model Integration**
- **Model_AppVariables.User**: Current user context for database operations
- **Model_AppVariables.Location**: Selected location state
- **Model_AppVariables.Operation**: Selected operation state  
- **Model_AppVariables.PartId**: Selected part ID state
- **Model_AppVariables.UserUiColors**: Theme-based color configuration

## Interactions/Events

### **Primary Events**
1. **ComboBox Selection Events**
   - `Control_InventoryTab_ComboBox_Part_SelectedIndexChanged`: Updates PartId state and validation
   - `Control_InventoryTab_ComboBox_Operation_SelectedIndexChanged`: Updates Operation state and validation
   - `Control_InventoryTab_ComboBox_Location_SelectedIndexChanged`: Updates Location state and validation

2. **Button Click Events**
   - `Control_InventoryTab_Button_Save_Click`: Validates and saves inventory item
   - `Control_InventoryTab_Button_Reset_Click`: Resets form (soft/hard reset based on Shift key)
   - `Control_InventoryTab_Button_AdvancedEntry_Click`: Opens advanced inventory features
   - `Control_InventoryTab_Button_Toggle_RightPanel_Click`: Toggles quick actions panel

3. **Text Input Events**
   - `Control_InventoryTab_TextBox_Quantity_TextChanged`: Validates quantity input
   - `Control_InventoryTab_TextBox_Quantity_KeyPress`: Numeric-only input enforcement

### **Keyboard Shortcuts**
- **F5**: Reset form (same as Reset button)
- **Enter**: Move to next control or save if on Save button
- **Escape**: Cancel current operation
- **Shift+F5**: Hard reset (refresh all data from database)

### **Validation Logic**
```csharp
private void Control_InventoryTab_Update_SaveButtonState()
{
    // Enable save only when all required fields are valid
    bool isValid = 
        Control_InventoryTab_ComboBox_Part.SelectedIndex > 0 &&
        Control_InventoryTab_ComboBox_Operation.SelectedIndex > 0 &&
        Control_InventoryTab_ComboBox_Location.SelectedIndex > 0 &&
        IsValidQuantity(Control_InventoryTab_TextBox_Quantity.Text);
    
    Control_InventoryTab_Button_Save.Enabled = isValid;
}
```

## Business Logic

### **Inventory Item Creation**
```csharp
private async Task<bool> SaveInventoryItemAsync()
{
    var inventoryResult = await Dao_Inventory.AddInventoryItemAsync(
        user: Model_AppVariables.User,
        partId: Model_AppVariables.PartId,
        operation: Model_AppVariables.Operation,
        location: Model_AppVariables.Location,
        quantity: quantity,
        notes: notes
    );
    
    return inventoryResult.IsSuccess;
}
```

### **Quick Button Integration**
- **Automatic Quick Button Updates**: Successfully saved items are automatically added/updated in the Quick Buttons panel
- **Position Management**: Uses `Dao_QuickButtons.AddOrShiftQuickButtonAsync()` for intelligent button positioning

### **Data Validation Rules**
1. **Part ID**: Must be selected from valid parts list (SelectedIndex > 0)
2. **Operation**: Must be selected from valid operations list (SelectedIndex > 0) 
3. **Location**: Must be selected from valid locations list (SelectedIndex > 0)
4. **Quantity**: Must be a valid positive number
5. **Notes**: Optional field, no validation required

### **Reset Operations**
- **Soft Reset**: Clears form fields, maintains current AutoCompleteBox data
- **Hard Reset**: Refreshes all AutoCompleteBox data from database, resets form fields
- **Progress Feedback**: Shows detailed progress during reset operations

### **User Privilege Enforcement**
- **Read-Only Users**: Cannot save inventory items (Save button disabled)
- **Normal Users**: Can save inventory items within their privileges
- **Admin Users**: Full access to all inventory operations

## Related Files

### **Direct Dependencies**
- `Controls/MainForm/Control_InventoryTab.Designer.cs` - UI layout and control definitions
- `Forms/MainForm/Classes/MainFormControlHelper.cs` - Control state management utilities

### **Business Logic Dependencies**
- `Data/Dao_Inventory.cs` - Core inventory database operations
- `Data/Dao_QuickButtons.cs` - Quick button management
- `Data/Dao_ErrorLog.cs` - Error logging and handling
- `Helpers/Helper_UI_AutoCompleteBoxes.cs` - AutoCompleteBox data management

### **Integration Points**
- `Controls/MainForm/Control_AdvancedInventory.cs` - Advanced inventory features
- `Controls/MainForm/Control_QuickButtons.cs` - Quick actions panel
- `Helpers/Helper_StoredProcedureProgress.cs` - Progress tracking
- `Models/Model_AppVariables.cs` - Application state management

### **Theme and UI Dependencies**
- `Core/Core_Themes.cs` - Theme application and DPI scaling
- `Models/Model_UserUiColors.cs` - Color configuration
- `Services/Service_ErrorHandler.cs` - User-friendly error dialogs

## Notes

### **Performance Optimizations**
- **Async Database Operations**: All database calls use async patterns to prevent UI blocking
- **Smart AutoCompleteBox Loading**: AutoCompleteBox data is cached and only refreshed when necessary
- **Progressive Loading**: Shows progress feedback for all time-consuming operations

### **User Experience Features**
- **Visual Validation**: Invalid fields show error colors immediately
- **Intelligent Focus**: Auto-focuses next logical control after valid input
- **Error Prevention**: Save button only enabled when all required fields are valid
- **Progress Feedback**: Detailed progress messages during save and reset operations

### **Error Handling**
- **Graceful Degradation**: Continues operation even if non-critical components fail
- **Comprehensive Logging**: All errors logged with full context for debugging
- **User-Friendly Messages**: Translates technical errors into actionable user messages

### **Accessibility Features**
- **Keyboard Navigation**: Full keyboard accessibility for all functions
- **Screen Reader Support**: Proper labeling and focus management
- **High Contrast Support**: Respects user theme preferences for visibility

### **Integration Architecture**
- **Loose Coupling**: Uses Helper classes and static references to minimize direct dependencies
- **Event-Driven**: Communicates with parent form and sibling controls via events
- **State Management**: Maintains application state through Model_AppVariables

### **Development Features**
- **Debug Tracing**: Comprehensive tracing of all user actions and business logic
- **Reflection-Based Integration**: Uses reflection for advanced control access patterns
- **Version Display**: Shows current control version for development tracking
