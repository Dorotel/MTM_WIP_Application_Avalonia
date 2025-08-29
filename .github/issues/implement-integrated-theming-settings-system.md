# 🎨⚙️ Implement Integrated Theming & Settings System with Unified Data Management Views

## 📋 Issue Summary
Implement a unified Theming and Settings system for the MTM WIP Application with **user-friendly unified data management views** that combine Add, Edit, and Remove operations into single, intuitive interfaces. This design follows modern UI/UX patterns with action-based interfaces rather than separate modal views for each operation.

## 🎯 Implementation Strategy Based on Questionnaire Results

### **Architecture Decision Summary**
Based on the completed questionnaire responses and modern UI/UX best practices:

1. **🏗️ Integration Approach**: Theme system fully integrated into settings form architecture (Q1: C)
2. **🚪 Entry Points**: Both settings panel and quick-switcher options (Q2: C)  
3. **👁️ Preview System**: Real-time application theming with instant feedback (Q3: D)
4. **⚙️ Service Integration**: Integrated with Configuration service following MTM patterns (Q4: B)
5. **📦 Registration**: Combined `services.AddMTMThemeAndSettingsServices()` registration (Q5: C)
6. **🔄 State Management**: Separate state managers with coordination layer (Q6: B)
7. **🎨 Unified UX**: Single views with action modes for better user experience

## 🏗️ Required Components

### **Core Service Architecture**
- [ ] **ThemeService.cs** - Integrated with Configuration service following MTM patterns
- [ ] **SettingsService.cs** - Unified settings management service  
- [ ] **ThemeStateManager.cs** - Separate theme state management with coordination
- [ ] **SettingsPanelStateManager.cs** - Settings panel state tracking
- [ ] **VirtualPanelManager.cs** - Performance-optimized panel loading

### **Data Persistence Layer**
- [ ] **Theme Preferences Table** - Separate database table linked to user settings (Q7: B)
- [ ] **Theme Export/Import Service** - Separate theme backup functionality (Q8: B)
- [ ] **Hierarchical Configuration** - System > Company > User > Custom hierarchy (Q9: C)

### **UI Components Architecture**
- [ ] **SettingsFormView.axaml** - Main settings interface with TreeView navigation
- [ ] **ThemeBuilderView.axaml** - Advanced Theme Builder as settings panel (Q10: A)
- [ ] **ThemeQuickSwitcher.axaml** - Toolbar quick-switcher component
- [ ] **Card-based Theme Selection** - Theme selection with live preview (Q11: C)
- [ ] **Integrated Accessibility Builder** - Accessibility options in main theme builder (Q12: A)

### **Unified Data Management Panels (9 Total)**
```
Settings Root
├── Theme & Appearance
│   ├── Theme Selection (Card-based with live preview)
│   ├── Advanced Theme Builder (Integrated accessibility)
│   └── Theme Export/Import
├── Database Settings (DatabaseSettingsViewModel)
├── User Management (UnifiedUserManagementView)
│   └── Single view with: List, Add/Edit Form, Action Buttons
├── Part Numbers (UnifiedPartManagementView)
│   └── Single view with: Search/Filter, Add/Edit Form, Bulk Actions
├── Operations (UnifiedOperationManagementView)
│   └── Single view with: List, Add/Edit Form, Status Management
├── Locations (UnifiedLocationManagementView)
│   └── Single view with: Grid, Add/Edit Form, Activation Controls
├── Item Types (UnifiedItemTypeManagementView)
│   └── Single view with: List, Add/Edit Form, Category Management
├── System Configuration (SystemConfigViewModel)
└── About Information (AboutViewModel)
```

## 🎨 Unified Data Management View Design

### **User-Friendly Interface Pattern**
Each data management panel follows a consistent, intuitive pattern:

