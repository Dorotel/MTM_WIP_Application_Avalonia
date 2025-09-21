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
/// ViewModel for editing existing part numbers in the MTM system.
/// Provides part selection, modification, and update functionality.
/// </summary>
public partial class EditPartViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    [Required(ErrorMessage = "Part description is required")]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _category = "Standard";

    [ObservableProperty]
    private string _unitOfMeasure = "Each";

    [ObservableProperty]
    [Range(0, double.MaxValue, ErrorMessage = "Standard cost must be non-negative")]
    private decimal _standardCost = 0.00m;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private string? _selectedPartId;

    /// <summary>
    /// Available parts for editing.
    /// </summary>
    public ObservableCollection<PartInfo> AvailableParts { get; } = new();

    /// <summary>
    /// Available part categories.
    /// </summary>
    public ObservableCollection<string> AvailableCategories { get; } = new()
    {
        "Standard", "Raw Material", "Finished Good", "Subassembly", "Tool", "Consumable", "Service"
    };

    /// <summary>
    /// Available units of measure.
    /// </summary>
    public ObservableCollection<string> AvailableUnits { get; } = new()
    {
        "Each", "Pound", "Kilogram", "Meter", "Foot", "Inch", "Liter", "Gallon", "Box", "Case"
    };

    public EditPartViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<EditPartViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("EditPartViewModel initialized");
    }

    /// <summary>
    /// Loads available parts for editing.
    /// </summary>
    [RelayCommand]
    private Task LoadPartsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading parts...";

            using var scope = Logger.BeginScope("LoadParts");
            Logger.LogInformation("Loading parts for editing");

            // Implementation would load from database
            // var parts = await _databaseService.GetPartsAsync().ConfigureAwait(false);
            
            AvailableParts.Clear();
            // Add sample data for now
            AvailableParts.Add(new PartInfo { PartId = "STD-BOL-001", Description = "Standard Bolt 1/4 inch", Category = "Standard", UnitOfMeasure = "Each", StandardCost = 0.25m, IsActive = true });
            AvailableParts.Add(new PartInfo { PartId = "RAW-STE-002", Description = "Steel Rod 1 inch", Category = "Raw Material", UnitOfMeasure = "Foot", StandardCost = 2.50m, IsActive = true });
            AvailableParts.Add(new PartInfo { PartId = "FIN-WID-003", Description = "Widget Assembly", Category = "Finished Good", UnitOfMeasure = "Each", StandardCost = 45.00m, IsActive = false });

            StatusMessage = $"Loaded {AvailableParts.Count} parts";
            Logger.LogInformation("Successfully loaded {PartCount} parts", AvailableParts.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading parts: {ex.Message}";
            Logger.LogError(ex, "Error loading parts for editing");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads selected part details for editing.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLoadPartDetails))]
    private Task LoadPartDetailsAsync()
    {
        if (string.IsNullOrEmpty(SelectedPartId)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading part details...";

            using var scope = Logger.BeginScope("LoadPartDetails");
            Logger.LogInformation("Loading details for part {PartId}", SelectedPartId);

            var selectedPart = AvailableParts.FirstOrDefault(p => p.PartId == SelectedPartId);
            if (selectedPart != null)
            {
                Description = selectedPart.Description;
                Category = selectedPart.Category;
                UnitOfMeasure = selectedPart.UnitOfMeasure;
                StandardCost = selectedPart.StandardCost;
                IsActive = selectedPart.IsActive;
                Notes = selectedPart.Notes ?? string.Empty;

                StatusMessage = "Part details loaded";
                Logger.LogInformation("Successfully loaded details for part {PartId}", SelectedPartId);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading part details: {ex.Message}";
            Logger.LogError(ex, "Error loading part details for {PartId}", SelectedPartId);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if part details can be loaded.
    /// </summary>
    private bool CanLoadPartDetails() => !string.IsNullOrEmpty(SelectedPartId) && !IsLoading;

    /// <summary>
    /// Updates the selected part with modified information.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUpdatePart))]
    private Task UpdatePartAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Updating part...";

            using var scope = Logger.BeginScope("UpdatePart");
            Logger.LogInformation("Updating part {PartId}", SelectedPartId);

            // Validate input
            if (string.IsNullOrWhiteSpace(Description))
            {
                StatusMessage = "Please fill in all required fields";
                return Task.CompletedTask;
            }

            // Implementation would update in database
            // var result = await _databaseService.UpdatePartAsync(SelectedPartId, Description, Category, UnitOfMeasure, StandardCost, IsActive, Notes).ConfigureAwait(false);

            // Update local collection
            var part = AvailableParts.FirstOrDefault(p => p.PartId == SelectedPartId);
            if (part != null)
            {
                part.Description = Description;
                part.Category = Category;
                part.UnitOfMeasure = UnitOfMeasure;
                part.StandardCost = StandardCost;
                part.IsActive = IsActive;
                part.Notes = Notes;
            }

            StatusMessage = "Part updated successfully";
            Logger.LogInformation("Successfully updated part {PartId}", SelectedPartId);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating part: {ex.Message}";
            Logger.LogError(ex, "Error updating part {PartId}", SelectedPartId);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if part can be updated.
    /// </summary>
    private bool CanUpdatePart() => !string.IsNullOrEmpty(SelectedPartId) && 
                                   !string.IsNullOrWhiteSpace(Description) && 
                                   StandardCost >= 0 &&
                                   !IsLoading;

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        Description = string.Empty;
        Category = "Standard";
        UnitOfMeasure = "Each";
        StandardCost = 0.00m;
        IsActive = true;
        Notes = string.Empty;
        SelectedPartId = null;
        StatusMessage = "Form reset";

        Logger.LogInformation("Part edit form reset");
        return Task.CompletedTask;
    }

    partial void OnSelectedPartIdChanged(string? value)
    {
        // Update command states
        LoadPartDetailsCommand.NotifyCanExecuteChanged();
        UpdatePartCommand.NotifyCanExecuteChanged();
        
        // Clear form when selection changes
        if (string.IsNullOrEmpty(value))
        {
            Description = string.Empty;
            Category = "Standard";
            UnitOfMeasure = "Each";
            StandardCost = 0.00m;
            IsActive = true;
            Notes = string.Empty;
        }
    }

    partial void OnDescriptionChanged(string value)
    {
        // Update command state when description changes
        UpdatePartCommand.NotifyCanExecuteChanged();
    }

    partial void OnStandardCostChanged(decimal value)
    {
        // Update command state when cost changes
        UpdatePartCommand.NotifyCanExecuteChanged();
    }
}

/// <summary>
/// Part information model for display and editing.
/// </summary>
public class PartInfo
{
    public string PartId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal StandardCost { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
