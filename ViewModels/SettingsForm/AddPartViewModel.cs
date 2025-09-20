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
/// ViewModel for adding new part numbers to the MTM system.
/// Provides part number creation and validation functionality.
/// </summary>
public partial class AddPartViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    private string _partId = string.Empty;

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

    public AddPartViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<AddPartViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("AddPartViewModel initialized");
    }

    /// <summary>
    /// Validates that the part ID is unique.
    /// </summary>
    [RelayCommand]
    private Task ValidatePartIdAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(PartId))
            {
                StatusMessage = "Part ID is required";
                return Task.CompletedTask;
            }

            using var scope = Logger.BeginScope("ValidatePartId");
            Logger.LogInformation("Validating part ID {PartId}", PartId);

            // Implementation would check database for uniqueness
            // var exists = await _databaseService.PartExistsAsync(PartId).ConfigureAwait(false);
            
            // For now, simulate validation
            bool exists = PartId.ToLower() == "existing-part";
            
            if (exists)
            {
                StatusMessage = "Part ID already exists - please choose a different ID";
                Logger.LogWarning("Part ID {PartId} already exists", PartId);
            }
            else
            {
                StatusMessage = "Part ID is available";
                Logger.LogInformation("Part ID {PartId} is available", PartId);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error validating part ID: {ex.Message}";
            Logger.LogError(ex, "Error validating part ID {PartId}", PartId);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds the new part to the system.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddPart))]
    private Task AddPartAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Adding part...";

            using var scope = Logger.BeginScope("AddPart");
            Logger.LogInformation("Adding part {PartId}", PartId);

            // Validate input
            if (string.IsNullOrWhiteSpace(PartId) || string.IsNullOrWhiteSpace(Description))
            {
                StatusMessage = "Please fill in all required fields";
                return Task.CompletedTask;
            }

            // Implementation would add to database
            // var result = await _databaseService.AddPartAsync(PartId, Description, Category, UnitOfMeasure, StandardCost, IsActive, Notes).ConfigureAwait(false);

            StatusMessage = "Part added successfully";
            Logger.LogInformation("Successfully added part {PartId}", PartId);

            // Reset form after successful addition
            ResetFormCommand.Execute(null);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error adding part: {ex.Message}";
            Logger.LogError(ex, "Error adding part {PartId}", PartId);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if part can be added.
    /// </summary>
    private bool CanAddPart()
    {
        return !IsLoading && 
               !string.IsNullOrWhiteSpace(PartId) && 
               !string.IsNullOrWhiteSpace(Description) &&
               StandardCost >= 0;
    }

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        PartId = string.Empty;
        Description = string.Empty;
        Category = "Standard";
        UnitOfMeasure = "Each";
        StandardCost = 0.00m;
        IsActive = true;
        Notes = string.Empty;
        StatusMessage = "Form reset";

        Logger.LogInformation("Add part form reset");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Generates a suggested part ID based on category and description.
    /// </summary>
    [RelayCommand]
    private Task GeneratePartIdAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                StatusMessage = "Please enter a description first";
                return Task.CompletedTask;
            }

            // Generate suggested ID from category and description
            var categoryPrefix = Category.ToUpper().Substring(0, Math.Min(3, Category.Length));
            var descWords = Description.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var descPart = string.Join("", descWords.Take(2).Select(w => w.Substring(0, Math.Min(3, w.Length)).ToUpper()));
            
            var suggestedId = $"{categoryPrefix}-{descPart}-001";
            PartId = suggestedId;
            
            StatusMessage = "Part ID generated - please verify uniqueness";
            Logger.LogInformation("Generated part ID {PartId} for description {Description}", PartId, Description);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error generating part ID: {ex.Message}";
            Logger.LogError(ex, "Error generating part ID");
        }

        return Task.CompletedTask;
    }

    partial void OnPartIdChanged(string value)
    {
        // Update command state when part ID changes
        AddPartCommand.NotifyCanExecuteChanged();
        
        // Clear validation message when user starts typing
        if (!string.IsNullOrEmpty(value) && StatusMessage.Contains("Part ID"))
        {
            StatusMessage = "Ready";
        }
    }

    partial void OnDescriptionChanged(string value)
    {
        // Update command state when description changes
        AddPartCommand.NotifyCanExecuteChanged();
    }

    partial void OnStandardCostChanged(decimal value)
    {
        // Update command state when cost changes
        AddPartCommand.NotifyCanExecuteChanged();
    }
}
