using System;
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models
{
    /// <summary>
    /// Transfer operation model representing a single inventory transfer request.
    /// Contains all data needed to execute a transfer from one location to another.
    /// Used by TransferService for business logic and validation.
    /// </summary>
    public class TransferOperation
    {
        /// <summary>
        /// Part identifier being transferred
        /// </summary>
        [Required(ErrorMessage = "Part ID is required")]
        public string PartId { get; set; } = string.Empty;

        /// <summary>
        /// Operation number (90, 100, 110, 120)
        /// </summary>
        [Required(ErrorMessage = "Operation is required")]
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// Source location identifier
        /// </summary>
        [Required(ErrorMessage = "From location is required")]
        public string FromLocation { get; set; } = string.Empty;

        /// <summary>
        /// Destination location identifier
        /// </summary>
        [Required(ErrorMessage = "To location is required")]
        public string ToLocation { get; set; } = string.Empty;

        /// <summary>
        /// Quantity to transfer (will be auto-capped if exceeds available)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Transfer quantity must be positive")]
        public int TransferQuantity { get; set; }

        /// <summary>
        /// User ID of the operator performing the transfer
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Optional notes for the transfer operation
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Batch number (preserved from source inventory)
        /// </summary>
        public string BatchNumber { get; set; } = string.Empty;

        /// <summary>
        /// Available quantity at source (for validation)
        /// </summary>
        public int AvailableQuantity { get; set; }

        /// <summary>
        /// Timestamp when transfer operation was requested
        /// </summary>
        public DateTime RequestedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Validates the transfer operation
        /// </summary>
        /// <returns>True if all required fields are valid</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(PartId) &&
                   !string.IsNullOrWhiteSpace(Operation) &&
                   !string.IsNullOrWhiteSpace(FromLocation) &&
                   !string.IsNullOrWhiteSpace(ToLocation) &&
                   !string.IsNullOrWhiteSpace(UserId) &&
                   TransferQuantity > 0 &&
                   FromLocation != ToLocation;
        }

        /// <summary>
        /// Gets validation error messages
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(PartId))
                errors.Add("Part ID is required");

            if (string.IsNullOrWhiteSpace(Operation))
                errors.Add("Operation is required");

            if (string.IsNullOrWhiteSpace(FromLocation))
                errors.Add("From location is required");

            if (string.IsNullOrWhiteSpace(ToLocation))
                errors.Add("To location is required");

            if (string.IsNullOrWhiteSpace(UserId))
                errors.Add("User ID is required");

            if (TransferQuantity <= 0)
                errors.Add("Transfer quantity must be positive");

            if (FromLocation == ToLocation)
                errors.Add("Source and destination locations cannot be the same");

            if (TransferQuantity > AvailableQuantity && AvailableQuantity > 0)
                errors.Add($"Transfer quantity ({TransferQuantity}) exceeds available quantity ({AvailableQuantity})");

            return errors;
        }

        /// <summary>
        /// Creates a copy of the transfer operation
        /// </summary>
        /// <returns>New TransferOperation instance</returns>
        public TransferOperation Clone()
        {
            return new TransferOperation
            {
                PartId = PartId,
                Operation = Operation,
                FromLocation = FromLocation,
                ToLocation = ToLocation,
                TransferQuantity = TransferQuantity,
                UserId = UserId,
                Notes = Notes,
                BatchNumber = BatchNumber,
                AvailableQuantity = AvailableQuantity,
                RequestedAt = RequestedAt
            };
        }

        /// <summary>
        /// Auto-caps transfer quantity to available quantity if needed
        /// </summary>
        /// <returns>True if quantity was capped</returns>
        public bool AutoCapQuantity()
        {
            if (AvailableQuantity > 0 && TransferQuantity > AvailableQuantity)
            {
                TransferQuantity = AvailableQuantity;
                return true;
            }
            return false;
        }

        /// <summary>
        /// String representation for logging and debugging
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"Transfer: {PartId} Op:{Operation} {TransferQuantity} from {FromLocation} to {ToLocation} by {UserId}";
        }
    }
}
