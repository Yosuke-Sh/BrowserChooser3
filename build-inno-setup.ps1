# BrowserChooser3 Inno Setup Build Script
param(
    [string]$BuildType = "Release",
    [switch]$SkipAppBuild = $false
)

Write-Host "Building BrowserChooser3 with Inno Setup..." -ForegroundColor Green

# リリースビルド
if (-not $SkipAppBuild) {
    Write-Host "Building application..." -ForegroundColor Yellow
    dotnet build BrowserChooser3/BrowserChooser3.csproj -c $BuildType
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Application build failed!" -ForegroundColor Red
        exit 1
    }
}

# Inno Setupでビルド
Write-Host "Building installer with Inno Setup..." -ForegroundColor Yellow

# Inno Setupのパスを確認
$InnoPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (-not (Test-Path $InnoPath)) {
    $InnoPath = "C:\Program Files\Inno Setup 6\ISCC.exe"
}

if (-not (Test-Path $InnoPath)) {
    Write-Host "Inno Setup not found!" -ForegroundColor Red
    Write-Host "Please install Inno Setup 6 from https://jrsoftware.org/isinfo.php" -ForegroundColor Red
    exit 1
}

Write-Host "Using Inno Setup from: $InnoPath" -ForegroundColor Cyan

# Inno Setupでビルド
& $InnoPath "BrowserChooser3-Setup.iss"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Inno Setup build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Build complete! Check dist folder for installer." -ForegroundColor Green
Write-Host "Generated file: dist\BrowserChooser3-Setup.exe" -ForegroundColor Cyan

# ファイルサイズを表示
$InstallerPath = "dist\BrowserChooser3-Setup.exe"
if (Test-Path $InstallerPath) {
    $FileSize = (Get-Item $InstallerPath).Length
    Write-Host "File size: $([math]::Round($FileSize / 1MB, 2)) MB" -ForegroundColor Cyan
} else {
    Write-Host "Installer file not found!" -ForegroundColor Red
}
