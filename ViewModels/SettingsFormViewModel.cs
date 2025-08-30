using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// Main coordinator ViewModel for comprehensive SettingsForm.
/// Manages TreeView navigation, TabView panels, and state management.
/// </summary>
public class SettingsFormViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly ISettingsService _settingsService;
    private readonly VirtualPanelManager _panelManager;
    private readonly SettingsPanelStateManager _stateManager;
    
    private SettingsCategoryViewModel? _selectedCategory;
    private SettingsPanelViewModel? _selectedPanel;
    private bool _isLoading;
    private string _currentStatusMessage = "Ready";
    private bool _hasUnsavedChanges;

    public SettingsFormViewModel(
        INavigationService navigationService,
        IThemeService themeService,
        ISettingsService settingsService,
        VirtualPanelManager panelManager,
        SettingsPanelStateManager stateManager,
        ILogger<SettingsFormViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        _panelManager = panelManager ?? throw new ArgumentNullException(nameof(panelManager));
        _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));

        // Initialize collections
        Categories = new ObservableCollection<SettingsCategoryViewModel>();
        LoadedPanels = new ObservableCollection<SettingsPanelViewModel>();

        // Initialize commands
        SaveAllChangesCommand = new AsyncCommand(ExecuteSaveAllChangesAsync, CanExecuteSaveAllChanges);
        RevertAllChangesCommand = new AsyncCommand(ExecuteRevertAllChangesAsync, CanExecuteRevertAllChanges);
        CloseCommand = new AsyncCommand(ExecuteCloseAsync);

        // Initialize categories and panels
        InitializeCategories();
        
        // Subscribe to events
        _stateManager.StateChanged += OnStateManagerStateChanged;

        Logger.LogInformation("SettingsFormViewModel initialized with {CategoryCount} categories", Categories.Count);
    }

    #region Properties

    /// <summary>
    /// TreeView navigation categories.
    /// </summary>
    public ObservableCollection<SettingsCategoryViewModel> Categories { get; }

    /// <summary>
    /// Currently selected category in TreeView.
    /// </summary>
    public SettingsCategoryViewModel? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                OnSelectedCategoryChanged();
            }
        }
    }

    /// <summary>
    /// Currently selected panel for display.
    /// </summary>
    public SettingsPanelViewModel? SelectedPanel
    {
        get => _selectedPanel;
        set => SetProperty(ref _selectedPanel, value);
    }

    /// <summary>
    /// Currently loaded panels in TabView.
    /// </summary>
    public ObservableCollection<SettingsPanelViewModel> LoadedPanels { get; }

    /// <summary>
    /// Indicates if operations are in progress.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Current status message for user feedback.
    /// </summary>
    public string CurrentStatusMessage
    {
        get => _currentStatusMessage;
        set => SetProperty(ref _currentStatusMessage, value);
    }

    /// <summary>
    /// Indicates if there are unsaved changes across panels.
    /// </summary>
    public bool HasUnsavedChanges
    {
        get => _hasUnsavedChanges;
        set => SetProperty(ref _hasUnsavedChanges, value);
    }

    /// <summary>
    /// Determines if changes can be saved.
    /// </summary>
    public bool CanSaveChanges => HasUnsavedChanges && !IsLoading;

    #endregion

    #region Commands

    /// <summary>
    /// Command to save all changes across panels.
    /// </summary>
    public ICommand SaveAllChangesCommand { get; }

    /// <summary>
    /// Command to revert all unsaved changes.
    /// </summary>
    public ICommand RevertAllChangesCommand { get; }

    /// <summary>
    /// Command to close the settings form.
    /// </summary>
    public ICommand CloseCommand { get; }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Saves all changes across all loaded panels.
    /// </summary>
    private async Task ExecuteSaveAllChangesAsync()
    {
        try
        {
            IsLoading = true;
            CurrentStatusMessage = "Saving all changes...";

            var result = await _stateManager.SaveAllChangesAsync();
            
            if (result.IsSuccess)
            {
                CurrentStatusMessage = "All changes saved successfully";
                HasUnsavedChanges = false;
                Logger.LogInformation("All settings changes saved successfully");
            }
            else
            {
                CurrentStatusMessage = $"Failed to save changes: {result.Message}";
                Logger.LogWarning("Failed to save all changes: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            CurrentStatusMessage = $"Error saving changes: {ex.Message}";
            Logger.LogError(ex, "Error saving all changes");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Determines if save all changes can be executed.
    /// </summary>
    private bool CanExecuteSaveAllChanges()
    {
        return CanSaveChanges;
    }

    /// <summary>
    /// Reverts all unsaved changes across panels.
    /// </summary>
    private async Task ExecuteRevertAllChangesAsync()
    {
        try
        {
            IsLoading = true;
            CurrentStatusMessage = "Reverting all changes...";

            var result = await _stateManager.RevertAllChangesAsync();
            
            if (result.IsSuccess)
            {
                CurrentStatusMessage = "All changes reverted";
                HasUnsavedChanges = false;
                Logger.LogInformation("All settings changes reverted successfully");
            }
            else
            {
                CurrentStatusMessage = $"Failed to revert changes: {result.Message}";
                Logger.LogWarning("Failed to revert all changes: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            CurrentStatusMessage = $"Error reverting changes: {ex.Message}";
            Logger.LogError(ex, "Error reverting all changes");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Determines if revert all changes can be executed.
    /// </summary>
    private bool CanExecuteRevertAllChanges()
    {
        return HasUnsavedChanges && !IsLoading;
    }

    /// <summary>
    /// Closes the settings form with unsaved changes check.
    /// </summary>
    private async Task ExecuteCloseAsync()
    {
        if (HasUnsavedChanges)
        {
            CurrentStatusMessage = "Warning: You have unsaved changes. Use Save All or Revert before closing.";
            return;
        }

        try
        {
            // Navigate back or close via navigation service
            _navigationService.GoBack();
            Logger.LogInformation("Settings form closed");
        }
        catch (Exception ex)
        {
            CurrentStatusMessage = $"Error closing form: {ex.Message}";
            Logger.LogError(ex, "Error closing settings form");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes the TreeView category structure.
    /// </summary>
    private void InitializeCategories()
    {
        Categories.Clear();

        // Database Settings
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "database",
            DisplayName = "Database Settings",
            Icon = "🗄️",
            PanelType = typeof(DatabaseSettingsViewModel)
        });

        // User Management
        var userManagement = new SettingsCategoryViewModel
        {
            Id = "user-management",
            DisplayName = "User Management",
            Icon = "👥",
            HasSubCategories = true
        };
        
        userManagement.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-user",
            DisplayName = "Add User",
            Icon = "➕",
            PanelType = typeof(AddUserViewModel),
            Parent = userManagement
        });
        
        userManagement.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-user",
            DisplayName = "Edit User",
            Icon = "✏️",
            PanelType = typeof(EditUserViewModel),
            Parent = userManagement
        });
        
        userManagement.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "delete-user",
            DisplayName = "Delete User",
            Icon = "🗑️",
            PanelType = typeof(DeleteUserViewModel),
            Parent = userManagement
        });
        
        Categories.Add(userManagement);

        // Part Numbers
        var partNumbers = new SettingsCategoryViewModel
        {
            Id = "part-numbers",
            DisplayName = "Part Numbers",
            Icon = "🔧",
            HasSubCategories = true
        };
        
        partNumbers.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-part",
            DisplayName = "Add Part Number",
            Icon = "➕",
            PanelType = typeof(AddPartViewModel),
            Parent = partNumbers
        });
        
        partNumbers.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-part",
            DisplayName = "Edit Part Number",
            Icon = "✏️",
            PanelType = typeof(EditPartViewModel),
            Parent = partNumbers
        });
        
        partNumbers.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "remove-part",
            DisplayName = "Remove Part Number",
            Icon = "🗑️",
            PanelType = typeof(RemovePartViewModel),
            Parent = partNumbers
        });
        
        Categories.Add(partNumbers);

        // Operations
        var operations = new SettingsCategoryViewModel
        {
            Id = "operations",
            DisplayName = "Operations",
            Icon = "⚙️",
            HasSubCategories = true
        };
        
        operations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-operation",
            DisplayName = "Add Operation",
            Icon = "➕",
            PanelType = typeof(AddOperationViewModel),
            Parent = operations
        });
        
        operations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-operation",
            DisplayName = "Edit Operation",
            Icon = "✏️",
            PanelType = typeof(EditOperationViewModel),
            Parent = operations
        });
        
        operations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "remove-operation",
            DisplayName = "Remove Operation",
            Icon = "🗑️",
            PanelType = typeof(RemoveOperationViewModel),
            Parent = operations
        });
        
        Categories.Add(operations);

        // Locations
        var locations = new SettingsCategoryViewModel
        {
            Id = "locations",
            DisplayName = "Locations",
            Icon = "📍",
            HasSubCategories = true
        };
        
        locations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-location",
            DisplayName = "Add Location",
            Icon = "➕",
            PanelType = typeof(AddLocationViewModel),
            Parent = locations
        });
        
        locations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-location",
            DisplayName = "Edit Location",
            Icon = "✏️",
            PanelType = typeof(EditLocationViewModel),
            Parent = locations
        });
        
        locations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "remove-location",
            DisplayName = "Remove Location",
            Icon = "🗑️",
            PanelType = typeof(RemoveLocationViewModel),
            Parent = locations
        });
        
        Categories.Add(locations);

        // ItemTypes
        var itemTypes = new SettingsCategoryViewModel
        {
            Id = "item-types",
            DisplayName = "Item Types",
            Icon = "📦",
            HasSubCategories = true
        };
        
        itemTypes.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-itemtype",
            DisplayName = "Add Item Type",
            Icon = "➕",
            PanelType = typeof(AddItemTypeViewModel),
            Parent = itemTypes
        });
        
        itemTypes.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-itemtype",
            DisplayName = "Edit Item Type",
            Icon = "✏️",
            PanelType = typeof(EditItemTypeViewModel),
            Parent = itemTypes
        });
        
        itemTypes.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "remove-itemtype",
            DisplayName = "Remove Item Type",
            Icon = "🗑️",
            PanelType = typeof(RemoveItemTypeViewModel),
            Parent = itemTypes
        });
        
        Categories.Add(itemTypes);

        // Advanced Theme Builder
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "theme-builder",
            DisplayName = "Advanced Theme Builder",
            Icon = "🎨",
            PanelType = typeof(ThemeBuilderViewModel)
        });

        // Shortcuts Configuration
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "shortcuts",
            DisplayName = "Shortcuts Configuration",
            Icon = "⌨️",
            PanelType = typeof(ShortcutsViewModel)
        });

        // About Information
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "about",
            DisplayName = "About Information",
            Icon = "ℹ️",
            PanelType = typeof(AboutViewModel)
        });

        // System Health & Diagnostics
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "system-health",
            DisplayName = "System Health & Diagnostics",
            Icon = "🩺",
            PanelType = typeof(SystemHealthViewModel)
        });

        // Backup & Recovery
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "backup-recovery",
            DisplayName = "Backup & Recovery",
            Icon = "💾",
            PanelType = typeof(BackupRecoveryViewModel)
        });

        // Security & Permissions
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "security-permissions",
            DisplayName = "Security & Permissions",
            Icon = "🔒",
            PanelType = typeof(SecurityPermissionsViewModel)
        });

        // Select first category by default
        if (Categories.Count > 0)
        {
            SelectedCategory = Categories.First();
        }
    }

    /// <summary>
    /// Handles selected category changes and loads corresponding panel.
    /// </summary>
    private async void OnSelectedCategoryChanged()
    {
        if (SelectedCategory?.PanelType == null) return;

        try
        {
            IsLoading = true;
            CurrentStatusMessage = $"Loading {SelectedCategory.DisplayName}...";

            // Check if panel is already loaded
            var existingPanel = LoadedPanels.FirstOrDefault(p => p.CategoryId == SelectedCategory.Id);
            
            if (existingPanel != null)
            {
                // Switch to existing panel
                SelectedPanel = existingPanel;
            }
            else
            {
                // Create new virtual panel
                var newPanel = await _panelManager.CreateVirtualPanelAsync(SelectedCategory);
                
                if (newPanel != null)
                {
                    LoadedPanels.Add(newPanel);
                    SelectedPanel = newPanel;
                    
                    // Create state snapshot for new panel
                    _stateManager.CreateSnapshot(SelectedCategory.Id, newPanel.ViewModel);
                }
            }

            CurrentStatusMessage = $"{SelectedCategory.DisplayName} loaded";
        }
        catch (Exception ex)
        {
            CurrentStatusMessage = $"Error loading panel: {ex.Message}";
            Logger.LogError(ex, "Error loading panel for category {Category}", SelectedCategory?.Id);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Handles state manager state changed events.
    /// </summary>
    private void OnStateManagerStateChanged(object? sender, PanelStateChangedEventArgs e)
    {
        HasUnsavedChanges = _stateManager.HasAnyUnsavedChanges;
        RaisePropertyChanged(nameof(CanSaveChanges));
    }

    #endregion

    #region Disposal

    /// <summary>
    /// Dispose resources and unsubscribe from events.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _stateManager.StateChanged -= OnStateManagerStateChanged;
            
            // Dispose loaded panels
            foreach (var panel in LoadedPanels)
            {
                panel.Dispose();
            }
            LoadedPanels.Clear();
        }
        
        base.Dispose(disposing);
    }

    #endregion
}