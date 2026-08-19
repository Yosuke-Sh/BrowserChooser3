# BrowserChooser3 リリースノート

各バージョンの詳細なリリースノートはリポジトリ直下の `RELEASE_NOTES_vX.Y.Z.md` を参照してください。ここでは要点のみをまとめます。

## 最新リリース

### v0.2.2
- マルチモニター環境での起動位置を改善（既定でマウスカーソルのある画面の中央に表示。オプションで従来のプライマリ画面中央にも変更可能）
- Windows 11で既定のブラウザに設定済みでも警告ログが出る不具合を修正（`UserChoice`レジストリベースの判定に変更）

### v0.2.1
- タイトルバー非表示設定が反映されない不具合を修正
- ログに記録されていた誤検知エラー・警告を解消

### v0.2.0
- .NET 10への移行
- ReadyToRunによる起動速度改善
- インストーラーに.NET 10 Desktop Runtimeの検出・自動導入処理を追加

### v0.1.4〜v0.1.7
- ポータブルモード機能（`UseExeDirectory`等）を完全に削除し、インストーラー版のみの配布に一本化
- 常駐モード中の強制終了バグ修正など、安定性向上

### v0.1.0〜v0.1.3
- 初期ベータリリース。アイコン選択機能、背景グラデーション、Focusタブ、システムトレイ常駐機能などを実装

## 📋 システム要件（現行）
- **OS**: Windows 10/11 (x64)
- **.NET**: .NET 10.0 Desktop Runtime（インストーラーが未導入時は自動導入）
- **メモリ**: 512MB以上
- **ディスク**: 10MB以上の空き容量

## 📦 配布パッケージ

インストーラー版（`BrowserChooser3-Setup.exe`）のみ。ポータブル版はv0.1.4で廃止されました。

## 📥 インストール方法

1. [Releases](https://github.com/Yosuke-Sh/BrowserChooser3/releases)ページから最新の`BrowserChooser3-Setup.exe`をダウンロード
2. 管理者権限で実行し、インストールウィザードに従う
3. 設定ファイルは`%APPDATA%\BrowserChooser3`に保存されます

## 📚 関連情報

- [インストールガイド](../GettingStarted/Installation-Guide)
- [クイックスタート](../GettingStarted/Quick-Start-Tutorial)
- [ユーザーガイド](../UserGuide/Basic-Usage)
- [トラブルシューティング](../AdvancedTopics/Troubleshooting)
- [既知の問題](Known-Issues)
- [貢献ガイドライン](../Development/Contributing-Guidelines)

### フィードバックチャンネル
- **GitHub Issues**: バグ報告・機能リクエスト — https://github.com/Yosuke-Sh/BrowserChooser3/issues
- **Wiki**: ドキュメント・ガイド — https://github.com/Yosuke-Sh/BrowserChooser3/wiki
