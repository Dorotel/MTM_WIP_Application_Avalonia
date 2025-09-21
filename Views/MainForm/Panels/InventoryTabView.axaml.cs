using MTM_WIP_Application_Avalonia.Models.Events;
using MTM_WIP_Application_Avalonia.Services.UI;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Models;
using System.Diagnostics;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for InventoryTabView.
/// Implements the primary inventory management interface within the MTM WIP Application.
/// Provides keyboard shortcuts, focus management, and UI event handling.
/// Business logic and database operations are handled by the ViewModel.
/// Uses standard .NET patterns without ReactiveUI dependencies.
/// Arrow key navigation is handled globally by MainWindow.
/// </summary>
public partial class InventoryTabView : UserControl
{
    private InventoryTabViewModel? _viewModel;
    private QuickButtonsViewModel? _quickButtonsViewModel;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ILogger<InventoryTabView>? _logger;
    private ISuggestionOverlayService? _suggestionOverlayService;
    private ISuccessOverlayService? _successOverlayService;

    // Flag to prevent cascading suggestion overlays
    private bool _isShowingSuggestionOverlay = false;

    // UI Control references for direct manipulation
    private TextBox? _partTextBox;
    private TextBox? _operationTextBox;
    private TextBox? _locationTextBox;
    private TextBox? _quantityTextBox;
    private TextBox? _notesTextBox;
    private Button? _saveButton;


