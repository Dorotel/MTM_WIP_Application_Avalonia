# Advanced Manufacturing Quality Assurance Framework - MTM WIP Application Instructions

**Framework**: .NET 8 with Comprehensive Quality Management  
**Pattern**: Manufacturing-Grade Quality Assurance and Control  
**Created**: 2025-09-15  

---

## 🎯 Overview

Advanced quality assurance framework for MTM WIP Application, implementing manufacturing-grade quality control, statistical process control, and comprehensive quality management systems integration.

## 🏭 Manufacturing Quality Management Architecture

### Quality Management System Integration

```csharp
// Comprehensive quality management service with advanced QA capabilities
public class AdvancedQualityManagementService : IAdvancedQualityManagementService
{
    private readonly IQualityDataService _qualityDataService;
    private readonly IStatisticalProcessControlService _spcService;
    private readonly IQualityAuditService _auditService;
    private readonly ISupplierQualityService _supplierQualityService;
    private readonly ICustomerQualityService _customerQualityService;
    private readonly IComplianceManagementService _complianceService;
    private readonly IQualityTrainingService _trainingService;
    private readonly IMessenger _messenger;
    private readonly ILogger<AdvancedQualityManagementService> _logger;

    public AdvancedQualityManagementService(
        IQualityDataService qualityDataService,
        IStatisticalProcessControlService spcService,
        IQualityAuditService auditService,
        ISupplierQualityService supplierQualityService,
        ICustomerQualityService customerQualityService,
        IComplianceManagementService complianceService,
        IQualityTrainingService trainingService,
        IMessenger messenger,
        ILogger<AdvancedQualityManagementService> logger)
    {
        _qualityDataService = qualityDataService;
        _spcService = spcService;
        _auditService = auditService;
        _supplierQualityService = supplierQualityService;
        _customerQualityService = customerQualityService;
        _complianceService = complianceService;
        _trainingService = trainingService;
        _messenger = messenger;
        _logger = logger;
    }

    public async Task<QualityAssessmentResult> PerformComprehensiveQualityAssessmentAsync(
        QualityAssessmentRequest request)
    {
        try
        {
            _logger.LogInformation("Starting comprehensive quality assessment for {PartId} at operation {Operation}",
                request.PartId, request.Operation);

            var assessmentTasks = new List<Task>
            {
                // Statistical Process Control Analysis
                PerformSPCAnalysisAsync(request),
                
                // Supplier Quality Validation
                ValidateSupplierQualityAsync(request),
                
                // Customer Requirements Compliance
                ValidateCustomerRequirementsAsync(request),
                
                // Regulatory Compliance Check
                ValidateRegulatoryComplianceAsync(request),
                
                // Historical Quality Trend Analysis
                AnalyzeQualityTrendsAsync(request),
                
                // Process Capability Analysis
                AnalyzeProcessCapabilityAsync(request)
            };

            await Task.WhenAll(assessmentTasks);

            // Compile comprehensive assessment results
            var result = await CompileAssessmentResultsAsync(request);

            // Generate quality alerts if needed
            await ProcessQualityAlertsAsync(result);

            // Update quality metrics
            await UpdateQualityMetricsAsync(result);

            // Store assessment results
            await StoreAssessmentResultsAsync(result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Comprehensive quality assessment failed for {PartId}", request.PartId);
            throw;
        }
    }

    private async Task<QualityAssessmentResult> PerformSPCAnalysisAsync(QualityAssessmentRequest request)
    {
        var measurementData = await _qualityDataService.GetMeasurementDataAsync(
            request.PartId, request.Operation, request.TimeRange);

        var spcResult = await _spcService.PerformControlChartAnalysisAsync(new SPCAnalysisRequest
        {
            PartId = request.PartId,
            Operation = request.Operation,
            MeasurementData = measurementData,
            ControlLimits = await GetControlLimitsAsync(request.PartId, request.Operation),
            SpecificationLimits = await GetSpecificationLimitsAsync(request.PartId)
        });

        return new QualityAssessmentResult
        {
            AssessmentType = QualityAssessmentType.StatisticalProcessControl,
            PartId = request.PartId,
            Operation = request.Operation,
            IsInControl = spcResult.ProcessInControl,
            ProcessCapability = spcResult.ProcessCapabilityMetrics,
            ControlChartData = spcResult.ControlChartResults,
            Recommendations = spcResult.ProcessImprovementRecommendations,
            QualityScore = CalculateQualityScore(spcResult),
            AssessmentDate = DateTime.UtcNow
        };
    }

    private async Task ValidateSupplierQualityAsync(QualityAssessmentRequest request)
    {
        var supplierInfo = await _supplierQualityService.GetSupplierQualityDataAsync(request.PartId);
        
        var supplierValidation = new SupplierQualityValidation
        {
            SupplierRating = supplierInfo.QualityRating,
            CertificationStatus = supplierInfo.CertificationStatus,
            RecentDefectRate = supplierInfo.RecentDefectRate,
            OnTimeDeliveryRate = supplierInfo.OnTimeDeliveryRate,
            QualitySystemMaturity = supplierInfo.QualitySystemMaturity
        };

        if (supplierValidation.RecentDefectRate > 0.01) // >1% defect rate
        {
            await _messenger.Send(new SupplierQualityAlertMessage
            {
                PartId = request.PartId,
                SupplierId = supplierInfo.SupplierId,
                AlertType = "HIGH_DEFECT_RATE",
                DefectRate = supplierValidation.RecentDefectRate,
                Severity = AlertSeverity.High
            });
        }

        await StoreSupplierQualityValidationAsync(request.PartId, supplierValidation);
    }

    private async Task ValidateCustomerRequirementsAsync(QualityAssessmentRequest request)
    {
        var customerRequirements = await _customerQualityService.GetCustomerRequirementsAsync(request.PartId);
        
        foreach (var requirement in customerRequirements)
        {
            var complianceResult = await ValidateRequirementComplianceAsync(request, requirement);
            
            if (!complianceResult.IsCompliant)
            {
                await _messenger.Send(new CustomerRequirementNonComplianceMessage
                {
                    PartId = request.PartId,
                    CustomerId = requirement.CustomerId,
                    RequirementId = requirement.RequirementId,
                    RequirementDescription = requirement.Description,
                    NonComplianceDetails = complianceResult.NonComplianceDetails,
                    Severity = requirement.CriticalityLevel
                });
            }
        }
    }

    private async Task ValidateRegulatoryComplianceAsync(QualityAssessmentRequest request)
    {
        var applicableRegulations = await _complianceService.GetApplicableRegulationsAsync(
            request.PartId, request.Operation);

        foreach (var regulation in applicableRegulations)
        {
            var complianceStatus = await _complianceService.ValidateComplianceAsync(
                request, regulation);

            if (complianceStatus.ComplianceLevel != ComplianceLevel.FullyCompliant)
            {
                await _messenger.Send(new RegulatoryComplianceAlertMessage
                {
                    PartId = request.PartId,
                    RegulationId = regulation.RegulationId,
                    RegulationName = regulation.Name,
                    ComplianceLevel = complianceStatus.ComplianceLevel,
                    RequiredActions = complianceStatus.RequiredCorrectiveActions,
                    DeadlineDate = complianceStatus.ComplianceDeadline,
                    Severity = DetermineComplianceSeverity(complianceStatus.ComplianceLevel)
                });
            }
        }
    }

    private async Task AnalyzeQualityTrendsAsync(QualityAssessmentRequest request)
    {
        var historicalData = await _qualityDataService.GetHistoricalQualityDataAsync(
            request.PartId, request.Operation, TimeSpan.FromDays(90));

        var trendAnalysis = await _spcService.PerformTrendAnalysisAsync(new TrendAnalysisRequest
        {
            PartId = request.PartId,
            Operation = request.Operation,
            HistoricalData = historicalData,
            AnalysisPeriod = TimeSpan.FromDays(90),
            TrendDetectionSensitivity = TrendSensitivity.High
        });

        if (trendAnalysis.HasNegativeTrend)
        {
            await _messenger.Send(new QualityTrendAlertMessage
            {
                PartId = request.PartId,
                Operation = request.Operation,
                TrendType = trendAnalysis.TrendType,
                TrendSeverity = trendAnalysis.TrendSeverity,
                ProjectedImpact = trendAnalysis.ProjectedQualityImpact,
                RecommendedActions = trendAnalysis.RecommendedCorrectiveActions
            });
        }
    }

    private async Task AnalyzeProcessCapabilityAsync(QualityAssessmentRequest request)
    {
        var processCapabilityData = await _qualityDataService.GetProcessCapabilityDataAsync(
            request.PartId, request.Operation);

        var capabilityAnalysis = await _spcService.PerformProcessCapabilityAnalysisAsync(
            new ProcessCapabilityRequest
            {
                PartId = request.PartId,
                Operation = request.Operation,
                MeasurementData = processCapabilityData.Measurements,
                SpecificationLimits = processCapabilityData.SpecificationLimits,
                ToleranceAnalysisRequired = true
            });

        // Process capability indices (Cp, Cpk, Pp, Ppk)
        if (capabilityAnalysis.Cpk < 1.33) // Below industry standard
        {
            await _messenger.Send(new ProcessCapabilityAlertMessage
            {
                PartId = request.PartId,
                Operation = request.Operation,
                CapabilityIndex = capabilityAnalysis.Cpk,
                RequiredCapabilityIndex = 1.33,
                ProcessSigmaLevel = capabilityAnalysis.SigmaLevel,
                ImprovementRecommendations = capabilityAnalysis.ProcessImprovementActions,
                Severity = capabilityAnalysis.Cpk < 1.0 ? AlertSeverity.Critical : AlertSeverity.High
            });
        }
    }

    private async Task<QualityAssessmentResult> CompileAssessmentResultsAsync(QualityAssessmentRequest request)
    {
        // Retrieve all assessment components
        var spcResults = await GetSPCAssessmentResultsAsync(request);
        var supplierResults = await GetSupplierAssessmentResultsAsync(request);
        var customerResults = await GetCustomerComplianceResultsAsync(request);
        var regulatoryResults = await GetRegulatoryComplianceResultsAsync(request);
        var trendResults = await GetTrendAnalysisResultsAsync(request);
        var capabilityResults = await GetProcessCapabilityResultsAsync(request);

        // Calculate composite quality score
        var compositeScore = CalculateCompositeQualityScore(
            spcResults, supplierResults, customerResults, regulatoryResults, trendResults, capabilityResults);

        // Determine overall quality status
        var overallStatus = DetermineOverallQualityStatus(compositeScore, spcResults, regulatoryResults);

        return new QualityAssessmentResult
        {
            AssessmentId = Guid.NewGuid().ToString(),
            PartId = request.PartId,
            Operation = request.Operation,
            AssessmentDate = DateTime.UtcNow,
            OverallQualityScore = compositeScore,
            OverallQualityStatus = overallStatus,
            SPCResults = spcResults,
            SupplierQualityResults = supplierResults,
            CustomerComplianceResults = customerResults,
            RegulatoryComplianceResults = regulatoryResults,
            QualityTrendResults = trendResults,
            ProcessCapabilityResults = capabilityResults,
            ConsolidatedRecommendations = await GenerateConsolidatedRecommendationsAsync(
                spcResults, supplierResults, customerResults, regulatoryResults, trendResults, capabilityResults),
            NextReviewDate = CalculateNextReviewDate(overallStatus, compositeScore)
        };
    }

    private double CalculateCompositeQualityScore(params IQualityAssessmentComponent[] components)
    {
        var weightedScores = components.Select(component => new
        {
            Score = component.QualityScore,
            Weight = component.AssessmentWeight
        });

        var totalWeight = weightedScores.Sum(ws => ws.Weight);
        var weightedSum = weightedScores.Sum(ws => ws.Score * ws.Weight);

        return totalWeight > 0 ? weightedSum / totalWeight : 0;
    }

    private QualityStatus DetermineOverallQualityStatus(double compositeScore, params IQualityAssessmentComponent[] criticalComponents)
    {
        // Check for critical failures first
        if (criticalComponents.Any(c => c.HasCriticalIssues))
            return QualityStatus.Critical;

        return compositeScore switch
        {
            >= 95 => QualityStatus.Excellent,
            >= 90 => QualityStatus.Good,
            >= 80 => QualityStatus.Acceptable,
            >= 70 => QualityStatus.NeedsImprovement,
            _ => QualityStatus.Unacceptable
        };
    }

    private async Task ProcessQualityAlertsAsync(QualityAssessmentResult result)
    {
        var alertParameters = new MySqlParameter[]
        {
            new("p_PartID", result.PartId),
            new("p_Operation", result.Operation),
            new("p_QualityScore", result.OverallQualityScore),
            new("p_QualityStatus", result.OverallQualityStatus.ToString()),
            new("p_AlertLevel", DetermineAlertLevel(result.OverallQualityStatus)),
            new("p_AssessmentDate", result.AssessmentDate),
            new("p_Recommendations", JsonSerializer.Serialize(result.ConsolidatedRecommendations))
        };

        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "qa_quality_alert_Process", alertParameters);

        // Send real-time alerts for critical issues
        if (result.OverallQualityStatus == QualityStatus.Critical || result.OverallQualityScore < 70)
        {
            await _messenger.Send(new CriticalQualityAlertMessage
            {
                PartId = result.PartId,
                Operation = result.Operation,
                QualityScore = result.OverallQualityScore,
                CriticalIssues = result.GetCriticalIssues(),
                ImmediateActions = result.GetImmediateActions(),
                AlertTime = DateTime.UtcNow,
                Severity = AlertSeverity.Critical
            });
        }
    }

    private async Task UpdateQualityMetricsAsync(QualityAssessmentResult result)
    {
        var metricsParameters = new MySqlParameter[]
        {
            new("p_PartID", result.PartId),
            new("p_Operation", result.Operation),
            new("p_QualityScore", result.OverallQualityScore),
            new("p_ProcessCapability", result.ProcessCapabilityResults?.Cpk ?? 0),
            new("p_DefectRate", result.CalculateDefectRate()),
            new("p_ComplianceRate", result.CalculateComplianceRate()),
            new("p_SupplierRating", result.SupplierQualityResults?.OverallRating ?? 0),
            new("p_CustomerSatisfaction", result.CustomerComplianceResults?.SatisfactionScore ?? 0),
            new("p_UpdateDate", DateTime.UtcNow)
        };

        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "qa_quality_metrics_Update", metricsParameters);
    }

    private async Task StoreAssessmentResultsAsync(QualityAssessmentResult result)
    {
        var storageParameters = new MySqlParameter[]
        {
            new("p_AssessmentId", result.AssessmentId),
            new("p_PartID", result.PartId),
            new("p_Operation", result.Operation),
            new("p_AssessmentData", JsonSerializer.Serialize(result)),
            new("p_QualityScore", result.OverallQualityScore),
            new("p_QualityStatus", result.OverallQualityStatus.ToString()),
            new("p_AssessmentDate", result.AssessmentDate),
            new("p_NextReviewDate", result.NextReviewDate)
        };

        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "qa_assessment_results_Store", storageParameters);
    }

    private DateTime CalculateNextReviewDate(QualityStatus status, double qualityScore)
    {
        var reviewInterval = status switch
        {
            QualityStatus.Critical => TimeSpan.FromHours(4),
            QualityStatus.Unacceptable => TimeSpan.FromHours(8),
            QualityStatus.NeedsImprovement => TimeSpan.FromDays(1),
            QualityStatus.Acceptable => TimeSpan.FromDays(3),
            QualityStatus.Good => TimeSpan.FromDays(7),
            QualityStatus.Excellent => TimeSpan.FromDays(14),
            _ => TimeSpan.FromDays(7)
        };

        return DateTime.UtcNow.Add(reviewInterval);
    }
}
```

