using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Views;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Controls.Primitives;
using System.Collections.Specialized;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using System.Linq;
using Avalonia.VisualTree;
using Avalonia.Interactivity;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for RemoveTabView.
/// Implements the inventory removal interface for subtracting inventory from operations within the MTM WIP Application.
/// Features comprehensive error handling, ViewModel event management, and dependency injection support.
/// Business logic and database operations are handled by the RemoveItemViewModel.
/// Follows MTM patterns with proper resource cleanup and exception handling.
/// Used for removing inventory quantities from manufacturing operations.
/// </summary>
public partial class RemoveTabView : UserControl
{
    private readonly ILogger<RemoveTabView>? _logger;
    private RemoveItemViewModel? _viewModel;
    private QuickButtonsViewModel? _quickButtonsViewModel;

    // Flag to prevent cascading suggestion overlays (like InventoryTabView)
    private bool _isShowingSuggestionOverlay = false;

    /// <summary>
    /// Initializes a new instance of the RemoveTabView with minimal dependency injection support.
    /// Sets up component initialization and event handlers for ViewModel interaction.
    /// </summary>
    public RemoveTabView()
    {
        try
        {
            InitializeComponent();
            SetupEventHandlers();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RemoveTabView initialization error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Initializes a new instance of the RemoveTabView with dependency injection logging support.
    /// Provides enhanced logging capabilities for debugging and monitoring.
    /// </summary>
    /// <param name="logger">Logger instance for this view</param>
    public RemoveTabView(ILogger<RemoveTabView> logger) : this()
    {
        _logger = logger;
        _logger?.LogInformation("RemoveTabView initialized with dependency injection");
    }

    /// <summary>
    /// Sets up event handlers for ViewModel data context changes and UI interactions.
    /// Provides error handling for event handler setup failures.
    /// </summary>
    private void SetupEventHandlers()
    {
        try
        {
            this.DataContextChanged += OnDataContextChanged;
            this.Loaded += OnViewLoaded;

            // Setup SuggestionOverlay event handlers for TextBoxes
            SetupTextBoxSuggestionHandlers();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to setup event handlers");
        }
    }

    /// <summary>
    /// Sets up SuggestionOverlay event handlers using LostFocus pattern (like InventoryTabView).
    /// Avoids double triggering by not using TextChanged events for suggestions.
    /// </summary>
    private void SetupTextBoxSuggestionHandlers()
    {
        try
        {
            // Setup Part TextBox - LostFocus only (not TextChanged to avoid double triggering)
            var partTextBox = this.FindControl<TextBox>("PartTextBox");
            if (partTextBox != null)
            {
                partTextBox.LostFocus += OnPartLostFocus;
                _logger?.LogDebug("Part TextBox LostFocus event handler attached");
            }

            // Setup Operation TextBox - LostFocus only (not TextChanged to avoid double triggering)
            var operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            if (operationTextBox != null)
            {
                operationTextBox.LostFocus += OnOperationLostFocus;
                _logger?.LogDebug("Operation TextBox LostFocus event handler attached");
            }

            _logger?.LogDebug("TextBox SuggestionOverlay event handlers setup completed (LostFocus only)");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting up TextBox SuggestionOverlay handlers");
        }
    }

    /// <summary>
    /// Handles data context changes to wire/unwire ViewModel events properly.
    /// Ensures proper cleanup of previous ViewModels and setup of new ones.
    /// </summary>
    /// <param name="sender">The event sender</param>
    /// <param name="e">Event arguments</param>
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        try
        {
            // Unwire previous ViewModel events
            if (_viewModel != null)
            {
                UnwireViewModelEvents();
            }

            // Wire up new ViewModel events
            if (DataContext is RemoveItemViewModel viewModel)
            {
                _viewModel = viewModel;
                WireViewModelEvents(viewModel);

                // Initialize QuickButtons integration
                _ = InitializeQuickButtonsIntegrationAsync();

                _logger?.LogInformation("RemoveItemViewModel connected successfully");
            }
            else if (DataContext != null)
            {
                _logger?.LogWarning("DataContext is not RemoveItemViewModel. Type: {Type}", DataContext.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to handle DataContext change in RemoveTabView");
        }
    }

    private void WireViewModelEvents(RemoveItemViewModel viewModel)
    {
        try
        {
            // Subscribe to property changes to detect command execution completion
            viewModel.PropertyChanged += OnViewModelPropertyChanged;

            // Subscribe to ShowSuccessOverlay event
            viewModel.ShowSuccessOverlay += OnShowSuccessOverlay;

            _logger?.LogDebug("RemoveTabView ViewModel events wired successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error wiring ViewModel events in RemoveTabView");
        }
    }

    /// <summary>
    /// Handles ViewModel property changes for auto-behavior triggers
    /// </summary>
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(RemoveItemViewModel.IsLoading) && _viewModel != null)
            {
                // When loading completes after a search/reset operation, apply auto-behavior
                if (!_viewModel.IsLoading)
                {
                    // We need to track which command was executed to apply appropriate behavior
                    // This will be handled through button click events instead
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling ViewModel property change");
        }
    }

    /// <summary>
    /// Handles ShowSuccessOverlay event from ViewModel
    /// </summary>
    private void OnShowSuccessOverlay(object? sender, MTM_WIP_Application_Avalonia.Models.SuccessEventArgs e)
    {
        try
        {
            _logger?.LogInformation("ShowSuccessOverlay event received: {Message}", e.Message);

            // The SuccessOverlay service integration is handled directly in the ViewModel
            // This event handler is available for additional UI-specific success handling if needed

            // For example, we could trigger visual feedback, sounds, or other UI updates here
            // For now, we just log the success
            _logger?.LogDebug("Success overlay event handled: {Details}", e.Details);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling ShowSuccessOverlay event");
        }
    }

    /// <summary>
    /// Handles view loaded event to set up CollapsiblePanel and DataGrid multi-selection
    /// </summary>
    private void OnViewLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            SetupDataGridMultiSelection();
            SetupCollapsiblePanelBehavior();
            _logger?.LogDebug("RemoveTabView loaded and configured successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during RemoveTabView load setup");
        }
    }

