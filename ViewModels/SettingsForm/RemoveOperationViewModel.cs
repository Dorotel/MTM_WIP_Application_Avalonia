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
/// ViewModel for removing operations from the MTM system.
/// Provides operation selection, deactivation, and removal functionality with safety measures.
/// </summary>
public partial class RemoveOperationViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    private string? _selectedOperationNumber;

    [ObservableProperty]
    private bool _permanentlyRemove;

    [ObservableProperty]
    [Required(ErrorMessage = "Reason is required for operation removal")]
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
    /// Available operations for removal.
    /// </summary>
    public ObservableCollection<OperationInfo> AvailableOperations { get; } = new();

    /// <summary>
    /// Operations that have active routes or transactions (cannot be deleted).
    /// </summary>
    public ObservableCollection<string> OperationsWithUsage { get; } = new();

    /// <summary>
    /// Gets selected operation details.
    /// </summary>
    public OperationInfo? SelectedOperation => AvailableOperations.FirstOrDefault(op => op.OperationNumber == SelectedOperationNumber);

    /// <summary>
    /// Gets whether selected operation has active usage.
    /// </summary>
    public bool SelectedOperationHasUsage => !string.IsNullOrEmpty(SelectedOperationNumber) && 
                                            OperationsWithUsage.Contains(SelectedOperationNumber);

    /// <summary>
    /// Gets usage details for the selected operation.
    /// </summary>
    public string UsageDetails { get; private set; } = string.Empty;

    public RemoveOperationViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<RemoveOperationViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("RemoveOperationViewModel initialized");
    }

    /// <summary>
    /// Loads available operations for removal.
    /// </summary>
    [RelayCommand]
    private Task LoadOperationsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading operations...";

            using var scope = Logger.BeginScope("LoadOperations");
            Logger.LogInformation("Loading operations for removal");

            // Implementation would load from database
            // var operations = await _databaseService.GetOperationsAsync().ConfigureAwait(false);
            // var usageData = await _databaseService.GetOperationsWithUsageAsync().ConfigureAwait(false);
            
            AvailableOperations.Clear();
            OperationsWithUsage.Clear();

            // Add sample data for now
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "10", 
                Description = "Material Prep", 
                Department = "Production", 
                IsActive = true 
            });
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "90", 
                Description = "CNC Machining", 
                Department = "Machining", 
                IsActive = true 
            });
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "100", 
                Description = "Assembly", 
                Department = "Assembly", 
                IsActive = true 
            });
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "110", 
                Description = "Quality Inspection", 
                Department = "Quality", 
                IsActive = true 
            });
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "120", 
                Description = "Obsolete Operation", 
                Department = "Production", 
                IsActive = false 
            });

            // Operations with active usage (cannot be permanently deleted)
            OperationsWithUsage.Add("10");
            OperationsWithUsage.Add("90");
            OperationsWithUsage.Add("100");

            StatusMessage = $"Loaded {AvailableOperations.Count} operations";
            Logger.LogInformation("Successfully loaded {OperationCount} operations for removal", AvailableOperations.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading operations: {ex.Message}";
            Logger.LogError(ex, "Error loading operations for removal");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if an operation has active usage (routes, transactions, etc.).
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCheckUsage))]
    private Task CheckUsageAsync()
    {
        if (string.IsNullOrEmpty(SelectedOperationNumber)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Checking operation usage...";

            using var scope = Logger.BeginScope("CheckUsage");
            Logger.LogInformation("Checking usage for operation {OperationNumber}", SelectedOperationNumber);

            // Implementation would check database for usage
            // var usageResult = await _databaseService.GetOperationUsageAsync(SelectedOperationNumber).ConfigureAwait(false);

            bool hasUsage = OperationsWithUsage.Contains(SelectedOperationNumber);
            
            if (hasUsage)
            {
                // Sample usage details
                UsageDetails = SelectedOperationNumber switch
                {
                    "10" => "Used in 5 active routes, 12 work orders in progress",
                    "90" => "Used in 3 active routes, 8 work orders in progress, 156 historical transactions",
                    "100" => "Used in 7 active routes, 15 work orders in progress, 342 historical transactions",
                    _ => "Active usage detected"
                };
                
                StatusMessage = "Operation has active usage - can only be deactivated";
                PermanentlyRemove = false; // Force deactivation only
            }
            else
            {
                UsageDetails = "No active usage found";
                StatusMessage = "Operation has no active usage - can be permanently removed";
            }

            OnPropertyChanged(nameof(UsageDetails));
            Logger.LogInformation("Operation {OperationNumber} usage check: {HasUsage}", SelectedOperationNumber, hasUsage);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error checking usage: {ex.Message}";
            Logger.LogError(ex, "Error checking usage for operation {OperationNumber}", SelectedOperationNumber);
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
    private bool CanCheckUsage() => !string.IsNullOrEmpty(SelectedOperationNumber) && !IsLoading;

    /// <summary>
    /// Removes or deactivates the selected operation.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRemoveOperation))]
    private async Task RemoveOperationAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = PermanentlyRemove ? "Permanently removing operation..." : "Deactivating operation...";

            using var scope = Logger.BeginScope("RemoveOperation");
            Logger.LogInformation("Removing operation {OperationNumber} (Permanent: {Permanent})", SelectedOperationNumber, PermanentlyRemove);

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

            // Check if operation has usage and permanent removal is requested
            if (PermanentlyRemove && SelectedOperationHasUsage)
            {
                StatusMessage = "Cannot permanently remove operation with active usage";
                return;
            }

            // Implementation would update/delete in database
            if (PermanentlyRemove)
            {
                // var result = await _databaseService.DeleteOperationAsync(SelectedOperationNumber, RemovalReason).ConfigureAwait(false);
                AvailableOperations.Remove(AvailableOperations.First(op => op.OperationNumber == SelectedOperationNumber));
                StatusMessage = "Operation permanently removed";
            }
            else
            {
                // var result = await _databaseService.DeactivateOperationAsync(SelectedOperationNumber, RemovalReason).ConfigureAwait(false);
                var operation = AvailableOperations.First(op => op.OperationNumber == SelectedOperationNumber);
                operation.IsActive = false;
                StatusMessage = "Operation deactivated";
            }

            Logger.LogInformation("Successfully processed removal for operation {OperationNumber}", SelectedOperationNumber);

            // Reset form after successful removal
            await ResetFormAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error removing operation: {ex.Message}";
            Logger.LogError(ex, "Error removing operation {OperationNumber}", SelectedOperationNumber);
        }
        finally
        {
            IsLoading = false;
        }

        return;
    }

    /// <summary>
    /// Determines if operation can be removed.
    /// </summary>
    private bool CanRemoveOperation() => !string.IsNullOrEmpty(SelectedOperationNumber) && 
                                        !string.IsNullOrWhiteSpace(RemovalReason) &&
                                        (!ConfirmationRequired || ConfirmationText.ToUpper() == "CONFIRM") &&
                                        !IsLoading &&
                                        !(PermanentlyRemove && SelectedOperationHasUsage);

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        SelectedOperationNumber = null;
        PermanentlyRemove = false;
        RemovalReason = string.Empty;
        ConfirmationText = string.Empty;
        ConfirmationRequired = true;
        UsageDetails = string.Empty;
        StatusMessage = "Form reset";

        OnPropertyChanged(nameof(UsageDetails));
        Logger.LogInformation("Operation removal form reset");
        return Task.CompletedTask;
    }

    partial void OnSelectedOperationNumberChanged(string? value)
    {
        // Update command states
        CheckUsageCommand.NotifyCanExecuteChanged();
        RemoveOperationCommand.NotifyCanExecuteChanged();
        
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
            // Auto-check usage when operation is selected
            _ = CheckUsageAsync();
        }

        OnPropertyChanged(nameof(SelectedOperation));
        OnPropertyChanged(nameof(SelectedOperationHasUsage));
        OnPropertyChanged(nameof(UsageDetails));
    }

    partial void OnPermanentlyRemoveChanged(bool value)
    {
        // Update command state and validation
        RemoveOperationCommand.NotifyCanExecuteChanged();
        
        // Force deactivation if operation has usage
        if (value && SelectedOperationHasUsage)
        {
            PermanentlyRemove = false;
            StatusMessage = "Cannot permanently remove operation with active usage";
        }
    }

    partial void OnRemovalReasonChanged(string value)
    {
        // Update command state when reason changes
        RemoveOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnConfirmationTextChanged(string value)
    {
        // Update command state when confirmation changes
        RemoveOperationCommand.NotifyCanExecuteChanged();
    }
}
