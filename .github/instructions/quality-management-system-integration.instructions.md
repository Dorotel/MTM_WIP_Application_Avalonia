# Quality Management System Integration - MTM WIP Application Instructions

**Framework**: .NET 8 with Quality Control Integration  
**Pattern**: Statistical Process Control and Quality Assurance  
**Created**: 2025-09-15  

---

## 🏭 Quality Management System Architecture

### Comprehensive Quality Control Service

```csharp
// Advanced quality management service with SPC and quality gate integration
public class QualityManagementService : IQualityManagementService
{
    private readonly IDatabaseService _databaseService;
    private readonly IStatisticalProcessControlService _spcService;
    private readonly IQualityAuditService _auditService;
    private readonly IMessenger _messenger;
    private readonly ILogger<QualityManagementService> _logger;
    
    // Quality standards and thresholds
    private const double DEFAULT_CONTROL_LIMIT_MULTIPLIER = 3.0; // 3-sigma control limits
    private const double QUALITY_TARGET_PERCENTAGE = 0.98; // 98% quality target
    private const int MINIMUM_SAMPLE_SIZE = 30; // Minimum sample for statistical analysis

    public QualityManagementService(
        IDatabaseService databaseService,
        IStatisticalProcessControlService spcService,
        IQualityAuditService auditService,
        IMessenger messenger,
        ILogger<QualityManagementService> logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _spcService = spcService ?? throw new ArgumentNullException(nameof(spcService));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<QualityResult> ProcessQualityInspectionAsync(QualityInspection inspection)
    {
        try
        {
            _logger.LogInformation("Processing quality inspection for part {PartId} at operation {Operation}", 
                inspection.PartId, inspection.Operation);

            // Validate inspection data
            var validationResult = await ValidateInspectionDataAsync(inspection);
            if (!validationResult.IsValid)
            {
                return QualityResult.Failure($"Inspection validation failed: {validationResult.ErrorMessage}");
            }

            // Record inspection in database
            await RecordQualityInspectionAsync(inspection);

            // Perform statistical process control analysis
            var spcAnalysis = await PerformSPCAnalysisAsync(inspection);

            // Check quality gates
            var qualityGateResult = await EvaluateQualityGatesAsync(inspection, spcAnalysis);

            // Generate quality alerts if needed
            await GenerateQualityAlertsAsync(inspection, spcAnalysis, qualityGateResult);

            // Update quality metrics
            await UpdateQualityMetricsAsync(inspection);

            // Send quality update message
            _messenger.Send(new QualityDataUpdatedMessage(
                inspection.PartId,
                inspection.Operation,
                qualityGateResult.OverallResult,
                DateTime.Now));

            var result = new QualityResult
            {
                IsPass = qualityGateResult.OverallResult == QualityGateResult.Pass,
                QualityScore = spcAnalysis.QualityScore,
                ControlChartAnalysis = spcAnalysis,
                QualityGateEvaluation = qualityGateResult,
                RecommendedActions = GenerateRecommendedActions(spcAnalysis, qualityGateResult)
            };

            _logger.LogInformation("Quality inspection processed successfully for part {PartId}: {Result}", 
                inspection.PartId, result.IsPass ? "PASS" : "FAIL");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing quality inspection for part {PartId}", inspection.PartId);
            await ErrorHandling.HandleErrorAsync(ex, "Process quality inspection");
            return QualityResult.Failure($"Quality inspection processing failed: {ex.Message}");
        }
    }

    public async Task<SPCAnalysisResult> PerformSPCAnalysisAsync(QualityInspection inspection)
    {
        try
        {
            // Get historical quality data for SPC analysis
            var historicalData = await GetHistoricalQualityDataAsync(
                inspection.PartId, 
                inspection.Operation, 
                inspection.MeasurementType);

            if (historicalData.Count < MINIMUM_SAMPLE_SIZE)
            {
                _logger.LogWarning("Insufficient historical data for SPC analysis: {Count} samples (minimum {MinimumSamples})", 
                    historicalData.Count, MINIMUM_SAMPLE_SIZE);
            }

            // Calculate control limits
            var controlLimits = _spcService.CalculateControlLimits(historicalData, DEFAULT_CONTROL_LIMIT_MULTIPLIER);

            // Analyze process capability
            var capabilityAnalysis = _spcService.AnalyzeProcessCapability(
                historicalData, 
                inspection.SpecificationLimits);

            // Check for out-of-control conditions
            var controlViolations = _spcService.CheckControlViolations(
                historicalData.Append(inspection.MeasurementValue), 
                controlLimits);

            // Calculate quality score
            var qualityScore = CalculateQualityScore(inspection, controlLimits, capabilityAnalysis);

            return new SPCAnalysisResult
            {
                ControlLimits = controlLimits,
                ProcessCapability = capabilityAnalysis,
                ControlViolations = controlViolations,
                QualityScore = qualityScore,
                SampleSize = historicalData.Count + 1,
                RecommendedAction = DetermineRecommendedAction(controlViolations, capabilityAnalysis)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing SPC analysis for part {PartId}", inspection.PartId);
            throw;
        }
    }

    public async Task<QualityGateEvaluation> EvaluateQualityGatesAsync(QualityInspection inspection, SPCAnalysisResult spcAnalysis)
    {
        try
        {
            var gateEvaluations = new List<IndividualGateResult>();

            // Gate 1: Specification compliance
            var specComplianceResult = EvaluateSpecificationCompliance(inspection);
            gateEvaluations.Add(specComplianceResult);

            // Gate 2: Statistical control
            var statisticalControlResult = EvaluateStatisticalControl(spcAnalysis);
            gateEvaluations.Add(statisticalControlResult);

            // Gate 3: Process capability
            var capabilityResult = EvaluateProcessCapability(spcAnalysis.ProcessCapability);
            gateEvaluations.Add(capabilityResult);

            // Gate 4: Quality trend analysis
            var trendResult = await EvaluateQualityTrendsAsync(inspection.PartId, inspection.Operation);
            gateEvaluations.Add(trendResult);

            // Determine overall result
            var overallResult = DetermineOverallQualityGateResult(gateEvaluations);

            return new QualityGateEvaluation
            {
                OverallResult = overallResult,
                IndividualGateResults = gateEvaluations,
                EvaluationTimestamp = DateTime.Now,
                EvaluatedBy = Environment.UserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating quality gates for part {PartId}", inspection.PartId);
            throw;
        }
    }

    public async Task<QualityAuditResult> ConductQualityAuditAsync(QualityAuditRequest auditRequest)
    {
        try
        {
            _logger.LogInformation("Starting quality audit for {AuditScope}", auditRequest.AuditScope);

            var auditResult = await _auditService.ConductAuditAsync(auditRequest);

            // Record audit results
            await RecordAuditResultsAsync(auditResult);

            // Generate corrective action plans if needed
            if (auditResult.NonConformances.Any())
            {
                await GenerateCorrectiveActionPlansAsync(auditResult);
            }

            _logger.LogInformation("Quality audit completed for {AuditScope}: {NonConformanceCount} non-conformances found", 
                auditRequest.AuditScope, auditResult.NonConformances.Count);

            return auditResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error conducting quality audit for {AuditScope}", auditRequest.AuditScope);
            await ErrorHandling.HandleErrorAsync(ex, "Conduct quality audit");
            throw;
        }
    }

    // Private helper methods
    private async Task<ValidationResult> ValidateInspectionDataAsync(QualityInspection inspection)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(inspection.PartId))
            errors.Add("Part ID is required");

        if (string.IsNullOrWhiteSpace(inspection.Operation))
            errors.Add("Operation is required");

        if (inspection.MeasurementValue < 0)
            errors.Add("Measurement value cannot be negative");

        if (inspection.SpecificationLimits != null)
        {
            if (inspection.SpecificationLimits.LowerLimit >= inspection.SpecificationLimits.UpperLimit)
                errors.Add("Lower specification limit must be less than upper specification limit");
        }

        // Validate against master data
        var partExists = await ValidatePartExistsAsync(inspection.PartId);
        if (!partExists)
            errors.Add($"Part ID {inspection.PartId} does not exist in master data");

        var operationValid = await ValidateOperationAsync(inspection.Operation);
        if (!operationValid)
            errors.Add($"Operation {inspection.Operation} is not valid");

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Error(string.Join(", ", errors));
    }

    private async Task RecordQualityInspectionAsync(QualityInspection inspection)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", inspection.PartId),
            new("p_OperationNumber", inspection.Operation),
            new("p_InspectionType", inspection.InspectionType),
            new("p_MeasurementType", inspection.MeasurementType),
            new("p_MeasurementValue", inspection.MeasurementValue),
            new("p_SpecificationLowerLimit", inspection.SpecificationLimits?.LowerLimit),
            new("p_SpecificationUpperLimit", inspection.SpecificationLimits?.UpperLimit),
            new("p_InspectorId", inspection.InspectorId),
            new("p_InspectionDate", inspection.InspectionDate),
            new("p_Notes", inspection.Notes ?? string.Empty)
        };

        await _databaseService.ExecuteStoredProcedureAsync("qms_inspection_Add", parameters);
    }

    private async Task<IList<double>> GetHistoricalQualityDataAsync(string partId, string operation, string measurementType)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation),
            new("p_MeasurementType", measurementType),
            new("p_LookbackDays", 30) // Get last 30 days of data
        };

        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "qms_historical_measurements_Get", parameters);

        var historicalData = new List<double>();
        foreach (DataRow row in result.Data.Rows)
        {
            if (double.TryParse(row["MeasurementValue"].ToString(), out double value))
            {
                historicalData.Add(value);
            }
        }

        return historicalData;
    }

    private double CalculateQualityScore(QualityInspection inspection, ControlLimits controlLimits, ProcessCapabilityResult capability)
    {
        double score = 100.0;

        // Deduct points for specification violations
        if (inspection.SpecificationLimits != null)
        {
            if (inspection.MeasurementValue < inspection.SpecificationLimits.LowerLimit ||
                inspection.MeasurementValue > inspection.SpecificationLimits.UpperLimit)
            {
                score -= 50.0; // Major deduction for specification violation
            }
        }

        // Deduct points for control limit violations
        if (inspection.MeasurementValue < controlLimits.LowerControlLimit ||
            inspection.MeasurementValue > controlLimits.UpperControlLimit)
        {
            score -= 25.0; // Moderate deduction for control limit violation
        }

        // Adjust based on process capability
        if (capability.Cpk < 1.0)
        {
            score -= (1.0 - capability.Cpk) * 20.0; // Deduct based on capability shortfall
        }

        return Math.Max(0.0, Math.Min(100.0, score));
    }

    private IndividualGateResult EvaluateSpecificationCompliance(QualityInspection inspection)
    {
        if (inspection.SpecificationLimits == null)
        {
            return new IndividualGateResult
            {
                GateName = "Specification Compliance",
                Result = QualityGateResult.Pass,
                Message = "No specification limits defined"
            };
        }

        var isWithinSpec = inspection.MeasurementValue >= inspection.SpecificationLimits.LowerLimit &&
                          inspection.MeasurementValue <= inspection.SpecificationLimits.UpperLimit;

        return new IndividualGateResult
        {
            GateName = "Specification Compliance",
            Result = isWithinSpec ? QualityGateResult.Pass : QualityGateResult.Fail,
            Message = isWithinSpec ? "Within specification limits" : "Outside specification limits"
        };
    }

    private IndividualGateResult EvaluateStatisticalControl(SPCAnalysisResult spcAnalysis)
    {
        var hasViolations = spcAnalysis.ControlViolations.Any(v => v.ViolationType != ControlViolationType.None);

        return new IndividualGateResult
        {
            GateName = "Statistical Control",
            Result = hasViolations ? QualityGateResult.Fail : QualityGateResult.Pass,
            Message = hasViolations ? "Process out of statistical control" : "Process in statistical control"
        };
    }

    private IndividualGateResult EvaluateProcessCapability(ProcessCapabilityResult capability)
    {
        var result = capability.Cpk >= 1.33 ? QualityGateResult.Pass :
                    capability.Cpk >= 1.00 ? QualityGateResult.Warning :
                    QualityGateResult.Fail;

        var message = capability.Cpk >= 1.33 ? "Process highly capable" :
                     capability.Cpk >= 1.00 ? "Process marginally capable" :
                     "Process not capable";

        return new IndividualGateResult
        {
            GateName = "Process Capability",
            Result = result,
            Message = $"{message} (Cpk = {capability.Cpk:F2})"
        };
    }

    private async Task<IndividualGateResult> EvaluateQualityTrendsAsync(string partId, string operation)
    {
        // Get recent quality trend data
        var trendData = await GetQualityTrendDataAsync(partId, operation, TimeSpan.FromDays(7));

        if (trendData.Count < 5)
        {
            return new IndividualGateResult
            {
                GateName = "Quality Trends",
                Result = QualityGateResult.Pass,
                Message = "Insufficient data for trend analysis"
            };
        }

        // Simple trend analysis - check if quality is declining
        var recentQuality = trendData.TakeLast(3).Average(d => d.QualityScore);
        var historicalQuality = trendData.Take(trendData.Count - 3).Average(d => d.QualityScore);

        var trendDirection = recentQuality - historicalQuality;

        var result = trendDirection >= -5.0 ? QualityGateResult.Pass :
                    trendDirection >= -10.0 ? QualityGateResult.Warning :
                    QualityGateResult.Fail;

        var message = trendDirection >= -5.0 ? "Quality trends stable or improving" :
                     trendDirection >= -10.0 ? "Quality trends declining (caution)" :
                     "Quality trends significantly declining";

        return new IndividualGateResult
        {
            GateName = "Quality Trends",
            Result = result,
            Message = message
        };
    }

    private QualityGateResult DetermineOverallQualityGateResult(IList<IndividualGateResult> gateResults)
    {
        if (gateResults.Any(g => g.Result == QualityGateResult.Fail))
            return QualityGateResult.Fail;

        if (gateResults.Any(g => g.Result == QualityGateResult.Warning))
            return QualityGateResult.Warning;

        return QualityGateResult.Pass;
    }

    private async Task GenerateQualityAlertsAsync(QualityInspection inspection, SPCAnalysisResult spcAnalysis, QualityGateEvaluation qualityGate)
    {
        var alerts = new List<QualityAlert>();

        // Generate alerts based on quality gate failures
        foreach (var gateResult in qualityGate.IndividualGateResults)
        {
            if (gateResult.Result == QualityGateResult.Fail)
            {
                alerts.Add(new QualityAlert
                {
                    AlertType = QualityAlertType.QualityGateFailure,
                    Severity = AlertSeverity.High,
                    PartId = inspection.PartId,
                    Operation = inspection.Operation,
                    Message = $"Quality gate failure: {gateResult.GateName} - {gateResult.Message}",
                    CreatedAt = DateTime.Now
                });
            }
        }

        // Generate alerts based on SPC violations
        foreach (var violation in spcAnalysis.ControlViolations)
        {
            if (violation.ViolationType != ControlViolationType.None)
            {
                alerts.Add(new QualityAlert
                {
                    AlertType = QualityAlertType.StatisticalControlViolation,
                    Severity = DetermineSeverityFromViolationType(violation.ViolationType),
                    PartId = inspection.PartId,
                    Operation = inspection.Operation,
                    Message = $"SPC violation: {violation.ViolationType} at data point {violation.DataPointIndex}",
                    CreatedAt = DateTime.Now
                });
            }
        }

        // Send alerts
        foreach (var alert in alerts)
        {
            await SendQualityAlertAsync(alert);
        }
    }

    private async Task SendQualityAlertAsync(QualityAlert alert)
    {
        // Record alert in database
        await RecordQualityAlertAsync(alert);

        // Send alert message
        _messenger.Send(new QualityAlertMessage(alert));

        _logger.LogWarning("Quality alert generated: {AlertType} for part {PartId} at operation {Operation}", 
            alert.AlertType, alert.PartId, alert.Operation);
    }

    private async Task RecordQualityAlertAsync(QualityAlert alert)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_AlertType", alert.AlertType.ToString()),
            new("p_Severity", alert.Severity.ToString()),
            new("p_PartID", alert.PartId),
            new("p_OperationNumber", alert.Operation),
            new("p_Message", alert.Message),
            new("p_CreatedAt", alert.CreatedAt),
            new("p_CreatedBy", Environment.UserName)
        };

        await _databaseService.ExecuteStoredProcedureAsync("qms_alert_Add", parameters);
    }

    private async Task<bool> ValidatePartExistsAsync(string partId)
    {
        var parameters = new MySqlParameter[] { new("p_PartID", partId) };
        var result = await _databaseService.ExecuteStoredProcedureAsync("md_part_ids_Check_Exists", parameters);
        return result.Data.Rows.Count > 0;
    }

    private async Task<bool> ValidateOperationAsync(string operation)
    {
        var parameters = new MySqlParameter[] { new("p_OperationNumber", operation) };
        var result = await _databaseService.ExecuteStoredProcedureAsync("md_operations_Check_Exists", parameters);
        return result.Data.Rows.Count > 0;
    }

    private async Task<IList<QualityTrendPoint>> GetQualityTrendDataAsync(string partId, string operation, TimeSpan lookbackPeriod)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation),
            new("p_StartDate", DateTime.Now - lookbackPeriod),
            new("p_EndDate", DateTime.Now)
        };

        var result = await _databaseService.ExecuteStoredProcedureAsync("qms_quality_trends_Get", parameters);

        var trendData = new List<QualityTrendPoint>();
        foreach (DataRow row in result.Data.Rows)
        {
            trendData.Add(new QualityTrendPoint
            {
                Timestamp = Convert.ToDateTime(row["Timestamp"]),
                QualityScore = Convert.ToDouble(row["QualityScore"])
            });
        }

        return trendData.OrderBy(t => t.Timestamp).ToList();
    }

    private IList<string> GenerateRecommendedActions(SPCAnalysisResult spcAnalysis, QualityGateEvaluation qualityGate)
    {
        var recommendations = new List<string>();

        // Recommendations based on process capability
        if (spcAnalysis.ProcessCapability.Cpk < 1.0)
        {
            recommendations.Add("Investigate process variation reduction opportunities");
            recommendations.Add("Review process parameters and control settings");
        }

        // Recommendations based on control violations
        if (spcAnalysis.ControlViolations.Any(v => v.ViolationType == ControlViolationType.OutOfControlLimits))
        {
            recommendations.Add("Immediate process investigation required");
            recommendations.Add("Check equipment calibration and setup");
        }

        // Recommendations based on quality gate failures
        foreach (var gateResult in qualityGate.IndividualGateResults)
        {
            if (gateResult.Result == QualityGateResult.Fail)
            {
                switch (gateResult.GateName)
                {
                    case "Specification Compliance":
                        recommendations.Add("Review product specifications and tolerances");
                        break;
                    case "Process Capability":
                        recommendations.Add("Implement process improvement initiatives");
                        break;
                    case "Quality Trends":
                        recommendations.Add("Investigate root cause of quality decline");
                        break;
                }
            }
        }

        return recommendations.Count > 0 ? recommendations : new List<string> { "No specific actions recommended" };
    }

    private AlertSeverity DetermineSeverityFromViolationType(ControlViolationType violationType)
    {
        return violationType switch
        {
            ControlViolationType.OutOfControlLimits => AlertSeverity.Critical,
            ControlViolationType.SevenPointTrend => AlertSeverity.High,
            ControlViolationType.TwoOfThreeOutsideWarning => AlertSeverity.Medium,
            ControlViolationType.FourOfFiveOutsideOneSigma => AlertSeverity.Medium,
            _ => AlertSeverity.Low
        };
    }

    private async Task UpdateQualityMetricsAsync(QualityInspection inspection)
    {
        // Update quality metrics for the part and operation
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", inspection.PartId),
            new("p_OperationNumber", inspection.Operation),
            new("p_InspectionDate", inspection.InspectionDate),
            new("p_IsPass", inspection.MeasurementValue >= (inspection.SpecificationLimits?.LowerLimit ?? 0) &&
                          inspection.MeasurementValue <= (inspection.SpecificationLimits?.UpperLimit ?? double.MaxValue))
        };

        await _databaseService.ExecuteStoredProcedureAsync("qms_metrics_Update", parameters);
    }
}
```

