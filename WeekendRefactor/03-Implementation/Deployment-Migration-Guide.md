# Overlay System Deployment and Migration Guide

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Audience**: MTM Application Deployment Engineers and DevOps Teams  

## 🚀 Deployment Overview

This guide provides step-by-step instructions for deploying the MTM overlay system, migrating from existing implementations, handling rollbacks, and managing production considerations. Includes deployment automation, monitoring setup, and maintenance procedures.

## 📋 Pre-Deployment Checklist

### **Environment Validation**

```powershell
# PowerShell script for deployment readiness validation
# File: scripts/Validate-DeploymentEnvironment.ps1

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Development", "Staging", "Production")]
    [string]$Environment
)

Write-Host "=== MTM Overlay System Deployment Validation ===" -ForegroundColor Cyan
Write-Host "Target Environment: $Environment" -ForegroundColor Yellow

# Check .NET Runtime
$dotnetVersion = dotnet --version 2>$null
if ($dotnetVersion -and $dotnetVersion.StartsWith("8.")) {
    Write-Host "✓ .NET 8.0 runtime available: $dotnetVersion" -ForegroundColor Green
} else {
    Write-Host "✗ .NET 8.0 runtime not found or incorrect version" -ForegroundColor Red
    exit 1
}

# Check MySQL Connectivity
try {
    $connectionString = Get-ConfigValue -Key "ConnectionStrings:DefaultConnection" -Environment $Environment
    Test-MySqlConnection -ConnectionString $connectionString
    Write-Host "✓ MySQL database connectivity verified" -ForegroundColor Green
} catch {
    Write-Host "✗ MySQL database connection failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Validate Configuration Files
$configFiles = @(
    "appsettings.$Environment.json",
    "appsettings.json"
)

foreach ($configFile in $configFiles) {
    if (Test-Path $configFile) {
        Write-Host "✓ Configuration file found: $configFile" -ForegroundColor Green
        
        # Validate JSON syntax
        try {
            Get-Content $configFile | ConvertFrom-Json | Out-Null
            Write-Host "✓ Configuration file JSON is valid: $configFile" -ForegroundColor Green
        } catch {
            Write-Host "✗ Invalid JSON in configuration file: $configFile" -ForegroundColor Red
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "✗ Configuration file missing: $configFile" -ForegroundColor Red
        exit 1
    }
}

# Check Required Services Registration
$serviceRegistrationFiles = @(
    "Extensions/ServiceCollectionExtensions.cs",
    "Program.cs"
)

foreach ($file in $serviceRegistrationFiles) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        
        # Check for Universal Overlay Service registration
        if ($content -match "AddUniversalOverlayService|IUniversalOverlayService") {
            Write-Host "✓ Universal Overlay Service registration found in: $file" -ForegroundColor Green
        } else {
            Write-Host "⚠ Universal Overlay Service registration not found in: $file" -ForegroundColor Yellow
        }
        
        # Check for overlay ViewModels registration
        if ($content -match "AddTransient.*OverlayViewModel|AddScoped.*OverlayViewModel") {
            Write-Host "✓ Overlay ViewModel registrations found in: $file" -ForegroundColor Green
        } else {
            Write-Host "⚠ Overlay ViewModel registrations not found in: $file" -ForegroundColor Yellow
        }
    }
}

# Validate Database Schema
Write-Host "Validating database schema..." -ForegroundColor Cyan
$requiredStoredProcedures = @(
    "inv_inventory_Update_QuickEdit",
    "inv_transaction_Add",
    "log_audit_Add_Entry",
    "usr_users_Get_All",
    "md_part_ids_Get_All"
)

foreach ($procedure in $requiredStoredProcedures) {
    if (Test-StoredProcedureExists -Name $procedure -ConnectionString $connectionString) {
        Write-Host "✓ Stored procedure exists: $procedure" -ForegroundColor Green
    } else {
        Write-Host "✗ Stored procedure missing: $procedure" -ForegroundColor Red
        exit 1
    }
}

Write-Host "=== Deployment Environment Validation Complete ===" -ForegroundColor Cyan
```

