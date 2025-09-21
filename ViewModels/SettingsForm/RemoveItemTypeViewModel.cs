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
/// ViewModel for removing item types from the MTM system.
/// Provides item type selection, deactivation, and removal functionality with safety measures.
/// </summary>
public partial class RemoveItemTypeViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    private string? _selectedItemTypeCode;

    [ObservableProperty]
    private bool _permanentlyRemove;

    [ObservableProperty]
    [Required(ErrorMessage = "Reason is required for item type removal")]
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
    /// Available item types for removal.
    /// </summary>
    public ObservableCollection<ItemTypeInfo> AvailableItemTypes { get; } = new();

    /// <summary>
    /// Item types that have parts or transactions (cannot be deleted).
    /// </summary>
    public ObservableCollection<string> ItemTypesWithUsage { get; } = new();

    /// <summary>
    /// Gets selected item type details.
    /// </summary>
    public ItemTypeInfo? SelectedItemType => AvailableItemTypes.FirstOrDefault(it => it.ItemTypeCode == SelectedItemTypeCode);

    /// <summary>
    /// Gets whether selected item type has usage.
    /// </summary>
    public bool SelectedItemTypeHasUsage => !string.IsNullOrEmpty(SelectedItemTypeCode) && 
                                           ItemTypesWithUsage.Contains(SelectedItemTypeCode);

    /// <summary>
    /// Gets usage details for the selected item type.
    /// </summary>
    public string UsageDetails { get; private set; } = string.Empty;

    public RemoveItemTypeViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<RemoveItemTypeViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("RemoveItemTypeViewModel initialized");
    }

    /// <summary>
    /// Loads available item types for removal.
    /// </summary>
    [RelayCommand]
    private Task LoadItemTypesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading item types...";

            using var scope = Logger.BeginScope("LoadItemTypes");
            Logger.LogInformation("Loading item types for removal");

            // Implementation would load from database
            // var itemTypes = await _databaseService.GetItemTypesAsync().ConfigureAwait(false);
            // var usageData = await _databaseService.GetItemTypesWithUsageAsync().ConfigureAwait(false);
            
            AvailableItemTypes.Clear();
            ItemTypesWithUsage.Clear();

            // Add sample data for now
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "RAW-MAT", 
                ItemTypeName = "Raw Material", 
                Category = "Raw Material",
                IsActive = true 
            });
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "COMPONENT", 
                ItemTypeName = "Component Part", 
                Category = "Component",
                IsActive = true 
            });
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "FINISHED", 
                ItemTypeName = "Finished Good", 
                Category = "Finished Good",
                IsActive = true 
            });
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "TOOL", 
                ItemTypeName = "Tooling", 
                Category = "Tool",
                IsActive = false 
            });
            AvailableItemTypes.Add(new ItemTypeInfo 
            { 
                ItemTypeCode = "OBSOLETE-TYPE", 
                ItemTypeName = "Obsolete Item Type", 
                Category = "General",
                IsActive = false 
            });

            // Item types with usage (cannot be permanently deleted)
            ItemTypesWithUsage.Add("RAW-MAT");
            ItemTypesWithUsage.Add("COMPONENT");
            ItemTypesWithUsage.Add("FINISHED");

            StatusMessage = $"Loaded {AvailableItemTypes.Count} item types";
            Logger.LogInformation("Successfully loaded {ItemTypeCount} item types for removal", AvailableItemTypes.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading item types: {ex.Message}";
            Logger.LogError(ex, "Error loading item types for removal");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if an item type has parts or transactions.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCheckUsage))]
    private Task CheckUsageAsync()
    {
        if (string.IsNullOrEmpty(SelectedItemTypeCode)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Checking item type usage...";

            using var scope = Logger.BeginScope("CheckUsage");
            Logger.LogInformation("Checking usage for item type {ItemTypeCode}", SelectedItemTypeCode);

            // Implementation would check database for usage
            // var usageResult = await _databaseService.GetItemTypeUsageAsync(SelectedItemTypeCode).ConfigureAwait(false);

            bool hasUsage = ItemTypesWithUsage.Contains(SelectedItemTypeCode);
            
            if (hasUsage)
            {
                // Sample usage details
                UsageDetails = SelectedItemTypeCode switch
                {
                    "RAW-MAT" => "Used by 25 parts, 1,245 inventory transactions",
                    "COMPONENT" => "Used by 87 parts, 3,567 inventory transactions",
                    "FINISHED" => "Used by 12 parts, 856 inventory transactions",
                    _ => "Active usage detected"
                };
                
                StatusMessage = "Item type has active usage - can only be deactivated";
                PermanentlyRemove = false; // Force deactivation only
            }
            else
            {
                UsageDetails = "No parts or transactions found";
                StatusMessage = "Item type has no active usage - can be permanently removed";
            }

            OnPropertyChanged(nameof(UsageDetails));
            Logger.LogInformation("Item type {ItemTypeCode} usage check: {HasUsage}", SelectedItemTypeCode, hasUsage);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error checking usage: {ex.Message}";
            Logger.LogError(ex, "Error checking usage for item type {ItemTypeCode}", SelectedItemTypeCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if usage can be checked.
    /// </summary>
    private bool CanCheckUsage() => !string.IsNullOrEmpty(SelectedItemTypeCode) && !IsLoading;

    /// <summary>
    /// Removes or deactivates the selected item type.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRemoveItemType))]
    private Task RemoveItemTypeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = PermanentlyRemove ? "Permanently removing item type..." : "Deactivating item type...";

            using var scope = Logger.BeginScope("RemoveItemType");
            Logger.LogInformation("Removing item type {ItemTypeCode} (Permanent: {Permanent})", SelectedItemTypeCode, PermanentlyRemove);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(RemovalReason))
            {
                StatusMessage = "Please provide a reason for removal";
                return Task.CompletedTask;
            }

            if (ConfirmationRequired && ConfirmationText.ToUpper() != "CONFIRM")
            {
                StatusMessage = "Please type 'CONFIRM' to proceed";
                return Task.CompletedTask;
            }

            // Check if item type has usage and permanent removal is requested
            if (PermanentlyRemove && SelectedItemTypeHasUsage)
            {
                StatusMessage = "Cannot permanently remove item type with active usage";
                return Task.CompletedTask;
            }

            // Implementation would update/delete in database
            if (PermanentlyRemove)
            {
                // var result = await _databaseService.DeleteItemTypeAsync(SelectedItemTypeCode, RemovalReason).ConfigureAwait(false);
                AvailableItemTypes.Remove(AvailableItemTypes.First(it => it.ItemTypeCode == SelectedItemTypeCode));
                StatusMessage = "Item type permanently removed";
            }
            else
            {
                // var result = await _databaseService.DeactivateItemTypeAsync(SelectedItemTypeCode, RemovalReason).ConfigureAwait(false);
                var itemType = AvailableItemTypes.First(it => it.ItemTypeCode == SelectedItemTypeCode);
                itemType.IsActive = false;
                StatusMessage = "Item type deactivated";
            }

            Logger.LogInformation("Successfully processed removal for item type {ItemTypeCode}", SelectedItemTypeCode);

            // Reset form after successful removal
            _ = ResetFormAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error removing item type: {ex.Message}";
            Logger.LogError(ex, "Error removing item type {ItemTypeCode}", SelectedItemTypeCode);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if item type can be removed.
    /// </summary>
    private bool CanRemoveItemType() => !string.IsNullOrEmpty(SelectedItemTypeCode) && 
                                       !string.IsNullOrWhiteSpace(RemovalReason) &&
                                       (!ConfirmationRequired || ConfirmationText.ToUpper() == "CONFIRM") &&
                                       !IsLoading &&
                                       !(PermanentlyRemove && SelectedItemTypeHasUsage);

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        SelectedItemTypeCode = null;
        PermanentlyRemove = false;
        RemovalReason = string.Empty;
        ConfirmationText = string.Empty;
        ConfirmationRequired = true;
        UsageDetails = string.Empty;
        StatusMessage = "Form reset";

        OnPropertyChanged(nameof(UsageDetails));
        Logger.LogInformation("Item type removal form reset");
        return Task.CompletedTask;
    }

    partial void OnSelectedItemTypeCodeChanged(string? value)
    {
        // Update command states
        CheckUsageCommand.NotifyCanExecuteChanged();
        RemoveItemTypeCommand.NotifyCanExecuteChanged();
        
        // Reset related fields when selection changes
        if (string.IsNullOrEmpty(value))
        {
            PermanentlyRemove = false;
            RemovalReason = string.Empty;
            ConfirmationText = string.Empty;
            UsageDetails = string.Empty;
        }
        else
        {
            // Auto-check usage when item type is selected
            _ = CheckUsageAsync();
        }

        OnPropertyChanged(nameof(SelectedItemType));
        OnPropertyChanged(nameof(SelectedItemTypeHasUsage));
        OnPropertyChanged(nameof(UsageDetails));
    }

    partial void OnPermanentlyRemoveChanged(bool value)
    {
        // Update command state and validation
        RemoveItemTypeCommand.NotifyCanExecuteChanged();
        
        // Force deactivation if item type has usage
        if (value && SelectedItemTypeHasUsage)
        {
            PermanentlyRemove = false;
            StatusMessage = "Cannot permanently remove item type with active usage";
        }
    }

    partial void OnRemovalReasonChanged(string value)
    {
        // Update command state when reason changes
        RemoveItemTypeCommand.NotifyCanExecuteChanged();
    }

    partial void OnConfirmationTextChanged(string value)
    {
        // Update command state when confirmation changes
        RemoveItemTypeCommand.NotifyCanExecuteChanged();
    }
}