    public InventoryTabView()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during InitializeComponent: {ex.Message}");
        }

        InitializeControlReferences();
        SetupEventHandlers();

        Loaded += OnLoaded;
    }

    public InventoryTabView(IServiceProvider serviceProvider) : this()
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider?.GetService<ILogger<InventoryTabView>>();

        // Try to resolve the suggestion overlay service immediately if we have a service provider
        try
        {
            _suggestionOverlayService = _serviceProvider?.GetService<ISuggestionOverlayService>();
            _successOverlayService = _serviceProvider?.GetService<ISuccessOverlayService>();
            _logger?.LogInformation("SuccessOverlayService resolved in constructor: {ServiceResolved}", _successOverlayService != null);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to resolve overlay services in constructor");
        }
    }

    #region Control Initialization

    /// <summary>
    /// Initializes references to UI controls for direct manipulation.
    /// </summary>
    private void InitializeControlReferences()
    {
        try
        {
            _partTextBox = this.FindControl<TextBox>("PartTextBox");
            _operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            _locationTextBox = this.FindControl<TextBox>("LocationTextBox");
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

    private void SetupEventHandlers()
    {
        try
        {
            if (_saveButton != null)
                _saveButton.Click += OnSaveButtonClick;

            KeyDown += OnKeyDown;

            // Attach TextChanged and LostFocus handlers for TextBox controls
            if (_partTextBox != null)
            {
                _partTextBox.LostFocus += OnPartLostFocus;
                _partTextBox.TextChanged += OnPartTextChanged;
            }
            if (_operationTextBox != null)
            {
                _operationTextBox.LostFocus += OnOperationLostFocus;
                _operationTextBox.TextChanged += OnOperationTextChanged;
            }
            if (_locationTextBox != null)
            {
                _locationTextBox.LostFocus += OnLocationLostFocus;
                _locationTextBox.TextChanged += OnLocationTextChanged;
            }
            if (_quantityTextBox != null)
            {
                _quantityTextBox.LostFocus += OnQuantityLostFocus;
                _quantityTextBox.TextChanged += OnQuantityTextChanged;
            }
            if (_notesTextBox != null)
            {
                _notesTextBox.TextChanged += OnNotesTextChanged;
            }

            _logger?.LogDebug("Event handlers set up successfully - arrow key handling delegated to MainWindow");
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

    #region ViewModel Integration

    /// <summary>
    /// Handles the Loaded event to set up the ViewModel via dependency injection.
    /// </summary>
    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {

            // Try to resolve services if not already resolved
            if (_suggestionOverlayService == null || _successOverlayService == null)
            {
                TryResolveServices(); // Remove await since method is no longer async
            }

            // Set up ViewModel if not already set
            if (DataContext is InventoryTabViewModel vm)
            {
                _viewModel = vm;

                // Subscribe to ViewModel events
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;

                // Subscribe to success overlay event
                _viewModel.ShowSuccessOverlay += OnShowSuccessOverlay;

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
    /// Attempts to resolve required services from various sources.
    /// </summary>
    private void TryResolveServices()
    {
        try
        {
            // Method 1: Try the injected service provider
            if (_serviceProvider != null && _suggestionOverlayService == null)
            {
                try
                {
                    if (_suggestionOverlayService == null)
                    {
                        _suggestionOverlayService = _serviceProvider.GetService<ISuggestionOverlayService>();
                        _logger?.LogDebug("Method 1 - SuggestionOverlayService resolution: {ServiceResolved}", _suggestionOverlayService != null);
                        System.Diagnostics.Debug.WriteLine($"Method 1 - SuggestionOverlayService resolution: {_suggestionOverlayService != null}");
                    }

                    if (_successOverlayService == null)
                    {
                        _successOverlayService = _serviceProvider.GetService<ISuccessOverlayService>();
                        // Method 1 - SuccessOverlayService resolution successful
                        System.Diagnostics.Debug.WriteLine($"Method 1 - SuccessOverlayService resolution: {_successOverlayService != null}");
                    }

                    // Additional debugging - check if services are registered
                    var suggestionServices = _serviceProvider.GetServices<ISuggestionOverlayService>();
                    var suggestionServiceCount = suggestionServices?.Count() ?? 0;
                    _logger?.LogDebug("ISuggestionOverlayService instances in container: {ServiceCount}", suggestionServiceCount);
                    System.Diagnostics.Debug.WriteLine($"ISuggestionOverlayService instances in container: {suggestionServiceCount}");

                    var successServices = _serviceProvider.GetServices<ISuccessOverlayService>();
                    var successServiceCount = successServices?.Count() ?? 0;
                    // Success overlay service instances count check
                    System.Diagnostics.Debug.WriteLine($"ISuccessOverlayService instances in container: {successServiceCount}");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to resolve overlay services from service provider");
                    System.Diagnostics.Debug.WriteLine($"Failed to resolve overlay services: {ex.Message}");
                }
            }

            // Method 2: Try to get from MainWindow if it has a service provider
            if (_suggestionOverlayService == null || _successOverlayService == null)
            {
                try
                {
                    var mainWindow = TopLevel.GetTopLevel(this);
                    if (mainWindow?.DataContext != null)
                    {
                        var serviceProviderProperty = mainWindow.DataContext.GetType().GetProperty("ServiceProvider");
                        if (serviceProviderProperty?.GetValue(mainWindow.DataContext) is IServiceProvider windowServiceProvider)
                        {
                            if (_suggestionOverlayService == null)
                            {
                                _suggestionOverlayService = windowServiceProvider.GetService<ISuggestionOverlayService>();
                                _logger?.LogDebug("Method 2 - MainWindow SuggestionOverlay resolution: {ServiceResolved}", _suggestionOverlayService != null);
                                System.Diagnostics.Debug.WriteLine($"Method 2 - MainWindow SuggestionOverlay resolution: {_suggestionOverlayService != null}");
                            }

                            if (_successOverlayService == null)
                            {
                                _successOverlayService = windowServiceProvider.GetService<ISuccessOverlayService>();
                                // Method 2 - MainWindow SuccessOverlay resolution successful
                                System.Diagnostics.Debug.WriteLine($"Method 2 - MainWindow SuccessOverlay resolution: {_successOverlayService != null}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to resolve overlay services from MainWindow");
                    System.Diagnostics.Debug.WriteLine($"Failed to resolve overlay services from MainWindow: {ex.Message}");
                }
            }

            // Method 3: Try to create instance manually as fallback
            if (_suggestionOverlayService == null || _successOverlayService == null)
            {
                try
                {
                    var loggerFactory = _serviceProvider?.GetService<ILoggerFactory>();
                    if (loggerFactory != null)
                    {
                        if (_suggestionOverlayService == null)
                        {
                            var suggestionServiceLogger = loggerFactory.CreateLogger<SuggestionOverlayService>();
                            _suggestionOverlayService = new SuggestionOverlayService(suggestionServiceLogger);
                            _logger?.LogWarning("Method 3 - Manual SuggestionOverlayService creation successful as fallback");
                            System.Diagnostics.Debug.WriteLine("Method 3 - Manual SuggestionOverlayService creation successful as fallback");
                        }

                        if (_successOverlayService == null)
                        {
                            var successServiceLogger = loggerFactory.CreateLogger<SuccessOverlayService>();
                            _successOverlayService = new SuccessOverlayService(successServiceLogger);
                            _logger?.LogWarning("Method 3 - Manual SuccessOverlayService creation successful as fallback");
                            System.Diagnostics.Debug.WriteLine("Method 3 - Manual SuccessOverlayService creation successful as fallback");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to create overlay services manually");
                    System.Diagnostics.Debug.WriteLine($"Failed to create overlay services manually: {ex.Message}");
                }
            }

            // Log final result with detailed information
            if (_suggestionOverlayService != null && _successOverlayService != null)
            {
                _logger?.LogInformation("Both overlay services successfully resolved - SuggestionType: {SuggestionType}, SuccessType: {SuccessType}",
                    _suggestionOverlayService.GetType().Name, _successOverlayService.GetType().Name);
                System.Diagnostics.Debug.WriteLine($"Both overlay services successfully resolved - SuggestionType: {_suggestionOverlayService.GetType().Name}, SuccessType: {_successOverlayService.GetType().Name}");
            }
            else
            {
                _logger?.LogError("One or more overlay services could not be resolved - Suggestion: {SuggestionResolved}, Success: {SuccessResolved}",
                    _suggestionOverlayService != null, _successOverlayService != null);
                System.Diagnostics.Debug.WriteLine($"One or more overlay services could not be resolved - Suggestion: {_suggestionOverlayService != null}, Success: {_successOverlayService != null}");

                // Additional diagnostic information
                if (_serviceProvider != null)
                {
                    try
                    {
                        var registeredServices = _serviceProvider.GetServices<object>().Count();
                        _logger?.LogDebug("Service provider contains {ServiceCount} registered services", registeredServices);
                        System.Diagnostics.Debug.WriteLine($"Service provider contains {registeredServices} registered services");

                        // Check if the specific services are registered
                        var suggestionsServices = _serviceProvider.GetServices<ISuggestionOverlayService>();
                        var suggestionServiceCount = suggestionsServices?.Count() ?? 0;
                        _logger?.LogDebug("ISuggestionOverlayService registrations found: {Count}", suggestionServiceCount);
                        System.Diagnostics.Debug.WriteLine($"ISuggestionOverlayService registrations found: {suggestionServiceCount}");

                        var successServices = _serviceProvider.GetServices<ISuccessOverlayService>();
                        var successServiceCount = successServices?.Count() ?? 0;
                        // ISuccessOverlayService registrations found
                        System.Diagnostics.Debug.WriteLine($"ISuccessOverlayService registrations found: {successServiceCount}");
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error checking service registrations");
                        System.Diagnostics.Debug.WriteLine($"Error checking service registrations: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error trying to resolve services");
            System.Diagnostics.Debug.WriteLine($"Error trying to resolve services: {ex.Message}");
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


            // Apply any saved user preferences
            ApplyUserPreferences();

            // Update control states based on ViewModel
            UpdateControlStates();
            MoveFocusToFirstControl();
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

            if (_partTextBox != null)
            {
                _partTextBox.Classes.Set("loading", isLoadingParts);
            }

            if (_operationTextBox != null)
            {
                _operationTextBox.Classes.Set("loading", isLoadingOperations);
            }

            if (_locationTextBox != null)
            {
                _locationTextBox.Classes.Set("loading", isLoadingLocations);
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

            if (_partTextBox != null)
            {
                _partTextBox.Classes.Set("error", !isPartValid);
            }

            if (_operationTextBox != null)
            {
                _operationTextBox.Classes.Set("error", !isOperationValid);
            }

            if (_locationTextBox != null)
            {
                _locationTextBox.Classes.Set("error", !isLocationValid);
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
            if (_viewModel == null || sender is not TextBox textBox) return;

            var partId = textBox.Text?.Trim() ?? string.Empty;

            // Update ViewModel property
            _viewModel.SelectedPart = partId;

            // Update control styling and save button
            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling part text change");
            _ = Services.Core.ErrorHandling.HandleErrorAsync(ex, "InventoryTabView_OnPartTextChanged", "System");
            System.Diagnostics.Debug.WriteLine($"Error handling part text change: {ex.Message}");
        }
    }


    private async void OnPartLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not TextBox textBox) return;

            // Ensure services are resolved before proceeding
            TryResolveServices();

            var value = textBox.Text?.Trim() ?? string.Empty;
            // Use PartIds collection which is populated from the PartID column via stored procedure
            var data = _viewModel.PartIds ?? Enumerable.Empty<string>();
            int count = data.Count();

            _logger?.LogInformation($"[FocusLost] PartTextBox lost focus. User entered: '{value}'. PartIds count: {count}");
            System.Diagnostics.Debug.WriteLine($"[FocusLost] PartTextBox lost focus. User entered: '{value}'. PartIds count: {count}");

            // Check if no data is available from server
            if (count == 0)
            {
                _logger?.LogWarning("No Part IDs available - likely database connectivity issue");
                System.Diagnostics.Debug.WriteLine("No Part IDs available - likely database connectivity issue");

                // Clear the textbox and show error message about data unavailability
                textBox.Text = string.Empty;
                _viewModel.SelectedPart = string.Empty;

                UpdateValidationStates();
                ValidateAndUpdateSaveButton();
                return;
            }

            // Debug: Log sample of available part IDs
            if (count > 0)
            {
                var sampleParts = string.Join(", ", data.Take(10));
                _logger?.LogInformation($"Sample PartIDs: {sampleParts}");
                System.Diagnostics.Debug.WriteLine($"Sample PartIDs: {sampleParts}");
            }

            // Show overlay only if:
            // 1. User entered something (not blank)
            // 2. Entered value is NOT an exact match to any PartID
            // 3. Entered value IS a substring of one or more PartIDs (semi-matches)
            var semiMatches = data
                .Where(partId => !string.IsNullOrEmpty(partId) &&
                               partId.Contains(value, StringComparison.OrdinalIgnoreCase))
                .OrderBy(partId => partId) // Sort suggestions alphabetically
                .ToList();

            bool isExactMatch = data.Any(partId =>
                string.Equals(partId, value, StringComparison.OrdinalIgnoreCase));

            _logger?.LogInformation($"Part '{value}' - ExactMatch: {isExactMatch}, SemiMatches: {semiMatches.Count}");
            System.Diagnostics.Debug.WriteLine($"Part '{value}' - ExactMatch: {isExactMatch}, SemiMatches: {semiMatches.Count}");

            if (!string.IsNullOrEmpty(value) &&
                !isExactMatch &&
                semiMatches.Count > 0 &&
                _suggestionOverlayService != null &&
                !_isShowingSuggestionOverlay)
            {
                _logger?.LogInformation($"Invoking overlay for Part: '{value}' with {semiMatches.Count} suggestions");
                System.Diagnostics.Debug.WriteLine($"Invoking overlay for Part: '{value}' with {semiMatches.Count} suggestions");

                // Log first few suggestions for debugging
                var firstFewSuggestions = string.Join(", ", semiMatches.Take(5));
                _logger?.LogInformation($"First 5 suggestions: {firstFewSuggestions}");
                System.Diagnostics.Debug.WriteLine($"First 5 suggestions: {firstFewSuggestions}");

                try
                {
                    _isShowingSuggestionOverlay = true;
                    var selected = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, semiMatches, value);

                    if (!string.IsNullOrEmpty(selected))
                    {
                        _logger?.LogInformation($"Part overlay - User selected: '{selected}'");
                        System.Diagnostics.Debug.WriteLine($"Part overlay - User selected: '{selected}'");
                        _viewModel.SelectedPart = selected;
                        textBox.Text = selected;
                    }
                    else
                    {
                        _logger?.LogInformation($"Part overlay - User cancelled, keeping: '{value}'");
                        System.Diagnostics.Debug.WriteLine($"Part overlay - User cancelled, keeping: '{value}'");
                        _viewModel.SelectedPart = value;
                    }
                }
                finally
                {
                    _isShowingSuggestionOverlay = false;
                }
            }
            else
            {
                // Log why overlay was not shown
                if (string.IsNullOrEmpty(value))
                {
                    _logger?.LogInformation($"Part overlay not shown - value is empty");
                    System.Diagnostics.Debug.WriteLine($"Part overlay not shown - value is empty");
                }
                else if (isExactMatch)
                {
                    _logger?.LogInformation($"Part overlay not shown - '{value}' is exact match");
                    System.Diagnostics.Debug.WriteLine($"Part overlay not shown - '{value}' is exact match");
                }
                else if (semiMatches.Count == 0)
                {
                    _logger?.LogInformation($"Part overlay not shown - no semi-matches for '{value}'");
                    System.Diagnostics.Debug.WriteLine($"Part overlay not shown - no semi-matches for '{value}'");

                    // MTM Pattern: Clear textbox when no matches found to maintain data integrity
                    _logger?.LogInformation($"Part '{value}' has no matches in validation source. Clearing textbox for data integrity.");
                    System.Diagnostics.Debug.WriteLine($"Part '{value}' has no matches in validation source. Clearing textbox for data integrity.");

                    textBox.Text = string.Empty;
                    _viewModel.SelectedPart = string.Empty;

                    // Show user feedback about the clearing action
                    try
                    {
                        await Services.Core.ErrorHandling.HandleErrorAsync(
                            new ArgumentException($"Invalid Part ID: '{value}' not found in available parts."),
                            "Part ID validation failed - input cleared",
                            "System"
                        );
                    }
                    catch (Exception errorEx)
                    {
                        _logger?.LogWarning(errorEx, "Failed to show error message for invalid Part ID");
                        System.Diagnostics.Debug.WriteLine($"Failed to show error message for invalid Part ID: {errorEx.Message}");
                    }
                }
                else if (_suggestionOverlayService == null)
                {
                    _logger?.LogWarning($"Part overlay not shown - SuggestionOverlayService is null");
                    System.Diagnostics.Debug.WriteLine($"Part overlay not shown - SuggestionOverlayService is null");
                }

                _viewModel.SelectedPart = value;
            }

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
            if (_viewModel == null || sender is not TextBox textBox) return;

            var operation = textBox.Text?.Trim() ?? string.Empty;
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
    /// Handles Operation field losing focus.
    /// </summary>
    private async void OnOperationLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not TextBox textBox) return;

            // Ensure services are resolved before proceeding
            TryResolveServices();

            var value = textBox.Text?.Trim() ?? string.Empty;
            var data = _viewModel.Operations ?? Enumerable.Empty<string>();
            int count = data.Count();
            _logger?.LogInformation($"[FocusLost] OperationTextBox lost focus. User entered: '{value}'. Operations count: {count}");

            // Check if no data is available from server
            if (count == 0)
            {
                _logger?.LogWarning("No Operations available - likely database connectivity issue");
                System.Diagnostics.Debug.WriteLine("No Operations available - likely database connectivity issue");

                // Clear the textbox and show error message about data unavailability
                textBox.Text = string.Empty;
                _viewModel.SelectedOperation = string.Empty;

                UpdateValidationStates();
                ValidateAndUpdateSaveButton();
                return;
            }

            // Only show suggestions if the user actually entered something
            if (!string.IsNullOrEmpty(value) &&
                !data.Contains(value, StringComparer.OrdinalIgnoreCase) &&
                _suggestionOverlayService != null &&
                !_isShowingSuggestionOverlay)
            {
                // Check if the value has any partial matches in the data
                var hasPartialMatches = data.Any(op =>
                    op.Contains(value, StringComparison.OrdinalIgnoreCase));

                if (hasPartialMatches)
                {
                    try
                    {
                        _isShowingSuggestionOverlay = true;
                        var selected = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, data, value);
                        if (!string.IsNullOrEmpty(selected))
                        {
                            _viewModel.SelectedOperation = selected;
                            textBox.Text = selected;
                        }
                        else
                        {
                            _viewModel.SelectedOperation = value;
                        }
                    }
                    finally
                    {
                        _isShowingSuggestionOverlay = false;
                    }
                }
                else
                {
                    // MTM Pattern: Clear textbox when no matches found to maintain data integrity
                    _logger?.LogInformation($"Operation '{value}' has no matches in validation source. Clearing textbox for data integrity.");
                    System.Diagnostics.Debug.WriteLine($"Operation '{value}' has no matches in validation source. Clearing textbox for data integrity.");

                    textBox.Text = string.Empty;
                    _viewModel.SelectedOperation = string.Empty;

                    // Show user feedback about the clearing action
                    try
                    {
                        await Services.Core.ErrorHandling.HandleErrorAsync(
                            new ArgumentException($"Invalid Operation: '{value}' not found in available operations."),
                            "Operation validation failed - input cleared",
                            "System"
                        );
                    }
                    catch (Exception errorEx)
                    {
                        _logger?.LogWarning(errorEx, "Failed to show error message for invalid Operation");
                        System.Diagnostics.Debug.WriteLine($"Failed to show error message for invalid Operation: {errorEx.Message}");
                    }
                }
            }
            else
            {
                _viewModel.SelectedOperation = value;
            }
            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling operation lost focus");
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
            if (_viewModel == null || sender is not TextBox textBox) return;

            var location = textBox.Text?.Trim() ?? string.Empty;
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
    /// Handles Location field losing focus.
    /// </summary>
    private async void OnLocationLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_viewModel == null || sender is not TextBox textBox) return;

            // Ensure services are resolved before proceeding
            TryResolveServices();

            var value = textBox.Text?.Trim() ?? string.Empty;
            var data = _viewModel.Locations ?? Enumerable.Empty<string>();
            int count = data.Count();
            _logger?.LogInformation($"[FocusLost] LocationTextBox lost focus. User entered: '{value}'. Locations count: {count}");

            // Check if no data is available from server
            if (count == 0)
            {
                _logger?.LogWarning("No Locations available - likely database connectivity issue");
                System.Diagnostics.Debug.WriteLine("No Locations available - likely database connectivity issue");

                // Clear the textbox and show error message about data unavailability
                textBox.Text = string.Empty;
                _viewModel.SelectedLocation = string.Empty;

                UpdateValidationStates();
                ValidateAndUpdateSaveButton();
                return;
            }

            // Only show suggestions if the user actually entered something
            if (!string.IsNullOrEmpty(value) &&
                !data.Contains(value, StringComparer.OrdinalIgnoreCase) &&
                _suggestionOverlayService != null &&
                !_isShowingSuggestionOverlay)
            {
                // Check if the value has any partial matches in the data
                var hasPartialMatches = data.Any(loc =>
                    loc.Contains(value, StringComparison.OrdinalIgnoreCase));

                if (hasPartialMatches)
                {
                    try
                    {
                        _isShowingSuggestionOverlay = true;
                        var selected = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, data, value);
                        if (!string.IsNullOrEmpty(selected))
                        {
                            _viewModel.SelectedLocation = selected;
                            textBox.Text = selected;
                        }
                        else
                        {
                            _viewModel.SelectedLocation = value;
                        }
                    }
                    finally
                    {
                        _isShowingSuggestionOverlay = false;
                    }
                }
                else
                {
                    // MTM Pattern: Clear textbox when no matches found to maintain data integrity
                    _logger?.LogInformation($"Location '{value}' has no matches in validation source. Clearing textbox for data integrity.");
                    System.Diagnostics.Debug.WriteLine($"Location '{value}' has no matches in validation source. Clearing textbox for data integrity.");

                    textBox.Text = string.Empty;
                    _viewModel.SelectedLocation = string.Empty;

                    // Show user feedback about the clearing action
                    try
                    {
                        await Services.Core.ErrorHandling.HandleErrorAsync(
                            new ArgumentException($"Invalid Location: '{value}' not found in available locations."),
                            "Location validation failed - input cleared",
                            "System"
                        );
                    }
                    catch (Exception errorEx)
                    {
                        _logger?.LogWarning(errorEx, "Failed to show error message for invalid Location");
                        System.Diagnostics.Debug.WriteLine($"Failed to show error message for invalid Location: {errorEx.Message}");
                    }
                }
            }
            else
            {
                _viewModel.SelectedLocation = value;
            }
            UpdateValidationStates();
            ValidateAndUpdateSaveButton();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling location lost focus");
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
            var saveCommand = GetPropertyValue<ICommand>(_viewModel, "SaveCommand");
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
    private Task InitializeQuickButtonsIntegrationAsync()
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

        return Task.CompletedTask;
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

                // Also set QuantityText property for UI binding (if it exists)
                var quantityTextProperty = _viewModel.GetType().GetProperty("QuantityText");
                if (quantityTextProperty?.CanWrite == true)
                {
                    quantityTextProperty.SetValue(_viewModel, quantity.ToString());
                }

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

                // Clear TextBox error classes that may persist from TextBoxFuzzyValidationBehavior
                ClearTextBoxErrorClasses();

                // Focus the location field (likely next field to fill)
                _locationTextBox?.Focus();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling quick action");
            System.Diagnostics.Debug.WriteLine($"Error handling quick action: {ex.Message}");
        }
    }

    /// <summary>
    /// Clears error CSS classes from TextBox controls that may persist after programmatic updates.
    /// This addresses the issue where TextBoxFuzzyValidationBehavior error classes remain
    /// after valid data is populated via QuickButtons.
    /// </summary>
    private void ClearTextBoxErrorClasses()
    {
        try
        {
            _logger?.LogDebug("Clearing TextBox error classes after QuickButton population");

            // Clear error class from Part ID TextBox if it has valid data
            if (_partTextBox != null && !string.IsNullOrEmpty(_partTextBox.Text))
            {
                if (_partTextBox.Classes.Contains("error"))
                {
                    _partTextBox.Classes.Remove("error");
                    System.Diagnostics.Debug.WriteLine($"Removed 'error' class from PartTextBox - valid value: '{_partTextBox.Text}'");
                }
            }

            // Clear error class from Operation TextBox if it has valid data
            if (_operationTextBox != null && !string.IsNullOrEmpty(_operationTextBox.Text))
            {
                if (_operationTextBox.Classes.Contains("error"))
                {
                    _operationTextBox.Classes.Remove("error");
                    System.Diagnostics.Debug.WriteLine($"Removed 'error' class from OperationTextBox - valid value: '{_operationTextBox.Text}'");
                }
            }

            // Clear error class from Quantity TextBox if it has valid data
            if (_quantityTextBox != null && !string.IsNullOrEmpty(_quantityTextBox.Text))
            {
                if (int.TryParse(_quantityTextBox.Text, out int quantity) && quantity > 0)
                {
                    if (_quantityTextBox.Classes.Contains("error"))
                    {
                        _quantityTextBox.Classes.Remove("error");
                        System.Diagnostics.Debug.WriteLine($"Removed 'error' class from QuantityTextBox - valid value: '{_quantityTextBox.Text}'");
                    }
                }
            }

            _logger?.LogDebug("TextBox error classes cleared successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error clearing TextBox error classes");
            System.Diagnostics.Debug.WriteLine($"Error clearing TextBox error classes: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the ShowSuccessOverlay event from the ViewModel
    /// </summary>
    private async void OnShowSuccessOverlay(object? sender, SuccessEventArgs e)
    {
        try
        {
            if (_successOverlayService == null)
            {
                _logger?.LogWarning("Success overlay service not available - cannot display overlay");
                return;
            }

            // Show the success overlay using the service
            await _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                null, // Auto-resolve MainView
                e.Message,
                e.Details,
                e.IconKind,
                e.Duration
            );

            _logger?.LogInformation("Success overlay displayed via View event handler");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error displaying success overlay via View event handler");
            _ = Services.Core.ErrorHandling.HandleErrorAsync(ex, "OnShowSuccessOverlay", "System");
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
    /// Note: Arrow key handling removed - handled globally by MainWindow.
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

                    // REMOVED: Arrow key handling - now handled globally by MainWindow
                    // case Key.Up:
                    // case Key.Down:
                    //     No longer handled here - MainWindow handles these globally
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
            if (_viewModel == null) return;

            var resetCommand = GetPropertyValue<ICommand>(_viewModel, "ResetCommand");
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

            if (_viewModel != null)
            {
                // Refresh data command
                var refreshCommand = GetPropertyValue<ICommand>(_viewModel, "RefreshDataCommand");
                if (refreshCommand?.CanExecute(null) == true)
                {
                    refreshCommand.Execute(null);
                }
            }

            if (_quickButtonsViewModel != null)
            {
                // Refresh QuickButtons if available
                var refreshButtonsCommand = GetPropertyValue<ICommand>(_quickButtonsViewModel, "RefreshButtonsCommand");
                if (refreshButtonsCommand?.CanExecute(null) == true)
                {
                    refreshButtonsCommand.Execute(null);
                }
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
                var saveCommand = GetPropertyValue<ICommand>(_viewModel, "SaveCommand");
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
            if (currentFocus == _partTextBox)
            {
                _operationTextBox?.Focus();
            }
            else if (currentFocus == _operationTextBox)
            {
                _locationTextBox?.Focus();
            }
            else if (currentFocus == _locationTextBox)
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
            _partTextBox?.Focus();
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
            if (_partTextBox != null)
            {
                _partTextBox.TextChanged -= OnPartTextChanged;
            }

            if (_operationTextBox != null)
            {
                _operationTextBox.TextChanged -= OnOperationTextChanged;
            }

            if (_locationTextBox != null)
            {
                _locationTextBox.TextChanged -= OnLocationTextChanged;
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
                _viewModel.ShowSuccessOverlay -= OnShowSuccessOverlay;
            }

            // QuickButtons event handlers would be unsubscribed here using reflection if needed

            _logger?.LogDebug("InventoryTabView cleanup completed - arrow key handling delegated to MainWindow");
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