### Statistical Process Control Service

```csharp
// Statistical Process Control implementation for manufacturing quality
public class StatisticalProcessControlService : IStatisticalProcessControlService
{
    private readonly ILogger<StatisticalProcessControlService> _logger;

    public StatisticalProcessControlService(ILogger<StatisticalProcessControlService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ControlLimits CalculateControlLimits(IEnumerable<double> data, double multiplier = 3.0)
    {
        var dataList = data.ToList();
        
        if (dataList.Count < 2)
        {
            throw new ArgumentException("At least 2 data points are required for control limit calculation");
        }

        var mean = dataList.Average();
        var standardDeviation = CalculateStandardDeviation(dataList, mean);

        return new ControlLimits
        {
            CenterLine = mean,
            UpperControlLimit = mean + (multiplier * standardDeviation),
            LowerControlLimit = mean - (multiplier * standardDeviation),
            UpperWarningLimit = mean + (2.0 * standardDeviation),
            LowerWarningLimit = mean - (2.0 * standardDeviation),
            StandardDeviation = standardDeviation
        };
    }

    public ProcessCapabilityResult AnalyzeProcessCapability(IEnumerable<double> data, SpecificationLimits? specLimits)
    {
        var dataList = data.ToList();
        
        if (dataList.Count < 10)
        {
            _logger.LogWarning("Insufficient data for reliable process capability analysis: {Count} points", dataList.Count);
        }

        if (specLimits == null)
        {
            return new ProcessCapabilityResult
            {
                Cp = 0,
                Cpk = 0,
                Pp = 0,
                Ppk = 0,
                Message = "No specification limits provided"
            };
        }

        var mean = dataList.Average();
        var standardDeviation = CalculateStandardDeviation(dataList, mean);

        // Calculate process capability indices
        var specRange = specLimits.UpperLimit - specLimits.LowerLimit;
        var processRange = 6 * standardDeviation;

        var cp = specRange / processRange;

        var cpkUpper = (specLimits.UpperLimit - mean) / (3 * standardDeviation);
        var cpkLower = (mean - specLimits.LowerLimit) / (3 * standardDeviation);
        var cpk = Math.Min(cpkUpper, cpkLower);

        // Calculate performance indices (using actual variation)
        var actualStdDev = CalculateActualStandardDeviation(dataList);
        var pp = specRange / (6 * actualStdDev);
        
        var ppkUpper = (specLimits.UpperLimit - mean) / (3 * actualStdDev);
        var ppkLower = (mean - specLimits.LowerLimit) / (3 * actualStdDev);
        var ppk = Math.Min(ppkUpper, ppkLower);

        return new ProcessCapabilityResult
        {
            Cp = cp,
            Cpk = cpk,
            Pp = pp,
            Ppk = ppk,
            ProcessMean = mean,
            ProcessStandardDeviation = standardDeviation,
            Message = GenerateCapabilityMessage(cp, cpk)
        };
    }

    public IList<ControlViolation> CheckControlViolations(IEnumerable<double> data, ControlLimits controlLimits)
    {
        var violations = new List<ControlViolation>();
        var dataList = data.ToList();

        for (int i = 0; i < dataList.Count; i++)
        {
            var value = dataList[i];

            // Check for out-of-control conditions
            if (value > controlLimits.UpperControlLimit || value < controlLimits.LowerControlLimit)
            {
                violations.Add(new ControlViolation
                {
                    ViolationType = ControlViolationType.OutOfControlLimits,
                    DataPointIndex = i,
                    Value = value,
                    Description = $"Point {i + 1} outside control limits ({value:F3})"
                });
            }
            
            // Check for warning limit violations
            else if (value > controlLimits.UpperWarningLimit || value < controlLimits.LowerWarningLimit)
            {
                violations.Add(new ControlViolation
                {
                    ViolationType = ControlViolationType.OutsideWarningLimits,
                    DataPointIndex = i,
                    Value = value,
                    Description = $"Point {i + 1} outside warning limits ({value:F3})"
                });
            }
        }

        // Check for trend patterns
        violations.AddRange(CheckTrendPatterns(dataList, controlLimits));

        return violations;
    }

    private IList<ControlViolation> CheckTrendPatterns(IList<double> data, ControlLimits controlLimits)
    {
        var violations = new List<ControlViolation>();

        // Check for 7 consecutive points on same side of center line
        violations.AddRange(CheckSevenPointTrend(data, controlLimits.CenterLine));

        // Check for 2 out of 3 consecutive points outside warning limits
        violations.AddRange(CheckTwoOfThreePattern(data, controlLimits));

        // Check for 4 out of 5 consecutive points outside 1-sigma
        violations.AddRange(CheckFourOfFivePattern(data, controlLimits));

        return violations;
    }

    private IList<ControlViolation> CheckSevenPointTrend(IList<double> data, double centerLine)
    {
        var violations = new List<ControlViolation>();

        for (int i = 6; i < data.Count; i++)
        {
            var consecutiveAbove = data.Skip(i - 6).Take(7).All(x => x > centerLine);
            var consecutiveBelow = data.Skip(i - 6).Take(7).All(x => x < centerLine);

            if (consecutiveAbove || consecutiveBelow)
            {
                violations.Add(new ControlViolation
                {
                    ViolationType = ControlViolationType.SevenPointTrend,
                    DataPointIndex = i,
                    Value = data[i],
                    Description = $"Seven consecutive points {(consecutiveAbove ? "above" : "below")} center line (ending at point {i + 1})"
                });
            }
        }

        return violations;
    }

    private IList<ControlViolation> CheckTwoOfThreePattern(IList<double> data, ControlLimits controlLimits)
    {
        var violations = new List<ControlViolation>();

        for (int i = 2; i < data.Count; i++)
        {
            var recentPoints = data.Skip(i - 2).Take(3).ToList();
            
            var outsideUpperWarning = recentPoints.Count(x => x > controlLimits.UpperWarningLimit);
            var outsideLowerWarning = recentPoints.Count(x => x < controlLimits.LowerWarningLimit);

            if (outsideUpperWarning >= 2 || outsideLowerWarning >= 2)
            {
                violations.Add(new ControlViolation
                {
                    ViolationType = ControlViolationType.TwoOfThreeOutsideWarning,
                    DataPointIndex = i,
                    Value = data[i],
                    Description = $"Two of three consecutive points outside warning limits (ending at point {i + 1})"
                });
            }
        }

        return violations;
    }

    private IList<ControlViolation> CheckFourOfFivePattern(IList<double> data, ControlLimits controlLimits)
    {
        var violations = new List<ControlViolation>();

        for (int i = 4; i < data.Count; i++)
        {
            var recentPoints = data.Skip(i - 4).Take(5).ToList();
            
            var oneSigmaUpper = controlLimits.CenterLine + controlLimits.StandardDeviation;
            var oneSigmaLower = controlLimits.CenterLine - controlLimits.StandardDeviation;
            
            var outsideOneSigmaUpper = recentPoints.Count(x => x > oneSigmaUpper);
            var outsideOneSigmaLower = recentPoints.Count(x => x < oneSigmaLower);

            if (outsideOneSigmaUpper >= 4 || outsideOneSigmaLower >= 4)
            {
                violations.Add(new ControlViolation
                {
                    ViolationType = ControlViolationType.FourOfFiveOutsideOneSigma,
                    DataPointIndex = i,
                    Value = data[i],
                    Description = $"Four of five consecutive points outside 1-sigma limits (ending at point {i + 1})"
                });
            }
        }

        return violations;
    }

    private double CalculateStandardDeviation(IList<double> data, double mean)
    {
        if (data.Count < 2) return 0;

        var sumSquaredDifferences = data.Sum(x => Math.Pow(x - mean, 2));
        return Math.Sqrt(sumSquaredDifferences / (data.Count - 1));
    }

    private double CalculateActualStandardDeviation(IList<double> data)
    {
        var mean = data.Average();
        return CalculateStandardDeviation(data, mean);
    }

    private string GenerateCapabilityMessage(double cp, double cpk)
    {
        if (cpk >= 1.67)
            return "Process is highly capable";
        else if (cpk >= 1.33)
            return "Process is adequately capable";
        else if (cpk >= 1.00)
            return "Process is marginally capable";
        else
            return "Process is not capable - improvement required";
    }
}
```

