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
/// ViewModel for removing locations from the MTM system.
/// Provides location selection, deactivation, and removal functionality with safety measures.
/// </summary>
public partial class RemoveLocationViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    private string? _selectedLocationCode;

    [ObservableProperty]
    private bool _permanentlyRemove;

    [ObservableProperty]
    [Required(ErrorMessage = "Reason is required for location removal")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    private string _removalReason = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private bool _confirmationRequired = true;

    [ObservableProperty]
    [Required(ErrorMessage = "Please confirm by typing 'CONFIRM'")]
    private string _confirmationText = string.Empty;

    /// <summary>
    /// Available locations for removal.
    /// </summary>
    public ObservableCollection<LocationInfo> AvailableLocations { get; } = new();

    /// <summary>
    /// Locations that have inventory or child locations (cannot be deleted).
    /// </summary>
    public ObservableCollection<string> LocationsWithContent { get; } = new();

    /// <summary>
    /// Gets selected location details.
    /// </summary>
    public LocationInfo? SelectedLocation => AvailableLocations.FirstOrDefault(loc => loc.LocationCode == SelectedLocationCode);

    /// <summary>
    /// Gets whether selected location has content.
    /// </summary>
    public bool SelectedLocationHasContent => !string.IsNullOrEmpty(SelectedLocationCode) && 
                                             LocationsWithContent.Contains(SelectedLocationCode);

    /// <summary>
    /// Gets content details for the selected location.
    /// </summary>
    public string ContentDetails { get; private set; } = string.Empty;

    public RemoveLocationViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<RemoveLocationViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("RemoveLocationViewModel initialized");
    }

    /// <summary>
    /// Loads available locations for removal.
    /// </summary>
    [RelayCommand]
    private Task LoadLocationsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading locations...";

            using var scope = Logger.BeginScope("LoadLocations");
            Logger.LogInformation("Loading locations for removal");

            // Implementation would load from database
            // var locations = await _databaseService.GetLocationsAsync().ConfigureAwait(false);
            // var contentData = await _databaseService.GetLocationsWithContentAsync().ConfigureAwait(false);
            
            AvailableLocations.Clear();
            LocationsWithContent.Clear();

            // Add sample data for now
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "MAIN-A1-1-1", 
                LocationName = "Main Building Zone A Shelf 1", 
                LocationType = "Storage", 
                IsActive = true 
            });
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "MAIN-A1-1-2", 
                LocationName = "Main Building Zone A Shelf 2", 
                LocationType = "Storage", 
                IsActive = true 
            });
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "WHA-A1-1-1", 
                LocationName = "Warehouse A Zone A", 
                LocationType = "Storage", 
                IsActive = true 
            });
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "RECV-01", 
                LocationName = "Receiving Dock 1", 
                LocationType = "Receiving", 
                IsActive = true 
            });
            AvailableLocations.Add(new LocationInfo 
            { 
                LocationCode = "OLD-LOC-01", 
                LocationName = "Obsolete Location", 
                LocationType = "Storage", 
                IsActive = false 
            });

            // Locations with content (cannot be permanently deleted)
            LocationsWithContent.Add("MAIN-A1-1-1");
            LocationsWithContent.Add("WHA-A1-1-1");
            LocationsWithContent.Add("RECV-01");

            StatusMessage = $"Loaded {AvailableLocations.Count} locations";
            Logger.LogInformation("Successfully loaded {LocationCount} locations for removal", AvailableLocations.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading locations: {ex.Message}";
            Logger.LogError(ex, "Error loading locations for removal");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a location has inventory or child locations.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCheckContent))]
    private Task CheckContentAsync()
    {
        if (string.IsNullOrEmpty(SelectedLocationCode)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Checking location content...";

            using var scope = Logger.BeginScope("CheckContent");
            Logger.LogInformation("Checking content for location {LocationCode}", SelectedLocationCode);

            // Implementation would check database for content
            // var contentResult = await _databaseService.GetLocationContentAsync(SelectedLocationCode).ConfigureAwait(false);

            bool hasContent = LocationsWithContent.Contains(SelectedLocationCode);
            
            if (hasContent)
            {
                // Sample content details
                ContentDetails = SelectedLocationCode switch
                {
                    "MAIN-A1-1-1" => "Contains 45 units of 3 different parts",
                    "WHA-A1-1-1" => "Contains 128 units of 7 different parts, has 2 child locations",
                    "RECV-01" => "Active receiving location with 12 pending receipts",
                    _ => "Active content detected"
                };
                
                StatusMessage = "Location has content - can only be deactivated";
                PermanentlyRemove = false; // Force deactivation only
            }
            else
            {
                ContentDetails = "No inventory or child locations found";
                StatusMessage = "Location is empty - can be permanently removed";
            }

            OnPropertyChanged(nameof(ContentDetails));
            Logger.LogInformation("Location {LocationCode} content check: {HasContent}", SelectedLocationCode, hasContent);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error checking content: {ex.Message}";
            Logger.LogError(ex, "Error checking content for location {LocationCode}", SelectedLocationCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if content can be checked.
    /// </summary>
    private bool CanCheckContent() => !string.IsNullOrEmpty(SelectedLocationCode) && !IsLoading;

    /// <summary>
    /// Removes or deactivates the selected location.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRemoveLocation))]
    private async Task RemoveLocationAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = PermanentlyRemove ? "Permanently removing location..." : "Deactivating location...";

            using var scope = Logger.BeginScope("RemoveLocation");
            Logger.LogInformation("Removing location {LocationCode} (Permanent: {Permanent})", SelectedLocationCode, PermanentlyRemove);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(RemovalReason))
            {
                StatusMessage = "Please provide a reason for removal";
                return;
            }

            if (ConfirmationRequired && ConfirmationText.ToUpper() != "CONFIRM")
            {
                StatusMessage = "Please type 'CONFIRM' to proceed";
                return;
            }

            // Check if location has content and permanent removal is requested
            if (PermanentlyRemove && SelectedLocationHasContent)
            {
                StatusMessage = "Cannot permanently remove location with content";
                return;
            }

            // Implementation would update/delete in database
            if (PermanentlyRemove)
            {
                // var result = await _databaseService.DeleteLocationAsync(SelectedLocationCode, RemovalReason).ConfigureAwait(false);
                AvailableLocations.Remove(AvailableLocations.First(loc => loc.LocationCode == SelectedLocationCode));
                StatusMessage = "Location permanently removed";
            }
            else
            {
                // var result = await _databaseService.DeactivateLocationAsync(SelectedLocationCode, RemovalReason).ConfigureAwait(false);
                var location = AvailableLocations.First(loc => loc.LocationCode == SelectedLocationCode);
                location.IsActive = false;
                StatusMessage = "Location deactivated";
            }

            Logger.LogInformation("Successfully processed removal for location {LocationCode}", SelectedLocationCode);

            // Reset form after successful removal
            await ResetFormAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error removing location: {ex.Message}";
            Logger.LogError(ex, "Error removing location {LocationCode}", SelectedLocationCode);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Determines if location can be removed.
    /// </summary>
    private bool CanRemoveLocation() => !string.IsNullOrEmpty(SelectedLocationCode) && 
                                       !string.IsNullOrWhiteSpace(RemovalReason) &&
                                       (!ConfirmationRequired || ConfirmationText.ToUpper() == "CONFIRM") &&
                                       !IsLoading &&
                                       !(PermanentlyRemove && SelectedLocationHasContent);

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        SelectedLocationCode = null;
        PermanentlyRemove = false;
        RemovalReason = string.Empty;
        ConfirmationText = string.Empty;
        ConfirmationRequired = true;
        ContentDetails = string.Empty;
        StatusMessage = "Form reset";

        OnPropertyChanged(nameof(ContentDetails));
        Logger.LogInformation("Location removal form reset");
        return Task.CompletedTask;
    }

    partial void OnSelectedLocationCodeChanged(string? value)
    {
        // Update command states
        CheckContentCommand.NotifyCanExecuteChanged();
        RemoveLocationCommand.NotifyCanExecuteChanged();
        
        // Reset related fields when selection changes
        if (string.IsNullOrEmpty(value))
        {
            PermanentlyRemove = false;
            RemovalReason = string.Empty;
            ConfirmationText = string.Empty;
            ContentDetails = string.Empty;
        }
        else
        {
            // Auto-check content when location is selected
            _ = CheckContentAsync();
        }

        OnPropertyChanged(nameof(SelectedLocation));
        OnPropertyChanged(nameof(SelectedLocationHasContent));
        OnPropertyChanged(nameof(ContentDetails));
    }

    partial void OnPermanentlyRemoveChanged(bool value)
    {
        // Update command state and validation
        RemoveLocationCommand.NotifyCanExecuteChanged();
        
        // Force deactivation if location has content
        if (value && SelectedLocationHasContent)
        {
            PermanentlyRemove = false;
            StatusMessage = "Cannot permanently remove location with content";
        }
    }

    partial void OnRemovalReasonChanged(string value)
    {
        // Update command state when reason changes
        RemoveLocationCommand.NotifyCanExecuteChanged();
    }

    partial void OnConfirmationTextChanged(string value)
    {
        // Update command state when confirmation changes
        RemoveLocationCommand.NotifyCanExecuteChanged();
    }
}