### **Database Migration Scripts**

```sql
-- File: database/migrations/001-overlay-system-tables.sql

-- Add overlay system audit table if not exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
               WHERE TABLE_SCHEMA = 'mtm_wip' AND TABLE_NAME = 'overlay_audit_log')
BEGIN
    CREATE TABLE overlay_audit_log (
        id INT PRIMARY KEY AUTO_INCREMENT,
        overlay_type VARCHAR(100) NOT NULL,
        overlay_id VARCHAR(36) NOT NULL,
        action VARCHAR(50) NOT NULL,
        user_name VARCHAR(100) NOT NULL,
        timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
        data JSON,
        INDEX idx_overlay_type (overlay_type),
        INDEX idx_user_name (user_name),
        INDEX idx_timestamp (timestamp)
    ) ENGINE=InnoDB;
    
    PRINT 'Created overlay_audit_log table';
END

-- Add overlay performance metrics table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
               WHERE TABLE_SCHEMA = 'mtm_wip' AND TABLE_NAME = 'overlay_performance_metrics')
BEGIN
    CREATE TABLE overlay_performance_metrics (
        id INT PRIMARY KEY AUTO_INCREMENT,
        overlay_type VARCHAR(100) NOT NULL,
        operation VARCHAR(50) NOT NULL,
        duration_ms INT NOT NULL,
        memory_usage_kb INT,
        timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
        environment VARCHAR(20) NOT NULL,
        INDEX idx_overlay_type_operation (overlay_type, operation),
        INDEX idx_timestamp (timestamp),
        INDEX idx_environment (environment)
    ) ENGINE=InnoDB;
    
    PRINT 'Created overlay_performance_metrics table';
END

-- Update existing stored procedures for overlay system compatibility
-- Add overlay support to inventory update procedure
DROP PROCEDURE IF EXISTS inv_inventory_Update_QuickEdit;

DELIMITER $$
CREATE PROCEDURE inv_inventory_Update_QuickEdit(
    IN p_PartID VARCHAR(50),
    IN p_Operation VARCHAR(10),
    IN p_Location VARCHAR(50),
    IN p_NewQuantity INT,
    IN p_Notes TEXT,
    IN p_User VARCHAR(100),
    IN p_OverlayId VARCHAR(36) DEFAULT NULL
)
BEGIN
    DECLARE v_OldQuantity INT DEFAULT 0;
    DECLARE v_TransactionId INT DEFAULT 0;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SELECT 0 as Status, 'Database error occurred' as Message;
    END;
    
    START TRANSACTION;
    
    -- Get current quantity
    SELECT Quantity INTO v_OldQuantity 
    FROM inventory 
    WHERE PartID = p_PartID AND OperationNumber = p_Operation AND Location = p_Location;
    
    -- Update inventory
    UPDATE inventory 
    SET Quantity = p_NewQuantity,
        Notes = p_Notes,
        LastUpdated = NOW(),
        LastUpdatedBy = p_User
    WHERE PartID = p_PartID AND OperationNumber = p_Operation AND Location = p_Location;
    
    -- Log transaction
    CALL inv_transaction_Add(p_PartID, p_Operation, 'QUICK_EDIT', 
                           (p_NewQuantity - v_OldQuantity), p_Notes, p_User);
    
    -- Log overlay audit if overlay ID provided
    IF p_OverlayId IS NOT NULL THEN
        INSERT INTO overlay_audit_log (overlay_type, overlay_id, action, user_name, data)
        VALUES ('InventoryQuickEdit', p_OverlayId, 'SAVE_COMPLETED', p_User,
                JSON_OBJECT(
                    'partId', p_PartID,
                    'operation', p_Operation,
                    'location', p_Location,
                    'oldQuantity', v_OldQuantity,
                    'newQuantity', p_NewQuantity,
                    'notes', p_Notes
                ));
    END IF;
    
    COMMIT;
    
    SELECT 1 as Status, 'Inventory updated successfully' as Message, 
           v_TransactionId as TransactionId;
END$$
DELIMITER ;

-- Grant permissions to application user
GRANT EXECUTE ON PROCEDURE inv_inventory_Update_QuickEdit TO 'mtm_app_user'@'%';
GRANT SELECT, INSERT, UPDATE ON overlay_audit_log TO 'mtm_app_user'@'%';
GRANT SELECT, INSERT ON overlay_performance_metrics TO 'mtm_app_user'@'%';
```

