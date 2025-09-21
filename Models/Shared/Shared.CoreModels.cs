using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MTM_Shared_Logic.Models
{
    /// <summary>
    /// MTM-specific core data models following MTM business patterns.
    /// All models follow the MTM data conventions:
    /// - Part IDs are strings
    /// - Operations are string numbers (e.g., "90", "100", "110")
    /// - Quantities are integers
    /// - UI positions use 1-based indexing
    /// </summary>

    #region User Management Models

    /// <summary>
    /// Represents a user in the MTM WIP Application system.
    /// Maps to usr_users table.
    /// </summary>
    public class User
    {
        public int ID { get; set; }
        public string User_Name { get; set; } = string.Empty; // Column is 'User' but property is User_Name to avoid conflicts

        // Add UserName property for compatibility
        public string UserName => User_Name;
        public string? UserId => User_Name; // For compatibility with existing code

        public string? FullName { get; set; }
        public string Shift { get; set; } = "1";
        public bool VitsUser { get; set; }
        public string? Pin { get; set; }
        public string LastShownVersion { get; set; } = "0.0.0.0";
        public string HideChangeLog { get; set; } = "false";
        public string Theme_Name { get; set; } = "Default (Black and White)";
        public int Theme_FontSize { get; set; } = 9;
        public string VisualUserName { get; set; } = "User Name";
        public string VisualPassword { get; set; } = "Password";
        public string WipServerAddress { get; set; } = "localhost";
        public string WIPDatabase { get; set; } = MTM_WIP_Application_Avalonia.Models.Core.Model_AppVariables.IsDebugMode ? "mtm_wip_application_test" : "mtm_wip_application";
        public string WipServerPort { get; set; } = "3306";
    }

    /// <summary>
    /// Represents a role in the system.
    /// Maps to sys_roles table.
    /// </summary>
    public class Role
    {
        public int ID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Permissions { get; set; }
        public bool IsSystem { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Represents a user role assignment.
    /// Maps to sys_user_roles table.
    /// </summary>
    public class UserRole
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string AssignedBy { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Role? Role { get; set; }
    }

    #endregion

    #region Inventory Models

    /// <summary>
    /// Represents an inventory item in the MTM system.
    /// Maps to inv_inventory table.
    /// Operations are stored as string numbers (e.g., "90", "100", "110").
    /// </summary>
    public class InventoryItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        public int ID { get; set; }
        public string PartID { get; set; } = string.Empty;

        // Add compatibility properties
        public int Id => ID; // For compatibility
        public string PartId => PartID;

        public string Location { get; set; } = string.Empty;
        public string? Operation { get; set; } // String numbers like "90", "100", "110"
        public int Quantity { get; set; }
        public string ItemType { get; set; } = "WIP";
        public DateTime ReceiveDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string User { get; set; } = string.Empty;

        // Add LastUpdatedBy property for compatibility
        public string LastUpdatedBy
        {
            get => User;
            set => User = value;
        }

        public string? BatchNumber { get; set; }
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets whether this inventory item is selected in the UI.
        /// Used for multi-selection scenarios in CustomDataGrid.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// Gets whether this inventory item has notes.
        /// Used for displaying note indicator in the UI.
        /// </summary>
        public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

        /// <summary>
        /// Gets the display text for the inventory item in lists.
        /// Format: (Operation) - [PartID x Quantity]
        /// </summary>
        public string DisplayText => $"({Operation}) - [{PartID} x {Quantity}]";

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    /// <summary>
    /// Represents an inventory transaction in the MTM system.
    /// Maps to inv_transaction table.
    /// TransactionType: IN, OUT, TRANSFER
    /// </summary>
    public class InventoryTransaction
    {
        public int ID { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? BatchNumber { get; set; }
        public string PartID { get; set; } = string.Empty;

        // Add PartId property for compatibility
        public string PartId
        {
            get => PartID;
            set => PartID = value;
        }

        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public string? Operation { get; set; } // String numbers like "90", "100", "110"
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public string User { get; set; } = string.Empty;
        public string ItemType { get; set; } = "WIP";
        public DateTime ReceiveDate { get; set; }

        // Add compatibility properties
        public string? Location
        {
            get => FromLocation;
            set => FromLocation = value;
        }

        public DateTime TransactionDateTime
        {
            get => ReceiveDate;
            set => ReceiveDate = value;
        }
        public string UserName
        {
            get => User;
            set => User = value;
        }
        public string? Comments
        {
            get => Notes;
            set => Notes = value;
        }
    }

    /// <summary>
    /// Transaction types for inventory operations.
    /// </summary>
    public enum TransactionType
    {
        IN,
        OUT,
        TRANSFER
    }

    /// <summary>
    /// Batch number sequence tracking.
    /// Maps to inv_inventory_batch_seq table.
    /// </summary>
    public class BatchSequence
    {
        public long LastBatchNumber { get; set; }
        public int CurrentMatch { get; set; }
    }

    #endregion

    #region Master Data Models

    /// <summary>
    /// Represents a part definition in the MTM system.
    /// Maps to md_part_ids table.
    /// </summary>
    public class PartDefinition
    {
        public int ID { get; set; }
        public string PartID { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public List<string>? Operations { get; set; } // JSON array of operation numbers
    }

    /// <summary>
    /// Represents a valid location in the MTM system.
    /// Maps to md_locations table.
    /// </summary>
    public class Location
    {
        public int ID { get; set; }
        public string LocationCode { get; set; } = string.Empty; // Column is 'Location' but property is LocationCode to avoid conflicts
        public string Building { get; set; } = "Expo";
        public string IssuedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a valid operation number in the MTM system.
    /// Maps to md_operation_numbers table.
    /// Operations are numeric values stored as strings.
    /// </summary>
    public class OperationNumber
    {
        public int ID { get; set; }
        public string Operation { get; set; } = string.Empty; // String number like "90", "100", "110"
        public string IssuedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a valid item type in the MTM system.
    /// Maps to md_item_types table.
    /// </summary>
    public class ItemType
    {
        public int ID { get; set; }
        public string ItemTypeName { get; set; } = string.Empty; // Column is 'ItemType' but property is ItemTypeName to avoid conflicts
        public string IssuedBy { get; set; } = string.Empty;
    }

    #endregion

    #region System Models

    /// <summary>
    /// Represents a quick transaction button for user convenience.
    /// Maps to qb_quickbuttons table.
    /// Position uses 1-based indexing for UI display.
    /// </summary>
    public class QuickTransactionButton
    {
        public int ID { get; set; }
        public string User { get; set; } = string.Empty;
        public string PartID { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty; // String number like "90", "100", "110"
        public int Quantity { get; set; }
        public DateTime ReceiveDate { get; set; }
        public int Position { get; set; } // 1-based indexing for UI display

        /// <summary>
        /// Gets the display text for the quick button.
        /// Format: (Operation) - [PartID x Quantity]
        /// </summary>
        public string DisplayText => $"({Operation}) - [{PartID} x {Quantity}]";
    }

    /// <summary>
    /// Represents application theme configuration.
    /// Maps to app_themes table.
    /// </summary>
    public class Theme
    {
        public string ThemeName { get; set; } = string.Empty;
        public Dictionary<string, object> Settings { get; set; } = new();
    }

    /// <summary>
    /// Represents user UI settings and preferences.
    /// Maps to usr_ui_settings table.
    /// </summary>
    public class UserUISettings
    {
        public string UserId { get; set; } = string.Empty;
        public Dictionary<string, object> Settings { get; set; } = new();
        public Dictionary<string, object> Shortcuts { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }

    #endregion

    #region Logging Models

    /// <summary>
    /// Represents an error log entry.
    /// Maps to log_error table.
    /// </summary>
    public class ErrorLog
    {
        public int ID { get; set; }
        public string? User { get; set; }
        public ErrorSeverity Severity { get; set; } = ErrorSeverity.Error;
        public string? ErrorType { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? ModuleName { get; set; }
        public string? MethodName { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? MachineName { get; set; }
        public string? OSVersion { get; set; }
        public string? AppVersion { get; set; }
        public DateTime ErrorTime { get; set; }
    }

    /// <summary>
    /// Error severity levels.
    /// </summary>
    public enum ErrorSeverity
    {
        Information,
        Warning,
        Error,
        Critical,
        High
    }

    /// <summary>
    /// Represents a changelog entry.
    /// Maps to log_changelog table.
    /// </summary>
    public class ChangelogEntry
    {
        public string Version { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    #endregion

    #region Application State Models

    /// <summary>
    /// Represents the current connection status of the application.
    /// </summary>
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Error,
        Timeout,
        Failed = Error // Add alias for existing code compatibility
    }

    /// <summary>
    /// Validation result for business rule validation.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();

        // Add ErrorMessages property for compatibility
        public List<string> ErrorMessages => Errors;

        public void AddError(string error) => Errors.Add(error);
        public void AddWarning(string warning) => Warnings.Add(warning);

        public static ValidationResult Success() => new() { IsValid = true };
        public static ValidationResult Failure(params string[] errors) => new()
        {
            IsValid = false,
            Errors = new List<string>(errors)
        };
    }

    #endregion

    #region Database Result Models

    /// <summary>
    /// Represents the result of a stored procedure execution with status and output parameters.
    /// Used by DatabaseService for stored procedure operations.
    /// </summary>
    public class DatabaseStoredProcedureResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int Status { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess => Status == 0; // MTM convention: 0 = success
        public Dictionary<string, object> OutputParameters { get; set; } = new Dictionary<string, object>();
        public int RowsAffected => Data.Count;

        /// <summary>
        /// Gets a typed output parameter value.
        /// </summary>
        public TParam? GetOutputParameter<TParam>(string parameterName)
        {
            if (OutputParameters.TryGetValue(parameterName, out var value))
            {
                if (value is TParam typedValue)
                    return typedValue;

                try
                {
                    return (TParam)Convert.ChangeType(value, typeof(TParam));
                }
                catch
                {
                    return default(TParam);
                }
            }

            return default(TParam);
        }
    }

    #endregion

    #region MTM-Specific Business Models

    /// <summary>
    /// Represents a complete inventory operation request.
    /// Used for business logic operations that might involve multiple database calls.
    /// </summary>
    public class InventoryOperationRequest
    {
        public string PartID { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty; // String number
        public string Location { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ItemType { get; set; } = "WIP";
        public string User { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? ToLocation { get; set; } // For TRANSFER operations
        public string? BatchNumber { get; set; }
    }

    /// <summary>
    /// Represents the result of an inventory operation.
    /// Contains both the success/failure status and any generated data.
    /// </summary>
    public class InventoryOperationResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? BatchNumber { get; set; }
        public int AffectedQuantity { get; set; }
        public DateTime OperationTime { get; set; }
        public List<InventoryItem> UpdatedItems { get; set; } = new();
        public List<InventoryTransaction> GeneratedTransactions { get; set; } = new();
    }

    /// <summary>
    /// Represents search criteria for inventory queries.
    /// </summary>
    public class InventorySearchCriteria
    {
        public string? PartID { get; set; }
        public string? Location { get; set; }
        public string? Operation { get; set; }
        public string? ItemType { get; set; }
        public string? User { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
    }

    #endregion
}
