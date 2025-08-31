# BrowserChooser3 ポータブル版リリース作成スクリプト
# 使用方法: .\create-portable-release.ps1 -Version "0.1.0"

param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

# エラー時に停止
$ErrorActionPreference = "Stop"

Write-Host "BrowserChooser3 ポータブル版リリース作成開始..." -ForegroundColor Green
Write-Host "バージョン: $Version" -ForegroundColor Yellow

# プロジェクトディレクトリの確認
if (-not (Test-Path "BrowserChooser3\BrowserChooser3.csproj")) {
    throw "BrowserChooser3.csproj が見つかりません。正しいディレクトリで実行してください。"
}

# リリースディレクトリの作成
$releaseDir = "Release"
$portableDir = "$releaseDir\BrowserChooser3-v$Version"
$zipFile = "$releaseDir\BrowserChooser3-v$Version.zip"

if (Test-Path $releaseDir) {
    Write-Host "既存のReleaseディレクトリを削除中..." -ForegroundColor Yellow
    Remove-Item -Path $releaseDir -Recurse -Force
}

Write-Host "Releaseディレクトリを作成中..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path $releaseDir -Force | Out-Null
New-Item -ItemType Directory -Path $portableDir -Force | Out-Null

# リリースビルドの実行
Write-Host "リリースビルドを実行中..." -ForegroundColor Yellow
dotnet build BrowserChooser3\BrowserChooser3.csproj -c Release

if ($LASTEXITCODE -ne 0) {
    throw "ビルドに失敗しました。"
}

# ビルド出力ディレクトリの確認
$buildOutputDir = "BrowserChooser3\bin\Release\net8.0-windows"
if (-not (Test-Path $buildOutputDir)) {
    throw "ビルド出力ディレクトリが見つかりません: $buildOutputDir"
}

# 必要なファイルをコピー
Write-Host "ファイルをコピー中..." -ForegroundColor Yellow

# 実行ファイルとDLL
Copy-Item "$buildOutputDir\BrowserChooser3.exe" -Destination $portableDir -Force
Copy-Item "$buildOutputDir\BrowserChooser3.dll" -Destination $portableDir -Force

# ランタイム設定ファイル
Copy-Item "$buildOutputDir\BrowserChooser3.runtimeconfig.json" -Destination $portableDir -Force -ErrorAction SilentlyContinue
Copy-Item "$buildOutputDir\BrowserChooser3.deps.json" -Destination $portableDir -Force -ErrorAction SilentlyContinue

# 設定ファイル
Copy-Item "$buildOutputDir\BrowserChooser3.dll.config" -Destination $portableDir -Force -ErrorAction SilentlyContinue

# ドキュメント
Copy-Item "$buildOutputDir\BrowserChooser3.xml" -Destination $portableDir -Force -ErrorAction SilentlyContinue

# 依存DLL（Microsoft.Win32.Registry等）
Get-ChildItem "$buildOutputDir\*.dll" | Where-Object { $_.Name -ne "BrowserChooser3.dll" } | ForEach-Object {
    Copy-Item $_.FullName -Destination $portableDir -Force
}

# インストール手順ファイルの作成
$installText = @"
BrowserChooser3 v$Version ポータブル版

インストール手順:
1. このフォルダを任意の場所に展開してください
2. .NET 8.0 Runtimeがインストールされていることを確認してください
   - 未インストールの場合は以下からダウンロード:
   https://dotnet.microsoft.com/download/dotnet/8.0
3. BrowserChooser3.exeをダブルクリックして起動してください

使用方法:
- コマンドライン引数としてURLを指定:
  BrowserChooser3.exe https://example.com
- 設定変更は起動後、Oキーを押してオプション画面を開いてください

システム要件:
- Windows 10/11 x64
- .NET 8.0 Runtime

詳細情報:
https://github.com/Yosuke-Sh/BrowserChooser3
"@

$installText | Out-File -FilePath "$portableDir\INSTALL.txt" -Encoding UTF8

# READMEファイルの作成
$readmeText = @"
# BrowserChooser3 v$Version ポータブル版

Browser Chooser 2の後継として開発された、現代的なUIと高度な機能を提供するブラウザ選択アプリケーションです。

## 主な機能

- 複数のブラウザから選択してURLを開く
- 自動ブラウザ検出機能
- カスタムブラウザの追加・編集
- アイコン選択機能（.exe、.ico、画像ファイル対応）
- 透明化とカスタマイズ機能
- アクセシビリティ機能
- システムトレイ常駐機能

## 使用方法

1. .NET 8.0 Runtimeがインストールされていることを確認
2. BrowserChooser3.exeをダブルクリックして起動
3. コマンドライン引数としてURLを指定:
   ```
   BrowserChooser3.exe https://example.com
   ```
4. 設定変更は起動後、Oキーを押してオプション画面を開く

## システム要件

- Windows 10/11 x64
- .NET 8.0 Runtime

## ライセンス

MIT License

詳細情報: https://github.com/Yosuke-Sh/BrowserChooser3
"@

$readmeText | Out-File -FilePath "$portableDir\README.md" -Encoding UTF8

# ZIPファイルの作成
Write-Host "ZIPファイルを作成中..." -ForegroundColor Yellow
if (Test-Path $zipFile) {
    Remove-Item $zipFile -Force
}

# PowerShell 5.0以降のCompress-Archiveを使用
try {
    Compress-Archive -Path "$portableDir\*" -DestinationPath $zipFile -Force
    Write-Host "ZIPファイル作成完了: $zipFile" -ForegroundColor Green
} catch {
    Write-Warning "PowerShellのCompress-Archiveが使用できません。手動でZIPファイルを作成してください。"
    Write-Host "ファイルは以下のディレクトリに配置されています: $portableDir" -ForegroundColor Yellow
}

# ファイルサイズの確認
if (Test-Path $zipFile) {
    $zipSize = (Get-Item $zipFile).Length
    $zipSizeMB = [math]::Round($zipSize / 1MB, 2)
    Write-Host "ZIPファイルサイズ: $zipSizeMB MB" -ForegroundColor Green
}

# ディレクトリサイズの確認
$dirSize = (Get-ChildItem $portableDir -Recurse | Measure-Object -Property Length -Sum).Sum
$dirSizeMB = [math]::Round($dirSize / 1MB, 2)
Write-Host "展開後サイズ: $dirSizeMB MB" -ForegroundColor Green

Write-Host "`nポータブル版リリース作成完了！" -ForegroundColor Green
Write-Host "リリースファイル:" -ForegroundColor Yellow
Write-Host "  - ZIPファイル: $zipFile" -ForegroundColor Cyan
Write-Host "  - 展開ディレクトリ: $portableDir" -ForegroundColor Cyan
Write-Host "`n次のステップ:" -ForegroundColor Yellow
Write-Host "1. GitHub Releasesページでリリースを作成" -ForegroundColor White
Write-Host "2. ZIPファイルをアップロード" -ForegroundColor White
Write-Host "3. リリースノートを追加" -ForegroundColor White
