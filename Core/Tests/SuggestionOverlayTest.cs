using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Extensions;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Core.Tests;

/// <summary>
/// Test harness for the suggestion overlay bug fix
/// Verifies that non-matching entries now display the overlay correctly
/// </summary>
public static class SuggestionOverlayTest
{
    public static async Task<bool> TestSuggestionOverlayFixAsync()
    {
        await Task.Yield(); // Ensure async execution
        
        try
        {
            Console.WriteLine("=== MTM Suggestion Overlay Fix Test ===");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Starting suggestion overlay fix test...");

            // Setup minimal service collection
            var services = new ServiceCollection();
            
            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Add configuration
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();
            services.AddSingleton<IConfiguration>(configuration);

            // Register suggestion overlay service
            services.AddScoped<ISuggestionOverlayService, SuggestionOverlayService>();

            using var serviceProvider = services.BuildServiceProvider();
            var suggestionService = serviceProvider.GetRequiredService<ISuggestionOverlayService>();

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] SuggestionOverlayService resolved successfully");

            // Test 1: Non-matching input should now include "Add new:" option
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 1: Testing non-matching input...");
            
            var suggestions = new List<string> { "PART001", "PART002", "ABC123", "XYZ789" };
            var userInput = "NONEXISTENT"; // This input doesn't match any suggestions

            // Access the private FilterSuggestions method using reflection for testing
            var suggestionServiceType = typeof(SuggestionOverlayService);
            var filterMethod = suggestionServiceType.GetMethod("FilterSuggestions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (filterMethod == null)
            {
                throw new InvalidOperationException("FilterSuggestions method not found");
            }

            var filteredResults = (List<string>)(filterMethod.Invoke(suggestionService, new object[] { suggestions, userInput }) ?? new List<string>());

            // Verify results
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Filtered results count: {filteredResults.Count}");
            foreach (var result in filteredResults)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - {result}");
            }

            // Test should pass if we have results and one is "Add new: NONEXISTENT"
            if (filteredResults.Count == 0)
            {
                throw new InvalidOperationException("❌ FAILED: FilterSuggestions returned empty list for non-matching input");
            }

            var hasAddNewOption = filteredResults.Any(r => r.StartsWith("Add new:", StringComparison.OrdinalIgnoreCase));
            if (!hasAddNewOption)
            {
                throw new InvalidOperationException("❌ FAILED: FilterSuggestions did not include 'Add new:' option for non-matching input");
            }

            var expectedAddNew = "Add new: NONEXISTENT";
            if (!filteredResults.Contains(expectedAddNew))
            {
                throw new InvalidOperationException($"❌ FAILED: Expected '{expectedAddNew}' not found in results");
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ Test 1 PASSED: Non-matching input correctly includes 'Add new' option");

            // Test 2: Matching input should not include "Add new:" option  
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 2: Testing exact matching input...");
            
            var exactMatchInput = "PART001"; // This exactly matches a suggestion
            var exactMatchResults = (List<string>)(filterMethod.Invoke(suggestionService, new object[] { suggestions, exactMatchInput }) ?? new List<string>());

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Exact match results count: {exactMatchResults.Count}");
            foreach (var result in exactMatchResults)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - {result}");
            }

            var hasAddNewForExact = exactMatchResults.Any(r => r.StartsWith("Add new:", StringComparison.OrdinalIgnoreCase));
            if (hasAddNewForExact)
            {
                throw new InvalidOperationException("❌ FAILED: FilterSuggestions incorrectly included 'Add new:' option for exact matching input");
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ Test 2 PASSED: Exact matching input correctly excludes 'Add new' option");

            // Test 3: Partial matching input should include both matches and "Add new:" option
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 3: Testing partial matching input...");
            
            var partialInput = "PART"; // This partially matches PART001 and PART002 but isn't exact
            var partialResults = (List<string>)(filterMethod.Invoke(suggestionService, new object[] { suggestions, partialInput }) ?? new List<string>());

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Partial match results count: {partialResults.Count}");
            foreach (var result in partialResults)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - {result}");
            }

            var hasPartialMatches = partialResults.Any(r => r.Contains("PART001") || r.Contains("PART002"));
            var hasAddNewForPartial = partialResults.Any(r => r.StartsWith("Add new:", StringComparison.OrdinalIgnoreCase));

            if (!hasPartialMatches)
            {
                throw new InvalidOperationException("❌ FAILED: FilterSuggestions did not include partial matches");
            }

            if (!hasAddNewForPartial)
            {
                throw new InvalidOperationException("❌ FAILED: FilterSuggestions did not include 'Add new:' option for partial matching input");
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ Test 3 PASSED: Partial matching input correctly includes both matches and 'Add new' option");

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ All suggestion overlay fix tests completed successfully!");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] 🎉 The bug fix is working correctly - overlay will now display for non-matching entries");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ❌ Suggestion overlay fix test failed:");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }
}