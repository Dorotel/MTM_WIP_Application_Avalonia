using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Commands;

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
    public bool IsFilterPanelExpanded { get => _isFilterPanelExpanded; set => SetProperty(ref _isFilterPanelExpanded, value); }
    public string FilterPanelWidth => IsFilterPanelExpanded ? "200" : "32";
    public string CollapseButtonIcon => IsFilterPanelExpanded ? "ChevronLeft" : "ChevronRight";
    
    private bool _isFilterPanelExpanded = true;
    #endregion

    #region Multiple Times
    public string? SelectedPartID { get => _selectedPartID; set => SetProperty(ref _selectedPartID, value); }
    public string? SelectedOperation { get => _selectedOperation; set => SetProperty(ref _selectedOperation, value); }
    public string? SelectedLocation { get => _selectedLocation; set => SetProperty(ref _selectedLocation, value); }
    public int Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
    public int RepeatTimes { get => _repeatTimes; set => SetProperty(ref _repeatTimes, value); }

    // Text properties for AutoCompleteBox two-way binding
    private string _partIDText = string.Empty;
    public string PartIDText
    {
        get => _partIDText;
        set => SetProperty(ref _partIDText, value ?? string.Empty);
    }

    private string _operationText = string.Empty;
    public string OperationText
    {
        get => _operationText;
        set => SetProperty(ref _operationText, value ?? string.Empty);
    }

    private string _locationText = string.Empty;
    public string LocationText
    {
        get => _locationText;
        set => SetProperty(ref _locationText, value ?? string.Empty);
    }

    // String wrapper properties for TextBox binding
    public string QuantityText 
    { 
        get => _quantity.ToString(); 
        set 
        { 
            if (int.TryParse(value, out var result) && result > 0) 
                Quantity = result; 
            this.OnPropertyChanged(nameof(QuantityText)); 
        }
    }
    
    public string RepeatTimesText 
    { 
        get => _repeatTimes.ToString(); 
        set 
        { 
            if (int.TryParse(value, out var result) && result > 0) 
                RepeatTimes = result; 
            this.OnPropertyChanged(nameof(RepeatTimesText)); 
        }
    }

    private string? _selectedPartID;
    private string? _selectedOperation;
    private string? _selectedLocation;
    private int _quantity = 1;
    private int _repeatTimes = 1;
    #endregion

    #region Multiple Locations
    public string? MultiLocationPartID { get => _multiLocationPartID; set => SetProperty(ref _multiLocationPartID, value); }
    public string? MultiLocationOperation { get => _multiLocationOperation; set => SetProperty(ref _multiLocationOperation, value); }
    public int MultiLocationQuantity { get => _multiLocationQuantity; set => SetProperty(ref _multiLocationQuantity, value); }
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
            this.OnPropertyChanged(nameof(MultiLocationQuantityText)); 
        }
    }

    private string? _multiLocationPartID;
    private string? _multiLocationOperation;
    private int _multiLocationQuantity = 1;
    #endregion

    #region Status
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    private bool _isBusy;
    private string _statusMessage = "Ready";
    #endregion

    #region Commands
    public ICommand LoadDataCommand { get; private set; } = null!;
    public ICommand AddMultipleTimesCommand { get; private set; } = null!;
    public ICommand ResetMultipleTimesCommand { get; private set; } = null!;
    public ICommand AddToMultipleLocationsCommand { get; private set; } = null!;
    public ICommand ResetMultipleLocationsCommand { get; private set; } = null!;
    public ICommand SelectAllLocationsCommand { get; private set; } = null!;
    public ICommand ClearAllLocationsCommand { get; private set; } = null!;
    public ICommand ImportFromExcelCommand { get; private set; } = null!;
    public ICommand BackToNormalCommand { get; private set; } = null!;
    public ICommand ToggleFilterPanelCommand { get; private set; } = null!;
    #endregion

    public event EventHandler? BackToNormalRequested;

    public bool CanAddMultipleTimes => !string.IsNullOrWhiteSpace(SelectedPartID) && 
                                      !string.IsNullOrWhiteSpace(SelectedOperation) && 
                                      !string.IsNullOrWhiteSpace(SelectedLocation) && 
                                      Quantity > 0 && RepeatTimes > 0;

    public bool CanAddToMultipleLocations => !string.IsNullOrWhiteSpace(MultiLocationPartID) && 
                                            !string.IsNullOrWhiteSpace(MultiLocationOperation) && 
                                            MultiLocationQuantity > 0;

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
        InitializeCommands();
        InitializeDesignTimeData();
        
        // Setup property change notifications for computed properties
        PropertyChanged += OnPropertyChanged;
    }
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update computed properties when dependencies change
        switch (e.PropertyName)
        {
            case nameof(SelectedPartID):
                OnPropertyChanged(nameof(CanAddMultipleTimes));
                PartIDText = SelectedPartID ?? string.Empty;
                break;
            case nameof(SelectedOperation):
                OnPropertyChanged(nameof(CanAddMultipleTimes));
                OperationText = SelectedOperation ?? string.Empty;
                break;
            case nameof(SelectedLocation):
                OnPropertyChanged(nameof(CanAddMultipleTimes));
                LocationText = SelectedLocation ?? string.Empty;
                break;
            case nameof(Quantity):
                OnPropertyChanged(nameof(CanAddMultipleTimes));
                OnPropertyChanged(nameof(QuantityText));
                break;
            case nameof(RepeatTimes):
                OnPropertyChanged(nameof(CanAddMultipleTimes));
                OnPropertyChanged(nameof(RepeatTimesText));
                break;
            case nameof(MultiLocationPartID):
            case nameof(MultiLocationOperation):
                OnPropertyChanged(nameof(CanAddToMultipleLocations));
                break;
            case nameof(MultiLocationQuantity):
                OnPropertyChanged(nameof(CanAddToMultipleLocations));
                OnPropertyChanged(nameof(MultiLocationQuantityText));
                break;
            case nameof(IsFilterPanelExpanded):
                OnPropertyChanged(nameof(FilterPanelWidth));
                OnPropertyChanged(nameof(CollapseButtonIcon));
                break;
            // Add synchronization from Text properties back to SelectedItem properties
            case nameof(PartIDText):
                if (!string.IsNullOrEmpty(PartIDText) && PartIDOptions.Contains(PartIDText) && SelectedPartID != PartIDText)
                    SelectedPartID = PartIDText;
                break;
            case nameof(OperationText):
                if (!string.IsNullOrEmpty(OperationText) && OperationOptions.Contains(OperationText) && SelectedOperation != OperationText)
                    SelectedOperation = OperationText;
                break;
            case nameof(LocationText):
                if (!string.IsNullOrEmpty(LocationText) && LocationOptions.Contains(LocationText) && SelectedLocation != LocationText)
                    SelectedLocation = LocationText;
                break;
        }
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

    private void InitializeCommands()
    {
        LoadDataCommand = new AsyncCommand(async () =>
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Loading options...";

                // Use Task.Run for CPU-bound operations to properly make this async
                await Task.Run(() =>
                {
                    PartIDOptions.Clear();
                    foreach (var p in new[] { "PART001", "PART002", "PART003", "PART004", "PART005" }) PartIDOptions.Add(p);
                    OperationOptions.Clear();
                    foreach (var o in new[] { "90", "100", "110", "120", "130" }) OperationOptions.Add(o);
                    LocationOptions.Clear();
                    foreach (var l in new[] { "WC01", "WC02", "WC03", "WC04", "WC05" }) LocationOptions.Add(l);

                    AvailableLocations.Clear();
                    foreach (var l in LocationOptions) AvailableLocations.Add(l);
                });

                StatusMessage = "Ready";
            }
            finally { IsBusy = false; }
        });

        AddMultipleTimesCommand = new AsyncCommand(async () =>
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
        }, () => CanAddMultipleTimes);

        ResetMultipleTimesCommand = new RelayCommand(() =>
        {
            ResetMultipleTimes();
        });

        AddToMultipleLocationsCommand = new AsyncCommand(async () =>
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
        }, () => CanAddToMultipleLocations);

        ResetMultipleLocationsCommand = new RelayCommand(() =>
        {
            ResetMultipleLocations();
        });

        SelectAllLocationsCommand = new RelayCommand(() =>
        {
            SelectedLocations.Clear();
            foreach (var loc in AvailableLocations) SelectedLocations.Add(loc);
        });

        ClearAllLocationsCommand = new RelayCommand(() =>
        {
            SelectedLocations.Clear();
        });

        ImportFromExcelCommand = new AsyncCommand(async () =>
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

        BackToNormalCommand = new RelayCommand(() =>
        {
            BackToNormalRequested?.Invoke(this, EventArgs.Empty);
        });

        ToggleFilterPanelCommand = new RelayCommand(() =>
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
