using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;
using MTM_WIP_Application_Avalonia.Services.Infrastructure;
using MTM_WIP_Application_Avalonia.Services.UI;
using MTM_WIP_Application_Avalonia.Services.Feature;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// Main coordinator ViewModel for comprehensive SettingsForm.
/// Manages TreeView navigation, TabView panels, and state management.
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly ISettingsService _settingsService;
    private readonly VirtualPanelManager _panelManager;
    private readonly SettingsPanelStateManager _stateManager;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveChanges))]
    private SettingsCategoryViewModel? _selectedCategory;

    [ObservableProperty]
    private SettingsPanelViewModel? _selectedPanel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveChanges))]
    private bool _isLoading;

    [ObservableProperty]
    private string _currentStatusMessage = "Ready";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveChanges))]
    private bool _hasUnsavedChanges;

    /// <summary>
    /// Initializes a new instance of the SettingsViewModel class.
    /// </summary>
    /// <param name="navigationService">Service for navigation between views</param>
    /// <param name="themeService">Service for theme management</param>
    /// <param name="settingsService">Service for settings configuration</param>
    /// <param name="panelManager">Manager for virtual panel lifecycle</param>
    /// <param name="stateManager">Manager for panel state tracking</param>
    /// <param name="logger">Logger for this ViewModel</param>
    /// <exception cref="ArgumentNullException">Thrown when any service parameter is null</exception>
    public SettingsViewModel(
        INavigationService navigationService,
        IThemeService themeService,
        ISettingsService settingsService,
        VirtualPanelManager panelManager,
        SettingsPanelStateManager stateManager,
        ILogger<SettingsViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        _panelManager = panelManager ?? throw new ArgumentNullException(nameof(panelManager));
        _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));

        // Initialize collections
        Categories = new ObservableCollection<SettingsCategoryViewModel>();
        LoadedPanels = new ObservableCollection<SettingsPanelViewModel>();

        // Initialize categories and panels
        InitializeCategories();

        // Subscribe to events
        _stateManager.StateChanged += OnStateManagerStateChanged;

        Logger.LogInformation("SettingsViewModel initialized with {CategoryCount} categories", Categories.Count);
    }

    #region Properties

    /// <summary>
    /// TreeView navigation categories.
    /// </summary>
    public ObservableCollection<SettingsCategoryViewModel> Categories { get; }

    /// <summary>
    /// Currently loaded panels in TabView.
    /// </summary>
    public ObservableCollection<SettingsPanelViewModel> LoadedPanels { get; }

    /// <summary>
    /// Determines if changes can be saved.
    /// </summary>
    public bool CanSaveChanges => HasUnsavedChanges && !IsLoading;

    #endregion

    #region Command Implementations

    /// <summary>
    /// Saves all changes across all loaded panels.
    /// </summary>
    /// <summary>
    /// Saves all changes across all loaded panels.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteSaveAllChanges))]
    private async Task SaveAllChangesAsync()
    {
        try
        {
            IsLoading = true;
            CurrentStatusMessage = "Saving all changes...";

            var result = await _stateManager.SaveAllChangesAsync().ConfigureAwait(false);

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
            Logger.LogError(ex, "Error saving all settings changes");
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
        return HasUnsavedChanges && !IsLoading;
    }

    /// <summary>
    /// Reverts all unsaved changes across panels.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteRevertAllChanges))]
    private async Task RevertAllChangesAsync()
    {
        try
        {
            IsLoading = true;
            CurrentStatusMessage = "Reverting all changes...";

            var result = await _stateManager.RevertAllChangesAsync().ConfigureAwait(false);

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
            Logger.LogError(ex, "Error reverting all settings changes");
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
    [RelayCommand]
    private void Close()
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

    #region Property Change Handlers

    /// <summary>
    /// Handles changes to the SelectedCategory property and loads corresponding panel.
    /// Implements virtual panel loading with state management integration.
    /// </summary>
    /// <param name="value">The newly selected category</param>
    partial void OnSelectedCategoryChanged(SettingsCategoryViewModel? value)
    {
        OnSelectedCategoryChanged();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes the TreeView category structure with all available settings panels.
    /// Creates hierarchical navigation structure for user management, part numbers,
    /// operations, locations, item types, and system configuration.
    /// </summary>
    private void InitializeCategories()
    {
        Categories.Clear();

        // Database Settings
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "database",
            DisplayName = "Database Settings",
            Icon = "ðŸ—„ï¸",
            PanelType = typeof(DatabaseSettingsViewModel)
        });

        // User Management
        var userManagement = new SettingsCategoryViewModel
        {
            Id = "user-management",
            DisplayName = "User Management",
            Icon = "ðŸ‘¥",
            HasSubCategories = true
        };

        userManagement.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-user",
            DisplayName = "Add User",
            Icon = "âž•",
            PanelType = typeof(AddUserViewModel),
            Parent = userManagement
        });

        userManagement.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-user",
            DisplayName = "Edit User",
            Icon = "âœï¸",
            PanelType = typeof(EditUserViewModel),
            Parent = userManagement
        });

        userManagement.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "delete-user",
            DisplayName = "Delete User",
            Icon = "ðŸ—‘ï¸",
            PanelType = typeof(RemoveUserViewModel),
            Parent = userManagement
        });

        Categories.Add(userManagement);

        // Part Numbers
        var partNumbers = new SettingsCategoryViewModel
        {
            Id = "part-numbers",
            DisplayName = "Part Numbers",
            Icon = "ðŸ”§",
            HasSubCategories = true
        };

        partNumbers.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-part",
            DisplayName = "Add Part Number",
            Icon = "âž•",
            PanelType = typeof(AddPartViewModel),
            Parent = partNumbers
        });

        partNumbers.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-part",
            DisplayName = "Edit Part Number",
            Icon = "âœï¸",
            PanelType = typeof(EditPartViewModel),
            Parent = partNumbers
        });

        partNumbers.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "remove-part",
            DisplayName = "Remove Part Number",
            Icon = "ðŸ—‘ï¸",
            PanelType = typeof(RemovePartViewModel),
            Parent = partNumbers
        });

        Categories.Add(partNumbers);

        // Operations
        var operations = new SettingsCategoryViewModel
        {
            Id = "operations",
            DisplayName = "Operations",
            Icon = "âš™ï¸",
            HasSubCategories = true
        };

        operations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-operation",
            DisplayName = "Add Operation",
            Icon = "âž•",
            PanelType = typeof(AddOperationViewModel),
            Parent = operations
        });

        operations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-operation",
            DisplayName = "Edit Operation",
            Icon = "âœï¸",
            PanelType = typeof(EditOperationViewModel),
            Parent = operations
        });

        operations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "remove-operation",
            DisplayName = "Remove Operation",
            Icon = "ðŸ—‘ï¸",
            PanelType = typeof(RemoveOperationViewModel),
            Parent = operations
        });

        Categories.Add(operations);

        // Locations
        var locations = new SettingsCategoryViewModel
        {
            Id = "locations",
            DisplayName = "Locations",
            Icon = "ðŸ“",
            HasSubCategories = true
        };

        locations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-location",
            DisplayName = "Add Location",
            Icon = "âž•",
            PanelType = typeof(AddLocationViewModel),
            Parent = locations
        });

        locations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-location",
            DisplayName = "Edit Location",
            Icon = "âœï¸",
            PanelType = typeof(EditLocationViewModel),
            Parent = locations
        });

        locations.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "remove-location",
            DisplayName = "Remove Location",
            Icon = "ðŸ—‘ï¸",
            PanelType = typeof(RemoveLocationViewModel),
            Parent = locations
        });

        Categories.Add(locations);

        // ItemTypes
        var itemTypes = new SettingsCategoryViewModel
        {
            Id = "item-types",
            DisplayName = "Item Types",
            Icon = "ðŸ“¦",
            HasSubCategories = true
        };

        itemTypes.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-itemtype",
            DisplayName = "Add Item Type",
            Icon = "âž•",
            PanelType = typeof(AddItemTypeViewModel),
            Parent = itemTypes
        });

        itemTypes.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "edit-itemtype",
            DisplayName = "Edit Item Type",
            Icon = "âœï¸",
            PanelType = typeof(EditItemTypeViewModel),
            Parent = itemTypes
        });

        itemTypes.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "remove-itemtype",
            DisplayName = "Remove Item Type",
            Icon = "ðŸ—‘ï¸",
            PanelType = typeof(RemoveItemTypeViewModel),
            Parent = itemTypes
        });

        Categories.Add(itemTypes);

        // Shortcuts Configuration
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "shortcuts",
            DisplayName = "Shortcuts Configuration",
            Icon = "âŒ¨ï¸",
            PanelType = typeof(ShortcutsViewModel)
        });

        // About Information
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "about",
            DisplayName = "About Information",
            Icon = "â„¹ï¸",
            PanelType = typeof(AboutViewModel)
        });

        // System Health & Diagnostics
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "system-health",
            DisplayName = "System Health & Diagnostics",
            Icon = "ðŸ©º",
            PanelType = typeof(SystemHealthViewModel)
        });

        // Backup & Recovery
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "backup-recovery",
            DisplayName = "Backup & Recovery",
            Icon = "ðŸ’¾",
            PanelType = typeof(BackupRecoveryViewModel)
        });

        // Security & Permissions
        Categories.Add(new SettingsCategoryViewModel
        {
            Id = "security-permissions",
            DisplayName = "Security & Permissions",
            Icon = "ðŸ”’",
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
                var newPanel = await _panelManager.CreateVirtualPanelAsync(SelectedCategory).ConfigureAwait(false);

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
    /// Handles state manager state changed events and updates UI accordingly.
    /// Updates unsaved changes status and command availability.
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Panel state change event arguments</param>
    private void OnStateManagerStateChanged(object? sender, PanelStateChangedEventArgs e)
    {
        HasUnsavedChanges = _stateManager.HasAnyUnsavedChanges;
        OnPropertyChanged(nameof(CanSaveChanges));
    }

    #endregion

    #region Disposal

    /// <summary>
    /// Dispose resources and unsubscribe from events.
    /// Properly disposes all loaded panels and clears collections.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
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