```
┌─────────────────────────────────────────────────────────────────────────┐
│ 📋 [Category Name] Management                                    [?Help] │
├─────────────────────────────────────────────────────────────────────────┤
│ 🔍 Search/Filter: [________________] 🎛️ [Filters▼] 📊 [Export] [Import] │
├─────────────────────────────────────────────────────────────────────────┤
│ ┌─ Data Grid/List (70% width) ─┐ │ ┌─ Action Panel (30% width) ─┐      │
│ │ ☑ Name           Status  Act │ │ │ 📝 Add/Edit Mode:          │      │
│ │ ☐ User1         Active   ⚙️📝❌ │ │ │                            │      │
│ │ ☐ User2         Inactive ⚙️📝❌ │ │ │ Name: [_____________]      │      │
│ │ ☐ User3         Active   ⚙️📝❌ │ │ │ Email: [____________]      │      │
│ │ ☐ User4         Active   ⚙️📝❌ │ │ │ Role: [Admin     ▼]       │      │
│ │ [More rows...]              │ │ │ Status: 🔘Active 🔘Inactive │      │
│ │                             │ │ │                            │      │
│ │ Pages: ◀ 1 2 3 4 5 ▶       │ │ │ [💾 Save] [↩️ Cancel]       │      │
│ └─────────────────────────────┘ │ │                            │      │
│                                  │ │ 🎯 Quick Actions:           │      │
│ 📦 Bulk Actions (when selected): │ │ ├ 📋 Duplicate Selected     │      │
│ [📝 Edit Selected] [❌ Delete]    │ │ ├ 📤 Export Selected       │      │
│ [✅ Activate] [⏸️ Deactivate]    │ │ ├ 📊 Generate Report       │      │
│                                  │ │ └ 🔄 Refresh Data          │      │
└─────────────────────────────────────────────────────────────────────────┘
```

### **Modern UX Features**
- **📱 Mobile-First Responsive**: Adapts to different screen sizes
- **⚡ Real-time Search**: Instant filtering as you type
- **🔄 Live Updates**: Real-time data refresh without page reload
- **♿ Accessibility**: Full keyboard navigation and screen reader support
- **🎨 Contextual Actions**: Right-click context menus for quick actions
- **📋 Batch Operations**: Multi-select for bulk operations
- **💾 Auto-save**: Automatic saving with visual confirmation
- **↩️ Undo/Redo**: Easy mistake correction

## 🔧 Technical Implementation Requirements

