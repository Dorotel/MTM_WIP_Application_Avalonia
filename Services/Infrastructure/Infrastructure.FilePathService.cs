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

#region File Path Service

/// <summary>
/// Service for file path operations and management
/// </summary>
public interface IFilePathService
{
    /// <summary>
    /// Gets application data directory path
    /// </summary>
    string GetAppDataPath();

    /// <summary>
    /// Gets user documents directory path
    /// </summary>
    string GetDocumentsPath();

    /// <summary>
    /// Ensures directory exists, creates if necessary
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <returns>True if directory exists or was created successfully</returns>
    bool EnsureDirectoryExists(string path);

    /// <summary>
    /// Gets temp directory path for the application
    /// </summary>
    string GetTempPath();

    /// <summary>
    /// Combines path segments safely
    /// </summary>
    string CombinePath(params string[] pathSegments);

    /// <summary>
    /// Validates if path is safe to use
    /// </summary>
    bool IsPathSafe(string path);
}

/// <summary>
/// File path service implementation
/// </summary>
public class FilePathService : IFilePathService
{
    private readonly ILogger<FilePathService> _logger;

    public FilePathService(ILogger<FilePathService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("FilePathService constructed successfully");
    }

    public string GetAppDataPath()
    {
        try
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTM_WIP_Application");
            EnsureDirectoryExists(path);
            return path;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get application data path");
            return Path.GetTempPath();
        }
    }

    public string GetDocumentsPath()
    {
        try
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get documents path");
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }

    public bool EnsureDirectoryExists(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _logger.LogDebug("Created directory: {Path}", path);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure directory exists: {Path}", path);
            return false;
        }
    }

    public string GetTempPath()
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "MTM_WIP_Application");
            EnsureDirectoryExists(tempPath);
            return tempPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get temp path");
            return Path.GetTempPath();
        }
    }

    public string CombinePath(params string[] pathSegments)
    {
        try
        {
            if (pathSegments == null || pathSegments.Length == 0)
                return string.Empty;

            return Path.Combine(pathSegments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to combine path segments");
            return string.Empty;
        }
    }

    public bool IsPathSafe(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // Check for invalid characters
            var invalidChars = Path.GetInvalidPathChars();
            if (path.Any(c => invalidChars.Contains(c)))
                return false;

            // Check for path traversal attempts
            var normalizedPath = Path.GetFullPath(path);
            return !normalizedPath.Contains("..");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Path safety check failed for: {Path}", path);
            return false;
        }
    }
}

#endregion
