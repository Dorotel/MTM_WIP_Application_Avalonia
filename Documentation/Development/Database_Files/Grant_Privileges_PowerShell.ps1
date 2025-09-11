# PowerShell script to execute MySQL GRANT statements individually
# This helps identify exactly which statement is causing the mysql.servers error

$mysqlPath = "C:\MAMP\bin\mysql\bin\mysql.exe"
$username = "root"

# Array of all GRANT statements for mtm_wip_application
$grantStatements = @(
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DLAFOND'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JEGBERT'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'RECEIVING'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SHOP2'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SHIPPING'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DHAMMONS'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JMAUER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KWILKER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MHANDLER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MIKESAMZ'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MLAURIN'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MLEDVINA'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'NPITSCH'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'PBAHR'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SCARBON'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TTELETZKE'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TLINDLOFF'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'ABEEMAN'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DHAGENOW'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JORNALES'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TSMAXWELL'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'CMUCHOWSKI'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'NWUNSCH'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'GWHITSON'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JCASTRO'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MVOSS'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'NLEE'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JMILLER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SSNYDER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'CSNYDER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'BAUSTIN'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DEBLAFOND'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'ASCHULTZ'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SJACKSON'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DRIEBE'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TRADDATZ'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SDETTLAFF'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JWETAK'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KSKATTEBO'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'AGAUTHIER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MBECKER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'RLESSER'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'AGROELLE'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'CEHLENBECK'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'BNEUMAN'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MTMDC'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KDREWIESKE'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MHERNANDEZ'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KLEE'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'ADMININT'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'CALVAREZ'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TYANG'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KSMITH'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JKOLL'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JBEHRMANN'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MDRESSEL'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DSMITH'@'%';",
    "GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'APIESCHEL'@'%';"
)

# Test connection first
Write-Host "Testing MySQL connection..." -ForegroundColor Yellow
$testResult = echo "SELECT 1;" | & $mysqlPath -u $username -p
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ MySQL connection failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ MySQL connection successful!" -ForegroundColor Green

# Execute GRANT statements for production database
Write-Host "`n🔧 Executing GRANT statements for mtm_wip_application..." -ForegroundColor Cyan
$successCount = 0
foreach ($statement in $grantStatements) {
    Write-Host "Executing: $statement" -ForegroundColor Gray
    $result = echo $statement | & $mysqlPath -u $username -p
    if ($LASTEXITCODE -eq 0) {
        $successCount++
        Write-Host "✅ Success" -ForegroundColor Green
    } else {
        Write-Host "❌ Failed: $statement" -ForegroundColor Red
    }
}

Write-Host "`n📊 Production Database Results: $successCount/$($grantStatements.Count) successful" -ForegroundColor Yellow

# Test FLUSH PRIVILEGES separately
Write-Host "`n🔄 Testing FLUSH PRIVILEGES..." -ForegroundColor Cyan
$flushResult = echo "FLUSH PRIVILEGES;" | & $mysqlPath -u $username -p
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ FLUSH PRIVILEGES successful!" -ForegroundColor Green
} else {
    Write-Host "❌ FLUSH PRIVILEGES failed (this is the mysql.servers error)" -ForegroundColor Red
    Write-Host "But privileges may still be applied automatically." -ForegroundColor Yellow
}

Write-Host "`n🎉 Privilege grant process completed!" -ForegroundColor Green
Write-Host "Note: Even if FLUSH PRIVILEGES failed, MySQL often applies GRANT changes immediately." -ForegroundColor Yellow
