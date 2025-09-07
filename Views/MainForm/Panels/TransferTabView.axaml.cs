using System;
using System.Collections.Generic;
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

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for TransferTabView.
/// Provides transfer inventory interface with CollapsiblePanel integration,
/// keyboard shortcuts, and service integrations.
/// Business logic is implemented in TransferItemViewModel.
/// </summary>
public partial class TransferTabView : UserControl
{
    private readonly ILogger<TransferTabView>? _logger;
    private readonly ISuggestionOverlayService? _suggestionOverlayService;
    private readonly IServiceProvider? _serviceProvider;
    
    // Control references
    private CollapsiblePanel? _transferConfigPanel;
    private TextBox? _partTextBox;
    private TextBox? _operationTextBox;
    private TextBox? _toLocationTextBox;
    private Button? _searchButton;
    private Button? _resetButton;
    private Button? _transferButton;
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
            _suggestionOverlayService = serviceProvider.GetService<ISuggestionOverlayService>();
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
            _partTextBox = this.FindControl<TextBox>("PartTextBox");
            _operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            _toLocationTextBox = this.FindControl<TextBox>("ToLocationTextBox");
            _searchButton = this.FindControl<Button>("SearchButton");
            _resetButton = this.FindControl<Button>("ResetButton");
            _transferButton = this.FindControl<Button>("TransferButton");
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
            // Focus the first input field when view loads
            Dispatcher.UIThread.Post(() =>
            {
                _partTextBox?.Focus();
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
                    _partTextBox?.Focus();
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
                    _partTextBox?.Focus();
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
    /// Clean up resources when view is detached
    /// </summary>
    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        try
        {
            // Clean up event subscriptions
            KeyDown -= OnKeyDown;
            
            if (_searchButton != null)
                _searchButton.Click -= OnSearchButtonClick;
                
            if (_resetButton != null)
                _resetButton.Click -= OnResetButtonClick;
                
            if (_transferConfigPanel != null)
                _transferConfigPanel.ExpandedChanged -= OnTransferConfigPanelExpandedChanged;
            
            _logger?.LogDebug("TransferTabView resources cleaned up");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during resource cleanup");
        }
        
        base.OnDetachedFromVisualTree(e);
    }
}