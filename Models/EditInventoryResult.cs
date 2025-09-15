using System;
using System.Collections.Generic;
using System.Linq;

namespace MTM_WIP_Application_Avalonia.Models
{
    /// <summary>
    /// Result model for inventory editing operations.
    /// Provides comprehensive feedback for edit operations including success status, 
    /// error messages, updated data, and operation metadata.
    /// </summary>
    public class EditInventoryResult
    {
        #region Core Result Properties

        /// <summary>
        /// Gets or sets whether the edit operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets any warning messages from the operation
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Gets or sets additional status information
        /// </summary>
        public string? StatusMessage { get; set; }

        #endregion

        #region Updated Data

        /// <summary>
        /// Gets or sets the updated inventory item after successful edit
        /// </summary>
        public MTM_Shared_Logic.Models.InventoryItem? UpdatedInventoryItem { get; set; }

        /// <summary>
        /// Gets or sets the edit model that was processed
        /// </summary>
        public EditInventoryModel? ProcessedEditModel { get; set; }

        /// <summary>
        /// Gets or sets the fields that were actually changed during the edit
        /// </summary>
        public List<string> ChangedFields { get; set; } = new();

        /// <summary>
        /// Gets or sets the old values for changed fields (for audit/rollback purposes)
        /// </summary>
        public Dictionary<string, object?> OldValues { get; set; } = new();

        /// <summary>
        /// Gets or sets the new values for changed fields
        /// </summary>
        public Dictionary<string, object?> NewValues { get; set; } = new();

        #endregion

        #region Operation Metadata

        /// <summary>
        /// Gets or sets the ID of the inventory item that was edited
        /// </summary>
        public int InventoryId { get; set; }

        /// <summary>
        /// Gets or sets the user who performed the edit
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the edit operation was performed
        /// </summary>
        public DateTime OperationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the database operation status code
        /// </summary>
        public int DatabaseStatus { get; set; }

        /// <summary>
        /// Gets or sets the number of database rows affected
        /// </summary>
        public int RowsAffected { get; set; }

        #endregion

        #region Validation Results

        /// <summary>
        /// Gets or sets validation errors that prevented the edit
        /// </summary>
        public List<string> ValidationErrors { get; set; } = new();

        /// <summary>
        /// Gets or sets business rule violations
        /// </summary>
        public List<string> BusinessRuleViolations { get; set; } = new();

        /// <summary>
        /// Gets or sets whether the model passed validation
        /// </summary>
        public bool IsValidModel { get; set; } = true;

        /// <summary>
        /// Gets or sets master data validation results
        /// </summary>
        public Dictionary<string, bool> MasterDataValidation { get; set; } = new();

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a successful edit result
        /// </summary>
        /// <param name="inventoryItem">The updated inventory item</param>
        /// <param name="editModel">The edit model that was processed</param>
        /// <param name="user">The user who performed the edit</param>
        /// <returns>Success EditInventoryResult</returns>
        public static EditInventoryResult CreateSuccess(
            MTM_Shared_Logic.Models.InventoryItem inventoryItem, 
            EditInventoryModel editModel, 
            string user)
        {
            return new EditInventoryResult
            {
                Success = true,
                UpdatedInventoryItem = inventoryItem,
                ProcessedEditModel = editModel,
                InventoryId = inventoryItem.ID,
                User = user,
                OperationTime = DateTime.Now,
                StatusMessage = "Inventory item updated successfully",
                DatabaseStatus = 1,
                RowsAffected = 1
            };
        }

        /// <summary>
        /// Creates a failed edit result due to validation errors
        /// </summary>
        /// <param name="validationErrors">List of validation errors</param>
        /// <param name="editModel">The edit model that failed validation</param>
        /// <returns>Failed EditInventoryResult</returns>
        public static EditInventoryResult ValidationFailure(
            List<string> validationErrors, 
            EditInventoryModel? editModel = null)
        {
            return new EditInventoryResult
            {
                Success = false,
                ValidationErrors = validationErrors ?? new List<string>(),
                ProcessedEditModel = editModel,
                ErrorMessage = "Validation failed: " + string.Join("; ", validationErrors ?? new List<string>()),
                IsValidModel = false,
                DatabaseStatus = -2
            };
        }

        /// <summary>
        /// Creates a failed edit result due to database errors
        /// </summary>
        /// <param name="errorMessage">The database error message</param>
        /// <param name="databaseStatus">The database operation status code</param>
        /// <param name="editModel">The edit model that failed to save</param>
        /// <returns>Failed EditInventoryResult</returns>
        public static EditInventoryResult DatabaseFailure(
            string errorMessage, 
            int databaseStatus, 
            EditInventoryModel? editModel = null)
        {
            return new EditInventoryResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                ProcessedEditModel = editModel,
                DatabaseStatus = databaseStatus,
                RowsAffected = 0,
                InventoryId = editModel?.Id ?? 0
            };
        }

        /// <summary>
        /// Creates a failed edit result due to business rule violations
        /// </summary>
        /// <param name="businessRuleViolations">List of business rule violations</param>
        /// <param name="editModel">The edit model that violated business rules</param>
        /// <returns>Failed EditInventoryResult</returns>
        public static EditInventoryResult BusinessRuleFailure(
            List<string> businessRuleViolations, 
            EditInventoryModel? editModel = null)
        {
            return new EditInventoryResult
            {
                Success = false,
                BusinessRuleViolations = businessRuleViolations ?? new List<string>(),
                ProcessedEditModel = editModel,
                ErrorMessage = "Business rule violations: " + string.Join("; ", businessRuleViolations ?? new List<string>()),
                DatabaseStatus = -3
            };
        }

