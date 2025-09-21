using MTM_WIP_Application_Avalonia.Services.UI;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.Behaviors;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for NewQuickButtonView - MTM QuickButton creation interface.
/// Implements minimal Avalonia UserControl pattern with SuggestionOverlay integration.
/// 
/// Responsibilities:
/// - SuggestionOverlay event subscription and handling
/// - Keyboard navigation and focus management
/// - UI-specific interactions that cannot be handled in ViewModel
/// - Resource cleanup and event unsubscription
/// 
/// Integration Points:
/// - TextBoxFuzzyValidationBehavior for Part ID and Operation fields
/// - SuggestionOverlayView for fuzzy search suggestions
/// - MainView overlay system for modal presentation
/// </summary>
public partial class NewQuickButtonView : UserControl
{
    #region Fields
    
    private readonly IServiceProvider? _serviceProvider;
    private readonly ILogger<NewQuickButtonView>? _logger;
    private ISuggestionOverlayService? _suggestionOverlayService;
    
    // Flag to prevent cascading suggestion overlays
    private bool _isShowingSuggestionOverlay = false;
    
    #endregion

    #region Constructor

    /// <summary>
    /// Initializes the NewQuickButtonView with minimal configuration.
    /// Follows MTM standard pattern for Avalonia UserControls.
    /// </summary>
    public NewQuickButtonView()
    {
        InitializeComponent();
        InitializeControlReferences();
        SetupEventHandlers();
        
        Loaded += OnLoaded;
    }
    
    /// <summary>
    /// Constructor for dependency injection support.
    /// </summary>
    /// <param name="serviceProvider">Service provider for resolving dependencies</param>
    public NewQuickButtonView(IServiceProvider serviceProvider) : this()
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider?.GetService<ILogger<NewQuickButtonView>>();
        
