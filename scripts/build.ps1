# Build script for PanoramicData.Os (Windows PowerShell)
# Run this from the repository root

param(
    [string]$Command = "build",
    [switch]$Rebuild
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir

# Configuration
$ImageName = "panos-builder"
$ContainerName = "panos-build"
$OutputDir = Join-Path $RepoRoot "output"

Write-Host "=== PanoramicData.Os Build System ===" -ForegroundColor Cyan
Write-Host "Repository: $RepoRoot"
Write-Host "Command: $Command"

# Create output directory
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# Check if image exists
$imageExists = docker images -q $ImageName 2>$null
if (-not $imageExists -or $Rebuild) {
    Write-Host "Building Docker image..." -ForegroundColor Yellow
    docker build -t $ImageName $RepoRoot
    if ($LASTEXITCODE -ne 0) { throw "Docker build failed" }
}

# Create build directory for intermediate files
$BuildDir = Join-Path $RepoRoot "build"
if (-not (Test-Path $BuildDir)) {
    New-Item -ItemType Directory -Path $BuildDir | Out-Null
}

# Run build in container
Write-Host "Running build in container..." -ForegroundColor Yellow
docker run --rm `
    -v "${RepoRoot}/src:/workspace/src" `
    -v "${RepoRoot}/configs:/workspace/configs:ro" `
    -v "${RepoRoot}/scripts:/workspace/scripts:ro" `
    -v "${BuildDir}:/workspace/build" `
    -v "${OutputDir}:/output" `
    --name $ContainerName `
    $ImageName `
    $Command

if ($LASTEXITCODE -ne 0) { throw "Build failed" }

Write-Host ""
Write-Host "=== Build Output ===" -ForegroundColor Green
Get-ChildItem $OutputDir | Format-Table Name, Length, LastWriteTime