## 🏗️ Deployment Stages

### **Stage 1: Infrastructure Preparation**

```yaml
# File: deployment/docker-compose.production.yml

version: '3.8'

services:
  mtm-application:
    image: mtm-wip-application:latest
    container_name: mtm-wip-app
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${MYSQL_CONNECTION_STRING}
      - OverlaySystem__EnablePerformanceMetrics=true
      - OverlaySystem__EnableAuditLogging=true
      - OverlaySystem__PoolingEnabled=true
      - OverlaySystem__MaxPoolSize=50
    volumes:
      - ./config/appsettings.Production.json:/app/Config/appsettings.Production.json:ro
      - ./logs:/app/logs
    ports:
      - "8080:80"
    depends_on:
      - mysql
    restart: unless-stopped
    
  mysql:
    image: mysql:8.0
    container_name: mtm-mysql
    environment:
      - MYSQL_ROOT_PASSWORD=${MYSQL_ROOT_PASSWORD}
      - MYSQL_DATABASE=mtm_wip
      - MYSQL_USER=mtm_app_user
      - MYSQL_PASSWORD=${MYSQL_USER_PASSWORD}
    volumes:
      - mysql_data:/var/lib/mysql
      - ./database/migrations:/docker-entrypoint-initdb.d:ro
    ports:
      - "3306:3306"
    restart: unless-stopped

  # Monitoring and logging
  prometheus:
    image: prom/prometheus:latest
    container_name: mtm-prometheus
    volumes:
      - ./monitoring/prometheus.yml:/etc/prometheus/prometheus.yml
    ports:
      - "9090:9090"
    restart: unless-stopped

volumes:
  mysql_data:
    driver: local
```

### **Stage 2: Application Configuration**

```json
# File: Config/appsettings.Production.json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=${MYSQL_HOST};Database=mtm_wip;User Id=mtm_app_user;Password=${MYSQL_PASSWORD};Allow User Variables=true;Convert Zero Datetime=true;"
  },
  
  "OverlaySystem": {
    "EnablePerformanceMetrics": true,
    "EnableAuditLogging": true,
    "PoolingEnabled": true,
    "MaxPoolSize": 100,
    "PoolTimeoutSeconds": 300,
    "AnimationDuration": "00:00:00.250",
    "DefaultTheme": "MTM_Blue",
    "CrossPlatformOptimization": true,
    "AccessibilityEnabled": true,
    "DebugMode": false
  },
  
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "MTM_WIP_Application_Avalonia": "Information",
      "MTM_WIP_Application_Avalonia.Services.UniversalOverlayService": "Debug",
      "MTM_WIP_Application_Avalonia.ViewModels.Overlay": "Information"
    },
    "File": {
      "Path": "logs/mtm-application.log",
      "MaxRollingFiles": 30,
      "FileSizeLimitBytes": 104857600,
      "IncludeScopes": true
    }
  },
  
  "Performance": {
    "EnableMetricsCollection": true,
    "MetricsRetentionDays": 90,
    "SlowOperationThresholdMs": 1000,
    "MemoryThresholdMB": 512
  },
  
  "Security": {
    "AuditAllOverlayOperations": true,
    "RequireUserAuthentication": true,
    "SessionTimeoutMinutes": 480,
    "MaxConcurrentUsers": 100
  }
}
```

### **Stage 3: Service Registration Updates**

