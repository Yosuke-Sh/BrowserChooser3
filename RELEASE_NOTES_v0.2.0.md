# BrowserChooser3 v0.2.0 リリースノート

## ⚠️ 破壊的変更（動作要件の変更）

### .NET 10 Desktop Runtime への移行
- 実行に必要なランタイムを **.NET 8.0** から **.NET 10.0（LTS）** へ移行しました
- .NET 8 は2026年11月10日にLTSサポートが終了するため、次期LTSである.NET 10へ直接移行しています（STSの.NET 9は既にサポート終了済みのため経由していません）
- 既存ユーザーの環境に .NET 10 Desktop Runtime が未導入の場合でも、**インストーラーが自動的に検出・ダウンロード・サイレントインストール**するため、通常は追加の手動作業は不要です
  - ダウンロードに失敗した場合は手動インストール用のURLを提示し、セットアップを中断します
- 設定ファイル（`BrowserChooser3Config.xml`）の後方互換性は維持されており、既存の設定はそのまま引き継がれます

## 🚀 パフォーマンス改善

### 起動時間の最適化（ReadyToRun）
- ビルド・インストーラー生成のパイプラインを `dotnet publish` ベースへ切り替え、ReadyToRun（事前ネイティブコード生成）を適用しました
- URLクリックのたびに起動されるホットパス上のアプリのため、JITコスト削減による起動時間の短縮が期待できます

### 描画のちらつき低減
- `MainForm` / `OptionsForm` に `DoubleBuffered` を設定し、リサイズ・再描画時のちらつきを低減しました（見た目・レイアウトの変更はありません）

## 🔧 技術的改善

### WFO1000警告の解消
- カスタムコントロール（`FFButton` / `FFCheckBox`）のプロパティに `[DefaultValue]` 属性を付与し、.NET 9以降で既定エラーとなる `WFO1000` 警告を解消しました

### 依存関係の整理
- `Microsoft.Win32.Registry` / `System.Configuration.ConfigurationManager` のパッケージ参照を削除（.NET 10 の Windows Desktop フレームワーク参照に同梱されるため不要になりました）
- テスト用パッケージを最新安定版へ更新（`Microsoft.NET.Test.Sdk`、`xunit`、`xunit.runner.visualstudio`）

## 📦 配布パッケージ

### インストーラー版
- ファイル名: `BrowserChooser3-Setup.exe`
- 設定: 管理者権限でのインストール、既定のブラウザ設定
- .NET 10 Desktop Runtime が未導入の場合は自動導入

## 🚀 インストール方法

### インストーラー版
1. `BrowserChooser3-Setup.exe`をダウンロード
2. 管理者権限で実行
3. .NET 10 Desktop Runtime が未導入の場合、インストーラーが自動的にダウンロード・インストールします
4. インストールウィザードに従ってインストール
5. 設定ファイルは`%APPDATA%\BrowserChooser3`に保存
6. ログファイルは`%LOCALAPPDATA%\BrowserChooser3\Logs`に保存

## 📋 システム要件

- **OS**: Windows 10/11 x64
- **.NET**: .NET 10.0 Desktop Runtime（未導入の場合はインストーラーが自動導入）
- **メモリ**: 512MB以上
- **ディスク**: 10MB以上の空き容量

## 🔄 アップデート方法

### v0.1.xからのアップデート
- 設定ファイルは自動的に保持されます
- 実行に必要なランタイムが .NET 8.0 から .NET 10.0 Desktop Runtime へ変更されているため、インストーラー実行時に自動導入が行われます

## 📝 既知の問題

- なし

## 🙏 謝辞

このリリースに貢献してくださった開発者、テスター、ユーザーの皆様に感謝いたします。

## 📞 サポート

- **GitHub**: https://github.com/Yosuke-Sh/BrowserChooser3
- **Issues**: https://github.com/Yosuke-Sh/BrowserChooser3/issues
- **Wiki**: https://github.com/Yosuke-Sh/BrowserChooser3/wiki

---

**BrowserChooser3 v0.2.0** - .NET 10 Desktop Runtimeへの移行とReadyToRunによる起動高速化

---
