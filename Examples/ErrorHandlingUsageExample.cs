using System;
using System.Collections.Generic;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Examples
{
    /// <summary>
    /// Example class demonstrating how to use the error handling system
    /// in various scenarios throughout the application.
    /// </summary>
    public class ErrorHandlingUsageExample
    {
        /// <summary>
        /// Example of handling a UI-related error in a button click event.
        /// </summary>
        public void Example_ButtonClick_ErrorHandling()
        {
            try
            {
                // Simulate some UI operation that might fail
                throw new InvalidOperationException("Unable to update control state");
            }
            catch (Exception ex)
            {
                // Log the error with UI category and medium severity
                Service_ErrorHandler.HandleException(ex, ErrorSeverity.Medium,
                    source: "MainWindow_Button_Save",
                    additionalData: new Dictionary<string, object>
                    {
                        ["Operation"] = "SaveUserData",
                        ["UserId"] = "12345",
                        ["FormData"] = "UserName=JohnDoe,Email=john@example.com"
                    });

                // Show user-friendly message
                var userMessage = ErrorMessageProvider.GetCompleteUserMessage(
                    ErrorCategory.UI, ex.GetType().FullName!, ErrorSeverity.Medium);
                
                // TODO: Display userMessage to user via MessageBox or notification
                Console.WriteLine($"User Message: {userMessage}");
            }
        }

        /// <summary>
        /// Example of handling a database error during save operation.
        /// </summary>
        public void Example_DatabaseOperation_ErrorHandling()
        {
            try
            {
                // Simulate database operation that fails
                throw new TimeoutException("MySQL connection timeout");
            }
            catch (Exception ex)
            {
                // Log with high severity since data wasn't saved
                Service_ErrorHandler.HandleException(ex, ErrorSeverity.High,
                    additionalData: new Dictionary<string, object>
                    {
                        ["Operation"] = "SaveInventoryItem",
                        ["ItemId"] = "INV-001",
                        ["ConnectionString"] = "Server=localhost;Database=inventory"
                    });

                // Check if we should show this error to the user
                var category = ErrorCategory.MySQL;
                if (ErrorMessageProvider.ShouldShowToUser(ErrorSeverity.High, category))
                {
                    var title = ErrorMessageProvider.GetDialogTitle(ErrorSeverity.High);
                    var message = ErrorMessageProvider.GetCompleteUserMessage(
                        category, ex.GetType().FullName!, ErrorSeverity.High);
                    
                    // TODO: Show error dialog with title and message
                    Console.WriteLine($"Dialog Title: {title}");
                    Console.WriteLine($"Dialog Message: {message}");
                }
            }
        }

        /// <summary>
        /// Example of handling a business logic validation error.
        /// </summary>
        public void Example_BusinessLogic_ErrorHandling()
        {
            try
            {
                // Simulate business rule validation failure
                throw new ArgumentException("Inventory quantity cannot be negative");
            }
            catch (Exception ex)
            {
                // Log with low severity since it's user input validation
                Service_ErrorHandler.HandleException(ex, ErrorSeverity.Low,
                    source: "InventoryForm_TextBox_Quantity",
                    additionalData: new Dictionary<string, object>
                    {
                        ["Operation"] = "ValidateInventoryQuantity",
                        ["ProvidedValue"] = "-5",
                        ["MinimumValue"] = "0"
                    });

                // For validation errors, show simple message without logging details
                var simpleMessage = ErrorMessageProvider.GetExceptionSpecificMessage(ex.GetType().FullName!);
                
                // TODO: Show validation message near the control
                Console.WriteLine($"Validation Message: {simpleMessage}");
            }
        }

        /// <summary>
        /// Example of application startup error handling configuration.
        /// </summary>
        public void Example_ApplicationStartup_Configuration()
        {
            try
            {
                // Initialize error handling configuration
                ErrorHandlingConfiguration.LoadFromConfiguration();
                
                // Validate configuration
                if (!ErrorHandlingConfiguration.ValidateConfiguration())
                {
                    Console.WriteLine("Warning: Error handling configuration is invalid. Using defaults.");
                    ErrorHandlingConfiguration.ResetToDefaults();
                }
                
                // Display configuration summary
                Console.WriteLine(ErrorHandlingConfiguration.GetConfigurationSummary());
                
                // Clear any previous session errors
                Service_ErrorHandler.ClearSessionCache();
                
                Console.WriteLine("Error handling system initialized successfully.");
            }
            catch (Exception ex)
            {
                // If error handling initialization fails, use minimal logging
                LoggingUtility.LogApplicationError(ex);
                Console.WriteLine($"Failed to initialize error handling: {ex.Message}");
            }
        }

        /// <summary>
        /// Example of handling critical application errors.
        /// </summary>
        public void Example_CriticalError_Handling()
        {
            try
            {
                // Simulate critical application error
                throw new OutOfMemoryException("Application ran out of memory");
            }
            catch (Exception ex)
            {
                // Log with critical severity
                Service_ErrorHandler.HandleException(ex, ErrorSeverity.Critical,
                    additionalData: new Dictionary<string, object>
                    {
                        ["Operation"] = "LoadLargeDataSet",
                        ["MemoryUsage"] = "2.1GB",
                        ["AvailableMemory"] = "1.8GB"
                    });

                // Critical errors should always be shown to users
                var title = ErrorMessageProvider.GetDialogTitle(ErrorSeverity.Critical);
                var message = ErrorMessageProvider.GetCompleteUserMessage(
                    ErrorCategory.Other, ex.GetType().FullName!, ErrorSeverity.Critical);
                
                // TODO: Show critical error dialog and potentially exit application
                Console.WriteLine($"CRITICAL ERROR - {title}: {message}");
                
                // TODO: Consider graceful application shutdown
            }
        }

        /// <summary>
        /// Example of checking error statistics during application runtime.
        /// </summary>
        public void Example_ErrorStatistics_Monitoring()
        {
            // Check error counts for different categories
            var uiErrors = Service_ErrorHandler.GetSessionErrorCount(ErrorCategory.UI);
            var dbErrors = Service_ErrorHandler.GetSessionErrorCount(ErrorCategory.MySQL);
            var networkErrors = Service_ErrorHandler.GetSessionErrorCount(ErrorCategory.Network);
            
            Console.WriteLine($"Session Error Summary:");
            Console.WriteLine($"  UI Errors: {uiErrors}");
            Console.WriteLine($"  Database Errors: {dbErrors}");
            Console.WriteLine($"  Network Errors: {networkErrors}");
            
            // If too many errors of a certain type, might want to take action
            if (dbErrors > 5)
            {
                Console.WriteLine("Warning: Multiple database errors detected. Check connection.");
            }
        }

        /// <summary>
        /// Example of network operation error handling.
        /// </summary>
        public async void Example_NetworkOperation_ErrorHandling()
        {
            try
            {
                // Simulate network operation that fails
                throw new TimeoutException("HTTP request timeout");
            }
            catch (Exception ex)
            {
                Service_ErrorHandler.HandleException(ex, ErrorSeverity.Medium,
                    additionalData: new Dictionary<string, object>
                    {
                        ["Operation"] = "FetchRemoteData",
                        ["Url"] = "https://api.example.com/data",
                        ["Timeout"] = "30000ms"
                    });

                // For network errors, might want to retry logic
                var userMessage = ErrorMessageProvider.GetUserFriendlyMessage(ErrorCategory.Network);
                var actions = ErrorMessageProvider.GetRecommendedActions(ErrorCategory.Network);
                
                Console.WriteLine($"Network Error: {userMessage}");
                Console.WriteLine($"Recommended Actions: {actions}");
                
                // TODO: Implement retry logic or offline mode
            }
        }
    }
}