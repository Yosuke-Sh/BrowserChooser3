# BrowserChooser3 リリースパッケージ作成スクリプト
# PowerShell スクリプト

param(
    [string]$Version = "1.0.0",
    [string]$OutputDir = ".\Release"
)

Write-Host "BrowserChooser3 リリースパッケージ作成開始..." -ForegroundColor Green
Write-Host "バージョン: $Version" -ForegroundColor Yellow
Write-Host "出力ディレクトリ: $OutputDir" -ForegroundColor Yellow

# 出力ディレクトリの作成
if (Test-Path $OutputDir) {
    Remove-Item $OutputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

# プロジェクトディレクトリに移動
Set-Location ".\BrowserChooser3"

try {
    # クリーンビルド
    Write-Host "クリーンビルド実行中..." -ForegroundColor Cyan
    dotnet clean -c Release
    
    # リリース用パブリッシュ（フレームワーク依存）
    Write-Host "リリース用パブリッシュ実行中..." -ForegroundColor Cyan
    dotnet publish -c Release -r win-x64 --self-contained false -o "..\$OutputDir\BrowserChooser3-v$Version"
    
    if ($LASTEXITCODE -ne 0) {
        throw "パブリッシュに失敗しました"
    }
    
    # 不要なファイルを削除
    Write-Host "不要なファイルを削除中..." -ForegroundColor Cyan
    $publishDir = "..\$OutputDir\BrowserChooser3-v$Version"
    
    # デバッグファイルを削除
    Remove-Item "$publishDir\*.pdb" -ErrorAction SilentlyContinue
    
    # インストール手順ファイルを作成
    $installInstructions = @"
BrowserChooser3 v$Version Installation Guide
==========================================

1. System Requirements
   - Windows 10/11
   - .NET 8.0 Runtime (https://dotnet.microsoft.com/download/dotnet/8.0)

2. Installation
   - Copy this folder to any location
   - Double-click BrowserChooser3.exe to start

3. Usage
   - Command line: BrowserChooser3.exe https://example.com
   - Settings: Press 'O' key after starting the app

4. Uninstallation
   - Simply delete the folder

Support: https://github.com/Yosuke-Sh/BrowserChooser3
"@
    
    $installInstructions | Out-File -FilePath "$publishDir\INSTALL.txt" -Encoding UTF8 -NoNewline
    
    # ZIPファイル作成
    Write-Host "ZIPファイル作成中..." -ForegroundColor Cyan
    $zipPath = "..\$OutputDir\BrowserChooser3-v$Version.zip"
    Compress-Archive -Path "$publishDir\*" -DestinationPath $zipPath -Force
    
    # ファイルサイズ確認
    $zipSize = (Get-Item $zipPath).Length / 1MB
    Write-Host "ZIPファイルサイズ: $([math]::Round($zipSize, 2)) MB" -ForegroundColor Green
    
    Write-Host "リリースパッケージ作成完了!" -ForegroundColor Green
    Write-Host "ZIPファイル: $zipPath" -ForegroundColor Yellow
    
} catch {
    Write-Host "エラーが発生しました: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    # 元のディレクトリに戻る
    Set-Location ".."
}