### **1. Unified View Architecture**
```csharp
// UnifiedDataManagementView.axaml - Base template for all data management
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Settings"
             x:Class="MTM_WIP_Application_Avalonia.Views.Settings.UnifiedDataManagementView"
             x:CompileBindings="True"
             x:DataType="vm:UnifiedDataManagementViewModel">

    <Grid RowDefinitions="Auto,Auto,*,Auto">
        <!-- Header with Title and Help -->
        <Border Grid.Row="0" Classes="header-panel">
            <Grid ColumnDefinitions="*,Auto">
                <TextBlock Grid.Column="0" Text="{Binding CategoryTitle}" Classes="section-title"/>
                <Button Grid.Column="1" Content="?" Classes="help-button" 
                        Command="{Binding ShowHelpCommand}"/>
            </Grid>
        </Border>

        <!-- Search and Filter Bar -->
        <Border Grid.Row="1" Classes="search-panel">
            <Grid ColumnDefinitions="Auto,*,Auto,Auto,Auto">
                <materialIcons:MaterialIcon Grid.Column="0" Kind="Search" Classes="search-icon"/>
                <TextBox Grid.Column="1" Text="{Binding SearchText}" 
                         Watermark="Search..." Classes="search-box"/>
                <Button Grid.Column="2" Content="Filters" Classes="filter-button"
                        Command="{Binding ShowFiltersCommand}"/>
                <Button Grid.Column="3" Content="Export" Classes="action-button"
                        Command="{Binding ExportCommand}"/>
                <Button Grid.Column="4" Content="Import" Classes="action-button"
                        Command="{Binding ImportCommand}"/>
            </Grid>
        </Border>

        <!-- Main Content Area -->
        <Grid Grid.Row="2" ColumnDefinitions="2*,*">
            <!-- Data Grid/List Panel -->
            <Border Grid.Column="0" Classes="data-panel">
                <Grid RowDefinitions="*,Auto">
                    <DataGrid Grid.Row="0" 
                             ItemsSource="{Binding FilteredItems}"
                             SelectedItems="{Binding SelectedItems}"
                             Classes="modern-datagrid">
                        <!-- Dynamic columns based on data type -->
                    </DataGrid>
                    
                    <!-- Pagination -->
                    <StackPanel Grid.Row="1" Orientation="Horizontal" 
                               HorizontalAlignment="Center" Classes="pagination">
                        <Button Content="◀" Command="{Binding PreviousPageCommand}"/>
                        <ItemsControl ItemsSource="{Binding PageNumbers}"/>
                        <Button Content="▶" Command="{Binding NextPageCommand}"/>
                    </StackPanel>
                </Grid>
            </Border>

            <!-- Action Panel -->
            <Border Grid.Column="1" Classes="action-panel">
                <Grid RowDefinitions="Auto,*,Auto">
                    <!-- Add/Edit Form -->
                    <Border Grid.Row="0" Classes="form-panel">
                        <StackPanel>
                            <TextBlock Text="{Binding FormTitle}" Classes="form-title"/>
                            <ContentControl Content="{Binding CurrentForm}"
                                          ContentTemplate="{Binding FormTemplate}"/>
                            <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                                <Button Content="💾 Save" Command="{Binding SaveCommand}" 
                                       Classes="primary-button"/>
                                <Button Content="↩️ Cancel" Command="{Binding CancelCommand}"
                                       Classes="secondary-button"/>
                            </StackPanel>
                        </StackPanel>
                    </Border>

                    <!-- Quick Actions -->
                    <Border Grid.Row="2" Classes="quick-actions-panel">
                        <StackPanel>
                            <TextBlock Text="🎯 Quick Actions" Classes="section-subtitle"/>
                            <Button Content="📋 Duplicate Selected" Command="{Binding DuplicateCommand}"/>
                            <Button Content="📤 Export Selected" Command="{Binding ExportSelectedCommand}"/>
                            <Button Content="📊 Generate Report" Command="{Binding GenerateReportCommand}"/>
                            <Button Content="🔄 Refresh Data" Command="{Binding RefreshCommand}"/>
                        </StackPanel>
                    </Border>
                </Grid>
            </Border>
        </Grid>

        <!-- Bulk Actions Bar (appears when items selected) -->
        <Border Grid.Row="3" Classes="bulk-actions-panel" 
                IsVisible="{Binding HasSelectedItems}">
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
                <TextBlock Text="{Binding SelectedItemsText}" Classes="selected-count"/>
                <Button Content="📝 Edit Selected" Command="{Binding EditSelectedCommand}"/>
                <Button Content="❌ Delete Selected" Command="{Binding DeleteSelectedCommand}"/>
                <Button Content="✅ Activate" Command="{Binding ActivateSelectedCommand}"/>
                <Button Content="⏸️ Deactivate" Command="{Binding DeactivateSelectedCommand}"/>
            </StackPanel>
        </Border>
    </Grid>
</UserControl>
```

