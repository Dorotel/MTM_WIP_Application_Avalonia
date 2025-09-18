using System;
using MTM_WIP_Application_Avalonia.Tests;

namespace MTM_WIP_Application_Avalonia.TestRunner
{
    /// <summary>
    /// Simple console test runner for Phase 2 sorting functionality
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("MTM CustomDataGrid Phase 2 Sorting Test Runner");
            Console.WriteLine("================================================");
            
            try
            {
                bool success = SortingFunctionalityTest.RunTests();
                
                if (success)
                {
                    Console.WriteLine("\n🎉 All tests passed! Phase 2 sorting functionality is working correctly.");
                    return 0; // Success
                }
                else
                {
                    Console.WriteLine("\n💥 Some tests failed.");
                    return 1; // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n💥 Test runner failed with exception: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 1; // Failure
            }
        }
    }
}