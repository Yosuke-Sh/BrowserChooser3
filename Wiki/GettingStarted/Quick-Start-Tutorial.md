# クイックスタートチュートリアル

BrowserChooser3を素早く使い始めるためのガイドです。

## 🚀 5分で始めるBrowserChooser3

### ステップ1: ダウンロードとインストール
1. [リリースページ](https://github.com/Yosuke-Sh/BrowserChooser3/releases)から最新版をダウンロード
2. インストーラー版またはポータブル版を選択
3. インストールまたは展開

### ステップ2: 初回起動
1. `BrowserChooser3.exe`をダブルクリック
2. アプリケーションが起動し、設定ファイルが自動生成されます

### ステップ3: 基本設定
1. `O`キーを押してオプションダイアログを開く
2. 「Browsers」タブでブラウザを確認・追加
3. 「Display」タブで外観をカスタマイズ

### ステップ4: 初回使用
1. コマンドラインから起動：
   ```cmd
   BrowserChooser3.exe https://www.google.com
   ```
2. ブラウザ選択画面が表示されます
3. 使用したいブラウザをクリックまたはキーで選択

## 🎯 基本的な使用方法

### URLを開く
```cmd
# 基本的な使用方法
BrowserChooser3.exe https://example.com

# 複数のURLを同時に開く
BrowserChooser3.exe https://google.com https://github.com
```

### キーボードショートカット
- `O`: オプションダイアログを開く
- `Esc`: アプリケーションを終了
- `Tab`: ブラウザ間を移動
- `Enter`: 選択したブラウザでURLを開く

## ⚙️ 最初の設定

### ブラウザの追加
1. オプションダイアログを開く（`O`キー）
2. 「Browsers」タブを選択
3. 「Add」ボタンをクリック
4. ブラウザの名前、パス、アイコンを設定

### 外観のカスタマイズ
1. 「Display」タブを選択
2. 透明度を調整
3. 角の丸みを設定
4. 背景色を変更

### アクセシビリティ設定
1. 「Focus」タブを選択
2. フォーカスボックスの色と幅を設定
3. キーボードナビゲーションを有効化

## 🔧 よく使う機能

### システムトレイ常駐
1. 「Startup」タブを選択
2. 「Always Resident in System Tray」を有効化
3. アプリケーションがバックグラウンドで動作

### 自動起動設定
1. 「Startup」タブを選択
2. 起動オプションを設定：
   - Start Minimized: 最小化で起動
   - Start in System Tray: システムトレイで起動

### プロトコルハンドラー
1. 「Protocols」タブを選択
2. カスタムプロトコルを追加
3. 特定のURLパターンにブラウザを割り当て

## 🎨 カスタマイズ例

### 透明なウィンドウ
```json
{
  "Transparency": 0.8,
  "CornerRadius": 10,
  "BackgroundColor": "#2D2D30"
}
```

### カスタムブラウザ設定
```json
{
  "Name": "Custom Browser",
  "Target": "C:\\Path\\To\\Browser.exe",
  "Arguments": "--new-window {url}",
  "IconPath": "C:\\Path\\To\\icon.ico"
}
```

## 🚨 トラブルシューティング

### よくある問題

**問題**: アプリケーションが起動しない
- **解決策**: .NET 8.0 Runtimeをインストール

**問題**: ブラウザが検出されない
- **解決策**: 手動でブラウザを追加

**問題**: 透明化が動作しない
- **解決策**: Windows透明化効果を有効化

## 📚 次のステップ

基本的な使用方法をマスターしたら、以下を試してみてください：

1. [設定ガイド](Configuration-Guide) - 詳細な設定方法
2. [透明化設定](Transparency-Settings) - 高度な外観カスタマイズ
3. [アクセシビリティ機能](Accessibility-Features) - アクセシビリティ設定
4. [トラブルシューティング](Troubleshooting) - 問題解決

## 💡 ヒント

- 設定は自動的に保存されます
- ポータブル版は任意のフォルダに配置可能
- コマンドライン引数でURLを直接指定可能
- システムトレイ常駐でバックグラウンド動作

---

*より詳細な情報については、[設定ガイド](Configuration-Guide)をご覧ください。*
