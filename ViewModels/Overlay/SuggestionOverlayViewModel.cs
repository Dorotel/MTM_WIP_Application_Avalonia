using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for managing suggestion overlay functionality with selection and cancellation capabilities.
/// Provides a clean interface for displaying and interacting with suggestion lists.
/// Uses manual property implementation following established MTM patterns.
/// </summary>
public partial class SuggestionOverlayViewModel : BaseViewModel
{
    private static int _debugInstanceIdCounter = 0;
    private readonly int _debugInstanceId;

    #region Private Fields

    private ObservableCollection<string> _suggestions = new();
    private string? _selectedSuggestion;
    private bool _isSuggestionSelected;
    private bool _isVisible;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the collection of available suggestions for selection.
    /// </summary>
    public ObservableCollection<string> Suggestions
    {
        get => _suggestions;
        set => SetPropertyWithLogging(ref _suggestions, value);
    }

    /// <summary>
    /// Gets or sets the currently selected suggestion from the list.
    /// </summary>
    public string? SelectedSuggestion
    {
        get => _selectedSuggestion;
        set
        {
            if (SetPropertyWithLogging(ref _selectedSuggestion, value))
            {
                UpdateSuggestionSelectedState();
                SelectCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a suggestion is currently selected.
    /// </summary>
    public bool IsSuggestionSelected
    {
        get => _isSuggestionSelected;
        set => SetPropertyWithLogging(ref _isSuggestionSelected, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the overlay is visible.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set => SetPropertyWithLogging(ref _isVisible, value);
    }

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

    #region Commands

    /// <summary>
    /// Command to select the current suggestion.
    /// </summary>
    public IRelayCommand SelectCommand { get; }

    /// <summary>
    /// Command to cancel the suggestion selection.
    /// </summary>
    public IRelayCommand CancelCommand { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the SuggestionOverlayViewModel class.
    /// </summary>
    /// <param name="logger">Logger for debugging and diagnostic information.</param>
    public SuggestionOverlayViewModel(ILogger<SuggestionOverlayViewModel> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _debugInstanceId = ++_debugInstanceIdCounter;

        using var scope = Logger.BeginScope("SuggestionOverlayInitialization");
        Logger.LogInformation("SuggestionOverlayViewModel instance {InstanceId} created", _debugInstanceId);

        // Initialize with empty collection
        Suggestions = new ObservableCollection<string>();

        Logger.LogInformation("SuggestionOverlayViewModel initialized with empty suggestions");

        // Initialize commands
        SelectCommand = new RelayCommand(ExecuteSelect, CanSelect);
        CancelCommand = new RelayCommand(ExecuteCancel);

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

        _debugInstanceId = ++_debugInstanceIdCounter;

        using var scope = Logger.BeginScope("SuggestionOverlayInitialization");
        Logger.LogInformation("SuggestionOverlayViewModel instance {InstanceId} created", _debugInstanceId);

        Suggestions = new ObservableCollection<string>(suggestions);

        Logger.LogInformation("SuggestionOverlayViewModel initialized with {SuggestionCount} suggestions", Suggestions.Count);

        // Initialize commands
        SelectCommand = new RelayCommand(ExecuteSelect, CanSelect);
        CancelCommand = new RelayCommand(ExecuteCancel);

        // Set up collection change monitoring
        SetupCollectionMonitoring();

        // Initial state update
        UpdateSuggestionSelectedState();
    }

    #endregion

    #region Command Methods

    /// <summary>
    /// Determines whether the select command can be executed.
    /// </summary>
    /// <returns>True if a valid suggestion is selected; otherwise, false.</returns>
    private bool CanSelect()
    {
        bool result = !string.IsNullOrEmpty(SelectedSuggestion) &&
                     Suggestions.Contains(SelectedSuggestion);

        return result;
    }

    /// <summary>
    /// Executes the select command to confirm the selected suggestion.
    /// </summary>
    private void ExecuteSelect()
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
    private void ExecuteCancel()
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

    #region Helper Methods

    /// <summary>
    /// Sets up monitoring for collection changes.
    /// </summary>
    private void SetupCollectionMonitoring()
    {
        // Ensure Select button state updates when Suggestions change
        Suggestions.CollectionChanged += (_, __) =>
        {
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
    }

    #endregion
}