## 🔬 Statistical Process Control (SPC) Implementation

### Advanced SPC Analysis Engine

```csharp
// Advanced Statistical Process Control implementation for manufacturing
public class StatisticalProcessControlService : IStatisticalProcessControlService
{
    private readonly ILogger<StatisticalProcessControlService> _logger;
    private readonly IQualityDataRepository _dataRepository;
    private readonly IMathematicalAnalysisService _mathService;
    private readonly IControlChartService _controlChartService;

    public StatisticalProcessControlService(
        ILogger<StatisticalProcessControlService> logger,
        IQualityDataRepository dataRepository,
        IMathematicalAnalysisService mathService,
        IControlChartService controlChartService)
    {
        _logger = logger;
        _dataRepository = dataRepository;
        _mathService = mathService;
        _controlChartService = controlChartService;
    }

    public async Task<SPCAnalysisResult> PerformControlChartAnalysisAsync(SPCAnalysisRequest request)
    {
        try
        {
            _logger.LogInformation("Performing SPC analysis for {PartId} operation {Operation}",
                request.PartId, request.Operation);

            // Validate measurement data
            ValidateMeasurementData(request.MeasurementData);

            // Perform multiple control chart analyses
            var analysisResults = new List<ControlChartResult>
            {
                // X-bar and R charts (for variable data)
                await PerformXBarRChartAnalysisAsync(request),
                
                // X-bar and S charts (for larger subgroups)
                await PerformXBarSChartAnalysisAsync(request),
                
                // Individual and Moving Range charts
                await PerformIndividualMovingRangeAnalysisAsync(request),
                
                // p-chart for proportion of defectives
                await PerformPChartAnalysisAsync(request),
                
                // np-chart for number of defectives
                await PerformNPChartAnalysisAsync(request),
                
                // c-chart for number of defects
                await PerformCChartAnalysisAsync(request),
                
                // u-chart for defects per unit
                await PerformUChartAnalysisAsync(request)
            };

            // Process capability analysis
            var processCapability = await PerformProcessCapabilityAnalysisAsync(request);

            // Statistical analysis
            var statisticalSummary = PerformStatisticalAnalysis(request.MeasurementData);

            // Out-of-control point analysis
            var outOfControlAnalysis = PerformOutOfControlAnalysis(analysisResults);

            // Trend analysis
            var trendAnalysis = await PerformAdvancedTrendAnalysisAsync(request.MeasurementData);

            // Pattern recognition
            var patternAnalysis = PerformPatternRecognition(request.MeasurementData);

            return new SPCAnalysisResult
            {
                PartId = request.PartId,
                Operation = request.Operation,
                AnalysisDate = DateTime.UtcNow,
                ControlChartResults = analysisResults,
                ProcessCapabilityMetrics = processCapability,
                StatisticalSummary = statisticalSummary,
                OutOfControlAnalysis = outOfControlAnalysis,
                TrendAnalysis = trendAnalysis,
                PatternAnalysis = patternAnalysis,
                ProcessInControl = DetermineProcessControlStatus(analysisResults, outOfControlAnalysis),
                ProcessImprovementRecommendations = GenerateProcessImprovementRecommendations(
                    analysisResults, processCapability, trendAnalysis, patternAnalysis)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SPC analysis failed for {PartId} operation {Operation}",
                request.PartId, request.Operation);
            throw;
        }
    }

    private async Task<ControlChartResult> PerformXBarRChartAnalysisAsync(SPCAnalysisRequest request)
    {
        var subgroups = GroupMeasurementsIntoSubgroups(request.MeasurementData, subgroupSize: 5);
        
        var xBarValues = subgroups.Select(sg => sg.Average()).ToList();
        var rangeValues = subgroups.Select(sg => sg.Max() - sg.Min()).ToList();
        
        // Calculate control limits for X-bar chart
        var xBarMean = xBarValues.Average();
        var rBarMean = rangeValues.Average();
        var d2 = GetD2Constant(subgroups.First().Count); // From SPC tables
        var a2 = GetA2Constant(subgroups.First().Count); // From SPC tables
        
        var xBarUCL = xBarMean + (a2 * rBarMean);
        var xBarLCL = xBarMean - (a2 * rBarMean);
        
        // Calculate control limits for R chart
        var d3 = GetD3Constant(subgroups.First().Count);
        var d4 = GetD4Constant(subgroups.First().Count);
        var rUCL = d4 * rBarMean;
        var rLCL = d3 * rBarMean;
        
        // Identify out-of-control points
        var xBarOutOfControlPoints = IdentifyOutOfControlPoints(xBarValues, xBarUCL, xBarLCL, xBarMean);
        var rOutOfControlPoints = IdentifyOutOfControlPoints(rangeValues, rUCL, rLCL, rBarMean);
        
        return new ControlChartResult
        {
            ChartType = ControlChartType.XBarR,
            CenterLine = xBarMean,
            UpperControlLimit = xBarUCL,
            LowerControlLimit = xBarLCL,
            DataPoints = xBarValues,
            OutOfControlPoints = xBarOutOfControlPoints.Concat(rOutOfControlPoints).ToList(),
            IsInControl = !xBarOutOfControlPoints.Any() && !rOutOfControlPoints.Any(),
            RangeChartData = new RangeChartData
            {
                CenterLine = rBarMean,
                UpperControlLimit = rUCL,
                LowerControlLimit = rLCL,
                DataPoints = rangeValues,
                OutOfControlPoints = rOutOfControlPoints
            }
        };
    }

    private async Task<ProcessCapabilityMetrics> PerformProcessCapabilityAnalysisAsync(SPCAnalysisRequest request)
    {
        var measurements = request.MeasurementData.Select(m => m.Value).ToList();
        var specLimits = request.SpecificationLimits;
        
        if (specLimits == null)
        {
            throw new ArgumentException("Specification limits are required for process capability analysis");
        }
        
        var mean = measurements.Average();
        var standardDeviation = _mathService.CalculateStandardDeviation(measurements);
        
        // Process capability indices
        var cp = (specLimits.UpperLimit - specLimits.LowerLimit) / (6 * standardDeviation);
        var cpk = Math.Min(
            (specLimits.UpperLimit - mean) / (3 * standardDeviation),
            (mean - specLimits.LowerLimit) / (3 * standardDeviation)
        );
        
        // Performance indices (using overall variation)
        var overallStdDev = _mathService.CalculateOverallStandardDeviation(measurements);
        var pp = (specLimits.UpperLimit - specLimits.LowerLimit) / (6 * overallStdDev);
        var ppk = Math.Min(
            (specLimits.UpperLimit - mean) / (3 * overallStdDev),
            (mean - specLimits.LowerLimit) / (3 * overallStdDev)
        );
        
        // Sigma level calculation
        var sigmaLevel = Math.Min(
            (specLimits.UpperLimit - mean) / standardDeviation,
            (mean - specLimits.LowerLimit) / standardDeviation
        );
        
        // Defect rate calculation
        var defectRate = CalculateDefectRate(measurements, specLimits);
        
        // Parts per million (PPM) defective
        var ppmDefective = defectRate * 1_000_000;
        
        return new ProcessCapabilityMetrics
        {
            Cp = cp,
            Cpk = cpk,
            Pp = pp,
            Ppk = ppk,
            SigmaLevel = sigmaLevel,
            DefectRate = defectRate,
            PPMDefective = ppmDefective,
            ProcessMean = mean,
            ProcessStandardDeviation = standardDeviation,
            OverallStandardDeviation = overallStdDev,
            CapabilityAssessment = AssessProcessCapability(cpk),
            ImprovementPotential = CalculateImprovementPotential(cp, cpk)
        };
    }

    private OutOfControlAnalysis PerformOutOfControlAnalysis(List<ControlChartResult> controlChartResults)
    {
        var allOutOfControlPoints = controlChartResults.SelectMany(ccr => ccr.OutOfControlPoints).ToList();
        
        var analysis = new OutOfControlAnalysis
        {
            TotalOutOfControlPoints = allOutOfControlPoints.Count,
            OutOfControlPointsByChart = controlChartResults.ToDictionary(
                ccr => ccr.ChartType,
                ccr => ccr.OutOfControlPoints.Count),
            OutOfControlPatterns = IdentifyOutOfControlPatterns(allOutOfControlPoints),
            MostCommonCauses = IdentifyMostCommonCauses(allOutOfControlPoints),
            RecommendedActions = GenerateOutOfControlActions(allOutOfControlPoints)
        };

        // Apply Nelson rules for additional pattern detection
        foreach (var chartResult in controlChartResults)
        {
            var nelsonRuleViolations = ApplyNelsonRules(chartResult);
            analysis.NelsonRuleViolations.AddRange(nelsonRuleViolations);
        }

        return analysis;
    }

    private List<NelsonRuleViolation> ApplyNelsonRules(ControlChartResult chartResult)
    {
        var violations = new List<NelsonRuleViolation>();
        var dataPoints = chartResult.DataPoints;
        var centerLine = chartResult.CenterLine;
        var ucl = chartResult.UpperControlLimit;
        var lcl = chartResult.LowerControlLimit;
        
        var sigma = (ucl - centerLine) / 3; // Approximation
        
        // Nelson Rule 1: One point beyond ±3σ
        violations.AddRange(ApplyNelsonRule1(dataPoints, ucl, lcl));
        
        // Nelson Rule 2: Nine points in a row on same side of center line
        violations.AddRange(ApplyNelsonRule2(dataPoints, centerLine));
        
        // Nelson Rule 3: Six points in a row steadily increasing or decreasing
        violations.AddRange(ApplyNelsonRule3(dataPoints));
        
        // Nelson Rule 4: Fourteen points in a row alternating up and down
        violations.AddRange(ApplyNelsonRule4(dataPoints));
        
        // Nelson Rule 5: Two out of three points beyond ±2σ
        violations.AddRange(ApplyNelsonRule5(dataPoints, centerLine, sigma));
        
        // Nelson Rule 6: Four out of five points beyond ±1σ
        violations.AddRange(ApplyNelsonRule6(dataPoints, centerLine, sigma));
        
        // Nelson Rule 7: Fifteen points in a row within ±1σ
        violations.AddRange(ApplyNelsonRule7(dataPoints, centerLine, sigma));
        
        // Nelson Rule 8: Eight points in a row beyond ±1σ (on both sides)
        violations.AddRange(ApplyNelsonRule8(dataPoints, centerLine, sigma));
        
        return violations;
    }

    private CapabilityAssessment AssessProcessCapability(double cpk)
    {
        return cpk switch
        {
            >= 2.0 => CapabilityAssessment.Excellent,
            >= 1.67 => CapabilityAssessment.VeryGood,
            >= 1.33 => CapabilityAssessment.Good,
            >= 1.0 => CapabilityAssessment.Adequate,
            >= 0.67 => CapabilityAssessment.Poor,
            _ => CapabilityAssessment.Unacceptable
        };
    }

    private double CalculateImprovementPotential(double cp, double cpk)
    {
        // If Cp and Cpk are similar, process is centered but may need variation reduction
        // If Cp >> Cpk, process is off-center and needs centering
        var centeringImprovement = cp - cpk;
        var variationImprovement = (2.0 - cp) / cp; // Improvement needed to reach Cp = 2.0
        
        return Math.Max(centeringImprovement, variationImprovement);
    }

    private List<ProcessImprovementRecommendation> GenerateProcessImprovementRecommendations(
        List<ControlChartResult> controlChartResults,
        ProcessCapabilityMetrics capability,
        TrendAnalysisResult trendAnalysis,
        PatternAnalysisResult patternAnalysis)
    {
        var recommendations = new List<ProcessImprovementRecommendation>();
        
        // Capability-based recommendations
        if (capability.Cpk < 1.33)
        {
            recommendations.Add(new ProcessImprovementRecommendation
            {
                Priority = RecommendationPriority.High,
                Category = "Process Capability",
                Issue = $"Process capability (Cpk = {capability.Cpk:F2}) is below industry standard (1.33)",
                Recommendation = capability.Cp > capability.Cpk 
                    ? "Focus on process centering - adjust process mean to target value"
                    : "Focus on variation reduction - identify and eliminate sources of variation",
                ExpectedImpact = "Improve process capability and reduce defect rate",
                ImplementationTimeframe = "2-4 weeks"
            });
        }
        
        // Control chart based recommendations
        if (controlChartResults.Any(ccr => !ccr.IsInControl))
        {
            recommendations.Add(new ProcessImprovementRecommendation
            {
                Priority = RecommendationPriority.Critical,
                Category = "Statistical Control",
                Issue = "Process is out of statistical control",
                Recommendation = "Identify and eliminate special causes of variation using fishbone diagram and 5-why analysis",
                ExpectedImpact = "Bring process into statistical control and improve predictability",
                ImplementationTimeframe = "Immediate"
            });
        }
        
        // Trend-based recommendations
        if (trendAnalysis.HasNegativeTrend)
        {
            recommendations.Add(new ProcessImprovementRecommendation
            {
                Priority = RecommendationPriority.High,
                Category = "Process Trends",
                Issue = $"Negative quality trend detected: {trendAnalysis.TrendDescription}",
                Recommendation = "Investigate root causes of trend deterioration and implement corrective actions",
                ExpectedImpact = "Prevent further quality degradation and improve trend direction",
                ImplementationTimeframe = "1-2 weeks"
            });
        }
        
        // Pattern-based recommendations
        if (patternAnalysis.HasSignificantPatterns)
        {
            recommendations.Add(new ProcessImprovementRecommendation
            {
                Priority = RecommendationPriority.Medium,
                Category = "Process Patterns",
                Issue = $"Significant patterns detected: {string.Join(", ", patternAnalysis.IdentifiedPatterns)}",
                Recommendation = "Analyze identified patterns to understand underlying process behavior and optimize accordingly",
                ExpectedImpact = "Optimize process performance by leveraging identified patterns",
                ImplementationTimeframe = "3-6 weeks"
            });
        }
        
        return recommendations.OrderBy(r => r.Priority).ToList();
    }
}
```

