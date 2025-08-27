# GitHub Releases 配布ガイド

## 📋 リリース手順

### 1. リリースパッケージの作成

```powershell
# PowerShellで実行
.\create-release.ps1 -Version "1.0.0"
```

または

```cmd
# バッチファイルで実行
create-release.bat
```

### 2. GitHub Releases の作成

1. **GitHubリポジトリにアクセス**
   - https://github.com/Yosuke-Sh/BrowserChooser3

2. **Releasesページに移動**
   - リポジトリページの右側「Releases」をクリック
   - 「Create a new release」をクリック

3. **タグの作成**
   - Tag version: `v1.0.0`
   - Target: `main` ブランチ（または適切なブランチ）

4. **リリース情報の入力**
   - Release title: `BrowserChooser3 v1.0.0`
   - Description: `RELEASE_NOTES_TEMPLATE.md`の内容をコピー&ペースト

5. **ファイルのアップロード**
   - `Release/BrowserChooser3-v1.0.0.zip`をドラッグ&ドロップ
   - ファイルサイズ: 約1.26MB

6. **リリースの公開**
   - 「Publish release」をクリック

## 📦 配布パッケージの内容

```
BrowserChooser3-v1.0.0/
├── BrowserChooser3.exe          # メインアプリケーション
├── BrowserChooser3.dll          # アプリケーションライブラリ
├── BrowserChooser3.runtimeconfig.json  # ランタイム設定
├── BrowserChooser3.deps.json    # 依存関係情報
├── BrowserChooser3.dll.config   # アプリケーション設定
├── BrowserChooser3.xml          # ドキュメント
└── INSTALL.txt                  # インストール手順
```

## 🔧 自動化のヒント

### バージョン管理
- プロジェクトファイル（`.csproj`）のバージョンを更新
- リリースノートの日付を更新
- タグ名とバージョンを一致させる

### 品質チェック
- リリース前にアプリケーションの動作確認
- 異なるWindows環境でのテスト
- .NET 8.0 Runtimeの依存関係確認

## 📊 配布統計

- **ファイルサイズ**: 約1.26MB（ZIP圧縮後）
- **展開後サイズ**: 約2.6MB
- **依存関係**: .NET 8.0 Runtime（別途インストール必要）
- **対応OS**: Windows 10/11 x64

## 🚀 配布後の作業

1. **READMEの更新**
   - 最新バージョンへのリンク確認
   - インストール手順の検証

2. **ドキュメントの更新**
   - 機能説明の更新
   - スクリーンショットの更新

3. **コミュニティへの告知**
   - リリースノートの共有
   - 新機能の紹介

## 🔍 トラブルシューティング

### よくある問題

1. **PowerShell実行ポリシーエラー**
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

2. **.NET SDKが見つからない**
   - .NET 8.0 SDKのインストール確認
   - `dotnet --version`でバージョン確認

3. **ZIPファイルが作成されない**
   - 出力ディレクトリの権限確認
   - ディスク容量の確認

## 📈 今後の改善案

- **自動化**: GitHub Actionsでの自動リリース
- **署名**: コード署名の追加
- **インストーラー**: MSIインストーラーの作成
- **パッケージマネージャー**: Chocolatey/Scoop対応
