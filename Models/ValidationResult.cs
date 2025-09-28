using System;
using System.Collections.Generic;
using System.Linq;

namespace MTM_WIP_Application_Avalonia.Models
{
    /// <summary>
    /// Validation result model for transfer operations.
    /// Contains validation status, errors, and warnings.
    /// Used by TransferService.ValidateTransferAsync method.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indicates if validation passed
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// List of validation error messages (prevent operation)
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// List of validation warning messages (allow but notify)
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Timestamp when validation was performed
        /// </summary>
        public DateTime ValidatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Additional validation context information
        /// </summary>
        public Dictionary<string, object> Context { get; set; } = new();

        /// <summary>
        /// Creates a successful validation result
        /// </summary>
        /// <param name="warnings">Optional warning messages</param>
        /// <returns>ValidationResult instance</returns>
        public static ValidationResult Success(params string[] warnings)
        {
            return new ValidationResult
            {
                IsValid = true,
                Warnings = warnings?.ToList() ?? new List<string>(),
                ValidatedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a failed validation result
        /// </summary>
        /// <param name="errors">Error messages</param>
        /// <returns>ValidationResult instance</returns>
        public static ValidationResult Failure(params string[] errors)
        {
            return new ValidationResult
            {
                IsValid = false,
                Errors = errors?.ToList() ?? new List<string>(),
                ValidatedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a failed validation result with both errors and warnings
        /// </summary>
        /// <param name="errors">Error messages</param>
        /// <param name="warnings">Warning messages</param>
        /// <returns>ValidationResult instance</returns>
        public static ValidationResult Failure(IEnumerable<string> errors, IEnumerable<string> warnings = null)
        {
            return new ValidationResult
            {
                IsValid = false,
                Errors = errors?.ToList() ?? new List<string>(),
                Warnings = warnings?.ToList() ?? new List<string>(),
                ValidatedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Adds an error message to the validation result
        /// </summary>
        /// <param name="error">Error message</param>
        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                Errors.Add(error);
                IsValid = false;
            }
        }

        /// <summary>
        /// Adds multiple error messages
        /// </summary>
        /// <param name="errors">Error messages</param>
        public void AddErrors(IEnumerable<string> errors)
        {
            if (errors != null)
            {
                foreach (var error in errors.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    Errors.Add(error);
                }
                if (Errors.Count > 0)
                    IsValid = false;
            }
        }

        /// <summary>
        /// Adds a warning message to the validation result
        /// </summary>
        /// <param name="warning">Warning message</param>
        public void AddWarning(string warning)
        {
            if (!string.IsNullOrWhiteSpace(warning))
            {
                Warnings.Add(warning);
            }
        }

        /// <summary>
        /// Adds multiple warning messages
        /// </summary>
        /// <param name="warnings">Warning messages</param>
        public void AddWarnings(IEnumerable<string> warnings)
        {
            if (warnings != null)
            {
                foreach (var warning in warnings.Where(w => !string.IsNullOrWhiteSpace(w)))
                {
                    Warnings.Add(warning);
                }
            }
        }

        /// <summary>
        /// Adds context information
        /// </summary>
        /// <param name="key">Context key</param>
        /// <param name="value">Context value</param>
        public void AddContext(string key, object value)
        {
            Context[key] = value;
        }

        /// <summary>
        /// Gets context value by key
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="key">Context key</param>
        /// <returns>Context value or default</returns>
        public T GetContext<T>(string key)
        {
            if (Context.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            return default(T);
        }

        /// <summary>
        /// Checks if validation has any issues (errors or warnings)
        /// </summary>
        /// <returns>True if there are errors or warnings</returns>
        public bool HasIssues()
        {
            return Errors.Count > 0 || Warnings.Count > 0;
        }

        /// <summary>
        /// Gets total count of issues
        /// </summary>
        /// <returns>Combined count of errors and warnings</returns>
        public int IssueCount()
        {
            return Errors.Count + Warnings.Count;
        }

        /// <summary>
        /// Gets formatted error message
        /// </summary>
        /// <returns>Combined error messages</returns>
        public string GetErrorMessage()
        {
            return Errors.Count > 0 ? string.Join(Environment.NewLine, Errors) : string.Empty;
        }

        /// <summary>
        /// Gets formatted warning message
        /// </summary>
        /// <returns>Combined warning messages</returns>
        public string GetWarningMessage()
        {
            return Warnings.Count > 0 ? string.Join(Environment.NewLine, Warnings) : string.Empty;
        }

        /// <summary>
        /// Gets all messages (errors and warnings) formatted
        /// </summary>
        /// <returns>Combined messages</returns>
        public string GetAllMessages()
        {
            var messages = new List<string>();

            if (Errors.Count > 0)
                messages.Add($"Errors: {string.Join(", ", Errors)}");

            if (Warnings.Count > 0)
                messages.Add($"Warnings: {string.Join(", ", Warnings)}");

            return string.Join(Environment.NewLine, messages);
        }

        /// <summary>
        /// Merges another validation result into this one
        /// </summary>
        /// <param name="other">Other validation result</param>
        public void MergeWith(ValidationResult other)
        {
            if (other == null) return;

            AddErrors(other.Errors);
            AddWarnings(other.Warnings);

            foreach (var context in other.Context)
            {
                Context[context.Key] = context.Value;
            }
        }

        /// <summary>
        /// String representation for logging
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            var result = IsValid ? "VALID" : "INVALID";
            var details = $"Errors: {Errors.Count}, Warnings: {Warnings.Count}";
            return $"ValidationResult: {result} ({details})";
        }
    }
}
