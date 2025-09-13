using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Stored Procedure Validation Service - Phase 1 Implementation
/// 
/// Provides comprehensive validation and analysis of stored procedures against C# code usage.
/// Implements the MTM WIP Application database consistency and error handling standardization requirements.
/// </summary>
public interface IStoredProcedureValidationService
{
    Task<ValidationReport> ValidateAllStoredProceduresAsync();
    Task<List<StoredProcedureDefinition>> ExtractStoredProcedureDefinitionsAsync();
    Task<List<StoredProcedureCall>> ExtractStoredProcedureCallsAsync();
    Task<ValidationReport> GenerateValidationReportAsync();
}

/// <summary>
/// Stored Procedure Validation Service Implementation
/// </summary>
public class StoredProcedureValidationService : IStoredProcedureValidationService
{
    private readonly ILogger<StoredProcedureValidationService> _logger;
    private readonly string _basePath;
    private readonly string _sqlFilesPath;

    public StoredProcedureValidationService(ILogger<StoredProcedureValidationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        _sqlFilesPath = Path.Combine(_basePath, "Documentation", "Development", "Database_Files");
    }

    public async Task<ValidationReport> ValidateAllStoredProceduresAsync()
    {
        _logger.LogInformation("Starting comprehensive stored procedure validation");
        
        try
        {
            var report = new ValidationReport
            {
                GeneratedOn = DateTime.Now,
                ValidationPhase = "Phase 1: Foundation and Analysis"
            };

            // Extract stored procedure definitions from SQL files
            report.StoredProcedureDefinitions = await ExtractStoredProcedureDefinitionsAsync();
            _logger.LogInformation("Extracted {Count} stored procedure definitions", report.StoredProcedureDefinitions.Count);

            // Extract stored procedure calls from C# files
            report.StoredProcedureCalls = await ExtractStoredProcedureCallsAsync();
            _logger.LogInformation("Extracted {Count} stored procedure calls from C# code", report.StoredProcedureCalls.Count);

            // Generate validation results
            report.ValidationResults = ValidateCallsAgainstDefinitions(report.StoredProcedureCalls, report.StoredProcedureDefinitions);
            
            // Generate summary statistics
            report.TotalProcedures = report.StoredProcedureDefinitions.Count;
            report.TotalCalls = report.StoredProcedureCalls.Count;
            report.MismatchedCalls = report.ValidationResults.Count(v => !v.IsValid);
            report.ParameterMismatches = report.ValidationResults.SelectMany(v => v.ParameterIssues).Count();

            _logger.LogInformation("Validation complete: {TotalProcedures} procedures, {TotalCalls} calls, {Mismatches} mismatches", 
                report.TotalProcedures, report.TotalCalls, report.MismatchedCalls);

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate stored procedures");
            throw;
        }
    }

    public async Task<List<StoredProcedureDefinition>> ExtractStoredProcedureDefinitionsAsync()
    {
        var definitions = new List<StoredProcedureDefinition>();
        
        var sqlFilePath = Path.Combine(_sqlFilesPath, "Development_Stored_Procedures.sql");
        _logger.LogInformation("Looking for SQL file at: {FilePath}", sqlFilePath);
        _logger.LogInformation("SQL files path: {SqlFilesPath}", _sqlFilesPath);
        _logger.LogInformation("Base path: {BasePath}", _basePath);
        
        if (!File.Exists(sqlFilePath))
        {
            _logger.LogWarning("SQL file not found: {FilePath}", sqlFilePath);
            
            // Try alternate paths
            var alternatePath = Path.Combine(Directory.GetCurrentDirectory(), "Documentation", "Development", "Database_Files", "Development_Stored_Procedures.sql");
            _logger.LogInformation("Trying alternate path: {AlternatePath}", alternatePath);
            
            if (File.Exists(alternatePath))
            {
                sqlFilePath = alternatePath;
                _logger.LogInformation("Found SQL file at alternate path: {FilePath}", sqlFilePath);
            }
            else
            {
                return definitions;
            }
        }

        var sqlContent = await File.ReadAllTextAsync(sqlFilePath);
        
        // Regex to match stored procedure definitions
        var procedureRegex = new Regex(
            @"CREATE\s+(?:DEFINER=`[^`]+`@`[^`]+`\s+)?PROCEDURE\s+`?([^`\s(]+)`?\s*\((.*?)\)\s*BEGIN",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        var matches = procedureRegex.Matches(sqlContent);
        
        foreach (Match match in matches)
        {
            try
            {
                var procedureName = match.Groups[1].Value.Trim();
                var parametersText = match.Groups[2].Value.Trim();
                
                var definition = new StoredProcedureDefinition
                {
                    Name = procedureName,
                    Parameters = ExtractParameters(parametersText),
                    SourceFile = "Development_Stored_Procedures.sql"
                };
                
                definitions.Add(definition);
                _logger.LogDebug("Extracted procedure: {Name} with {ParameterCount} parameters", 
                    procedureName, definition.Parameters.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse stored procedure definition: {Match}", match.Value);
            }
        }

        return definitions;
    }

    public async Task<List<StoredProcedureCall>> ExtractStoredProcedureCallsAsync()
    {
        var calls = new List<StoredProcedureCall>();
        
        // Search for C# files that contain stored procedure calls
        var csharpFiles = Directory.GetFiles(_basePath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains("bin") && !f.Contains("obj") && !f.Contains("Documentation"))
            .ToList();

        foreach (var file in csharpFiles)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file);
                var fileCalls = ExtractCallsFromFile(content, file);
                calls.AddRange(fileCalls);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read file: {File}", file);
            }
        }

