using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.Views;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Interface for the suggestion overlay service that provides autocomplete functionality.
/// </summary>
public interface ISuggestionOverlayService
{
    /// <summary>
    /// Shows a suggestion overlay with filtered options based on user input.
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="suggestions">The list of available suggestions</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    Task<string?> ShowSuggestionsAsync(Control targetControl, IEnumerable<string> suggestions, string userInput);
}

/// <summary>
/// Service that provides suggestion overlay functionality for autocomplete scenarios.
/// Implements a lightweight popup overlay system for suggesting values to users.
/// Uses standard .NET patterns without ReactiveUI dependencies.
/// </summary>
public class SuggestionOverlayService : ISuggestionOverlayService
{
    private readonly ILogger<SuggestionOverlayService> _logger;

    /// <summary>
    /// Initializes a new instance of the SuggestionOverlayService.
    /// </summary>
    /// <param name="logger">Logger for debugging and diagnostics</param>
    public SuggestionOverlayService(ILogger<SuggestionOverlayService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("SuggestionOverlayService created successfully");
    }

    /// <summary>
    /// Shows a suggestion overlay with filtered options based on user input.
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="suggestions">The list of available suggestions</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowSuggestionsAsync(Control targetControl, IEnumerable<string> suggestions, string userInput)
    {
        try
        {
            if (targetControl == null)
            {
                _logger.LogWarning("Target control is null, cannot show suggestions");
                return null;
            }

            var suggestionList = suggestions?.ToList() ?? new List<string>();
            
            if (!suggestionList.Any())
            {
                _logger.LogDebug("No suggestions provided, returning null");
                return null;
            }

            // Filter suggestions based on user input
            var filteredSuggestions = FilterSuggestions(suggestionList, userInput);
            
            if (!filteredSuggestions.Any())
            {
                _logger.LogDebug("No matching suggestions found for input: '{UserInput}'", userInput);
                return null;
            }

            _logger.LogDebug("Showing {Count} suggestions for input: '{UserInput}'", filteredSuggestions.Count, userInput);

            // Create and show the actual overlay
            return await ShowOverlayAsync(targetControl, filteredSuggestions, userInput);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing suggestions overlay");
            return null;
        }
    }

    /// <summary>
    /// Shows the actual suggestion overlay in the MainView panel and waits for user interaction.
    /// </summary>
    /// <param name="targetControl">The control to find the MainView from</param>
    /// <param name="filteredSuggestions">The filtered list of suggestions to display</param>
    /// <param name="userInput">The original user input</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    private async Task<string?> ShowOverlayAsync(Control targetControl, List<string> filteredSuggestions, string userInput)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                _logger.LogDebug("Creating suggestion overlay with {Count} suggestions", filteredSuggestions.Count);

                // Find the MainView instance in the visual tree
                var mainView = MTM_WIP_Application_Avalonia.Views.MainView.FindMainView(targetControl);
                if (mainView == null)
                {
                    _logger.LogWarning("Could not find MainView instance, falling back to popup");
                    return await ShowPopupOverlayAsync(targetControl, filteredSuggestions, userInput);
                }

                // Create the ViewModel with suggestions
                var viewModel = new SuggestionOverlayViewModel(filteredSuggestions);
                
                // Debug: Log the suggestions that were passed to the ViewModel
                _logger.LogDebug("ViewModel created with {Count} suggestions: {Suggestions}", 
                    viewModel.Suggestions.Count, 
                    string.Join(", ", viewModel.Suggestions.Take(3)) + (viewModel.Suggestions.Count > 3 ? "..." : ""));
                
                // Create the overlay view
                var overlayView = new SuggestionOverlayView
                {
                    DataContext = viewModel,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
                };

                // Set up completion source to wait for user interaction
                var completionSource = new TaskCompletionSource<string?>();

                // Handle suggestion selection
                viewModel.SuggestionSelected += (selectedSuggestion) =>
                {
                    _logger.LogDebug("User selected suggestion: '{Suggestion}'", selectedSuggestion);
                    mainView.HideSuggestionOverlay();
                    completionSource.TrySetResult(selectedSuggestion);
                };

                // Handle cancellation
                viewModel.Cancelled += () =>
                {
                    _logger.LogDebug("User cancelled suggestion overlay");
                    mainView.HideSuggestionOverlay();
                    completionSource.TrySetResult(null);
                };