### **2. Unified ViewModel Pattern**
```csharp
// UnifiedDataManagementViewModel.cs - Base class for all data management
public abstract class UnifiedDataManagementViewModel<T> : BaseViewModel, INotifyPropertyChanged
    where T : class, new()
{
    protected readonly IDataService<T> _dataService;
    protected readonly IThemeService _themeService;
    protected readonly INavigationService _navigationService;

    // Data Management
    public ObservableCollection<T> AllItems { get; } = new();
    public ObservableCollection<T> FilteredItems { get; } = new();
    public ObservableCollection<T> SelectedItems { get; } = new();
    
    // Form Management
    private T? _currentItem;
    public T? CurrentItem
    {
        get => _currentItem;
        set => SetProperty(ref _currentItem, value);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    // Search and Filter
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFiltersAsync().ConfigureAwait(false);
            }
        }
    }

    // UI State
    public abstract string CategoryTitle { get; }
    public string FormTitle => IsEditMode ? $"Edit {CategoryTitle}" : $"Add {CategoryTitle}";
    public bool HasSelectedItems => SelectedItems.Count > 0;
    public string SelectedItemsText => $"{SelectedItems.Count} item(s) selected";

    // Commands
    public ICommand AddNewCommand { get; private set; }
    public ICommand EditCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand RefreshCommand { get; private set; }
    public ICommand DuplicateCommand { get; private set; }
    public ICommand ExportCommand { get; private set; }
    public ICommand ImportCommand { get; private set; }

    protected UnifiedDataManagementViewModel(
        IDataService<T> dataService,
        IThemeService themeService,
        INavigationService navigationService,
        ILogger logger) : base(logger)
    {
        _dataService = dataService;
        _themeService = themeService;
        _navigationService = navigationService;

        InitializeCommands();
        LoadDataAsync().ConfigureAwait(false);
    }

    private void InitializeCommands()
    {
        AddNewCommand = new AsyncCommand(ExecuteAddNewAsync);
        EditCommand = new AsyncCommand<T>(ExecuteEditAsync);
        DeleteCommand = new AsyncCommand<T>(ExecuteDeleteAsync);
        SaveCommand = new AsyncCommand(ExecuteSaveAsync);
        CancelCommand = new RelayCommand(ExecuteCancel);
        RefreshCommand = new AsyncCommand(ExecuteRefreshAsync);
        DuplicateCommand = new AsyncCommand<T>(ExecuteDuplicateAsync);
        ExportCommand = new AsyncCommand(ExecuteExportAsync);
        ImportCommand = new AsyncCommand(ExecuteImportAsync);
    }

    // Real-time filtering with performance optimization
    private async Task ApplyFiltersAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? AllItems.ToList()
                    : AllItems.Where(item => MatchesSearchCriteria(item, SearchText)).ToList();

                Application.Current?.Dispatcher.Invoke(() =>
                {
                    FilteredItems.Clear();
                    foreach (var item in filtered)
                    {
                        FilteredItems.Add(item);
                    }
                });
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying filters");
        }
    }

    protected abstract bool MatchesSearchCriteria(T item, string searchText);
    
    // Abstract methods for concrete implementations
    protected abstract Task<Result> SaveItemAsync(T item);
    protected abstract Task<Result> DeleteItemAsync(T item);
    protected abstract T CreateNewItem();
    protected abstract T DuplicateItem(T source);
}

// UserManagementViewModel.cs - Specific implementation
public class UserManagementViewModel : UnifiedDataManagementViewModel<UserInfo>
{
    public override string CategoryTitle => "User Management";

    public UserManagementViewModel(
        IUserDataService userDataService,
        IThemeService themeService,
        INavigationService navigationService,
        ILogger<UserManagementViewModel> logger)
        : base(userDataService, themeService, navigationService, logger)
    {
    }

    protected override bool MatchesSearchCriteria(UserInfo user, string searchText)
    {
        var search = searchText.ToLowerInvariant();
        return user.Username.ToLowerInvariant().Contains(search) ||
               user.FullName.ToLowerInvariant().Contains(search) ||
               user.Email.ToLowerInvariant().Contains(search) ||
               user.Role.ToLowerInvariant().Contains(search);
    }

    protected override async Task<Result> SaveItemAsync(UserInfo user)
    {
        return IsEditMode 
            ? await _dataService.UpdateAsync(user)
            : await _dataService.CreateAsync(user);
    }

    protected override async Task<Result> DeleteItemAsync(UserInfo user)
    {
        return await _dataService.DeleteAsync(user.Id);
    }

    protected override UserInfo CreateNewItem()
    {
        return new UserInfo
        {
            IsActive = true,
            CreatedDate = DateTime.Now,
            Role = "User" // Default role
        };
    }

    protected override UserInfo DuplicateItem(UserInfo source)
    {
        return new UserInfo
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            Role = source.Role,
            IsActive = true,
            CreatedDate = DateTime.Now,
            Username = $"{source.Username}_copy",
            Email = string.Empty // User must set new email
        };
    }
}
```

