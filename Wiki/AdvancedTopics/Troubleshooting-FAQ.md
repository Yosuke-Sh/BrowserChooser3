# トラブルシューティング・FAQ

## よくある問題

**アプリケーションが起動しない**
.NET 10.0 Desktop Runtimeがインストールされているか`dotnet --list-runtimes`で確認してください（インストーラー版は未導入時に自動導入します）。それでも起動しない場合は管理者権限で実行するか、アンチウイルスの除外リストに追加してください。

**ブラウザが検出されない**
「Browsers & applications」タブの「Detect」で再検出するか、手動で追加してください。

**透明化が動作しない**
Windowsの透明化効果（設定 > 個人用設定 > 色）を有効にし、グラフィックドライバーを更新してください。「Display」タブで「Enable Transparency」がオンになっているかも確認してください。

**既定のブラウザに設定したのに警告ログが出る（v0.2.2で修正済み）**
v0.2.1以前は既定ブラウザの判定にWindows 8以前の仕組み（`HKCR\http\shell\open\command`）のみを参照しており、Windows 11では既定に設定していても誤って「未設定」と判定されていました。v0.2.2でWindows 11以降が実際に使用する`UserChoice`レジストリキーを参照するよう修正済みです。v0.2.2以降でも解消しない場合は、設定アプリから**HTTPとHTTPSの両方**を個別にBrowserChooser3へ設定してください（片方だけでは既定と認識されません）。

**設定が保存されない**
`%APPDATA%\BrowserChooser3`への書き込み権限とディスク容量を確認してください。

## デバッグ方法

1. 「Privacy」タブで「Enable Logging」を有効化し、ログレベルを選択
2. `%LOCALAPPDATA%\BrowserChooser3\Logs\`のCSVログを確認
3. コマンドラインで`BrowserChooser3.exe --debug`を付けて起動するとデバッグログが有効になる

Auto URLs/Protocolsが期待通りに動かない場合は、ログレベルをTraceにして「マッチング成功」ログを確認してください。優先順位はAuto URLs > Protocols > 通常処理です。

## 既知の制限

**混在DPI環境での表示**
モニターごとにDPIスケーリング倍率が異なる環境では、副モニターに表示した際の見た目が最適化されない場合があります（アプリのDPIモードが`SystemAware`のため）。

## その他のFAQ

**自動アップデート機能はありますか？** — ありません。新しいバージョンは[Releases](https://github.com/Yosuke-Sh/BrowserChooser3/releases)から手動でダウンロードしてください。

**個人情報は収集されますか？** — いいえ。設定はローカル（`%APPDATA%\BrowserChooser3`）にのみ保存されます。

**自動起動を設定できますか？** — アプリ内には設定項目がありません。`shell:startup`フォルダにショートカットを作成するか、タスクスケジューラーで設定してください。

## 問題を報告する

[GitHub Issues](https://github.com/Yosuke-Sh/BrowserChooser3/issues)に、Windows/.NETのバージョン、再現手順、ログファイル（該当する場合）を添えて報告してください。機能要望も同じ場所で受け付けています。
