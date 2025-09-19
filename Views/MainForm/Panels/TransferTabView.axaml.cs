using System;
using System.ComponentModel;
using System.Linq;
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
    private ISuccessOverlayService? _successOverlayService;
    private ISuggestionOverlayService? _suggestionOverlayService;
    private readonly IServiceProvider? _serviceProvider;
    private bool _isShowingSuggestionOverlay = false;

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

            // Setup TextBox LostFocus handlers for SuggestionOverlay (following InventoryTabView pattern)
            SetupTextBoxSuggestionHandlers();

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
            // Try to resolve services if not already resolved
            if (_suggestionOverlayService == null || _successOverlayService == null)
            {
                TryResolveServices();
            }

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
    /// Attempts to resolve required services from various sources (following InventoryTabView pattern).
    /// </summary>
    private void TryResolveServices()
    {
        try
        {
            // Method 1: Try the injected service provider
            if (_serviceProvider != null && _suggestionOverlayService == null)
            {
                try
                {
                    if (_suggestionOverlayService == null)
                    {
                        _suggestionOverlayService = _serviceProvider.GetService<ISuggestionOverlayService>();
                        _logger?.LogDebug("Method 1 - SuggestionOverlayService resolution: {ServiceResolved}", _suggestionOverlayService != null);
                        System.Diagnostics.Debug.WriteLine($"Method 1 - SuggestionOverlayService resolution: {_suggestionOverlayService != null}");
                    }

                    if (_successOverlayService == null)
                    {
                        _successOverlayService = _serviceProvider.GetService<ISuccessOverlayService>();
                        System.Diagnostics.Debug.WriteLine($"Method 1 - SuccessOverlayService resolution: {_successOverlayService != null}");
                    }

                    // Additional debugging - check if services are registered
                    var suggestionServices = _serviceProvider.GetServices<ISuggestionOverlayService>();
                    var suggestionServiceCount = suggestionServices?.Count() ?? 0;
                    _logger?.LogDebug("ISuggestionOverlayService instances in container: {ServiceCount}", suggestionServiceCount);
                    System.Diagnostics.Debug.WriteLine($"ISuggestionOverlayService instances in container: {suggestionServiceCount}");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to resolve overlay services from service provider");
                    System.Diagnostics.Debug.WriteLine($"Failed to resolve overlay services: {ex.Message}");
                }
            }

            // Method 2: Try to get from MainWindow if it has a service provider
            if (_suggestionOverlayService == null || _successOverlayService == null)
            {
                try
                {
                    var mainWindow = TopLevel.GetTopLevel(this);
                    if (mainWindow?.DataContext != null)
                    {
                        var serviceProviderProperty = mainWindow.DataContext.GetType().GetProperty("ServiceProvider");
                        if (serviceProviderProperty?.GetValue(mainWindow.DataContext) is IServiceProvider windowServiceProvider)
                        {
                            if (_suggestionOverlayService == null)
                            {
                                _suggestionOverlayService = windowServiceProvider.GetService<ISuggestionOverlayService>();
                                _logger?.LogDebug("Method 2 - MainWindow SuggestionOverlay resolution: {ServiceResolved}", _suggestionOverlayService != null);
                                System.Diagnostics.Debug.WriteLine($"Method 2 - MainWindow SuggestionOverlay resolution: {_suggestionOverlayService != null}");
                            }

                            if (_successOverlayService == null)
                            {
                                _successOverlayService = windowServiceProvider.GetService<ISuccessOverlayService>();
                                System.Diagnostics.Debug.WriteLine($"Method 2 - MainWindow SuccessOverlay resolution: {_successOverlayService != null}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to resolve overlay services from MainWindow");
                    System.Diagnostics.Debug.WriteLine($"Failed to resolve overlay services from MainWindow: {ex.Message}");
                }
            }

            // Method 3: Try to create instance manually as fallback
            if (_suggestionOverlayService == null || _successOverlayService == null)
            {
                try
                {
                    var loggerFactory = _serviceProvider?.GetService<ILoggerFactory>();
                    if (loggerFactory != null)
                    {
                        if (_suggestionOverlayService == null)
                        {
                            var suggestionServiceLogger = loggerFactory.CreateLogger<SuggestionOverlayService>();
                            _suggestionOverlayService = new SuggestionOverlayService(suggestionServiceLogger);
                            _logger?.LogWarning("Method 3 - Manual SuggestionOverlayService creation successful as fallback");
                            System.Diagnostics.Debug.WriteLine("Method 3 - Manual SuggestionOverlayService creation successful as fallback");
                        }

                        if (_successOverlayService == null)
                        {
                            var successServiceLogger = loggerFactory.CreateLogger<SuccessOverlayService>();
                            var focusManagementLogger = loggerFactory.CreateLogger<FocusManagementService>();
                            var focusService = _serviceProvider?.GetService<IFocusManagementService>() ?? new FocusManagementService(focusManagementLogger);
                            _successOverlayService = new SuccessOverlayService(successServiceLogger, focusService);
                            _logger?.LogWarning("Method 3 - Manual SuccessOverlayService creation successful as fallback");
                            System.Diagnostics.Debug.WriteLine("Method 3 - Manual SuccessOverlayService creation successful as fallback");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to create overlay services manually");
                    System.Diagnostics.Debug.WriteLine($"Failed to create overlay services manually: {ex.Message}");
                }
            }

            // Final logging of service resolution status
            _logger?.LogInformation("Both overlay services successfully resolved - SuggestionType: {SuggestionType}, SuccessType: {SuccessType}",
                _suggestionOverlayService?.GetType().Name ?? "null",
                _successOverlayService?.GetType().Name ?? "null");
            System.Diagnostics.Debug.WriteLine($"Both overlay services successfully resolved - SuggestionType: {_suggestionOverlayService?.GetType().Name ?? "null"}, SuccessType: {_successOverlayService?.GetType().Name ?? "null"}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Critical error during service resolution");
            System.Diagnostics.Debug.WriteLine($"Critical error during service resolution: {ex.Message}");
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

            // Setup Location TextBox - LostFocus only (not TextChanged to avoid double triggering)
            var locationTextBox = this.FindControl<TextBox>("ToLocationTextBox");
            if (locationTextBox != null)
            {
                locationTextBox.LostFocus += OnLocationLostFocus;
                _logger?.LogDebug("Location TextBox LostFocus event handler attached");
            }

            _logger?.LogDebug("TextBox SuggestionOverlay event handlers setup completed (LostFocus only)");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting up TextBox SuggestionOverlay handlers");
        }
    }

    /// <summary>
    /// Handles Part TextBox lost focus event for SuggestionOverlay (following InventoryTabView pattern).
    /// Avoids double triggering by using LostFocus instead of TextChanged.
    /// </summary>
    private async void OnPartLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_suggestionOverlayService == null || sender is not TextBox textBox) return;

        try
        {
            var value = textBox.Text?.Trim() ?? string.Empty;
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel == null) return;

            // Get the master data for validation
            var data = viewModel.PartIds ?? Enumerable.Empty<string>();
            var dataList = data.ToList();

            _logger?.LogDebug("Part LostFocus - Value: '{Value}', DataCount: {DataCount}", value, dataList.Count);

            // If no data available (server down), return without processing
            if (dataList.Count == 0)
            {
                _logger?.LogWarning("Part validation skipped - no validation data available (server may be down)");
                return;
            }

            // Check for exact match (case insensitive)
            bool isExactMatch = dataList.Any(part =>
                string.Equals(part, value, StringComparison.OrdinalIgnoreCase));

            // Find partial matches (not exact matches)
            var semiMatches = dataList
                .Where(part => !string.IsNullOrEmpty(part) &&
                               part.Contains(value, StringComparison.OrdinalIgnoreCase))
                .OrderBy(part => part)
                .ToList();

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
                    var selected = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, semiMatches, value);

                    if (!string.IsNullOrEmpty(selected) && selected != value)
                    {
                        _logger?.LogDebug("Part overlay - User selected: '{Selected}'", selected);
                        viewModel.SelectedPart = selected;
                        textBox.Text = selected;
                    }
                    else
                    {
                        _logger?.LogDebug("Part overlay - User cancelled or no selection, keeping: '{Value}'", value);
                        viewModel.SelectedPart = value;
                    }
                }
                finally
                {
                    _isShowingSuggestionOverlay = false;
                }
            }
            else if (!string.IsNullOrEmpty(value) && !isExactMatch && semiMatches.Count == 0)
            {
                // No matches found - MTM Pattern: Clear textbox to maintain data integrity
                _logger?.LogInformation("Part '{Value}' has no matches in validation source. Clearing textbox for data integrity.", value);
                viewModel.SelectedPart = string.Empty;
                textBox.Text = string.Empty;
            }
            else if (isExactMatch)
            {
                // Exact match found - update ViewModel
                viewModel.SelectedPart = value;
                _logger?.LogDebug("Part exact match found: '{Value}'", value);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in Part LostFocus handler");
        }
    }

    /// <summary>
    /// Handles Operation TextBox lost focus event for SuggestionOverlay (following InventoryTabView pattern).
    /// Avoids double triggering by using LostFocus instead of TextChanged.
    /// </summary>
    private async void OnOperationLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_suggestionOverlayService == null || sender is not TextBox textBox) return;

        try
        {
            var value = textBox.Text?.Trim() ?? string.Empty;
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel == null) return;

            // Get the master data for validation
            var data = viewModel.Operations ?? Enumerable.Empty<string>();
            var dataList = data.ToList();

            _logger?.LogDebug("Operation LostFocus - Value: '{Value}', DataCount: {DataCount}", value, dataList.Count);

            // If no data available (server down), return without processing
            if (dataList.Count == 0)
            {
                _logger?.LogWarning("Operation validation skipped - no validation data available (server may be down)");
                return;
            }

            // Check for exact match (case insensitive)
            bool isExactMatch = dataList.Any(operation =>
                string.Equals(operation, value, StringComparison.OrdinalIgnoreCase));

            // Find partial matches (not exact matches)
            var semiMatches = dataList
                .Where(operation => !string.IsNullOrEmpty(operation) &&
                                   operation.Contains(value, StringComparison.OrdinalIgnoreCase))
                .OrderBy(operation => operation)
                .ToList();

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
                    var selected = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, semiMatches, value);

                    if (!string.IsNullOrEmpty(selected) && selected != value)
                    {
                        _logger?.LogDebug("Operation overlay - User selected: '{Selected}'", selected);
                        viewModel.SelectedOperation = selected;
                        textBox.Text = selected;
                    }
                    else
                    {
                        _logger?.LogDebug("Operation overlay - User cancelled or no selection, keeping: '{Value}'", value);
                        viewModel.SelectedOperation = value;
                    }
                }
                finally
                {
                    _isShowingSuggestionOverlay = false;
                }
            }
            else if (!string.IsNullOrEmpty(value) && !isExactMatch && semiMatches.Count == 0)
            {
                // No matches found - MTM Pattern: Clear textbox to maintain data integrity
                _logger?.LogInformation("Operation '{Value}' has no matches in validation source. Clearing textbox for data integrity.", value);
                viewModel.SelectedOperation = string.Empty;
                textBox.Text = string.Empty;
            }
            else if (isExactMatch)
            {
                // Exact match found - update ViewModel
                viewModel.SelectedOperation = value;
                _logger?.LogDebug("Operation exact match found: '{Value}'", value);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in Operation LostFocus handler");
        }
    }

    /// <summary>
    /// Handles Location TextBox lost focus event for SuggestionOverlay (following InventoryTabView pattern).
    /// Avoids double triggering by using LostFocus instead of TextChanged.
    /// </summary>
    private async void OnLocationLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_suggestionOverlayService == null || sender is not TextBox textBox) return;

        try
        {
            var value = textBox.Text?.Trim() ?? string.Empty;
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel == null) return;

            // Get the master data for validation
            var data = viewModel.Locations ?? Enumerable.Empty<string>();
            var dataList = data.ToList();

            _logger?.LogDebug("Location LostFocus - Value: '{Value}', DataCount: {DataCount}", value, dataList.Count);

            // If no data available (server down), return without processing
            if (dataList.Count == 0)
            {
                _logger?.LogWarning("Location validation skipped - no validation data available (server may be down)");
                return;
            }

            // Check for exact match (case insensitive)
            bool isExactMatch = dataList.Any(location =>
                string.Equals(location, value, StringComparison.OrdinalIgnoreCase));

            // Find partial matches (not exact matches)
            var semiMatches = dataList
                .Where(location => !string.IsNullOrEmpty(location) &&
                                  location.Contains(value, StringComparison.OrdinalIgnoreCase))
                .OrderBy(location => location)
                .ToList();

            _logger?.LogDebug("Location '{Value}' - ExactMatch: {IsExactMatch}, SemiMatches: {SemiMatchesCount}",
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
                    var selected = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, semiMatches, value);

                    if (!string.IsNullOrEmpty(selected) && selected != value)
                    {
                        _logger?.LogDebug("Location overlay - User selected: '{Selected}'", selected);
                        viewModel.SelectedToLocation = selected;
                        textBox.Text = selected;
                    }
                    else
                    {
                        _logger?.LogDebug("Location overlay - User cancelled or no selection, keeping: '{Value}'", value);
                        viewModel.SelectedToLocation = value;
                    }
                }
                finally
                {
                    _isShowingSuggestionOverlay = false;
                }
            }
            else if (!string.IsNullOrEmpty(value) && !isExactMatch && semiMatches.Count == 0)
            {
                // No matches found - MTM Pattern: Clear textbox to maintain data integrity
                _logger?.LogInformation("Location '{Value}' has no matches in validation source. Clearing textbox for data integrity.", value);
                viewModel.SelectedToLocation = string.Empty;
                textBox.Text = string.Empty;
            }
            else if (isExactMatch)
            {
                // Exact match found - update ViewModel
                viewModel.SelectedToLocation = value;
                _logger?.LogDebug("Location exact match found: '{Value}'", value);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in Location LostFocus handler");
        }
    }

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

            // Clean up TextBox LostFocus handlers
            var partTextBox = this.FindControl<TextBox>("PartTextBox");
            if (partTextBox != null)
                partTextBox.LostFocus -= OnPartLostFocus;

            var operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            if (operationTextBox != null)
                operationTextBox.LostFocus -= OnOperationLostFocus;

            var locationTextBox = this.FindControl<TextBox>("ToLocationTextBox");
            if (locationTextBox != null)
                locationTextBox.LostFocus -= OnLocationLostFocus;

            _logger?.LogDebug("TransferTabView resources cleaned up");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during resource cleanup");
        }

        base.OnDetachedFromVisualTree(e);
    }
}
