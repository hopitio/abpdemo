#!/usr/bin/env pwsh

Write-Host "Starting Supplier Seeding..." -ForegroundColor Green

# Set environment variable to seed suppliers only
$env:SeedSuppliersOnly = "true"

try {
    # Navigate to DbMigrator directory
    Set-Location "c:\www\abpAngular\AbpAngular\src\AbpAngular.DbMigrator"
    
    # Run the DbMigrator with supplier seeding
    Write-Host "Running DbMigrator for supplier seeding..." -ForegroundColor Yellow
    dotnet run --no-build
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Supplier seeding completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Supplier seeding failed!" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
} finally {
    # Clean up environment variable
    Remove-Item Env:SeedSuppliersOnly -ErrorAction SilentlyContinue
}
