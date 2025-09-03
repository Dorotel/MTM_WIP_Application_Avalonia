using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// QuickButtonsViewModel manages the quick action buttons that provide shortcuts
/// to frequently used inventory operations. Uses MVVM Community Toolkit for 
/// property and command management with comprehensive database integration.
/// </summary>
public partial class QuickButtonsViewModel : BaseViewModel
{
    private readonly IQuickButtonsService _quickButtonsService;
    private readonly IProgressService _progressService;
    private readonly IApplicationStateService _applicationState;
    private bool _isUpdatingFromService = false; // Flag to prevent reload loops

    // Observable collections
    public ObservableCollection<QuickButtonItemViewModel> QuickButtons { get; } = new();

    // Event to notify the parent about quick action execution
    public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

    /// <summary>
    /// Gets the count of non-empty quick buttons for display
    /// </summary>
    public int NonEmptyQuickButtonsCount => QuickButtons.Count(button => !button.IsEmpty);

    /// <summary>
    /// Gets the non-empty quick buttons for display
    /// </summary>
    public IEnumerable<QuickButtonItemViewModel> NonEmptyQuickButtons => 
        QuickButtons.Where(button => !button.IsEmpty);

    /// <summary>
    /// Gets whether any button can move up
    /// </summary>
    public bool CanAnyButtonMoveUp => QuickButtons.Any(CanMoveButtonUp);

    /// <summary>
    /// Gets whether any button can move down
    /// </summary>
    public bool CanAnyButtonMoveDown => QuickButtons.Any(CanMoveButtonDown);

    public QuickButtonsViewModel(
        IQuickButtonsService quickButtonsService,
        IProgressService progressService,
        IApplicationStateService applicationState,
        ILogger<QuickButtonsViewModel> logger) : base(logger)
    {
        _quickButtonsService = quickButtonsService ?? throw new ArgumentNullException(nameof(quickButtonsService));
        _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        // Add console output to ensure we can see constructor execution
        Console.WriteLine("🔧🔧🔧 QuickButtonsViewModel constructor STARTED");
        System.Diagnostics.Debug.WriteLine("🔧🔧🔧 QuickButtonsViewModel constructor STARTED");

        Logger.LogInformation("QuickButtonsViewModel initialized with dependency injection");
        Logger.LogInformation("🔧 QuickButtonsViewModel constructor started");

        // Check current user immediately
        var currentUser = _applicationState.CurrentUser;
        Console.WriteLine($"🔧🔧🔧 Current user in constructor: '{currentUser ?? "NULL"}'");
        System.Diagnostics.Debug.WriteLine($"🔧🔧🔧 Current user in constructor: '{currentUser ?? "NULL"}'");
        Logger.LogInformation("🔧 Current user in constructor: '{CurrentUser}'", currentUser ?? "NULL");

        // Subscribe to service events
        _quickButtonsService.QuickButtonsChanged += OnQuickButtonsChanged;

        // Handle collection changes to update count
        QuickButtons.CollectionChanged += (sender, e) =>
        {
            OnPropertyChanged(nameof(NonEmptyQuickButtonsCount));
            OnPropertyChanged(nameof(NonEmptyQuickButtons));
            OnPropertyChanged(nameof(CanAnyButtonMoveUp));
            OnPropertyChanged(nameof(CanAnyButtonMoveDown));
        };

        Console.WriteLine("🔧🔧🔧 Starting Dispatcher.UIThread.InvokeAsync for LoadLast10TransactionsAsync");
        System.Diagnostics.Debug.WriteLine("🔧🔧🔧 Starting Dispatcher.UIThread.InvokeAsync for LoadLast10TransactionsAsync");
        Logger.LogInformation("🔧 Starting Dispatcher.UIThread.InvokeAsync for LoadLast10TransactionsAsync");

        // Load data from database service on UI thread to avoid threading issues
        _ = Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                Console.WriteLine("🔧🔧🔧 Inside Dispatcher.UIThread.InvokeAsync - about to call LoadLast10TransactionsAsync");
                System.Diagnostics.Debug.WriteLine("🔧🔧🔧 Inside Dispatcher.UIThread.InvokeAsync - about to call LoadLast10TransactionsAsync");
                Logger.LogInformation("🔧 Inside Dispatcher.UIThread.InvokeAsync - about to call LoadLast10TransactionsAsync");
                await LoadLast10TransactionsAsync();
                Console.WriteLine("🔧🔧🔧 Dispatcher.UIThread.InvokeAsync completed LoadLast10TransactionsAsync");
                System.Diagnostics.Debug.WriteLine("🔧🔧🔧 Dispatcher.UIThread.InvokeAsync completed LoadLast10TransactionsAsync");
                Logger.LogInformation("🔧 Dispatcher.UIThread.InvokeAsync completed LoadLast10TransactionsAsync");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔧🔧🔧 Failed to load initial quick buttons data in Dispatcher.UIThread.InvokeAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"🔧🔧🔧 Failed to load initial quick buttons data in Dispatcher.UIThread.InvokeAsync: {ex.Message}");
                Logger.LogError(ex, "Failed to load initial quick buttons data");
                Logger.LogError(ex, "🔧 Failed to load initial quick buttons data in Dispatcher.UIThread.InvokeAsync");
                
