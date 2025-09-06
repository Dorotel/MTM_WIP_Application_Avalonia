using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace MTM_WIP_Application_Avalonia.Behaviors
{
	/// <summary>
	/// Avalonia attached behavior that enhances AutoCompleteBox controls with keyboard navigation and dropdown management
	/// for improved user experience in MTM manufacturing data entry scenarios.
	/// 
	/// This behavior provides essential keyboard navigation functionality for manufacturing data input,
	/// particularly useful for part IDs, operation numbers, and location selection where users need
	/// efficient keyboard-driven workflows for rapid data entry in manufacturing environments.
	/// 
	/// Key functionality provided:
	/// - Keyboard navigation (Up/Down arrows) through suggestion lists
	/// - Automatic dropdown opening on user interaction (Down arrow, mouse click)
	/// - Selection commitment via Enter/Tab keys for efficient data entry
	/// - Escape key handling for dropdown dismissal
	/// - Integration with manufacturing suggestion systems for parts, operations, and locations
	/// 
	/// Manufacturing use cases:
	/// - Part ID selection with fuzzy matching and keyboard navigation
	/// - Operation number selection (90, 100, 110, etc.) for workflow routing
	/// - Location selection for inventory management and tracking
	/// - User-friendly data entry in fast-paced manufacturing environments
	/// 
	/// Debugging and diagnostics:
	/// - Comprehensive debug output for troubleshooting manufacturing data entry issues
	/// - Performance monitoring for keyboard interaction responsiveness
	/// - Event logging for manufacturing operation audit trails
	/// </summary>
	public static class ComboBoxBehavior
	{
		/// <summary>
		/// Attached property that enables the ComboBox behavior on AutoCompleteBox controls.
		/// When set to true, the behavior is activated and provides enhanced keyboard navigation
		/// and dropdown management functionality for manufacturing data entry scenarios.
		/// </summary>
		public static readonly AttachedProperty<bool> EnableComboBoxBehaviorProperty =
			AvaloniaProperty.RegisterAttached<AutoCompleteBox, bool>(
				"EnableComboBoxBehavior",
				typeof(ComboBoxBehavior),
				false);

		/// <summary>
		/// Static constructor that initializes the behavior system by registering for property change events.
		/// This ensures that the behavior is automatically applied when the EnableComboBoxBehavior property
		/// is set to true on any AutoCompleteBox control in the manufacturing application.
		/// </summary>
		static ComboBoxBehavior()
		{
			EnableComboBoxBehaviorProperty.Changed.AddClassHandler<AutoCompleteBox>(OnEnableComboBoxBehaviorChanged);
		}

		/// <summary>
		/// Gets the current value of the EnableComboBoxBehavior attached property for the specified AutoCompleteBox.
		/// Used by the Avalonia property system and XAML binding for manufacturing form controls.
		/// </summary>
		/// <param name="element">The AutoCompleteBox element to query</param>
		/// <returns>True if the behavior is enabled, false otherwise</returns>
		public static bool GetEnableComboBoxBehavior(AutoCompleteBox element) =>
			element.GetValue(EnableComboBoxBehaviorProperty);

		/// <summary>
		/// Sets the value of the EnableComboBoxBehavior attached property for the specified AutoCompleteBox.
		/// When set to true, enables enhanced keyboard navigation and dropdown management for the control.
		/// Typically called from XAML in manufacturing form definitions.
		/// </summary>
		/// <param name="element">The AutoCompleteBox element to configure</param>
		/// <param name="value">True to enable the behavior, false to disable</param>
		public static void SetEnableComboBoxBehavior(AutoCompleteBox element, bool value) =>
			element.SetValue(EnableComboBoxBehaviorProperty, value);

		/// <summary>
		/// Event handler called when the EnableComboBoxBehavior property changes on an AutoCompleteBox.
		/// Attaches or detaches the keyboard and mouse event handlers based on the property value.
		/// Includes diagnostic logging for troubleshooting manufacturing UI behavior issues.
		/// </summary>
		/// <param name="sender">The AutoCompleteBox whose behavior property changed</param>
		/// <param name="args">Event arguments containing the old and new property values</param>
		private static void OnEnableComboBoxBehaviorChanged(AutoCompleteBox sender, AvaloniaPropertyChangedEventArgs args)
		{
			if (args is AvaloniaPropertyChangedEventArgs<bool> e && sender is AutoCompleteBox box)
			{
				if (e.NewValue.Value)
				{
					Debug.WriteLine($"[ComboBoxBehavior] ENABLED for {box.Name ?? box.ToString()} at {DateTime.Now:HH:mm:ss.fff}");
					box.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
					box.PointerPressed += OnPointerPressed;
				}
				else
				{
					Debug.WriteLine($"[ComboBoxBehavior] DISABLED for {box.Name ?? box.ToString()} at {DateTime.Now:HH:mm:ss.fff}");
					box.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
					box.PointerPressed -= OnPointerPressed;
				}
			}
		}

		/// <summary>
		/// Handles keyboard input events for enhanced navigation in manufacturing data entry scenarios.
		/// Provides keyboard shortcuts for efficient data selection in manufacturing workflows:
		/// - Down Arrow: Opens dropdown or moves selection down through manufacturing options
		/// - Up Arrow: Moves selection up through available choices  
		/// - Enter/Tab: Commits current selection for rapid manufacturing data entry
		/// - Escape: Cancels dropdown interaction and returns focus to manufacturing form
		/// 
		/// Essential for efficient manufacturing operations where rapid keyboard data entry
		/// is required for part IDs, operation numbers, and location selections.
		/// </summary>
		/// <param name="sender">The AutoCompleteBox that received the keyboard input</param>
		/// <param name="e">Keyboard event arguments containing key information and handling state</param>
		private static void OnKeyDown(object? sender, KeyEventArgs e)
		{
			if (sender is not AutoCompleteBox box)
				return;

			// Open dropdown on Down arrow
			Debug.WriteLine($"[ComboBoxBehavior] KeyDown: {e.Key} | Text='{box.Text}' | SelectedItem='{box.SelectedItem}' | DropDownOpen={box.IsDropDownOpen} at {DateTime.Now:HH:mm:ss.fff}");

			if (e.Key == Key.Down)
			{
				if (!box.IsDropDownOpen)
				{
					Debug.WriteLine($"[ComboBoxBehavior] DropDown opened by Down key at {DateTime.Now:HH:mm:ss.fff}");
					box.IsDropDownOpen = true;
					e.Handled = true;
				}
				else
				{
					var items = box.ItemsSource as System.Collections.IList;
					if (items != null && items.Count > 0)
					{
						int currentIndex = -1;
						if (box.SelectedItem != null)
							currentIndex = IndexOf(items, box.SelectedItem);
						if (currentIndex < items.Count - 1)
						{
							Debug.WriteLine($"[ComboBoxBehavior] MoveSelection DOWN from {currentIndex} to {currentIndex + 1} at {DateTime.Now:HH:mm:ss.fff}");
							MoveSelection(box, 1);
							e.Handled = true;
						}
						else
						{
							Debug.WriteLine($"[ComboBoxBehavior] At end of list, cannot move DOWN at {DateTime.Now:HH:mm:ss.fff}");
							e.Handled = true;
						}
					}
					else
					{
						Debug.WriteLine($"[ComboBoxBehavior] No items to move DOWN at {DateTime.Now:HH:mm:ss.fff}");
						e.Handled = true;
					}
				}
			}
			// Up arrow
			else if (e.Key == Key.Up)
			{
				if (box.IsDropDownOpen)
				{
					var items = box.ItemsSource as System.Collections.IList;
					if (items != null && items.Count > 0)
					{
						int currentIndex = -1;
						if (box.SelectedItem != null)
							currentIndex = IndexOf(items, box.SelectedItem);
						if (currentIndex > 0)
						{
							Debug.WriteLine($"[ComboBoxBehavior] MoveSelection UP from {currentIndex} to {currentIndex - 1} at {DateTime.Now:HH:mm:ss.fff}");
							MoveSelection(box, -1);
							e.Handled = true;
						}
						else
						{
							Debug.WriteLine($"[ComboBoxBehavior] At start of list, cannot move UP at {DateTime.Now:HH:mm:ss.fff}");
							e.Handled = true;
						}
					}
					else
					{
						Debug.WriteLine($"[ComboBoxBehavior] No items to move UP at {DateTime.Now:HH:mm:ss.fff}");
						e.Handled = true;
					}
				}
			}
			// Enter or Tab commits selection
			else if (e.Key == Key.Enter || e.Key == Key.Tab)
			{
				if (box.IsDropDownOpen)
				{
					Debug.WriteLine($"[ComboBoxBehavior] CommitSelection by {e.Key} at {DateTime.Now:HH:mm:ss.fff}");
					CommitSelection(box);
					box.IsDropDownOpen = false;
					e.Handled = true;
				}
			}
			// Escape closes dropdown
			else if (e.Key == Key.Escape)
			{
				if (box.IsDropDownOpen)
				{
					Debug.WriteLine($"[ComboBoxBehavior] DropDown closed by Escape at {DateTime.Now:HH:mm:ss.fff}");
					box.IsDropDownOpen = false;
					e.Handled = true;
				}
			}
		}

		/// <summary>
		/// Handles mouse/pointer input events to provide intuitive dropdown interaction for manufacturing users.
		/// Automatically opens the dropdown when users click on the AutoCompleteBox, providing consistent
		/// behavior between keyboard and mouse interaction in manufacturing data entry scenarios.
		/// Includes comprehensive diagnostic logging for troubleshooting UI interaction issues.
		/// </summary>
		/// <param name="sender">The AutoCompleteBox that received the pointer input</param>
		/// <param name="e">Pointer event arguments containing click information and position data</param>
		private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			if (sender is AutoCompleteBox box)
			{
				var pt = e.GetCurrentPoint(box);
				var pointerType = pt.Pointer.Type;
				var updateKind = pt.Properties.PointerUpdateKind;
				var pos = pt.Position;
				Debug.WriteLine($"[ComboBoxBehavior] PointerPressed: Type={pointerType}, UpdateKind={updateKind}, Position=({pos.X},{pos.Y}) | DropDownOpen={box.IsDropDownOpen} at {DateTime.Now:HH:mm:ss.fff}");
				// Open dropdown on mouse click if not already open
				if (!box.IsDropDownOpen)
				{
					Debug.WriteLine($"[ComboBoxBehavior] DropDown opened by mouse at {DateTime.Now:HH:mm:ss.fff}");
					box.IsDropDownOpen = true;
				}
			}
		}

		/// <summary>
		/// Moves the selection in the dropdown by the specified direction (positive for down, negative for up).
		/// Updates the SelectedItem property but does not update the Text property until selection is committed.
		/// This allows users to navigate through manufacturing options (parts, operations, locations)
		/// without immediately committing to a selection, providing better user experience in data entry.
		/// </summary>
		/// <param name="box">The AutoCompleteBox to update selection for</param>
		/// <param name="direction">Direction to move selection: positive for down, negative for up</param>
		private static void MoveSelection(AutoCompleteBox box, int direction)
		{
			// Use public API: SelectedItem and ItemsSource
			var items = box.ItemsSource as System.Collections.IList;
			if (items == null || items.Count == 0)
				return;

			int currentIndex = -1;
			if (box.SelectedItem != null)
				currentIndex = IndexOf(items, box.SelectedItem);

			int newIndex = currentIndex + direction;
			if (newIndex < 0) newIndex = 0;
			if (newIndex >= items.Count) newIndex = items.Count - 1;

			box.SelectedItem = items[newIndex];
			Debug.WriteLine($"[ComboBoxBehavior] MoveSelection: SelectedItem now '{box.SelectedItem}' at {DateTime.Now:HH:mm:ss.fff}");
			// Do NOT update Text here; only update on CommitSelection
		}

		/// <summary>
		/// Helper method to find the index of a value in an IList collection.
		/// Used for determining current selection position when navigating through
		/// manufacturing data options (part IDs, operation numbers, location codes).
		/// Handles null values safely for robust manufacturing UI behavior.
		/// </summary>
		/// <param name="items">The list of items to search through</param>
		/// <param name="value">The value to find the index of</param>
		/// <returns>Index of the value, or -1 if not found</returns>
		private static int IndexOf(System.Collections.IList items, object? value)
		{
			if (value == null) return -1;
			for (int i = 0; i < items.Count; i++)
			{
				if (Equals(items[i], value))
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Commits the current selection by updating the Text property of the AutoCompleteBox.
		/// This is the final step in the selection process, typically triggered by Enter or Tab keys.
		/// Essential for manufacturing data entry workflows where users need to confirm their
		/// selection of parts, operations, or locations before proceeding with inventory transactions.
		/// </summary>
		/// <param name="box">The AutoCompleteBox to commit selection for</param>
		private static void CommitSelection(AutoCompleteBox box)
		{
			// Commit the current selection to Text
			if (box.SelectedItem != null)
			{
				Debug.WriteLine($"[ComboBoxBehavior] CommitSelection: Text set to '{box.SelectedItem}' at {DateTime.Now:HH:mm:ss.fff}");
				box.Text = box.SelectedItem.ToString() ?? string.Empty;
			}
		}
	}
}
