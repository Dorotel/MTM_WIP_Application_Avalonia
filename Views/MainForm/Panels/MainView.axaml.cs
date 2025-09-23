using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Views;

namespace MTM_WIP_Application_Avalonia.Views
{
    public partial class MainView : UserControl
    {
        private bool _isInitialized = false;
        private MainViewViewModel? _viewModel;

        /// <summary>
        /// Static flag to track if a tab switch is currently in progress
        /// Used by SuggestionOverlay service to prevent showing overlays during tab transitions
        /// </summary>
        public static bool IsTabSwitchInProgress { get; private set; } = false;

        /// <summary>
        /// Forces clearing of the tab switch flag (for emergency cleanup or testing)
        /// </summary>
        public static void ClearTabSwitchFlag()
        {
            IsTabSwitchInProgress = false;
            System.Diagnostics.Debug.WriteLine("MANUAL: Tab switch flag forcibly cleared");
        }

        public MainView()
        {
            InitializeComponent();
            Loaded += OnMainViewLoaded;
            SizeChanged += OnMainViewSizeChanged;
            DataContextChanged += OnDataContextChanged;

            // CRITICAL: Subscribe to TabControl property changes immediately to catch tab switching early
            SetupEarlyTabMonitoring();

            // DEBUG: Add theme diagnostic when view loads
            Loaded += OnMainViewLoadedThemeDebug;

            // Setup theme editor navigation
            SetupThemeEditorNavigation();
        }

        /// <summary>
        /// Sets up theme editor navigation by connecting ThemeQuickSwitcher events
        /// </summary>
        private void SetupThemeEditorNavigation()
        {
            this.Loaded += (sender, e) =>
            {
                try
                {
                    // Find the ThemeQuickSwitcher control
                    var themeQuickSwitcher = this.FindControl<Views.MainForm.Overlays.ThemeQuickSwitcher>("ThemeQuickSwitcher") ??
                                           FindControlInVisualTree<Views.MainForm.Overlays.ThemeQuickSwitcher>(this, "ThemeQuickSwitcher");

                    if (themeQuickSwitcher != null)
                    {
                        // Subscribe to the theme editor request event
                        themeQuickSwitcher.ThemeEditorRequested += OnThemeEditorRequested;
                        System.Diagnostics.Debug.WriteLine("Successfully subscribed to ThemeQuickSwitcher.ThemeEditorRequested event");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("WARNING: Could not find ThemeQuickSwitcher control");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error setting up theme editor navigation: {ex.Message}");
                }
            };
        }

        /// <summary>
        /// Handles theme editor request by navigating to the theme editor
        /// </summary>
        private void OnThemeEditorRequested(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Theme editor requested - navigating to theme editor");

                // Get navigation service and navigate to theme editor
                var navigationService = Program.GetOptionalService<Services.INavigationService>();
                if (navigationService != null)
                {
                    // Get theme editor view from DI container
                    var themeEditorViewModel = Program.GetOptionalService<ViewModels.ThemeEditorViewModel>();
                    if (themeEditorViewModel != null)
                    {
                        var themeEditorView = new Views.ThemeEditorView
                        {
                            DataContext = themeEditorViewModel
                        };

                        navigationService.NavigateTo(themeEditorView);
                        System.Diagnostics.Debug.WriteLine("Successfully navigated to theme editor");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: ThemeEditorViewModel not found in DI container");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: NavigationService not available");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling theme editor request: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets up early monitoring of TabControl changes to clear inputs before focus events
        /// </summary>
        private void SetupEarlyTabMonitoring()
        {
            try
            {
                // Find the TabControl and subscribe to property changes immediately
                this.Loaded += (sender, e) =>
                {
                    var tabControl = this.FindControl<TabControl>("MainForm_TabControl");
                    if (tabControl != null)
                    {
                        System.Diagnostics.Debug.WriteLine("EARLY: TabControl found, setting up property change monitoring");

                        // Subscribe to property changes to catch SelectedIndex changes early
                        tabControl.PropertyChanged += OnTabControlPropertyChanged;

                        System.Diagnostics.Debug.WriteLine("EARLY: TabControl property change monitoring setup complete");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("EARLY WARNING: TabControl 'MainForm_TabControl' not found for early monitoring");
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up early tab monitoring: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles TabControl pointer pressed events to set tab switch flag and clear inputs ONLY when clicking on tab headers.
        /// This prevents any focus events from triggering SuggestionOverlay during actual tab switching.
        /// Called BEFORE any selection change or focus events, but only when clicking on actual tab headers.
        /// </summary>
        private void OnTabControlPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            try
            {
                // Only proceed if we're actually clicking on a tab header, not content area
                if (!IsClickingOnTabHeader(e))
                {
                    System.Diagnostics.Debug.WriteLine("POINTER: Click detected on TabControl content area - no action needed");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("IMMEDIATE: TabControl pointer pressed on tab header - setting tab switch flag and clearing inputs");

                // Set the tab switch flag to prevent SuggestionOverlay from showing
                IsTabSwitchInProgress = true;

                // Clear ALL inputs immediately when user starts clicking on tabs
                // This happens BEFORE any selection change or focus events
                ClearAllTabInputsImmediate();

                // Schedule clearing the flag after a brief delay to allow tab switch to complete
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await Task.Delay(500); // Allow tab switch animation and focus events to complete
                    IsTabSwitchInProgress = false;
                    System.Diagnostics.Debug.WriteLine("IMMEDIATE: Tab switch flag cleared after delay");
                });
            }
            catch (Exception ex)
            {
                IsTabSwitchInProgress = false; // Ensure flag is cleared on error
                System.Diagnostics.Debug.WriteLine($"Error in tab pointer pressed handler: {ex.Message}");
            }
        }

        /// <summary>
        /// Determines if the pointer click is occurring on a tab header (actual tab button)
        /// rather than in the tab content area.
        /// </summary>
        /// <param name="e">Pointer pressed event args</param>
        /// <returns>True if clicking on a tab header, false if clicking in content area</returns>
        private bool IsClickingOnTabHeader(Avalonia.Input.PointerPressedEventArgs e)
        {
            try
            {
                // Get the TabControl
                if (this.FindControl<TabControl>("MainForm_TabControl") is not TabControl tabControl)
                {
                    System.Diagnostics.Debug.WriteLine("POINTER: Could not find MainForm_TabControl");
                    return false;
                }

                // Get click position relative to TabControl
                var clickPosition = e.GetPosition(tabControl);

                // Check if click is within tab header area - typically the top portion of TabControl
                // For most themes, tab headers are in the top ~40-50 pixels
                var tabHeaderHeight = 45.0; // Approximate height of tab headers

                if (clickPosition.Y <= tabHeaderHeight)
                {
                    System.Diagnostics.Debug.WriteLine($"POINTER: Click at Y={clickPosition.Y:F1} is within tab header area (height: {tabHeaderHeight})");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"POINTER: Click at Y={clickPosition.Y:F1} is in content area (below header height: {tabHeaderHeight})");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error determining click location: {ex.Message}");
                // If we can't determine, err on the side of NOT clearing inputs
                return false;
            }
        }

        /// <summary>
        /// Handles TabControl property changes to clear inputs BEFORE focus events
        /// </summary>
        private void OnTabControlPropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            try
            {
                if (e.Property.Name == "SelectedIndex" && sender is TabControl tabControl)
                {
                    System.Diagnostics.Debug.WriteLine($"EARLY: TabControl SelectedIndex changed to {tabControl.SelectedIndex} - setting tab switch flag and clearing inputs immediately");

                    // Set the tab switch flag to prevent SuggestionOverlay from showing
                    IsTabSwitchInProgress = true;

                    // Clear ALL inputs immediately when any tab selection change is detected
                    ClearAllTabInputsImmediate();

                    // Schedule clearing the flag after a brief delay
                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        await Task.Delay(300); // Shorter delay since this is mid-process
                        IsTabSwitchInProgress = false;
                        System.Diagnostics.Debug.WriteLine("EARLY: Tab switch flag cleared after property change delay");
                    });
                }
            }
            catch (Exception ex)
            {
                IsTabSwitchInProgress = false; // Ensure flag is cleared on error
                System.Diagnostics.Debug.WriteLine($"Error in early tab property change handler: {ex.Message}");
            }
        }

        private void OnMainViewLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_isInitialized) return;

            try
            {
                // Ensure the window is properly positioned and sized on the current monitor
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel is Window window)
                {
                    EnsureWindowOnCurrentMonitor(window);
                    SetOptimalWindowSize(window);
                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during MainView initialization: {ex.Message}");
            }
        }

