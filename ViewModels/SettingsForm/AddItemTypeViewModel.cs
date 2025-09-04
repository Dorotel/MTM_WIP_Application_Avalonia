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
/// ViewModel for adding new item types to the MTM system.
/// Provides item type creation with validation and categorization.
/// </summary>
public partial class AddItemTypeViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    [Required(ErrorMessage = "Item type code is required")]
    [RegularExpression(@"^[A-Z0-9\-]{2,20}$", ErrorMessage = "Code must be 2-20 characters (A-Z, 0-9, -)")]
    private string _itemTypeCode = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Item type name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    private string _itemTypeName = string.Empty;

    [ObservableProperty]
    private string _category = "General";

    [ObservableProperty]
    private string _defaultUnitOfMeasure = "Each";

    [ObservableProperty]
    private bool _trackSerial;

    [ObservableProperty]
    private bool _trackLot;

    [ObservableProperty]
    private bool _trackExpiration;

    [ObservableProperty]
    private bool _requiresQualityInspection;

    [ObservableProperty]
    private bool _allowNegativeInventory;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Lead time must be non-negative")]
    private int _defaultLeadTimeDays = 5;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Reorder point must be non-negative")]
    private int _defaultReorderPoint = 10;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Reorder quantity must be non-negative")]
    private int _defaultReorderQuantity = 50;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _notes = string.Empty;

    /// <summary>
    /// Available item type categories.
    /// </summary>
    public ObservableCollection<string> AvailableCategories { get; } = new()
    {
        "General", "Raw Material", "Component", "Subassembly", "Finished Good", "Tool", "Consumable", "Service", "Packaging"
    };

    /// <summary>
    /// Available units of measure.
    /// </summary>
    public ObservableCollection<string> AvailableUnits { get; } = new()
    {
        "Each", "Pound", "Kilogram", "Meter", "Foot", "Inch", "Liter", "Gallon", "Box", "Case", "Sheet", "Roll"
    };

    /// <summary>
    /// Existing item type codes to prevent duplicates.
    /// </summary>
    public ObservableCollection<string> ExistingItemTypes { get; } = new();

    /// <summary>
    /// Gets whether the item type code already exists.
    /// </summary>
    public bool ItemTypeExists => !string.IsNullOrEmpty(ItemTypeCode) && 
                                 ExistingItemTypes.Contains(ItemTypeCode);

    public AddItemTypeViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<AddItemTypeViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("AddItemTypeViewModel initialized");
        
        // Load initial data
        _ = LoadExistingItemTypesAsync();
    }

    /// <summary>
    /// Loads existing item type codes.
    /// </summary>
    [RelayCommand]
    private Task LoadExistingItemTypesAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("LoadExistingItemTypes");
            Logger.LogInformation("Loading existing item type codes");

            // Implementation would load from database
            // var itemTypes = await _databaseService.GetItemTypeCodesAsync().ConfigureAwait(false);
            
            ExistingItemTypes.Clear();
            
            // Add sample data for now
            ExistingItemTypes.Add("RAW-MAT");
            ExistingItemTypes.Add("COMPONENT");
            ExistingItemTypes.Add("SUBASSY");
            ExistingItemTypes.Add("FINISHED");
            ExistingItemTypes.Add("TOOL");
            ExistingItemTypes.Add("CONSUMABLE");
            ExistingItemTypes.Add("SERVICE");

            Logger.LogInformation("Successfully loaded {ItemTypeCount} existing item types", ExistingItemTypes.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading existing item types");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Suggests an item type code based on the name.
    /// </summary>
    [RelayCommand]
    private Task SuggestItemTypeCodeAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("SuggestItemTypeCode");
            Logger.LogInformation("Suggesting item type code for name: {ItemTypeName}", ItemTypeName);

            if (string.IsNullOrWhiteSpace(ItemTypeName))
            {
                StatusMessage = "Please enter an item type name first";
                return Task.CompletedTask;
            }

            // Generate code from name
            var words = ItemTypeName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string suggestedCode;

            if (words.Length == 1)
            {
                // Single word: take up to 8 characters
                suggestedCode = words[0].ToUpper().Substring(0, Math.Min(8, words[0].Length));
            }
            else
            {
                // Multiple words: take first 3-4 letters of each word
                suggestedCode = string.Join("-", words.Take(3).Select(w => 
                    w.ToUpper().Substring(0, Math.Min(4, w.Length))));
            }

            // Ensure it doesn't exceed 20 characters
            if (suggestedCode.Length > 20)
            {
                suggestedCode = suggestedCode.Substring(0, 20);
            }

            // Check if it already exists and modify if needed
            string finalCode = suggestedCode;
            int counter = 1;
            while (ExistingItemTypes.Contains(finalCode) && counter < 100)
            {
                finalCode = $"{suggestedCode}-{counter:D2}";
                if (finalCode.Length > 20)
                {
                    var baseLength = 20 - 3; // Reserve space for -XX
                    finalCode = $"{suggestedCode.Substring(0, baseLength)}-{counter:D2}";
                }
                counter++;
            }

            ItemTypeCode = finalCode;
            StatusMessage = $"Suggested item type code: {finalCode}";
            
            Logger.LogInformation("Suggested item type code: {ItemTypeCode}", finalCode);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error suggesting item type code: {ex.Message}";
            Logger.LogError(ex, "Error suggesting item type code");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets default values based on selected category.
    /// </summary>
    [RelayCommand]
    private Task ApplyCategoryDefaultsAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("ApplyCategoryDefaults");
            Logger.LogInformation("Applying defaults for category: {Category}", Category);

            switch (Category)
            {
                case "Raw Material":
                    TrackLot = true;
                    RequiresQualityInspection = true;
                    DefaultLeadTimeDays = 14;
                    DefaultReorderPoint = 20;
                    DefaultReorderQuantity = 100;
                    DefaultUnitOfMeasure = "Pound";
                    break;

                case "Component":
                    TrackSerial = false;
                    TrackLot = true;
                    RequiresQualityInspection = true;
                    DefaultLeadTimeDays = 7;
                    DefaultReorderPoint = 50;
                    DefaultReorderQuantity = 200;
                    break;

                case "Finished Good":
                    TrackSerial = true;
                    RequiresQualityInspection = true;
                    DefaultLeadTimeDays = 3;
                    DefaultReorderPoint = 5;
                    DefaultReorderQuantity = 25;
                    break;

                case "Tool":
                    TrackSerial = true;
                    TrackLot = false;
                    RequiresQualityInspection = false;
                    AllowNegativeInventory = false;
                    DefaultLeadTimeDays = 21;
                    DefaultReorderPoint = 2;
                    DefaultReorderQuantity = 5;
                    break;

                case "Consumable":
                    TrackExpiration = true;
                    RequiresQualityInspection = false;
                    AllowNegativeInventory = true;
                    DefaultLeadTimeDays = 7;
                    DefaultReorderPoint = 10;
                    DefaultReorderQuantity = 50;
                    break;

                case "Service":
                    TrackSerial = false;
                    TrackLot = false;
                    TrackExpiration = false;
                    RequiresQualityInspection = false;
                    AllowNegativeInventory = true;
                    DefaultUnitOfMeasure = "Each";
                    DefaultLeadTimeDays = 1;
                    DefaultReorderPoint = 0;
                    DefaultReorderQuantity = 0;
                    break;

                default: // General
                    // Keep current values or set basic defaults
                    break;
            }

            StatusMessage = $"Applied defaults for {Category} category";
            Logger.LogInformation("Applied category defaults for {Category}", Category);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error applying category defaults: {ex.Message}";
            Logger.LogError(ex, "Error applying category defaults");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates a new item type with the specified details.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreateItemType))]
    private Task CreateItemTypeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Creating item type...";

            using var scope = Logger.BeginScope("CreateItemType");
            Logger.LogInformation("Creating item type {ItemTypeCode}", ItemTypeCode);

            // Validate input
            if (string.IsNullOrWhiteSpace(ItemTypeCode) || string.IsNullOrWhiteSpace(ItemTypeName))
            {
                StatusMessage = "Please fill in all required fields";
                return Task.CompletedTask;
            }

            if (ItemTypeExists)
            {
                StatusMessage = "Item type code already exists";
                return Task.CompletedTask;
            }

            // Validate item type code format
            if (!System.Text.RegularExpressions.Regex.IsMatch(ItemTypeCode, @"^[A-Z0-9\-]{2,20}$"))
            {
                StatusMessage = "Item type code format is invalid";
                return Task.CompletedTask;
            }

            // Implementation would save to database
            // var result = await _databaseService.CreateItemTypeAsync(
            //     ItemTypeCode, ItemTypeName, Category, DefaultUnitOfMeasure, Description,
            //     TrackSerial, TrackLot, TrackExpiration, RequiresQualityInspection, AllowNegativeInventory,
            //     DefaultLeadTimeDays, DefaultReorderPoint, DefaultReorderQuantity, IsActive, Notes).ConfigureAwait(false);

            // Add to existing item types
            ExistingItemTypes.Add(ItemTypeCode);

            StatusMessage = "Item type created successfully";
            Logger.LogInformation("Successfully created item type {ItemTypeCode}", ItemTypeCode);

            // Reset form after successful creation
            _ = ResetFormAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating item type: {ex.Message}";
            Logger.LogError(ex, "Error creating item type {ItemTypeCode}", ItemTypeCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if item type can be created.
    /// </summary>
    private bool CanCreateItemType() => !string.IsNullOrWhiteSpace(ItemTypeCode) && 
                                       !string.IsNullOrWhiteSpace(ItemTypeName) &&
                                       !ItemTypeExists &&
                                       DefaultLeadTimeDays >= 0 &&
                                       DefaultReorderPoint >= 0 &&
                                       DefaultReorderQuantity >= 0 &&
                                       !IsLoading;

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        ItemTypeCode = string.Empty;
        ItemTypeName = string.Empty;
        Category = "General";
        DefaultUnitOfMeasure = "Each";
        Description = string.Empty;
        TrackSerial = false;
        TrackLot = false;
        TrackExpiration = false;
        RequiresQualityInspection = false;
        AllowNegativeInventory = false;
        DefaultLeadTimeDays = 5;
        DefaultReorderPoint = 10;
        DefaultReorderQuantity = 50;
        IsActive = true;
        Notes = string.Empty;
        StatusMessage = "Form reset";

        Logger.LogInformation("Item type creation form reset");
        return Task.CompletedTask;
    }

    partial void OnItemTypeCodeChanged(string value)
    {
        // Update command state and validation
        CreateItemTypeCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(ItemTypeExists));
        
        if (ItemTypeExists)
        {
            StatusMessage = "Item type code already exists";
        }
        else if (!string.IsNullOrEmpty(value))
        {
            StatusMessage = "Ready";
        }
    }

    partial void OnItemTypeNameChanged(string value)
    {
        // Update command state when name changes
        CreateItemTypeCommand.NotifyCanExecuteChanged();
    }

    partial void OnCategoryChanged(string value)
    {
        // Auto-apply category defaults when category changes
        _ = ApplyCategoryDefaultsAsync();
    }

    partial void OnDefaultLeadTimeDaysChanged(int value)
    {
        // Update command state when lead time changes
        CreateItemTypeCommand.NotifyCanExecuteChanged();
    }

    partial void OnDefaultReorderPointChanged(int value)
    {
        // Update command state when reorder point changes
        CreateItemTypeCommand.NotifyCanExecuteChanged();
    }

    partial void OnDefaultReorderQuantityChanged(int value)
    {
        // Update command state when reorder quantity changes
        CreateItemTypeCommand.NotifyCanExecuteChanged();
    }
}