                // If database service fails, just load empty buttons
                LoadEmptyButtons();
            }
        });

        Console.WriteLine("🔧🔧🔧 QuickButtonsViewModel constructor COMPLETED");
        System.Diagnostics.Debug.WriteLine("🔧🔧🔧 QuickButtonsViewModel constructor COMPLETED");
        Logger.LogInformation("🔧 QuickButtonsViewModel constructor completed");
    }

    #region RelayCommand Methods

    /// <summary>
    /// Refreshes the quick buttons by loading the last 10 transactions
    /// </summary>
    [RelayCommand]
    private async Task RefreshButtons()
    {
        Logger.LogInformation("🔧 RefreshButtonsCommand executed");
        await LoadLast10TransactionsAsync();
    }

    /// <summary>
    /// Executes a quick action for the specified button
    /// </summary>
    [RelayCommand]
    private async Task ExecuteQuickAction(QuickButtonItemViewModel? button)
    {
        if (button != null)
        {
            await ExecuteQuickActionAsync(button);
        }
    }

    /// <summary>
    /// Removes the specified quick button
    /// </summary>
    [RelayCommand]
    private async Task RemoveButton(QuickButtonItemViewModel? button)
    {
        if (button != null)
        {
            await RemoveButtonAsync(button);
        }
    }

    /// <summary>
    /// Clears all quick buttons
    /// </summary>
    [RelayCommand]
    private async Task ClearAllButtons()
    {
        await ClearAllButtonsAsync();
    }

    /// <summary>
    /// Resets the button order to the last 10 transactions
    /// </summary>
    [RelayCommand]
    private async Task ResetOrder()
    {
        await ResetButtonOrderAsync();
    }

    /// <summary>
    /// Moves a button up one position
    /// </summary>
    [RelayCommand]
    private void MoveButtonUp(QuickButtonItemViewModel? button)
    {
        if (button != null)
        {
            MoveButtonUpImplementation(button);
        }
    }

    /// <summary>
    /// Moves a button down one position
    /// </summary>
    [RelayCommand]
    private void MoveButtonDown(QuickButtonItemViewModel? button)
    {
        if (button != null)
        {
            MoveButtonDownImplementation(button);
        }
    }

    /// <summary>
    /// Toggles modify mode for quick buttons
    /// </summary>
    [RelayCommand]
    private void ModifyMode()
    {
        Logger.LogInformation("ModifyModeCommand executed - Toggle modify mode for quick buttons");
        // Toggle modify mode for quick buttons
        // This could enable/disable drag-and-drop or show/hide management UI
    }

    /// <summary>
    /// Opens the quick button management interface
    /// </summary>
    [RelayCommand]
    private void ManageButtons()
    {
        Logger.LogInformation("ManageButtonsCommand executed - Open quick button management interface");
        // Open a management interface for quick buttons
        // This could navigate to a dedicated management view
    }

    #endregion

    private async Task LoadLast10TransactionsAsync()
    {
        try
        {
            Console.WriteLine("🔧🔧🔧 LoadLast10TransactionsAsync ENTRY POINT");
            System.Diagnostics.Debug.WriteLine("🔧🔧🔧 LoadLast10TransactionsAsync ENTRY POINT");
            Logger.LogInformation("🔧 LoadLast10TransactionsAsync ENTRY POINT");
            _progressService.StartOperation("Loading recent transactions...", false);

            // Ensure we have a valid user before making the database call
            var currentUser = _applicationState.CurrentUser;
            Console.WriteLine($"🔧🔧🔧 Current user in LoadLast10TransactionsAsync: '{currentUser ?? "NULL"}'");
            System.Diagnostics.Debug.WriteLine($"🔧🔧🔧 Current user in LoadLast10TransactionsAsync: '{currentUser ?? "NULL"}'");
            Logger.LogInformation("🔧 Current user in LoadLast10TransactionsAsync: '{CurrentUser}'", currentUser ?? "NULL");

            // If no user is set, use the current Windows user as default
            if (string.IsNullOrEmpty(currentUser))
            {
                currentUser = Environment.UserName.ToUpper();
                Logger.LogInformation("🔧 No user set, using Windows user: {CurrentUser}", currentUser);
                Console.WriteLine($"🔧🔧🔧 No user set, using Windows user: {currentUser}");
                System.Diagnostics.Debug.WriteLine($"🔧🔧🔧 No user set, using Windows user: {currentUser}");

                // Update the application state with the default user
                _applicationState.CurrentUser = currentUser;
            }

            Console.WriteLine($"🔧🔧🔧 About to call QuickButtons service with user: {currentUser}");
            System.Diagnostics.Debug.WriteLine($"🔧🔧🔧 About to call QuickButtons service with user: {currentUser}");
            Logger.LogInformation("🔧 About to call QuickButtons service with user: {CurrentUser}", currentUser);

            var transactions = await _quickButtonsService.LoadLast10TransactionsAsync(currentUser);

            Console.WriteLine($"🔧🔧🔧 QuickButtons service returned {transactions.Count} transactions");
            System.Diagnostics.Debug.WriteLine($"🔧🔧🔧 QuickButtons service returned {transactions.Count} transactions");
            Logger.LogInformation("🔧 QuickButtons service returned {Count} transactions", transactions.Count);

            // Update collection on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                QuickButtons.Clear();

                // Convert service data to ViewModel items
                for (int i = 0; i < Math.Min(transactions.Count, 10); i++)
                {
                    var transaction = transactions[i];
                    Logger.LogDebug("🔧 Creating button {Index}: PartId={PartId}, Operation={Operation}, Quantity={Quantity}",
                        i + 1, transaction.PartId, transaction.Operation, transaction.Quantity);

                    var button = new QuickButtonItemViewModel
                    {
                        Position = i + 1,
                        PartId = transaction.PartId,
                        Operation = transaction.Operation,
                        Quantity = transaction.Quantity,
                        DisplayText = transaction.PartId,
                        SubText = $"{transaction.Operation} - {transaction.Quantity} parts",
                        ToolTipText = $"Position {i + 1}: Click to populate Part ID: {transaction.PartId}, Operation: {transaction.Operation}, Quantity: {transaction.Quantity} in the active tab. Right-click for move and remove options."
                    };

                    // Subscribe to property changes
                    button.PropertyChanged += OnButtonPropertyChanged;
                    QuickButtons.Add(button);
                }

                Logger.LogInformation("🔧 Added {ActualButtonCount} transaction buttons to QuickButtons collection",
                    QuickButtons.Count(b => !b.IsEmpty));

                // Fill remaining slots with empty buttons
                for (int i = transactions.Count; i < 10; i++)
                {
                    var emptyButton = new QuickButtonItemViewModel
                    {
                        Position = i + 1,
                        PartId = string.Empty,
                        Operation = "EMPTY",
                        Quantity = 0,
                        DisplayText = "Empty Slot",
                        SubText = "Click to assign",
                        ToolTipText = $"Empty slot {i + 1} - Click to assign a quick action. Right-click for options."
                    };

                    emptyButton.PropertyChanged += OnButtonPropertyChanged;
                    QuickButtons.Add(emptyButton);
                }
            });

            Console.WriteLine($"🔧🔧🔧 Total QuickButtons collection now has {QuickButtons.Count} items ({QuickButtons.Count(b => !b.IsEmpty)} non-empty, {QuickButtons.Count(b => b.IsEmpty)} empty)");
            System.Diagnostics.Debug.WriteLine($"🔧🔧🔧 Total QuickButtons collection now has {QuickButtons.Count} items ({QuickButtons.Count(b => !b.IsEmpty)} non-empty, {QuickButtons.Count(b => b.IsEmpty)} empty)");
            Logger.LogInformation("🔧 Total QuickButtons collection now has {TotalCount} items ({NonEmpty} non-empty, {Empty} empty)",
                QuickButtons.Count,
                QuickButtons.Count(b => !b.IsEmpty),
                QuickButtons.Count(b => b.IsEmpty));

            // Update move command validation
            UpdateMoveCommandValidation();

            _progressService.CompleteOperation("Transactions loaded successfully");
        }
        catch (Exception ex)
        {
            _progressService.ReportError($"Failed to load transactions: {ex.Message}");
            Console.WriteLine($"🔧🔧🔧 LoadLast10TransactionsAsync FAILED with exception: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"🔧🔧🔧 LoadLast10TransactionsAsync FAILED with exception: {ex.Message}");
            Logger.LogError(ex, "🔧 LoadLast10TransactionsAsync FAILED with exception");

            // If database service fails, just load empty buttons
            LoadEmptyButtons();
        }
    }

    private async Task ExecuteQuickActionAsync(QuickButtonItemViewModel button)
    {
        try
        {
            if (button.IsEmpty)
            {
                Logger.LogInformation("Empty button clicked, no action to execute");
                return;
            }

            Logger.LogInformation("Executing quick action: {PartId}, {Operation}, {Quantity}", 
                button.PartId, button.Operation, button.Quantity);

            // Fire event to populate InventoryTab fields
            QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
            {
                PartId = button.PartId,
                Operation = button.Operation,
                Quantity = button.Quantity
            });
            
            // Update last used date in service
            await _quickButtonsService.SaveQuickButtonAsync(new QuickButtonData
            {
                UserId = _applicationState.CurrentUser,
                Position = button.Position,
                PartId = button.PartId,
                Operation = button.Operation,
                Quantity = button.Quantity,
                LastUsedDate = DateTime.Now
            });

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to execute quick action for button: {PartId}", button.PartId);
        }
    }

    private async Task RemoveButtonAsync(QuickButtonItemViewModel button)
    {
        try
        {
            _isUpdatingFromService = true; // Prevent reload during our update
            _progressService.StartOperation($"Removing quick button: {button.PartId}...", false);
            
            // Remove from service first
            var success = await _quickButtonsService.RemoveQuickButtonAsync(button.Position, _applicationState.CurrentUser);
            
            if (success)
            {
                // Remove from local collection
                QuickButtons.Remove(button);
                UpdateButtonPositions();
                _progressService.CompleteOperation("Quick button removed successfully");
                Logger.LogInformation("Removed quick button: {PartId}", button.PartId);
            }
            else
            {
                _progressService.ReportError("Failed to remove quick button");
            }
        }
        catch (Exception ex)
        {
            _progressService.ReportError($"Error removing button: {ex.Message}");
            Logger.LogError(ex, "Failed to remove quick button: {PartId}", button.PartId);
        }
        finally
        {
            _isUpdatingFromService = false; // Re-enable reload
        }
    }

    private async Task ClearAllButtonsAsync()
    {
        try
        {
            _progressService.StartOperation("Clearing all quick buttons...", false);
            
            var success = await _quickButtonsService.ClearAllQuickButtonsAsync(_applicationState.CurrentUser);
            
            if (success)
            {
                QuickButtons.Clear();
                LoadEmptyButtons(); // Load empty slots instead of sample data
                _progressService.CompleteOperation("All quick buttons cleared");
                Logger.LogInformation("Cleared all quick buttons for user: {User}", _applicationState.CurrentUser);
            }
            else
            {
                _progressService.ReportError("Failed to clear quick buttons");
            }
        }
        catch (Exception ex)
        {
            _progressService.ReportError($"Error clearing buttons: {ex.Message}");
            Logger.LogError(ex, "Failed to clear all quick buttons");
        }
    }

    private async Task ResetButtonOrderAsync()
    {
        try
        {
            _progressService.StartOperation("Resetting button order...", false);
            await LoadLast10TransactionsAsync();
            _progressService.CompleteOperation("Button order reset successfully");
        }
        catch (Exception ex)
        {
            _progressService.ReportError($"Error resetting order: {ex.Message}");
            Logger.LogError(ex, "Failed to reset button order");
        }
    }

    private void UpdateButtonPositions()
    {
        for (int i = 0; i < QuickButtons.Count; i++)
        {
            QuickButtons[i].Position = i + 1;
        }
        
        // Update move command validation for each button
        UpdateMoveCommandValidation();
    }

    /// <summary>
    /// Updates the move command validation properties for all buttons
    /// </summary>
    private void UpdateMoveCommandValidation()
    {
        foreach (var button in QuickButtons)
        {
            button.CanMoveUp = CanMoveButtonUp(button);
            button.CanMoveDown = CanMoveButtonDown(button);
        }
        
        // Notify parent-level properties
        OnPropertyChanged(nameof(CanAnyButtonMoveUp));
        OnPropertyChanged(nameof(CanAnyButtonMoveDown));
    }

    private void LoadEmptyButtons()
    {
        // Ensure this runs on UI thread
        Dispatcher.UIThread.Post(() =>
        {
            QuickButtons.Clear();
            
            // Load 10 empty button slots
            for (int i = 1; i <= 10; i++)
            {
                var emptyButton = new QuickButtonItemViewModel
                {
                    Position = i,
                    PartId = string.Empty,
                    Operation = "EMPTY",
                    Quantity = 0,
                    DisplayText = "Empty Slot",
                    SubText = "Click to assign",
                    ToolTipText = $"Empty slot {i} - Click to assign a quick action. Right-click for options."
                };
                
                // Subscribe to property changes to update count
                emptyButton.PropertyChanged += OnButtonPropertyChanged;
                QuickButtons.Add(emptyButton);
            }
            
            // Update move command validation
            UpdateMoveCommandValidation();
            
            // Raise initial count update
            OnPropertyChanged(nameof(NonEmptyQuickButtonsCount));
            OnPropertyChanged(nameof(NonEmptyQuickButtons));
            
            Logger.LogInformation("Loaded 10 empty quick button slots");
        });
    }

    /// <summary>
    /// Adds a new quick button based on a successful operation
    /// This is called when an inventory operation is completed to update the quick buttons
    /// </summary>
    public async Task AddQuickButtonFromOperationAsync(string partId, string operation, int quantity)
    {
        try
        {
            // First, add to the sys_last_10_transactions table
            var success = await _quickButtonsService.AddTransactionToLast10Async(
                _applicationState.CurrentUser, partId, operation, quantity);
                
            if (success)
            {
                Logger.LogInformation("Transaction added to last 10 for quick buttons: {PartId}, {Operation}, {Quantity}", 
                    partId, operation, quantity);
                
                // Refresh the buttons to show the new one
                await LoadLast10TransactionsAsync();
            }
            else
            {
                Logger.LogWarning("Failed to add transaction to last 10 for quick buttons");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to add quick button from operation");
        }
    }

    /// <summary>
    /// Saves changes to a quick button (used by edit functionality)
    /// This method is kept for potential future use but no longer accessible via UI
    /// </summary>
    private async Task SaveButtonAsync(QuickButtonItemViewModel button)
    {
        try
        {
            _isUpdatingFromService = true; // Prevent reload during our update
            _progressService.StartOperation($"Saving quick button: {button.PartId}...", false);
            
            var buttonData = new QuickButtonData
            {
                UserId = _applicationState.CurrentUser,
                Position = button.Position,
                PartId = button.PartId,
                Operation = button.Operation,
                Quantity = button.Quantity,
                LastUsedDate = DateTime.Now
            };
            
            var success = await _quickButtonsService.SaveQuickButtonAsync(buttonData);
            
            if (success)
            {
                _progressService.CompleteOperation("Quick button saved successfully");
                Logger.LogInformation("Saved quick button: {PartId} at position {Position}", button.PartId, button.Position);
                
                // Update the display properties to reflect the saved data
                button.DisplayText = button.PartId;
                button.SubText = $"{button.Quantity} parts.";
                button.ToolTipText = $"Position {button.Position}: Click to populate Part ID: {button.PartId}, Operation: {button.Operation}, Quantity: {button.Quantity} in the active tab. Right-click for move and remove options.";
            }
            else
            {
                _progressService.ReportError("Failed to save quick button");
            }
        }
        catch (Exception ex)
        {
            _progressService.ReportError($"Error saving button: {ex.Message}");
            Logger.LogError(ex, "Failed to save quick button: {PartId}", button.PartId);
        }
        finally
        {
            _isUpdatingFromService = false; // Re-enable reload
        }
    }

    /// <summary>
    /// Handles quick buttons service change events
    /// </summary>
    private async void OnQuickButtonsChanged(object? sender, QuickButtonsChangedEventArgs e)
    {
        try
        {
            if (e.UserId == _applicationState.CurrentUser && !_isUpdatingFromService)
            {
                Logger.LogInformation("Quick buttons changed for current user: {ChangeType}", e.ChangeType);
                
                // Only refresh for certain change types that require a full reload
                switch (e.ChangeType)
                {
                    case QuickButtonChangeType.Added:
                        // Only reload if we didn't trigger this change
                        await LoadLast10TransactionsAsync();
                        break;
                    case QuickButtonChangeType.Cleared:
                        // Full clear - reload everything
                        await LoadLast10TransactionsAsync();
                        break;
                    // Don't auto-reload for these - they're handled locally
                    case QuickButtonChangeType.Updated:
                    case QuickButtonChangeType.Removed:
                    case QuickButtonChangeType.Reordered:
                        Logger.LogDebug("Skipping reload for {ChangeType} - handled locally", e.ChangeType);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling quick buttons change event");
        }
    }

    /// <summary>
    /// Handles property changes for individual button ViewModels
    /// </summary>
    private void OnButtonPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(QuickButtonItemViewModel.PartId) || 
            e.PropertyName == nameof(QuickButtonItemViewModel.Operation))
        {
            OnPropertyChanged(nameof(NonEmptyQuickButtonsCount));
            OnPropertyChanged(nameof(NonEmptyQuickButtons));
            OnPropertyChanged(nameof(CanAnyButtonMoveUp));
            OnPropertyChanged(nameof(CanAnyButtonMoveDown));
        }
    }

    /// <summary>
    /// Reorders a button from one position to another (used by drag and drop)
    /// Prevents moving buttons beyond valid boundaries and into empty slots
    /// </summary>
    public async Task ReorderButtonAsync(int fromPosition, int toPosition)
    {
        if (fromPosition == toPosition) return;

        var fromButton = QuickButtons.FirstOrDefault(b => b.Position == fromPosition);
        var toButton = QuickButtons.FirstOrDefault(b => b.Position == toPosition);
        
        if (fromButton == null || toButton == null) return;
        
        // Cannot move empty buttons
        if (fromButton.IsEmpty)
        {
            Logger.LogDebug("Cannot reorder empty button from position {FromPosition}", fromPosition);
            return;
        }
        
        // Find the boundaries for valid moves
        var lastNonEmptyPosition = QuickButtons
            .Where(b => !b.IsEmpty)
            .Max(b => b.Position);
        
        // Cannot move to a position beyond the last non-empty button
        if (toPosition > lastNonEmptyPosition && toPosition > fromPosition)
        {
            Logger.LogDebug("Cannot move button to position {ToPosition}: beyond last non-empty button at position {LastPosition}", 
                toPosition, lastNonEmptyPosition);
            return;
        }
        
        // Cannot move to position 0 or negative
        if (toPosition < 1)
        {
            Logger.LogDebug("Cannot move button to position {ToPosition}: below minimum position 1", toPosition);
            return;
        }

        try
        {
            _isUpdatingFromService = true; // Prevent reload during our update
            _progressService.StartOperation("Reordering quick buttons...", false);

            // Perform the swap locally first for immediate UI feedback
            var fromIndex = QuickButtons.IndexOf(fromButton);
            var toIndex = QuickButtons.IndexOf(toButton);
            
            if (fromIndex >= 0 && toIndex >= 0)
            {
                QuickButtons.Move(fromIndex, toIndex);
                UpdateButtonPositions();
                
                // Persist the new order to the database
                var reorderedButtons = QuickButtons.Where(b => !b.IsEmpty)
                    .Select(b => new QuickButtonData
                    {
                        UserId = _applicationState.CurrentUser,
                        Position = b.Position,
                        PartId = b.PartId,
                        Operation = b.Operation,
                        Quantity = b.Quantity
                    }).ToList();
                
                var success = await _quickButtonsService.ReorderQuickButtonsAsync(_applicationState.CurrentUser, reorderedButtons);
                
                if (success)
                {
                    _progressService.CompleteOperation("Quick buttons reordered successfully");
                    Logger.LogInformation("Reordered quick buttons from position {From} to {To}", fromPosition, toPosition);
                }
                else
                {
                    // If database update fails, revert the UI change
                    QuickButtons.Move(toIndex, fromIndex);
                    UpdateButtonPositions();
                    _progressService.ReportError("Failed to save button order");
                }
            }
        }
        catch (Exception ex)
        {
            _progressService.ReportError($"Error reordering buttons: {ex.Message}");
            Logger.LogError(ex, "Failed to reorder quick buttons");
        }
        finally
        {
            _isUpdatingFromService = false; // Re-enable reload
        }
    }

    /// <summary>
    /// Checks if a reorder operation is valid
    /// </summary>
    public bool CanReorderButton(int fromPosition, int toPosition)
    {
        if (fromPosition == toPosition || fromPosition < 1 || toPosition < 1)
            return false;
            
        var fromButton = QuickButtons.FirstOrDefault(b => b.Position == fromPosition);
        if (fromButton == null || fromButton.IsEmpty)
            return false;
        
        // Find the last non-empty button position
        var lastNonEmptyPosition = QuickButtons
            .Where(b => !b.IsEmpty)
            .DefaultIfEmpty()
            .Max(b => b?.Position ?? 0);
        
        // Cannot move beyond the last non-empty button (unless moving backwards)
        return toPosition <= lastNonEmptyPosition || toPosition < fromPosition;
    }

    /// <summary>
    /// Moves a button up one position in the list and persists to server
    /// Prevents moving past position 1 or beyond the last non-empty button
    /// </summary>
    private async void MoveButtonUpImplementation(QuickButtonItemViewModel button)
    {
        var currentIndex = QuickButtons.IndexOf(button);
        
        // Cannot move up if already at position 1 (index 0)
        if (currentIndex <= 0)
        {
            Logger.LogDebug("Cannot move button up: already at position 1");
            return;
        }
        
        // Cannot move empty buttons
        if (button.IsEmpty)
        {
            Logger.LogDebug("Cannot move empty button up");
            return;
        }

        try
        {
            _isUpdatingFromService = true; // Prevent reload during our update
            _progressService.StartOperation("Moving button up...", false);
            
            // Perform the move locally first for immediate UI feedback
            QuickButtons.Move(currentIndex, currentIndex - 1);
            UpdateButtonPositions();
            
            // Persist the new order to the database
            await PersistButtonOrderAsync();
            
            _progressService.CompleteOperation("Button moved up successfully");
            Logger.LogInformation("Moved button up: {PartId} from position {OldPos} to {NewPos}", 
                button.PartId, currentIndex + 1, currentIndex);
        }
        catch (Exception ex)
        {
            // If server update fails, revert the UI change
            QuickButtons.Move(currentIndex - 1, currentIndex);
            UpdateButtonPositions();
            
            _progressService.ReportError($"Failed to move button: {ex.Message}");
            Logger.LogError(ex, "Failed to move button up: {PartId}", button.PartId);
        }
        finally
        {
            _isUpdatingFromService = false; // Re-enable reload
        }
    }

    /// <summary>
    /// Moves a button down one position in the list and persists to server
    /// Prevents moving past the last non-empty button (cannot move into empty slots)
    /// </summary>
    private async void MoveButtonDownImplementation(QuickButtonItemViewModel button)
    {
        var currentIndex = QuickButtons.IndexOf(button);
        
        // Cannot move down if already at the last position
        if (currentIndex >= QuickButtons.Count - 1)
        {
            Logger.LogDebug("Cannot move button down: already at last position");
            return;
        }
        
        // Cannot move empty buttons
        if (button.IsEmpty)
        {
            Logger.LogDebug("Cannot move empty button down");
            return;
        }
        
        // Find the last non-empty button position
        var lastNonEmptyIndex = -1;
        for (int i = QuickButtons.Count - 1; i >= 0; i--)
        {
            if (!QuickButtons[i].IsEmpty)
            {
                lastNonEmptyIndex = i;
                break;
            }
        }
        
        // Cannot move down if we're already at the last non-empty position
        // or if moving down would put us past the last non-empty button
        if (currentIndex >= lastNonEmptyIndex)
        {
            Logger.LogDebug("Cannot move button down: would move past last non-empty button at index {LastIndex}", lastNonEmptyIndex);
            return;
        }

        try
        {
            _isUpdatingFromService = true; // Prevent reload during our update
            _progressService.StartOperation("Moving button down...", false);
            
            // Perform the move locally first for immediate UI feedback
            QuickButtons.Move(currentIndex, currentIndex + 1);
            UpdateButtonPositions();
            
            // Persist the new order to the database
            await PersistButtonOrderAsync();
            
            _progressService.CompleteOperation("Button moved down successfully");
            Logger.LogInformation("Moved button down: {PartId} from position {OldPos} to {NewPos}", 
                button.PartId, currentIndex + 1, currentIndex + 2);
        }
        catch (Exception ex)
        {
            // If server update fails, revert the UI change
            QuickButtons.Move(currentIndex + 1, currentIndex);
            UpdateButtonPositions();
            
            _progressService.ReportError($"Failed to move button: {ex.Message}");
            Logger.LogError(ex, "Failed to move button down: {PartId}", button.PartId);
        }
        finally
        {
            _isUpdatingFromService = false; // Re-enable reload
        }
    }

    /// <summary>
    /// Checks if a button can be moved up
    /// </summary>
    public bool CanMoveButtonUp(QuickButtonItemViewModel button)
    {
        if (button == null || button.IsEmpty)
            return false;
            
        var currentIndex = QuickButtons.IndexOf(button);
        return currentIndex > 0;
    }

    /// <summary>
    /// Checks if a button can be moved down
    /// </summary>
    public bool CanMoveButtonDown(QuickButtonItemViewModel button)
    {
        if (button == null || button.IsEmpty)
            return false;
            
        var currentIndex = QuickButtons.IndexOf(button);
        
        // Find the last non-empty button position
        var lastNonEmptyIndex = -1;
        for (int i = QuickButtons.Count - 1; i >= 0; i--)
        {
            if (!QuickButtons[i].IsEmpty)
            {
                lastNonEmptyIndex = i;
                break;
            }
        }
        
        // Can move down if not at the last non-empty position
        return currentIndex >= 0 && currentIndex < lastNonEmptyIndex;
    }

    /// <summary>
    /// Persists the current button order to the server
    /// </summary>
    private async Task PersistButtonOrderAsync()
    {
        var reorderedButtons = QuickButtons.Where(b => !b.IsEmpty)
            .Select(b => new QuickButtonData
            {
                UserId = _applicationState.CurrentUser,
                Position = b.Position,
                PartId = b.PartId,
                Operation = b.Operation,
                Quantity = b.Quantity
            }).ToList();

        var success = await _quickButtonsService.ReorderQuickButtonsAsync(_applicationState.CurrentUser, reorderedButtons);
        
        if (!success)
        {
            throw new InvalidOperationException("Failed to persist button order to server");
        }
    }

    /// <summary>
    /// Manual test method - call this from a button or command to test the service
    /// </summary>
    public async Task TestLoadTransactions()
    {
        try
        {
            Logger.LogInformation("🧪 TEST: Manual test started");
            
            var currentUser = _applicationState.CurrentUser;
            Logger.LogInformation("🧪 TEST: Current user: '{CurrentUser}'", currentUser ?? "NULL");
            
            if (string.IsNullOrEmpty(currentUser))
            {
                // Try with a test user
                currentUser = "admin";
                Logger.LogInformation("🧪 TEST: Using test user: {TestUser}", currentUser);
            }
            
            var transactions = await _quickButtonsService.LoadLast10TransactionsAsync(currentUser);
            
            Logger.LogInformation("🧪 TEST: Service returned {Count} transactions", transactions.Count);
            
            foreach (var transaction in transactions)
            {
                Logger.LogInformation("🧪 TEST: Transaction - PartId: {PartId}, Operation: {Operation}, Quantity: {Quantity}", 
                    transaction.PartId, transaction.Operation, transaction.Quantity);
            }
            
            Logger.LogInformation("🧪 TEST: Manual test completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "🧪 TEST: Manual test failed");
        }
    }
}

/// <summary>
/// QuickButtonItemViewModel represents a single quick action button
/// with properties for part information and UI display. Uses MVVM
/// Community Toolkit for efficient property change notifications.
/// </summary>
public partial class QuickButtonItemViewModel : BaseViewModel
{
    /// <summary>
    /// Gets or sets the position of this button (1-based)
    /// </summary>
    [ObservableProperty]
    private int _position;

    /// <summary>
    /// Gets or sets the part ID for this quick action
    /// </summary>
    [ObservableProperty]
    private string _partId = string.Empty;

    /// <summary>
    /// Gets or sets the operation for this quick action
    /// </summary>
    [ObservableProperty]
    private string _operation = string.Empty;

    /// <summary>
    /// Gets or sets the quantity for this quick action
    /// </summary>
    [ObservableProperty]
    private int _quantity;

    /// <summary>
    /// Gets or sets the display text shown on the button
    /// </summary>
    [ObservableProperty]
    private string _displayText = string.Empty;

    /// <summary>
    /// Gets or sets the sub-text shown below the main text
    /// </summary>
    [ObservableProperty]
    private string _subText = string.Empty;

    /// <summary>
    /// Gets or sets the tooltip text for the button
    /// </summary>
    [ObservableProperty]
    private string _toolTipText = string.Empty;

    /// <summary>
    /// Gets or sets whether this button can be moved up
    /// </summary>
    [ObservableProperty]
    private bool _canMoveUp;

    /// <summary>
    /// Gets or sets whether this button can be moved down
    /// </summary>
    [ObservableProperty]
    private bool _canMoveDown;

    /// <summary>
    /// Gets whether this button represents an empty slot
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty(PartId) || Operation == "EMPTY";
}
