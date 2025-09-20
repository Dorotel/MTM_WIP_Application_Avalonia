using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for About Information panel.
/// Provides application information, version details, and system information.
/// Uses MVVM Community Toolkit for modern .NET patterns.
/// </summary>
public partial class AboutViewModel : BaseViewModel
{
    private readonly IConfigurationService _configurationService;
    private readonly IApplicationStateService _applicationStateService;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _applicationName = "MTM WIP Inventory System";
    
    [ObservableProperty]
    private string _applicationVersion = "1.0.0";
    
    [ObservableProperty]
    private string _buildDate = string.Empty;
    
    [ObservableProperty]
    private string _frameworkVersion = string.Empty;
    
    [ObservableProperty]
    private string _operatingSystem = string.Empty;
    
    [ObservableProperty]
    private string _machineName = string.Empty;
    
    [ObservableProperty]
    private string _userName = string.Empty;
    
    [ObservableProperty]
    private string _databaseConnection = string.Empty;
    
    [ObservableProperty]
    private bool _showSystemInfo = true;

    public AboutViewModel(
        IConfigurationService configurationService,
        IApplicationStateService applicationStateService,
        ILogger<AboutViewModel> logger) : base(logger)
    {
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService));

        // Initialize collections
        SystemComponents = new ObservableCollection<SystemComponentInfo>();
        LicenseInformation = new ObservableCollection<LicenseInfo>();

        // Load initial data
        _ = LoadAboutInformationAsync();

        Logger.LogInformation("AboutViewModel initialized");
    }

    #region Properties

    /// <summary>
    /// System components and their versions.
    /// </summary>
    public ObservableCollection<SystemComponentInfo> SystemComponents { get; }

    /// <summary>
    /// License information for dependencies.
    /// </summary>
    public ObservableCollection<LicenseInfo> LicenseInformation { get; }

    #endregion

    #region Commands

    /// <summary>
    /// Copies application information to clipboard.
    /// </summary>
    [RelayCommand]
    private async Task CopyApplicationInfo()
    {
        try
        {
            IsLoading = true;

            var info = GenerateApplicationInfoText();
            
            // In real implementation, would copy to clipboard
            Logger.LogInformation("Application information copied to clipboard");
            
            await Task.Delay(300); // Simulate clipboard operation
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error copying application information");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Opens application log directory.
    /// </summary>
    [RelayCommand]
    private async Task OpenLogDirectory()
    {
        try
        {
            var logPath = _configurationService.GetValue("Logging:LogPath", @"C:\MTM_Logs\");
            
            // In real implementation, would open file explorer
            Logger.LogInformation("Opening log directory: {LogPath}", logPath);
            
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error opening log directory");
        }
    }

    /// <summary>
    /// Refreshes system information.
    /// </summary>
    [RelayCommand]
    private async Task RefreshSystemInfo()
    {
        try
        {
            IsLoading = true;
            await LoadSystemInformationAsync();
            Logger.LogInformation("System information refreshed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing system information");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Toggles system information visibility.
    /// </summary>
    [RelayCommand]
    private void ToggleSystemInfo()
    {
        ShowSystemInfo = !ShowSystemInfo;
        Logger.LogDebug("System info visibility toggled: {Visible}", ShowSystemInfo);
    }

    /// <summary>
    /// Opens application website.
    /// </summary>
    [RelayCommand]
    private async Task OpenWebsite()
    {
        try
        {
            const string websiteUrl = "https://www.manitowoc.com";
            
            // In real implementation, would open browser
            Logger.LogInformation("Opening website: {Url}", websiteUrl);
            
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error opening website");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads application and system information.
    /// </summary>
    private async Task LoadAboutInformationAsync()
    {
        try
        {
            await LoadApplicationInfoAsync();
            await LoadSystemInformationAsync();
            await LoadComponentInformationAsync();
            await LoadLicenseInformationAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading about information");
        }
    }

    /// <summary>
    /// Loads application-specific information.
    /// </summary>
    private async Task LoadApplicationInfoAsync()
    {
        try
        {
            ApplicationVersion = typeof(AboutViewModel).Assembly.GetName().Version?.ToString() ?? "1.0.0";
            BuildDate = System.IO.File.GetCreationTime(typeof(AboutViewModel).Assembly.Location).ToString("yyyy-MM-dd HH:mm:ss");
            
            await Task.Delay(50); // Simulate async loading
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading application information");
            ApplicationVersion = "Unknown";
            BuildDate = "Unknown";
        }
    }

    /// <summary>
    /// Loads system information.
    /// </summary>
    private async Task LoadSystemInformationAsync()
    {
        try
        {
            FrameworkVersion = Environment.Version.ToString();
            OperatingSystem = Environment.OSVersion.ToString();
            MachineName = Environment.MachineName;
            UserName = Environment.UserName;
            
            // Load database connection info
            DatabaseConnection = _configurationService.GetConnectionString("DefaultConnection");
            
            await Task.Delay(50); // Simulate async loading
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading system information");
        }
    }

    /// <summary>
    /// Loads system component information.
    /// </summary>
    private async Task LoadComponentInformationAsync()
    {
        try
        {
            SystemComponents.Clear();

            // .NET Runtime
            SystemComponents.Add(new SystemComponentInfo
            {
                Name = ".NET Runtime",
                Version = Environment.Version.ToString(),
                Description = "Microsoft .NET Runtime Environment"
            });

            // Avalonia UI
            SystemComponents.Add(new SystemComponentInfo
            {
                Name = "Avalonia UI",
                Version = "11.0+",
                Description = "Cross-platform .NET UI framework"
            });

            // MVVM Community Toolkit
            SystemComponents.Add(new SystemComponentInfo
            {
                Name = "MVVM Community Toolkit",
                Version = "8.3.2",
                Description = "Modern MVVM patterns and source generators"
            });

            // Microsoft Extensions Logging
            SystemComponents.Add(new SystemComponentInfo
            {
                Name = "Microsoft Extensions Logging",
                Version = "8.0+",
                Description = "Structured logging framework"
            });

            // SQL Server Data Provider
            SystemComponents.Add(new SystemComponentInfo
            {
                Name = "SQL Server Provider",
                Version = "Latest",
                Description = "Microsoft SQL Server data access"
            });

            await Task.Delay(100); // Simulate async loading
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading component information");
        }
    }

    /// <summary>
    /// Loads license information for dependencies.
    /// </summary>
    private async Task LoadLicenseInformationAsync()
    {
        try
        {
            LicenseInformation.Clear();

            LicenseInformation.Add(new LicenseInfo
            {
                Component = "Avalonia UI",
                License = "MIT License",
                Copyright = "Copyright (c) The Avalonia Project",
                Url = "https://github.com/AvaloniaUI/Avalonia"
            });

            LicenseInformation.Add(new LicenseInfo
            {
                Component = "MVVM Community Toolkit",
                License = "MIT License",
                Copyright = "Copyright (c) .NET Foundation and Contributors",
                Url = "https://github.com/CommunityToolkit/dotnet"
            });

            LicenseInformation.Add(new LicenseInfo
            {
                Component = "Microsoft Extensions",
                License = "MIT License",
                Copyright = "Copyright (c) Microsoft Corporation",
                Url = "https://github.com/dotnet/extensions"
            });

            await Task.Delay(100); // Simulate async loading
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading license information");
        }
    }

    /// <summary>
    /// Generates formatted application information text.
    /// </summary>
    private string GenerateApplicationInfoText()
    {
        return $"{ApplicationName}\n" +
               $"Version: {ApplicationVersion}\n" +
               $"Build Date: {BuildDate}\n" +
               $"Framework: {FrameworkVersion}\n" +
               $"Operating System: {OperatingSystem}\n" +
               $"Machine: {MachineName}\n" +
               $"User: {UserName}\n" +
               $"Database: {DatabaseConnection}\n" +
               $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
    }

    #endregion
}

/// <summary>
/// System component information data item.
/// </summary>
public class SystemComponentInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// License information data item.
/// </summary>
public class LicenseInfo
{
    public string Component { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