## 🔍 Advanced Quality Audit System

### Comprehensive Quality Audit Framework

```csharp
// Advanced quality audit system with comprehensive audit capabilities
public class QualityAuditService : IQualityAuditService
{
    private readonly ILogger<QualityAuditService> _logger;
    private readonly IAuditPlanningService _auditPlanning;
    private readonly IAuditExecutionService _auditExecution;
    private readonly INonConformanceService _nonConformanceService;
    private readonly ICorrectiveActionService _correctiveActionService;
    private readonly IComplianceTrackingService _complianceTracking;

    public QualityAuditService(
        ILogger<QualityAuditService> logger,
        IAuditPlanningService auditPlanning,
        IAuditExecutionService auditExecution,
        INonConformanceService nonConformanceService,
        ICorrectiveActionService correctiveActionService,
        IComplianceTrackingService complianceTracking)
    {
        _logger = logger;
        _auditPlanning = auditPlanning;
        _auditExecution = auditExecution;
        _nonConformanceService = nonConformanceService;
        _correctiveActionService = correctiveActionService;
        _complianceTracking = complianceTracking;
    }

    public async Task<QualityAuditResult> PerformComprehensiveAuditAsync(QualityAuditRequest request)
    {
        try
        {
            _logger.LogInformation("Starting comprehensive quality audit for {AuditScope}", request.AuditScope);

            // Phase 1: Audit Planning and Preparation
            var auditPlan = await _auditPlanning.CreateAuditPlanAsync(request);
            
            // Phase 2: Audit Execution
            var auditFindings = await _auditExecution.ExecuteAuditAsync(auditPlan);
            
            // Phase 3: Non-Conformance Identification and Classification
            var nonConformances = await _nonConformanceService.IdentifyNonConformancesAsync(auditFindings);
            
            // Phase 4: Risk Assessment of Findings
            var riskAssessment = await AssessAuditRisksAsync(nonConformances);
            
            // Phase 5: Corrective Action Planning
            var correctiveActions = await _correctiveActionService.PlanCorrectiveActionsAsync(nonConformances);
            
            // Phase 6: Compliance Gap Analysis
            var complianceGaps = await _complianceTracking.AnalyzeComplianceGapsAsync(auditFindings);
            
            // Phase 7: Audit Report Generation
            var auditReport = await GenerateComprehensiveAuditReportAsync(
                auditPlan, auditFindings, nonConformances, riskAssessment, correctiveActions, complianceGaps);

            // Store audit results
            await StoreAuditResultsAsync(auditReport);

            return new QualityAuditResult
            {
                AuditId = auditReport.AuditId,
                AuditType = request.AuditType,
                AuditScope = request.AuditScope,
                AuditDate = DateTime.UtcNow,
                AuditPlan = auditPlan,
                AuditFindings = auditFindings,
                NonConformances = nonConformances,
                RiskAssessment = riskAssessment,
                CorrectiveActions = correctiveActions,
                ComplianceGaps = complianceGaps,
                OverallAuditScore = CalculateOverallAuditScore(auditFindings, nonConformances),
                AuditConclusion = DetermineAuditConclusion(auditFindings, nonConformances, riskAssessment),
                RecommendedActions = GenerateAuditRecommendations(nonConformances, riskAssessment, complianceGaps),
                NextAuditDate = CalculateNextAuditDate(riskAssessment.OverallRiskLevel)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Comprehensive quality audit failed for {AuditScope}", request.AuditScope);
            throw;
        }
    }

    private async Task<RiskAssessmentResult> AssessAuditRisksAsync(List<NonConformance> nonConformances)
    {
        var riskAssessments = new List<RiskAssessment>();
        
        foreach (var nonConformance in nonConformances)
        {
            var riskAssessment = new RiskAssessment
            {
                NonConformanceId = nonConformance.Id,
                RiskCategory = DetermineRiskCategory(nonConformance),
                Probability = AssessProbability(nonConformance),
                Severity = AssessSeverity(nonConformance),
                Detection = AssessDetection(nonConformance),
                RiskPriorityNumber = 0 // Will be calculated
            };
            
            riskAssessment.RiskPriorityNumber = riskAssessment.Probability * riskAssessment.Severity * riskAssessment.Detection;
            riskAssessment.RiskLevel = DetermineRiskLevel(riskAssessment.RiskPriorityNumber);
            
            riskAssessments.Add(riskAssessment);
        }
        
        var overallRiskLevel = DetermineOverallRiskLevel(riskAssessments);
        
        return new RiskAssessmentResult
        {
            IndividualRiskAssessments = riskAssessments,
            OverallRiskLevel = overallRiskLevel,
            HighestRiskItems = riskAssessments
                .Where(ra => ra.RiskLevel >= RiskLevel.High)
                .OrderByDescending(ra => ra.RiskPriorityNumber)
                .Take(10)
                .ToList(),
            RiskMitigationRecommendations = GenerateRiskMitigationRecommendations(riskAssessments)
        };
    }

    private async Task<AuditReport> GenerateComprehensiveAuditReportAsync(
        AuditPlan auditPlan,
        List<AuditFinding> auditFindings,
        List<NonConformance> nonConformances,
        RiskAssessmentResult riskAssessment,
        List<CorrectiveAction> correctiveActions,
        List<ComplianceGap> complianceGaps)
    {
        var auditReport = new AuditReport
        {
            AuditId = Guid.NewGuid().ToString(),
            AuditTitle = $"Comprehensive Quality Audit - {auditPlan.AuditScope}",
            AuditDate = DateTime.UtcNow,
            AuditTeam = auditPlan.AuditTeam,
            AuditScope = auditPlan.AuditScope,
            
            // Executive Summary
            ExecutiveSummary = new AuditExecutiveSummary
            {
                TotalFindingsCount = auditFindings.Count,
                NonConformanceCount = nonConformances.Count,
                HighRiskItemsCount = riskAssessment.HighestRiskItems.Count,
                ComplianceGapsCount = complianceGaps.Count,
                OverallAuditScore = CalculateOverallAuditScore(auditFindings, nonConformances),
                KeyFindings = auditFindings.Take(5).Select(af => af.FindingDescription).ToList(),
                CriticalIssues = nonConformances.Where(nc => nc.Severity == Severity.Critical).ToList(),
                RecommendedActions = correctiveActions.Where(ca => ca.Priority == Priority.High).Take(3).ToList()
            },
            
            // Detailed Sections
            AuditFindings = auditFindings,
            NonConformanceAnalysis = new NonConformanceAnalysis
            {
                NonConformances = nonConformances,
                NonConformancesByCategory = nonConformances.GroupBy(nc => nc.Category).ToDictionary(g => g.Key, g => g.ToList()),
                NonConformancesBySeverity = nonConformances.GroupBy(nc => nc.Severity).ToDictionary(g => g.Key, g => g.Count()),
                TrendAnalysis = await AnalyzeNonConformanceTrendsAsync(nonConformances)
            },
            
            RiskAssessment = riskAssessment,
            CorrectiveActionPlan = correctiveActions,
            ComplianceGapAnalysis = complianceGaps,
            
            // Conclusions and Recommendations
            AuditConclusion = DetermineAuditConclusion(auditFindings, nonConformances, riskAssessment),
            Recommendations = GenerateAuditRecommendations(nonConformances, riskAssessment, complianceGaps),
            
            // Follow-up Requirements
            FollowUpAuditRequired = DetermineFollowUpRequirement(riskAssessment.OverallRiskLevel),
            NextAuditDate = CalculateNextAuditDate(riskAssessment.OverallRiskLevel)
        };

        return auditReport;
    }

    private double CalculateOverallAuditScore(List<AuditFinding> findings, List<NonConformance> nonConformances)
    {
        var baseScore = 100.0;
        
        // Deduct points for findings
        foreach (var finding in findings)
        {
            var deduction = finding.Severity switch
            {
                Severity.Critical => 20.0,
                Severity.High => 10.0,
                Severity.Medium => 5.0,
                Severity.Low => 2.0,
                _ => 1.0
            };
            baseScore -= deduction;
        }
        
        // Additional deduction for non-conformances
        foreach (var nonConformance in nonConformances)
        {
            var deduction = nonConformance.Severity switch
            {
                Severity.Critical => 15.0,
                Severity.High => 8.0,
                Severity.Medium => 4.0,
                Severity.Low => 2.0,
                _ => 1.0
            };
            baseScore -= deduction;
        }
        
        return Math.Max(0, Math.Min(100, baseScore));
    }

    private AuditConclusion DetermineAuditConclusion(
        List<AuditFinding> findings,
        List<NonConformance> nonConformances,
        RiskAssessmentResult riskAssessment)
    {
        var criticalIssues = nonConformances.Count(nc => nc.Severity == Severity.Critical);
        var highRiskItems = riskAssessment.HighestRiskItems.Count;
        
        if (criticalIssues > 0 || riskAssessment.OverallRiskLevel == RiskLevel.Critical)
        {
            return AuditConclusion.Unsatisfactory;
        }
        
        if (highRiskItems > 5 || riskAssessment.OverallRiskLevel == RiskLevel.High)
        {
            return AuditConclusion.NeedsImprovement;
        }
        
        if (nonConformances.Count <= 5 && riskAssessment.OverallRiskLevel <= RiskLevel.Medium)
        {
            return AuditConclusion.Satisfactory;
        }
        
        if (nonConformances.Count == 0 && riskAssessment.OverallRiskLevel == RiskLevel.Low)
        {
            return AuditConclusion.Excellent;
        }
        
        return AuditConclusion.Acceptable;
    }

    private DateTime CalculateNextAuditDate(RiskLevel overallRiskLevel)
    {
        var auditInterval = overallRiskLevel switch
        {
            RiskLevel.Critical => TimeSpan.FromDays(30),
            RiskLevel.High => TimeSpan.FromDays(60),
            RiskLevel.Medium => TimeSpan.FromDays(90),
            RiskLevel.Low => TimeSpan.FromDays(180),
            _ => TimeSpan.FromDays(365)
        };
        
        return DateTime.UtcNow.Add(auditInterval);
    }

    private async Task StoreAuditResultsAsync(AuditReport auditReport)
    {
        var auditParameters = new MySqlParameter[]
        {
            new("p_AuditId", auditReport.AuditId),
            new("p_AuditTitle", auditReport.AuditTitle),
            new("p_AuditScope", auditReport.AuditScope),
            new("p_AuditDate", auditReport.AuditDate),
            new("p_OverallScore", auditReport.ExecutiveSummary.OverallAuditScore),
            new("p_AuditConclusion", auditReport.AuditConclusion.ToString()),
            new("p_FindingsCount", auditReport.ExecutiveSummary.TotalFindingsCount),
            new("p_NonConformanceCount", auditReport.ExecutiveSummary.NonConformanceCount),
            new("p_NextAuditDate", auditReport.NextAuditDate),
            new("p_AuditData", JsonSerializer.Serialize(auditReport))
        };

        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "qa_audit_results_Store", auditParameters);
    }
}
```

