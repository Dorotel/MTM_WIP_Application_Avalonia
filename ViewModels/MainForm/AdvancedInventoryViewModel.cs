using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

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
    private readonly IConfigurationService? _configurationService;

    // Common options
    public ObservableCollection<string> PartIDOptions { get; } = new();
    public ObservableCollection<string> OperationOptions { get; } = new();
    public ObservableCollection<string> LocationOptions { get; } = new();

    #region CollapsiblePanel Properties
    /// <summary>
    /// Gets or sets whether the mode selection panel is expanded
    /// </summary>
    [ObservableProperty]
    private bool _isModeSelectionExpanded = true;

    /// <summary>
    /// Gets or sets whether the filter panel is expanded (legacy support)
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

    #region Mode Selection Properties
    /// <summary>
    /// Gets or sets whether Multiple Times mode is active
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMultipleLocationsMode))]
    private bool _isMultipleTimesMode = true; // Default to first mode

    /// <summary>
    /// Gets or sets whether Multiple Locations mode is active
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMultipleTimesMode))]
    private bool _isMultipleLocationsMode;
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
    public AdvancedInventoryViewModel() : this(CreateDesignTimeLogger<AdvancedInventoryViewModel>(), null)
    {
        // Only initialize for design-time
        if (IsDesignMode())
        {
            InitializeDesignTimeData();
        }
    }

    public AdvancedInventoryViewModel(ILogger<AdvancedInventoryViewModel> logger, IConfigurationService? configurationService = null) : base(logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationService = configurationService;
        
        InitializeDesignTimeData();
        
        // Setup property change notifications for computed properties
        PropertyChanged += OnPropertyChanged;

        // Load master data if not in design mode
        if (_configurationService != null && !IsDesignMode())
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load initial data");
                }
            });
        }

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
            // Mode selection - ensure only one mode is active at a time
            case nameof(IsMultipleTimesMode):
                if (IsMultipleTimesMode)
                {
                    IsMultipleLocationsMode = false;
                }
                break;
            case nameof(IsMultipleLocationsMode):
                if (IsMultipleLocationsMode)
                {
                    IsMultipleTimesMode = false;
                }
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
    /// Loads ComboBox data from database for advanced inventory operations using stored procedures
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Loading options from database...";

            // Get connection string from configuration service
            var connectionString = _configurationService?.GetConnectionString() ?? throw new InvalidOperationException("Configuration service not available");

            // Load Part IDs using stored procedure
            var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            if (partResult.Status == 1)
            {
                PartIDOptions.Clear();
                foreach (DataRow row in partResult.Data.Rows)
                {
                    var partId = row["PartID"].ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(partId))
                        PartIDOptions.Add(partId);
                }
                _logger.LogInformation("Loaded {Count} part IDs from database", PartIDOptions.Count);
            }
            else
            {
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Failed to load parts: Status {partResult.Status}"),
                    "Load Part IDs",
                    Environment.UserName
                );
                // Keep empty collection to indicate data unavailability
            }

            // Load Operations using stored procedure
            var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "md_operation_numbers_Get_All",
                new Dictionary<string, object>()
            );

            if (operationResult.Status == 1)
            {
                OperationOptions.Clear();
                foreach (DataRow row in operationResult.Data.Rows)
                {
                    var operation = row["OperationNumber"].ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(operation))
                        OperationOptions.Add(operation);
                }
                _logger.LogInformation("Loaded {Count} operations from database", OperationOptions.Count);
            }
            else
            {
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Failed to load operations: Status {operationResult.Status}"),
                    "Load Operations",
                    Environment.UserName
                );
                // Keep empty collection to indicate data unavailability
            }

            // Load Locations using stored procedure
            var locationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "md_locations_Get_All",
                new Dictionary<string, object>()
            );

            if (locationResult.Status == 1)
            {
                LocationOptions.Clear();
                AvailableLocations.Clear();
                foreach (DataRow row in locationResult.Data.Rows)
                {
                    var location = row["Location"].ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(location))
                    {
                        LocationOptions.Add(location);
                        AvailableLocations.Add(location);
                    }
                }
                _logger.LogInformation("Loaded {Count} locations from database", LocationOptions.Count);
            }
            else
            {
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Failed to load locations: Status {locationResult.Status}"),
                    "Load Locations",
                    Environment.UserName
                );
                // Keep empty collection to indicate data unavailability
            }

            StatusMessage = $"Loaded {PartIDOptions.Count} parts, {OperationOptions.Count} operations, {LocationOptions.Count} locations";
            _logger.LogInformation("Successfully loaded master data for advanced inventory operations");
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Load Master Data", Environment.UserName);
            StatusMessage = "Error loading data from database";
        }
        finally 
        { 
            IsBusy = false; 
        }
    }

    /// <summary>
    /// Adds the same item multiple times to inventory using database stored procedures
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddMultipleTimes))]
    private async Task AddMultipleTimesAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = $"Adding {RepeatTimes}x {SelectedPartID} to {SelectedLocation}...";
            
            var connectionString = _configurationService?.GetConnectionString() ?? throw new InvalidOperationException("Configuration service not available");
            var successCount = 0;
            var failureCount = 0;

            // Add each transaction individually
            for (int i = 0; i < RepeatTimes; i++)
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_PartID"] = SelectedPartID ?? string.Empty,
                    ["p_Location"] = SelectedLocation ?? string.Empty,
                    ["p_Operation"] = SelectedOperation ?? string.Empty,
                    ["p_Quantity"] = Quantity,
                    ["p_ItemType"] = "Standard",
                    ["p_User"] = Environment.UserName,
                    ["p_Notes"] = $"Advanced Inventory - Multiple Times ({i + 1} of {RepeatTimes})"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    connectionString,
                    "inv_inventory_Add_Item",
                    parameters
                );

                if (result.Status == 1)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                    _logger.LogWarning("Failed to add inventory item {Count} of {Total}: Status {Status}", 
                        i + 1, RepeatTimes, result.Status);
                }

                // Update progress
                StatusMessage = $"Added {successCount} of {RepeatTimes} transactions...";
            }
            
            if (successCount > 0)
            {
                StatusMessage = $"Successfully added {successCount} transactions" + (failureCount > 0 ? $" ({failureCount} failed)" : "");
                _logger.LogInformation("Added {SuccessCount} items of {PartID} to {Location} (Multiple Times mode)", 
                    successCount, SelectedPartID, SelectedLocation);
                
                if (failureCount == 0)
                    ResetMultipleTimes();
            }
            else
            {
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"All {RepeatTimes} transactions failed"),
                    "Add Multiple Times",
                    Environment.UserName
                );
            }
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Add Multiple Times", Environment.UserName);
            StatusMessage = "Error adding multiple items";
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
    /// Adds the same item to multiple locations using database stored procedures
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddToMultipleLocations))]
    private async Task AddToMultipleLocationsAsync()
    {
        try
        {
            IsBusy = true;
            var count = SelectedLocations.Count;
            StatusMessage = $"Adding {MultiLocationPartID} to {count} locations...";
            
            var connectionString = _configurationService?.GetConnectionString() ?? throw new InvalidOperationException("Configuration service not available");
            var successCount = 0;
            var failureCount = 0;

            // Add to each selected location
            foreach (var location in SelectedLocations)
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_PartID"] = MultiLocationPartID ?? string.Empty,
                    ["p_Location"] = location,
                    ["p_Operation"] = MultiLocationOperation ?? string.Empty,
                    ["p_Quantity"] = MultiLocationQuantity,
                    ["p_ItemType"] = "Standard",
                    ["p_User"] = Environment.UserName,
                    ["p_Notes"] = $"Advanced Inventory - Multiple Locations (Location: {location})"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    connectionString,
                    "inv_inventory_Add_Item",
                    parameters
                );

                if (result.Status == 1)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                    _logger.LogWarning("Failed to add inventory item to location {Location}: Status {Status}", 
                        location, result.Status);
                }

                // Update progress
                StatusMessage = $"Added to {successCount} of {count} locations...";
            }
            
            if (successCount > 0)
            {
                StatusMessage = $"Successfully added to {successCount} locations" + (failureCount > 0 ? $" ({failureCount} failed)" : "");
                _logger.LogInformation("Added {PartID} to {SuccessCount} locations (Multiple Locations mode)", 
                    MultiLocationPartID, successCount);
                
                if (failureCount == 0)
                    ResetMultipleLocations();
            }
            else
            {
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"All {count} location additions failed"),
                    "Add To Multiple Locations",
                    Environment.UserName
                );
            }
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Add To Multiple Locations", Environment.UserName);
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
