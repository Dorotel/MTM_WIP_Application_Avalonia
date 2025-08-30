using System;
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
/// Provides keyboard shortcuts, focus management, and UI event handling.
/// Business logic and database operations are handled by the ViewModel.
/// Uses standard .NET patterns without ReactiveUI dependencies.
/// </summary>
public partial class InventoryTabView : UserControl
{
    private InventoryTabViewModel? _viewModel;
    private QuickButtonsViewModel? _quickButtonsViewModel;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ILogger<InventoryTabView>? _logger;

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
        _logger = _serviceProvider?.GetService<ILogger<InventoryTabView>>();
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

            _logger?.LogDebug("UI control references initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing control references");
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

            // AutoCompleteBox event handlers for UI interaction
            if (_partAutoCompleteBox != null)
            {
                _partAutoCompleteBox.TextChanged += OnPartTextChanged;
                _partAutoCompleteBox.SelectionChanged += OnPartSelectionChanged;
                _partAutoCompleteBox.LostFocus += OnPartLostFocus;
                _partAutoCompleteBox.KeyDown += OnControlKeyDown; // Prevent arrow key navigation
            }

            if (_operationAutoCompleteBox != null)
            {
                _operationAutoCompleteBox.TextChanged += OnOperationTextChanged;
                _operationAutoCompleteBox.SelectionChanged += OnOperationSelectionChanged;
                _operationAutoCompleteBox.LostFocus += OnOperationLostFocus;
                _operationAutoCompleteBox.KeyDown += OnControlKeyDown; // Prevent arrow key navigation
            }

            if (_locationAutoCompleteBox != null)
            {
                _locationAutoCompleteBox.TextChanged += OnLocationTextChanged;
                _locationAutoCompleteBox.SelectionChanged += OnLocationSelectionChanged;
                _locationAutoCompleteBox.LostFocus += OnLocationLostFocus;
                _locationAutoCompleteBox.KeyDown += OnControlKeyDown; // Prevent arrow key navigation
            }

            // TextBox event handlers
            if (_quantityTextBox != null)
            {
                _quantityTextBox.TextChanged += OnQuantityTextChanged;
                _quantityTextBox.LostFocus += OnQuantityLostFocus;
                _quantityTextBox.KeyDown += OnControlKeyDown; // Prevent arrow key navigation
            }

            if (_notesTextBox != null)
            {
                _notesTextBox.TextChanged += OnNotesTextChanged;
                _notesTextBox.KeyDown += OnControlKeyDown; // Prevent arrow key navigation
            }

            // Button event handlers
            if (_saveButton != null)
            {
                _saveButton.Click += OnSaveButtonClick;
            }

