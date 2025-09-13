using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// File selection service interface for unified file operations.
/// Provides standardized file selection, validation, and panel-aware UI integration.
/// </summary>
public interface IFileSelectionService
{
    /// <summary>
    /// Opens file selection dialog for import operations
    /// </summary>
    /// <param name="options">File selection configuration</param>
    /// <returns>Selected file path or null if cancelled</returns>
    Task<string?> SelectFileForImportAsync(FileSelectionOptions options);

    /// <summary>
    /// Opens file selection dialog for export operations
    /// </summary>
    /// <param name="options">File selection configuration</param>
    /// <returns>Selected file path or null if cancelled</returns>
    Task<string?> SelectLocationForExportAsync(FileSelectionOptions options);

    /// <summary>
    /// Validates file access permissions and existence
    /// </summary>
    /// <param name="filePath">Path to validate</param>
    /// <returns>True if file is accessible</returns>
    Task<bool> ValidateFileAccessAsync(string filePath);

    /// <summary>
    /// Gets detailed file information
    /// </summary>
    /// <param name="filePath">Path to examine</param>
    /// <returns>FileInfo object or null if file doesn't exist</returns>
    Task<FileInfo?> GetFileInfoAsync(string filePath);

    /// <summary>
    /// Shows file selection view in appropriate panel based on context
    /// </summary>
    /// <param name="sourceControl">Control requesting file selection</param>
    /// <param name="options">File selection configuration</param>
    /// <param name="onFileSelected">Callback when file is selected</param>
    Task ShowFileSelectionViewAsync(Control sourceControl, FileSelectionOptions options, Action<string?> onFileSelected);
}

/// <summary>
/// File selection service implementation using Avalonia storage provider.
/// Follows MTM patterns for error handling, logging, and service architecture.
/// </summary>
public class FileSelectionService : IFileSelectionService
{
    private readonly ILogger<FileSelectionService> _logger;
    private readonly INavigationService _navigationService;