```csharp
// File: Extensions/ServiceCollectionExtensions.cs - Production Updates

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMOverlaySystem(
        this IServiceCollection services, 
        IConfiguration configuration,
        bool isProduction = false)
    {
        // Core overlay services
        services.TryAddSingleton<IUniversalOverlayService, UniversalOverlayService>();
        services.TryAddSingleton<IOverlayPoolService, OverlayPoolService>();
        services.TryAddSingleton<IOverlayThemeService, OverlayThemeService>();
        
        // Production-specific services
        if (isProduction)
        {
            services.TryAddSingleton<IOverlayPerformanceMetricsService, 
                OverlayPerformanceMetricsService>();
            services.TryAddSingleton<IOverlayAuditService, OverlayAuditService>();
            services.TryAddSingleton<IOverlayMonitoringService, OverlayMonitoringService>();
        }
        else
        {
            // Development/test services
            services.TryAddSingleton<IOverlayPerformanceMetricsService, 
                NullOverlayPerformanceMetricsService>();
            services.TryAddSingleton<IOverlayAuditService, NullOverlayAuditService>();
        }
        
        // Register all overlay ViewModels
        RegisterOverlayViewModels(services);
        
        // Configure options
        services.Configure<OverlaySystemOptions>(
            configuration.GetSection("OverlaySystem"));
        
        return services;
    }

    private static void RegisterOverlayViewModels(IServiceCollection services)
    {
        // Register all overlay ViewModels for dependency injection
        var overlayViewModelTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && 
                       t.IsSubclassOf(typeof(BasePoolableOverlayViewModel)))
            .ToList();

        foreach (var viewModelType in overlayViewModelTypes)
        {
            services.TryAddTransient(viewModelType);
        }
    }
}

// Update Program.cs for production deployment
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var app = builder.Build();

        // Configure overlay system with production settings
        var environment = app.Services.GetRequiredService<IWebHostEnvironment>();
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        
        app.Services.AddMTMOverlaySystem(configuration, environment.IsProduction());
        
        // Initialize overlay system
        InitializeOverlaySystemAsync(app.Services).GetAwaiter().GetResult();
        
        app.Run();
    }

    private static async Task InitializeOverlaySystemAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        try
        {
            // Initialize overlay pool
            var poolService = services.GetRequiredService<IOverlayPoolService>();
            await poolService.InitializeAsync();
            
            // Validate overlay registrations
            var overlayService = services.GetRequiredService<IUniversalOverlayService>();
            await overlayService.ValidateConfigurationAsync();
            
            // Start performance monitoring (production only)
            if (services.GetService<IOverlayPerformanceMetricsService>() is 
                OverlayPerformanceMetricsService metricsService)
            {
                await metricsService.StartCollectionAsync();
            }
            
            logger.LogInformation("MTM Overlay System initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize MTM Overlay System");
            throw;
        }
    }
}
```

## 🔄 Migration Strategy

### **Migration from Existing Overlay Implementations**