            _logger?.LogDebug("Event handlers set up successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting up event handlers");
            System.Diagnostics.Debug.WriteLine($"Error setting up event handlers: {ex.Message}");
        }
    }

    #endregion

    #region Validation and Save Button Management

    /// <summary>
    /// Validates all required fields and updates save button state.
    /// </summary>
    private void ValidateAndUpdateSaveButton()
    {
        try
        {
            if (_viewModel == null || _saveButton == null) return;

            bool canSave = IsFormValid();
            
            // Update ViewModel CanSave property if it exists
            var canSaveProperty = _viewModel.GetType().GetProperty("CanSave");
            if (canSaveProperty?.CanWrite == true)
            {
                canSaveProperty.SetValue(_viewModel, canSave);
            }
            
            // Directly update button state as fallback
            _saveButton.IsEnabled = canSave;

            _logger?.LogDebug("Save button validation completed - CanSave: {CanSave}", canSave);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error validating save button state");
        }
    }

    /// <summary>
    /// Checks if the form has valid data for all required fields.
    /// </summary>
    private bool IsFormValid()
    {
        try
        {
            if (_viewModel == null) return false;

            // Check Part ID
            var partId = _viewModel.SelectedPart?.Trim();
            if (string.IsNullOrEmpty(partId)) return false;

            // Check Operation
            var operation = _viewModel.SelectedOperation?.Trim();
            if (string.IsNullOrEmpty(operation)) return false;

            // Check Location
            var location = _viewModel.SelectedLocation?.Trim();
            if (string.IsNullOrEmpty(location)) return false;

            // Check Quantity
            var quantity = _viewModel.Quantity;
            if (quantity <= 0) return false;

            // Additional validation from ViewModel if available
            var isPartValid = GetPropertyValue<bool>(_viewModel, "IsPartValid", true);
            var isOperationValid = GetPropertyValue<bool>(_viewModel, "IsOperationValid", true);
            var isLocationValid = GetPropertyValue<bool>(_viewModel, "IsLocationValid", true);
            var isQuantityValid = GetPropertyValue<bool>(_viewModel, "IsQuantityValid", true);

            return isPartValid && isOperationValid && isLocationValid && isQuantityValid;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error checking form validity");
            return false;
        }
    }

    #endregion

    #region Arrow Key Navigation Prevention

    /// <summary>
    /// Handles key down events for individual controls to prevent arrow key tab navigation.
    /// </summary>
    private void OnControlKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            // Prevent arrow keys from causing tab navigation
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                // For AutoCompleteBox controls, allow arrow keys for dropdown navigation
                if (sender is AutoCompleteBox autoCompleteBox)
                {
                    // Only allow arrow keys if dropdown is open
                    if (autoCompleteBox.IsDropDownOpen)
                    {
                        return; // Allow normal arrow key behavior in dropdown
                    }
                    else
                    {
                        e.Handled = true; // Prevent tab navigation when dropdown is closed
                        return;
                    }
                }
                
                // For TextBox controls, prevent arrow key tab navigation
                if (sender is TextBox)
                {
                    e.Handled = true;
                    return;
                }
            }

            // Allow other keys to pass through normally
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling control key down");
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
                
                // Subscribe to ViewModel events
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                
                // Initialize form state and focus
                InitializeFormState();
                
                // Initial validation
                ValidateAndUpdateSaveButton();
                
                _logger?.LogInformation("InventoryTabView ViewModel connected successfully");
            }

            // Initialize QuickButtons integration if available
            await InitializeQuickButtonsIntegrationAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to initialize InventoryTabView");
            System.Diagnostics.Debug.WriteLine($"Error setting up InventoryTabView ViewModel: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes the form state and applies initial settings.
    /// </summary>
    private void InitializeFormState()
    {
        try
        {
            // Set initial focus to Part ID field
            MoveFocusToFirstControl();

            // Apply any saved user preferences
            ApplyUserPreferences();

            // Update control states based on ViewModel
            UpdateControlStates();

            _logger?.LogDebug("Form state initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing form state");
            System.Diagnostics.Debug.WriteLine($"Error initializing form state: {ex.Message}");
        }
    }

    /// <summary>
    /// Applies user preferences from application state service.
    /// </summary>
    private void ApplyUserPreferences()
    {
        try
        {
            if (_viewModel == null) return;

            // Apply any application state defaults from service
            if (_serviceProvider?.GetService<IApplicationStateService>() is IApplicationStateService appStateService)
            {
                // Set default operation if available and ViewModel property exists
                if (!string.IsNullOrEmpty(appStateService.CurrentOperation) && 
                    string.IsNullOrEmpty(_viewModel.SelectedOperation))
                {
                    _viewModel.SelectedOperation = appStateService.CurrentOperation;
                }

                // Set default location if available and ViewModel property exists
                if (!string.IsNullOrEmpty(appStateService.CurrentLocation) && 
                    string.IsNullOrEmpty(_viewModel.SelectedLocation))
                {
                    _viewModel.SelectedLocation = appStateService.CurrentLocation;
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error applying user preferences");
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

            // Update loading states if ViewModel has these properties
            UpdateLoadingStates();

            // Update error states
            UpdateValidationStates();

            // Update save button state
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating control states");
            System.Diagnostics.Debug.WriteLine($"Error updating control states: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates loading states on UI controls.
    /// </summary>
    private void UpdateLoadingStates()
    {
        try
        {
            if (_viewModel == null) return;

            // Only update if ViewModel has these properties
            var isLoadingParts = GetPropertyValue<bool>(_viewModel, "IsLoadingParts");
            var isLoadingOperations = GetPropertyValue<bool>(_viewModel, "IsLoadingOperations");
            var isLoadingLocations = GetPropertyValue<bool>(_viewModel, "IsLoadingLocations");

            if (_partAutoCompleteBox != null)
            {
                _partAutoCompleteBox.Classes.Set("loading", isLoadingParts);
            }

            if (_operationAutoCompleteBox != null)
            {
                _operationAutoCompleteBox.Classes.Set("loading", isLoadingOperations);
            }

            if (_locationAutoCompleteBox != null)
            {
                _locationAutoCompleteBox.Classes.Set("loading", isLoadingLocations);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating loading states");
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

            // Only update if ViewModel has validation properties
            var isPartValid = GetPropertyValue<bool>(_viewModel, "IsPartValid", true);
            var isOperationValid = GetPropertyValue<bool>(_viewModel, "IsOperationValid", true);
            var isLocationValid = GetPropertyValue<bool>(_viewModel, "IsLocationValid", true);
            var isQuantityValid = GetPropertyValue<bool>(_viewModel, "IsQuantityValid", true);

            if (_partAutoCompleteBox != null)
            {
                _partAutoCompleteBox.Classes.Set("error", !isPartValid);
            }

            if (_operationAutoCompleteBox != null)
            {
                _operationAutoCompleteBox.Classes.Set("error", !isOperationValid);
            }

            if (_locationAutoCompleteBox != null)
            {
                _locationAutoCompleteBox.Classes.Set("error", !isLocationValid);
            }

            if (_quantityTextBox != null)
            {
                _quantityTextBox.Classes.Set("error", !isQuantityValid);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating validation states");
        }
    }

    /// <summary>
    /// Helper method to safely get property values using reflection.
    /// </summary>
    private T GetPropertyValue<T>(object obj, string propertyName, T defaultValue = default!)
    {
        try
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null && property.CanRead && property.PropertyType == typeof(T))
            {
                var value = property.GetValue(obj);
                return value is T result ? result : defaultValue;
            }
        }
        catch
        {
            // Return default value if property doesn't exist or can't be read
        }
        return defaultValue;
    }

    #endregion

    #region UI Event Handlers - Part ID

    /// <summary>
    /// Handles Part ID text changes.
    /// </summary>
    private void OnPartTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var partId = autoCompleteBox.Text?.Trim() ?? string.Empty;

            // Update ViewModel property
            _viewModel.SelectedPart = partId;

            // Update control styling and save button
            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling part text change");
            System.Diagnostics.Debug.WriteLine($"Error handling part text change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Part ID selection from dropdown.
    /// </summary>
    private void OnPartSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var selectedPart = autoCompleteBox.SelectedItem?.ToString() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(selectedPart))
            {
                _viewModel.SelectedPart = selectedPart;
                
                // Update validation and save button
                UpdateValidationStates();
                ValidateAndUpdateSaveButton();
                
                // Move to next field automatically
                MoveFocusToNextControl();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling part selection");
            System.Diagnostics.Debug.WriteLine($"Error handling part selection: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Part ID field losing focus.
    /// </summary>
    private void OnPartLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var partId = autoCompleteBox.Text?.Trim() ?? string.Empty;
            _viewModel.SelectedPart = partId;

            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling part lost focus");
            System.Diagnostics.Debug.WriteLine($"Error handling part lost focus: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Operation

    /// <summary>
    /// Handles Operation text changes.
    /// </summary>
    private void OnOperationTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var operation = autoCompleteBox.Text?.Trim() ?? string.Empty;
            _viewModel.SelectedOperation = operation;

            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling operation text change");
            System.Diagnostics.Debug.WriteLine($"Error handling operation text change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Operation selection from dropdown.
    /// </summary>
    private void OnOperationSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var selectedOperation = autoCompleteBox.SelectedItem?.ToString() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(selectedOperation))
            {
                _viewModel.SelectedOperation = selectedOperation;
                
                // Update validation and save button
                UpdateValidationStates();
                ValidateAndUpdateSaveButton();
                
                // Move to next field automatically
                MoveFocusToNextControl();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling operation selection");
            System.Diagnostics.Debug.WriteLine($"Error handling operation selection: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Operation field losing focus.
    /// </summary>
    private void OnOperationLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var operation = autoCompleteBox.Text?.Trim() ?? string.Empty;
            _viewModel.SelectedOperation = operation;

            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling operation lost focus");
            System.Diagnostics.Debug.WriteLine($"Error handling operation lost focus: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Location

    /// <summary>
    /// Handles Location text changes.
    /// </summary>
    private void OnLocationTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var location = autoCompleteBox.Text?.Trim() ?? string.Empty;
            _viewModel.SelectedLocation = location;

            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling location text change");
            System.Diagnostics.Debug.WriteLine($"Error handling location text change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Location selection from dropdown.
    /// </summary>
    private void OnLocationSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var selectedLocation = autoCompleteBox.SelectedItem?.ToString() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(selectedLocation))
            {
                _viewModel.SelectedLocation = selectedLocation;
                
                // Update validation and save button
                UpdateValidationStates();
                ValidateAndUpdateSaveButton();
                
                // Move to next field automatically
                MoveFocusToNextControl();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling location selection");
            System.Diagnostics.Debug.WriteLine($"Error handling location selection: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Location field losing focus.
    /// </summary>
    private void OnLocationLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not AutoCompleteBox autoCompleteBox) return;

            var location = autoCompleteBox.Text?.Trim() ?? string.Empty;
            _viewModel.SelectedLocation = location;

            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling location lost focus");
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
                // Invalid quantity - let ViewModel handle validation
            }

            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling quantity text change");
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
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling quantity lost focus");
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
            
            // Notes don't affect save button validation, but update states
            UpdateValidationStates();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling notes text change");
            System.Diagnostics.Debug.WriteLine($"Error handling notes text change: {ex.Message}");
        }
    }

    #endregion

    #region UI Event Handlers - Buttons

    /// <summary>
    /// Handles Save button click by executing the ViewModel's save command.
    /// </summary>
    private void OnSaveButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null) return;

            // Double-check validation before saving
            if (!IsFormValid())
            {
                _logger?.LogWarning("Save attempted with invalid form data");
                return;
            }

            // Execute save command (business logic handled by ViewModel)
            var saveCommand = GetPropertyValue<System.Windows.Input.ICommand>(_viewModel, "SaveCommand");
            if (saveCommand?.CanExecute(null) == true)
            {
                saveCommand.Execute(null);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling save button click");
            System.Diagnostics.Debug.WriteLine($"Error handling save button click: {ex.Message}");
        }
    }

    #endregion

    #region QuickButtons Integration

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
                
                // Subscribe to quick action executed events if they exist
                var quickActionEvent = _quickButtonsViewModel.GetType().GetEvent("QuickActionExecuted");
                if (quickActionEvent != null)
                {
                    // Use reflection to subscribe to the event
                    var handler = new EventHandler<object>((sender, args) => OnQuickActionExecuted(sender, args));
                    quickActionEvent.AddEventHandler(_quickButtonsViewModel, handler);
                }
                
                _logger?.LogInformation("QuickButtons integration initialized successfully");
            }
            else
            {
                _logger?.LogDebug("QuickButtonsView not found in visual tree - integration skipped");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing QuickButtons integration");
            System.Diagnostics.Debug.WriteLine($"Error initializing QuickButtons integration: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles quick action executed events from QuickButtonsView.
    /// </summary>
    private void OnQuickActionExecuted(object? sender, object e)
    {
        try
        {
            if (_viewModel == null) return;

            // Use reflection to get properties from the event args
            var partId = GetPropertyValue<string>(e, "PartId");
            var operation = GetPropertyValue<string>(e, "Operation");
            var quantity = GetPropertyValue<int>(e, "Quantity");

            if (!string.IsNullOrEmpty(partId))
            {
                _logger?.LogInformation("Quick action applied: PartId={PartId}, Operation={Operation}, Quantity={Quantity}", 
                    partId, operation, quantity);

                // Populate form fields with quick action data
                _viewModel.SelectedPart = partId;
                _viewModel.SelectedOperation = operation;
                _viewModel.Quantity = quantity;
                
                // Clear previous error state if ViewModel has error properties
                var hasErrorProperty = _viewModel.GetType().GetProperty("HasError");
                var errorMessageProperty = _viewModel.GetType().GetProperty("ErrorMessage");
                
                if (hasErrorProperty?.CanWrite == true)
                {
                    hasErrorProperty.SetValue(_viewModel, false);
                }
                
                if (errorMessageProperty?.CanWrite == true)
                {
                    errorMessageProperty.SetValue(_viewModel, string.Empty);
                }
                
                // Update UI control states and save button
                UpdateControlStates();
                ValidateAndUpdateSaveButton();
                
                // Focus the location field (likely next field to fill)
                _locationAutoCompleteBox?.Focus();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling quick action");
            System.Diagnostics.Debug.WriteLine($"Error handling quick action: {ex.Message}");
        }
    }

    #endregion

    #region ViewModel Property Change Handling

    /// <summary>
    /// Handles ViewModel property changes to update UI state.
    /// </summary>
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (_viewModel == null) return;

            switch (e.PropertyName)
            {
                case "SelectedPart":
                case "SelectedOperation":
                case "SelectedLocation":
                case "Quantity":
                case "Notes":
                    UpdateValidationStates();
                    ValidateAndUpdateSaveButton(); // Update save button on property changes
                    break;
                    
                case "IsLoading":
                case "IsLoadingParts":
                case "IsLoadingOperations":
                case "IsLoadingLocations":
                    UpdateLoadingStates();
                    break;

                case "HasError":
                case "ErrorMessage":
                case "IsPartValid":
                case "IsOperationValid":
                case "IsLocationValid":
                case "IsQuantityValid":
                    UpdateValidationStates();
                    ValidateAndUpdateSaveButton(); // Validation changes affect save button
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling property change");
            System.Diagnostics.Debug.WriteLine($"Error handling property change: {ex.Message}");
        }
    }

    #endregion

    #region Keyboard Shortcuts and Navigation

    /// <summary>
    /// Handles keyboard shortcuts for enhanced user experience.
    /// F5: Reset form, Shift+F5: Hard reset, Enter: Next field/Save, Escape: Clear errors
    /// </summary>
    private void OnKeyDown(object? sender, KeyEventArgs e)
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
                        ExecuteHardReset();
                    }
                    else
                    {
                        // F5: Soft reset
                        ExecuteSoftReset();
                    }
                    break;

                case Key.Enter:
                    e.Handled = true;
                    HandleEnterKey(e.Source);
                    break;

                case Key.Escape:
                    e.Handled = true;
                    // Clear any error state and focus the first control
                    ClearErrorState();
                    MoveFocusToFirstControl();
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling keyboard shortcut");
            System.Diagnostics.Debug.WriteLine($"Error handling keyboard shortcut: {ex.Message}");
        }
    }

    /// <summary>
    /// Executes a soft reset of the form.
    /// </summary>
    private void ExecuteSoftReset()
    {
        try
        {
            var resetCommand = GetPropertyValue<System.Windows.Input.ICommand>(_viewModel, "ResetCommand");
            if (resetCommand?.CanExecute(null) == true)
            {
                resetCommand.Execute(null);
                MoveFocusToFirstControl();
                ValidateAndUpdateSaveButton(); // Update save button after reset
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during soft reset");
        }
    }

    /// <summary>
    /// Executes a hard reset including database refresh.
    /// </summary>
    private void ExecuteHardReset()
    {
        try
        {
            // Execute reset command
            ExecuteSoftReset();

            // Refresh data command
            var refreshCommand = GetPropertyValue<System.Windows.Input.ICommand>(_viewModel, "RefreshDataCommand");
            if (refreshCommand?.CanExecute(null) == true)
            {
                refreshCommand.Execute(null);
            }

            // Refresh QuickButtons if available
            var refreshButtonsCommand = GetPropertyValue<System.Windows.Input.ICommand>(_quickButtonsViewModel, "RefreshButtonsCommand");
            if (refreshButtonsCommand?.CanExecute(null) == true)
            {
                refreshButtonsCommand.Execute(null);
            }

            MoveFocusToFirstControl();
            ValidateAndUpdateSaveButton(); // Update save button after hard reset
            _logger?.LogDebug("Hard reset completed successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during hard reset");
        }
    }

    /// <summary>
    /// Clears error state from the ViewModel.
    /// </summary>
    private void ClearErrorState()
    {
        try
        {
            var hasErrorProperty = _viewModel?.GetType().GetProperty("HasError");
            var errorMessageProperty = _viewModel?.GetType().GetProperty("ErrorMessage");
            
            if (hasErrorProperty?.CanWrite == true)
            {
                hasErrorProperty.SetValue(_viewModel, false);
            }
            
            if (errorMessageProperty?.CanWrite == true)
            {
                errorMessageProperty.SetValue(_viewModel, string.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error clearing error state");
        }
    }

    /// <summary>
    /// Handles Enter key navigation - moves to next logical control or executes save.
    /// </summary>
    private void HandleEnterKey(object? source)
    {
        if (_viewModel == null) return;

        try
        {
            // If focused on Save button and can save, execute save command
            if (source == _saveButton && IsFormValid())
            {
                var saveCommand = GetPropertyValue<System.Windows.Input.ICommand>(_viewModel, "SaveCommand");
                if (saveCommand?.CanExecute(null) == true)
                {
                    saveCommand.Execute(null);
                    return;
                }
            }

            // Otherwise, move focus to next control
            MoveFocusToNextControl();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Enter key");
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
            _logger?.LogError(ex, "Error moving focus to next control");
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
            _logger?.LogError(ex, "Error moving focus to first control");
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
                _partAutoCompleteBox.KeyDown -= OnControlKeyDown;
            }

            if (_operationAutoCompleteBox != null)
            {
                _operationAutoCompleteBox.TextChanged -= OnOperationTextChanged;
                _operationAutoCompleteBox.SelectionChanged -= OnOperationSelectionChanged;
                _operationAutoCompleteBox.LostFocus -= OnOperationLostFocus;
                _operationAutoCompleteBox.KeyDown -= OnControlKeyDown;
            }

            if (_locationAutoCompleteBox != null)
            {
                _locationAutoCompleteBox.TextChanged -= OnLocationTextChanged;
                _locationAutoCompleteBox.SelectionChanged -= OnLocationSelectionChanged;
                _locationAutoCompleteBox.LostFocus -= OnLocationLostFocus;
                _locationAutoCompleteBox.KeyDown -= OnControlKeyDown;
            }

            if (_quantityTextBox != null)
            {
                _quantityTextBox.TextChanged -= OnQuantityTextChanged;
                _quantityTextBox.LostFocus -= OnQuantityLostFocus;
                _quantityTextBox.KeyDown -= OnControlKeyDown;
            }

            if (_notesTextBox != null)
            {
                _notesTextBox.TextChanged -= OnNotesTextChanged;
                _notesTextBox.KeyDown -= OnControlKeyDown;
            }

            if (_saveButton != null)
            {
                _saveButton.Click -= OnSaveButtonClick;
            }
            
            // ViewModel event handlers
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
            
            // QuickButtons event handlers would be unsubscribed here using reflection if needed
            
            _logger?.LogDebug("InventoryTabView cleanup completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during InventoryTabView cleanup");
            System.Diagnostics.Debug.WriteLine($"Error during InventoryTabView cleanup: {ex.Message}");
        }
        finally
        {
            base.OnDetachedFromVisualTree(e);
        }
    }

    #endregion
}
