# MainForm.instructions.md

## UI Element Name
**MainForm**

## Description
The primary application window for the MTM (Manitowoc Tool and Manufacturing) WIP Inventory System. This is the central hub that hosts all main application functionality including inventory management, removal operations, transfers, and administrative functions. The form provides a tabbed interface with integrated progress tracking, connection monitoring, and comprehensive menu system.

## Visual Representation
- **Window Type**: Primary Application Window (Form)
- **Layout**: TableLayoutPanel-based with SplitContainer for main content area
- **Components**:
  - MenuStrip with File, View, and Help menus
  - StatusStrip with connection status, progress bar, and status text
  - TabControl with three main tabs (Inventory, Remove, Transfer)
  - SplitContainer hosting tab content and advanced controls
  - Connection strength indicator control

## Component Structure

### **Main Layout Hierarchy**
```
MainForm
├── MainForm_MenuStrip (MenuStrip)
│   ├── File Menu
│   │   ├── Settings
│   │   └── Exit
│   ├── View Menu
│   │   └── Personal History
│   └── Help Menu (Development builds only)
├── MainForm_TableLayout (TableLayoutPanel)
│   ├── MainForm_SplitContainer_Middle (SplitContainer)
│   │   ├── Panel1: MainForm_TabControl (TabControl)
│   │   │   ├── Inventory Tab → Control_InventoryTab
│   │   │   ├── Remove Tab → Control_RemoveTab
│   │   │   └── Transfer Tab → Control_TransferTab
│   │   └── Panel2: Advanced Controls Container
│   │       ├── Control_QuickButtons
│   │       ├── Control_AdvancedInventory
│   │       └── Control_AdvancedRemove
│   └── MainForm_UserControl_SignalStrength (Connection Monitor)
└── MainForm_StatusStrip (StatusStrip)
    ├── Connection Status Label
    ├── Progress Bar
    └── Status Text Label
```

### **Key Controls and Properties**
| Control Name | Type | Purpose |
|--------------|------|---------|
| MainForm_TabControl | TabControl | Main navigation between Inventory, Remove, Transfer |
| MainForm_SplitContainer_Middle | SplitContainer | Separates main tabs from quick actions/advanced features |
| MainForm_StatusStrip | StatusStrip | Displays connection status, progress, and operation feedback |
| MainForm_MenuStrip | MenuStrip | Provides access to settings, views, and help |
| MainForm_UserControl_SignalStrength | Custom Control | Real-time database connection strength indicator |

## Props/Inputs

### **Constructor Parameters**
- None (parameterless constructor)

### **Key Properties**
- **ConnectionRecoveryManager**: Manages database connection recovery
- **ProgressHelper**: Provides progress tracking for operations
- **ConnectionStrengthChecker**: Monitors database connection quality

### **Configuration Dependencies**
- **Model_AppVariables.User**: Current user context
- **Model_AppVariables.UserType**: User privilege level (Admin/Normal/ReadOnly)
- **Model_AppVariables.UserUiColors**: Theme configuration

## Interactions/Events

### **Primary Events**
1. **Form Load Events**
   - `MainForm_Shown`: Initializes all components and sets focus
   - `DpiChanged`: Handles DPI scaling changes for multi-monitor setups

2. **Tab Navigation Events**
   - `MainForm_TabControl_Selecting`: Validates tab changes (warns if unsaved work)
   - `MainForm_TabControl_SelectedIndexChanged`: Resets controls and updates UI state

3. **Menu Events**
   - `MainForm_MenuStrip_File_Settings_Click`: Opens settings dialog
   - `MainForm_MenuStrip_Exit_Click`: Safely closes application
   - `MainForm_MenuStrip_View_PersonalHistory_Click`: Opens transaction history

4. **Panel Toggle Events**
   - SplitContainer panel collapse/expand for quick actions panel

### **Keyboard Shortcuts**
- **F5**: Refresh current tab data
- **Ctrl+S**: Settings dialog
- **Ctrl+H**: Personal history
- **Escape**: Cancel current operation

## Business Logic

