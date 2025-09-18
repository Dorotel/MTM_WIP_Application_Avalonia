using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using Material.Icons.Avalonia;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Column Management Panel for CustomDataGrid
    /// Phase 3a implementation: Column Visibility Toggle
    /// </summary>
    public partial class ColumnManagementPanel : UserControl
    {
        #region Private Fields
        
        private readonly ILogger<ColumnManagementPanel> _logger;
        private StackPanel? _columnListContainer;
        private readonly List<ColumnToggleItem> _columnToggleItems = new();
        
        /// <summary>
        /// Collection of column items managed by this panel
        /// </summary>
        public ObservableCollection<ColumnItem> ColumnItems { get; } = new();
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Raised when a column visibility is toggled
        /// </summary>
        public event EventHandler<ColumnVisibilityChangedEventArgs>? ColumnVisibilityChanged;
        
        /// <summary>
        /// Raised when the panel should be closed
        /// </summary>
        public event EventHandler? CloseRequested;
        
        /// <summary>
        /// Raised when all columns should be shown
        /// </summary>
        public event EventHandler? ShowAllRequested;
        
        /// <summary>
        /// Raised when all non-essential columns should be hidden
        /// </summary>
        public event EventHandler? HideAllRequested;
        
        /// <summary>
        /// Raised when layout should be reset to default
        /// </summary>
        public event EventHandler? ResetLayoutRequested;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of ColumnManagementPanel
        /// </summary>
        public ColumnManagementPanel()
        {
            // Create logger manually since this is a UserControl
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ColumnManagementPanel>();
            
            InitializeComponent();
            InitializeControls();
        }
        
        #endregion
        
        #region Initialization
        
        /// <summary>
        /// Initialize controls after AXAML is loaded
        /// </summary>
        private void InitializeControls()
        {
            try
            {
                _columnListContainer = this.FindControl<StackPanel>("ColumnListContainer");
                
                if (_columnListContainer == null)
                {
                    _logger.LogError("Failed to find ColumnListContainer in ColumnManagementPanel");
                }
                else
                {
                    _logger.LogDebug("ColumnManagementPanel initialized successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing ColumnManagementPanel controls");
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Updates the panel with the current column configuration
        /// </summary>
        /// <param name="columns">The current columns to display</param>
        public void UpdateColumns(IEnumerable<ColumnItem> columns)
        {
            try
            {
                _logger.LogDebug("Updating column management panel with {Count} columns", columns.Count());
                
                _columnToggleItems.Clear();
                _columnListContainer?.Children.Clear();
                
                foreach (var column in columns.OrderBy(c => c.Order))
                {
                    var toggleItem = CreateColumnToggleItem(column);
                    _columnToggleItems.Add(toggleItem);
                    _columnListContainer?.Children.Add(toggleItem.Control);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating columns in ColumnManagementPanel");
            }
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Creates a toggle item for a column
        /// </summary>
        private ColumnToggleItem CreateColumnToggleItem(ColumnItem column)
        {
            var border = new Border
            {
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Avalonia.Thickness(1),
                CornerRadius = new Avalonia.CornerRadius(4),
                Padding = new Avalonia.Thickness(8),
                Margin = new Avalonia.Thickness(0, 2)
            };
            
            // Apply hover and active states
            border.Classes.Add("column-toggle-item");
            
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto")
            };
            
            // Visibility checkbox
            var visibilityCheckBox = new CheckBox
            {
                IsChecked = column.IsVisible,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                IsEnabled = !column.IsRequired // Essential columns cannot be hidden
            };
            
            visibilityCheckBox.Click += (s, e) => OnColumnVisibilityToggled(column, visibilityCheckBox.IsChecked ?? false);
            
            Grid.SetColumn(visibilityCheckBox, 0);
            grid.Children.Add(visibilityCheckBox);
            
            // Column name and info
            var namePanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Vertical,
                Margin = new Avalonia.Thickness(8, 0, 0, 0),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            
            var nameText = new TextBlock
            {
                Text = column.DisplayName,
                FontWeight = FontWeight.Medium,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Black)
            };
            
            namePanel.Children.Add(nameText);
            
            if (column.IsRequired)
            {
                var requiredText = new TextBlock
                {
                    Text = "(Required)",
                    FontSize = 10,
                    FontStyle = FontStyle.Italic,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    Margin = new Avalonia.Thickness(0, 2, 0, 0)
                };
                namePanel.Children.Add(requiredText);
            }
            
            Grid.SetColumn(namePanel, 1);
            grid.Children.Add(namePanel);
            
            // Type indicator icon
            var typeIcon = GetTypeIcon(column.DataType);
            if (typeIcon != null)
            {
                Grid.SetColumn(typeIcon, 2);
                grid.Children.Add(typeIcon);
            }
            
            border.Child = grid;
            
            return new ColumnToggleItem
            {
                Column = column,
                Control = border,
                CheckBox = visibilityCheckBox
            };
        }
        
        /// <summary>
        /// Gets an appropriate type icon for the column data type
        /// </summary>
        private MaterialIcon? GetTypeIcon(string dataType)
        {
            var iconKind = dataType.ToLowerInvariant() switch
            {
                var t when t.Contains("string") => MaterialIconKind.FormatText,
                var t when t.Contains("int") || t.Contains("decimal") || t.Contains("double") => MaterialIconKind.Numeric,
                var t when t.Contains("datetime") || t.Contains("date") => MaterialIconKind.Calendar,
                var t when t.Contains("bool") => MaterialIconKind.CheckboxOutline,
                _ => MaterialIconKind.HelpCircleOutline
            };
            
            return new MaterialIcon
            {
                Kind = iconKind,
                Width = 16,
                Height = 16,
                Foreground = new SolidColorBrush(Colors.Gray),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }
        
        #endregion
        
        #region Event Handlers
        
        /// <summary>
        /// Handles column visibility toggle
        /// </summary>
        private void OnColumnVisibilityToggled(ColumnItem column, bool isVisible)
        {
            try
            {
                _logger.LogDebug("Column {ColumnName} visibility toggled to {IsVisible}", column.DisplayName, isVisible);
                
                var args = new ColumnVisibilityChangedEventArgs
                {
                    PropertyName = column.PropertyName,
                    DisplayName = column.DisplayName,
                    IsVisible = isVisible
                };
                
                ColumnVisibilityChanged?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling column visibility toggle for {ColumnName}", column.DisplayName);
            }
        }
        
        /// <summary>
        /// Handles close button click
        /// </summary>
        private void OnClosePanel(object? sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Column management panel close requested");
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Handles show all columns request
        /// </summary>
        private void OnShowAllColumns(object? sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Show all columns requested");
            
            // Update all checkboxes
            foreach (var item in _columnToggleItems)
            {
                if (item.CheckBox.IsEnabled) // Only if column can be toggled
                {
                    item.CheckBox.IsChecked = true;
                }
            }
            
            ShowAllRequested?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Handles hide all columns request
        /// </summary>
        private void OnHideAllColumns(object? sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Hide all non-essential columns requested");
            
            // Update checkboxes for non-essential columns only
            foreach (var item in _columnToggleItems)
            {
                if (item.CheckBox.IsEnabled && !item.Column.IsRequired) // Only non-essential columns
                {
                    item.CheckBox.IsChecked = false;
                }
            }
            
            HideAllRequested?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Handles reset layout request
        /// </summary>
        private void OnResetLayout(object? sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Reset layout requested");
            ResetLayoutRequested?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Updates the UI with the current column items
        /// </summary>
        private void UpdateColumnItemsUI()
        {
            try
            {
                // Clear existing UI elements
                _columnToggleItems.Clear();
                _columnListContainer?.Children.Clear();
                
                // Recreate UI elements for each column item
                foreach (var columnItem in ColumnItems)
                {
                    CreateColumnToggleItem(columnItem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating column items UI");
            }
        }
        
        /// <summary>
        /// Sets the column items for the management panel
        /// </summary>
        /// <param name="columnItems">The column items to display</param>
        public void SetColumnItems(IEnumerable<ColumnItem> columnItems)
        {
            try
            {
                // Clear existing items
                ColumnItems.Clear();
                
                // Add new items
                foreach (var item in columnItems.OrderBy(x => x.Order))
                {
                    ColumnItems.Add(item);
                }
                
                // Update the UI
                UpdateColumnItemsUI();
            }
            catch (Exception ex)
            {
                // Handle error appropriately
                System.Diagnostics.Debug.WriteLine($"Error setting column items: {ex.Message}");
            }
        }
        
        #endregion
    }
    
    #region Helper Classes
    
    /// <summary>
    /// Represents a column item in the management panel
    /// </summary>
    public class ColumnItem
    {
        public string PropertyName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;
        public bool IsRequired { get; set; } = false;
        public string DataType { get; set; } = "string";
        public int Order { get; set; } = 0;
        
        // Additional properties needed for Phase 3a
        public string Id
        {
            get => PropertyName;
            set => PropertyName = value;
        }
        
        public bool CanHide
        {
            get => !IsRequired;
            set => IsRequired = !value;
        }
        
        public double Width { get; set; } = 100;
    }
    
    /// <summary>
    /// Internal class for managing column toggle UI items
    /// </summary>
    public class ColumnToggleItem
    {
        public ColumnItem Column { get; set; } = new();
        public Control Control { get; set; } = new();
        public CheckBox CheckBox { get; set; } = new();
    }
    
    /// <summary>
    /// Event arguments for column visibility changes
    /// </summary>
    public class ColumnVisibilityChangedEventArgs : EventArgs
    {
        public string PropertyName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = false;
        
        // Alias for compatibility
        public string ColumnId
        {
            get => PropertyName;
            set => PropertyName = value;
        }
    }
    
    #endregion
}