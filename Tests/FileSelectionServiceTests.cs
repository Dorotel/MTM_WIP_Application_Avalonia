using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Tests;

/// <summary>
/// Simple validation tests for FileSelectionService to ensure basic functionality works.
/// These are integration-style tests that validate the service interfaces and key methods.
/// </summary>
public static class FileSelectionServiceTests
{
    /// <summary>
    /// Validates that FileSelectionService can be instantiated and basic methods work
    /// </summary>
    public static async Task<bool> ValidateFileSelectionService()
    {
        try
        {
            // Create test logger
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<FileSelectionService>();
            
            // Create mock navigation service (minimal implementation for testing)
            var mockNavigation = new TestNavigationService();
            
            // Create the service
            var service = new FileSelectionService(logger, mockNavigation);
            
            Console.WriteLine("[TEST] FileSelectionService instantiated successfully");

            // Test 1: Validate file access with non-existent file
            var invalidFile = await service.ValidateFileAccessAsync("non-existent-file.json");
            if (invalidFile)
            {
                Console.WriteLine("[ERROR] ValidateFileAccessAsync should return false for non-existent file");
                return false;
            }
            Console.WriteLine("[PASS] ValidateFileAccessAsync correctly returns false for non-existent file");

            // Test 2: Create a test file and validate access
            var testFile = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.json");
            await File.WriteAllTextAsync(testFile, "{}");
            
            var validFile = await service.ValidateFileAccessAsync(testFile);
            if (!validFile)
            {
                Console.WriteLine("[ERROR] ValidateFileAccessAsync should return true for valid file");
                return false;
            }
            Console.WriteLine("[PASS] ValidateFileAccessAsync correctly returns true for valid file");

            // Test 3: Get file info
            var fileInfo = await service.GetFileInfoAsync(testFile);
            if (fileInfo == null)
            {
                Console.WriteLine("[ERROR] GetFileInfoAsync should return FileInfo for valid file");
                return false;
            }
            Console.WriteLine($"[PASS] GetFileInfoAsync returned FileInfo - Size: {fileInfo.Length} bytes");

            // Test 4: Test FileSelectionOptions
            var options = new FileSelectionOptions
            {
                Title = "Test Selection",
                Extensions = new[] { "*.json", "*.txt" },
                Mode = FileSelectionMode.Import,
                PreferredPlacement = PanelPlacement.Auto
            };

            if (options.Title != "Test Selection")
            {
                Console.WriteLine("[ERROR] FileSelectionOptions not working correctly");
                return false;
            }
            Console.WriteLine("[PASS] FileSelectionOptions configuration works correctly");

            // Cleanup
            File.Delete(testFile);

            Console.WriteLine("[SUCCESS] All FileSelectionService tests passed!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] FileSelectionService test failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Minimal test navigation service for testing
    /// </summary>
    private class TestNavigationService : INavigationService
    {
        public object? CurrentView => null;
        public bool CanGoBack => false;
        public bool CanGoForward => false;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<NavigationEventArgs>? Navigated;

        public void ClearHistory() { }
        public void GoBack() { }
        public void GoForward() { }
        public void NavigateTo<T>() where T : class { }
        public void NavigateTo(object viewModel) { }
        public void NavigateTo(string viewKey) { }
    }
}

/// <summary>
/// Simple test runner for validation tests
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Runs all validation tests for the File Selection functionality
    /// </summary>
    public static async Task<bool> RunAllTests()
    {
        Console.WriteLine("=== MTM File Selection Service Tests ===");
        Console.WriteLine();

        var allPassed = true;

        // Test FileSelectionService
        Console.WriteLine("Testing FileSelectionService...");
        var fileSelectionPassed = await FileSelectionServiceTests.ValidateFileSelectionService();
        allPassed &= fileSelectionPassed;

        Console.WriteLine();
        Console.WriteLine($"=== Test Results: {(allPassed ? "ALL PASSED" : "SOME FAILED")} ===");
        
        return allPassed;
    }
}