

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Views;
using MTM_WIP_Application_Avalonia.Models;
using Avalonia.Threading;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace API.ViewModels.MainForm;

/// <summary>
/// ViewModel for the inventory removal interface (Control_RemoveTab).
/// Provides comprehensive functionality for removing inventory items from the system,
/// including search capabilities, batch deletion operations, undo functionality,
/// and transaction history tracking.
/// </summary>
public partial class RemoveItemViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;
    private readonly ISuggestionOverlayService _suggestionOverlayService;
    private readonly ISuccessOverlayService _successOverlayService;
    private readonly IQuickButtonsService _quickButtonsService;
    private readonly IRemoveService _removeService;
    private readonly IPrintService? _printService;
    private readonly INavigationService? _navigationService;
    private readonly IServiceProvider _serviceProvider;

    // ViewModel cache for EditInventoryView - maintains data between dialog openings
    private readonly Dictionary<int, EditInventoryViewModel> _editViewModelCache = new();

    #region Observable Collections (InventoryTabView Pattern)

    /// <summary>
    /// Available part IDs for filtering (InventoryTabView pattern)
    /// </summary>
    public ObservableCollection<string> PartIds { get; } = new();

    /// <summary>
    /// Available operations for filtering (InventoryTabView pattern)
    /// </summary>
    public ObservableCollection<string> Operations { get; } = new();

    #endregion

    #region Legacy Observable Collections (for backward compatibility)

    /// <summary>
    /// Available part options for filtering
    /// </summary>
    public ObservableCollection<string> PartOptions { get; } = new();

    /// <summary>
    /// Available operation options for refined filtering
    /// </summary>
    public ObservableCollection<string> OperationOptions { get; } = new();

    /// <summary>
    /// Current inventory items displayed in the DataGrid
    /// </summary>
    public ObservableCollection<InventoryItem> InventoryItems { get; } = new();

    /// <summary>
    /// Currently selected items in the DataGrid for batch operations
    /// </summary>
    public ObservableCollection<InventoryItem> SelectedItems { get; } = new();

    /// <summary>
    /// Currently selected inventory item in the DataGrid
    /// </summary>
    [ObservableProperty]
    private InventoryItem? _selectedItem;

    #endregion

    #region Watermark Properties (InventoryTabView Pattern)

    /// <summary>
    /// Dynamic watermark for Part field - shows error or placeholder
    /// </summary>
    public string PartWatermark => string.IsNullOrWhiteSpace(SelectedPart) ? "Enter part ID to search..." :
                                  "Enter part ID to search...";

    /// <summary>
    /// Dynamic watermark for Operation field - shows error or placeholder
    /// </summary>
    public string OperationWatermark => "Enter operation (optional)...";

    #endregion

    #region Search Criteria Properties

    /// <summary>
    /// Selected part ID for filtering inventory.
    /// Must be a valid part ID from the available options.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDelete))]
    private string? _selectedPart;

    /// <summary>
    /// Selected operation for refined filtering (optional).
    /// Must be a valid operation number if specified.
    /// </summary>
    [ObservableProperty]
    private string? _selectedOperation;

    /// <summary>
    /// Text content for Part AutoCompleteBox.
    /// Synchronized with SelectedPart property.
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Part text is required for search operations")]
    [StringLength(50, ErrorMessage = "Part text cannot exceed 50 characters")]
    private string _partText = string.Empty;

    /// <summary>
    /// Text content for Operation AutoCompleteBox.
    /// Synchronized with SelectedOperation property.
    /// </summary>
    [ObservableProperty]
    [StringLength(10, ErrorMessage = "Operation text cannot exceed 10 characters")]
    private string _operationText = string.Empty;

    #endregion

    #region State Properties

    /// <summary>
    /// Indicates if a background operation is in progress.
    /// When true, prevents user interactions that could cause conflicts.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDelete), nameof(CanUndo))]
    private bool _isLoading;

    /// <summary>
    /// Indicates if there are inventory items to display
    /// </summary>
    public bool HasInventoryItems => InventoryItems.Count > 0;

    /// <summary>
    /// Indicates if search fields should be enabled (when CustomDataGrid is empty)
    /// </summary>
    public bool AreSearchFieldsEnabled => !HasInventoryItems;

    /// <summary>
    /// Indicates if delete operation can be performed (items selected)
    /// </summary>
    public bool CanDelete => SelectedItems.Count > 0 && !IsLoading;

    /// <summary>
    /// Indicates if there are items available for undo operation (delegates to RemoveService)
    /// </summary>
    public bool HasUndoItems => _removeService?.HasUndoItems ?? false;

    /// <summary>
    /// Indicates if undo operation is available
    /// </summary>
    public bool CanUndo => HasUndoItems && !IsLoading;

    #endregion

    #region Note Editor Properties

    /// <summary>
    /// Indicates if the note editor overlay is currently visible
    /// </summary>
    [ObservableProperty]
    private bool _isNoteEditorVisible;

    /// <summary>
    /// Currently selected inventory item for note editing
    /// </summary>
    [ObservableProperty]
    private InventoryItem? _noteEditorItem;

    /// <summary>
    /// Note editor view model instance
    /// </summary>
    [ObservableProperty]
    private NoteEditorViewModel? _noteEditorViewModel;

    /// <summary>
    /// Current edit dialog ViewModel instance for comprehensive inventory editing.
    /// Used for managing the EditInventoryView dialog lifecycle and data binding.
    /// </summary>
    [ObservableProperty]
    private EditInventoryViewModel? _editDialogViewModel;

    /// <summary>
    /// Controls the visibility of the edit dialog overlay.
    /// Used by the UI to show/hide the comprehensive inventory edit dialog.
    /// </summary>
    [ObservableProperty]
    private bool _isEditDialogVisible;

    #endregion

    #region Undo Functionality



    #endregion

    #region Events

    /// <summary>
    /// Event fired when items are successfully removed
    /// </summary>
    public event EventHandler<ItemsRemovedEventArgs>? ItemsRemoved;

    /// <summary>
    /// Event fired when panel toggle is requested
    /// </summary>
    public event EventHandler? PanelToggleRequested;

    /// <summary>
    /// Event fired when advanced removal is requested
    /// </summary>
    public event EventHandler? AdvancedRemovalRequested;

    /// <summary>
    /// Event fired when the success overlay should be shown
    /// </summary>
    public event EventHandler<MTM_WIP_Application_Avalonia.Models.SuccessEventArgs>? ShowSuccessOverlay;

    #endregion

    #region Constructor

    public RemoveItemViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ISuggestionOverlayService suggestionOverlayService,
        ISuccessOverlayService successOverlayService,
        IQuickButtonsService quickButtonsService,
        IRemoveService removeService,
        IServiceProvider serviceProvider,
        ILogger<RemoveItemViewModel> logger,
        IPrintService? printService = null,
        INavigationService? navigationService = null)
        : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _suggestionOverlayService = suggestionOverlayService ?? throw new ArgumentNullException(nameof(suggestionOverlayService));
        _successOverlayService = successOverlayService ?? throw new ArgumentNullException(nameof(successOverlayService));
        _quickButtonsService = quickButtonsService ?? throw new ArgumentNullException(nameof(quickButtonsService));
        _removeService = removeService ?? throw new ArgumentNullException(nameof(removeService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _printService = printService;
        _navigationService = navigationService;

        // Subscribe to RemoveService events
        _removeService.ItemsRemoved += OnItemsRemovedFromService;
        _removeService.LoadingStateChanged += OnLoadingStateChangedFromService;

        // Sync InventoryItems with RemoveService collection
        // Note: In a more complex scenario, we could use CollectionChanged events for two-way sync
        _ = LoadData(); // Load real data from database

        // Setup property change notifications for computed properties
        PropertyChanged += OnPropertyChanged;
    }


    /// <summary>
    /// Handles items removed events from the RemoveService
    /// </summary>
    private void OnItemsRemovedFromService(object? sender, ItemsRemovedEventArgs e)
    {
        // Propagate the event to the UI
        ItemsRemoved?.Invoke(this, e);

        // Update UI state by notifying property changed
        OnPropertyChanged(nameof(HasUndoItems));
        OnPropertyChanged(nameof(CanUndo));

        Logger.LogInformation("Items removed event received from RemoveService: {Count} items", e.RemovedItems.Count);
    }

    /// <summary>
    /// Handles loading state changes from the RemoveService
    /// </summary>
    private void OnLoadingStateChangedFromService(object? sender, bool isLoading)
    {
        IsLoading = isLoading;
        Logger.LogDebug("Loading state changed from RemoveService: {IsLoading}", isLoading);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update computed properties when dependencies change
        switch (e.PropertyName)
        {
            case nameof(SelectedItems):
            case nameof(IsLoading):
                OnPropertyChanged(nameof(CanDelete));
                break;
            case nameof(HasUndoItems):
                OnPropertyChanged(nameof(CanUndo));
                break;
            case nameof(InventoryItems):
                OnPropertyChanged(nameof(HasInventoryItems));
                OnPropertyChanged(nameof(AreSearchFieldsEnabled));
                break;
            case nameof(SelectedPart):
                PartText = SelectedPart ?? string.Empty;
                OnPropertyChanged(nameof(PartWatermark));
                break;
            case nameof(SelectedOperation):
                OperationText = SelectedOperation ?? string.Empty;
                OnPropertyChanged(nameof(OperationWatermark));
                break;
            case nameof(PartText):
                if (!string.IsNullOrEmpty(PartText) && (PartOptions.Contains(PartText) || PartIds.Contains(PartText)))
                    SelectedPart = PartText;
                break;
            case nameof(OperationText):
                if (!string.IsNullOrEmpty(OperationText) && (OperationOptions.Contains(OperationText) || Operations.Contains(OperationText)))
                    SelectedOperation = OperationText;
                break;
        }
    }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Opens advanced removal features
    /// </summary>
    [RelayCommand]
    private void AdvancedRemoval()
    {
        AdvancedRemovalRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Toggles quick actions panel
    /// </summary>
    [RelayCommand]
    private void TogglePanel()
    {
        PanelToggleRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Executes inventory search based on selected criteria using RemoveService
    /// </summary>
    [RelayCommand]
    private async Task Search()
    {
        try
        {
            Logger.LogInformation("Executing search via RemoveService for Part: {PartId}, Operation: {Operation}",
                SelectedPart, SelectedOperation);

            // Delegate search to RemoveService
            var result = await _removeService.SearchInventoryAsync(
                SelectedPart,
                SelectedOperation,
                location: null,
                user: null
            ).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                // Synchronize search results from RemoveService to ViewModel's collection
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    InventoryItems.Clear();
                    if (result.Value != null)
                    {
                        Logger.LogDebug("Adding {Count} items to InventoryItems collection", result.Value.Count);
                        foreach (var item in result.Value)
                        {
                            Logger.LogDebug("Adding item - ID: {ID}, PartID: {PartID}, Location: {Location}, Operation: {Operation}, Quantity: {Quantity}",
                                item.Id, item.PartId, item.Location, item.Operation, item.Quantity);
                            InventoryItems.Add(item);
                        }
                        Logger.LogDebug("InventoryItems collection now has {Count} items", InventoryItems.Count);
                    }

                    // Manually trigger HasInventoryItems property change notification
                    OnPropertyChanged(nameof(HasInventoryItems));
                    OnPropertyChanged(nameof(AreSearchFieldsEnabled));
                    Logger.LogDebug("HasInventoryItems property changed, value: {HasItems}", HasInventoryItems);
                });

                Logger.LogInformation("Search completed successfully: {Count} items found", result.Value?.Count ?? 0);
            }
            else
            {
                Logger.LogError("Search failed: {Message}", result.Message);
                throw new InvalidOperationException($"Search failed: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to execute search operation");
            throw new ApplicationException("Search operation failed", ex);
        }
    }

    /// <summary>
    /// Resets search criteria and refreshes all data using RemoveService
    /// </summary>
    [RelayCommand]
    private async Task Reset()
    {
        try
        {
            Logger.LogInformation("Resetting search criteria and clearing CustomDataGrid via RemoveService");

            // Clear search criteria
            SelectedPart = null;
            SelectedOperation = null;
            PartText = string.Empty;
            OperationText = string.Empty;

            // Clear the InventoryItems collection to reset CustomDataGrid display
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                InventoryItems.Clear();
                SelectedItems.Clear();
                SelectedItem = null;
            });

            // Use RemoveService to refresh inventory data (clears RemoveService collection too)
            var result = await _removeService.RefreshInventoryAsync().ConfigureAwait(false);

            if (result.IsSuccess)
            {
                Logger.LogInformation("Reset completed successfully: {Message}", result.Message);
            }
            else
            {
                Logger.LogError("Reset failed: {Message}", result.Message);
                throw new InvalidOperationException($"Reset failed: {result.Message}");
            }

            // Reload master data
            await LoadData().ConfigureAwait(false);

            // Manually trigger property change notifications
            OnPropertyChanged(nameof(HasInventoryItems));
            OnPropertyChanged(nameof(CanDelete));

            Logger.LogInformation("Search criteria reset and CustomDataGrid cleared successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to reset search criteria and clear CustomDataGrid");
            throw new ApplicationException("Failed to reset inventory data", ex);
        }
    }

    /// <summary>
    /// Batch deletes selected items using RemoveService with comprehensive transaction logging
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task Delete()
    {
        if (SelectedItems.Count == 0)
        {
            Logger.LogWarning("Delete operation attempted with no items selected");
            throw new InvalidOperationException("No items selected for deletion");
        }

        try
        {
            Logger.LogInformation("Initiating batch delete operation via RemoveService for {Count} items", SelectedItems.Count);

            // Delegate to RemoveService for business logic
            var result = await _removeService.RemoveInventoryItemsAsync(
                SelectedItems.ToList(),
                _applicationState.CurrentUser,
                "Removed via RemoveTabView batch operation"
            ).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                var removalResult = result.Value!;

                // Remove deleted items from ViewModel's collection and clear selection
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // Remove successfully deleted items from the InventoryItems collection
                    foreach (var removedItem in removalResult.SuccessfulRemovals)
                    {
                        InventoryItems.Remove(removedItem);
                    }

                    // Clear selection since items were deleted
                    SelectedItems.Clear();
                    SelectedItem = null;
                });

                // Clean up cached ViewModels for removed items to prevent memory leaks
                CleanupRemovedViewModels(removalResult.SuccessfulRemovals);

                // Show success overlay for successful operations
                if (removalResult.HasSuccesses)
                {
                    var successMessage = removalResult.SuccessCount == 1
                        ? "Successfully removed inventory item"
                        : $"Successfully removed {removalResult.SuccessCount} inventory items";

                    var detailsText = removalResult.SuccessCount == 1
                        ? $"Part ID: {removalResult.SuccessfulRemovals[0].PartId}\nOperation: {removalResult.SuccessfulRemovals[0].Operation}\nLocation: {removalResult.SuccessfulRemovals[0].Location}\nQuantity: {removalResult.SuccessfulRemovals[0].Quantity}"
                        : $"Batch operation completed\nItems removed: {removalResult.SuccessCount}\nTotal quantity removed: {removalResult.SuccessfulRemovals.Sum(x => x.Quantity)}";

                    // Fire SuccessOverlay event for View to handle
                    var successArgs = new MTM_WIP_Application_Avalonia.Models.SuccessEventArgs
                    {
                        Message = successMessage,
                        Details = detailsText,
                        IconKind = "CheckCircle",
                        Duration = 4000, // 4 seconds for removal confirmation
                        SuccessTime = DateTime.Now
                    };

                    ShowSuccessOverlay?.Invoke(this, successArgs);

                    // Also use SuccessOverlay service directly
                    try
                    {
                        if (_successOverlayService != null)
                        {
                            _ = _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                                null, // Auto-resolve MainView
                                successMessage,
                                detailsText,
                                "CheckCircle",
                                4000 // 4 seconds total
                            );
                            Logger.LogInformation("Success overlay started for inventory removal operation");
                        }
                    }
                    catch (Exception overlayEx)
                    {
                        Logger.LogWarning(overlayEx, "Failed to show success overlay via service");
                    }
                }

                // Report any failures
                if (removalResult.HasFailures)
                {
                    var failureMessage = $"Failed to remove {removalResult.FailureCount} items:\n" +
                        string.Join("\n", removalResult.Failures.Select(f => $"• {f.Item.PartId}: {f.Error}"));
                    Logger.LogWarning("Batch deletion had failures: {FailureMessage}", failureMessage);
                }

                Logger.LogInformation("Delete operation completed successfully via RemoveService: {SuccessCount} successful, {FailureCount} failed",
                    removalResult.SuccessCount, removalResult.FailureCount);
            }
            else
            {
                Logger.LogError("Delete operation failed: {Message}", result.Message);
                throw new InvalidOperationException($"Delete operation failed: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error during batch inventory deletion");
            throw new ApplicationException($"Failed to delete inventory items: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Restores last deleted items using RemoveService undo functionality
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    private async Task Undo()
    {
        try
        {
            Logger.LogInformation("Initiating undo operation via RemoveService");

            // Delegate to RemoveService for undo functionality
            var result = await _removeService.UndoLastRemovalAsync(_applicationState.CurrentUser).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                var restoreResult = result.Value!;

                Logger.LogInformation("Undo operation completed successfully via RemoveService: {SuccessCount} restored, {FailureCount} failed",
                    restoreResult.SuccessCount, restoreResult.FailureCount);

                // Update UI state - RemoveService handles collection updates, notify property changes
                OnPropertyChanged(nameof(HasUndoItems));
                OnPropertyChanged(nameof(CanUndo));

                // Report any failures
                if (restoreResult.HasFailures)
                {
                    var failureMessage = $"Failed to restore {restoreResult.FailureCount} items:\n" +
                        string.Join("\n", restoreResult.Failures.Select(f => $"• {f.Item.PartId}: {f.Error}"));
                    Logger.LogWarning("Undo operation had failures: {FailureMessage}", failureMessage);
                }
            }
            else
            {
                Logger.LogError("Undo operation failed: {Message}", result.Message);
                throw new InvalidOperationException($"Undo operation failed: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error during undo operation");
            throw new ApplicationException($"Failed to undo removal: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deletes a single inventory item from DataGrid action button.
    /// Used by CustomDataGrid DeleteItemCommand for individual row delete operations.
    /// </summary>
    [RelayCommand]
    private async Task DeleteSingleItem(InventoryItem? item)
    {
        if (item == null)
        {
            Logger.LogWarning("DeleteSingleItem called with null item");
            return;
        }

        try
        {
            Logger.LogInformation("Initiating single item delete operation via RemoveService for item: {PartId}", item.PartId);

            // Add timeout to prevent UI freeze
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // 30 second timeout

            // Delegate to RemoveService for business logic with timeout
            var removeTask = _removeService.RemoveInventoryItemAsync(
                item,
                _applicationState.CurrentUser,
                "Removed via CustomDataGrid action button"
            );

            var result = await removeTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                var removalResult = result.Value!;

                // Remove the item from ViewModel's collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    InventoryItems.Remove(item);

                    // Also remove from selected items if it was selected
                    if (SelectedItems.Contains(item))
                    {
                        SelectedItems.Remove(item);
                    }

                    // Clear selected item if it was the one deleted
                    if (SelectedItem == item)
                    {
                        SelectedItem = null;
                    }
                });

                // Clean up cached ViewModel for removed item to prevent memory leaks
                CleanupRemovedViewModels(new[] { item });

                // Show success overlay for successful operation
                if (removalResult.HasSuccesses)
                {
                    var message = "Successfully removed inventory item";
                    var details = $"Part ID: {item.PartId}\nOperation: {item.Operation}\nLocation: {item.Location}\nQuantity: {item.Quantity}";

                    // Trigger success overlay if service is available
                    if (_successOverlayService != null)
                    {
                        await _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                            null, // sourceControl - will find MainView
                            message,
                            details,
                            "CheckCircle",
                            4000 // 4 seconds for removal confirmation
                        );
                    }

                    Logger.LogInformation("Single item delete operation completed successfully via RemoveService");
                }

                // Update UI state - ViewModel collection updated, notify property changes
                OnPropertyChanged(nameof(HasInventoryItems));
                OnPropertyChanged(nameof(CanDelete));
                OnPropertyChanged(nameof(HasUndoItems));
                OnPropertyChanged(nameof(CanUndo));
            }
            else
            {
                Logger.LogError("Single item delete operation failed: {Message}", result.Message);

                // Handle delete failure gracefully without throwing
                await ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Delete operation failed: {result.Message}"),
                    $"Failed to delete inventory item {item.PartId}",
                    _applicationState.CurrentUser
                ).ConfigureAwait(false);

                // Show error overlay to user
                if (_successOverlayService != null)
                {
                    await _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                        null,
                        "Delete Failed",
                        $"Could not delete {item.PartId}: {result.Message}",
                        "AlertCircle", // Error icon
                        5000, // 5 seconds
                        true // isError = true
                    );
                }

                return; // Exit without throwing
            }
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning("Delete operation timed out for item: {PartId}", item.PartId);

            await ErrorHandling.HandleErrorAsync(
                new TimeoutException("Delete operation timed out after 30 seconds"),
                $"Delete timeout for item {item.PartId}",
                _applicationState.CurrentUser
            ).ConfigureAwait(false);

            // Show timeout error overlay
            if (_successOverlayService != null)
            {
                await _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                    null,
                    "Delete Timeout",
                    $"Delete operation for {item.PartId} timed out. Please try again.",
                    "ClockAlert", // Timeout icon
                    5000, // 5 seconds
                    true // isError = true
                );
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error during single item deletion for {PartId}", item.PartId);

            // Don't throw - handle gracefully with user notification
            await ErrorHandling.HandleErrorAsync(ex,
                $"Failed to delete inventory item {item.PartId}",
                _applicationState.CurrentUser
            ).ConfigureAwait(false);

            // Show error overlay to user using success overlay with error icon
            if (_successOverlayService != null)
            {
                await _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                    null,
                    "Delete Error",
                    $"Unexpected error deleting {item.PartId}: {ex.Message}",
                    "AlertCircle", // Error icon
                    5000, // 5 seconds
                    true // isError = true
                );
            }
        }
    }

    /// <summary>
    /// Prints current inventory view with formatted output using Print Service
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasInventoryItems))]
    private async Task Print()
    {
        try
        {
            if (_printService == null || _navigationService == null)
            {
                Logger.LogWarning("Print service or navigation service not available");
                return;
            }

            IsLoading = true;
            Logger.LogInformation("Initiating print operation for {Count} inventory items", InventoryItems.Count);

            // Convert inventory items to DataTable for printing
            var dataTable = ConvertInventoryToDataTable(InventoryItems);

            // Get or create PrintViewModel
            var printViewModel = _serviceProvider.GetService<PrintViewModel>();
            if (printViewModel == null)
            {
                Logger.LogError("PrintViewModel not available from DI container");
                return;
            }

            // Configure print data
            printViewModel.PrintData = dataTable;
            printViewModel.DataSourceType = MTM_WIP_Application_Avalonia.Models.PrintDataSourceType.Remove;
            printViewModel.DocumentTitle = "Inventory Removal Report";
            printViewModel.OriginalViewContext = this; // Store current context for navigation back

            // Create and navigate to PrintView
            var printView = new PrintView
            {
                DataContext = printViewModel
            };

            // Initialize print view with data
            await printViewModel.InitializeAsync();

            // Navigate to print view using NavigationService
            _navigationService.NavigateTo(printView);

            Logger.LogInformation("Navigated to print view with {Count} inventory items", InventoryItems.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initiating print operation");
            await MTM_WIP_Application_Avalonia.Services.ErrorHandling.HandleErrorAsync(ex, "Failed to open print interface", Environment.UserName);
        }
    }

    /// <summary>
    /// Opens the note editor overlay for the specified inventory item.
    /// Used by CustomDataGrid EditItemCommand for individual row editing (including notes).
    /// </summary>
    [RelayCommand]
    private async Task ReadNote(InventoryItem? item)
    {
        if (item == null)
        {
            Logger.LogWarning("ReadNote called with null item");
            return;
        }

        try
        {
            Logger.LogInformation("Opening note editor for item: PartID={PartId}, Operation={Operation}, Location={Location}",
                item.PartId, item.Operation, item.Location);

            // Store the item being edited
            NoteEditorItem = item;

            // Create and configure note editor ViewModel
            var noteEditorViewModel = _serviceProvider.GetService<NoteEditorViewModel>();
            if (noteEditorViewModel == null)
            {
                Logger.LogError("NoteEditorViewModel not available from DI container");
                await MTM_WIP_Application_Avalonia.Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException("Note editor not available"),
                    "Note Editor Error",
                    _applicationState.CurrentUser);
                return;
            }

            // Initialize with inventory item data - using full parameter version for proper stored procedure call
            await noteEditorViewModel.InitializeAsync(
                item.Id,
                item.PartId ?? string.Empty,
                item.Operation ?? string.Empty,
                item.Location ?? string.Empty,
                item.Notes ?? string.Empty,
                item.BatchNumber ?? string.Empty,
                item.User ?? "SYSTEM",
                isReadOnly: false // Allow editing by default
            );

            // Subscribe to note edit completion event
            noteEditorViewModel.NoteEditCompleted -= OnNoteEditCompleted;
            noteEditorViewModel.NoteEditCompleted += OnNoteEditCompleted;

            // Set the ViewModel and show the overlay
            NoteEditorViewModel = noteEditorViewModel;
            IsNoteEditorVisible = true;

            Logger.LogInformation("Note editor overlay opened successfully for {PartId}", item.PartId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to open note editor for item: {PartId}", item?.PartId);
            await MTM_WIP_Application_Avalonia.Services.ErrorHandling.HandleErrorAsync(ex, "Note Editor Error", _applicationState.CurrentUser);
        }
    }

    /// <summary>
    /// Handles note edit completion from the NoteEditorViewModel
    /// </summary>
    private async void OnNoteEditCompleted(object? sender, NoteEditorResult e)
    {
        try
        {
            Logger.LogInformation("Note edit completed for inventory ID={InventoryId}, Success={Success}",
                e.InventoryId, e.Success);

            if (e.Success && NoteEditorItem != null)
            {
                // Update the inventory item's Notes property
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (NoteEditorItem != null)
                    {
                        NoteEditorItem.Notes = e.UpdatedNote ?? string.Empty;
                        Logger.LogDebug("Updated inventory item notes for {PartId}", NoteEditorItem.PartId);
                    }
                });

                // Optionally refresh the search results to show updated data from database
                if (!string.IsNullOrWhiteSpace(SelectedPart))
                {
                    Logger.LogDebug("Refreshing search results to reflect database changes");
                    await Search().ConfigureAwait(false);
                }
            }
            else if (!e.Success)
            {
                Logger.LogWarning("Note edit was cancelled or failed for inventory ID={InventoryId}", e.InventoryId);
            }

            // Close the overlay
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                IsNoteEditorVisible = false;
                NoteEditorItem = null;
                NoteEditorViewModel = null;
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling note edit completion");
            await MTM_WIP_Application_Avalonia.Services.ErrorHandling.HandleErrorAsync(ex, "Note Editor Error", _applicationState.CurrentUser);
        }
    }

    /// <summary>
    /// Command to edit an inventory item with comprehensive editing dialog.
    /// Opens the EditInventoryView dialog for full field editing with validation.
    /// Uses cached ViewModel instances per inventory item ID to preserve data across multiple openings.
    /// </summary>
    [RelayCommand]
    private async Task EditItem(InventoryItem? item)
    {
        // CRITICAL: Log at the very start to ensure this method is being called
        Logger.LogCritical("🚨 QA CRITICAL: EditItem method called with item: {PartId} (ID: {Id})", item?.PartId ?? "NULL", item?.Id ?? -1);

        if (item == null)
        {
            Logger.LogWarning("EditItem called with null item");
            return;
        }

        try
        {
            Logger.LogInformation("Opening comprehensive edit dialog for item: PartID={PartId}, Operation={Operation}, Location={Location}, ID={Id}",
                item.PartId, item.Operation, item.Location, item.Id);

            // Get or create EditInventoryViewModel from cache based on inventory item ID
            EditInventoryViewModel editViewModel;
            if (_editViewModelCache.TryGetValue(item.Id, out var cachedViewModel))
            {
                Logger.LogInformation("🔍 QA REMOVE: Using CACHED EditInventoryViewModel for inventory ID {Id} with PartId {PartId}", item.Id, item.PartId);
                editViewModel = cachedViewModel;
            }
            else
            {
                Logger.LogInformation("🔍 QA REMOVE: Creating NEW EditInventoryViewModel for inventory ID {Id} with PartId {PartId}", item.Id, item.PartId);
                editViewModel = _serviceProvider.GetRequiredService<EditInventoryViewModel>();
                _editViewModelCache[item.Id] = editViewModel;

                // Subscribe to dialog events (only for new ViewModels)
                editViewModel.DialogClosed += OnEditDialogClosed;
                editViewModel.InventorySaved += OnInventoryItemSaved;
            }

            // CRITICAL FIX: Always initialize/refresh the ViewModel with current inventory item data
            // This ensures cached ViewModels display correct data when switching between different items
            Logger.LogInformation("🔍 QA REMOVE: About to call InitializeAsync for inventory ID {Id} with BatchNumber {BatchNumber}", item.Id, item.BatchNumber);
            await editViewModel.InitializeAsync(item);
            Logger.LogInformation("🔍 QA REMOVE: InitializeAsync completed for inventory ID {Id}", item.Id);

            // Show the edit dialog using the cached (or newly created) ViewModel
            EditDialogViewModel = editViewModel;
            IsEditDialogVisible = true;

            Logger.LogInformation("Edit dialog opened successfully for {PartId} using cached ViewModel", item.PartId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to open edit dialog for item: {PartId}", item?.PartId);
            await MTM_WIP_Application_Avalonia.Services.ErrorHandling.HandleErrorAsync(ex, "Edit Dialog Error", _applicationState.CurrentUser);
        }
    }

    /// <summary>
    /// Handles edit dialog closure event.
    /// Preserves cached ViewModel instances for reuse across multiple dialog openings.
    /// </summary>
    private void OnEditDialogClosed(object? sender, EventArgs e)
    {
        try
        {
            Logger.LogInformation("Edit dialog closed");

            // Clean up UI state but preserve ViewModel in cache
            if (EditDialogViewModel != null)
            {
                // Call cleanup to reset UI state while preserving data
                EditDialogViewModel.Cleanup();

                // NOTE: Do NOT unsubscribe events or set ViewModel to null
                // The ViewModel remains cached for reuse with preserved data
                Logger.LogDebug("EditInventoryViewModel cleaned up but preserved in cache");
            }

            // Hide the dialog overlay
            EditDialogViewModel = null;
            IsEditDialogVisible = false;

            // NOTE: Do NOT refresh data here - selective updates are handled by OnInventoryItemSaved()
            Logger.LogDebug("Edit dialog closed successfully - search results preserved, ViewModel cached");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling edit dialog closure");
        }
    }

    /// <summary>
    /// Handles successful inventory item save event.
    /// Updates the specific edited row in CustomDataGrid instead of full data refresh.
    /// </summary>
    private async void OnInventoryItemSaved(object? sender, InventorySavedEventArgs e)
    {
        try
        {
            var partId = e.SavedItem?.PartId ?? "Unknown";
            Logger.LogInformation("Inventory item saved successfully: {PartId}, updating specific grid row", partId);

            if (e.SavedItem != null)
            {
                // Find and update the specific item in the collection instead of full refresh
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var existingItem = InventoryItems.FirstOrDefault(item =>
                        item.PartId == e.SavedItem.PartId &&
                        item.Operation == e.SavedItem.Operation &&
                        item.Location == e.SavedItem.Location);

                    if (existingItem != null)
                    {
                        // Update the existing item's properties with saved data
                        var index = InventoryItems.IndexOf(existingItem);
                        if (index >= 0)
                        {
                            // Replace the item to trigger ObservableCollection notifications
                            InventoryItems[index] = e.SavedItem;
                            Logger.LogDebug("Updated InventoryItems collection at index {Index} with edited data", index);
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Could not find item to update in InventoryItems collection: {PartId} {Operation} {Location}",
                            e.SavedItem.PartId, e.SavedItem.Operation, e.SavedItem.Location);
                    }
                });

                // Show success message using the correct service method signature
                await _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                    null, // Control parameter - not needed for this usage
                    $"Successfully updated inventory item: {e.SavedItem?.PartId ?? e.PartId}",
                    "Item details updated successfully",
                    "CheckCircle", // Icon kind
                    2500 // Duration in milliseconds
                );
            }
            else
            {
                Logger.LogWarning("SavedItem was null in InventorySavedEventArgs, falling back to full refresh");
                await RefreshCurrentData();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling inventory item save completion");
            await MTM_WIP_Application_Avalonia.Services.ErrorHandling.HandleErrorAsync(ex, "Data Update Error", _applicationState.CurrentUser);
        }
    }

    /// <summary>
    /// Refreshes the current inventory data to reflect any changes.
    /// </summary>
    private async Task RefreshCurrentData()
    {
        try
        {
            // If we have current search criteria, re-run the search
            if (!string.IsNullOrWhiteSpace(SelectedPart) || !string.IsNullOrWhiteSpace(SelectedOperation))
            {
                Logger.LogDebug("Refreshing search results with current criteria");
                await Search();
            }
            else
            {
                Logger.LogDebug("No active search criteria, clearing results");
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    InventoryItems.Clear();
                });
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to refresh inventory data");
        }
    }

    #endregion

    #region Data Loading

    /// <summary>
    /// Loads ComboBox data from database using stored procedures with RemoveService integration
    /// </summary>
    [RelayCommand]
    private async Task LoadData()
    {
        try
        {
            using var scope = Logger.BeginScope("DataLoading");
            Logger.LogInformation("Loading ComboBox data from database");

            // Load Parts using md_part_ids_Get_All stored procedure
            Logger.LogDebug("Calling md_part_ids_Get_All stored procedure");
            var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            ).ConfigureAwait(false);

            Logger.LogDebug("md_part_ids_Get_All result: IsSuccess={IsSuccess}, RowCount={RowCount}",
                partResult.IsSuccess, partResult.Data?.Rows.Count ?? 0);

            if (partResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    PartOptions.Clear();
                    PartIds.Clear(); // InventoryTabView pattern
                    foreach (System.Data.DataRow row in (partResult.Data?.Rows ?? new System.Data.DataTable().Rows))
                    {
                        var partId = row["PartID"]?.ToString();
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartOptions.Add(partId);
                            PartIds.Add(partId); // InventoryTabView pattern
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} parts", PartOptions.Count);
            }
            else
            {
                Logger.LogError("Failed to load parts from md_part_ids_Get_All: {Message}", partResult.Message);
                Logger.LogInformation("Loading sample part data as fallback");
                await LoadSampleDataAsync().ConfigureAwait(false);
            }

            // Load Operations using md_operation_numbers_Get_All stored procedure
            Logger.LogDebug("Calling md_operation_numbers_Get_All stored procedure");
            var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_operation_numbers_Get_All",
                new Dictionary<string, object>()
            ).ConfigureAwait(false);

            Logger.LogDebug("md_operation_numbers_Get_All result: IsSuccess={IsSuccess}, RowCount={RowCount}",
                operationResult.IsSuccess, operationResult.Data?.Rows.Count ?? 0);

            if (operationResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    OperationOptions.Clear();
                    Operations.Clear(); // InventoryTabView pattern
                    foreach (System.Data.DataRow row in (operationResult.Data?.Rows ?? new System.Data.DataTable().Rows))
                    {
                        var operation = row["Operation"]?.ToString();
                        if (!string.IsNullOrEmpty(operation))
                        {
                            OperationOptions.Add(operation);
                            Operations.Add(operation); // InventoryTabView pattern
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} operations", OperationOptions.Count);
            }
            else
            {
                Logger.LogError("Failed to load operations from md_operation_numbers_Get_All: {Message}", operationResult.Message);
                Logger.LogInformation("Loading sample operation data as fallback");
                await LoadSampleDataAsync().ConfigureAwait(false);
            }

            Logger.LogInformation("ComboBox data loaded successfully - Parts: {PartCount}, Operations: {OperationCount}",
                PartOptions.Count, OperationOptions.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load ComboBox data from database");
            throw new ApplicationException("Failed to initialize application data", ex);
        }
    }

    /// <summary>
    /// Loads sample data for demonstration purposes
    /// </summary>
    private Task LoadSampleDataAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            // Clear existing data
            PartOptions.Clear();
            PartIds.Clear(); // InventoryTabView pattern
            OperationOptions.Clear();
            Operations.Clear(); // InventoryTabView pattern

            // Sample parts
            var sampleParts = new[] { "PART001", "PART002", "PART003", "PART004", "PART005" };
            foreach (var part in sampleParts)
            {
                PartOptions.Add(part);
                PartIds.Add(part); // InventoryTabView pattern
            }

            // Sample operations (MTM uses string numbers)
            var sampleOperations = new[] { "90", "100", "110", "120", "130" };
            foreach (var operation in sampleOperations)
            {
                OperationOptions.Add(operation);
                Operations.Add(operation); // InventoryTabView pattern
            }
        });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads sample inventory data for demonstration
    /// </summary>
    private Task LoadSampleInventoryDataAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var sampleItems = new[]
            {
                new InventoryItem
                {
                    Id = 1,
                    PartId = "PART001",
                    Operation = "100",
                    Location = "WC01",
                    Quantity = 25,
                    Notes = "Ready for next operation",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddHours(-2)
                },
                new InventoryItem
                {
                    Id = 2,
                    PartId = "PART001",
                    Operation = "110",
                    Location = "WC02",
                    Quantity = 15,
                    Notes = "Quality check required",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddHours(-1)
                },
                new InventoryItem
                {
                    Id = 3,
                    PartId = "PART002",
                    Operation = "90",
                    Location = "WC01",
                    Quantity = 40,
                    Notes = "Incoming from supplier",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddMinutes(-30)
                }
            };

            // Filter sample data based on search criteria
            var filteredItems = sampleItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SelectedPart))
            {
                filteredItems = filteredItems.Where(item =>
                    item.PartId.Equals(SelectedPart, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedOperation))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Operation?.Equals(SelectedOperation, StringComparison.OrdinalIgnoreCase) == true);
            }

            foreach (var item in filteredItems)
            {
                InventoryItems.Add(item);
            }
        });
        return Task.CompletedTask;
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Handles exceptions from command operations
    /// </summary>
    /// <summary>
    /// Handles exceptions with user-friendly error presentation
    /// </summary>
    private void HandleException(Exception ex)
    {
        Logger.LogError(ex, "Error in RemoveItemViewModel operation");

        // Present user-friendly error message via centralized error service
        _ = MTM_WIP_Application_Avalonia.Services.ErrorHandling.HandleErrorAsync(ex, "Remove Operation", _applicationState.CurrentUser);

        // Update UI state to reflect error
        // Note: StatusMessage property may need to be added to this ViewModel for UI feedback
        Logger.LogInformation("User-friendly error message: {Message}", GetUserFriendlyErrorMessage(ex));
    }

    /// <summary>
    /// Gets a user-friendly error message based on the exception type
    /// </summary>
    private string GetUserFriendlyErrorMessage(Exception ex) => ex switch
    {
        InvalidOperationException => "The removal operation could not be completed. Please verify the item details and try again.",
        TimeoutException => "The removal operation timed out. Please check your connection and try again.",
        UnauthorizedAccessException => "You do not have permission to perform this removal operation.",
        ArgumentException => "Invalid removal details provided. Please check your input and try again.",
        _ => "An unexpected error occurred during the removal operation. Please contact support if this continues."
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Programmatically triggers a search operation
    /// </summary>
    public async Task TriggerSearchAsync()
    {
        if (SearchCommand.CanExecute(null))
        {
            await SearchCommand.ExecuteAsync(null);
        }
    }

    /// <summary>
    /// Selects all visible inventory items
    /// </summary>
    public void SelectAllItems()
    {
        SelectedItems.Clear();
        foreach (var item in InventoryItems)
        {
            SelectedItems.Add(item);
        }
    }

    /// <summary>
    /// Clears all selected items
    /// </summary>
    public void ClearSelection()
    {
        SelectedItem = null;
        SelectedItems.Clear();
    }

    /// <summary>
    /// Converts inventory items to DataTable for print service
    /// </summary>
    private DataTable ConvertInventoryToDataTable(ObservableCollection<InventoryItem> inventoryItems)
    {
        var dataTable = new DataTable();

        // Define columns based on InventoryItem properties
        dataTable.Columns.Add("PartId", typeof(string));
        dataTable.Columns.Add("Operation", typeof(string));
        dataTable.Columns.Add("Location", typeof(string));
        dataTable.Columns.Add("Quantity", typeof(int));
        dataTable.Columns.Add("Notes", typeof(string));
        dataTable.Columns.Add("LastUpdated", typeof(DateTime));
        dataTable.Columns.Add("LastUpdatedBy", typeof(string));

        // Add rows
        foreach (var item in inventoryItems)
        {
            var row = dataTable.NewRow();
            row["PartId"] = item.PartId ?? string.Empty;
            row["Operation"] = item.Operation ?? string.Empty;
            row["Location"] = item.Location ?? string.Empty;
            row["Quantity"] = item.Quantity;
            row["Notes"] = item.Notes ?? string.Empty;
            row["LastUpdated"] = item.LastUpdated;
            row["LastUpdatedBy"] = item.User ?? string.Empty;
            dataTable.Rows.Add(row);
        }

        Logger.LogDebug("Converted {ItemCount} inventory items to DataTable with {ColumnCount} columns",
            inventoryItems.Count, dataTable.Columns.Count);

        return dataTable;
    }

    #endregion

    #region SuggestionOverlay Integration Methods

    /// <summary>
    /// Cleans up cached EditInventoryViewModels for removed inventory items.
    /// Prevents memory leaks by disposing ViewModels that are no longer needed.
    /// </summary>
    private void CleanupRemovedViewModels(IEnumerable<InventoryItem> removedItems)
    {
        try
        {
            foreach (var item in removedItems)
            {
                if (_editViewModelCache.TryGetValue(item.Id, out var cachedViewModel))
                {
                    Logger.LogDebug("Removing cached EditInventoryViewModel for inventory ID {Id}", item.Id);

                    // Unsubscribe from events
                    cachedViewModel.DialogClosed -= OnEditDialogClosed;
                    cachedViewModel.InventorySaved -= OnInventoryItemSaved;

                    // Remove from cache
                    _editViewModelCache.Remove(item.Id);
                }
            }

            Logger.LogDebug("ViewModel cache cleanup completed. Cache size: {CacheSize}", _editViewModelCache.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during ViewModel cache cleanup");
        }
    }

    #endregion

    #region SuggestionOverlay Integration Methods

    /// <summary>
    /// Shows part ID suggestions using the RemoveService and SuggestionOverlay service
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowPartSuggestionsAsync(Control targetControl, string userInput)
    {
        try
        {
            Logger.LogDebug("Showing part suggestions for input: {Input}", userInput);

            // Get suggestions from RemoveService
            var suggestions = await _removeService.GetPartSuggestionsAsync(userInput).ConfigureAwait(false);

            if (suggestions.Any())
            {
                Logger.LogDebug("RemoveService provided {Count} part suggestions", suggestions.Count);
                return await _suggestionOverlayService.ShowSuggestionsAsync(targetControl, suggestions, userInput);
            }
            else
            {
                // Fall back to local collections if RemoveService doesn't have suggestions
                Logger.LogDebug("Falling back to local PartOptions collection ({Count} items)", PartOptions.Count);
                return await _suggestionOverlayService.ShowSuggestionsAsync(targetControl, PartOptions, userInput);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to show part suggestions");
            return null;
        }
    }

    /// <summary>
    /// Shows operation suggestions using the SuggestionOverlay service
    /// <summary>
    /// Shows operation suggestions using the RemoveService and SuggestionOverlay service
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowOperationSuggestionsAsync(Control targetControl, string userInput)
    {
        try
        {
            Logger.LogDebug("Showing operation suggestions for input: {Input}", userInput);

            // Get suggestions from RemoveService
            var suggestions = await _removeService.GetOperationSuggestionsAsync(userInput).ConfigureAwait(false);

            if (suggestions.Any())
            {
                Logger.LogDebug("RemoveService provided {Count} operation suggestions", suggestions.Count);
                return await _suggestionOverlayService.ShowSuggestionsAsync(targetControl, suggestions, userInput);
            }
            else
            {
                // Fall back to local collections if RemoveService doesn't have suggestions
                Logger.LogDebug("Falling back to local OperationOptions collection ({Count} items)", OperationOptions.Count);
                return await _suggestionOverlayService.ShowSuggestionsAsync(targetControl, OperationOptions, userInput);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to show operation suggestions");
            return null;
        }
    }

    /// <summary>
    /// Handles QuickButton integration for field population
    /// </summary>
    /// <param name="partId">Part ID from QuickButton</param>
    /// <param name="operation">Operation from QuickButton</param>
    /// <param name="location">Location from QuickButton (ignored)</param>
    public void PopulateFromQuickButton(string? partId, string? operation, string? location)
    {
        try
        {
            Logger.LogInformation("Populating fields from QuickButton: Part={PartId}, Operation={Operation}",
                partId, operation);

            if (!string.IsNullOrWhiteSpace(partId))
            {
                SelectedPart = partId; // InventoryTabView pattern
            }

            if (!string.IsNullOrWhiteSpace(operation))
            {
                SelectedOperation = operation; // InventoryTabView pattern
            }

            // Auto-execute search after populating fields
            _ = Search();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to populate fields from QuickButton");
        }
    }

    #endregion
}

