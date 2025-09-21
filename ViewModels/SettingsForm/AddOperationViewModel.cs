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
/// ViewModel for adding new operations to the MTM system.
/// Provides operation creation with validation and categorization.
/// </summary>
public partial class AddOperationViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    [Required(ErrorMessage = "Operation number is required")]
    [RegularExpression(@"^\d{2,4}$", ErrorMessage = "Operation must be 2-4 digits")]
    private string _operationNumber = string.Empty;

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
    /// Existing operation numbers to prevent duplicates.
    /// </summary>
    public ObservableCollection<string> ExistingOperations { get; } = new();

    /// <summary>
    /// Gets whether the operation number already exists.
    /// </summary>
    public bool OperationExists => !string.IsNullOrEmpty(OperationNumber) && 
                                  ExistingOperations.Contains(OperationNumber);

    public AddOperationViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<AddOperationViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("AddOperationViewModel initialized");
        
        // Load initial data
        _ = LoadWorkCentersAsync();
        _ = LoadExistingOperationsAsync();
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

            Logger.LogInformation("Successfully loaded {WorkCenterCount} work centers", AvailableWorkCenters.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading work centers");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads existing operation numbers.
    /// </summary>
    [RelayCommand]
    private Task LoadExistingOperationsAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("LoadExistingOperations");
            Logger.LogInformation("Loading existing operation numbers");

            // Implementation would load from database
            // var operations = await _databaseService.GetOperationNumbersAsync().ConfigureAwait(false);
            
            ExistingOperations.Clear();
            
            // Add sample data for now
            ExistingOperations.Add("10");
            ExistingOperations.Add("20");
            ExistingOperations.Add("30");
            ExistingOperations.Add("90");
            ExistingOperations.Add("100");
            ExistingOperations.Add("110");
            ExistingOperations.Add("120");

            Logger.LogInformation("Successfully loaded {OperationCount} existing operations", ExistingOperations.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading existing operations");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Suggests the next available operation number.
    /// </summary>
    [RelayCommand]
    private Task SuggestOperationNumberAsync()
    {
        try
        {
            using var scope = Logger.BeginScope("SuggestOperationNumber");
            Logger.LogInformation("Suggesting next operation number");

            // Find next available operation number
            var existingNumbers = ExistingOperations.Select(op => int.TryParse(op, out int num) ? num : 0)
                                                   .Where(num => num > 0)
                                                   .OrderBy(num => num)
                                                   .ToList();

            int suggestedNumber = 10; // Start with 10
            
            if (existingNumbers.Any())
            {
                // Find gaps in the sequence or suggest next increment of 10
                for (int i = 10; i <= 9999; i += 10)
                {
                    if (!existingNumbers.Contains(i))
                    {
                        suggestedNumber = i;
                        break;
                    }
                }
            }

            OperationNumber = suggestedNumber.ToString();
            StatusMessage = $"Suggested operation number: {suggestedNumber}";
            
            Logger.LogInformation("Suggested operation number: {OperationNumber}", suggestedNumber);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error suggesting operation number: {ex.Message}";
            Logger.LogError(ex, "Error suggesting operation number");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates a new operation with the specified details.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreateOperation))]
    private async Task CreateOperationAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Creating operation...";

            using var scope = Logger.BeginScope("CreateOperation");
            Logger.LogInformation("Creating operation {OperationNumber}", OperationNumber);

            // Validate input
            if (string.IsNullOrWhiteSpace(OperationNumber) || string.IsNullOrWhiteSpace(Description))
            {
                StatusMessage = "Please fill in all required fields";
                return;
            }

            if (OperationExists)
            {
                StatusMessage = "Operation number already exists";
                return;
            }

            // Validate operation number format
            if (!System.Text.RegularExpressions.Regex.IsMatch(OperationNumber, @"^\d{2,4}$"))
            {
                StatusMessage = "Operation number must be 2-4 digits";
                return;
            }

            // Implementation would save to database
            // var result = await _databaseService.CreateOperationAsync(
            //     OperationNumber, Description, Department, WorkCenter, 
            //     SetupTimeHours, RuntimePerUnit, LaborRate, 
            //     RequiresQualityCheck, IsActive, Notes).ConfigureAwait(false);

            // Add to existing operations
            ExistingOperations.Add(OperationNumber);

            StatusMessage = "Operation created successfully";
            Logger.LogInformation("Successfully created operation {OperationNumber}", OperationNumber);

            // Reset form after successful creation
            await ResetFormAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating operation: {ex.Message}";
            Logger.LogError(ex, "Error creating operation {OperationNumber}", OperationNumber);
        }
        finally
        {
            IsLoading = false;
        }

        return;
    }

    /// <summary>
    /// Determines if operation can be created.
    /// </summary>
    private bool CanCreateOperation() => !string.IsNullOrWhiteSpace(OperationNumber) && 
                                        !string.IsNullOrWhiteSpace(Description) &&
                                        !OperationExists &&
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
        OperationNumber = string.Empty;
        Description = string.Empty;
        Department = "Production";
        WorkCenter = "WC001";
        SetupTimeHours = 0.5m;
        RuntimePerUnit = 0.1m;
        LaborRate = 25.00m;
        RequiresQualityCheck = true;
        IsActive = true;
        Notes = string.Empty;
        StatusMessage = "Form reset";

        Logger.LogInformation("Operation creation form reset");
        return Task.CompletedTask;
    }

    partial void OnOperationNumberChanged(string value)
    {
        // Update command state and validation
        CreateOperationCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(OperationExists));
        
        if (OperationExists)
        {
            StatusMessage = "Operation number already exists";
        }
        else if (!string.IsNullOrEmpty(value))
        {
            StatusMessage = "Ready";
        }
    }

    partial void OnDescriptionChanged(string value)
    {
        // Update command state when description changes
        CreateOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnSetupTimeHoursChanged(decimal value)
    {
        // Update command state when setup time changes
        CreateOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnRuntimePerUnitChanged(decimal value)
    {
        // Update command state when runtime changes
        CreateOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnLaborRateChanged(decimal value)
    {
        // Update command state when labor rate changes
        CreateOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnDepartmentChanged(string value)
    {
        // Filter work centers by department
        var filteredWorkCenters = AvailableWorkCenters.Where(wc => wc.Department == value).ToList();
        if (filteredWorkCenters.Any())
        {
            WorkCenter = filteredWorkCenters.First().WorkCenterCode;
        }
    }
}
