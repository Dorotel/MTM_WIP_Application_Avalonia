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
/// ViewModel for editing existing item types in the MTM system.
/// Provides item type selection, modification, and update functionality.
/// </summary>
public partial class EditItemTypeViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

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

    [ObservableProperty]
    private string? _selectedItemTypeCode;

    /// <summary>
    /// Available item types for editing.
    /// </summary>
    public ObservableCollection<ItemTypeInfo> AvailableItemTypes { get; } = new();

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

    public EditItemTypeViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<EditItemTypeViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("EditItemTypeViewModel initialized");
    }

    /// <summary>
    /// Loads available item types for editing.
    /// </summary>
    [RelayCommand]
    private Task LoadItemTypesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading item types...";

            using var scope = Logger.BeginScope("LoadItemTypes");
            Logger.LogInformation("Loading item types for editing");

            // Implementation would load from database
            // var itemTypes = await _databaseService.GetItemTypesAsync().ConfigureAwait(false);
            
            AvailableItemTypes.Clear();
            
            // Add sample data for now
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "RAW-MAT", 
                ItemTypeName = "Raw Material", 
                Category = "Raw Material",
                DefaultUnitOfMeasure = "Pound",
                TrackLot = true,
                RequiresQualityInspection = true,
                DefaultLeadTimeDays = 14,
                DefaultReorderPoint = 20,
                DefaultReorderQuantity = 100,
                IsActive = true 
            });
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "COMPONENT", 
                ItemTypeName = "Component Part", 
                Category = "Component",
                DefaultUnitOfMeasure = "Each",
                TrackLot = true,
                RequiresQualityInspection = true,
                DefaultLeadTimeDays = 7,
                DefaultReorderPoint = 50,
                DefaultReorderQuantity = 200,
                IsActive = true 
            });
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "FINISHED", 
                ItemTypeName = "Finished Good", 
                Category = "Finished Good",
                DefaultUnitOfMeasure = "Each",
                TrackSerial = true,
                RequiresQualityInspection = true,
                DefaultLeadTimeDays = 3,
                DefaultReorderPoint = 5,
                DefaultReorderQuantity = 25,
                IsActive = true 
            });
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "TOOL", 
                ItemTypeName = "Tooling", 
                Category = "Tool",
                DefaultUnitOfMeasure = "Each",
                TrackSerial = true,
                DefaultLeadTimeDays = 21,
                DefaultReorderPoint = 2,
                DefaultReorderQuantity = 5,
                IsActive = false 
            });

            StatusMessage = $"Loaded {AvailableItemTypes.Count} item types";
            Logger.LogInformation("Successfully loaded {ItemTypeCount} item types", AvailableItemTypes.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading item types: {ex.Message}";
            Logger.LogError(ex, "Error loading item types for editing");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads selected item type details for editing.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLoadItemTypeDetails))]
    private Task LoadItemTypeDetailsAsync()
    {
        if (string.IsNullOrEmpty(SelectedItemTypeCode)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading item type details...";

            using var scope = Logger.BeginScope("LoadItemTypeDetails");
            Logger.LogInformation("Loading details for item type {ItemTypeCode}", SelectedItemTypeCode);

            var selectedItemType = AvailableItemTypes.FirstOrDefault(it => it.ItemTypeCode == SelectedItemTypeCode);
            if (selectedItemType != null)
            {
                ItemTypeName = selectedItemType.ItemTypeName;
                Category = selectedItemType.Category;
                DefaultUnitOfMeasure = selectedItemType.DefaultUnitOfMeasure;
                Description = selectedItemType.Description ?? string.Empty;
                TrackSerial = selectedItemType.TrackSerial;
                TrackLot = selectedItemType.TrackLot;
                TrackExpiration = selectedItemType.TrackExpiration;
                RequiresQualityInspection = selectedItemType.RequiresQualityInspection;
                AllowNegativeInventory = selectedItemType.AllowNegativeInventory;
                DefaultLeadTimeDays = selectedItemType.DefaultLeadTimeDays;
                DefaultReorderPoint = selectedItemType.DefaultReorderPoint;
                DefaultReorderQuantity = selectedItemType.DefaultReorderQuantity;
                IsActive = selectedItemType.IsActive;
                Notes = selectedItemType.Notes ?? string.Empty;

                StatusMessage = "Item type details loaded";
                Logger.LogInformation("Successfully loaded details for item type {ItemTypeCode}", SelectedItemTypeCode);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading item type details: {ex.Message}";
            Logger.LogError(ex, "Error loading item type details for {ItemTypeCode}", SelectedItemTypeCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if item type details can be loaded.
    /// </summary>
    private bool CanLoadItemTypeDetails() => !string.IsNullOrEmpty(SelectedItemTypeCode) && !IsLoading;

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
                    if (DefaultUnitOfMeasure == "Each") DefaultUnitOfMeasure = "Pound";
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
    /// Updates the selected item type with modified information.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUpdateItemType))]
    private Task UpdateItemTypeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Updating item type...";

            using var scope = Logger.BeginScope("UpdateItemType");
            Logger.LogInformation("Updating item type {ItemTypeCode}", SelectedItemTypeCode);

            // Validate input
            if (string.IsNullOrWhiteSpace(ItemTypeName))
            {
                StatusMessage = "Please fill in all required fields";
                return Task.CompletedTask;
            }

            // Implementation would update in database
            // var result = await _databaseService.UpdateItemTypeAsync(SelectedItemTypeCode, ItemTypeName, Category, 
            //     DefaultUnitOfMeasure, Description, TrackSerial, TrackLot, TrackExpiration, RequiresQualityInspection, 
            //     AllowNegativeInventory, DefaultLeadTimeDays, DefaultReorderPoint, DefaultReorderQuantity, IsActive, Notes).ConfigureAwait(false);

            // Update local collection
            var itemType = AvailableItemTypes.FirstOrDefault(it => it.ItemTypeCode == SelectedItemTypeCode);
            if (itemType != null)
            {
                itemType.ItemTypeName = ItemTypeName;
                itemType.Category = Category;
                itemType.DefaultUnitOfMeasure = DefaultUnitOfMeasure;
                itemType.Description = Description;
                itemType.TrackSerial = TrackSerial;
                itemType.TrackLot = TrackLot;
                itemType.TrackExpiration = TrackExpiration;
                itemType.RequiresQualityInspection = RequiresQualityInspection;
                itemType.AllowNegativeInventory = AllowNegativeInventory;
                itemType.DefaultLeadTimeDays = DefaultLeadTimeDays;
                itemType.DefaultReorderPoint = DefaultReorderPoint;
                itemType.DefaultReorderQuantity = DefaultReorderQuantity;
                itemType.IsActive = IsActive;
                itemType.Notes = Notes;
            }

            StatusMessage = "Item type updated successfully";
            Logger.LogInformation("Successfully updated item type {ItemTypeCode}", SelectedItemTypeCode);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating item type: {ex.Message}";
            Logger.LogError(ex, "Error updating item type {ItemTypeCode}", SelectedItemTypeCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if item type can be updated.
    /// </summary>
    private bool CanUpdateItemType() => !string.IsNullOrEmpty(SelectedItemTypeCode) && 
                                       !string.IsNullOrWhiteSpace(ItemTypeName) && 
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
        SelectedItemTypeCode = null;
        StatusMessage = "Form reset";

        Logger.LogInformation("Item type edit form reset");
        return Task.CompletedTask;
    }

    partial void OnSelectedItemTypeCodeChanged(string? value)
    {
        // Update command states
        LoadItemTypeDetailsCommand.NotifyCanExecuteChanged();
        UpdateItemTypeCommand.NotifyCanExecuteChanged();
        
        // Clear form when selection changes
        if (string.IsNullOrEmpty(value))
        {
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
        }
    }

    partial void OnItemTypeNameChanged(string value)
    {
        // Update command state when name changes
        UpdateItemTypeCommand.NotifyCanExecuteChanged();
    }

    partial void OnDefaultLeadTimeDaysChanged(int value)
    {
        // Update command state when lead time changes
        UpdateItemTypeCommand.NotifyCanExecuteChanged();
    }

    partial void OnDefaultReorderPointChanged(int value)
    {
        // Update command state when reorder point changes
        UpdateItemTypeCommand.NotifyCanExecuteChanged();
    }

    partial void OnDefaultReorderQuantityChanged(int value)
    {
        // Update command state when reorder quantity changes
        UpdateItemTypeCommand.NotifyCanExecuteChanged();
    }
}

/// <summary>
/// Item type information model for display and editing.
/// </summary>
public class ItemTypeInfo
{
    public string ItemTypeCode { get; set; } = string.Empty;
    public string ItemTypeName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string DefaultUnitOfMeasure { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool TrackSerial { get; set; }
    public bool TrackLot { get; set; }
    public bool TrackExpiration { get; set; }
    public bool RequiresQualityInspection { get; set; }
    public bool AllowNegativeInventory { get; set; }
    public int DefaultLeadTimeDays { get; set; }
    public int DefaultReorderPoint { get; set; }
    public int DefaultReorderQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