                // Show the overlay in the MainView panel
                _logger.LogDebug("Showing suggestion overlay in MainView panel");
                mainView.ShowSuggestionOverlay(overlayView);

                // Wait for user interaction
                var result = await completionSource.Task;
                _logger.LogDebug("Suggestion overlay completed with result: '{Result}'", result ?? "null");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShowOverlayAsync");
                return null;
            }
        });
    }

    /// <summary>
    /// Fallback method that shows a popup overlay when MainView is not accessible.
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="filteredSuggestions">The filtered list of suggestions to display</param>
    /// <param name="userInput">The original user input</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    private async Task<string?> ShowPopupOverlayAsync(Control targetControl, List<string> filteredSuggestions, string userInput)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                _logger.LogDebug("Using fallback popup overlay");

                // Create the ViewModel with suggestions
                var viewModel = new SuggestionOverlayViewModel(filteredSuggestions);
                
                // Create the overlay view
                var overlayView = new SuggestionOverlayView
                {
                    DataContext = viewModel,
                    Width = 500,
                    Height = 400,
                    MinWidth = 400,
                    MinHeight = 300
                };

                // Find the main window or top-level control for better positioning
                var window = TopLevel.GetTopLevel(targetControl);
                var placementTarget = window ?? targetControl;

                // Create a popup to host the overlay
                var popup = new Popup
                {
                    Child = overlayView,
                    PlacementTarget = placementTarget,
                    Placement = PlacementMode.Center,
                    IsLightDismissEnabled = true,
                    Width = 500,
                    Height = 400,
                    HorizontalOffset = 0,
                    VerticalOffset = 0
                };

                // Set up completion source to wait for user interaction
                var completionSource = new TaskCompletionSource<string?>();

                // Handle suggestion selection
                viewModel.SuggestionSelected += (selectedSuggestion) =>
                {
                    _logger.LogDebug("User selected suggestion: '{Suggestion}'", selectedSuggestion);
                    popup.IsOpen = false;
                    completionSource.TrySetResult(selectedSuggestion);
                };

                // Handle cancellation
                viewModel.Cancelled += () =>
                {
                    _logger.LogDebug("User cancelled suggestion overlay");
                    popup.IsOpen = false;
                    completionSource.TrySetResult(null);
                };

                // Handle popup closed (light dismiss)
                popup.Closed += (sender, e) =>
                {
                    _logger.LogDebug("Suggestion overlay popup closed");
                    if (!completionSource.Task.IsCompleted)
                    {
                        completionSource.TrySetResult(null);
                    }
                };

                // Show the popup
                _logger.LogDebug("Opening suggestion overlay popup");
                popup.IsOpen = true;

                // Wait for user interaction
                var result = await completionSource.Task;
                _logger.LogDebug("Suggestion overlay completed with result: '{Result}'", result ?? "null");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShowPopupOverlayAsync");
                return null;
            }
        });
    }

    /// <summary>
    /// Filters suggestions based on user input using case-insensitive substring matching.
    /// </summary>
    /// <param name="suggestions">All available suggestions</param>
    /// <param name="userInput">The user's current input</param>
    /// <returns>Filtered list of suggestions that match the input</returns>
    private List<string> FilterSuggestions(List<string> suggestions, string userInput)
    {
        if (string.IsNullOrEmpty(userInput))
        {
            return suggestions.Take(10).ToList(); // Limit to 10 suggestions
        }

        var filtered = suggestions
            .Where(s => !string.IsNullOrEmpty(s) && 
                       s.Contains(userInput, StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.StartsWith(userInput, StringComparison.OrdinalIgnoreCase) ? 0 : 1) // Prefer starts-with matches
            .ThenBy(s => s.Length) // Prefer shorter matches
            .ThenBy(s => s) // Alphabetical order as final tie-breaker
            .Take(10) // Limit to 10 suggestions for performance
            .ToList();

        return filtered;
    }
}

/// <summary>
/// Simple data model for suggestion items.
/// </summary>
public class SuggestionItem
{
    public string Value { get; set; } = string.Empty;
    public string DisplayText { get; set; } = string.Empty;
    public bool IsExactMatch { get; set; }
    
    public SuggestionItem(string value, string? displayText = null)
    {
        Value = value ?? string.Empty;
        DisplayText = displayText ?? value ?? string.Empty;
    }
}