        return calls;
    }

    public async Task<ValidationReport> GenerateValidationReportAsync()
    {
        return await ValidateAllStoredProceduresAsync();
    }

    private List<ParameterDefinition> ExtractParameters(string parametersText)
    {
        var parameters = new List<ParameterDefinition>();
        
        if (string.IsNullOrWhiteSpace(parametersText))
            return parameters;

        // Split parameters by comma, but be careful of commas inside parentheses
        var parameterPattern = new Regex(
            @"(IN|OUT|INOUT)\s+([`\w]+)\s+(\w+(?:\([^)]*\))?)",
            RegexOptions.IgnoreCase
        );

        var matches = parameterPattern.Matches(parametersText);
        
        foreach (Match match in matches)
        {
            var direction = match.Groups[1].Value.ToUpperInvariant();
            var name = match.Groups[2].Value.Trim('`');
            var type = match.Groups[3].Value;
            
            parameters.Add(new ParameterDefinition
            {
                Name = name,
                Type = type,
                Direction = direction
            });
        }

        return parameters;
    }

    private List<StoredProcedureCall> ExtractCallsFromFile(string content, string filePath)
    {
        var calls = new List<StoredProcedureCall>();
        
        // Look for the full pattern: Dictionary construction followed by stored procedure call
        var fullPattern = new Regex(
            @"var\s+parameters\s*=\s*new\s+Dictionary<string,\s*object>\s*\{(.*?)\}.*?Helper_Database_StoredProcedure\.(?:ExecuteDataTableWithStatus|ExecuteWithStatus)\s*\(\s*[^,]+,\s*[""']([^""']+)[""']",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        var matches = fullPattern.Matches(content);
        
        foreach (Match match in matches)
        {
            try
            {
                var parametersText = match.Groups[1].Value;
                var procedureName = match.Groups[2].Value;
                
                // Find line number for the stored procedure call
                var callIndex = match.Value.LastIndexOf("Helper_Database_StoredProcedure");
                var lineNumber = GetLineNumber(content, match.Index + callIndex);
                
                var call = new StoredProcedureCall
                {
                    ProcedureName = procedureName,
                    SourceFile = Path.GetRelativePath(_basePath, filePath),
                    LineNumber = lineNumber,
                    Parameters = ExtractDictionaryParameters(parametersText)
                };
                
                calls.Add(call);
                
                _logger.LogDebug("Extracted call: {ProcedureName} from {File}:{Line} with {ParameterCount} parameters: {Parameters}",
                    procedureName, call.SourceFile, lineNumber, call.Parameters.Count, string.Join(", ", call.Parameters.Keys));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse stored procedure call: {Match}", match.Value.Substring(0, Math.Min(100, match.Value.Length)));
            }
        }

        return calls;
    }

    private Dictionary<string, string> ExtractDictionaryParameters(string dictionaryContent)
    {
        var parameters = new Dictionary<string, string>();
        
        // Pattern to match Dictionary entries: ["key"] = value
        var entryPattern = new Regex(@"\[""([^""]+)""\]\s*=\s*([^,]+)", RegexOptions.IgnoreCase);
        var matches = entryPattern.Matches(dictionaryContent);
        
        foreach (Match match in matches)
        {
            var key = match.Groups[1].Value;
            var value = match.Groups[2].Value.Trim();
            
            // Clean up the value (remove common patterns)
            value = value.TrimEnd(',', ' ', '\r', '\n', '\t');
            
            parameters[key] = value;
        }
        
        return parameters;
    }

    private int GetLineNumber(string content, int position)
    {
        return content.Take(position).Count(c => c == '\n') + 1;
    }

    private List<ValidationResult> ValidateCallsAgainstDefinitions(
        List<StoredProcedureCall> calls, 
        List<StoredProcedureDefinition> definitions)
    {
        var results = new List<ValidationResult>();
        var definitionLookup = definitions.ToDictionary(d => d.Name, StringComparer.OrdinalIgnoreCase);
        
        foreach (var call in calls)
        {
            var result = new ValidationResult
            {
                ProcedureName = call.ProcedureName,
                SourceFile = call.SourceFile,
                LineNumber = call.LineNumber,
                ParameterIssues = new List<string>()
            };

            if (!definitionLookup.TryGetValue(call.ProcedureName, out var definition))
            {
                result.IsValid = false;
                result.ParameterIssues.Add($"Stored procedure '{call.ProcedureName}' not found in SQL definitions");
            }
            else
            {
                // Validate parameters
                ValidateParameters(call, definition, result);
            }

            results.Add(result);
        }

        return results;
    }

    private void ValidateParameters(StoredProcedureCall call, StoredProcedureDefinition definition, ValidationResult result)
    {
        result.IsValid = true;
        
        // Check if all required input parameters are provided
        var requiredInputParams = definition.Parameters
            .Where(p => p.Direction == "IN")
            .ToList();

        foreach (var requiredParam in requiredInputParams)
        {
            var paramName = requiredParam.Name;
            
            // Check common parameter name variations
            var possibleNames = new[] { paramName, paramName.TrimStart('p', '_'), $"p_{paramName}" };
            
            if (!possibleNames.Any(name => call.Parameters.ContainsKey(name)))
            {
                result.IsValid = false;
                result.ParameterIssues.Add($"Required parameter '{paramName}' not found in call");
            }
        }

        // Check for extra parameters that don't exist in the procedure
        foreach (var callParam in call.Parameters.Keys)
        {
            if (!definition.Parameters.Any(p => 
                string.Equals(p.Name, callParam, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p.Name, $"p_{callParam}", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p.Name.TrimStart('p', '_'), callParam, StringComparison.OrdinalIgnoreCase)))
            {
                result.IsValid = false;
                result.ParameterIssues.Add($"Parameter '{callParam}' not found in procedure definition");
            }
        }

        // Check for standard MTM output parameters
        var hasStatusParam = definition.Parameters.Any(p => 
            string.Equals(p.Name, "p_Status", StringComparison.OrdinalIgnoreCase) && p.Direction == "OUT");
        var hasErrorMsgParam = definition.Parameters.Any(p => 
            string.Equals(p.Name, "p_ErrorMsg", StringComparison.OrdinalIgnoreCase) && p.Direction == "OUT");

        if (!hasStatusParam)
        {
            result.IsValid = false;
            result.ParameterIssues.Add("Missing standard p_Status OUTPUT parameter");
        }

        if (!hasErrorMsgParam)
        {
            result.IsValid = false;
            result.ParameterIssues.Add("Missing standard p_ErrorMsg OUTPUT parameter");
        }
    }
}

