# コマンドラインオプション

BrowserChooser3のコマンドラインオプションについて詳しく説明します。

## 🚀 基本的な使用方法

### 基本的な構文
```cmd
BrowserChooser3.exe [オプション] [URL...]
```

### 引数なしでの起動
```cmd
BrowserChooser3.exe
```
- オプションダイアログが表示されます
- 設定画面からブラウザの管理や外観のカスタマイズが可能

## 🌐 URL指定

### 単一URLの指定
```cmd
BrowserChooser3.exe https://www.google.com
```

### 複数URLの指定
```cmd
BrowserChooser3.exe https://www.google.com https://github.com https://stackoverflow.com
```

### ローカルファイルの指定
```cmd
BrowserChooser3.exe file:///C:/path/to/file.html
```

### 特殊プロトコルの指定
```cmd
BrowserChooser3.exe ftp://example.com
BrowserChooser3.exe mailto:user@example.com
```

## ⚙️ コマンドラインオプション

### ヘルプ表示
```cmd
BrowserChooser3.exe --help
BrowserChooser3.exe -h
```
- 利用可能なオプションの一覧を表示
- 使用方法の説明を表示

### バージョン表示
```cmd
BrowserChooser3.exe --version
BrowserChooser3.exe -v
```
- アプリケーションのバージョン情報を表示
- ビルド情報も含めて表示

### ログ有効化
```cmd
BrowserChooser3.exe --log
BrowserChooser3.exe -l
```
- ログ機能を有効化
- デバッグ情報をログファイルに出力

### デバッグモード
```cmd
BrowserChooser3.exe --debug
BrowserChooser3.exe -d
```
- デバッグモードで起動
- 詳細なデバッグ情報を表示

### 設定ファイル指定
```cmd
BrowserChooser3.exe --config "C:\Path\To\Config.xml"
BrowserChooser3.exe -c "C:\Path\To\Config.xml"
```
- カスタム設定ファイルを指定
- デフォルトの設定ファイルとは別のファイルを使用

### ポータブルモード
```cmd
BrowserChooser3.exe --portable
BrowserChooser3.exe -p
```
- ポータブルモードで起動
- インストールパスで自動判定

### システムトレイ起動
```cmd
BrowserChooser3.exe --system-tray
BrowserChooser3.exe -s
```
- システムトレイに直接起動
- メインウィンドウを表示せずにバックグラウンドで動作

### 最小化起動
```cmd
BrowserChooser3.exe --minimized
BrowserChooser3.exe -m
```
- 最小化された状態で起動
- タスクバーに最小化アイコンが表示

## 🔧 高度なオプション

### 強制ブラウザ指定
```cmd
BrowserChooser3.exe --browser "Chrome" https://example.com
BrowserChooser3.exe -b "Firefox" https://example.com
```
- 特定のブラウザを強制的に使用
- ブラウザ選択画面をスキップ

### プロファイル指定
```cmd
BrowserChooser3.exe --profile "Work" https://example.com
BrowserChooser3.exe --profile "Personal" https://example.com
```
- 特定のプロファイルを使用
- ブラウザのプロファイルを指定

### プライベートモード
```cmd
BrowserChooser3.exe --incognito https://example.com
BrowserChooser3.exe --private https://example.com
```
- プライベートモードでブラウザを起動
- 履歴やキャッシュを残さない

### 新しいウィンドウ
```cmd
BrowserChooser3.exe --new-window https://example.com
BrowserChooser3.exe -n https://example.com
```
- 新しいウィンドウでブラウザを起動
- 既存のウィンドウは使用しない

### 新しいタブ
```cmd
BrowserChooser3.exe --new-tab https://example.com
BrowserChooser3.exe -t https://example.com
```
- 新しいタブでブラウザを起動
- 既存のウィンドウの新しいタブを使用

## 📝 オプションの組み合わせ

### 複数オプションの使用
```cmd
BrowserChooser3.exe --log --debug --system-tray https://example.com
```

### オプションの優先順位
1. コマンドラインオプション（最高優先度）
2. 設定ファイルの設定
3. デフォルト設定（最低優先度）

### 競合するオプション
- `--system-tray`と`--minimized`は同時に使用できません
- 後から指定されたオプションが優先されます

## 🎯 使用例

### 基本的な使用例
```cmd
# 単一URLを開く
BrowserChooser3.exe https://www.google.com

# 複数URLを同時に開く
BrowserChooser3.exe https://google.com https://github.com https://stackoverflow.com

# 設定画面を開く
BrowserChooser3.exe
```

### 開発・デバッグ例
```cmd
# デバッグモードで起動
BrowserChooser3.exe --debug --log

# カスタム設定ファイルで起動
BrowserChooser3.exe --config "C:\Dev\BrowserChooser3\test-config.xml"

# ポータブルモードでデバッグ
BrowserChooser3.exe --portable --debug --log
```

### システム管理例
```cmd
# システムトレイに常駐
BrowserChooser3.exe --system-tray

# 最小化で起動
BrowserChooser3.exe --minimized

# 自動起動用の設定
BrowserChooser3.exe --system-tray --portable
```

### ブラウザ指定例
```cmd
# Chromeで強制起動
BrowserChooser3.exe --browser "Chrome" https://example.com

# Firefoxでプライベートモード
BrowserChooser3.exe --browser "Firefox" --incognito https://example.com

# Edgeで新しいウィンドウ
BrowserChooser3.exe --browser "Edge" --new-window https://example.com
```

## 🔍 オプションの詳細

### ログオプション
```cmd
# 基本ログ
BrowserChooser3.exe --log

# デバッグログ
BrowserChooser3.exe --log --debug

# ログファイルの場所
# Logs/BrowserChooser3.log
```

### 設定ファイルオプション
```cmd
# カスタム設定ファイル
BrowserChooser3.exe --config "C:\Custom\Config.xml"

# 設定ファイルの形式
# XML形式で保存
```

### ポータブルモードオプション
```cmd
# ポータブルモード
BrowserChooser3.exe --portable

# 判定基準
# インストールパスで自動判定
```

## 🚨 エラーハンドリング

### 無効なオプション
```cmd
# 無効なオプションを指定した場合
BrowserChooser3.exe --invalid-option
# エラーメッセージが表示され、ヘルプが表示されます
```

### 無効なURL
```cmd
# 無効なURLを指定した場合
BrowserChooser3.exe invalid-url
# エラーメッセージが表示され、処理が停止します
```

### ファイルが見つからない
```cmd
# 設定ファイルが見つからない場合
BrowserChooser3.exe --config "C:\NonExistent\Config.xml"
# デフォルト設定で起動します
```

## 💡 ベストプラクティス

### オプションの使用
- 必要最小限のオプションを使用
- 長期的な設定は設定ファイルで管理
- 一時的な設定はコマンドラインオプションで指定

### パフォーマンス
- 不要なオプションは避ける
- デバッグオプションは開発時のみ使用
- ログオプションは問題解決時のみ使用

### セキュリティ
- プライベートモードを適切に使用
- プロファイルを分けて使用
- 機密情報を含むURLは注意して使用

## 📚 関連情報

### 設定ファイル
- 設定ファイルの詳細: [設定ガイド](Configuration-Guide)
- 設定ファイルの形式: XML

### ログ機能
- ログの詳細: [トラブルシューティング](Troubleshooting)
- ログファイルの場所: `Logs/BrowserChooser3.log`

### プロトコル管理
- プロトコルの詳細: [プロトコル管理](Protocol-Management)
- カスタムプロトコルの設定

---

*より詳細な設定については、[設定ガイド](Configuration-Guide)をご覧ください。*
