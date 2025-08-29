@echo off
echo 背景色変更テストを開始します...

REM アプリケーションを起動
echo アプリケーションを起動します...
start BrowserChooser3.exe

echo.
echo 手順:
echo 1. アプリケーションが起動したら、オプション画面を開いてください
echo 2. 背景色を変更してください
echo 3. メイン画面に戻って、ブラウザアイコンが表示されているか確認してください
echo 4. ログファイルを確認して、デバッグ情報を確認してください
echo.
echo ログファイルの場所: %APPDATA%\BrowserChooser3\logs\
echo.

pause
