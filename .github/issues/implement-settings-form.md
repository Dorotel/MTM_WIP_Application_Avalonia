# 🎛️ Implement Advanced SettingsForm with MVVM Architecture and Dynamic Panel Management

## 📋 Issue Summary
Implement a comprehensive SettingsForm for the MTM WIP Application using Avalonia MVVM patterns, following the implementation guide from the SettingsForm Assessment. This will replace the existing WinForms-based settings interface with a modern, service-integrated administrative hub supporting 19 dynamic panels, advanced theming integration, and comprehensive user/data management capabilities.

## 🎯 Implementation Strategy Based on Assessment Results

### **Selected Architecture Approach**
Based on the completed assessment questionnaire:

1. **🏗️ Architecture**: MVVM with Separate ViewModels per Panel
2. **🔄 Navigation**: TabView with Hidden Tabs + TreeView Navigator  
3. **👥 User Management**: Service Layer with Error Handling Integration
4. **🎨 Theming**: Advanced Theme Builder with Live Preview
5. **💾 State Management**: Per-Panel Change Tracking with State Snapshots
6. **⚡ Performance**: Virtual Panel System with Adaptive Performance

## 📊 Required Components

### **Core Architecture Components**
- [ ] **SettingsFormView.axaml** - Main Avalonia UserControl with TreeView + TabView layout
- [ ] **SettingsFormViewModel.cs** - Main coordinator ViewModel with navigation logic
- [ ] **19 Panel ViewModels** - Individual ViewModels for each settings category
- [ ] **SettingsNavigationService** - Extension of existing Navigation.cs service
- [ ] **SettingsPanelStateManager** - State tracking and snapshot management
- [ ] **VirtualPanelManager** - Adaptive performance-based panel creation

### **Dynamic Panel Structure (19 Panels)**
```
Settings Root
├── Database Settings (DatabaseSettingsViewModel)
├── User Management
│   ├── Add User (AddUserViewModel)
│   ├── Edit User (EditUserViewModel)
│   └── Delete User (DeleteUserViewModel)
├── Part Numbers
│   ├── Add Part Number (AddPartViewModel)
│   ├── Edit Part Number (EditPartViewModel)
│   └── Remove Part Number (RemovePartViewModel)
├── Operations
│   ├── Add Operation (AddOperationViewModel)
│   ├── Edit Operation (EditOperationViewModel)
│   └── Remove Operation (RemoveOperationViewModel)
├── Locations
│   ├── Add Location (AddLocationViewModel)
│   ├── Edit Location (EditLocationViewModel)
│   └── Remove Location (RemoveLocationViewModel)
├── ItemTypes
│   ├── Add ItemType (AddItemTypeViewModel)
│   ├── Edit ItemType (EditItemTypeViewModel)
│   └── Remove ItemType (RemoveItemTypeViewModel)
├── Advanced Theme Builder (ThemeBuilderViewModel)
├── Shortcuts Configuration (ShortcutsViewModel)
└── About Information (AboutViewModel)
```

## 🔧 Technical Implementation Requirements

### **1. Main SettingsForm Architecture**
```csharp
// SettingsFormViewModel.cs - Main coordinator
public class SettingsFormViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly IErrorHandlingService _errorHandling;
    private readonly VirtualPanelManager _panelManager;
    private readonly SettingsPanelStateManager _stateManager;

    // TreeView navigation
    public ObservableCollection<SettingsCategoryViewModel> Categories { get; }
    public SettingsCategoryViewModel? SelectedCategory { get; set; }
    
    // TabView management
    public int SelectedTabIndex { get; set; }
    public ObservableCollection<SettingsPanelViewModel> LoadedPanels { get; }
    
    // State management
    public bool HasUnsavedChanges { get; set; }
    public string CurrentStatusMessage { get; set; }
}
```

### **2. AXAML Layout Structure**
```xml
<!-- SettingsFormView.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Views.SettingsFormView">
    
    <Grid ColumnDefinitions="200,*">
        <!-- TreeView Navigator -->
        <TreeView Grid.Column="0"
                  ItemsSource="{Binding Categories}"
                  SelectedItem="{Binding SelectedCategory}"/>
        
        <!-- Dynamic Content Area -->
        <Grid Grid.Column="1" RowDefinitions="*,Auto">
            <!-- Hidden TabView for Panel Management -->
            <TabView Grid.Row="0"
                     TabStripPlacement="Hidden"
                     SelectedIndex="{Binding SelectedTabIndex}"
                     ItemsSource="{Binding LoadedPanels}"/>
            
            <!-- Status Bar -->
            <Border Grid.Row="1" Classes="status-bar">
                <Grid ColumnDefinitions="*,Auto">
                    <TextBlock Grid.Column="0" Text="{Binding CurrentStatusMessage}"/>
                    <ProgressBar Grid.Column="1" IsVisible="{Binding IsLoading}"/>
                </Grid>
            </Border>
        </Grid>
    </Grid>
</UserControl>
```

