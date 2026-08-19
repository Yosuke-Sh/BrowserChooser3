# BrowserChooser3

![.NET](https://img.shields.io/badge/.NET-10.0-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![License](https://img.shields.io/badge/License-MIT-green)

BrowserChooser3は、Windows環境で複数のブラウザから選択してURLを開くためのアプリケーションです。Browser Chooser 2の後継として開発されました。

## 🚀 主要機能

- **ブラウザ選択**: 複数のブラウザから選択してURLを開く。自動検出、カスタム追加、アイコン選択（.exe/.ico/画像）に対応
- **外観カスタマイズ**: 透明化（Opacity）、角の丸み、背景色・グラデーション、タイトルバー非表示
- **マルチモニター対応**: 起動位置をマウスカーソルのある画面／プライマリ画面から選択可能（v0.2.2以降）
- **アクセシビリティ**: フォーカス表示、フォーカスボックスのカスタマイズ、アクセシブルレンダリング
- **プロトコルハンドラー**: カスタムプロトコルの登録、URL正規化、短縮URL展開
- **システムトレイ常駐**: バックグラウンド動作、起動遅延・起動メッセージ設定

## 📋 システム要件

- **OS**: Windows 10（1903以降）/ Windows 11、x64
- **.NET**: .NET 10.0 Desktop Runtime（インストーラーが未導入時は自動導入）

## 🛠️ インストール

1. [Releases](https://github.com/Yosuke-Sh/BrowserChooser3/releases)ページから最新の`BrowserChooser3-Setup.exe`をダウンロード
2. 管理者権限で実行し、インストールウィザードに従う

配布物はインストーラーのみです（ポータブル版はv0.1.4で廃止）。

### ソースからビルド
```bash
git clone https://github.com/Yosuke-Sh/BrowserChooser3.git
cd BrowserChooser3
dotnet build
dotnet test
```

インストーラー作成には[Inno Setup 6](https://jrsoftware.org/isinfo.php)が必要です：
```bash
.\build-inno-setup.bat
```

## 🎯 使用方法

```bash
BrowserChooser3.exe https://example.com
```

表示されたブラウザ一覧から選択します。`O`キーでオプション画面を開き、以下のタブから設定できます：

**Browsers & applications** / **Auto URLs** / **Protocols** / **Display** / **Accessibility** / **Grid** / **Privacy** / **Startup** / **Others**

主なコマンドラインオプション：`-d <秒>`（遅延起動）、`-b <GUID>`（ブラウザ指定）、`--silent`（サイレントモード）、`--debug`（デバッグログ）。詳細やその他の設定は[Wiki](https://github.com/Yosuke-Sh/BrowserChooser3/wiki)を参照してください。

## 📁 保存先

- 設定ファイル: `%APPDATA%\BrowserChooser3\BrowserChooser3Config.xml`
- ログファイル: `%LOCALAPPDATA%\BrowserChooser3\Logs\`（CSV形式、「Privacy」タブで有効化）

## 🐛 よくある問題

- **ブラウザが検出されない**: 「Browsers & applications」タブで手動追加、または「Detect」で再検出
- **既定のブラウザに設定したのに警告が出る**: Windows 11では設定アプリからHTTP・HTTPSの両方を個別に設定する必要があります（詳細は[Wiki](https://github.com/Yosuke-Sh/BrowserChooser3/wiki)参照）

## 🤝 貢献

1. リポジトリをクローンし、.NET 10.0 SDKでビルド
2. `dotnet test`でテストが通ることを確認
3. プルリクエストを作成

詳細は[開発ガイド](https://github.com/Yosuke-Sh/BrowserChooser3/wiki/Development)を参照してください。

## 📄 ライセンス

MITライセンス。詳細は[LICENSE](LICENSE)を参照してください。

## 📞 サポート

- **Issues**: [GitHub Issues](https://github.com/Yosuke-Sh/BrowserChooser3/issues)
- **Wiki**: [GitHub Wiki](https://github.com/Yosuke-Sh/BrowserChooser3/wiki)
