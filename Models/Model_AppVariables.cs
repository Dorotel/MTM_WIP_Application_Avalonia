using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MTM_WIP_Application_Avalonia.Models
{
    /// <summary>
    /// Application-wide variables and constants for the MTM WIP Application.
    /// Provides centralized access to configuration settings and application state.
    /// 
    /// This class bridges the gap between legacy code expecting Model_AppVariables 
    /// and the modern configuration system.
    /// </summary>
    public static class Model_AppVariables
    {
        private static IConfiguration? _configuration;
        private static string? _connectionString;

        /// <summary>
        /// Initializes the application variables with configuration.
        /// Must be called during application startup.
        /// </summary>
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Gets the database connection string.
        /// CRITICAL: This property is used throughout the application for database access.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    // Fallback to environment variable or default
                    _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                        ?? "Server=localhost;Database=mtm_wip_application;Uid=root;Pwd=;SslMode=Required;";
                }
                return _connectionString;
            }
            set => _connectionString = value;
        }

        /// <summary>
        /// Gets the current application version.
        /// </summary>
        public static string ApplicationVersion => "1.0.0.0";

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public static string ApplicationName => "MTM WIP Application Avalonia";

        /// <summary>
        /// Gets the application company name.
        /// </summary>
        public static string CompanyName => "Manitowoc Tool and Manufacturing";

        /// <summary>
        /// Gets the current user name.
        /// TODO: Replace with proper user service when authentication is implemented.
        /// </summary>
        public static string CurrentUser { get; set; } = Environment.UserName;

        /// <summary>
        /// Gets or sets the current user's full name.
        /// </summary>
        public static string CurrentUserFullName { get; set; } = Environment.UserName;

        /// <summary>
        /// Gets or sets whether the application is in debug mode.
        /// </summary>
        public static bool IsDebugMode 
        { 
            get
            {
#if DEBUG
                return true;
#else
                return _configuration?.GetValue<bool>("Debug", false) ?? false;
#endif
            }
        }

        /// <summary>
        /// Gets the application data directory.
        /// </summary>
        public static string ApplicationDataDirectory
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var mtmDir = Path.Combine(appData, "MTM_WIP_Application");
                
                if (!Directory.Exists(mtmDir))
                    Directory.CreateDirectory(mtmDir);
                
                return mtmDir;
            }
        }

        /// <summary>
        /// Gets the logs directory.
        /// </summary>
        public static string LogsDirectory
        {
            get
            {
                var logsDir = Path.Combine(ApplicationDataDirectory, "Logs");
                
                if (!Directory.Exists(logsDir))
                    Directory.CreateDirectory(logsDir);
                
                return logsDir;
            }
        }

        /// <summary>
        /// Gets the configuration directory.
        /// </summary>
        public static string ConfigurationDirectory
        {
            get
            {
                var configDir = Path.Combine(ApplicationDataDirectory, "Config");
                
                if (!Directory.Exists(configDir))
                    Directory.CreateDirectory(configDir);
                
                return configDir;
            }
        }

        /// <summary>
        /// Database configuration settings.
        /// </summary>
        public static class Database
        {
            /// <summary>
            /// Gets the command timeout in seconds.
            /// </summary>
            public static int CommandTimeout => _configuration?.GetValue<int>("Database:CommandTimeout", 30) ?? 30;

            /// <summary>
            /// Gets the maximum retry attempts for database operations.
            /// </summary>
            public static int MaxRetryAttempts => _configuration?.GetValue<int>("Database:MaxRetryAttempts", 3) ?? 3;

            /// <summary>
            /// Gets whether to log all database operations.
            /// </summary>
            public static bool LogAllOperations => _configuration?.GetValue<bool>("Database:LogAllOperations", false) ?? false;
        }

        /// <summary>
        /// UI configuration settings.
        /// </summary>
        public static class UI
        {
            /// <summary>
            /// Gets the default theme name.
            /// </summary>
            public static string DefaultTheme => _configuration?.GetValue<string>("UI:DefaultTheme", "MTM_Purple") ?? "MTM_Purple";

            /// <summary>
            /// Gets the default font size.
            /// </summary>
            public static int DefaultFontSize => _configuration?.GetValue<int>("UI:DefaultFontSize", 10) ?? 10;

            /// <summary>
            /// Gets whether to show debug information in the UI.
            /// </summary>
            public static bool ShowDebugInfo => _configuration?.GetValue<bool>("UI:ShowDebugInfo", IsDebugMode) ?? IsDebugMode;
        }

        /// <summary>
        /// Logging configuration settings.
        /// </summary>
        public static class Logging
        {
            /// <summary>
            /// Gets the minimum log level.
            /// </summary>
            public static string MinimumLevel => _configuration?.GetValue<string>("Logging:LogLevel:Default", "Information") ?? "Information";

            /// <summary>
            /// Gets whether to log to file.
            /// </summary>
            public static bool LogToFile => _configuration?.GetValue<bool>("Logging:LogToFile", true) ?? true;

            /// <summary>
            /// Gets whether to log to database.
            /// </summary>
            public static bool LogToDatabase => _configuration?.GetValue<bool>("Logging:LogToDatabase", true) ?? true;

            /// <summary>
            /// Gets the maximum log file size in MB.
            /// </summary>
            public static int MaxLogFileSizeMB => _configuration?.GetValue<int>("Logging:MaxLogFileSizeMB", 10) ?? 10;
        }

        /// <summary>
        /// Error handling configuration settings.
        /// </summary>
        public static class ErrorHandling
        {
            /// <summary>
            /// Gets whether to show detailed error messages.
            /// </summary>
            public static bool ShowDetailedErrors => _configuration?.GetValue<bool>("ErrorHandling:ShowDetailedErrors", IsDebugMode) ?? IsDebugMode;

            /// <summary>
            /// Gets whether to automatically report errors.
            /// </summary>
            public static bool AutoReportErrors => _configuration?.GetValue<bool>("ErrorHandling:AutoReportErrors", false) ?? false;

            /// <summary>
            /// Gets the error reporting endpoint.
            /// </summary>
            public static string? ErrorReportingEndpoint => _configuration?.GetValue<string>("ErrorHandling:ErrorReportingEndpoint");
        }

        /// <summary>
        /// MTM-specific business rules and constants.
        /// </summary>
        public static class MTM
        {
            /// <summary>
            /// Valid transaction types for inventory operations.
            /// </summary>
            public static readonly string[] ValidTransactionTypes = { "IN", "OUT", "TRANSFER" };

            /// <summary>
            /// Default item type for new inventory items.
            /// </summary>
            public static string DefaultItemType => "WIP";

            /// <summary>
            /// Maximum quantity allowed for a single transaction.
            /// </summary>
            public static int MaxTransactionQuantity => _configuration?.GetValue<int>("MTM:MaxTransactionQuantity", 10000) ?? 10000;

            /// <summary>
            /// Default location for new inventory items.
            /// </summary>
            public static string DefaultLocation => _configuration?.GetValue<string>("MTM:DefaultLocation", "FLOOR") ?? "FLOOR";

            /// <summary>
            /// Company primary color (MTM Purple).
            /// </summary>
            public static string PrimaryColor => "#6a0dad";

            /// <summary>
            /// Company secondary color.
            /// </summary>
            public static string SecondaryColor => "#4a0880";
        }

        /// <summary>
        /// Gets a configuration value with a default fallback.
        /// </summary>
        /// <typeparam name="T">Type of the configuration value</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value or default</returns>
        public static T GetConfigValue<T>(string key, T defaultValue)
        {
            if (_configuration == null)
                return defaultValue;
                
            try 
            {
                return _configuration.GetValue<T>(key) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Updates the connection string at runtime.
        /// Used for testing or when switching between databases.
        /// </summary>
        public static void UpdateConnectionString(string newConnectionString)
        {
            if (string.IsNullOrWhiteSpace(newConnectionString))
                throw new ArgumentException("Connection string cannot be empty", nameof(newConnectionString));

            _connectionString = newConnectionString;
        }
    }
}