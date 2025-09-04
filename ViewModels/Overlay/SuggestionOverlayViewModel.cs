using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for managing suggestion overlay functionality with selection and cancellation capabilities.
/// Provides a clean interface for displaying and interacting with suggestion lists.
/// </summary>
public partial class SuggestionOverlayViewModel : BaseViewModel
{
    private static int _debugInstanceIdCounter = 0;
    private readonly int _debugInstanceId;

    #region Observable Properties

    /// <summary>
    /// Gets or sets the collection of available suggestions for selection.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> suggestions = new();

    /// <summary>
    /// Gets or sets the currently selected suggestion from the list.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SelectCommand))]
    private string? selectedSuggestion;

    /// <summary>
    /// Gets or sets a value indicating whether a suggestion is currently selected.
    /// </summary>
    [ObservableProperty]
    private bool isSuggestionSelected;

    /// <summary>
    /// Gets or sets a value indicating whether the overlay is visible.
    /// </summary>
    [ObservableProperty]
    private bool isVisible;

    #endregion

    #region Events

    /// <summary>
    /// Event raised when a suggestion is selected.
    /// </summary>
    public event Action<string?>? SuggestionSelected;

    /// <summary>
    /// Event raised when the overlay is cancelled.
    /// </summary>
    public event Action? Cancelled;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the SuggestionOverlayViewModel class.
    /// </summary>
    /// <param name="logger">Logger for debugging and diagnostic information.</param>
    public SuggestionOverlayViewModel(ILogger<SuggestionOverlayViewModel> logger) : base(logger)
    {
        _debugInstanceId = ++_debugInstanceIdCounter;
        
        using var scope = Logger.BeginScope("SuggestionOverlayInitialization");
        Logger.LogInformation("SuggestionOverlayViewModel instance {InstanceId} created", _debugInstanceId);

        // Initialize with empty collection
        Suggestions = new ObservableCollection<string>();
        
        Logger.LogInformation("SuggestionOverlayViewModel initialized with empty suggestions");

        // Set up collection change monitoring
        SetupCollectionMonitoring();

        // Initial state update
        UpdateSuggestionSelectedState();
    }

    /// <summary>
    /// Initializes a new instance of the SuggestionOverlayViewModel class with suggestions.
    /// </summary>
    /// <param name="suggestions">The initial collection of suggestions to display.</param>
    /// <param name="logger">Logger for debugging and diagnostic information.</param>
    public SuggestionOverlayViewModel(
        IEnumerable<string> suggestions,
        ILogger<SuggestionOverlayViewModel> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(suggestions);

        _debugInstanceId = ++_debugInstanceIdCounter;
        
        using var scope = Logger.BeginScope("SuggestionOverlayInitialization");
        Logger.LogInformation("SuggestionOverlayViewModel instance {InstanceId} created", _debugInstanceId);

        Suggestions = new ObservableCollection<string>(suggestions);
        
        Logger.LogInformation("SuggestionOverlayViewModel initialized with {SuggestionCount} suggestions", Suggestions.Count);

        // Set up collection change monitoring
        SetupCollectionMonitoring();

        // Initial state update
        UpdateSuggestionSelectedState();
    }

    #endregion

    #region Commands

    /// <summary>
    /// Determines whether the select command can be executed.
    /// </summary>
    /// <returns>True if a valid suggestion is selected; otherwise, false.</returns>
    private bool CanSelect()
    {
        bool result = !string.IsNullOrEmpty(SelectedSuggestion) && 
                     Suggestions.Contains(SelectedSuggestion);
        
        Logger.LogDebug("SelectCommand.CanExecute check: SelectedSuggestion='{SelectedSuggestion}', Result={Result}", 
            SelectedSuggestion, result);
        
        return result;
    }

    /// <summary>
    /// Executes the select command to confirm the selected suggestion.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSelect))]
    private void Select()
    {
        try
        {
            using var scope = Logger.BeginScope("SuggestionSelection");
            Logger.LogInformation("SelectCommand executed: SelectedSuggestion={SelectedSuggestion}", SelectedSuggestion);

            if (!string.IsNullOrEmpty(SelectedSuggestion))
            {
                SuggestionSelected?.Invoke(SelectedSuggestion);
                IsVisible = false;
                Logger.LogInformation("Suggestion selected and overlay closed: {SelectedSuggestion}", SelectedSuggestion);
            }
            else
            {
                Logger.LogWarning("Select command executed but no valid suggestion was selected");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing select command for suggestion: {SelectedSuggestion}", SelectedSuggestion);
        }
    }

    /// <summary>
    /// Executes the cancel command to close the overlay without selection.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        try
        {
            using var scope = Logger.BeginScope("SuggestionCancellation");
            Logger.LogInformation("CancelCommand executed - closing suggestion overlay");

            Cancelled?.Invoke();
            IsVisible = false;
            Logger.LogInformation("Suggestion overlay cancelled and closed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing cancel command for suggestion overlay");
        }
    }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handles changes to the SelectedSuggestion property.
    /// </summary>
    /// <param name="value">The new selected suggestion value.</param>
    partial void OnSelectedSuggestionChanged(string? value)
    {
        Logger.LogDebug("SelectedSuggestion changed to '{SelectedSuggestion}'", value);
        UpdateSuggestionSelectedState();
        SelectCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Sets up monitoring for collection changes.
    /// </summary>
    private void SetupCollectionMonitoring()
    {
        // Ensure Select button state updates when Suggestions change
        Suggestions.CollectionChanged += (_, __) =>
        {
            Logger.LogDebug("Suggestions collection changed. Count: {SuggestionCount}", Suggestions.Count);
            UpdateSuggestionSelectedState();
            SelectCommand.NotifyCanExecuteChanged();
        };
    }

    /// <summary>
    /// Updates the suggestion selected state based on current selection.
    /// </summary>
    private void UpdateSuggestionSelectedState()
    {
        IsSuggestionSelected = !string.IsNullOrEmpty(SelectedSuggestion);
        Logger.LogDebug("IsSuggestionSelected updated to {IsSuggestionSelected}", IsSuggestionSelected);
    }

    #endregion
}