### **3. Theme Integration with Live Preview**
```csharp
// ThemeBuilderViewModel.cs - Advanced theme customization
public class ThemeBuilderViewModel : BaseViewModel
{
    private readonly IThemeService _themeService;
    private readonly IThemeStateManager _stateManager;
    
    // Real-time preview properties (Q3: D)
    public ITheme PreviewTheme { get; set; }
    public bool IsPreviewMode { get; set; }
    
    // Card-based selection (Q11: C)
    public ObservableCollection<ThemeCardViewModel> AvailableThemes { get; }
    
    // Live color picker
    private Color _primaryColor = Color.Parse("#6a0dad");
    public Color PrimaryColor
    {
        get => _primaryColor;
        set
        {
            if (SetProperty(ref _primaryColor, value))
            {
                ApplyLivePreviewAsync().ConfigureAwait(false);
            }
        }
    }

    private Color _accentColor = Color.Parse("#ba45ed");
    public Color AccentColor
    {
        get => _accentColor;
        set
        {
            if (SetProperty(ref _accentColor, value))
            {
                ApplyLivePreviewAsync().ConfigureAwait(false);
            }
        }
    }

    // Accessibility options integrated (Q12: A)
    private bool _highContrastMode;
    public bool HighContrastMode
    {
        get => _highContrastMode;
        set
        {
            if (SetProperty(ref _highContrastMode, value))
            {
                ApplyAccessibilityThemeAsync().ConfigureAwait(false);
            }
        }
    }

    private double _fontScale = 1.0;
    public double FontScale
    {
        get => _fontScale;
        set
        {
            if (SetProperty(ref _fontScale, Math.Max(0.8, Math.Min(2.0, value))))
            {
                ApplyLivePreviewAsync().ConfigureAwait(false);
            }
        }
    }

    // Commands
    public ICommand ApplyThemeCommand { get; private set; }
    public ICommand ResetToDefaultCommand { get; private set; }
    public ICommand SaveCustomThemeCommand { get; private set; }
    public ICommand ExportThemeCommand { get; private set; }
    public ICommand ImportThemeCommand { get; private set; }

    private async Task ApplyLivePreviewAsync()
    {
        try
        {
            var customTheme = CreateCustomTheme();
            await _themeService.ApplyThemeAsync(customTheme, isPreview: true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying live theme preview");
        }
    }

    private ITheme CreateCustomTheme()
    {
        return new CustomTheme
        {
            PrimaryColor = PrimaryColor,
            AccentColor = AccentColor,
            HighContrastMode = HighContrastMode,
            FontScale = FontScale,
            Name = "Custom Theme",
            IsUserCreated = true
        };
    }
}
```

