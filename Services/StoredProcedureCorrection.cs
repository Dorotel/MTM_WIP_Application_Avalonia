using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Stored Procedure Analysis and Correction Service - Phase 2 Implementation
/// 
/// Provides prioritized analysis of validation issues and generates specific correction recommendations
/// for the most critical database consistency problems identified in Phase 1.
/// </summary>
public interface IStoredProcedureCorrectionService
{
    Task<CorrectionReport> AnalyzeValidationResultsAsync(ValidationReport validationReport);
    Task<List<CorrectionAction>> GenerateCorrectionActionsAsync(ValidationReport validationReport);
    Task<bool> ApplyCorrectionActionsAsync(List<CorrectionAction> actions, bool dryRun = true);
}

public class StoredProcedureCorrectionService : IStoredProcedureCorrectionService
{
    private readonly ILogger<StoredProcedureCorrectionService> _logger;
    private readonly IStoredProcedureValidationService _validationService;

    public StoredProcedureCorrectionService(
        ILogger<StoredProcedureCorrectionService> logger,
        IStoredProcedureValidationService validationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    public async Task<CorrectionReport> AnalyzeValidationResultsAsync(ValidationReport validationReport)
    {
        _logger.LogInformation("Starting analysis of validation results for correction prioritization");

        var report = new CorrectionReport
        {
            GeneratedOn = DateTime.Now,
            ValidationPhase = "Phase 2: Parameter Validation and Correction Analysis",
            SourceValidationReport = validationReport
        };

        // Analyze the validation results to identify patterns and priorities
        AnalyzeMissingOutputParameters(validationReport, report);
        AnalyzeParameterNameMismatches(validationReport, report);
        AnalyzeMissingRequiredParameters(validationReport, report);
        AnalyzeUnknownParameters(validationReport, report);
        GeneratePriorityRecommendations(report);

        _logger.LogInformation("Analysis complete: {TotalIssueCategories} issue categories identified", 
            report.IssueCategories.Count);

        return report;
    }

    public async Task<List<CorrectionAction>> GenerateCorrectionActionsAsync(ValidationReport validationReport)
    {
        var actions = new List<CorrectionAction>();
        var analysisReport = await AnalyzeValidationResultsAsync(validationReport);

        // Generate specific correction actions based on the analysis
        foreach (var category in analysisReport.IssueCategories.Where(c => c.Priority <= 2)) // High and critical priority only
        {
            switch (category.CategoryType)
            {
                case IssueCategoryType.MissingOutputParameters:
                    actions.AddRange(GenerateOutputParameterCorrections(category));
                    break;
                case IssueCategoryType.ParameterNameMismatches:
                    actions.AddRange(GenerateParameterNameCorrections(category));
                    break;
                case IssueCategoryType.MissingRequiredParameters:
                    actions.AddRange(GenerateRequiredParameterCorrections(category));
                    break;
            }
        }

        _logger.LogInformation("Generated {ActionCount} correction actions", actions.Count);
        return actions;
    }

    public async Task<bool> ApplyCorrectionActionsAsync(List<CorrectionAction> actions, bool dryRun = true)
    {
        if (dryRun)
        {
            _logger.LogInformation("DRY RUN: Would apply {ActionCount} correction actions", actions.Count);
            
            foreach (var action in actions.Take(10)) // Show first 10 for preview
            {
                _logger.LogInformation("DRY RUN Action: {ActionType} - {Description}", 
                    action.ActionType, action.Description);
            }
            
            return true;
        }

        // Actual implementation would apply the corrections
        // This is a placeholder for the actual correction logic
        _logger.LogWarning("Actual correction application not yet implemented");
        return false;
    }

    private void AnalyzeMissingOutputParameters(ValidationReport validationReport, CorrectionReport report)
    {
        var missingOutputParams = validationReport.ValidationResults
            .Where(r => r.ParameterIssues.Any(i => i.Contains("Missing standard p_Status OUTPUT parameter") || 
                                                   i.Contains("Missing standard p_ErrorMsg OUTPUT parameter")))
            .ToList();

        if (missingOutputParams.Any())
        {
            report.IssueCategories.Add(new IssueCategory
            {
                CategoryType = IssueCategoryType.MissingOutputParameters,
                Title = "Missing Standard Output Parameters",
                Description = "Stored procedures missing required @p_Status and @p_ErrorMsg output parameters for MTM standard compliance",
                Priority = 1, // Critical - affects error handling
                AffectedProcedures = missingOutputParams.Select(r => r.ProcedureName).Distinct().ToList(),
                IssueCount = missingOutputParams.Count,
                RecommendedAction = "Add standard OUTPUT parameters @p_Status INT and @p_ErrorMsg VARCHAR(255) to all procedures"
            });
        }
    }

    private void AnalyzeParameterNameMismatches(ValidationReport validationReport, CorrectionReport report)
    {
        var nameMismatches = validationReport.ValidationResults
            .Where(r => r.ParameterIssues.Any(i => i.Contains("not found in procedure definition")))
            .ToList();

        if (nameMismatches.Any())
        {
            report.IssueCategories.Add(new IssueCategory
            {
                CategoryType = IssueCategoryType.ParameterNameMismatches,
                Title = "Parameter Name Mismatches",
                Description = "C# code using parameter names that don't match stored procedure definitions",
                Priority = 2, // High - causes runtime errors
                AffectedProcedures = nameMismatches.Select(r => r.ProcedureName).Distinct().ToList(),
                IssueCount = nameMismatches.SelectMany(r => r.ParameterIssues).Count(i => i.Contains("not found in procedure definition")),
                RecommendedAction = "Update C# parameter names to match stored procedure definitions or update procedure parameters"
            });
        }
    }

    private void AnalyzeMissingRequiredParameters(ValidationReport validationReport, CorrectionReport report)
    {
        var missingRequired = validationReport.ValidationResults
            .Where(r => r.ParameterIssues.Any(i => i.Contains("Required parameter") && i.Contains("not found in call")))
            .ToList();

        if (missingRequired.Any())
        {
            report.IssueCategories.Add(new IssueCategory
            {
                CategoryType = IssueCategoryType.MissingRequiredParameters,
                Title = "Missing Required Parameters",
                Description = "C# code not providing all required input parameters for stored procedure calls",
                Priority = 1, // Critical - causes procedure execution failures
                AffectedProcedures = missingRequired.Select(r => r.ProcedureName).Distinct().ToList(),
                IssueCount = missingRequired.SelectMany(r => r.ParameterIssues).Count(i => i.Contains("Required parameter") && i.Contains("not found in call")),
                RecommendedAction = "Add missing required parameters to C# stored procedure calls"
            });
        }
    }

    private void AnalyzeUnknownParameters(ValidationReport validationReport, CorrectionReport report)
    {
        var unknownParams = validationReport.ValidationResults
            .Where(r => r.ParameterIssues.Any(i => i.Contains("not found in procedure definition") && !i.Contains("Required parameter")))
            .ToList();

        if (unknownParams.Any())
        {
            report.IssueCategories.Add(new IssueCategory
            {
                CategoryType = IssueCategoryType.UnknownParameters,
                Title = "Unknown Parameters in C# Calls",
                Description = "C# code providing parameters that don't exist in stored procedure definitions",
                Priority = 3, // Medium - may indicate outdated code or incorrect parameter names
                AffectedProcedures = unknownParams.Select(r => r.ProcedureName).Distinct().ToList(),
                IssueCount = unknownParams.SelectMany(r => r.ParameterIssues).Count(i => i.Contains("not found in procedure definition") && !i.Contains("Required parameter")),
                RecommendedAction = "Remove unused parameters from C# calls or add missing parameters to stored procedures"
            });
        }
    }

    private void GeneratePriorityRecommendations(CorrectionReport report)
    {
        report.PriorityRecommendations = new List<string>();

        // Critical issues first
        var criticalCategories = report.IssueCategories.Where(c => c.Priority == 1).OrderByDescending(c => c.IssueCount).ToList();
        foreach (var category in criticalCategories)
        {
            report.PriorityRecommendations.Add($"CRITICAL: {category.Title} affects {category.IssueCount} calls in {category.AffectedProcedures.Count} procedures - {category.RecommendedAction}");
        }

        // High priority issues
        var highCategories = report.IssueCategories.Where(c => c.Priority == 2).OrderByDescending(c => c.IssueCount).ToList();
        foreach (var category in highCategories)
        {
            report.PriorityRecommendations.Add($"HIGH: {category.Title} affects {category.IssueCount} calls in {category.AffectedProcedures.Count} procedures - {category.RecommendedAction}");
        }

        // Medium priority issues
        var mediumCategories = report.IssueCategories.Where(c => c.Priority == 3).OrderByDescending(c => c.IssueCount).ToList();
        foreach (var category in mediumCategories.Take(3)) // Limit to top 3 medium priority
        {
            report.PriorityRecommendations.Add($"MEDIUM: {category.Title} affects {category.IssueCount} calls in {category.AffectedProcedures.Count} procedures - {category.RecommendedAction}");
        }
    }

    private List<CorrectionAction> GenerateOutputParameterCorrections(IssueCategory category)
    {
        return category.AffectedProcedures.Select(procedureName => new CorrectionAction
        {
            ActionType = CorrectionActionType.AddOutputParameters,
            ProcedureName = procedureName,
            Description = $"Add standard OUTPUT parameters @p_Status INT and @p_ErrorMsg VARCHAR(255) to {procedureName}",
            Priority = category.Priority,
            SqlChanges = new List<string>
            {
                $"ALTER PROCEDURE {procedureName} ADD @p_Status INT OUTPUT, @p_ErrorMsg VARCHAR(255) OUTPUT",
                $"-- Add to end of {procedureName}: SET @p_Status = 1; SET @p_ErrorMsg = 'Success';"
            }
        }).ToList();
    }

    private List<CorrectionAction> GenerateParameterNameCorrections(IssueCategory category)
    {
        // This would need more detailed analysis of the specific parameter mismatches
        // For now, return placeholder actions
        return new List<CorrectionAction>
        {
            new CorrectionAction
            {
                ActionType = CorrectionActionType.FixParameterNames,
                Description = $"Review and fix parameter name mismatches in {category.AffectedProcedures.Count} procedures",
                Priority = category.Priority,
                RequiresManualReview = true
            }
        };
    }

    private List<CorrectionAction> GenerateRequiredParameterCorrections(IssueCategory category)
    {
        // This would need more detailed analysis of the specific missing parameters
        // For now, return placeholder actions
        return new List<CorrectionAction>
        {
            new CorrectionAction
            {
                ActionType = CorrectionActionType.AddMissingParameters,
                Description = $"Add missing required parameters to C# calls for {category.AffectedProcedures.Count} procedures",
                Priority = category.Priority,
                RequiresManualReview = true
            }
        };
    }
}

/// <summary>
/// Report containing analysis results and correction recommendations
/// </summary>
public class CorrectionReport
{
    public DateTime GeneratedOn { get; set; }
    public string ValidationPhase { get; set; } = string.Empty;
    public ValidationReport? SourceValidationReport { get; set; }
    public List<IssueCategory> IssueCategories { get; set; } = new();
    public List<string> PriorityRecommendations { get; set; } = new();