## 📊 Advanced Quality Metrics Dashboard

### Real-Time Quality Metrics System

```csharp
// Real-time quality metrics dashboard with advanced analytics
public class QualityMetricsDashboardService : IQualityMetricsDashboardService
{
    private readonly IQualityDataService _qualityDataService;
    private readonly IQualityKPICalculationService _kpiCalculationService;
    private readonly IQualityTrendAnalysisService _trendAnalysisService;
    private readonly ISignalRHubContext<QualityDashboardHub> _hubContext;
    private readonly ILogger<QualityMetricsDashboardService> _logger;

    public QualityMetricsDashboardService(
        IQualityDataService qualityDataService,
        IQualityKPICalculationService kpiCalculationService,
        IQualityTrendAnalysisService trendAnalysisService,
        ISignalRHubContext<QualityDashboardHub> hubContext,
        ILogger<QualityMetricsDashboardService> logger)
    {
        _qualityDataService = qualityDataService;
        _kpiCalculationService = kpiCalculationService;
        _trendAnalysisService = trendAnalysisService;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<QualityDashboardData> GetRealTimeQualityDashboardDataAsync()
    {
        try
        {
            _logger.LogInformation("Generating real-time quality dashboard data");

            var dashboardTasks = new List<Task>
            {
                GetOverallEquipmentEffectivenessAsync(),
                GetDefectRateMetricsAsync(), 
                GetProcessCapabilityMetricsAsync(),
                GetCustomerSatisfactionMetricsAsync(),
                GetSupplierQualityMetricsAsync(),
                GetQualityComplianceMetricsAsync(),
                GetQualityTrendAnalysisAsync(),
                GetQualityAlertsAndNotificationsAsync()
            };

            await Task.WhenAll(dashboardTasks);

            var dashboardData = new QualityDashboardData
            {
                LastUpdated = DateTime.UtcNow,
                OverallEquipmentEffectiveness = await GetOverallEquipmentEffectivenessAsync(),
                DefectRateMetrics = await GetDefectRateMetricsAsync(),
                ProcessCapabilityMetrics = await GetProcessCapabilityMetricsAsync(),
                CustomerSatisfactionMetrics = await GetCustomerSatisfactionMetricsAsync(),
                SupplierQualityMetrics = await GetSupplierQualityMetricsAsync(),
                ComplianceMetrics = await GetQualityComplianceMetricsAsync(),
                QualityTrends = await GetQualityTrendAnalysisAsync(),
                ActiveAlerts = await GetQualityAlertsAndNotificationsAsync(),
                QualityScorecard = await GenerateQualityScoreCardAsync()
            };

            // Broadcast real-time updates to connected clients
            await _hubContext.Clients.All.SendAsync("QualityDashboardUpdate", dashboardData);

            return dashboardData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate quality dashboard data");
            throw;
        }
    }

    private async Task<OverallEquipmentEffectivenessMetrics> GetOverallEquipmentEffectivenessAsync()
    {
        var oeeData = await _kpiCalculationService.CalculateOEEAsync();
        
        return new OverallEquipmentEffectivenessMetrics
        {
            OverallOEE = oeeData.Availability * oeeData.Performance * oeeData.Quality,
            Availability = oeeData.Availability,
            Performance = oeeData.Performance,
            Quality = oeeData.Quality,
            AvailabilityTrend = await _trendAnalysisService.GetAvailabilityTrendAsync(),
            PerformanceTrend = await _trendAnalysisService.GetPerformanceTrendAsync(),
            QualityTrend = await _trendAnalysisService.GetQualityTrendAsync(),
            OEEByEquipment = await GetOEEByEquipmentAsync(),
            OEETarget = 0.85, // 85% target
            OEEStatus = DetermineOEEStatus(oeeData.Availability * oeeData.Performance * oeeData.Quality)
        };
    }

    private async Task<DefectRateMetrics> GetDefectRateMetricsAsync()
    {
        var defectData = await _qualityDataService.GetDefectRateDataAsync();
        
        return new DefectRateMetrics
        {
            OverallDefectRate = defectData.OverallDefectRate,
            DefectRateByProduct = defectData.DefectRateByProduct,
            DefectRateByProcess = defectData.DefectRateByProcess,
            DefectRateBySupplier = defectData.DefectRateBySupplier,
            DefectRateTrend = await _trendAnalysisService.GetDefectRateTrendAsync(),
            TopDefectTypes = defectData.TopDefectTypes.Take(10).ToList(),
            FirstPassYield = defectData.FirstPassYield,
            ReworkRate = defectData.ReworkRate,
            ScrapRate = defectData.ScrapRate,
            DefectRateTarget = 0.001, // 0.1% target
            DefectRateStatus = DetermineDefectRateStatus(defectData.OverallDefectRate)
        };
    }

    private async Task<ProcessCapabilityDashboardMetrics> GetProcessCapabilityMetricsAsync()
    {
        var capabilityData = await _kpiCalculationService.CalculateProcessCapabilityMetricsAsync();
        
        return new ProcessCapabilityDashboardMetrics
        {
            AverageCpk = capabilityData.AverageCpk,
            ProcessesAboveTarget = capabilityData.ProcessesWithCpkAbove133.Count,
            ProcessesBelowTarget = capabilityData.ProcessesWithCpkBelow133.Count,
            SixSigmaProcesses = capabilityData.SixSigmaProcesses.Count,
            CapabilityByProduct = capabilityData.CapabilityByProduct,
            CapabilityByProcess = capabilityData.CapabilityByProcess,
            CapabilityTrend = await _trendAnalysisService.GetCapabilityTrendAsync(),
            ProcessImprovementOpportunities = capabilityData.ProcessesWithCpkBelow133.Take(5).ToList(),
            CapabilityTarget = 1.33,
            CapabilityStatus = DetermineCapabilityStatus(capabilityData.AverageCpk)
        };
    }

    private async Task<QualityScorecard> GenerateQualityScoreCardAsync()
    {
        var scorecardMetrics = new List<QualityScorecardMetric>
        {
            new QualityScorecardMetric
            {
                MetricName = "Overall Equipment Effectiveness (OEE)",
                CurrentValue = await _kpiCalculationService.GetCurrentOEEAsync(),
                TargetValue = 85.0,
                Unit = "%",
                Trend = await _trendAnalysisService.GetOEETrendAsync(),
                Status = QualityMetricStatus.OnTarget,
                Weight = 0.25
            },
            new QualityScorecardMetric
            {
                MetricName = "Defect Rate",
                CurrentValue = await _kpiCalculationService.GetCurrentDefectRateAsync(),
                TargetValue = 0.1,
                Unit = "%",
                Trend = await _trendAnalysisService.GetDefectRateTrendAsync(),
                Status = QualityMetricStatus.BelowTarget,
                Weight = 0.20
            },
            new QualityScorecardMetric
            {
                MetricName = "Process Capability (Average Cpk)",
                CurrentValue = await _kpiCalculationService.GetAverageProcessCapabilityAsync(),
                TargetValue = 1.33,
                Unit = "Cpk",
                Trend = await _trendAnalysisService.GetCapabilityTrendAsync(),
                Status = QualityMetricStatus.OnTarget,
                Weight = 0.20
            },
            new QualityScorecardMetric
            {
                MetricName = "Customer Satisfaction",
                CurrentValue = await _kpiCalculationService.GetCustomerSatisfactionScoreAsync(),
                TargetValue = 95.0,
                Unit = "%",
                Trend = await _trendAnalysisService.GetCustomerSatisfactionTrendAsync(),
                Status = QualityMetricStatus.AboveTarget,
                Weight = 0.15
            },
            new QualityScorecardMetric
            {
                MetricName = "Supplier Quality Rating",
                CurrentValue = await _kpiCalculationService.GetSupplierQualityRatingAsync(),
                TargetValue = 90.0,
                Unit = "%",
                Trend = await _trendAnalysisService.GetSupplierQualityTrendAsync(),
                Status = QualityMetricStatus.OnTarget,
                Weight = 0.10
            },
            new QualityScorecardMetric
            {
                MetricName = "Regulatory Compliance",
                CurrentValue = await _kpiCalculationService.GetCompliancePercentageAsync(),
                TargetValue = 100.0,
                Unit = "%",
                Trend = await _trendAnalysisService.GetComplianceTrendAsync(),
                Status = QualityMetricStatus.OnTarget,
                Weight = 0.10
            }
        };

        var overallScore = scorecardMetrics.Sum(m => (m.CurrentValue / m.TargetValue * 100) * m.Weight);
        var overallStatus = DetermineOverallScorecardStatus(overallScore);

        return new QualityScorecard
        {
            ScorecardDate = DateTime.UtcNow,
            OverallScore = overallScore,
            OverallStatus = overallStatus,
            Metrics = scorecardMetrics,
            PerformanceComparison = await GetPerformanceComparisonAsync(),
            TrendSummary = await GetScorecardTrendSummaryAsync(),
            KeyInsights = await GenerateKeyInsightsAsync(scorecardMetrics),
            ActionItems = await GenerateActionItemsAsync(scorecardMetrics)
        };
    }

    private QualityMetricStatus DetermineOverallScorecardStatus(double overallScore)
    {
        return overallScore switch
        {
            >= 100 => QualityMetricStatus.AboveTarget,
            >= 95 => QualityMetricStatus.OnTarget,
            >= 85 => QualityMetricStatus.NearTarget,
            >= 70 => QualityMetricStatus.BelowTarget,
            _ => QualityMetricStatus.Critical
        };
    }
}
```

## 📚 Related Documentation

- **Manufacturing KPI Dashboard**: [KPI Integration Patterns](./manufacturing-kpi-dashboard-integration.instructions.md)
- **Service Integration**: [Cross-Service Communication](./service-integration.instructions.md)
- **Database Integration**: [Advanced Database Patterns](./database-integration.instructions.md)
- **MVVM Patterns**: [Complex ViewModel Implementation](./mvvm-community-toolkit.instructions.md)

---

**Document Status**: ✅ Complete Advanced Quality Assurance Framework Reference  
**Framework Version**: .NET 8 with Comprehensive Quality Management  
**Last Updated**: 2025-09-15  
**Quality Assurance Owner**: MTM Development Team

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