### **3. Service Integration Pattern**
```csharp
// Following MTM service organization rules
public interface ISettingsService
{
    Task<Result> SaveUserSettingsAsync(UserSettings settings);
    Task<Result> SaveDatabaseSettingsAsync(DatabaseSettings settings);
    Task<Result> SaveThemeSettingsAsync(ThemeSettings settings);
    Task<Result<T>> LoadSettingsAsync<T>(string category) where T : class;
}

// Add to Configuration.cs following SERVICE FILE ORGANIZATION RULE
public class SettingsService : ISettingsService
{
    private readonly IDatabaseService _database;
    private readonly IErrorHandlingService _errorHandling;
    
    // Implementation using existing Helper_Database_StoredProcedure patterns
}
```

### **4. Advanced Theme Integration**
```csharp
// ThemeBuilderViewModel.cs - Advanced theme customization
public class ThemeBuilderViewModel : BaseViewModel
{
    private readonly IThemeService _themeService;
    
    // Live preview properties
    public ITheme PreviewTheme { get; set; }
    public Color PrimaryColor { get; set; }
    public PerformanceLevel PerformanceLevel { get; set; }
    public AccessibilitySettings AccessibilitySettings { get; set; }
    
    // Commands
    public ICommand ApplyThemeCommand { get; }
    public ICommand CreateCustomThemeCommand { get; }
    public ICommand ExportThemeCommand { get; }
    
    // Live preview panel
    public UserControl ThemePreviewPanel { get; set; }
}
```

### **5. State Management System**
```csharp
// SettingsPanelStateManager.cs - Per-panel state tracking
public class SettingsPanelStateManager
{
    private readonly Dictionary<string, PanelStateSnapshot> _stateSnapshots = new();
    
    public class PanelStateSnapshot
    {
        public string PanelId { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
        public Dictionary<string, object> CurrentValues { get; set; }
        public bool HasChanges => !OriginalValues.SequenceEqual(CurrentValues);
    }
    
    public void CreateSnapshot(string panelId, object viewModel);
    public bool HasUnsavedChanges(string panelId);
    public Task<bool> SaveChangesAsync(string panelId);
    public void RestoreSnapshot(string panelId, object viewModel);
}
```

### **6. Virtual Panel System**
```csharp
// VirtualPanelManager.cs - Adaptive performance management
public class VirtualPanelManager
{
    private readonly IThemeService _themeService;
    private readonly IServiceProvider _serviceProvider;
    
    public SettingsPanelViewModel CreateVirtualPanel(string category)
    {
        var performanceLevel = _themeService.CurrentPerformanceLevel;
        return performanceLevel switch
        {
            PerformanceLevel.High => CreateFullFeaturedPanel(category),
            PerformanceLevel.Medium => CreateStandardPanel(category),
            PerformanceLevel.Low => CreateLightweightPanel(category)
        };
    }
}
```

## 🔗 Integration Points

### **Database Integration**
- [ ] **User Management**: Integrate with `sys_users_Add`, `sys_users_Update`, `sys_users_Delete` stored procedures
- [ ] **Part Numbers**: Use `Dao_PartNumbers` CRUD operations via Database.cs service
- [ ] **Operations**: Use `Dao_Operations` CRUD operations via Database.cs service  
- [ ] **Locations**: Use `Dao_Locations` CRUD operations via Database.cs service
- [ ] **Item Types**: Use `Dao_ItemTypes` CRUD operations via Database.cs service
- [ ] **Settings Persistence**: Store via `Model_AppVariables.Database` and configuration tables

