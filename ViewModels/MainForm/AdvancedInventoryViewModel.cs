using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// Simplified Advanced Inventory VM with three areas:
/// - Single item multiple times
/// - Single item multiple locations
/// - Import (Excel)
/// Keeps minimal state and commands like other tabs.
/// </summary>
public class AdvancedInventoryViewModel : BaseViewModel
{
    // Common options
    public ObservableCollection<string> PartIDOptions { get; } = new();
    public ObservableCollection<string> OperationOptions { get; } = new();
    public ObservableCollection<string> LocationOptions { get; } = new();

    #region Multiple Times
    public string? SelectedPartID { get => _selectedPartID; set => this.RaiseAndSetIfChanged(ref _selectedPartID, value); }
    public string? SelectedOperation { get => _selectedOperation; set => this.RaiseAndSetIfChanged(ref _selectedOperation, value); }
    public string? SelectedLocation { get => _selectedLocation; set => this.RaiseAndSetIfChanged(ref _selectedLocation, value); }
    public int Quantity { get => _quantity; set => this.RaiseAndSetIfChanged(ref _quantity, value); }
    public int RepeatTimes { get => _repeatTimes; set => this.RaiseAndSetIfChanged(ref _repeatTimes, value); }

    // String wrapper properties for TextBox binding
    public string QuantityText 
    { 
        get => _quantity.ToString(); 
        set 
        { 
            if (int.TryParse(value, out var result) && result > 0) 
                Quantity = result; 
            this.RaisePropertyChanged(); 
        } 
    }
    
    public string RepeatTimesText 
    { 
        get => _repeatTimes.ToString(); 
        set 
        { 
            if (int.TryParse(value, out var result) && result > 0) 
                RepeatTimes = result; 
            this.RaisePropertyChanged(); 
        } 
    }

    private string? _selectedPartID;
    private string? _selectedOperation;
    private string? _selectedLocation;
    private int _quantity = 1;
    private int _repeatTimes = 1;
    #endregion

    #region Multiple Locations
    public string? MultiLocationPartID { get => _multiLocationPartID; set => this.RaiseAndSetIfChanged(ref _multiLocationPartID, value); }
    public string? MultiLocationOperation { get => _multiLocationOperation; set => this.RaiseAndSetIfChanged(ref _multiLocationOperation, value); }
    public int MultiLocationQuantity { get => _multiLocationQuantity; set => this.RaiseAndSetIfChanged(ref _multiLocationQuantity, value); }
    public ObservableCollection<string> AvailableLocations { get; } = new();
    public ObservableCollection<string> SelectedLocations { get; } = new();

    // String wrapper property for TextBox binding
    public string MultiLocationQuantityText 
    { 
        get => _multiLocationQuantity.ToString(); 
        set 
        { 
            if (int.TryParse(value, out var result) && result > 0) 
                MultiLocationQuantity = result; 
            this.RaisePropertyChanged(); 
        } 
    }

    private string? _multiLocationPartID;
    private string? _multiLocationOperation;
    private int _multiLocationQuantity = 1;
    #endregion

    #region Status
    public bool IsBusy { get => _isBusy; set => this.RaiseAndSetIfChanged(ref _isBusy, value); }
    public string StatusMessage { get => _statusMessage; set => this.RaiseAndSetIfChanged(ref _statusMessage, value); }
    private bool _isBusy;
    private string _statusMessage = "Ready";
    #endregion

    #region Commands
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> AddMultipleTimesCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ResetMultipleTimesCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> AddToMultipleLocationsCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ResetMultipleLocationsCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> SelectAllLocationsCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ClearAllLocationsCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ImportFromExcelCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> BackToNormalCommand { get; private set; } = null!;
    #endregion

    public event EventHandler? BackToNormalRequested;

    private readonly ObservableAsPropertyHelper<bool> _canAddMultipleTimes;
    public bool CanAddMultipleTimes => _canAddMultipleTimes.Value;

    private readonly ObservableAsPropertyHelper<bool> _canAddToMultipleLocations;
    public bool CanAddToMultipleLocations => _canAddToMultipleLocations.Value;