    public string ToSummaryReport()
    {
        var report = $"""
        MTM WIP Application - Database Correction Analysis Report
        Generated: {GeneratedOn:yyyy-MM-dd HH:mm:ss}
        Phase: {ValidationPhase}
        
        ISSUE CATEGORIES IDENTIFIED:
        """;

        foreach (var category in IssueCategories.OrderBy(c => c.Priority))
        {
            var priorityText = category.Priority switch
            {
                1 => "CRITICAL",
                2 => "HIGH",
                3 => "MEDIUM",
                _ => "LOW"
            };

            report += $"""
            
            {priorityText}: {category.Title}
            - Affects {category.IssueCount} calls in {category.AffectedProcedures.Count} procedures
            - {category.Description}
            - Recommended: {category.RecommendedAction}
            """;
        }

        report += $"""
        
        PRIORITY RECOMMENDATIONS:
        """;

        foreach (var recommendation in PriorityRecommendations)
        {
            report += $"\n- {recommendation}";
        }

        return report;
    }
}

/// <summary>
/// Category of validation issues for analysis and prioritization
/// </summary>
public class IssueCategory
{
    public IssueCategoryType CategoryType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; } // 1 = Critical, 2 = High, 3 = Medium, 4 = Low
    public List<string> AffectedProcedures { get; set; } = new();
    public int IssueCount { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
}

/// <summary>
/// Types of validation issue categories
/// </summary>
public enum IssueCategoryType
{
    MissingOutputParameters,
    ParameterNameMismatches,
    MissingRequiredParameters,
    UnknownParameters,
    StatusCodeInconsistencies,
    TypeMismatches
}

/// <summary>
/// Specific correction action to fix identified issues
/// </summary>
public class CorrectionAction
{
    public CorrectionActionType ActionType { get; set; }
    public string ProcedureName { get; set; } = string.Empty;
    public string SourceFile { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<string> SqlChanges { get; set; } = new();
    public List<string> CSharpChanges { get; set; } = new();
    public bool RequiresManualReview { get; set; }
}

/// <summary>
/// Types of correction actions
/// </summary>
public enum CorrectionActionType
{
    AddOutputParameters,
    FixParameterNames,
    AddMissingParameters,
    RemoveUnusedParameters,
    UpdateStatusCodeHandling,
    FixParameterTypes
}