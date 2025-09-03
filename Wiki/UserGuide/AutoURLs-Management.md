# AutoURLs管理

BrowserChooser3でのAutoURLs機能について詳しく説明します。

## 🌐 AutoURLsとは

### AutoURLsの定義
AutoURLsは、特定のURLパターンにマッチした場合に、指定されたブラウザで自動的に起動する機能です。

### 機能の特徴
- **自動起動**: パターンマッチングによる自動的なブラウザ起動
- **遅延起動**: カウントダウン表示付きの遅延起動
- **自動終了**: ブラウザ起動後のアプリケーション自動終了
- **ワイルドカード対応**: `*`を使用した柔軟なパターンマッチング

## 🔧 AutoURLsの設定

### 設定方法
1. オプションダイアログを開く（`O`キー）
2. 「AutoURLs」タブを選択
3. AutoURLsの設定を行う

## ➕ AutoURLsの追加

### 手動追加の手順
1. 「AutoURLs」タブを選択
2. 「Add」ボタンをクリック
3. 以下の情報を入力：

#### 必須項目
- **URL Pattern**: URLパターン（例：`https://www.google.com/*`）
- **Browser**: 使用するブラウザを選択

#### オプション項目
- **Delay**: 遅延時間（秒）、空欄でデフォルト設定を使用
- **Active**: AutoURLsの有効/無効

### URLパターンの指定

#### 基本形式
```
URLパターン
```

#### ワイルドカードの使用
- `*`: 任意の文字列にマッチ
- `https://www.google.com/*`: GoogleドメインのすべてのURLにマッチ
- `https://example.com/path/*`: 特定パス以下のすべてのURLにマッチ

#### パターン例
```
https://www.google.com/*
https://github.com/*/issues
https://stackoverflow.com/questions/*
```

### 遅延時間の設定
- **数値**: 指定した秒数後に起動
- **空欄**: 設定画面のデフォルト遅延時間を使用
- **0**: 即座に起動

## ✏️ AutoURLsの編集

### 編集手順
1. AutoURLs一覧から編集したい項目を選択
2. 「Edit」ボタンをクリック
3. 設定を変更
4. 「OK」をクリックして保存

### 編集可能項目
- **URL Pattern**: URLパターン
- **Browser**: 使用するブラウザ
- **Delay**: 遅延時間
- **Active**: AutoURLsの有効/無効

## 🗑️ AutoURLsの削除

### 削除手順
1. AutoURLs一覧から削除したい項目を選択
2. 「Remove」ボタンをクリック
3. 確認ダイアログで「Yes」をクリック

### 削除の注意事項
- 削除したAutoURLsは完全に削除されます
- 削除後は再追加が必要です

## 🎯 AutoURLsの動作

### 動作の流れ
1. URLが入力される
2. AutoURLsパターンとのマッチングが実行される
3. マッチした場合、指定されたブラウザで起動
4. 遅延設定に応じて即座または遅延起動
5. ブラウザ起動後、アプリケーションが自動終了

### 優先順位
1. **AutoURLs**: 最優先（URLパターンマッチング）
2. **Protocol**: 2番目（プロトコルマッチング）
3. **通常処理**: 最後（デフォルトブラウザ選択）

### 遅延起動機能
- 遅延時間が設定されている場合、カウントダウン表示が表示されます
- カウントダウン中は一時停止/再開が可能です
- 遅延時間終了後にブラウザが起動します

### 自動終了機能
- AutoURLs処理が完了すると、アプリケーションが自動的に閉じます
- メイン画面の「ブラウザを選択後に自動的に閉じる」設定が反映されます

## 📋 設定例

### Google検索の自動起動
```
URL Pattern: https://www.google.com/*
Browser: Google Chrome
Delay: 2
Active: ✓
```

### GitHub Issuesの自動起動
```
URL Pattern: https://github.com/*/issues
Browser: Microsoft Edge
Delay: 0
Active: ✓
```

### 特定ドメインの即座起動
```
URL Pattern: https://example.com/*
Browser: Firefox
Delay: 0
Active: ✓
```

## 🔍 トラブルシューティング

### よくある問題

**Q: AutoURLsが動作しません**
A: 以下を確認してください：
- URLパターンが正しく設定されているか
- ブラウザが正しく選択されているか
- Activeがチェックされているか
- パターンに``プレフィックスが付いているか

**Q: 遅延起動が動作しません**
A: 以下を確認してください：
- Delayに正しい数値が設定されているか
- メイン画面のデフォルト遅延時間が設定されているか

**Q: 自動終了しません**
A: 以下を確認してください：
- メイン画面の「ブラウザを選択後に自動的に閉じる」がチェックされているか
- ログでAutoClose実行のメッセージが出力されているか

### ログの確認
問題が発生した場合は、ログレベルを`Trace`に設定して詳細なログを確認してください。

## 📚 関連項目

- [プロトコル管理](Protocol-Management.md)
- [ブラウザ管理](Browser-Management.md)
- [基本使用方法](Basic-Usage.md)
- [コマンドラインオプション](Command-Line-Options.md)

---

**AutoURLs機能** - パターンマッチングによる自動的なブラウザ起動と改善されたユーザビリティ