### Quality Management Data Models

```csharp
// Quality management data models for manufacturing quality control
namespace MTM_WIP_Application_Avalonia.Models
{
    public class QualityInspection
    {
        public string PartId { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string InspectionType { get; set; } = string.Empty; // "Incoming", "In-Process", "Final"
        public string MeasurementType { get; set; } = string.Empty; // "Dimensional", "Visual", "Functional"
        public double MeasurementValue { get; set; }
        public SpecificationLimits? SpecificationLimits { get; set; }
        public string InspectorId { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
    }

    public class SpecificationLimits
    {
        public double LowerLimit { get; set; }
        public double UpperLimit { get; set; }
        public double Target { get; set; }
    }

    public class QualityResult
    {
        public bool IsPass { get; set; }
        public double QualityScore { get; set; }
        public SPCAnalysisResult ControlChartAnalysis { get; set; } = new();
        public QualityGateEvaluation QualityGateEvaluation { get; set; } = new();
        public IList<string> RecommendedActions { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;

        public static QualityResult Failure(string message) =>
            new() { IsPass = false, Message = message };
    }

    public class SPCAnalysisResult
    {
        public ControlLimits ControlLimits { get; set; } = new();
        public ProcessCapabilityResult ProcessCapability { get; set; } = new();
        public IList<ControlViolation> ControlViolations { get; set; } = new List<ControlViolation>();
        public double QualityScore { get; set; }
        public int SampleSize { get; set; }
        public string RecommendedAction { get; set; } = string.Empty;
    }

    public class ControlLimits
    {
        public double UpperControlLimit { get; set; }
        public double LowerControlLimit { get; set; }
        public double UpperWarningLimit { get; set; }
        public double LowerWarningLimit { get; set; }
        public double CenterLine { get; set; }
        public double StandardDeviation { get; set; }
    }

    public class ProcessCapabilityResult
    {
        public double Cp { get; set; }  // Process capability index
        public double Cpk { get; set; } // Process capability index (accounts for centering)
        public double Pp { get; set; }  // Process performance index
        public double Ppk { get; set; } // Process performance index (accounts for centering)
        public double ProcessMean { get; set; }
        public double ProcessStandardDeviation { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ControlViolation
    {
        public ControlViolationType ViolationType { get; set; }
        public int DataPointIndex { get; set; }
        public double Value { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public enum ControlViolationType
    {
        None,
        OutOfControlLimits,
        OutsideWarningLimits,
        SevenPointTrend,
        TwoOfThreeOutsideWarning,
        FourOfFiveOutsideOneSigma
    }

    public class QualityGateEvaluation
    {
        public QualityGateResult OverallResult { get; set; }
        public IList<IndividualGateResult> IndividualGateResults { get; set; } = new List<IndividualGateResult>();
        public DateTime EvaluationTimestamp { get; set; }
        public string EvaluatedBy { get; set; } = string.Empty;
    }

    public class IndividualGateResult
    {
        public string GateName { get; set; } = string.Empty;
        public QualityGateResult Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public enum QualityGateResult
    {
        Pass,
        Warning,
        Fail
    }

    public class QualityAlert
    {
        public QualityAlertType AlertType { get; set; }
        public AlertSeverity Severity { get; set; }
        public string PartId { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public enum QualityAlertType
    {
        QualityGateFailure,
        StatisticalControlViolation,
        ProcessCapabilityIssue,
        QualityTrendDecline
    }

    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class QualityTrendPoint
    {
        public DateTime Timestamp { get; set; }
        public double QualityScore { get; set; }
    }

    public class QualityAuditRequest
    {
        public string AuditScope { get; set; } = string.Empty; // "Process", "Product", "System"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AuditorId { get; set; } = string.Empty;
        public IList<string> AreasToAudit { get; set; } = new List<string>();
    }

    public class QualityAuditResult
    {
        public string AuditId { get; set; } = string.Empty;
        public string AuditScope { get; set; } = string.Empty;
        public DateTime AuditDate { get; set; }
        public string AuditorId { get; set; } = string.Empty;
        public double OverallScore { get; set; }
        public IList<AuditFinding> Findings { get; set; } = new List<AuditFinding>();
        public IList<NonConformance> NonConformances { get; set; } = new List<NonConformance>();
    }

    public class AuditFinding
    {
        public string Area { get; set; } = string.Empty;
        public string Finding { get; set; } = string.Empty;
        public FindingSeverity Severity { get; set; }
    }

    public class NonConformance
    {
        public string Description { get; set; } = string.Empty;
        public NonConformanceSeverity Severity { get; set; }
        public string RootCause { get; set; } = string.Empty;
        public string CorrectiveAction { get; set; } = string.Empty;
        public DateTime TargetCompletionDate { get; set; }
    }

    public enum FindingSeverity
    {
        Observation,
        Minor,
        Major,
        Critical
    }

    public enum NonConformanceSeverity
    {
        Minor,
        Major,
        Critical
    }
}
```

### Quality Management Messages

```csharp
// MVVM Community Toolkit messages for quality management system
namespace MTM_WIP_Application_Avalonia.Messages
{
    public record QualityDataUpdatedMessage(
        string PartId,
        string Operation,
        QualityGateResult Result,
        DateTime Timestamp);

    public record QualityAlertMessage(QualityAlert Alert);

    public record QualityAuditCompletedMessage(
        string AuditId,
        int NonConformanceCount,
        double OverallScore,
        DateTime CompletionDate);
}
```

---

## 📚 Related Documentation

- **Manufacturing KPI Dashboard**: [KPI Integration Patterns](./manufacturing-kpi-dashboard-integration.instructions.md)
- **Service Integration**: [Cross-Service Communication](./service-integration.instructions.md)
- **Database Integration**: [Advanced Database Patterns](./database-integration.instructions.md)
- **MVVM Patterns**: [Complex ViewModel Implementation](./mvvm-community-toolkit.instructions.md)

---

**Document Status**: ✅ Complete Quality Management System Integration Reference  
**Framework Version**: .NET 8 with Quality Control Integration  
**Last Updated**: 2025-09-15  
**Quality Management Owner**: MTM Development Team

## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**
- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**
- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**
- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**
- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**
- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**
- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**
- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.
