using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTM_WIP_Application_Avalonia.Models.UI
{
    /// <summary>
    /// Represents a saved column configuration for CustomDataGrid
    /// Provides column layout persistence with metadata and validation
    /// Follows MTM architecture patterns with proper serialization support
    /// </summary>
    public class ColumnConfiguration : INotifyPropertyChanged
    {
        #region Private Fields

        private string _configurationId = string.Empty;
        private string _displayName = string.Empty;
        private string _description = string.Empty;
        private bool _isDefault = false;
        private string _createdBy = string.Empty;
        private DateTime _createdDate = DateTime.Now;
        private DateTime _lastModified = DateTime.Now;
        private List<ColumnSetting> _columnSettings = new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the unique identifier for this configuration
        /// </summary>
        public string ConfigurationId
        {
            get => _configurationId;
            set => SetProperty(ref _configurationId, value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the display name for this configuration
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the description of this configuration
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets whether this is the default configuration
        /// </summary>
        public bool IsDefault
        {
            get => _isDefault;
            set => SetProperty(ref _isDefault, value);
        }

        /// <summary>
        /// Gets or sets the user who created this configuration
        /// </summary>
        public string CreatedBy
        {
            get => _createdBy;
            set => SetProperty(ref _createdBy, value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets when this configuration was created
        /// </summary>
        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        /// <summary>
        /// Gets or sets when this configuration was last modified
        /// </summary>
        public DateTime LastModified
        {
            get => _lastModified;
            set => SetProperty(ref _lastModified, value);
        }

        /// <summary>
        /// Gets or sets the column settings for this configuration
        /// </summary>
        public List<ColumnSetting> ColumnSettings
        {
            get => _columnSettings;
            set => SetProperty(ref _columnSettings, value ?? new List<ColumnSetting>());
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of ColumnConfiguration
        /// </summary>
        public ColumnConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance with basic information
        /// </summary>
        /// <param name="configurationId">The unique identifier</param>
        /// <param name="displayName">The display name</param>
        public ColumnConfiguration(string configurationId, string displayName)
        {
            ConfigurationId = configurationId;
            DisplayName = displayName;
            CreatedBy = Environment.UserName;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance with full configuration
        /// </summary>
        /// <param name="configurationId">The unique identifier</param>
        /// <param name="displayName">The display name</param>
        /// <param name="columnSettings">The column settings</param>
        public ColumnConfiguration(string configurationId, string displayName, List<ColumnSetting> columnSettings)
            : this(configurationId, displayName)
        {
            ColumnSettings = columnSettings ?? new List<ColumnSetting>();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns>True if the configuration is valid, false otherwise</returns>
        public bool IsValid()
        {
            // Configuration ID is required
            if (string.IsNullOrWhiteSpace(ConfigurationId))
                return false;

            // Display name is required
            if (string.IsNullOrWhiteSpace(DisplayName))
                return false;

            // Must have at least one column setting
            if (ColumnSettings == null || ColumnSettings.Count == 0)
                return false;

            // Validate each column setting
            if (ColumnSettings.Any(cs => !cs.IsValid()))
                return false;

            // Check for duplicate column orders
            var orders = ColumnSettings.Where(cs => cs.DisplayOrder >= 0)
                                     .Select(cs => cs.DisplayOrder)
                                     .ToList();

            if (orders.Count != orders.Distinct().Count())
                return false;

            return true;
        }

        /// <summary>
        /// Gets validation errors for the current configuration
        /// </summary>
        /// <returns>Array of validation error messages</returns>
        public string[] GetValidationErrors()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(ConfigurationId))
                errors.Add("ConfigurationId is required");

            if (string.IsNullOrWhiteSpace(DisplayName))
                errors.Add("DisplayName is required");

            if (ColumnSettings == null || ColumnSettings.Count == 0)
                errors.Add("At least one column setting is required");

            if (ColumnSettings != null)
            {
                for (int i = 0; i < ColumnSettings.Count; i++)
                {
                    var setting = ColumnSettings[i];
                    if (!setting.IsValid())
                    {
                        var settingErrors = setting.GetValidationErrors();
                        foreach (var error in settingErrors)
                        {
                            errors.Add($"Column {i + 1}: {error}");
                        }
                    }
                }

                // Check for duplicate orders
                var orders = ColumnSettings.Where(cs => cs.DisplayOrder >= 0)
                                          .GroupBy(cs => cs.DisplayOrder)
                                          .Where(g => g.Count() > 1)
                                          .Select(g => g.Key);

                foreach (var order in orders)
                {
                    errors.Add($"Duplicate display order: {order}");
                }
            }

            return errors.ToArray();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a deep copy of this configuration
        /// </summary>
        /// <returns>New ColumnConfiguration with copied values</returns>
        public ColumnConfiguration Clone()
        {
            return new ColumnConfiguration
            {
                ConfigurationId = ConfigurationId,
                DisplayName = DisplayName,
                Description = Description,
                IsDefault = IsDefault,
                CreatedBy = CreatedBy,
                CreatedDate = CreatedDate,
                LastModified = LastModified,
                ColumnSettings = ColumnSettings.Select(cs => cs.Clone()).ToList()
            };
        }

        /// <summary>
        /// Updates the last modified timestamp
        /// </summary>
        public void Touch()
        {
            LastModified = DateTime.Now;
        }

        /// <summary>
        /// Gets the column setting for a specific property name
        /// </summary>
        /// <param name="propertyName">The property name to find</param>
        /// <returns>The column setting or null if not found</returns>
        public ColumnSetting? GetColumnSetting(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            return ColumnSettings.FirstOrDefault(cs =>
                string.Equals(cs.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Adds or updates a column setting
        /// </summary>
        /// <param name="setting">The column setting to add or update</param>
        public void SetColumnSetting(ColumnSetting setting)
        {
            ArgumentNullException.ThrowIfNull(setting);

            var existing = GetColumnSetting(setting.PropertyName);
            if (existing != null)
            {
                // Update existing
                var index = ColumnSettings.IndexOf(existing);
                ColumnSettings[index] = setting;
            }
            else
            {
                // Add new
                ColumnSettings.Add(setting);
            }

            Touch();
        }

        /// <summary>
        /// Removes a column setting by property name
        /// </summary>
        /// <param name="propertyName">The property name to remove</param>
        /// <returns>True if removed, false if not found</returns>
        public bool RemoveColumnSetting(string propertyName)
        {
            var setting = GetColumnSetting(propertyName);
            if (setting != null)
            {
                ColumnSettings.Remove(setting);
                Touch();
                return true;
            }
            return false;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this configuration
        /// </summary>
        public override string ToString()
        {
            return $"{DisplayName} ({ConfigurationId}) - {ColumnSettings.Count} columns";
        }

        /// <summary>
        /// Determines equality based on ConfigurationId
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is ColumnConfiguration other)
            {
                return string.Equals(ConfigurationId, other.ConfigurationId, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Gets hash code based on ConfigurationId
        /// </summary>
        public override int GetHashCode()
        {
            return ConfigurationId?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
        }

        #endregion
    }

    /// <summary>
    /// Represents a single column setting within a configuration
    /// Stores column display properties, ordering, and visibility
    /// </summary>
    public class ColumnSetting : INotifyPropertyChanged
    {
        #region Private Fields

        private string _propertyName = string.Empty;
        private string _displayName = string.Empty;
        private double _width = double.NaN;
        private bool _isVisible = true;
        private int _displayOrder = -1;
        private string? _stringFormat;
        private ColumnAlignment _alignment = ColumnAlignment.Left;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the property name for data binding
        /// </summary>
        public string PropertyName
        {
            get => _propertyName;
            set => SetProperty(ref _propertyName, value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the display name for the column header
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the column width
        /// </summary>
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        /// <summary>
        /// Gets or sets whether the column is visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        /// <summary>
        /// Gets or sets the display order (-1 for default ordering)
        /// </summary>
        public int DisplayOrder
        {
            get => _displayOrder;
            set => SetProperty(ref _displayOrder, value);
        }

        /// <summary>
        /// Gets or sets the string format for displaying values
        /// </summary>
        public string? StringFormat
        {
            get => _stringFormat;
            set => SetProperty(ref _stringFormat, value);
        }

        /// <summary>
        /// Gets or sets the column content alignment
        /// </summary>
        public ColumnAlignment Alignment
        {
            get => _alignment;
            set => SetProperty(ref _alignment, value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of ColumnSetting
        /// </summary>
        public ColumnSetting()
        {
        }

        /// <summary>
        /// Initializes a new instance with basic information
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="displayName">The display name</param>
        public ColumnSetting(string propertyName, string displayName)
        {
            PropertyName = propertyName;
            DisplayName = displayName;
        }

        /// <summary>
        /// Initializes a new instance with full configuration
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="displayName">The display name</param>
        /// <param name="width">The column width</param>
        /// <param name="isVisible">Whether the column is visible</param>
        /// <param name="displayOrder">The display order</param>
        public ColumnSetting(string propertyName, string displayName, double width, bool isVisible, int displayOrder)
            : this(propertyName, displayName)
        {
            Width = width;
            IsVisible = isVisible;
            DisplayOrder = displayOrder;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates the column setting
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            // Property name is required
            if (string.IsNullOrWhiteSpace(PropertyName))
                return false;

            // Display name should be provided
            if (string.IsNullOrWhiteSpace(DisplayName))
                return false;

            // Width validation (if specified)
            if (!double.IsNaN(Width) && Width <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Gets validation errors for the current setting
        /// </summary>
        /// <returns>Array of validation error messages</returns>
        public string[] GetValidationErrors()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(PropertyName))
                errors.Add("PropertyName is required");

            if (string.IsNullOrWhiteSpace(DisplayName))
                errors.Add("DisplayName is required");

            if (!double.IsNaN(Width) && Width <= 0)
                errors.Add("Width must be positive or NaN for auto-sizing");

            return errors.ToArray();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a deep copy of this setting
        /// </summary>
        /// <returns>New ColumnSetting with copied values</returns>
        public ColumnSetting Clone()
        {
            return new ColumnSetting
            {
                PropertyName = PropertyName,
                DisplayName = DisplayName,
                Width = Width,
                IsVisible = IsVisible,
                DisplayOrder = DisplayOrder,
                StringFormat = StringFormat,
                Alignment = Alignment
            };
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this setting
        /// </summary>
        public override string ToString()
        {
            return $"{DisplayName} ({PropertyName}) - Width: {Width}, Visible: {IsVisible}, Order: {DisplayOrder}";
        }

        /// <summary>
        /// Determines equality based on PropertyName
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is ColumnSetting other)
            {
                return string.Equals(PropertyName, other.PropertyName, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Gets hash code based on PropertyName
        /// </summary>
        public override int GetHashCode()
        {
            return PropertyName?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
        }

        #endregion
    }

    /// <summary>
    /// Enumeration of column alignment options (shared with CustomDataGridColumn)
    /// </summary>
    public enum ColumnAlignment
    {
        Left,
        Center,
        Right,
        Stretch
    }
}