### **4. Performance Optimization (Q13: B, Q14: B, Q15: C)**
```csharp
// VirtualPanelManager.cs - Independent virtual panels with coordination
public class VirtualPanelManager : IVirtualPanelManager
{
    private readonly IThemeService _themeService;
    private readonly Dictionary<string, WeakReference<SettingsPanelViewModel>> _panelCache = new();
    private readonly SemaphoreSlim _loadingSemaphore = new(3); // Limit concurrent loads
    
    // Lazy loading with intelligent pre-caching (Q14: B)
    public async Task<SettingsPanelViewModel> GetPanelAsync(string panelId)
    {
        if (_panelCache.TryGetValue(panelId, out var weakRef) && 
            weakRef.TryGetTarget(out var cachedPanel))
        {
            return cachedPanel;
        }
        
        await _loadingSemaphore.WaitAsync();
        try
        {
            var panel = await CreatePanelAsync(panelId);
            _panelCache[panelId] = new WeakReference<SettingsPanelViewModel>(panel);
            
            // Pre-cache adjacent panels
            _ = Task.Run(() => PreCacheAdjacentPanelsAsync(panelId));
            
            return panel;
        }
        finally
        {
            _loadingSemaphore.Release();
        }
    }
    
    // Progressive enhancement transitions (Q15: C)
    public void ApplyThemeTransition(ITheme newTheme, PerformanceLevel systemPerformance)
    {
        switch (systemPerformance)
        {
            case PerformanceLevel.High:
                ApplyInstantTransition(newTheme);
                break;
            case PerformanceLevel.Medium:
                ApplyStagedTransition(newTheme);
                break;
            case PerformanceLevel.Low:
                ApplyMinimalTransition(newTheme);
                break;
        }
    }

    private async Task PreCacheAdjacentPanelsAsync(string currentPanelId)
    {
        try
        {
            var adjacentPanels = GetAdjacentPanelIds(currentPanelId);
            foreach (var panelId in adjacentPanels)
            {
                if (!_panelCache.ContainsKey(panelId))
                {
                    await GetPanelAsync(panelId);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error pre-caching adjacent panels");
        }
    }
}
```

## 🔗 Integration Points

### **Service Layer Integration**
- [ ] **ErrorHandling.cs**: Separate error contexts with shared service (Q21: B)
- [ ] **Configuration.cs**: Hierarchical theme configuration integration
- [ ] **Navigation.cs**: Theme-aware navigation with state preservation
- [ ] **Database.cs**: Theme preferences table and settings persistence

### **Theme Quick-Switcher Integration**
- [ ] **MainView Toolbar**: Quick theme switcher component
- [ ] **Real-time Application**: Instant theme feedback throughout app
- [ ] **State Coordination**: Synchronization between quick-switcher and settings

### **Testing Strategy (Q24: C)**
- [ ] **Unit Tests**: Service layer and ViewModel logic
- [ ] **Integration Tests**: Theme-settings coordination and database operations
- [ ] **End-to-End Tests**: Complete user workflows and state management

## 📋 Implementation Roadmap

### **Phase 1: MVP - Unified Views & Basic Integration (Q22: D, Q23: A)**
1. **Core Services Setup**
   - Create ThemeService integrated with Configuration service
   - Implement basic SettingsService
   - Setup service registration with `AddMTMThemeAndSettingsServices()`

2. **Unified UI Framework**
   - Create UnifiedDataManagementView base template
   - Implement UserManagementViewModel as first concrete implementation
   - Add theme quick-switcher to main toolbar

3. **Live Theme Switching**
   - Real-time theme application without restart
   - Theme changes preserved during navigation
   - Basic theme selection with card-based interface

### **Phase 2: Advanced Features**
1. **Complete Unified Views**
   - Implement all 5 unified data management views
   - Advanced Theme Builder as settings panel
   - Integrated accessibility options

2. **Virtual Panel System**
   - Performance-optimized panel loading
   - Lazy loading with intelligent pre-caching
   - Progressive enhancement for transitions

3. **Enhanced UX Features**
   - Real-time search and filtering
   - Bulk operations and batch processing
   - Auto-save with visual confirmation

### **Phase 3: Data Persistence & Export**
1. **Database Integration**
   - Separate theme preferences table
   - Hierarchical configuration support
   - User preference storage and retrieval

2. **Import/Export System**
   - Theme export/import functionality
   - Settings backup and restore
   - Configuration file management

### **Phase 4: Polish & Extensibility**
1. **Advanced Features**
   - Undo/Redo functionality
   - Advanced filtering and sorting
   - Report generation

2. **Quality & Performance**
   - Complete testing suite implementation
   - Performance optimization
   - User experience refinements

