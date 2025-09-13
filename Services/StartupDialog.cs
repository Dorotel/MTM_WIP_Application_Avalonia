using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.Extensions.Configuration;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Service for displaying startup information dialog.
/// Shows current user information and log file locations.
/// </summary>
public static class StartupDialog
{
    /// <summary>
    /// Displays startup information including username and log path.
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    public static async Task ShowStartupInfoAsync(IConfiguration configuration)
    {
        try
        {
            var username = Environment.UserName.ToUpper(); // Convert to uppercase
            var logPath = GetLogPath(configuration);
            var appName = configuration["MTM:ApplicationName"] ?? "MTM WIP Application";
            var version = configuration["MTM:Version"] ?? "1.0.0";

            var message = $"""
                {appName} v{version}
                
                Current User: {username}
                
                Primary Log Location:
                {logPath}
                
                Fallback Log Location:
                {GetFallbackLogPath()}
                
                Application ready to use.
                """;

            await ShowStartupMessageAsync("MTM WIP Application - Startup Information", message);
        }
        catch (Exception ex)
        {
            // Fallback message if there's an error getting configuration
            var fallbackMessage = $"""
                MTM WIP Application - Startup Information
                
                Current User: {Environment.UserName.ToUpper()}
                
                Error loading configuration: {ex.Message}
                
                Application will continue with default settings.
                """;

            await ShowStartupMessageAsync("MTM WIP Application - Startup Information", fallbackMessage);
        }
    }

    /// <summary>
    /// Gets the primary log path from configuration.
    /// </summary>
    private static string GetLogPath(IConfiguration configuration)
    {
        // Get from configuration
        var configuredPath = configuration["ErrorHandling:FileServerPath"] ??
                           configuration["Logging:File:BasePath"];

        if (!string.IsNullOrEmpty(configuredPath))
        {
            return Path.Combine(configuredPath, Environment.UserName.ToUpper());
        }

        // Default fallback
        return Path.Combine(@"\\mtmanu-fs01\Expo Drive\MH_RESOURCE\Material_Handler\MTM WIP App\Logs", Environment.UserName.ToUpper());
    }

    /// <summary>
    /// Gets the fallback local log path.
    /// </summary>
    private static string GetFallbackLogPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "MTM WIP Application",
            "Logs",
            Environment.UserName.ToUpper()
        );
    }

    /// <summary>
    /// Shows a startup message using a custom Avalonia window.
    /// </summary>
    private static async Task ShowStartupMessageAsync(string title, string message)
    {
        try
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var messageWindow = new StartupInfoWindow(title, message);
                await messageWindow.ShowDialog(desktop.MainWindow!);
            }
        }
        catch (Exception ex)
        {
            // Fallback to console output if dialog fails
            Console.WriteLine($"{title}:");
            Console.WriteLine(message);
            Console.WriteLine($"Dialog Error: {ex.Message}");
        }
    }
}

/// <summary>
/// Custom startup information window.
/// </summary>
public class StartupInfoWindow : Window
{
    private readonly string _messageText;

    public StartupInfoWindow(string title, string messageText)
    {
        _messageText = messageText;
        Title = title;
        
        Width = 600;
        Height = 400;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        CanResize = false;
        ShowInTaskbar = false;
        
        // Apply MTM styling using theme resources
        Background = Application.Current?.FindResource("MTM_Shared_Logic.CardBackgroundBrush") as IBrush ?? 
                    new SolidColorBrush(Color.FromRgb(250, 250, 250));
        
        InitializeContent();
    }

    private void InitializeContent()
    {
        // Create the main DockPanel
        var dockPanel = new DockPanel();

        // Header with MTM branding
        var headerPanel = new Border
        {
            Background = Application.Current?.FindResource("MTM_Shared_Logic.PrimaryAction") as IBrush ?? 
                        new SolidColorBrush(Color.FromRgb(106, 13, 173)),
            Height = 60,
            [DockPanel.DockProperty] = Dock.Top
        };

        var headerText = new TextBlock
        {
            Text = "MTM WIP Application",
            Foreground = Application.Current?.FindResource("MTM_Shared_Logic.OverlayTextBrush") as IBrush ?? Brushes.White,
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        headerPanel.Child = headerText;

        // Button panel
        var buttonPanel = new Border
        {
            Height = 70,
            Background = Application.Current?.FindResource("MTM_Shared_Logic.PanelBackgroundBrush") as IBrush ?? 
                       new SolidColorBrush(Color.FromRgb(240, 240, 240)),
            [DockPanel.DockProperty] = Dock.Bottom
        };

        var okButton = new Button
        {
            Content = "OK",
            Width = 100,
            Height = 35,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Background = Application.Current?.FindResource("MTM_Shared_Logic.PrimaryAction") as IBrush ?? 
                        new SolidColorBrush(Color.FromRgb(106, 13, 173)),
            Foreground = Application.Current?.FindResource("MTM_Shared_Logic.OverlayTextBrush") as IBrush ?? Brushes.White,
            FontWeight = FontWeight.SemiBold
        };
        okButton.Click += (s, e) => Close();
        buttonPanel.Child = okButton;

        // Content area (fills remaining space)
        var contentScrollViewer = new ScrollViewer
        {
            [DockPanel.DockProperty] = Dock.Top // This will fill the remaining space
        };

        var contentPanel = new StackPanel
        {
            Margin = new Thickness(30),
            Spacing = 20
        };

        // Message text - now using the constructor parameter
        var messageBlock = new TextBlock
        {
            Text = _messageText,
            FontSize = 14,
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 22,
            Foreground = Application.Current?.FindResource("MTM_Shared_Logic.BodyText") as IBrush ?? 
                       new SolidColorBrush(Color.FromRgb(51, 51, 51))
        };
        contentPanel.Children.Add(messageBlock);
        contentScrollViewer.Content = contentPanel;

        // Add panels to dock panel in correct order
        dockPanel.Children.Add(headerPanel);
        dockPanel.Children.Add(buttonPanel);
        dockPanel.Children.Add(contentScrollViewer);

        Content = dockPanel;
    }
}
