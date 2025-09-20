using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MTM_Shared_Logic.Models;

namespace MTM_WIP_Application_Avalonia.Models.CustomDataGrid.UI;

/// <summary>
/// Transfer-specific inventory item model for the TransferCustomDataGrid.
/// Extends the base InventoryItem with transfer-specific properties and validation.
/// Follows MTM patterns with MVVM Community Toolkit observability.
/// </summary>
public partial class TransferInventoryItem : ObservableValidator
{
    #region Base Inventory Properties

    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    private string _partId = string.Empty;

    [ObservableProperty]
    private string? _operation;

    [ObservableProperty]
    [Required(ErrorMessage = "Location is required")]
    private string _location = string.Empty;

    [ObservableProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    private int _quantity;

    [ObservableProperty]
    private string _itemType = string.Empty;

    [ObservableProperty]
    private string? _batchNumber;

    [ObservableProperty]
    private string? _user;

    [ObservableProperty]
    private DateTime _receiveDate;

    [ObservableProperty]
    private DateTime _lastUpdated;

    [ObservableProperty]
    private string? _notes;

    #endregion

    #region Transfer-Specific Properties

    [ObservableProperty]
    [Required(ErrorMessage = "From location is required")]
    private string _fromLocation = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "To location is required")]
    private string _toLocation = string.Empty;

    [ObservableProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Transfer quantity must be at least 1")]
    private int _transferQuantity = 1;

    [ObservableProperty]
    private int _availableQuantity;

    [ObservableProperty]
    private bool _isTransferValid = true;

    [ObservableProperty]
    private List<string> _validationErrors = new();

    [ObservableProperty]
    private bool _hasNotes;

    [ObservableProperty]
    private string _transferStatus = "Pending";

    [ObservableProperty]
    private DateTime? _plannedTransferDate;

    [ObservableProperty]
    private string? _transferReason;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    public TransferInventoryItem()
    {
    }

    /// <summary>
    /// Creates a TransferInventoryItem from an existing InventoryItem
    /// </summary>
    /// <param name="inventoryItem">The source inventory item</param>
    /// <param name="toLocation">The destination location for transfer</param>
    public TransferInventoryItem(InventoryItem inventoryItem, string toLocation = "")
    {
        ArgumentNullException.ThrowIfNull(inventoryItem);

        // Copy base properties
        Id = inventoryItem.Id;
        PartId = inventoryItem.PartId;
        Operation = inventoryItem.Operation;
        Location = inventoryItem.Location;
        Quantity = inventoryItem.Quantity;
        ItemType = inventoryItem.ItemType;
        BatchNumber = inventoryItem.BatchNumber;
        User = inventoryItem.User;
        ReceiveDate = inventoryItem.ReceiveDate;
        LastUpdated = inventoryItem.LastUpdated;
        Notes = inventoryItem.Notes;

        // Set transfer-specific properties
        FromLocation = inventoryItem.Location;
        ToLocation = toLocation;
        AvailableQuantity = inventoryItem.Quantity;
        TransferQuantity = Math.Min(1, inventoryItem.Quantity); // Default to 1 or available quantity

        // Initialize computed properties
        UpdateHasNotes();
    }

    #endregion

    #region Property Change Handlers

    partial void OnNotesChanged(string? value)
    {
        UpdateHasNotes();
    }

    partial void OnValidationErrorsChanged(List<string> value)
    {
        IsTransferValid = value.Count == 0;
    }

    partial void OnTransferQuantityChanged(int value)
    {
        // Ensure transfer quantity doesn't exceed available quantity
        if (value > AvailableQuantity)
        {
            TransferQuantity = AvailableQuantity;
        }

        // Validate transfer quantity
        ValidateTransferQuantity();
    }

    partial void OnFromLocationChanged(string value)
    {
        ValidateLocations();
    }

    partial void OnToLocationChanged(string value)
    {
        ValidateLocations();
    }

    #endregion

    #region Validation Methods

    /// <summary>
    /// Validates the transfer configuration
    /// </summary>
    public void ValidateTransfer()
    {
        ValidationErrors.Clear();

        ValidateTransferQuantity();
        ValidateLocations();
        ValidateBasicProperties();

        IsTransferValid = ValidationErrors.Count == 0;
    }

    private void ValidateTransferQuantity()
    {
        if (TransferQuantity <= 0)
        {
            AddValidationError("Transfer quantity must be greater than 0");
        }

        if (TransferQuantity > AvailableQuantity)
        {
            AddValidationError($"Transfer quantity cannot exceed available quantity ({AvailableQuantity})");
        }
    }

    private void ValidateLocations()
    {
        if (string.IsNullOrWhiteSpace(FromLocation))
        {
            AddValidationError("From location is required");
        }

        if (string.IsNullOrWhiteSpace(ToLocation))
        {
            AddValidationError("To location is required");
        }

        if (!string.IsNullOrWhiteSpace(FromLocation) && !string.IsNullOrWhiteSpace(ToLocation) &&
            FromLocation.Equals(ToLocation, StringComparison.OrdinalIgnoreCase))
        {
            AddValidationError("From and To locations cannot be the same");
        }
    }

    private void ValidateBasicProperties()
    {
        if (string.IsNullOrWhiteSpace(PartId))
        {
            AddValidationError("Part ID is required");
        }

        if (AvailableQuantity <= 0)
        {
            AddValidationError("Available quantity must be greater than 0");
        }
    }

    private void AddValidationError(string error)
    {
        if (!ValidationErrors.Contains(error))
        {
            ValidationErrors.Add(error);
        }
    }

    #endregion

    #region Helper Methods

    private void UpdateHasNotes()
    {
        HasNotes = !string.IsNullOrWhiteSpace(Notes);
    }

    /// <summary>
    /// Gets a summary of the transfer operation
    /// </summary>
    public string GetTransferSummary()
    {
        return $"{PartId} ({TransferQuantity}/{AvailableQuantity}) from {FromLocation} → {ToLocation}";
    }

    /// <summary>
    /// Creates a copy of this transfer item
    /// </summary>
    public TransferInventoryItem Clone()
    {
        return new TransferInventoryItem
        {
            // Base properties
            Id = Id,
            PartId = PartId,
            Operation = Operation,
            Location = Location,
            Quantity = Quantity,
            ItemType = ItemType,
            BatchNumber = BatchNumber,
            User = User,
            ReceiveDate = ReceiveDate,
            LastUpdated = LastUpdated,
            Notes = Notes,

            // Transfer properties
            FromLocation = FromLocation,
            ToLocation = ToLocation,
            TransferQuantity = TransferQuantity,
            AvailableQuantity = AvailableQuantity,
            IsTransferValid = IsTransferValid,
            ValidationErrors = new List<string>(ValidationErrors),
            TransferStatus = TransferStatus,
            PlannedTransferDate = PlannedTransferDate,
            TransferReason = TransferReason
        };
    }

    /// <summary>
    /// Converts this transfer item back to a standard InventoryItem
    /// </summary>
    public InventoryItem ToInventoryItem()
    {
        return new InventoryItem
        {
            Id = Id,
            PartId = PartId,
            Operation = Operation ?? string.Empty,
            Location = ToLocation, // Use destination location
            Quantity = TransferQuantity, // Use transfer quantity
            ItemType = ItemType,
            BatchNumber = BatchNumber ?? string.Empty,
            User = User ?? string.Empty,
            ReceiveDate = ReceiveDate,
            LastUpdated = DateTime.Now,
            Notes = Notes ?? string.Empty
        };
    }

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Creates a TransferInventoryItem from an InventoryItem with specified destination
    /// </summary>
    public static TransferInventoryItem FromInventoryItem(InventoryItem item, string toLocation = "")
    {
        return new TransferInventoryItem(item, toLocation);
    }

    /// <summary>
    /// Creates multiple TransferInventoryItems from a collection of InventoryItems
    /// </summary>
    public static List<TransferInventoryItem> FromInventoryItems(
        IEnumerable<InventoryItem> items,
        string defaultToLocation = "")
    {
        return items.Select(item => FromInventoryItem(item, defaultToLocation)).ToList();
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return GetTransferSummary();
    }

    public override bool Equals(object? obj)
    {
        if (obj is TransferInventoryItem other)
        {
            return Id == other.Id &&
                   PartId.Equals(other.PartId, StringComparison.OrdinalIgnoreCase) &&
                   FromLocation.Equals(other.FromLocation, StringComparison.OrdinalIgnoreCase) &&
                   ToLocation.Equals(other.ToLocation, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, PartId, FromLocation, ToLocation);
    }

    #endregion
}
