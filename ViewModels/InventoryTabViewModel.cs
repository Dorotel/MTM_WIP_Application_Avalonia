using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class InventoryTabViewModel : ReactiveObject
{
    // TODO: Inject services
    // private readonly IInventoryService _inventoryService;
    // private readonly IProgressService _progressService;
    // private readonly IQuickButtonsService _quickButtonsService;

    // Observable collections for ComboBoxes
    public ObservableCollection<string> PartOptions { get; } = new();
    public ObservableCollection<string> OperationOptions { get; } = new();
    public ObservableCollection<string> LocationOptions { get; } = new();

    // Form fields
    private string? _selectedPart;
    public string? SelectedPart
    {
        get => _selectedPart;
        set => this.RaiseAndSetIfChanged(ref _selectedPart, value);
    }

    private string? _selectedOperation;
    public string? SelectedOperation
    {
        get => _selectedOperation;
        set => this.RaiseAndSetIfChanged(ref _selectedOperation, value);
    }

    private string? _selectedLocation;
    public string? SelectedLocation
    {
        get => _selectedLocation;
        set => this.RaiseAndSetIfChanged(ref _selectedLocation, value);
    }

    private string _quantity = string.Empty;
    public string Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    private string _notes = string.Empty;
    public string Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    // Version display
    private string _versionText = "Version: 4.6.0.0";
    public string VersionText
    {
        get => _versionText;
        set => this.RaiseAndSetIfChanged(ref _versionText, value);
    }

    // Validation state
    private readonly ObservableAsPropertyHelper<bool> _canSave;
    public bool CanSave => _canSave.Value;

    // Commands
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }
    public ReactiveCommand<Unit, Unit> AdvancedEntryCommand { get; }
    public ReactiveCommand<Unit, Unit> TogglePanelCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

    // Events for inter-component communication
    public event EventHandler<InventoryItemSavedEventArgs>? InventoryItemSaved;
    public event EventHandler? PanelToggleRequested;

    public InventoryTabViewModel()
    {
        // Validation logic - enable save only when all required fields are valid
        _canSave = this.WhenAnyValue(
                vm => vm.SelectedPart,
                vm => vm.SelectedOperation,
                vm => vm.SelectedLocation,
                vm => vm.Quantity,
                (part, operation, location, quantity) =>
                    !string.IsNullOrWhiteSpace(part) &&
                    !string.IsNullOrWhiteSpace(operation) &&
                    !string.IsNullOrWhiteSpace(location) &&
                    IsValidQuantity(quantity))
            .ToProperty(this, vm => vm.CanSave);

        // Initialize commands
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await SaveInventoryItemAsync();
        }, this.WhenAnyValue(vm => vm.CanSave));

        ResetCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ResetFormAsync();
        });

        AdvancedEntryCommand = ReactiveCommand.Create(() =>
        {
            // TODO: Open advanced inventory features
        });

        TogglePanelCommand = ReactiveCommand.Create(() =>
        {
            PanelToggleRequested?.Invoke(this, EventArgs.Empty);
        });

        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await LoadComboBoxDataAsync();
        });

        // Error handling
        SaveCommand.ThrownExceptions.Subscribe(HandleException);
        ResetCommand.ThrownExceptions.Subscribe(HandleException);
        LoadDataCommand.ThrownExceptions.Subscribe(HandleException);

        // Load initial data
        LoadSampleData();
    }

    private bool IsValidQuantity(string? quantity)
    {
        if (string.IsNullOrWhiteSpace(quantity))
            return false;

        return int.TryParse(quantity, out int result) && result > 0;
    }

    private async Task SaveInventoryItemAsync()
    {
        // TODO: Implement database save operation
        // var inventoryResult = await Dao_Inventory.AddInventoryItemAsync(
        //     user: Model_AppVariables.User,
        //     partId: SelectedPart,
        //     operation: SelectedOperation,
        //     location: SelectedLocation,
        //     quantity: int.Parse(Quantity),
        //     notes: Notes
        // );

        await Task.CompletedTask;

        // Fire event for quick buttons integration
        InventoryItemSaved?.Invoke(this, new InventoryItemSavedEventArgs
        {
            PartId = SelectedPart ?? string.Empty,
            Operation = SelectedOperation ?? string.Empty,
            Location = SelectedLocation ?? string.Empty,
            Quantity = int.TryParse(Quantity, out int qty) ? qty : 0,
            Notes = Notes
        });

        // Reset form after successful save
        await ResetFormAsync(soft: true);
    }

    private async Task ResetFormAsync(bool soft = true)
    {
        // TODO: Check if Shift key is pressed for hard reset
        // Hard reset refreshes ComboBox data from database
        
        SelectedPart = null;
        SelectedOperation = null;
        SelectedLocation = null;
        Quantity = string.Empty;
        Notes = string.Empty;

        if (!soft)
        {
            // Hard reset - reload all ComboBox data
            await LoadComboBoxDataAsync();
        }

        await Task.CompletedTask;
    }

    private async Task LoadComboBoxDataAsync()
    {
        // TODO: Implement database loading
        // var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        //     Model_AppVariables.ConnectionString,
        //     "sys_parts_Get_All",
        //     new Dictionary<string, object>()
        // );

        await Task.CompletedTask;
        LoadSampleData(); // For now, load sample data
    }

    private void LoadSampleData()
    {
        // Sample data for demonstration
        PartOptions.Clear();
        OperationOptions.Clear();
        LocationOptions.Clear();

        // Sample parts
        var sampleParts = new[] { "PART001", "PART002", "PART003", "PART004", "PART005" };
        foreach (var part in sampleParts)
        {
            PartOptions.Add(part);
        }

        // Sample operations (typically numbers in MTM)
        var sampleOperations = new[] { "90", "100", "110", "120", "130" };
        foreach (var operation in sampleOperations)
        {
            OperationOptions.Add(operation);
        }

        // Sample locations
        var sampleLocations = new[] { "WC01", "WC02", "WC03", "WC04", "WC05" };
        foreach (var location in sampleLocations)
        {
            LocationOptions.Add(location);
        }
    }

    private void HandleException(Exception ex)
    {
        // TODO: Log and present user-friendly error
        // await _errorService.LogErrorAsync(ex);
        // Show user-friendly message
    }
}

public class InventoryItemSavedEventArgs : EventArgs
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
}