        /// <summary>
        /// Creates a failed edit result due to master data validation failure
        /// </summary>
        /// <param name="masterDataErrors">Dictionary of field validation failures</param>
        /// <param name="editModel">The edit model with invalid master data</param>
        /// <returns>Failed EditInventoryResult</returns>
        public static EditInventoryResult MasterDataFailure(
            Dictionary<string, bool> masterDataErrors, 
            EditInventoryModel? editModel = null)
        {
            var errorMessages = masterDataErrors
                .Where(kvp => !kvp.Value)
                .Select(kvp => $"Invalid {kvp.Key}")
                .ToList();

            return new EditInventoryResult
            {
                Success = false,
                MasterDataValidation = masterDataErrors,
                ProcessedEditModel = editModel,
                ErrorMessage = "Master data validation failed: " + string.Join("; ", errorMessages),
                ValidationErrors = errorMessages,
                DatabaseStatus = -4
            };
        }

        /// <summary>
        /// Creates a result with warnings but successful operation
        /// </summary>
        /// <param name="inventoryItem">The updated inventory item</param>
        /// <param name="editModel">The edit model that was processed</param>
        /// <param name="user">The user who performed the edit</param>
        /// <param name="warnings">List of warning messages</param>
        /// <returns>Success EditInventoryResult with warnings</returns>
        public static EditInventoryResult SuccessWithWarnings(
            MTM_Shared_Logic.Models.InventoryItem inventoryItem, 
            EditInventoryModel editModel, 
            string user,
            List<string> warnings)
        {
            var result = CreateSuccess(inventoryItem, editModel, user);
            result.Warnings = warnings ?? new List<string>();
            result.StatusMessage = $"Inventory item updated successfully with {warnings?.Count ?? 0} warning(s)";
            return result;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Adds a warning message to the result
        /// </summary>
        /// <param name="warning">The warning message</param>
        public void AddWarning(string warning)
        {
            if (!string.IsNullOrWhiteSpace(warning) && !Warnings.Contains(warning))
            {
                Warnings.Add(warning);
            }
        }

        /// <summary>
        /// Adds a validation error to the result
        /// </summary>
        /// <param name="validationError">The validation error</param>
        public void AddValidationError(string validationError)
        {
            if (!string.IsNullOrWhiteSpace(validationError) && !ValidationErrors.Contains(validationError))
            {
                ValidationErrors.Add(validationError);
                IsValidModel = false;
                
                if (Success)
                {
                    Success = false;
                    ErrorMessage = "Validation failed: " + string.Join("; ", ValidationErrors);
                }
            }
        }

        /// <summary>
        /// Adds a business rule violation to the result
        /// </summary>
        /// <param name="violation">The business rule violation</param>
        public void AddBusinessRuleViolation(string violation)
        {
            if (!string.IsNullOrWhiteSpace(violation) && !BusinessRuleViolations.Contains(violation))
            {
                BusinessRuleViolations.Add(violation);
                
                if (Success)
                {
                    Success = false;
                    ErrorMessage = "Business rule violations: " + string.Join("; ", BusinessRuleViolations);
                }
            }
        }

        /// <summary>
        /// Records a field change for audit purposes
        /// </summary>
        /// <param name="fieldName">Name of the field that changed</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        public void RecordFieldChange(string fieldName, object? oldValue, object? newValue)
        {
            if (!ChangedFields.Contains(fieldName))
            {
                ChangedFields.Add(fieldName);
            }
            
            OldValues[fieldName] = oldValue;
            NewValues[fieldName] = newValue;
        }

        /// <summary>
        /// Sets master data validation result for a specific field
        /// </summary>
        /// <param name="fieldName">The field name (PartId, Operation, Location, etc.)</param>
        /// <param name="isValid">Whether the field value is valid</param>
        public void SetMasterDataValidation(string fieldName, bool isValid)
        {
            MasterDataValidation[fieldName] = isValid;
            
            if (!isValid)
            {
                AddValidationError($"Invalid {fieldName} - not found in master data");
            }
        }

        /// <summary>
        /// Gets whether the result has any errors or warnings
        /// </summary>
        /// <returns>True if there are issues, false if clean success</returns>
        public bool HasIssues()
        {
            return !Success || 
                   ValidationErrors.Any() || 
                   BusinessRuleViolations.Any() || 
                   Warnings.Any();
        }

        /// <summary>
        /// Gets a summary message for the edit operation
        /// </summary>
        /// <returns>Human-readable summary</returns>
        public string GetSummary()
        {
            if (Success)
            {
                var summary = $"Successfully updated inventory item {InventoryId}";
                if (ChangedFields.Any())
                {
                    summary += $" - Changed: {string.Join(", ", ChangedFields)}";
                }
                if (Warnings.Any())
                {
                    summary += $" (with {Warnings.Count} warning(s))";
                }
                return summary;
            }
            else
            {
                return $"Failed to update inventory item {InventoryId}: {ErrorMessage}";
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this edit result
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return GetSummary();
        }

        #endregion
    }
}