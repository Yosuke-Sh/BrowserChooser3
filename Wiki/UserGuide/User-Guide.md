# 使い方ガイド

インストールから設定までの実用リファレンスです。導入手順や機能一覧は[README](https://github.com/Yosuke-Sh/BrowserChooser3#readme)を参照してください。ここではWikiでしか触れていない詳細・具体的な操作手順のみをまとめます。

## 起動とブラウザ選択

```bash
BrowserChooser3.exe https://example.com
```

- `Tab`/矢印キー: ブラウザ間を移動、`Enter`: 選択したブラウザで開く、`Esc`: キャンセル、`O`: オプション画面を開く
- 数字キー（0-9）で対応するブラウザを直接起動できます

## コマンドラインオプション

`BrowserChooser3.exe [URL] [オプション]`。実装（`CommandLineProcessor.cs`）でサポートされているのはこれだけです：

| オプション | 短縮形 | 説明 |
|---|---|---|
| `--help` | `-h` | ヘルプを表示 |
| `--version` | `-v` | バージョン情報を表示 |
| `--delay <秒>` | `-d` | 指定秒数後にブラウザを起動 |
| `--browser <GUID>` | `-b` | 指定ブラウザのGUIDで起動 |
| `--unshorten` | `-u` | URL短縮解除 |
| `--debug` | - | デバッグログを有効化 |
| `--ignore-settings` | - | 設定ファイルを無視 |
| `--silent` | - | UIなしで既定ブラウザへ即座に渡す |
| `--auto-launch` | - | 遅延なしで即座に既定ブラウザを起動 |

環境変数`BROWSERCHOOSER_DEBUG=true`/`BROWSERCHOOSER_IGNORE_SETTINGS=true`でも同等の指定ができます。

## オプション画面の設定タブ

`O`キーで開きます。実際のタブ構成は以下の9つです（「Focus」「Windows Default」等の名称は古いドキュメントの誤りです）。

### Browsers & applications
ブラウザの追加・編集・複製・削除、「Detect」による自動検出。項目：Name / Target（実行ファイルパス）/ Arguments / Icon。

**Arguments**にURLを直接渡す必要はありません。指定しない場合は起動時にURLが自動的に末尾へ付加されます。プロトコル部分と残り部分を明示的に組み立てたい場合は`{0}`（プロトコル、例：`https`）と`{1}`（残り）が使えます。

### Auto URLs
URLパターンにマッチしたら自動的に指定ブラウザへ遅延起動する機能。パターンは`*`によるワイルドカード、または`re:`接頭辞で正規表現を指定できます。項目：URL Pattern / Browser / Delay（秒、空欄で既定値）。

### Protocols
プロトコル（`http`/`https`/カスタムプロトコル）ごとに対応ブラウザを割り当てます。

### Display
透明化（Enable Transparency / Opacity 0.01-1.00、既定0.8）、タイトルバー非表示、角の丸み（Rounded Corners Radius 0-50、既定20、0で無効）、背景色・背景グラデーション、**Startup Position**（マウスカーソルのある画面の中央／プライマリ画面の中央、v0.2.2以降）。

色ベースの透明化（マゼンタキー方式）はv0.2.1で廃止され、現在は`Opacity`のみで半透明化します。

### Accessibility
フォーカス表示（Show Focus / Show Visual Focus）、Focus Box Color / Line Width、Use Accessible Rendering（アクセシビリティAPIとの互換性向上。個別のスクリーンリーダー製品ごとの動作保証はありません）。

### Grid
ブラウザ選択画面のアイコングリッドの列数・行数（Width/Height、既定5/1）。

### Privacy
ログの有効/無効（既定：有効）とログレベル（None/Error/Warning/Info/Debug/Trace）。ログは`%LOCALAPPDATA%\BrowserChooser3\Logs\`にCSV形式で出力されます。

### Startup
- **Start in System Tray**: システムトレイに格納した状態で起動
- **Always Resident in System Tray**: ウィンドウを閉じても常駐（完全終了はトレイメニューの「Exit」から）
- **Startup Delay**: 起動遅延（**ミリ秒**、0-10000、既定0）
- **Startup Message**: 起動時メッセージ（既定は空欄。変数展開は非対応）

自動起動（Windows起動時）の設定項目はアプリ内にはありません。必要なら`shell:startup`フォルダにショートカットを作成するか、タスクスケジューラーで設定してください。

### Others
ブラウザ選択後にウィンドウを開いたままにするか、デフォルト遅延時間（既定5ms）、セパレーター文字（既定` - `）、ユーザーエージェント、オプション画面のショートカットキー（既定`O`）、既定メッセージ（既定"Choose a Browser"）。

## 既定のブラウザに設定する

Windows 11ではアプリからプログラム的に既定ブラウザを変更できません。設定アプリから手動で行う必要があります：

1. 「設定」→「アプリ」→「既定のアプリ」→「BrowserChooser3」を検索
2. **HTTPとHTTPSの両方**を個別にBrowserChooser3へ設定する（片方だけでは既定と認識されません）

v0.2.2以降、初回起動時にこの設定画面が自動的に開きます。

## 設定ファイル

`%APPDATA%\BrowserChooser3\BrowserChooser3Config.xml`にフラットなXML構造で保存されます（`DisplaySettings`のようなネストしたグループ要素はありません）。テキストエディタで直接編集も可能ですが、構文エラーに注意してください。

---

*より詳細な情報は[トラブルシューティング・FAQ](Troubleshooting-FAQ)、開発者向け情報は[開発ガイド](Development)を参照してください。*
