using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// Utility class for handling all logging operations including file server CSV logging
    /// and MySQL database logging. Manages user-specific log folders and file creation.
    /// </summary>
    public static class LoggingUtility
    {
        private static string _fileServerBasePath = @"\\FileServer\Logs"; // TODO: Configure from settings
        private static readonly object _fileLockObject = new();
        
        /// <summary>
        /// Logs an error entry to the file server in CSV format.
        /// Creates user-specific folders and category-specific CSV files automatically.
        /// </summary>
        /// <param name="errorEntry">The error entry to log</param>
        public static void LogToFileServer(ErrorEntry errorEntry)
        {
            try
            {
                if (!ErrorHandlingConfiguration.EnableFileServerLogging)
                    return;

                var userLogFolder = Path.Combine(_fileServerBasePath, errorEntry.UserId);
                var csvFileName = GetCsvFileName(errorEntry.Category);
                var csvFilePath = Path.Combine(userLogFolder, csvFileName);
                
                EnsureUserLogFolderExists(userLogFolder);
                
                lock (_fileLockObject)
                {
                    var fileExists = File.Exists(csvFilePath);
                    using var writer = new StreamWriter(csvFilePath, append: true);
                    
                    if (!fileExists)
                    {
                        writer.WriteLine(GetCsvHeader());
                    }
                    
                    writer.WriteLine(FormatErrorEntryAsCsv(errorEntry));
                }
            }
            catch (Exception ex)
            {
                // Fallback to local logging when file server logging fails
                LogToFallbackLocation(errorEntry, ex);
            }
        }

        /// <summary>
        /// Logs an error entry to the MySQL database.
        /// Uses category-specific tables with consistent structure.
        /// </summary>
        /// <param name="errorEntry">The error entry to log</param>
        public static async Task LogToMySQL(ErrorEntry errorEntry)
        {
            try
            {
                if (!ErrorHandlingConfiguration.EnableMySqlLogging || 
                    string.IsNullOrWhiteSpace(ErrorHandlingConfiguration.MySqlConnectionString))
                    return;

                var tableName = GetMySqlTableName(errorEntry.Category);
                
                using var connection = new MySqlConnection(ErrorHandlingConfiguration.MySqlConnectionString);
                await connection.OpenAsync();
                
                // Ensure table exists
                await EnsureTableExists(connection, tableName);
                
                // Insert error entry
                var insertSql = $@"
                    INSERT INTO {tableName} 
                    (Timestamp, UserId, MachineName, Category, Severity, ErrorMessage, 
                     FileName, MethodName, LineNumber, StackTrace, source, 
                     AdditionalData, ExceptionType)
                    VALUES 
                    (@Timestamp, @UserId, @MachineName, @Category, @Severity, @ErrorMessage,
                     @FileName, @MethodName, @LineNumber, @StackTrace, @source,
                     @AdditionalData, @ExceptionType)";

                using var command = new MySqlCommand(insertSql, connection);
                AddMySqlParameters(command, errorEntry);
                
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                LogApplicationError(ex);
            }
        }

        /// <summary>
        /// Ensures the MySQL table exists for the specified category.
        /// </summary>
        private static async Task EnsureTableExists(MySqlConnection connection, string tableName)
        {
            var createTableSql = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Timestamp DATETIME NOT NULL,
                    UserId VARCHAR(255) NOT NULL,
                    MachineName VARCHAR(255) NOT NULL,
                    Category VARCHAR(50) NOT NULL,
                    Severity VARCHAR(20) NOT NULL,
                    ErrorMessage TEXT,
                    FileName VARCHAR(500),
                    MethodName VARCHAR(255),
                    LineNumber INT,
                    StackTrace TEXT,
                    source VARCHAR(255),
                    AdditionalData TEXT,
                    ExceptionType VARCHAR(500),
                    INDEX idx_timestamp (Timestamp),
                    INDEX idx_userid (UserId),
                    INDEX idx_severity (Severity)
                )";

            using var command = new MySqlCommand(createTableSql, connection);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Adds parameters to the MySQL command for error entry insertion.
        /// </summary>
        private static void AddMySqlParameters(MySqlCommand command, ErrorEntry errorEntry)
        {
            command.Parameters.AddWithValue("@Timestamp", errorEntry.Timestamp);
            command.Parameters.AddWithValue("@UserId", errorEntry.UserId);
            command.Parameters.AddWithValue("@MachineName", errorEntry.MachineName);
            command.Parameters.AddWithValue("@Category", errorEntry.Category.ToString());
            command.Parameters.AddWithValue("@Severity", errorEntry.Severity.ToString());
            command.Parameters.AddWithValue("@ErrorMessage", errorEntry.ErrorMessage);
            command.Parameters.AddWithValue("@FileName", errorEntry.FileName);
            command.Parameters.AddWithValue("@MethodName", errorEntry.MethodName);
            command.Parameters.AddWithValue("@LineNumber", errorEntry.LineNumber);
            command.Parameters.AddWithValue("@StackTrace", errorEntry.StackTrace);
            command.Parameters.AddWithValue("@source", errorEntry.source ?? "");
            command.Parameters.AddWithValue("@AdditionalData", errorEntry.AdditionalData);
            command.Parameters.AddWithValue("@ExceptionType", errorEntry.ExceptionType);
        }

        /// <summary>
        /// Logs to fallback location when primary logging fails.
        /// </summary>
        private static void LogToFallbackLocation(ErrorEntry errorEntry, Exception originalException)
        {
            try
            {
                var fallbackPath = Path.Combine(ErrorHandlingConfiguration.FallbackLocalPath, 
                    errorEntry.UserId, GetCsvFileName(errorEntry.Category));
                
                Directory.CreateDirectory(Path.GetDirectoryName(fallbackPath)!);
                
                var fileExists = File.Exists(fallbackPath);
                using var writer = new StreamWriter(fallbackPath, append: true);
                
                if (!fileExists)
                {
                    writer.WriteLine(GetCsvHeader());
                }
                
                writer.WriteLine(FormatErrorEntryAsCsv(errorEntry));
                
                // Also log the original exception that caused the fallback
                LogApplicationError(originalException);
            }
            catch (Exception fallbackEx)
            {
                Console.WriteLine($"Critical: Both primary and fallback logging failed. Original: {originalException.Message}, Fallback: {fallbackEx.Message}");
            }
        }

        /// <summary>
        /// Logs application-level errors that occur within the logging system itself.
        /// Uses a simplified logging approach to avoid recursive failures.
        /// </summary>
        /// <param name="exception">The exception that occurred in the logging system</param>
        public static void LogApplicationError(Exception exception)
        {
            try
            {
                // TODO: Implement minimal fallback logging
                // - Use local file system if file server is unavailable
                // - Write to application event log as last resort
                // - Avoid complex operations that could cause recursive errors
                
                var fallbackPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                    "MTM_WIP_Application", "Logs", "application_errors.log");
                
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {exception.GetType().Name}: {exception.Message}{Environment.NewLine}";
                
                Directory.CreateDirectory(Path.GetDirectoryName(fallbackPath)!);
                File.AppendAllText(fallbackPath, logEntry);
            }
            catch
            {
                // Last resort - write to console
                Console.WriteLine($"Critical logging failure: {exception.Message}");
            }
        }

        /// <summary>
        /// Gets the appropriate CSV file name based on the error category.
        /// </summary>
        private static string GetCsvFileName(ErrorCategory category)
        {
            return category switch
            {
                ErrorCategory.UI => "ui_errors.csv",
                ErrorCategory.BusinessLogic => "business_logic_errors.csv", 
                ErrorCategory.MySQL => "mysql_errors.csv",
                ErrorCategory.Network => "network_errors.csv",
                ErrorCategory.Other => "other_errors.csv",
                _ => "unknown_errors.csv"
            };
        }

        /// <summary>
        /// Gets the appropriate MySQL table name based on the error category.
        /// </summary>
        private static string GetMySqlTableName(ErrorCategory category)
        {
            return category switch
            {
                ErrorCategory.UI => "ui_errors",
                ErrorCategory.BusinessLogic => "business_logic_errors",
                ErrorCategory.MySQL => "mysql_errors", 
                ErrorCategory.Network => "network_errors",
                ErrorCategory.Other => "other_errors",
                _ => "unknown_errors"
            };
        }

        /// <summary>
        /// Ensures the user-specific log folder exists on the file server.
        /// </summary>
        private static void EnsureUserLogFolderExists(string userLogFolder)
        {
            try
            {
                if (!Directory.Exists(userLogFolder))
                {
                    Directory.CreateDirectory(userLogFolder);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create user log folder: {userLogFolder}", ex);
            }
        }

        /// <summary>
        /// Gets the CSV header row for error log files.
        /// </summary>
        private static string GetCsvHeader()
        {
            return "Timestamp,UserId,MachineName,Category,Severity,ErrorMessage,FileName,MethodName,LineNumber,StackTrace,source,AdditionalData,ExceptionType";
        }

        /// <summary>
        /// Formats an error entry as a CSV row with proper escaping.
        /// </summary>
        private static string FormatErrorEntryAsCsv(ErrorEntry errorEntry)
        {
            // TODO: Implement proper CSV formatting with escaping
            // - Handle commas, quotes, and newlines in field values
            // - Ensure all fields are properly escaped
            
            return string.Join(",",
                EscapeCsvField(errorEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")),
                EscapeCsvField(errorEntry.UserId),
                EscapeCsvField(errorEntry.MachineName),
                EscapeCsvField(errorEntry.Category.ToString()),
                EscapeCsvField(errorEntry.Severity.ToString()),
                EscapeCsvField(errorEntry.ErrorMessage),
                EscapeCsvField(errorEntry.FileName),
                EscapeCsvField(errorEntry.MethodName),
                EscapeCsvField(errorEntry.LineNumber.ToString()),
                EscapeCsvField(errorEntry.StackTrace),
                EscapeCsvField(errorEntry.source ?? ""),
                EscapeCsvField(errorEntry.AdditionalData),
                EscapeCsvField(errorEntry.ExceptionType)
            );
        }

        /// <summary>
        /// Escapes a field value for safe CSV formatting.
        /// </summary>
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "\"\"";
            
            // TODO: Implement proper CSV escaping
            // - Wrap in quotes if contains comma, quote, or newline
            // - Escape internal quotes by doubling them
            
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            
            return field;
        }

        /// <summary>
        /// Gets the MySQL connection string from configuration.
        /// </summary>
        private static string GetMySqlConnectionString()
        {
            // TODO: Implement configuration-based connection string retrieval
            return "Server=localhost;Database=mtm_logs;Uid=app_user;Pwd=placeholder;";
        }

        /// <summary>
        /// Sets the file server base path for logging. Used for configuration and testing.
        /// </summary>
        /// <param name="basePath">The base path on the file server for log storage</param>
        public static void SetFileServerBasePath(string basePath)
        {
            _fileServerBasePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }

        /// <summary>
        /// Gets the current file server base path.
        /// </summary>
        public static string GetFileServerBasePath()
        {
            return _fileServerBasePath;
        }
    }
}