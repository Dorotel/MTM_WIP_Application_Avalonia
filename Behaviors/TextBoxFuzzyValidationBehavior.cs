using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls.Templates;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

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
            EnableFuzzyValidationProperty.Changed.AddClassHandler<TextBox>(OnEnableFuzzyValidationChanged);
        }

        public static IEnumerable? GetValidationSource(TextBox element) =>
            element.GetValue(ValidationSourceProperty);
        public static void SetValidationSource(TextBox element, IEnumerable? value) =>
            element.SetValue((AvaloniaProperty)ValidationSourceProperty, value);

        public static bool GetEnableFuzzyValidation(TextBox element) =>
            element.GetValue(EnableFuzzyValidationProperty);
        public static void SetEnableFuzzyValidation(TextBox element, bool value) =>
            element.SetValue(EnableFuzzyValidationProperty, value);

        private static void OnEnableFuzzyValidationChanged(TextBox sender, AvaloniaPropertyChangedEventArgs args)
        {
            if (args is AvaloniaPropertyChangedEventArgs<bool> e && sender is TextBox box)
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
            
            System.Diagnostics.Debug.WriteLine($"TextBoxFuzzyValidationBehavior.OnLostFocus: Processing text '{text}'");
            
            // Check if validation source has any data
            var sourceItems = source.Cast<object>().ToList();
            
            if (sourceItems.Count == 0)
            {
                // No data available from server - clear textbox and show appropriate message
                System.Diagnostics.Debug.WriteLine($"TextBoxFuzzyValidationBehavior.OnLostFocus: No data available from server. Clearing textbox for '{text}'.");
                box.Text = string.Empty;
                
                // Clear error styling since this is a server issue, not user input error
                if (box.Classes.Contains("error"))
                {
                    box.Classes.Remove("error");
                    System.Diagnostics.Debug.WriteLine($"Removed 'error' class from textbox - clearing due to server connectivity issue");
                }
                
                // Show server connectivity error message
                try
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Services.ErrorHandling.HandleErrorAsync(
                                new InvalidOperationException("No data available due to server connectivity issues. Please check server connection."),
                                "Data unavailable - input cleared",
                                "System"
                            );
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("No data available due to server connectivity issues. Input cleared.");
                        }
                    });
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("No data available due to server connectivity issues. Input cleared.");
                }
                return;
            }
            
            // Check for exact match
            var exact = sourceItems.FirstOrDefault(item => string.Equals(item?.ToString(), text, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                System.Diagnostics.Debug.WriteLine($"TextBoxFuzzyValidationBehavior.OnLostFocus: Found exact match '{exact}', clearing any error messages");
                
                // Clear error state for valid input by removing error class
                if (box.Classes.Contains("error"))
                {
                    box.Classes.Remove("error");
                    System.Diagnostics.Debug.WriteLine($"Removed 'error' class from textbox for valid input: '{text}'");
                }
                
                // Force the binding to update by simulating a text change
                // This should trigger the ViewModel's property setters and validation logic
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Attempting to trigger binding update for '{text}'");
                    
                    // Force binding update using Avalonia's approach
                    // Step 1: Temporarily change the text to trigger property change
                    var originalText = box.Text;
                    box.Text = string.Empty;        // Clear first - this will make SelectedPart empty
                    
                    // Force the binding to update by raising the TextChanged event
                    box.RaiseEvent(new RoutedEventArgs(TextBox.TextChangedEvent));
                    
                    // Step 2: Set back to original text - this will make SelectedPart = originalText  
                    box.Text = originalText;
                    box.RaiseEvent(new RoutedEventArgs(TextBox.TextChangedEvent));
                    
                    // Additionally try to directly set bound property to ensure it's updated
                    var dataContext = box.DataContext;
                    if (dataContext != null)
                    {
                        // Try to identify which property this textbox is bound to and update it
                        var boundProperties = new[] { "SelectedPart", "SelectedOperation", "SelectedLocation" };
                        var watermarkProperties = new[] { "PartWatermark", "OperationWatermark", "LocationWatermark" };
                        
                        foreach (var boundProp in boundProperties)
                        {
                            var property = dataContext.GetType().GetProperty(boundProp);
                            if (property != null && property.CanWrite)
                            {
                                var currentValue = property.GetValue(dataContext)?.ToString();
                                if (currentValue == originalText) // This is likely the bound property
                                {
                                    property.SetValue(dataContext, originalText);
                                    System.Diagnostics.Debug.WriteLine($"Directly set {boundProp} property to '{originalText}'");
                                    break;
                                }
                            }
                        }
                        
                        // Trigger property change notifications for validation and watermarks
                        if (dataContext is System.ComponentModel.INotifyPropertyChanged)
                        {
                            var onPropertyChangedMethod = dataContext.GetType().GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                            if (onPropertyChangedMethod != null)
                            {
                                // Trigger all watermark properties to ensure they update
                                foreach (var watermarkProp in watermarkProperties)
                                {
                                    try
                                    {
                                        onPropertyChangedMethod.Invoke(dataContext, new object[] { watermarkProp });
                                        System.Diagnostics.Debug.WriteLine($"Triggered OnPropertyChanged for {watermarkProp}");
                                    }
                                    catch
                                    {
                                        // Ignore individual failures
                                    }
                                }
                            }
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Successfully triggered binding update for valid input: '{text}'");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to trigger binding update: {ex.Message}");
                }
                
                return; // Exact match, do nothing more
            }
            
            // Fuzzy match: contains or startswith
            var like = sourceItems
                .Where(item => item != null && item.ToString() != null && item.ToString()!.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                .Take(20)
                .ToList();
                
            System.Diagnostics.Debug.WriteLine($"TextBoxFuzzyValidationBehavior.OnLostFocus: Found {like.Count} matches");
            
            if (like.Count == 0)
            {
                // No fuzzy matches found - clear the textbox to maintain data integrity
                System.Diagnostics.Debug.WriteLine($"TextBoxFuzzyValidationBehavior.OnLostFocus: No matches found for '{text}'. Clearing textbox.");
                box.Text = string.Empty;
                
                // Add error styling for invalid input
                if (!box.Classes.Contains("error"))
                {
                    box.Classes.Add("error");
                    System.Diagnostics.Debug.WriteLine($"Added 'error' class to textbox for invalid input: '{text}'");
                }
                
                // Show user feedback about the clearing action
                try
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Services.ErrorHandling.HandleErrorAsync(
                                new ArgumentException($"Invalid input: '{text}' not found in available options."),
                                "Input validation failed - textbox cleared",
                                "System"
                            );
                        }
                        catch
                        {
                            // Fallback if ErrorHandling service is not available
                            System.Diagnostics.Debug.WriteLine($"Invalid input: '{text}' not found in available options. Input cleared.");
                        }
                    });
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine($"Invalid input: '{text}' not found in available options. Input cleared.");
                }
                return;
            }
                
            var handler = SuggestionOverlayRequested;
            System.Diagnostics.Debug.WriteLine($"TextBoxFuzzyValidationBehavior.OnLostFocus: Event handler is {(handler != null ? "not null" : "null")}");
            
            if (handler != null)
            {
                // Clear error state for fuzzy matches since we're showing suggestions (not an error)
                if (box.Classes.Contains("error"))
                {
                    box.Classes.Remove("error");
                    System.Diagnostics.Debug.WriteLine($"Removed 'error' class from textbox - showing fuzzy match suggestions for: '{text}'");
                }
                
                // Also trigger validation to clear error messages for partial matches
                try
                {
                    var dataContext = box.DataContext;
                    if (dataContext != null)
                    {
                        // Clear error state since we have partial matches (not an error condition)
                        var hasErrorProperty = dataContext.GetType().GetProperty("HasError");
                        if (hasErrorProperty != null && hasErrorProperty.CanWrite)
                        {
                            hasErrorProperty.SetValue(dataContext, false);
                            System.Diagnostics.Debug.WriteLine($"Cleared HasError for fuzzy matches: '{text}'");
                        }
                        
                        var errorMessageProperty = dataContext.GetType().GetProperty("ErrorMessage");
                        if (errorMessageProperty != null && errorMessageProperty.CanWrite)
                        {
                            errorMessageProperty.SetValue(dataContext, string.Empty);
                            System.Diagnostics.Debug.WriteLine($"Cleared ErrorMessage for fuzzy matches: '{text}'");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to clear error state for fuzzy matches: {ex.Message}");
                }
                
                System.Diagnostics.Debug.WriteLine($"TextBoxFuzzyValidationBehavior.OnLostFocus: Firing SuggestionOverlayRequested event with {like.Count} suggestions");
                handler(box, like);
            }
        }

    // REMOVED: ShowSuggestionFlyout. Overlay is now handled by parent.
    }
}
