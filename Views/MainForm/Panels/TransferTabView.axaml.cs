using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Controls;
using MTM_Shared_Logic.Models;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for TransferTabView.
/// Provides transfer inventory interface with CollapsiblePanel integration,
/// keyboard shortcuts, and multi-selection DataGrid support.
/// Business logic is implemented in TransferItemViewModel.
/// Uses TextBoxFuzzyValidationBehavior for input validation and suggestions.
/// </summary>
public partial class TransferTabView : UserControl
{
    private readonly ILogger<TransferTabView>? _logger;
    private readonly ISuccessOverlayService? _successOverlayService;
    private readonly IServiceProvider? _serviceProvider;
    
    // Control references
    private CollapsiblePanel? _transferConfigPanel;
    private Button? _searchButton;
    private Button? _resetButton;
    private DataGrid? _inventoryDataGrid;

    /// <summary>
    /// Initializes a new instance of the TransferTabView class.
    /// </summary>
    public TransferTabView()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during InitializeComponent: {ex.Message}");
        }

        InitializeControlReferences();
        SetupEventHandlers();
        
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Constructor with service provider for dependency injection.
    /// </summary>
    public TransferTabView(IServiceProvider serviceProvider) : this()
    {
        _serviceProvider = serviceProvider;
        
        try
        {
            _logger = serviceProvider.GetService<ILogger<TransferTabView>>();
            _successOverlayService = serviceProvider.GetService<ISuccessOverlayService>();
            _logger?.LogDebug("TransferTabView created with dependency injection");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to resolve services in constructor");
        }
    }

    /// <summary>
    /// Initialize control references using FindControl
    /// </summary>
    private void InitializeControlReferences()
    {
        try
        {
            _transferConfigPanel = this.FindControl<CollapsiblePanel>("TransferConfigPanel");
            _searchButton = this.FindControl<Button>("SearchButton");
            _resetButton = this.FindControl<Button>("ResetButton");
            _inventoryDataGrid = this.FindControl<DataGrid>("InventoryDataGrid");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing control references");
        }
    }

    /// <summary>
    /// Setup event handlers for keyboard shortcuts and panel behavior
    /// </summary>
    private void SetupEventHandlers()
    {
        try
        {
            // Keyboard event handling
            KeyDown += OnKeyDown;
            
            // Search button - collapse panel after search
            if (_searchButton != null)
            {
                _searchButton.Click += OnSearchButtonClick;
            }
            
            // Reset button - expand panel after reset
            if (_resetButton != null)
            {
                _resetButton.Click += OnResetButtonClick;
            }
            
            // Auto-focus first field when panel expands
            if (_transferConfigPanel != null)
            {
                _transferConfigPanel.ExpandedChanged += OnTransferConfigPanelExpandedChanged;
            }

            // Handle DataGrid selection changes for multi-selection support
            if (_inventoryDataGrid != null)
            {
                _inventoryDataGrid.SelectionChanged += OnInventoryDataGridSelectionChanged;
            }
            
            _logger?.LogDebug("Event handlers setup completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting up event handlers");
        }
    }

    /// <summary>
    /// Handle view loaded event
    /// </summary>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Subscribe to ViewModel events
            if (DataContext is TransferItemViewModel viewModel)
            {
                viewModel.PanelExpandRequested += OnPanelExpandRequested;
                viewModel.SuccessOverlayRequested += OnSuccessOverlayRequested;
                viewModel.ProgressReported += OnProgressReported;
            }

            // Focus the first input field when view loads
            Dispatcher.UIThread.Post(() =>
            {
                var partTextBox = this.FindControl<TextBox>("PartTextBox");
                partTextBox?.Focus();
            }, DispatcherPriority.ApplicationIdle);
            
            _logger?.LogDebug("TransferTabView loaded successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnLoaded event handler");
        }
    }

    /// <summary>
    /// Handle keyboard shortcuts
    /// </summary>
    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel == null) return;

            switch (e.Key)
            {
                case Key.F5:
                    // Search operation
                    e.Handled = true;
                    if (viewModel.SearchCommand.CanExecute(null))
                    {
                        await ExecuteSearchWithPanelBehavior();
                    }
                    break;

                case Key.Enter:
                    // Transfer operation (if conditions are met)
                    if (viewModel.CanTransfer && viewModel.TransferCommand.CanExecute(null))
                    {
                        e.Handled = true;
                        viewModel.TransferCommand.Execute(null);
                    }
                    break;

                case Key.Escape:
                    // Reset operation
                    e.Handled = true;
                    if (viewModel.ResetCommand.CanExecute(null))
                    {
                        await ExecuteResetWithPanelBehavior();
                    }
                    break;

                case Key.P when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                    // Print operation
                    e.Handled = true;
                    if (viewModel.PrintCommand.CanExecute(null))
                    {
                        viewModel.PrintCommand.Execute(null);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling keyboard shortcut: {Key}", e.Key);
        }
    }

    /// <summary>
    /// Handle search button click with panel auto-collapse behavior
    /// </summary>
    private async void OnSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        await ExecuteSearchWithPanelBehavior();
    }

    /// <summary>
    /// Handle reset button click with panel auto-expand behavior
    /// </summary>
    private async void OnResetButtonClick(object? sender, RoutedEventArgs e)
    {
        await ExecuteResetWithPanelBehavior();
    }

    /// <summary>
    /// Execute search with auto-collapse panel behavior
    /// </summary>
    private async Task ExecuteSearchWithPanelBehavior()
    {
        try
        {
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel?.SearchCommand.CanExecute(null) == true)
            {
                // Execute search
                viewModel.SearchCommand.Execute(null);
                
                // Auto-collapse panel after search to maximize DataGrid view
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (_transferConfigPanel != null && _transferConfigPanel.IsExpanded)
                    {
                        _transferConfigPanel.SetExpanded(false, true);
                        _logger?.LogDebug("Transfer config panel auto-collapsed after search");
                    }
                }, DispatcherPriority.Background);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing search with panel behavior");
        }
    }

    /// <summary>
    /// Execute reset with auto-expand panel behavior
    /// </summary>
    private async Task ExecuteResetWithPanelBehavior()
    {
        try
        {
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel?.ResetCommand.CanExecute(null) == true)
            {
                // Execute reset
                viewModel.ResetCommand.Execute(null);
                
                // Auto-expand panel after reset for new search configuration
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (_transferConfigPanel != null && !_transferConfigPanel.IsExpanded)
                    {
                        _transferConfigPanel.SetExpanded(true, true);
                        _logger?.LogDebug("Transfer config panel auto-expanded after reset");
                    }
                    
                    // Focus first field after reset
                    var partTextBox = this.FindControl<TextBox>("PartTextBox");
                    partTextBox?.Focus();
                }, DispatcherPriority.Background);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing reset with panel behavior");
        }
    }

    /// <summary>
    /// Handle CollapsiblePanel expanded/collapsed events
    /// </summary>
    private void OnTransferConfigPanelExpandedChanged(object? sender, bool isExpanded)
    {
        try
        {
            if (isExpanded)
            {
                // Focus first field when panel expands
                Dispatcher.UIThread.Post(() =>
                {
                    var partTextBox = this.FindControl<TextBox>("PartTextBox");
                    partTextBox?.Focus();
                }, DispatcherPriority.ApplicationIdle);
                
                _logger?.LogDebug("Transfer config panel expanded - focused first field");
            }
            else
            {
                _logger?.LogDebug("Transfer config panel collapsed - maximized DataGrid view");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling panel expanded changed event");
        }
    }

    /// <summary>
    /// Handle panel expand request from ViewModel
    /// </summary>
    private void OnPanelExpandRequested(object? sender, EventArgs e)
    {
        try
        {
            if (_transferConfigPanel != null && !_transferConfigPanel.IsExpanded)
            {
                _transferConfigPanel.SetExpanded(true, true);
                _logger?.LogDebug("Transfer config panel expanded via ViewModel request");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling panel expand request");
        }
    }

    /// <summary>
    /// Handle SuccessOverlay request from ViewModel
    /// </summary>
    private async void OnSuccessOverlayRequested(object? sender, SuccessOverlayEventArgs e)
    {
        try
        {
            if (_successOverlayService != null)
            {
                // Use the current control as the target for the overlay
                await _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                    this, 
                    e.Message, 
                    e.Details, 
                    e.IconKind, 
                    e.Duration
                );
                
                _logger?.LogDebug("SuccessOverlay shown: {Message}", e.Message);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error showing SuccessOverlay");
        }
    }

    /// <summary>
    /// Handle progress reporting from ViewModel to MainView status bar
    /// </summary>
    private void OnProgressReported(object? sender, ProgressReportEventArgs e)
    {
        try
        {
            // TODO: Forward progress to MainView status bar
            // This would typically be done via an event or service call to MainView
            // For now, just log the progress for debugging
            _logger?.LogInformation("Transfer progress: {Message} ({Percentage}%) - {Operation}", 
                e.Message, e.ProgressPercentage ?? 0, e.Operation);
            
            // In a full implementation, this would call something like:
            // mainViewService?.UpdateStatusBar(e.Message, e.ProgressPercentage, e.IsComplete, e.IsError);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling progress report");
        }
    }

    #region Event Handlers

    /// <summary>
    /// Handle DataGrid selection changes to support multi-selection
    /// </summary>
    private void OnInventoryDataGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (DataContext is TransferItemViewModel viewModel && _inventoryDataGrid != null)
            {
                // Update the SelectedInventoryItems collection in the ViewModel
                viewModel.SelectedInventoryItems.Clear();
                
                foreach (var selectedItem in _inventoryDataGrid.SelectedItems)
                {
                    if (selectedItem is InventoryItem inventoryItem)
                    {
                        viewModel.SelectedInventoryItems.Add(inventoryItem);
                    }
                }
                
                _logger?.LogDebug("DataGrid selection updated: {Count} items selected", viewModel.SelectedInventoryItems.Count);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling DataGrid selection change");
        }
    }

    #endregion

    /// <summary>
    /// Clean up resources when view is detached
    /// </summary>
    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        try
        {
            // Unsubscribe from ViewModel events
            if (DataContext is TransferItemViewModel viewModel)
            {
                viewModel.PanelExpandRequested -= OnPanelExpandRequested;
                viewModel.SuccessOverlayRequested -= OnSuccessOverlayRequested;
                viewModel.ProgressReported -= OnProgressReported;
            }

            // Clean up event subscriptions
            KeyDown -= OnKeyDown;
            
            if (_searchButton != null)
                _searchButton.Click -= OnSearchButtonClick;
                
            if (_resetButton != null)
                _resetButton.Click -= OnResetButtonClick;
                
            if (_transferConfigPanel != null)
                _transferConfigPanel.ExpandedChanged -= OnTransferConfigPanelExpandedChanged;

            if (_inventoryDataGrid != null)
                _inventoryDataGrid.SelectionChanged -= OnInventoryDataGridSelectionChanged;
            
            _logger?.LogDebug("TransferTabView resources cleaned up");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during resource cleanup");
        }
        
        base.OnDetachedFromVisualTree(e);
    }
}