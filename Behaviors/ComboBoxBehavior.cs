using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace MTM_WIP_Application_Avalonia.Behaviors
{
	public static class ComboBoxBehavior
	{
		public static readonly AttachedProperty<bool> EnableComboBoxBehaviorProperty =
			AvaloniaProperty.RegisterAttached<AutoCompleteBox, bool>(
				"EnableComboBoxBehavior",
				typeof(ComboBoxBehavior),
				false);

		static ComboBoxBehavior()
		{
			EnableComboBoxBehaviorProperty.Changed.Subscribe(OnEnableComboBoxBehaviorChanged);
		}

		public static bool GetEnableComboBoxBehavior(AutoCompleteBox element) =>
			element.GetValue(EnableComboBoxBehaviorProperty);

		public static void SetEnableComboBoxBehavior(AutoCompleteBox element, bool value) =>
			element.SetValue(EnableComboBoxBehaviorProperty, value);

		private static void OnEnableComboBoxBehaviorChanged(AvaloniaPropertyChangedEventArgs<bool> e)
		{
			if (e.Sender is AutoCompleteBox box)
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