```csharp
// File: Services/OverlayMigrationService.cs

public interface IOverlayMigrationService
{
    Task<MigrationResult> MigrateOverlaysAsync(MigrationPlan plan);
    Task<MigrationPlan> CreateMigrationPlanAsync();
    Task ValidateMigrationAsync(MigrationPlan plan);
    Task RollbackMigrationAsync(string migrationId);
}

public class OverlayMigrationService : IOverlayMigrationService
{
    private readonly ILogger<OverlayMigrationService> _logger;
    private readonly IConfigurationService _configurationService;

    public OverlayMigrationService(
        ILogger<OverlayMigrationService> logger,
        IConfigurationService configurationService)
    {
        _logger = logger;
        _configurationService = configurationService;
    }

    public async Task<MigrationResult> MigrateOverlaysAsync(MigrationPlan plan)
    {
        var migrationId = Guid.NewGuid().ToString();
        var result = new MigrationResult { MigrationId = migrationId };
        
        _logger.LogInformation("Starting overlay migration: {MigrationId}", migrationId);
        
        try
        {
            // Phase 1: Backup existing configurations
            await BackupExistingConfigurationsAsync(migrationId);
            
            // Phase 2: Update service registrations
            await UpdateServiceRegistrationsAsync(plan);
            
            // Phase 3: Migrate overlay ViewModels
            await MigrateOverlayViewModelsAsync(plan);
            
            // Phase 4: Update AXAML views
            await MigrateOverlayViewsAsync(plan);
            
            // Phase 5: Update database schema
            await MigrateDatabaseSchemaAsync(plan);
            
            // Phase 6: Validate migration
            await ValidateMigrationAsync(plan);
            
            result.IsSuccess = true;
            result.Message = "Migration completed successfully";
            
            _logger.LogInformation("Overlay migration completed successfully: {MigrationId}", 
                migrationId);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Migration failed: {ex.Message}";
            result.Exception = ex;
            
            _logger.LogError(ex, "Overlay migration failed: {MigrationId}", migrationId);
            
            // Attempt rollback
            try
            {
                await RollbackMigrationAsync(migrationId);
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, 
                    "Migration rollback failed for: {MigrationId}", migrationId);
            }
        }
        
        return result;
    }

    public async Task<MigrationPlan> CreateMigrationPlanAsync()
    {
        _logger.LogInformation("Creating overlay migration plan");
        
        var plan = new MigrationPlan
        {
            CreatedAt = DateTime.UtcNow,
            MigrationSteps = new List<MigrationStep>()
        };

        // Analyze existing overlay implementations
        var existingOverlays = await AnalyzeExistingOverlaysAsync();
        
        foreach (var overlay in existingOverlays)
        {
            var migrationStep = await CreateMigrationStepAsync(overlay);
            plan.MigrationSteps.Add(migrationStep);
        }
        
        // Sort by dependency order
        plan.MigrationSteps = plan.MigrationSteps
            .OrderBy(s => s.Priority)
            .ToList();
        
        _logger.LogInformation("Created migration plan with {Count} steps", 
            plan.MigrationSteps.Count);
        
        return plan;
    }

    private async Task<List<ExistingOverlayInfo>> AnalyzeExistingOverlaysAsync()
    {
        var overlays = new List<ExistingOverlayInfo>();
        
        // Scan for existing overlay patterns
        var sourceFiles = Directory.GetFiles(".", "*.cs", SearchOption.AllDirectories)
            .Where(f => f.Contains("Overlay") || f.Contains("Dialog"))
            .ToList();

        foreach (var file in sourceFiles)
        {
            var content = await File.ReadAllTextAsync(file);
            
            // Look for existing overlay patterns
            if (IsLegacyOverlay(content))
            {
                var overlayInfo = await ExtractOverlayInfoAsync(file, content);
                overlays.Add(overlayInfo);
            }
        }
        
        return overlays;
    }

    private bool IsLegacyOverlay(string content)
    {
        // Check for old overlay patterns that need migration
        var legacyPatterns = new[]
        {
            "ReactiveObject", // ReactiveUI
            "Window.ShowDialog", // Direct WPF dialogs
            "MessageBox.Show", // MessageBox calls
            "this.RaiseAndSetIfChanged", // ReactiveUI property pattern
            "ReactiveCommand<", // ReactiveUI commands
        };

        return legacyPatterns.Any(pattern => content.Contains(pattern));
    }

    private async Task<MigrationStep> CreateMigrationStepAsync(ExistingOverlayInfo overlay)
    {
        return new MigrationStep
        {
            Id = Guid.NewGuid().ToString(),
            OverlayName = overlay.Name,
            SourceFile = overlay.FilePath,
            MigrationType = DetermineMigrationType(overlay),
            Priority = CalculateMigrationPriority(overlay),
            Dependencies = await AnalyzeDependenciesAsync(overlay),
            EstimatedDuration = TimeSpan.FromMinutes(30), // Base estimate
            ValidationSteps = CreateValidationSteps(overlay)
        };
    }

    private async Task MigrateOverlayViewModelsAsync(MigrationPlan plan)
    {
        foreach (var step in plan.MigrationSteps.Where(s => s.MigrationType.HasFlag(MigrationType.ViewModel)))
        {
            _logger.LogInformation("Migrating ViewModel: {OverlayName}", step.OverlayName);
            
            // Read existing ViewModel
            var existingContent = await File.ReadAllTextAsync(step.SourceFile);
            
            // Transform to MVVM Community Toolkit pattern
            var migratedContent = await TransformViewModelAsync(existingContent, step);
            
            // Create backup
            var backupPath = $"{step.SourceFile}.migration-backup";
            await File.WriteAllTextAsync(backupPath, existingContent);
            
            // Write migrated content
            await File.WriteAllTextAsync(step.SourceFile, migratedContent);
            
            _logger.LogInformation("ViewModel migration completed: {OverlayName}", step.OverlayName);
        }
    }

    private async Task<string> TransformViewModelAsync(string existingContent, MigrationStep step)
    {
        var transformed = existingContent;
        
        // Replace ReactiveUI patterns with MVVM Community Toolkit
        var replacements = new Dictionary<string, string>
        {
            ["ReactiveObject"] = "[ObservableObject]\npublic partial class",
            ["this.RaiseAndSetIfChanged(ref "] = "[ObservableProperty]\nprivate ",
            ["ReactiveCommand<"] = "[RelayCommand]\nprivate async Task",
            ["WhenAnyValue("] = "// TODO: Replace with property change handler",
        };

        foreach (var replacement in replacements)
        {
            transformed = transformed.Replace(replacement.Key, replacement.Value);
        }

        // Add required using statements
        var usingStatements = new[]
        {
            "using CommunityToolkit.Mvvm.ComponentModel;",
            "using CommunityToolkit.Mvvm.Input;",
            "using MTM_WIP_Application_Avalonia.ViewModels.Overlay;",
        };

        foreach (var usingStatement in usingStatements)
        {
            if (!transformed.Contains(usingStatement))
            {
                transformed = $"{usingStatement}\n{transformed}";
            }
        }

        return transformed;
    }
}

public record MigrationPlan
{
    public DateTime CreatedAt { get; init; }
    public List<MigrationStep> MigrationSteps { get; set; } = new();
}

public record MigrationStep
{
    public string Id { get; init; } = string.Empty;
    public string OverlayName { get; init; } = string.Empty;
    public string SourceFile { get; init; } = string.Empty;
    public MigrationType MigrationType { get; init; }
    public int Priority { get; init; }
    public List<string> Dependencies { get; init; } = new();
    public TimeSpan EstimatedDuration { get; init; }
    public List<ValidationStep> ValidationSteps { get; init; } = new();
}

[Flags]
public enum MigrationType
{
    ViewModel = 1,
    View = 2,
    Service = 4,
    Database = 8,
    Configuration = 16
}
```