### **Service Layer Integration**
- [ ] **ErrorHandling.cs**: Use `HandleErrorAsync()` for comprehensive error management
- [ ] **Configuration.cs**: Extend with settings management capabilities
- [ ] **Navigation.cs**: Extend with settings navigation support
- [ ] **ThemeService**: Full integration with advanced theming system
- [ ] **Database.cs**: Use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`

### **Advanced Theme System Integration**
- [ ] **Theme Preview**: Real-time theme preview in settings panel
- [ ] **Custom Accessibility**: Accessibility theme builder interface
- [ ] **Performance Adaptation**: Adaptive UI based on system performance
- [ ] **Live Switching**: Instant theme application without restart
- [ ] **Export/Import**: Theme configuration export and import functionality

## 📋 Implementation Roadmap

### **Phase 1: Core Architecture Setup**
1. **Create SettingsFormView.axaml** using `@ui:create` hotkey pattern
2. **Create SettingsFormViewModel.cs** using `@ui:viewmodel` hotkey pattern  
3. **Setup TreeView navigation structure** with hierarchical categories
4. **Implement TabView with hidden tabs** for panel management
5. **Create VirtualPanelManager** for adaptive performance

### **Phase 2: Service Layer Integration**
1. **Extend Configuration.cs** with SettingsService following MTM organization rules
2. **Create SettingsPanelStateManager** for state tracking
3. **Integrate with ErrorHandling.cs** for comprehensive error management
4. **Setup dependency injection** in ServiceCollectionExtensions.cs
5. **Create navigation extensions** in Navigation.cs

### **Phase 3: Panel ViewModels Implementation**
1. **Database Settings Panel** (DatabaseSettingsViewModel)
2. **User Management Panels** (AddUser, EditUser, DeleteUser ViewModels)
3. **Data Management Panels** (Parts, Operations, Locations, ItemTypes)
4. **Advanced Theme Builder Panel** (ThemeBuilderViewModel with live preview)
5. **Shortcuts and About Panels** (ShortcutsViewModel, AboutViewModel)

### **Phase 4: Advanced Features**
1. **State management system** with snapshots and rollback
2. **Theme integration** with live preview and custom builder
3. **Performance optimization** with virtual panel loading
4. **Validation system** with real-time feedback
5. **Import/Export functionality** for settings backup

### **Phase 5: Testing & Quality Assurance**
1. **Unit tests** for all ViewModels and services
2. **Integration tests** for database operations
3. **UI tests** for navigation and state management
4. **Performance tests** for virtual panel system
5. **Accessibility tests** for theme builder

## 🎯 Acceptance Criteria

### **✅ Core Functionality**
- [ ] TreeView navigation works with all 19 categories
- [ ] TabView hidden tabs switch correctly based on TreeView selection
- [ ] All user management operations (Add/Edit/Delete) function correctly
- [ ] Database settings can be modified and saved
- [ ] Part Numbers, Operations, Locations, ItemTypes can be managed

### **✅ Advanced Features** 
- [ ] Theme builder provides live preview of changes
- [ ] Custom accessibility themes can be created and applied
- [ ] Performance adapts based on system capabilities
- [ ] State management preserves unsaved changes across navigation
- [ ] Export/Import functionality works for settings backup

### **✅ Integration Requirements**
- [ ] Integrates seamlessly with existing MTM services
- [ ] Follows MTM service organization patterns
- [ ] Uses ErrorHandling.cs for all error management
- [ ] Integrates with advanced ThemeService from questionnaire
- [ ] Maintains proper Avalonia MVVM architecture

### **✅ Quality Standards**
- [ ] No AVLN2000 compilation errors in AXAML
- [ ] Follows MTM naming conventions and coding standards
- [ ] Proper disposal and memory management
- [ ] Comprehensive error handling with user-friendly messages
- [ ] Accessibility compliance with keyboard navigation

## 🏷️ Implementation Hotkeys

**Recommended Hotkey Sequence:**
```
@ui:create SettingsFormView → @ui:viewmodel SettingsFormViewModel → @sys:config integration → @sys:theme integration → @qa:verify
```

**Supporting Hotkeys:**
- `@ui:create` - Create AXAML views and UserControls
- `@ui:viewmodel` - Create ViewModels with proper MVVM patterns
- `@sys:config` - Integrate with Configuration service
- `@sys:theme` - Integrate with advanced ThemeService
- `@biz:handler` - Create business logic handlers
- `@db:service` - Integrate database operations
- `@qa:verify` - Quality assurance and testing

## 📝 Notes

### **Critical Requirements**
- **Service Organization**: Follow the MTM SERVICE FILE ORGANIZATION RULE
- **No ReactiveUI**: Use standard .NET patterns with INotifyPropertyChanged
- **Database Access**: Use stored procedures only via Helper_Database_StoredProcedure
- **Error Handling**: Use ErrorHandling.HandleErrorAsync() throughout
- **AXAML Compliance**: Prevent AVLN2000 errors with proper Avalonia syntax

### **Integration Dependencies**
- Requires completion of advanced ThemeService implementation
- Depends on existing Configuration.cs, ErrorHandling.cs, Navigation.cs services
- Needs Database.cs service for all data operations
- Must integrate with InventoryTabView theming patterns

## 🎯 Priority
**High** - This provides the comprehensive administrative interface needed for MTM WIP Application configuration and user management.

## 🏷️ Labels
- `enhancement`
- `ui`
- `mvvm`
- `settings-management`
- `service-integration`
- `advanced-theming`
- `mtm-patterns`

## 👥 Assignee
@developer (assign to appropriate team member)

## 📅 Target Milestone
Next major release supporting comprehensive settings management and advanced theming capabilities

---

**Implementation Status**: Ready for development using assessment-based architecture decisions and MTM established patterns.