/// <summary>
/// Data model for stored procedure definition
/// </summary>
public class StoredProcedureDefinition
{
    public string Name { get; set; } = string.Empty;
    public List<ParameterDefinition> Parameters { get; set; } = new();
    public string SourceFile { get; set; } = string.Empty;
}

/// <summary>
/// Data model for stored procedure parameter definition
/// </summary>
public class ParameterDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty; // IN, OUT, INOUT
}

/// <summary>
/// Data model for stored procedure call from C# code
/// </summary>
public class StoredProcedureCall
{
    public string ProcedureName { get; set; } = string.Empty;
    public string SourceFile { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
}

/// <summary>
/// Data model for validation result
/// </summary>
public class ValidationResult
{
    public string ProcedureName { get; set; } = string.Empty;
    public string SourceFile { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public bool IsValid { get; set; }
    public List<string> ParameterIssues { get; set; } = new();
}

/// <summary>
/// Comprehensive validation report
/// </summary>
public class ValidationReport
{
    public DateTime GeneratedOn { get; set; }
    public string ValidationPhase { get; set; } = string.Empty;
    public int TotalProcedures { get; set; }
    public int TotalCalls { get; set; }
    public int MismatchedCalls { get; set; }
    public int ParameterMismatches { get; set; }
    
    public List<StoredProcedureDefinition> StoredProcedureDefinitions { get; set; } = new();
    public List<StoredProcedureCall> StoredProcedureCalls { get; set; } = new();
    public List<ValidationResult> ValidationResults { get; set; } = new();

    public string ToJsonReport()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }

    public string ToSummaryReport()
    {
        var report = $"""
        MTM WIP Application - Database Stored Procedure Validation Report
        Generated: {GeneratedOn:yyyy-MM-dd HH:mm:ss}
        Phase: {ValidationPhase}
        
        SUMMARY STATISTICS:
        - Total Stored Procedures: {TotalProcedures}
        - Total Procedure Calls: {TotalCalls}
        - Mismatched Calls: {MismatchedCalls}
        - Parameter Issues: {ParameterMismatches}
        
        VALIDATION RESULTS:
        """;

        foreach (var result in ValidationResults.Where(r => !r.IsValid))
        {
            report += $"""
            
            PROCEDURE: {result.ProcedureName}
            File: {result.SourceFile}:{result.LineNumber}
            Issues:
            """;
            
            foreach (var issue in result.ParameterIssues)
            {
                report += $"  - {issue}\n";
            }
        }

        return report;
    }
}