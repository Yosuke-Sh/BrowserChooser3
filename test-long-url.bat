@echo off
echo 長いURLテストを開始します...

REM 長いURLを指定してアプリケーションを起動
echo テスト1: 通常の長いURL
BrowserChooser3.exe "https://www.google.com/search?q=goole&oq=goole&gs_lcrp=EgZjaHJvbWUyBggAEEUYOTIPCAEQABgKGIMBGLEDGIAEMg8IAhAAGAoYgwEYsQMYgAQyDwgDEAAYChiDARixAxiABDIPCAQQABgKGIMBGLEDGIAEMg8IBRAAGAoYgwEYsQMYgAQyDwgGEAAYChiDARixAxiABDIGCAcQRRg80gEIMjA3M2owajSoAgawAgHxBaFbFMlJv5Ac&sourceid=chrome&ie=UTF-8"

echo.
echo テスト2: 非常に長いURL（8191文字超）
set LONG_URL=https://example.com/
for /L %%i in (1,1,100) do set LONG_URL=!LONG_URL!a
BrowserChooser3.exe "!LONG_URL!"

echo.
echo テスト3: URLエンコーディングを含むURL
BrowserChooser3.exe "https://example.com/search?q=test%20space&param=value%3D123"

echo.
echo テスト完了
pause
