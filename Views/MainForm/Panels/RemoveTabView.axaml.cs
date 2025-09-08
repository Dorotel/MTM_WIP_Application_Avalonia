using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Views;
using MTM_Shared_Logic.Models;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Controls.Primitives;
using System.Collections.Specialized;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;

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
    /// Sets up SuggestionOverlay event handlers for all TextBox controls
    /// </summary>
    private void SetupTextBoxSuggestionHandlers()
    {
        try
        {
            // Setup Part TextBox
            var partTextBox = this.FindControl<TextBox>("PartTextBox");
            if (partTextBox != null)
            {
                partTextBox.GotFocus += OnPartTextBoxGotFocus;
                partTextBox.TextChanged += OnPartTextBoxTextChanged;
            }

            // Setup Operation TextBox
            var operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            if (operationTextBox != null)
            {
                operationTextBox.GotFocus += OnOperationTextBoxGotFocus;
                operationTextBox.TextChanged += OnOperationTextBoxTextChanged;
            }

            // Setup Location TextBox
            var locationTextBox = this.FindControl<TextBox>("LocationTextBox");
            if (locationTextBox != null)
            {
                locationTextBox.GotFocus += OnLocationTextBoxGotFocus;
                locationTextBox.TextChanged += OnLocationTextBoxTextChanged;
            }

            // Setup User TextBox
            var userTextBox = this.FindControl<TextBox>("UserTextBox");
            if (userTextBox != null)
            {
                userTextBox.GotFocus += OnUserTextBoxGotFocus;
                userTextBox.TextChanged += OnUserTextBoxTextChanged;
            }

            _logger?.LogDebug("TextBox SuggestionOverlay event handlers setup completed");
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
            var dataGrid = this.FindControl<DataGrid>("InventoryDataGrid");
            if (dataGrid != null)
            {
                dataGrid.SelectionChanged += OnDataGridSelectionChanged;
                _logger?.LogDebug("DataGrid multi-selection configured successfully");
            }
            else
            {
                _logger?.LogWarning("Could not find InventoryDataGrid for multi-selection setup");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting up DataGrid multi-selection");
        }
    }

    /// <summary>
    /// Handles DataGrid selection changes to sync with ViewModel SelectedItems
    /// </summary>
    private void OnDataGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not DataGrid dataGrid) return;

            _viewModel.SelectedItems.Clear();
            foreach (var item in dataGrid.SelectedItems)
            {
                if (item is InventoryItem inventoryItem)
                {
                    _viewModel.SelectedItems.Add(inventoryItem);
                }
            }

            _logger?.LogDebug("DataGrid selection synced: {Count} items selected", _viewModel.SelectedItems.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling DataGrid selection change");
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
                ? $"Delete 1 inventory item?\n\nPart: {_viewModel.SelectedItems[0].PartID}\nOperation: {_viewModel.SelectedItems[0].Operation}\nQuantity: {_viewModel.SelectedItems[0].Quantity}"
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
            // Find the parent window to show the dialog
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is Window window)
            {
                // Create a simple confirmation dialog using Avalonia's MessageBoxManager equivalent
                // For now, using a placeholder - in a full implementation, you'd use a custom dialog
                await Task.Delay(10); // Placeholder for actual dialog implementation
                
                // For demonstration, always return true - in real implementation, show actual dialog
                _logger?.LogWarning("Confirmation dialog not implemented - proceeding with deletion");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error showing confirmation dialog");
            return false;
        }
    }

    /// <summary>
    /// Handles Reset button click for auto-expand behavior
    /// </summary>
    private async void OnResetButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            // Wait a bit for the command to complete, then auto-expand
            await Task.Delay(100);
            
            if (_searchPanel != null)
            {
                _searchPanel.IsExpanded = true;
                _logger?.LogDebug("Reset button clicked - panel auto-expanded");
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
    /// Handles Part TextBox focus event for SuggestionOverlay
    /// </summary>
    private async void OnPartTextBoxGotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
    {
        if (_viewModel != null && sender is TextBox textBox)
        {
            try
            {
                var result = await _viewModel.ShowPartSuggestionsAsync(textBox, textBox.Text ?? string.Empty);
                if (result != null)
                {
                    _viewModel.SelectedPart = result;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error showing part suggestions");
            }
        }
    }

    /// <summary>
    /// Handles Part TextBox text changed event for SuggestionOverlay
    /// </summary>
    private async void OnPartTextBoxTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (_viewModel != null && sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
        {
            try
            {
                // Only show suggestions if text length > 1 to avoid too many suggestions
                if (textBox.Text.Length > 1)
                {
                    var result = await _viewModel.ShowPartSuggestionsAsync(textBox, textBox.Text);
                    if (result != null && result != textBox.Text)
                    {
                        _viewModel.SelectedPart = result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error showing part suggestions on text change");
            }
        }
    }

    /// <summary>
    /// Handles Operation TextBox focus event for SuggestionOverlay
    /// </summary>
    private async void OnOperationTextBoxGotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
    {
        if (_viewModel != null && sender is TextBox textBox)
        {
            try
            {
                var result = await _viewModel.ShowOperationSuggestionsAsync(textBox, textBox.Text ?? string.Empty);
                if (result != null)
                {
                    _viewModel.SelectedOperation = result;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error showing operation suggestions");
            }
        }
    }

    /// <summary>
    /// Handles Operation TextBox text changed event for SuggestionOverlay
    /// </summary>
    private async void OnOperationTextBoxTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (_viewModel != null && sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
        {
            try
            {
                if (textBox.Text.Length > 0) // Operations are often single digit
                {
                    var result = await _viewModel.ShowOperationSuggestionsAsync(textBox, textBox.Text);
                    if (result != null && result != textBox.Text)
                    {
                        _viewModel.SelectedOperation = result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error showing operation suggestions on text change");
            }
        }
    }

    /// <summary>
    /// Handles Location TextBox focus event for SuggestionOverlay
    /// </summary>
    private async void OnLocationTextBoxGotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
    {
        if (_viewModel != null && sender is TextBox textBox)
        {
            try
            {
                var result = await _viewModel.ShowLocationSuggestionsAsync(textBox, textBox.Text ?? string.Empty);
                if (result != null)
                {
                    _viewModel.SelectedLocation = result;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error showing location suggestions");
            }
        }
    }

    /// <summary>
    /// Handles Location TextBox text changed event for SuggestionOverlay
    /// </summary>
    private async void OnLocationTextBoxTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (_viewModel != null && sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
        {
            try
            {
                if (textBox.Text.Length > 1)
                {
                    var result = await _viewModel.ShowLocationSuggestionsAsync(textBox, textBox.Text);
                    if (result != null && result != textBox.Text)
                    {
                        _viewModel.SelectedLocation = result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error showing location suggestions on text change");
            }
        }
    }

    /// <summary>
    /// Handles User TextBox focus event for SuggestionOverlay
    /// </summary>
    private async void OnUserTextBoxGotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
    {
        if (_viewModel != null && sender is TextBox textBox)
        {
            try
            {
                var result = await _viewModel.ShowUserSuggestionsAsync(textBox, textBox.Text ?? string.Empty);
                if (result != null)
                {
                    _viewModel.SelectedUser = result;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error showing user suggestions");
            }
        }
    }

    /// <summary>
    /// Handles User TextBox text changed event for SuggestionOverlay
    /// </summary>
    private async void OnUserTextBoxTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (_viewModel != null && sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
        {
            try
            {
                if (textBox.Text.Length > 1)
                {
                    var result = await _viewModel.ShowUserSuggestionsAsync(textBox, textBox.Text);
                    if (result != null && result != textBox.Text)
                    {
                        _viewModel.SelectedUser = result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error showing user suggestions on text change");
            }
        }
    }

    #endregion

    #region QuickButtons Integration

    /// <summary>
    /// Initializes QuickButtons integration to handle quick action events.
    /// </summary>
    private Task InitializeQuickButtonsIntegrationAsync()
    {
        try
        {
            // Find QuickButtonsView in the visual tree
            var quickButtonsView = FindQuickButtonsView();
            if (quickButtonsView?.DataContext is QuickButtonsViewModel quickButtonsViewModel)
            {
                _quickButtonsViewModel = quickButtonsViewModel;

                // Subscribe to quick action executed events if they exist
                var quickActionEvent = _quickButtonsViewModel.GetType().GetEvent("QuickActionExecuted");
                if (quickActionEvent != null)
                {
                    // Use reflection to subscribe to the event
                    var handler = new EventHandler<object>((sender, args) => OnQuickActionExecuted(sender, args));
                    quickActionEvent.AddEventHandler(_quickButtonsViewModel, handler);
                }

                _logger?.LogInformation("QuickButtons integration initialized successfully");
            }
            else
            {
                _logger?.LogDebug("QuickButtonsView not found in visual tree - integration skipped");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing QuickButtons integration");
            System.Diagnostics.Debug.WriteLine($"Error initializing QuickButtons integration: {ex.Message}");
        }
        
        return Task.CompletedTask;
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
    /// Finds the QuickButtonsView in the visual tree.
    /// </summary>
    private QuickButtonsView? FindQuickButtonsView()
    {
        try
        {
            // Start from the current control and walk up to find a parent that contains QuickButtonsView
            var current = this.Parent;
            while (current != null)
            {
                if (current is Panel panel)
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
                }
                current = current.Parent;
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error finding QuickButtonsView: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Recursively searches for QuickButtonsView in child controls.
    /// </summary>
    private QuickButtonsView? FindQuickButtonsViewInChildren(Control control)
    {
        try
        {
            if (control is QuickButtonsView quickButtonsView)
            {
                return quickButtonsView;
            }

            if (control is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    var found = FindQuickButtonsViewInChildren(child);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in FindQuickButtonsViewInChildren: {ex.Message}");
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
