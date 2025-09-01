using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls.Templates;
using System.Linq;

namespace MTM_WIP_Application_Avalonia.Behaviors
{
    public static class TextBoxFuzzyValidationBehavior
    {
    public static event Action<TextBox, List<object>>? SuggestionOverlayRequested;
        public static readonly AttachedProperty<IEnumerable> ValidationSourceProperty =
            AvaloniaProperty.RegisterAttached<TextBox, IEnumerable>(
                "ValidationSource",
                typeof(TextBoxFuzzyValidationBehavior));

        public static readonly AttachedProperty<bool> EnableFuzzyValidationProperty =
            AvaloniaProperty.RegisterAttached<TextBox, bool>(
                "EnableFuzzyValidation",
                typeof(TextBoxFuzzyValidationBehavior),
                false);

        static TextBoxFuzzyValidationBehavior()
        {
            EnableFuzzyValidationProperty.Changed.Subscribe(OnEnableFuzzyValidationChanged);
        }

        public static IEnumerable? GetValidationSource(TextBox element) =>
            element.GetValue(ValidationSourceProperty);
        public static void SetValidationSource(TextBox element, IEnumerable? value) =>
            element.SetValue((AvaloniaProperty)ValidationSourceProperty, value);

        public static bool GetEnableFuzzyValidation(TextBox element) =>
            element.GetValue(EnableFuzzyValidationProperty);
        public static void SetEnableFuzzyValidation(TextBox element, bool value) =>
            element.SetValue(EnableFuzzyValidationProperty, value);

        private static void OnEnableFuzzyValidationChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is TextBox box)
            {
                if (e.NewValue.Value)
                {
                    box.LostFocus += OnLostFocus;
                }
                else
                {
                    box.LostFocus -= OnLostFocus;
                }
            }
        }

        private static void OnLostFocus(object? sender, RoutedEventArgs e)
        {
            if (sender is not TextBox box)
                return;
            var source = GetValidationSource(box);
            if (source == null)
                return;
            var text = box.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
                return;
            // Check for exact match
            var exact = source.Cast<object>().FirstOrDefault(item => string.Equals(item?.ToString(), text, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
                return; // Exact match, do nothing
            // Fuzzy match: contains or startswith
            var like = source.Cast<object>()
                .Where(item => item != null && item.ToString() != null && item.ToString()!.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                .Take(20)
                .ToList();
            if (like.Count == 0)
                return;
            var handler = SuggestionOverlayRequested;
            if (handler != null)
                handler(box, like);
        }

    // REMOVED: ShowSuggestionFlyout. Overlay is now handled by parent.
    }
}
