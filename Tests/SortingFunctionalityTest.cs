using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Tests
{
    /// <summary>
    /// Simple test to verify sorting functionality works correctly
    /// Tests Phase 2 implementation: Column sorting with visual indicators and multi-column support
    /// </summary>
    public static class SortingFunctionalityTest
    {
        /// <summary>
        /// Runs basic sorting tests to verify Phase 2 implementation
        /// </summary>
        public static bool RunTests()
        {
            Console.WriteLine("=== MTM CustomDataGrid Phase 2 Sorting Tests ===");
            
            try
            {
                TestBasicSortConfiguration();
                TestSortManager();
                TestSortCriteria();
                TestMultiColumnSorting();
                
                Console.WriteLine("✅ All sorting tests passed successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Sorting test failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        private static void TestBasicSortConfiguration()
        {
            Console.WriteLine("\n🧪 Testing SortConfiguration...");
            
            var sortConfig = new SortConfiguration();
            
            // Test single column sort
            sortConfig.ApplySingleColumnSort("PartId", SortDirection.Ascending);
            
            var direction = sortConfig.GetSortDirection("PartId");
            if (direction != SortDirection.Ascending)
                throw new Exception($"Expected Ascending, got {direction}");
                
            var hasActive = sortConfig.HasActiveSorts();
            if (!hasActive)
                throw new Exception("Should have active sorts");
                
            var activeCriteria = sortConfig.GetActiveSortCriteria().ToList();
            if (activeCriteria.Count != 1)
                throw new Exception($"Expected 1 active criteria, got {activeCriteria.Count}");
                
            Console.WriteLine("✅ SortConfiguration basic tests passed");
        }

        private static void TestSortManager()
        {
            Console.WriteLine("\n🧪 Testing SortManager...");
            
            var sortManager = new SortManager();
            var testData = new List<TestItem>
            {
                new() { PartId = "PART003", Operation = "100", Quantity = 15 },
                new() { PartId = "PART001", Operation = "90", Quantity = 25 },
                new() { PartId = "PART002", Operation = "110", Quantity = 5 }
            };
            
            var sortCriteria = new List<SortCriteria>
            {
                new("PartId", SortDirection.Ascending, 0)
            };
            
            var sortedData = sortManager.ApplyMultiColumnSort(testData, sortCriteria).Cast<TestItem>().ToList();
            
            if (sortedData.Count != 3)
                throw new Exception($"Expected 3 items, got {sortedData.Count}");
                
            if (sortedData[0].PartId != "PART001")
                throw new Exception($"Expected PART001 first, got {sortedData[0].PartId}");
                
            if (sortedData[1].PartId != "PART002")
                throw new Exception($"Expected PART002 second, got {sortedData[1].PartId}");
                
            if (sortedData[2].PartId != "PART003")
                throw new Exception($"Expected PART003 third, got {sortedData[2].PartId}");
            
            Console.WriteLine("✅ SortManager tests passed");
        }

        private static void TestSortCriteria()
        {
            Console.WriteLine("\n🧪 Testing SortCriteria...");
            
            var criteria = new SortCriteria("TestColumn", SortDirection.Descending, 1);
            criteria.IsActive = true;
            
            if (criteria.ColumnId != "TestColumn")
                throw new Exception($"Expected TestColumn, got {criteria.ColumnId}");
                
            if (criteria.Direction != SortDirection.Descending)
                throw new Exception($"Expected Descending, got {criteria.Direction}");
                
            if (criteria.Precedence != 1)
                throw new Exception($"Expected precedence 1, got {criteria.Precedence}");
                
            if (!criteria.IsActive)
                throw new Exception("Expected criteria to be active");
            
            Console.WriteLine("✅ SortCriteria tests passed");
        }

        private static void TestMultiColumnSorting()
        {
            Console.WriteLine("\n🧪 Testing Multi-Column Sorting...");
            
            var sortConfig = new SortConfiguration();
            
            // Apply multi-column sort: Primary by Operation, Secondary by PartId
            sortConfig.ApplyMultiColumnSort("Operation", SortDirection.Ascending);
            sortConfig.ApplyMultiColumnSort("PartId", SortDirection.Descending);
            
            var activeSorts = sortConfig.GetActiveSortCriteria().ToList();
            if (activeSorts.Count != 2)
                throw new Exception($"Expected 2 active sorts, got {activeSorts.Count}");
                
            // Verify precedence order
            Console.WriteLine($"📋 Active sorts configuration:");
            for (int i = 0; i < activeSorts.Count; i++)
            {
                var sort = activeSorts[i];
                Console.WriteLine($"  [{i}] ColumnId: {sort.ColumnId}, Direction: {sort.Direction}, Precedence: {sort.Precedence}, IsActive: {sort.IsActive}");
            }
            
            if (activeSorts[0].ColumnId != "Operation" || activeSorts[0].Precedence != 0)
                throw new Exception("Operation should be primary sort (precedence 0)");
                
            if (activeSorts[1].ColumnId != "PartId" || activeSorts[1].Precedence != 1)
                throw new Exception("PartId should be secondary sort (precedence 1)");
            
            // Test with SortManager
            var sortManager = new SortManager();
            var testData = new List<TestItem>
            {
                new() { PartId = "PART003", Operation = "100", Quantity = 15 },
                new() { PartId = "PART001", Operation = "100", Quantity = 25 },
                new() { PartId = "PART002", Operation = "90", Quantity = 5 }
            };

            Console.WriteLine("📋 Original test data:");
            for (int i = 0; i < testData.Count; i++)
            {
                var item = testData[i];
                Console.WriteLine($"  [{i}] PartId: {item.PartId}, Operation: {item.Operation}, Quantity: {item.Quantity}");
            }
            
            IEnumerable<object> sortedData;
            try
            {
                sortedData = sortManager.ApplyMultiColumnSort(testData, activeSorts);
                var sortedList = sortedData.Cast<TestItem>().ToList();
                
                // Debug output to understand what we got
                Console.WriteLine("📋 Sorted data result:");
                for (int i = 0; i < sortedList.Count; i++)
                {
                    var item = sortedList[i];
                    Console.WriteLine($"  [{i}] PartId: {item.PartId}, Operation: {item.Operation}, Quantity: {item.Quantity}");
                }
                
                // Should be sorted by Operation ASC, then PartId DESC within same operation
                if (sortedList[0].Operation != "90" || sortedList[0].PartId != "PART002")
                    throw new Exception($"First item should be Operation=90, PartId=PART002, but got Operation={sortedList[0].Operation}, PartId={sortedList[0].PartId}");
                    
                if (sortedList[1].Operation != "100" || sortedList[1].PartId != "PART003")
                    throw new Exception("Second item should be Operation=100, PartId=PART003 (DESC order)");
                    
                if (sortedList[2].Operation != "100" || sortedList[2].PartId != "PART001")
                    throw new Exception("Third item should be Operation=100, PartId=PART001 (DESC order)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception during sorting: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
            
            Console.WriteLine("✅ Multi-column sorting tests passed");
        }

        /// <summary>
        /// Simple test data class for sorting tests
        /// </summary>
        private class TestItem
        {
            public string PartId { get; set; } = string.Empty;
            public string Operation { get; set; } = string.Empty;
            public int Quantity { get; set; }
        }
    }
}