## 📊 Monitoring and Health Checks

### **Production Monitoring Setup**

```csharp
// File: Services/OverlayHealthCheckService.cs

public class OverlayHealthCheckService : IHealthCheck
{
    private readonly IUniversalOverlayService _overlayService;
    private readonly IOverlayPoolService _poolService;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<OverlayHealthCheckService> _logger;

    public OverlayHealthCheckService(
        IUniversalOverlayService overlayService,
        IOverlayPoolService poolService,
        IConfigurationService configurationService,
        ILogger<OverlayHealthCheckService> logger)
    {
        _overlayService = overlayService;
        _poolService = poolService;
        _configurationService = configurationService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var healthData = new Dictionary<string, object>();
            var warnings = new List<string>();

            // Check overlay service availability
            var serviceHealthy = await CheckOverlayServiceHealthAsync();
            healthData["overlayServiceHealthy"] = serviceHealthy;
            
            if (!serviceHealthy)
            {
                return HealthCheckResult.Unhealthy("Overlay service is not responding");
            }

            // Check pool service health
            var poolHealth = await CheckPoolServiceHealthAsync();
            healthData["poolHealth"] = poolHealth;
            
            if (poolHealth.UsagePercentage > 90)
            {
                warnings.Add($"Pool usage high: {poolHealth.UsagePercentage}%");
            }

            // Check database connectivity for overlay operations
            var databaseHealthy = await CheckDatabaseHealthAsync();
            healthData["databaseHealthy"] = databaseHealthy;
            
            if (!databaseHealthy)
            {
                warnings.Add("Database connectivity issues detected");
            }

            // Check memory usage
            var memoryUsage = GC.GetTotalMemory(false) / (1024 * 1024); // MB
            healthData["memoryUsageMB"] = memoryUsage;
            
            if (memoryUsage > 512) // 512 MB threshold
            {
                warnings.Add($"High memory usage: {memoryUsage} MB");
            }

            // Check performance metrics
            var avgResponseTime = await GetAverageResponseTimeAsync();
            healthData["averageResponseTimeMs"] = avgResponseTime;
            
            if (avgResponseTime > 1000) // 1 second threshold
            {
                warnings.Add($"Slow response times: {avgResponseTime} ms");
            }

            // Determine overall health status
            if (warnings.Count == 0)
            {
                return HealthCheckResult.Healthy("All overlay system checks passed", healthData);
            }
            else
            {
                var warningMessage = string.Join("; ", warnings);
                return HealthCheckResult.Degraded($"Overlay system warnings: {warningMessage}", 
                    healthData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for overlay system");
            return HealthCheckResult.Unhealthy("Overlay system health check failed", ex);
        }
    }

    private async Task<bool> CheckOverlayServiceHealthAsync()
    {
        try
        {
            // Test overlay service with a simple diagnostic request
            var diagnosticRequest = new DiagnosticOverlayRequest();
            var diagnosticTask = _overlayService.ShowOverlayAsync(
                typeof(DiagnosticOverlayRequest),
                typeof(DiagnosticOverlayResponse),
                typeof(DiagnosticOverlayViewModel),
                diagnosticRequest
            );

            // Cancel after 5 seconds to avoid hanging
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            cts.Token.Register(() => diagnosticTask.TrySetCanceled());

            var response = await diagnosticTask;
            return response.Result == OverlayResult.Confirmed;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Overlay service health check failed");
            return false;
        }
    }

    private async Task<PoolHealthInfo> CheckPoolServiceHealthAsync()
    {
        var poolStats = await _poolService.GetPoolStatisticsAsync();
        
        return new PoolHealthInfo
        {
            TotalInstances = poolStats.TotalInstances,
            ActiveInstances = poolStats.ActiveInstances,
            AvailableInstances = poolStats.AvailableInstances,
            UsagePercentage = (poolStats.ActiveInstances / (double)poolStats.TotalInstances) * 100
        };
    }

    private async Task<bool> CheckDatabaseHealthAsync()
    {
        try
        {
            var connectionString = await _configurationService.GetConnectionStringAsync();
            
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            using var command = new MySqlCommand("SELECT 1", connection);
            var result = await command.ExecuteScalarAsync();
            
            return result != null && result.ToString() == "1";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Database health check failed");
            return false;
        }
    }

    private async Task<double> GetAverageResponseTimeAsync()
    {
        try
        {
            var connectionString = await _configurationService.GetConnectionStringAsync();
            
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            using var command = new MySqlCommand(@"
                SELECT AVG(duration_ms) 
                FROM overlay_performance_metrics 
                WHERE timestamp > DATE_SUB(NOW(), INTERVAL 1 HOUR)
            ", connection);
            
            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToDouble(result) : 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get average response time");
            return 0;
        }
    }
}

public record PoolHealthInfo
{
    public int TotalInstances { get; init; }
    public int ActiveInstances { get; init; }
    public int AvailableInstances { get; init; }
    public double UsagePercentage { get; init; }
}

// Health check registration in Program.cs
services.AddHealthChecks()
    .AddCheck<OverlayHealthCheckService>("overlay-system")
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<MemoryHealthCheck>("memory");
```