        // Try to resolve the suggestion overlay service immediately if we have a service provider
        try
        {
            // Try to get both services from the service provider
            _suggestionOverlayService = _serviceProvider?.GetService<ISuggestionOverlayService>();
            
            // If we got the service from DI, it should already have focus management
            // If not available, we'll create it manually in TryResolveServices
            _logger?.LogInformation("SuggestionOverlayService resolved in constructor: {ServiceResolved}", _suggestionOverlayService != null);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to resolve SuggestionOverlayService in constructor");
        }
    }

    #endregion
    
    #region Control Initialization
    
    /// <summary>
    /// Initializes references to UI controls for direct manipulation.
    /// </summary>
    private void InitializeControlReferences()
    {
        try
        {
            // Control references are available via x:Name attributes in AXAML
            _logger?.LogDebug("UI control references initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing control references");
            System.Diagnostics.Debug.WriteLine($"Error initializing control references: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Sets up event handlers for the form controls.
    /// </summary>
    private void SetupEventHandlers()
    {
        try
        {
            // Set up LostFocus handlers for TextBoxes with SuggestionOverlay support
            var partIdTextBox = this.FindControl<TextBox>("PartIdTextBox");
            var operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            
            if (partIdTextBox != null)
            {
                partIdTextBox.LostFocus += OnPartIdLostFocus;
                _logger?.LogDebug("PartId TextBox LostFocus event handler attached");
            }
            
            if (operationTextBox != null)
            {
                operationTextBox.LostFocus += OnOperationLostFocus;
                _logger?.LogDebug("Operation TextBox LostFocus event handler attached");
            }
            
            KeyDown += OnUserControlKeyDown;
            
            _logger?.LogDebug("Event handlers set up successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting up event handlers");
            System.Diagnostics.Debug.WriteLine($"Error setting up event handlers: {ex.Message}");
        }
    }
    
    #endregion

    #region SuggestionOverlay LostFocus Handlers

    /// <summary>
    /// Handles Part ID field losing focus with SuggestionOverlay integration.
    /// Follows the same pattern as InventoryTabView for consistent user experience.
    /// </summary>
    private async void OnPartIdLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not NewQuickButtonOverlayViewModel viewModel || sender is not TextBox textBox) 
                return;

            // Ensure services are resolved before proceeding
            TryResolveServices();

            var value = textBox.Text?.Trim() ?? string.Empty;
            var data = viewModel.PartIds ?? Enumerable.Empty<string>();
            int count = data.Count();
            _logger?.LogInformation($"[FocusLost] PartIdTextBox lost focus. User entered: '{value}'. PartIds count: {count}");

            // Check if no data is available from server
            if (count == 0)
            {
                _logger?.LogWarning("No Part IDs available - likely database connectivity issue");
                System.Diagnostics.Debug.WriteLine("No Part IDs available - likely database connectivity issue");
                
                // Clear the textbox and show error message about data unavailability
                textBox.Text = string.Empty;
                viewModel.PartId = string.Empty;
                return;
            }

            // Only show suggestions if the user actually entered something
            if (!string.IsNullOrEmpty(value) && 
                !data.Contains(value, StringComparer.OrdinalIgnoreCase) && 
                _suggestionOverlayService != null &&
                !_isShowingSuggestionOverlay)
            {
                // Check if the value has any partial matches in the data
                var hasPartialMatches = data.Any(part => 
                    part.Contains(value, StringComparison.OrdinalIgnoreCase));

                if (hasPartialMatches)
                {
                    try
                    {
                        _isShowingSuggestionOverlay = true;
                        var selected = await _suggestionOverlayService.ShowSuggestionsAsync(textBox, data, value);
                        if (!string.IsNullOrEmpty(selected))
                        {
                            viewModel.PartId = selected;
                            textBox.Text = selected;
                            _logger?.LogInformation($"Part ID suggestion selected: '{selected}'");
                        }
                        else
                        {
                            viewModel.PartId = value;
                            _logger?.LogInformation($"Part ID suggestion cancelled, keeping: '{value}'");
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
                    _logger?.LogInformation($"Part ID '{value}' has no matches in validation source. Clearing textbox for data integrity.");
                    System.Diagnostics.Debug.WriteLine($"Part ID '{value}' has no matches in validation source. Clearing textbox for data integrity.");
                    
                    textBox.Text = string.Empty;
                    viewModel.PartId = string.Empty;
                    
                    // Show user feedback about the clearing action
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await Services.Core.ErrorHandling.HandleErrorAsync(
                                new ArgumentException($"Invalid Part ID: '{value}' not found in available options."),
                                "Part ID validation failed - input cleared",
                                "System"
                            );
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid Part ID: '{value}' not found. Input cleared.");
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Part ID lost focus");
            System.Diagnostics.Debug.WriteLine($"Error handling Part ID lost focus: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Operation field losing focus with SuggestionOverlay integration.
    /// Follows the same pattern as InventoryTabView for consistent user experience.
    /// </summary>
    private async void OnOperationLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not NewQuickButtonOverlayViewModel viewModel || sender is not TextBox textBox) 
                return;

            // Ensure services are resolved before proceeding
            TryResolveServices();

            var value = textBox.Text?.Trim() ?? string.Empty;
            var data = viewModel.Operations ?? Enumerable.Empty<string>();
            int count = data.Count();
            _logger?.LogInformation($"[FocusLost] OperationTextBox lost focus. User entered: '{value}'. Operations count: {count}");

            // Check if no data is available from server
            if (count == 0)
            {
                _logger?.LogWarning("No Operations available - likely database connectivity issue");
                System.Diagnostics.Debug.WriteLine("No Operations available - likely database connectivity issue");
                
                // Clear the textbox and show error message about data unavailability
                textBox.Text = string.Empty;
                viewModel.Operation = string.Empty;
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
                            viewModel.Operation = selected;
                            textBox.Text = selected;
                            _logger?.LogInformation($"Operation suggestion selected: '{selected}'");
                        }
                        else
                        {
                            viewModel.Operation = value;
                            _logger?.LogInformation($"Operation suggestion cancelled, keeping: '{value}'");
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
                    viewModel.Operation = string.Empty;
                    
                    // Show user feedback about the clearing action
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await Services.Core.ErrorHandling.HandleErrorAsync(
                                new ArgumentException($"Invalid Operation: '{value}' not found in available options."),
                                "Operation validation failed - input cleared",
                                "System"
                            );
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid Operation: '{value}' not found. Input cleared.");
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Operation lost focus");
            System.Diagnostics.Debug.WriteLine($"Error handling Operation lost focus: {ex.Message}");
        }
    }

    /// <summary>
    /// Attempts to resolve SuggestionOverlayService if not already available.
    /// Uses multiple fallback methods similar to InventoryTabView pattern.
    /// </summary>
    private void TryResolveServices()
    {
        try
        {
            if (_suggestionOverlayService != null) return;

            if (_serviceProvider != null)
            {
                _suggestionOverlayService = _serviceProvider.GetService<ISuggestionOverlayService>();
                
                // If ISuggestionOverlayService is not registered, create it manually with dependencies
                if (_suggestionOverlayService == null)
                {
                    var suggestionLogger = _serviceProvider.GetService<ILogger<SuggestionOverlayService>>();
                    var focusService = _serviceProvider.GetService<IFocusManagementService>();
                    
                    if (suggestionLogger != null)
                    {
                        _suggestionOverlayService = new SuggestionOverlayService(suggestionLogger, focusService);
                        _logger?.LogDebug("SuggestionOverlayService created manually with DI dependencies");
                    }
                }
                
                _logger?.LogDebug("SuggestionOverlayService resolved via ServiceProvider: {ServiceResolved}", 
                    _suggestionOverlayService != null);
                return;
            }

            // Try to resolve from MainWindow if ServiceProvider not available
            if (TopLevel.GetTopLevel(this) is Window mainWindow)
            {
                // Try to get service provider from DataContext or other means
                try 
                {
                    // Attempt to resolve service via App.Current
                    if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop &&
                        desktop.MainWindow is MainWindow mtmMainWindow)
                    {
                        var dataContext = mtmMainWindow.DataContext;
                        if (dataContext != null)
                        {
                            // Try to find a service provider property or field
                            var serviceProviderProperty = dataContext.GetType().GetProperty("ServiceProvider");
                            if (serviceProviderProperty != null)
                            {
                                var windowServiceProvider = serviceProviderProperty.GetValue(dataContext) as IServiceProvider;
                                _suggestionOverlayService = windowServiceProvider?.GetService<ISuggestionOverlayService>();
                                _logger?.LogDebug("SuggestionOverlayService resolved via MainWindow DataContext: {ServiceResolved}", 
                                    _suggestionOverlayService != null);
                                if (_suggestionOverlayService != null) return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogDebug(ex, "Failed to resolve service via MainWindow DataContext");
                }
            }

            // Final fallback - create service directly with focus management
            if (_suggestionOverlayService == null)
            {
                var serviceLogger = Microsoft.Extensions.Logging.LoggerFactory
                    .Create(builder => builder.AddConsole())
                    .CreateLogger<SuggestionOverlayService>();
                    
                // Try to get IFocusManagementService or create one
                var focusService = _serviceProvider?.GetService<IFocusManagementService>();
                if (focusService == null)
                {
                    var focusLogger = Microsoft.Extensions.Logging.LoggerFactory
                        .Create(builder => builder.AddConsole())
                        .CreateLogger<FocusManagementService>();
                    focusService = new FocusManagementService(focusLogger);
                }
                    
                _suggestionOverlayService = new SuggestionOverlayService(serviceLogger, focusService);
                _logger?.LogDebug("SuggestionOverlayService created directly as fallback with focus management");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error resolving SuggestionOverlayService");
            System.Diagnostics.Debug.WriteLine($"Error resolving SuggestionOverlayService: {ex.Message}");
        }
    }

    #endregion

    #region Old SuggestionOverlay Event Handling (Deprecated)

    /// <summary>
    /// Handles suggestion overlay requests from TextBoxFuzzyValidationBehavior.
    /// This integrates the NewQuickButtonView with the MTM suggestion system
    /// used throughout the application for consistent user experience.
    /// 
    /// NOTE: This method is deprecated in favor of direct LostFocus handlers above.
    /// Kept for backward compatibility but should not be actively used.
    /// </summary>
    /// <param name="sourceTextBox">The TextBox that triggered the suggestion request</param>
    /// <param name="suggestions">List of suggestion items to display</param>
    private void OnSuggestionOverlayRequested(TextBox sourceTextBox, List<object> suggestions)
    {
        try
        {
            // Get control references
            var partIdTextBox = this.FindControl<TextBox>("PartIdTextBox");
            var operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            
            // Verify that the suggestion request is from our TextBox controls
            if (sourceTextBox != partIdTextBox && sourceTextBox != operationTextBox)
            {
                // Not our TextBox - ignore the request
                return;
            }

            // Determine suggestion type based on source TextBox
            string suggestionContext = sourceTextBox == partIdTextBox ? "Part ID" : "Operation";
            
            // Log for backward compatibility tracking
            System.Diagnostics.Debug.WriteLine(
                $"[DEPRECATED] SuggestionOverlay requested for {suggestionContext} with {suggestions.Count} suggestions - Use LostFocus handlers instead");
        }
        catch (Exception ex)
        {
            // Log error but don't crash the UI
            System.Diagnostics.Debug.WriteLine($"Error handling suggestion overlay request: {ex.Message}");
        }
    }

    #endregion

    #region Keyboard Navigation

    /// <summary>
    /// Handles keyboard navigation within the form.
    /// Provides consistent keyboard behavior following MTM UI standards.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">Keyboard event args</param>
    private void OnUserControlKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            switch (e.Key)
            {
                case Key.Escape:
                    // Escape key cancels the dialog
                    if (DataContext is NewQuickButtonOverlayViewModel viewModel)
                    {
                        viewModel.CancelCommand.Execute(null);
                    }
                    e.Handled = true;
                    break;
                    
                case Key.Enter:
                    // Enter key advances focus or triggers create if on Create button
                    if (e.Source == CreateButton && DataContext is NewQuickButtonOverlayViewModel vm && vm.CanCreate)
                    {
                        vm.CreateCommand.Execute(null);
                        e.Handled = true;
                    }
                    else
                    {
                        // Move to next focusable element
                        MoveFocusToNextElement();
                        e.Handled = true;
                    }
                    break;
                    
                case Key.Tab:
                    // Let default Tab behavior handle focus movement
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in keyboard navigation: {ex.Message}");
        }
    }

    /// <summary>
    /// Moves focus to the next logical element in the tab order
    /// </summary>
    private void MoveFocusToNextElement()
    {
        try
        {
            var currentFocus = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
            
            // Get control references
            var partIdTextBox = this.FindControl<TextBox>("PartIdTextBox");
            var operationTextBox = this.FindControl<TextBox>("OperationTextBox");
            var quantityTextBox = this.FindControl<TextBox>("QuantityTextBox");
            var createButton = this.FindControl<Button>("CreateButton");
            var cancelButton = this.FindControl<Button>("CancelButton");
            
            // Define the logical tab order for this form
            Control? nextControl = currentFocus switch
            {
                _ when ReferenceEquals(currentFocus, partIdTextBox) => operationTextBox,
                _ when ReferenceEquals(currentFocus, operationTextBox) => quantityTextBox,
                _ when ReferenceEquals(currentFocus, quantityTextBox) => createButton,
                _ when ReferenceEquals(currentFocus, createButton) => cancelButton,
                _ => partIdTextBox // Default to first field
            };

            nextControl?.Focus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving focus: {ex.Message}");
        }
    }

    #endregion

    #region Focus Management

    /// <summary>
    /// Handles GotFocus events for TextBox controls to select all text.
    /// This provides better user experience for data entry scenarios.
    /// </summary>
    /// <param name="sender">The TextBox that gained focus</param>
    /// <param name="e">Focus event args</param>
    private void OnTextBoxGotFocus(object? sender, GotFocusEventArgs e)
    {
        try
        {
            if (sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
                // Select all text when gaining focus for easy replacement
                textBox.SelectAll();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in TextBox focus handling: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles LostFocus events for TextBox controls to trigger validation.
    /// This ensures validation occurs when user moves between fields.
    /// </summary>
    /// <param name="sender">The TextBox that lost focus</param>
    /// <param name="e">Focus event args</param>
    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Validation is handled automatically by the ViewModel's property change handlers
            // This method is available for any additional UI-specific focus handling if needed
            
            if (sender is TextBox textBox && DataContext is NewQuickButtonOverlayViewModel)
            {
                // Trigger any UI-specific validation feedback if needed
                // Currently handled by ViewModel and AXAML binding
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in TextBox lost focus handling: {ex.Message}");
        }
    }

    #endregion

    #region Event Handling

    /// <summary>
    /// Handles the Loaded event to set initial focus and perform setup.
    /// </summary>
    /// <param name="sender">The UserControl</param>
    /// <param name="e">Loaded event args</param>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Set initial focus to the first input field
            var partIdTextBox = this.FindControl<TextBox>("PartIdTextBox");
            partIdTextBox?.Focus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnLoaded: {ex.Message}");
        }
    }

    #endregion

    #region Resource Cleanup

    /// <summary>
    /// Handles cleanup when the control is detached from the visual tree.
    /// Unsubscribes from events to prevent memory leaks.
    /// </summary>
    /// <param name="e">Visual tree attachment event args</param>
    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        try
        {
            // Clean up any resources or subscriptions
            // (LostFocus handlers are cleaned up automatically when control is disposed)
            
            // Call base cleanup
            base.OnDetachedFromVisualTree(e);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during cleanup: {ex.Message}");
            // Still call base even if error occurs
            base.OnDetachedFromVisualTree(e);
        }
    }

    #endregion
}

