# インストールガイド

このガイドは、WindowsシステムにBrowserChooser3をインストールしてセットアップするのに役立ちます。

## 📋 システム要件

### 最小要件
- **オペレーティングシステム**: Windows 10（バージョン1903以降）またはWindows 11
- **アーキテクチャ**: x64（64ビット）
- **.NETランタイム**: .NET 8.0 Runtime
- **RAM**: 100 MBの利用可能メモリ
- **ストレージ**: 50 MBの空きディスク容量

### 推奨要件
- **オペレーティングシステム**: Windows 11（最新バージョン）
- **RAM**: 200 MBの利用可能メモリ
- **ストレージ**: 100 MBの空きディスク容量

## 🔽 ダウンロードオプション

### オプション1: 事前ビルド済み実行ファイル（推奨）
1. [リリースページ](https://github.com/Yosuke-Sh/BrowserChooser3/releases)にアクセス
2. 最新の`BrowserChooser3-vX.X.X.zip`ファイルをダウンロード
3. 希望する場所に保存（例：`C:\Programs\BrowserChooser3\`）

### オプション2: ソースからビルド
ソースからビルドしたい場合や最新の開発版が必要な場合：

```bash
git clone https://github.com/Yosuke-Sh/BrowserChooser3.git
cd BrowserChooser3
dotnet build --configuration Release
```

## 🛠️ インストール手順

### ステップ1: .NET 8.0 Runtimeのインストール
BrowserChooser3の実行には.NET 8.0 Runtimeが必要です。

1. [Microsoft .NET ダウンロード](https://dotnet.microsoft.com/download/dotnet/8.0)にアクセス
2. Windows x64用の**.NET 8.0 Runtime**（SDKではない）をダウンロード
3. インストーラーを実行し、セットアップウィザードに従う
4. プロンプトが表示されたらコンピューターを再起動

### ステップ2: BrowserChooser3のダウンロード
1. リリースページから`BrowserChooser3-vX.X.X.zip`をダウンロード
2. BrowserChooser3用のフォルダを作成（例：`C:\Programs\BrowserChooser3\`）
3. ZIPファイルを展開して実行ファイルをこのフォルダに配置

### ステップ3: 初回実行セットアップ
1. `BrowserChooser3.exe`をダブルクリックして実行
2. アプリケーションが設定ファイル（`BrowserChooser3Config.xml`）を作成
3. `O`キーを押してオプションダイアログを開く
4. 必要に応じてブラウザと設定を構成

## 🔧 インストール方法

### ポータブルインストール（推奨）
BrowserChooser3はポータブル設計で、インストールは不要です。

**利点:**
- レジストリの変更なし
- コンピューター間での移動が簡単
- アンインストールプロセス不要
- USBドライブから実行可能

**手順:**
1. フォルダを作成: `C:\Programs\BrowserChooser3\`
2. `BrowserChooser3.exe`をダウンロードしてこのフォルダに配置
3. 実行ファイルを実行

### システム全体インストール
システム全体でアクセスするには、BrowserChooser3をシステムPATHに追加できます。

**手順:**
1. `BrowserChooser3.exe`を永続的な場所に配置
2. フォルダをシステムPATHに追加:
   - システムのプロパティ → 詳細設定 → 環境変数を開く
   - BrowserChooser3フォルダをPATH変数に追加
3. コマンドプロンプトまたはターミナルを再起動

### 自動起動設定
BrowserChooser3を自動起動させるには：

**方法1: スタートアップフォルダ**
1. `Win + R`を押し、`shell:startup`と入力してEnter
2. スタートアップフォルダに`BrowserChooser3.exe`へのショートカットを作成

**方法2: タスクスケジューラー**
1. タスクスケジューラーを開く
2. 新しいタスクを作成
3. アクションを`BrowserChooser3.exe`の開始に設定
4. トリガーを設定（例：ログオン時）

## 🧪 検証

インストール後、BrowserChooser3が正しく動作することを確認してください：

### テスト1: 基本機能
```bash
# コマンドプロンプトを開き、BrowserChooser3フォルダに移動
cd C:\Programs\BrowserChooser3
BrowserChooser3.exe https://www.google.com
```

### テスト2: オプションダイアログ
1. BrowserChooser3を実行
2. `O`キーを押してオプションを開く
3. すべてのタブがアクセス可能であることを確認
4. ブラウザ検出が動作していることを確認

### テスト3: 透明化機能
1. オプションダイアログを開く
2. Displayタブに移動
3. 透明化を有効にする
4. 透明度を調整し、角の丸みをテスト

## 🚨 インストールのトラブルシューティング

### よくある問題

**問題: "アプリケーションの開始に失敗しました"**
- **解決策**: .NET 8.0 Runtimeをインストール
- **確認**: `dotnet --version`で.NETインストールを確認

**問題: "ファイルが見つかりません"エラー**
- **解決策**: すべてのファイルが同じフォルダにあることを確認
- **確認**: ファイルの権限を確認

**問題: BrowserChooser3が応答しない**
- **解決策**: 管理者として実行
- **確認**: Windows Defenderまたはアンチウイルス設定を確認

**問題: 透明化が動作しない**
- **解決策**: グラフィックドライバーを更新
- **確認**: Windows透明化効果が有効であることを確認

### トラブルシューティング用ログ
問題が発生した場合、ログを有効にしてください：

```bash
BrowserChooser3.exe --log
```

詳細なエラー情報については`Logs/`フォルダを確認してください。

## 🔄 更新

### 自動更新
BrowserChooser3は自動的に更新をチェックできます：
1. オプションダイアログを開く（`O`キー）
2. "Other"タブに移動
3. "Automatic Updates"を有効にする

### 手動更新
1. 最新リリースをダウンロード
2. 古い`BrowserChooser3.exe`を新しいものに置き換え
3. 設定ファイルは保持されます

## 🗑️ アンインストール

BrowserChooser3はポータブルなので、アンインストールは簡単です：

1. **BrowserChooser3を含むフォルダを削除**
2. **作成したショートカットを削除**
3. **システム全体に追加した場合はPATHから削除**
4. **自動起動を設定した場合は削除**

**注意**: 設定ファイル（`BrowserChooser3Config.xml`）はアプリケーションフォルダと一緒に削除されます。設定を保持したい場合は、削除前にこのファイルをバックアップしてください。

## 📞 サポート

インストールで問題が発生した場合：

1. **FAQを確認**して一般的な解決策を探す
2. **GitHubで既存の問題を検索**
3. **詳細な情報で新しい問題を作成**:
   - Windowsバージョン
   - .NETバージョン
   - エラーメッセージ
   - 再現手順

---

*より詳細な情報については、[設定ガイド](Configuration-Guide)と[トラブルシューティング](Troubleshooting)ページをご覧ください。*