### **Automated Rollback Procedures**

```powershell
# File: scripts/Rollback-OverlayDeployment.ps1

param(
    [Parameter(Mandatory=$true)]
    [string]$BackupVersion,
    
    [Parameter(Mandatory=$true)]
    [ValidateSet("Development", "Staging", "Production")]
    [string]$Environment
)

Write-Host "=== MTM Overlay System Rollback Procedure ===" -ForegroundColor Red
Write-Host "Target Environment: $Environment" -ForegroundColor Yellow
Write-Host "Rollback Version: $BackupVersion" -ForegroundColor Yellow

# Stop application
Write-Host "Stopping application..." -ForegroundColor Cyan
if ($Environment -eq "Production") {
    docker-compose -f docker-compose.production.yml down
} else {
    Stop-Process -Name "MTM_WIP_Application_Avalonia" -Force -ErrorAction SilentlyContinue
}

# Restore database backup
Write-Host "Restoring database backup..." -ForegroundColor Cyan
$backupFile = "database/backups/mtm_wip_$BackupVersion.sql"
if (Test-Path $backupFile) {
    & mysql -h $env:MYSQL_HOST -u root -p$env:MYSQL_ROOT_PASSWORD mtm_wip < $backupFile
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database backup restored successfully" -ForegroundColor Green
    } else {
        Write-Host "✗ Database backup restore failed" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "✗ Database backup file not found: $backupFile" -ForegroundColor Red
    exit 1
}

# Restore application files
Write-Host "Restoring application files..." -ForegroundColor Cyan
$backupPath = "backups/application_$BackupVersion"
if (Test-Path $backupPath) {
    Copy-Item -Path "$backupPath\*" -Destination "." -Recurse -Force
    Write-Host "✓ Application files restored successfully" -ForegroundColor Green
} else {
    Write-Host "✗ Application backup not found: $backupPath" -ForegroundColor Red
    exit 1
}

# Restore configuration
Write-Host "Restoring configuration files..." -ForegroundColor Cyan
$configBackup = "backups/config_$BackupVersion"
if (Test-Path $configBackup) {
    Copy-Item -Path "$configBackup\*" -Destination "Config\" -Recurse -Force
    Write-Host "✓ Configuration restored successfully" -ForegroundColor Green
} else {
    Write-Host "⚠ Configuration backup not found, using existing config" -ForegroundColor Yellow
}

# Restart application
Write-Host "Starting application..." -ForegroundColor Cyan
if ($Environment -eq "Production") {
    docker-compose -f docker-compose.production.yml up -d
} else {
    Start-Process -FilePath "MTM_WIP_Application_Avalonia.exe"
}

# Wait for application to start
Start-Sleep -Seconds 30

# Validate rollback
Write-Host "Validating rollback..." -ForegroundColor Cyan
$healthCheckUrl = "http://localhost:8080/health"
try {
    $response = Invoke-RestMethod -Uri $healthCheckUrl -Method GET -TimeoutSec 10
    if ($response.status -eq "Healthy") {
        Write-Host "✓ Rollback completed successfully - Application is healthy" -ForegroundColor Green
    } else {
        Write-Host "⚠ Rollback completed but application health check failed" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Rollback may have failed - Unable to reach health endpoint" -ForegroundColor Red
}

Write-Host "=== Rollback Procedure Complete ===" -ForegroundColor Red
```

## 🎯 Best Practices Summary

### **Deployment Best Practices**

1. **Environment Validation**
   - Always validate environment before deployment
   - Test database connectivity and schema compatibility
   - Verify service registrations and configurations

2. **Gradual Rollout**
   - Deploy to staging environment first
   - Run comprehensive integration tests
   - Monitor performance metrics closely
   - Use blue-green deployment for production

3. **Monitoring and Alerting**
   - Set up comprehensive health checks
   - Monitor overlay performance metrics
   - Create alerts for system degradation
   - Track user experience metrics

4. **Backup and Recovery**
   - Always create backups before deployment
   - Test rollback procedures regularly
   - Document recovery procedures
   - Maintain multiple backup versions

5. **Migration Strategy**
   - Plan migrations carefully with dependency analysis
   - Test migration procedures in non-production environments
   - Provide clear migration documentation
   - Have rollback plans for each migration step

This comprehensive deployment and migration guide ensures successful MTM overlay system deployment with minimal risk and maximum reliability.