## 🎯 Acceptance Criteria

### **✅ Core Functionality**
- [ ] Theme system fully integrated into settings form architecture
- [ ] Both settings panel and quick-switcher theme entry points working
- [ ] Real-time application theming with instant feedback
- [ ] All 9 unified settings panels functioning with TreeView navigation
- [ ] Advanced Theme Builder operational as settings panel

### **✅ Unified UX Requirements**
- [ ] Single views combine Add/Edit/Remove operations seamlessly
- [ ] Real-time search and filtering across all data management views
- [ ] Bulk operations working with multi-select functionality
- [ ] Auto-save with visual confirmation and error handling
- [ ] Responsive design adapts to different screen sizes

### **✅ Performance Requirements**
- [ ] Independent virtual panels with theme service coordination
- [ ] Lazy loading with intelligent pre-caching implemented
- [ ] Progressive enhancement for theme transitions
- [ ] Theme changes preserved during settings navigation
- [ ] Theme previews persist across all settings panels

### **✅ Data Management**
- [ ] Separate theme preferences table linked to user settings
- [ ] Hierarchical configuration: System > Company > User > Custom
- [ ] Separate theme export/import functionality
- [ ] Smart upgrade system with user preference preservation

### **✅ Integration Standards**
- [ ] Follows MTM service organization patterns
- [ ] Uses standard .NET patterns with INotifyPropertyChanged
- [ ] Integrates with existing ErrorHandling service
- [ ] Proper Avalonia AXAML without AVLN2000 errors

### **✅ Quality Standards**
- [ ] Layered testing: unit, integration, and end-to-end scenarios
- [ ] Configuration-driven theme and panel systems
- [ ] Separate ViewModels with shared coordination service
- [ ] Modular Views: SettingsFormView + unified data management views

## 🏷️ Implementation Hotkeys

**Primary Implementation Sequence:**
```
@sys:config integration → @sys:theme setup → @ui:unified create → @ui:theme builder → @qa:verify
```

**Supporting Hotkeys:**
- `@sys:service` - Create and register theme and settings services
- `@ui:unified` - Create unified data management views and components
- `@ui:viewmodel` - Create ViewModels with coordination patterns
- `@db:theme` - Implement theme database persistence
- `@perf:virtual` - Implement virtual panel system
- `@qa:test` - Create layered testing strategy

## 📝 Notes

### **Critical Requirements**
- **Service Integration**: Follow MTM SERVICE FILE ORGANIZATION RULE
- **No ReactiveUI**: Use standard .NET patterns throughout
- **Database Access**: Use stored procedures via Helper_Database_StoredProcedure
- **Error Handling**: Use ErrorHandling service for all error management
- **AXAML Compliance**: Prevent AVLN2000 errors with proper Avalonia syntax

### **UX Design Principles**
- **Unified Operations**: All CRUD operations in single, cohesive views
- **Real-time Feedback**: Instant search, live themes, auto-save confirmation
- **Progressive Disclosure**: Advanced features accessible but not overwhelming
- **Accessibility First**: Keyboard navigation, screen reader support, high contrast
- **Mobile-Responsive**: Adapts gracefully to different screen sizes

## 🎯 Priority
**High** - This provides the unified administrative interface and advanced theming system required for MTM WIP Application modernization with significantly improved user experience.

## 🏷️ Labels
- `enhancement`
- `ui`
- `mvvm`
- `theming-system`
- `settings-management`
- `service-integration`
- `mtm-patterns`
- `avalonia`
- `unified-ux`

## 👥 Assignee
@developer (assign to appropriate team member)

## 📅 Target Milestone
Next major release supporting integrated theming and comprehensive settings management with unified user experience

---

**Implementation Status**: Ready for development based on questionnaire analysis, MTM established patterns, and modern UX best practices.
**Questionnaire Reference**: [integrated-theming-settings-questionnaire.md](.github/questionnaires/integrated-theming-settings-questionnaire.md)
