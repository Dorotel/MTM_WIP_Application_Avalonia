using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia;
using Avalonia.Controls;

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

    #region Filter Panel Properties
    public bool IsFilterPanelExpanded { get => _isFilterPanelExpanded; set => this.RaiseAndSetIfChanged(ref _isFilterPanelExpanded, value); }
    public string FilterPanelWidth => IsFilterPanelExpanded ? "200" : "32";
    public string CollapseButtonIcon => IsFilterPanelExpanded ? "ChevronLeft" : "ChevronRight";
    
    private bool _isFilterPanelExpanded = true;
    #endregion

    #region Multiple Times
    public string? SelectedPartID { get => _selectedPartID; set => this.RaiseAndSetIfChanged(ref _selectedPartID, value); }
    public string? SelectedOperation { get => _selectedOperation; set => this.RaiseAndSetIfChanged(ref _selectedOperation, value); }
    public string? SelectedLocation { get => _selectedLocation; set => this.RaiseAndSetIfChanged(ref _selectedLocation, value); }
    public int Quantity { get => _quantity; set => this.RaiseAndSetIfChanged(ref _quantity, value); }
    public int RepeatTimes { get => _repeatTimes; set => this.RaiseAndSetIfChanged(ref _repeatTimes, value); }

    // Text properties for AutoCompleteBox two-way binding
    private string _partIDText = string.Empty;
    public string PartIDText
    {
        get => _partIDText;
        set => this.RaiseAndSetIfChanged(ref _partIDText, value ?? string.Empty);
    }

    private string _operationText = string.Empty;
    public string OperationText
    {
        get => _operationText;
        set => this.RaiseAndSetIfChanged(ref _operationText, value ?? string.Empty);
    }

    private string _locationText = string.Empty;
    public string LocationText
    {
        get => _locationText;
        set => this.RaiseAndSetIfChanged(ref _locationText, value ?? string.Empty);
    }

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
    public ReactiveCommand<Unit, Unit> ToggleFilterPanelCommand { get; private set; } = null!;
    #endregion

    public event EventHandler? BackToNormalRequested;

    private readonly ObservableAsPropertyHelper<bool> _canAddMultipleTimes;
    public bool CanAddMultipleTimes => _canAddMultipleTimes.Value;

    private readonly ObservableAsPropertyHelper<bool> _canAddToMultipleLocations;
    public bool CanAddToMultipleLocations => _canAddToMultipleLocations.Value;

    // Design-time constructor
    public AdvancedInventoryViewModel() : this(CreateDesignTimeLogger<AdvancedInventoryViewModel>())
    {
        // Only initialize for design-time
        if (IsDesignMode())
        {
            InitializeDesignTimeData();
        }
    }

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

        // Sync text properties with selected items
        this.WhenAnyValue(x => x.SelectedPartID)
            .Subscribe(selected => PartIDText = selected ?? string.Empty);
        
        this.WhenAnyValue(x => x.SelectedOperation)
            .Subscribe(selected => OperationText = selected ?? string.Empty);
        
        this.WhenAnyValue(x => x.SelectedLocation)
            .Subscribe(selected => LocationText = selected ?? string.Empty);

        // Sync selected items when text matches exactly
        this.WhenAnyValue(x => x.PartIDText)
            .Where(text => !string.IsNullOrEmpty(text) && PartIDOptions.Contains(text))
            .Subscribe(text => SelectedPartID = text);
        
        this.WhenAnyValue(x => x.OperationText)
            .Where(text => !string.IsNullOrEmpty(text) && OperationOptions.Contains(text))
            .Subscribe(text => SelectedOperation = text);
        
        this.WhenAnyValue(x => x.LocationText)
            .Where(text => !string.IsNullOrEmpty(text) && LocationOptions.Contains(text))
            .Subscribe(text => SelectedLocation = text);
   

        // Wire up filter panel property changes
        this.WhenAnyValue(vm => vm.IsFilterPanelExpanded).Subscribe(_ => 
        {
            this.RaisePropertyChanged(nameof(FilterPanelWidth));
            this.RaisePropertyChanged(nameof(CollapseButtonIcon));
        });

        InitializeCommands();

        // Handle error subscription safely
        try
        {
            Observable.Merge(
                LoadDataCommand.ThrownExceptions,
                AddMultipleTimesCommand.ThrownExceptions,
                AddToMultipleLocationsCommand.ThrownExceptions,
                ImportFromExcelCommand.ThrownExceptions,
                ToggleFilterPanelCommand.ThrownExceptions
            ).Subscribe(ex =>
            {
                Logger.LogError(ex, "AdvancedInventoryViewModel error");
                IsBusy = false;
                StatusMessage = $"Error: {ex.Message}";
            });

            // Only execute LoadDataCommand if not in design mode
            if (!IsDesignMode())
            {
                _ = LoadDataCommand.Execute();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during AdvancedInventoryViewModel initialization");
        }
    }

    private static bool IsDesignMode()
    {
        try
        {
            return Design.IsDesignMode;
        }
        catch
        {
            // Fallback if Design class isn't available
            return false;
        }
    }

    private void InitializeDesignTimeData()
    {
        // Populate design-time data
        foreach (var p in new[] { "PART001", "PART002", "PART003" }) PartIDOptions.Add(p);
        foreach (var o in new[] { "90", "100", "110" }) OperationOptions.Add(o);
        foreach (var l in new[] { "WC01", "WC02", "WC03" }) LocationOptions.Add(l);
        foreach (var l in LocationOptions) AvailableLocations.Add(l);
        
        // Set some sample data
        SelectedPartID = "PART001";
        SelectedOperation = "90";
        SelectedLocation = "WC01";
        StatusMessage = "Design Mode - Ready";
    }

    private static ILogger<T> CreateDesignTimeLogger<T>()
    {
        try
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<T>();
        }
        catch
        {
            return Microsoft.Extensions.Logging.Abstractions.NullLogger<T>.Instance;
        }
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

        ToggleFilterPanelCommand = ReactiveCommand.Create(() =>
        {
            IsFilterPanelExpanded = !IsFilterPanelExpanded;
        });
    }

    private void ResetMultipleTimes()
    {
        SelectedPartID = null;
        SelectedOperation = null;
        SelectedLocation = null;
        PartIDText = string.Empty;
        OperationText = string.Empty;
        LocationText = string.Empty;
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
