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
/// ViewModel for removing parts from the MTM system.
/// Provides part selection, deactivation, and removal functionality with safety measures.
/// </summary>
public partial class RemovePartViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    private string? _selectedPartId;

    [ObservableProperty]
    private bool _permanentlyRemove;

    [ObservableProperty]
    [Required(ErrorMessage = "Reason is required for part removal")]
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
    /// Available parts for removal.
    /// </summary>
    public ObservableCollection<PartInfo> AvailableParts { get; } = new();

    /// <summary>
    /// Parts that have inventory transactions (cannot be deleted).
    /// </summary>
    public ObservableCollection<string> PartsWithTransactions { get; } = new();

    /// <summary>
    /// Gets selected part details.
    /// </summary>
    public PartInfo? SelectedPart => AvailableParts.FirstOrDefault(p => p.PartId == SelectedPartId);

    /// <summary>
    /// Gets whether selected part has transactions.
    /// </summary>
    public bool SelectedPartHasTransactions => !string.IsNullOrEmpty(SelectedPartId) && 
                                              PartsWithTransactions.Contains(SelectedPartId);

    public RemovePartViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<RemovePartViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("RemovePartViewModel initialized");
    }

    /// <summary>
    /// Loads available parts for removal.
    /// </summary>
    [RelayCommand]
    private Task LoadPartsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading parts...";

            using var scope = Logger.BeginScope("LoadParts");
            Logger.LogInformation("Loading parts for removal");

            // Implementation would load from database
            // var parts = await _databaseService.GetPartsAsync().ConfigureAwait(false);
            // var transactionParts = await _databaseService.GetPartsWithTransactionsAsync().ConfigureAwait(false);
            
            AvailableParts.Clear();
            PartsWithTransactions.Clear();

            // Add sample data for now
            AvailableParts.Add(new PartInfo { PartId = "STD-BOL-001", Description = "Standard Bolt 1/4 inch", Category = "Standard", IsActive = true });
            AvailableParts.Add(new PartInfo { PartId = "RAW-STE-002", Description = "Steel Rod 1 inch", Category = "Raw Material", IsActive = true });
            AvailableParts.Add(new PartInfo { PartId = "FIN-WID-003", Description = "Widget Assembly", Category = "Finished Good", IsActive = false });
            AvailableParts.Add(new PartInfo { PartId = "OLD-PAR-004", Description = "Obsolete Part", Category = "Standard", IsActive = false });

            // Parts with transactions (cannot be permanently deleted)
            PartsWithTransactions.Add("STD-BOL-001");
            PartsWithTransactions.Add("RAW-STE-002");

            StatusMessage = $"Loaded {AvailableParts.Count} parts";
            Logger.LogInformation("Successfully loaded {PartCount} parts for removal", AvailableParts.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading parts: {ex.Message}";
            Logger.LogError(ex, "Error loading parts for removal");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a part has inventory transactions.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCheckTransactions))]
    private Task CheckTransactionsAsync()
    {
        if (string.IsNullOrEmpty(SelectedPartId)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Checking part transactions...";

            using var scope = Logger.BeginScope("CheckTransactions");
            Logger.LogInformation("Checking transactions for part {PartId}", SelectedPartId);

            // Implementation would check database for transactions
            // var hasTransactions = await _databaseService.HasTransactionsAsync(SelectedPartId).ConfigureAwait(false);

            bool hasTransactions = PartsWithTransactions.Contains(SelectedPartId);

            if (hasTransactions)
            {
                StatusMessage = "Part has inventory transactions - can only be deactivated";
                PermanentlyRemove = false; // Force deactivation only
            }
            else
            {
                StatusMessage = "Part has no transactions - can be permanently removed";
            }

            Logger.LogInformation("Part {PartId} transaction check: {HasTransactions}", SelectedPartId, hasTransactions);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error checking transactions: {ex.Message}";
            Logger.LogError(ex, "Error checking transactions for part {PartId}", SelectedPartId);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if transactions can be checked.
    /// </summary>
    private bool CanCheckTransactions() => !string.IsNullOrEmpty(SelectedPartId) && !IsLoading;

    /// <summary>
    /// Removes or deactivates the selected part.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRemovePart))]
    private async Task RemovePartAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = PermanentlyRemove ? "Permanently removing part..." : "Deactivating part...";

            using var scope = Logger.BeginScope("RemovePart");
            Logger.LogInformation("Removing part {PartId} (Permanent: {Permanent})", SelectedPartId, PermanentlyRemove);

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

            // Check if part has transactions and permanent removal is requested
            if (PermanentlyRemove && SelectedPartHasTransactions)
            {
                StatusMessage = "Cannot permanently remove part with transactions";
                return;
            }

            // Implementation would update/delete in database
            if (PermanentlyRemove)
            {
                // var result = await _databaseService.DeletePartAsync(SelectedPartId, RemovalReason).ConfigureAwait(false);
                AvailableParts.Remove(AvailableParts.First(p => p.PartId == SelectedPartId));
                StatusMessage = "Part permanently removed";
            }
            else
            {
                // var result = await _databaseService.DeactivatePartAsync(SelectedPartId, RemovalReason).ConfigureAwait(false);
                var part = AvailableParts.First(p => p.PartId == SelectedPartId);
                part.IsActive = false;
                StatusMessage = "Part deactivated";
            }

            Logger.LogInformation("Successfully processed removal for part {PartId}", SelectedPartId);

            // Reset form after successful removal
            await ResetFormAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error removing part: {ex.Message}";
            Logger.LogError(ex, "Error removing part {PartId}", SelectedPartId);
        }
        finally
        {
            IsLoading = false;
        }

        return;
    }

    /// <summary>
    /// Determines if part can be removed.
    /// </summary>
    private bool CanRemovePart() => !string.IsNullOrEmpty(SelectedPartId) && 
                                   !string.IsNullOrWhiteSpace(RemovalReason) &&
                                   (!ConfirmationRequired || ConfirmationText.ToUpper() == "CONFIRM") &&
                                   !IsLoading &&
                                   !(PermanentlyRemove && SelectedPartHasTransactions);

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        SelectedPartId = null;
        PermanentlyRemove = false;
        RemovalReason = string.Empty;
        ConfirmationText = string.Empty;
        ConfirmationRequired = true;
        StatusMessage = "Form reset";

        Logger.LogInformation("Part removal form reset");
        return Task.CompletedTask;
    }

    partial void OnSelectedPartIdChanged(string? value)
    {
        // Update command states
        CheckTransactionsCommand.NotifyCanExecuteChanged();
        RemovePartCommand.NotifyCanExecuteChanged();
        
        // Reset related fields when selection changes
        if (string.IsNullOrEmpty(value))
        {
            PermanentlyRemove = false;
            RemovalReason = string.Empty;
            ConfirmationText = string.Empty;
        }
        else
        {
            // Auto-check transactions when part is selected
            _ = CheckTransactionsAsync();
        }

        OnPropertyChanged(nameof(SelectedPart));
        OnPropertyChanged(nameof(SelectedPartHasTransactions));
    }

    partial void OnPermanentlyRemoveChanged(bool value)
    {
        // Update command state and validation
        RemovePartCommand.NotifyCanExecuteChanged();
        
        // Force deactivation if part has transactions
        if (value && SelectedPartHasTransactions)
        {
            PermanentlyRemove = false;
            StatusMessage = "Cannot permanently remove part with transactions";
        }
    }

    partial void OnRemovalReasonChanged(string value)
    {
        // Update command state when reason changes
        RemovePartCommand.NotifyCanExecuteChanged();
    }

    partial void OnConfirmationTextChanged(string value)
    {
        // Update command state when confirmation changes
        RemovePartCommand.NotifyCanExecuteChanged();
    }
}
