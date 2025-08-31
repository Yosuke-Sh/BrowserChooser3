# GitHub Releases 配布ガイド

## 📋 リリース手順

### 1. リリースパッケージの作成

#### インストーラー版
```cmd
# Inno Setup 6が必要
.\build-inno-setup.bat
```

#### ポータブル版
```powershell
# PowerShellで実行
.\create-portable-release.ps1 -Version "0.1.0"
```

### 2. GitHub Releases の作成

1. **GitHubリポジトリにアクセス**
   - https://github.com/Yosuke-Sh/BrowserChooser3

2. **Releasesページに移動**
   - リポジトリページの右側「Releases」をクリック
   - 「Create a new release」をクリック

3. **タグの作成**
   - Tag version: `v0.1.0`
   - Target: `developer` ブランチ（または適切なブランチ）

4. **リリース情報の入力**
   - Release title: `BrowserChooser3 v0.1.0`
   - Description: `RELEASE_NOTES_TEMPLATE.md`の内容をコピー&ペースト

5. **ファイルのアップロード**
   - `dist/BrowserChooser3-Setup.exe`（インストーラー）をドラッグ&ドロップ
   - または `Release/BrowserChooser3-v0.1.0.zip`（ポータブル版）をドラッグ&ドロップ
   - ファイルサイズ: 約1.5MB（インストーラー）

6. **リリースの公開**
   - 「Publish release」をクリック

## 📦 配布パッケージの内容

### インストーラー版
```
BrowserChooser3-Setup.exe        # Inno Setup インストーラー
```

### ポータブル版
```
BrowserChooser3-v0.1.0/
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

### インストーラー版
- **ファイルサイズ**: 約1.5MB（インストーラー）
- **インストール後サイズ**: 約3MB
- **依存関係**: .NET 8.0 Runtime（自動インストール）
- **対応OS**: Windows 10/11 x64

### ポータブル版
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
- **パッケージマネージャー**: Chocolatey/Scoop対応
- **CI/CD**: 継続的インテグレーション・デプロイメント
- **テスト自動化**: 自動テスト実行とカバレッジレポート
- **自動アップデート機能**: アプリケーション内での自動更新チェック・ダウンロード・インストール
- **多言語対応**: 日本語、英語、その他言語のサポート
