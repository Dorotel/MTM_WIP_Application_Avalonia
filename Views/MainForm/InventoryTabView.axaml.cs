using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for InventoryTabView.
/// Implements the primary inventory management interface within the MTM WIP Application.
/// Provides keyboard shortcuts, focus management, event handling, and database integration.
/// Uses standard .NET patterns without ReactiveUI dependencies.
/// </summary>
public partial class InventoryTabView : UserControl
{
    private InventoryTabViewModel? _viewModel;
    private QuickButtonsViewModel? _quickButtonsViewModel;
    private readonly IServiceProvider? _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the InventoryTabView.
    /// </summary>
    public InventoryTabView()
    {
        InitializeComponent();
        
        // Set up keyboard event handling
        KeyDown += OnKeyDown;
        
        // Set up loaded event to initialize ViewModel
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Initializes a new instance of the InventoryTabView with dependency injection support.
    /// </summary>
    public InventoryTabView(IServiceProvider serviceProvider) : this()
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Handles the Loaded event to set up the ViewModel via dependency injection.
    /// </summary>
    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Set up ViewModel if not already set
            if (DataContext is InventoryTabViewModel vm)
            {
                _viewModel = vm;
                
                // Subscribe to ViewModel events for database operations
                _viewModel.SaveCompleted += OnInventorySaveCompleted;
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                
                // Initialize database connections and load lookup data
                await InitializeDatabaseIntegrationAsync();
                
                System.Diagnostics.Debug.WriteLine("InventoryTabView ViewModel connected successfully");
            }

            // Initialize QuickButtons integration if available
            await InitializeQuickButtonsIntegrationAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't crash the UI
            System.Diagnostics.Debug.WriteLine($"Error setting up InventoryTabView ViewModel: {ex.Message}");
            
            // Use error handling service if available
            if (_serviceProvider?.GetService(typeof(ILogger<InventoryTabView>)) is ILogger<InventoryTabView> logger)
            {
                logger.LogError(ex, "Failed to initialize InventoryTabView");
            }
        }
    }

    /// <summary>
    /// Handles inventory save completion to update QuickButtons.
    /// </summary>
    private async void OnInventorySaveCompleted(object? sender, InventorySavedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Inventory saved: Part={e.PartId}, Operation={e.Operation}, Quantity={e.Quantity}");

            // Update QuickButtons after successful save
            if (_quickButtonsViewModel != null)
            {
                await _quickButtonsViewModel.AddQuickButtonFromOperationAsync(
                    e.PartId,
                    e.Operation,
                    e.Quantity);

                System.Diagnostics.Debug.WriteLine($"Added quick button: {e.PartId}, {e.Operation}, {e.Quantity}");
            }
            
            // Log the action for audit purposes
            if (_serviceProvider?.GetService(typeof(ILogger<InventoryTabView>)) is ILogger<InventoryTabView> logger)
            {
                logger.LogInformation("Inventory saved and added to QuickButtons: PartId={PartId}, Operation={Operation}, Quantity={Quantity}", 
                    e.PartId, e.Operation, e.Quantity);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling inventory save completion: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes database integration and loads initial lookup data.
    /// </summary>
    private async Task InitializeDatabaseIntegrationAsync()
    {
        try
        {
            if (_viewModel == null) return;

            // Load initial lookup data from database
            if (_viewModel.LoadDataCommand.CanExecute(null))
            {
                _viewModel.LoadDataCommand.Execute(null);
            }

            // Load application state from database if needed
            await LoadApplicationStateAsync();
            
            System.Diagnostics.Debug.WriteLine("Database integration initialized successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing database integration: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes QuickButtons integration to handle quick action events.
    /// </summary>
    private async Task InitializeQuickButtonsIntegrationAsync()
    {
        try
        {
            // Find QuickButtonsView in the visual tree (it might be in a parent or sibling container)
            var quickButtonsView = FindQuickButtonsView();
            if (quickButtonsView?.DataContext is QuickButtonsViewModel quickButtonsViewModel)
            {
                _quickButtonsViewModel = quickButtonsViewModel;
                
                // Subscribe to quick action executed events
                _quickButtonsViewModel.QuickActionExecuted += OnQuickActionExecuted;
                
                System.Diagnostics.Debug.WriteLine("QuickButtons integration initialized successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("QuickButtonsView not found in visual tree - integration skipped");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing QuickButtons integration: {ex.Message}");
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
                    // Search through child controls
                    foreach (var child in panel.Children)
                    {
                        if (child is QuickButtonsView quickButtonsView)
                        {
                            return quickButtonsView;
                        }
                        
                        // Recursively search in child panels
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
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Handles quick action executed events from QuickButtonsView.
    /// Populates the inventory form with the selected quick action data.
    /// </summary>
    private async void OnQuickActionExecuted(object? sender, QuickActionExecutedEventArgs e)
    {
        try
        {
            if (_viewModel == null) return;

            System.Diagnostics.Debug.WriteLine($"Quick action executed: Part={e.PartId}, Operation={e.Operation}, Quantity={e.Quantity}");

            // Populate form fields with quick action data
            _viewModel.SelectedPart = e.PartId;
            _viewModel.SelectedOperation = e.Operation;
            _viewModel.Quantity = e.Quantity;
            
            // Clear previous error state
            _viewModel.HasError = false;
            _viewModel.ErrorMessage = string.Empty;
            
            // Focus the first control for potential editing
            MoveFocusToFirstControl();
            
            // Log the action for audit purposes
            if (_serviceProvider?.GetService(typeof(ILogger<InventoryTabView>)) is ILogger<InventoryTabView> logger)
            {
                logger.LogInformation("Quick action applied: PartId={PartId}, Operation={Operation}, Quantity={Quantity}", 
                    e.PartId, e.Operation, e.Quantity);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling quick action: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles ViewModel property changes to respond to database operations.
    /// </summary>
    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(_viewModel.SelectedPart):
                    await OnPartChangedAsync();
                    break;
                    
                case nameof(_viewModel.SelectedOperation):
                    await OnOperationChangedAsync();
                    break;
                    
                case nameof(_viewModel.SelectedLocation):
                    await OnLocationChangedAsync();
                    break;
                    
                case nameof(_viewModel.IsLoading):
                    // Handle loading state changes if needed
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling property change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles part selection changes to update related data.
    /// </summary>
    private async Task OnPartChangedAsync()
    {
        try
        {
            if (_viewModel == null || string.IsNullOrEmpty(_viewModel.SelectedPart)) return;

            // Load part-specific data if needed (operations, default locations, etc.)
            await LoadPartSpecificDataAsync(_viewModel.SelectedPart);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling part change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles operation selection changes to update related data.
    /// </summary>
    private async Task OnOperationChangedAsync()
    {
        try
        {
            if (_viewModel == null || string.IsNullOrEmpty(_viewModel.SelectedOperation)) return;

            // Load operation-specific data if needed (default locations, work centers, etc.)
            await LoadOperationSpecificDataAsync(_viewModel.SelectedOperation);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling operation change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles location selection changes to update related data.
    /// </summary>
    private async Task OnLocationChangedAsync()
    {
        try
        {
            if (_viewModel == null || string.IsNullOrEmpty(_viewModel.SelectedLocation)) return;

            // Validate location or load location-specific data if needed
            await ValidateLocationAsync(_viewModel.SelectedLocation);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling location change: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads application state from database.
    /// </summary>
    private async Task LoadApplicationStateAsync()
    {
        try
        {
            // Load user preferences, last used values, etc. from database
            if (_serviceProvider?.GetService(typeof(IApplicationStateService)) is IApplicationStateService appStateService)
            {
                // Set default values based on application state
                if (_viewModel != null)
                {
                    if (!string.IsNullOrEmpty(appStateService.CurrentOperation))
                    {
                        _viewModel.SelectedOperation = appStateService.CurrentOperation;
                    }
                    
                    if (!string.IsNullOrEmpty(appStateService.CurrentLocation))
                    {
                        _viewModel.SelectedLocation = appStateService.CurrentLocation;
                    }
                }
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading application state: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads part-specific data from database.
    /// </summary>
    private async Task LoadPartSpecificDataAsync(string partId)
    {
        try
        {
            if (_serviceProvider?.GetService(typeof(IDatabaseService)) is IDatabaseService databaseService)
            {
                // Load part details for validation/display purposes
                var parameters = new Dictionary<string, object>
                {
                    ["p_ItemNumber"] = partId
                };

                // Use stored procedure to get part details for validation
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    databaseService.GetConnectionString(),
                    "md_part_ids_Get_ByItemNumber", // Stored procedure to get part details
                    parameters
                );

                if (result.IsSuccess && result.Data.Rows.Count > 0 && _viewModel != null)
                {
                    // Part exists - could update UI with additional part info if needed
                    var partRow = result.Data.Rows[0];
                    var description = partRow["Description"]?.ToString();
                    var customer = partRow["Customer"]?.ToString();
                    
                    // Clear any previous part-related errors
                    if (_viewModel.ErrorMessage.Contains("Part"))
                    {
                        _viewModel.HasError = false;
                        _viewModel.ErrorMessage = string.Empty;
                    }
                }
                else if (_viewModel != null)
                {
                    // Part not found - show warning but don't prevent entry
                    System.Diagnostics.Debug.WriteLine($"Part ID '{partId}' not found in master data");
                }
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading part-specific data: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads operation-specific data from database.
    /// </summary>
    private async Task LoadOperationSpecificDataAsync(string operation)
    {
        try
        {
            // Load work centers, default locations for this operation, etc.
            if (_serviceProvider?.GetService(typeof(IDatabaseService)) is IDatabaseService databaseService)
            {
                // Example: Load valid locations for this operation
                // var locations = await databaseService.GetLocationsForOperationAsync(operation);
                // Update _viewModel.Locations collection if needed
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading operation-specific data: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates the selected location against database.
    /// </summary>
    private async Task ValidateLocationAsync(string location)
    {
        try
        {
            if (_serviceProvider?.GetService(typeof(IDatabaseService)) is IDatabaseService databaseService)
            {
                // Validate location exists and is active
                var parameters = new Dictionary<string, object>
                {
                    ["p_Location"] = location
                };

                // Use stored procedure to validate location
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    databaseService.GetConnectionString(),
                    "inv_locations_Validate", // Stored procedure to validate location
                    parameters
                );

                if (_viewModel != null)
                {
                    if (result.IsSuccess && result.Data.Rows.Count > 0)
                    {
                        var isActive = Convert.ToBoolean(result.Data.Rows[0]["IsActive"] ?? false);
                        if (!isActive)
                        {
                            _viewModel.HasError = true;
                            _viewModel.ErrorMessage = $"Location '{location}' is not active or available.";
                        }
                        else
                        {
                            // Clear any previous location-related errors
                            if (_viewModel.ErrorMessage.Contains("Location"))
                            {
                                _viewModel.HasError = false;
                                _viewModel.ErrorMessage = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        _viewModel.HasError = true;
                        _viewModel.ErrorMessage = $"Location '{location}' is not valid.";
                    }
                }
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error validating location: {ex.Message}");
            
            // Don't show validation errors to user unless it's a critical issue
            if (_viewModel != null && ex.Message.Contains("Location"))
            {
                _viewModel.HasError = true;
                _viewModel.ErrorMessage = "Unable to validate location. Please verify manually.";
            }
        }
    }

    /// <summary>
    /// Handles keyboard shortcuts as specified in MTM requirements.
    /// F5: Reset form (same as Reset button)
    /// Enter: Move to next control or save if on Save button
    /// Escape: Cancel current operation
    /// Shift+F5: Hard reset (refresh all data from database)
    /// </summary>
    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            switch (e.Key)
            {
                case Key.F5:
                    e.Handled = true;
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        // Shift+F5: Hard reset with database refresh
                        await ExecuteHardResetAsync();
                    }
                    else
                    {
                        // F5: Soft reset
                        if (_viewModel.ResetCommand.CanExecute(null))
                        {
                            _viewModel.ResetCommand.Execute(null);
                        }
                    }
                    break;

                case Key.Enter:
                    e.Handled = true;
                    await HandleEnterKeyAsync(e.Source);
                    break;

                case Key.Escape:
                    e.Handled = true;
                    // Clear any error state and focus the first control
                    if (_viewModel != null)
                    {
                        _viewModel.HasError = false;
                        _viewModel.ErrorMessage = string.Empty;
                    }
                    MoveFocusToFirstControl();
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling keyboard shortcut: {ex.Message}");
        }
    }

    /// <summary>
    /// Executes a hard reset including database refresh.
    /// </summary>
    private async Task ExecuteHardResetAsync()
    {
        try
        {
            if (_viewModel == null) return;

            // Reset form fields
            if (_viewModel.ResetCommand.CanExecute(null))
            {
                _viewModel.ResetCommand.Execute(null);
            }

            // Refresh all lookup data from database
            if (_viewModel.RefreshDataCommand.CanExecute(null))
            {
                _viewModel.RefreshDataCommand.Execute(null);
            }

            // Refresh QuickButtons if available
            if (_quickButtonsViewModel?.RefreshButtonsCommand.CanExecute(null) == true)
            {
                _quickButtonsViewModel.RefreshButtonsCommand.Execute(null);
            }

            System.Diagnostics.Debug.WriteLine("Hard reset completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during hard reset: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Enter key navigation - moves to next logical control or executes save.
    /// </summary>
    private async Task HandleEnterKeyAsync(object? source)
    {
        if (_viewModel == null) return;

        try
        {
            // If focused on Save button and can save, execute save command
            if (source is Button button && button.Name?.Contains("Save") == true)
            {
                if (_viewModel.SaveCommand.CanExecute(null))
                {
                    _viewModel.SaveCommand.Execute(null);
                    // Save completion will be handled by OnInventorySaveCompleted event
                    return;
                }
            }

            // Otherwise, move focus to next control
            MoveFocusToNextControl();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling Enter key: {ex.Message}");
        }
    }

    /// <summary>
    /// Moves focus to the next logical control in the form.
    /// Follows the order: Part -> Operation -> Location -> Quantity -> Notes -> Save button.
    /// </summary>
    private void MoveFocusToNextControl()
    {
        try
        {
            var currentFocus = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
            
            // Define the logical tab order based on the form layout
            var tabOrder = new[]
            {
                "PartAutoCompleteBox",
                "OperationAutoCompleteBox", 
                "LocationAutoCompleteBox",
                "QuantityTextBox",
                "NotesTextBox",
                "SaveButton"
            };

            // Find current control in tab order
            string? currentName = (currentFocus as Control)?.Name;
            int currentIndex = Array.IndexOf(tabOrder, currentName);
            
            // Move to next control, or start from beginning if not found
            int nextIndex = currentIndex >= 0 ? (currentIndex + 1) % tabOrder.Length : 0;
            string nextControlName = tabOrder[nextIndex];
            
            // Find and focus the next control
            var nextControl = this.FindControl<Control>(nextControlName);
            nextControl?.Focus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving focus to next control: {ex.Message}");
        }
    }

    /// <summary>
    /// Moves focus to the first control in the form (Part AutoCompleteBox).
    /// </summary>
    private void MoveFocusToFirstControl()
    {
        try
        {
            var partAutoCompleteBox = this.FindControl<AutoCompleteBox>("PartAutoCompleteBox");
            partAutoCompleteBox?.Focus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving focus to first control: {ex.Message}");
        }
    }

    #region Database Event Handlers

    /// <summary>
    /// Handles database connection state changes.
    /// </summary>
    private void OnDatabaseConnectionChanged(bool isConnected)
    {
        try
        {
            if (_viewModel != null)
            {
                if (!isConnected)
                {
                    _viewModel.HasError = true;
                    _viewModel.ErrorMessage = "Database connection lost. Please check your connection and try again.";
                }
                else
                {
                    _viewModel.HasError = false;
                    _viewModel.ErrorMessage = string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling database connection change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles database errors from background operations.
    /// </summary>
    private void OnDatabaseError(string operation, Exception exception)
    {
        try
        {
            if (_viewModel != null)
            {
                _viewModel.HasError = true;
                _viewModel.ErrorMessage = $"Database error during {operation}: {exception.Message}";
            }
            
            System.Diagnostics.Debug.WriteLine($"Database error in {operation}: {exception.Message}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling database error: {ex.Message}");
        }
    }

    #endregion

    #region Cleanup

    /// <summary>
    /// Cleans up event subscriptions when the view is being disposed.
    /// </summary>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        try
        {
            // Clean up event subscriptions
            KeyDown -= OnKeyDown;
            Loaded -= OnLoaded;
            
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel.SaveCompleted -= OnInventorySaveCompleted;
            }
            
            if (_quickButtonsViewModel != null)
            {
                _quickButtonsViewModel.QuickActionExecuted -= OnQuickActionExecuted;
            }
            
            System.Diagnostics.Debug.WriteLine("InventoryTabView cleanup completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during InventoryTabView cleanup: {ex.Message}");
        }
        finally
        {
            base.OnDetachedFromVisualTree(e);
        }
    }

    #endregion
}
