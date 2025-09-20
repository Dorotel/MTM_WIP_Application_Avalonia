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
/// ViewModel for editing existing operations in the MTM system.
/// Provides operation selection, modification, and update functionality.
/// </summary>
public partial class EditOperationViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    [Required(ErrorMessage = "Operation description is required")]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _department = "Production";

    [ObservableProperty]
    private string _workCenter = "WC001";

    [ObservableProperty]
    [Range(0.01, 999.99, ErrorMessage = "Setup time must be between 0.01 and 999.99 hours")]
    private decimal _setupTimeHours = 0.5m;

    [ObservableProperty]
    [Range(0.01, 999.99, ErrorMessage = "Runtime must be between 0.01 and 999.99 hours per unit")]
    private decimal _runtimePerUnit = 0.1m;

    [ObservableProperty]
    [Range(0, double.MaxValue, ErrorMessage = "Labor rate must be non-negative")]
    private decimal _laborRate = 25.00m;

    [ObservableProperty]
    private bool _requiresQualityCheck = true;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private string? _selectedOperationNumber;

    /// <summary>
    /// Available operations for editing.
    /// </summary>
    public ObservableCollection<OperationInfo> AvailableOperations { get; } = new();

    /// <summary>
    /// Available departments for operations.
    /// </summary>
    public ObservableCollection<string> AvailableDepartments { get; } = new()
    {
        "Production", "Machining", "Assembly", "Quality", "Finishing", "Packaging", "Maintenance"
    };

    /// <summary>
    /// Available work centers.
    /// </summary>
    public ObservableCollection<WorkCenter> AvailableWorkCenters { get; } = new();

    /// <summary>
    /// Work centers filtered by selected department.
    /// </summary>
    public ObservableCollection<WorkCenter> FilteredWorkCenters { get; } = new();

    public EditOperationViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<EditOperationViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("EditOperationViewModel initialized");
        
        // Load initial data
        _ = LoadWorkCentersAsync();
    }

    /// <summary>
    /// Loads available operations for editing.
    /// </summary>
    [RelayCommand]
    private Task LoadOperationsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading operations...";

            using var scope = Logger.BeginScope("LoadOperations");
            Logger.LogInformation("Loading operations for editing");

            // Implementation would load from database
            // var operations = await _databaseService.GetOperationsAsync().ConfigureAwait(false);
            
            AvailableOperations.Clear();
            
            // Add sample data for now
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "10", 
                Description = "Material Prep", 
                Department = "Production", 
                WorkCenter = "WC001",
                SetupTimeHours = 0.25m,
                RuntimePerUnit = 0.05m,
                LaborRate = 22.00m,
                RequiresQualityCheck = false,
                IsActive = true 
            });
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "90", 
                Description = "CNC Machining", 
                Department = "Machining", 
                WorkCenter = "WC001",
                SetupTimeHours = 1.0m,
                RuntimePerUnit = 0.15m,
                LaborRate = 28.00m,
                RequiresQualityCheck = true,
                IsActive = true 
            });
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "100", 
                Description = "Assembly", 
                Department = "Assembly", 
                WorkCenter = "WC002",
                SetupTimeHours = 0.5m,
                RuntimePerUnit = 0.20m,
                LaborRate = 25.00m,
                RequiresQualityCheck = true,
                IsActive = true 
            });
            AvailableOperations.Add(new OperationInfo 
            { 
                OperationNumber = "110", 
                Description = "Quality Inspection", 
                Department = "Quality", 
                WorkCenter = "WC003",
                SetupTimeHours = 0.1m,
                RuntimePerUnit = 0.10m,
                LaborRate = 30.00m,
                RequiresQualityCheck = true,
                IsActive = true 
            });

            StatusMessage = $"Loaded {AvailableOperations.Count} operations";
            Logger.LogInformation("Successfully loaded {OperationCount} operations", AvailableOperations.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading operations: {ex.Message}";
            Logger.LogError(ex, "Error loading operations for editing");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads available work centers.
    /// </summary>
    [RelayCommand]
    private Task LoadWorkCentersAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("LoadWorkCenters");
            Logger.LogInformation("Loading work centers");

            // Implementation would load from database
            // var workCenters = await _databaseService.GetWorkCentersAsync().ConfigureAwait(false);
            
            AvailableWorkCenters.Clear();
            
            // Add sample data for now
            AvailableWorkCenters.Add(new WorkCenter { WorkCenterCode = "WC001", Name = "CNC Machining Center 1", Department = "Machining" });
            AvailableWorkCenters.Add(new WorkCenter { WorkCenterCode = "WC002", Name = "Assembly Station A", Department = "Assembly" });
            AvailableWorkCenters.Add(new WorkCenter { WorkCenterCode = "WC003", Name = "Quality Inspection", Department = "Quality" });
            AvailableWorkCenters.Add(new WorkCenter { WorkCenterCode = "WC004", Name = "Manual Machining", Department = "Machining" });
            AvailableWorkCenters.Add(new WorkCenter { WorkCenterCode = "WC005", Name = "Finishing Station", Department = "Finishing" });

            // Initialize filtered work centers
            UpdateFilteredWorkCenters();

            Logger.LogInformation("Successfully loaded {WorkCenterCount} work centers", AvailableWorkCenters.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading work centers");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads selected operation details for editing.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLoadOperationDetails))]
    private Task LoadOperationDetailsAsync()
    {
        if (string.IsNullOrEmpty(SelectedOperationNumber)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading operation details...";

            using var scope = Logger.BeginScope("LoadOperationDetails");
            Logger.LogInformation("Loading details for operation {OperationNumber}", SelectedOperationNumber);

            var selectedOperation = AvailableOperations.FirstOrDefault(op => op.OperationNumber == SelectedOperationNumber);
            if (selectedOperation != null)
            {
                Description = selectedOperation.Description;
                Department = selectedOperation.Department;
                WorkCenter = selectedOperation.WorkCenter;
                SetupTimeHours = selectedOperation.SetupTimeHours;
                RuntimePerUnit = selectedOperation.RuntimePerUnit;
                LaborRate = selectedOperation.LaborRate;
                RequiresQualityCheck = selectedOperation.RequiresQualityCheck;
                IsActive = selectedOperation.IsActive;
                Notes = selectedOperation.Notes ?? string.Empty;

                // Update filtered work centers for the department
                UpdateFilteredWorkCenters();

                StatusMessage = "Operation details loaded";
                Logger.LogInformation("Successfully loaded details for operation {OperationNumber}", SelectedOperationNumber);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading operation details: {ex.Message}";
            Logger.LogError(ex, "Error loading operation details for {OperationNumber}", SelectedOperationNumber);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if operation details can be loaded.
    /// </summary>
    private bool CanLoadOperationDetails() => !string.IsNullOrEmpty(SelectedOperationNumber) && !IsLoading;

    /// <summary>
    /// Updates the selected operation with modified information.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUpdateOperation))]
    private Task UpdateOperationAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Updating operation...";

            using var scope = Logger.BeginScope("UpdateOperation");
            Logger.LogInformation("Updating operation {OperationNumber}", SelectedOperationNumber);

            // Validate input
            if (string.IsNullOrWhiteSpace(Description))
            {
                StatusMessage = "Please fill in all required fields";
                return Task.CompletedTask;
            }

            // Implementation would update in database
            // var result = await _databaseService.UpdateOperationAsync(SelectedOperationNumber, Description, Department, WorkCenter, 
            //     SetupTimeHours, RuntimePerUnit, LaborRate, RequiresQualityCheck, IsActive, Notes).ConfigureAwait(false);

            // Update local collection
            var operation = AvailableOperations.FirstOrDefault(op => op.OperationNumber == SelectedOperationNumber);
            if (operation != null)
            {
                operation.Description = Description;
                operation.Department = Department;
                operation.WorkCenter = WorkCenter;
                operation.SetupTimeHours = SetupTimeHours;
                operation.RuntimePerUnit = RuntimePerUnit;
                operation.LaborRate = LaborRate;
                operation.RequiresQualityCheck = RequiresQualityCheck;
                operation.IsActive = IsActive;
                operation.Notes = Notes;
            }

            StatusMessage = "Operation updated successfully";
            Logger.LogInformation("Successfully updated operation {OperationNumber}", SelectedOperationNumber);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating operation: {ex.Message}";
            Logger.LogError(ex, "Error updating operation {OperationNumber}", SelectedOperationNumber);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if operation can be updated.
    /// </summary>
    private bool CanUpdateOperation() => !string.IsNullOrEmpty(SelectedOperationNumber) && 
                                        !string.IsNullOrWhiteSpace(Description) && 
                                        SetupTimeHours > 0 &&
                                        RuntimePerUnit > 0 &&
                                        LaborRate >= 0 &&
                                        !IsLoading;

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        Description = string.Empty;
        Department = "Production";
        WorkCenter = "WC001";
        SetupTimeHours = 0.5m;
        RuntimePerUnit = 0.1m;
        LaborRate = 25.00m;
        RequiresQualityCheck = true;
        IsActive = true;
        Notes = string.Empty;
        SelectedOperationNumber = null;
        StatusMessage = "Form reset";

        Logger.LogInformation("Operation edit form reset");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates the filtered work centers based on selected department.
    /// </summary>
    private void UpdateFilteredWorkCenters()
    {
        FilteredWorkCenters.Clear();
        var filtered = AvailableWorkCenters.Where(wc => wc.Department == Department).ToList();
        foreach (var workCenter in filtered)
        {
            FilteredWorkCenters.Add(workCenter);
        }
    }

    partial void OnSelectedOperationNumberChanged(string? value)
    {
        // Update command states
        LoadOperationDetailsCommand.NotifyCanExecuteChanged();
        UpdateOperationCommand.NotifyCanExecuteChanged();
        
        // Clear form when selection changes
        if (string.IsNullOrEmpty(value))
        {
            Description = string.Empty;
            Department = "Production";
            WorkCenter = "WC001";
            SetupTimeHours = 0.5m;
            RuntimePerUnit = 0.1m;
            LaborRate = 25.00m;
            RequiresQualityCheck = true;
            IsActive = true;
            Notes = string.Empty;
        }
    }

    partial void OnDescriptionChanged(string value)
    {
        // Update command state when description changes
        UpdateOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnDepartmentChanged(string value)
    {
        // Update filtered work centers and select first available
        UpdateFilteredWorkCenters();
        if (FilteredWorkCenters.Any())
        {
            WorkCenter = FilteredWorkCenters.First().WorkCenterCode;
        }
    }

    partial void OnSetupTimeHoursChanged(decimal value)
    {
        // Update command state when setup time changes
        UpdateOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnRuntimePerUnitChanged(decimal value)
    {
        // Update command state when runtime changes
        UpdateOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnLaborRateChanged(decimal value)
    {
        // Update command state when labor rate changes
        UpdateOperationCommand.NotifyCanExecuteChanged();
    }
}

/// <summary>
/// Operation information model for display and editing.
/// </summary>
public class OperationInfo
{
    public string OperationNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string WorkCenter { get; set; } = string.Empty;
    public decimal SetupTimeHours { get; set; }
    public decimal RuntimePerUnit { get; set; }
    public decimal LaborRate { get; set; }
    public bool RequiresQualityCheck { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

/// <summary>
/// Data model for work center information.
/// </summary>
public class WorkCenter
{
    public string WorkCenterCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
