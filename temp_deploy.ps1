#!/usr/bin/env pwsh
# ═══════════════════════════════════════════════════════════════════════
#  deploy.ps1  —  Azure App Service Deployment (Zip Deploy)
#  Phantoms API  ·  Onion Architecture  ·  .NET 10
#
#  Usage:  Run from the solution root  →  .\deploy.ps1
# ═══════════════════════════════════════════════════════════════════════

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ── Azure Configuration ─────────────────────────────────────────────
$RESOURCE_GROUP = "OnionApiRG"
$WEB_APP_NAME   = "codephantoms"
$APP_URL        = "https://${WEB_APP_NAME}.azurewebsites.net"

# ── Project Paths ───────────────────────────────────────────────────
$SOLUTION_ROOT  = $PSScriptRoot
$API_PROJECT    = Join-Path $SOLUTION_ROOT "src\Presentation\Phantoms.API\Phantoms.API.csproj"
$PUBLISH_DIR    = Join-Path $SOLUTION_ROOT "publish"
$PUBLISH_ZIP    = Join-Path $SOLUTION_ROOT "publish.zip"

# ── Helper Functions ────────────────────────────────────────────────

function Write-StepHeader {
    param(
        [string]$StepNumber,
        [string]$Title
    )
    Write-Host ""
    Write-Host "  ╔══════════════════════════════════════════════════════════╗" -ForegroundColor DarkCyan
    Write-Host "  ║  [$StepNumber]  $Title" -ForegroundColor Cyan -NoNewline
    # Pad to align the closing box border
    $padding = 56 - $StepNumber.Length - $Title.Length - 5
    if ($padding -lt 0) { $padding = 0 }
    Write-Host (" " * $padding) -NoNewline
    Write-Host "║" -ForegroundColor DarkCyan
    Write-Host "  ╚══════════════════════════════════════════════════════════╝" -ForegroundColor DarkCyan
    Write-Host ""
}

function Write-Success {
    param([string]$Message)
    Write-Host "  ✅ $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "  ℹ️  $Message" -ForegroundColor Gray
}

function Write-Warn {
    param([string]$Message)
    Write-Host "  ⚠️  $Message" -ForegroundColor Yellow
}

function Stop-WithError {
    param([string]$Message)
    Write-Host ""
    Write-Host "  ╔══════════════════════════════════════════════════════════╗" -ForegroundColor Red
    Write-Host "  ║  ❌ ERROR OCCURRED!                                      ║" -ForegroundColor Red
    Write-Host "  ╚══════════════════════════════════════════════════════════╝" -ForegroundColor Red
    Write-Host ""
    Write-Host "  $Message" -ForegroundColor Red
    Write-Host ""
    exit 1
}

# ═══════════════════════════════════════════════════════════════════════
#  STARTUP BANNER
# ═══════════════════════════════════════════════════════════════════════

Write-Host ""
Write-Host "  ██████╗ ██╗  ██╗ █████╗ ███╗   ██╗████████╗ ██████╗ ███╗   ███╗███████╗" -ForegroundColor Magenta
Write-Host "  ██╔══██╗██║  ██║██╔══██╗████╗  ██║╚══██╔══╝██╔═══██╗████╗ ████║██╔════╝" -ForegroundColor Magenta
Write-Host "  ██████╔╝███████║███████║██╔██╗ ██║   ██║   ██║   ██║██╔████╔██║███████╗" -ForegroundColor Magenta
Write-Host "  ██╔═══╝ ██╔══██║██╔══██║██║╚██╗██║   ██║   ██║   ██║██║╚██╔╝██║╚════██║" -ForegroundColor Magenta
Write-Host "  ██║     ██║  ██║██║  ██║██║ ╚████║   ██║   ╚██████╔╝██║ ╚═╝ ██║███████║" -ForegroundColor Magenta
Write-Host "  ╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝╚══════╝" -ForegroundColor Magenta
Write-Host ""
Write-Host "  Azure App Service Deployment" -ForegroundColor DarkGray
Write-Host "  ─────────────────────────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""

# ═══════════════════════════════════════════════════════════════════════
#  STEP 0 — Pre-flight Configuration Check
# ═══════════════════════════════════════════════════════════════════════

Write-StepHeader "0/5" "Pre-flight Configuration Check"

Write-Warn "Please verify the following before deploying:"
Write-Host ""
Write-Host "    1. appsettings.json → ConnectionStrings:DefaultConnection" -ForegroundColor Yellow
Write-Host "       Must point to your Azure PostgreSQL Flexible Server." -ForegroundColor DarkYellow
Write-Host ""
Write-Host "    2. appsettings.json → CORS Settings" -ForegroundColor Yellow
Write-Host "       Ensure your frontend URLs are listed in AllowedOrigins." -ForegroundColor DarkYellow
Write-Host ""
Write-Host "    3. appsettings.json → JwtSettings, SmtpSettings" -ForegroundColor Yellow
Write-Host "       Ensure production values are properly configured." -ForegroundColor DarkYellow
Write-Host ""

$confirmation = Read-Host "  Configuration is ready? Type [Y/y] to continue"
if ($confirmation -notin @("Y", "y")) {
    Write-Host ""
    Write-Warn "Deployment cancelled. Update your configuration and try again."
    Write-Host ""
    exit 0
}

Write-Success "Configuration confirmed. Proceeding..."

# ═══════════════════════════════════════════════════════════════════════
#  STEP 1 — Cleanup Previous Artifacts
# ═══════════════════════════════════════════════════════════════════════

