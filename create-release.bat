@echo off
REM BrowserChooser3 リリースパッケージ作成バッチファイル
REM 簡易版（PowerShellが利用できない環境用）

set VERSION=1.0.0
set OUTPUT_DIR=.\Release

echo BrowserChooser3 リリースパッケージ作成開始...
echo バージョン: %VERSION%
echo 出力ディレクトリ: %OUTPUT_DIR%

REM 出力ディレクトリの作成
if exist "%OUTPUT_DIR%" rmdir /s /q "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%"

REM プロジェクトディレクトリに移動
cd BrowserChooser3

REM クリーンビルド
echo クリーンビルド実行中...
dotnet clean -c Release

REM リリース用パブリッシュ
echo リリース用パブリッシュ実行中...
dotnet publish -c Release -r win-x64 --self-contained false -o "..\%OUTPUT_DIR%\BrowserChooser3-v%VERSION%"

if %ERRORLEVEL% neq 0 (
    echo パブリッシュに失敗しました
    cd ..
    exit /b 1
)

REM 元のディレクトリに戻る
cd ..

echo リリースパッケージ作成完了!
echo 出力先: %OUTPUT_DIR%\BrowserChooser3-v%VERSION%

pause
