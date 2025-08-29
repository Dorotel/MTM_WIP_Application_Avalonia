using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for InventoryTabView.
/// Implements the primary inventory management interface within the MTM WIP Application.
/// Provides keyboard shortcuts, focus management, event handling, and database integration.
/// Uses standard .NET patterns without ReactiveUI dependencies.
/// </summary>
public partial class InventoryTabView : UserControl
{
    private InventoryTabViewModel? _viewModel;
    private QuickButtonsViewModel? _quickButtonsViewModel;
    private readonly IServiceProvider? _serviceProvider;

    // UI Control references for direct manipulation
    private AutoCompleteBox? _partAutoCompleteBox;
    private AutoCompleteBox? _operationAutoCompleteBox;
    private AutoCompleteBox? _locationAutoCompleteBox;
    private TextBox? _quantityTextBox;
    private TextBox? _notesTextBox;
    private Button? _saveButton;

    /// <summary>
    /// Initializes a new instance of the InventoryTabView.
    /// </summary>
    public InventoryTabView()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during InitializeComponent: {ex.Message}");
            // Continue with manual initialization if InitializeComponent fails
        }
        
        // Set up event handling
        InitializeControlReferences();
        SetupEventHandlers();
        
        // Set up loaded event to initialize ViewModel
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Initializes a new instance of the InventoryTabView with dependency injection support.
    /// </summary>
    public InventoryTabView(IServiceProvider serviceProvider) : this()
    {
        _serviceProvider = serviceProvider;
    }

    #region Control Initialization

    /// <summary>
    /// Initializes references to UI controls for direct manipulation.
    /// </summary>
    private void InitializeControlReferences()
    {
        try
        {
            _partAutoCompleteBox = this.FindControl<AutoCompleteBox>("PartAutoCompleteBox");
            _operationAutoCompleteBox = this.FindControl<AutoCompleteBox>("OperationAutoCompleteBox");
            _locationAutoCompleteBox = this.FindControl<AutoCompleteBox>("LocationAutoCompleteBox");
            _quantityTextBox = this.FindControl<TextBox>("QuantityTextBox");
            _notesTextBox = this.FindControl<TextBox>("NotesTextBox");
            _saveButton = this.FindControl<Button>("SaveButton");

            System.Diagnostics.Debug.WriteLine("UI control references initialized successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing control references: {ex.Message}");
        }
    }

    /// <summary>
    /// Sets up event handlers for UI controls.
    /// </summary>
    private void SetupEventHandlers()
    {
        try
        {
            // Global keyboard event handling
            KeyDown += OnKeyDown;

            // AutoCompleteBox event handlers for validation and business logic
            if (_partAutoCompleteBox != null)
            {
                _partAutoCompleteBox.TextChanged += OnPartTextChanged;
                _partAutoCompleteBox.SelectionChanged += OnPartSelectionChanged;
                _partAutoCompleteBox.LostFocus += OnPartLostFocus;
            }

            if (_operationAutoCompleteBox != null)
            {
                _operationAutoCompleteBox.TextChanged += OnOperationTextChanged;
                _operationAutoCompleteBox.SelectionChanged += OnOperationSelectionChanged;
                _operationAutoCompleteBox.LostFocus += OnOperationLostFocus;
            }

            if (_locationAutoCompleteBox != null)
            {
                _locationAutoCompleteBox.TextChanged += OnLocationTextChanged;
                _locationAutoCompleteBox.SelectionChanged += OnLocationSelectionChanged;
                _locationAutoCompleteBox.LostFocus += OnLocationLostFocus;
            }

            // TextBox event handlers
            if (_quantityTextBox != null)
            {
                _quantityTextBox.TextChanged += OnQuantityTextChanged;
                _quantityTextBox.LostFocus += OnQuantityLostFocus;
            }

            if (_notesTextBox != null)
            {
                _notesTextBox.TextChanged += OnNotesTextChanged;
            }

            // Button event handlers for additional business logic
            if (_saveButton != null)
            {
                _saveButton.Click += OnSaveButtonClick;
            }

            System.Diagnostics.Debug.WriteLine("Event handlers set up successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting up event handlers: {ex.Message}");
        }
    }

    #endregion

    #region ViewModel Integration

    /// <summary>
    /// Handles the Loaded event to set up the ViewModel via dependency injection.
    /// </summary>
    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Set up ViewModel if not already set
            if (DataContext is InventoryTabViewModel vm)
            {
                _viewModel = vm;
                
                // Subscribe to ViewModel events for database operations
                _viewModel.SaveCompleted += OnInventorySaveCompleted;
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                
                // Initialize database connections and load lookup data
                await InitializeDatabaseIntegrationAsync();
                
                // Apply initial form state and focus
                InitializeFormState();
                
                System.Diagnostics.Debug.WriteLine("InventoryTabView ViewModel connected successfully");
            }

            // Initialize QuickButtons integration if available
            await InitializeQuickButtonsIntegrationAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't crash the UI
            System.Diagnostics.Debug.WriteLine($"Error setting up InventoryTabView ViewModel: {ex.Message}");
            
            // Use error handling service if available
            if (_serviceProvider?.GetService(typeof(ILogger<InventoryTabView>)) is ILogger<InventoryTabView> logger)
            {
                logger.LogError(ex, "Failed to initialize InventoryTabView");
            }
        }
    }

    /// <summary>
    /// Initializes the form state and applies business rules.
    /// </summary>
    private void InitializeFormState()
    {
        try
        {
            // Set initial focus to Part ID field
            MoveFocusToFirstControl();

            // Apply any saved user preferences
            ApplyUserPreferences();

            // Enable/disable controls based on initial state
            UpdateControlStates();

            System.Diagnostics.Debug.WriteLine("Form state initialized successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing form state: {ex.Message}");
        }
    }

    /// <summary>
    /// Applies user preferences and last used values.
    /// </summary>
    private void ApplyUserPreferences()
    {
        try
        {
            if (_viewModel == null) return;

            // Apply any application state defaults
            if (_serviceProvider?.GetService(typeof(IApplicationStateService)) is IApplicationStateService appStateService)
            {
                // Set default operation if available
                if (!string.IsNullOrEmpty(appStateService.CurrentOperation) && string.IsNullOrEmpty(_viewModel.SelectedOperation))
                {
                    _viewModel.SelectedOperation = appStateService.CurrentOperation;
                }

                // Set default location if available
                if (!string.IsNullOrEmpty(appStateService.CurrentLocation) && string.IsNullOrEmpty(_viewModel.SelectedLocation))
                {
                    _viewModel.SelectedLocation = appStateService.CurrentLocation;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying user preferences: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates control states based on current ViewModel state.
    /// </summary>
    private void UpdateControlStates()
    {
        try
        {
            if (_viewModel == null) return;

            // Update AutoCompleteBox loading states
            if (_partAutoCompleteBox != null)
            {
                _partAutoCompleteBox.Classes.Set("loading", _viewModel.IsLoadingParts);
            }

            if (_operationAutoCompleteBox != null)
            {
                _operationAutoCompleteBox.Classes.Set("loading", _viewModel.IsLoadingOperations);
            }

            if (_locationAutoCompleteBox != null)
            {
                _locationAutoCompleteBox.Classes.Set("loading", _viewModel.IsLoadingLocations);
            }

            // Update error states
            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating control states: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates validation states on UI controls.
    /// </summary>
    private void UpdateValidationStates()
    {
        try
        {
            if (_viewModel == null) return;

            // Update error styling based on validation
            if (_partAutoCompleteBox != null)
            {
                _partAutoCompleteBox.Classes.Set("error", !_viewModel.IsPartValid);
            }

            if (_operationAutoCompleteBox != null)
            {
                _operationAutoCompleteBox.Classes.Set("error", !_viewModel.IsOperationValid);
            }

            if (_locationAutoCompleteBox != null)
            {
                _locationAutoCompleteBox.Classes.Set("error", !_viewModel.IsLocationValid);
            }

            if (_quantityTextBox != null)
            {
                _quantityTextBox.Classes.Set("error", !_viewModel.IsQuantityValid);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating validation states: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Part ID

    /// <summary>
    /// Handles Part ID text changes for real-time validation.
    /// </summary>
    private async void OnPartTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var partId = autoCompleteBox.Text?.Trim() ?? string.Empty;

            // Update ViewModel property
            _viewModel.SelectedPart = partId;

            // Perform real-time validation if part ID has sufficient length
            if (partId.Length >= 3)
            {
                await ValidatePartIdAsync(partId);
            }

            // Update control styling
            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling part text change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Part ID selection from dropdown.
    /// </summary>
    private async void OnPartSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var selectedPart = autoCompleteBox.SelectedItem?.ToString() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(selectedPart))
            {
                _viewModel.SelectedPart = selectedPart;
                await LoadPartSpecificDataAsync(selectedPart);
                
                // Move to next field automatically
                MoveFocusToNextControl();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling part selection: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Part ID field losing focus for validation.
    /// </summary>
    private async void OnPartLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var partId = autoCompleteBox.Text?.Trim() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(partId))
            {
                await ValidatePartIdAsync(partId);
            }

            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling part lost focus: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Operation

    /// <summary>
    /// Handles Operation text changes.
    /// </summary>
    private async void OnOperationTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var operation = autoCompleteBox.Text?.Trim() ?? string.Empty;
            _viewModel.SelectedOperation = operation;

            // Load operation-specific data if valid operation
            if (!string.IsNullOrEmpty(operation) && operation.Length >= 2)
            {
                await LoadOperationSpecificDataAsync(operation);
            }

            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling operation text change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Operation selection from dropdown.
    /// </summary>
    private async void OnOperationSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var selectedOperation = autoCompleteBox.SelectedItem?.ToString() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(selectedOperation))
            {
                _viewModel.SelectedOperation = selectedOperation;
                await LoadOperationSpecificDataAsync(selectedOperation);
                
                // Move to next field automatically
                MoveFocusToNextControl();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling operation selection: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Operation field losing focus.
    /// </summary>
    private async void OnOperationLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var operation = autoCompleteBox.Text?.Trim() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(operation))
            {
                await LoadOperationSpecificDataAsync(operation);
            }

            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling operation lost focus: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Location

    /// <summary>
    /// Handles Location text changes.
    /// </summary>
    private async void OnLocationTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var location = autoCompleteBox.Text?.Trim() ?? string.Empty;
            _viewModel.SelectedLocation = location;

            // Validate location if sufficient length
            if (!string.IsNullOrEmpty(location) && location.Length >= 2)
            {
                await ValidateLocationAsync(location);
            }

            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling location text change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Location selection from dropdown.
    /// </summary>
    private async void OnLocationSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var selectedLocation = autoCompleteBox.SelectedItem?.ToString() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(selectedLocation))
            {
                _viewModel.SelectedLocation = selectedLocation;
                await ValidateLocationAsync(selectedLocation);
                
                // Move to next field automatically
                MoveFocusToNextControl();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling location selection: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Location field losing focus.
    /// </summary>
    private async void OnLocationLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var location = autoCompleteBox.Text?.Trim() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(location))
            {
                await ValidateLocationAsync(location);
            }

            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling location lost focus: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Quantity

    /// <summary>
    /// Handles Quantity text changes with real-time validation.
    /// </summary>
    private void OnQuantityTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not TextBox textBox) return;

            var quantityText = textBox.Text?.Trim() ?? string.Empty;
            
            // Parse and validate quantity
            if (int.TryParse(quantityText, out int quantity) && quantity > 0)
            {
                _viewModel.Quantity = quantity;
            }
            else if (!string.IsNullOrEmpty(quantityText))
            {
                // Invalid quantity - keep the text but mark as invalid
                // The ViewModel's IsQuantityValid will handle validation
            }

            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling quantity text change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Quantity field losing focus with validation.
    /// </summary>
    private void OnQuantityLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not TextBox textBox) return;

            var quantityText = textBox.Text?.Trim() ?? string.Empty;
            
            // Validate and format quantity
            if (int.TryParse(quantityText, out int quantity))
            {
                if (quantity <= 0)
                {
                    // Reset to minimum valid quantity
                    _viewModel.Quantity = 1;
                    textBox.Text = "1";
                }
                else
                {
                    _viewModel.Quantity = quantity;
                    // Format the text for consistency
                    textBox.Text = quantity.ToString();
                }
            }
            else if (!string.IsNullOrEmpty(quantityText))
            {
                // Invalid input - reset to default
                _viewModel.Quantity = 1;
                textBox.Text = "1";
            }

            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling quantity lost focus: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Notes

    /// <summary>
    /// Handles Notes text changes.
    /// </summary>
    private void OnNotesTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not TextBox textBox) return;

            _viewModel.Notes = textBox.Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling notes text change: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Buttons

    /// <summary>
    /// Handles Save button click with additional business logic.
    /// </summary>
    private async void OnSaveButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null) return;

            // Perform final validation before save
            if (!await PerformFinalValidationAsync())
            {
                return;
            }

            // Clear any previous errors
            _viewModel.HasError = false;
            _viewModel.ErrorMessage = string.Empty;

            // Execute save command (this will be handled by the ViewModel)
            if (_viewModel.SaveCommand.CanExecute(null))
            {
                _viewModel.SaveCommand.Execute(null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling save button click: {ex.Message}");
            
            if (_viewModel != null)
            {
                _viewModel.HasError = true;
                _viewModel.ErrorMessage = "An error occurred while saving. Please try again.";
            }
        }
    }

    #endregion

    #region Business Logic Validation

    /// <summary>
    /// Validates Part ID against database and business rules.
    /// </summary>
    private async Task<bool> ValidatePartIdAsync(string partId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(partId)) return false;

            if (_serviceProvider?.GetService(typeof(IDatabaseService)) is IDatabaseService databaseService)
            {
                // Check if part exists in master data
                var parameters = new Dictionary<string, object>
                {
                    ["p_ItemNumber"] = partId
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    databaseService.GetConnectionString(),
                    "md_part_ids_Get_ByItemNumber",
                    parameters
                );

                if (result.IsSuccess && result.Data.Rows.Count > 0)
                {
                    // Part exists - clear any previous errors
                    if (_viewModel?.ErrorMessage.Contains("Part") == true)
                    {
                        _viewModel.HasError = false;
                        _viewModel.ErrorMessage = string.Empty;
                    }
                    return true;
                }
                else
                {
                    // Part not found - show warning but allow entry
                    System.Diagnostics.Debug.WriteLine($"Part ID '{partId}' not found in master data - allowing entry");
                    return true; // Allow entry even if not in master data
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error validating part ID: {ex.Message}");
            return true; // Don't block user on validation errors
        }
    }

    /// <summary>
    /// Validates location against database and business rules.
    /// </summary>
    private async Task<bool> ValidateLocationAsync(string location)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(location)) return false;

            if (_serviceProvider?.GetService(typeof(IDatabaseService)) is IDatabaseService databaseService)
            {
                // Validate location exists and is active
                var parameters = new Dictionary<string, object>
                {
                    ["p_Location"] = location
                };

                // Note: This stored procedure may not exist yet - handle gracefully
                try
                {
                    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                        databaseService.GetConnectionString(),
                        "md_locations_Validate",
                        parameters
                    );

                    if (result.IsSuccess && result.Data.Rows.Count > 0)
                    {
                        var isActive = Convert.ToBoolean(result.Data.Rows[0]["IsActive"] ?? true);
                        if (!isActive && _viewModel != null)
                        {
                            _viewModel.HasError = true;
                            _viewModel.ErrorMessage = $"Location '{location}' is not active.";
                            return false;
                        }
                        
                        // Clear any previous location errors
                        if (_viewModel?.ErrorMessage.Contains("Location") == true)
                        {
                            _viewModel.HasError = false;
                            _viewModel.ErrorMessage = string.Empty;
                        }
                        return true;
                    }
                }
                catch
                {
                    // If validation stored procedure doesn't exist, just check if location is in the list
                    if (_viewModel?.Locations.Contains(location) == true)
                    {
                        return true;
                    }
                }
            }

            return true; // Allow entry even if validation fails
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error validating location: {ex.Message}");
            return true; // Don't block user on validation errors
        }
    }

    /// <summary>
    /// Performs final validation before save operation.
    /// </summary>
    private async Task<bool> PerformFinalValidationAsync()
    {
        try
        {
            if (_viewModel == null) return false;

            var validationErrors = new List<string>();

            // Validate required fields
            if (string.IsNullOrWhiteSpace(_viewModel.SelectedPart))
            {
                validationErrors.Add("Part ID is required");
            }

            if (string.IsNullOrWhiteSpace(_viewModel.SelectedOperation))
            {
                validationErrors.Add("Operation is required");
            }

            if (string.IsNullOrWhiteSpace(_viewModel.SelectedLocation))
            {
                validationErrors.Add("Location is required");
            }

            if (_viewModel.Quantity <= 0)
            {
                validationErrors.Add("Quantity must be greater than zero");
            }

            // Validate business rules
            if (!string.IsNullOrWhiteSpace(_viewModel.SelectedPart))
            {
                var isValidPart = await ValidatePartIdAsync(_viewModel.SelectedPart);
                if (!isValidPart)
                {
                    validationErrors.Add("Invalid Part ID");
                }
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.SelectedLocation))
            {
                var isValidLocation = await ValidateLocationAsync(_viewModel.SelectedLocation);
                if (!isValidLocation)
                {
                    validationErrors.Add("Invalid Location");
                }
            }

            // Show validation errors
            if (validationErrors.Count > 0)
            {
                _viewModel.HasError = true;
                _viewModel.ErrorMessage = string.Join("; ", validationErrors);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error performing final validation: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Data Loading Methods

    /// <summary>
    /// Loads part-specific data from database.
    /// </summary>
    private async Task LoadPartSpecificDataAsync(string partId)
    {
        try
        {
            if (_serviceProvider?.GetService(typeof(IDatabaseService)) is IDatabaseService databaseService)
            {
                // Load part details for validation/display purposes
                var parameters = new Dictionary<string, object>
                {
                    ["p_ItemNumber"] = partId
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    databaseService.GetConnectionString(),
                    "md_part_ids_Get_ByItemNumber",
                    parameters
                );

                if (result.IsSuccess && result.Data.Rows.Count > 0 && _viewModel != null)
                {
                    // Part exists - could update UI with additional part info if needed
                    var partRow = result.Data.Rows[0];
                    var description = partRow["Description"]?.ToString();
                    var customer = partRow["Customer"]?.ToString();
                    
                    // Clear any previous part-related errors
                    if (_viewModel.ErrorMessage.Contains("Part"))
                    {
                        _viewModel.HasError = false;
                        _viewModel.ErrorMessage = string.Empty;
                    }

                    System.Diagnostics.Debug.WriteLine($"Loaded part data: {description} for customer {customer}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading part-specific data: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads operation-specific data from database.
    /// </summary>
    private async Task LoadOperationSpecificDataAsync(string operation)
    {
        try
        {
            // Load operation-specific business rules, default locations, etc.
            if (_serviceProvider?.GetService(typeof(IDatabaseService)) is IDatabaseService databaseService)
            {
                // Example: Load default location for this operation
                // This could be enhanced with additional business logic
                System.Diagnostics.Debug.WriteLine($"Loading operation-specific data for operation: {operation}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading operation-specific data: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes database integration and loads initial lookup data.
    /// </summary>
    private async Task InitializeDatabaseIntegrationAsync()
    {
        try
        {
            if (_viewModel == null) return;

            // Load initial lookup data from database
            if (_viewModel.LoadDataCommand.CanExecute(null))
            {
                _viewModel.LoadDataCommand.Execute(null);
            }

            // Load application state from database if needed
            await LoadApplicationStateAsync();
            
            System.Diagnostics.Debug.WriteLine("Database integration initialized successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing database integration: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads application state from database.
    /// </summary>
    private async Task LoadApplicationStateAsync()
    {
        try
        {
            // Load user preferences, last used values, etc. from database
            if (_serviceProvider?.GetService(typeof(IApplicationStateService)) is IApplicationStateService appStateService)
            {
                // Set default values based on application state
                if (_viewModel != null)
                {
                    if (!string.IsNullOrEmpty(appStateService.CurrentOperation))
                    {
                        _viewModel.SelectedOperation = appStateService.CurrentOperation;
                    }
                    
                    if (!string.IsNullOrEmpty(appStateService.CurrentLocation))
                    {
                        _viewModel.SelectedLocation = appStateService.CurrentLocation;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading application state: {ex.Message}");
        }
    }

    #endregion

    #region Save Completion and QuickButtons Integration

    /// <summary>
    /// Handles inventory save completion to update QuickButtons.
    /// </summary>
    private async void OnInventorySaveCompleted(object? sender, InventorySavedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Inventory saved: Part={e.PartId}, Operation={e.Operation}, Quantity={e.Quantity}");

            // Update QuickButtons after successful save
            if (_quickButtonsViewModel != null)
            {
                await _quickButtonsViewModel.AddQuickButtonFromOperationAsync(
                    e.PartId,
                    e.Operation,
                    e.Quantity);

                System.Diagnostics.Debug.WriteLine($"Added quick button: {e.PartId}, {e.Operation}, {e.Quantity}");
            }
            
            // Reset form focus to first control for next entry
            MoveFocusToFirstControl();
            
            // Log the action for audit purposes
            if (_serviceProvider?.GetService(typeof(ILogger<InventoryTabView>)) is ILogger<InventoryTabView> logger)
            {
                logger.LogInformation("Inventory saved and added to QuickButtons: PartId={PartId}, Operation={Operation}, Quantity={Quantity}", 
                    e.PartId, e.Operation, e.Quantity);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling inventory save completion: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes QuickButtons integration to handle quick action events.
    /// </summary>
    private async Task InitializeQuickButtonsIntegrationAsync()
    {
        try
        {
            // Find QuickButtonsView in the visual tree
            var quickButtonsView = FindQuickButtonsView();
            if (quickButtonsView?.DataContext is QuickButtonsViewModel quickButtonsViewModel)
            {
                _quickButtonsViewModel = quickButtonsViewModel;
                
                // Subscribe to quick action executed events
                _quickButtonsViewModel.QuickActionExecuted += OnQuickActionExecuted;
                
                System.Diagnostics.Debug.WriteLine("QuickButtons integration initialized successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("QuickButtonsView not found in visual tree - integration skipped");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing QuickButtons integration: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles quick action executed events from QuickButtonsView.
    /// </summary>
    private async void OnQuickActionExecuted(object? sender, QuickActionExecutedEventArgs e)
    {
        try
        {
            if (_viewModel == null) return;

            System.Diagnostics.Debug.WriteLine($"Quick action executed: Part={e.PartId}, Operation={e.Operation}, Quantity={e.Quantity}");

            // Populate form fields with quick action data
            _viewModel.SelectedPart = e.PartId;
            _viewModel.SelectedOperation = e.Operation;
            _viewModel.Quantity = e.Quantity;
            
            // Clear previous error state
            _viewModel.HasError = false;
            _viewModel.ErrorMessage = string.Empty;
            
            // Update UI control states
            UpdateControlStates();
            
            // Focus the location field (likely next field to fill)
            _locationAutoCompleteBox?.Focus();
            
            // Log the action for audit purposes
            if (_serviceProvider?.GetService(typeof(ILogger<InventoryTabView>)) is ILogger<InventoryTabView> logger)
            {
                logger.LogInformation("Quick action applied: PartId={PartId}, Operation={Operation}, Quantity={Quantity}", 
                    e.PartId, e.Operation, e.Quantity);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling quick action: {ex.Message}");
        }
    }

    #endregion

    #region ViewModel Property Change Handling

    /// <summary>
    /// Handles ViewModel property changes to respond to database operations.
    /// </summary>
    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(_viewModel.SelectedPart):
                    await OnPartChangedAsync();
                    break;
                    
                case nameof(_viewModel.SelectedOperation):
                    await OnOperationChangedAsync();
                    break;
                    
                case nameof(_viewModel.SelectedLocation):
                    await OnLocationChangedAsync();
                    break;
                    
                case nameof(_viewModel.IsLoading):
                case nameof(_viewModel.IsLoadingParts):
                case nameof(_viewModel.IsLoadingOperations):
                case nameof(_viewModel.IsLoadingLocations):
                    UpdateControlStates();
                    break;

                case nameof(_viewModel.HasError):
                case nameof(_viewModel.ErrorMessage):
                    UpdateValidationStates();
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling property change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles part selection changes to update related data.
    /// </summary>
    private async Task OnPartChangedAsync()
    {
        try
        {
            if (_viewModel == null || string.IsNullOrEmpty(_viewModel.SelectedPart)) return;

            await LoadPartSpecificDataAsync(_viewModel.SelectedPart);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling part change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles operation selection changes to update related data.
    /// </summary>
    private async Task OnOperationChangedAsync()
    {
        try
        {
            if (_viewModel == null || string.IsNullOrEmpty(_viewModel.SelectedOperation)) return;

            await LoadOperationSpecificDataAsync(_viewModel.SelectedOperation);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling operation change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles location selection changes to update related data.
    /// </summary>
    private async Task OnLocationChangedAsync()
    {
        try
        {
            if (_viewModel == null || string.IsNullOrEmpty(_viewModel.SelectedLocation)) return;

            await ValidateLocationAsync(_viewModel.SelectedLocation);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling location change: {ex.Message}");
        }
    }

    #endregion

    #region Keyboard Shortcuts and Navigation

    /// <summary>
    /// Handles keyboard shortcuts for enhanced user experience.
    /// F5: Reset form, Shift+F5: Hard reset, Enter: Next field/Save, Escape: Clear errors
    /// </summary>
    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            switch (e.Key)
            {
                case Key.F5:
                    e.Handled = true;
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        // Shift+F5: Hard reset with database refresh
                        await ExecuteHardResetAsync();
                    }
                    else
                    {
                        // F5: Soft reset
                        if (_viewModel.ResetCommand.CanExecute(null))
                        {
                            _viewModel.ResetCommand.Execute(null);
                            MoveFocusToFirstControl();
                        }
                    }
                    break;

                case Key.Enter:
                    e.Handled = true;
                    await HandleEnterKeyAsync(e.Source);
                    break;

                case Key.Escape:
                    e.Handled = true;
                    // Clear any error state and focus the first control
                    _viewModel.HasError = false;
                    _viewModel.ErrorMessage = string.Empty;
                    MoveFocusToFirstControl();
                    break;

                case Key.Tab:
                    // Tab key handling is automatic, but we can enhance it
                    if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        // Forward tab - let normal behavior occur
                    }
                    else
                    {
                        // Shift+Tab - reverse navigation
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling keyboard shortcut: {ex.Message}");
        }
    }

    /// <summary>
    /// Executes a hard reset including database refresh.
    /// </summary>
    private async Task ExecuteHardResetAsync()
    {
        try
        {
            if (_viewModel == null) return;

            // Reset form fields
            if (_viewModel.ResetCommand.CanExecute(null))
            {
                _viewModel.ResetCommand.Execute(null);
            }

            // Refresh all lookup data from database
            if (_viewModel.RefreshDataCommand.CanExecute(null))
            {
                _viewModel.RefreshDataCommand.Execute(null);
            }

            // Refresh QuickButtons if available
            if (_quickButtonsViewModel?.RefreshButtonsCommand.CanExecute(null) == true)
            {
                _quickButtonsViewModel.RefreshButtonsCommand.Execute(null);
            }

            // Reset focus
            MoveFocusToFirstControl();

            System.Diagnostics.Debug.WriteLine("Hard reset completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during hard reset: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Enter key navigation - moves to next logical control or executes save.
    /// </summary>
    private async Task HandleEnterKeyAsync(object? source)
    {
        if (_viewModel == null) return;

        try
        {
            // If focused on Save button and can save, execute save command
            if (source == _saveButton && _viewModel.SaveCommand.CanExecute(null))
            {
                _viewModel.SaveCommand.Execute(null);
                return;
            }

            // Otherwise, move focus to next control
            MoveFocusToNextControl();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling Enter key: {ex.Message}");
        }
    }

    /// <summary>
    /// Moves focus to the next logical control in the form.
    /// </summary>
    private void MoveFocusToNextControl()
    {
        try
        {
            var currentFocus = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
            
            // Define the logical tab order
            if (currentFocus == _partAutoCompleteBox)
            {
                _operationAutoCompleteBox?.Focus();
            }
            else if (currentFocus == _operationAutoCompleteBox)
            {
                _locationAutoCompleteBox?.Focus();
            }
            else if (currentFocus == _locationAutoCompleteBox)
            {
                _quantityTextBox?.Focus();
            }
            else if (currentFocus == _quantityTextBox)
            {
                _notesTextBox?.Focus();
            }
            else if (currentFocus == _notesTextBox)
            {
                _saveButton?.Focus();
            }
            else
            {
                // Default to first control
                MoveFocusToFirstControl();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving focus to next control: {ex.Message}");
        }
    }

    /// <summary>
    /// Moves focus to the first control in the form (Part AutoCompleteBox).
    /// </summary>
    private void MoveFocusToFirstControl()
    {
        try
        {
            _partAutoCompleteBox?.Focus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving focus to first control: {ex.Message}");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Finds the QuickButtonsView in the visual tree.
    /// </summary>
    private QuickButtonsView? FindQuickButtonsView()
    {
        try
        {
            // Start from the current control and walk up to find a parent that contains QuickButtonsView
            var current = this.Parent;
            while (current != null)
            {
                if (current is Panel panel)
                {
                    foreach (var child in panel.Children)
                    {
                        if (child is QuickButtonsView quickButtonsView)
                        {
                            return quickButtonsView;
                        }
                        
                        var found = FindQuickButtonsViewInChildren(child);
                        if (found != null)
                        {
                            return found;
                        }
                    }
                }
                current = current.Parent;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error finding QuickButtonsView: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Recursively searches for QuickButtonsView in child controls.
    /// </summary>
    private QuickButtonsView? FindQuickButtonsViewInChildren(Control control)
    {
        try
        {
            if (control is QuickButtonsView quickButtonsView)
            {
                return quickButtonsView;
            }
            
            if (control is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    var found = FindQuickButtonsViewInChildren(child);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Cleanup

    /// <summary>
    /// Cleans up event subscriptions when the view is being disposed.
    /// </summary>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        try
        {
            // Clean up event subscriptions
            KeyDown -= OnKeyDown;
            Loaded -= OnLoaded;
            
            // UI control event handlers
            if (_partAutoCompleteBox != null)
            {
                _partAutoCompleteBox.TextChanged -= OnPartTextChanged;
                _partAutoCompleteBox.SelectionChanged -= OnPartSelectionChanged;
                _partAutoCompleteBox.LostFocus -= OnPartLostFocus;
            }

            if (_operationAutoCompleteBox != null)
            {
                _operationAutoCompleteBox.TextChanged -= OnOperationTextChanged;
                _operationAutoCompleteBox.SelectionChanged -= OnOperationSelectionChanged;
                _operationAutoCompleteBox.LostFocus -= OnOperationLostFocus;
            }

            if (_locationAutoCompleteBox != null)
            {
                _locationAutoCompleteBox.TextChanged -= OnLocationTextChanged;
                _locationAutoCompleteBox.SelectionChanged -= OnLocationSelectionChanged;
                _locationAutoCompleteBox.LostFocus -= OnLocationLostFocus;
            }

            if (_quantityTextBox != null)
            {
                _quantityTextBox.TextChanged -= OnQuantityTextChanged;
                _quantityTextBox.LostFocus -= OnQuantityLostFocus;
            }

            if (_notesTextBox != null)
            {
                _notesTextBox.TextChanged -= OnNotesTextChanged;
            }

            if (_saveButton != null)
            {
                _saveButton.Click -= OnSaveButtonClick;
            }
            
            // ViewModel event handlers
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel.SaveCompleted -= OnInventorySaveCompleted;
            }
            
            if (_quickButtonsViewModel != null)
            {
                _quickButtonsViewModel.QuickActionExecuted -= OnQuickActionExecuted;
            }
            
            System.Diagnostics.Debug.WriteLine("InventoryTabView cleanup completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during InventoryTabView cleanup: {ex.Message}");
        }
        finally
        {
            base.OnDetachedFromVisualTree(e);
        }
    }

    #endregion
}