    public AdvancedInventoryViewModel(ILogger<AdvancedInventoryViewModel> logger) : base(logger)
    {
        _canAddMultipleTimes = this.WhenAnyValue(
            vm => vm.SelectedPartID, vm => vm.SelectedOperation, vm => vm.SelectedLocation,
            vm => vm.Quantity, vm => vm.RepeatTimes,
            (p, o, l, q, r) => !string.IsNullOrWhiteSpace(p) && !string.IsNullOrWhiteSpace(o) && !string.IsNullOrWhiteSpace(l) && q > 0 && r > 0)
            .ToProperty(this, vm => vm.CanAddMultipleTimes, initialValue: false);

        _canAddToMultipleLocations = this.WhenAnyValue(
            vm => vm.MultiLocationPartID, vm => vm.MultiLocationOperation, vm => vm.MultiLocationQuantity,
            (p, o, q) => !string.IsNullOrWhiteSpace(p) && !string.IsNullOrWhiteSpace(o) && q > 0)
            .ToProperty(this, vm => vm.CanAddToMultipleLocations, initialValue: false);

        // Wire up property change notifications for wrapper properties
        this.WhenAnyValue(vm => vm.Quantity).Subscribe(_ => this.RaisePropertyChanged(nameof(QuantityText)));
        this.WhenAnyValue(vm => vm.RepeatTimes).Subscribe(_ => this.RaisePropertyChanged(nameof(RepeatTimesText)));
        this.WhenAnyValue(vm => vm.MultiLocationQuantity).Subscribe(_ => this.RaisePropertyChanged(nameof(MultiLocationQuantityText)));

        InitializeCommands();

        Observable.Merge(
            LoadDataCommand.ThrownExceptions,
            AddMultipleTimesCommand.ThrownExceptions,
            AddToMultipleLocationsCommand.ThrownExceptions,
            ImportFromExcelCommand.ThrownExceptions
        ).Subscribe(ex =>
        {
            Logger.LogError(ex, "AdvancedInventoryViewModel error");
            IsBusy = false;
            StatusMessage = $"Error: {ex.Message}";
        });

        _ = LoadDataCommand.Execute();
    }

    private void InitializeCommands()
    {
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Loading options...";

                PartIDOptions.Clear();
                foreach (var p in new[] { "PART001", "PART002", "PART003", "PART004", "PART005" }) PartIDOptions.Add(p);
                OperationOptions.Clear();
                foreach (var o in new[] { "90", "100", "110", "120", "130" }) OperationOptions.Add(o);
                LocationOptions.Clear();
                foreach (var l in new[] { "WC01", "WC02", "WC03", "WC04", "WC05" }) LocationOptions.Add(l);

                AvailableLocations.Clear();
                foreach (var l in LocationOptions) AvailableLocations.Add(l);

                StatusMessage = "Ready";
            }
            finally { IsBusy = false; }
        });

        AddMultipleTimesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                IsBusy = true;
                StatusMessage = $"Adding {RepeatTimes}x {SelectedPartID}...";
                await Task.Delay(500); // TODO: Dao_Inventory calls in a loop
                StatusMessage = $"Added {RepeatTimes} item(s)";
                ResetMultipleTimes();
            }
            finally { IsBusy = false; }
        }, this.WhenAnyValue(vm => vm.CanAddMultipleTimes));

        ResetMultipleTimesCommand = ReactiveCommand.Create(() =>
        {
            ResetMultipleTimes();
        });

        AddToMultipleLocationsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                IsBusy = true;
                var count = SelectedLocations.Count;
                StatusMessage = $"Adding to {count} locations...";
                await Task.Delay(500); // TODO: iterate locations and call Dao_Inventory
                StatusMessage = $"Added to {count} location(s)";
                ResetMultipleLocations();
            }
            finally { IsBusy = false; }
        }, this.WhenAnyValue(vm => vm.CanAddToMultipleLocations));

        ResetMultipleLocationsCommand = ReactiveCommand.Create(() =>
        {
            ResetMultipleLocations();
        });

        SelectAllLocationsCommand = ReactiveCommand.Create(() =>
        {
            SelectedLocations.Clear();
            foreach (var loc in AvailableLocations) SelectedLocations.Add(loc);
        });

        ClearAllLocationsCommand = ReactiveCommand.Create(() =>
        {
            SelectedLocations.Clear();
        });

        ImportFromExcelCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Importing from Excel...";
                await Task.Delay(800); // TODO: Helper_Excel integration
                StatusMessage = "Import complete";
            }
            finally { IsBusy = false; }
        });

        BackToNormalCommand = ReactiveCommand.Create(() =>
        {
            BackToNormalRequested?.Invoke(this, EventArgs.Empty);
        });
    }

    private void ResetMultipleTimes()
    {
        SelectedPartID = null;
        SelectedOperation = null;
        SelectedLocation = null;
        Quantity = 1;
        RepeatTimes = 1;
    }

    private void ResetMultipleLocations()
    {
        MultiLocationPartID = null;
        MultiLocationOperation = null;
        MultiLocationQuantity = 1;
        SelectedLocations.Clear();
    }
}