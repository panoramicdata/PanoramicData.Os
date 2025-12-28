# Run QEMU for PanoramicData.Os (Windows PowerShell)
# This script boots the OS image in QEMU with serial console output

param(
    [switch]$Graphics,
    [string]$Memory = "512M",
    [int]$Cpus = 2,
    [string]$Kernel = "",
    [string]$Initrd = ""
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$OutputDir = Join-Path $RepoRoot "output"

# Set defaults if not provided
if (-not $Kernel) { $Kernel = Join-Path $OutputDir "vmlinuz" }
if (-not $Initrd) { $Initrd = Join-Path $OutputDir "initramfs.cpio.gz" }

# Check if files exist
if (-not (Test-Path $Kernel)) {
    Write-Host "Error: Kernel not found at $Kernel" -ForegroundColor Red
    Write-Host "Run .\scripts\build.ps1 first to build the image."
    exit 1
}

if (-not (Test-Path $Initrd)) {
    Write-Host "Error: Initrd not found at $Initrd" -ForegroundColor Red
    Write-Host "Run .\scripts\build.ps1 first to build the image."
    exit 1
}

Write-Host "=== PanoramicData.Os QEMU Runner ===" -ForegroundColor Cyan
Write-Host "Kernel: $Kernel"
Write-Host "Initrd: $Initrd"
Write-Host "Memory: $Memory"
Write-Host "CPUs: $Cpus"
Write-Host "Graphics: $(if ($Graphics) { 'Enabled' } else { 'Disabled' })"
Write-Host ""
Write-Host "Press Ctrl+A, X to exit QEMU" -ForegroundColor Yellow
Write-Host "===================================" -ForegroundColor Cyan

# Build QEMU arguments
# Note: On Windows, use WHPX or TCG instead of KVM
$QemuArgs = @(
    "-kernel", $Kernel,
    "-initrd", $Initrd,
    "-append", "console=ttyS0,115200n8 console=tty0 panic=1",
    "-m", $Memory,
    "-smp", $Cpus,
    "-device", "virtio-net-pci,netdev=net0",
    "-netdev", "user,id=net0,hostfwd=tcp::2222-:22",
    "-device", "virtio-rng-pci"
)

# Try to enable hardware acceleration
# Check for WHPX (Windows Hypervisor Platform)
$whpxAvailable = $false
try {
    $whpxCheck = & qemu-system-x86_64 -accel help 2>&1
    if ($whpxCheck -match "whpx") {
        $whpxAvailable = $true
    }
} catch {}

if ($whpxAvailable) {
    Write-Host "Using WHPX acceleration" -ForegroundColor Green
    $QemuArgs += @("-accel", "whpx", "-cpu", "max")
} else {
    Write-Host "Using TCG (software emulation)" -ForegroundColor Yellow
    $QemuArgs += @("-cpu", "max")
}

# Add graphics options
if ($Graphics) {
    $QemuArgs += @("-display", "sdl", "-serial", "stdio")
} else {
    $QemuArgs += @("-nographic", "-serial", "mon:stdio")
}

# Run QEMU
& qemu-system-x86_64 @QemuArgs
