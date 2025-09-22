using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for creating new QuickButton shortcuts in the MTM WIP Application.
/// Provides form-driven interface for defining custom inventory operation shortcuts
/// with validation, duplicate detection, and integration with the QuickButtons system.
///
/// Features:
/// - Real-time validation against master data (Part IDs, Operations)
/// - Duplicate QuickButton detection and warning system
/// - Integration with TextBoxFuzzyValidationBehavior for suggestion overlays
/// - Manufacturing data integrity enforcement (no fallback data pattern)
/// - Event-driven architecture for UI coordination and refresh
///
/// Manufacturing Use Case:
/// Enables operators to create personalized shortcuts for frequently used
/// Part ID + Operation + Quantity combinations to streamline inventory operations.
/// </summary>
public partial class NewQuickButtonOverlayViewModel : BaseViewModel
{
    #region Private Fields

    private readonly IQuickButtonsService _quickButtonsService;
    private readonly IMasterDataService _masterDataService;

    #endregion

    #region Observable Properties - Form Fields

    /// <summary>
    /// Part ID input field with validation against master data
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required")]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _partId = string.Empty;

    /// <summary>
    /// Operation input field with validation against master data
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Operation is required")]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _operation = string.Empty;

    /// <summary>
    /// Quantity input field with numeric validation
    /// </summary>
    [ObservableProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private int _quantity = 0;  // Start with 0 to show error styling initially

    /// <summary>
    /// String representation of quantity for TextBox binding to avoid cast exceptions
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _quantityText = string.Empty;  // Start empty to show error styling

    /// <summary>
    /// Optional notes field for additional context
    /// </summary>
    [ObservableProperty]
    private string _notes = string.Empty;

    #endregion

    #region Observable Properties - Validation State

    /// <summary>
    /// Indicates if Part ID field has validation errors
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private bool _isPartIdValid = true;

    /// <summary>
    /// Indicates if Operation field has validation errors
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private bool _isOperationValid = true;

    /// <summary>
    /// Indicates if Quantity field has validation errors
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private bool _isQuantityValid = false;  // Start as false to match initial quantity = 0

    #endregion

    #region Observable Properties - UI State

    /// <summary>
    /// Loading state for create operation
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private bool _isLoading;

    /// <summary>
    /// Status message for user feedback
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStatusMessage))]
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Duplicate warning state
    /// </summary>
    [ObservableProperty]
    private bool _hasDuplicateWarning;

    /// <summary>
    /// Duplicate warning message
    /// </summary>
    [ObservableProperty]
    private string _duplicateWarningMessage = string.Empty;

    #endregion

    #region Observable Properties - Watermarks

    /// <summary>
    /// Dynamic watermark for Part ID field
    /// </summary>
    [ObservableProperty]
    private string _partIdWatermark = "Enter part ID...";

    /// <summary>
    /// Dynamic watermark for Operation field
    /// </summary>
    [ObservableProperty]
    private string _operationWatermark = "Enter operation...";

    /// <summary>
    /// Dynamic watermark for Quantity field
    /// </summary>
    [ObservableProperty]
    private string _quantityWatermark = "Enter quantity...";

    #endregion

    #region Computed Properties

    /// <summary>
    /// Indicates if status message should be visible
    /// </summary>
    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    /// <summary>
    /// Determines if Create button should be enabled
    /// </summary>
    public bool CanCreate =>
        !IsLoading &&
        IsPartIdValid &&
        IsOperationValid &&
        IsQuantityValid &&
        !string.IsNullOrWhiteSpace(PartId) &&
        !string.IsNullOrWhiteSpace(Operation) &&
        Quantity > 0;

    /// <summary>
    /// Master data collection for Part ID validation and suggestions
    /// </summary>
    public ObservableCollection<string> PartIds => _masterDataService?.PartIds ?? new ObservableCollection<string>();

    /// <summary>
    /// Master data collection for Operation validation and suggestions
    /// </summary>
    public ObservableCollection<string> Operations => _masterDataService?.Operations ?? new ObservableCollection<string>();

    #endregion

    #region Constructor

    public NewQuickButtonOverlayViewModel(
        ILogger<NewQuickButtonOverlayViewModel> logger,
        IQuickButtonsService quickButtonsService,
        IMasterDataService masterDataService)
        : base(logger)
    {
        _quickButtonsService = quickButtonsService ?? throw new ArgumentNullException(nameof(quickButtonsService));
        _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));

        Logger.LogInformation("NewQuickButtonOverlayViewModel initialized with dependency injection");

        // Initialize validation state - validate based on initial values
        IsPartIdValid = true;
        IsOperationValid = true;
        ParseAndValidateQuantity();  // This will set IsQuantityValid = false for initial empty QuantityText

        // Set up property change handlers for validation and duplicate checking
        PropertyChanged += OnPropertyChanged;

        // Initialize master data loading
        _ = Task.Run(async () =>
        {
            try
            {
                await _masterDataService.LoadAllMasterDataAsync();
                Logger.LogInformation("Master data loaded successfully for NewQuickButtonOverlayViewModel");

                // Trigger validation after master data is loaded
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ValidatePartId();
                    ValidateOperation();
                    ValidateQuantity();
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading master data for NewQuickButtonOverlayViewModel");
            }
        });
    }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handles property changes for validation and duplicate detection
    /// </summary>
    private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PartId):
                ValidatePartId();
                _ = CheckForDuplicatesAsync();
                break;
            case nameof(Operation):
                ValidateOperation();
                _ = CheckForDuplicatesAsync();
                break;
            case nameof(Quantity):
                ValidateQuantity();
                break;
            case nameof(QuantityText):
                ParseAndValidateQuantity();
                break;
        }
    }

    /// <summary>
    /// Property change handler specifically for Part ID with manufacturing validation
    /// </summary>
    partial void OnPartIdChanged(string value)
    {
        // Format Part ID according to manufacturing standards
        if (!string.IsNullOrWhiteSpace(value))
        {
            // Auto-uppercase and trim for consistency
            PartId = value.ToUpperInvariant().Trim();
        }

    }

    /// <summary>
    /// Property change handler for Operation with workflow validation
    /// </summary>
    partial void OnOperationChanged(string value)
    {
        // Format operation according to manufacturing standards
        if (!string.IsNullOrWhiteSpace(value))
        {
            // Operations are typically numeric strings in MTM (90, 100, 110, etc.)
            Operation = value.Trim();
        }

    }

    /// <summary>
    /// Property change handler for Quantity with range validation
    /// </summary>
    partial void OnQuantityChanged(int value)
    {
        // Sync QuantityText when Quantity is changed programmatically (like during reset)
        if (QuantityText != value.ToString())
        {
            QuantityText = value.ToString();
        }

    }

    /// <summary>
    /// Property change handler for QuantityText to prevent cast exceptions
    /// </summary>
    partial void OnQuantityTextChanged(string value)
    {
    }

    #endregion

    #region Validation Methods

    /// <summary>
    /// Validates Part ID against master data
    /// </summary>
    private void ValidatePartId()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(PartId))
            {
                IsPartIdValid = false;
                PartIdWatermark = "Part ID is required";
                return;
            }

            // Check against master data if available
            if (_masterDataService?.PartIds?.Count > 0)
            {
                IsPartIdValid = _masterDataService.PartIds.Contains(PartId, StringComparer.OrdinalIgnoreCase);
                PartIdWatermark = IsPartIdValid ? "Valid part ID" : "Part ID not found in master data";

                // Log first few part IDs for debugging if validation fails
                if (!IsPartIdValid)
                {
                    var firstFew = _masterDataService.PartIds.Take(5).ToList();
                }
            }
            else
            {
                // No master data available - assume valid if basic format is correct
                // Allow any non-empty string as Part ID when master data is unavailable
                IsPartIdValid = !string.IsNullOrWhiteSpace(PartId);
                PartIdWatermark = "Master data unavailable - validation skipped";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating Part ID: {PartId}", PartId);
            IsPartIdValid = false;
            PartIdWatermark = "Validation error";
        }
    }

    /// <summary>
    /// Validates Operation against master data
    /// </summary>
    private void ValidateOperation()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Operation))
            {
                IsOperationValid = false;
                OperationWatermark = "Operation is required";
                return;
            }

            // Check against master data if available
            if (_masterDataService?.Operations?.Count > 0)
            {
                IsOperationValid = _masterDataService.Operations.Contains(Operation, StringComparer.OrdinalIgnoreCase);
                OperationWatermark = IsOperationValid ? "Valid operation" : "Operation not found in master data";

                // Log first few operations for debugging if validation fails
                if (!IsOperationValid)
                {
                    var firstFew = _masterDataService.Operations.Take(10).ToList();
                }
            }
            else
            {
                // No master data available - assume valid if basic format is correct
                // Allow any non-empty string as Operation when master data is unavailable
                IsOperationValid = !string.IsNullOrWhiteSpace(Operation);
                OperationWatermark = "Master data unavailable - validation skipped";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating Operation: {Operation}", Operation);
            IsOperationValid = false;
            OperationWatermark = "Validation error";
        }
    }

    /// <summary>
    /// Validates Quantity for manufacturing requirements
    /// </summary>
    private void ValidateQuantity()
    {
        try
        {
            IsQuantityValid = Quantity > 0;
            QuantityWatermark = IsQuantityValid ? "Valid quantity" : "Quantity must be greater than 0";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating Quantity: {Quantity}", Quantity);
            IsQuantityValid = false;
            QuantityWatermark = "Validation error";
        }
    }

    /// <summary>
    /// Parses QuantityText to integer and validates, preventing cast exceptions
    /// </summary>
    private void ParseAndValidateQuantity()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(QuantityText))
            {
                // Empty input - set to 0 and mark invalid
                Quantity = 0;
                IsQuantityValid = false;
                QuantityWatermark = "Quantity is required";
                return;
            }

            // Try to parse the text to integer
            if (int.TryParse(QuantityText.Trim(), out int parsedQuantity))
            {
                // Valid integer - update quantity and validate
                Quantity = parsedQuantity;
                IsQuantityValid = Quantity > 0;
                QuantityWatermark = IsQuantityValid ? "Valid quantity" : "Quantity must be greater than 0";
            }
            else
            {
                // Invalid format - don't update Quantity, mark as invalid
                IsQuantityValid = false;
                QuantityWatermark = "Invalid number format";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error parsing/validating QuantityText: {QuantityText}", QuantityText);
            IsQuantityValid = false;
            QuantityWatermark = "Validation error";
        }
    }

    #endregion

    #region Duplicate Detection

    /// <summary>
    /// Checks for existing QuickButtons with the same Part ID + Operation combination
    /// </summary>
    private async Task CheckForDuplicatesAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(PartId) || string.IsNullOrWhiteSpace(Operation))
            {
                HasDuplicateWarning = false;
                DuplicateWarningMessage = string.Empty;
                return;
            }

            // Load current user's QuickButtons to check for duplicates
            var currentUser = Models.Model_AppVariables.CurrentUser;
            var existingButtons = await _quickButtonsService.LoadUserQuickButtonsAsync(currentUser);

            var duplicateButton = existingButtons.FirstOrDefault(b =>
                string.Equals(b.PartId, PartId, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(b.Operation, Operation, StringComparison.OrdinalIgnoreCase));

            if (duplicateButton != null)
            {
                HasDuplicateWarning = true;
                DuplicateWarningMessage = $"QuickButton exists: {PartId}/{Operation} (Qty: {duplicateButton.Quantity}). Will replace existing.";
                Logger.LogInformation("Duplicate QuickButton detected: {PartId}/{Operation}", PartId, Operation);
            }
            else
            {
                HasDuplicateWarning = false;
                DuplicateWarningMessage = string.Empty;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking for duplicate QuickButtons");
            // Don't show warning on error - allow creation to proceed
            HasDuplicateWarning = false;
            DuplicateWarningMessage = string.Empty;
        }
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates the new QuickButton with validation and database integration
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreate))]
    private async Task CreateAsync()
    {
        try
        {
            Logger.LogInformation("Creating new QuickButton: {PartId}/{Operation}/{Quantity}", PartId, Operation, Quantity);

            // Final validation before creation
            ValidatePartId();
            ValidateOperation();
            ValidateQuantity();

            // Check validation BEFORE setting IsLoading = true
            var validationPassed = IsPartIdValid &&
                                 IsOperationValid &&
                                 IsQuantityValid &&
                                 !string.IsNullOrWhiteSpace(PartId) &&
                                 !string.IsNullOrWhiteSpace(Operation) &&
                                 Quantity > 0;

            if (!validationPassed)
            {
                var validationErrors = new List<string>();
                if (!IsPartIdValid) validationErrors.Add($"Part ID ({PartIdWatermark})");
                if (!IsOperationValid) validationErrors.Add($"Operation ({OperationWatermark})");
                if (!IsQuantityValid) validationErrors.Add($"Quantity ({QuantityWatermark})");

                StatusMessage = $"Validation errors: {string.Join(", ", validationErrors)}";
                Logger.LogWarning("Cannot create QuickButton due to validation errors:");
                Logger.LogWarning("  - Part ID Valid: {PartIdValid}, Value: '{PartId}', Master data count: {PartIdCount}", IsPartIdValid, PartId, PartIds?.Count ?? 0);
                Logger.LogWarning("  - Operation Valid: {OperationValid}, Value: '{Operation}', Master data count: {OperationCount}", IsOperationValid, Operation, Operations?.Count ?? 0);
                Logger.LogWarning("  - Quantity Valid: {QuantityValid}, Value: {Quantity}", IsQuantityValid, Quantity);
                return;
            }

            // Now set loading state after validation passes
            IsLoading = true;
            StatusMessage = "Creating QuickButton...";

            // Create the QuickButton using the service
            var success = await _quickButtonsService.CreateQuickButtonAsync(
                PartId,
                Operation,
                string.Empty, // Location - not used in QuickButton creation context
                Quantity,
                Notes);

            if (success)
            {
                StatusMessage = "QuickButton created successfully!";
                Logger.LogInformation("Successfully created QuickButton: {PartId}/{Operation}/{Quantity}", PartId, Operation, Quantity);

                // Fire event to notify parent components
                QuickButtonCreated?.Invoke(this, EventArgs.Empty);

                // Reset form for potential additional entries
                await ResetFormAsync();
            }
            else
            {
                StatusMessage = "Failed to create QuickButton. Please try again.";
                Logger.LogError("Failed to create QuickButton: {PartId}/{Operation}/{Quantity}", PartId, Operation, Quantity);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in Create QuickButton: {Message}", ex.Message);
            await Services.ErrorHandling.HandleErrorAsync(ex, "Create QuickButton", Models.Model_AppVariables.CurrentUser);
            StatusMessage = "Error creating QuickButton. Please check logs for details.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Cancels the QuickButton creation and closes the overlay
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        Logger.LogInformation("QuickButton creation cancelled by user");

        // Fire event to notify parent components
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Resets the form to initial state
    /// </summary>
    [RelayCommand]
    private async Task ResetFormAsync()
    {
        try
        {
            PartId = string.Empty;
            Operation = string.Empty;
            Quantity = 1;
            QuantityText = "1";  // Set QuantityText to match Quantity
            Notes = string.Empty;

            // Reset validation state
            IsPartIdValid = true;
            IsOperationValid = true;
            IsQuantityValid = true;

            // Reset UI state
            HasDuplicateWarning = false;
            DuplicateWarningMessage = string.Empty;
            StatusMessage = string.Empty;

            // Reset watermarks
            PartIdWatermark = "Enter part ID...";
            OperationWatermark = "Enter operation...";
            QuantityWatermark = "Enter quantity...";


            await Task.CompletedTask; // Make async for consistency
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resetting form");
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Event fired when a QuickButton is successfully created
    /// </summary>
    public event EventHandler? QuickButtonCreated;

    /// <summary>
    /// Event fired when the user cancels QuickButton creation
    /// </summary>
    public event EventHandler? Cancelled;

    #endregion

    #region Cleanup

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                // Unsubscribe from events
                PropertyChanged -= OnPropertyChanged;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error disposing NewQuickButtonOverlayViewModel");
            }
        }

        base.Dispose(disposing);
    }

    #endregion
}
