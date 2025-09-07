using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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
/// Enhanced with focus management for better user experience.
/// </summary>
public class SuggestionOverlayService : ISuggestionOverlayService
{
    private readonly ILogger<SuggestionOverlayService> _logger;
    private readonly IFocusManagementService? _focusManagementService;

    /// <summary>
    /// Initializes a new instance of the SuggestionOverlayService.
    /// </summary>
    /// <param name="logger">Logger for debugging and diagnostics</param>
    /// <param name="focusManagementService">Optional focus management service for enhanced UX</param>
    public SuggestionOverlayService(
        ILogger<SuggestionOverlayService> logger, 
        IFocusManagementService? focusManagementService = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _focusManagementService = focusManagementService;
        _logger.LogDebug("SuggestionOverlayService created successfully with focus management: {HasFocusManagement}", 
            _focusManagementService != null);
    }

    /// <summary>
    /// Shows a suggestion overlay with filtered options based on user input.
    /// Includes focus management: moves to next tab index on selection, stays on current tab index on cancellation.
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="suggestions">The list of available suggestions</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowSuggestionsAsync(Control targetControl, IEnumerable<string> suggestions, string userInput)
    {
        try
        {
            // CRITICAL: Check if a tab switch is in progress - if so, don't show overlay
            if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
            {
                _logger.LogDebug("Tab switch in progress - skipping suggestion overlay for input: '{UserInput}'", userInput);
                return null;
            }

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

            // Double-check tab switch flag before filtering (extra safety)
            if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
            {
                _logger.LogDebug("Tab switch detected during suggestion processing - aborting overlay");
                return null;
            }

            // Filter suggestions based on user input
            var filteredSuggestions = FilterSuggestions(suggestionList, userInput);
            
            if (!filteredSuggestions.Any())
            {
                _logger.LogDebug("No matching suggestions found for input: '{UserInput}'", userInput);
                return null;
            }

            // Final check before showing overlay
            if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
            {
                _logger.LogDebug("Tab switch detected before showing overlay - aborting for input: '{UserInput}'", userInput);
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
                // Final safety check - if tab switch is in progress, don't show overlay
                if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
                {
                    _logger.LogDebug("Tab switch detected in ShowOverlayAsync - aborting overlay creation");
                    return null;
                }

                _logger.LogDebug("Creating suggestion overlay with {Count} suggestions", filteredSuggestions.Count);

                // Find the MainView instance in the visual tree
                var mainView = MTM_WIP_Application_Avalonia.Views.MainView.FindMainView(targetControl);
                if (mainView == null)
                {
                    _logger.LogWarning("Could not find MainView instance, falling back to popup");
                    return await ShowPopupOverlayAsync(targetControl, filteredSuggestions, userInput);
                }

                // One more check before creating ViewModel (tab switch could have started)
                if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
                {
                    _logger.LogDebug("Tab switch detected before ViewModel creation - aborting overlay");
                    return null;
                }

                // Create the ViewModel with suggestions and logger
                var vmLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SuggestionOverlayViewModel>();
                var viewModel = new SuggestionOverlayViewModel(filteredSuggestions, vmLogger);
                
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
                    
                    // Handle focus management - move to next tab index after selection
                    if (_focusManagementService != null)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await _focusManagementService.SetNextTabIndexFocusAsync(targetControl);
                                _logger.LogDebug("Focus moved to next tab index after suggestion selection");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error setting focus to next tab index after suggestion selection");
                            }
                        });
                    }
                    
                    completionSource.TrySetResult(selectedSuggestion);
                };

                // Handle cancellation
                viewModel.Cancelled += () =>
                {
                    _logger.LogDebug("User cancelled suggestion overlay");
                    mainView.HideSuggestionOverlay();
                    
                    // Handle focus management - stay on current tab index after cancellation
                    if (_focusManagementService != null)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await _focusManagementService.SetCurrentTabIndexFocusAsync(targetControl);
                                _logger.LogDebug("Focus maintained on current tab index after overlay cancellation");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error setting focus to current tab index after overlay cancellation");
                            }
                        });
                    }
                    
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
                // Check if tab switch is in progress before showing popup
                if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
                {
                    _logger.LogDebug("Tab switch detected in ShowPopupOverlayAsync - aborting popup overlay");
                    return null;
                }

                _logger.LogDebug("Using fallback popup overlay");

                // Create the ViewModel with suggestions and logger
                var vmLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SuggestionOverlayViewModel>();
                var viewModel = new SuggestionOverlayViewModel(filteredSuggestions, vmLogger);
                
                // Create the overlay view
                var overlayView = new SuggestionOverlayView
                {
                    DataContext = viewModel,
                    Width = 600,
                    Height = 500,
                    MinWidth = 500,
                    MinHeight = 400
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
                    Width = 600,
                    Height = 500,
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
                    
                    // Handle focus management - move to next tab index after selection
                    if (_focusManagementService != null)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await _focusManagementService.SetNextTabIndexFocusAsync(targetControl);
                                _logger.LogDebug("Focus moved to next tab index after suggestion selection (popup)");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error setting focus to next tab index after suggestion selection (popup)");
                            }
                        });
                    }
                    
                    completionSource.TrySetResult(selectedSuggestion);
                };

                // Handle cancellation
                viewModel.Cancelled += () =>
                {
                    _logger.LogDebug("User cancelled suggestion overlay");
                    popup.IsOpen = false;
                    
                    // Handle focus management - stay on current tab index after cancellation
                    if (_focusManagementService != null)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await _focusManagementService.SetCurrentTabIndexFocusAsync(targetControl);
                                _logger.LogDebug("Focus maintained on current tab index after overlay cancellation (popup)");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error setting focus to current tab index after overlay cancellation (popup)");
                            }
                        });
                    }
                    
                    completionSource.TrySetResult(null);
                };

                // Handle popup closed (light dismiss)
                popup.Closed += (sender, e) =>
                {
                    _logger.LogDebug("Suggestion overlay popup closed");
                    if (!completionSource.Task.IsCompleted)
                    {
                        // Handle focus management for light dismiss - stay on current tab index
                        if (_focusManagementService != null)
                        {
                            Task.Run(async () =>
                            {
                                try
                                {
                                    await _focusManagementService.SetCurrentTabIndexFocusAsync(targetControl);
                                    _logger.LogDebug("Focus maintained on current tab index after light dismiss (popup)");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error setting focus to current tab index after light dismiss (popup)");
                                }
                            });
                        }
                        
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
    /// Supports wildcard matching using % symbol (e.g., "R-%-0%" matches "R-ABC-01", "R-XYZ-02").
    /// </summary>
    /// <param name="suggestions">All available suggestions</param>
    /// <param name="userInput">The user's current input (may contain % wildcards)</param>
    /// <returns>Filtered list of suggestions that match the input</returns>
    private List<string> FilterSuggestions(List<string> suggestions, string userInput)
    {
        if (string.IsNullOrEmpty(userInput))
        {
            return suggestions.Take(100).ToList(); // Allow up to 100 suggestions when no input
        }

        // Check if the input contains wildcards (% symbols)
        bool hasWildcards = userInput.Contains('%');

        List<string> filtered;

        if (hasWildcards)
        {
            // Convert wildcard pattern to regex for matching
            var regexPattern = ConvertWildcardToRegex(userInput);
            _logger.LogDebug("Wildcard pattern '{Input}' converted to regex: '{Regex}'", userInput, regexPattern);
            
            try
            {
                var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

                filtered = suggestions
                    .Where(s => !string.IsNullOrEmpty(s) && regex.IsMatch(s))
                    .OrderBy(s => s.Length) // Prefer shorter matches
                    .ThenBy(s => s) // Alphabetical order as tie-breaker
                    .Take(50) // Allow up to 50 filtered suggestions with scrolling support
                    .ToList();

                _logger.LogDebug("Wildcard search '{Input}' found {Count} matches", userInput, filtered.Count);
                if (filtered.Count <= 5) // Log first few matches for debugging
                {
                    _logger.LogDebug("First matches: {Matches}", string.Join(", ", filtered));
                }
            }
            catch (ArgumentException ex)
            {
                // If regex fails, fall back to simple contains matching
                _logger.LogWarning("Invalid wildcard pattern '{Pattern}': {Error}. Using simple matching.", userInput, ex.Message);
                
                filtered = suggestions
                    .Where(s => !string.IsNullOrEmpty(s) && 
                               s.Contains(userInput.Replace("%", ""), StringComparison.OrdinalIgnoreCase))
                    .Take(50)
                    .ToList();
            }
        }
        else
        {
            // Standard substring matching (no wildcards)
            filtered = suggestions
                .Where(s => !string.IsNullOrEmpty(s) && 
                           s.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.StartsWith(userInput, StringComparison.OrdinalIgnoreCase) ? 0 : 1) // Prefer starts-with matches
                .ThenBy(s => s.Length) // Prefer shorter matches
                .ThenBy(s => s) // Alphabetical order as final tie-breaker
                .Take(50) // Allow up to 50 filtered suggestions with scrolling support
                .ToList();
        }

        return filtered;
    }

    /// <summary>
    /// Converts a wildcard pattern (using % symbols) to a regex pattern.
    /// Examples: "R-%-0%" becomes "^R\-.*\-0.*$"
    /// </summary>
    /// <param name="wildcardPattern">Pattern with % wildcards</param>
    /// <returns>Equivalent regex pattern</returns>
    private string ConvertWildcardToRegex(string wildcardPattern)
    {
        if (string.IsNullOrEmpty(wildcardPattern))
            return string.Empty;

        _logger.LogDebug("Converting wildcard pattern: '{Pattern}'", wildcardPattern);

        // First replace % with a placeholder that won't be escaped
        var withPlaceholder = wildcardPattern.Replace("%", "<!WILDCARD!>");
        _logger.LogDebug("With placeholder: '{Pattern}'", withPlaceholder);
        
        // Escape special regex characters (% is now safe as placeholder)
        var escaped = Regex.Escape(withPlaceholder);
        _logger.LogDebug("After escaping: '{Escaped}'", escaped);
        
        // Replace placeholder with .* (match any characters)
        var regexPattern = escaped.Replace("<!WILDCARD!>", ".*");
        _logger.LogDebug("After replacing wildcards: '{Pattern}'", regexPattern);
        
        // Anchor the pattern to match the entire string
        regexPattern = "^" + regexPattern + "$";
        _logger.LogDebug("Final regex pattern: '{FinalPattern}'", regexPattern);
        
        return regexPattern;
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