### **User Authentication & Authorization**
```csharp
// Title generation based on user privileges
private static string GetUserPrivilegeDisplayText()
{
    if (Model_AppVariables.UserTypeAdmin)
        return "Administrator";
    else if (Model_AppVariables.UserTypeNormal)
        return "Normal User";
    else if (Model_AppVariables.UserTypeReadOnly)
        return "Read Only";
    else
        return "Unknown";
}
```

### **Connection Management**
- **Connection Monitoring**: Real-time database connection strength tracking
- **Recovery Management**: Automatic connection recovery with user notification
- **Status Reporting**: Visual connection status in status strip

### **Progress System Integration**
```csharp
// Centralized progress helper initialization
private void InitializeProgressControl()
{
    _progressHelper = Helper_StoredProcedureProgress.Create(
        MainForm_ProgressBar, 
        MainForm_StatusText, 
        this
    );
}
```

### **Tab State Management**
- **Advanced Control Visibility**: Manages advanced inventory/remove controls
- **Quick Actions Panel**: Collapsible panel with frequently used functions
- **Work Preservation**: Warns users before losing unsaved work on tab changes

### **Theme and DPI Integration**
- **Runtime DPI Scaling**: Automatically adjusts for DPI changes
- **Theme Application**: Applies user-selected color themes
- **Multi-Monitor Support**: Handles DPI changes when moving between monitors

## Related Files

### **Direct Dependencies**
- `Forms/MainForm/MainForm.Designer.cs` - UI layout and control definitions
- `Forms/MainForm/Classes/MainFormControlHelper.cs` - Control management utilities
- `Forms/MainForm/Classes/MainFormTabResetHelper.cs` - Tab reset operations
- `Forms/MainForm/Classes/MainFormUserSettingsHelper.cs` - User settings integration

### **Child Controls**
- `Controls/MainForm/Control_InventoryTab.cs` - Primary inventory management
- `Controls/MainForm/Control_RemoveTab.cs` - Inventory removal operations
- `Controls/MainForm/Control_TransferTab.cs` - Inventory transfer operations
- `Controls/MainForm/Control_QuickButtons.cs` - Quick action buttons
- `Controls/MainForm/Control_AdvancedInventory.cs` - Advanced inventory features
- `Controls/MainForm/Control_AdvancedRemove.cs` - Advanced removal features
- `Controls/Addons/Control_ConnectionStrengthControl.cs` - Connection monitoring

### **Integration Points**
- `Core/Core_Themes.cs` - Theme and DPI scaling
- `Services/Service_ConnectionRecoveryManager.cs` - Connection management
- `Helpers/Helper_StoredProcedureProgress.cs` - Progress tracking
- `Models/Model_AppVariables.cs` - Application state
- `Services/Service_ErrorHandler.cs` - Error handling and dialogs

### **Business Logic Dependencies**
- `Data/Dao_ErrorLog.cs` - Error logging operations
- `Models/Model_UserUiColors.cs` - Theme configuration
- `Logging/LoggingUtility.cs` - Application logging

## Notes

### **Architecture Highlights**
- **MVP Pattern**: Form acts as View with Helper classes providing Presenter functionality
- **Event-Driven**: Extensive use of event delegation for user interactions
- **Progress-Aware**: Integrated progress tracking for all database operations
- **Connection-Resilient**: Built-in connection recovery and monitoring

### **Performance Considerations**
- **Async Operations**: Tab loading and control resets use async patterns
- **Memory Management**: Proper disposal of timers and event handlers
- **DPI Optimization**: Efficient DPI scaling without full form recreation

### **Security Features**
- **User Privilege Enforcement**: UI adjusts based on user permission levels
- **Operation Validation**: Confirms destructive operations before execution
- **Session Management**: Tracks user session for audit purposes

### **Debugging Support**
- **Development Menu**: Special debug menu available in development builds
- **Trace Integration**: Comprehensive tracing of UI actions and business logic
- **Error Recovery**: Graceful error handling with detailed logging

### **Accessibility**
- **Keyboard Navigation**: Full keyboard accessibility for all functions
- **Screen Reader Support**: Proper labeling for assistive technologies
- **High DPI Support**: Scales properly on high-resolution displays