        private void OnMainViewSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            try
            {
                // Ensure window stays within screen bounds when content changes size
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel is Window window && _isInitialized)
                {
                    EnsureWindowWithinScreenBounds(window);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during size change handling: {ex.Message}");
            }
        }

        private void EnsureWindowOnCurrentMonitor(Window window)
        {
            try
            {
                var screens = window.Screens?.All;
                if (screens == null || !screens.Any()) return;

                // Get the screen where the mouse cursor is currently located
                var currentScreen = GetCurrentScreen(window) ?? screens.First();

                // Calculate center position on the current screen
                var workingArea = currentScreen.WorkingArea;
                var screenCenter = new PixelPoint(
                    workingArea.X + workingArea.Width / 2,
                    workingArea.Y + workingArea.Height / 2
                );

                // Get current window size or use default
                var windowWidth = window.Width > 0 ? (int)window.Width : 1200;
                var windowHeight = window.Height > 0 ? (int)window.Height : 700;

                // Ensure the window size fits within the screen
                var maxWidth = (int)(workingArea.Width * 0.9); // Use 90% of screen width max
                var maxHeight = (int)(workingArea.Height * 0.9); // Use 90% of screen height max

                windowWidth = Math.Min(windowWidth, maxWidth);
                windowHeight = Math.Min(windowHeight, maxHeight);

                // Calculate position to center the window
                var newPosition = new PixelPoint(
                    screenCenter.X - windowWidth / 2,
                    screenCenter.Y - windowHeight / 2
                );

                // Apply the new position and size
                window.Position = newPosition;
                window.Width = windowWidth;
                window.Height = windowHeight;

                System.Diagnostics.Debug.WriteLine($"Window positioned at {newPosition} with size {windowWidth}x{windowHeight} on screen {currentScreen.DisplayName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error positioning window on current monitor: {ex.Message}");
            }
        }

