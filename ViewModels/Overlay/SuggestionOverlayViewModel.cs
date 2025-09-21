using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for managing suggestion overlay functionality with selection and cancellation capabilities.
/// Follows WeekendRefactor patterns for proper overlay architecture without circular dependencies.
/// Uses MVVM Community Toolkit [ObservableProperty] and [RelayCommand] patterns.
/// </summary>
public partial class SuggestionOverlayViewModel : BaseViewModel
{
    #region Observable Properties (MVVM Community Toolkit)

    /// <summary>
    /// Collection of available suggestions for selection.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> suggestions = new();

    /// <summary>
    /// Currently selected suggestion from the list.
    /// </summary>
    [ObservableProperty]
    private string? selectedSuggestion;

    /// <summary>
    /// Indicates whether a suggestion is currently selected.
    /// </summary>
    [ObservableProperty]
    private bool isSuggestionSelected;

    /// <summary>
    /// Indicates whether the overlay is visible.
    /// </summary>
    [ObservableProperty]
    private bool isVisible;

    /// <summary>
    /// Title displayed at the top of the overlay.
    /// </summary>
    [ObservableProperty]
    private string title = "Did you mean?";

    /// <summary>
    /// Message displayed to the user.
    /// </summary>
    [ObservableProperty]
    private string message = "No exact match found. Please select from similar items:";

    #endregion

    #region Events

    /// <summary>
    /// Event raised when a suggestion is selected.
    /// Args: Selected suggestion string
    /// </summary>
    public event Action<string>? SuggestionSelected;

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
        ArgumentNullException.ThrowIfNull(logger);

        Logger.LogInformation("SuggestionOverlayViewModel created with empty suggestions");

        // Initialize with empty collection
        Suggestions = new ObservableCollection<string>();

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
        ArgumentNullException.ThrowIfNull(logger);

        Suggestions = new ObservableCollection<string>(suggestions);

        Logger.LogInformation("SuggestionOverlayViewModel created with {SuggestionCount} suggestions", Suggestions.Count);

        // Set up collection change monitoring
        SetupCollectionMonitoring();

        // Initial state update
        UpdateSuggestionSelectedState();
    }

    #endregion

    #region Commands (MVVM Community Toolkit)

    /// <summary>
    /// Command to select the current suggestion.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSelect))]
    private void Select()
    {
        try
        {
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
    /// Command to cancel the suggestion selection.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        try
        {
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

    /// <summary>
    /// Command to dismiss the overlay (same as cancel).
    /// </summary>
    [RelayCommand]
    private void Dismiss()
    {
        Cancel();
    }

    #endregion

    #region Command Helper Methods

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

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Called when SelectedSuggestion property changes.
    /// Updates related UI state and command availability.
    /// </summary>
    partial void OnSelectedSuggestionChanged(string? value)
    {
        UpdateSuggestionSelectedState();
        SelectCommand.NotifyCanExecuteChanged();

        Logger.LogDebug("SelectedSuggestion changed to: '{SelectedSuggestion}'", value);
    }

    /// <summary>
    /// Called when Suggestions collection property changes.
    /// Updates command availability.
    /// </summary>
    partial void OnSuggestionsChanged(ObservableCollection<string> value)
    {
        SetupCollectionMonitoring();
        UpdateSuggestionSelectedState();
        SelectCommand.NotifyCanExecuteChanged();

        Logger.LogDebug("Suggestions collection changed. New count: {Count}", value?.Count ?? 0);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Sets up monitoring for collection changes.
    /// </summary>
    private void SetupCollectionMonitoring()
    {
        if (Suggestions != null)
        {
            // Monitor collection changes to update UI state
            Suggestions.CollectionChanged += (_, __) =>
            {
                Logger.LogDebug("Suggestions collection changed. Count: {SuggestionCount}", Suggestions.Count);
                UpdateSuggestionSelectedState();
                SelectCommand.NotifyCanExecuteChanged();
            };
        }
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

    #region Public Methods

    /// <summary>
    /// Shows the overlay with the provided suggestions.
    /// </summary>
    /// <param name="suggestions">Suggestions to display.</param>
    public void ShowWithSuggestions(IEnumerable<string> suggestions)
    {
        ArgumentNullException.ThrowIfNull(suggestions);

        Suggestions.Clear();
        foreach (var suggestion in suggestions)
        {
            Suggestions.Add(suggestion);
        }

        // Select first suggestion by default if available
        if (Suggestions.Count > 0)
        {
            SelectedSuggestion = Suggestions[0];
        }

        IsVisible = true;

        Logger.LogInformation("SuggestionOverlay shown with {Count} suggestions", Suggestions.Count);
    }

    /// <summary>
    /// Hides the overlay without selection.
    /// </summary>
    public void Hide()
    {
        IsVisible = false;
        Logger.LogInformation("SuggestionOverlay hidden");
    }

    #endregion
}
