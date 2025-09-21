using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Core;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services.Infrastructure;

#region File Selection Service

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
    /// <param name="sourceControl">Control that initiated the request</param>
    /// <param name="selectionType">Type of file selection operation</param>
    /// <param name="options">File selection options</param>
    Task<string?> ShowFileSelectionViewAsync(Control? sourceControl, FileSelectionType selectionType, FileSelectionOptions options);
}

/// <summary>
/// File selection configuration options
/// </summary>
public class FileSelectionOptions
{
    public string Title { get; set; } = "Select File";
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
    public string? DefaultFileName { get; set; }
    public string? InitialDirectory { get; set; }
    public bool AllowMultiple { get; set; } = false;
}

/// <summary>
/// File selection operation types
/// </summary>
public enum FileSelectionType
{
    Import,
    Export,
    Open,
    Save
}

/// <summary>
/// File selection service implementation using Avalonia storage provider.
/// Follows MTM patterns for error handling, logging, and service architecture.
/// </summary>
public class FileSelectionService : IFileSelectionService
{
    private readonly ILogger<FileSelectionService> _logger;

    public FileSelectionService(ILogger<FileSelectionService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("FileSelectionService constructed successfully");
    }

    /// <inheritdoc />
    public async Task<string?> SelectFileForImportAsync(FileSelectionOptions options)
    {
        try
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider == null)
            {
                _logger.LogError("Storage provider not available");
                return null;
            }

            var fileTypes = CreateFileTypes(options.AllowedExtensions);
            var startLocation = await GetStartLocationAsync(options.InitialDirectory);

            var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = options.Title,
                FileTypeFilter = fileTypes,
                SuggestedStartLocation = startLocation,
                AllowMultiple = options.AllowMultiple
            });

            if (result?.Count > 0)
            {
                var selectedPath = result[0].Path.LocalPath;
                _logger.LogInformation("File selected for import: {FilePath}", selectedPath);
                return selectedPath;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to select file for import");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<string?> SelectLocationForExportAsync(FileSelectionOptions options)
    {
        try
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider == null)
            {
                _logger.LogError("Storage provider not available");
                return null;
            }

            var fileTypes = CreateFileTypes(options.AllowedExtensions);
            var startLocation = await GetStartLocationAsync(options.InitialDirectory);
            var defaultExtension = GetDefaultExtension(options.AllowedExtensions);

            var result = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = options.Title,
                FileTypeChoices = fileTypes,
                SuggestedStartLocation = startLocation,
                SuggestedFileName = options.DefaultFileName,
                DefaultExtension = defaultExtension
            });

            if (result != null)
            {
                var selectedPath = result.Path.LocalPath;
                _logger.LogInformation("Export location selected: {FilePath}", selectedPath);
                return selectedPath;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to select export location");
            return null;
        }
    }

    /// <inheritdoc />
    public Task<bool> ValidateFileAccessAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.FromResult(false);

            var fileInfo = new FileInfo(filePath);
            var isValid = fileInfo.Exists && !fileInfo.Attributes.HasFlag(FileAttributes.Hidden);

            _logger.LogDebug("File access validation for {FilePath}: {IsValid}", filePath, isValid);
            return Task.FromResult(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate file access for {FilePath}", filePath);
            return Task.FromResult(false);
        }
    }

    /// <inheritdoc />
    public Task<FileInfo?> GetFileInfoAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.FromResult<FileInfo?>(null);

            var fileInfo = new FileInfo(filePath);
            return Task.FromResult(fileInfo.Exists ? fileInfo : null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file info for {FilePath}", filePath);
            return Task.FromResult<FileInfo?>(null);
        }
    }

    /// <inheritdoc />
    public async Task<string?> ShowFileSelectionViewAsync(Control? sourceControl, FileSelectionType selectionType, FileSelectionOptions options)
    {
        return selectionType switch
        {
            FileSelectionType.Import or FileSelectionType.Open => await SelectFileForImportAsync(options),
            FileSelectionType.Export or FileSelectionType.Save => await SelectLocationForExportAsync(options),
            _ => throw new ArgumentOutOfRangeException(nameof(selectionType), selectionType, null)
        };
    }

    private IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.StorageProvider;
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

    private async Task<IStorageFolder?> GetStartLocationAsync(string? initialDirectory)
    {
        if (string.IsNullOrWhiteSpace(initialDirectory))
            return null;

        try
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider != null)
            {
                return await storageProvider.TryGetFolderFromPathAsync(initialDirectory);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get start location for directory: {Directory}", initialDirectory);
        }

        return null;
    }
}

#endregion