        private void EnsureWindowWithinScreenBounds(Window window)
        {
            try
            {
                var screens = window.Screens?.All;
                if (screens == null || !screens.Any()) return;

                var currentScreen = GetCurrentScreen(window) ?? screens.First();
                var workingArea = currentScreen.WorkingArea;

                var windowBounds = new PixelRect(
                    window.Position.X,
                    window.Position.Y,
                    (int)window.Width,
                    (int)window.Height
                );

                // Check if window is outside screen bounds
                if (!workingArea.Contains(windowBounds))
                {
                    // Adjust size if window is too large
                    var newWidth = Math.Min((int)window.Width, workingArea.Width);
                    var newHeight = Math.Min((int)window.Height, workingArea.Height);

                    // Adjust position to keep window on screen
                    var newX = Math.Max(workingArea.X,
                        Math.Min(window.Position.X, workingArea.Right - newWidth));
                    var newY = Math.Max(workingArea.Y,
                        Math.Min(window.Position.Y, workingArea.Bottom - newHeight));

                    window.Position = new PixelPoint(newX, newY);
                    window.Width = newWidth;
                    window.Height = newHeight;

                    System.Diagnostics.Debug.WriteLine($"Window adjusted to stay within screen bounds: {newX},{newY} {newWidth}x{newHeight}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ensuring window within screen bounds: {ex.Message}");
            }
        }

        private void SetOptimalWindowSize(Window window)
        {
            try
            {
                var screens = window.Screens?.All;
                if (screens == null || !screens.Any()) return;

                var currentScreen = GetCurrentScreen(window) ?? screens.First();
                var workingArea = currentScreen.WorkingArea;

                // Set optimal window size based on content and screen size
                var optimalWidth = Math.Min(1200, (int)(workingArea.Width * 0.8));
                var optimalHeight = Math.Min(700, (int)(workingArea.Height * 0.8));

                // Set minimum constraints
                window.MinWidth = 800;
                window.MinHeight = 500;

                // Set maximum constraints based on screen size
                window.MaxWidth = workingArea.Width;
                window.MaxHeight = workingArea.Height;

                // Apply optimal size if not already set appropriately
                if (window.Width <= 0 || window.Height <= 0)
                {
                    window.Width = optimalWidth;
                    window.Height = optimalHeight;
                }

                System.Diagnostics.Debug.WriteLine($"Window size constraints set: Min({window.MinWidth}x{window.MinHeight}) Max({window.MaxWidth}x{window.MaxHeight}) Current({window.Width}x{window.Height})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting optimal window size: {ex.Message}");
            }
        }

        private Screen? GetCurrentScreen(Window window)
        {
            try
            {
                var screens = window.Screens?.All;
                if (screens == null || !screens.Any()) return null;

                // Try to get the screen where the window is currently located
                var windowCenter = new PixelPoint(
                    window.Position.X + (int)window.Width / 2,
                    window.Position.Y + (int)window.Height / 2
                );

                foreach (var screen in screens)
                {
                    if (screen.WorkingArea.Contains(windowCenter))
                    {
                        return screen;
                    }
                }

                // If window center is not on any screen, find the screen with most overlap
                var bestScreen = screens.First();
                var maxOverlap = 0;

                var windowBounds = new PixelRect(
                    window.Position.X,
                    window.Position.Y,
                    (int)window.Width,
                    (int)window.Height
                );

                foreach (var screen in screens)
                {
                    var intersection = screen.WorkingArea.Intersect(windowBounds);
                    var overlapArea = intersection.Width * intersection.Height;

                    if (overlapArea > maxOverlap)
                    {
                        maxOverlap = overlapArea;
                        bestScreen = screen;
                    }
                }

                return bestScreen;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current screen: {ex.Message}");
                return window.Screens?.All?.FirstOrDefault();
            }
        }

        /// <summary>
        /// Public method to manually center the window on the current monitor
        /// </summary>
        public void CenterOnCurrentMonitor()
        {
            try
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel is Window window)
                {
                    EnsureWindowOnCurrentMonitor(window);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error centering window on current monitor: {ex.Message}");
            }
        }