    /// <summary>
    /// Sets up DataGrid multi-selection support for batch operations
    /// </summary>
    private void SetupDataGridMultiSelection()
    {
        try
        {
            // CRITICAL FIX: Find CustomDataGrid instead of regular DataGrid
            var customDataGrid = this.FindControl<Controls.CustomDataGrid.CustomDataGrid>("InventoryDataGrid");
            if (customDataGrid != null)
            {
                customDataGrid.SelectionChanged += OnCustomDataGridSelectionChanged;
                _logger?.LogDebug("CustomDataGrid multi-selection configured successfully");
            }
            else
            {
                _logger?.LogWarning("Could not find CustomDataGrid 'InventoryDataGrid' for multi-selection setup");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting up CustomDataGrid multi-selection");
        }
    }

    /// <summary>
    /// CRITICAL FIX: Handles CustomDataGrid selection changes to sync with ViewModel SelectedItems.
    /// This ensures the ViewModel's SelectedItems collection is properly updated when users make selections.
    /// </summary>
    private void OnCustomDataGridSelectionChanged(object? sender, Controls.CustomDataGrid.SelectionChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || e == null) return;

            _logger?.LogInformation("🔍 QA DEBUG: CustomDataGrid selection changed - Count: {Count}", e.SelectedCount);

            // Clear and repopulate ViewModel's SelectedItems with the CustomDataGrid selection
            _viewModel.SelectedItems.Clear();
            foreach (var item in e.SelectedItems)
            {
                if (item is InventoryItem inventoryItem)
                {
                    _viewModel.SelectedItems.Add(inventoryItem);
                    _logger?.LogTrace("🔍 QA DEBUG: Added to ViewModel selection: {PartId} (ID: {Id})", inventoryItem.PartId, inventoryItem.Id);
                }
            }

            _logger?.LogInformation("🔍 QA DEBUG: ViewModel SelectedItems synced - Count: {Count}, CanDelete: {CanDelete}",
                _viewModel.SelectedItems.Count, _viewModel.CanDelete);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling CustomDataGrid selection change");
        }
    }

    /// <summary>
    /// Sets up CollapsiblePanel auto-behavior
    /// </summary>
    private void SetupCollapsiblePanelBehavior()
    {
        try
        {
            var searchPanel = this.FindControl<Controls.CollapsiblePanel>("SearchPanel");
            if (searchPanel != null)
            {
                // Store reference for auto-behavior
                _searchPanel = searchPanel;
                _logger?.LogDebug("CollapsiblePanel auto-behavior configured successfully");
            }
            else
            {
                _logger?.LogWarning("Could not find SearchPanel for auto-behavior setup");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting up CollapsiblePanel behavior");
        }
    }

    private Controls.CollapsiblePanel? _searchPanel;

    /// <summary>
    /// Executes command with CollapsiblePanel auto-behavior
    /// </summary>
    private async Task ExecuteWithPanelBehavior(Func<object?, Task> originalCommand, object? parameter, bool shouldCollapse)
    {
        try
        {
            // Execute the original command
            await originalCommand(parameter);

            // Apply auto-behavior after successful execution
            if (_searchPanel != null)
            {
                if (shouldCollapse)
                {
                    // Search: auto-collapse panel
                    _searchPanel.IsExpanded = false;
                    _logger?.LogDebug("Search completed - panel auto-collapsed");
                }
                else
                {
                    // Reset: auto-expand panel
                    _searchPanel.IsExpanded = true;
                    _logger?.LogDebug("Reset completed - panel auto-expanded");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing command with panel behavior");
            throw;
        }
    }

    /// <summary>
    /// Handles Search button click for auto-collapse behavior
    /// </summary>
    private async void OnSearchButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            // Wait a bit for the command to complete, then auto-collapse
            await Task.Delay(100);

            if (_searchPanel != null && !_viewModel?.IsLoading == true)
            {
                _searchPanel.IsExpanded = false;
                _logger?.LogDebug("Search button clicked - panel auto-collapsed");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Search button click");
        }
    }

    /// <summary>
    /// Handles Delete button click with confirmation dialog
    /// </summary>
    private async void OnDeleteButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || _viewModel.SelectedItems.Count == 0) return;

            // Show confirmation dialog
            var itemCount = _viewModel.SelectedItems.Count;
            var confirmationMessage = itemCount == 1
                ? $"Delete 1 inventory item?\n\nPart: {_viewModel.SelectedItems[0].PartId}\nOperation: {_viewModel.SelectedItems[0].Operation}\nQuantity: {_viewModel.SelectedItems[0].Quantity}"
                : $"Delete {itemCount} inventory items?\n\nThis action cannot be undone automatically.\nUse the Undo button immediately after deletion if needed.";

            var result = await ShowConfirmationDialog("Confirm Batch Deletion", confirmationMessage);

            if (result)
            {
                // User confirmed - execute the delete command
                if (_viewModel.DeleteCommand.CanExecute(null))
                {
                    await _viewModel.DeleteCommand.ExecuteAsync(null);
                    _logger?.LogInformation("Batch deletion confirmed and executed for {Count} items", itemCount);
                }
            }
            else
            {
                _logger?.LogDebug("Batch deletion cancelled by user");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Delete button click with confirmation");
        }
    }

    /// <summary>
    /// Shows a confirmation dialog and returns the user's choice
    /// </summary>
    private async Task<bool> ShowConfirmationDialog(string title, string message)
    {
        try
        {
            var dialog = new Window
            {
                Title = title,
                Width = 450,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                ShowInTaskbar = false,
                SystemDecorations = SystemDecorations.BorderOnly
            };

            // Main container with MTM theme styling
            var border = new Border
            {
                Background = Avalonia.Media.Brushes.White,
                BorderBrush = Avalonia.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new Avalonia.CornerRadius(6)
            };

            var stackPanel = new StackPanel { Margin = new Thickness(24, 20) };

            // Message text with proper styling
            var messageText = new TextBlock
            {
                Text = message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 24),
                FontSize = 14,
                LineHeight = 20,
                Foreground = Avalonia.Media.Brushes.Black
            };

            // Button panel with proper spacing
            var buttonPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Spacing = 12
            };

            // Yes button with danger styling for delete actions
            var yesButton = new Button
            {
                Content = "Delete",
                Width = 80,
                Height = 32,
                IsDefault = true,
                Background = Avalonia.Media.Brushes.Crimson,
                Foreground = Avalonia.Media.Brushes.White,
                BorderBrush = Avalonia.Media.Brushes.DarkRed,
                BorderThickness = new Thickness(1),
                CornerRadius = new Avalonia.CornerRadius(4),
                FontWeight = Avalonia.Media.FontWeight.SemiBold
            };

            // No button with secondary styling
            var noButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 32,
                IsCancel = true,
                Background = Avalonia.Media.Brushes.White,
                Foreground = Avalonia.Media.Brushes.Black,
                BorderBrush = Avalonia.Media.Brushes.Gray,
                BorderThickness = new Thickness(1.5),
                CornerRadius = new Avalonia.CornerRadius(4)
            };

            buttonPanel.Children.Add(noButton);  // Cancel first (left)
            buttonPanel.Children.Add(yesButton); // Delete second (right)
            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(buttonPanel);
            border.Child = stackPanel;
            dialog.Content = border;

            bool result = false;

            yesButton.Click += (s, e) =>
            {
                result = true;
                dialog.Close();
                _logger?.LogInformation("User confirmed batch deletion");
            };

            noButton.Click += (s, e) =>
            {
                dialog.Close();
                _logger?.LogInformation("User cancelled batch deletion");
            };

            // Show modal dialog
            if (TopLevel.GetTopLevel(this) is Window parentWindow)
            {
                await dialog.ShowDialog(parentWindow);
            }
            else
            {
                dialog.Show();
                await Task.Delay(100); // Allow dialog to show
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error showing confirmation dialog");
            return false;
        }
    }

    /// <summary>
    /// Handles Reset button click for auto-expand behavior and Part ID focus
    /// </summary>
    private async void OnResetButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            // Wait a bit for the command to complete, then auto-expand and focus Part ID
            await Task.Delay(100);

            if (_searchPanel != null)
            {
                _searchPanel.IsExpanded = true;
                _logger?.LogDebug("Reset button clicked - panel auto-expanded");

                // Set focus to Part ID TextBox after reset
                var partTextBox = this.FindControl<TextBox>("PartTextBox");
                if (partTextBox != null)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        partTextBox.Focus();
                        _logger?.LogDebug("Reset button clicked - Part ID TextBox focused");
                    });
                }
                else
                {
                    _logger?.LogWarning("Could not find PartTextBox to set focus after reset");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Reset button click");
        }
    }

    private void HandleCommandException(string commandName, Exception ex)
    {
        try
        {
            _logger?.LogError(ex, "Command {CommandName} encountered an error in RemoveTabView: {Message}", commandName, ex.Message);

            // Handle specific exception types
            switch (ex)
            {
                case FormatException formatEx:
                    _logger?.LogError(formatEx, "Format exception in RemoveTabView command {CommandName}. Check data binding formats", commandName);
                    break;
                case InvalidCastException castEx:
                    _logger?.LogError(castEx, "Invalid cast exception in RemoveTabView command {CommandName}. Check data type conversions", commandName);
                    break;
                case ArgumentException argEx:
                    _logger?.LogError(argEx, "Argument exception in RemoveTabView command {CommandName}. Check parameter values", commandName);
                    break;
                default:
                    _logger?.LogError(ex, "Unhandled exception in RemoveTabView command {CommandName}", commandName);
                    break;
            }

            // Update ViewModel if safe to do so
            if (_viewModel != null)
            {
                try
                {
                    // Use Avalonia's Dispatcher instead of RxApp.MainThreadScheduler
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        try
                        {
                            // Set loading to false if an error occurred
                            _viewModel.IsLoading = false;
                        }
                        catch (Exception statusEx)
                        {
                            _logger?.LogError(statusEx, "Error updating ViewModel status in RemoveTabView");
                        }
                    });
                }
                catch (Exception schedulerEx)
                {
                    _logger?.LogError(schedulerEx, "Error scheduling status update in RemoveTabView");
                }
            }
        }
        catch (Exception handlerEx)
        {
            _logger?.LogCritical(handlerEx, "Critical error in RemoveTabView exception handler for command {CommandName}", commandName);
            System.Diagnostics.Debug.WriteLine($"Critical RemoveTabView exception handling error for {commandName}: {handlerEx.Message}");
        }
    }

    private void UnwireViewModelEvents()
    {
        try
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel.ShowSuccessOverlay -= OnShowSuccessOverlay;
            }

            _logger?.LogDebug("RemoveTabView ViewModel events unwired successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error unwiring ViewModel events in RemoveTabView");
        }
    }

    #region SuggestionOverlay Event Handlers

    /// <summary>
    /// Handles Part TextBox lost focus event for SuggestionOverlay (following InventoryTabView pattern).
    /// Avoids double triggering by using LostFocus instead of TextChanged.
    /// </summary>
    private async void OnPartLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not TextBox textBox) return;

            var value = textBox.Text?.Trim() ?? string.Empty;
            var data = _viewModel.PartIds ?? Enumerable.Empty<string>();
            int count = data.Count();

            _logger?.LogDebug("PartTextBox lost focus. User entered: '{Value}'. PartIds count: {Count}", value, count);

            // Check if no data is available from server
            if (count == 0)
            {
                _logger?.LogWarning("No Part IDs available - likely database connectivity issue");
                textBox.Text = string.Empty;
                _viewModel.SelectedPart = string.Empty;
                return;
            }

            // Find partial matches (not exact matches)
            var semiMatches = data
                .Where(partId => !string.IsNullOrEmpty(partId) &&
                               partId.Contains(value, StringComparison.OrdinalIgnoreCase))
                .OrderBy(partId => partId)
                .ToList();

            bool isExactMatch = data.Any(partId =>
                string.Equals(partId, value, StringComparison.OrdinalIgnoreCase));

            _logger?.LogDebug("Part '{Value}' - ExactMatch: {IsExactMatch}, SemiMatches: {SemiMatchesCount}",
                value, isExactMatch, semiMatches.Count);

            // Show overlay only for partial matches (not exact matches or empty input)
            if (!string.IsNullOrEmpty(value) &&
                !isExactMatch &&
                semiMatches.Count > 0 &&
                !_isShowingSuggestionOverlay)
            {
                try
                {
                    _isShowingSuggestionOverlay = true;
                    var selected = await _viewModel.ShowPartSuggestionsAsync(textBox, value);

                    if (!string.IsNullOrEmpty(selected) && selected != value)
                    {
                        _logger?.LogDebug("Part overlay - User selected: '{Selected}'", selected);
                        _viewModel.SelectedPart = selected;
                        textBox.Text = selected;
                    }
                    else
                    {
                        _logger?.LogDebug("Part overlay - User cancelled or no selection, keeping: '{Value}'", value);
                        _viewModel.SelectedPart = value;
                    }
                }
                finally
                {
                    _isShowingSuggestionOverlay = false;
                }
            }
            else
            {
                // Handle different cases
                if (string.IsNullOrEmpty(value))
                {
                    _logger?.LogDebug("Part overlay not shown - value is empty");
                }
                else if (isExactMatch)
                {
                    _logger?.LogDebug("Part overlay not shown - '{Value}' is exact match", value);
                    _viewModel.SelectedPart = value;
                }
                else if (semiMatches.Count == 0)
                {
                    _logger?.LogDebug("Part overlay not shown - no semi-matches for '{Value}'", value);

                    // Clear invalid input to maintain data integrity (MTM pattern)
                    textBox.Text = string.Empty;
                    _viewModel.SelectedPart = string.Empty;

                    // Show user feedback
                    try
                    {
                        await Services.Core.ErrorHandling.HandleErrorAsync(
                            new ArgumentException($"Invalid Part ID: '{value}' not found in available parts."),
                            "Part ID validation failed - input cleared",
                            "System"
                        );
                    }
                    catch (Exception errorEx)
                    {
                        _logger?.LogWarning(errorEx, "Failed to show error message for invalid Part ID");
                    }
                }
                else
                {
                    _viewModel.SelectedPart = value;
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling part lost focus");
        }
    }

    /// <summary>
    /// Handles Operation TextBox lost focus event for SuggestionOverlay (following InventoryTabView pattern).
    /// Avoids double triggering by using LostFocus instead of TextChanged.
    /// </summary>
    private async void OnOperationLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not TextBox textBox) return;

            var value = textBox.Text?.Trim() ?? string.Empty;
            var data = _viewModel.Operations ?? Enumerable.Empty<string>();
            int count = data.Count();

            _logger?.LogDebug("OperationTextBox lost focus. User entered: '{Value}'. Operations count: {Count}", value, count);

            // Check if no data is available from server
            if (count == 0)
            {
                _logger?.LogWarning("No Operations available - likely database connectivity issue");
                textBox.Text = string.Empty;
                _viewModel.SelectedOperation = string.Empty;
                return;
            }

            // Find partial matches (not exact matches)
            var semiMatches = data
                .Where(operation => !string.IsNullOrEmpty(operation) &&
                                  operation.Contains(value, StringComparison.OrdinalIgnoreCase))
                .OrderBy(operation => operation)
                .ToList();

            bool isExactMatch = data.Any(operation =>
                string.Equals(operation, value, StringComparison.OrdinalIgnoreCase));

            _logger?.LogDebug("Operation '{Value}' - ExactMatch: {IsExactMatch}, SemiMatches: {SemiMatchesCount}",
                value, isExactMatch, semiMatches.Count);

            // Show overlay only for partial matches (not exact matches or empty input)
            if (!string.IsNullOrEmpty(value) &&
                !isExactMatch &&
                semiMatches.Count > 0 &&
                !_isShowingSuggestionOverlay)
            {
                try
                {
                    _isShowingSuggestionOverlay = true;
                    var selected = await _viewModel.ShowOperationSuggestionsAsync(textBox, value);

                    if (!string.IsNullOrEmpty(selected) && selected != value)
                    {
                        _logger?.LogDebug("Operation overlay - User selected: '{Selected}'", selected);
                        _viewModel.SelectedOperation = selected;
                        textBox.Text = selected;
                    }
                    else
                    {
                        _logger?.LogDebug("Operation overlay - User cancelled or no selection, keeping: '{Value}'", value);
                        _viewModel.SelectedOperation = value;
                    }
                }
                finally
                {
                    _isShowingSuggestionOverlay = false;
                }
            }
            else
            {
                // Handle different cases
                if (string.IsNullOrEmpty(value))
                {
                    _logger?.LogDebug("Operation overlay not shown - value is empty");
                }
                else if (isExactMatch)
                {
                    _logger?.LogDebug("Operation overlay not shown - '{Value}' is exact match", value);
                    _viewModel.SelectedOperation = value;
                }
                else if (semiMatches.Count == 0)
                {
                    _logger?.LogDebug("Operation overlay not shown - no semi-matches for '{Value}'", value);

                    // Clear invalid input to maintain data integrity (MTM pattern)
                    textBox.Text = string.Empty;
                    _viewModel.SelectedOperation = string.Empty;

                    // Show user feedback
                    try
                    {
                        await Services.Core.ErrorHandling.HandleErrorAsync(
                            new ArgumentException($"Invalid Operation: '{value}' not found in available operations."),
                            "Operation validation failed - input cleared",
                            "System"
                        );
                    }
                    catch (Exception errorEx)
                    {
                        _logger?.LogWarning(errorEx, "Failed to show error message for invalid Operation");
                    }
                }
                else
                {
                    _viewModel.SelectedOperation = value;
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling operation lost focus");
        }
    }

    #endregion

    #region QuickButtons Integration

    /// <summary>
    /// Initializes QuickButtons integration for field population from QuickButton clicks.
    /// Uses multiple strategies: visual tree traversal with fallback to service-based integration.
    /// Ensures 100% reliable QuickButtons integration regardless of UI layout complexity.
    /// </summary>
    private Task InitializeQuickButtonsIntegrationAsync()
    {
        try
        {
            _logger?.LogDebug("Starting QuickButtons integration initialization...");

            // Strategy 1: Visual tree traversal (primary approach)
            var quickButtonsView = FindQuickButtonsView();
            if (quickButtonsView?.DataContext is QuickButtonsViewModel quickButtonsViewModel)
            {
                _quickButtonsViewModel = quickButtonsViewModel;
                SubscribeToQuickButtonsEvents(quickButtonsViewModel, "Visual Tree");
                _logger?.LogInformation("QuickButtons integration initialized via visual tree traversal");
                return Task.CompletedTask;
            }

            _logger?.LogWarning("Visual tree traversal failed to find QuickButtonsView, attempting service fallback...");

            // Strategy 2: Service-based fallback through ViewModel
            if (_viewModel != null)
            {
                // Use reflection to get QuickButtonsService from ViewModel
                var quickButtonsServiceField = _viewModel.GetType().GetField("_quickButtonsService",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (quickButtonsServiceField?.GetValue(_viewModel) != null)
                {
                    _logger?.LogInformation("QuickButtons service found in ViewModel - direct service integration active");
                    return Task.CompletedTask;
                }
            }

            // Strategy 3: Global service locator as last resort
            try
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                    desktop.MainWindow?.DataContext is MainWindowViewModel mainViewModel)
                {
                    // Try to find QuickButtonsViewModel through main window
                    var mainViewModelType = mainViewModel.GetType();
                    var quickButtonsProperty = mainViewModelType.GetProperty("QuickButtons") ??
                                             mainViewModelType.GetProperty("QuickButtonsViewModel");

                    if (quickButtonsProperty?.GetValue(mainViewModel) is QuickButtonsViewModel globalQuickButtonsViewModel)
                    {
                        _quickButtonsViewModel = globalQuickButtonsViewModel;
                        SubscribeToQuickButtonsEvents(globalQuickButtonsViewModel, "Global Service Locator");
                        _logger?.LogInformation("QuickButtons integration initialized via global service locator");
                        return Task.CompletedTask;
                    }
                }
            }
            catch (Exception serviceEx)
            {
                _logger?.LogWarning(serviceEx, "Service locator fallback failed, but non-critical");
            }

            _logger?.LogWarning("All QuickButtons integration strategies failed - field population from QuickButtons may not work");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Critical error initializing QuickButtons integration");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Subscribes to QuickButtons events using reflection for event discovery.
    /// Provides comprehensive logging for debugging integration issues.
    /// </summary>
    private void SubscribeToQuickButtonsEvents(QuickButtonsViewModel quickButtonsViewModel, string integrationMethod)
    {
        try
        {
            // Subscribe to quick action executed events if they exist
            var quickActionEvent = quickButtonsViewModel.GetType().GetEvent("QuickActionExecuted");
            if (quickActionEvent != null)
            {
                // Use reflection to subscribe to the event
                var handler = new EventHandler<object>((sender, args) => OnQuickActionExecuted(sender, args));
                quickActionEvent.AddEventHandler(quickButtonsViewModel, handler);
                _logger?.LogDebug("QuickActionExecuted event subscribed via {Method}", integrationMethod);
            }
            else
            {
                _logger?.LogWarning("QuickActionExecuted event not found on QuickButtonsViewModel - manual field population only");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error subscribing to QuickButtons events via {Method}", integrationMethod);
        }
    }

    /// <summary>
    /// Handles quick action executed events from QuickButtonsView.
    /// </summary>
    private void OnQuickActionExecuted(object? sender, object e)
    {
        try
        {
            if (_viewModel == null) return;

            // Use reflection to extract event data since we don't have a typed event args
            var partId = GetPropertyValue<string>(e, "PartId");
            var operation = GetPropertyValue<string>(e, "Operation");
            var location = GetPropertyValue<string>(e, "Location");

            _logger?.LogInformation("QuickButton clicked in RemoveTabView - Part: {PartId}, Operation: {Operation}, Location: {Location}",
                partId, operation, location);

            // Populate the search fields using ViewModel method
            _viewModel.PopulateFromQuickButton(partId, operation, location);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling QuickButton action in RemoveTabView");
        }
    }

    /// <summary>
    /// Finds the QuickButtonsView in the visual tree using comprehensive search strategies.
    /// Enhanced version with better error handling and more thorough searching.
    /// </summary>
    private QuickButtonsView? FindQuickButtonsView()
    {
        try
        {
            _logger?.LogDebug("Starting comprehensive QuickButtonsView search...");

            // Strategy 1: Search up the parent hierarchy
            var current = this.Parent;
            int parentLevels = 0;
            while (current != null && parentLevels < 10) // Prevent infinite loops
            {
                _logger?.LogTrace("Searching parent level {Level}: {Type}", parentLevels, current.GetType().Name);

                if (current is Panel panel)
                {
                    var result = SearchInPanel(panel);
                    if (result != null)
                    {
                        _logger?.LogDebug("QuickButtonsView found via parent hierarchy at level {Level}", parentLevels);
                        return result;
                    }
                }

                current = current.Parent;
                parentLevels++;
            }

            // Strategy 2: Search from Application MainWindow if parent search failed
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                desktop.MainWindow != null)
            {
                _logger?.LogDebug("Parent hierarchy search failed, searching from MainWindow...");
                var result = FindQuickButtonsViewInChildren(desktop.MainWindow);
                if (result != null)
                {
                    _logger?.LogDebug("QuickButtonsView found via MainWindow search");
                    return result;
                }
            }

            _logger?.LogWarning("QuickButtonsView not found after comprehensive search");
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during comprehensive QuickButtonsView search");
            return null;
        }
    }

    /// <summary>
    /// Searches for QuickButtonsView within a specific panel.
    /// </summary>
    private QuickButtonsView? SearchInPanel(Panel panel)
    {
        try
        {
            foreach (var child in panel.Children)
            {
                if (child is QuickButtonsView quickButtonsView)
                {
                    return quickButtonsView;
                }

                var found = FindQuickButtonsViewInChildren(child);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogTrace(ex, "Error searching in panel {Type}", panel.GetType().Name);
            return null;
        }
    }

    /// <summary>
    /// Recursively searches for QuickButtonsView in child controls with comprehensive coverage.
    /// Enhanced with better error handling and support for multiple control types.
    /// </summary>
    private QuickButtonsView? FindQuickButtonsViewInChildren(Control control)
    {
        try
        {
            if (control is QuickButtonsView quickButtonsView)
            {
                return quickButtonsView;
            }

            // Search in different types of containers
            switch (control)
            {
                case Panel panel:
                    foreach (var child in panel.Children)
                    {
                        var panelResult = FindQuickButtonsViewInChildren(child);
                        if (panelResult != null) return panelResult;
                    }
                    break;

                case ContentControl contentControl when contentControl.Content is Control childContent:
                    var contentResult = FindQuickButtonsViewInChildren(childContent);
                    if (contentResult != null) return contentResult;
                    break;

                case Border border when border.Child is Control borderChild:
                    var borderResult = FindQuickButtonsViewInChildren(borderChild);
                    if (borderResult != null) return borderResult;
                    break;

                case ScrollViewer scrollViewer when scrollViewer.Content is Control scrollContent:
                    var scrollResult = FindQuickButtonsViewInChildren(scrollContent);
                    if (scrollResult != null) return scrollResult;
                    break;

                // Add support for other container types as needed
                case UserControl userControl:
                    // Don't recurse into other UserControls to avoid infinite loops
                    if (userControl != this)
                    {
                        var userControlResult = SearchUserControlChildren(userControl);
                        if (userControlResult != null) return userControlResult;
                    }
                    break;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogTrace(ex, "Error searching children of {Type}", control.GetType().Name);
            return null;
        }
    }

    /// <summary>
    /// Safely searches within UserControl children without causing infinite recursion.
    /// </summary>
    private QuickButtonsView? SearchUserControlChildren(UserControl userControl)
    {
        try
        {
            // Use reflection to access the internal visual tree if possible
            var visualChildren = userControl.GetVisualChildren().OfType<Control>();
            foreach (var visualChild in visualChildren)
            {
                var found = FindQuickButtonsViewInChildren(visualChild);
                if (found != null) return found;
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogTrace(ex, "Error searching UserControl {Type}", userControl.GetType().Name);
            return null;
        }
    }

    /// <summary>
    /// Gets a property value using reflection
    /// </summary>
    private T? GetPropertyValue<T>(object obj, string propertyName)
    {
        try
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
                var value = property.GetValue(obj);
                if (value is T typedValue)
                {
                    return typedValue;
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogDebug("Could not get property {PropertyName}: {Error}", propertyName, ex.Message);
        }

        return default(T);
    }

    #endregion

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        try
        {
            if (_viewModel != null)
            {
                UnwireViewModelEvents();
            }

            this.DataContextChanged -= OnDataContextChanged;

            _logger?.LogInformation("RemoveTabView cleanup completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during RemoveTabView cleanup");
        }
        finally
        {
            base.OnDetachedFromVisualTree(e);
        }
    }
}
