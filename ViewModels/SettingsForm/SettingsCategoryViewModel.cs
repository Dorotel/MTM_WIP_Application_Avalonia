using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for TreeView category items in SettingsForm navigation.
/// Represents hierarchical settings categories with optional sub-categories.
/// </summary>
public partial class SettingsCategoryViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _isExpanded = true;

    public SettingsCategoryViewModel()
    {
        SubCategories = new ObservableCollection<SettingsCategoryViewModel>();
    }

    #region Properties

    /// <summary>
    /// Unique identifier for the category.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown in TreeView.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Icon emoji or symbol for the category.
    /// </summary>
    public string Icon { get; set; } = "ðŸ“";

    /// <summary>
    /// Description of the category functionality.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of ViewModel panel associated with this category.
    /// </summary>
    public Type? PanelType { get; set; }

    /// <summary>
    /// Parent category for hierarchical structure.
    /// </summary>
    public SettingsCategoryViewModel? Parent { get; set; }

    /// <summary>
    /// Sub-categories for hierarchical navigation.
    /// </summary>
    public ObservableCollection<SettingsCategoryViewModel> SubCategories { get; }

    /// <summary>
    /// Indicates if this category has sub-categories.
    /// </summary>
    public bool HasSubCategories { get; set; }

    /// <summary>
    /// Indicates if this category represents an actual panel (not just a grouping).
    /// </summary>
    public bool IsPanel => PanelType != null;

    /// <summary>
    /// Full path of the category including parent hierarchy.
    /// </summary>
    public string FullPath
    {
        get
        {
            if (Parent != null)
            {
                return $"{Parent.FullPath} > {DisplayName}";
            }
            return DisplayName;
        }
    }

    #endregion
}

/// <summary>
/// ViewModel for individual settings panels within TabView.
/// Represents a loaded settings panel with its content and state.
/// </summary>
public class SettingsPanelViewModel : IDisposable
{
    public SettingsPanelViewModel(string categoryId, string displayName, UserControl content, BaseViewModel viewModel)
    {
        CategoryId = categoryId ?? throw new ArgumentNullException(nameof(categoryId));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        
        LoadedAt = DateTime.UtcNow;
    }

    #region Properties

    /// <summary>
    /// Associated category identifier.
    /// </summary>
    public string CategoryId { get; }

    /// <summary>
    /// Display name for the panel.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// UserControl content for the panel.
    /// </summary>
    public UserControl Content { get; }

    /// <summary>
    /// Associated ViewModel for the panel.
    /// </summary>
    public BaseViewModel ViewModel { get; }

    /// <summary>
    /// Timestamp when the panel was loaded.
    /// </summary>
    public DateTime LoadedAt { get; }

    /// <summary>
    /// Indicates if the panel is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates if the panel has been modified since loading.
    /// </summary>
    public bool IsModified { get; set; }

    /// <summary>
    /// Last access timestamp for virtual panel management.
    /// </summary>
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

    #endregion

    #region Methods

    /// <summary>
    /// Updates the last accessed timestamp.
    /// </summary>
    public void MarkAccessed()
    {
        LastAccessed = DateTime.UtcNow;
    }

    /// <summary>
    /// Determines if panel is eligible for virtual disposal based on access time.
    /// </summary>
    public bool IsEligibleForDisposal(TimeSpan threshold)
    {
        return DateTime.UtcNow - LastAccessed > threshold && !IsModified;
    }

    #endregion

    #region IDisposable

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose ViewModel if it implements IDisposable
                if (ViewModel is IDisposable disposableViewModel)
                {
                    disposableViewModel.Dispose();
                }
            }
            _disposed = true;
        }
    }

    #endregion
}
