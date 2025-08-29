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