    public FileSelectionService(ILogger<FileSelectionService> logger, INavigationService navigationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    /// <inheritdoc />
    public async Task<string?> SelectFileForImportAsync(FileSelectionOptions options)
    {
        try
        {
            _logger.LogDebug("Starting file import selection with title: {Title}", options.Title);

            var topLevel = GetTopLevelFromCurrentView();
            if (topLevel?.StorageProvider == null)
            {
                _logger.LogError("StorageProvider not available for file selection");
                await ErrorHandling.HandleErrorAsync(new InvalidOperationException("File selection not available"), "File selection unavailable", Environment.UserName);
                return null;
            }

            var fileTypes = CreateFileTypes(options.Extensions);
            var startLocation = await GetStartLocationAsync(options.InitialDirectory);

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = options.Title,
                FileTypeFilter = fileTypes,
                SuggestedStartLocation = startLocation,
                AllowMultiple = options.AllowMultipleSelection
            });

            if (files?.Count > 0)
            {
                var selectedPath = files[0].Path.LocalPath;
                _logger.LogInformation("File selected for import: {FilePath}", selectedPath);

                // Validate file access
                if (await ValidateFileAccessAsync(selectedPath))
                {
                    return selectedPath;
                }
                else
                {
                    _logger.LogWarning("Selected file is not accessible: {FilePath}", selectedPath);
                    await ErrorHandling.HandleErrorAsync(new UnauthorizedAccessException("Selected file is not accessible"), "File access validation failed", Environment.UserName);
                    return null;
                }
            }

            _logger.LogDebug("File selection cancelled by user");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during file import selection");
            await ErrorHandling.HandleErrorAsync(ex, "File selection for import", Environment.UserName);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<string?> SelectLocationForExportAsync(FileSelectionOptions options)
    {
        try
        {
            _logger.LogDebug("Starting file export location selection with title: {Title}", options.Title);

            var topLevel = GetTopLevelFromCurrentView();
            if (topLevel?.StorageProvider == null)
            {
                _logger.LogError("StorageProvider not available for file selection");
                await ErrorHandling.HandleErrorAsync(new InvalidOperationException("File selection not available"), "File selection unavailable", Environment.UserName);
                return null;
            }

            var fileTypes = CreateFileTypes(options.Extensions);
            var startLocation = await GetStartLocationAsync(options.InitialDirectory);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = options.Title,
                FileTypeChoices = fileTypes,
                SuggestedStartLocation = startLocation,
                DefaultExtension = GetDefaultExtension(options.Extensions),
                ShowOverwritePrompt = true
            });

            if (file != null)
            {
                var selectedPath = file.Path.LocalPath;
                _logger.LogInformation("Export location selected: {FilePath}", selectedPath);

                // Ensure directory exists
                var directory = Path.GetDirectoryName(selectedPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    try
                    {
                        Directory.CreateDirectory(directory);
                        _logger.LogDebug("Created directory: {Directory}", directory);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create directory: {Directory}", directory);
                        await ErrorHandling.HandleErrorAsync(ex, "Directory creation failed", Environment.UserName);
                        return null;
                    }
                }

                return selectedPath;
            }

            _logger.LogDebug("Export location selection cancelled by user");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during file export location selection");
            await ErrorHandling.HandleErrorAsync(ex, "File selection for export", Environment.UserName);
            return null;
        }
    }

    /// <inheritdoc />
    public Task<bool> ValidateFileAccessAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.LogWarning("File path is null or empty");
                return Task.FromResult(false);
            }

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("File does not exist: {FilePath}", filePath);
                return Task.FromResult(false);
            }

            // Test read access
            using var stream = File.OpenRead(filePath);
            var canRead = stream.CanRead;

            _logger.LogDebug("File access validation for {FilePath}: {CanRead}", filePath, canRead);
            return Task.FromResult(canRead);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "File access validation failed for: {FilePath}", filePath);
            return Task.FromResult(false);
        }
    }

    /// <inheritdoc />
    public Task<FileInfo?> GetFileInfoAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                _logger.LogDebug("File does not exist: {FilePath}", filePath);
                return Task.FromResult<FileInfo?>(null);
            }

            var fileInfo = new FileInfo(filePath);
            _logger.LogDebug("Retrieved file info for: {FilePath}, Size: {Size} bytes", filePath, fileInfo.Length);
            
            return Task.FromResult<FileInfo?>(fileInfo);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get file info for: {FilePath}", filePath);
            return Task.FromResult<FileInfo?>(null);
        }
    }

    /// <inheritdoc />
    public async Task ShowFileSelectionViewAsync(Control sourceControl, FileSelectionOptions options, Action<string?> onFileSelected)
    {
        try
        {
            _logger.LogDebug("Showing file selection view for mode: {Mode}", options.Mode);

            // Determine appropriate panel placement
            var placement = DeterminePanelPlacement(sourceControl, options.PreferredPlacement);
            
            // For now, use direct file dialogs - panel implementation will be added in Phase 2
            switch (options.Mode)
            {
                case FileSelectionMode.Import:
                case FileSelectionMode.Load:
                    var importPath = await SelectFileForImportAsync(options);
                    onFileSelected?.Invoke(importPath);
                    break;
                    
                case FileSelectionMode.Export:
                case FileSelectionMode.Save:
                    var exportPath = await SelectLocationForExportAsync(options);
                    onFileSelected?.Invoke(exportPath);
                    break;
                    
                default:
                    _logger.LogWarning("Unknown file selection mode: {Mode}", options.Mode);
                    onFileSelected?.Invoke(null);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing file selection view");
            await ErrorHandling.HandleErrorAsync(ex, "Show file selection view", Environment.UserName);
            onFileSelected?.Invoke(null);
        }
    }

    #region Helper Methods

    private TopLevel? GetTopLevelFromCurrentView()
    {
        // Try to get the current application's main window
        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    private List<FilePickerFileType> CreateFileTypes(string[] extensions)
    {
        var fileTypes = new List<FilePickerFileType>();

        if (extensions.Length > 0)
        {
            // Create specific file type
            var patterns = extensions.Select(ext => ext.StartsWith("*") ? ext : $"*{ext}").ToList();
            var name = GetFileTypeName(extensions);
            
            fileTypes.Add(new FilePickerFileType(name)
            {
                Patterns = patterns
            });
        }

        // Always add "All Files" option
        fileTypes.Add(FilePickerFileTypes.All);

        return fileTypes;
    }

    private string GetFileTypeName(string[] extensions)
    {
        if (extensions.Length == 0) return "All Files";
        
        if (extensions.Any(ext => ext.Contains("json", StringComparison.OrdinalIgnoreCase)))
            return "JSON Files";
        if (extensions.Any(ext => ext.Contains("xml", StringComparison.OrdinalIgnoreCase)))
            return "XML Files";
        if (extensions.Any(ext => ext.Contains("txt", StringComparison.OrdinalIgnoreCase)))
            return "Text Files";
        if (extensions.Any(ext => ext.Contains("csv", StringComparison.OrdinalIgnoreCase)))
            return "CSV Files";

        return "Configuration Files";
    }

    private string? GetDefaultExtension(string[] extensions)
    {
        if (extensions.Length > 0)
        {
            var ext = extensions[0];
            return ext.StartsWith("*") ? ext.Substring(1) : ext;
        }
        return null;
    }

    private async Task<IStorageFolder?> GetStartLocationAsync(string initialDirectory)
    {
        try
        {
            var topLevel = GetTopLevelFromCurrentView();
            if (topLevel?.StorageProvider == null) return null;

            if (!string.IsNullOrWhiteSpace(initialDirectory) && Directory.Exists(initialDirectory))
            {
                return await topLevel.StorageProvider.TryGetFolderFromPathAsync(initialDirectory);
            }

            // Fallback to Documents folder
            return await topLevel.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get start location: {Directory}", initialDirectory);
            return null;
        }
    }

    private PanelPlacement DeterminePanelPlacement(Control sourceControl, PanelPlacement preferred)
    {
        if (preferred != PanelPlacement.Auto) return preferred;
        
        // Walk up visual tree to find context
        var parent = sourceControl;
        while (parent != null)
        {
            var typeName = parent.GetType().Name;
            if (typeName.Contains("MainView")) return PanelPlacement.LeftPanel;
            if (typeName.Contains("SettingsView") || typeName.Contains("SettingsForm")) return PanelPlacement.RightPanel;
            parent = parent.Parent as Control;
        }
        
        return PanelPlacement.Overlay; // Fallback
    }

    #endregion
}

/// <summary>
/// Configuration options for file selection operations
/// </summary>
public class FileSelectionOptions
{
    public string Title { get; set; } = "Select File";
    public string[] Extensions { get; set; } = new[] { "*.json" };
    public string InitialDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public FileSelectionMode Mode { get; set; } = FileSelectionMode.Import;
    public PanelPlacement PreferredPlacement { get; set; } = PanelPlacement.Auto;
    public bool AllowMultipleSelection { get; set; } = false;
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB default
}

/// <summary>
/// File selection operation modes
/// </summary>
public enum FileSelectionMode
{
    Import,
    Export,
    Save,
    Load
}

/// <summary>
/// Panel placement options for file selection UI
/// </summary>
public enum PanelPlacement
{
    Auto,       // Automatically determine based on calling context
    LeftPanel,  // Force left panel (MainView tab area)
    RightPanel, // Force right panel (SettingsForm content area)
    Overlay     // Use overlay if no panel space available
}