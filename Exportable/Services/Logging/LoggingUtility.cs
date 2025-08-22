using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace MTM.Core.Services
{
    /// <summary>
    /// Framework-agnostic logging utility for handling error logging to files and databases.
    /// Supports multiple database providers and configurable logging destinations.
    /// </summary>
    public static class LoggingUtility
    {
        private static string _fileServerBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Application_Logs");
        private static readonly object _fileLockObject = new();
        private static IDbConnectionFactory? _connectionFactory;
        
        /// <summary>
        /// Sets the database connection factory for database logging.
        /// </summary>
        /// <param name="connectionFactory">Factory for creating database connections</param>
        public static void SetConnectionFactory(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

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

                var userLogFolder = Path.Combine(_fileServerBasePath, SanitizeFileName(errorEntry.UserId));
                var csvFileName = GetCsvFileName(errorEntry.Category);
                var csvFilePath = Path.Combine(userLogFolder, csvFileName);
                
                EnsureDirectoryExists(userLogFolder);
                
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
                LogToFallbackLocation(errorEntry, ex);
            }
        }

        /// <summary>
        /// Logs an error entry to the database.
        /// Uses configurable database provider through connection factory.
        /// </summary>
        /// <param name="errorEntry">The error entry to log</param>
        public static async Task LogToDatabase(ErrorEntry errorEntry)
        {
            try
            {
                if (!ErrorHandlingConfiguration.EnableDatabaseLogging || _connectionFactory == null)
                    return;

                var tableName = GetDatabaseTableName(errorEntry.Category);
                
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                
                // Ensure table exists
                await EnsureTableExists(connection, tableName);
                
                // Insert error entry
                await InsertErrorEntry(connection, tableName, errorEntry);
            }
            catch (Exception ex)
            {
                LogApplicationError(ex);
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
                var fallbackPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                    "Application_Logs", "Internal", "application_errors.log");
                
                var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC - {exception.GetType().Name}: {exception.Message}{Environment.NewLine}";
                
                EnsureDirectoryExists(Path.GetDirectoryName(fallbackPath)!);
                File.AppendAllText(fallbackPath, logEntry);

                // Also log to console for debugging
                if (ErrorHandlingConfiguration.EnableConsoleLogging)
                {
                    Console.WriteLine($"LOGGING ERROR: {logEntry.Trim()}");
                }
            }
            catch
            {
                // Last resort - write to console only
                Console.WriteLine($"CRITICAL LOGGING FAILURE: {exception.Message}");
            }
        }

        /// <summary>
        /// Ensures table exists for the specified category.
        /// </summary>
        private static async Task EnsureTableExists(DbConnection connection, string tableName)
        {
            var createTableSql = GetCreateTableSql(tableName);
            
            using var command = connection.CreateCommand();
            command.CommandText = createTableSql;
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Gets the SQL for creating an error table (generic SQL that works with most databases).
        /// </summary>
        private static string GetCreateTableSql(string tableName)
        {
            return $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    Id INTEGER PRIMARY KEY,
                    Timestamp DATETIME NOT NULL,
                    UserId VARCHAR(255) NOT NULL,
                    MachineName VARCHAR(255) NOT NULL,
                    ApplicationName VARCHAR(255),
                    Category VARCHAR(50) NOT NULL,
                    Severity VARCHAR(20) NOT NULL,
                    ErrorMessage TEXT,
                    FileName VARCHAR(500),
                    MethodName VARCHAR(255),
                    LineNumber INTEGER,
                    StackTrace TEXT,
                    Source VARCHAR(255),
                    AdditionalData TEXT,
                    ExceptionType VARCHAR(500),
                    InnerException TEXT
                )";
        }

        /// <summary>
        /// Inserts error entry into database.
        /// </summary>
        private static async Task InsertErrorEntry(DbConnection connection, string tableName, ErrorEntry errorEntry)
        {
            var insertSql = $@"
                INSERT INTO {tableName} 
                (Timestamp, UserId, MachineName, ApplicationName, Category, Severity, ErrorMessage, 
                 FileName, MethodName, LineNumber, StackTrace, Source, AdditionalData, ExceptionType, InnerException)
                VALUES 
                (@Timestamp, @UserId, @MachineName, @ApplicationName, @Category, @Severity, @ErrorMessage,
                 @FileName, @MethodName, @LineNumber, @StackTrace, @Source, @AdditionalData, @ExceptionType, @InnerException)";

            using var command = connection.CreateCommand();
            command.CommandText = insertSql;
            
            AddParameter(command, "@Timestamp", errorEntry.Timestamp);
            AddParameter(command, "@UserId", errorEntry.UserId);
            AddParameter(command, "@MachineName", errorEntry.MachineName);
            AddParameter(command, "@ApplicationName", errorEntry.ApplicationName);
            AddParameter(command, "@Category", errorEntry.Category.ToString());
            AddParameter(command, "@Severity", errorEntry.Severity.ToString());
            AddParameter(command, "@ErrorMessage", errorEntry.ErrorMessage);
            AddParameter(command, "@FileName", errorEntry.FileName);
            AddParameter(command, "@MethodName", errorEntry.MethodName);
            AddParameter(command, "@LineNumber", errorEntry.LineNumber);
            AddParameter(command, "@StackTrace", errorEntry.StackTrace);
            AddParameter(command, "@Source", errorEntry.Source ?? "");
            AddParameter(command, "@AdditionalData", errorEntry.AdditionalData);
            AddParameter(command, "@ExceptionType", errorEntry.ExceptionType);
            AddParameter(command, "@InnerException", errorEntry.InnerException ?? "");
            
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Adds parameter to database command in a provider-agnostic way.
        /// </summary>
        private static void AddParameter(DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Logs to fallback location when primary logging fails.
        /// </summary>
        private static void LogToFallbackLocation(ErrorEntry errorEntry, Exception originalException)
        {
            try
            {
                var fallbackPath = Path.Combine(
                    ErrorHandlingConfiguration.FallbackLocalPath, 
                    SanitizeFileName(errorEntry.UserId), 
                    GetCsvFileName(errorEntry.Category));
                
                EnsureDirectoryExists(Path.GetDirectoryName(fallbackPath)!);
                
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
                Console.WriteLine($"CRITICAL: Both primary and fallback logging failed. Original: {originalException.Message}, Fallback: {fallbackEx.Message}");
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
                ErrorCategory.Database => "database_errors.csv",
                ErrorCategory.Network => "network_errors.csv",
                ErrorCategory.Security => "security_errors.csv",
                ErrorCategory.Other => "other_errors.csv",
                _ => "unknown_errors.csv"
            };
        }

        /// <summary>
        /// Gets the appropriate database table name based on the error category.
        /// </summary>
        private static string GetDatabaseTableName(ErrorCategory category)
        {
            return category switch
            {
                ErrorCategory.UI => "ui_errors",
                ErrorCategory.BusinessLogic => "business_logic_errors",
                ErrorCategory.Database => "database_errors", 
                ErrorCategory.Network => "network_errors",
                ErrorCategory.Security => "security_errors",
                ErrorCategory.Other => "other_errors",
                _ => "unknown_errors"
            };
        }

        /// <summary>
        /// Ensures the specified directory exists.
        /// </summary>
        private static void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// Sanitizes a filename to remove invalid characters.
        /// </summary>
        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                fileName = fileName.Replace(invalidChar, '_');
            }
            return fileName;
        }

        /// <summary>
        /// Gets the CSV header row for error log files.
        /// </summary>
        private static string GetCsvHeader()
        {
            return "Timestamp,UserId,MachineName,ApplicationName,Category,Severity,ErrorMessage,FileName,MethodName,LineNumber,StackTrace,Source,AdditionalData,ExceptionType,InnerException";
        }

        /// <summary>
        /// Formats an error entry as a CSV row with proper escaping.
        /// </summary>
        private static string FormatErrorEntryAsCsv(ErrorEntry errorEntry)
        {
            return string.Join(",",
                EscapeCsvField(errorEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")),
                EscapeCsvField(errorEntry.UserId),
                EscapeCsvField(errorEntry.MachineName),
                EscapeCsvField(errorEntry.ApplicationName),
                EscapeCsvField(errorEntry.Category.ToString()),
                EscapeCsvField(errorEntry.Severity.ToString()),
                EscapeCsvField(errorEntry.ErrorMessage),
                EscapeCsvField(errorEntry.FileName),
                EscapeCsvField(errorEntry.MethodName),
                EscapeCsvField(errorEntry.LineNumber.ToString()),
                EscapeCsvField(errorEntry.StackTrace),
                EscapeCsvField(errorEntry.Source ?? ""),
                EscapeCsvField(errorEntry.AdditionalData),
                EscapeCsvField(errorEntry.ExceptionType),
                EscapeCsvField(errorEntry.InnerException ?? "")
            );
        }

        /// <summary>
        /// Escapes a field value for safe CSV formatting.
        /// </summary>
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "\"\"";
            
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            
            return field;
        }

        /// <summary>
        /// Sets the file server base path for logging.
        /// </summary>
        /// <param name="basePath">The base path for file logging</param>
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

        /// <summary>
        /// Gets logging statistics for monitoring purposes.
        /// </summary>
        public static LoggingStatistics GetStatistics()
        {
            return new LoggingStatistics
            {
                FileLoggingEnabled = ErrorHandlingConfiguration.EnableFileServerLogging,
                DatabaseLoggingEnabled = ErrorHandlingConfiguration.EnableDatabaseLogging,
                ConsoleLoggingEnabled = ErrorHandlingConfiguration.EnableConsoleLogging,
                FileBasePath = _fileServerBasePath,
                HasConnectionFactory = _connectionFactory != null
            };
        }
    }

    /// <summary>
    /// Interface for creating database connections in a provider-agnostic way.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates a new database connection.
        /// </summary>
        /// <returns>A new database connection</returns>
        DbConnection CreateConnection();
    }

    /// <summary>
    /// Statistics about the logging system.
    /// </summary>
    public class LoggingStatistics
    {
        public bool FileLoggingEnabled { get; set; }
        public bool DatabaseLoggingEnabled { get; set; }
        public bool ConsoleLoggingEnabled { get; set; }
        public string FileBasePath { get; set; } = string.Empty;
        public bool HasConnectionFactory { get; set; }

        public override string ToString()
        {
            return $"Logging Statistics: File={FileLoggingEnabled}, DB={DatabaseLoggingEnabled}, Console={ConsoleLoggingEnabled}, Path={FileBasePath}, Factory={HasConnectionFactory}";
        }
    }
}