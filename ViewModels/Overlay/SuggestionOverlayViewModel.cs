using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for SuggestionOverlayView providing suggestion selection functionality.
/// Designed for overlay management with comprehensive logging and debugging support.
/// Uses MVVM Community Toolkit for property change notifications and command handling.
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
    [NotifyCanExecuteChangedFor(nameof(ExecuteSelectCommand))]
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
    /// Occurs when a suggestion has been selected by the user.
    /// </summary>
    public event Action<string?>? SuggestionSelected;

    /// <summary>
    /// Occurs when the suggestion overlay has been cancelled by the user.
    /// </summary>
    public event Action? Cancelled;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SuggestionOverlayViewModel"/> class.
    /// </summary>
    /// <param name="logger">The logger for this ViewModel. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public SuggestionOverlayViewModel(ILogger<SuggestionOverlayViewModel> logger) 
        : base(logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        _debugInstanceId = ++_debugInstanceIdCounter;
        LogInfo($"[DBG:{_debugInstanceId}] SuggestionOverlayViewModel instance created");
        Debug.WriteLine($"[DBG:{_debugInstanceId}] SuggestionOverlayViewModel instance created");
        
        Logger.LogInformation("SuggestionOverlayViewModel initialized with dependency injection");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuggestionOverlayViewModel"/> class with suggestions.
    /// </summary>
    /// <param name="suggestions">The initial collection of suggestions. Cannot be null.</param>
    /// <param name="logger">The logger for this ViewModel. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when suggestions or logger is null.</exception>
    public SuggestionOverlayViewModel(IEnumerable<string> suggestions, ILogger<SuggestionOverlayViewModel> logger) 
        : this(logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        if (suggestions == null)
            throw new ArgumentNullException(nameof(suggestions));

        Suggestions = new ObservableCollection<string>(suggestions);
        Debug.WriteLine($"[DBG:VM] SuggestionOverlayViewModel constructor. Suggestions.Count={Suggestions.Count}");
        LogInfo($"[DBG:{_debugInstanceId}] SuggestionOverlayViewModel constructor. Suggestions.Count={Suggestions.Count}");
        
        // Ensure Select button state updates when Suggestions change
        Suggestions.CollectionChanged += (_, __) =>
        {
            Debug.WriteLine($"[DBG:VM] Suggestions collection changed. Suggestions.Count={Suggestions.Count}");
            LogInfo($"[DBG:{_debugInstanceId}] Suggestions collection changed. Suggestions.Count={Suggestions.Count}");
            IsSuggestionSelected = SelectedSuggestion != null;
        };

        // Initial state update
        IsSuggestionSelected = SelectedSuggestion != null;
        Debug.WriteLine($"[DBG:VM] SuggestionOverlayViewModel initialized");
        LogInfo($"[DBG:{_debugInstanceId}] SuggestionOverlayViewModel initialized");
    }

    #region Property Change Handlers

    /// <summary>
    /// Handles changes to the SelectedSuggestion property.
    /// </summary>
    /// <param name="value">The new selected suggestion value.</param>
    partial void OnSelectedSuggestionChanged(string? value)
    {
        var oldValue = SelectedSuggestion;
        Debug.WriteLine($"[DBG:VM] SelectedSuggestion.SET triggered. OldValue='{oldValue}', NewValue='{value}'.");
        LogInfo($"[DBG:{_debugInstanceId}] SelectedSuggestion changed from '{oldValue}' to '{value}'");
        
        IsSuggestionSelected = value != null;
    }

    /// <summary>
    /// Handles changes to the IsSuggestionSelected property.
    /// </summary>
    /// <param name="value">The new suggestion selected state.</param>
    partial void OnIsSuggestionSelectedChanged(bool value)
    {
        Debug.WriteLine($"[DBG:VM] IsSuggestionSelected changed to '{value}'");
        LogInfo($"[DBG:{_debugInstanceId}] IsSuggestionSelected set to: {value}");
    }

    #endregion

    #region Command Methods

    /// <summary>
    /// Determines whether the select command can be executed.
    /// </summary>
    /// <returns>True if a suggestion is selected and valid; otherwise, false.</returns>
    private bool CanExecuteSelect()
    {
        bool result = !string.IsNullOrEmpty(SelectedSuggestion) && Suggestions.Contains(SelectedSuggestion!);
        Debug.WriteLine($"[DBG:VM] SelectCommand.CanExecute check. SelectedSuggestion='{SelectedSuggestion}', Result={result}");
        LogInfo($"[DBG:{_debugInstanceId}] SelectCommand.CanExecute check. SelectedSuggestion='{SelectedSuggestion}', Result={result}");
        return result;
    }

    /// <summary>
    /// Executes the select command to confirm the selected suggestion.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteSelect))]
    private void ExecuteSelect()
    {
        try
        {
            using var scope = Logger.BeginScope("SuggestionSelection");
            var sel = SelectedSuggestion ?? "null";
            Debug.WriteLine($"[DBG:VM] OnSelect called. SelectedSuggestion='{sel}'");
            Logger.LogInformation("SelectCommand executed: SelectedSuggestion={SelectedSuggestion}", sel);
            
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
    private void ExecuteCancel()
    {
        try
        {
            using var scope = Logger.BeginScope("SuggestionCancellation");
            Debug.WriteLine($"[DBG:VM] OnCancel called");
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
    /// Logs information with debug output support.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Optional message arguments.</param>
    private void LogInfo(string message, params object[] args)
    {
        Logger.LogInformation(message, args);
        try
        {
            string debugMsg = args != null && args.Length > 0 
                ? string.Format(message.Replace("{", "{0:"), args) 
                : message;
            Debug.WriteLine($"[SuggestionOverlayViewModel] {debugMsg}");
        }
        catch
        {
            Debug.WriteLine($"[SuggestionOverlayViewModel] {message} (logging error)");
        }
    }

    #endregion
}
