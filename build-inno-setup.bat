@echo off
echo Building BrowserChooser3 with Inno Setup...

REM リリースビルド
echo Building application...
dotnet build BrowserChooser3/BrowserChooser3.csproj -c Release

if %ERRORLEVEL% NEQ 0 (
    echo Application build failed!
    pause
    exit /b 1
)

REM Inno Setupでビルド
echo Building installer with Inno Setup...

REM Inno Setupのパスを確認
set INNO_PATH=C:\Program Files (x86)\Inno Setup 6\ISCC.exe
if not exist "%INNO_PATH%" (
    set INNO_PATH=C:\Program Files\Inno Setup 6\ISCC.exe
)

if not exist "%INNO_PATH%" (
    echo Inno Setup not found!
    echo Please install Inno Setup 6 from https://jrsoftware.org/isinfo.php
    pause
    exit /b 1
)

echo Using Inno Setup from: %INNO_PATH%

REM Inno Setupでビルド
"%INNO_PATH%" BrowserChooser3-Setup.iss

if %ERRORLEVEL% NEQ 0 (
    echo Inno Setup build failed!
    pause
    exit /b 1
)

echo.
echo Build complete! Check dist folder for installer.
echo Generated file: dist\BrowserChooser3-Setup.exe

REM ファイルサイズを表示
if exist "dist\BrowserChooser3-Setup.exe" (
    for %%A in ("dist\BrowserChooser3-Setup.exe") do (
        echo File size: %%~zA bytes
    )
)

