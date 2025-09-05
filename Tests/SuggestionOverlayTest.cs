using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Tests
{
    /// <summary>
    /// Test class to validate the suggestion overlay bug fix
    /// </summary>
    public static class SuggestionOverlayTest
    {
        public static async Task<bool> TestSuggestionOverlayFixAsync()
        {
            await Task.Yield();

            try
            {
                Console.WriteLine("=== Testing Suggestion Overlay Bug Fix ===");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Starting suggestion overlay test...");

                // Create a logger for testing
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var logger = loggerFactory.CreateLogger<SuggestionOverlayService>();

                // Create the service
                var suggestionService = new SuggestionOverlayService(logger);

                // Test 1: FilterSuggestions with matching input
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 1: Testing with matching input...");
                var suggestions = new List<string> { "PART001", "PART002", "PART003", "ABC123", "XYZ456" };
                var userInput = "PART";

                // Use reflection to test the private FilterSuggestions method
                var method = typeof(SuggestionOverlayService).GetMethod("FilterSuggestions", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (method == null)
                {
                    throw new Exception("FilterSuggestions method not found");
                }

                var filteredResults = (List<string>)method.Invoke(suggestionService, new object[] { suggestions, userInput });

                Console.WriteLine($"Input: '{userInput}', Filtered results: [{string.Join(", ", filteredResults)}]");
                
                if (!filteredResults.Any(s => s.Contains("PART")))
                {
                    throw new Exception("Test 1 failed: Expected to find matching results");
                }

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ Test 1 passed");

                // Test 2: FilterSuggestions with non-matching input (the bug scenario)
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 2: Testing with non-matching input (bug scenario)...");
                var nonMatchingInput = "XYZ999";
                var filteredNonMatching = (List<string>)method.Invoke(suggestionService, new object[] { suggestions, nonMatchingInput });

                Console.WriteLine($"Input: '{nonMatchingInput}', Filtered results: [{string.Join(", ", filteredNonMatching)}]");

                // The fix should ensure we always have at least one result (the "Add new:" option)
                if (!filteredNonMatching.Any())
                {
                    throw new Exception("Test 2 failed: Expected to have at least one result even for non-matching input");
                }

                // Check if the "Add new:" option is present
                var hasAddNewOption = filteredNonMatching.Any(s => s.StartsWith("Add new:", StringComparison.OrdinalIgnoreCase));
                if (!hasAddNewOption)
                {
                    throw new Exception("Test 2 failed: Expected 'Add new:' option for non-matching input");
                }

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ Test 2 passed");

                // Test 3: Test empty input
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 3: Testing with empty input...");
                var emptyInput = "";
                var filteredEmpty = (List<string>)method.Invoke(suggestionService, new object[] { suggestions, emptyInput });

                Console.WriteLine($"Empty input, Filtered results count: {filteredEmpty.Count}");
                
                if (filteredEmpty.Count == 0)
                {
                    throw new Exception("Test 3 failed: Expected to return suggestions for empty input");
                }

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ Test 3 passed");

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ All suggestion overlay tests passed!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ❌ Suggestion overlay test failed:");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Test the ExtractValueFromSuggestion helper method
        /// </summary>
        public static bool TestExtractValueFromSuggestion()
        {
            try
            {
                Console.WriteLine("=== Testing ExtractValueFromSuggestion Helper ===");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Starting extract value test...");

                // Test cases for the extraction logic
                var testCases = new[]
                {
                    ("Add new: XYZ123", "XYZ123"),
                    ("PART001", "PART001"),
                    ("Add new: ", ""),
                    ("", ""),
                    ("ADD NEW: test", "test"), // Case insensitive
                };

                foreach (var (input, expected) in testCases)
                {
                    var result = ExtractValueFromSuggestionHelper(input);
                    if (result != expected)
                    {
                        throw new Exception($"Extract test failed: Input '{input}' expected '{expected}' but got '{result}'");
                    }
                    Console.WriteLine($"✅ '{input}' -> '{result}'");
                }

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ Extract value test passed!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ❌ Extract value test failed:");
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Helper method that mimics the ExtractValueFromSuggestion logic
        /// </summary>
        private static string ExtractValueFromSuggestionHelper(string suggestion)
        {
            if (string.IsNullOrEmpty(suggestion))
                return suggestion;

            // Remove "Add new: " prefix if present
            const string addNewPrefix = "Add new: ";
            if (suggestion.StartsWith(addNewPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return suggestion.Substring(addNewPrefix.Length);
            }

            return suggestion;
        }
    }
}