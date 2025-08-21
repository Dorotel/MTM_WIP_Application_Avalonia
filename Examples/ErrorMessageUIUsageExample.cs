using System;
using System.Collections.Generic;
using Avalonia.Controls;
using MTM_WIP_Application_Avalonia.Controls;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Examples
{
    /// <summary>
    /// Example class demonstrating how to use the Control_ErrorMessage UI element
    /// in various scenarios throughout the application.
    /// </summary>
    public class ErrorMessageUIUsageExample
    {
        /// <summary>
        /// Example of displaying an error message in a panel or overlay.
        /// </summary>
        public void Example_ShowErrorInPanel()
        {
            try
            {
                // Simulate an operation that fails
                throw new InvalidOperationException("Unable to save user data");
            }
            catch (Exception ex)
            {
                // Create and configure error message control
                var errorControl = new Control_ErrorMessage();
                errorControl.Initialize(ex, ErrorSeverity.Medium, 
                    controlName: "UserForm_Button_Save",
                    contextData: new Dictionary<string, object>
                    {
                        ["Operation"] = "SaveUserData",
                        ["UserId"] = "12345",
                        ["FormValid"] = true
                    });

                // Wire up event handlers
                errorControl.CloseRequested += (sender, e) =>
                {
                    // TODO: Remove error control from UI
                    Console.WriteLine("User requested to close error display");
                };

                errorControl.RetryRequested += (sender, e) =>
                {
                    // TODO: Handle retry request
                    Console.WriteLine("User requested to retry operation");
                };

                // TODO: Add errorControl to your UI container
                // For example: MainGrid.Children.Add(errorControl);
            }
        }

        /// <summary>
        /// Example of showing an error with retry functionality.
        /// </summary>
        public void Example_ShowErrorWithRetry()
        {
            var attemptCount = 0;
            
            try
            {
                // Simulate database operation that might fail
                throw new TimeoutException("Database connection timeout");
            }
            catch (Exception ex)
            {
                // Define retry action
                Func<bool> retryAction = () =>
                {
                    attemptCount++;
                    Console.WriteLine($"Retry attempt #{attemptCount}");
                    
                    // TODO: Implement actual retry logic
                    // Return true if retry succeeds, false if it fails
                    return attemptCount < 3; // Simulate success after 3 attempts
                };

                // Create error control with retry capability
                var errorControl = Control_ErrorMessage.ShowError(ex, ErrorSeverity.High,
                    controlName: "DatabaseService_SaveOperation",
                    retryAction: retryAction);

                // Handle events
                errorControl.CloseRequested += (sender, e) =>
                {
                    Console.WriteLine("Error dialog closed");
                };

                errorControl.RetryRequested += (sender, e) =>
                {
                    Console.WriteLine("Retry operation initiated");
                };

                // TODO: Display errorControl in your UI
            }
        }

        /// <summary>
        /// Example of showing a critical error that requires application attention.
        /// </summary>
        public void Example_ShowCriticalError()
        {
            try
            {
                // Simulate critical system failure
                throw new OutOfMemoryException("Application is running out of memory");
            }
            catch (Exception ex)
            {
                var errorControl = new Control_ErrorMessage();
                errorControl.Initialize(ex, ErrorSeverity.Critical,
                    controlName: "ApplicationCore",
                    contextData: new Dictionary<string, object>
                    {
                        ["MemoryUsage"] = "95%",
                        ["AvailableMemory"] = "128MB",
                        ["ActiveProcesses"] = 47
                    });

                // For critical errors, might want to force user attention
                errorControl.CloseRequested += (sender, e) =>
                {
                    // TODO: Consider graceful application shutdown for critical errors
                    Console.WriteLine("Critical error acknowledged - consider app shutdown");
                };

                errorControl.ReportRequested += (sender, e) =>
                {
                    // TODO: Automatically submit critical error reports
                    Console.WriteLine("Critical error report submitted automatically");
                };

                // TODO: Display as modal or prominent overlay
            }
        }

        /// <summary>
        /// Example of displaying a low-severity information message.
        /// </summary>
        public void Example_ShowInformationMessage()
        {
            try
            {
                // Simulate minor validation issue
                throw new ArgumentException("Invalid email format");
            }
            catch (Exception ex)
            {
                var errorControl = new Control_ErrorMessage();
                errorControl.Initialize(ex, ErrorSeverity.Low,
                    controlName: "UserForm_TextBox_Email",
                    contextData: new Dictionary<string, object>
                    {
                        ["ProvidedEmail"] = "invalid-email",
                        ["ValidationRule"] = "EmailFormat"
                    });

                // Low severity errors might auto-dismiss
                var timer = new System.Timers.Timer(5000); // 5 seconds
                timer.Elapsed += (sender, e) =>
                {
                    timer.Stop();
                    // TODO: Auto-remove from UI
                    Console.WriteLine("Low severity error auto-dismissed");
                };
                timer.Start();

                // TODO: Display as subtle notification
            }
        }

        /// <summary>
        /// Example of integrating error display with a main window or dialog.
        /// </summary>
        public void Example_IntegrateWithMainWindow(Window mainWindow)
        {
            try
            {
                // Simulate network operation failure
                throw new System.Net.NetworkInformation.PingException("Network unreachable");
            }
            catch (Exception ex)
            {
                var errorControl = new Control_ErrorMessage();
                errorControl.Initialize(ex, ErrorSeverity.Medium,
                    controlName: "NetworkService_CheckConnection",
                    contextData: new Dictionary<string, object>
                    {
                        ["TargetHost"] = "api.example.com",
                        ["NetworkInterface"] = "WiFi",
                        ["LastSuccessfulConnection"] = DateTime.Now.AddMinutes(-5)
                    });

                // Handle error control events
                errorControl.CloseRequested += (sender, e) =>
                {
                    // Remove from main window
                    if (mainWindow.Content is Panel panel)
                    {
                        panel.Children.Remove(errorControl);
                    }
                };

                errorControl.ReportRequested += (sender, e) =>
                {
                    // TODO: Open detailed error reporting dialog
                    Console.WriteLine("Opening detailed error reporting dialog");
                };

                // Add to main window overlay or panel
                if (mainWindow.Content is Grid mainGrid)
                {
                    // TODO: Add errorControl to an overlay layer
                    // mainGrid.Children.Add(errorControl);
                    
                    // Position as overlay
                    errorControl.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                    errorControl.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
                    errorControl.Margin = new Avalonia.Thickness(0, 50, 0, 0);
                }
            }
        }

        /// <summary>
        /// Example of creating a custom error display for specific error types.
        /// </summary>
        public void Example_CustomErrorHandling()
        {
            try
            {
                // Simulate MySQL-specific error (using a valid way to create one)
                throw new InvalidOperationException("MySQL connection string is invalid") 
                { 
                    Source = "MySql.Data.MySqlClient" 
                };
            }
            catch (Exception ex)
            {
                var errorControl = new Control_ErrorMessage();
                
                // Provide database-specific context
                errorControl.Initialize(ex, ErrorSeverity.High,
                    controlName: "DatabaseManager_Connect",
                    contextData: new Dictionary<string, object>
                    {
                        ["DatabaseType"] = "MySQL",
                        ["Server"] = "localhost",
                        ["Database"] = "inventory_system",
                        ["ConnectionTimeout"] = "30 seconds",
                        ["LastSuccessfulConnection"] = DateTime.Now.AddHours(-2)
                    });

                // Database errors might need special handling
                errorControl.RetryRequested += async (sender, e) =>
                {
                    Console.WriteLine("Attempting database reconnection...");
                    
                    // TODO: Implement database-specific retry logic
                    await System.Threading.Tasks.Task.Delay(1000); // Simulate connection attempt
                    
                    Console.WriteLine("Database retry completed");
                };

                errorControl.ReportRequested += (sender, e) =>
                {
                    // TODO: Send database error report to IT team
                    Console.WriteLine("Database error reported to system administrators");
                };

                // TODO: Display with database-specific styling or behavior
            }
        }

        /// <summary>
        /// Example of creating error notifications for background operations.
        /// </summary>
        public void Example_BackgroundOperationError()
        {
            try
            {
                // Simulate background file processing error
                throw new System.IO.FileNotFoundException("Required configuration file not found");
            }
            catch (Exception ex)
            {
                var errorControl = new Control_ErrorMessage();
                errorControl.Initialize(ex, ErrorSeverity.Medium,
                    controlName: "BackgroundProcessor_LoadConfig",
                    contextData: new Dictionary<string, object>
                    {
                        ["OperationType"] = "BackgroundProcessing",
                        ["ConfigFile"] = "app.config",
                        ["WorkingDirectory"] = Environment.CurrentDirectory,
                        ["ProcessId"] = Environment.ProcessId
                    });

                // Background errors might need different presentation
                errorControl.CloseRequested += (sender, e) =>
                {
                    Console.WriteLine("Background error acknowledged by user");
                };

                // TODO: Display as notification toast or status bar message
                // rather than blocking the main UI
            }
        }
    }
}