using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace MTM_WIP_Application_Avalonia.Views
{
    public partial class MainView : UserControl
    {
        private bool _isInitialized = false;
        
        public MainView()
        {
            InitializeComponent();
            Loaded += OnMainViewLoaded;
            SizeChanged += OnMainViewSizeChanged;
            
            // DEBUG: Add theme diagnostic when view loads
            Loaded += OnMainViewLoadedThemeDebug;
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
    }
}
