using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// Enhanced utility class for handling all logging operations with structured logging support,
    /// MTM business context integration, and comprehensive fallback mechanisms.
    /// </summary>
    public static class LoggingUtility
    {
        private static string _fileServerBasePath = @"\\FileServer\Logs";
        private static readonly object _fileLockObject = new();
        
        /// <summary>
        /// Logs an error entry to the file server with enhanced CSV format including business context.
        /// </summary>
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
                        writer.WriteLine(GetEnhancedCsvHeader());
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
        /// Enhanced MySQL logging with business context and structured data support.
        /// </summary>
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
                
                await EnsureEnhancedTableExists(connection, tableName);
                
                var insertSql = $@"
                    INSERT INTO {tableName} 
                    (Timestamp, UserId, MachineName, Category, Severity, ErrorMessage, 
                     FileName, MethodName, LineNumber, StackTrace, source, 
                     AdditionalData, ExceptionType, BusinessContext)
                    VALUES 
                    (@Timestamp, @UserId, @MachineName, @Category, @Severity, @ErrorMessage,
                     @FileName, @MethodName, @LineNumber, @StackTrace, @source,
                     @AdditionalData, @ExceptionType, @BusinessContext)";

                using var command = new MySqlCommand(insertSql, connection);
                AddEnhancedMySqlParameters(command, errorEntry);
                
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                LogApplicationError(ex);
            }
        }

        /// <summary>
        /// Structured logging for MTM business operations with stored procedure integration patterns.
        /// </summary>
        public static async Task LogBusinessOperationAsync(
            string operation,
            string userId,
            Dictionary<string, object> parameters,
            Exception? exception = null,
            string? result = null)
        {
            try
            {
                var logEntry = new Dictionary<string, object>
                {
                    ["Operation"] = operation,
                    ["UserId"] = userId,
                    ["Timestamp"] = DateTime.UtcNow,
                    ["Parameters"] = JsonSerializer.Serialize(parameters),
                    ["Result"] = result ?? "Success",
                    ["HasError"] = exception != null
                };

                if (exception != null)
                {
                    logEntry["ErrorMessage"] = exception.Message;
                    logEntry["ErrorType"] = exception.GetType().FullName ?? "";
                }

                // TODO: Replace with actual Helper_Database_StoredProcedure call when available
                // var logParameters = new Dictionary<string, object>
                // {
                //     ["p_Operation"] = operation,
                //     ["p_UserId"] = userId,
                //     ["p_Context"] = JsonSerializer.Serialize(logEntry),
                //     ["p_Severity"] = exception != null ? "Error" : "Information"
                // };
                //
                // await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                //     Model_AppVariables.ConnectionString,
                //     "sys_business_operation_Log",
                //     logParameters
                // );

                // Fallback to file logging for now
                var businessLogPath = Path.Combine(_fileServerBasePath, userId, "business_operations.log");
                Directory.CreateDirectory(Path.GetDirectoryName(businessLogPath)!);
                
                var logLine = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {operation} | {JsonSerializer.Serialize(logEntry)}{Environment.NewLine}";
                await File.AppendAllTextAsync(businessLogPath, logLine);
            }
            catch (Exception ex)
            {
                LogApplicationError(ex);
            }
        }

        /// <summary>
        /// Enhanced table creation with business context column.
        /// </summary>
        private static async Task EnsureEnhancedTableExists(MySqlConnection connection, string tableName)
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
                    BusinessContext TEXT,
                    INDEX idx_timestamp (Timestamp),
                    INDEX idx_userid (UserId),
                    INDEX idx_severity (Severity),
                    INDEX idx_business_context (BusinessContext(255))
                )";

            using var command = new MySqlCommand(createTableSql, connection);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Enhanced parameter addition including business context.
        /// </summary>
        private static void AddEnhancedMySqlParameters(MySqlCommand command, ErrorEntry errorEntry)
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
            command.Parameters.AddWithValue("@BusinessContext", errorEntry.BusinessContext);
        }

        /// <summary>
        /// Enhanced CSV header including business context.
        /// </summary>
        private static string GetEnhancedCsvHeader()
        {
            return "Timestamp,UserId,MachineName,Category,Severity,ErrorMessage,FileName,MethodName,LineNumber,StackTrace,source,AdditionalData,ExceptionType,BusinessContext";
        }

        /// <summary>
        /// Enhanced CSV formatting with business context.
        /// </summary>
        private static string FormatErrorEntryAsCsv(ErrorEntry errorEntry)
        {
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
                EscapeCsvField(errorEntry.ExceptionType),
                EscapeCsvField(errorEntry.BusinessContext)
            );
        }

        /// <summary>
        /// Performance monitoring and logging for database operations.
        /// </summary>
        public static async Task LogPerformanceMetricsAsync(
            string operation,
            TimeSpan duration,
            string userId,
            Dictionary<string, object>? additionalMetrics = null)
        {
            try
            {
                var metrics = new Dictionary<string, object>
                {
                    ["Operation"] = operation,
                    ["Duration"] = duration.TotalMilliseconds,
                    ["UserId"] = userId,
                    ["Timestamp"] = DateTime.UtcNow
                };

                if (additionalMetrics != null)
                {
                    foreach (var metric in additionalMetrics)
                    {
                        metrics[metric.Key] = metric.Value;
                    }
                }

                // Log to performance log file
                var performanceLogPath = Path.Combine(_fileServerBasePath, "performance", $"performance_{DateTime.UtcNow:yyyy-MM}.log");
                Directory.CreateDirectory(Path.GetDirectoryName(performanceLogPath)!);
                
                var logLine = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {JsonSerializer.Serialize(metrics)}{Environment.NewLine}";
                await File.AppendAllTextAsync(performanceLogPath, logLine);
            }
            catch (Exception ex)
            {
                LogApplicationError(ex);
            }
        }

        /// <summary>
        /// Audit trail logging for MTM business operations.
        /// </summary>
        public static async Task LogAuditTrailAsync(
            string action,
            string userId,
            string entityType,
            string entityId,
            Dictionary<string, object>? beforeState = null,
            Dictionary<string, object>? afterState = null)
        {
            try
            {
                var auditEntry = new Dictionary<string, object>
                {
                    ["Action"] = action,
                    ["UserId"] = userId,
                    ["EntityType"] = entityType,
                    ["EntityId"] = entityId,
                    ["Timestamp"] = DateTime.UtcNow,
                    ["BeforeState"] = beforeState != null ? JsonSerializer.Serialize(beforeState) : null,
                    ["AfterState"] = afterState != null ? JsonSerializer.Serialize(afterState) : null
                };

                // TODO: Replace with actual Helper_Database_StoredProcedure call when available
                // await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                //     Model_AppVariables.ConnectionString,
                //     "sys_audit_trail_Insert",
                //     auditEntry
                // );

                // Fallback to file logging for now
                var auditLogPath = Path.Combine(_fileServerBasePath, "audit", $"audit_{DateTime.UtcNow:yyyy-MM}.log");
                Directory.CreateDirectory(Path.GetDirectoryName(auditLogPath)!);
                
                var logLine = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {JsonSerializer.Serialize(auditEntry)}{Environment.NewLine}";
                await File.AppendAllTextAsync(auditLogPath, logLine);
            }
            catch (Exception ex)
            {
                LogApplicationError(ex);
            }
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
                    writer.WriteLine(GetEnhancedCsvHeader());
                }
                
                writer.WriteLine(FormatErrorEntryAsCsv(errorEntry));
                
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
                var fallbackPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                    "MTM_WIP_Application", "Logs", "application_errors.log");
                
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {exception.GetType().Name}: {exception.Message}{Environment.NewLine}";
                
                Directory.CreateDirectory(Path.GetDirectoryName(fallbackPath)!);
                File.AppendAllText(fallbackPath, logEntry);
            }
            catch
            {
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