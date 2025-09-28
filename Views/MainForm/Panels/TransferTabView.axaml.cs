using System;
using System.Collections.Generic;
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
    private readonly ISuccessOverlayService? _successOverlayService;
    private readonly ISuggestionOverlayService? _suggestionOverlayService;

    // Control references
    private CollapsiblePanel? _searchConfigPanel;
    private Button? _searchButton;
    private Button? _resetButton;
    private DataGrid? _transferInventoryDataGrid;

    /// <summary>
    /// Initializes a new instance of the TransferTabView class.
    /// </summary>
    public TransferTabView()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] TransferTabView() constructor started");
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] InitializeComponent() completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Error during InitializeComponent: {ex.Message}");
        }

        InitializeControlReferences();
        SetupEventHandlers();

        Loaded += OnLoaded;
        System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] TransferTabView() constructor completed");
    }

    /// <summary>
    /// Constructor with service provider for dependency injection.
    /// </summary>
    public TransferTabView(IServiceProvider? serviceProvider) : this()
    {
        // Get services from service provider (following MTM pattern)
        try
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] TransferTabView(IServiceProvider) constructor started");
            _logger = serviceProvider?.GetService<ILogger<TransferTabView>>();
            _successOverlayService = serviceProvider?.GetService<ISuccessOverlayService>();
            _suggestionOverlayService = serviceProvider?.GetService<ISuggestionOverlayService>();

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Service resolution results:");
            System.Diagnostics.Debug.WriteLine($"  - Logger: {(_logger != null ? "RESOLVED" : "NULL")}");
            System.Diagnostics.Debug.WriteLine($"  - SuccessOverlayService: {(_successOverlayService != null ? "RESOLVED" : "NULL")}");
            System.Diagnostics.Debug.WriteLine($"  - SuggestionOverlayService: {(_suggestionOverlayService != null ? "RESOLVED" : "NULL")}");

            _logger?.LogInformation("[TRANSFER-DEBUG] TransferTabView constructor - Services resolved: Logger={HasLogger}, Success={HasSuccess}, Suggestion={HasSuggestion}",
                _logger != null, _successOverlayService != null, _suggestionOverlayService != null);
            _logger?.LogDebug("TransferTabView created with dependency injection");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Failed to resolve services: {ex.Message}");
            _logger?.LogError(ex, "[TRANSFER-DEBUG] Failed to resolve services in TransferTabView constructor");
        }
    }

    /// <summary>
    /// Initialize control references using FindControl
    /// </summary>
    private void InitializeControlReferences()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] InitializeControlReferences started");

            _searchConfigPanel = this.FindControl<CollapsiblePanel>("SearchConfigPanel");
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] SearchConfigPanel: {(_searchConfigPanel != null ? "FOUND" : "NOT FOUND")}");

            _searchButton = this.FindControl<Button>("SearchButton");
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] SearchButton: {(_searchButton != null ? "FOUND" : "NOT FOUND")}");

            _resetButton = this.FindControl<Button>("ResetButton");
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] ResetButton: {(_resetButton != null ? "FOUND" : "NOT FOUND")}");

            _transferInventoryDataGrid = this.FindControl<DataGrid>("InventoryDataGrid");
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] InventoryDataGrid: {(_transferInventoryDataGrid != null ? "FOUND" : "NOT FOUND")}");

            _logger?.LogInformation("[TRANSFER-DEBUG] Control references initialized: Panel={HasPanel}, SearchButton={HasSearch}, ResetButton={HasReset}, DataGrid={HasDataGrid}",
                _searchConfigPanel != null, _searchButton != null, _resetButton != null, _transferInventoryDataGrid != null);

            if (_transferInventoryDataGrid == null)
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] CRITICAL: TransferInventoryDataGrid control not found - DataGrid functionality may not work");
                _logger?.LogWarning("[TRANSFER-DEBUG] TransferInventoryDataGrid control not found - DataGrid functionality may not work");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] TransferInventoryDataGrid found - Setting up standard DataGrid event handlers");
                SetupStandardDataGridEvents();
            }

            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] InitializeControlReferences completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] ERROR in InitializeControlReferences: {ex.Message}");
            _logger?.LogError(ex, "[TRANSFER-DEBUG] Error initializing control references");
        }
    }

    /// <summary>
    /// Setup standard DataGrid specific event handlers
    /// </summary>
    private void SetupStandardDataGridEvents()
    {
        try
        {
            if (_transferInventoryDataGrid == null) return;

            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] SetupStandardDataGridEvents started");

            // Subscribe to standard DataGrid selection events
            _transferInventoryDataGrid.SelectionChanged += OnDataGridSelectionChanged;
            _transferInventoryDataGrid.DoubleTapped += OnDataGridDoubleTapped;

            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] Standard DataGrid event handlers setup completed");
            _logger?.LogInformation("[TRANSFER-DEBUG] Standard DataGrid event handlers setup completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Error setting up standard DataGrid events: {ex.Message}");
            _logger?.LogError(ex, "[TRANSFER-DEBUG] Error setting up standard DataGrid events");
        }
    }

    /// <summary>
    /// Handle DataGrid selection changes
    /// </summary>
    private void OnDataGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (sender is DataGrid dataGrid && DataContext is TransferItemViewModel viewModel)
            {
                // Update ViewModel with selected item
                if (dataGrid.SelectedItem is TransferInventoryItem selectedItem)
                {
                    viewModel.SelectedInventoryItem = selectedItem;
                    _logger?.LogDebug("DataGrid selection changed to: {PartId}", selectedItem.PartId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling DataGrid selection change");
        }
    }

    /// <summary>
    /// Handle DataGrid double-tap to edit item
    /// </summary>
    private async void OnDataGridDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        try
        {
            if (sender is DataGrid dataGrid && DataContext is TransferItemViewModel viewModel)
            {
                if (dataGrid.SelectedItem is TransferInventoryItem selectedItem)
                {
                    await viewModel.EditItemCommand.ExecuteAsync(selectedItem);
                    _logger?.LogDebug("DataGrid double-tapped - opening edit for: {PartId}", selectedItem.PartId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling DataGrid double-tap");
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

            if (_searchButton != null)
            {
                _searchButton.Click += OnSearchButtonClick;
            }

            if (_resetButton != null)
            {
                _resetButton.Click += OnResetButtonClick;
            }

            if (_searchConfigPanel != null)
            {
                _searchConfigPanel.ExpandedChanged += OnTransferConfigPanelExpandedChanged;
            }

            // Setup SuggestionOverlay event handlers
            SetupTextBoxSuggestionHandlers();
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
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] OnLoaded event triggered");

            // Verify services are available
            if (_suggestionOverlayService == null || _successOverlayService == null)
            {
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Required overlay services not available - Suggestion: {_suggestionOverlayService != null}, Success: {_successOverlayService != null}");
                _logger?.LogWarning("Required overlay services not available");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] All overlay services are available");
            }

            // Subscribe to ViewModel events
            if (DataContext is TransferItemViewModel viewModel)
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] DataContext is TransferItemViewModel - subscribing to events");
                viewModel.PanelExpandRequested += OnPanelExpandRequested;
                viewModel.PanelCollapseRequested += OnPanelCollapseRequested;
                viewModel.SuccessOverlayRequested += OnSuccessOverlayRequested;
                viewModel.ProgressReported += OnProgressReported;
                viewModel.AutoSizeColumnsRequested += OnAutoSizeColumnsRequested;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] DataContext is not TransferItemViewModel: {DataContext?.GetType().Name ?? "null"}");
            }

            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] TransferTabView loaded successfully");
            _logger?.LogDebug("TransferTabView loaded successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Error in OnLoaded event handler: {ex.Message}");
            _logger?.LogError(ex, "Error in OnLoaded event handler");
        }
    }

    /// <summary>
    /// Handle keyboard shortcuts
    /// </summary>
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel == null) return;

            switch (e.Key)
            {
                case Key.F5:
                    e.Handled = true;
                    if (viewModel.SearchCommand.CanExecute(null))
                    {
                        viewModel.SearchCommand.Execute(null);
                    }
                    break;

                case Key.Enter:
                    e.Handled = true;
                    if (viewModel.CanTransfer && viewModel.TransferCommand.CanExecute(null))
                    {
                        viewModel.TransferCommand.Execute(null);
                    }
                    break;

                case Key.Escape:
                    e.Handled = true;
                    if (viewModel.ResetCommand.CanExecute(null))
                    {
                        viewModel.ResetCommand.Execute(null);
                    }
                    break;

                case Key.P when e.KeyModifiers.HasFlag(KeyModifiers.Control):
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
            _logger?.LogError(ex, "Error handling keyboard shortcut");
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
                viewModel.SearchCommand.Execute(null);

                // Auto-collapse panel after successful search
                if (_searchConfigPanel != null && _searchConfigPanel.IsExpanded)
                {
                    await Task.Delay(500); // Allow search to complete
                    _searchConfigPanel.IsExpanded = false;
                    _logger?.LogDebug("Transfer config panel auto-collapsed after search");
                }
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
                viewModel.ResetCommand.Execute(null);

                // Auto-expand panel after reset
                if (_searchConfigPanel != null && !_searchConfigPanel.IsExpanded)
                {
                    await Task.Delay(200); // Allow reset to complete
                    _searchConfigPanel.IsExpanded = true;
                    _logger?.LogDebug("Transfer config panel auto-expanded after reset");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing reset with panel behavior");
        }
    }

    /// <summary>
    /// Handle CollapsiblePanel property changes
    /// </summary>
    private void OnTransferConfigPanelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CollapsiblePanel.IsExpanded) && sender is CollapsiblePanel panel)
        {
            OnTransferConfigPanelExpandedChanged(this, panel.IsExpanded);
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
                _logger?.LogDebug("Transfer config panel expanded - focusing first input");

                // Focus first input when panel expands
                var partTextBox = this.FindControl<TextBox>("PartTextBox");
                partTextBox?.Focus();
            }
            else
            {
                _logger?.LogDebug("Transfer config panel collapsed");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling panel expanded changed");
        }
    }

    /// <summary>
    /// Handle panel expand request from ViewModel
    /// </summary>
    private void OnPanelExpandRequested(object? sender, EventArgs e)
    {
        try
        {
            if (_searchConfigPanel != null && !_searchConfigPanel.IsExpanded)
            {
                _searchConfigPanel.IsExpanded = true;
                _logger?.LogDebug("Panel expand requested and executed");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling panel expand request");
        }
    }

    /// <summary>
    /// Handle panel collapse request from ViewModel (after successful search)
    /// </summary>
    private void OnPanelCollapseRequested(object? sender, EventArgs e)
    {
        try
        {
            if (_searchConfigPanel != null && _searchConfigPanel.IsExpanded)
            {
                _searchConfigPanel.IsExpanded = false;
                _logger?.LogDebug("Panel collapse requested and executed after successful search");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling panel collapse request");
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
                await _successOverlayService.ShowSuccessOverlayAsync(
                    targetControl: this,
                    message: e.Message,
                    details: e.Details,
                    iconKind: e.IconKind,
                    duration: e.Duration
                );
                _logger?.LogDebug("Success overlay displayed: {Message}", e.Message);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error displaying success overlay");
        }
    }

    /// <summary>
    /// Handle progress reporting from ViewModel to MainView status bar
    /// </summary>
    private void OnProgressReported(object? sender, ProgressReportEventArgs e)
    {
        try
        {
            // Progress reporting is handled by MainView via ViewModel events
            _logger?.LogTrace("Progress reported: {Message}", e.Message);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling progress report");
        }
    }

    /// <summary>
    /// Handles DataGrid column auto-sizing request from ViewModel
    /// </summary>
    private async void OnAutoSizeColumnsRequested(object? sender, EventArgs e)
    {
        try
        {
            if (_transferInventoryDataGrid == null)
            {
                _logger?.LogWarning("Cannot auto-size columns: DataGrid reference is null");
                return;
            }

            // Use Dispatcher to ensure we're on the UI thread and data is fully loaded
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] Auto-sizing DataGrid columns");

                    // Auto-size all columns to fit their content
                    foreach (var column in _transferInventoryDataGrid.Columns)
                    {
                        if (column is DataGridTextColumn textColumn)
                        {
                            // Set to auto-size based on content
                            textColumn.Width = DataGridLength.Auto;
                        }
                    }

                    // Force a layout update to apply the changes
                    _transferInventoryDataGrid.InvalidateArrange();
                    _transferInventoryDataGrid.UpdateLayout();

                    _logger?.LogDebug("DataGrid columns auto-sized successfully");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error during DataGrid column auto-sizing");
                }
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling auto-size columns request");
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
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] SetupTextBoxSuggestionHandlers started");

            var partTextBox = this.FindControl<TextBox>("PartTextBox");
            if (partTextBox != null)
            {
                partTextBox.LostFocus += OnPartLostFocus;
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] PartTextBox found and LostFocus event attached");
                _logger?.LogInformation("[TRANSFER-DEBUG] PartTextBox found and LostFocus event attached");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] PartTextBox not found - suggestion overlay will not work for Part input");
                _logger?.LogWarning("[TRANSFER-DEBUG] PartTextBox not found - suggestion overlay will not work for Part input");
            }

            var operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            if (operationTextBox != null)
            {
                operationTextBox.LostFocus += OnOperationLostFocus;
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] OperationTextBox found and LostFocus event attached");
                _logger?.LogInformation("[TRANSFER-DEBUG] OperationTextBox found and LostFocus event attached");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] OperationTextBox not found - suggestion overlay will not work for Operation input");
                _logger?.LogWarning("[TRANSFER-DEBUG] OperationTextBox not found - suggestion overlay will not work for Operation input");
            }

            var locationTextBox = this.FindControl<TextBox>("ToLocationTextBox");
            if (locationTextBox != null)
            {
                locationTextBox.LostFocus += OnLocationLostFocus;
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] ToLocationTextBox found and LostFocus event attached");
                _logger?.LogInformation("[TRANSFER-DEBUG] ToLocationTextBox found and LostFocus event attached");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] ToLocationTextBox not found - suggestion overlay will not work for Location input");
                _logger?.LogWarning("[TRANSFER-DEBUG] ToLocationTextBox not found - suggestion overlay will not work for Location input");
            }

            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] SetupTextBoxSuggestionHandlers completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Error setting up TextBox suggestion handlers: {ex.Message}");
            _logger?.LogError(ex, "[TRANSFER-DEBUG] Error setting up TextBox suggestion handlers");
        }
    }

    /// <summary>
    /// Handles Part TextBox lost focus event for SuggestionOverlay (following RemoveTabView pattern).
    /// Uses direct SuggestionOverlayService call instead of TextBoxFuzzyValidationBehavior event.
    /// </summary>
    private async void OnPartLostFocus(object? sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] OnPartLostFocus triggered");
        _logger?.LogInformation("[TRANSFER-DEBUG] OnPartLostFocus triggered");

        if (_suggestionOverlayService == null)
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] SuggestionOverlayService is null in OnPartLostFocus");
            _logger?.LogWarning("[TRANSFER-DEBUG] SuggestionOverlayService is null in OnPartLostFocus");
            return;
        }

        if (sender is not TextBox textBox)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Sender is not TextBox in OnPartLostFocus: {sender?.GetType().Name ?? "null"}");
            _logger?.LogWarning("[TRANSFER-DEBUG] Sender is not TextBox in OnPartLostFocus: {SenderType}", sender?.GetType().Name ?? "null");
            return;
        }

        try
        {
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel == null)
            {
                _logger?.LogWarning("[TRANSFER-DEBUG] DataContext is not TransferItemViewModel: {DataContextType}", DataContext?.GetType().Name ?? "null");
                return;
            }

            var value = textBox.Text?.Trim() ?? string.Empty;
            var dataList = viewModel.PartIds?.ToList() ?? [];

            _logger?.LogDebug("[TRANSFER-DEBUG] OnPartLostFocus - Input: '{Value}', DataList Count: {Count}", value, dataList.Count);

            if (dataList.Count == 0 || string.IsNullOrEmpty(value))
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Part LostFocus: No PartIds data available or empty input");
                return;
            }

            // Check for exact match (following RemoveTabView pattern)
            var exactMatch = dataList.Any(x =>
                string.Equals(x, value, StringComparison.OrdinalIgnoreCase));

            if (exactMatch)
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Part '{Value}' - Exact match found", value);
                return;
            }

            // Find fuzzy matches (following RemoveTabView pattern)
            var semiMatches = dataList.Where(x =>
                x.Contains(value, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(x, value, StringComparison.OrdinalIgnoreCase))
                .Take(50)
                .ToList();

            _logger?.LogDebug("[TRANSFER-DEBUG] OnPartLostFocus - ExactMatch: {ExactMatch}, SemiMatches: {SemiMatchCount}",
                exactMatch, semiMatches.Count);

            if (semiMatches.Count > 0)
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Calling ShowSuggestionsAsync for Part with {Count} suggestions", semiMatches.Count);

                // Direct call to SuggestionOverlayService (matching RemoveTabView pattern)
                var result = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, semiMatches, value);

                _logger?.LogDebug("[TRANSFER-DEBUG] ShowSuggestionsAsync returned: '{Result}'", result ?? "null");

                if (!string.IsNullOrEmpty(result))
                {
                    textBox.Text = result;
                    viewModel.PartText = result;
                    _logger?.LogDebug("[TRANSFER-DEBUG] Part suggestion selected: {Result}", result);
                }
            }
            else
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Part '{Value}' - No matches found", value);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[TRANSFER-DEBUG] Error in OnPartLostFocus");
        }
    }

    /// <summary>
    /// Handles Operation TextBox lost focus event for SuggestionOverlay (following RemoveTabView pattern).
    /// Uses direct SuggestionOverlayService call instead of TextBoxFuzzyValidationBehavior event.
    /// </summary>
    private async void OnOperationLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_suggestionOverlayService == null || sender is not TextBox textBox) return;

        try
        {
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel == null) return;

            var value = textBox.Text?.Trim() ?? string.Empty;
            var dataList = viewModel.Operations?.ToList() ?? [];

            if (dataList.Count == 0 || string.IsNullOrEmpty(value))
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Operation LostFocus: No Operations data available or empty input");
                return;
            }

            // Check for exact match (following RemoveTabView pattern)
            var exactMatch = dataList.Any(x =>
                string.Equals(x, value, StringComparison.OrdinalIgnoreCase));

            if (exactMatch)
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Operation '{Value}' - Exact match found", value);
                return;
            }

            // Find fuzzy matches (following RemoveTabView pattern)
            var semiMatches = dataList.Where(x =>
                x.Contains(value, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(x, value, StringComparison.OrdinalIgnoreCase))
                .Take(50)
                .ToList();

            if (semiMatches.Count > 0)
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Calling ShowSuggestionsAsync for Operation with {Count} suggestions", semiMatches.Count);

                // Direct call to SuggestionOverlayService (matching RemoveTabView pattern)
                var result = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, semiMatches, value);

                if (!string.IsNullOrEmpty(result))
                {
                    textBox.Text = result;
                    viewModel.OperationText = result;
                    _logger?.LogDebug("[TRANSFER-DEBUG] Operation suggestion selected: {Result}", result);
                }
            }
            else
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Operation '{Value}' - No matches found", value);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[TRANSFER-DEBUG] Error in OnOperationLostFocus");
        }
    }

    /// <summary>
    /// Handles Location TextBox lost focus event for SuggestionOverlay (following RemoveTabView pattern).
    /// Uses direct SuggestionOverlayService call instead of TextBoxFuzzyValidationBehavior event.
    /// </summary>
    private async void OnLocationLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_suggestionOverlayService == null || sender is not TextBox textBox) return;

        try
        {
            var viewModel = DataContext as TransferItemViewModel;
            if (viewModel == null) return;

            var value = textBox.Text?.Trim() ?? string.Empty;
            var dataList = viewModel.Locations?.ToList() ?? [];

            if (dataList.Count == 0 || string.IsNullOrEmpty(value))
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Location LostFocus: No Locations data available or empty input");
                return;
            }

            // Check for exact match (following RemoveTabView pattern)
            var exactMatch = dataList.Any(x =>
                string.Equals(x, value, StringComparison.OrdinalIgnoreCase));

            if (exactMatch)
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Location '{Value}' - Exact match found", value);
                return;
            }

            // Find fuzzy matches (following RemoveTabView pattern)
            var semiMatches = dataList.Where(x =>
                x.Contains(value, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(x, value, StringComparison.OrdinalIgnoreCase))
                .Take(50)
                .ToList();

            if (semiMatches.Count > 0)
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Calling ShowSuggestionsAsync for Location with {Count} suggestions", semiMatches.Count);

                // Direct call to SuggestionOverlayService (matching RemoveTabView pattern)
                var result = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, semiMatches, value);

                if (!string.IsNullOrEmpty(result))
                {
                    textBox.Text = result;
                    viewModel.ToLocationText = result;
                    _logger?.LogDebug("[TRANSFER-DEBUG] Location suggestion selected: {Result}", result);
                }
            }
            else
            {
                _logger?.LogDebug("[TRANSFER-DEBUG] Location '{Value}' - No matches found", value);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[TRANSFER-DEBUG] Error in OnLocationLostFocus");
        }
    }

    #endregion

    /// <summary>
    /// Clean up resources when view is detached
    /// </summary>
    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        _logger?.LogDebug("TransferTabView detached from visual tree, performing cleanup");

        try
        {
            // Unsubscribe from ViewModel events
            if (DataContext is TransferItemViewModel viewModel)
            {
                viewModel.PanelExpandRequested -= OnPanelExpandRequested;
                viewModel.PanelCollapseRequested -= OnPanelCollapseRequested;
                viewModel.SuccessOverlayRequested -= OnSuccessOverlayRequested;
                viewModel.ProgressReported -= OnProgressReported;
                viewModel.AutoSizeColumnsRequested -= OnAutoSizeColumnsRequested;
            }

            // Unsubscribe from control events
            if (_searchButton != null)
            {
                _searchButton.Click -= OnSearchButtonClick;
            }

            if (_resetButton != null)
            {
                _resetButton.Click -= OnResetButtonClick;
            }

            if (_searchConfigPanel != null)
            {
                _searchConfigPanel.ExpandedChanged -= OnTransferConfigPanelExpandedChanged;
            }

            // Unsubscribe from TextBox events
            var partTextBox = this.FindControl<TextBox>("PartTextBox");
            if (partTextBox != null)
            {
                partTextBox.LostFocus -= OnPartLostFocus;
            }

            var operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            if (operationTextBox != null)
            {
                operationTextBox.LostFocus -= OnOperationLostFocus;
            }

            var locationTextBox = this.FindControl<TextBox>("ToLocationTextBox");
            if (locationTextBox != null)
            {
                locationTextBox.LostFocus -= OnLocationLostFocus;
            }

            // Unsubscribe from DataGrid events
            if (_transferInventoryDataGrid != null)
            {
                _transferInventoryDataGrid.SelectionChanged -= OnDataGridSelectionChanged;
                _transferInventoryDataGrid.DoubleTapped -= OnDataGridDoubleTapped;
            }

            KeyDown -= OnKeyDown;
            Loaded -= OnLoaded;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during cleanup");
        }

        base.OnDetachedFromVisualTree(e);
    }
}
