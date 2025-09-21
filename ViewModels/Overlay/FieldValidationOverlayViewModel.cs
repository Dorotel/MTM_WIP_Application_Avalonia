using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay
{
    /// <summary>
    /// ViewModel for the Field Validation overlay providing real-time field validation
    /// with error highlighting, suggestions, and validation rules display.
    /// </summary>
    public partial class FieldValidationOverlayViewModel : BaseOverlayViewModel
    {
        [ObservableProperty]
        private string _fieldName = string.Empty;

        [ObservableProperty]
        private string _fieldValue = string.Empty;

        [ObservableProperty]
        private string _fieldLabel = string.Empty;

        [ObservableProperty]
        private bool _hasValidationErrors;

        [ObservableProperty]
        private bool _isValidating;

        [ObservableProperty]
        private string _validationMessage = string.Empty;

        [ObservableProperty]
        private ValidationType _validationType = ValidationType.None;

        [ObservableProperty]
        private string _suggestionText = string.Empty;

        [ObservableProperty]
        private bool _showSuggestions;

        [ObservableProperty]
        private string _helpText = string.Empty;

        public ObservableCollection<ValidationRule> ValidationRules { get; } = new();
        public ObservableCollection<string> Suggestions { get; } = new();
        public ObservableCollection<ValidationError> ValidationErrors { get; } = new();

        public FieldValidationOverlayViewModel(ILogger<FieldValidationOverlayViewModel> logger) : base(logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            Title = "Field Validation";
        }

        /// <summary>
        /// Initializes the field validation overlay with specific field context
        /// </summary>
        public async Task InitializeValidationAsync(string fieldName, string fieldValue, string fieldLabel, ValidationType validationType = ValidationType.None)
        {
            try
            {
                FieldName = fieldName;
                FieldValue = fieldValue;
                FieldLabel = fieldLabel;
                ValidationType = validationType;

                // Load validation rules for the field
                await LoadValidationRulesAsync();

                // Load field-specific help text
                LoadHelpText();

                // Load suggestions if applicable
                await LoadSuggestionsAsync();

                // Perform initial validation
                await ValidateFieldAsync();

                _logger.LogInformation("Field validation initialized for {FieldName}", fieldName);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to initialize field validation");
            }
        }

        /// <summary>
        /// Validates the current field value in real-time
        /// </summary>
        [RelayCommand]
        public async Task ValidateFieldAsync()
        {
            if (IsValidating) return;

            IsValidating = true;
            ValidationErrors.Clear();

            try
            {
                var errors = new List<ValidationError>();

                // Apply validation rules
                foreach (var rule in ValidationRules.Where(r => r.IsEnabled))
                {
                    var validationResult = await ApplyValidationRuleAsync(rule, FieldValue);
                    if (!validationResult.IsValid)
                    {
                        errors.Add(new ValidationError
                        {
                            RuleName = rule.Name,
                            ErrorMessage = validationResult.ErrorMessage,
                            Severity = rule.Severity
                        });
                    }
                }

                // Update validation state
                foreach (var error in errors)
                {
                    ValidationErrors.Add(error);
                }

                HasValidationErrors = ValidationErrors.Any();
                UpdateValidationMessage();

                _logger.LogDebug("Field validation completed for {FieldName}: {ErrorCount} errors", FieldName, ValidationErrors.Count);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, $"Failed to validate field {FieldName}");
            }
            finally
            {
                IsValidating = false;
            }
        }

        /// <summary>
        /// Applies a suggestion to the field value
        /// </summary>
        [RelayCommand]
        public async Task ApplySuggestionAsync(string suggestion)
        {
            if (string.IsNullOrEmpty(suggestion)) return;

            try
            {
                FieldValue = suggestion;
                ShowSuggestions = false;

                // Re-validate with new value
                await ValidateFieldAsync();

                _logger.LogInformation("Applied suggestion '{Suggestion}' to field {FieldName}", suggestion, FieldName);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to apply suggestion");
            }
        }

        /// <summary>
        /// Shows help information for the current field
        /// </summary>
        [RelayCommand]
        public async Task ShowHelpAsync()
        {
            try
            {
                // Load comprehensive help text
                await LoadDetailedHelpAsync();

                _logger.LogInformation("Showed help for field {FieldName}", FieldName);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to show field help");
            }
        }

        /// <summary>
        /// Accepts the current field value and closes the overlay
        /// </summary>
        [RelayCommand]
        public async Task AcceptValidationAsync()
        {
            try
            {
                if (HasValidationErrors)
                {
                    var criticalErrors = ValidationErrors.Where(e => e.Severity == ValidationSeverity.Error).ToList();
                    if (criticalErrors.Any())
                    {
                        ValidationMessage = $"Cannot accept field with {criticalErrors.Count} critical error(s)";
                        return;
                    }
                }

                // Field value is acceptable
                var result = new FieldValidationResult
                {
                    FieldName = FieldName,
                    AcceptedValue = FieldValue,
                    IsValid = !HasValidationErrors || ValidationErrors.All(e => e.Severity != ValidationSeverity.Error),
                    ValidationErrors = ValidationErrors.ToList()
                };

                await HideAsync();

                _logger.LogInformation("Accepted field validation for {FieldName} with value '{Value}'", FieldName, FieldValue);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to accept field validation");
            }
        }

        /// <summary>
        /// Cancels the field validation and closes the overlay
        /// </summary>
        [RelayCommand]
        public async Task CancelValidationAsync()
        {
            try
            {
                await HideAsync();
                _logger.LogInformation("Cancelled field validation for {FieldName}", FieldName);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to cancel field validation");
            }
        }

        /// <summary>
        /// Loads validation rules based on field type and context
        /// </summary>
        private async Task LoadValidationRulesAsync()
        {
            await Task.Run(() =>
            {
                ValidationRules.Clear();

                // Load rules based on validation type
                switch (ValidationType)
                {
                    case ValidationType.PartId:
                        LoadPartIdValidationRules();
                        break;
                    case ValidationType.Quantity:
                        LoadQuantityValidationRules();
                        break;
                    case ValidationType.Location:
                        LoadLocationValidationRules();
                        break;
                    case ValidationType.Operation:
                        LoadOperationValidationRules();
                        break;
                    case ValidationType.Required:
                        LoadRequiredFieldValidationRules();
                        break;
                    case ValidationType.Email:
                        LoadEmailValidationRules();
                        break;
                    case ValidationType.Numeric:
                        LoadNumericValidationRules();
                        break;
                    default:
                        LoadDefaultValidationRules();
                        break;
                }
            });
        }

        /// <summary>
        /// Loads contextual suggestions for the field
        /// </summary>
        private async Task LoadSuggestionsAsync()
        {
            await Task.Run(() =>
            {
                Suggestions.Clear();

                // Load suggestions based on validation type
                switch (ValidationType)
                {
                    case ValidationType.PartId:
                        LoadPartIdSuggestions();
                        break;
                    case ValidationType.Location:
                        LoadLocationSuggestions();
                        break;
                    case ValidationType.Operation:
                        LoadOperationSuggestions();
                        break;
                }

                ShowSuggestions = Suggestions.Any();
            });
        }

        /// <summary>
        /// Applies a single validation rule to a field value
        /// </summary>
        private async Task<ValidationResult> ApplyValidationRuleAsync(ValidationRule rule, string value)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Apply rule logic based on rule type
                    return rule.RuleType switch
                    {
                        ValidationRuleType.Required => ValidateRequired(value, rule),
                        ValidationRuleType.MinLength => ValidateMinLength(value, rule),
                        ValidationRuleType.MaxLength => ValidateMaxLength(value, rule),
                        ValidationRuleType.Pattern => ValidatePattern(value, rule),
                        ValidationRuleType.Range => ValidateRange(value, rule),
                        ValidationRuleType.Custom => ValidateCustom(value, rule),
                        _ => new ValidationResult { IsValid = true }
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Validation rule {RuleName} failed to execute", rule.Name);
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Validation rule execution failed"
                    };
                }
            });
        }

        #region Validation Rule Loaders

        private void LoadPartIdValidationRules()
        {
            ValidationRules.Add(new ValidationRule
            {
                Name = "Required",
                Description = "Part ID is required",
                RuleType = ValidationRuleType.Required,
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });

            ValidationRules.Add(new ValidationRule
            {
                Name = "Length",
                Description = "Part ID must be 3-20 characters",
                RuleType = ValidationRuleType.MinLength,
                MinValue = 3,
                MaxValue = 20,
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });

            ValidationRules.Add(new ValidationRule
            {
                Name = "Format",
                Description = "Part ID should contain only alphanumeric characters and dashes",
                RuleType = ValidationRuleType.Pattern,
                Pattern = @"^[A-Za-z0-9\-]+$",
                Severity = ValidationSeverity.Warning,
                IsEnabled = true
            });
        }

        private void LoadQuantityValidationRules()
        {
            ValidationRules.Add(new ValidationRule
            {
                Name = "Required",
                Description = "Quantity is required",
                RuleType = ValidationRuleType.Required,
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });

            ValidationRules.Add(new ValidationRule
            {
                Name = "Numeric",
                Description = "Quantity must be a positive number",
                RuleType = ValidationRuleType.Range,
                MinValue = 1,
                MaxValue = int.MaxValue,
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });
        }

        private void LoadLocationValidationRules()
        {
            ValidationRules.Add(new ValidationRule
            {
                Name = "Required",
                Description = "Location is required",
                RuleType = ValidationRuleType.Required,
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });
        }

        private void LoadOperationValidationRules()
        {
            ValidationRules.Add(new ValidationRule
            {
                Name = "Required",
                Description = "Operation is required",
                RuleType = ValidationRuleType.Required,
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });
        }

        private void LoadRequiredFieldValidationRules()
        {
            ValidationRules.Add(new ValidationRule
            {
                Name = "Required",
                Description = "This field is required",
                RuleType = ValidationRuleType.Required,
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });
        }

        private void LoadEmailValidationRules()
        {
            ValidationRules.Add(new ValidationRule
            {
                Name = "Email Format",
                Description = "Must be a valid email address",
                RuleType = ValidationRuleType.Pattern,
                Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });
        }

        private void LoadNumericValidationRules()
        {
            ValidationRules.Add(new ValidationRule
            {
                Name = "Numeric",
                Description = "Must be a valid number",
                RuleType = ValidationRuleType.Pattern,
                Pattern = @"^\d+(\.\d+)?$",
                Severity = ValidationSeverity.Error,
                IsEnabled = true
            });
        }

        private void LoadDefaultValidationRules()
        {
            // No default rules
        }

        #endregion

        #region Suggestion Loaders

        private void LoadPartIdSuggestions()
        {
            // These would typically come from a service
            var commonParts = new[] { "PART001", "PART002", "ABC-123", "DEF-456" };

            foreach (var part in commonParts.Where(p => p.Contains(FieldValue, StringComparison.OrdinalIgnoreCase)))
            {
                Suggestions.Add(part);
            }
        }

        private void LoadLocationSuggestions()
        {
            // These would typically come from master data service
            var commonLocations = new[] { "A1-01", "A1-02", "B2-01", "B2-02" };

            foreach (var location in commonLocations.Where(l => l.Contains(FieldValue, StringComparison.OrdinalIgnoreCase)))
            {
                Suggestions.Add(location);
            }
        }

        private void LoadOperationSuggestions()
        {
            var commonOperations = new[] { "90", "100", "110", "120" };

            foreach (var operation in commonOperations.Where(o => o.Contains(FieldValue, StringComparison.OrdinalIgnoreCase)))
            {
                Suggestions.Add(operation);
            }
        }

        #endregion

        #region Validation Methods

        private ValidationResult ValidateRequired(string value, ValidationRule rule)
        {
            var isValid = !string.IsNullOrWhiteSpace(value);
            return new ValidationResult
            {
                IsValid = isValid,
                ErrorMessage = isValid ? string.Empty : rule.Description
            };
        }

        private ValidationResult ValidateMinLength(string value, ValidationRule rule)
        {
            var isValid = string.IsNullOrEmpty(value) || value.Length >= rule.MinValue;
            return new ValidationResult
            {
                IsValid = isValid,
                ErrorMessage = isValid ? string.Empty : $"Minimum length is {rule.MinValue} characters"
            };
        }

        private ValidationResult ValidateMaxLength(string value, ValidationRule rule)
        {
            var isValid = string.IsNullOrEmpty(value) || value.Length <= rule.MaxValue;
            return new ValidationResult
            {
                IsValid = isValid,
                ErrorMessage = isValid ? string.Empty : $"Maximum length is {rule.MaxValue} characters"
            };
        }

        private ValidationResult ValidatePattern(string value, ValidationRule rule)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(rule.Pattern))
                return new ValidationResult { IsValid = true };

            try
            {
                var isValid = System.Text.RegularExpressions.Regex.IsMatch(value, rule.Pattern);
                return new ValidationResult
                {
                    IsValid = isValid,
                    ErrorMessage = isValid ? string.Empty : rule.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid regex pattern: {Pattern}", rule.Pattern);
                return new ValidationResult { IsValid = true };
            }
        }

        private ValidationResult ValidateRange(string value, ValidationRule rule)
        {
            if (string.IsNullOrEmpty(value))
                return new ValidationResult { IsValid = true };

            if (int.TryParse(value, out var intValue))
            {
                var isValid = intValue >= rule.MinValue && intValue <= rule.MaxValue;
                return new ValidationResult
                {
                    IsValid = isValid,
                    ErrorMessage = isValid ? string.Empty : $"Value must be between {rule.MinValue} and {rule.MaxValue}"
                };
            }

            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Value must be a valid number"
            };
        }

        private ValidationResult ValidateCustom(string value, ValidationRule rule)
        {
            // Custom validation would be implemented based on specific business rules
            return new ValidationResult { IsValid = true };
        }

        #endregion

        private void LoadHelpText()
        {
            HelpText = ValidationType switch
            {
                ValidationType.PartId => "Enter a valid part identifier. Part IDs should be 3-20 characters containing letters, numbers, and dashes.",
                ValidationType.Quantity => "Enter a positive whole number representing the quantity of items.",
                ValidationType.Location => "Enter a valid location code. Use the suggestions or browse available locations.",
                ValidationType.Operation => "Enter a valid operation number (e.g., 90, 100, 110, 120).",
                ValidationType.Email => "Enter a valid email address in the format name@domain.com",
                ValidationType.Numeric => "Enter a valid number. Decimal numbers are allowed.",
                ValidationType.Required => "This field is required and cannot be left empty.",
                _ => "Enter a valid value for this field."
            };
        }

        private async Task LoadDetailedHelpAsync()
        {
            await Task.Run(() =>
            {
                // Load more comprehensive help text
                HelpText = ValidationType switch
                {
                    ValidationType.PartId => @"Part ID Guidelines:
â€¢ Must be 3-20 characters long
â€¢ Can contain letters (A-Z, a-z), numbers (0-9), and dashes (-)
â€¢ Should be unique within the system
â€¢ Examples: PART001, ABC-123, ENGINE-V8-001
â€¢ Avoid special characters like spaces, periods, or symbols",

                    ValidationType.Quantity => @"Quantity Guidelines:
â€¢ Must be a positive whole number (1, 2, 3, etc.)
â€¢ Cannot be zero or negative
â€¢ Maximum value depends on system limits
â€¢ For decimal quantities, use the decimal field type instead",

                    ValidationType.Location => @"Location Guidelines:
â€¢ Must match an existing location in the system
â€¢ Format typically follows warehouse-aisle-position pattern
â€¢ Examples: A1-01, B2-15, C3-BULK
â€¢ Use suggestions or location browser for valid options",

                    ValidationType.Operation => @"Operation Guidelines:
â€¢ Must be a valid operation number from the workflow
â€¢ Common operations: 90 (Receiving), 100 (Inspection), 110 (Storage), 120 (Shipping)
â€¢ Operations define the workflow step for the part
â€¢ Contact supervisor for custom operation numbers",

                    _ => HelpText
                };
            });
        }

        private void UpdateValidationMessage()
        {
            if (!HasValidationErrors)
            {
                ValidationMessage = "Field validation passed";
                return;
            }

            var errorCount = ValidationErrors.Count(e => e.Severity == ValidationSeverity.Error);
            var warningCount = ValidationErrors.Count(e => e.Severity == ValidationSeverity.Warning);

            if (errorCount > 0)
            {
                ValidationMessage = $"{errorCount} error(s)";
                if (warningCount > 0)
                    ValidationMessage += $", {warningCount} warning(s)";
            }
            else if (warningCount > 0)
            {
                ValidationMessage = $"{warningCount} warning(s)";
            }
        }
    }

    #region Supporting Models

    public class ValidationRule
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ValidationRuleType RuleType { get; set; }
        public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
        public bool IsEnabled { get; set; } = true;
        public string Pattern { get; set; } = string.Empty;
        public int MinValue { get; set; }
        public int MaxValue { get; set; } = int.MaxValue;
    }

    public class ValidationError
    {
        public string RuleName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class FieldValidationResult
    {
        public string FieldName { get; set; } = string.Empty;
        public string AcceptedValue { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public List<ValidationError> ValidationErrors { get; set; } = new();
    }

    public enum ValidationType
    {
        None,
        Required,
        PartId,
        Quantity,
        Location,
        Operation,
        Email,
        Numeric
    }

    public enum ValidationRuleType
    {
        Required,
        MinLength,
        MaxLength,
        Pattern,
        Range,
        Custom
    }

    public enum ValidationSeverity
    {
        Information,
        Warning,
        Error
    }

    #endregion
}