        /// <summary>
        /// Public method to reset window to default size
        /// </summary>
        public void ResetToDefaultSize()
        {
            try
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel is Window window)
                {
                    window.Width = 1200;
                    window.Height = 700;
                    EnsureWindowOnCurrentMonitor(window);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting window to default size: {ex.Message}");
            }
        }

        private void OnMainViewLoadedThemeDebug(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                // Debug theme resource resolution
                DebugThemeResources();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in theme debug: {ex.Message}");
            }
        }

        /// <summary>
        /// Debug method to check what theme resources are actually resolved
        /// </summary>
        private void DebugThemeResources()
        {
            try
            {
                if (Application.Current?.Resources == null)
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG: Application.Current.Resources is null");
                    return;
                }

                // Check key theme resources
                var primaryAction = Application.Current.Resources.TryGetResource("MTM_Shared_Logic.PrimaryAction", null, out var primaryValue);
                var OverlayTextBrush = Application.Current.Resources.TryGetResource("MTM_Shared_Logic.OverlayTextBrush", null, out var textValue);

                System.Diagnostics.Debug.WriteLine($"DEBUG: MTM_Shared_Logic.PrimaryAction found: {primaryAction}, value: {primaryValue}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: MTM_Shared_Logic.OverlayTextBrush found: {OverlayTextBrush}, value: {textValue}");

                // Check if we can find tab-related resources
                var cardBackground = Application.Current.Resources.TryGetResource("MTM_Shared_Logic.CardBackgroundBrush", null, out var cardValue);
                System.Diagnostics.Debug.WriteLine($"DEBUG: MTM_Shared_Logic.CardBackgroundBrush found: {cardBackground}, value: {cardValue}");

                // Check additional theme resources
                var borderAccent = Application.Current.Resources.TryGetResource("MTM_Shared_Logic.BorderAccentBrush", null, out var borderValue);
                var hoverBackground = Application.Current.Resources.TryGetResource("MTM_Shared_Logic.HoverBackground", null, out var hoverValue);
                var bodyText = Application.Current.Resources.TryGetResource("MTM_Shared_Logic.BodyText", null, out var bodyTextValue);

                System.Diagnostics.Debug.WriteLine($"DEBUG: MTM_Shared_Logic.BorderAccentBrush found: {borderAccent}, value: {borderValue}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: MTM_Shared_Logic.HoverBackground found: {hoverBackground}, value: {hoverValue}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: MTM_Shared_Logic.BodyText found: {bodyText}, value: {bodyTextValue}");

                // Check the current theme variant
                System.Diagnostics.Debug.WriteLine($"DEBUG: Current RequestedThemeVariant: {Application.Current.RequestedThemeVariant}");

                // Check how many merged dictionaries we have
                System.Diagnostics.Debug.WriteLine($"DEBUG: Merged dictionaries count: {Application.Current.Resources.MergedDictionaries.Count}");

                // Try to find the active MTM Light theme
                int lightThemeIndex = -1;
                for (int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++)
                {
                    var dict = Application.Current.Resources.MergedDictionaries[i];
                    if (dict.TryGetResource("MTM_Shared_Logic.PrimaryAction", null, out var testValue) &&
                        testValue?.ToString() == "#B8860B")
                    {
                        lightThemeIndex = i;
                        break;
                    }
                }
                System.Diagnostics.Debug.WriteLine($"DEBUG: MTM Light theme found at index: {lightThemeIndex}");

                // Force invalidate visual to trigger re-rendering
                this.InvalidateVisual();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG: Error checking theme resources: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows the suggestion overlay panel with the specified content.
        /// </summary>
        /// <param name="overlayContent">The content to display in the overlay</param>
        public void ShowSuggestionOverlay(Control overlayContent)
        {
            try
            {
                var overlayPanel = this.FindControl<Border>("SuggestionOverlayPanel");
                var contentControl = this.FindControl<ContentControl>("SuggestionOverlayContent");

                if (overlayPanel != null && contentControl != null)
                {
                    contentControl.Content = overlayContent;
                    overlayPanel.IsVisible = true;
                    System.Diagnostics.Debug.WriteLine("Suggestion overlay panel shown successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could not find overlay panel or content control");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing suggestion overlay: {ex.Message}");
            }
        }

        /// <summary>
        /// Hides the suggestion overlay panel.
        /// </summary>
        public void HideSuggestionOverlay()
        {
            try
            {
                var overlayPanel = this.FindControl<Border>("SuggestionOverlayPanel");
                var contentControl = this.FindControl<ContentControl>("SuggestionOverlayContent");

                if (overlayPanel != null && contentControl != null)
                {
                    overlayPanel.IsVisible = false;
                    contentControl.Content = null;
                    System.Diagnostics.Debug.WriteLine("Suggestion overlay panel hidden successfully");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding suggestion overlay: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows the NewQuickButton view panel.
        /// </summary>
        /// <param name="viewModel">The ViewModel for the NewQuickButton view</param>
        public void ShowNewQuickButtonOverlay(object? viewModel = null)
        {
            try
            {
                var overlayPanel = this.FindControl<Border>("NewQuickButtonOverlayPanel");
                var newQuickButtonView = this.FindControl<MTM_WIP_Application_Avalonia.Views.NewQuickButtonView>("NewQuickButtonView");

                if (overlayPanel != null && newQuickButtonView != null)
                {
                    // Set the DataContext for the view if provided
                    if (viewModel != null)
                    {
                        newQuickButtonView.DataContext = viewModel;
                    }

                    overlayPanel.IsVisible = true;
                    System.Diagnostics.Debug.WriteLine("NewQuickButton view panel shown successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could not find NewQuickButton view panel or view control");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing NewQuickButton view: {ex.Message}");
            }
        }

        /// <summary>
        /// Hides the NewQuickButton view panel.
        /// </summary>
        public void HideNewQuickButtonOverlay()
        {
            try
            {
                var overlayPanel = this.FindControl<Border>("NewQuickButtonOverlayPanel");

                if (overlayPanel != null)
                {
                    overlayPanel.IsVisible = false;
                    System.Diagnostics.Debug.WriteLine("NewQuickButton view panel hidden successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could not find NewQuickButton view panel");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding NewQuickButton view: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows the success overlay panel with the specified content.
        /// </summary>
        /// <param name="overlayContent">The success overlay content to display</param>
        public void ShowSuccessOverlay(Control overlayContent)
        {
            try
            {
                var overlayPanel = this.FindControl<Border>("SuccessOverlayPanel");
                var contentControl = this.FindControl<ContentControl>("SuccessOverlayContent");

                if (overlayPanel != null && contentControl != null)
                {
                    contentControl.Content = overlayContent;
                    overlayPanel.IsVisible = true;
                    System.Diagnostics.Debug.WriteLine("Success overlay panel shown successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could not find success overlay panel or content control");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing success overlay: {ex.Message}");
            }
        }

        /// <summary>
        /// Hides the success overlay panel.
        /// </summary>
        public void HideSuccessOverlay()
        {
            try
            {
                var overlayPanel = this.FindControl<Border>("SuccessOverlayPanel");
                var contentControl = this.FindControl<ContentControl>("SuccessOverlayContent");

                if (overlayPanel != null && contentControl != null)
                {
                    overlayPanel.IsVisible = false;
                    contentControl.Content = null;
                    System.Diagnostics.Debug.WriteLine("Success overlay panel hidden successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could not find success overlay panel or content control");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding success overlay: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current instance of MainView from the visual tree.
        /// </summary>
        /// <param name="control">Any control to start the search from</param>
        /// <returns>The MainView instance or null if not found</returns>
        public static MainView? FindMainView(Control control)
        {
            try
            {
                var current = control;
                while (current != null)
                {
                    if (current is MainView mainView)
                        return mainView;
                    current = current.Parent as Control;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error finding MainView: {ex.Message}");
                return null;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            try
            {
                // Clean up event handlers
                Loaded -= OnMainViewLoaded;
                SizeChanged -= OnMainViewSizeChanged;
                DataContextChanged -= OnDataContextChanged;

                // Clean up TabControl property change subscription
                var tabControl = this.FindControl<TabControl>("MainForm_TabControl");
                if (tabControl != null)
                {
                    tabControl.PropertyChanged -= OnTabControlPropertyChanged;
                    System.Diagnostics.Debug.WriteLine("CLEANUP: TabControl property change monitoring unsubscribed");
                }

                // Unsubscribe from ViewModel events
                if (_viewModel != null)
                {
                    _viewModel.TriggerLostFocusRequested -= OnTriggerLostFocusRequested;
                    _viewModel.FocusManagementRequested -= OnFocusManagementRequested;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during MainView cleanup: {ex.Message}");
            }
            finally
            {
                base.OnDetachedFromVisualTree(e);
            }
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            try
            {
                // Unsubscribe from previous ViewModel
                if (_viewModel != null)
                {
                    _viewModel.TriggerLostFocusRequested -= OnTriggerLostFocusRequested;
                    _viewModel.FocusManagementRequested -= OnFocusManagementRequested;
                    UnsubscribeFromViewModelEvents(_viewModel);
                }

                // Subscribe to new ViewModel
                _viewModel = DataContext as MainViewViewModel;
                if (_viewModel != null)
                {
                    _viewModel.TriggerLostFocusRequested += OnTriggerLostFocusRequested;
                    _viewModel.FocusManagementRequested += OnFocusManagementRequested;
                    SubscribeToViewModelEvents(_viewModel);
                    System.Diagnostics.Debug.WriteLine("MainView subscribed to ViewModel events");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling DataContext change: {ex.Message}");
            }
        }

        /// <summary>
        /// Subscribes to ViewModel events for input clearing functionality
        /// </summary>
        private void SubscribeToViewModelEvents(MainViewViewModel viewModel)
        {
            try
            {
                // Subscribe to InventoryTabViewModel events for Advanced button
                if (viewModel.InventoryTabViewModel != null)
                {
                    viewModel.InventoryTabViewModel.AdvancedEntryRequested += OnAdvancedEntryRequested;
                }

                // Subscribe to RemoveItemViewModel events for Advanced button
                if (viewModel.RemoveItemViewModel != null)
                {
                    viewModel.RemoveItemViewModel.AdvancedRemovalRequested += OnAdvancedRemovalRequested;
                }

                // Subscribe to QuickButtonsViewModel events for New button
                if (viewModel.QuickButtonsViewModel != null)
                {
                    viewModel.QuickButtonsViewModel.NewQuickButtonRequested += OnNewQuickButtonRequested;
                }

                // Subscribe to AdvancedInventoryViewModel events for Back button
                if (viewModel.AdvancedInventoryViewModel != null)
                {
                    // Note: BackToNormalCommand is handled through the ViewModel command pattern
                    // We'll need to monitor property changes or implement command notifications
                }

                // Subscribe to AdvancedRemoveViewModel events for Back button
                if (viewModel.AdvancedRemoveViewModel != null)
                {
                    // Note: BackToNormalCommand is handled through the ViewModel command pattern
                    // We'll need to monitor property changes or implement command notifications
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error subscribing to ViewModel events: {ex.Message}");
            }
        }

        /// <summary>
        /// Unsubscribes from ViewModel events
        /// </summary>
        private void UnsubscribeFromViewModelEvents(MainViewViewModel viewModel)
        {
            try
            {
                if (viewModel.InventoryTabViewModel != null)
                {
                    viewModel.InventoryTabViewModel.AdvancedEntryRequested -= OnAdvancedEntryRequested;
                }

                if (viewModel.RemoveItemViewModel != null)
                {
                    viewModel.RemoveItemViewModel.AdvancedRemovalRequested -= OnAdvancedRemovalRequested;
                }

                if (viewModel.QuickButtonsViewModel != null)
                {
                    viewModel.QuickButtonsViewModel.NewQuickButtonRequested -= OnNewQuickButtonRequested;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error unsubscribing from ViewModel events: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles Advanced Entry button click - clears inventory tab inputs and shows progress overlay
        /// </summary>
        private async void OnAdvancedEntryRequested(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Advanced Entry requested - clearing inventory inputs");

                // Show progress overlay for advanced view switching
                var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
                if (progressOverlayService != null)
                {
                    await progressOverlayService.ShowProgressOverlayAsync(
                        title: "Loading Advanced View",
                        statusMessage: "Preparing advanced inventory entry...",
                        isDeterminate: false,
                        cancellable: false
                    );
                }

                ClearInventoryTabInputs();

                // Simulate loading time for advanced view preparation
                await Task.Delay(150);

                // Hide progress overlay
                if (progressOverlayService != null)
                {
                    await progressOverlayService.HideProgressOverlayAsync();
                }
            }
            catch (Exception ex)
            {
                // Hide progress overlay on error
                try
                {
                    var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
                    if (progressOverlayService != null)
                    {
                        await progressOverlayService.HideProgressOverlayAsync();
                    }
                }
                catch (Exception progressEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error hiding progress overlay: {progressEx.Message}");
                }

                System.Diagnostics.Debug.WriteLine($"Error handling Advanced Entry request: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles Advanced Removal button click - clears remove tab inputs and shows progress overlay
        /// </summary>
        private async void OnAdvancedRemovalRequested(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Advanced Removal requested - clearing remove inputs");

                // Show progress overlay for advanced view switching
                var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
                if (progressOverlayService != null)
                {
                    await progressOverlayService.ShowProgressOverlayAsync(
                        title: "Loading Advanced View",
                        statusMessage: "Preparing advanced removal interface...",
                        isDeterminate: false,
                        cancellable: false
                    );
                }

                ClearRemoveTabInputs();

                // Simulate loading time for advanced view preparation
                await Task.Delay(150);

                // Hide progress overlay
                if (progressOverlayService != null)
                {
                    await progressOverlayService.HideProgressOverlayAsync();
                }
            }
            catch (Exception ex)
            {
                // Hide progress overlay on error
                try
                {
                    var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
                    if (progressOverlayService != null)
                    {
                        await progressOverlayService.HideProgressOverlayAsync();
                    }
                }
                catch (Exception progressEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error hiding progress overlay: {progressEx.Message}");
                }

                System.Diagnostics.Debug.WriteLine($"Error handling Advanced Removal request: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles New QuickButton button click - shows the NewQuickButton overlay
        /// </summary>
        private void OnNewQuickButtonRequested(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("New QuickButton requested - showing view");

                // Get the NewQuickButtonOverlayViewModel from DI
                var overlayViewModel = Program.GetService<ViewModels.Overlay.NewQuickButtonOverlayViewModel>();
                if (overlayViewModel != null)
                {
                    // Show the view with the ViewModel
                    ShowNewQuickButtonOverlay(overlayViewModel);

                    // Subscribe to overlay events
                    overlayViewModel.QuickButtonCreated += OnQuickButtonCreated;
                    overlayViewModel.Cancelled += OnNewQuickButtonOverlayClosed;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: NewQuickButtonOverlayViewModel not found in DI container");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling New QuickButton request: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles when the NewQuickButton overlay is closed/cancelled
        /// </summary>
        private void OnNewQuickButtonOverlayClosed(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("NewQuickButton overlay closed - hiding overlay");

                // Hide the overlay
                HideNewQuickButtonOverlay();

                // Unsubscribe from overlay events
                if (sender is ViewModels.Overlay.NewQuickButtonOverlayViewModel overlayViewModel)
                {
                    overlayViewModel.QuickButtonCreated -= OnQuickButtonCreated;
                    overlayViewModel.Cancelled -= OnNewQuickButtonOverlayClosed;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling NewQuickButton overlay close: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles when a new QuickButton is successfully created
        /// </summary>
        private void OnQuickButtonCreated(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("QuickButton created successfully");

                // Hide the overlay
                HideNewQuickButtonOverlay();

                // Refresh the QuickButtons view
                if (_viewModel?.QuickButtonsViewModel != null)
                {
                    _viewModel.QuickButtonsViewModel.RefreshButtonsCommand.Execute(null);
                }

                // Unsubscribe from overlay events
                if (sender is ViewModels.Overlay.NewQuickButtonOverlayViewModel overlayViewModel)
                {
                    overlayViewModel.QuickButtonCreated -= OnQuickButtonCreated;
                    overlayViewModel.Cancelled -= OnNewQuickButtonOverlayClosed;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling QuickButton created: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears inputs when returning from advanced views to normal views
        /// Call this method when BackToNormal commands are executed
        /// </summary>
        public void ClearAdvancedViewInputs()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Clearing inputs when returning from advanced views");

                // Clear advanced inventory inputs
                if (_viewModel?.AdvancedInventoryViewModel != null)
                {
                    _viewModel.AdvancedInventoryViewModel.SelectedPartID = null;
                    _viewModel.AdvancedInventoryViewModel.SelectedOperation = null;
                    _viewModel.AdvancedInventoryViewModel.SelectedLocation = null;
                    _viewModel.AdvancedInventoryViewModel.PartIDText = string.Empty;
                    _viewModel.AdvancedInventoryViewModel.OperationText = string.Empty;
                    _viewModel.AdvancedInventoryViewModel.LocationText = string.Empty;
                    _viewModel.AdvancedInventoryViewModel.Quantity = 1;
                    _viewModel.AdvancedInventoryViewModel.RepeatTimes = 1;
                    _viewModel.AdvancedInventoryViewModel.MultiLocationPartID = null;
                    _viewModel.AdvancedInventoryViewModel.MultiLocationOperation = null;
                    _viewModel.AdvancedInventoryViewModel.MultiLocationQuantity = 1;
                    System.Diagnostics.Debug.WriteLine("Cleared Advanced Inventory inputs");
                }

                // Clear advanced remove inputs
                if (_viewModel?.AdvancedRemoveViewModel != null)
                {
                    // Clear any advanced remove specific inputs
                    System.Diagnostics.Debug.WriteLine("Cleared Advanced Remove inputs");
                }

                // Also clear the corresponding normal tab inputs to prevent SuggestionOverlay triggers
                ClearInventoryTabInputs();
                ClearRemoveTabInputs();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing advanced view inputs: {ex.Message}");
            }
        }

        private async void OnTriggerLostFocusRequested(object? sender, TriggerLostFocusEventArgs e)
        {
            try
            {
                if (e.FocusOnly)
                {
                    System.Diagnostics.Debug.WriteLine($"MainView received request to focus {e.FieldNames.Count} fields on tab {e.TabIndex}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"MainView received request to trigger LostFocus on {e.FieldNames.Count} fields on tab {e.TabIndex}");
                }

                // Use Dispatcher to ensure UI operations happen on the correct thread
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    if (e.FocusOnly)
                    {
                        await FocusOnFieldsAsync(e.FieldNames, e.TabIndex, e.DelayBetweenFields);
                    }
                    else
                    {
                        await TriggerLostFocusOnFieldsAsync(e.FieldNames, e.TabIndex, e.DelayBetweenFields);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling TriggerLostFocusRequested: {ex.Message}");
            }
        }

        private async Task TriggerLostFocusOnFieldsAsync(List<string> fieldNames, int tabIndex, int delayBetweenFields)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Triggering LostFocus events for fields: {string.Join(", ", fieldNames)} on tab {tabIndex}");

                // First, ensure we're on the correct tab if needed
                var tabControl = this.FindControl<TabControl>("MainForm_TabControl");
                if (tabControl != null && tabControl.SelectedIndex != tabIndex)
                {
                    tabControl.SelectedIndex = tabIndex;
                    System.Diagnostics.Debug.WriteLine($"Switched to tab {tabIndex}");

                    // Small delay to allow tab switch to complete
                    await Task.Delay(100);
                }

                // Trigger LostFocus event on each field with delays
                foreach (var fieldName in fieldNames)
                {
                    await TriggerLostFocusOnField(fieldName);

                    // Add delay between fields if specified
                    if (delayBetweenFields > 0)
                    {
                        await Task.Delay(delayBetweenFields);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Completed triggering LostFocus events on all fields");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error triggering LostFocus events: {ex.Message}");
            }
        }

        private async Task FocusOnFieldsAsync(List<string> fieldNames, int tabIndex, int delayBetweenFields)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Focusing on fields: {string.Join(", ", fieldNames)} on tab {tabIndex}");

                // First, ensure we're on the correct tab if needed
                var tabControl = this.FindControl<TabControl>("MainForm_TabControl");
                if (tabControl != null && tabControl.SelectedIndex != tabIndex)
                {
                    tabControl.SelectedIndex = tabIndex;
                    System.Diagnostics.Debug.WriteLine($"Switched to tab {tabIndex}");

                    // Small delay to allow tab switch to complete
                    await Task.Delay(100);
                }

                // Focus on each field with delays
                foreach (var fieldName in fieldNames)
                {
                    await FocusOnField(fieldName);

                    // Add delay between fields if specified
                    if (delayBetweenFields > 0)
                    {
                        await Task.Delay(delayBetweenFields);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Completed focusing on all fields");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error focusing on fields: {ex.Message}");
            }
        }

        private async Task TriggerLostFocusOnField(string fieldName)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Attempting to trigger LostFocus for field: {fieldName}");

                Control? targetControl = null;

                // The TextBox controls are inside the InventoryTabView, so we need to find them differently
                // First get the tab control and the current tab content
                var tabControl = this.FindControl<TabControl>("MainForm_TabControl");
                if (tabControl?.SelectedIndex == 0) // Inventory tab
                {
                    // Find the InventoryTabView content within the tab
                    var inventoryContent = FindControlInVisualTree<UserControl>(this, "InventoryTabView") ??
                                          FindControlByType<UserControl>(this, "InventoryTabView");

                    if (inventoryContent != null)
                    {
                        // Look for controls within the InventoryTabView
                        switch (fieldName.ToLowerInvariant())
                        {
                            case "partid":
                            case "part":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "PartTextBox");
                                break;
                            case "operation":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "OperationTextBox");
                                break;
                            case "quantity":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "QuantityTextBox");
                                break;
                            case "location":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "LocationTextBox");
                                break;
                            case "notes":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "NotesTextBox");
                                break;
                        }
                    }
                }

                if (targetControl == null)
                {
                    // Fallback: try to find control directly in the main view (might work for some cases)
                    switch (fieldName.ToLowerInvariant())
                    {
                        case "partid":
                        case "part":
                            targetControl = this.FindControl<TextBox>("PartTextBox");
                            break;
                        case "operation":
                            targetControl = this.FindControl<TextBox>("OperationTextBox");
                            break;
                        case "quantity":
                            targetControl = this.FindControl<TextBox>("QuantityTextBox");
                            break;
                        case "location":
                            targetControl = this.FindControl<TextBox>("LocationTextBox");
                            break;
                        case "notes":
                            targetControl = this.FindControl<TextBox>("NotesTextBox");
                            break;
                    }
                }

                if (targetControl != null)
                {
                    // Trigger the LostFocus event by first focusing the control, then moving focus away
                    targetControl.Focus();
                    await Task.Delay(50); // Brief delay to ensure focus is established

                    // Move focus away to trigger LostFocus - focus on another control or the parent
                    var parentContainer = targetControl.Parent;
                    if (parentContainer is Control parentControl && parentControl.Focusable)
                    {
                        parentControl.Focus();
                    }
                    else
                    {
                        // Fallback: try to focus the tab control or main container
                        var tabContainer = this.FindControl<TabControl>("MainForm_TabControl");
                        if (tabContainer != null && tabContainer.Focusable)
                        {
                            tabContainer.Focus();
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Successfully triggered LostFocus for field: {fieldName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Could not find control for field: {fieldName}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error triggering LostFocus for field {fieldName}: {ex.Message}");
            }
        }

        private async Task FocusOnField(string fieldName)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Attempting to focus field: {fieldName}");

                Control? targetControl = null;

                // The TextBox controls are inside the InventoryTabView, so we need to find them differently
                // First get the tab control and the current tab content
                var tabControl = this.FindControl<TabControl>("MainForm_TabControl");
                if (tabControl?.SelectedIndex == 0) // Inventory tab
                {
                    // Find the InventoryTabView content within the tab
                    var inventoryContent = FindControlInVisualTree<UserControl>(this, "InventoryTabView") ??
                                          FindControlByType<UserControl>(this, "InventoryTabView");

                    if (inventoryContent != null)
                    {
                        // Look for controls within the InventoryTabView
                        switch (fieldName.ToLowerInvariant())
                        {
                            case "partid":
                            case "part":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "PartTextBox");
                                break;
                            case "operation":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "OperationTextBox");
                                break;
                            case "quantity":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "QuantityTextBox");
                                break;
                            case "location":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "LocationTextBox");
                                break;
                            case "notes":
                                targetControl = FindControlInVisualTree<TextBox>(inventoryContent, "NotesTextBox");
                                break;
                        }
                    }
                }

                if (targetControl == null)
                {
                    // Fallback: try to find control directly in the main view (might work for some cases)
                    switch (fieldName.ToLowerInvariant())
                    {
                        case "partid":
                        case "part":
                            targetControl = this.FindControl<TextBox>("PartTextBox");
                            break;
                        case "operation":
                            targetControl = this.FindControl<TextBox>("OperationTextBox");
                            break;
                        case "quantity":
                            targetControl = this.FindControl<TextBox>("QuantityTextBox");
                            break;
                        case "location":
                            targetControl = this.FindControl<TextBox>("LocationTextBox");
                            break;
                        case "notes":
                            targetControl = this.FindControl<TextBox>("NotesTextBox");
                            break;
                    }
                }

                if (targetControl != null)
                {
                    // Simply focus the control without triggering LostFocus
                    targetControl.Focus();
                    await Task.Delay(50); // Brief delay to ensure focus is established

                    System.Diagnostics.Debug.WriteLine($"Successfully focused field: {fieldName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Could not find control for field: {fieldName}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error focusing field {fieldName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method to find a control in the visual tree by name and type
        /// </summary>
        private T? FindControlInVisualTree<T>(Control parent, string name) where T : Control
        {
            try
            {
                if (parent.Name == name && parent is T typedControl)
                    return typedControl;

                var children = parent.GetVisualChildren().OfType<Control>();
                foreach (var child in children)
                {
                    var result = FindControlInVisualTree<T>(child, name);
                    if (result != null)
                        return result;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Helper method to find a UserControl by class name
        /// </summary>
        private T? FindControlByType<T>(Control parent, string className) where T : Control
        {
            try
            {
                if (parent.GetType().Name.Contains(className, StringComparison.OrdinalIgnoreCase) && parent is T typedControl)
                    return typedControl;

                var children = parent.GetVisualChildren().OfType<Control>();
                foreach (var child in children)
                {
                    var result = FindControlByType<T>(child, className);
                    if (result != null)
                        return result;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Handles focus management requests from the ViewModel
        /// </summary>
        private async void OnFocusManagementRequested(object? sender, FocusManagementEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"MainView received focus management request: {e.FocusType} for tab {e.TabIndex} with {e.DelayMs}ms delay");

                // Get the FocusManagementService from the service provider
                var focusService = Program.GetService<Services.IFocusManagementService>();
                if (focusService == null)
                {
                    System.Diagnostics.Debug.WriteLine("FocusManagementService not available");
                    return;
                }

                // Handle different types of focus requests
                switch (e.FocusType)
                {
                    case FocusRequestType.Startup:
                        System.Diagnostics.Debug.WriteLine("Processing startup focus request");
                        await focusService.SetStartupFocusAsync(this);
                        break;

                    case FocusRequestType.TabSwitch:
                        System.Diagnostics.Debug.WriteLine($"Processing tab switch focus request for tab {e.TabIndex}");
                        await focusService.SetTabSwitchFocusAsync(this, e.TabIndex);
                        break;

                    case FocusRequestType.ViewSwitch:
                        System.Diagnostics.Debug.WriteLine("Processing view switch focus request");
                        await focusService.SetInitialFocusAsync(this, e.DelayMs);
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine($"Unknown focus request type: {e.FocusType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling focus management request: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles tab selection changes to clear input fields and prevent SuggestionOverlay triggers
        /// Shows progress overlay during view switching operations
        /// </summary>
        private async void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            try
            {
                var tabControl = sender as TabControl;
                if (tabControl == null || _viewModel == null) return;

                var newIndex = tabControl.SelectedIndex;
                var oldIndex = e.RemovedItems?.Count > 0 ? tabControl.Items.IndexOf(e.RemovedItems[0]!) : -1;

                System.Diagnostics.Debug.WriteLine($"Tab changed from {oldIndex} to {newIndex} - ensuring tab switch flag is set");

                // Get progress overlay service and show switching indicator
                var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
                if (progressOverlayService != null)
                {
                    var tabNames = new[] { "Inventory", "Remove", "Transfer" };
                    var targetTabName = newIndex >= 0 && newIndex < tabNames.Length ? tabNames[newIndex] : "View";

                    // Show progress overlay for view switching
                    await progressOverlayService.ShowProgressOverlayAsync(
                        title: "Switching View",
                        statusMessage: $"Loading {targetTabName} tab...",
                        isDeterminate: false,
                        cancellable: false
                    );
                }

                // Ensure the tab switch flag is set (in case earlier events didn't trigger)
                IsTabSwitchInProgress = true;

                // CRITICAL: Clear inputs from ALL tabs immediately to prevent SuggestionOverlay triggers
                // This happens BEFORE any focus events can trigger SuggestionOverlay
                ClearAllTabInputsImmediate();

                // Simulate brief loading time for view switching (real operations would be actual content loading)
                await Task.Delay(200);

                // Hide progress overlay
                if (progressOverlayService != null)
                {
                    await progressOverlayService.HideProgressOverlayAsync();
                }

                // Final cleanup of the flag after tab change completes (don't await to prevent blocking)
                _ = Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await Task.Delay(400); // Allow final focus events to complete
                    IsTabSwitchInProgress = false;
                    System.Diagnostics.Debug.WriteLine("FINAL: Tab switch flag cleared after selection change");
                });
            }
            catch (Exception ex)
            {
                IsTabSwitchInProgress = false; // Ensure flag is cleared on error

                // Hide progress overlay on error
                try
                {
                    var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
                    if (progressOverlayService != null)
                    {
                        await progressOverlayService.HideProgressOverlayAsync();
                    }
                }
                catch (Exception progressEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error hiding progress overlay: {progressEx.Message}");
                }

                System.Diagnostics.Debug.WriteLine($"Error handling tab selection change: {ex.Message}");
            }
        }

        /// <summary>
        /// Immediately clears ALL tab inputs to prevent any SuggestionOverlay triggers during tab switching
        /// This is called before focus events can process and trigger SuggestionOverlay
        /// </summary>
        private void ClearAllTabInputsImmediate()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("IMMEDIATE: Clearing all tab inputs to prevent SuggestionOverlay");

                // Clear ALL tab inputs immediately - don't wait to determine which tab
                ClearInventoryTabInputs();
                ClearRemoveTabInputs();
                ClearTransferTabInputs();

                System.Diagnostics.Debug.WriteLine("IMMEDIATE: All tab inputs cleared successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in immediate tab input clearing: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all input fields for the specified tab to prevent SuggestionOverlay triggers
        /// </summary>
        /// <param name="tabIndex">The index of the tab whose inputs should be cleared</param>
        private void ClearTabInputs(int tabIndex)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Clearing inputs for tab {tabIndex}");

                switch (tabIndex)
                {
                    case 0: // Inventory Tab
                        ClearInventoryTabInputs();
                        break;
                    case 1: // Remove Tab
                        ClearRemoveTabInputs();
                        break;
                    case 2: // Transfer Tab
                        ClearTransferTabInputs();
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"Unknown tab index: {tabIndex}");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing tab inputs: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all input fields in the Inventory tab
        /// </summary>
        private void ClearInventoryTabInputs()
        {
            try
            {
                if (_viewModel?.InventoryTabViewModel != null)
                {
                    _viewModel.InventoryTabViewModel.SelectedPart = string.Empty;
                    _viewModel.InventoryTabViewModel.SelectedOperation = string.Empty;
                    _viewModel.InventoryTabViewModel.SelectedLocation = string.Empty;
                    _viewModel.InventoryTabViewModel.Quantity = 0;
                    _viewModel.InventoryTabViewModel.Notes = string.Empty;
                    _viewModel.InventoryTabViewModel.PartText = string.Empty;
                    _viewModel.InventoryTabViewModel.OperationText = string.Empty;
                    _viewModel.InventoryTabViewModel.LocationText = string.Empty;
                    _viewModel.InventoryTabViewModel.QuantityText = string.Empty;
                    System.Diagnostics.Debug.WriteLine("Cleared Inventory tab inputs");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing Inventory tab inputs: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all input fields in the Remove tab unless edit dialog is open
        /// </summary>
        private void ClearRemoveTabInputs()
        {
            try
            {
                if (_viewModel?.RemoveItemViewModel != null)
                {
                    // CRITICAL: Preserve search inputs during edit dialog operations
                    // Only clear inputs during actual tab switches, not within-tab operations
                    if (_viewModel.RemoveItemViewModel.IsEditDialogVisible)
                    {
                        System.Diagnostics.Debug.WriteLine("Preserving Remove tab inputs - edit dialog is open");
                        return;
                    }

                    _viewModel.RemoveItemViewModel.SelectedPart = null;
                    _viewModel.RemoveItemViewModel.SelectedOperation = null;
                    _viewModel.RemoveItemViewModel.PartText = string.Empty;
                    _viewModel.RemoveItemViewModel.OperationText = string.Empty;
                    System.Diagnostics.Debug.WriteLine("Cleared Remove tab inputs");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing Remove tab inputs: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all input fields in the Transfer tab
        /// </summary>
        private void ClearTransferTabInputs()
        {
            try
            {
                if (_viewModel?.TransferItemViewModel != null)
                {
                    _viewModel.TransferItemViewModel.SelectedPart = null;
                    _viewModel.TransferItemViewModel.SelectedOperation = null;
                    _viewModel.TransferItemViewModel.SelectedToLocation = null;
                    _viewModel.TransferItemViewModel.PartText = string.Empty;
                    _viewModel.TransferItemViewModel.OperationText = string.Empty;
                    _viewModel.TransferItemViewModel.ToLocationText = string.Empty;
                    _viewModel.TransferItemViewModel.TransferQuantity = 1;
                    System.Diagnostics.Debug.WriteLine("Cleared Transfer tab inputs");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing Transfer tab inputs: {ex.Message}");
            }
        }
    }
}
