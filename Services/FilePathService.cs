using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Centralized file path management service for the MTM WIP Application.
/// Provides organized folder structure in MyDocuments (non-OneDrive) with hierarchical organization.
/// Replaces %AppData% usage with MyDocuments/MTM WIP Application/{File Type}/{Sub Type}/{Sub Sub Type}/{File}
/// </summary>
public interface IFilePathService
{
    string GetApplicationBasePath();
    string GetQuickButtonsPath();
    string GetQuickButtonsExportPath();
    string GetQuickButtonsImportPath();
    string GetLogsPath();
    string GetThemesPath();
    string GetExportsPath();
    string GetImportsPath();
    string GetReportsPath();
    string GetConfigurationPath();
    string GetBackupsPath();
    void EnsureDirectoryExists(string path);
    string GetUniqueFileName(string basePath, string fileName, string extension);
}

/// <summary>
/// Implementation of centralized file path management for MTM WIP Application.
/// Creates organized folder structure: MyDocuments/MTM WIP Application/{Category}/{SubCategory}/{SubSubCategory}/
/// </summary>
public class FilePathService : IFilePathService
{
    private readonly string _applicationBasePath;
    
    public FilePathService()
    {
        // Use local Documents instead of OneDrive Documents - specific to avoid cloud sync
        // MyDocuments can redirect to OneDrive, so we need to use the actual local path
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var localDocuments = Path.Combine(userProfile, "Documents");
        _applicationBasePath = Path.Combine(localDocuments, "MTM WIP Application");
        
        // Ensure base directory exists
        EnsureDirectoryExists(_applicationBasePath);
    }

    /// <summary>
    /// Gets the root application directory in MyDocuments
    /// </summary>
    public string GetApplicationBasePath()
    {
        return _applicationBasePath;
    }

    /// <summary>
    /// Gets the QuickButtons data storage path
    /// Structure: MyDocuments/MTM WIP Application/Configuration/QuickButtons/
    /// </summary>
    public string GetQuickButtonsPath()
    {
        var path = Path.Combine(_applicationBasePath, "Configuration", "QuickButtons");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the QuickButtons export path
    /// Structure: MyDocuments/MTM WIP Application/Exports/QuickButtons/
    /// </summary>
    public string GetQuickButtonsExportPath()
    {
        var path = Path.Combine(_applicationBasePath, "Exports", "QuickButtons");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the QuickButtons import path (for user-provided files)
    /// Structure: MyDocuments/MTM WIP Application/Imports/QuickButtons/
    /// </summary>
    public string GetQuickButtonsImportPath()
    {
        var path = Path.Combine(_applicationBasePath, "Imports", "QuickButtons");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the application logs path
    /// Structure: MyDocuments/MTM WIP Application/Logs/
    /// </summary>
    public string GetLogsPath()
    {
        var path = Path.Combine(_applicationBasePath, "Logs");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the themes storage path
    /// Structure: MyDocuments/MTM WIP Application/Themes/Custom/
    /// </summary>
    public string GetThemesPath()
    {
        var path = Path.Combine(_applicationBasePath, "Themes", "Custom");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the general exports path
    /// Structure: MyDocuments/MTM WIP Application/Exports/
    /// </summary>
    public string GetExportsPath()
    {
        var path = Path.Combine(_applicationBasePath, "Exports");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the general imports path
    /// Structure: MyDocuments/MTM WIP Application/Imports/
    /// </summary>
    public string GetImportsPath()
    {
        var path = Path.Combine(_applicationBasePath, "Imports");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the reports export path
    /// Structure: MyDocuments/MTM WIP Application/Exports/Reports/
    /// </summary>
    public string GetReportsPath()
    {
        var path = Path.Combine(_applicationBasePath, "Exports", "Reports");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the configuration files path
    /// Structure: MyDocuments/MTM WIP Application/Configuration/
    /// </summary>
    public string GetConfigurationPath()
    {
        var path = Path.Combine(_applicationBasePath, "Configuration");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Gets the backups path
    /// Structure: MyDocuments/MTM WIP Application/Backups/
    /// </summary>
    public string GetBackupsPath()
    {
        var path = Path.Combine(_applicationBasePath, "Backups");
        EnsureDirectoryExists(path);
        return path;
    }

    /// <summary>
    /// Ensures the specified directory exists, creating it if necessary
    /// </summary>
    public void EnsureDirectoryExists(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        catch (Exception ex)
        {
            // Can't log here to avoid circular dependency - just rethrow
            throw new InvalidOperationException($"Failed to create directory: {path}", ex);
        }
    }

    /// <summary>
    /// Gets a unique filename by appending a number if the file already exists
    /// </summary>
    public string GetUniqueFileName(string basePath, string fileName, string extension)
    {
        var baseFileName = Path.GetFileNameWithoutExtension(fileName);
        extension = extension.StartsWith(".") ? extension : "." + extension;
        
        var fullPath = Path.Combine(basePath, baseFileName + extension);
        
        if (!File.Exists(fullPath))
        {
            return fullPath;
        }

        int counter = 1;
        do
        {
            var uniqueFileName = $"{baseFileName}_{counter:000}{extension}";
            fullPath = Path.Combine(basePath, uniqueFileName);
            counter++;
        }
        while (File.Exists(fullPath) && counter < 1000);

        return fullPath;
    }
}

/// <summary>
/// Extension methods for additional file path utilities
/// </summary>
public static class FilePathExtensions
{
    /// <summary>
    /// Gets a timestamped filename for exports
    /// </summary>
    public static string GetTimestampedFileName(this IFilePathService filePathService, string baseName, string extension)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{baseName}_{timestamp}";
        extension = extension.StartsWith(".") ? extension : "." + extension;
        return fileName + extension;
    }

    /// <summary>
    /// Gets a user-specific filename
    /// </summary>
    public static string GetUserSpecificFileName(this IFilePathService filePathService, string baseName, string extension)
    {
        var userName = Environment.UserName.ToLowerInvariant();
        var fileName = $"{baseName}_{userName}";
        extension = extension.StartsWith(".") ? extension : "." + extension;
        return fileName + extension;
    }
}