Write-StepHeader "1/5" "Cleaning Up Previous Artifacts"

if (Test-Path $PUBLISH_DIR) {
    Remove-Item -Path $PUBLISH_DIR -Recurse -Force
    Write-Info "Removed old 'publish' directory."
} else {
    Write-Info "'publish' directory not found — already clean."
}

if (Test-Path $PUBLISH_ZIP) {
    Remove-Item -Path $PUBLISH_ZIP -Force
    Write-Info "Removed old 'publish.zip' file."
} else {
    Write-Info "'publish.zip' not found — already clean."
}

Write-Success "Cleanup completed."

# ═══════════════════════════════════════════════════════════════════════
#  STEP 2 — Build & Publish
# ═══════════════════════════════════════════════════════════════════════

Write-StepHeader "2/5" "Build and Publish (Release)"

# Verify the project file exists
if (-not (Test-Path $API_PROJECT)) {
    Stop-WithError "API project file not found: $API_PROJECT"
}

Write-Info "Running dotnet publish..."
Write-Info "Configuration: Release | Output: $PUBLISH_DIR"
Write-Host ""

dotnet publish $API_PROJECT `
    --configuration Release `
    --output $PUBLISH_DIR `
    --no-self-contained

if ($LASTEXITCODE -ne 0) {
    Stop-WithError "dotnet publish failed! (Exit Code: $LASTEXITCODE)`nCheck the build errors above."
}

# Verify the publish directory is not empty
$publishedFiles = Get-ChildItem -Path $PUBLISH_DIR -Recurse -File
if ($publishedFiles.Count -eq 0) {
    Stop-WithError "Publish directory is empty! Something went wrong during the build."
}

Write-Success "Build completed successfully. ($($publishedFiles.Count) files published)"

# ═══════════════════════════════════════════════════════════════════════
#  STEP 3 — Create ZIP Archive
# ═══════════════════════════════════════════════════════════════════════

Write-StepHeader "3/5" "Creating ZIP Archive"

Write-Info "Compressing files with Compress-Archive..."

try {
    Compress-Archive -Path "$PUBLISH_DIR\*" -DestinationPath $PUBLISH_ZIP -Force
} catch {
    Stop-WithError "Failed to create ZIP archive!`n$($_.Exception.Message)"
}

# Verify the ZIP file was created
if (-not (Test-Path $PUBLISH_ZIP)) {
    Stop-WithError "publish.zip could not be created!"
}

$zipSize = [math]::Round((Get-Item $PUBLISH_ZIP).Length / 1MB, 2)
Write-Success "ZIP archive created: publish.zip ($zipSize MB)"

# ═══════════════════════════════════════════════════════════════════════
#  STEP 4 — Deploy to Azure App Service
# ═══════════════════════════════════════════════════════════════════════

Write-StepHeader "4/5" "Deploying to Azure App Service"

# Check if Azure CLI is installed
$azCmd = Get-Command az -ErrorAction SilentlyContinue
if (-not $azCmd) {
    Stop-WithError "Azure CLI (az) not found!`nInstall it from: https://aka.ms/installazurecli"
}

# Check Azure login status
Write-Info "Checking Azure authentication status..."
$azAccount = az account show --output json 2>$null | ConvertFrom-Json -ErrorAction SilentlyContinue
if (-not $azAccount) {
    Write-Warn "You are not logged in to Azure. Running 'az login'..."
    az login
    if ($LASTEXITCODE -ne 0) {
        Stop-WithError "Azure login failed!"
    }
}

Write-Info "Azure Account : $($azAccount.user.name)"
Write-Info "Subscription  : $($azAccount.name)"
Write-Host ""

Write-Info "Deploying to $WEB_APP_NAME ($RESOURCE_GROUP)..."
Write-Info "Method: ZIP Deploy"
Write-Host ""

az webapp deploy `
    --resource-group $RESOURCE_GROUP `
    --name $WEB_APP_NAME `
    --src-path $PUBLISH_ZIP `
    --type zip `
    --clean true `
    --restart true

if ($LASTEXITCODE -ne 0) {
    Stop-WithError "Azure deployment failed! (Exit Code: $LASTEXITCODE)`nCheck the Azure CLI errors above."
}

Write-Success "Azure deployment completed."

# ═══════════════════════════════════════════════════════════════════════
#  STEP 5 — Deployment Summary
# ═══════════════════════════════════════════════════════════════════════

Write-StepHeader "5/5" "Deployment Summary"

Write-Host ""
Write-Host "  ╔══════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "  ║                                                        ║" -ForegroundColor Green
Write-Host "  ║   🚀 Deployment completed successfully!                ║" -ForegroundColor Green
Write-Host "  ║                                                        ║" -ForegroundColor Green
Write-Host "  ╚══════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "  🌐 API URL    : " -ForegroundColor DarkGray -NoNewline
Write-Host "$APP_URL" -ForegroundColor Cyan
Write-Host "  📚 Swagger UI : " -ForegroundColor DarkGray -NoNewline
Write-Host "$APP_URL/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "  ─────────────────────────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  Resource Group : $RESOURCE_GROUP" -ForegroundColor DarkGray
Write-Host "  Web App        : $WEB_APP_NAME" -ForegroundColor DarkGray
Write-Host "  Database       : PostgreSQL (Azure Flexible Server)" -ForegroundColor DarkGray
Write-Host "  ─────────────────────────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
