using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MTM_Shared_Logic.Models;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MTM_WIP_Application_Avalonia.Models.Core
{
    /// <summary>
    /// Model for editing inventory items in the MTM WIP Application.
    /// Supports full field editing with validation and change tracking.
    /// Used by inventory editing dialogs, CustomDataGrid editing, and bulk operations.
    /// </summary>
    public partial class EditInventoryModel : ObservableValidator
    {
        #region Core Inventory Fields

        /// <summary>
        /// Gets or sets the inventory item ID (read-only)
        /// </summary>
        [ObservableProperty]
        private int _id;

        /// <summary>
        /// Gets or sets the Part ID
        /// </summary>
        [ObservableProperty]
        [Required(ErrorMessage = "Part ID is required")]
        [StringLength(300, ErrorMessage = "Part ID cannot exceed 300 characters")]
        private string _partId = string.Empty;

        /// <summary>
        /// Gets or sets the Location
        /// </summary>
        [ObservableProperty]
        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        private string _location = string.Empty;

        /// <summary>
        /// Gets or sets the Operation number (string format like "90", "100", "110")
        /// </summary>
        [ObservableProperty]
        [Required(ErrorMessage = "Operation is required")]
        [StringLength(10, ErrorMessage = "Operation cannot exceed 10 characters")]
        private string _operation = string.Empty;

        /// <summary>
        /// Gets or sets the Quantity
        /// </summary>
        [ObservableProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Enter Quantity")]
        private int _quantity;

        /// <summary>
        /// Gets or sets the Item Type
        /// </summary>
        [ObservableProperty]
        [Required(ErrorMessage = "Item Type is required")]
        [StringLength(50, ErrorMessage = "Item Type cannot exceed 50 characters")]
        private string _itemType = "WIP";

        /// <summary>
        /// Gets or sets the Batch Number
        /// </summary>
        [ObservableProperty]
        [StringLength(100, ErrorMessage = "Batch Number cannot exceed 100 characters")]
        private string _batchNumber = string.Empty;

        /// <summary>
        /// Gets or sets the Notes
        /// </summary>
        [ObservableProperty]
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        private string _notes = string.Empty;

        /// <summary>
        /// Gets or sets the User who last updated this item
        /// </summary>
        [ObservableProperty]
        [Required(ErrorMessage = "User is required")]
        [StringLength(100, ErrorMessage = "User cannot exceed 100 characters")]
        private string _user = string.Empty;

        #endregion

        #region Timestamp Fields (Read-Only)

        /// <summary>
        /// Gets or sets the Receive Date (when item was first created)
        /// </summary>
        [ObservableProperty]
        private DateTime _receiveDate = DateTime.Now;

        /// <summary>
        /// Gets or sets the Last Updated timestamp (automatically set on save)
        /// </summary>
        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.Now;

        #endregion

        #region UI State Properties

        /// <summary>
        /// Gets or sets whether this model is in edit mode
        /// </summary>
        [ObservableProperty]
        private bool _isEditing;

        /// <summary>
        /// Gets or sets whether this model has unsaved changes
        /// </summary>
        [ObservableProperty]
        private bool _hasChanges;

        /// <summary>
        /// Gets or sets whether validation is enabled
        /// </summary>
        [ObservableProperty]
        private bool _isValidationEnabled = true;

        /// <summary>
        /// Gets or sets validation error message
        /// </summary>
        [ObservableProperty]
        private string _validationError = string.Empty;

        #endregion

        #region Original Values (Change Tracking)

        private int _originalId;
        private string _originalPartId = string.Empty;
        private string _originalLocation = string.Empty;
        private string _originalOperation = string.Empty;
        private int _originalQuantity;
        private string _originalItemType = string.Empty;
        private string _originalBatchNumber = string.Empty;
        private string _originalNotes = string.Empty;
        private string _originalUser = string.Empty;
        private DateTime _originalReceiveDate;
        private DateTime _originalLastUpdated;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of EditInventoryModel
        /// </summary>
        public EditInventoryModel()
        {
            // Set default values
            PartId = string.Empty;
            Location = string.Empty;
            Operation = string.Empty;
            Quantity = 0;
            ItemType = "WIP";
            BatchNumber = string.Empty;
            Notes = string.Empty;
            User = "SYSTEM";
            ReceiveDate = DateTime.Now;
            LastUpdated = DateTime.Now;

            // Start change tracking
            SaveOriginalValues();
        }

        /// <summary>
        /// Initializes a new instance from an existing InventoryItem
        /// </summary>
        /// <param name="inventoryItem">The inventory item to edit</param>
        public EditInventoryModel(InventoryItem inventoryItem)
        {
            if (inventoryItem == null) throw new ArgumentNullException(nameof(inventoryItem));

            // Load all values from the inventory item
            Id = inventoryItem.Id;
            PartId = inventoryItem.PartId;
            Location = inventoryItem.Location;
            Operation = inventoryItem.Operation ?? string.Empty;
            Quantity = inventoryItem.Quantity;
            ItemType = inventoryItem.ItemType;
            BatchNumber = inventoryItem.BatchNumber ?? string.Empty;
            Notes = inventoryItem.Notes ?? string.Empty;
            User = inventoryItem.User;
            ReceiveDate = inventoryItem.ReceiveDate;
            LastUpdated = inventoryItem.LastUpdated;

            // Start change tracking with current values as original
            SaveOriginalValues();
        }

        #endregion

        #region Change Tracking Methods

        /// <summary>
        /// Saves current values as the original values for change tracking
        /// </summary>
        public void SaveOriginalValues()
        {
            _originalId = Id;
            _originalPartId = PartId;
            _originalLocation = Location;
            _originalOperation = Operation;
            _originalQuantity = Quantity;
            _originalItemType = ItemType;
            _originalBatchNumber = BatchNumber;
            _originalNotes = Notes;
            _originalUser = User;
            _originalReceiveDate = ReceiveDate;
            _originalLastUpdated = LastUpdated;

            HasChanges = false;
        }

        /// <summary>
        /// Reverts all changes to their original values
        /// </summary>
        public void RevertChanges()
        {
            Id = _originalId;
            PartId = _originalPartId;
            Location = _originalLocation;
            Operation = _originalOperation;
            Quantity = _originalQuantity;
            ItemType = _originalItemType;
            BatchNumber = _originalBatchNumber;
            Notes = _originalNotes;
            User = _originalUser;
            ReceiveDate = _originalReceiveDate;
            LastUpdated = _originalLastUpdated;

            HasChanges = false;
            ValidationError = string.Empty;
        }

        /// <summary>
        /// Checks if any values have changed from their originals
        /// </summary>
        /// <returns>True if changes exist, false otherwise</returns>
        public bool CheckForChanges()
        {
            var hasChanges =
                Id != _originalId ||
                !string.Equals(PartId, _originalPartId, StringComparison.Ordinal) ||
                !string.Equals(Location, _originalLocation, StringComparison.Ordinal) ||
                !string.Equals(Operation, _originalOperation, StringComparison.Ordinal) ||
                Quantity != _originalQuantity ||
                !string.Equals(ItemType, _originalItemType, StringComparison.Ordinal) ||
                !string.Equals(BatchNumber, _originalBatchNumber, StringComparison.Ordinal) ||
                !string.Equals(Notes, _originalNotes, StringComparison.Ordinal) ||
                !string.Equals(User, _originalUser, StringComparison.Ordinal) ||
                ReceiveDate != _originalReceiveDate ||
                LastUpdated != _originalLastUpdated;

            HasChanges = hasChanges;
            return hasChanges;
        }

        #endregion

        #region Property Change Handlers

        /// <summary>
        /// Handles property changes to update HasChanges flag
        /// </summary>
        partial void OnPartIdChanged(string value)
        {
            CheckForChanges();
            if (IsValidationEnabled) ValidateProperty(nameof(PartId), value);
        }

        partial void OnLocationChanged(string value)
        {
            CheckForChanges();
            if (IsValidationEnabled) ValidateProperty(nameof(Location), value);
        }

        partial void OnOperationChanged(string value)
        {
            CheckForChanges();
            if (IsValidationEnabled) ValidateProperty(nameof(Operation), value);
        }

        partial void OnQuantityChanged(int value)
        {
            CheckForChanges();
            if (IsValidationEnabled) ValidateProperty(nameof(Quantity), value);
        }

        partial void OnItemTypeChanged(string value)
        {
            CheckForChanges();
            if (IsValidationEnabled) ValidateProperty(nameof(ItemType), value);
        }

        partial void OnBatchNumberChanged(string value)
        {
            CheckForChanges();
        }

        partial void OnNotesChanged(string value)
        {
            CheckForChanges();
        }

        partial void OnUserChanged(string value)
        {
            CheckForChanges();
            if (IsValidationEnabled) ValidateProperty(nameof(User), value);
        }

        partial void OnReceiveDateChanged(DateTime value)
        {
            CheckForChanges();
        }

        partial void OnLastUpdatedChanged(DateTime value)
        {
            CheckForChanges();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates a specific property
        /// </summary>
        /// <param name="propertyName">Name of the property to validate</param>
        /// <param name="value">Value to validate</param>
        private void ValidateProperty(string propertyName, object value)
        {
            var context = new ValidationContext(this) { MemberName = propertyName };
            var results = new System.Collections.Generic.List<ValidationResult>();

            if (!Validator.TryValidateProperty(value, context, results))
            {
                ValidationError = results[0].ErrorMessage ?? $"Invalid value for {propertyName}";
            }
            else if (ValidationError.Contains(propertyName))
            {
                // Clear validation error for this property if it's now valid
                ValidationError = string.Empty;
            }
        }

        /// <summary>
        /// Validates the entire model
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            var context = new ValidationContext(this);
            var results = new System.Collections.Generic.List<ValidationResult>();

            var isValid = Validator.TryValidateObject(this, context, results, true);

            if (!isValid)
            {
                ValidationError = string.Join("; ", results.Select(r => r.ErrorMessage));
            }
            else
            {
                ValidationError = string.Empty;
            }

            return isValid;
        }

        /// <summary>
        /// Custom validation logic (implements IValidatableObject)
        /// </summary>
        /// <param name="validationContext">The validation context</param>
        /// <returns>Collection of validation results</returns>
        public System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new System.Collections.Generic.List<ValidationResult>();

            // Custom validation rules for MTM business logic

            // Part ID format validation (if needed)
            if (!string.IsNullOrWhiteSpace(PartId))
            {
                // Add any specific Part ID format validation here
                if (PartId.Length > 300)
                {
                    results.Add(new ValidationResult("Part ID cannot exceed 300 characters", new[] { nameof(PartId) }));
                }
            }

            // Operation number validation (should be numeric string)
            if (!string.IsNullOrWhiteSpace(Operation))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(Operation, @"^\d+$"))
                {
                    results.Add(new ValidationResult("Operation must be a numeric value", new[] { nameof(Operation) }));
                }
            }

            // Quantity validation
            if (Quantity <= 0)
            {
                results.Add(new ValidationResult("Enter Quantity", new[] { nameof(Quantity) }));
            }

            // Batch number format validation (if applicable)
            if (!string.IsNullOrWhiteSpace(BatchNumber) && BatchNumber.Length > 100)
            {
                results.Add(new ValidationResult("Batch Number cannot exceed 100 characters", new[] { nameof(BatchNumber) }));
            }

            return results;
        }

        #endregion

        #region Conversion Methods

        /// <summary>
        /// Converts this edit model back to an InventoryItem
        /// </summary>
        /// <returns>InventoryItem with current values</returns>
        public InventoryItem ToInventoryItem()
        {
            return new InventoryItem
            {
                Id = Id,
                PartId = PartId,
                Location = Location,
                Operation = Operation,
                Quantity = Quantity,
                ItemType = ItemType,
                BatchNumber = BatchNumber,
                Notes = Notes,
                User = User,
                ReceiveDate = ReceiveDate,
                LastUpdated = LastUpdated
            };
        }

        /// <summary>
        /// Updates an existing InventoryItem with values from this model
        /// </summary>
        /// <param name="inventoryItem">The inventory item to update</param>
        public void UpdateInventoryItem(InventoryItem inventoryItem)
        {
            if (inventoryItem == null) throw new ArgumentNullException(nameof(inventoryItem));

            // Update all editable fields
            inventoryItem.PartId = PartId;
            inventoryItem.Location = Location;
            inventoryItem.Operation = Operation;
            inventoryItem.Quantity = Quantity;
            inventoryItem.ItemType = ItemType;
            inventoryItem.BatchNumber = BatchNumber;
            inventoryItem.Notes = Notes;
            inventoryItem.User = User;
            inventoryItem.LastUpdated = DateTime.Now; // Always update timestamp
            // Note: ID and ReceiveDate typically shouldn't change during editing
        }

        /// <summary>
        /// Creates a copy of this edit model
        /// </summary>
        /// <returns>New EditInventoryModel with copied values</returns>
        public EditInventoryModel Clone()
        {
            return new EditInventoryModel
            {
                Id = Id,
                PartId = PartId,
                Location = Location,
                Operation = Operation,
                Quantity = Quantity,
                ItemType = ItemType,
                BatchNumber = BatchNumber,
                Notes = Notes,
                User = User,
                ReceiveDate = ReceiveDate,
                LastUpdated = LastUpdated,
                IsEditing = IsEditing,
                HasChanges = HasChanges,
                IsValidationEnabled = IsValidationEnabled,
                ValidationError = ValidationError,

                // Copy original values for change tracking
                _originalId = _originalId,
                _originalPartId = _originalPartId,
                _originalLocation = _originalLocation,
                _originalOperation = _originalOperation,
                _originalQuantity = _originalQuantity,
                _originalItemType = _originalItemType,
                _originalBatchNumber = _originalBatchNumber,
                _originalNotes = _originalNotes,
                _originalUser = _originalUser,
                _originalReceiveDate = _originalReceiveDate,
                _originalLastUpdated = _originalLastUpdated
            };
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this edit model
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return $"EditInventoryModel - ID: {Id}, Part: {PartId}, Operation: {Operation}, Location: {Location}, Qty: {Quantity}, Changes: {HasChanges}";
        }

        /// <summary>
        /// Determines equality based on ID and current values
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if equal, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            if (obj is EditInventoryModel other)
            {
                return Id == other.Id &&
                       string.Equals(PartId, other.PartId, StringComparison.Ordinal) &&
                       string.Equals(Operation, other.Operation, StringComparison.Ordinal) &&
                       string.Equals(BatchNumber, other.BatchNumber, StringComparison.Ordinal);
            }

            if (obj is InventoryItem inventoryItem)
            {
                return Id == inventoryItem.Id &&
                       string.Equals(PartId, inventoryItem.PartId, StringComparison.Ordinal) &&
                       string.Equals(Operation, inventoryItem.Operation, StringComparison.Ordinal) &&
                       string.Equals(BatchNumber, inventoryItem.BatchNumber, StringComparison.Ordinal);
            }

            return false;
        }

        /// <summary>
        /// Gets hash code based on ID and key identifying fields
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, PartId, Operation, BatchNumber);
        }

        /// <summary>
        /// Factory method to create EditInventoryModel from InventoryItem
        /// </summary>
        /// <param name="inventoryItem">The source inventory item</param>
        /// <returns>New EditInventoryModel with values from inventory item</returns>
        public static EditInventoryModel FromInventoryItem(InventoryItem inventoryItem)
        {
            return new EditInventoryModel(inventoryItem);
        }

        /// <summary>
        /// Resets change tracking by updating original values to current values
        /// </summary>
        public void ResetChangeTracking()
        {
            _originalId = Id;
            _originalPartId = PartId;
            _originalLocation = Location;
            _originalOperation = Operation;
            _originalQuantity = Quantity;
            _originalItemType = ItemType;
            _originalBatchNumber = BatchNumber;
            _originalNotes = Notes;
            _originalUser = User;
            _originalReceiveDate = ReceiveDate;
            _originalLastUpdated = LastUpdated;
        }

        #endregion
    }
}
