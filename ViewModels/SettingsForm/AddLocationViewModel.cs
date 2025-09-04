using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for adding new locations to the MTM system.
/// Provides location creation with validation and hierarchical organization.
/// </summary>
public partial class AddLocationViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    [Required(ErrorMessage = "Location code is required")]
    [RegularExpression(@"^[A-Z0-9\-]{2,10}$", ErrorMessage = "Location code must be 2-10 characters (A-Z, 0-9, -)")]
    private string _locationCode = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Location name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    private string _locationName = string.Empty;

    [ObservableProperty]
    private string _locationType = "Storage";

    [ObservableProperty]
    private string? _parentLocationCode;

    [ObservableProperty]
    private string _building = "Main";

    [ObservableProperty]
    private string _zone = "A";

    [ObservableProperty]
    private string _aisle = "1";

    [ObservableProperty]
    private string _bay = "1";

    [ObservableProperty]
    private string _shelf = "1";

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Capacity must be non-negative")]
    private int _capacity = 100;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _allowMixedParts;

    [ObservableProperty]
    private bool _requiresTemperatureControl;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string _notes = string.Empty;

    /// <summary>
    /// Available location types.
    /// </summary>
    public ObservableCollection<string> AvailableLocationTypes { get; } = new()
    {
        "Storage", "Work Center", "Receiving", "Shipping", "Quality", "Scrap", "Tool Crib", "Raw Material", "Finished Goods"
    };

    /// <summary>
    /// Available parent locations for hierarchical organization.
    /// </summary>
    public ObservableCollection<LocationInfo> AvailableParentLocations { get; } = new();

    /// <summary>
    /// Available buildings.
    /// </summary>
    public ObservableCollection<string> AvailableBuildings { get; } = new()
    {
        "Main", "Warehouse A", "Warehouse B", "Manufacturing", "Office", "External"
    };

    /// <summary>
    /// Existing location codes to prevent duplicates.
    /// </summary>
    public ObservableCollection<string> ExistingLocations { get; } = new();

    /// <summary>
    /// Gets whether the location code already exists.
    /// </summary>
    public bool LocationExists => !string.IsNullOrEmpty(LocationCode) && 
                                 ExistingLocations.Contains(LocationCode);

    /// <summary>
    /// Gets the generated location code based on components.
    /// </summary>
    public string GeneratedLocationCode => $"{Building}-{Zone}{Aisle}-{Bay}-{Shelf}";

    public AddLocationViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<AddLocationViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("AddLocationViewModel initialized");
        
        // Load initial data
        _ = LoadParentLocationsAsync();
        _ = LoadExistingLocationsAsync();
    }

    /// <summary>
    /// Loads available parent locations.
    /// </summary>
    [RelayCommand]
    private Task LoadParentLocationsAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("LoadParentLocations");
            Logger.LogInformation("Loading parent locations");

            // Implementation would load from database
            // var locations = await _databaseService.GetLocationsAsync().ConfigureAwait(false);
            
            AvailableParentLocations.Clear();
            
            // Add sample data for now
            AvailableParentLocations.Add(new LocationInfo { LocationCode = "MAIN-A", LocationName = "Main Building - Zone A", LocationType = "Zone" });
            AvailableParentLocations.Add(new LocationInfo { LocationCode = "MAIN-B", LocationName = "Main Building - Zone B", LocationType = "Zone" });
            AvailableParentLocations.Add(new LocationInfo { LocationCode = "WHA-A", LocationName = "Warehouse A - Zone A", LocationType = "Zone" });
            AvailableParentLocations.Add(new LocationInfo { LocationCode = "WHA-B", LocationName = "Warehouse A - Zone B", LocationType = "Zone" });

            Logger.LogInformation("Successfully loaded {ParentLocationCount} parent locations", AvailableParentLocations.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading parent locations");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads existing location codes.
    /// </summary>
    [RelayCommand]
    private Task LoadExistingLocationsAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("LoadExistingLocations");
            Logger.LogInformation("Loading existing location codes");

            // Implementation would load from database
            // var locations = await _databaseService.GetLocationCodesAsync().ConfigureAwait(false);
            
            ExistingLocations.Clear();
            
            // Add sample data for now
            ExistingLocations.Add("MAIN-A1-1-1");
            ExistingLocations.Add("MAIN-A1-1-2");
            ExistingLocations.Add("MAIN-A1-2-1");
            ExistingLocations.Add("WHA-A1-1-1");
            ExistingLocations.Add("RECV-01");
            ExistingLocations.Add("SHIP-01");
            ExistingLocations.Add("QC-INSP");

            Logger.LogInformation("Successfully loaded {LocationCount} existing locations", ExistingLocations.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading existing locations");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Generates a location code based on the selected components.
    /// </summary>
    [RelayCommand]
    private Task GenerateLocationCodeAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("GenerateLocationCode");
            Logger.LogInformation("Generating location code");

            var generated = GeneratedLocationCode;
            
            // Check if generated code already exists
            if (ExistingLocations.Contains(generated))
            {
                // Try to find next available shelf number
                int shelfNumber = int.Parse(Shelf);
                while (ExistingLocations.Contains($"{Building}-{Zone}{Aisle}-{Bay}-{shelfNumber}") && shelfNumber < 99)
                {
                    shelfNumber++;
                }
                
                if (shelfNumber < 99)
                {
                    Shelf = shelfNumber.ToString();
                    generated = GeneratedLocationCode;
                }
            }

            LocationCode = generated;
            StatusMessage = $"Generated location code: {generated}";
            
            Logger.LogInformation("Generated location code: {LocationCode}", generated);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error generating location code: {ex.Message}";
            Logger.LogError(ex, "Error generating location code");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates a new location with the specified details.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreateLocation))]
    private Task CreateLocationAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Creating location...";

            using var scope = Logger.BeginScope("CreateLocation");
            Logger.LogInformation("Creating location {LocationCode}", LocationCode);

            // Validate input
            if (string.IsNullOrWhiteSpace(LocationCode) || string.IsNullOrWhiteSpace(LocationName))
            {
                StatusMessage = "Please fill in all required fields";
                return Task.CompletedTask;
            }

            if (LocationExists)
            {
                StatusMessage = "Location code already exists";
                return Task.CompletedTask;
            }

            // Validate location code format
            if (!System.Text.RegularExpressions.Regex.IsMatch(LocationCode, @"^[A-Z0-9\-]{2,10}$"))
            {
                StatusMessage = "Location code format is invalid";
                return Task.CompletedTask;
            }

            // Implementation would save to database
            // var result = await _databaseService.CreateLocationAsync(
            //     LocationCode, LocationName, LocationType, ParentLocationCode, 
            //     Building, Zone, Aisle, Bay, Shelf, Capacity, 
            //     IsActive, AllowMixedParts, RequiresTemperatureControl, Notes).ConfigureAwait(false);

            // Add to existing locations
            ExistingLocations.Add(LocationCode);

            StatusMessage = "Location created successfully";
            Logger.LogInformation("Successfully created location {LocationCode}", LocationCode);

            // Reset form after successful creation
            _ = ResetFormAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating location: {ex.Message}";
            Logger.LogError(ex, "Error creating location {LocationCode}", LocationCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if location can be created.
    /// </summary>
    private bool CanCreateLocation() => !string.IsNullOrWhiteSpace(LocationCode) && 
                                       !string.IsNullOrWhiteSpace(LocationName) &&
                                       !LocationExists &&
                                       Capacity >= 0 &&
                                       !IsLoading;

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        LocationCode = string.Empty;
        LocationName = string.Empty;
        LocationType = "Storage";
        ParentLocationCode = null;
        Building = "Main";
        Zone = "A";
        Aisle = "1";
        Bay = "1";
        Shelf = "1";
        Capacity = 100;
        IsActive = true;
        AllowMixedParts = false;
        RequiresTemperatureControl = false;
        Notes = string.Empty;
        StatusMessage = "Form reset";

        Logger.LogInformation("Location creation form reset");
        return Task.CompletedTask;
    }

    partial void OnLocationCodeChanged(string value)
    {
        // Update command state and validation
        CreateLocationCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(LocationExists));
        
        if (LocationExists)
        {
            StatusMessage = "Location code already exists";
        }
        else if (!string.IsNullOrEmpty(value))
        {
            StatusMessage = "Ready";
        }
    }

    partial void OnLocationNameChanged(string value)
    {
        // Update command state when name changes
        CreateLocationCommand.NotifyCanExecuteChanged();
    }

    partial void OnCapacityChanged(int value)
    {
        // Update command state when capacity changes
        CreateLocationCommand.NotifyCanExecuteChanged();
    }

    partial void OnBuildingChanged(string value)
    {
        // Update generated location code when building changes
        OnPropertyChanged(nameof(GeneratedLocationCode));
    }

    partial void OnZoneChanged(string value)
    {
        // Update generated location code when zone changes
        OnPropertyChanged(nameof(GeneratedLocationCode));
    }

    partial void OnAisleChanged(string value)
    {
        // Update generated location code when aisle changes
        OnPropertyChanged(nameof(GeneratedLocationCode));
    }

    partial void OnBayChanged(string value)
    {
        // Update generated location code when bay changes
        OnPropertyChanged(nameof(GeneratedLocationCode));
    }

    partial void OnShelfChanged(string value)
    {
        // Update generated location code when shelf changes
        OnPropertyChanged(nameof(GeneratedLocationCode));
    }
}

/// <summary>
/// Location information model.
/// </summary>
public class LocationInfo
{
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string? ParentLocationCode { get; set; }
    public string Building { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string Aisle { get; set; } = string.Empty;
    public string Bay { get; set; } = string.Empty;
    public string Shelf { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AllowMixedParts { get; set; }
    public bool RequiresTemperatureControl { get; set; }
    public string? Notes { get; set; }
}
