using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for editing existing locations in the MTM system.
/// Provides location selection, modification, and update functionality.
/// </summary>
public partial class EditLocationViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

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

    [ObservableProperty]
    private string? _selectedLocationCode;

    /// <summary>
    /// Available locations for editing.
    /// </summary>
    public ObservableCollection<LocationInfo> AvailableLocations { get; } = new();

    /// <summary>
    /// Available location types.
    /// </summary>
    public ObservableCollection<string> AvailableLocationTypes { get; } = new()
    {
        "Storage", "Work Center", "Receiving", "Shipping", "Quality", "Scrap", "Tool Crib", "Raw Material", "Finished Goods", "WIP"
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
        "Expo Drive", "Vits Drive", "Other"
    };

    public EditLocationViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<EditLocationViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("EditLocationViewModel initialized");
    }

    /// <summary>
    /// Loads available locations for editing.
    /// </summary>
    [RelayCommand]
    private Task LoadLocationsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading locations...";

            using var scope = Logger.BeginScope("LoadLocations");
            Logger.LogInformation("Loading locations for editing");

            // Implementation would load from database
            // var locations = await _databaseService.GetLocationsAsync().ConfigureAwait(false);
            
            AvailableLocations.Clear();
            AvailableParentLocations.Clear();
            
            // Add sample data for now
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "MAIN-A1-1-1", 
                LocationName = "Main Building Zone A Shelf 1", 
                LocationType = "Storage", 
                Building = "Main",
                Zone = "A",
                Aisle = "1",
                Bay = "1", 
                Shelf = "1",
                Capacity = 100,
                IsActive = true,
                AllowMixedParts = false
            });
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "MAIN-A1-1-2", 
                LocationName = "Main Building Zone A Shelf 2", 
                LocationType = "Storage", 
                Building = "Main",
                Zone = "A",
                Aisle = "1",
                Bay = "1", 
                Shelf = "2",
                Capacity = 100,
                IsActive = true,
                AllowMixedParts = true
            });
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "WHA-A1-1-1", 
                LocationName = "Warehouse A Zone A", 
                LocationType = "Storage", 
                Building = "Warehouse A",
                Zone = "A",
                Aisle = "1",
                Bay = "1", 
                Shelf = "1",
                Capacity = 500,
                IsActive = true,
                RequiresTemperatureControl = true
            });
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "RECV-01", 
                LocationName = "Receiving Dock 1", 
                LocationType = "Receiving", 
                Building = "Main",
                Capacity = 50,
                IsActive = true
            });

            // Parent locations (zones, buildings)
            AvailableParentLocations.Add(new LocationInfo { LocationCode = "MAIN-A", LocationName = "Main Building - Zone A", LocationType = "Zone" });
            AvailableParentLocations.Add(new LocationInfo { LocationCode = "MAIN-B", LocationName = "Main Building - Zone B", LocationType = "Zone" });
            AvailableParentLocations.Add(new LocationInfo { LocationCode = "WHA-A", LocationName = "Warehouse A - Zone A", LocationType = "Zone" });

            StatusMessage = $"Loaded {AvailableLocations.Count} locations";
            Logger.LogInformation("Successfully loaded {LocationCount} locations", AvailableLocations.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading locations: {ex.Message}";
            Logger.LogError(ex, "Error loading locations for editing");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads selected location details for editing.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLoadLocationDetails))]
    private Task LoadLocationDetailsAsync()
    {
        if (string.IsNullOrEmpty(SelectedLocationCode)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading location details...";

            using var scope = Logger.BeginScope("LoadLocationDetails");
            Logger.LogInformation("Loading details for location {LocationCode}", SelectedLocationCode);

            var selectedLocation = AvailableLocations.FirstOrDefault(loc => loc.LocationCode == SelectedLocationCode);
            if (selectedLocation != null)
            {
                LocationName = selectedLocation.LocationName;
                LocationType = selectedLocation.LocationType;
                ParentLocationCode = selectedLocation.ParentLocationCode;
                Building = selectedLocation.Building;
                Zone = selectedLocation.Zone;
                Aisle = selectedLocation.Aisle;
                Bay = selectedLocation.Bay;
                Shelf = selectedLocation.Shelf;
                Capacity = selectedLocation.Capacity;
                IsActive = selectedLocation.IsActive;
                AllowMixedParts = selectedLocation.AllowMixedParts;
                RequiresTemperatureControl = selectedLocation.RequiresTemperatureControl;
                Notes = selectedLocation.Notes ?? string.Empty;

                StatusMessage = "Location details loaded";
                Logger.LogInformation("Successfully loaded details for location {LocationCode}", SelectedLocationCode);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading location details: {ex.Message}";
            Logger.LogError(ex, "Error loading location details for {LocationCode}", SelectedLocationCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if location details can be loaded.
    /// </summary>
    private bool CanLoadLocationDetails() => !string.IsNullOrEmpty(SelectedLocationCode) && !IsLoading;

    /// <summary>
    /// Updates the selected location with modified information.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUpdateLocation))]
    private Task UpdateLocationAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Updating location...";

            using var scope = Logger.BeginScope("UpdateLocation");
            Logger.LogInformation("Updating location {LocationCode}", SelectedLocationCode);

            // Validate input
            if (string.IsNullOrWhiteSpace(LocationName))
            {
                StatusMessage = "Please fill in all required fields";
                return Task.CompletedTask;
            }

            // Implementation would update in database
            // var result = await _databaseService.UpdateLocationAsync(SelectedLocationCode, LocationName, LocationType, 
            //     ParentLocationCode, Building, Zone, Aisle, Bay, Shelf, Capacity, 
            //     IsActive, AllowMixedParts, RequiresTemperatureControl, Notes).ConfigureAwait(false);

            // Update local collection
            var location = AvailableLocations.FirstOrDefault(loc => loc.LocationCode == SelectedLocationCode);
            if (location != null)
            {
                location.LocationName = LocationName;
                location.LocationType = LocationType;
                location.ParentLocationCode = ParentLocationCode;
                location.Building = Building;
                location.Zone = Zone;
                location.Aisle = Aisle;
                location.Bay = Bay;
                location.Shelf = Shelf;
                location.Capacity = Capacity;
                location.IsActive = IsActive;
                location.AllowMixedParts = AllowMixedParts;
                location.RequiresTemperatureControl = RequiresTemperatureControl;
                location.Notes = Notes;
            }

            StatusMessage = "Location updated successfully";
            Logger.LogInformation("Successfully updated location {LocationCode}", SelectedLocationCode);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating location: {ex.Message}";
            Logger.LogError(ex, "Error updating location {LocationCode}", SelectedLocationCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if location can be updated.
    /// </summary>
    private bool CanUpdateLocation() => !string.IsNullOrEmpty(SelectedLocationCode) && 
                                       !string.IsNullOrWhiteSpace(LocationName) && 
                                       Capacity >= 0 &&
                                       !IsLoading;

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
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
        SelectedLocationCode = null;
        StatusMessage = "Form reset";

        Logger.LogInformation("Location edit form reset");
        return Task.CompletedTask;
    }

    partial void OnSelectedLocationCodeChanged(string? value)
    {
        // Update command states
        LoadLocationDetailsCommand.NotifyCanExecuteChanged();
        UpdateLocationCommand.NotifyCanExecuteChanged();
        
        // Clear form when selection changes
        if (string.IsNullOrEmpty(value))
        {
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
        }
    }

    partial void OnLocationNameChanged(string value)
    {
        // Update command state when name changes
        UpdateLocationCommand.NotifyCanExecuteChanged();
    }

    partial void OnCapacityChanged(int value)
    {
        // Update command state when capacity changes
        UpdateLocationCommand.NotifyCanExecuteChanged();
    }
}
