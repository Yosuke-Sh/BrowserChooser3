# BrowserChooser3 ポータブル版作成スクリプト
# 使用方法: .\create-portable-release.ps1
# バージョン: 0.1.2

param(
    [string]$Version = "0.1.2"
)

Write-Host "BrowserChooser3 ポータブル版作成開始 (v$Version)" -ForegroundColor Green

# リリースディレクトリの作成
$releaseDir = "Release\BrowserChooser3-v$Version"
if (Test-Path $releaseDir) {
    Remove-Item -Path $releaseDir -Recurse -Force
}
New-Item -ItemType Directory -Path $releaseDir -Force | Out-Null

Write-Host "リリースディレクトリを作成しました: $releaseDir" -ForegroundColor Green

# ビルドの実行
Write-Host "アプリケーションをビルドしています..." -ForegroundColor Yellow
dotnet build BrowserChooser3\BrowserChooser3.csproj -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "ビルドに失敗しました" -ForegroundColor Red
    exit 1
}

Write-Host "ビルドが完了しました" -ForegroundColor Green

# 必要なファイルをコピー
$sourceDir = "BrowserChooser3\bin\Release\net8.0-windows"
$filesToCopy = @(
    "BrowserChooser3.exe",
    "BrowserChooser3.dll",
    "BrowserChooser3.deps.json",
    "BrowserChooser3.runtimeconfig.json",
    "BrowserChooser3.xml"
)

foreach ($file in $filesToCopy) {
    $sourcePath = Join-Path $sourceDir $file
    if (Test-Path $sourcePath) {
        Copy-Item -Path $sourcePath -Destination $releaseDir -Force
        Write-Host "コピー完了: $file" -ForegroundColor Green
    } else {
        Write-Host "警告: ファイルが見つかりません: $file" -ForegroundColor Yellow
    }
}

# ポータブル版用のiniファイルを動的に生成
$portableIniContent = @"
[Paths]
; 出力フォルダの選択
; true: exe実行フォルダに出力（ポータブル版用）
; false: ユーザーディレクトリに出力（インストーラー版用）
UseExeDirectory=true

; ログファイルの出力先
; 空の場合: UseExeDirectoryの設定に従う
; 指定した場合: そのフォルダに出力
LogDirectory=

; 設定ファイルの出力先
; 空の場合: UseExeDirectoryの設定に従う
; 指定した場合: そのフォルダに出力
ConfigDirectory=
"@

$portableIniContent | Out-File -FilePath "$releaseDir\BrowserChooser3.ini" -Encoding UTF8
Write-Host "ポータブル版用のiniファイルを生成しました: $releaseDir\BrowserChooser3.ini" -ForegroundColor Green

# ZIPファイルの作成
$zipPath = "Release\BrowserChooser3-v$Version.zip"
if (Test-Path $zipPath) {
    Remove-Item -Path $zipPath -Force
}

Write-Host "ZIPファイルを作成しています..." -ForegroundColor Yellow
Compress-Archive -Path $releaseDir -DestinationPath $zipPath -Force

if (Test-Path $zipPath) {
    $zipSize = (Get-Item $zipPath).Length
    Write-Host "ZIPファイル作成完了: $zipPath" -ForegroundColor Green
    Write-Host "ファイルサイズ: $($zipSize) bytes" -ForegroundColor Green
} else {
    Write-Host "ZIPファイルの作成に失敗しました" -ForegroundColor Red
}

Write-Host "ポータブル版作成完了!" -ForegroundColor Green
Write-Host "リリースディレクトリ: $releaseDir" -ForegroundColor Cyan
Write-Host "ZIPファイル: $zipPath" -ForegroundColor Cyan
