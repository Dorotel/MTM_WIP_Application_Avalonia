using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// ViewModel for advanced inventory operations with three specialized areas:
/// - Single item multiple times
/// - Single item multiple locations  
/// - Import from Excel functionality
/// Uses MVVM Community Toolkit for modern .NET patterns.
/// </summary>
public partial class AdvancedInventoryViewModel : BaseViewModel
{
    private readonly ILogger<AdvancedInventoryViewModel> _logger;

    // Common options
    public ObservableCollection<string> PartIDOptions { get; } = new();
    public ObservableCollection<string> OperationOptions { get; } = new();
    public ObservableCollection<string> LocationOptions { get; } = new();

    #region Filter Panel Properties
    /// <summary>
    /// Gets or sets whether the filter panel is expanded
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilterPanelWidth))]
    [NotifyPropertyChangedFor(nameof(CollapseButtonIcon))]
    private bool _isFilterPanelExpanded = true;

    /// <summary>
    /// Gets the width of the filter panel based on expansion state
    /// </summary>
    public string FilterPanelWidth => IsFilterPanelExpanded ? "200" : "32";
    
    /// <summary>
    /// Gets the icon for the collapse button based on expansion state
    /// </summary>
    public string CollapseButtonIcon => IsFilterPanelExpanded ? "ChevronLeft" : "ChevronRight";
    #endregion

    #region Multiple Times
    /// <summary>
    /// Gets or sets the selected part ID for multiple times operation
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanAddMultipleTimes))]
    private string? _selectedPartID;

    /// <summary>
    /// Gets or sets the selected operation for multiple times operation
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanAddMultipleTimes))]
    private string? _selectedOperation;

    /// <summary>
    /// Gets or sets the selected location for multiple times operation
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanAddMultipleTimes))]
    private string? _selectedLocation;

    /// <summary>
    /// Gets or sets the quantity for each addition
    /// </summary>
    [ObservableProperty]
    [Range(1, 999999, ErrorMessage = "Quantity must be between 1 and 999,999")]
    [NotifyPropertyChangedFor(nameof(CanAddMultipleTimes))]
    [NotifyPropertyChangedFor(nameof(QuantityText))]
    private int _quantity = 1;

    /// <summary>
    /// Gets or sets the number of times to repeat the addition
    /// </summary>
    [ObservableProperty]
    [Range(1, 999, ErrorMessage = "Repeat times must be between 1 and 999")]
    [NotifyPropertyChangedFor(nameof(CanAddMultipleTimes))]
    [NotifyPropertyChangedFor(nameof(RepeatTimesText))]
    private int _repeatTimes = 1;

    /// <summary>
    /// Gets or sets the text representation of the part ID for AutoCompleteBox binding
    /// </summary>
    [ObservableProperty]
    private string _partIDText = string.Empty;

    /// <summary>
    /// Gets or sets the text representation of the operation for AutoCompleteBox binding
    /// </summary>
    [ObservableProperty]
    private string _operationText = string.Empty;

    /// <summary>
    /// Gets or sets the text representation of the location for AutoCompleteBox binding
    /// </summary>
    [ObservableProperty]
    private string _locationText = string.Empty;

    // String wrapper properties for TextBox binding
    public string QuantityText 
    { 
        get => Quantity.ToString(); 
        set 
        { 
            if (int.TryParse(value, out var result) && result > 0) 
                Quantity = result; 
            this.OnPropertyChanged(nameof(QuantityText)); 
        }
    }
    
    public string RepeatTimesText 
    { 
        get => RepeatTimes.ToString(); 
        set 
        { 
            if (int.TryParse(value, out var result) && result > 0) 
                RepeatTimes = result; 
            this.OnPropertyChanged(nameof(RepeatTimesText)); 
        }
    }

    #endregion

    #region Multiple Locations
    /// <summary>
    /// Gets or sets the part ID for multi-location operation
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanAddToMultipleLocations))]
    private string? _multiLocationPartID;

    /// <summary>
    /// Gets or sets the operation for multi-location operation
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanAddToMultipleLocations))]
    private string? _multiLocationOperation;

    /// <summary>
    /// Gets or sets the quantity for multi-location operation
    /// </summary>
    [ObservableProperty]
    [Range(1, 999999, ErrorMessage = "Quantity must be between 1 and 999,999")]
    [NotifyPropertyChangedFor(nameof(CanAddToMultipleLocations))]
    [NotifyPropertyChangedFor(nameof(MultiLocationQuantityText))]
    private int _multiLocationQuantity = 1;

    public ObservableCollection<string> AvailableLocations { get; } = new();
    public ObservableCollection<string> SelectedLocations { get; } = new();

    // String wrapper property for TextBox binding
    public string MultiLocationQuantityText 
    { 
        get => MultiLocationQuantity.ToString(); 
        set 
        { 
            if (int.TryParse(value, out var result) && result > 0) 
                MultiLocationQuantity = result; 
            this.OnPropertyChanged(nameof(MultiLocationQuantityText)); 
        }
    }
    #endregion

    #region Status
    /// <summary>
    /// Gets or sets whether the ViewModel is busy processing operations
    /// </summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>
    /// Gets or sets the current status message
    /// </summary>
    [ObservableProperty]
    private string _statusMessage = "Ready";
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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        InitializeDesignTimeData();
        
        // Setup property change notifications for computed properties
        PropertyChanged += OnPropertyChanged;

        _logger.LogInformation("AdvancedInventoryViewModel initialized with dependency injection");
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

    #region Command Implementations

    /// <summary>
    /// Loads ComboBox data from database for advanced inventory operations
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
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
            }).ConfigureAwait(false);

            StatusMessage = "Ready";
            _logger.LogInformation("Loaded ComboBox data for advanced inventory operations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ComboBox data for advanced inventory");
            StatusMessage = "Error loading data";
        }
        finally 
        { 
            IsBusy = false; 
        }
    }

    /// <summary>
    /// Adds the same item multiple times to inventory
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddMultipleTimes))]
    private async Task AddMultipleTimesAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = $"Adding {RepeatTimes}x {SelectedPartID}...";
            
            // TODO: Implement Dao_Inventory calls in a loop
            await Task.Delay(500).ConfigureAwait(false);
            
            StatusMessage = $"Added {RepeatTimes} item(s)";
            _logger.LogInformation("Added {RepeatTimes} items of {PartID} to {Location}", 
                RepeatTimes, SelectedPartID, SelectedLocation);
            
            ResetMultipleTimes();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add multiple items");
            StatusMessage = "Error adding items";
        }
        finally 
        { 
            IsBusy = false; 
        }
    }

    /// <summary>
    /// Resets the multiple times form
    /// </summary>
    [RelayCommand]
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
        
        _logger.LogInformation("Reset multiple times form");
    }

    /// <summary>
    /// Adds the same item to multiple locations
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddToMultipleLocations))]
    private async Task AddToMultipleLocationsAsync()
    {
        try
        {
            IsBusy = true;
            var count = SelectedLocations.Count;
            StatusMessage = $"Adding to {count} locations...";
            
            // TODO: iterate locations and call Dao_Inventory
            await Task.Delay(500).ConfigureAwait(false);
            
            StatusMessage = $"Added to {count} location(s)";
            _logger.LogInformation("Added {PartID} to {Count} locations", 
                MultiLocationPartID, count);
            
            ResetMultipleLocations();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add to multiple locations");
            StatusMessage = "Error adding to locations";
        }
        finally 
        { 
            IsBusy = false; 
        }
    }

    /// <summary>
    /// Resets the multiple locations form
    /// </summary>
    [RelayCommand]
    private void ResetMultipleLocations()
    {
        MultiLocationPartID = null;
        MultiLocationOperation = null;
        MultiLocationQuantity = 1;
        SelectedLocations.Clear();
        
        _logger.LogInformation("Reset multiple locations form");
    }

    /// <summary>
    /// Selects all available locations
    /// </summary>
    [RelayCommand]
    private void SelectAllLocations()
    {
        SelectedLocations.Clear();
        foreach (var loc in AvailableLocations) 
            SelectedLocations.Add(loc);
        
        _logger.LogInformation("Selected all {Count} locations", AvailableLocations.Count);
    }

    /// <summary>
    /// Clears all selected locations
    /// </summary>
    [RelayCommand]
    private void ClearAllLocations()
    {
        SelectedLocations.Clear();
        _logger.LogInformation("Cleared all selected locations");
    }

    /// <summary>
    /// Imports inventory data from Excel file
    /// </summary>
    [RelayCommand]
    private async Task ImportFromExcelAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Importing from Excel...";
            
            // TODO: Helper_Excel integration
            await Task.Delay(800).ConfigureAwait(false);
            
            StatusMessage = "Import complete";
            _logger.LogInformation("Excel import operation completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import from Excel");
            StatusMessage = "Import failed";
        }
        finally 
        { 
            IsBusy = false; 
        }
    }

    /// <summary>
    /// Returns to normal inventory view
    /// </summary>
    [RelayCommand]
    private void BackToNormal()
    {
        BackToNormalRequested?.Invoke(this, EventArgs.Empty);
        _logger.LogInformation("Back to normal view requested");
    }

    /// <summary>
    /// Toggles the filter panel expansion state
    /// </summary>
    [RelayCommand]
    private void ToggleFilterPanel()
    {
        IsFilterPanelExpanded = !IsFilterPanelExpanded;
        _logger.LogDebug("Filter panel toggled to {State}", IsFilterPanelExpanded ? "expanded" : "collapsed");
    }

    #endregion
}
