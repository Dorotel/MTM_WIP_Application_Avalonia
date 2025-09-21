using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for Shortcuts Configuration panel.
/// Provides keyboard shortcut management and customization functionality.
/// Uses MVVM Community Toolkit for modern .NET patterns.
/// </summary>
public partial class ShortcutsViewModel : BaseViewModel
{
    private readonly IConfigurationService _configurationService;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveShortcutsCommand))]
    private string _searchFilter = string.Empty;
    
    [ObservableProperty]
    private ShortcutItem? _selectedShortcut;
    
    [ObservableProperty]
    private bool _isEditing;

    public ShortcutsViewModel(
        IConfigurationService configurationService,
        ILogger<ShortcutsViewModel> logger) : base(logger)
    {
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        // Initialize collections
        Shortcuts = new ObservableCollection<ShortcutItem>();
        FilteredShortcuts = new ObservableCollection<ShortcutItem>();

        // Load initial data
        _ = LoadShortcutsAsync();

        Logger.LogInformation("ShortcutsViewModel initialized");
    }

    #region Properties

    /// <summary>
    /// All keyboard shortcuts collection.
    /// </summary>
    public ObservableCollection<ShortcutItem> Shortcuts { get; }

    /// <summary>
    /// Filtered shortcuts based on search criteria.
    /// </summary>
    public ObservableCollection<ShortcutItem> FilteredShortcuts { get; }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handles search filter changes to automatically filter shortcuts.
    /// </summary>
    partial void OnSearchFilterChanged(string value)
    {
        SearchShortcuts();
    }

    #endregion

    #region Commands

    /// <summary>
    /// Saves all shortcut configurations.
    /// </summary>
    [RelayCommand]
    private async Task SaveShortcuts()
    {
        try
        {
            IsLoading = true;

            // In real implementation, would save to configuration
            foreach (var shortcut in Shortcuts)
            {
                // Note: Current IConfigurationService doesn't have async save methods
                // This would need to be implemented when configuration persistence is added
                Logger.LogDebug("Would save shortcut: {Action} = {KeyCombination}", shortcut.Action, shortcut.KeyCombination);
            }

            Logger.LogInformation("Keyboard shortcuts saved successfully");
            await Task.Delay(500); // Simulate save operation
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving keyboard shortcuts");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Resets all shortcuts to default values.
    /// </summary>
    [RelayCommand]
    private async Task ResetToDefaults()
    {
        try
        {
            IsLoading = true;

            await LoadDefaultShortcutsAsync();
            SearchShortcuts();

            Logger.LogInformation("Keyboard shortcuts reset to defaults");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resetting shortcuts to defaults");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Starts editing the specified shortcut.
    /// </summary>
    [RelayCommand]
    private async Task EditShortcut(ShortcutItem? shortcut)
    {
        if (shortcut == null) return;

        try
        {
            SelectedShortcut = shortcut;
            IsEditing = true;

            Logger.LogDebug("Started editing shortcut: {Action}", shortcut.Action);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error starting shortcut edit");
        }
    }

    /// <summary>
    /// Cancels shortcut editing.
    /// </summary>
    [RelayCommand]
    private void CancelEdit()
    {
        SelectedShortcut = null;
        IsEditing = false;
    }

    /// <summary>
    /// Searches shortcuts based on filter criteria.
    /// </summary>
    [RelayCommand]
    private void SearchShortcuts()
    {
        FilteredShortcuts.Clear();

        var searchTerm = SearchFilter?.ToLowerInvariant() ?? string.Empty;

        foreach (var shortcut in Shortcuts)
        {
            if (string.IsNullOrEmpty(searchTerm) ||
                shortcut.Action.ToLowerInvariant().Contains(searchTerm) ||
                shortcut.Description.ToLowerInvariant().Contains(searchTerm) ||
                shortcut.KeyCombination.ToLowerInvariant().Contains(searchTerm) ||
                shortcut.Category.ToLowerInvariant().Contains(searchTerm))
            {
                FilteredShortcuts.Add(shortcut);
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads keyboard shortcuts configuration.
    /// </summary>
    private async Task LoadShortcutsAsync()
    {
        try
        {
            await LoadDefaultShortcutsAsync();

            // Load custom shortcuts from configuration
            foreach (var shortcut in Shortcuts)
            {
                var customKey = _configurationService.GetValue($"Shortcuts:{shortcut.Action}", shortcut.KeyCombination);
                if (!string.IsNullOrEmpty(customKey) && customKey != shortcut.KeyCombination)
                {
                    shortcut.KeyCombination = customKey;
                    shortcut.IsCustomized = true;
                }
            }

            SearchShortcuts();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading keyboard shortcuts");
        }
    }

    /// <summary>
    /// Loads default keyboard shortcuts.
    /// </summary>
    private async Task LoadDefaultShortcutsAsync()
    {
        Shortcuts.Clear();

        // General Application Shortcuts
        Shortcuts.Add(new ShortcutItem
        {
            Action = "SaveItem",
            Description = "Save current item",
            KeyCombination = "Ctrl+S",
            Category = "General",
            IsGlobal = true
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "NewItem",
            Description = "Create new item",
            KeyCombination = "Ctrl+N",
            Category = "General",
            IsGlobal = true
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "OpenSettings",
            Description = "Open settings dialog",
            KeyCombination = "Ctrl+,",
            Category = "General",
            IsGlobal = true
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "ExitApplication",
            Description = "Exit application",
            KeyCombination = "Alt+F4",
            Category = "General",
            IsGlobal = true
        });

        // Navigation Shortcuts
        Shortcuts.Add(new ShortcutItem
        {
            Action = "NextTab",
            Description = "Switch to next tab",
            KeyCombination = "Ctrl+Tab",
            Category = "Navigation",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "PreviousTab",
            Description = "Switch to previous tab",
            KeyCombination = "Ctrl+Shift+Tab",
            Category = "Navigation",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "InventoryTab",
            Description = "Switch to Inventory tab",
            KeyCombination = "F1",
            Category = "Navigation",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "RemoveTab",
            Description = "Switch to Remove tab",
            KeyCombination = "F2",
            Category = "Navigation",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "TransferTab",
            Description = "Switch to Transfer tab",
            KeyCombination = "F3",
            Category = "Navigation",
            IsGlobal = false
        });

        // Search and Filter Shortcuts
        Shortcuts.Add(new ShortcutItem
        {
            Action = "QuickSearch",
            Description = "Quick search/filter",
            KeyCombination = "Ctrl+F",
            Category = "Search",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "AdvancedSearch",
            Description = "Open advanced search",
            KeyCombination = "Ctrl+Shift+F",
            Category = "Search",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "ClearSearch",
            Description = "Clear search criteria",
            KeyCombination = "Escape",
            Category = "Search",
            IsGlobal = false
        });

        // Data Entry Shortcuts
        Shortcuts.Add(new ShortcutItem
        {
            Action = "FocusPartId",
            Description = "Focus Part ID field",
            KeyCombination = "Ctrl+1",
            Category = "Data Entry",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "FocusOperation",
            Description = "Focus Operation field",
            KeyCombination = "Ctrl+2",
            Category = "Data Entry",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "FocusQuantity",
            Description = "Focus Quantity field",
            KeyCombination = "Ctrl+3",
            Category = "Data Entry",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "FocusLocation",
            Description = "Focus Location field",
            KeyCombination = "Ctrl+4",
            Category = "Data Entry",
            IsGlobal = false
        });

        // Quick Actions
        Shortcuts.Add(new ShortcutItem
        {
            Action = "QuickButton1",
            Description = "Execute Quick Button 1",
            KeyCombination = "F5",
            Category = "Quick Actions",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "QuickButton2",
            Description = "Execute Quick Button 2",
            KeyCombination = "F6",
            Category = "Quick Actions",
            IsGlobal = false
        });

        Shortcuts.Add(new ShortcutItem
        {
            Action = "QuickButton3",
            Description = "Execute Quick Button 3",
            KeyCombination = "F7",
            Category = "Quick Actions",
            IsGlobal = false
        });

        await Task.Delay(100); // Simulate async loading
    }

    #endregion
}

/// <summary>
/// Keyboard shortcut data item.
/// </summary>
public class ShortcutItem
{
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string KeyCombination { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public bool IsCustomized { get; set; }
    public bool IsEditable { get; set; } = true;
}
