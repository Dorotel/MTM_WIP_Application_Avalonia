using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class QuickButtonsViewModel : ReactiveObject
{
    // TODO: Inject services
    // private readonly IQuickButtonsService _quickButtonsService;
    // private readonly IProgressService _progressService;

    // Observable collections
    public ObservableCollection<QuickButtonItemViewModel> QuickButtons { get; } = new();

    // Computed property to count non-empty quick buttons
    public int NonEmptyQuickButtonsCount => QuickButtons.Count(button => !button.IsEmpty);

    // Computed property to get non-empty quick buttons for display
    public IEnumerable<QuickButtonItemViewModel> NonEmptyQuickButtons => 
        QuickButtons.Where(button => !button.IsEmpty);

    // Event to notify the parent about quick action execution
    public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

    // Commands - Now all accessible via context menu only
    public ReactiveCommand<Unit, Unit> RefreshButtonsCommand { get; }
    public ReactiveCommand<QuickButtonItemViewModel, Unit> ExecuteQuickActionCommand { get; }
    public ReactiveCommand<QuickButtonItemViewModel, Unit> EditButtonCommand { get; }
    public ReactiveCommand<QuickButtonItemViewModel, Unit> RemoveButtonCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearAllButtonsCommand { get; }
    public ReactiveCommand<Unit, Unit> ManageButtonsCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetOrderCommand { get; }
    public ReactiveCommand<Unit, Unit> ModifyModeCommand { get; }
    
    // Manual reordering commands
    public ReactiveCommand<QuickButtonItemViewModel, Unit> MoveButtonUpCommand { get; }
    public ReactiveCommand<QuickButtonItemViewModel, Unit> MoveButtonDownCommand { get; }

    public QuickButtonsViewModel()
    {
        // Initialize commands
        RefreshButtonsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await LoadLast10TransactionsAsync();
        });

        ExecuteQuickActionCommand = ReactiveCommand.CreateFromTask<QuickButtonItemViewModel>(async (button) =>
        {
            await ExecuteQuickActionAsync(button);
        });

        EditButtonCommand = ReactiveCommand.Create<QuickButtonItemViewModel>((button) =>
        {
            // TODO: Open edit dialog for button
        });

        RemoveButtonCommand = ReactiveCommand.CreateFromTask<QuickButtonItemViewModel>(async (button) =>
        {
            await RemoveButtonAsync(button);
        });

        ClearAllButtonsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ClearAllButtonsAsync();
        });

        ResetOrderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ResetButtonOrderAsync();
        });

        // Manual reordering commands
        MoveButtonUpCommand = ReactiveCommand.Create<QuickButtonItemViewModel>((button) =>
        {
            MoveButtonUp(button);
        });

        MoveButtonDownCommand = ReactiveCommand.Create<QuickButtonItemViewModel>((button) =>
        {
            MoveButtonDown(button);
        });

        // Handle collection changes to update count
        QuickButtons.CollectionChanged += (sender, e) =>
        {
            this.RaisePropertyChanged(nameof(NonEmptyQuickButtonsCount));
            this.RaisePropertyChanged(nameof(NonEmptyQuickButtons));
        };

        // Error handling for all commands
        RefreshButtonsCommand.ThrownExceptions.Subscribe(HandleException);
        ExecuteQuickActionCommand.ThrownExceptions.Subscribe(HandleException);
        EditButtonCommand.ThrownExceptions.Subscribe(HandleException);
        RemoveButtonCommand.ThrownExceptions.Subscribe(HandleException);
        ClearAllButtonsCommand.ThrownExceptions.Subscribe(HandleException);
        ResetOrderCommand.ThrownExceptions.Subscribe(HandleException);
        MoveButtonUpCommand.ThrownExceptions.Subscribe(HandleException);
        MoveButtonDownCommand.ThrownExceptions.Subscribe(HandleException);

        // Load initial data
        LoadSampleData();
    }

    private void HandleException(Exception ex)
    {
        // TODO: Log and present user-friendly error
    }

    private async Task LoadLast10TransactionsAsync()
    {
        // TODO: Implement database loading
        // var dataResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        //     Model_AppVariables.ConnectionString,
        //     "sys_last_10_transactions_Get_ByUser",
        //     new Dictionary<string, object> { ["User"] = currentUser }
        // );
        
        await Task.CompletedTask;
        LoadSampleData(); // For now, load sample data
    }

    private async Task ExecuteQuickActionAsync(QuickButtonItemViewModel button)
    {
        // TODO: Execute inventory operation with stored parameters
        // Fire event to populate InventoryTab fields
        QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
        {
            PartId = button.PartId,
            Operation = button.Operation,
            Quantity = button.Quantity
        });
        
        await Task.CompletedTask;
    }

    private async Task RemoveButtonAsync(QuickButtonItemViewModel button)
    {
        // TODO: Implement Dao_QuickButtons.RemoveQuickButtonAndShiftAsync()
        QuickButtons.Remove(button);
        // Reorder remaining buttons
        UpdateButtonPositions();
        await Task.CompletedTask;
    }

    private async Task ClearAllButtonsAsync()
    {
        // TODO: Implement Dao_QuickButtons.DeleteAllQuickButtonsForUserAsync()
        QuickButtons.Clear();
        LoadSampleData(); // Reload with empty slots
        await Task.CompletedTask;
    }

    private async Task ResetButtonOrderAsync()
    {
        // TODO: Reset button order to original sequence
        await LoadLast10TransactionsAsync();
    }

    private void UpdateButtonPositions()
    {
        for (int i = 0; i < QuickButtons.Count; i++)
        {
            QuickButtons[i].Position = i + 1;
        }
    }

    private void LoadSampleData()
    {
        QuickButtons.Clear();
        
        // Sample quick buttons for demonstration - Always 10 slots
        // Operations are just numbers (like "90", "100", "110", etc.)
        var sampleOperations = new[] { "90", "100", "110", "120", "130" };
        var sampleParts = new[] { "PART001", "PART002", "PART003", "PART004", "PART005", "PART006", "PART007", "PART008", "PART009", "PART010" };
        
        for (int i = 1; i <= 10; i++)
        {
            QuickButtonItemViewModel button;
            
            if (i <= 7) // Show 7 sample buttons, leave 3 empty slots
            {
                var operation = sampleOperations[(i - 1) % sampleOperations.Length];
                var partId = sampleParts[i - 1];
                var quantity = i * 5;
                
                button = new QuickButtonItemViewModel
                {
                    Position = i,
                    PartId = partId,
                    Operation = operation,
                    Quantity = quantity,
                    DisplayText = $"{partId}",
                    SubText = $"{quantity} parts.",
                    ToolTipText = $"Position {i}: Click to populate Part ID: {partId}, Operation: {operation}, Quantity: {quantity} in the active tab. Right-click for options."
                };
            }
            else
            {
                // Empty slot
                button = new QuickButtonItemViewModel
                {
                    Position = i,
                    PartId = string.Empty,
                    Operation = "EMPTY",
                    Quantity = 0,
                    DisplayText = "Empty Slot",
                    SubText = "Click to assign",
                    ToolTipText = $"Empty slot {i} - Click to assign a quick action. Right-click for options."
                };
            }
            
            // Subscribe to property changes to update count
            button.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(QuickButtonItemViewModel.PartId) || 
                    e.PropertyName == nameof(QuickButtonItemViewModel.Operation))
                {
                    this.RaisePropertyChanged(nameof(NonEmptyQuickButtonsCount));
                    this.RaisePropertyChanged(nameof(NonEmptyQuickButtons));
                }
            };
            
            QuickButtons.Add(button);
        }
        
        // Raise initial count update
        this.RaisePropertyChanged(nameof(NonEmptyQuickButtonsCount));
        this.RaisePropertyChanged(nameof(NonEmptyQuickButtons));
    }

    /// <summary>
    /// Adds a new quick button based on a successful operation
    /// This is called when an inventory operation is completed to update the quick buttons
    /// </summary>
    public void AddQuickButtonFromOperation(string partId, string operation, int quantity)
    {
        // TODO: Implement Dao_QuickButtons.AddOrShiftQuickButtonAsync()
        // For now, add to the first available slot or shift existing buttons
        
        var existingButton = QuickButtons.FirstOrDefault(b => b.PartId == partId && b.Operation == operation);
        if (existingButton != null)
        {
            // Update existing button
            existingButton.Quantity = quantity;
            existingButton.DisplayText = $"{partId}";
            existingButton.SubText = $"{quantity} parts.";
            existingButton.ToolTipText = $"Position {existingButton.Position}: Click to populate Part ID: {partId}, Operation: {operation}, Quantity: {quantity} in the active tab. Right-click for options.";
        }
        else
        {
            // Find first empty slot or replace the last one
            var emptySlot = QuickButtons.FirstOrDefault(b => b.IsEmpty);
            if (emptySlot != null)
            {
                emptySlot.PartId = partId;
                emptySlot.Operation = operation;
                emptySlot.Quantity = quantity;
                emptySlot.DisplayText = $"{partId}";
                emptySlot.SubText = $"{quantity} parts.";
                emptySlot.ToolTipText = $"Position {emptySlot.Position}: Click to populate Part ID: {partId}, Operation: {operation}, Quantity: {quantity} in the active tab. Right-click for options.";
            }
            else
            {
                // Shift all buttons and add new one at position 1
                for (int i = QuickButtons.Count - 1; i > 0; i--)
                {
                    var current = QuickButtons[i];
                    var previous = QuickButtons[i - 1];
                    
                    current.PartId = previous.PartId;
                    current.Operation = previous.Operation;
                    current.Quantity = previous.Quantity;
                    current.DisplayText = previous.DisplayText;
                    current.SubText = previous.SubText;
                    current.ToolTipText = previous.ToolTipText?.Replace($"Position {previous.Position}", $"Position {current.Position}");
                }
                
                // Update first button with new data
                var firstButton = QuickButtons[0];
                firstButton.PartId = partId;
                firstButton.Operation = operation;
                firstButton.Quantity = quantity;
                firstButton.DisplayText = $"{partId}";
                firstButton.SubText = $"{quantity} parts.";
                firstButton.ToolTipText = $"Position 1: Click to populate Part ID: {partId}, Operation: {operation}, Quantity: {quantity} in the active tab. Right-click for options.";
            }
        }
    }

    /// <summary>
    /// Reorders a button from one position to another (used by drag and drop)
    /// </summary>
    public void ReorderButton(int fromPosition, int toPosition)
    {
        if (fromPosition == toPosition) return;

        var fromButton = QuickButtons.FirstOrDefault(b => b.Position == fromPosition);
        var toButton = QuickButtons.FirstOrDefault(b => b.Position == toPosition);
        
        if (fromButton == null || toButton == null) return;

        // TODO: Implement Dao_QuickButtons.ReorderButtonAsync()
        
        // Perform the swap locally
        var fromIndex = QuickButtons.IndexOf(fromButton);
        var toIndex = QuickButtons.IndexOf(toButton);
        
        if (fromIndex >= 0 && toIndex >= 0)
        {
            QuickButtons.Move(fromIndex, toIndex);
            UpdateButtonPositions();
        }
    }

    /// <summary>
    /// Moves a button up one position in the list
    /// </summary>
    private void MoveButtonUp(QuickButtonItemViewModel button)
    {
        var currentIndex = QuickButtons.IndexOf(button);
        if (currentIndex > 0)
        {
            QuickButtons.Move(currentIndex, currentIndex - 1);
            UpdateButtonPositions();
        }
    }

    /// <summary>
    /// Moves a button down one position in the list
    /// </summary>
    private void MoveButtonDown(QuickButtonItemViewModel button)
    {
        var currentIndex = QuickButtons.IndexOf(button);
        if (currentIndex < QuickButtons.Count - 1)
        {
            QuickButtons.Move(currentIndex, currentIndex + 1);
            UpdateButtonPositions();
        }
    }
}

public class QuickButtonItemViewModel : ReactiveObject
{
    private int _position;
    public int Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
    }

    private string _partId = string.Empty;
    public string PartId
    {
        get => _partId;
        set => this.RaiseAndSetIfChanged(ref _partId, value);
    }

    private string _operation = string.Empty;
    public string Operation
    {
        get => _operation;
        set => this.RaiseAndSetIfChanged(ref _operation, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    private string _displayText = string.Empty;
    public string DisplayText
    {
        get => _displayText;
        set => this.RaiseAndSetIfChanged(ref _displayText, value);
    }

    private string _subText = string.Empty;
    public string SubText
    {
        get => _subText;
        set => this.RaiseAndSetIfChanged(ref _subText, value);
    }

    private string _toolTipText = string.Empty;
    public string ToolTipText
    {
        get => _toolTipText;
        set => this.RaiseAndSetIfChanged(ref _toolTipText, value);
    }

    public bool IsEmpty => string.IsNullOrEmpty(PartId) || Operation == "EMPTY";
}

public class QuickActionExecutedEventArgs : EventArgs
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
