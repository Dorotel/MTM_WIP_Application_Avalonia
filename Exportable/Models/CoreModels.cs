using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MTM.Core.Models
{
    /// <summary>
    /// Base entity class that provides common properties for all entities.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the date and time when the entity was created (UTC).
        /// </summary>
        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated (UTC).
        /// </summary>
        public virtual DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a value indicating whether the entity is soft deleted.
        /// </summary>
        public virtual bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Updates the UpdatedAt timestamp to the current UTC time.
        /// </summary>
        public virtual void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the entity as deleted (soft delete).
        /// </summary>
        public virtual void MarkAsDeleted()
        {
            IsDeleted = true;
            Touch();
        }

        /// <summary>
        /// Restores a soft-deleted entity.
        /// </summary>
        public virtual void Restore()
        {
            IsDeleted = false;
            Touch();
        }
    }

    /// <summary>
    /// Auditable entity that extends BaseEntity with auditing information.
    /// </summary>
    public abstract class AuditableEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the user who created the entity.
        /// </summary>
        public virtual string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last updated the entity.
        /// </summary>
        public virtual string? UpdatedBy { get; set; }

        /// <summary>
        /// Updates the entity with auditing information.
        /// </summary>
        /// <param name="userId">The ID of the user making the update</param>
        public virtual void UpdateAudit(string userId)
        {
            UpdatedBy = userId;
            Touch();
        }

        /// <summary>
        /// Sets the creation audit information.
        /// </summary>
        /// <param name="userId">The ID of the user creating the entity</param>
        public virtual void SetCreationAudit(string userId)
        {
            CreatedBy = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's role.
        /// </summary>
        [StringLength(50)]
        public string Role { get; set; } = "User";

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time of the last login (UTC).
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// Records a login for the user.
        /// </summary>
        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            Touch();
        }

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            Touch();
        }

        /// <summary>
        /// Activates the user.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            Touch();
        }

        public override string ToString()
        {
            return $"{Username} ({FullName})";
        }
    }

    /// <summary>
    /// Base class for value objects that represent a concept by its attributes.
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        /// <summary>
        /// Gets the atomic values that make up the value object.
        /// </summary>
        /// <returns>The atomic values</returns>
        protected abstract IEnumerable<object> GetAtomicValues();

        /// <summary>
        /// Determines whether the specified value object is equal to the current value object.
        /// </summary>
        public bool Equals(ValueObject? other)
        {
            if (other is null || other.GetType() != GetType())
                return false;

            return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current value object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return Equals(obj as ValueObject);
        }

        /// <summary>
        /// Returns a hash code for the current value object.
        /// </summary>
        public override int GetHashCode()
        {
            return GetAtomicValues()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// Determines whether two value objects are equal.
        /// </summary>
        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two value objects are not equal.
        /// </summary>
        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Represents money as a value object.
    /// </summary>
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "USD")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }

        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            return new Money(left.Amount - right.Amount, left.Currency);
        }

        public override string ToString()
        {
            return $"{Amount:C} {Currency}";
        }
    }

    /// <summary>
    /// TODO: Implement additional core models for your domain.
    /// Examples of models you might need:
    /// 
    /// - InventoryItem: Represents items in inventory
    /// - InventoryTransaction: Represents inventory movements
    /// - QuickButtonItem: Represents quick action buttons
    /// - Location: Represents physical locations
    /// - Operation: Represents business operations
    /// - PartNumber: Represents part identifiers
    /// - ItemType: Represents categories of items
    /// 
    /// Use custom prompts 22 and following to implement these models
    /// based on your specific business requirements.
    /// </summary>
}